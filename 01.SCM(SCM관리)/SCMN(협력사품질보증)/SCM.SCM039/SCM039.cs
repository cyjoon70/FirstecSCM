using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WNDW;

/// <summary>
/// 변경점
/// </summary>
namespace SCM.SCM039
{
	public partial class SCM039 : UIForm.FPCOMM1
	{

		#region 변수
		// 승인 권한
		string strGAuth = string.Empty;

		// scm에서 저장된 데이터는 수정 불가. 반려후 가능
		string SaveYn = string.Empty;

		// 파일 임시저장을 위한 number
		string strRan = string.Empty;
		#endregion

		#region 생성자
		public SCM039()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void SCM039_Load(object sender, EventArgs e)
		{
			SetAuth();

			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);
			SystemBase.Validation.GroupBox_Setting(groupBox3);

			// 4M1E 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboFM, "usp_B_COMMON @pType='COMM', @pCODE = 'SC190', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
			SystemBase.ComboMake.C1Combo(cboFmOe, "usp_B_COMMON @pType='COMM', @pCODE = 'SC190', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);

			// 등록구분
			SystemBase.ComboMake.C1Combo(cboREG_TYPE, "usp_B_COMMON @pType='COMM', @pCODE = 'SC200', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);

			// 날짜유형 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosDAY_TYPE, "usp_SC007 @pType='C1', @pMAJOR_CD = 'SC110', @pREL_CD1 = 'SC006', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");

			// 진행상태 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosSTATUS, "usp_SC007 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC006', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 3);

			strRan = Regex.Replace(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), @"[^0-9a-zA-Z가-힣]", "");

			SetInit();
		}

		private void SetInit()
		{
			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
			dtsDAY_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			cbosDAY_TYPE.SelectedValue = "10";  // 신고일

			txtREG_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
			cdtREC_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			SetCondition();
		}

		private void SetCondition()
		{
			SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);

			if (chkAPPROVAL_Y.Checked || SaveYn == "Y")
				SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
			else
				SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
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

		#region 첨부파일
		private void btnFiles_Click(object sender, EventArgs e)
		{
			bool bAuth = true;

			if (txtMGT_STATUS.Text == "승인" || SaveYn == "Y") bAuth = false;

			WNDWS01 pu = new WNDWS01(txtMGT_NO.Text, txtMGT_NO.Text, "", "", "", "", bAuth, strRan, "변경점", "SCMCH");
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

			SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
			SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);

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
				strQuery = " usp_SCM039 @pTYPE = 'S1' ";
				strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery = strQuery + ", @sDAY_TYPE		= '" + cbosDAY_TYPE.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sDAY_FR		= '" + dtsDAY_FR.Text + "' ";
				strQuery = strQuery + ", @sDAY_TO		= '" + dtsDAY_TO.Text + "' ";
				strQuery = strQuery + ", @sSTATUS		= '" + cbosSTATUS.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sENT_CD		= '" + txtsENT_CD.Text + "' ";
				strQuery = strQuery + ", @sITEM_CD		= '" + txtsITEM_CD.Text + "' ";
				strQuery = strQuery + ", @sPROJECT_NO	= '" + txtPROJ_NO.Text + "' ";
				strQuery = strQuery + ", @sFM			= '" + cboFM.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sCUST_CD		= '" + txtsCUST_CD.Text + "' ";
				strQuery = strQuery + ", @sMGT_NO		= '" + txtsMGT_NO.Text + "' ";

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
				SystemBase.Validation.GroupBox_Reset(groupBox2);
				SystemBase.Validation.GroupBox_Reset(groupBox3);

				string strSql = " usp_SCM039 @pTYPE		= 'S2' ";
				strSql = strSql + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql = strSql + ", @sMGT_NO			= '" + strNo + "' ";

				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

				// SCM 등록
				txtMGT_NO.Value = dt.Rows[0]["MGT_NO"].ToString();              //관리번호
				txtCUST_NM.Value = dt.Rows[0]["CUST_NM"].ToString();            //협력업체코드
				txtENT_NM.Value = dt.Rows[0]["ENT_NM"].ToString();              //사업명
				txtPROJECT_NO.Value = dt.Rows[0]["PROJ_NO"].ToString();         //프로젝트번호
				txtPROJECT_NM.Value = dt.Rows[0]["PROJECT_NM"].ToString();      //프로젝트명
				txtITEM_CD.Value = dt.Rows[0]["ITEM_CD"].ToString();            //품목코드
				txtITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();            //품목명
				cboREG_TYPE.SelectedValue = dt.Rows[0]["REG_TYPE"].ToString();	//등록구분
				txtMGT_STATUS.Value = dt.Rows[0]["MGT_STATUS"].ToString();      //상태
				cboFmOe.SelectedValue = dt.Rows[0]["FM"].ToString();                      //4M
				txtDEC_MSG.Value = dt.Rows[0]["DEC_MSG"].ToString();            //신고내용
				txtREG_PERSON.Value = dt.Rows[0]["REG_PERSON"].ToString();      //등록자
				txtREG_DT.Value = dt.Rows[0]["REG_DT"].ToString();              //등록일
				txtREMARKS.Value = dt.Rows[0]["REMARKS"].ToString();            //비고

				// 퍼스텍 등록

				if (dt.Rows[0]["APPROVAL_YN"].ToString() == "Y")                // 승인결과
					chkAPPROVAL_Y.Checked = true;
				else if (dt.Rows[0]["APPROVAL_YN"].ToString() == "N")
					chkAPPROVAL_N.Checked = true;

				cdtREC_DT.Value = dt.Rows[0]["REC_DT"].ToString();              //접수일
				cdtAPPROVAL_DT.Value = dt.Rows[0]["APPROVAL_DT"].ToString();    //승인일
				txtAPPROVAL_MSG.Value = dt.Rows[0]["APPROVAL_MSG"].ToString();  //승인의견
				txtQA_DEPT.Value = dt.Rows[0]["QA_DEPT"].ToString();            //품질부서장
				txtQA_DEPT_NM.Value = dt.Rows[0]["QA_DEPT_NM"].ToString();      //품질부서장 이름

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
			string ERRCode = "ER", MSGCode = "", Seq = "", Flag = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
			{

				try
				{
					if (string.IsNullOrEmpty(txtMGT_NO.Text))
						Flag = "I1";
					else
						Flag = "U1";

					string strQuery = "";
					strQuery = " usp_SCM039 @pTYPE = '" + Flag + "' ";
					strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @sMGT_NO		= '" + txtMGT_NO.Text + "' ";
					strQuery = strQuery + ", @pUP_ID		= '" + SystemBase.Base.gstrUserID + "' ";
					strQuery = strQuery + ", @pCUST_CD		= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @pENT_CD		= '" + txtENT_CD.Text + "' ";
					strQuery = strQuery + ", @pPROJ_NO		= '" + txtPROJECT_NO.Text + "' ";       
					strQuery = strQuery + ", @pITEM_CD		= '" + txtITEM_CD.Text + "' ";          
					strQuery = strQuery + ", @pREG_TYPE		= '" + cboREG_TYPE.SelectedValue + "' ";
					strQuery = strQuery + ", @pF_M			= '" + cboFmOe.SelectedValue + "' ";
					strQuery = strQuery + ", @pDEC_MSG		= '" + txtDEC_MSG.Text.Replace("'", "''") + "' ";
					strQuery = strQuery + ", @pREG_PERSON	= '" + txtREG_PERSON.Text + "' ";
					strQuery = strQuery + ", @pREMARKS		= '" + txtREMARKS.Text + "' ";


					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();

					if (string.IsNullOrEmpty(txtMGT_NO.Text))
						Seq = ds.Tables[0].Rows[0][2].ToString();
					else
						Seq = txtMGT_NO.Text;

					if (ERRCode == "ER")
					{
						Trans.Rollback();
						goto Exit;  // ER 코드 Return시 점프
					}
					else
					{
						SaveYn = "Y";

						// 임시저장 첨부파일 키 값 업데이트
						if (Flag == "I1")
						{
							strQuery = "";
							strQuery = " usp_SCM039 @pTYPE = 'UF' ";
							strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
							strQuery = strQuery + ", @pMGT_NO		= '" + Seq + "' ";
							strQuery = strQuery + ", @pFILES_NO		= '" + strRan + "' ";

							DataSet ds2 = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
							ERRCode = ds2.Tables[0].Rows[0][0].ToString();
							MSGCode = ds2.Tables[0].Rows[0][1].ToString();

							if (ERRCode == "ER")
							{
								Trans.Rollback();
								goto Exit;  // ER 코드 Return시 점프
							}

							Trans.Commit();
						}
					}
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

		#region 삭제
		protected override void DeleteExec()
		{
			if (string.IsNullOrEmpty(txtMGT_NO.Text)) return;

			if (chkAPPROVAL_Y.Checked)
			{
				MessageBox.Show("승인처리된 건은 삭제할 수 없습니다.");
				return;
			}

			DialogResult result = SystemBase.MessageBoxComm.Show("삭제 하시겠습니까?", "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (result == DialogResult.Yes)
			{
				string ERRCode, MSGCode = "";

				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

				try
				{
					string strQuery = "";
					strQuery = " usp_SCM039 @pTYPE = 'D1' ";
					strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @sMGT_NO =" + txtsMGT_NO.Text + "";

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();

					if (ERRCode == "ER")
					{
						Trans.Rollback();
						dbConn.Close();
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
						return;
					}
					else
					{
						Trans.Commit();
						goto Exit;
					}
				}
				catch (Exception ex)
				{
					Trans.Rollback();
					MessageBox.Show(ex.ToString());
					MSGCode = "P0001";
					dbConn.Close();
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
					return;
				}

			Exit:
				dbConn.Close();
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
				SystemBase.Validation.GroupBox_Reset(groupBox2);
				SelectExec("");

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
		private void btnPROJ_Click(object sender, EventArgs e)
		{
			GetProjInfo(txtPROJ_NO, txtPROJ_NM);
		}

		private void btnsProj_Click(object sender, EventArgs e)
		{
			GetProjInfo(txtPROJ_NO, txtPROJ_NM);
		}

		private void GetProjInfo(C1.Win.C1Input.C1TextBox id, C1.Win.C1Input.C1TextBox name)
		{
			try
			{
				string strQuery = " usp_SC_COMM_POPUP @pTYPE = 'S2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pCUST_CD = '" + txtsCUST_CD.Text + "'";
				string[] strWhere = new string[] { "@pCD", "@pNM" };
				string[] strSearch;

				strSearch = new string[] { id.Text, name.Text };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM_COMM_PROJ", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트조회");
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

		#region 사업 조회
		private void btnENT_Click(object sender, EventArgs e)
		{
			GetEntInfo(txtENT_CD, txtENT_NM);
		}

		private void btnsEnt_Click(object sender, EventArgs e)
		{
			GetEntInfo(txtsENT_CD, txtsENT_NM);
		}

		private void GetEntInfo(C1.Win.C1Input.C1TextBox id, C1.Win.C1Input.C1TextBox name)
		{
			try
			{
				string strQuery = " usp_SC_COMM_POPUP @pTYPE = 'S3' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pCUST_CD = '" + txtsCUST_CD.Text + "'";
				string[] strWhere = new string[] { "@pCD", "@pNM" };
				string[] strSearch;

				strSearch = new string[] { id.Text, name.Text };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM_COMM_ENT", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업조회");
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

		#region 품목조회
		private void btnITEM_Click(object sender, EventArgs e)
		{
			GetItemInfo(txtITEM_CD, txtITEM_NM);
		}

		private void btnsItem_Click(object sender, EventArgs e)
		{
			GetItemInfo(txtsITEM_CD, txtsITEM_NM);
		}

		private void GetItemInfo(C1.Win.C1Input.C1TextBox id, C1.Win.C1Input.C1TextBox name)
		{
			try
			{
				string strQuery = " usp_SC_COMM_POPUP @pTYPE = 'S1' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pCUST_CD = '" + txtsCUST_CD.Text + "'";
				string[] strWhere = new string[] { "@pCD", "@pNM" };
				string[] strSearch;

				strSearch = new string[] { id.Text, name.Text };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM_COMM_ITEM", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목조회");
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

	}
}
