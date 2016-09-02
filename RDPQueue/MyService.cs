using System;
using System.Net.Sockets;
using NewLife;
using NewLife.Agent;
using NewLife.Log;
using NewLife.Threading;

namespace RDPQueue
{
    class MyService : AgentServiceBase<MyService>
    {
        public override String Description { get { return "远程桌面排队共享系统"; } }

        public MyService()
        {
            ServiceName = this.GetType().Name;
        }

        private RDPServer Svr;

        public override void StartWork()
        {
            // 要求马上重新加载配置
            Setting.Current = null;
            var set = Setting.Current;

            // 初始化队列
            ServerItem.Init();
            if (ServerItem.Queue.Count < 1)
            {
                WriteLine("没有正确配置远程地址，停止服务");
                Stop();
                return;
            }

            Svr = new RDPServer();
            Svr.Port = set.Port;
            if (set.Debug) Svr.Log = XTrace.Log;
            Svr.Start();

            _timer = new TimerX(Check, null, 10000, 10000);
        }

        public override void StopWork()
        {
            Svr.TryDispose();
            Svr = null;
        }

        #region 检查
        private TimerX _timer;

        /// <summary>检查服务器是否可用</summary>
        /// <returns></returns>
        public void Check(Object state)
        {
            foreach (var item in ServerItem.Queue)
            {
                if (item.Status == ServerStatus.Busy) continue;

                try
                {
                    var client = new TcpClient();
                    client.Connect(item.EndPoint);

                    try
                    {
                        client.Close();
                        client.Client.Shutdown(SocketShutdown.Both);
                    }
                    catch { }

                    if (item.Status != ServerStatus.Busy) item.Status = ServerStatus.Free;
                }
                catch
                {
                    item.Status = ServerStatus.Disable;
                }
            }
        }
        #endregion
    }
}