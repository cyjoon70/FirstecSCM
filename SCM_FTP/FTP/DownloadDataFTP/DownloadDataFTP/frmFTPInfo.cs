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
    public partial class frmFTPInfo : Form
    {
        public frmFTPInfo()
        {
            InitializeComponent();
        }


        private void frmFTPInfo_Load(object sender, EventArgs e)
        {
            txtFTPAddress.Text = Base.strFTPAddress.Replace("ftp://", "");
            txtUsername.Text = Base.strUsername;
            txtPassword.Text = Base.strPassword;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

            SetIniValue("FTP", "FTPServer", Base.Encode(txtFTPAddress.Text), Base.AppFolder + "\\E2MAX_SCM_FTP.ini");
            SetIniValue("FTP", "FTPUser", Base.Encode(txtUsername.Text), Base.AppFolder + "\\E2MAX_SCM_FTP.ini");
            SetIniValue("FTP", "FTPPass", Base.Encode(txtPassword.Text), Base.AppFolder + "\\E2MAX_SCM_FTP.ini");

            MessageBox.Show(this, "FTP 환경이 정상적으로 저장되었습니다. 다시 실행하여 주십시요", "E2Max", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ftpForm myForm = new ftpForm();
            myForm = new ftpForm();

            this.Close();
            
        }

        // INI 값 설정 
        public void SetIniValue(String Section, String Key, String Value, String iniPath)
        {
            Base.WritePrivateProfileString(Section, Key, Value, iniPath);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
