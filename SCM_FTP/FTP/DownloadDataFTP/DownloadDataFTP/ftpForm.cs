
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
using System.Diagnostics;



namespace DownloadDataFTP
{
    

    public partial class ftpForm : Form
    {

        #region DllImport
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //이미실행중이면 화면 맨앞으로오게 하고
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void BringWindowToTop(IntPtr hWnd);
        //이미실행중이면 포커스(Activate)를 준다.
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void SetForegroundWindow(IntPtr hWnd);
        #endregion

        public ftpForm()
        {
            InitializeComponent();
        }


        private byte[] downloadedData;
        int totCnt = 0;

        #region Form Load
        private void ftpForm_Load(object sender, EventArgs e)
        {
            #region E2MAXMenu 가 실행중이면 강제로 KILL 시킨다.
            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.Process[] proc2 = System.Diagnostics.Process.GetProcessesByName("E2MAXSCM");  //실행파일명
            int ProcessCnt = proc2.Length;  //프로세스 로드 수
            bool PSYN = true;// false인 경우 최대화 할 프로그램이 없음

            if (ProcessCnt >= 1)
            {
                //다운받을 파일이 존재
                DialogResult dsMsg = MessageBox.Show("이미 E2MAX SCM이 실행중입니다. 중단하시고 다시 여시겠습니까?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    for (int i = 0; i < ProcessCnt; i++)
                    {
                        if (proc2[i].Id.ToString() != currentProcess.Id.ToString())
                        {
                           proc2[i].Kill();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < ProcessCnt; i++)
                    {
                        if (proc2[i].Id.ToString() != currentProcess.Id.ToString())
                        {
                            SetForegroundWindow(proc2[i].MainWindowHandle);
                            PSYN = ShowWindow(proc2[i].MainWindowHandle, 3);//3- 최대화, 4-이전 크기로
                            BringWindowToTop(proc2[i].MainWindowHandle);

                        }
                    }

                    this.Close();
                    return;
                    
                    
                }
            }
            #endregion


            Base.AppFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            StreamReader objReader = new StreamReader(Base.AppFolder + "\\E2MAX_SCM_FTP.ini");
            string sLine = "";
            ArrayList arrText = new ArrayList();

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    arrText.Add(sLine);

                    //DATABASE
                    if (sLine.Length >= 10 && sLine.Substring(0, 10).ToString() == "ServerName")
                    {
                        string[] strTemp = sLine.Split('=');
                        Base.strDBServer = Base.Decode(strTemp[1].Trim());

                    }

                    if (sLine.Length >= 10 && sLine.Substring(0, 5).ToString() == "LogId")
                    {
                        string[] strTemp = sLine.Split('=');
                        Base.strDBLoginID = Base.Decode(strTemp[1].Trim());

                    }

                    if (sLine.Length >= 7 && sLine.Substring(0, 7).ToString() == "LogPass")
                    {
                        string[] strTemp = sLine.Split('=');
                        Base.strDBPass = Base.Decode(strTemp[1].Trim());
                    }

                    if (sLine.Length >= 8 && sLine.Substring(0, 8).ToString() == "Database")
                    {
                        string[] strTemp = sLine.Split('=');
                        Base.strDBName = Base.Decode(strTemp[1].Trim());
                    }

                    //FTP
                    if (sLine.Length >= 9 && sLine.Substring(0, 9).ToString() == "FTPServer")
                    {
                        string[] strTemp = sLine.Split('=');
                        Base.strFTPAddress = "ftp://" + Base.Decode(strTemp[1].Trim());
                    }

                    if (sLine.Length >= 7 && sLine.Substring(0, 7).ToString() == "FTPUser")
                    {
                        string[] strTemp = sLine.Split('=');
                        Base.strUsername = Base.Decode(strTemp[1].Trim());

                    }

                    if (sLine.Length >= 7 && sLine.Substring(0, 7).ToString() == "FTPPass")
                    {
                        string[] strTemp = sLine.Split('=');
                        Base.strPassword = Base.Decode(strTemp[1].Trim());
                    }

                    if (sLine.Length >= 7 && sLine.Substring(0, 7).ToString() == "FTPPath")
                    {
                        string[] strTemp = sLine.Split('=');
                        Base.strFTPPath = strTemp[1].Trim();
                    }
                }
            }
            objReader.Close();


            if (Base.strDBServer == "" || Base.strDBLoginID == "" || Base.strDBPass == "" || Base.strDBName == "")
            {
                MessageBox.Show(this, "Database Server 서버정보가 없습니다. 관리자에게 문의 하세요", "E2Max", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }
            else
            {
                DbOpen.gstrDbConn = @"server=" + Base.strDBServer + ";uid=" + Base.strDBLoginID + ";pwd=" + Base.strDBPass + ";database=" + Base.strDBName;	//DB 연결정보
            }


            if (Base.strFTPAddress == "" || Base.strUsername == "" || Base.strPassword == "")
            {
                MessageBox.Show(this, "FTP Server 서버정보가 없습니다. 관리자에게 문의 하세요", "E2Max", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            totCnt = fnc_DownFileCount(Base.AppFolder);

            if (totCnt < 0)
            {
                frmDBInfo myForm = new frmDBInfo();
                myForm = new frmDBInfo();

                myForm.ShowDialog();

                this.Close();
                return;
            }


            if (totCnt == 0)
            {
                //다운받을 파일이 없다
                Process.Start("E2MAXSCM.exe", "ST:" + Base.strDBServer + ":" + Base.strDBName + ":" + Base.strDBLoginID + ":" + Base.strDBPass);
                //E2MAXMenu_Loading();
                this.Close();
            }
            else
            {
                //다운받을 파일이 존재
                DialogResult dsMsg = MessageBox.Show("변경된 파일이 " + totCnt.ToString() + " 개 존재합니다. 다운로드 하시겠습니까?", "다운로드 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    Application.DoEvents();
                    this.Show();
                    this.Activate();

                    Application.DoEvents();
                    Down_Start();

                }
                else
                {
                    Process.Start("E2MAXSCM.exe", "ST:" + Base.strDBServer + ":" + Base.strDBName + ":" + Base.strDBLoginID + ":" + Base.strDBPass);
                    //E2MAXMenu_Loading();
                    this.Close();
                }

            }

        }
        #endregion

        #region 다운받을 파일 개수 카운드 fnc_DownFileCount
        private static int fnc_DownFileCount(string AppFolder)
        {
            int iCount = 0;

            try
            {

                string strFullFile = "";
                string strSql = "SELECT SFILE, CPATH, CONVERT(VARCHAR(19), FTIME, 120) AS FTIME, FSIZE ";
                strSql = strSql + " FROM CO_AUTO_DOWNLOAD ";
                strSql = strSql + " WHERE USE_TYPE IN ('00', '30') ";
                strSql = strSql + " ORDER BY SFILE";

                DataTable dt = DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "ER")
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strFullFile = AppFolder + dt.Rows[i]["CPATH"].ToString() + dt.Rows[i]["SFILE"].ToString();

                        if (File.Exists(strFullFile))
                        {
                            FileInfo fInfo = new FileInfo(strFullFile);
                            string strFileSize = fInfo.Length.ToString();

                            if (File.GetLastWriteTime(strFullFile).ToString("yyyy-MM-dd HH:mm:ss") != dt.Rows[i]["FTIME"].ToString() || strFileSize != dt.Rows[i]["FSIZE"].ToString())
                            {
                                iCount = iCount + 1;
                            }

                        }
                        else
                        {
                            iCount = iCount + 1;
                        }

                        Application.DoEvents();
                    }

                }
                else
                {
                    MessageBox.Show("Database 접속을 실패 하였습니다. Database 환경을 다시 설정하여 주십시요", "E2Max", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    iCount = -1;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Database 접속을 실패 하였습니다. Database 환경을 다시 설정하여 주십시요", "E2Max", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                iCount = -1;
            }

            return iCount;
        }
        #endregion

        #region 다운로드 시작 Down_Start
        private void Down_Start()
        {
            Boolean bolErr = false;

            try
			{
                string strFullFile = "";

                string strSql =  "SELECT SFILE, CPATH, CONVERT(VARCHAR(19), FTIME, 120) AS FTIME, FSIZE ";
                strSql = strSql + " FROM CO_AUTO_DOWNLOAD ";
                strSql = strSql + " WHERE USE_TYPE IN ('00', '30') ";
                strSql = strSql + " ORDER BY SFILE";

                DataTable dt = DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "ER")
                {
                    progressBar2.Value = 0;
                    progressBar2.Maximum = totCnt;
                    progressBar2.Text = "0/" + totCnt.ToString();

                    //for (int i = 0; i < 2; i++)
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strFullFile = Base.AppFolder + dt.Rows[i]["CPATH"].ToString() + dt.Rows[i]["SFILE"].ToString();

                        if (File.Exists(strFullFile))
                        {
                            FileInfo fInfo = new FileInfo(strFullFile);
                            string strFileSize = fInfo.Length.ToString();

                            if (File.GetLastWriteTime(strFullFile).ToString("yyyy-MM-dd HH:mm:ss") != dt.Rows[i]["FTIME"].ToString() || strFileSize != dt.Rows[i]["FSIZE"].ToString())
                            {
                                txtFileName.Text = dt.Rows[i]["SFILE"].ToString();
                                Application.DoEvents();

                                string FullFileName = "";

                                //if (dt.Rows[i]["CPATH"].ToString() != "")
                                //{
                                    //FullFileName = dt.Rows[i]["CPATH"].ToString().Replace(@"\", @"/") + dt.Rows[i]["SFILE"].ToString();
                                    FullFileName = dt.Rows[i]["SFILE"].ToString();
                                //}

                                Down_Load(FullFileName);

                                if (downloadedData != null && downloadedData.Length != 0)
                                {
                                    //Write the bytes to a file
                                    FileStream newFile = new FileStream(Base.AppFolder + dt.Rows[i]["CPATH"].ToString() + dt.Rows[i]["SFILE"].ToString(), FileMode.Create);

                                    newFile.Write(downloadedData, 0, downloadedData.Length);
                                    newFile.Close();

                                    //파일 최종수정일자 update
                                    File.Exists(strFullFile);
                                    File.SetLastWriteTime(strFullFile, Convert.ToDateTime(dt.Rows[i]["FTIME"]));

                                    //Update the progress bar
                                    if (progressBar2.Value + 1 <= progressBar2.Maximum)
                                    {
                                        progressBar2.Value += 1;
                                        lbProgressTot.Text = progressBar2.Value.ToString() + " 개 / " + totCnt.ToString() + " 개";

                                        progressBar2.Refresh();
                                        Application.DoEvents();
                                    }
                                }
                            }

                        }
                        else
                        {

                            string sDirPath;
                            sDirPath = Base.AppFolder + dt.Rows[i]["CPATH"].ToString();
                            DirectoryInfo di = new DirectoryInfo(sDirPath);
                            if (di.Exists == false)
                            {
                                di.Create();
                            }


                            txtFileName.Text = dt.Rows[i]["SFILE"].ToString();
                            Application.DoEvents();

                            string FullFileName = "";

                            //if (dt.Rows[i]["CPATH"].ToString() != "")
                            //{
                                //FullFileName = dt.Rows[i]["CPATH"].ToString().Replace(@"\", @"/") + dt.Rows[i]["SFILE"].ToString();
                                FullFileName = dt.Rows[i]["SFILE"].ToString();
                            //}
                            Down_Load(FullFileName);

                            if (downloadedData != null && downloadedData.Length != 0)
                            {
                                //Write the bytes to a file
                                FileStream newFile = new FileStream(Base.AppFolder + dt.Rows[i]["CPATH"].ToString() + dt.Rows[i]["SFILE"].ToString(), FileMode.Create);

                                newFile.Write(downloadedData, 0, downloadedData.Length);
                                newFile.Close();

                                //파일 최종수정일자 update
                                File.Exists(strFullFile);
                                File.SetLastWriteTime(strFullFile, Convert.ToDateTime(dt.Rows[i]["FTIME"]));

                                //Update the progress bar
                                if (progressBar2.Value + 1 <= progressBar2.Maximum)
                                {
                                    progressBar2.Value += 1;
                                    lbProgressTot.Text = progressBar2.Value.ToString() + " 개 / " + totCnt.ToString() + " 개";

                                    progressBar2.Refresh();
                                    Application.DoEvents();
                                }
                            }

                        }

                        Application.DoEvents();
                    }

                }
                else
                {
                    bolErr = true;

                    MessageBox.Show(this, "Database 접속을 실패 하였습니다. Database 환경을 다시 설정하여 주십시요", "E2Max", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    frmDBInfo myForm = new frmDBInfo();
                    myForm = new frmDBInfo();

                    myForm.ShowDialog();

                    this.Close();
                    return;
                }

            }
			catch(Exception f)
			{
                MessageBox.Show(f.ToString());

                this.Close();
                return;

			}

            if (bolErr == false)
            {

                E2MAXMenu_Loading();

                this.Cursor = Cursors.Default;
            }

            this.Close();
            return;
            
        }
        #endregion

        #region 다운로드 파일
        private void Down_Load(string File_Name)
        {
            downloadFile(Base.strFTPAddress, File_Name, Base.strUsername, Base.strPassword);
        }

        //Connects to the FTP server and downloads the file
        private void downloadFile(string FTPAddress, string filename, string username, string password)
        {
            downloadedData = new byte[0];

            try
            {
                //Optional
                this.Text = "최신 파일을 다운로드 중입니다. 잠시만 기다려 주세요...";
                Application.DoEvents();

                //Create FTP request
                //Note: format is ftp://server.com/file.ext
                FtpWebRequest request = FtpWebRequest.Create(FTPAddress + "/" + Base.strFTPPath + "/" + filename) as FtpWebRequest;
                request.Proxy = null;


                //Optional
                //this.Text = "최신 파일을 다운로드 중입니다. 잠시만 기다려 주세요...";
                Application.DoEvents();

                //Get the file size first (for progress bar)
                request.Method = WebRequestMethods.Ftp.GetFileSize;
                request.Credentials = new NetworkCredential(username, password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = true; //don't close the connection

                int dataLength = (int)request.GetResponse().ContentLength;

                //Optional500894U
                //this.Text = "파일을 다운로드 합니다...";
                //Application.DoEvents();

                //Now get the actual data
                request = FtpWebRequest.Create(FTPAddress + "/" + Base.strFTPPath + "/" + filename) as FtpWebRequest;
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(username, password);
                request.UsePassive = false;
                request.UseBinary = true;
                request.KeepAlive = false; //close the connection when done

                //Set up progress bar
                progressBar1.Value = 0;
                progressBar1.Maximum = dataLength;
                lbProgress.Text = "0 Byte / " + dataLength.ToString() + " Byte";

                //Streams
                FtpWebResponse response = request.GetResponse() as FtpWebResponse;
                Stream reader = response.GetResponseStream();

                //Download to memory
                //Note: adjust the streams here to download directly to the hard drive
                MemoryStream memStream = new MemoryStream();
                byte[] buffer = new byte[1024]; //downloads in chuncks

                while (true)
                {
                    Application.DoEvents(); //prevent application from crashing

                    //Try to read the data
                    int bytesRead = reader.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        //Nothing was read, finished downloading
                        progressBar1.Value = progressBar1.Maximum;
                        lbProgress.Text = dataLength.ToString() + " Byte / " + dataLength.ToString() + " Byte";
                        Application.DoEvents();

                        //progressBar2.Value = progressBar2.Maximum;
                        //lbProgressTot.Text = totCnt.ToString() +"/" + totCnt.ToString();
                        //Application.DoEvents();
                        break;
                    }
                    else
                    {
                        //Write the downloaded data
                        memStream.Write(buffer, 0, bytesRead);

                        //Update the progress bar
                        if (progressBar1.Value + bytesRead <= progressBar1.Maximum)
                        {
                            progressBar1.Value += bytesRead;
                            lbProgress.Text = progressBar1.Value.ToString() + " Byte / " + dataLength.ToString() + " Byte";

                            progressBar1.Refresh();
                            Application.DoEvents();
                        }

                        
                    }

                    
                }

                //Convert the downloaded stream to a byte array
                downloadedData = memStream.ToArray();

                //Clean up
                reader.Close();
                memStream.Close();
                response.Close();

                //MessageBox.Show("Downloaded Successfully");
            }
            catch (Exception f)
            {
                //MessageBox.Show(this, "FTP Server 서버에 접속을 실패 하였습니다. 관리자에게 문의 하세요", "E2Max",  MessageBoxButtons.OK, MessageBoxIcon.Warning);

                //MessageBox.Show(this, f.ToString() + "::::" + FTPAddress + ":" + username + ":" + password, "E2Max", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MessageBox.Show(this, "FTP Server 접속을 실패 하였습니다. Database 환경을 다시 설정하여 주십시요", "E2Max", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                frmFTPInfo myForm = new frmFTPInfo();
                myForm = new frmFTPInfo();

                myForm.ShowDialog();

                this.Close();

            }

            txtData.Text = downloadedData.Length.ToString();
            //this.Text = "Download Data through FTP";

        }
        #endregion

        #region E2MAXMenu.exe Run
        private void E2MAXMenu_Loading()
        {
            Process.Start("E2MAXSCM.exe", "ST:" + Base.strDBServer + ":" + Base.strDBName + ":" + Base.strDBLoginID + ":" + Base.strDBPass);
        }
        #endregion



    }
}