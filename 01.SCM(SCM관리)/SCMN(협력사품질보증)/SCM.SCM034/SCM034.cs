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
/// 시정조치
/// </summary>
namespace SCM.SCM034
{
    public partial class SCM034 : UIForm.FPCOMM1
    {

		#region 변수
		// scm에서 저장된 데이터는 수정 불가. 반려후 가능
		string SaveYn = string.Empty;
		#endregion

		#region 생성자
		public SCM034()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 
        private void SCM034_Load(object sender, EventArgs e)
        {
			SetAuth();

			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);

            // 발행유형 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cbosACTION_TYPE, "usp_B_COMMON @pType='COMM', @pCODE = 'SC100', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboACTION_TYPE, "usp_B_COMMON @pType='COMM', @pCODE = 'SC100', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);

            // 날짜유형 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cbosDAY_TYPE, "usp_SC003 @pType='C1', @pMAJOR_CD = 'SC110', @pREL_CD1 = 'SC003', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");

            // 진행상태 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cbosSTATUS, "usp_SC003 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC003', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboCORR_STATUS, "usp_SC003 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC003', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 9);

			SetInit();
		}

        private void SetInit()
        {
			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);
			SystemBase.Validation.GroupBox_Setting(groupBox3);
			SystemBase.Validation.GroupBox_Setting(groupBox4);
			SystemBase.Validation.GroupBox_Setting(groupBox5);
			SystemBase.Validation.GroupBox_Setting(groupBox6);


			SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
			SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
			SystemBase.Validation.GroupBoxControlsLock(groupBox5, true);
			SystemBase.Validation.GroupBoxControlsLock(groupBox6, true);

			SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);

			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtsDAY_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString();
            dtREG_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
		}

		private void SetAuth()
		{
			if (SystemBase.Base.gstrScmAdmin == "N")
			{
				btnSCust.Tag = ";2;;";
				txtsCUST_CD.Tag = ";2;;";

				txtsCUST_CD.Text = SystemBase.Base.gstrUserID;
				txtsCUST_NM.Text = SystemBase.Base.gstrUserName;
			}
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
            SystemBase.Validation.GroupBox_Reset(groupBox5);
            SystemBase.Validation.GroupBox_Reset(groupBox6);
			
            fpSpread1.Sheets[0].Rows.Count = 0;

			SaveYn = string.Empty;

			SetInit();
		}
        #endregion

        #region 협력사 조회
        private void btnSCust_Click(object sender, EventArgs e)
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

        #region 조회

		// 리스트 조회
        protected override void SearchExec()
        {
            SelectExec("");
        }
        
        private void SelectExec(string CORR_NO)
        {
            try
            {
                string strQuery = "";
                strQuery = " usp_SCM034 @pTYPE = 'S1' ";
                strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @sACTION_TYPE	= '" + cbosACTION_TYPE.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @sDAY_TYPE		= '" + cbosDAY_TYPE.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @sDAY_FR		= '" + dtsDAY_FR.Text + "' ";
                strQuery = strQuery + ", @sDAY_TO		= '" + dtsDAY_TO.Text + "' ";
                strQuery = strQuery + ", @sSTATUS		= '" + cbosSTATUS.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @sCUST_CD		= '" + txtsCUST_CD.Text +"' ";
                strQuery = strQuery + ", @sTITLE		= '" + txtsTITLE.Text +"' ";
                strQuery = strQuery + ", @sCORR_NO		= '" + txtsCORR_NO.Text +"' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int x = 0, y = 0;

                    if (!string.IsNullOrEmpty(CORR_NO))
                    {
                        fpSpread1.Search(0, CORR_NO, false, false, false, false, 0, 0, ref x, ref y);

                        if (x >= 0)
                        {
                            fpSpread1.Sheets[0].SetActiveCell(x, y);
                            fpSpread1.Sheets[0].AddSelection(x, 1, 1, fpSpread1.Sheets[0].ColumnCount);

                            //상세정보조회
                            SubSearch(CORR_NO);
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

		// 상세 정보 조회
		private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{
			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				try
				{
					int intRow = fpSpread1.Sheets[0].GetSelection(0).Row;
					string strCorrNo = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "시정조치번호")].Text.ToString();
					SaveYn = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "저장여부")].Text.ToString();

					SubSearch(strCorrNo);
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

				string strSql = " usp_SCM034 @pTYPE	 = 'S2' ";
				strSql = strSql + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql = strSql + ", @pCORR_NO = '" + strNo + "' ";

				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

				// 최초 등록
				txtCORR_NO.Value = dt.Rows[0]["CORR_NO"].ToString();
				cboACTION_TYPE.SelectedValue = dt.Rows[0]["ACTION_TYPE"].ToString();
				txtREG_DEPT.Value = dt.Rows[0]["REG_DEPT"].ToString();
				txtREG_PERSON.Value = dt.Rows[0]["REG_PERSON"].ToString();
				txtREG_PERSON_NM.Value = dt.Rows[0]["REG_PERSON_NM"].ToString();
				dtREG_DT.Value = dt.Rows[0]["REG_DT"].ToString();
				dtCOMP_REQ_DT.Value = dt.Rows[0]["COMP_REQ_DT"].ToString();
				txtTITLE.Value = dt.Rows[0]["TITLE"].ToString();
				cboCORR_STATUS.SelectedValue = dt.Rows[0]["CORR_STATUS"].ToString();
				txtREQ_MSG.Value = dt.Rows[0]["REQ_MSG"].ToString();
				txtDEPT_PERSON.Value = dt.Rows[0]["DEPT_PERSON"].ToString();
				txtDEPT_PERSON_NM.Value = dt.Rows[0]["DEPT_PERSON_NM"].ToString();
				txtDEPT_REMARKS.Value = dt.Rows[0]["DEPT_REMARKS"].ToString();
				txtCUST_CD.Value = dt.Rows[0]["CUST_CD"].ToString();
				txtCUST_NM.Value = dt.Rows[0]["CUST_NM"].ToString();
				txtFileApprId.Value = dt.Rows[0]["FILE_APPR"].ToString();
				txtFileApprNm.Value = dt.Rows[0]["FILE_APPR_NM"].ToString();

				// 업체 등록
				txtCUST_DEPT.Value = dt.Rows[0]["CUST_DEPT"].ToString();
				txtCUST_POSITION.Value = dt.Rows[0]["CUST_POSITION"].ToString();
				txtCUST_PERSON.Value = dt.Rows[0]["CUST_PERSON"].ToString();
				dtCUST_REG_DT.Value = dt.Rows[0]["CUST_REG_DT"].ToString();

				if (dt.Rows[0]["CONN_PROC_YN"].ToString() == "Y")
					chkCONN_PROC_Y.Checked = true;
				else if (dt.Rows[0]["CONN_PROC_YN"].ToString() == "N")
					chkCONN_PROC_N.Checked = true;

				txtIMMED_MSG.Value = dt.Rows[0]["IMMED_MSG"].ToString().Replace("\n", "\r\n");
				txtROOT_CAUSE.Value = dt.Rows[0]["ROOT_CAUSE"].ToString();
				txtCAUSE_TYPE.Value = dt.Rows[0]["CAUSE_TYPE"].ToString();
				txtCAUSE_TYPE_NM.Value = dt.Rows[0]["CAUSE_TYPE_NM"].ToString();
				txtROOT_CAUSE_MSG.Value = dt.Rows[0]["ROOT_CAUSE_MSG"].ToString();

				if (dt.Rows[0]["ADD_BAD_YN"].ToString() == "Y")
					chkADD_BAD_Y.Checked = true;
				else if (dt.Rows[0]["ADD_BAD_YN"].ToString() == "N")
					chkADD_BAD_N.Checked = true;

				txtACTION_DEPT.Value = dt.Rows[0]["ACTION_DEPT"].ToString();
				dtACTION_DT.Value = dt.Rows[0]["ACTION_DT"].ToString();
				txtACTION_MSG.Value = dt.Rows[0]["ACTION_MSG"].ToString();

				// 퍼스텍 담당자 등록
				txtFST_PERSON.Value = dt.Rows[0]["FST_PERSON"].ToString();
				txtFST_PERSON_NM.Value = dt.Rows[0]["FST_PERSON_NM"].ToString();

				if (dt.Rows[0]["APPROVAL_YN"].ToString() == "Y")
					chkAPPROVAL_Y.Checked = true;
				else if (dt.Rows[0]["APPROVAL_YN"].ToString() == "N")
					chkAPPROVAL_N.Checked = true;

				dtAPPROVAL_DT.Value = dt.Rows[0]["APPROVAL_DT"].ToString();
				txtCORR_RESULT.Value = dt.Rows[0]["CORR_RESULT"].ToString();
				txtCORR_EFFECTS.Value = dt.Rows[0]["CORR_EFFECTS"].ToString();
				dtCORR_EFFECTS_DT.Value = dt.Rows[0]["CORR_EFFECTS_DT"].ToString();

				// 퍼스텍 승인권자 등록
				txtFST_APPROVAL.Value = dt.Rows[0]["FST_APPROVAL"].ToString();
				txtFST_APPROVAL_NM.Value = dt.Rows[0]["FST_APPROVAL_NM"].ToString();

				if (dt.Rows[0]["FST_APPROVAL_YN"].ToString() == "Y")
					chkFST_APPROVAL_Y.Checked = true;
				else if (dt.Rows[0]["FST_APPROVAL_YN"].ToString() == "N")
					chkFST_APPROVAL_N.Checked = true;

				dtFST_APPR_DT.Value = dt.Rows[0]["FST_APPR_DT"].ToString();
				txtFINAL_REMARKS.Value = dt.Rows[0]["FINAL_REMARKS"].ToString();

				// 기 저장된 데이터는 수정 불가.반려처리 후 수정 가능
				if (SaveYn == "Y")
					SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);
				else
					SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);
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

		#region 저장
		protected override void SaveExec()
        {
            string ERRCode = "ER", MSGCode = "", CorrNo = "", strCONN_PROC_YN = "", strADD_BAD_YN = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))
            {
                try
                {
					if (chkCONN_PROC_Y.Checked)
						strCONN_PROC_YN = "Y";
					else
						strCONN_PROC_YN = "N";

					if (chkADD_BAD_Y.Checked)
						strADD_BAD_YN = "Y";
					else
						strADD_BAD_YN = "N";

					string strQuery = "";
                    strQuery = " usp_SCM034 @pTYPE = 'U1' ";
                    strQuery = strQuery + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery = strQuery + ", @pCORR_NO			= '" + txtCORR_NO.Text + "' ";
					strQuery = strQuery + ", @pCUST_DEPT     	= '" + txtCUST_DEPT.Text + "' ";
					strQuery = strQuery + ", @pCUST_POSITION 	= '" + txtCUST_POSITION.Text + "' ";
					strQuery = strQuery + ", @pCUST_PERSON   	= '" + txtCUST_PERSON.Text + "' ";
					strQuery = strQuery + ", @pCUST_REG_DT   	= '" + dtCUST_REG_DT.Text + "' ";
					strQuery = strQuery + ", @pCONN_PROC_YN  	= '" + strCONN_PROC_YN + "' ";
					strQuery = strQuery + ", @pIMMED_MSG     	= '" + txtIMMED_MSG.Text + "' ";
					strQuery = strQuery + ", @pROOT_CAUSE    	= '" + txtROOT_CAUSE.Text + "' ";
					strQuery = strQuery + ", @pCAUSE_TYPE    	= '" + txtCAUSE_TYPE.Text + "' ";
					strQuery = strQuery + ", @pROOT_CAUSE_MSG	= '" + txtROOT_CAUSE_MSG.Text + "' ";
					strQuery = strQuery + ", @pADD_BAD_YN    	= '" + strADD_BAD_YN + "' ";
					strQuery = strQuery + ", @pACTION_DEPT   	= '" + txtACTION_DEPT.Text + "' ";
					strQuery = strQuery + ", @pACTION_DT     	= '" + dtACTION_DT.Text + "' ";
					strQuery = strQuery + ", @pACTION_MSG    	= '" + txtACTION_MSG.Text + "' ";
					strQuery = strQuery + ", @pUP_ID         	= '" + SystemBase.Base.gstrUserID + "' ";

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();
                    CorrNo = txtCORR_NO.Text;

                    if (ERRCode == "ER")
                    {
                        Trans.Rollback();
                        goto Exit;  // ER 코드 Return시 점프
                    }
					else
					{
						SaveYn = "Y";
					}
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
                    SelectExec(CorrNo);
            }

        }
        #endregion
        
		#region 첨부파일 처리
		private void btnAddFiles_Click(object sender, EventArgs e)
		{
			bool bAuth = true;

			try
			{
				if (chkAPPROVAL_Y .Checked || chkFST_APPROVAL_Y.Checked || SaveYn == "Y") bAuth = false;

				 // 첨부파일 팝업 띄움.
				 WNDWS01 pu = new WNDWS01(txtCORR_NO.Text, txtCORR_NO.Text, "", "", "", "", bAuth, "", "시정조치", "SCMCA");
				pu.ShowDialog();
			}
			catch (Exception f)
			{
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region long text popup 처리
		private void txtREQ_MSG_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("조치요구내용");
		}

		private void txtDEPT_REMARKS_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("발행부서의견");
		}

		private void txtIMMED_MSG_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("즉시조치");
		}

		private void txtROOT_CAUSE_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("근본원인");
		}

		private void txtROOT_CAUSE_MSG_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("근본원인시정및조치걔획");
		}

		private void txtACTION_MSG_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("조치부서의견");
		}

		private void txtCORR_RESULT_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("시정조치확인결과");
		}

		private void txtFINAL_REMARKS_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("승인부서의견");
		}

		private void popupMsg(string msg)
		{
			if (!string.IsNullOrEmpty(txtCORR_NO.Text))
			{
				//SCM034P1 myForm = new SCM034P1(txtCORR_NO.Text, msg);
				//myForm.ShowDialog();
			}
		}


		#endregion

		#region 원인분류코드 조회
		private void bCAUSE_TYPE_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='SC150' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtCAUSE_TYPE.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00033", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "원인분류코드 팝업");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{

					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtCAUSE_TYPE.Text = Msgs[0].ToString();
					txtCAUSE_TYPE_NM.Value = Msgs[1].ToString();
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

		#region 체크박스 이벤트
		private void chkCONN_PROC_Y_CheckedChanged(object sender, EventArgs e)
		{
			if (chkCONN_PROC_Y.Checked)
				chkCONN_PROC_N.Checked = false;
		}

		private void chkCONN_PROC_N_CheckedChanged(object sender, EventArgs e)
		{
			if (chkCONN_PROC_N.Checked)
				chkCONN_PROC_Y.Checked = false;
		}

		private void chkADD_BAD_Y_CheckedChanged(object sender, EventArgs e)
		{
			if (chkADD_BAD_Y.Checked)
				chkADD_BAD_N.Checked = false;
		}

		private void chkADD_BAD_N_CheckedChanged(object sender, EventArgs e)
		{
			if (chkADD_BAD_N.Checked)
				chkADD_BAD_Y.Checked = false;
		}
		#endregion
	}
}
