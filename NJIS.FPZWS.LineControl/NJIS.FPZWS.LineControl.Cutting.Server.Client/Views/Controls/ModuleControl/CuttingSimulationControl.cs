﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NJIS.FPZWS.LineControl.Cutting.Server.Client.Views.Controls.ModuleControl
{
    public partial class CuttingSimulationControl : UserControl
    {
        public CuttingSimulationControl()
        {
            InitializeComponent();
            dtpPlanDate.Value = DateTime.Today;
        }
    }
}
