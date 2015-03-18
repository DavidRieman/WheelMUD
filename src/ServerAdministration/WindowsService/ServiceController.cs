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
        private bool _isRunning;

        /// <summary>
        /// Initializes a new instance of the ServiceController class.
        /// </summary>
        public ServiceController()
        {
            InitializeComponent();
            ServiceTrayIcon.BalloonTipText = "This application was minimized to tray";
            ServiceTrayIcon.BalloonTipTitle = "WheelMUD Windows Service";

            // Display the Notify Baloon for 1 second.
            ServiceTrayIcon.ShowBalloonTip(1000);

            // Set the WindowState in Minimized Mode.
            WindowState = FormWindowState.Minimized;
        }

        /// <summary>Gets or sets the WheelMUD service.</summary>
        public WheelMudService Service { get; set; }

        /// <summary>
        /// The ButtonController button click handler.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event args defining the click.</param>
        private void ButtonController_Click(object sender, EventArgs e)
        {
            if (Service != null)
            {
                if (_isRunning)
                {
                    lblStatus.Text = "Not Running";
                    lblStatus.ForeColor = Color.Red;

                    Service.Stop(null);

                    _isRunning = false;

                    btnController.Text = "Start Service";
                }
                else
                {
                    // Set the WindowState in Minimized Mode.
                    WindowState = FormWindowState.Minimized;

                    lblStatus.Text = "Running";
                    lblStatus.ForeColor = Color.Green;

                    Service.Start(null);

                    _isRunning = true;

                    btnController.Text = "Stop Service";
                }
            }
        }

        private void ServiceController_ResizeEnd(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ServiceTrayIcon.Visible = true;
                ServiceTrayIcon.ShowBalloonTip(2000);
            }

            if (WindowState == FormWindowState.Normal)
            {
                ServiceTrayIcon.Visible = false;
            }
        }

        private void ServiceController_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                ShowInTaskbar = true;
                ServiceTrayIcon.Visible = true;
                Hide();
            }
        }

        private void ServiceTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            ServiceTrayIcon.Visible = false;
            ShowInTaskbar = false;
        }
    }
}