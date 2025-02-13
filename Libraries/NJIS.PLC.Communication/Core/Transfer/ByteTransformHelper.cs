﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.LineControl.Drilling
//   项目名称：NJIS.PLC.Communication
//   文 件 名：ByteTransformHelper.cs
//   创建时间：2018-11-08 16:16
//   作    者：
//   说    明：
//   修改时间：2018-11-08 16:16
//   修 改 人：
//  Copyright © 2017 广州宁基智能系统有限公司. 版权所有
//  *************************************************************************************

using System;
using System.Text;
using NJIS.PLC.Communication.BasicFramework;
using NJIS.PLC.Communication.Core.Types;

namespace NJIS.PLC.Communication.Core.Transfer
{
    /// <summary>
    ///     所有数据转换类的静态辅助方法
    ///     Static helper method for all data conversion classes
    /// </summary>
    public static class ByteTransformHelper
    {
        /// <summary>
        ///     结果转换操作的基础方法，需要支持类型，及转换的委托
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="result">源</param>
        /// <param name="translator">实际转换的委托</param>
        /// <returns>转换结果</returns>
        public static OperateResult<TResult> GetResultFromBytes<TResult>(OperateResult<byte[]> result,
            Func<byte[], TResult> translator)
        {
            try
            {
                if (result.IsSuccess)
                {
                    return OperateResult.CreateSuccessResult(translator(result.Content));
                }

                return OperateResult.CreateFailedResult<TResult>(result);
            }
            catch (Exception ex)
            {
                return new OperateResult<TResult>
                {
                    Message = StringResources.Language.DataTransformError + SoftBasic.ByteToHexString(result.Content) +
                              " : " + ex.Message
                };
            }
        }


        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<bool> GetBoolResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransBool(m, 0));
        }

        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<byte> GetByteResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransByte(m, 0));
        }

        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<byte[]> GetBytesResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransByte(m, 0, m.Length));
        }

        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<short> GetInt16ResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransInt16(m, 0));
        }


        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<ushort> GetUInt16ResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransUInt16(m, 0));
        }

        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<int> GetInt32ResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransInt32(m, 0));
        }

        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<uint> GetUInt32ResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransUInt32(m, 0));
        }

        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<long> GetInt64ResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransInt64(m, 0));
        }

        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<ulong> GetUInt64ResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransUInt64(m, 0));
        }

        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<float> GetSingleResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransSingle(m, 0));
        }

        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<double> GetDoubleResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransDouble(m, 0));
        }

        /// <summary>
        ///     将指定的OperateResult类型转化
        /// </summary>
        /// <param name="result">原始的类型</param>
        /// <param name="byteTransform">数据转换方法</param>
        /// <returns>转化后的类型</returns>
        public static OperateResult<string> GetStringResultFromBytes(OperateResult<byte[]> result,
            IByteTransform byteTransform)
        {
            return GetResultFromBytes(result, m => byteTransform.TransString(m, m.Length, Encoding.ASCII));
        }

        /// <summary>
        ///     结果转换操作的基础方法，需要支持类型，及转换的委托
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="result">源结果</param>
        /// <returns>转换结果</returns>
        public static OperateResult<TResult> GetResultFromArray<TResult>(OperateResult<TResult[]> result)
        {
            if (!result.IsSuccess) return OperateResult.CreateFailedResult<TResult>(result);

            return OperateResult.CreateSuccessResult(result.Content[0]);
        }
    }
}