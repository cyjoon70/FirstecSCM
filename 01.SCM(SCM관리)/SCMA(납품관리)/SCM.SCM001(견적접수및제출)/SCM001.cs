#region 작성정보
/*********************************************************************/
// 단위업무명 : 견적접수및제출
// 작 성 자 : 김현근
// 작 성 일 : 2013-05-15
// 작성내용 : 견적접수및제출
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;


namespace SCM.SCM001
{
    public partial class SCM001 : UIForm.FPCOMM1 
    {
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        int btnR=0;
        string sm_ChangeEventRtn = "";
        public SCM001()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void SCM001_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting

            //GropBox1 조회조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboScmDeliveryType, "usp_B_COMMON @pTYPE = 'COMM2' , @pCODE = 'M017', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //영업담당

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            dtpEstReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpEstReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            txtCustCd.Text = SystemBase.Base.gstrUserID;
            if (SystemBase.Base.gstrUserID == "KO132")
            {
                btnCustCd.Enabled = true;
                txtCustCd.ReadOnly = false;
                txtCustCd.BackColor = System.Drawing.Color.FromArgb(242, 252, 254);	// 필수 입력     
            }
            else
            {
                btnCustCd.Enabled = false;
                txtCustCd.ReadOnly = true;
                txtCustCd.BackColor = System.Drawing.Color.FromArgb(239, 239, 239);   // 읽기전용
            }

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Base.GroupBoxReset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            dtpEstReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpEstReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoEstStatus_1.Checked = true;

            txtCustCd.Text = SystemBase.Base.gstrUserID;
            if (SystemBase.Base.gstrUserID == "KO132")
                btnCustCd.Enabled = true;
            else
                btnCustCd.Enabled = false;
        }
        #endregion

        #region 팝업창 열기
        private void btnCustCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM001 @pTYPE = 'P2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
        private void btnProjectNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM001 @pTYPE = 'P3' , @pCUST_CD='" + txtCustCd.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pPROJECT_NO", "@pPROJECT_NM" };
                string[] strSearch = new string[] { txtProjectNo.Text, txtProjectNm.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM001P3", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트조회");
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 텍스트박스 변환시 
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
        }

        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string Cfm = "";
                    if (rdoEstStatus_1.Checked == true)
                        Cfm = "1";
                    else if (rdoEstStatus_2.Checked == true)
                        Cfm = "2";
                    else if (rdoEstStatus_3.Checked == true)
                        Cfm = "3";

                    string strQuery = " usp_SCM001 @pTYPE='S1' ";
                    strQuery += ", @pEST_REQ_DT_FR ='" + dtpEstReqDtFr.Text + "'";
                    strQuery += ", @pEST_REQ_DT_TO ='" + dtpEstReqDtTo.Text + "'";
                    strQuery += ", @pEST_STATUS ='" + Cfm + "'";
                    strQuery += ", @pSCM_DT_FR ='" + dtpScmDtFr.Text + "'";
                    strQuery += ", @pSCM_DT_TO ='" + dtpScmDtTo.Text + "'";
                    strQuery += ", @pCUST_DUTY_NM ='" + txtCustDutyNm.Text + "'";
                    strQuery += ", @pEST_REQ_DELIVERY_DT_FR ='" + dtpEstReqDeliveryDtFr.Text + "'";
                    strQuery += ", @pEST_REQ_DELIVERY_DT_TO ='" + dtpEstReqDeliveryDtTo.Text + "'";
                    strQuery += ", @pITEM_NM ='" + txtItemNm.Text + "'";
                    strQuery += ", @pEST_NO = '" + txtEstNo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pREQ_NO = '" + txtReqNo.Text + "'";
                    strQuery += ", @pCUST_CD ='" + txtCustCd.Text + "'";  
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    Set_Lock();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }

        //견적의뢰상태에 따른 Locking
        private void Set_Lock()
        {            
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                string EstStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적의뢰상태")].Text;
                if (EstStatus == "1")
                { 
                    //2|3#3|3#4|3#5|3#6|3#7|3#8|3#10|3#17|3#18|3
                     UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "제출") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출일자") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출수량") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출금액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "가능일") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "SCM납품가능형태") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출비고") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "담당자") + "|3"
                        );
                }
                else if (EstStatus == "2")
                {
                    //"2|0#3|0#4|0#5|0#6|0#7|0#8|0#10|0#17|0#18|0");
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "제출") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출일자") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출수량") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출금액") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "가능일") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "SCM납품가능형태") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출비고") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "담당자") + "|0"
                        );
                }
                else if (EstStatus == "3")
                {
                    //"1|3#2|1#3|1#4|1#5|1#6|1#7|1#8|1#10|0#17|0#18|0");
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "접수") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출일자") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출수량") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출금액") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "가능일") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "SCM납품가능형태") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출비고") + "|0"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "담당자") + "|0"
                        );
                }
                else
                {
                    //"1|3#2|3#3|3#4|3#5|3#6|3#7|3#8|3#10|3#17|3#18|3");
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "접수") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출일자") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출수량") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제출금액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "가능일") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "SCM납품가능형태") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출비고") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "담당자") + "|3"
                        );
                }
            }
        }

        // 합계 그리드 재정의
        private void Set_Section()
        {
            int iCnt = fpSpread1.Sheets[0].RowCount;

            //합계 컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text == "합계")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].ColumnSpan = 2;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;

                    for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor1;
                    }
                }
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //Major 코드 필수항목 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))// 그리드 필수항목 체크 
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

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
                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                case "D": strGbn = "D1"; break;
                                case "I": strGbn = "I1"; break;
                                default: strGbn = ""; break;
                            }

                             string strEstStatus = "";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "1" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "True")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출")].Text == "1" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출")].Text == "True")
                                    strEstStatus = "3";
                                else
                                    strEstStatus = "2";
                            }
                            else
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출")].Text == "1" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출")].Text == "True")
                                    strEstStatus = "3";
                                else
                                    strEstStatus = "1";
                            }

                            string strScmDt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출일자")].Text;
                            string strCurrency = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text;
                            string strScmPrice = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출단가")].Text;
                            string strScmQty = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출수량")].Text;
                            string strScmAmt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제출금액")].Text;
                            string strScmDeliveryDt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납품가능일자")].Text;
                            string strScmDeliveryType = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM납품가능형태")].Text;
                            
                            string strScmRemark = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적제출비고")].Text;
                            string strcustDutyNm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자")].Text;
                            string strScmMoqYn = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MOQ 여부")].Text;
                            string strScmMoqQty = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MOQ 요구잔량")].Text;
                            string strEstNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적번호")].Text;
                            string strEstSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text;
                            string strScmProof = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매요청번호")].Text;
                           
                            string strSql = " usp_SCM001 '" + strGbn + "'";
                            strSql = strSql + ", @pEST_STATUS = '" + strEstStatus + "'";

                            if(strScmDt == "")
                                strSql = strSql + ", @pSCM_DT = null";
                            else
                                strSql = strSql + ", @pSCM_DT = '" + strScmDt + "'";

                            strSql = strSql + ", @pCURRENCY = '" + strCurrency + "'";

                            if (strScmPrice == "")
                              strSql = strSql + ", @pSCM_PRICE =0";
                            else
                                strSql = strSql + ", @pSCM_PRICE =" + strScmPrice;

                            if (strScmQty == "")
                                strSql = strSql + ", @pSCM_QTY = 0";
                            else
                                strSql = strSql + ", @pSCM_QTY = " + strScmQty + "";

                            if (strScmAmt == "")
                                strSql = strSql + ", @pSCM_AMT = 0";
                            else
                                strSql = strSql + ", @pSCM_AMT = " + strScmAmt;

                            if (strScmDeliveryDt == "")
                                strSql = strSql + ", @pSCM_DELIVERY_DT = null";
                            else
                                strSql = strSql + ", @pSCM_DELIVERY_DT = '" + strScmDeliveryDt + "'";

                            strSql = strSql + ", @pSCM_DELIVERY_TYPE = '" + strScmDeliveryType + "'";

                            if (strScmMoqYn == "1" || strScmMoqYn == "True")
                                strSql = strSql + ", @pSCM_MOQ_YN  = 'Y'";
                            else
                                strSql = strSql + ", @pSCM_MOQ_YN = 'N'";

                            if (strScmMoqQty == "")
                                strSql = strSql + ", @pSCM_REQ_MOQ_QTY = 0";
                            else
                                strSql = strSql + ", @pSCM_REQ_MOQ_QTY = " + strScmMoqQty;

                            strSql = strSql + ", @pSCM_REMARK = '" + strScmRemark + "'";
                            strSql = strSql + ", @pCUST_DUTY_NM = '" + strcustDutyNm + "'";
                            strSql = strSql + ", @pEST_NO = '" + strEstNo + "'";
                            strSql = strSql + ", @pEST_SEQ = '" + strEstSeq + "'";
                            strSql = strSql + ", @pSCM_QUALITY_PROOF   = '" + strScmProof + "'";
                            strSql = strSql + ", @pCUST_CD = '" + txtCustCd.Text + "'";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }
        #endregion

        #region 버튼클릭
        //가능일 일괄적용
        private void btnDeliveryDtSelect_Click(object sender, EventArgs e)
        {
            AllSelect("1");
        }

        //구매담당 일괄적용
        private void btnPurDuty_Click(object sender, EventArgs e)
        {
            AllSelect("2");
        }

        //SCM납품가능형태 일괄적용
        private void btnScmDeliveryType_Click(object sender, EventArgs e)
        {
            AllSelect("3");
        }

        private void AllSelect(string param)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "1" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "True")
                {
                    btnR = i;
                    if (param == "1")
                    {
                        if (txtDt.Text != "")
                        {
                            SMDblClickRtnInsData(txtDt.Text, "8", "0", 0);
                        }
                        else
                        {
                            SMDblClickRtnInsData("", "8", "0", 0);
                        }
                        SM_CGEvent();
                    }
                    else if (param == "2")
                    {
                        if (txtPurDuty.Text != "")
                        {
                            SMDblClickRtnInsData(txtPurDuty.Text, "18", "0", 0);
                        }
                        else
                        {
                            SMDblClickRtnInsData("", "18", "0", 0);
                        }
                    }
                    else if (param == "3")
                    {
                        if (cboScmDeliveryType.SelectedValue.ToString() != "")
                        {
                            SMDblClickRtnInsData(cboScmDeliveryType.SelectedValue.ToString(), "10", "0", 0);
                        }
                        else
                        {
                            SMDblClickRtnInsData("", "10", "0", 0);
                        }
                    }
                }
            }
        }

        private void SMDblClickRtnInsData(string Msg, string MsgXY, string RtnXY, int Focus)
        {	//팝업 떠블클릭 리턴 메세지 Return
            try
            {

                string MSG = Msg.Replace("|", "#");
                string MSGXY = MsgXY.Replace("|", "#");
                string RTNXY = RtnXY.Replace("|", "#");

                Regex rx1 = new Regex("#");
                Regex rx2 = new Regex("#");
                Regex rx3 = new Regex("#");

                string[] Msgs = rx1.Split(MSG);
                string[] MsgXYs = rx2.Split(MSGXY);
                string[] RtnXYs = rx3.Split(RTNXY);

                for (int i = 0; i < RtnXYs.Length; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[btnR, Convert.ToInt32(MsgXYs[i])].CellType != null && fpSpread1.Sheets[0].Cells[btnR, Convert.ToInt32(MsgXYs[i])].CellType.ToString() == "ComboBoxCellType")
                    {
                        fpSpread1.Sheets[0].Cells[btnR, Convert.ToInt32(MsgXYs[i])].Value = Msgs[Convert.ToInt32(RtnXYs[i])];
                    }
                    else
                    {

                        fpSpread1.Sheets[0].Cells[btnR, Convert.ToInt32(MsgXYs[i])].Text = Msgs[Convert.ToInt32(RtnXYs[i])];
                    }
                }

                if (Focus != 0)
                    fpSpread1.ActiveSheet.SetActiveCell(btnR, Focus);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }

       //제출금액 변환, 가능일
       private void SM_CGEvent()
       {
           string retval = "";
           string retval2 = "";
           int BtnR = btnR;
           int BtnC =0;								
				
           retval = sm_ChangeEventRtn;

               //제출단가
               if(BtnC == 5)
               {
                   retval2 = grdData(BtnR, BtnC+1); //제출수량
                   if(retval2 == "" || grdData(BtnR, BtnC) == "")
                   {
                       SMDblClickRtnInsData("", (BtnC+2).ToString(), "0", 0);
                   }
                   else
                   {
                       SMDblClickRtnInsData((int.Parse(retval2) * int.Parse(retval)).ToString(), (BtnC+2).ToString(), "0", 0);
                   }				
					
               }				
				
               //제출수량
               if(BtnC == 6)
               {			
                   retval2 =grdData(BtnR, BtnC-1);//제출단가									
					
                   if(retval2 == "" || grdData(BtnR, BtnC) == "")
                   {			
                       SMDblClickRtnInsData("", (BtnC+1).ToString(), "0", 0);
                   }
                   else
                   {
                      SMDblClickRtnInsData((int.Parse(retval2) * int.Parse(retval)).ToString(), (BtnC+1).ToString(), "0", 0);						
                   }
					
                   string EstReqQty = grdData(BtnR, 15);					
                   string Result =grdData(BtnR, BtnC);

                   if(int.Parse(Result) - int.Parse(EstReqQty) > 0)
                   {
                      SMDblClickRtnInsData("1|"+ (int.Parse(Result) - int.Parse(EstReqQty)).ToString() + "'", "22|23", "0|1", 0);
                   }
                   else
                   {
                      SMDblClickRtnInsData("0|"+ (int.Parse(Result) - int.Parse(EstReqQty)).ToString() + "'", "22|23", "0|1", 0);
                   }										
               }
				
               //가능일
               if(BtnC == 8 || BtnC == 3)
               {	
                   if(grdData(BtnR, 3) != "")
                   {	
                       string DaysToAdd = grdData(BtnR, 8);
						
                       string grdDate = grdData(BtnR, 3);
                       grdDate = grdDate.Substring(0, 10);
                       int year = int.Parse(grdDate.Substring(0,4));
                       int month = int.Parse(grdDate.Substring(5, 2));
                       int day = int.Parse(grdDate.Substring(8, 2));


                       DateTime newdate = new DateTime(year, month, day);
                       string RtnValue = "";

                       DateTime newtimems = newdate.AddDays(double.Parse(DaysToAdd));

                       year=newdate.Year;
                       if (year < 1000)
                           year+=1900;
						
                       month=newdate.Month;
                       string reMonth = "";
                       if (month<10)
                           reMonth = "0" + month.ToString();

                       day = newdate.Day;
                       string reDays = "";
                       if (day < 10)
                           reDays = "0" + day.ToString();

                       RtnValue = year.ToString() + "-" + reMonth + "-" + reDays;	
							
                       SMDblClickRtnInsData(RtnValue, "9", "0", 0);
                   }
                   else
                   {							
                       SMDblClickRtnInsData("", "9", "0", 0);
                   }
               }
       }

       private string grdData(int Row, int Colunm)
       {
           string MSG = "";

           if (fpSpread1.Sheets[0].Cells[Row, Colunm].Value != null || fpSpread1.Sheets[0].Cells[Row, Colunm].Text.ToString() != "")
           {
               MSG = fpSpread1.Sheets[0].Cells[Row, Colunm].Value.ToString();
           }

           return MSG;
       }
        #endregion

       #region 그리드 체인지
       private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
       {
           fpSpread1_ChangeEvent(e.Row, e.Column);
       }

       public void fpSpread1_ChangeEvent(int Row, int Col)
       {
           try
           {
               if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text != "I")
                   fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "U";

               if (fpSpread1.Sheets[0].Cells[Row, Col].Value != null || fpSpread1.Sheets[0].Cells[Row, Col].Text.ToString() != "")
               {
                   sm_ChangeEventRtn = fpSpread1.Sheets[0].Cells[Row, Col].Value.ToString();
               }
               else
               {
                   sm_ChangeEventRtn = "";
               }

               //if (SM_ChangeEvent != null)
               //{
               //    Type type = sm_ChangeEvent.GetType();
               //    type.InvokeMember("", BindingFlags.InvokeMethod, null, sm_ChangeEvent, null);
               //}
           }
           catch (Exception f)
           {
               MessageBox.Show("Change - 수정 플래그 등록 실패 \n\n" + f.ToString());
           }
       }

       #endregion
    }
}
