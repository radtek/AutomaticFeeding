﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.Libraries
//   项目名称：NJIS.PLC.Communication
//   文 件 名：HslTimeOut.cs
//   创建时间：2018-11-08 16:16
//   作    者：
//   说    明：
//   修改时间：2018-11-08 16:16
//   修 改 人：
//  Copyright © 2017 广州宁基智能系统有限公司. 版权所有
//  *************************************************************************************

using System;
using System.Net.Sockets;
using NJIS.PLC.Communication.Core.Thread;

namespace NJIS.PLC.Communication.Core.Types
{
    /****************************************************************************
     * 
     *    应用于一些操作超时请求的判断功能 
     * 
     *    When applied to a network connection request timeouts
     * 
     ****************************************************************************/


    /// <summary>
    ///     超时操作的类 [a class use to indicate the time-out of the connection]
    /// </summary>
    internal class HslTimeOut
    {
        /// <summary>
        ///     实例化对象
        /// </summary>
        public HslTimeOut()
        {
            StartTime = DateTime.Now;
            IsSuccessful = false;
            HybirdLock = new SimpleHybirdLock();
        }

        /// <summary>
        ///     操作的开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     操作是否成功
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        ///     延时的时间，单位毫秒
        /// </summary>
        public int DelayTime { get; set; }

        /// <summary>
        ///     连接超时用的Socket
        /// </summary>
        public Socket WorkSocket { get; set; }

        /// <summary>
        ///     用于超时执行的方法
        /// </summary>
        public Action Operator { get; set; }

        /// <summary>
        ///     当前对象判断的同步锁
        /// </summary>
        public SimpleHybirdLock HybirdLock { get; set; }
    }
}