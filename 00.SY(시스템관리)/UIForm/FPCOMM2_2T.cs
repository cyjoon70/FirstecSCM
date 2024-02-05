﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win.Spread;
using System.Text.RegularExpressions;

namespace UIForm
{
    public partial class FPCOMM2_2T : Buttons
    {
        #region 그리드 디자인 변수 정의
		public string[]	G1Head1		= null;// 첫번째 Head Text
		public string[]	G1Head2		= null;// 두번째 Head Text
		public string[]	G1Head3		= null;// 세번째 Head Text
		public int[]	G1Width		= null;// Cell 넓이
		public string[]	G1Align		= null;// Cell 데이타 정렬방식
		public string[]	G1Type		= null;// CellType 지정
		public int[]	G1Color		= null;// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
		public string[]	G1Etc		= new string[100];//null;// Mask 양식 등
		public int		G1HeadCnt	= 0;   // Head 수
		public int[]	G1SEQ		= null;// 키

		public string[]	G2Head1		= null;// 첫번째 Head Text
		public string[]	G2Head2		= null;// 두번째 Head Text
		public string[]	G2Head3		= null;// 세번째 Head Text
		public int[]	G2Width		= null;// Cell 넓이
		public string[]	G2Align		= null;// Cell 데이타 정렬방식
		public string[]	G2Type		= null;// CellType 지정
		public int[]	G2Color		= null;// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
		public string[]	G2Etc		= null;// Mask 양식 등
		public int		G2HeadCnt	= 0;   // Head 수
		public int[]	G2SEQ		= null;// 키

        int GridSelectRow = 0;
        int GridSelectRowCount = 0;
        int ClipboardRowCount = 1;
        int ClipboardColCount = 1;
        string ClipValue = "";
		#endregion

        private System.Windows.Forms.ContextMenu ctmGrid1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.ContextMenu ctmGrid2;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.TextBox txtRowCnt;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem menuItem10;
        public System.Windows.Forms.Panel GridCommPanel2;
        public System.Windows.Forms.GroupBox GridCommGroupBox2;
        public FarPoint.Win.Spread.FpSpread fpSpread2;
        public FarPoint.Win.Spread.SheetView fpSpread2_Sheet1;
        public System.Windows.Forms.Panel TabCommPanel;
        public C1.Win.C1Command.C1DockingTab c1DockingTab1;
        public C1.Win.C1Command.C1DockingTabPage c1DockingTabPage1;
        public System.Windows.Forms.Panel GridCommPanel1;
        public System.Windows.Forms.GroupBox GridCommGroupBox1;
        public FarPoint.Win.Spread.FpSpread fpSpread1;
        public FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;

        private FindText frm = new FindText();

        public FPCOMM2_2T()
        {
            InitializeComponent();
        }

        #region SaveExec 저장
        protected virtual void SaveExec() { }
        protected override void SaveExec2()
        {	// 저장
            try
            {
                this.BtnInsert.Focus();
                SaveExec();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region RCopyExec 그리드 Row 복사
        protected virtual void RCopyExe() { }
        protected override void RCopyExec()
        {
            try
            {
                if (fpSpread2.Focused == true)
                    UIForm.FPMake.RowCopy(fpSpread2);
                else
                    UIForm.FPMake.RowCopy(fpSpread1);

                RCopyExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region DelExec 행 삭제
        protected virtual void DelExe() { }
        protected override void DelExec()
        {	// 행 삭제
            try
            {
                if (fpSpread2.Focused == true)
                    UIForm.FPMake.RowRemove(fpSpread2);
                else
                    UIForm.FPMake.RowRemove(fpSpread1);

                DelExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region DelExec 싱글 삭제로직
        protected virtual void DeleteExe() { }
        protected override void DeleteExec()
        {	// 행 삭제
            try
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("SY010"), "전체삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    DeleteExe();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "전체삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region CancelExec 행 취소
        protected virtual void CancelExe() { }
        protected override void CancelExec()
        {	// 행 삭제
            try
            {
                if (fpSpread2.Focused == true)
                {
                    if (fpSpread2.ActiveSheet.GetSelection(0) == null || fpSpread2.ActiveSheet.ActiveRowIndex < 0) return;

                    if (fpSpread2.ActiveSheet.ActiveRowIndex.ToString() != "")
                    {
                        GridSelectRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                        int Row = fpSpread2.ActiveSheet.ActiveRowIndex;
                        int Col = fpSpread2.ActiveSheet.ActiveColumnIndex;

                        GridSelectRowCount = 1;
                        if (fpSpread2.ActiveSheet.GetCellType(Row, Col).ToString() != "ComboBoxCellType" && fpSpread2.ActiveSheet.GetCellType(Row, Col).ToString() != "CheckBoxCellType")
                        {
                            GridSelectRowCount = fpSpread2.Sheets[0].GetSelection(0).RowCount;
                        }
                    }
                    UIForm.FPMake.Cancel(fpSpread2, GridSelectRow, GridSelectRowCount);
                }
                else
                {
                    if (fpSpread1.ActiveSheet.GetSelection(0) == null || fpSpread1.ActiveSheet.ActiveRowIndex < 0) return;

                    if (fpSpread1.ActiveSheet.ActiveRowIndex.ToString() != "")
                    {
                        GridSelectRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                        int Row = fpSpread1.ActiveSheet.ActiveRowIndex;
                        int Col = fpSpread1.ActiveSheet.ActiveColumnIndex;

                        GridSelectRowCount = 1;
                        if (fpSpread1.ActiveSheet.GetCellType(Row, Col).ToString() != "ComboBoxCellType" && fpSpread1.ActiveSheet.GetCellType(Row, Col).ToString() != "CheckBoxCellType")
                        {
                            GridSelectRowCount = fpSpread1.Sheets[0].GetSelection(0).RowCount;
                        }
                    }
                    UIForm.FPMake.Cancel(fpSpread1, GridSelectRow, GridSelectRowCount);
                }
                CancelExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행취소"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region RowInsExec 행 추가
        protected virtual void RowInsExe() { }
        protected override void RowInsExec()
        {	// 행 추가
            try
            {
                if (fpSpread2.Focused == true)
                    UIForm.FPMake.RowInsert(fpSpread2);
                else
                    UIForm.FPMake.RowInsert(fpSpread1);

                RowInsExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region ExcelExec() Excel 저장
        protected virtual void ExcelExe() { }
        protected override void ExcelExec()
        {
            try
            {
                if (fpSpread1.Focused == true)
                    UIForm.FPMake.ExcelMake(fpSpread1, this.Text.ToString() + "_1");
                else
                    UIForm.FPMake.ExcelMake(fpSpread2, this.Text.ToString() + "_2");

                ExcelExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Excel 저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region PrintMake(그리드, 미리보기) 그리드 Print
        protected virtual void PrintExe() { }
        protected override void PrintExec()
        {
            try
            {
                if (fpSpread2.Focused == true)
                    UIForm.FPMake.PrintMake(fpSpread2, this.Text);
                else
                    UIForm.FPMake.PrintMake(fpSpread1, this.Text);

                PrintExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "프린트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region fpSpread1_ComboCloseUp 스프레드에서 콤보 선택시 Focus 이동
        protected virtual void fpComboCloseUp() { }
        private void fpSpread1_ComboCloseUp(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                fpSpread1.ActiveSheet.SetActiveCell(e.Row, e.Column + 1);

                fpComboCloseUp();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 콤보 선택"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region fpSpread2_ComboCloseUp 스프레드에서 콤보 선택시 Focus 이동
        protected virtual void fpComboCloseUp2() { }
        private void fpSpread2_ComboCloseUp(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                fpSpread2.ActiveSheet.SetActiveCell(e.Row, e.Column + 1);

                fpComboCloseUp();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 콤보 선택"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region fpSpread1_Change 데이타 수정시 U 플래그 등록
        protected virtual void fpChange() { }
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                UIForm.FPMake.fpChange(fpSpread1, e.Row);
                fpChange();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 EditChange 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region fpSpread2_Change 데이타 수정시 U 플래그 등록
        private void fpSpread2_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                UIForm.FPMake.fpChange(fpSpread2, e.Row);
                fpChange();

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 EditChange 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region fpSpread2_ButtonClicked 버튼 클릭 Event
        protected virtual void fpButtonClick2(int Row, int Column) { }
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                fpButtonClick2(e.Row, e.Column);
                if (fpSpread2.Sheets[0].GetCellType(e.Row, e.Column).ToString() == "CheckBoxCellType" && fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text != "I")
                {
                    fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "U";

                    fpSpread2.Sheets[0].RowHeader.Rows[e.Row].BackColor = SystemBase.Base.Color_Update;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 버튼 클릭"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpSpread1_ButtonClicked 버튼 클릭 Event
        protected virtual void fpButtonClick(int Row, int Column) { }
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {

            try
            {
                fpButtonClick(e.Row, e.Column);
                if (fpSpread1.Sheets[0].GetCellType(e.Row, e.Column).ToString() == "CheckBoxCellType" && fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text != "I")
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "U";

                    fpSpread1.Sheets[0].RowHeader.Rows[e.Row].BackColor = SystemBase.Base.Color_Update;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 버튼 클릭"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        #endregion

        #region FPCOMM1_Load
        private void FPCOMM2_2_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (SystemBase.Base.ProgramWhere.Length > 0)
                {
                    string Query = " usp_BAA004 'S3', @PFORM_ID='" + this.Name.ToString() + "', @PGRID_NAME='fpSpread1', @PIN_ID='" + SystemBase.Base.gstrUserID + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    DataTable dt = SystemBase.DbOpen.TranDataTable(Query);
                    int G1RowCount = dt.Rows.Count + 1;

                    if (G1RowCount > 1)
                    {
                        G1Head1 = new string[G1RowCount];// 첫번째 Head Text
                        G1Head2 = new string[G1RowCount];// 두번째 Head Text
                        G1Head3 = new string[G1RowCount];// 세번째 Head Text
                        G1Width = new int[G1RowCount];// Cell 넓이
                        G1Align = new string[G1RowCount];// Cell 데이타 정렬방식
                        G1Type = new string[G1RowCount];// CellType 지정
                        G1Color = new int[G1RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
                        //G1Etc		= new string[G1RowCount];
                        G1HeadCnt = Convert.ToInt32(dt.Rows[0][0].ToString());
                        G1SEQ = new int[G1RowCount];// 키

                        /********************1번째 숨김필드 정의******************/
                        G1Head1[0] = "";
                        if (Convert.ToInt32(dt.Rows[0][0].ToString()) >= 1)
                            G1Head2[0] = "";
                        if (Convert.ToInt32(dt.Rows[0][0].ToString()) >= 2)
                            G1Head3[0] = "";
                        G1Width[0] = 0;
                        G1Align[0] = "";
                        G1Type[0] = "";
                        G1Color[0] = 0;
                        G1Etc[0] = "";
                        /********************1번째 숨김필드 정의******************/

                        //####################그리드 Head 순번######################
                        GHIdx1 = new string[G1RowCount - 1, 2];	// 그리드 Head Index 변수 길이
                        //string OldHeadName = null;
                        int OldHeadNameCount = 1;
                        //####################그리드 Head 순번######################
                        for (int i = 1; i < G1RowCount; i++)
                        {
                            G1Head1[i] = dt.Rows[i - 1][1].ToString();
                            if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) >= 1)
                                G1Head2[i] = dt.Rows[i - 1][2].ToString();
                            if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) >= 2)
                                G1Head3[i] = dt.Rows[i - 1][3].ToString();

                            G1Width[i] = Convert.ToInt32(dt.Rows[i - 1][4].ToString());
                            G1Align[i] = dt.Rows[i - 1][5].ToString();
                            G1Type[i] = dt.Rows[i - 1][6].ToString();
                            G1Color[i] = Convert.ToInt32(dt.Rows[i - 1][7].ToString());
                            if (G1Etc[i] == null)
                                G1Etc[i] = dt.Rows[i - 1][8].ToString();

                            G1SEQ[i] = Convert.ToInt32(dt.Rows[i - 1][9].ToString());


                            //####################그리드 Head 순번######################                            
                            OldHeadNameCount = 1;
                            GHIdx1[0, 0] = dt.Rows[0][1].ToString().ToUpper();
                            for (int k = 0; k < i - 1; k++)
                            {
                                if (dt.Rows[i - 1][1].ToString().ToUpper() == GHIdx1[k, 0].ToUpper())
                                {
                                    OldHeadNameCount++;
                                }
                                else if (GHIdx1[k, 0].ToUpper().LastIndexOf("_") > 0 && dt.Rows[i - 1][1].ToString().ToUpper() == GHIdx1[k, 0].ToUpper().Substring(0, GHIdx1[k, 0].ToUpper().LastIndexOf("_")))
                                {
                                    OldHeadNameCount++;
                                }
                            }

                            if (OldHeadNameCount > 1)
                            {
                                GHIdx1[i - 1, 0] = dt.Rows[i - 1][1].ToString().ToUpper() + "_" + OldHeadNameCount.ToString();	// 그리드 Head명
                            }
                            else
                            {
                                GHIdx1[i - 1, 0] = dt.Rows[i - 1][1].ToString().ToUpper();	// 그리드 Head명
                            }

                            GHIdx1[i - 1, 1] = Convert.ToString(i);			// 그리드 Head 위치
                            //####################그리드 Head 순번######################


                        }
                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                    }

                    /********************2번째 그리드(fpSpread2) 디자인 시작******************/
                    string Query2 = " usp_BAA004 'S3', @PFORM_ID='" + this.Name.ToString() + "', @PGRID_NAME='fpSpread2', @PIN_ID='" + SystemBase.Base.gstrUserID + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    DataTable dt2 = SystemBase.DbOpen.TranDataTable(Query2);
                    int G2RowCount = dt2.Rows.Count + 1;

                    if (G2RowCount > 1)
                    {
                        G2Head1 = new string[G2RowCount];// 첫번째 Head Text
                        G2Head2 = new string[G2RowCount];// 두번째 Head Text
                        G2Head3 = new string[G2RowCount];// 세번째 Head Text
                        G2Width = new int[G2RowCount];// Cell 넓이
                        G2Align = new string[G2RowCount];// Cell 데이타 정렬방식
                        G2Type = new string[G2RowCount];// CellType 지정
                        G2Color = new int[G2RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
                        G2Etc = new string[G2RowCount];
                        G2HeadCnt = Convert.ToInt32(dt2.Rows[0][0].ToString());
                        G2SEQ = new int[G2RowCount];// 키

                        /********************1번째 숨김필드 정의******************/
                        G2Head1[0] = "";
                        if (Convert.ToInt32(dt2.Rows[0][0].ToString()) >= 1)
                            G2Head2[0] = "";
                        if (Convert.ToInt32(dt2.Rows[0][0].ToString()) >= 2)
                            G2Head3[0] = "";
                        G2Width[0] = 0;
                        G2Align[0] = "";
                        G2Type[0] = "";
                        G2Color[0] = 0;
                        G2Etc[0] = "";
                        /********************1번째 숨김필드 정의******************/

                        //####################그리드 Head 순번######################
                        GHIdx2 = new string[G2RowCount - 1, 2];	// 그리드 Head Index 변수 길이
                        //string OldHeadName2 = null;
                        int OldHeadNameCount2 = 1;
                        //####################그리드 Head 순번######################
                        for (int i = 1; i < G2RowCount; i++)
                        {
                            G2Head1[i] = dt2.Rows[i - 1][1].ToString();
                            if (Convert.ToInt32(dt2.Rows[i - 1][0].ToString()) >= 1)
                                G2Head2[i] = dt2.Rows[i - 1][2].ToString();
                            if (Convert.ToInt32(dt2.Rows[i - 1][0].ToString()) >= 2)
                                G2Head3[i] = dt2.Rows[i - 1][3].ToString();

                            G2Width[i] = Convert.ToInt32(dt2.Rows[i - 1][4].ToString());
                            G2Align[i] = dt2.Rows[i - 1][5].ToString();
                            G2Type[i] = dt2.Rows[i - 1][6].ToString();
                            G2Color[i] = Convert.ToInt32(dt2.Rows[i - 1][7].ToString());
                            G2Etc[i] = dt2.Rows[i - 1][8].ToString();

                            G2SEQ[i] = Convert.ToInt32(dt2.Rows[i - 1][9].ToString());


                            //####################그리드 Head 순번######################
                            OldHeadNameCount2 = 1;
                            GHIdx2[0, 0] = dt2.Rows[0][1].ToString().ToUpper();
                            for (int k = 0; k < i - 1; k++)
                            {
                                if (dt2.Rows[i - 1][1].ToString().ToUpper() == GHIdx2[k, 0].ToUpper())
                                {
                                    OldHeadNameCount2++;
                                }
                                else if (GHIdx2[k, 0].ToUpper().LastIndexOf("_") > 0 && dt2.Rows[i - 1][1].ToString().ToUpper() == GHIdx2[k, 0].ToUpper().Substring(0, GHIdx2[k, 0].ToUpper().LastIndexOf("_")))
                                {
                                    OldHeadNameCount2++;
                                }

                            }

                            if (OldHeadNameCount2 > 1)
                            {
                                GHIdx2[i - 1, 0] = dt2.Rows[i - 1][1].ToString().ToUpper() + "_" + OldHeadNameCount2.ToString();	// 그리드 Head명
                            }
                            else
                            {
                                GHIdx2[i - 1, 0] = dt2.Rows[i - 1][1].ToString().ToUpper();	// 그리드 Head명
                            }

                            GHIdx2[i - 1, 1] = Convert.ToString(i);			// 그리드 Head 위치
                            //####################그리드 Head 순번######################
                        }
                        UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
                    }

                    /*----------------------------------------------------------------------------------*/
                    // 행추가 SUBMENU
                    // 툴바에 Add 메뉴가 비활성화시 행추가도 비활성화
                    string btnQuery = "usp_TOOLBARSET '" + SystemBase.Base.gstrUserID.ToString() + "','" + SystemBase.Base.RodeFormID.ToString() + "'";
                    DataTable Rowdt = SystemBase.DbOpen.NoTranDataTable(btnQuery);

                    if (Rowdt.Rows[0][4].ToString() == "0")
                    {
                        menuItem5.Enabled = false;
                        menuItem6.Enabled = false;
                    }
                    /*----------------------------------------------------------------------------------*/
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "화면 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region   fpSpread1_CellClick
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                {
                    if (e.ColumnHeader == true)
                    {
                        if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, e.Column].Text == "True")
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = false;
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread1.Sheets[0].Cells[i, e.Column].Value = false;
                                }
                            }
                        }
                        else
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = true;
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread1.Sheets[0].Cells[i, e.Column].Value = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region fpSpread2_CellClick
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                {
                    if (e.ColumnHeader == true)
                    {
                        if (fpSpread2.Sheets[0].ColumnHeader.Cells[0, e.Column].Text == "True")
                        {
                            fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = false;
                            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread2.Sheets[0].Cells[i, e.Column].Value = false;
                                }
                            }
                        }
                        else
                        {
                            fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = true;
                            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread2.Sheets[0].Cells[i, e.Column].Value = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region  사용자정의 그리드(넓이) 초기화
        private void menuItem2_Click(object sender, System.EventArgs e)
        {	// 왼쪽 그리드 초기화
            if (MessageBox.Show(SystemBase.Base.MessageRtn("SY012"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {	//초기화하시겠습니까?
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                cmd.Transaction = Trans;
                //cmd.CommandTimeout = 10000;
                try
                {
                    string Query = " usp_BAA004 'S5' ";
                    Query = Query + ", @pFORM_ID = '" + this.Name.ToString() + "'";
                    Query = Query + ", @pGRID_NAME = 'fpSpread2'";
                    Query = Query + ", @pIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                    Query = Query + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();
                    Trans.Commit();
                }
                catch//(Exception f)
                {
                    Trans.Rollback();
                    //RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                    //MessageBox.Show(RtnMsg);
                }

                try
                {
                    // 초기화된 그리드 넓이 화면에 적용
                    string Query2 = " usp_BAA004 'S3' ";
                    Query2 = Query2 + ", @pFORM_ID='" + this.Name.ToString() + "'";
                    Query2 = Query2 + ", @pGRID_NAME='fpSpread2'";
                    Query2 = Query2 + ", @pIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                    Query2 = Query2 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query2);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        fpSpread2.Sheets[0].Columns[i + 1].Width = Convert.ToInt32(dt.Rows[i]["HEAD_WIDTH"].ToString());
                        G2Width[i + 1] = Convert.ToInt32(dt.Rows[i]["HEAD_WIDTH"].ToString());
                    }
                    // 초기화된 그리드 넓이 화면에 적용
                }
                catch { }

                dbConn.Close();
            }
        }

        private void menuItem1_Click(object sender, System.EventArgs e)
        {	//오른쪽 그리드 넓이 초기화
            if (MessageBox.Show(SystemBase.Base.MessageRtn("SY012"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {	//초기화하시겠습니까?
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                cmd.Transaction = Trans;
                //cmd.CommandTimeout = 10000;
                try
                {
                    string Query = " usp_BAA004 'S5' ";
                    Query = Query + ", @pFORM_ID='" + this.Name.ToString() + "'";
                    Query = Query + ", @pGRID_NAME='fpSpread1'";
                    Query = Query + ", @pIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                    Query = Query + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";


                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();
                    Trans.Commit();


                }
                catch//(Exception f)
                {
                    Trans.Rollback();
                    //RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                    //MessageBox.Show(RtnMsg);
                }

                try
                {
                    // 초기화된 그리드 넓이 화면에 적용
                    string Query2 = " usp_BAA004 'S3' ";
                    Query2 = Query2 + ", @pFORM_ID='" + this.Name.ToString() + "'";
                    Query2 = Query2 + ", @pGRID_NAME='fpSpread1'";
                    Query2 = Query2 + ",@pIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                    Query2 = Query2 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query2);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].Columns[i + 1].Width = Convert.ToInt32(dt.Rows[i]["HEAD_WIDTH"].ToString());
                        G1Width[i + 1] = Convert.ToInt32(dt.Rows[i]["HEAD_WIDTH"].ToString());
                    }
                    // 초기화된 그리드 넓이 화면에 적용
                }
                catch { }

                dbConn.Close();
            }
        }
        #endregion

        #region 그리드 넓이 사용자정의 저장
        private void fpSpread1_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {	//오른쪽 그리드 넓이 사용자정의 저장
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            cmd.Transaction = Trans;
            //cmd.CommandTimeout = 10000;
            try
            {
                for (int i = 1; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    string Query = " usp_BAA004 'S4' ";
                    Query = Query + ", @pFORM_ID='" + this.Name.ToString() + "'";
                    Query = Query + ", @pGRID_NAME='fpSpread1'";
                    Query = Query + ", @pIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                    Query = Query + ", @PSEQ='" + G1SEQ[i].ToString() + "' ";
                    Query = Query + ", @PHEAD_WIDTH='" + fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Column.Width.ToString() + "' ";
                    Query = Query + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();

                    G1Width[i] = Convert.ToInt32(fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Column.Width.ToString());
                }
                Trans.Commit();
            }
            catch//(Exception f)
            {
                Trans.Rollback();
                //RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                //MessageBox.Show(RtnMsg);
            }
            dbConn.Close();
        }

        private void fpSpread2_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {	//왼쪽 그리드 넓이 사용자정의 저장
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            cmd.Transaction = Trans;
            //cmd.CommandTimeout = 10000;
            try
            {
                for (int i = 1; i < fpSpread2.Sheets[0].Columns.Count; i++)
                {
                    string Query = " usp_BAA004 'S4' ";
                    Query = Query + ", @pFORM_ID='" + this.Name.ToString() + "'";
                    Query = Query + ", @pGRID_NAME='fpSpread2'";
                    Query = Query + ", @pIN_ID='" + SystemBase.Base.gstrUserID + "' ";
                    Query = Query + ", @PSEQ='" + G2SEQ[i].ToString() + "' ";
                    Query = Query + ", @PHEAD_WIDTH='" + fpSpread2.Sheets[0].ColumnHeader.Cells[0, i].Column.Width.ToString() + "' ";
                    Query = Query + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();

                    G2Width[i] = Convert.ToInt32(fpSpread2.Sheets[0].ColumnHeader.Cells[0, i].Column.Width.ToString());

                }
                Trans.Commit();
            }
            catch//(Exception f)
            {
                Trans.Rollback();
                //RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                //MessageBox.Show(RtnMsg);
            }
            dbConn.Close();
        }
        #endregion

        #region 창 닫을때 저장되지 않은 데이타 확인 메세지
        private void FPCOMM2_2_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (strFormClosingMsg == true)
            {
                int UpCount = 0;
                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                        UpCount++;
                }

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                        UpCount++;
                }

                if (UpCount > 0)
                {
                    DialogResult Rtn = MessageBox.Show(SystemBase.Base.MessageRtn("SY011"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (Rtn != DialogResult.OK)
                        e.Cancel = true;
                }
            }
        }
        #endregion

        #region 자동정렬
        private void menuItem3_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].GetColumnAllowAutoSort(0))
            {
                fpSpread1.Sheets[0].SetColumnAllowAutoSort(-1, false);
            }
            else
            {
                fpSpread1.Sheets[0].SetColumnAllowAutoSort(-1, true);
            }
        }

        private void menuItem4_Click(object sender, System.EventArgs e)
        {
            if (fpSpread2.Sheets[0].GetColumnAllowAutoSort(0))
            {
                fpSpread2.Sheets[0].SetColumnAllowAutoSort(-1, false);
            }
            else
            {
                fpSpread2.Sheets[0].SetColumnAllowAutoSort(-1, true);
            }
        }
        #endregion

        #region 행추가
        private void menuItem5_Click(object sender, System.EventArgs e)
        {
            InsertRow frm = new InsertRow(txtRowCnt);
            frm.ShowDialog();

            this.Cursor = Cursors.WaitCursor;

            if (txtRowCnt.Text != "")
            {
                int Row = 0;

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    Row = fpSpread1.Sheets[0].ActiveRowIndex;
                }

                fpSpread1.ActiveSheet.DrawingContainer.Redraw = false;

                for (int i = Row; i < Row + Convert.ToInt32(txtRowCnt.Text); i++)
                {
                    RowInsExec();
                }

                fpSpread1.ActiveSheet.DrawingContainer.Redraw = true;

                if (Convert.ToInt32(txtRowCnt.Text) > 0)
                {
                    if (Row > 0)
                    {
                        fpSpread1.ActiveSheet.SetActiveCell(Row + 1, 1);
                    }
                    else
                    {
                        fpSpread1.ActiveSheet.SetActiveCell(Row, 1);
                    }

                    fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                }
            }

            this.Cursor = Cursors.Default;
        }

        private void menuItem6_Click(object sender, System.EventArgs e)
        {
            InsertRow frm = new InsertRow(txtRowCnt);
            frm.ShowDialog();

            this.Cursor = Cursors.WaitCursor;

            if (txtRowCnt.Text != "")
            {
                int Row = 0;

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    Row = fpSpread2.Sheets[0].ActiveRowIndex;
                }

                fpSpread2.ActiveSheet.DrawingContainer.Redraw = false;

                for (int i = Row; i < Row + Convert.ToInt32(txtRowCnt.Text); i++)
                {
                    RowInsExec();
                }

                fpSpread2.ActiveSheet.DrawingContainer.Redraw = true;

                if (Convert.ToInt32(txtRowCnt.Text) > 0)
                {
                    if (Row > 0)
                    {
                        fpSpread2.ActiveSheet.SetActiveCell(Row + 1, 1);
                    }
                    else
                    {
                        fpSpread2.ActiveSheet.SetActiveCell(Row, 1);
                    }

                    fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
                }
            }

            this.Cursor = Cursors.Default;
        }

        #endregion

        #region 그리드 ChangeEvent
        protected virtual void fpSpread1_ChangeEvent(int Row, int Col) { }
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            fpSpread1_ChangeEvent(e.Row, e.Column);
        }

        protected virtual void fpSpread2_ChangeEvent(int Row, int Col) { }
        private void fpSpread2_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            fpSpread2_ChangeEvent(e.Row, e.Column);
        }
        #endregion

        #region Ctrl+ C, V 설정
        private void fpSpread1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    e.Handled = true;
                    Clipboard.Clear();
                    //fpSpread1.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.AllHeaders;
                    //fpSpread1.Sheets[0].ClipboardCopy(FarPoint.Win.Spread.ClipboardCopyOptions.AsStringSkipHidden);
                    fpSpread1.Sheets[0].ClipboardCopy(ClipboardCopyOptions.All);
                }

                if (e.Control && e.KeyCode == Keys.V)
                {
                    fpSpread1.Sheets[0].ClipboardPaste(ClipboardPasteOptions.Values);

                    // 복사된 행의 열을 구하기 위하여 클립보드 사용.

                    IDataObject iData = Clipboard.GetDataObject();

                    string strClp = (string)iData.GetData(DataFormats.Text);

                    if (strClp != "" && strClp != null && strClp.Length > 0)
                    {
                        Regex rx1 = new Regex("\r\n");
                        string[] arrData = rx1.Split(strClp.ToString());


                        int DataCount = 0;
                        if (arrData.Length > 1)
                            DataCount = arrData.Length - 1;
                        else
                            DataCount = arrData.Length;

                        if (DataCount > 0)
                        {
                            int STRow = fpSpread1.ActiveSheet.ActiveRowIndex;
                            if (STRow < 0)
                                STRow = 0;

                            int ClipRowCount = STRow + DataCount;
                            if (fpSpread1.Sheets[0].RowCount < DataCount)
                                ClipRowCount = fpSpread1.Sheets[0].RowCount - STRow;

                            for (int i = STRow; i < ClipRowCount; i++)
                            {
                                if (i < fpSpread1.Sheets[0].RowCount
                                    || fpSpread1.Sheets[0].Cells[i, fpSpread1.ActiveSheet.ActiveColumnIndex].Locked != true)
                                {
                                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                                    {
                                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                    }

                                    fpSpread1_ChangeEvent(i, fpSpread1.ActiveSheet.ActiveColumnIndex);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                //MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Clipboard 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void fpSpread2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    e.Handled = true;
                    Clipboard.Clear();
                    //fpSpread1.ClipboardOptions = FarPoint.Win.Spread.ClipboardOptions.AllHeaders;
                    //fpSpread1.Sheets[0].ClipboardCopy(FarPoint.Win.Spread.ClipboardCopyOptions.AsStringSkipHidden);
                    fpSpread2.Sheets[0].ClipboardCopy(ClipboardCopyOptions.All);
                }

                if (e.Control && e.KeyCode == Keys.V)
                {
                    fpSpread2.Sheets[0].ClipboardPaste(ClipboardPasteOptions.Values);

                    // 복사된 행의 열을 구하기 위하여 클립보드 사용.

                    IDataObject iData = Clipboard.GetDataObject();

                    string strClp = (string)iData.GetData(DataFormats.Text);

                    if (strClp != "" && strClp != null && strClp.Length > 0)
                    {
                        Regex rx1 = new Regex("\r\n");
                        string[] arrData = rx1.Split(strClp.ToString());


                        int DataCount = 0;
                        if (arrData.Length > 1)
                            DataCount = arrData.Length - 1;
                        else
                            DataCount = arrData.Length;

                        if (DataCount > 0)
                        {
                            int STRow = fpSpread2.ActiveSheet.ActiveRowIndex;
                            if (STRow < 0)
                                STRow = 0;

                            int ClipRowCount = STRow + DataCount;
                            if (fpSpread2.Sheets[0].RowCount < DataCount)
                                ClipRowCount = fpSpread2.Sheets[0].RowCount - STRow;

                            for (int i = STRow; i < ClipRowCount; i++)
                            {
                                if (i < fpSpread2.Sheets[0].RowCount
                                    || fpSpread2.Sheets[0].Cells[i, fpSpread2.ActiveSheet.ActiveColumnIndex].Locked != true)
                                {
                                    if (fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                                    {
                                        fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                    }

                                    fpSpread2_ChangeEvent(i, fpSpread2.ActiveSheet.ActiveColumnIndex);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                //MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Clipboard 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Excel
        private void menuItem10_Click(object sender, System.EventArgs e)
        {
            try
            {
                UIForm.FPMake.ExcelMake(fpSpread1, this.Text.ToString() + "_1");
                ExcelExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Excel 저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuItem8_Click(object sender, System.EventArgs e)
        {
            try
            {
                UIForm.FPMake.ExcelMake(fpSpread2, this.Text.ToString() + "_2");
                ExcelExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Excel 저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region menuItem11_Click(Grid Print)
        private void menuItem11_Click(object sender, EventArgs e)
        {
            try
            {
                UIForm.FPMake.PrintMake(fpSpread1, this.Text);
                PrintExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Grid Print"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region menuItem12_Click(Grid Print)
        private void menuItem12_Click(object sender, EventArgs e)
        {
            try
            {
                UIForm.FPMake.PrintMake(fpSpread2, this.Text);
                PrintExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Grid Print"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 찾기
        private void menuItem13_Click(object sender, EventArgs e)
        {
            if (!frm.Created)
            {
                frm = new FindText(fpSpread1);
                frm.ShowDialog();
            }
            else
            {
                frm.Activate();
            }
        }

        private void menuItem15_Click(object sender, EventArgs e)
        {
            if (!frm.Created)
            {
                frm = new FindText(fpSpread2);
                frm.ShowDialog();
            }
            else
            {
                frm.Activate();
            }
        }

        private void FPCOMM2_2T_KeyDown(object sender, KeyEventArgs e)
        {
            FarPoint.Win.Spread.FpSpread grid;

            if (fpSpread2.Focused == true)
            {
                grid = fpSpread2;
            }
            else
            {
                grid = fpSpread1;
            }

            if (e.Control && e.KeyCode == Keys.F)
            {
                if (!frm.Created)
                {
                    frm = new FindText(grid);
                    frm.ShowDialog();
                }
                else
                {
                    frm.Activate();
                }
            }
        }
        #endregion
    }
}
