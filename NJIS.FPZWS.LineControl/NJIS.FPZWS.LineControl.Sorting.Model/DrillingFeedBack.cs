﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.LineControl.Sorting
//   项目名称：NJIS.FPZWS.LineControl.Sorting.Model
//   文 件 名：DrillingFeedBack.cs
//   创建时间：2018-12-26 8:26
//   作    者：
//   说    明：
//   修改时间：2018-12-26 8:26
//   修 改 人：
//  Copyright © 2017 广州宁基智能系统有限公司. 版权所有
//  *************************************************************************************

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NJIS.Model.Attributes;

namespace NJIS.FPZWS.LineControl.Sorting.Model
{
    [Table("DrillingFeedBack")]
    public class DrillingFeedBack
    {
        [Key] [Identity] public long LineID { get; set; }

        public string PartID { get; set; }
        public long UPID { get; set; }
        public string WorkshopCode { get; set; }
        public string ProductionLine { get; set; }
        public DateTime ProductionDate { get; set; }
        public string DrillingMachine { get; set; }
        public DateTime DrillingStartTime { get; set; }
        public DateTime DrillingFinishTime { get; set; }
        public int DrillingStatus { get; set; }
        public string StatusInformation { get; set; }
        public int FeedbackStatus { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}