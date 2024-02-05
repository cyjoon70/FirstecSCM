#region 작성정보
/*********************************************************************/
// 단위업무명 : 라우팅정보등록
// 작 성 자 : 권순철
// 작 성 일 : 2013-05-23
// 작성내용 : 라우팅정보등록 및 관리
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
using System.Data.SqlClient;
using WNDW;

namespace SCM.SCM016
{
    public partial class SCM016 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strSchNo = "";
        string strBtn = "N";
        string strITEM_CD = "";
        int intRow = 0;
        #endregion

        #region
        public SCM016()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SCM016_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //그리드 콤보박스 세팅			
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공정단계")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            //기타 세팅
            txtCustCd.Text = SystemBase.Base.gstrUserID;
            if (SystemBase.Base.gstrUserID != "KO132")
            {
                txtCustCd.Tag = ";2;;";
                btnCustCd.Tag = ";2;;";
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            txtCustCd.Text = SystemBase.Base.gstrUserID;
        }
        #endregion

        #region 조회조건 TextChanged
        //거래처
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 조회조건 팝업
        //거래처 팝업
        private void btnCustCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM016 @pTYPE = 'P2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strQuery = "usp_SCM016 'S1'";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 상세조회
        private void SubSearch(int iRow)
        {

            string strQuery = "usp_SCM016 'S2'";
            strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strQuery += ", @pITEM_CD = '" + fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text + "' ";

            strITEM_CD = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "품목코드")].Text;

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region 마스터 그리드 선택시
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;

                    //같은 Row 조회 되지 않게
                    if (intRow < 0)
                    {
                        return;
                    }

                    if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                    {
                        return;
                    }

                    SubSearch(intRow);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);
        }
        #endregion

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드_2"))
            {
                try
                {
                    string strQuery = " usp_SCM016 @pTYPE = 'P1' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pPROC_CD","@pPROC_NM" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드")].Text,"" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM016P1", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정코드조회");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string MSG = pu.ReturnVal.Replace("|", "#");
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(MSG);
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            try
            {
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드")].Text, " AND MAJOR_CD = 'P001'");
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                string strJOB_CD = "";
                string strJOB_NM = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //그리드 상단 필수 체크
                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            string strGbn = "";

                            if (strHead.Length > 0)
                            {
                                #region 유효성 체크
                                //공정항목코드 유효성 체크
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text == "")
                                {
                                    //존재하지 않는 공정 코드입니다
                                    MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "공정"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드"));

                                    Trans.Rollback();
                                    this.Cursor = Cursors.Default;
                                    return;
                                }
                                #endregion

                                switch (strHead)
                                {
                                    case "U": strGbn = "U1"; break;
                                    case "I": strGbn = "I1"; break;
                                    case "D": strGbn = "D1"; break;
                                    default: strGbn = ""; break;
                                }

                                string strSUB = "N"; if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주여부")].Text.ToString() == "True") strSUB = "Y";
                                string strSql = " usp_SCM016 '" + strGbn + "'";

                                strJOB_CD = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정코드")].Text;
                                strJOB_NM = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text;
                                strSql += ", @pITEM_CD = '" + strITEM_CD + "' ";
                                strSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + "' ";
                                strSql += ", @pPROC_CD = '" + strJOB_CD + "' ";
                                strSql += ", @pPROC_NM = '" + strJOB_NM + "' ";
                                strSql += ", @pPROC_STEP = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정단계")].Value + "' ";
                                strSql += ", @pSUB_FLAG = '" + strSUB + "' ";
                                strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                strSql += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프					
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
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread2, strITEM_CD);
                    SubSearch(intRow);
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
