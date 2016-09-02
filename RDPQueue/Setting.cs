using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLife.Xml;

namespace RDPQueue
{
    [XmlConfigFile("")]
    public class Setting : XmlConfig<Setting>
    {
        /// <summary>是否调试。默认true</summary>
        [Description("是否调试。默认true")]
        public Boolean Debug { get; set; } = true;

        /// <summary>本地监听端口。默认3388</summary>
        [Description("本地监听端口。默认3388")]
        public Int32 Port { get; set; } = 3388;

        /// <summary>目标服务器</summary>
        [Description("目标服务器")]
        public String[] Servers { get; set; }

        protected override void OnNew()
        {
            if (Servers == null || Servers.Length == 0) Servers = new String[] { "127.0.0.1:3389" };

            base.OnNew();
        }
    }
}