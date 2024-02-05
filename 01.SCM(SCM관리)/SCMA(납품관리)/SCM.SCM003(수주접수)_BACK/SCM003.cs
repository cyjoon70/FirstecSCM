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


namespace SCM.SCM003
{
    public partial class SCM003 : UIForm.FPCOMM1 
    {
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        int btnR=0;
        bool strChk = false;

        public SCM003()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void SCM003_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            txtPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            txtPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

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

            txtPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            txtPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoScmAcceptYn_N.Checked = true;

            txtCustCd.Text = SystemBase.Base.gstrUserID;
            if (SystemBase.Base.gstrUserID == "KO132")
                btnCustCd.Enabled = true;
            else
                btnCustCd.Enabled = false;
            strChk = false;
        }
        #endregion

        #region 팝업창 열기
        //거래처
        private void btnCustCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM003 @pTYPE = 'P3' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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

        //프로젝트
        private void btnProjectNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM003 @pTYPE = 'P4' , @pCUST_CD='" + txtCustCd.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
        //품목
        private void btnItemNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM003 @pTYPE = 'P2' , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCUST_CD='" + txtCustCd.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pITEM_CD" , "@pITEM_NM"};
                string[] strSearch = new string[] { txtItemCd.Text, txtItemNm.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM003P2", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트조회");
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
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

        //품목
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
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
                    if (rdoScmAcceptYn_N.Checked == true)
                        Cfm = "N";
                    else if (rdoScmAcceptYn_Y.Checked == true)
                        Cfm = "Y";

                    string strQuery = " usp_SCM003 @pTYPE= 'S1'";
                    strQuery += ", @pPO_DT_FR= '" + txtPoDtFr.Text + "'";
                    strQuery += ", @pPO_DT_TO= '" + txtPoDtTo.Text + "'";
                    strQuery += ", @pSCM_ACCEPT_YN= '" + Cfm + "'";
                    strQuery += ", @pDELIVERY_DT_FR= '" + txtDeliveryDtFr.Text + "'";
                    strQuery += ", @pDELIVERY_DT_TO= '" + txtDeliveryDtTo.Text + "'";
                    strQuery += ", @pSCM_DELIVERY_DT_FR= '" + txtScmDeliveryDtFr.Text + "'";
                    strQuery += ", @pSCM_DELIVERY_DT_TO= '" + txtScmDeliveryDtTo.Text + "'";
                    strQuery += ", @pITEM_CD= '" + txtItemCd.Text + "'";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pCUST_CD= '" + txtCustCd.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                    strChk = false;
                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                    {
                        //발주진행상태 와 마감여부, 출고처리여부에따른 비활성화

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주진행상태")].Text == "9" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "마감여부")].Text == "Y" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM번호")].Text != "")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수") + "|3"
                                                          + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납품예정일자") + "|3"
                                                          + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                                            );
                        }
                        else
                        {
                            //접수체크시 컬럼 필수정의
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "1" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "True")
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수") + "|1"
                                                          + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납품예정일자") + "|1"
                                                          + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                                            );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수") + "|0"
                                                          + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납품예정일자") + "|0"
                                                          + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                                            );
                            }
                        }
                    }		
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //Major 코드 필수항목 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))// 그리드 필수항목 체크 
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

                            //발주진행상태(PO_STATUS)가 마감(9)이거나 마감여부(CLOSE_YN)가 Y 이면 수정X
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주진행상태")].Text != "9" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "마감여부")].Text != "Y")
                            {
                                string strScmAcceptYn = "";
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "1" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "True") { strScmAcceptYn = "Y"; }
                                else { strScmAcceptYn = "N"; }

                                string strQuery = " usp_SCM003 '" + strGbn + "'";
                                strQuery += ", @pSCM_ACCEPT_YN = '" + strScmAcceptYn + "'";

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납품예정일자")].Text == "") { strQuery += ", @pSCM_DELIVERY_DT = null"; }
                                else { strQuery += ", @pSCM_DELIVERY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납품예정일자")].Text + "'"; }

                                strQuery += ", @pSCM_REMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                                strQuery += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text + "'";
                                strQuery += ", @pPO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text + "'";

                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "1" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "True")
                                    strQuery += ", @pSCM_QUALITY_PROOF = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙")].Text + "'";
                                else
                                    strQuery += ", @pSCM_QUALITY_PROOF = ''";

                                strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
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

        #region 그리드 헤드 체크박스 클릭시
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "접수"))
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                        {
                            if (e.ColumnHeader == true)
                            {
                                if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, e.Column].Value.ToString() == "False")
                                {
                                    fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = false;
                                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                    {
                                        if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
                                        {
                                            fpSpread1.Sheets[0].Cells[i, e.Column].Value = false;

                                            if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                                            {
                                                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                                            }

                                            GRID_READONLY(i);
                                        }
                                    }
                                }
                                else
                                {
                                    fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = true;
                                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                    {
                                        if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
                                        {
                                            fpSpread1.Sheets[0].Cells[i, e.Column].Value = true;

                                            UIForm.FPMake.fpChange(fpSpread1, i);

                                            GRID_READONLY(i);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 납품예정일자 필수및 초기화
        protected void GRID_READONLY(int Row)
        {
            try
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "1" || fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "접수")].Text == "True")
                {
                    UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "접수") + "|1"
                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납품예정일자") + "|1"
                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                                );
                }
                else
                {
                    UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "접수") + "|0"
                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납품예정일자") + "|0"
                                              + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                                );
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "납품예정일자")].Text = "";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                //접수체크시 컬럼 필수정의
                GRID_READONLY(e.Row);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

    }
}
