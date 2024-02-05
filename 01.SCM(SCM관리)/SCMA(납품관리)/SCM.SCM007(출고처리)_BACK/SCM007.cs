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

            dtpPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString().Substring(0, 10);
            dtpPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            txtCustCd.Value = SystemBase.Base.gstrUserID;

            dtpPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString().Substring(0, 10);
            dtpPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
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
                    strQuery += ", @pITEM_CD = '" + txtItemNm.Text + "'";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

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
                        retVal += decimal.Parse(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Text) * decimal.Parse(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고단가")].Text);
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
                string Msg = "";
                string strReorgId = "";                

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
                            strQuery += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, 2].Text + "'";
                            strQuery += ", @pPO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, 3].Text + "'";
                            strQuery += ", @pSUPPLY_QTY = '" + fpSpread1.Sheets[0].Cells[i, 12].Value + "'";
                            strQuery += ", @pSUPPLY_DT = '" + fpSpread1.Sheets[0].Cells[i, 14].Text + "'";
                            strQuery += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, 29].Text + "'";
                            strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery += ", @pSCM_MVMT_NO = '" + Msg + "'";
                            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
                            Msg = ds.Tables[0].Rows[0][2].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
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
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region fpButtonClick
        //품질증빙 조회 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
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
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품질증빙 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion
               
    }
}
