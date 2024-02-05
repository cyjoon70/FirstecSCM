#region 작성정보
/*********************************************************************/
// 단위업무명 : 월별집계/상세현황
// 작 성 자 : 권순철
// 작 성 일 : 2013-05-16
// 작성내용 : 월별집계/상세현황 및 관리
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

namespace SCM.SCM013
{
    public partial class SCM013 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strSchNo = "";
        string strBtn = "N";
        #endregion

        #region
        public SCM013()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SCM013_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            txtBasisDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-12).ToString().Substring(0,7);
            txtBasisDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0,7);
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
            txtBasisDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-12).ToString().Substring(0, 7);
            txtBasisDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
            txtCustCd.Text = SystemBase.Base.gstrUserID;
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
        #endregion

        #region 조회조건 팝업
        //거래처 팝업
        private void btnCustCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM013 @pTYPE = 'P2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
        //품목
        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_SCM013 @pTYPE = 'P3' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCUST_CD='" + txtCustCd.Text + "'";
                string[] strWhere = new string[] { "@pITEM_CD", "@pITEM_NM" };
                string[] strSearch = new string[] { txtItemCd.Text, "" };

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
                    string strQuery = "usp_SCM013";
                    if (rdoBasis_Scm.Checked == true)
                    {
                        strQuery += " @pTYPE = 'S1'";
                    }
                    if (rdoBasis_Rc.Checked == true)
                    {
                        strQuery += " @pTYPE = 'S2'";
                    }
                    if (rdoBasis_Iv.Checked == true)
                    {
                        strQuery += " @pTYPE = 'S3'";
                    }
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemNm.Text + "' ";
                    strQuery += ", @pBASIS_DT_FR = '" + txtBasisDtFr.Text + "' ";
                    strQuery += ", @pBASIS_DT_TO = '" + txtBasisDtTo.Text + "' ";

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

            string strQuery = "usp_SCM013";
            if (rdoBasis_Scm.Checked == true)
            {
                strQuery += " @pTYPE = 'S4'";
            }
            if (rdoBasis_Rc.Checked == true)
            {
                strQuery += " @pTYPE = 'S5'";
            }
            if (rdoBasis_Iv.Checked == true)
            {
                strQuery += " @pTYPE = 'S6'";
            }
            strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
            strQuery += ", @pBASIS_DT = '" + fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "년월")].Text + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
        }
        #endregion

        #region 마스터 그리드 선택시
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
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

        #region 조회기준 변경
        private void rdoBasis_Scm_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoBasis_Scm.Checked == true)
            {
                c1Label11.Text = "수주년월";
                txtBasisDtFr.Tag = "수주년월;1;;";
                txtBasisDtTo.Tag = "수주년월;1;;";
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }
        }
        private void rdoBasis_Rc_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoBasis_Rc.Checked == true)
            {
                c1Label11.Text = "납품년월";
                txtBasisDtFr.Tag = "납품년월;1;;";
                txtBasisDtTo.Tag = "납품년월;1;;";
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }
        }
        private void rdoBasis_Iv_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoBasis_Iv.Checked == true)
            {
                c1Label11.Text = "매출년월";
                txtBasisDtFr.Tag = "매출년월;1;;";
                txtBasisDtTo.Tag = "매출년월;1;;";
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }
        }
        #endregion

    }
}
