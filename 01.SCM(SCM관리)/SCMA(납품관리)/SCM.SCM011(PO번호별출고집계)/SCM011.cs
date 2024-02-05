using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;


namespace SCM.SCM011
{ 
    public partial class SCM011 : UIForm.FPCOMM1
    {
        #region 생성자
        public SCM011()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load
        private void SCM011_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            txtCustCd.Value = SystemBase.Base.gstrUserID;
            if (SystemBase.Base.gstrUserID == "KO132")
            {
                txtCustCd.Tag = "거래처;1;;";
                btnCustCd.Tag = "";
            }
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtCustCd.Value = SystemBase.Base.gstrUserID;

            dtpBasisDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString().Substring(0, 10);
            dtpBasisDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            txtCustCd.Value = SystemBase.Base.gstrUserID;

            dtpBasisDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString().Substring(0, 10);
            dtpBasisDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region Button Click
        //거래처 팝업
        private void btnCustCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM007 @pTYPE = 'P2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCUST_CD", "@pCUST_NM" };
                string[] strSearch = new string[] { txtCustCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM001P2", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "거래처조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtCustCd.Value = Msgs[0].ToString();
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

                    txtItemCd.Value = Msgs[0].ToString();
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

                    txtProjectNo.Value = Msgs[0].ToString();
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
        
        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
            Search("");
        }

        private void Search(string strPoNo)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    var div = "";
					if (rdoStatus_1.Checked == true) div = "1";
					else if(rdoStatus_2.Checked == true) div = "2";
										
					var Cfm = "";					
					if(rdoBasis_Scm.Checked == true)
					{
						if(div == "1") Cfm = "S1";
						else  Cfm = "S3";
							
					}
                    else if (rdoBasis_Mvmt.Checked == true) 
					{
						if(div == "1") Cfm = "S2";
						else  Cfm = "S4";
					}

                    string Query = " usp_SCM011 @pType='" + Cfm + "'";
                    Query += ", @pBASIS_DT_FR = '" + dtpBasisDtFr.Text + "'";
                    Query += ", @pBASIS_DT_TO = '" + dtpBasisDtTo.Text + "'";
                    Query += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    Query += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    Query += ", @pPO_NO = '" + txtPoNo.Text + "'";
                    Query += ", @pSCM_MVMT_NO = '" + txtScmMvmtNo.Text + "'";
                    Query += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    Query += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
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

    }
}
