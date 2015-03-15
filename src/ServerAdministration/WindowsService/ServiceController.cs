//-----------------------------------------------------------------------------
// <copyright file="ServiceController.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The service controller form.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Administration.WindowsService
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The service controller form.
    /// </summary>
    public partial class ServiceController : Form
    {
        /// <summary>Whether the service is running or not.</summary>
        private bool isRunning;

        /// <summary>
        /// Initializes a new instance of the ServiceController class.
        /// </summary>
        public ServiceController()
        {
            this.InitializeComponent();
            this.ServiceTrayIcon.BalloonTipText = "This application was minimized to tray";
            this.ServiceTrayIcon.BalloonTipTitle = "WheelMUD Windows Service";

            // Display the Notify Baloon for 1 second.
            this.ServiceTrayIcon.ShowBalloonTip(1000);

            // Set the WindowState in Minimized Mode.
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>Gets or sets the WheelMUD service.</summary>
        public WheelMUDService Service { get; set; }

        /// <summary>
        /// The ButtonController button click handler.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event args defining the click.</param>
        private void ButtonController_Click(object sender, EventArgs e)
        {
            if (this.Service != null)
            {
                if (this.isRunning)
                {
                    this.lblStatus.Text = "Not Running";
                    this.lblStatus.ForeColor = Color.Red;

                    this.Service.StopService();

                    this.isRunning = false;

                    this.btnController.Text = "Start Service";
                }
                else
                {
                    // Set the WindowState in Minimized Mode.
                    this.WindowState = FormWindowState.Minimized;

                    this.lblStatus.Text = "Running";
                    this.lblStatus.ForeColor = Color.Green;

                    this.Service.StartService();

                    this.isRunning = true;

                    this.btnController.Text = "Stop Service";
                }
            }
        }

        private void ServiceController_ResizeEnd(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ServiceTrayIcon.Visible = true;
                this.ServiceTrayIcon.ShowBalloonTip(2000);
            }

            if (this.WindowState == FormWindowState.Normal)
            {
                this.ServiceTrayIcon.Visible = false;
            }
        }

        private void ServiceController_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.ShowInTaskbar = true;
                this.ServiceTrayIcon.Visible = true;
                this.Hide();
            }
        }

        private void ServiceTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ServiceTrayIcon.Visible = false;
            this.ShowInTaskbar = false;
        }
    }
}