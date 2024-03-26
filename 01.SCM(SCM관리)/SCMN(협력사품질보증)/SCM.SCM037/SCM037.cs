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

/// <summary>
/// 품질보증계획서
/// </summary>
namespace SCM.SCM037
{
	public partial class SCM037 : UIForm.FPCOMM1
	{

		#region 변수
		// 권한
		string strGAuth = string.Empty;

		// scm에서 저장된 데이터는 수정 불가. 반려후 가능
		string SaveYn = string.Empty;
		#endregion

		#region 생성자
		public SCM037()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void SCM037_Load(object sender, EventArgs e)
		{
			SetAuth();

			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);
			SystemBase.Validation.GroupBox_Setting(groupBox3);
			SystemBase.Validation.GroupBox_Setting(groupBox4);

			// 날짜유형 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosDAY_TYPE, "usp_SCM037 @pType='C1', @pMAJOR_CD = 'SC110', @pREL_CD1 = 'SC005', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");

			// 진행상태 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosSTATUS, "usp_SCM037 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC005', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 3);

			SetInit();
		}

		private void SetInit()
		{
			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
			dtsDAY_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString();

			cbosDAY_TYPE.SelectedValue = "07";  // 요청일

			cdtpREQ_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			SetCondition(true);
		}

		// 화면 모드(strGProc)에 따라 컨트롤 설정
		private void SetCondition(bool bLoad)
		{

			// 첨부파일 처리
			if (string.IsNullOrEmpty(txtpSEQ.Text))
				btnFiles.Enabled = false;
			else
				btnFiles.Enabled = true;

			SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
			SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);

			// 승인건은 모두 lock 처리. 
			if (chkpAPPROVAL_Y.Checked || SaveYn == "Y" || bLoad)
				SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
			else
				SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);

			// 컨트롤 back color 설정
			foreach (System.Windows.Forms.Control c in groupBox2.Controls)
			{
				#region 컨트롤 체크
				if (c.GetType().Name == "C1Combo")
				{
					C1.Win.C1List.C1Combo cbo = (C1.Win.C1List.C1Combo)c;

					if (!cbo.Enabled)
						cbo.EditorBackColor = SystemBase.Validation.Kind_Gainsboro;
					
				}
				else if (c.GetType().Name == "C1TextBox")
				{
					C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;

					if (ctb.ReadOnly)
						ctb.BackColor = SystemBase.Validation.Kind_Gainsboro;
					
				}
				else if (c.GetType().Name == "C1NumericEdit")
				{
					C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;

					if (cne.ReadOnly)
						cne.BackColor = SystemBase.Validation.Kind_Gainsboro;
					
				}
				else if (c.GetType().Name == "C1DateEdit")
				{
					C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;

					if (cde.ReadOnly)
						cde.BackColor = SystemBase.Validation.Kind_Gainsboro;
					
				}
				#endregion
			}
		}

		private void SetAuth()
		{
			if (SystemBase.Base.gstrScmAdmin == "N")
			{
				btnsCust.Tag = ";2;;";
				txtsCUST_CD.Tag = ";2;;";

				txtsCUST_CD.Text = SystemBase.Base.gstrUserID;
				txtsCUST_NM.Text = SystemBase.Base.gstrUserName;
			}
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

		private void btnCust_Click(object sender, EventArgs e)
		{
			GetCustInfo(txtpCUST_CD, txtpCUST_NM);
		}

		private void txtpCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtpCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtpCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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

		#region 프로젝트 조회
		private void btnsProj_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_SC_COMM_POPUP @pTYPE = 'S2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pCUST_CD = '" + txtsCUST_CD.Text + "'";
				string[] strWhere = new string[] { "@pCD", "@pNM" };
				string[] strSearch;

				strSearch = new string[] { txtsPROJ_NO.Text, txtsPROJ_NM.Text };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM_COMM_PROJ", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtsPROJ_NO.Value = Msgs[0].ToString();
					txtsPROJ_NM.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void txtsPROJ_NO_TextChanged(object sender, EventArgs e)
		{
			txtsPROJ_NM.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtsPROJ_NO.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

		#region 첨부파일
		private void btnFiles_Click(object sender, EventArgs e)
		{
			bool bAuth = true;

			if (chkpAPPROVAL_Y.Checked || SaveYn == "Y")
				bAuth = false;

			WNDWS01 pu = new WNDWS01(txtpSEQ.Text, txtpSEQ.Text, "", "", "", "", bAuth, "", "품질보증계획서", "SCMQD");
			pu.ShowDialog();
		}
		#endregion

		#region New
		protected override void NewExec()
		{
			SetAuth();

			SystemBase.Validation.GroupBox_Reset(groupBox1);
			SystemBase.Validation.GroupBox_Reset(groupBox2);
			SystemBase.Validation.GroupBox_Reset(groupBox3);
			SystemBase.Validation.GroupBox_Reset(groupBox4);

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
				strQuery = " usp_SCM037 @pTYPE = 'S1' ";
				strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery = strQuery + ", @sDAY_TYPE		= '" + cbosDAY_TYPE.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sDAY_FR		= '" + dtsDAY_FR.Text + "' ";
				strQuery = strQuery + ", @sDAY_TO		= '" + dtsDAY_TO.Text + "' ";
				strQuery = strQuery + ", @sSTATUS		= '" + cbosSTATUS.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sCUST_CD		= '" + txtsCUST_CD.Text + "' ";
				strQuery = strQuery + ", @sPROJECT_NO	= '" + txtsPROJ_NO.Text + "' ";
				strQuery = strQuery + ", @sPO_NO		= '" + txtsPO_NO.Text + "' ";

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
					string strSeq = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "일련번호")].Text.ToString();
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

				string strSql = " usp_SCM037 @pTYPE	 = 'S2' ";
				strSql = strSql + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql = strSql + ", @pSEQ = '" + strNo + "' ";

				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

				// 최초 등록
				txtpSEQ.Value = dt.Rows[0]["SEQ"].ToString();						// 일련번호
				txtpPROJ_NO.Value = dt.Rows[0]["PROJ_NO"].ToString();				// 프로젝트번호
				txtpPROJ_NM.Value = dt.Rows[0]["PROJECT_NM"].ToString();			// 프로젝트명
				txtpBIZ_CD.Value = dt.Rows[0]["BIZ_CD"].ToString();					// 사업코드
				txtpBIZ_NM.Value = dt.Rows[0]["ENT_NM"].ToString();					// 사업명
				txtpITEM_CD.Value = dt.Rows[0]["ITEM_CD"].ToString();				// 품목코드
				txtpITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();				// 품목명
				txtpPO_NO.Value = dt.Rows[0]["PO_NO"].ToString();					// 발주번호
				txtpCUST_CD.Value = dt.Rows[0]["CUST_CD"].ToString();				// 업체코드
				txtpCUST_NM.Value = dt.Rows[0]["CUST_NM"].ToString();				// 업체명
				cdtpREG_LIMIT_DT.Value = dt.Rows[0]["REG_LIMIT_DT"].ToString();		// 등록기한
				txtpFST_PERSON.Value = dt.Rows[0]["FST_PERSON"].ToString();         // 퍼스텍 담당자
				txtpFST_PERSON_NM.Value = dt.Rows[0]["FST_PERSON_NM"].ToString();   // 퍼스텍 담당자명
				cdtpREQ_DT.Value = dt.Rows[0]["REQ_DT"].ToString();                 // 요구일
				txtpFST_REMARKS.Value = dt.Rows[0]["FST_REMARKS"].ToString();		// 퍼스텍 담당자 의견

				// SCM 업체 등록
				cdtpREG_DT.Value = dt.Rows[0]["REG_DT"].ToString();					// 등록일
				txtpREG_PERSON.Value = dt.Rows[0]["REG_PERSON"].ToString();			// 등록자

				// 퍼스텍 승인권자 등록
				txtpFST_APPR.Value = dt.Rows[0]["FST_APPR"].ToString();				// 승인자
				txtpFST_APPR_NM.Value = dt.Rows[0]["FST_APPR_NM"].ToString();		// 승인자 이름
				cdtpAPPROVAL_DT.Value = dt.Rows[0]["APPROVAL_DT"].ToString();       // 처리일

				if (dt.Rows[0]["APPROVAL_YN"].ToString() == "Y")
					chkpAPPROVAL_Y.Checked = true;
				else if (dt.Rows[0]["APPROVAL_YN"].ToString() == "N")
					chkpAPPROVAL_N.Checked = true;


				SetCondition(false);
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
			string ERRCode = "ER", MSGCode = "", Seq = "", pType = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))
			{
				try
				{
					
					string strQuery = "";
					strQuery = " usp_SCM037 @pTYPE = 'U1' ";
					strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @pSEQ			= '" + txtpSEQ.Text + "' ";
					strQuery = strQuery + ", @pREG_DT		= '" + cdtpREG_DT.Text + "' ";
					strQuery = strQuery + ", @pREG_PERSON	= '" + txtpREG_PERSON.Text + "' ";
					strQuery = strQuery + ", @pUP_ID		= '" + SystemBase.Base.gstrUserID + "' ";   // 수정자

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();

					Seq = txtpSEQ.Text;

					if (ERRCode == "ER")
					{
						Trans.Rollback();
						goto Exit;  // ER 코드 Return시 점프
					}
					else
						SaveYn = "Y";
				}
				catch (Exception ex)
				{
					Trans.Rollback();
					MessageBox.Show(ex.ToString());
					MSGCode = "P0001";
					goto Exit;  // ER 코드 Return시 점프
				}
				Trans.Commit();

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
