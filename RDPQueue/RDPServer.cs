using System;
using NewLife.Net;
using NewLife.Net.Proxy;

namespace RDPQueue
{
    /// <summary>RDP服务器</summary>
    class RDPServer : ProxyBase<RDPSession>
    {
        public RDPServer()
        {
            ProtocolType = NetType.Tcp;
        }
    }

    class RDPSession : ProxySession<RDPServer, RDPSession>
    {
        public ServerItem Rdp { get; set; }

        public override void Start()
        {
            var rdp = ServerItem.Pop(Remote.Address + "");
            if (rdp == null)
            {
                WriteLog("没有取得可用服务器。");

                Dispose();

                return;
            }

            // 输出日志
            WriteLog("取得服务器 " + rdp);

            RemoteServerUri = new NetUri(NetType.Tcp, rdp.EndPoint);

            Rdp = rdp;

            base.Start();
        }

        protected override ISocketClient CreateRemote(ReceivedEventArgs e)
        {
            return base.CreateRemote(e);
        }

        protected override void OnDispose(Boolean disposing)
        {
            var rdp = Rdp;
            if (rdp == null) return;
            Rdp = null;

            // 输出日志
            WriteLog(" 归还服务器 " + rdp);

            rdp.Push();

            base.OnDispose(disposing);
        }
    }
}