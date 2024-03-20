using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using WNDW;

/// <summary>
/// 공지사항
/// </summary>
namespace SCM.SCM031
{
	public partial class SCM031 : UIForm.FPCOMM1
	{
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        long CurrSeq = 0;
        #endregion

        #region 생성자
        public SCM031()
		{
			InitializeComponent();
		}
        #endregion

        #region Form Load
        private void SCM031_Load(object sender, EventArgs e)
        {
            // 업무구분 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cbosJobType, "usp_B_COMMON @pType='COMM', @pCODE = 'NO_JOB_TYP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboJobType, "usp_B_COMMON @pType='COMM', @pCODE = 'NO_JOB_TYP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

            txtsCUST_CD.Text = SystemBase.Base.gstrUserID;

            if (SystemBase.Base.gstrUserID == "KO132")
            {

            }
            else
            {
                btnsCUST_CD.Tag = ";2;;";
                txtsCUST_CD.Tag = ";2;;";
            }

            SelectExec(false);

            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox3);
        }
        #endregion

        #region SelectExec() 그리드 조회 로직
        private void SelectExec(bool Msg)
        {
            try
            {
                string strQuery = "";
                strQuery = " usp_SCM031 @pTYPE = 'S1' ";
                strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @sJOB_TYPE ='" + cbosJobType.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @sSEARCH_WORDS ='" + txtWords.Text.ToString().Trim() + "' ";
                strQuery = strQuery + ", @sCUST_CD ='" + txtsCUST_CD.Value + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, Msg);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region SearchExec() -- 검색
        protected override void SearchExec()
        {
            SelectExec(true);
        }
		#endregion

		#region 초기화
		protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);

            fpSpread1.Sheets[0].Rows.Count = 0;

            txtsCUST_CD.Text = SystemBase.Base.gstrUserID;
        }
		#endregion

		#region QA001_Activated
		private void QA001_Activated(object sender, System.EventArgs e)
        {
            SelectExec(false);
        }
		#endregion

		#region 상세 조회
		private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                    //같은 Row 조회 되지 않게
                    if (intRow < 0)
                    {
                        return;
                    }

                    if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                    {
                        return;
                    }

                    CurrSeq = Convert.ToInt64(fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "SEQ")].Text);

                    HitUpdate(CurrSeq);
                    SubSearch(CurrSeq);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }

        private void SubSearch(long seq)
		{
            string strQuery = "";
            strQuery = " usp_SCM031 @pTYPE = 'S2' ";
            strQuery = strQuery + ", @pSEQ = " + seq;
            strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

            txtSeq.Value = ds.Tables[0].Rows[0][1].ToString();              //Seq
            cboJobType.SelectedValue = ds.Tables[0].Rows[0][2].ToString();  //업무구분
            txtTitle.Value = ds.Tables[0].Rows[0][3].ToString();             //제목
            txtContents.Value = ds.Tables[0].Rows[0][4].ToString();          //내용
            txtRegUserId.Value = ds.Tables[0].Rows[0][5].ToString();        //등록자 ID
            txtRegUser.Value = ds.Tables[0].Rows[0][6].ToString();          //등록자
            dtRegDt.Value = ds.Tables[0].Rows[0][7].ToString();              //등록일
            dtLimitDt.Value = ds.Tables[0].Rows[0][8].ToString();            //공지만료일
            txtCustCd.Value = ds.Tables[0].Rows[0][9].ToString();            //공개업체코드
            txtCustNm.Value = ds.Tables[0].Rows[0][10].ToString();          //공개업체명
            txtReadCnt.Value = ds.Tables[0].Rows[0][11].ToString();         //조회수

            SystemBase.Validation.GroupBox_Setting(groupBox3);
        }

        private void HitUpdate(long idx)
        {
            string strQuery = "";
            strQuery = " usp_SCM031 @pTYPE = 'H1' ";
            strQuery = strQuery + ", @pSEQ = " + idx.ToString() + "";
            strQuery = strQuery + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
            strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";

            SystemBase.DbOpen.NoTranDataTable(strQuery);
        }
		#endregion

		#region 공개 업체 조회
		private void btnsCUST_CD_Click(object sender, EventArgs e)
		{
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtsCUST_CD.Value = Msgs[1].ToString();
                    txtsCUST_NM.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

		private void txtsCUST_CD_TextChanged(object sender, EventArgs e)
		{
            txtsCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtsCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
		#endregion

		#region 첨부파일 조회
		private void btnPopup_Click(object sender, EventArgs e)
		{
            try
            {

                // 첨부파일 팝업 띄움.
                WNDWS01 pu = new WNDWS01(txtSeq.Text, txtSeq.Text, "", "", "", "", false, "", "공지사항");
                pu.ShowDialog();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            /*
            string strAuth = string.Empty;
            string strFilsNo = string.Empty;

            strAuth = "N#Y#N";
            strFilsNo = txtSeq.Text;

            UIForm.FileUpDown fileUpDown = new UIForm.FileUpDown("SC01" + strFilsNo, strAuth);
            fileUpDown.ShowDialog();
            */
        }
		#endregion

	}
}
