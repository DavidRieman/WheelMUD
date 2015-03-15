//-----------------------------------------------------------------------------
// <copyright file="ServiceController.designer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The Service Controller class (continued for design aspects).
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Administration.WindowsService
{
    /// <summary>
    /// The Service Controller class (continued for design aspects).
    /// </summary>
    public partial class ServiceController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>The group box for server status and controls.</summary>
        private System.Windows.Forms.GroupBox groupBox1;

        /// <summary>The status label.</summary>
        private System.Windows.Forms.Label lblStatus;

        /// <summary>The start/stop service button.</summary>
        private System.Windows.Forms.Button btnController;

        private System.Windows.Forms.NotifyIcon ServiceTrayIcon { get; set; }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceController));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnController = new System.Windows.Forms.Button();
            this.ServiceTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Controls.Add(this.btnController);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Blue;
            this.groupBox1.Location = new System.Drawing.Point(12, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 105);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Service Status";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(168, 45);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(127, 24);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Not Running";
            // 
            // btnController
            // 
            this.btnController.ForeColor = System.Drawing.Color.Black;
            this.btnController.Location = new System.Drawing.Point(19, 36);
            this.btnController.Name = "btnController";
            this.btnController.Size = new System.Drawing.Size(117, 47);
            this.btnController.TabIndex = 0;
            this.btnController.Text = "Start Service";
            this.btnController.UseVisualStyleBackColor = true;
            this.btnController.Click += new System.EventHandler(this.ButtonController_Click);
            // 
            // ServiceTrayIcon
            // 
            this.ServiceTrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.ServiceTrayIcon.BalloonTipTitle = "WheelMUD Windows Service";
            this.ServiceTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("ServiceTrayIcon.Icon")));
            this.ServiceTrayIcon.Text = "WheelMUD Windows Service";
            this.ServiceTrayIcon.Visible = true;
            this.ServiceTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ServiceTrayIcon_MouseDoubleClick);
            // 
            // ServiceController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 153);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ServiceController";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WheelMUD Service";
            this.Resize += new System.EventHandler(this.ServiceController_Resize);
            this.ResizeEnd += new System.EventHandler(this.ServiceController_ResizeEnd);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}