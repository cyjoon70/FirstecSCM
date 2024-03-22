using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WNDW;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.IO;

/// <summary>
/// 특수공정
/// </summary>
namespace SCM.SCM038
{
	public partial class SCM038 : UIForm.FPCOMM1
	{

        #region 변수
        string strGAuth = string.Empty; // 원래는 승인권자 변수였으나, 특수공정은 승인권자가 없으므로 선행 저장 체크 변수로 사용

		// 파일 임시저장을 위한 number
		string strRan = string.Empty;

		// scm에서 저장된 데이터는 수정 불가. 반려후 가능
		string SaveYn = string.Empty;
		#endregion

		#region 생성자
		public SCM038()
		{
			InitializeComponent();
		}

		#endregion

		#region Form Load
		private void SCM038_Load(object sender, EventArgs e)
		{
			if (SystemBase.Base.gstrUserID != "KO132")
			{
				txtsCUST_CD.Tag = ";2;;";
				btnSCust.Tag = ";2;;";
			}

			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);
			SystemBase.Validation.GroupBox_Setting(groupBox3);

			// 공정 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboPROCESS, "usp_B_COMMON @pType='COMM', @pCODE = 'SC170', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
			SystemBase.ComboMake.C1Combo(cboPROC_CD, "usp_B_COMMON @pType='COMM', @pCODE = 'SC170', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);

			// 날짜유형 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosDAY_TYPE, "usp_SCM038 @pType='C1', @pMAJOR_CD = 'SC110', @pREL_CD1 = 'SC006', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");

			// 진행상태 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosSTATUS, "usp_SCM038 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC006', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 3);
			SystemBase.ComboMake.C1Combo(cboAPP_STATUS, "usp_SCM038 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC006', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 9);

            SystemBase.ComboMake.C1Combo(cboEST_TYPE, "usp_B_COMMON @pType='COMM', @pCODE = 'SC180', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);

			strRan = Regex.Replace(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), @"[^0-9a-zA-Z가-힣]", "");

			SetInit();
		}

		private void SetInit()
		{
			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
			dtsDAY_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			cdtAPP_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			cbosDAY_TYPE.SelectedValue = "08";  // 접수일

			cdtREC_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			SetCondition();
		}

		private void SetCondition()
		{
			SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
			SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);

			if (string.IsNullOrEmpty(txtAPPLICATION_NO.Text) || chkEST_RESULT_Y.Checked || SaveYn == "Y")
				SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
			else
				SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

		}
        #endregion

        #region 협력업체 조회 
        private void btnsCust_Click(object sender, EventArgs e)
		{
			GetCustInfo(txtsCUST_CD, txtsCUST_NM);
		}

		private void txtsCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtsCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtsCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void GetCustInfo(C1.Win.C1Input.C1TextBox id, C1.Win.C1Input.C1TextBox name)
		{
			try
			{
				WNDW002 pu = new WNDW002(id.Text, "");
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					id.Value = Msgs[1].ToString();
					name.Value = Msgs[2].ToString();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

		#region 접수자, 평가자, 검토자 조회
		private void btnREC_PERSON_Click(object sender, EventArgs e)
		{
			GetPerson(txtREC_PERSON, txtREC_PERSON_NM);
		}

		private void txtREC_PERSON_TextChanged(object sender, EventArgs e)
		{
			txtREC_PERSON_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtREC_PERSON.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void btnEST_PERSON_Click(object sender, EventArgs e)
		{
			GetPerson(txtEST_PERSON, txtEST_PERSON_NM);
		}

		private void txtEST_PERSON_TextChanged(object sender, EventArgs e)
		{
			txtEST_PERSON_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtEST_PERSON.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void btnTEST_PERSON_Click(object sender, EventArgs e)
		{
			GetPerson(txtTEST_PERSON, txtTEST_PERSON_NM);
		}

		private void txtTEST_PERSON_TextChanged(object sender, EventArgs e)
		{
			txtTEST_PERSON_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtTEST_PERSON.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void GetPerson(C1.Win.C1Input.C1TextBox id, C1.Win.C1Input.C1TextBox name)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { id.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사원 팝업");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{

					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					id.Value = Msgs[0].ToString();
					name.Value = Msgs[1].ToString();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

		#region 첨부파일
		private void btnFiles_Click(object sender, EventArgs e)
		{
			bool bAuth = true;

			if (cboAPP_STATUS.Text == "승인")
				bAuth = false;
			else
			{
				if (!chkEST_TECH_RESULT_Y.Checked) bAuth = false;
			}					   		

			// 첨부파일 팝업 띄움.
			WNDWS01 pu = new WNDWS01(txtAPPLICATION_NO.Text, txtAPPLICATION_NO.Text, "", "", "", "", bAuth, strRan, "특수공정", "SCMSP");
			pu.ShowDialog();
		}
		#endregion

		#region New
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			SystemBase.Validation.GroupBox_Reset(groupBox2);
			SystemBase.Validation.GroupBox_Reset(groupBox3);

			SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

			fpSpread1.Sheets[0].Rows.Count = 0;

			SaveYn = string.Empty;

			SetInit();
		}
		#endregion

		#region 조회
		protected override void SearchExec()
		{
			SelectExec("");
		}

		private void SelectExec(string SEQ)
		{
			try
			{
				string strQuery = "";
				strQuery = " usp_SCM038 @pTYPE = 'S1' ";
				strQuery = strQuery + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery = strQuery + ", @sDAY_TYPE			= '" + cbosDAY_TYPE.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sDAY_FR			= '" + dtsDAY_FR.Text + "' ";
				strQuery = strQuery + ", @sDAY_TO			= '" + dtsDAY_TO.Text + "' ";
				strQuery = strQuery + ", @sSTATUS			= '" + cbosSTATUS.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sPROCESS			= '" + cboPROCESS.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sCUST_CD			= '" + txtsCUST_CD.Text + "' ";
				strQuery = strQuery + ", @sAPPLICATION_NO	= '" + txtsAPPLICATION_NO.Text + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);
				fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					int x = 0, y = 0;

					if (!string.IsNullOrEmpty(SEQ))
					{
						fpSpread1.Search(0, SEQ, false, false, false, false, 0, 0, ref x, ref y);

						if (x >= 0)
						{
							fpSpread1.Sheets[0].SetActiveCell(x, y);
							fpSpread1.Sheets[0].AddSelection(x, 1, 1, fpSpread1.Sheets[0].ColumnCount);

							//상세정보조회
							SubSearch(SEQ);
						}
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
			}
		}

		#region 상세 정보 조회
		private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{
			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				try
				{
					int intRow = fpSpread1.Sheets[0].GetSelection(0).Row;
					string strSeq = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "신청번호")].Text.ToString();
					SaveYn = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM 저장여부")].Text.ToString();

					SubSearch(strSeq);
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					//데이터 조회 중 오류가 발생하였습니다.				
				}
			}
		}

		private void SubSearch(string strNo)
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
                SystemBase.Validation.GroupBox_Setting(groupBox2);
                SystemBase.Validation.GroupBox_Setting(groupBox3);
                SystemBase.Validation.GroupBox_Setting(groupBox4);

                SystemBase.Validation.GroupBox_Reset(groupBox2);
				SystemBase.Validation.GroupBox_Reset(groupBox3);
                SystemBase.Validation.GroupBox_Reset(groupBox4);

                string strSql = " usp_SC006 @pTYPE		= 'S2' ";
				strSql = strSql + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql = strSql + ", @sAPPLICATION_NO	= '" + strNo + "' ";

				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

				// SCM 등록
				txtAPPLICATION_NO.Value = dt.Rows[0]["APPLICATION_NO"].ToString();      // 신청번호
				txtCUST_NM.Value = dt.Rows[0]["CUST_NM"].ToString();					// 협력업체코드
				txtPROJECT_NO.Value = dt.Rows[0]["PROJ_NO"].ToString();                 // 프로젝트번호
				txtPROJECT_NM.Value = dt.Rows[0]["PROJECT_NM"].ToString();				// 프로젝트명
				txtAPP_PERSON.Value = dt.Rows[0]["APP_PERSON"].ToString();				// 신청자
				cdtAPP_DT.Value = dt.Rows[0]["APP_DT"].ToString();                      // 신청일
				cboPROC_CD.SelectedValue = dt.Rows[0]["PROC_CD"].ToString();			// 공정
				cboEST_TYPE.SelectedValue = dt.Rows[0]["EST_TYPE"].ToString();			// 평가구분
				txtSPEC_NO.Value = dt.Rows[0]["SPEC_NO"].ToString();					// 규격번호
				cboAPP_STATUS.SelectedValue = dt.Rows[0]["APP_STATUS"].ToString();		// 상태
				txtACT_CUST_NM.Value = dt.Rows[0]["ACT_CUST_NM"].ToString();			// 수행업체-업체명
				txtACT_CUST_PERSON.Value = dt.Rows[0]["ACT_CUST_PERSON"].ToString();	// 수행업체-담당자
				txtACT_CUST_CONN.Value = dt.Rows[0]["ACT_CUST_CONN"].ToString();		// 수행업체-연락처
				txtACT_CUST_ADDR.Value = dt.Rows[0]["ACT_CUST_ADDR"].ToString();		// 수행업체-주소

				// 퍼스텍 등록
				txtREC_PERSON.Value = dt.Rows[0]["REC_PERSON"].ToString();              // 접수자
				txtREC_PERSON_NM.Value = dt.Rows[0]["REC_PERSON_NM"].ToString();		// 접수자명
				cdtREC_DT.Value = dt.Rows[0]["REC_DT"].ToString();						// 접수일
				txtEST_PERSON.Value = dt.Rows[0]["EST_PERSON"].ToString();              // 평가자
				txtEST_PERSON_NM.Value = dt.Rows[0]["EST_PERSON_NM"].ToString();		// 평가자명
				cdtEST_PLAN_DT.Value = dt.Rows[0]["EST_PLAN_DT"].ToString();            // 평가예정일

				if (dt.Rows[0]["EST_TECH_RESULT"].ToString() == "Y")                     // 기술검토결과
					chkEST_TECH_RESULT_Y.Checked = true;
				else if (dt.Rows[0]["EST_TECH_RESULT"].ToString() == "N")
					chkEST_TECH_RESULT_N.Checked = true;


				txtTEST_PERSON.Value = dt.Rows[0]["TEST_PERSON"].ToString();            // 검토자
				txtTEST_PERSON_NM.Value = dt.Rows[0]["TEST_PERSON_NM"].ToString();		// 검토자명
				cdtTEST_DT.Value = dt.Rows[0]["TEST_DT"].ToString();					// 검토일
				txtEST_TECH_MSG.Value = dt.Rows[0]["EST_TECH_MSG"].ToString();			// 기술검토의견


				if (dt.Rows[0]["EST_RESULT"].ToString() == "Y")                         // 평가결과
					chkEST_RESULT_Y.Checked = true;
				else if (dt.Rows[0]["EST_RESULT"].ToString() == "N")
					chkEST_RESULT_N.Checked = true;

				cdtAPPROVAL_DT.Value = dt.Rows[0]["APPROVAL_DT"].ToString();			// 승인일
				cdtAVAILABLE_DT.Value = dt.Rows[0]["AVAILABLE_DT"].ToString();			// 유효일
				txtADD_INFO.Value = dt.Rows[0]["ADD_INFO"].ToString();					// 부가정보
				txtAPPROVAL_GUBUN.Value = dt.Rows[0]["APPROVAL_GUBUN"].ToString();		// 승인구분
				txtAPPROVAL_RANGE.Value = dt.Rows[0]["APPROVAL_RANGE"].ToString();		// 승인범위

				SetCondition();

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}

			this.Cursor = Cursors.Default;
		}
		#endregion

		#endregion

		#region 저장
		protected override void SaveExec()
		{
			string ERRCode = "ER", MSGCode = "", Seq = "", EST_TECH_RESULT = "", EST_RESULT = "", FLAG = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
			{
				try
				{
					if (string.IsNullOrEmpty(txtAPPLICATION_NO.Text))
						FLAG = "I1";
					else
						FLAG = "U1";

					string strQuery = "";
					strQuery = " usp_SCM038 @pTYPE = '" + FLAG + "' ";
					strQuery = strQuery + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD	+ "' ";				// 법인코드
					strQuery = strQuery + ", @sAPPLICATION_NO	= '" + txtAPPLICATION_NO.Text		+ "' ";				// 신청번호
					strQuery = strQuery + ", @pCUST_CD			= '" + SystemBase.Base.gstrUserID	+ "' ";             // 협력업체코드
					strQuery = strQuery + ", @pUP_ID			= '" + SystemBase.Base.gstrUserID	+ "' ";				// 수정자
					strQuery = strQuery + ", @pPROJ_NO			= '" + txtPROJECT_NO.Text			+ "' ";				// 프로젝트번호
					strQuery = strQuery + ", @pAPP_PERSON		= '" + txtAPP_PERSON.Text			+ "' ";				// 신청자
					strQuery = strQuery + ", @pAPP_DT			= '" + cdtAPP_DT.Text				+ "' ";				// 신청일
					strQuery = strQuery + ", @pPROC_CD			= '" + cboPROC_CD.SelectedValue		+ "' ";				// 공정
					strQuery = strQuery + ", @pEST_TYPE			= '" + cboEST_TYPE.SelectedValue	+ "' ";				// 평가구분
					strQuery = strQuery + ", @pACT_CUST_PERSON	= '" + txtACT_CUST_PERSON.Text + "' ";                  // 수행업체-담당자
					strQuery = strQuery + ", @pACT_CUST_CONN	= '" + txtACT_CUST_CONN.Text + "' ";                    // 수행업체-연락처
					strQuery = strQuery + ", @pSPEC_NO			= '" + txtSPEC_NO.Text.Replace("'", "''")		+ "' ";	// 규격번호
					strQuery = strQuery + ", @pACT_CUST_NM		= '" + txtACT_CUST_NM.Text.Replace("'", "''")	+ "' ";	// 수행업체-업체명
					strQuery = strQuery + ", @pACT_CUST_ADDR	= '" + txtACT_CUST_ADDR.Text.Replace("'", "''")	+ "' ";	// 수행업체-주소

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();

					if (string.IsNullOrEmpty(txtAPPLICATION_NO.Text))
						Seq = ds.Tables[0].Rows[0][2].ToString();
					else
						Seq = txtAPPLICATION_NO.Text;

					if (ERRCode == "ER")
					{
						Trans.Rollback();
						goto Exit;  // ER 코드 Return시 점프
					}
					else
						SaveYn = "Y";

					Trans.Commit();
				}
				catch (Exception ex)
				{
					Trans.Rollback();
					MessageBox.Show(ex.ToString());
					MSGCode = "P0001";
					goto Exit;  // ER 코드 Return시 점프
				}
			Exit:
				dbConn.Close();
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));

				if (ERRCode == "OK")
					SelectExec(Seq);
			}

		}
        #endregion

    }
}
