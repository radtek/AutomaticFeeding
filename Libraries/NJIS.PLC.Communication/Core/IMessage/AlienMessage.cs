﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.LineControl.Drilling
//   项目名称：NJIS.PLC.Communication
//   文 件 名：AlienMessage.cs
//   创建时间：2018-11-08 16:16
//   作    者：
//   说    明：
//   修改时间：2018-11-08 16:16
//   修 改 人：
//  Copyright © 2017 广州宁基智能系统有限公司. 版权所有
//  *************************************************************************************

namespace NJIS.PLC.Communication.Core.IMessage
{
    /// <summary>
    ///     异形消息对象，用于异形客户端的注册包接收以及验证使用
    /// </summary>
    public class AlienMessage : INetMessage
    {
        /// <summary>
        ///     本协议的消息头长度
        /// </summary>
        public int ProtocolHeadBytesLength => 5;


        /// <summary>
        ///     头子节信息
        /// </summary>
        public byte[] HeadBytes { get; set; }

        /// <summary>
        ///     内容字节信息
        /// </summary>
        public byte[] ContentBytes { get; set; }


        /// <summary>
        ///     检查接收的数据是否合法
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>是否合法</returns>
        public bool CheckHeadBytesLegal(byte[] token)
        {
            if (HeadBytes == null) return false;

            if (HeadBytes[0] == 0x48 &&
                HeadBytes[1] == 0x73 &&
                HeadBytes[2] == 0x6E)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     从头子节信息中解析出接下来需要接收的数据长度
        /// </summary>
        /// <returns>接下来的数据长度</returns>
        public int GetContentLengthByHeadBytes()
        {
            return HeadBytes[4];
        }

        /// <summary>
        ///     获取头子节里的特殊标识
        /// </summary>
        /// <returns>标识信息</returns>
        public int GetHeadBytesIdentity()
        {
            return 0;
        }


        /// <summary>
        ///     发送的字节信息
        /// </summary>
        public byte[] SendBytes { get; set; }
    }
}