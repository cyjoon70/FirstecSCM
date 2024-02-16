using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.Model;
using FarPoint.Win.Spread.CellType;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Network;
using EDocument.Spread;

namespace SCM.SCM031
{
	public partial class SCM031P1 : UIForm.FPCOMM1
	{

        #region Field
        string NoticeSeq = string.Empty;
        bool GwStatus = true;

        /// <summary>문서카테고리 코드</summary>
        const string docCtgCd = "SCM";

        // 디테일 그리드 컬럼(문서 목록)
        int colDocId = -1;
        int colSvrPath = -1;
        int colSvrFnm = -1;
        int colOrgFnm = -1;
        int colDocCd = -1;
        int colDocNm = -1;
        int colDocNo = -1;
        int colRevNo = -1;
        int colRemark = -1;
        int colRegUsrId = -1;
        int colRegUsrNm = -1;

        /// <summary>문서코드별 문서번호 유무</summary>
        Dictionary<string, string> docNoReqs = null;
        /// <summary>첨부파일목록 파일버튼 관리자</summary>
        FileButtonManager buttonManager;
        /// <summary>첨부문서표시 관리자</summary>
        AttachmentManager attachmentManager;

        string[] returnVal = null;          // 2022.05.20. hma 추가
        string strSaveFlag = "";            // 2022.05.20. hma 추가
        int iDocCnt = 0;                    // 2022.05.20. hma 추가
        #endregion

        # region 생성자
        public SCM031P1()
        {
            InitializeComponent();
        }

        public SCM031P1(string NOTICE_SEQ, bool GW_STATUS) : this()
        {
            this.NoticeSeq = NOTICE_SEQ;
            this.GwStatus = GW_STATUS;

            this.Size = new System.Drawing.Size(1240, 785);
        }
        #endregion

        # region Method
        /// <summary>
        /// 임시로 다운로드한 파일을 모두 삭제합니다.
        /// </summary>
        void ViewDeleteTempFiles()
        {
            foreach (FileInfo f in new DirectoryInfo(Path.GetTempPath()).GetFiles(ViewGetTempFilenamePrefix() + "*.*")) // 프리픽스파일 모두 삭제
            {
                try { f.Delete(); }
                catch { }
            }
        }
        /// <summary>
        /// 임시파일명의 프리픽스로 사용할 고정된 문자열을 반환합니다.
        /// </summary>
        /// <returns></returns>
        string ViewGetTempFilenamePrefix()
        {
            return string.Format("{0:X}", this.GetHashCode()) + "_";
        }
        #endregion

        #region Form 핸들러
        /// <summary>
        /// Form Load
        /// </summary>
        private void SCM031P1_Load(object sender, System.EventArgs e)
        {
            try
            {
                
                UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "공지사항 첨부파일";
                this.Width = 1180;
                this.Height = 530;

                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_CTG_CD = 'SCM', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); // 문서종류

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                SheetView sheet = fpSpread1.ActiveSheet;
                colDocId = sheet.FindHeaderColumnIndex("문서ID");
                colSvrPath = sheet.FindHeaderColumnIndex("서버경로");
                colSvrFnm = sheet.FindHeaderColumnIndex("서버파일명");
                colOrgFnm = sheet.FindHeaderColumnIndex("파일명") + 3;     // 파일선택 버튼, 미리보기 버튼, 다운로드 버튼 다음이 파일명 컬럼
                colDocCd = sheet.FindHeaderColumnIndex("문서코드");
                colDocNm = sheet.FindHeaderColumnIndex("문서종류");
                colDocNo = sheet.FindHeaderColumnIndex("문서번호");
                colRevNo = sheet.FindHeaderColumnIndex("개정번호");
                colRemark = sheet.FindHeaderColumnIndex("비고");
                colRegUsrId = sheet.FindHeaderColumnIndex("등록자ID");
                colRegUsrNm = sheet.FindHeaderColumnIndex("등록자");

                // 첨부파일목록 파일버튼 관리자 초기화
                buttonManager = new FileButtonManager(fpSpread1.ActiveSheet, FileButtonManager.ServerFileType.DocumentFile)
                {
                    ServerPathColumnIndex = colSvrPath,
                    ServerFilenameColumnIndex = colSvrFnm,
                    FileSelectButtonColumnIndex = colOrgFnm - 3,
                    FileViewButtonColumnIndex = colOrgFnm - 2,
                    FileDownloadButtonColumnIndex = colOrgFnm - 1,
                    FilenameColumnIndex = colOrgFnm,
                    DocTypeNameColumnIndex = colDocNm,
                    DocRevisionColumnIndex = colRevNo,
                    DocNumberColumnIndex = colDocNo,
                };

                //picDoc.SizeMode = PictureBoxSizeMode.AutoSize;      // 2022.02.23 hma 추가

                SearchExec();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 조회
        /// </summary>        
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string query = "usp_T_DOC 'S1'"
                    + ", @pDOC_CTG_CD = 'SCM'"
                    + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                    + ", @pATT_KEY = '" + this.NoticeSeq + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                buttonManager.UpdateButtons(); // 버튼 업데이트

                SheetView sheet = fpSpread1.ActiveSheet;
                ((TextCellType)sheet.Columns[colRevNo].CellType).MaxLength = 5; // 개정번호
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 이벤트 핸들러
        /// <summary>
        /// 그리드 Change 이벤트
        /// </summary>
        protected override void fpSpread1_ChangeEvent(int row, int col)
        {
            try
            {
                // 문서종류
                if (col == colDocNm)
                {
                    SheetView sheet = fpSpread1.ActiveSheet;
                    sheet.Cells[row, colDocCd].Value = sheet.Cells[row, colDocNm].Value;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 증빙문서 저장여부 및 증빙문서건수 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string SaveYn, string DocCnt)
        {
            returnVal = new string[2];
            returnVal[0] = SaveYn;
            returnVal[1] = DocCnt;
        }
        #endregion

        #region 증빙문서저장여부 체크해서 증빙문서건수 리턴
        private void SCM031P1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (strSaveFlag == "Y")      // 증빙문서 저장을 한경우
            {
                RtnStr("Y", iDocCnt.ToString());
            }
            else
            {
                RtnStr("N", "");
            }
        }
		#endregion

		#endregion

	}
}
