using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WNDW
{
	public partial class WNDW091 : UIForm.FPCOMM1
	{
        #region 변수선언
        string[] returnVal = null;
        string strAcct = "";

        int SDown = 1;		    // 조회 횟수
        int AddRow = 100;	    // 조회 건수
        string strtype = "S1";  // 조회 조건
        string strItemCd = "";
        string strCustCd = "";
        #endregion

        #region 생성자
        public WNDW091(string strCuCd)
        {

            strCustCd = strCuCd;

            InitializeComponent();
        }

        public WNDW091(string ItemCd, string strItemAcc)
        {
            //품목계정값 10-제품, 20-반제품, 25-재공품, 30-원자재, 33-저장품, 35-부자재, 50-상품, 60-포장재, 70-공구소모품, CUST- 거래처품목

            strItemCd = ItemCd;
            strAcct = strItemAcc;

            InitializeComponent();
        }

        public WNDW091()
        {
            strAcct = "";		//퀴리문 없는조건

            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW091_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("110000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'"); //품목계정
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");     //단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'P032', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'"); //품목구분

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            this.Text = "고객 품목 정보 조회";
            
            Grid_search(strtype, false);

            txtItemCd.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_search(strtype, true); }
        #endregion

        #region 그리드 더블클릭
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            RtnStr(e.Row);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region 그리드 선택값 입력밑 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int R)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = new string[fpSpread1.Sheets[0].Columns.Count];

                for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    returnVal[i] = Convert.ToString(fpSpread1.Sheets[0].Cells[R, i].Value);
                }
            }
        }
        #endregion

        #region 조회함수
        private void Grid_search(string strType, bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_WNDW091 @pTYPE = '" + strType + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                strQuery += ", @pCUST_CD ='" + strCustCd + "'";
                strQuery += ", @pITEM_CD ='" + txtItemCd.Text + "'";
                strQuery += ", @pITEM_NM ='" + txtItemNm.Text + "'";
                strQuery += ", @pITEM_SPEC ='" + txtItemSpec.Text.Trim() + "'";
                strQuery += ", @pDRAW_NO ='" + txtDrawNo.Text.Trim() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

    }
}
