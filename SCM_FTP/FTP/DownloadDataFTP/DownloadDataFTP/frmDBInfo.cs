using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Data.SqlClient;


namespace DownloadDataFTP
{
    public partial class frmDBInfo : Form
    {
        public frmDBInfo()
        {
            InitializeComponent();
        }

        private void frmDBInfo_Load(object sender, EventArgs e)
        {
            txtServerName.Text = Base.strDBServer;
            txtLoginID.Text = Base.strDBLoginID;
            txtPassword.Text = Base.strDBPass;
            txtDatabase.Text = Base.strDBName;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SetIniValue("DATABASE", "ServerName", Base.Encode(txtServerName.Text), Base.AppFolder + "\\E2MAX_SCM_FTP.ini");
            SetIniValue("DATABASE", "LogId", Base.Encode(txtLoginID.Text), Base.AppFolder + "\\E2MAX_SCM_FTP.ini");
            SetIniValue("DATABASE", "LogPass", Base.Encode(txtPassword.Text), Base.AppFolder + "\\E2MAX_SCM_FTP.ini");
            SetIniValue("DATABASE", "Database", Base.Encode(txtDatabase.Text), Base.AppFolder + "\\E2MAX_SCM_FTP.ini");

            MessageBox.Show(this, "데이터베이스 환경이 정상적으로 저장되었습니다. 다시 실행하여 주십시요", "E2Max", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ftpForm myForm = new ftpForm();
            myForm = new ftpForm();

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // INI 값 설정 
        public void SetIniValue(String Section, String Key, String Value, String iniPath)
        {
            Base.WritePrivateProfileString(Section, Key, Value, iniPath);
        }
    }
}
