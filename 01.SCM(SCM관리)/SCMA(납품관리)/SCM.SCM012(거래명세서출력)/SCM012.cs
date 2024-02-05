#region 작성정보
/*********************************************************************/
// 단위업무명 : 거래명세서출력
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-05-15
// 작성내용 : 거래명세서출력 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace SCM.SCM012
{
    public partial class SCM012 : UIForm.Buttons
    {
        public SCM012()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void SCM012_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타세팅
            dtpMvmtDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString();
            dtpMvmtDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
            txtCustCd.Value = SystemBase.Base.gstrUserID;

            if (SystemBase.Base.gstrUserID != "KO132")
            {
                txtCustCd.Tag = ";2;;";
                btnCustCd.Tag = ";2;;";
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //기타세팅
            dtpMvmtDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString();
            dtpMvmtDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region 조회조건 팝업
        //프로젝트번호 팝업
        private void btnProjectNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM012 @pTYPE = 'P3' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pCUST_CD='" + txtCustCd.Text + "'";
                string[] strWhere = new string[] { "@pPROJECT_NO", "@pPROJECT_NM" };
                string[] strSearch = new string[] { txtProjectNoFr.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM001P3", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtProjectNoFr.Text = Msgs[0].ToString();
                    txtProjectNmFr.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        private void btnProjectNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM012 @pTYPE = 'P3' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pCUST_CD='" + txtCustCd.Text + "'";
                string[] strWhere = new string[] { "@pPROJECT_NO", "@pPROJECT_NM" };
                string[] strSearch = new string[] { txtProjectNoTo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM001P3", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtProjectNoTo.Text = Msgs[0].ToString();
                    txtProjectNmTo.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //거래처 팝업
        private void btnCustCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM012 @pTYPE = 'P2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
        #endregion

        #region 조회조건 TextChanged

        //프로젝트번호 From
        private void txtProjectNoFr_TextChanged(object sender, System.EventArgs e)
        {
            txtProjectNmFr.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNoFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //프로젝트번호 To
        private void txtProjectNoTo_TextChanged(object sender, System.EventArgs e)
        {
            txtProjectNmTo.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNoTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        //거래처 From
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 레포트 출력
        private void butPreview_Click(object sender, System.EventArgs e)
        {
            //조회 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string RptName = "";
                

                if (rdoStatus_1.Checked == true)
                {
                    string[] RptParmValue = new string[18];   // SP 파라메타 값
                    RptName = SystemBase.Base.ProgramWhere + @"\Report\MIM506_SCM.rpt";    // 레포트경로+레포트명
                    RptParmValue[0] = "R3";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = SystemBase.Base.gstrLangCd;
                    RptParmValue[3] = "";
                    RptParmValue[4] = txtPurDeptCd.Text;
                    RptParmValue[5] = dtpDeliveryDt.Text;
                    RptParmValue[6] = "";
                    RptParmValue[7] = dtpMvmtDtFr.Text;
                    RptParmValue[8] = dtpMvmtDtTo.Text;
                    RptParmValue[9] = txtPoNoFr.Text;
                    RptParmValue[10] = txtPoNoTo.Text;
                    RptParmValue[11] = txtCustCd.Text;
                    RptParmValue[12] = txtCustCd.Text;
                    RptParmValue[13] = txtProjectNoFr.Text;
                    RptParmValue[14] = txtProjectNoTo.Text;
                    RptParmValue[15] = "";
                    RptParmValue[16] = "";
                    RptParmValue[17] = "";

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전				
                    frm.ShowDialog();
                }
                else
                {
                    string[] RptParmValue = new string[21];   // SP 파라메타 값
                    RptName = SystemBase.Base.ProgramWhere + @"\Report\MIM520_SCM.rpt";    // 레포트경로+레포트명
                    RptParmValue[0]  = "R3";
                    RptParmValue[1]  = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2]  = SystemBase.Base.gstrLangCd;
                    RptParmValue[3]  = txtPurDeptCd.Text;
                    RptParmValue[4]  = dtpDeliveryDt.Text;
                    RptParmValue[5]  = "";
                    RptParmValue[6]  = dtpMvmtDtFr.Text;
                    RptParmValue[7]  = dtpMvmtDtTo.Text;
                    RptParmValue[8]  = txtCustCd.Text;
                    RptParmValue[9]  = txtCustCd.Text;
                    RptParmValue[10] = txtProjectNoFr.Text;
                    RptParmValue[11] = txtProjectNoTo.Text;
                    RptParmValue[12] = "";
                    RptParmValue[13] = "";
                    RptParmValue[14] = "";
                    RptParmValue[15] = "";
                    RptParmValue[16] = txtPoNoTo.Text;
                    RptParmValue[17] = txtPoNoFr.Text;
                    RptParmValue[18] = "";
                    RptParmValue[19] = "";
                    RptParmValue[20] = "";
                    //RptParmValue[0]  = "R3";
                    //RptParmValue[1] = SystemBase.Base.gstrLangCd;
                    //RptParmValue[2] = txtPurDeptCd.Text;
                    //RptParmValue[3] = dtpDeliveryDt.Text;
                    //RptParmValue[4] = "";
                    //RptParmValue[5] = dtpMvmtDtFr.Text;
                    //RptParmValue[6] = dtpMvmtDtTo.Text;
                    //RptParmValue[7] = txtCustCd.Text;
                    //RptParmValue[8] = txtCustCd.Text;
                    //RptParmValue[9] = txtProjectNoFr.Text;
                    //RptParmValue[10] = txtProjectNoTo.Text;
                    //RptParmValue[11] = "";
                    //RptParmValue[12] = "";
                    //RptParmValue[13] = "";
                    //RptParmValue[14] = "";
                    //RptParmValue[15] = txtPoNoTo.Text;
                    //RptParmValue[16] = txtPoNoFr.Text;
                    //RptParmValue[17] = "";
                    //RptParmValue[18] = "";
                    //RptParmValue[19] = SystemBase.Base.gstrCOMCD;
                    //RptParmValue[20] = "";

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전				
                    frm.ShowDialog();
                }
            }
        }
        #endregion


        #region 부품식별표레포트 출력
        private void c1Button1_Click(object sender, EventArgs e)
        {
            //조회 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string RptName = "";

                    if (rdoStatus_1.Checked == true)
                    {
                        string[] RptParmValue = new string[9];   // SP 파라메타 값
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\SCM009_1.rpt";    // 레포트경로+레포트명
                        RptParmValue[0] = "S1";
                        RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                        RptParmValue[2] = SystemBase.Base.gstrLangCd;
                        RptParmValue[3] = "";
                        RptParmValue[4] = txtCustCd.Text;
                        RptParmValue[5] = txtProjectNoFr.Text;
                        RptParmValue[6] = "";
                        RptParmValue[7] = dtpMvmtDtFr.Text;
                        RptParmValue[8] = dtpMvmtDtTo.Text;

                        UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전				
                        frm.ShowDialog();
                    }
                    else
                    {
                        string[] RptParmValue = new string[9];   // SP 파라메타 값
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\SCM009_2.rpt";    // 레포트경로+레포트명
                        RptParmValue[0] = "S3";
                        RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                        RptParmValue[2] = SystemBase.Base.gstrLangCd;
                        RptParmValue[3] = "";
                        RptParmValue[4] = txtCustCd.Text;
                        RptParmValue[5] = txtProjectNoFr.Text;
                        RptParmValue[6] = "";
                        RptParmValue[7] = dtpMvmtDtFr.Text;
                        RptParmValue[8] = dtpMvmtDtTo.Text;

                        UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전				
                        frm.ShowDialog();
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString());
                }
            }
        }
        #endregion
    }
}
