using System;
using System.Collections.Generic;
using System.Text;

namespace RDPQueue
{
    /// <summary>
    /// 服务状态
    /// </summary>
    public enum ServerStatus
    {
        /// <summary>
        /// 未知状态
        /// </summary>
        Unkown = 0,

        /// <summary>
        /// 可用
        /// </summary>
        Free = 1,

        /// <summary>
        /// 不可用
        /// </summary>
        Disable = 2,

        /// <summary>
        /// 忙
        /// </summary>
        Busy = 3
    }
}