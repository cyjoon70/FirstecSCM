#region 작성정보
/*********************************************************************/
// 단위업무명 : 발주서출력
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-14
// 작성내용 : 발주서출력 및 관리
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

namespace SCM.SCM020
{
    public partial class SCM020 : UIForm.Buttons
    {
        #region 생성자
        public SCM020()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void SCM020_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtCustCd.Text = SystemBase.Base.gstrUserID;
            if (SystemBase.Base.gstrUserID == "KO132")
            {
                txtCustCd.ReadOnly = false;
                txtCustCd.BackColor = System.Drawing.Color.FromArgb(242, 252, 254);	// 필수 입력     
            }
            else
            {
                txtCustCd.ReadOnly = true;
                txtCustCd.BackColor = System.Drawing.Color.FromArgb(239, 239, 239);   // 읽기전용
            }

            //기타세팅
            dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            rdoCfmPoOld.Visible = false;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //기타세팅
            dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
            dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region 조회 조건 팝업
        //공급처 팝업
        private void btnCustCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                    txtCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //구매담당자
        private void btnPurDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDuty.Text = Msgs[0].ToString();
                    txtPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //발주번호
        private void btnPoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW018 pu = new WNDW018();
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPoNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트번호 FROM
        private void btnProjectNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNoFr.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNoFr.Text = Msgs[3].ToString();
                    txtProjectNmFr.Value = Msgs[4].ToString();
                    txtProjectSeqFr.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트번호 TO
        private void btnProjectNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNoTo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNoTo.Text = Msgs[3].ToString();
                    txtProjectNmTo.Value = Msgs[4].ToString();
                    txtProjectSeqTo.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수 FROM
        private void btnProjectSeqFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNoFr.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeqFr.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트차수 TO
        private void btnProjectSeqTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNoTo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeqTo.Text = Msgs[0].ToString();
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

        #region 조회조건 TextChanged
        //공급처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCustCd.Text != "")
                {
                    txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtCustNm.Value = "";
                }
            }
            catch
            {

            }

        }

        //구매담당자
        private void txtPurDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPurDuty.Text != "")
                {
                    txtPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPurDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //발주번호 FROM
        private void txtProjectNoFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNoFr.Text != "")
                {
                    txtProjectNmFr.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNoFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNmFr.Value = "";
                }
                if (txtProjectNmFr.Text == "")
                    txtProjectSeqFr.Text = "";
            }
            catch
            {

            }

        }

        //발주번호 TO
        private void txtProjectNoTo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNoTo.Text != "")
                {
                    txtProjectNmTo.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNoTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNmTo.Value = "";
                }
                if (txtProjectNmTo.Text == "")
                    txtProjectSeqTo.Text = "";
            }
            catch
            {

            }

        }
        #endregion
        
        #region 레포트 출력
        private void butPreview_Click(object sender, System.EventArgs e)
        {
            //조회 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string RptName = "";    // 레포트경로+레포트명
                    string strType = "";
                    string[] RptParmValue = new string[12];   // SP 파라메타 값

                    if (rdoCfmPoOld.Checked == true)
                    {
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\SCM020_1.rpt";
                        strType = "R1";
                    }
                    else if (rdoCfmOffer.Checked == true)
                    {
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\MPO506_SCM2.rpt";
                        strType = "R2";
                    }
                    else
                    {
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\MPO506_SCM1.rpt";
                        strType = "R3";
                    }

                    RptParmValue[0] = strType.Trim();
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = SystemBase.Base.gstrLangCd;
                    RptParmValue[3] = txtCustCd.Text;
                    RptParmValue[4] = txtPurDuty.Text;
                    RptParmValue[5] = txtPoNo.Text;
                    RptParmValue[6] = dtpPoDtFr.Text;
                    RptParmValue[7] = dtpPoDtTo.Text;
                    RptParmValue[8] = txtProjectNoFr.Text;
                    RptParmValue[9] = txtProjectNoTo.Text;
                    RptParmValue[10] = txtProjectSeqFr.Text;
                    RptParmValue[11] = txtProjectSeqTo.Text;

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, RptName, RptParmValue); //공통크리스탈 10버전				
                    frm.ShowDialog();
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
