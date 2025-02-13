﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.LineControl.Drilling
//   项目名称：NJIS.PLC.Communication
//   文 件 名：S7AddressData.cs
//   创建时间：2019-08-19 12:34
//   作    者：
//   说    明：
//   修改时间：2019-08-19 12:34
//   修 改 人：
//  Copyright © 2017 广州宁基智能系统有限公司. 版权所有
//  *************************************************************************************

using System;
using NJIS.PLC.Communication.Core.Types;

namespace NJIS.PLC.Communication.Core.Address
{
    /// <summary>
    ///     西门子的地址数据信息，当处于写入时，Length无效
    /// </summary>
    public class S7AddressData : DeviceAddressDataBase
    {
        /// <summary>
        ///     需要读取的数据的代码
        /// </summary>
        public byte DataCode { get; set; }

        /// <summary>
        ///     PLC的DB块数据信息
        /// </summary>
        public ushort DbBlock { get; set; }

        /// <summary>
        ///     从指定的地址信息解析成真正的设备地址信息
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        public override void Parse(string address, ushort length)
        {
            var addressData = ParseFrom(address, length);
            if (addressData.IsSuccess)
            {
                AddressStart = addressData.Content.AddressStart;
                Length = addressData.Content.Length;
                DataCode = addressData.Content.DataCode;
                DbBlock = addressData.Content.DbBlock;
            }
        }

        #region Static Method

        /// <summary>
        ///     计算特殊的地址信息 -> Calculate Special Address information
        /// </summary>
        /// <param name="address">字符串地址 -> String address</param>
        /// <returns>实际值 -> Actual value</returns>
        public static int CalculateAddressStarted(string address)
        {
            if (address.IndexOf('.') < 0)
            {
                return Convert.ToInt32(address) * 8;
            }

            var temp = address.Split('.');
            return Convert.ToInt32(temp[0]) * 8 + Convert.ToInt32(temp[1]);
        }

        /// <summary>
        ///     从实际的西门子的地址里面
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<S7AddressData> ParseFrom(string address)
        {
            return ParseFrom(address, 0);
        }

        /// <summary>
        ///     从实际的西门子的地址里面
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<S7AddressData> ParseFrom(string address, ushort length)
        {
            var addressData = new S7AddressData();
            try
            {
                addressData.Length = length;
                addressData.DbBlock = 0;
                if (address[0] == 'I')
                {
                    addressData.DataCode = 0x81;
                    addressData.AddressStart = CalculateAddressStarted(address.Substring(1));
                }
                else if (address[0] == 'Q')
                {
                    addressData.DataCode = 0x82;
                    addressData.AddressStart = CalculateAddressStarted(address.Substring(1));
                }
                else if (address[0] == 'M')
                {
                    addressData.DataCode = 0x83;
                    addressData.AddressStart = CalculateAddressStarted(address.Substring(1));
                }
                else if (address[0] == 'D' || address.Substring(0, 2) == "DB")
                {
                    addressData.DataCode = 0x84;
                    var adds = address.Split('.');
                    if (address[1] == 'B')
                    {
                        addressData.DbBlock = Convert.ToUInt16(adds[0].Substring(2));
                    }
                    else
                    {
                        addressData.DbBlock = Convert.ToUInt16(adds[0].Substring(1));
                    }

                    addressData.AddressStart = CalculateAddressStarted(address.Substring(address.IndexOf('.') + 1));
                }
                else if (address[0] == 'T')
                {
                    addressData.DataCode = 0x1D;
                    addressData.AddressStart = CalculateAddressStarted(address.Substring(1));
                }
                else if (address[0] == 'C')
                {
                    addressData.DataCode = 0x1C;
                    addressData.AddressStart = CalculateAddressStarted(address.Substring(1));
                }
                else if (address[0] == 'V')
                {
                    addressData.DataCode = 0x84;
                    addressData.DbBlock = 1;
                    addressData.AddressStart = CalculateAddressStarted(address.Substring(1));
                }
                else
                {
                    return new OperateResult<S7AddressData>(StringResources.Language.NotSupportedDataType);
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<S7AddressData>(ex.Message);
            }

            return OperateResult.CreateSuccessResult(addressData);
        }

        #endregion
    }
}