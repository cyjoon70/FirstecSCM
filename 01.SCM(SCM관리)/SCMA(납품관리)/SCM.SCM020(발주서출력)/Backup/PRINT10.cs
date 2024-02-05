using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.Net;
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows;

namespace PRINT10
{
	/// <summary>
	/// PRINT에 대한 요약 설명입니다.
	/// </summary>
	public class PRINT10 : System.Windows.Forms.Form
	{
		CrystalDecisions.CrystalReports.Engine.ReportDocument rpt = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
		private System.ComponentModel.Container components = null;

		string Server = "", DbName = "", Pwd = "", UsrId = "";

		string[] FormulaField	= null;
		string[] FormulaFieldName = null;

		string RptName1			= null;
		string[] RptParmValue1	= null;

		string SubRptName2		= null;
		string[] RptParmValue2	= null;

		string SubRptName3		= null;
		string[] RptParmValue3	= null;

		string SubRptName4		= null;
		string[] RptParmValue4	= null;

		string SubRptName5		= null;
		private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
		string[] RptParmValue5	= null;

		public PRINT10(
			string			FormText1
			, string[]		FormulaField2
			, string		RptName11
			, string[]		RptParmValue11
			, string		strServer
			, string		strDbName
			, string		strUsrId
			, string		strPwd
			)
		{
			InitializeComponent();
			this.Text		= FormText1;

			FormulaField	= FormulaField2;
			RptName1		= RptName11;
			RptParmValue1	= RptParmValue11;
			
			Server = strServer;
			DbName = strDbName;
			UsrId = strUsrId;
			Pwd = strPwd;
		}

		public PRINT10(			
			string			FormText1
			, string[]		FormulaFieldValue
			, string[]		FormulaFieldNameValue
			, string		RptName11
			, string[]		RptParmValue11
			, string		strServer
			, string		strDbName
			, string		strUsrId
			, string		strPwd
			)
		{
			InitializeComponent();
			this.Text		= FormText1;

			FormulaField	= FormulaFieldValue;
			FormulaFieldName= FormulaFieldNameValue;

			RptName1		= RptName11;
			RptParmValue1	= RptParmValue11;

			Server = strServer;
			DbName = strDbName;
			UsrId = strUsrId;
			Pwd = strPwd;

		}

		public PRINT10(
			string			FormText1
			, string[]		FormulaField2
			, string		RptName11
			, string[]		RptParmValue11
			, string		SubRptName21
			, string[]		RptParmValue21
			, string		strServer
			, string		strDbName
			, string		strUsrId
			, string		strPwd
			)
		{
			InitializeComponent();
			this.Text		= FormText1;

			FormulaField	= FormulaField2;

			RptName1		= RptName11;
			RptParmValue1	= RptParmValue11;

			SubRptName2		= SubRptName21;
			RptParmValue2	= RptParmValue21;

			Server = strServer;
			DbName = strDbName;
			UsrId = strUsrId;
			Pwd = strPwd;
		}

		public PRINT10(
			string			FormText1
			, string[]		FormulaField2
			, string		RptName11
			, string[]		RptParmValue11
			, string		SubRptName21
			, string[]		RptParmValue21
			, string		SubRptName31
			, string[]		RptParmValue31
			, string		strServer
			, string		strDbName
			, string		strUsrId
			, string		strPwd
			)
		{
			InitializeComponent();
			this.Text		= FormText1;

			FormulaField	= FormulaField2;

			RptName1		= RptName11;
			RptParmValue1	= RptParmValue11;

			SubRptName2		= SubRptName21;
			RptParmValue2	= RptParmValue21;

			SubRptName3		= SubRptName31;
			RptParmValue3	= RptParmValue31;

			Server = strServer;
			DbName = strDbName;
			UsrId = strUsrId;
			Pwd = strPwd;
		}

		public PRINT10(
			string			FormText1
			, string[]		FormulaField2
			, string		RptName11
			, string[]		RptParmValue11
			, string		SubRptName21
			, string[]		RptParmValue21
			, string		SubRptName31
			, string[]		RptParmValue31
			, string		SubRptName41
			, string[]		RptParmValue41
			, string		strServer
			, string		strDbName
			, string		strUsrId
			, string		strPwd
			)
		{
			InitializeComponent();
			this.Text		= FormText1;

			FormulaField	= FormulaField2;

			RptName1		= RptName11;
			RptParmValue1	= RptParmValue11;

			SubRptName2		= SubRptName21;
			RptParmValue2	= RptParmValue21;

			SubRptName3		= SubRptName31;
			RptParmValue3	= RptParmValue31;
		
			SubRptName4		= SubRptName41;
			RptParmValue4	= RptParmValue41;

			Server = strServer;
			DbName = strDbName;
			UsrId = strUsrId;
			Pwd = strPwd;
		}

		public PRINT10(
			string			FormText1
			, string[]		FormulaField2
			, string		RptName11
			, string[]		RptParmValue11
			, string		SubRptName21
			, string[]		RptParmValue21
			, string		SubRptName31
			, string[]		RptParmValue31
			, string		SubRptName41
			, string[]		RptParmValue41
			, string		SubRptName51
			, string[]		RptParmValue51
			, string		strServer
			, string		strDbName
			, string		strUsrId
			, string		strPwd
			)
		{
			InitializeComponent();
			this.Text		= FormText1;

			FormulaField	= FormulaField2;

			RptName1		= RptName11;
			RptParmValue1	= RptParmValue11;

			SubRptName2		= SubRptName21;
			RptParmValue2	= RptParmValue21;

			SubRptName3		= SubRptName31;
			RptParmValue3	= RptParmValue31;
		
			SubRptName4		= SubRptName41;
			RptParmValue4	= RptParmValue41;

			SubRptName5		= SubRptName51;
			RptParmValue5	= RptParmValue51;

			Server = strServer;
			DbName = strDbName;
			UsrId = strUsrId;
			Pwd = strPwd;
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form 디자이너에서 생성한 코드
		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PRINT10));
			this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
			this.SuspendLayout();
			// 
			// crystalReportViewer1
			// 
			this.crystalReportViewer1.ActiveViewIndex = -1;
			this.crystalReportViewer1.DisplayGroupTree = false;
			this.crystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.crystalReportViewer1.Location = new System.Drawing.Point(0, 0);
			this.crystalReportViewer1.Name = "crystalReportViewer1";
			this.crystalReportViewer1.ReportSource = null;
			this.crystalReportViewer1.Size = new System.Drawing.Size(544, 366);
			this.crystalReportViewer1.TabIndex = 0;
			// 
			// PRINT10
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(544, 366);
			this.Controls.Add(this.crystalReportViewer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "PRINT10";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "PRINT10";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.PRINT10_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void LogonToReport(string server, string database,string ID, string password)
		{
			TableLogOnInfo logonInfo = new TableLogOnInfo();
			foreach(CrystalDecisions.CrystalReports.Engine.Table table in rpt.Database.Tables)
			{
				logonInfo = table.LogOnInfo;
				logonInfo.ConnectionInfo.ServerName        = server;
				logonInfo.ConnectionInfo.DatabaseName    = database;
				logonInfo.ConnectionInfo.UserID            = ID;
				logonInfo.ConnectionInfo.Password        = password;
				table.ApplyLogOnInfo(logonInfo);
			}            
		} 

		private void PRINT10_Load(object sender, System.EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;

				/* db 연결 */
//				CrystalDecisions.Shared.TableLogOnInfos myTingos = new CrystalDecisions.Shared.TableLogOnInfos();
//				CrystalDecisions.Shared.TableLogOnInfo myTingo = new CrystalDecisions.Shared.TableLogOnInfo();
//				CrystalDecisions.Shared.ConnectionInfo myConn = new CrystalDecisions.Shared.ConnectionInfo();
//
//				
//				myConn.ServerName = SystemBase.Base.gstrServerNM.ToString();
//				myConn.DatabaseName = SystemBase.Base.gstrDbName.ToString();
//				myConn.UserID = SystemBase.Base.gstrServerId.ToString();
//				myConn.Password = SystemBase.Base.gstrServerPwd.ToString();
//
//				myTingo.ConnectionInfo = myConn;
//				myTingos.Add(myTingo);
//				crystalReportViewer1.LogOnInfo = myTingos;

				rpt.Load(RptName1);

				LogonToReport(Server, DbName, UsrId, Pwd);

//				rpt.SetDatabaseLogon(SystemBase.Base.gstrServerId, SystemBase.Base.gstrServerPwd, SystemBase.Base.gstrServerNM, SystemBase.Base.gstrDbName);
				
//				rpt.SetDatabaseLogon(SystemBase.Base.gstrServerId, SystemBase.Base.gstrServerPwd);

				if(FormulaField != null)
				{
					for(int i = 0; i < rpt.DataDefinition.FormulaFields.Count; i++)
					{
						string FieldName = rpt.DataDefinition.FormulaFields[i].Name;

						for(int j = 0; j < FormulaField.Length; j++)
						{
							if(FieldName == FormulaFieldName[j])
							{
								rpt.DataDefinition.FormulaFields[i].Text = FormulaField[j] ;
							}
						}
					}

//					crystalReportViewer1.ReportSource = rpt;
				}
				
				for(int i = 0; i < RptParmValue1.Length; i++)
				{
					rpt.SetParameterValue(i, RptParmValue1[i]);
				}
				crystalReportViewer1.ReportSource = rpt;

				if(RptParmValue2 != null)
				{
					rpt.OpenSubreport(SubRptName2);
					for(int i = 0; i < RptParmValue2.Length; i++)
					{
						rpt.SetParameterValue(i, RptParmValue2[i]);
					}
					crystalReportViewer1.ReportSource = rpt;
				}

				if(RptParmValue3 != null)
				{
					rpt.OpenSubreport(SubRptName3);
					for(int i = 0; i < RptParmValue3.Length; i++)
					{
						rpt.SetParameterValue(i, RptParmValue3[i]);
					}
					crystalReportViewer1.ReportSource = rpt;
				}
		
				if(RptParmValue4 != null)
				{
					rpt.OpenSubreport(SubRptName4);
					for(int i = 0; i < RptParmValue4.Length; i++)
					{
						rpt.SetParameterValue(i, RptParmValue4[i]);
					}
					crystalReportViewer1.ReportSource = rpt;
				}
		
				if(RptParmValue5 != null)
				{
					rpt.OpenSubreport(SubRptName5);
					for(int i = 0; i < RptParmValue5.Length; i++)
					{
						rpt.SetParameterValue(i, RptParmValue5[i]);
					}
					crystalReportViewer1.ReportSource = rpt;
				}

				this.Cursor = Cursors.Default;
			}
			catch(Exception f)
			{
				MessageBox.Show(f.ToString(), "PRINT ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}

/**********사용예제*******************
string RptName = @"Report\Report2.rpt";				// 레포트경로+레포트명
string[] RptParmValue = new string[]{"a"};			// SP 파라메타 값
UIForm.PRINT10 frm = new UIForm.PRINT10( "프린트 테스트", FormulaField, RptName, RptParmValue);	//공통크리스탈 10버전
frm.ShowDialog();
**************************************/

/**********사용예제*******************
string RptName = @"Report\test.rpt";				// 레포트경로+레포트명
string[] RptParmValue = new string[]{"a"};			// SP 파라메타 값
string SubRptName2 = "bbb";							// 서브레포트 명
string[] RptParmValue2 = new string[]{"b"};			// SP 파라메타 값
UIForm.PRINT10 frm = new UIForm.PRINT10("프린트 테스트", FormulaField, RptName, RptParmValue, SubRptName2, RptParmValue2);	//공통크리스탈 10버전
frm.ShowDialog();
**************************************/

/**********사용예제*******************
string[] FormulaField = new string[]{"111", "222"};	// Formula
string RptName = @"Report\test.rpt";				// 레포트경로+레포트명
string[] RptParmValue = new string[]{"a"};			// SP 파라메타 값
string SubRptName2 = "bbb";							// 서브레포트 명
string[] RptParmValue2 = new string[]{"b"};			// SP 파라메타 값
UIForm.PRINT10 frm = new UIForm.PRINT10("프린트 테스트", FormulaField, RptName, RptParmValue, SubRptName2, RptParmValue2);	//공통크리스탈 10버전
frm.ShowDialog();
**************************************/
