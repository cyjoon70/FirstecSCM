using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace SCM.SCM008
{
    public partial class SCM008 : UIForm.FPCOMM2
    {

        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strSCM_MVMT_NO = "";
        int NewFlg = 1;//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정
        string strSearchData = "", strSaveData = ""; //컨트롤 저장 체크 변수
        #endregion

        #region 생성자
        public SCM008()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load
        private void SCM008_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("110001011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            txtCustCd.Value = SystemBase.Base.gstrUserID;
            if (SystemBase.Base.gstrUserID == "KO132")
            {
                txtCustCd.Tag = "거래처;1;;";
                btnCustCd.Tag = "";
            }
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpSupplyDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString().Substring(0, 10);
            dtpSupplyDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            txtCustCd.Value = SystemBase.Base.gstrUserID;

            dtpSupplyDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString().Substring(0, 10);
            dtpSupplyDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region 행삭제 버튼 클릭 이벤트
        protected override void DelExec()
        {	// 행 삭제
            try
            {
                UIForm.FPMake.RowRemove(fpSpread1);
                DelExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    string Query = " usp_SCM008 @pTYPE='S1'";
                    Query += ", @pSUPPLY_DT_FR = '" + dtpSupplyDtFr.Text + "'";
                    Query += ", @pSUPPLY_DT_TO = '" + dtpSupplyDtTo.Text + "'";
                    Query += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "'";
                    Query += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "'";
                    Query += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "'";
                    Query += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "'";
                    Query += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    Query += ", @pPO_NO = '" + txtPoNo.Text + "'";
                    Query += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    Query += ", @pSCM_MVMT_NO = '" + txtScmMvmtNo.Text + "'";
                    Query += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    Query += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, Query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

					if (fpSpread2.Sheets[0].Rows.Count == 0) { fpSpread1.Sheets[0].Rows.Count = 0; }
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
            this.Cursor = Cursors.WaitCursor;


            GroupBox[] gBox = null;

            string strMstType = "";
            string strInUpFlag = "I";
			string strQuery = string.Empty;

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////

            //컨트롤 체크값 초기화
            strSaveData = ""; ;

            //기존 컨트롤 데이터와 현재 컨트롤 데이터 비교
            if (strSearchData == strSaveData && UIForm.FPMake.HasSaveData(fpSpread1) == false)
            {
                //변경되거나 처리할 데이터가 없습니다.
                MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Cursor = Cursors.Default;
                return;
            }

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strSoSeq = "";

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
                                default: strGbn = ""; break;
                            }

							strQuery = string.Empty;

                            strQuery = " usp_SCM008 '" + strGbn + "'";
							strQuery += ", @pSCM_MVMT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고번호")].Value + "'";
							strQuery += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Value + "'";
							strQuery += ", @pPO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Value + "'";
                            strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrCOMCD + "'";
                            strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds1 = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

							// LOT 삭제
							if (string.Compare(strGbn, "D1", true) == 0)
							{
								strQuery = string.Empty;

								strQuery = " usp_T_SCM_IN_INFO_CUDR ";
								strQuery += " @pTYPE		= 'D1' ";
								strQuery += ",@pCO_CD		= '" + SystemBase.Base.gstrCOMCD + "' ";
								strQuery += ",@pPLANT_CD	= '" + SystemBase.Base.gstrPLANT_CD + "' ";
								strQuery += ",@pSCM_MVMT_NO	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고번호")].Value + "'";
								strQuery += ",@pPO_NO		= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Value + "'";
								strQuery += ",@pPO_SEQ		= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Value + "'";

								DataSet ds2 = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
								ERRCode = ds2.Tables[0].Rows[0][0].ToString();
								MSGCode = ds2.Tables[0].Rows[0][1].ToString();

								if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
							}

                        }
                    }
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    this.Cursor = Cursors.Default;
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Search("");
                    UIForm.FPMake.GridSetFocus(fpSpread2, strSCM_MVMT_NO, SystemBase.Base.GridHeadIndex(GHIdx2, "출고번호"));                   
                    SubSearch(strSCM_MVMT_NO);
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
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0038"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//최소 한건 이상의 DETAIL정보가 존재하지 않으면 등록할 수 없습니다.
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Master그리드 선택시 상세정보 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    try
                    {
                        int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;

                        //같은 Row 조회 되지 않게
                        if (intRow < 0)
                        {
                            return;
                        }

                        if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                        {
                            return;
                        }

                        strSCM_MVMT_NO = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "출고번호")].Text.ToString();//수주번호

                        SubSearch(strSCM_MVMT_NO);
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                    }
                }
            }
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string strCode)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                fpSpread1.Sheets[0].Rows.Count = 0;
                //수주Detail그리드 정보.
                String Query = " usp_SCM008 @pTYPE = 'S2'";
                Query += ", @pSCM_MVMT_NO = '" + strCode + "'";
                Query += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

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
