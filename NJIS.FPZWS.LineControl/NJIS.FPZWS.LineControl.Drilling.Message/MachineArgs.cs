﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.LineControl.Drilling
//   项目名称：NJIS.FPZWS.LineControl.Drilling.Message
//   文 件 名：MsgArgs.cs
//   创建时间：2018-11-29 16:00
//   作    者：
//   说    明：
//   修改时间：2018-11-29 16:00
//   修 改 人：
//  Copyright © 2017 广州宁基智能系统有限公司. 版权所有
//  *************************************************************************************

using System;

namespace NJIS.FPZWS.LineControl.Drilling.Message
{
    [Serializable]
    public class MachineArgs : MqttMessageArgsBase
    {
        public MachineArgs()
        {
            CreatedTime = DateTime.Now;
        }

        public string SN { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public int IsProcessSingle { get; set; }
        public int IsProcessDouble { get; set; }           
    }
}