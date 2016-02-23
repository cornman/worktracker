using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace worktracker
{
    public partial class Form1 : Form
    {
        enum workstates { start, working, finished }

        workstates state = workstates.start;

        string date;

        Session session;

        public bool initDirectory { get; set; } = false;



        public Form1()
        {
            InitializeComponent();
            date = DateTime.Today.Day + "." + DateTime.Today.Month + "." + DateTime.Today.Year;
            button1.Text = "start working...\n" + date;
            if (initDirectory)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (state)
            {
                case workstates.start:
                    session = new Session(date);
                    button1.Text = "working...";
                    Text = "working...";
                    state = workstates.working;
                    notifyIcon1.Visible = true;
                    notifyIcon1.ShowBalloonTip(3000);
                    WindowState = FormWindowState.Minimized;
                    ShowInTaskbar = false;
                    break;
                case workstates.working:
                    session.End();
                    button1.Text = "finished work. \n total time: " + session.totalTime + "\n write to file and exit?";
                    state = workstates.finished;
                    break;
                case workstates.finished:
                    session.Log();
                    Application.Exit();
                    break;
            }
        }

        public string GetFilePath()
        {
            string inipath = Directory.GetCurrentDirectory() + "\\config.ini";
            IniFile ini;
            DialogResult res;

            if (!File.Exists(inipath))
            {
                File.Create(inipath).Close();
                File.AppendAllText(inipath, "[CloudSettings]\npath=\"null\"");
                ini = new IniFile(inipath);
            }
            else
            {
                ini = new IniFile(inipath);
            }

            if (ini.ReadValue("CloudSettings", "path", "null") == "null")
            {
                openFileDialog1.Title = "Choose File Directory...";
                res = openFileDialog1.ShowDialog();
                while (res != DialogResult.OK)
                {
                    res = openFileDialog1.ShowDialog();
                }
                ini.WriteValue("CloudSettings", "path", openFileDialog1.FileName);
                return ini.ReadValue("CloudSettings", "path", "null");
            }
            else
            {
                return ini.ReadValue("CloudSettings", "path", "null");
            }

        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            session.End();
            Text = "done working.";
            button1.Text = "finished work. \n total time: " + session.totalTime + "\n write to file and exit?";
            state = workstates.finished;
        }
    }

    public class Session
    {
        public string date { get; set; }
        public DateTime startTime { get; }
        private DateTime endTime;
        public TimeSpan totalTime { get; set; }

        public Session(string _date)
        {
            date = _date;
            startTime = DateTime.Now;
        }

        public void End()
        {
            endTime = DateTime.Now;
            totalTime = endTime - startTime;
        }
        public void Log()
        {
            string filePath = (Form.ActiveForm as Form1).GetFilePath();
            if (filePath != null)
            {
                File.AppendAllText(filePath, "\n" + date + " - total: " + totalTime + " - start: " + startTime + " - end: " + endTime);
            }
        }
    }
}
