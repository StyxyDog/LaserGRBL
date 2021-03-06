﻿//Copyright (c) 2016-2020 Diego Settimi - https://github.com/arkypita/

// This program is free software; you can redistribute it and/or modify  it under the terms of the GPLv3 General Public License as published by  the Free Software Foundation; either version 3 of the License, or (at  your option) any later version.
// This program is distributed in the hope that it will be useful, but  WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GPLv3  General Public License for more details.
// You should have received a copy of the GPLv3 General Public License  along with this program; if not, write to the Free Software  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307,  USA. using System;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LaserGRBL
{
	public partial class SettingsForm : Form
	{
        private GrblCore Core;
        public static event EventHandler SettingsChanged;

		public SettingsForm(GrblCore core)
		{
			InitializeComponent();

            this.Core = core;

            BackColor = ColorScheme.FormBackColor;
            ForeColor = ColorScheme.FormForeColor;
            TpRasterImport.BackColor = TpHardware.BackColor = TpJogControl.BackColor = TpAutoCooling.BackColor  = TpGCodeSettings.BackColor = BtnCancel.BackColor = BtnSave.BackColor = ColorScheme.FormBackColor;

            InitCoreCB();
			InitProtocolCB();
			InitStreamingCB();
			InitThreadingCB();

            CBCore.SelectedItem = (Firmware)Settings.GetObject("Firmware Type", Firmware.Grbl);
			CBSupportPWM.Checked = (bool)Settings.GetObject("Support Hardware PWM", true);
			CBProtocol.SelectedItem = Settings.GetObject("ComWrapper Protocol", ComWrapper.WrapperType.UsbSerial);
			CBStreamingMode.SelectedItem = Settings.GetObject("Streaming Mode", GrblCore.StreamingMode.Buffered);
			CbUnidirectional.Checked = (bool)Settings.GetObject("Unidirectional Engraving", false);
			CbThreadingMode.SelectedItem = Settings.GetObject("Threading Mode", GrblCore.ThreadingMode.UltraFast);
			CbIssueDetector.Checked = !(bool)Settings.GetObject("Do not show Issue Detector", false);
			CbSoftReset.Checked = (bool)Settings.GetObject("Reset Grbl On Connect", true);
			CbHardReset.Checked = (bool)Settings.GetObject("HardReset Grbl On Connect", false);

            CbContinuosJog.Checked = (bool)Settings.GetObject("Enable Continuous Jog", false);
            CbEnableZJog.Checked = (bool)Settings.GetObject("Enale Z Jog Control", false);

			TBHeader.Text = (string)Settings.GetObject("GCode.CustomHeader", GrblCore.GCODE_STD_HEADER);
            TBHeader.ForeColor = ColorScheme.FormForeColor;
            TBHeader.BackColor = ColorScheme.FormBackColor;
            TBPasses.Text = (string)Settings.GetObject("GCode.CustomPasses", GrblCore.GCODE_STD_PASSES);
            TBPasses.ForeColor = ColorScheme.FormForeColor;
            TBPasses.BackColor = ColorScheme.FormBackColor;
            TBFooter.Text = (string)Settings.GetObject("GCode.CustomFooter", GrblCore.GCODE_STD_FOOTER);
            TBFooter.ForeColor = ColorScheme.FormForeColor;
            TBFooter.BackColor = ColorScheme.FormBackColor;

            groupBox1.ForeColor = groupBox2.ForeColor = groupBox3.ForeColor = ColorScheme.FormForeColor;

            InitAutoCoolingTab();
        }

		private void InitAutoCoolingTab()
		{
			CbAutoCooling.Checked = (bool)Settings.GetObject("AutoCooling", false);
			CbOffMin.Items.Clear();
			CbOffSec.Items.Clear();
			CbOnMin.Items.Clear();
			CbOnSec.Items.Clear();

			for (int i = 0; i <= 60; i++)
				CbOnMin.Items.Add(i);
			for (int i = 0; i <= 10; i++)
				CbOffMin.Items.Add(i);

			for (int i = 0; i <= 59; i++)
				CbOnSec.Items.Add(i);
			for (int i = 0; i <= 59; i++)
				CbOffSec.Items.Add(i);

			TimeSpan CoolingOn = (TimeSpan)Settings.GetObject("AutoCooling TOn", TimeSpan.FromMinutes(10));
			TimeSpan CoolingOff = (TimeSpan)Settings.GetObject("AutoCooling TOff", TimeSpan.FromMinutes(1));

			CbOnMin.SelectedItem = CoolingOn.Minutes;
			CbOffMin.SelectedItem = CoolingOff.Minutes;
			CbOnSec.SelectedItem = CoolingOn.Seconds;
			CbOffSec.SelectedItem = CoolingOff.Seconds;
		}

		private void InitCoreCB()
        {
            CBCore.BeginUpdate();
            CBCore.Items.Add(Firmware.Grbl);
            CBCore.Items.Add(Firmware.Smoothie);
            CBCore.Items.Add(Firmware.Marlin);
            CBCore.EndUpdate();
        }

        private void InitThreadingCB()
		{
			CbThreadingMode.BeginUpdate();
			CbThreadingMode.Items.Add(GrblCore.ThreadingMode.Insane);
			CbThreadingMode.Items.Add(GrblCore.ThreadingMode.UltraFast);
			CbThreadingMode.Items.Add(GrblCore.ThreadingMode.Fast);
			CbThreadingMode.Items.Add(GrblCore.ThreadingMode.Quiet);
			CbThreadingMode.Items.Add(GrblCore.ThreadingMode.Slow);
			CbThreadingMode.EndUpdate();
		}

		private void InitProtocolCB()
		{
			CBProtocol.BeginUpdate();
			CBProtocol.Items.Add(ComWrapper.WrapperType.UsbSerial);
			CBProtocol.Items.Add(ComWrapper.WrapperType.Telnet);
			CBProtocol.Items.Add(ComWrapper.WrapperType.LaserWebESP8266);
			CBProtocol.Items.Add(ComWrapper.WrapperType.Emulator);
			CBProtocol.EndUpdate();
		}

		private void InitStreamingCB()
		{
			CBStreamingMode.BeginUpdate();
			CBStreamingMode.Items.Add(GrblCore.StreamingMode.Buffered);
			CBStreamingMode.Items.Add(GrblCore.StreamingMode.Synchronous);
			CBStreamingMode.Items.Add(GrblCore.StreamingMode.RepeatOnError);
			CBStreamingMode.EndUpdate();
		}

		internal static void CreateAndShowDialog(GrblCore core)
		{
			using (SettingsForm sf = new SettingsForm(core))
				sf.ShowDialog();
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
            Settings.SetObject("Firmware Type", CBCore.SelectedItem);
            Settings.SetObject("Support Hardware PWM", CBSupportPWM.Checked);
			Settings.SetObject("ComWrapper Protocol", CBProtocol.SelectedItem);
			Settings.SetObject("Streaming Mode", CBStreamingMode.SelectedItem);
			Settings.SetObject("Unidirectional Engraving", CbUnidirectional.Checked);
			Settings.SetObject("Threading Mode", CbThreadingMode.SelectedItem);
			Settings.SetObject("Do not show Issue Detector", !CbIssueDetector.Checked);
			Settings.SetObject("Reset Grbl On Connect", CbSoftReset.Checked);
			Settings.SetObject("HardReset Grbl On Connect", CbHardReset.Checked);
            Settings.SetObject("Enable Continuous Jog", CbContinuosJog.Checked);
            Settings.SetObject("Enale Z Jog Control", CbEnableZJog.Checked);

			Settings.SetObject("AutoCooling", CbAutoCooling.Checked);
			Settings.SetObject("AutoCooling TOn", MaxTs(TimeSpan.FromSeconds(10), new TimeSpan(0, (int)CbOnMin.SelectedItem, (int)CbOnSec.SelectedItem)));
			Settings.SetObject("AutoCooling TOff", MaxTs(TimeSpan.FromSeconds(10), new TimeSpan(0, (int)CbOffMin.SelectedItem, (int)CbOffSec.SelectedItem)));

			Settings.SetObject("GCode.CustomHeader", TBHeader.Text.Trim());
			Settings.SetObject("GCode.CustomPasses", TBPasses.Text.Trim());
			Settings.SetObject("GCode.CustomFooter", TBFooter.Text.Trim());

			Settings.Save();

            SettingsChanged?.Invoke(this, null);

            Close();

            if (Core.Type != (Firmware)Settings.GetObject("Firmware Type", Firmware.Grbl) && MessageBox.Show(Strings.FirmwareRequireRestartNow, Strings.FirmwareRequireRestart, MessageBoxButtons.OKCancel) == DialogResult.OK)
                Application.Restart();
		}

		private TimeSpan MaxTs(TimeSpan a, TimeSpan b)
		{ return TimeSpan.FromTicks(Math.Max(a.Ticks, b.Ticks)); }

		private void BtnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void BtnModulationInfo_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(@"http://lasergrbl.com/configuration/#pwm-support");
		}

		private void BtnLaserMode_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(@"http://lasergrbl.com/configuration/#laser-mode");
		}

		private void BtnProtocol_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(@"http://lasergrbl.com/configuration/#protocol");
		}

		private void BtnStreamingMode_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(@"http://lasergrbl.com/configuration/#streaming-mode");
		}

		private void BtnThreadingModel_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(@"http://lasergrbl.com/configuration/#threading-mode");
		}

        private void BtnFType_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://lasergrbl.com/configuration/#firmware-type");
        }
    }
}
