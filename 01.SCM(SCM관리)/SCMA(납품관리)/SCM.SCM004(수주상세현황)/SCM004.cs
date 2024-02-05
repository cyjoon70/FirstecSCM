#region 작성정보
/*********************************************************************/
// 단위업무명:
// 작 성 자 : 
// 작 성 일 : 
// 작성내용 : 
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
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using WNDW;

namespace SCM.SCM004
{
    public partial class SCM004 : UIForm.FPCOMM1
    {
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;

        public SCM004()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void SCM004_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting


            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            txtPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-3).ToShortDateString();
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
            rdoUnDeliveryCfm_All.Checked = true;
            rdoScmAcceptYn_All.Checked = true;
            optCloseYN.Checked = true;              // 2016.08.17. hma 추가: 초기화시 마감여부 '전체'로 선택되도록 함.
            radioButton1.Checked = true;
            txtCustCd.Text = SystemBase.Base.gstrUserID;
            if (SystemBase.Base.gstrUserID == "KO132")
                btnCustCd.Enabled = true;
            else
                btnCustCd.Enabled = false;
        }
        #endregion

        #region 팝업창 열기
        //거래처
        private void btnCustCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM004 @pTYPE = 'P3' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                string strQuery = " usp_SCM004 @pTYPE = 'P4' , @pCUST_CD='" + txtCustCd.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                string strQuery = " usp_SCM004 @pTYPE = 'P2' , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCUST_CD='" + txtCustCd.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pITEM_CD", "@pITEM_NM" };
                string[] strSearch = new string[] { txtItemCd.Text, txtItemNm.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM003P2", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목코드 조회");
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
        private void txtUserId_TextChanged(object sender, System.EventArgs e)
        {
           txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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
                    if (rdoUnDeliveryCfm_Y.Checked == true)
                        Cfm = "N";
                    else if (rdoUnDeliveryCfm_N.Checked == true)
                        Cfm = "Y";

                    string DefectCfm = "";
                    if (chkConfirm.Checked == true)
                        DefectCfm = "Y";
                    else
                        DefectCfm = "N";

                    string div = "";
                    if (rdoStatus_1.Checked == true)        // 접수구분을 '구매'로 선택한 경우
                        div = "S1";
                    else if (rdoStatus_2.Checked == true)   // 접수구분을 '외주'로 선택한 경우
                        div = "S2";
                    else
                        div = "S3";                         // 접수구분을 '전체'로 선택한 경우

                    string AcceptY = "";
                    if (rdoScmAcceptYn_N.Checked == true)
                        AcceptY = "N";
                    else if (rdoScmAcceptYn_Y.Checked == true)
                        AcceptY = "Y";

                    string strQuery = " usp_SCM004 @pTYPE= '" + div + "'";
                    strQuery += ", @pPO_DT_FR = '" + txtPoDtFr.Text + "'";
                    strQuery += ", @pPO_DT_TO = '" + txtPoDtTo.Text + "'";
                    // 2020.02.12. hma 수정(Start): 납기일 검색조건을 변경납기일로 변경하여 매개변수도 변경함.
                    //strQuery += ", @pDELIVERY_DT_FR = '" + txtDeliveryDtFr.Text + "'";
                    //strQuery += ", @pDELIVERY_DT_TO = '" + txtDeliveryDtTo.Text + "'";
                    strQuery += ", @pDELIVERY_DT_REF_FR = '" + txtDeliveryDtFr.Text + "'";
                    strQuery += ", @pDELIVERY_DT_REF_TO = '" + txtDeliveryDtTo.Text + "'";
                    // 2020.02.12. hma 수정(End)
                    strQuery += ", @pUNDELIVERY_YN = '" + Cfm + "'";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pDEFECT_CFM = '" + DefectCfm + "'";
                    strQuery += ", @pCUST_CD= '" + txtCustCd.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pSCM_ACCEPT_YN= '" + AcceptY + "'";
                    if (txtUserId.Text != "")
                        strQuery += ", @pPUR_DUTY= '" + txtUserId.Text + "'";

                    // 2016.08.17. hma 추가(Start): 발주 마감여부 검색조건 화면에 추가하고 조회시 선택된 조건에 해당되는 데이터만 나오도록 함.
                    string strPOCloseYN = "";
                    if (optCloseY.Checked == true)
                        strPOCloseYN = "Y";
                    else if (optCloseN.Checked == true)
                        strPOCloseYN = "N";
                    strQuery += ", @pPO_CLOSE_YN = '" + strPOCloseYN + "'"; 
                    // 2016.08.17. hma 추가(End)

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, true, false);

                    for (int i = 0; i < fpSpread1.ActiveSheet.RowCount - 1; i++)
                    {
                        // 2016.10.17. hma 수정(Start): 납기일자와 변경납기일자가 다른 경우 변경납기일자를 붉은색 글자로 표시되도록 함.
                        //if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수정납기일자")].Text != "")
                        //    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수정납기일자")].ForeColor = Color.Red;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수정납기일자")].Text !=
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자")].Text)
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수정납기일자")].ForeColor = Color.Red;
                        }
                        // 2016.10.17. hma 추가(End)
                        
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

        private void butUser_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Value = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

    }
}
