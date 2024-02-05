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


namespace SCM.SCM002
{
    public partial class SCM002 : UIForm.FPCOMM1 
    {
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        int btnR=0;

        public SCM002()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void SCM002_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            txtReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            txtReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

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

            txtReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            txtReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        
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
                string strQuery = " usp_SCM002 @pTYPE = 'P2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                string strQuery = " usp_SCM002 @pTYPE = 'P3' , @pCUST_CD='" + txtCustCd.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                    string strQuery = " usp_SCM002 @pTYPE='S1' ";
                    strQuery += ", @pREQ_DT_FR ='" + txtReqDtFr.Text + "'";
                    strQuery += ", @pREQ_DT_TO ='" + txtReqDtTo.Text + "'";
                    strQuery += ", @pREQ_NO ='" + txtReqNo.Text + "'";
                    strQuery += ", @pEST_REQ_DT_FR ='" + txtEstReqDtFr.Text + "'";
                    strQuery += ", @pEST_REQ_DT_TO ='" + txtEstReqDtTo.Text + "'";
                    strQuery += ", @pEST_NO = '" + txtEstNo.Text + "'";
                    strQuery += ", @pSCM_DT_FR ='" + txtScmDtFr.Text + "'";
                    strQuery += ", @pSCM_DT_TO ='" + txtScmDtTo.Text + "'";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pCUST_DUTY_NM = '" + txtCustDutyNm.Text + "'";
                    strQuery += ", @pCUST_CD ='" + txtCustCd.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
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

  
    }
}
