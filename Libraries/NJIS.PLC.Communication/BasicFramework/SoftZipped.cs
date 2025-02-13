﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.LineControl.Drilling
//   项目名称：NJIS.PLC.Communication
//   文 件 名：SoftZipped.cs
//   创建时间：2018-11-08 16:16
//   作    者：
//   说    明：
//   修改时间：2018-11-08 16:16
//   修 改 人：
//  Copyright © 2017 广州宁基智能系统有限公司. 版权所有
//  *************************************************************************************

using System;
using System.IO;
using System.IO.Compression;

namespace NJIS.PLC.Communication.BasicFramework
{
    /// <summary>
    ///     一个负责压缩解压数据字节的类
    /// </summary>
    public class SoftZipped
    {
        // 压缩字节
        // 1.创建压缩的数据流 
        // 2.设定compressStream为存放被压缩的文件流,并设定为压缩模式
        // 3.将需要压缩的字节写到被压缩的文件流


        /// <summary>
        ///     压缩字节数据
        /// </summary>
        /// <param name="bytes">等待被压缩的数据</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>压缩之后的字节数据</returns>
        public static byte[] CompressBytes(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");

            using (var compressStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(compressStream, CompressionMode.Compress))
                    zipStream.Write(bytes, 0, bytes.Length);
                return compressStream.ToArray();
            }
        }


        // 解压缩字节
        // 1.创建被压缩的数据流
        // 2.创建zipStream对象，并传入解压的文件流
        // 3.创建目标流
        // 4.zipStream拷贝到目标流
        // 5.返回目标流输出字节

        /// <summary>
        ///     解压压缩后的数据
        /// </summary>
        /// <param name="bytes">压缩后的数据</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>压缩前的原始字节数据</returns>
        public static byte[] Decompress(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");

            using (var compressStream = new MemoryStream(bytes))
            {
                using (var zipStream = new GZipStream(compressStream, CompressionMode.Decompress))
                {
                    using (var resultStream = new MemoryStream())
                    {
                        var readLength = 1024;
                        var buf = new byte[readLength];
                        var len = 0;
                        while ((len = zipStream.Read(buf, 0, readLength)) > 0)
                        {
                            resultStream.Write(buf, 0, len);
                        }

                        return resultStream.ToArray();
                    }
                }
            }
        }
    }
}