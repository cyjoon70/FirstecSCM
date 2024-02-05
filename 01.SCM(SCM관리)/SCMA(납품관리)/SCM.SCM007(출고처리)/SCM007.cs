using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using WNDW;


namespace SCM.SCM007
{
    public partial class SCM007 : UIForm.FPCOMM1
    {

		#region 변수

		// 저장된 LOT 정보이면 LOT 팝업 화면에서 조회만 가능하도록 하기 위한 변수
		bool bSaveLot = true;

		// Lot 분할/수정/삭제 팝업에서 Lot 수량을 변경 적용 후, Parent Form 입고수량을 수정해 주어야 하고 이때 불필요한 확인 메시지는 나타나지 않게 한다.
		bool bMsgYN = true;

		// LOT 팝업화면에서 넘어온 데이터를 담는 데이터테이블
		public DataTable dt = new DataTable();
		
		#endregion

        #region 생성자
        public SCM007()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load
        private void SCM007_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("110000011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            txtCustCd.Value = SystemBase.Base.gstrUserID;
            if (SystemBase.Base.gstrUserID == "KO132")
            {
                txtCustCd.Tag = "거래처;1;;";
                btnCustCd.Tag = "";
            }
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);            

            dtpPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 10);
            dtpPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
			bSaveLot = true;
			dt.Clear();

            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            txtCustCd.Value = SystemBase.Base.gstrUserID;

            dtpPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString().Substring(0, 10);
            dtpPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search("");
        }

        private void Search(string strPoNo)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {

				dt.Clear();

                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    var div = "";
                    if (rdoStatus_1.Checked == true) div = "S2";
                    else if (rdoStatus_2.Checked == true) div = "S1";

                    string strQuery = "usp_SCM007";
                    strQuery += " @pTYPE = '" + div + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "'";
                    strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "'";
                    strQuery += ", @pDELIVERY_DT_FR = '" + dtpScmDeliveryDtFr.Text + "'";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpScmDeliveryDtTo.Text + "'";
                    strQuery += ", @pSCM_DELIVERY_DT_FR = '" + dtpScmDeliveryDtFr.Text + "'";
                    strQuery += ", @pSCM_DELIVERY_DT_TO = '" + dtpScmDeliveryDtTo.Text + "'";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

					if (fpSpread1.Sheets[0].Rows.Count > 0)
					{
						for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
						{
							// Lot 추적 품목에 따른 Locking 처리
							if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text) == true) // 구매 처리
							{
								if (string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "True", true) == 0)
								{
									UIForm.FPMake.grdReMake(fpSpread1, i,
										SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|0"
										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|0");
								}
								else
								{
									UIForm.FPMake.grdReMake(fpSpread1, i,
										SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"
										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|3");
								}
							}
							else // 외주 위탁가공 처리
							{
								UIForm.FPMake.grdReMake(fpSpread1, i,
										SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3"
										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|3");
							}

                            // 2016.11.24. hma 추가(Start): 지체일수가 0보다 큰 경우 빨간색 글자로 표기되도록 함.
                            if (Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지체일수")].Value) > 0)
                            {
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지체일수")].ForeColor = Color.Red;
                            }
                            // 2016.11.24. hma 추가(End)

                        }
                    }

                }
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

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            decimal retVal = 0;
            int iVal = 0;
            //행수만큼 처리
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                    {
                        retVal += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고단가")].Value);
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Text == ""
                        || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고일자")].Text == "")
                        { iVal++; }
                    }
                }
            }
            decimal retAmt = Math.Round(retVal * 100, 2) / 100;

            if (MessageBox.Show("제출금액은 " + String.Format("{0:###,###,###,###,###,###,###.00}", retAmt) + " 입니다. 제출하시겠습니까?", "제출금액확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Save();
            }
        }

        private void Save()
        { 
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (true)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                string Msg = string.Empty;
				string strReorgId = string.Empty;
				string strReturn_SCM_MVMT_NO = string.Empty;
				string strItemCD = string.Empty;
				string strProjectNo = string.Empty;
				string strBAR_CODE = string.Empty;
				string strLotNo = string.Empty;
				bool bCUDR = true;

				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {                    

                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";
                        if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                        {
                            strGbn = "U1";

                            string strQuery = " usp_SCM007";
                            strQuery += " @pTYPE = '" + strGbn + "'";
							strQuery += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text + "'";
							strQuery += ", @pPO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text + "'";
							strQuery += ", @pSUPPLY_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value + "'";
							strQuery += ", @pSUPPLY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고일자")].Text + "'";
							strQuery += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM비고")].Text + "'";
                            strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery += ", @pSCM_MVMT_NO = '" + Msg + "'";
                            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
                            Msg = ds.Tables[0].Rows[0][2].ToString();
							strReturn_SCM_MVMT_NO = ds.Tables[0].Rows[0][2].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

							if (string.IsNullOrEmpty(strReturn_SCM_MVMT_NO) == false)
							{
								#region lot 저장

								// 먼저 모든 바코드를 자동생성한 후 프로젝트, 품목, lot번호 별로 같은 바코드로 묶어 주는 작업을 해준다.

								// Lot 수기 등록 저장
								if (
									fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True" &&
									string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text.Replace(" ", ""), "Lot분할", true) != 0
								   )
								{
									bCUDR = false;

									strQuery = string.Empty;

									strQuery = "usp_T_SCM_IN_INFO_CUDR ";
									strQuery += "  @pTYPE        = 'I2'";
									strQuery += ", @pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
									strQuery += ", @pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
									strQuery += ", @pSCM_MVMT_NO = '" + strReturn_SCM_MVMT_NO + "' ";
									strQuery += ", @pPO_NO       = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text + "' ";
									strQuery += ", @pPO_SEQ      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text + "' ";
									strQuery += ", @pITEM_CD     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
									strQuery += ", @pTR_TYPE     = 'I' ";
									strQuery += ", @pIN_DATE     = '' ";

									if (rdoStatus_2.Checked == true) // 외주
									{
										strQuery += ", @pBAR_CODE      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
										strQuery += ", @pLOT_NO      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
									}
									else // 구매
									{
										strQuery += ", @pBAR_CODE    = '' ";
										strQuery += ", @pLOT_NO      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
									}

									strQuery += ", @pPROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
									strQuery += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' ";
									strQuery += ", @pRCPT_QTY    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value + "' ";
									strQuery += ", @pDEFECT_QTY  = 0 ";
									strQuery += ", @pIN_TRAN_NO  = '' ";
									strQuery += ", @pIN_TRAN_SEQ = NULL ";
									strQuery += ", @pIN_TRAN_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value + "' ";
									strQuery += ", @pSTOCK_QTY   = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value + "' ";
									strQuery += ", @pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
									strQuery += ", @pEND_YN      = '' ";
									strQuery += ", @pREMARK      = '' ";
									strQuery += ", @pATT_DOC_CDS = '' ";
									strQuery += ", @WORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Value + "' ";
									strQuery += ", @PROC_SEQ	 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순번")].Text + "' ";
									strQuery += ", @pUSER_ID     = '" + SystemBase.Base.gstrUserID + "' ";

									DataSet ds2 = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
									ERRCode = ds2.Tables[0].Rows[0][0].ToString();
									MSGCode = ds2.Tables[0].Rows[0][1].ToString();
									if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
									strLotNo = ds2.Tables[0].Rows[0][2].ToString();
									strBAR_CODE = ds2.Tables[0].Rows[0][3].ToString();
									
								}

								// Lot 분할 저장 
								if (dt.Rows.Count > 0)
								{

									bCUDR = false;

									for (int j = 0; j <= dt.Rows.Count - 1; j++)
									{

										if (
											string.Compare(dt.Rows[j]["SingleYN"].ToString(), "N", true) == 0 &&
											(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text == dt.Rows[j]["PO_NO"].ToString()) &&
											(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text == dt.Rows[j]["PO_SEQ"].ToString())
										   )
										{
											strQuery = string.Empty;

											strQuery = " usp_T_SCM_IN_INFO_CUDR ";
											strQuery += " @pTYPE        = 'I2' ";
											strQuery += ",@pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
											strQuery += ",@pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
											strQuery += ",@pSCM_MVMT_NO = '" + strReturn_SCM_MVMT_NO + "' ";
											strQuery += ",@pPO_NO       = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text + "' ";
											strQuery += ",@pPO_SEQ      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text + "' ";
											strQuery += ",@pITEM_CD     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
											strQuery += ",@pTR_TYPE     = 'I' ";
											strQuery += ",@pIN_DATE     = NULL ";

											if (rdoStatus_2.Checked == true) // 외주
											{
												strQuery += ",@pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
												strQuery += ",@pLOT_NO      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
											}
											else // 구매
											{
												strQuery += ",@pBAR_CODE    = '' ";
												strQuery += ",@pLOT_NO      = '" + dt.Rows[j]["LOT_NO"].ToString() + "' ";
											}
											
											strQuery += ",@pPROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
											strQuery += ",@pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' ";
											strQuery += ",@pRCPT_QTY    = " + dt.Rows[j]["RCPT_QTY"];
											strQuery += ",@pDEFECT_QTY  = 0 ";
											strQuery += ",@pIN_TRAN_NO  = '' ";
											strQuery += ",@pIN_TRAN_SEQ = NULL ";
											strQuery += ",@pIN_TRAN_QTY = " + dt.Rows[j]["RCPT_QTY"];
											strQuery += ",@pSTOCK_QTY   = " + dt.Rows[j]["RCPT_QTY"];
											strQuery += ",@pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
											strQuery += ",@pEND_YN      = '' ";
											strQuery += ",@pREMARK      = '' ";
											strQuery += ",@pATT_DOC_CDS = '' ";
											strQuery += ",@WORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Value + "' ";
											strQuery += ",@PROC_SEQ		= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순번")].Text + "' ";
											strQuery += ",@pUSER_ID     = '" + SystemBase.Base.gstrUserID + "' ";

											DataSet ds5 = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
											ERRCode = ds5.Tables[0].Rows[0][0].ToString();
											MSGCode = ds5.Tables[0].Rows[0][1].ToString();
											if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
											strLotNo = ds5.Tables[0].Rows[0][2].ToString();
											strBAR_CODE = ds5.Tables[0].Rows[0][3].ToString();
										}
									}
								}
								#endregion
							}
							else
							{
								ERRCode = "ER";
								MSGCode = "반환된 SCM 출고 번호가 없습니다.";
								Trans.Rollback();
								goto Exit;
							}
							

						}
                    }

					#region 프로젝트, 품목, lot번호 별로 같은 바코드로 묶어 준다.

					if (string.IsNullOrEmpty(strReturn_SCM_MVMT_NO) == false && bCUDR == false)
					{
						string strBarCD = string.Empty;
						string strSCM_MvmtNo = string.Empty;
						string strPONo = string.Empty;
						string strPOSeq = string.Empty;
						string strPreBarCD = string.Empty;
						string strPreLotNo = string.Empty;

						string strQry = "SELECT BAR_CODE, SCM_MVMT_NO, PO_NO, PO_SEQ, ITEM_CD, PROJECT_NO, LOT_NO FROM T_SCM_IN_INFO(NOLOCK) ";
						strQry += "WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND SCM_MVMT_NO = '" + strReturn_SCM_MVMT_NO + "' ";
						strQry += "ORDER BY PROJECT_NO, ITEM_CD, LOT_NO, BAR_CODE";
						DataTable dtResult = SystemBase.DbOpen.NoTranDataTable(strQry);

						strItemCD = string.Empty;
						strProjectNo = string.Empty;
						strLotNo = string.Empty;
						strBarCD = string.Empty;
						strPreBarCD = string.Empty;
						strPreLotNo = string.Empty;

						if (dtResult.Rows.Count > 0)
						{
							for (int m = 0; m <= dtResult.Rows.Count - 1; m++)
							{

								if (m == 0)
								{
									strProjectNo = dtResult.Rows[m]["PROJECT_NO"].ToString();
									strItemCD = dtResult.Rows[m]["ITEM_CD"].ToString();
									strLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
									strPreBarCD = dtResult.Rows[m]["BAR_CODE"].ToString();
									strPreLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
								}

								if (
									string.Compare(dtResult.Rows[m]["PROJECT_NO"].ToString(), strProjectNo, true) == 0 &&
									string.Compare(dtResult.Rows[m]["ITEM_CD"].ToString(), strItemCD, true) == 0
								   )
								{

									if (
										string.Compare(dtResult.Rows[m]["LOT_NO"].ToString(), strPreLotNo, true) == 0 ||
										string.Compare(dtResult.Rows[m]["SCM_MVMT_NO"].ToString(), dtResult.Rows[m]["LOT_NO"].ToString(), true) == 0
									   )
									{
										strBarCD = dtResult.Rows[m]["BAR_CODE"].ToString();
										strSCM_MvmtNo = dtResult.Rows[m]["SCM_MVMT_NO"].ToString();
										strPONo = dtResult.Rows[m]["PO_NO"].ToString();
										strPOSeq = dtResult.Rows[m]["PO_SEQ"].ToString();											

										string strQuery = string.Empty;

										strQuery = " usp_T_SCM_IN_INFO_CUDR ";
										strQuery += " @pTYPE		 = 'U1' ";
										strQuery += ",@pCO_CD		 = '" + SystemBase.Base.gstrCOMCD + "' ";
										strQuery += ",@pPLANT_CD	 = '" + SystemBase.Base.gstrPLANT_CD + "' ";
										strQuery += ",@pBAR_CODE	 = '" + strBarCD + "' ";
										strQuery += ",@pPRE_BAR_CODE = '" + strPreBarCD + "' ";
										strQuery += ",@pPRE_LOT_NO	 = '" + strPreLotNo + "' ";
										strQuery += ",@pSCM_MVMT_NO	 = '" + strSCM_MvmtNo + "' ";
										strQuery += ",@pPO_NO		 = '" + strPONo + "' ";
										strQuery += ",@pPO_SEQ		 = '" + strPOSeq + "' ";
										strQuery += ",@pUSER_ID		 = '" + SystemBase.Base.gstrUserID + "' ";

										DataSet dsFinal = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
										ERRCode = dsFinal.Tables[0].Rows[0][0].ToString();
										MSGCode = dsFinal.Tables[0].Rows[0][1].ToString();
										if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
									}
									else
									{
										strProjectNo = dtResult.Rows[m]["PROJECT_NO"].ToString();
										strItemCD = dtResult.Rows[m]["ITEM_CD"].ToString();
										strLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
										strPreBarCD = dtResult.Rows[m]["BAR_CODE"].ToString();
										strPreLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
									}
								}
								else
								{
									strProjectNo = dtResult.Rows[m]["PROJECT_NO"].ToString();
									strItemCD = dtResult.Rows[m]["ITEM_CD"].ToString();
									strLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
									strPreBarCD = dtResult.Rows[m]["BAR_CODE"].ToString();
									strPreLotNo = dtResult.Rows[m]["LOT_NO"].ToString();
								}
							}
						}

						bCUDR = true;
					}

					#endregion

                    Trans.Commit();
					dt.Clear();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strReorgId); //그리드 위치를 가져온다                    
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

			dt.Clear();
			bMsgYN = true;
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

		#region 팝업 조회
		//거래처 팝업
		private void btnCustCd_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = " usp_SCM007 @pTYPE = 'P2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCUST_CD", "@pCUST_NM" };
				string[] strSearch = new string[] { txtCustCd.Text, txtCustNm.Text };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM001P2", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "거래처조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtCustCd.Text = Msgs[0].ToString();
					txtCustNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}

		//품목
		private void btnItemCd_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = " usp_SCM007 @pTYPE = 'P3' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCUST_CD='" + txtCustCd.Text + "'";
				string[] strWhere = new string[] { "@pITEM_CD" };
				string[] strSearch = new string[] { txtItemCd.Text };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM003P2", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtItemCd.Text = Msgs[0].ToString();
					txtItemNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}

		//프로젝트번호 조회 팝업
		private void btnProjectNo_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_SCM007 @pTYPE = 'P1' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCUST_CD='" + txtCustCd.Text + "'";
				string[] strWhere = new string[] { "@pPROJECT_NO" };
				string[] strSearch = new string[] { txtProjectNo.Text };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM001P3", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtProjectNo.Text = Msgs[0].ToString();
					txtProjectNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

		#region 조회조건 TextChanged
		//거래처
		private void txtCustCd_TextChanged(object sender, EventArgs e)
		{
			txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
		//품목
		private void txtItemCd_TextChanged(object sender, EventArgs e)
		{
			txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
		//프로젝트번호
		private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
		{
			txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

        #region Grid Button Click
        protected override void fpButtonClick(int Row, int Column)
        {

			decimal dSum = 0;

            try
            {

                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙_2"))
                {
                    string strQuery = " usp_SCM007 @pTYPE = 'P4' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                    string[] strWhere = new string[] { "@pPO_NO", "@pPO_SEQ" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text, fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM002P1", strQuery, strWhere, strSearch, new int[] { 3, 4 }, "품질증빙");
                    pu.ShowDialog();
                }

				if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2"))
				{

					SCM007P1 scm007p1 = new SCM007P1();
					scm007p1.strPO_NO = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text.ToString();
					scm007p1.strPO_SEQ = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text.ToString();
					scm007p1.strSCM_MVMT_NO = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM 입고번호")].Text.ToString();
					scm007p1.bSave = bSaveLot;
					scm007p1.dtM1 = dt;

					scm007p1.ShowDialog();

					if (scm007p1.DialogResult == DialogResult.OK)
					{

						// lot 팝업에서 수정된 내용이 있다면 현재 po와 관계된 기존 내용을 삭제한 후 새로운 내용으로 대치한다.
						if (string.Compare(scm007p1.strSaveYN, "Y", true) == 0)
						{
							for (int j = dt.Rows.Count - 1; j >= 0; j--)
							{
								if (
									(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text == dt.Rows[j]["PO_NO"].ToString()) &&
									(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text == dt.Rows[j]["PO_SEQ"].ToString())
								   )
								{
									dt.Rows.RemoveAt(j);
								}
							}
						}

						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "";
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = 0;

						if (dt.Rows.Count > 0)
						{
							for (int i = 0; i <= scm007p1.dt.Rows.Count - 1; i++)
							{
								DataRow dr = scm007p1.dt.Rows[i];
								dt.Rows.Add(dr.ItemArray);
							}
						}
						else
						{
							dt = scm007p1.dt;
						}

						if (scm007p1.dt.Rows.Count > 1)
						{

							for (int i = 0; i <= scm007p1.dt.Rows.Count - 1; i++)
							{
								if (scm007p1.dt.Rows[0]["RCPT_QTY"] == DBNull.Value) scm007p1.dt.Rows[0]["RCPT_QTY"] = 0;
								dSum += Convert.ToDecimal(scm007p1.dt.Rows[i]["RCPT_QTY"]);
							}

							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "Lot 분할";
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = dSum;

							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Locked = true;
						}
						else if (scm007p1.dt.Rows.Count == 1)
						{
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = scm007p1.dt.Rows[0]["LOT_NO"].ToString();
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = scm007p1.dt.Rows[0]["RCPT_QTY"].ToString();

							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Locked = true;
						}

						Set_Amt(fpSpread1.Sheets[0].ActiveRowIndex);

					}

					scm007p1.strSaveYN = string.Empty;
					scm007p1.dLotSum = 0;
					bMsgYN = true;
				}

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show("데이터 조회 중 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

		#region 금액계산
		private void Set_Amt(int Row)
		{
			decimal Amt = 0;
			decimal LocAmt = 0;
			decimal Price = 0;
			decimal Qty = 0;
			decimal Xch_rate = 0;

			if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Text.Trim() != "")
				Qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value);
			if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고단가")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고단가")].Text.Trim() != "")
				Price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고단가")].Value);
			if (Price != 0 && Qty != 0)
			{
				Amt = Price * Qty;
				LocAmt = Amt * Xch_rate;
				fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = Amt;

			}

		}
		#endregion

		#region Colimn Header Check Click
		private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{
			try
			{
				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
					{
						if (e.ColumnHeader == true && e.Column == 1)
						{
							for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
							{
								fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
							}
						}
					}
				}
			}
			catch (Exception f)
			{
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 출고잔량 체크
		private void fpSpread1_EditModeOff(object sender, EventArgs e)
		{
			try
			{
				if (
					Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value) >
					Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "잔량")].Value)
				   )
				{
					MessageBox.Show("출고수량은 잔량을 초과할 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value =
					Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "잔량")].Value);
					return;
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show("데이터 처리 중 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

	}
}
