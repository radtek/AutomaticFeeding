﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.LineControl.Edgebanding
//   项目名称：NJIS.FPZWS.LineControl.Edgebanding.Plc
//   文 件 名：PositionInputEntity.cs
//   创建时间：2018-12-13 16:58
//   作    者：
//   说    明：
//   修改时间：2018-12-13 16:58
//   修 改 人：
//  Copyright © 2017 广州宁基智能系统有限公司. 版权所有
//  *************************************************************************************

namespace NJIS.FPZWS.LineControl.Edgebanding.Plc.Control.Entitys
{
    public class PositionInputEntity : DbProcInputEntity
    {
        public string Data { get; set; }

        public int Position { get; set; }
    }
}