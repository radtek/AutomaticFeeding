﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.LineControl.Drilling
//   项目名称：NJIS.PLC.Communication
//   文 件 名：NetworkDeviceBase.cs
//   创建时间：2019-04-22 19:58
//   作    者：
//   说    明：
//   修改时间：2019-04-22 19:58
//   修 改 人：
//  Copyright © 2017 广州宁基智能系统有限公司. 版权所有
//  *************************************************************************************

using System;
using System.Text;
using NJIS.PLC.Communication.BasicFramework;
using NJIS.PLC.Communication.Core.IMessage;
using NJIS.PLC.Communication.Core.Reflection;
using NJIS.PLC.Communication.Core.Transfer;
using NJIS.PLC.Communication.Core.Types;
#if !NET35
using System.Threading.Tasks;

#endif

namespace NJIS.PLC.Communication.Core.Net.NetworkBase
{
    /// <summary>
    ///     设备类的基类，提供了基础的字节读写方法
    /// </summary>
    /// <typeparam name="TNetMessage">指定了消息的解析规则</typeparam>
    /// <typeparam name="TTransform">指定了数据转换的规则</typeparam>
    /// <remarks>需要继承实现采用使用。</remarks>
    public class NetworkDeviceBase<TNetMessage, TTransform> : NetworkDoubleBase<TNetMessage, TTransform>, IReadWriteNet
        where TNetMessage : INetMessage, new() where TTransform : IByteTransform, new()
    {
        #region Protect Member

        /// <summary>
        ///     单个数据字节的长度，西门子为2，三菱，欧姆龙，modbusTcp就为1，AB PLC无效
        /// </summary>
        /// <remarks>对设备来说，一个地址的数据对应的字节数，或是1个字节或是2个字节</remarks>
        protected ushort WordLength { get; set; } = 1;

        #endregion

        #region Object Override

        /// <summary>
        ///     返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串数据</returns>
        public override string ToString()
        {
            return $"NetworkDeviceBase<{typeof(TNetMessage)}, {typeof(TTransform)}>[{IpAddress}:{Port}]";
        }

        #endregion

        #region Virtual Method

        /**************************************************************************************************
         * 
         *    说明：子类中需要重写基础的读取和写入方法，来支持不同的数据访问规则
         *    
         *    此处没有将读写位纳入进来，因为各种设备的支持不尽相同，比较麻烦
         * 
         **************************************************************************************************/


        /// <summary>
        ///     从设备读取原始数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">地址长度</param>
        /// <returns>带有成功标识的结果对象</returns>
        /// <remarks>需要在继承类中重写实现，并且实现地址解析操作</remarks>
        public virtual OperateResult<byte[]> Read(string address, ushort length)
        {
            return new OperateResult<byte[]>();
        }


        public OperateResult<T> Read<T>() where T : class, new()
        {
            return ReflectionHelper.Read<T>(this);
        }

        /// <summary>
        ///     将原始数据写入设备
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">原始数据</param>
        /// <returns>带有成功标识的结果对象</returns>
        /// <remarks>需要在继承类中重写实现，并且实现地址解析操作</remarks>
        public virtual OperateResult Write(string address, byte[] value)
        {
            return new OperateResult();
        }

        #endregion

        #region Customer Support

        /// <summary>
        ///     读取自定义类型的数据，需要规定解析规则
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的结果对象</returns>
        /// <remarks>
        ///     需要是定义一个类，选择好相对于的ByteTransform实例，才能调用该方法。
        /// </remarks>
        /// <example>
        ///     此处演示三菱的读取示例，先定义一个类，实现<see cref="IDataTransfer" />接口
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="IDataTransfer Example" title="DataMy示例" />
        ///     接下来就可以实现数据的读取了
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadCustomerExample" title="ReadCustomer示例" />
        /// </example>
        public OperateResult<T> ReadCustomer<T>(string address) where T : IDataTransfer, new()
        {
            var result = new OperateResult<T>();
            var Content = new T();
            var read = Read(address, Content.ReadCount);
            if (read.IsSuccess)
            {
                Content.ParseSource(read.Content);
                result.Content = Content;
                result.IsSuccess = true;
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }

            return result;
        }

        /// <summary>
        ///     写入自定义类型的数据到设备去，需要规定生成字节的方法
        /// </summary>
        /// <typeparam name="T">自定义类型</typeparam>
        /// <param name="address">起始地址</param>
        /// <param name="data">实例对象</param>
        /// <returns>带有成功标识的结果对象</returns>
        /// <remarks>
        ///     需要是定义一个类，选择好相对于的<see cref="IDataTransfer" />实例，才能调用该方法。
        /// </remarks>
        /// <example>
        ///     此处演示三菱的读取示例，先定义一个类，实现<see cref="IDataTransfer" />接口
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="IDataTransfer Example" title="DataMy示例" />
        ///     接下来就可以实现数据的读取了
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteCustomerExample" title="WriteCustomer示例" />
        /// </example>
        public OperateResult WriteCustomer<T>(string address, T data) where T : IDataTransfer, new()
        {
            return Write(address, data.ToSource());
        }

        public OperateResult Write<T>(T data) where T : class, new()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Read Support

        /// <summary>
        ///     读取设备的short类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt16" title="Int16类型示例" />
        /// </example>
        public virtual OperateResult<short> ReadInt16(string address)
        {
            return GetInt16ResultFromBytes(Read(address, WordLength));
        }


        /// <summary>
        ///     读取设备的short类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt16Array" title="Int16类型示例" />
        /// </example>
        public virtual OperateResult<short[]> ReadInt16(string address, ushort length)
        {
            var read = Read(address, (ushort) (length * WordLength));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<short[]>(read);
            return OperateResult.CreateSuccessResult(ByteTransform.TransInt16(read.Content, 0, length));
        }

        /// <summary>
        ///     读取设备的ushort数据类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt16" title="UInt16类型示例" />
        /// </example>
        public virtual OperateResult<ushort> ReadUInt16(string address)
        {
            return GetUInt16ResultFromBytes(Read(address, WordLength));
        }


        /// <summary>
        ///     读取设备的ushort类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt16Array" title="UInt16类型示例" />
        /// </example>
        public virtual OperateResult<ushort[]> ReadUInt16(string address, ushort length)
        {
            var read = Read(address, (ushort) (length * WordLength));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<ushort[]>(read);
            return OperateResult.CreateSuccessResult(ByteTransform.TransUInt16(read.Content, 0, length));
        }


        /// <summary>
        ///     读取设备的int类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt32" title="Int32类型示例" />
        /// </example>
        public virtual OperateResult<int> ReadInt32(string address)
        {
            return GetInt32ResultFromBytes(Read(address, (ushort) (2 * WordLength)));
        }


        /// <summary>
        ///     读取设备的int类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt32Array" title="Int32类型示例" />
        /// </example>
        public virtual OperateResult<int[]> ReadInt32(string address, ushort length)
        {
            var read = Read(address, (ushort) (length * WordLength * 2));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<int[]>(read);
            return OperateResult.CreateSuccessResult(ByteTransform.TransInt32(read.Content, 0, length));
        }


        /// <summary>
        ///     读取设备的uint类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt32" title="UInt32类型示例" />
        /// </example>
        public virtual OperateResult<uint> ReadUInt32(string address)
        {
            return GetUInt32ResultFromBytes(Read(address, (ushort) (2 * WordLength)));
        }


        /// <summary>
        ///     读取设备的uint类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt32Array" title="UInt32类型示例" />
        /// </example>
        public virtual OperateResult<uint[]> ReadUInt32(string address, ushort length)
        {
            var read = Read(address, (ushort) (length * WordLength * 2));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<uint[]>(read);
            return OperateResult.CreateSuccessResult(ByteTransform.TransUInt32(read.Content, 0, length));
        }

        /// <summary>
        ///     读取设备的float类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadFloat" title="Float类型示例" />
        /// </example>
        public virtual OperateResult<float> ReadFloat(string address)
        {
            return GetSingleResultFromBytes(Read(address, (ushort) (2 * WordLength)));
        }


        /// <summary>
        ///     读取设备的float类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadFloatArray" title="Float类型示例" />
        /// </example>
        public virtual OperateResult<float[]> ReadFloat(string address, ushort length)
        {
            var read = Read(address, (ushort) (length * WordLength * 2));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<float[]>(read);
            return OperateResult.CreateSuccessResult(ByteTransform.TransSingle(read.Content, 0, length));
        }

        /// <summary>
        ///     读取设备的long类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt64" title="Int64类型示例" />
        /// </example>
        public virtual OperateResult<long> ReadInt64(string address)
        {
            return GetInt64ResultFromBytes(Read(address, (ushort) (4 * WordLength)));
        }

        /// <summary>
        ///     读取设备的long类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt64Array" title="Int64类型示例" />
        /// </example>
        public virtual OperateResult<long[]> ReadInt64(string address, ushort length)
        {
            var read = Read(address, (ushort) (length * WordLength * 4));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<long[]>(read);
            return OperateResult.CreateSuccessResult(ByteTransform.TransInt64(read.Content, 0, length));
        }

        /// <summary>
        ///     读取设备的ulong类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt64" title="UInt64类型示例" />
        /// </example>
        public virtual OperateResult<ulong> ReadUInt64(string address)
        {
            return GetUInt64ResultFromBytes(Read(address, (ushort) (4 * WordLength)));
        }

        /// <summary>
        ///     读取设备的ulong类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt64Array" title="UInt64类型示例" />
        /// </example>
        public virtual OperateResult<ulong[]> ReadUInt64(string address, ushort length)
        {
            var read = Read(address, (ushort) (length * WordLength * 4));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<ulong[]>(read);
            return OperateResult.CreateSuccessResult(ByteTransform.TransUInt64(read.Content, 0, length));
        }


        /// <summary>
        ///     读取设备的double类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadDouble" title="Double类型示例" />
        /// </example>
        public virtual OperateResult<double> ReadDouble(string address)
        {
            return GetDoubleResultFromBytes(Read(address, (ushort) (4 * WordLength)));
        }


        /// <summary>
        ///     读取设备的double类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadDoubleArray" title="Double类型示例" />
        /// </example>
        public virtual OperateResult<double[]> ReadDouble(string address, ushort length)
        {
            var read = Read(address, (ushort) (length * WordLength * 4));
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<double[]>(read);
            return OperateResult.CreateSuccessResult(ByteTransform.TransDouble(read.Content, 0, length));
        }


        /// <summary>
        ///     读取设备的字符串数据，编码为ASCII
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">地址长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadString" title="String类型示例" />
        /// </example>
        public virtual OperateResult<string> ReadString(string address, ushort length)
        {
            return GetStringResultFromBytes(Read(address, length));
        }

        #endregion

        #region Read Write Async Support

#if !NET35

        /// <summary>
        ///     使用异步的操作从原始的设备中读取数据信息
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">地址长度</param>
        /// <returns>带有成功标识的结果对象</returns>
        public Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
        {
            return Task.Run(() => Read(address, length));
        }

        /// <summary>
        ///     异步读取设备的short类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt16Async" title="Int16类型示例" />
        /// </example>
        public Task<OperateResult<short>> ReadInt16Async(string address)
        {
            return Task.Run(() => ReadInt16(address));
        }

        /// <summary>
        ///     异步读取设备的ushort类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt16ArrayAsync" title="Int16类型示例" />
        /// </example>
        public Task<OperateResult<short[]>> ReadInt16Async(string address, ushort length)
        {
            return Task.Run(() => ReadInt16(address, length));
        }


        /// <summary>
        ///     异步读取设备的ushort数据类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt16Async" title="UInt16类型示例" />
        /// </example>
        public Task<OperateResult<ushort>> ReadUInt16Async(string address)
        {
            return Task.Run(() => ReadUInt16(address));
        }

        /// <summary>
        ///     异步读取设备的ushort类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt16ArrayAsync" title="UInt16类型示例" />
        /// </example>
        public Task<OperateResult<ushort[]>> ReadUInt16Async(string address, ushort length)
        {
            return Task.Run(() => ReadUInt16(address, length));
        }

        /// <summary>
        ///     异步读取设备的int类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt32Async" title="Int32类型示例" />
        /// </example>
        public Task<OperateResult<int>> ReadInt32Async(string address)
        {
            return Task.Run(() => ReadInt32(address));
        }

        /// <summary>
        ///     异步读取设备的int类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt32ArrayAsync" title="Int32类型示例" />
        /// </example>
        public Task<OperateResult<int[]>> ReadInt32Async(string address, ushort length)
        {
            return Task.Run(() => ReadInt32(address, length));
        }

        /// <summary>
        ///     异步读取设备的uint类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt32Async" title="UInt32类型示例" />
        /// </example>
        public Task<OperateResult<uint>> ReadUInt32Async(string address)
        {
            return Task.Run(() => ReadUInt32(address));
        }

        /// <summary>
        ///     异步读取设备的uint类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt32ArrayAsync" title="UInt32类型示例" />
        /// </example>
        public Task<OperateResult<uint[]>> ReadUInt32Async(string address, ushort length)
        {
            return Task.Run(() => ReadUInt32(address, length));
        }

        /// <summary>
        ///     异步读取设备的float类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadFloatAsync" title="Float类型示例" />
        /// </example>
        public Task<OperateResult<float>> ReadFloatAsync(string address)
        {
            return Task.Run(() => ReadFloat(address));
        }

        /// <summary>
        ///     异步读取设备的float类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadFloatArrayAsync" title="Float类型示例" />
        /// </example>
        public Task<OperateResult<float[]>> ReadFloatAsync(string address, ushort length)
        {
            return Task.Run(() => ReadFloat(address, length));
        }

        /// <summary>
        ///     异步读取设备的long类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt64Async" title="Int64类型示例" />
        /// </example>
        public Task<OperateResult<long>> ReadInt64Async(string address)
        {
            return Task.Run(() => ReadInt64(address));
        }

        /// <summary>
        ///     异步读取设备的long类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadInt64ArrayAsync" title="Int64类型示例" />
        /// </example>
        public Task<OperateResult<long[]>> ReadInt64Async(string address, ushort length)
        {
            return Task.Run(() => ReadInt64(address, length));
        }

        /// <summary>
        ///     异步读取设备的ulong类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt64Async" title="UInt64类型示例" />
        /// </example>
        public Task<OperateResult<ulong>> ReadUInt64Async(string address)
        {
            return Task.Run(() => ReadUInt64(address));
        }

        /// <summary>
        ///     异步读取设备的ulong类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadUInt64ArrayAsync" title="UInt64类型示例" />
        /// </example>
        public Task<OperateResult<ulong[]>> ReadUInt64Async(string address, ushort length)
        {
            return Task.Run(() => ReadUInt64(address, length));
        }

        /// <summary>
        ///     异步读取设备的double类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadDoubleAsync" title="Double类型示例" />
        /// </example>
        public Task<OperateResult<double>> ReadDoubleAsync(string address)
        {
            return Task.Run(() => ReadDouble(address));
        }

        /// <summary>
        ///     异步读取设备的double类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadDoubleArrayAsync" title="Double类型示例" />
        /// </example>
        public Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
        {
            return Task.Run(() => ReadDouble(address, length));
        }

        /// <summary>
        ///     异步读取设备的字符串数据，编码为ASCII
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">地址长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadStringAsync" title="String类型示例" />
        /// </example>
        public Task<OperateResult<string>> ReadStringAsync(string address, ushort length)
        {
            return Task.Run(() => ReadString(address, length));
        }


        /// <summary>
        ///     异步将原始数据写入设备
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">原始数据</param>
        /// <returns>带有成功标识的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteAsync" title="bytes类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, byte[] value)
        {
            return Task.Run(() => Write(address, value));
        }

        /// <summary>
        ///     异步向设备中写入short数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt16ArrayAsync" title="Int16类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, short[] values)
        {
            return Task.Run(() => Write(address, values));
        }

        /// <summary>
        ///     异步向设备中写入short数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt16Async" title="Int16类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, short value)
        {
            return Task.Run(() => Write(address, value));
        }

        /// <summary>
        ///     异步向设备中写入ushort数组，返回是否写入成功
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt16ArrayAsync" title="UInt16类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, ushort[] values)
        {
            return Task.Run(() => Write(address, values));
        }


        /// <summary>
        ///     异步向设备中写入ushort数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt16Async" title="UInt16类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, ushort value)
        {
            return Task.Run(() => Write(address, value));
        }

        /// <summary>
        ///     异步向设备中写入int数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt32ArrayAsync" title="Int32类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, int[] values)
        {
            return Task.Run(() => Write(address, values));
        }

        /// <summary>
        ///     异步向设备中写入int数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt32Async" title="Int32类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, int value)
        {
            return Task.Run(() => Write(address, value));
        }

        /// <summary>
        ///     异步向设备中写入uint数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt32ArrayAsync" title="UInt32类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, uint[] values)
        {
            return Task.Run(() => Write(address, values));
        }

        /// <summary>
        ///     异步向设备中写入uint数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt32Async" title="UInt32类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, uint value)
        {
            return Task.Run(() => Write(address, value));
        }

        /// <summary>
        ///     异步向设备中写入float数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>返回写入结果</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteFloatArrayAsync" title="Float类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, float[] values)
        {
            return Task.Run(() => Write(address, values));
        }

        /// <summary>
        ///     异步向设备中写入float数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>返回写入结果</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteFloatAsync" title="Float类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, float value)
        {
            return Task.Run(() => Write(address, value));
        }

        /// <summary>
        ///     异步向设备中写入long数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt64ArrayAsync" title="Int64类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, long[] values)
        {
            return Task.Run(() => Write(address, values));
        }

        /// <summary>
        ///     异步向设备中写入long数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt64Async" title="Int64类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, long value)
        {
            return Task.Run(() => Write(address, value));
        }

        /// <summary>
        ///     异步向P设备中写入ulong数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt64ArrayAsync" title="UInt64类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, ulong[] values)
        {
            return Task.Run(() => Write(address, values));
        }

        /// <summary>
        ///     异步向设备中写入ulong数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt64Async" title="UInt64类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, ulong value)
        {
            return Task.Run(() => Write(address, value));
        }

        /// <summary>
        ///     异步向设备中写入double数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteDoubleArrayAsync" title="Double类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, double[] values)
        {
            return Task.Run(() => Write(address, values));
        }

        /// <summary>
        ///     异步向设备中写入double数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteDoubleAsync" title="Double类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, double value)
        {
            return Task.Run(() => Write(address, value));
        }

        /// <summary>
        ///     异步向设备中写入字符串，编码格式为ASCII
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">字符串数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteStringAsync" title="String类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, string value)
        {
            return Task.Run(() => Write(address, value));
        }

        /// <summary>
        ///     异步向设备中写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">字符串数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteString2Async" title="String类型示例" />
        /// </example>
        public Task<OperateResult> WriteAsync(string address, string value, int length)
        {
            return Task.Run(() => Write(address, value, length));
        }

        /// <summary>
        ///     异步向设备中写入字符串，编码格式为Unicode
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">字符串数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        public Task<OperateResult> WriteUnicodeStringAsync(string address, string value)
        {
            return Task.Run(() => WriteUnicodeString(address, value));
        }

        /// <summary>
        ///     异步向设备中写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">字符串数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        public Task<OperateResult> WriteUnicodeStringAsync(string address, string value, int length)
        {
            return Task.Run(() => WriteUnicodeString(address, value, length));
        }

        /// <summary>
        ///     异步读取自定义类型的数据，需要规定解析规则
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的结果对象</returns>
        /// <remarks>
        ///     需要是定义一个类，选择好相对于的ByteTransform实例，才能调用该方法。
        /// </remarks>
        /// <example>
        ///     此处演示三菱的读取示例，先定义一个类，实现<see cref="IDataTransfer" />接口
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="IDataTransfer Example" title="DataMy示例" />
        ///     接下来就可以实现数据的读取了
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="ReadCustomerAsyncExample" title="ReadCustomerAsync示例" />
        /// </example>
        public Task<OperateResult<T>> ReadCustomerAsync<T>(string address) where T : IDataTransfer, new()
        {
            return Task.Run(() => ReadCustomer<T>(address));
        }

        /// <summary>
        ///     异步写入自定义类型的数据到设备去，需要规定生成字节的方法
        /// </summary>
        /// <typeparam name="T">自定义类型</typeparam>
        /// <param name="address">起始地址</param>
        /// <param name="data">实例对象</param>
        /// <returns>带有成功标识的结果对象</returns>
        /// <remarks>
        ///     需要是定义一个类，选择好相对于的<see cref="IDataTransfer" />实例，才能调用该方法。
        /// </remarks>
        /// <example>
        ///     此处演示三菱的读取示例，先定义一个类，实现<see cref="IDataTransfer" />接口
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="IDataTransfer Example" title="DataMy示例" />
        ///     接下来就可以实现数据的读取了
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteCustomerAsyncExample" title="WriteCustomerAsync示例" />
        /// </example>
        public Task<OperateResult> WriteCustomerAsync<T>(string address, T data) where T : IDataTransfer, new()
        {
            return Task.Run(() => WriteCustomer(address, data));
        }

#endif

        #endregion

        #region Bool Support

        // Bool类型的读写，不一定所有的设备都实现，比如西门子，就没有实现bool[]的读写，Siemens的fetch/write没有实现bool操作

        /// <summary>
        ///     批量读取底层的数据信息，需要指定地址和长度，具体的结果取决于实现
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>带有成功标识的bool[]数组</returns>
        public virtual OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            return new OperateResult<bool[]>(StringResources.Language.NotSupportedFunction);
        }

        /// <summary>
        ///     读取底层的bool数据信息，具体的结果取决于实现
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>带有成功标识的bool数组</returns>
        public virtual OperateResult<bool> ReadBool(string address)
        {
            var read = ReadBool(address, 1);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>(read);

            return OperateResult.CreateSuccessResult(read.Content[0]);
        }

        /// <summary>
        ///     写入bool数组数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        public virtual OperateResult Write(string address, bool[] value)
        {
            return new OperateResult(StringResources.Language.NotSupportedFunction);
        }

        /// <summary>
        ///     写入bool数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        public virtual OperateResult Write(string address, bool value)
        {
            return Write(address, new[] {value});
        }

        #endregion

        #region Write Int16

        /// <summary>
        ///     向设备中写入short数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt16Array" title="Int16类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, short[] values)
        {
            return Write(address, ByteTransform.TransByte(values));
        }

        /// <summary>
        ///     向设备中写入short数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt16" title="Int16类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, short value)
        {
            return Write(address, new[] {value});
        }

        #endregion

        #region Write UInt16

        /// <summary>
        ///     向设备中写入ushort数组，返回是否写入成功
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt16Array" title="UInt16类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, ushort[] values)
        {
            return Write(address, ByteTransform.TransByte(values));
        }


        /// <summary>
        ///     向设备中写入ushort数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt16" title="UInt16类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, ushort value)
        {
            return Write(address, new[] {value});
        }

        #endregion

        #region Write Int32

        /// <summary>
        ///     向设备中写入int数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt32Array" title="Int32类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, int[] values)
        {
            return Write(address, ByteTransform.TransByte(values));
        }

        /// <summary>
        ///     向设备中写入int数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt32" title="Int32类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, int value)
        {
            return Write(address, new[] {value});
        }

        #endregion

        #region Write UInt32

        /// <summary>
        ///     向设备中写入uint数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt32Array" title="UInt32类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, uint[] values)
        {
            return Write(address, ByteTransform.TransByte(values));
        }

        /// <summary>
        ///     向设备中写入uint数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt32" title="UInt32类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, uint value)
        {
            return Write(address, new[] {value});
        }

        #endregion

        #region Write Float

        /// <summary>
        ///     向设备中写入float数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>返回写入结果</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteFloatArray" title="Float类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, float[] values)
        {
            return Write(address, ByteTransform.TransByte(values));
        }

        /// <summary>
        ///     向设备中写入float数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>返回写入结果</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteFloat" title="Float类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, float value)
        {
            return Write(address, new[] {value});
        }

        #endregion

        #region Write Int64

        /// <summary>
        ///     向设备中写入long数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt64Array" title="Int64类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, long[] values)
        {
            return Write(address, ByteTransform.TransByte(values));
        }

        /// <summary>
        ///     向设备中写入long数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteInt64" title="Int64类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, long value)
        {
            return Write(address, new[] {value});
        }

        #endregion

        #region Write UInt64

        /// <summary>
        ///     向P设备中写入ulong数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt64Array" title="UInt64类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, ulong[] values)
        {
            return Write(address, ByteTransform.TransByte(values));
        }

        /// <summary>
        ///     向设备中写入ulong数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteUInt64" title="UInt64类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, ulong value)
        {
            return Write(address, new[] {value});
        }

        #endregion

        #region Write Double

        /// <summary>
        ///     向设备中写入double数组，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="values">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteDoubleArray" title="Double类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, double[] values)
        {
            return Write(address, ByteTransform.TransByte(values));
        }

        /// <summary>
        ///     向设备中写入double数据，返回是否写入成功
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">实际数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteDouble" title="Double类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, double value)
        {
            return Write(address, new[] {value});
        }

        #endregion

        #region Write String

        /// <summary>
        ///     向设备中写入字符串，编码格式为ASCII
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">字符串数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteString" title="String类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, string value)
        {
            var temp = ByteTransform.TransByte(value, Encoding.ASCII);
            if (WordLength == 1) temp = SoftBasic.ArrayExpandToLengthEven(temp);
            return Write(address, temp);
        }

        /// <summary>
        ///     向设备中写入指定长度的字符串,超出截断，不够补0，编码格式为ASCII
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">字符串数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        /// <example>
        ///     以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        ///     <code lang="cs"
        ///         source="NJIS.PLC.Communication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs"
        ///         region="WriteString2" title="String类型示例" />
        /// </example>
        public virtual OperateResult Write(string address, string value, int length)
        {
            var temp = ByteTransform.TransByte(value, Encoding.ASCII);
            if (WordLength == 1) temp = SoftBasic.ArrayExpandToLengthEven(temp);
            temp = SoftBasic.ArrayExpandToLength(temp, length);
            return Write(address, temp);
        }

        /// <summary>
        ///     向设备中写入字符串，编码格式为Unicode
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">字符串数据</param>
        /// <returns>是否写入成功的结果对象</returns>
        public virtual OperateResult WriteUnicodeString(string address, string value)
        {
            var temp = ByteTransform.TransByte(value, Encoding.Unicode);
            return Write(address, temp);
        }

        /// <summary>
        ///     向设备中写入指定长度的字符串,超出截断，不够补0，编码格式为Unicode
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="value">字符串数据</param>
        /// <param name="length">指定的字符串长度，必须大于0</param>
        /// <returns>是否写入成功的结果对象 -> Whether to write a successful result object</returns>
        public virtual OperateResult WriteUnicodeString(string address, string value, int length)
        {
            var temp = ByteTransform.TransByte(value, Encoding.Unicode);
            temp = SoftBasic.ArrayExpandToLength(temp, length * 2);
            return Write(address, temp);
        }

        #endregion
    }
}