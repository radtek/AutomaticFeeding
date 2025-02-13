﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.LineControl.Drilling
//   项目名称：NJIS.PLC.Communication
//   文 件 名：S7Message.cs
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
    ///     西门子S7协议的消息解析规则
    /// </summary>
    public class S7Message : INetMessage
    {
        /// <summary>
        ///     西门子头字节的长度
        /// </summary>
        public int ProtocolHeadBytesLength => 4;


        /// <summary>
        ///     头子节的数据
        /// </summary>
        public byte[] HeadBytes { get; set; }


        /// <summary>
        ///     内容字节的数据
        /// </summary>
        public byte[] ContentBytes { get; set; }


        /// <summary>
        ///     检查头子节是否合法的判断
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>是否合法的</returns>
        public bool CheckHeadBytesLegal(byte[] token)
        {
            if (HeadBytes == null) return false;

            if (HeadBytes[0] == 0x03 && HeadBytes[1] == 0x00)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        ///     获取剩余的内容长度
        /// </summary>
        /// <returns>数据内容长度</returns>
        public int GetContentLengthByHeadBytes()
        {
            if (HeadBytes?.Length >= 4)
            {
                return HeadBytes[2] * 256 + HeadBytes[3] - 4;
            }

            return 0;
        }

        /// <summary>
        ///     获取消息号，此处无效
        /// </summary>
        /// <returns>消息标识</returns>
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