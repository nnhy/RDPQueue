using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RDPQueue
{
    /// <summary>RDP服务器</summary>
    public class ServerItem
    {
        #region 属性
        /// <summary>IP结点</summary>
        public IPEndPoint EndPoint { get; set; }

        /// <summary>状态</summary>
        public ServerStatus Status { get; set; } = ServerStatus.Free;

        /// <summary>上一个客户端。优先安排上一次使用的服务器</summary>
        public String LastClient { get; set; }

        /// <summary>上一次使用时间。优先安排最久不使用的服务器</summary>
        public DateTime LastTime { get; set; }

        /// <summary>引用计数</summary>
        public Int32 Times { get; set; }

        public override string ToString()
        {
            return EndPoint.ToString();
        }
        #endregion

        #region 静态方法
        /// <summary>队列</summary>
        public static List<ServerItem> Queue { get; } = new List<ServerItem>();

        public static void Init()
        {
            Queue.Clear();

            var set = Setting.Current;
            // 准备服务器集合
            foreach (var item in set.Servers)
            {
                var rdp = new ServerItem();
                rdp.EndPoint = NetHelper.ParseEndPoint(item, 3389);
                Queue.Add(rdp);
            }
        }
        #endregion

        #region 取还
        /// <summary>借出</summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static ServerItem Pop(String ip)
        {
            lock (typeof(ServerItem))
            {
                ServerItem rdp = null;
                // 优先安排上一次使用的服务器
                var list2 = Queue.Where(e => e.LastClient == ip).ToList();
                if (list2 != null && list2.Count > 0)
                {
                    // 最近使用的，倒序
                    rdp = list2.OrderByDescending(e => e.LastTime).FirstOrDefault();
                }
                else
                {
                    // 优先安排最久不使用的服务器，防止抢了别人不用而断线的
                    rdp = Queue.Where(e => e.Status == ServerStatus.Free).OrderBy(e => e.LastTime).FirstOrDefault();
                }
                if (rdp == null) return null;

                // 借出
                rdp.LastClient = ip;
                rdp.LastTime = DateTime.Now;
                rdp.Status = ServerStatus.Busy;
                rdp.Times++;

                return rdp;
            }
        }

        /// <summary>
        /// 归还
        /// </summary>
        public void Push()
        {
            lock (this)
            {
                LastTime = DateTime.Now;
                Times--;
                if (Times < 1) Status = ServerStatus.Free;
            }
        }
        #endregion
    }
}
