#region 작성정보
/*********************************************************************/
// 단위업무명 : CLAIM 관리
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-05-22
// 작성내용 : CLAIM 관리
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
using WNDW;

namespace SCM.SCM014
{
    public partial class SCM014 : UIForm.FPCOMM1
    {
        public SCM014()
        {
            InitializeComponent();
        }

        #region 폼 로드시
        private void SCM014_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtCustCd.Value = SystemBase.Base.gstrUserID;
            txtSupplyDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString();
            txtSupplyDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            if (SystemBase.Base.gstrUserID != "KO132")
            {
                txtCustCd.Tag = ";2;;";
                btnCustCd.Tag = ";2;;";
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //조회조건 초기화
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region 조회조건 텍스트 변경
        //거래처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 조회조건 팝업
        //거래처
        private void btnCustCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM014 @pTYPE = 'P2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCUST_CD", "@pCUST_NM" };
                string[] strSearch = new string[] { txtCustCd.Text, "" };

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
        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM012 @pTYPE = 'P3' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pCUST_CD='" + txtCustCd.Text + "'";
                string[] strWhere = new string[] { "@pPROJECT_NO", "@pPROJECT_NM" };
                string[] strSearch = new string[] { txtProjectNo.Text, "" };

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

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string Cfm = "";
                if (rdoCfmStatus_Y.Checked == true) Cfm = "Y";
                if (rdoCfmStatus_N.Checked == true) Cfm = "N";

                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_SCM014  @pTYPE = 'S1'";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "' ";
                    strQuery += ", @pSUPPLY_DT_FR = '" + txtSupplyDtFr.Text + "' ";
                    strQuery += ", @pSUPPLY_DT_TO = '" + txtSupplyDtTo.Text + "' ";
                    strQuery += ", @pPO_DT_FR = '" + txtPoDtFr.Text + "' ";
                    strQuery += ", @pPO_DT_TO = '" + txtPoDtTo.Text + "' ";
                    strQuery += ", @pINSP_DT_FR = '" + txtInspDtFr.Text + "' ";
                    strQuery += ", @pINSP_DT_TO = '" + txtInspDtTo.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pCFM_STATUS  = '" + Cfm + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            fpSpread1.Focus();

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strITEMCD = "";

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
                                    case "I": strGbn = "I1"; break;
                                    case "D": strGbn = "D1"; break;
                                    default: strGbn = ""; break;
                                }

                                string strSql = " usp_SCM014 '" + strGbn + "'";

                                strITEMCD = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                                strSql += ", @pCLAIM_SCM_REMARK1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "업체비고1")].Text + "'";
                                strSql += ", @pCLAIM_SCM_REMARK2 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "업체비고2")].Text + "'";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pMVMT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매입고번호")].Text + "'";
                                strSql += ", @pMVMT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구매입고순번")].Text + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프					
                            }
                        }
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = e.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strITEMCD);
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Cursor = Cursors.Default;
            }
        }
        #endregion


    }
}
