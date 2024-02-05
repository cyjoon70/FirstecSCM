using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Management;
using System.Net;
using SystemBase;
using System.Globalization;

namespace E2MAXSCM
{
    public partial class LoginForm : System.Windows.Forms.Form
    {
        protected Bitmap BackgroundBitmap = null;
        string AppFolder = "";
        string SaveId = "N";

        string strIp = "", strDbName = "", strId = "", strPwd = "";
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;

        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        //private System.ComponentModel.Container components = null;


        public LoginForm(string Ip, string DbName, string Id, string Pwd)
        {
            strIp = Ip;
            strDbName = DbName;
            strId = Id;
            strPwd = Pwd;

            InitializeComponent();

            try
            {
                AppFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            }
            catch { }
        }

        public LoginForm()
        {

            InitializeComponent();

            try
            {
                AppFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            }
            catch { }
        }

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>

        public void Login()
        {
            try
            {
                int CK_ID = txtID.Text.LastIndexOf("'");

                if (CK_ID > -1)
                {
                    MessageBox.Show(@"(')는 ID로 사용할수 없는 문자열입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    string Query = "EXEC usp_USERLOGIN @pType='S3', @pUSR_ID='" + txtID.Text + "', @pCO_CD = '" + cboCompCd.SelectedValue.ToString() + "' ";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
                    
                    if (dt.Rows.Count > 0)
                    {
                        string Encode = SystemBase.Base.DeCode(dt.Rows[0][0].ToString());

                        if (Encode == txtPW.Text)
                        {
                            ////////////////////////보안관련/////////////////////////
                            SystemBase.Base.ProgramWhere = AppFolder;			        //프로그램 위치
                            E2MAXSCM.Main.UserID = txtID.Text;					        //유저ID
                            E2MAXSCM.Main.UserName = dt.Rows[0][2].ToString();	        //유저명
                            SystemBase.Base.gstrServerNM = strIp.Trim();				//서버IP
                            SystemBase.Base.gstrUserID = txtID.Text;					//유저ID
                            SystemBase.Base.gstrUserName = dt.Rows[0][2].ToString();			//사용자명

                            SystemBase.Base.gstrCOMCD = "";			//법인코드
                            SystemBase.Base.gstrBIZCD = "";			//사업장코드
                            SystemBase.Base.gstrBIZNM = "";			//사업장명
                            SystemBase.Base.gstrPLANT_CD = "";			//공장코드
                            SystemBase.Base.gstrREORG_ID = "";			//부서개편ID
                            SystemBase.Base.gstrDEPT = "";			//부서코드
                            SystemBase.Base.gstrDEPTNM = "";			//부서명

                            SystemBase.Base.gstrCOMCD = cboCompCd.SelectedValue.ToString();			//법인코드
                            SystemBase.Base.gstrCOMNM = cboCompCd.SelectedText;			//법인명
                            //SystemBase.Base.gstrBIZCD = dt.Rows[0][4].ToString();			//사업장코드
                            //SystemBase.Base.gstrBIZNM = dt.Rows[0][5].ToString();			//사업장명
                            //SystemBase.Base.gstrPLANT_CD = dt.Rows[0][9].ToString();			//공장코드
                            //SystemBase.Base.gstrREORG_ID = dt.Rows[0][6].ToString();			//부서개편ID
                            //SystemBase.Base.gstrDEPT = dt.Rows[0][7].ToString();			//부서코드
                            //SystemBase.Base.gstrDEPTNM = dt.Rows[0][8].ToString();			//부서명

                            //SCM 관련 추가
                            SystemBase.Base.gstrCUST_TYPE = dt.Rows[0][3].ToString();   //거래처구분
                            SystemBase.Base.gstrTRADE_TYPE = dt.Rows[0][4].ToString();  //거래유형

                            if (chkSaveId.Checked == true)
                            {
                                SetIniValue("DATABASE", "SaveId", "Y", SystemBase.Base.ProgramWhere + "\\E2MAX_SCM_FTP.ini");
                                SetIniValue("DATABASE", "UserId", SystemBase.Base.gstrUserID, SystemBase.Base.ProgramWhere + "\\E2MAX_SCM_FTP.ini");
                            }
                            else
                            {
                                SetIniValue("DATABASE", "SaveId", "N", SystemBase.Base.ProgramWhere + "\\E2MAX_SCM_FTP.ini");
                                SetIniValue("DATABASE", "UserId", "", SystemBase.Base.ProgramWhere + "\\E2MAX_SCM_FTP.ini");
                            }

                            SetIniValue("DATABASE", "ComCd", SystemBase.Base.gstrCOMCD, SystemBase.Base.ProgramWhere + "\\E2MAX_SCM_FTP.ini");



                            ////////////////////////////////////////////////////////////////
                            // 데이타베이스명 임시 MTMS_FT_TEST => MTMS_FT로 강제 변경
                            ////////////////////////////////////////////////////////////////
                            //SystemBase.Base.gstrDbName = "MTMS_FT";
                            //strDbName = "MTMS_FT";
                            //SetIniValue("DATABASE", "Database", SystemBase.Base.gstrDbName, SystemBase.Base.ProgramWhere + "\\E2MAX_SCM_FTP.ini");
                            SystemBase.Base.gstrDbConn = "server=" + strIp.Trim() + ";uid=" + strId.Trim() + ";pwd=" + strPwd.Trim() + ";database=" + strDbName.Trim() + " ";
                            ////////////////////////////////////////////////////////////////



                            this.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            MessageBox.Show("비밀번호가 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtPW.Text = "";
                        }
                    }
                    else
                    {
                        MessageBox.Show("등록된 ID가 없습니다.\n\nID를 다시한번 확인해 보세요.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("데이타베이스 접속실패입니다. 서버 접속정보를 확인해 보세요.", "[MTMS]Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log("로그인 실패", e.ToString());
            }
        }

        private void cboCompCd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtID.Focus();
            }
        }

        private void txtID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPW.Focus();
            }
        }

        private void txtPW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Login();
            }
        }

        private void pictureBox2_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            Login();
        }

        private void pictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
           // this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        }

        private void pictureBox1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
           // this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }

        private void pictureBox2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
           // this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        }

        private void pictureBox2_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
           // this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }


        #region 이미지 위치
        public void SetBackgroundBitmap(string strFilename, Color transparencyColor)
        {
            BackgroundBitmap = new Bitmap(strFilename);
            Width = BackgroundBitmap.Width;
            Height = BackgroundBitmap.Height;
            Region = BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        public void SetBackgroundBitmap(Image image, Color transparencyColor)
        {
            BackgroundBitmap = new Bitmap(image);
            Width = BackgroundBitmap.Width;
            Height = BackgroundBitmap.Height;
            Region = BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        protected Region BitmapToRegion(Bitmap bitmap, Color transparencyColor)
        {
            if (bitmap == null)
                throw new ArgumentNullException("Bitmap", "Bitmap cannot be null!");

            int height = bitmap.Height;
            int width = bitmap.Width;

            GraphicsPath path = new GraphicsPath();

            for (int j = 7; j < height; j++)
                for (int i = 5; i < width; i++)
                {
                    if (bitmap.GetPixel(i, j) == transparencyColor)
                        continue;

                    int x0 = i;

                    while ((i < width) && (bitmap.GetPixel(i, j) != transparencyColor))
                        i++;

                    path.AddRectangle(new Rectangle(x0, j, i - x0, 1));
                }

            Region region = new Region(path);
            path.Dispose();
            return region;
        }
        #endregion

        private void pictureBox1_MouseLeave(object sender, System.EventArgs e)
        {
            this.pictureBox1.BackgroundImage = null;
        }

        private void pictureBox2_MouseLeave(object sender, System.EventArgs e)
        {
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;

        }
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(AppFolder+ @"\images\image\login_btn_over.gif");
            pictureBox1.BackgroundImage = bitMap;
        }

        private void LoginForm_Load(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(AppFolder + @"\images\image\login(퍼스텍).jpg");
            panel1.BackgroundImage = bitMap;

            //임시
            //txtID.Text = "ADMIN";
            //txtPW.Text = "zemax";

            SystemBase.Base.gstrDbConn = "server=" + strIp.Trim() + ";uid=" + strId.Trim() + ";pwd=" + strPwd.Trim() + ";database=" + strDbName.Trim() + " ";

            string Query = "SELECT CO_CD, CO_NM FROM B_COMP_INFO(NOLOCK)";
            SystemBase.ComboMake.C1Combo(cboCompCd, Query);

            cboCompCd.Splits[0].DisplayColumns[0].Width = 0;

            ReadINI();

            if (SaveId == "Y")
            {
                chkSaveId.Checked = true;
                txtID.Text = SystemBase.Base.gstrUserID.ToString();
            }
            else
            {
                chkSaveId.Checked = false;
                txtID.Text = "";
            }

            cboCompCd.SelectedValue = "KO132"; //SCM은 퍼스텍으로 고정
            cboCompCd.Enabled = false;
            //cboCompCd.SelectedValue = SystemBase.Base.gstrCOMCD.ToString();

            txtPW.Focus();
        }

        #region INI 값 읽기, 설정
        // INI 값 읽기, 설정 
        public void SetIniValue(String Section, String Key, String Value, String iniPath)
        {
            SystemBase.Base.WritePrivateProfileString(Section, Key, Value, iniPath);
        }

        public void ReadINI()
        {
            StreamReader objReader = new StreamReader(AppFolder + "\\E2MAX_SCM_FTP.ini");
            string sLine = "";
            ArrayList arrText = new ArrayList();

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    arrText.Add(sLine);

                    if (sLine.Length > 6 && sLine.Substring(0, 6).ToString() == "SaveId")
                    {
                        string[] strTemp = sLine.Split('=');
                        SaveId = strTemp[1].Trim();
                    }

                    if (sLine.Length > 5 && sLine.Substring(0, 5).ToString() == "ComCd")
                    {
                        string[] strTemp = sLine.Split('=');
                        SystemBase.Base.gstrCOMCD = strTemp[1].Trim();
                    }

                    if (sLine.Length > 6 && sLine.Substring(0, 6).ToString() == "UserId")
                    {
                        string[] strTemp = sLine.Split('=');
                        SystemBase.Base.gstrUserID = strTemp[1].Trim();
                    }
                }
            }
            objReader.Close();
        }
        #endregion
    }
}
