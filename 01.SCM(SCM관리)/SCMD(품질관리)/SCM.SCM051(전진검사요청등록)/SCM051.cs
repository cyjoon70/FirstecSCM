#region 작성정보
/*********************************************************************/
// 단위업무명 : 전진검사요청등록
// 작 성 자 : 한 미 애
// 작 성 일 : 2017.11.27. 
// 작성내용 : 전진검사요청등록
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using WNDW;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace SCM.SCM051
{
    public partial class SCM051 : UIForm.FPCOMM1
    {
        #region 변수선언

        #endregion

        #region 생성자
        public SCM051()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void SCM051_Load(object sender, System.EventArgs e)
        {
            UIForm.Buttons.ReButton("110000011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            txtCustCd.Value = SystemBase.Base.gstrUserID;
            if (SystemBase.Base.gstrUserID == "KO132")
            {
                txtCustCd.Tag = "거래처;1;;";
                btnCust.Tag = "";
            }

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
 
            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q035', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q036', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            // 2018.01.17. hma 추가(Start)	
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사원")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM1', @pCODE = 'Q037', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품보원")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM1', @pCODE = 'Q030', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "검사결과")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q004', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            // 2018.01.17. hma 추가(End)

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            dtpPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 10);
            dtpPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

        }
        #endregion
      
        #region 버튼 Click
        //품목코드
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM051 @pTYPE = 'P3' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCUST_CD='" + txtCustCd.Text + "'";
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

        //공급처
        private void btnCust_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM051 @pTYPE = 'P2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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

        //프로젝트번호
        private void btnProj_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM051 @pTYPE = 'P1' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCUST_CD='" + txtCustCd.Text + "'";
                string[] strWhere = new string[] { "@pPROJECT_NO" };
                string[] strSearch = new string[] { txtProjectNo.Text };

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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region TextChanged
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

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            txtCustCd.Value = SystemBase.Base.gstrUserID;

            dtpPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString().Substring(0, 10);
            dtpPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
                {
                    string strQuery = " usp_SCM051  @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "'";
                    strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "'";
                    strQuery += ", @pDELIVERY_DT_FR = '" + dtpPODelivDtFr.Text + "'";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpPODelivDtTo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
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
            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    string Msg = string.Empty;

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        //그리드 상단 필수 체크
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
                        {
                            //행수만큼 처리
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                string strGbn = "";
                                if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                                {
                                    strGbn = "I1";

                                    string strQuery = " usp_SCM051";
                                    strQuery += " @pTYPE = '" + strGbn + "'";
                                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                                    strQuery += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text + "'";
                                    strQuery += ", @pPO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text + "'";
                                    strQuery += ", @pINSP_REQ_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사요청수량")].Value + "'";
                                    strQuery += ", @pINSP_CLASS_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사항목")].Value.ToString() + "'";
                                    strQuery += ", @pINSP_SIDE_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사구분")].Value.ToString() + "'";
                                    strQuery += ", @pINSP_SELF_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자체검사일")].Text + "'";
                                    strQuery += ", @pINSP_PLAN_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수검계획일")].Text + "'";
                                    strQuery += ", @pINSP_REQ_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰일")].Text + "'";
                                    strQuery += ", @pINSP_DEMAND_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사요청일")].Text + "'";
                                    // 2018.01.10. hma 추가(Start): 전진검사일, 전진검사원, 품보원, 검사결과를 업체에서 등록하는걸로 요청하여 추가.
                                    //strQuery += ", @pINSP_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사일")].Value + "'";
                                    //strQuery += ", @pDECISION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사결과")].Value.ToString() + "'";
                                    //strQuery += ", @pPROG_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "진척율")].Text + "'";
                                    string strInspectorCd = "", strProgInspCd = "";
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품보원")].Text == "")
                                        strInspectorCd = "";
                                    else
                                        strInspectorCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품보원")].Value.ToString();

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사원")].Text == "")
                                        strProgInspCd = "";
                                    else
                                        strProgInspCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전진검사원")].Value.ToString();
                                    strQuery += ", @pINSPECTOR_CD = '" + strInspectorCd + "'";
                                    strQuery += ", @pPROG_INSPECTOR_CD = '" + strProgInspCd + "'";
                                    // 2018.01.10. hma 추가(End)
                                    strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strQuery += ", @pINSP_REQ_NO = '" + Msg + "'";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();
                                    Msg = ds.Tables[0].Rows[0][2].ToString();       // 리턴된 검사요청번호 저장

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프
                                }
                            }
                        }
                        else
                        {
                            Trans.Rollback();
                            this.Cursor = Cursors.Default;
                            return;
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
                    this.Cursor = Cursors.Default;
                }
            }
        }
        #endregion

        #region fpSpread1_EditModeOff() 검사잔량 체크
        private void fpSpread1_EditModeOff(object sender, EventArgs e)
        {
            try
            {
                if (
                    Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사요청수량")].Value) >
                    Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사잔량")].Value)
                   )
                {
                    MessageBox.Show("검사요청수량은 검사잔량을 초과할 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사요청수량")].Value =
                    Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "검사잔량")].Value);
                    return;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show("데이터 처리 중 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}

