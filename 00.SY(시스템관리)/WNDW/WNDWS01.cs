﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.Model;
using FarPoint.Win.Spread.CellType;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Network;
using EDocument.Spread;

namespace WNDW
{
    public partial class WNDWS01 : UIForm.FPCOMM1
    {
        #region Field
        string AttKey = string.Empty;
        string AttKey1 = string.Empty;
        string AttKey2 = string.Empty;
        string AttKey3 = string.Empty;
        string AttKey4 = string.Empty;
        string ApprId = string.Empty;
        string RandNo = string.Empty;
        string Titles = string.Empty;

        /// <summary>
        /// 파일 업로드 권한
        /// </summary>
        bool GwStatus = true;

        const string docCtgCd = "SCM";  //SCM

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

        # region Initialize
        public WNDWS01()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="strAttKey"></param>
        /// <param name="strAttKey1"></param>
        /// <param name="strAttKey2"></param>
        /// <param name="strAttKey3"></param>
        /// <param name="strAttKey4"></param>
        /// <param name="APPR_ID"></param>
        /// <param name="GW_STATUS"></param>파일 권한
        /// <param name="strRandNo"></param>
        /// <param name="strTitle"></param>
        public WNDWS01(string strAttKey, string strAttKey1, string strAttKey2, string strAttKey3, string strAttKey4,
                        string APPR_ID, bool GW_STATUS, string strRandNo, string strTitle) : this()
        {
            this.AttKey = strAttKey;
            this.AttKey1 = strAttKey1;
            this.AttKey2 = strAttKey2;
            this.AttKey3 = strAttKey3;
            this.AttKey4 = strAttKey4;
            this.ApprId = APPR_ID;
            this.GwStatus = GW_STATUS;
            this.RandNo = strRandNo;
            this.Titles = strTitle;

            this.Size = new System.Drawing.Size(1240, 650);
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
        private void WNDWS01_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (GwStatus)
                    UIForm.Buttons.ReButton("010111010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                else
                    UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);


                this.Text = this.Titles + " 첨부파일";

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
            string query = string.Empty;

            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (string.IsNullOrEmpty(this.AttKey))
                {
                    query = "usp_T_DOC 'ST1'"
                    + ", @pDOC_CTG_CD   = 'SCM'"
                    + ", @pCO_CD        = '" + SystemBase.Base.gstrCOMCD + "'"
                    + ", @pTMP_NO       = '" + this.RandNo + "'";
                }
                else
                {
                    query = "usp_T_DOC 'S1'"
                    + ", @pDOC_CTG_CD   = 'SCM'"
                    + ", @pCO_CD        = '" + SystemBase.Base.gstrCOMCD + "'"
                    + ", @pATT_KEY      = '" + this.AttKey + "'";
                }

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool GetAuth()
        {
            bool bResult = true;

            if (!GwStatus)
            {
                MessageBox.Show("권한이 없습니다.");
                bResult = false;
            }

            return bResult;
        }

        /// <summary>
        /// 행 추가
        /// </summary>
        protected override void RowInsExec()
        {
            if (!GetAuth()) return;

            SheetView sheet = fpSpread1.ActiveSheet;
            fpSpread1.Focus();

            UIForm.FPMake.RowInsert(fpSpread1); // 행추가
            int newRow = sheet.ActiveRowIndex;
            sheet.Cells[newRow, colRegUsrId].Value = SystemBase.Base.gstrUserID;
            sheet.Cells[newRow, colRegUsrNm].Value = SystemBase.Base.gstrUserName;
            buttonManager.UpdateButtons(newRow); // 버튼 업데이트
        }

        /// <summary>
        /// 행 삭제
        /// </summary>
        protected override void DelExec()
        {
            if (!GetAuth()) return;

            SheetView sheet = fpSpread1.ActiveSheet;
            if (sheet.RowCount < 1) return;
            CellRange[] ranges = sheet.GetSelections();
            if (ranges.Length == 0) return;

            base.DelExec();
        }

        /// <summary>
        /// 저장
        /// </summary>
        protected override void SaveExec()
        {
            if (!GetAuth()) return;

            SheetView sheet = fpSpread1.ActiveSheet;        // 2022.04.19. hma 수정: 선언 위치 이동

            //SheetView sheet = fpSpread1.ActiveSheet;      // 2022.04.19. hma 수정: 선언 위치를 위로 이동하고 여기는 주석 처리
            if (sheet.Rows.Count < 1) return;
            if (!SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) return;

            this.Cursor = Cursors.WaitCursor;
            fpSpread1.Focus();

            string strResultMsg = "";       // 2022.02.19. hma 추가

            string resultCode = "WR", resultMessage = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                for (int row = 0; row < sheet.RowCount; row++)
                {
                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text;
                    if (string.IsNullOrEmpty(strHead)) continue;

                    string strGbn = string.Empty;
                    switch (strHead)
                    {
                        case "U": strGbn = "U1"; break;
                        case "I": strGbn = "I1"; break;
                        case "D": strGbn = "D1"; break;
                        default: continue;
                    }

                    if (strHead == "D")
                    {
                        // 문서 삭제
                        string strSql = string.Format("usp_T_DOC @pTYPE = '" + strGbn + "', @pDOC_ID = {0}", sheet.Cells[row, colDocId].Value);

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        strResultMsg = resultMessage;       // 2022.02.18. hma 추가
                    }
                    else
                    {
                        string query = "usp_T_DOC @pTYPE = '" + strGbn + "'";
                        if (strHead == "I") // 새로 추가
                        {
                            query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                                    + ", @pPLANT_CD = 'FS1' "
                                    + ", @pDOC_CTG_CD = '" + docCtgCd + "'"
                                    + ", @pDOC_CD = '" + sheet.Cells[row, colDocCd].Text + "'"     //txtDocCode.Text + "'";
                                    + ", @pFST_IN = 'Y'"
                                    + ", @pAPPR_ID = '" + this.ApprId + "'"
                                    + ", @pREMARK = '" + sheet.Cells[row, colRemark].Text + "'"
                                    + ", @pIN_ID = '" + SystemBase.Base.gstrUserID + "'"
                                    + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                            // 부모 키 값 생성이전 첨부파일을 동시에 저장하기 위함
                            if (string.IsNullOrEmpty(this.AttKey))
                            {
                                query += ", @pATT_KEY = '" + this.RandNo + "'"
                                    + ", @pATT_KEY1 = '" + this.RandNo + "'"
                                    + ", @pATT_KEY2 = '" + this.AttKey2 + "'"
                                    + ", @pATT_KEY3 = '" + this.AttKey3 + "'"
                                    + ", @pATT_KEY4 = '" + this.AttKey4 + "'"
                                    + ", @pTMP_NO = '" + this.RandNo + "'";
                            }
                            else
                            {
                                query += ", @pATT_KEY = '" + this.AttKey + "'"
                                    + ", @pATT_KEY1 = '" + this.AttKey1 + "'"
                                    + ", @pATT_KEY2 = '" + this.AttKey2 + "'"
                                    + ", @pATT_KEY3 = '" + this.AttKey3 + "'"
                                    + ", @pATT_KEY4 = '" + this.AttKey4 + "'";
                            }
                        }
                        else // 내용 변경
                        {
                            query += ", @pDOC_ID = '" + sheet.Cells[row, colDocId].Text + "'"
                                    + ", @pREMARK = '" + sheet.Cells[row, colRemark].Text + "'"
                                    + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        }

                        // 문서정보 저장
                        DataSet ds = SystemBase.DbOpen.TranDataSet(query, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        strResultMsg = resultMessage;       // 2022.02.18. hma 추가

                        // 새 문서 추가인 경우 파일 업로드 및 정보 업데이트
                        if (strHead == "I")
                        {
                            //if (Server.UploadDocumentFile(docCtgCd, sheet.Cells[row, colDocCd].Text, Convert.ToInt32(ds.Tables[0].Rows[0][2]), Convert.ToDateTime(ds.Tables[0].Rows[0][3]), buttonManager.GetAttachedFilename(row), dbConn, Trans) != Server.UploadResultState.Ok)
                            if (Server.UploadDocumentFile(docCtgCd, sheet.Cells[row, colDocCd].Text, Convert.ToInt32(ds.Tables[0].Rows[0][2]), Convert.ToDateTime(ds.Tables[0].Rows[0][3]), buttonManager.GetAttachedFilename(row), dbConn, Trans) != Server.UploadResultState.Ok)
                            { Trans.Rollback(); goto Exit; }; // 실패시 롤백
                        }
                    }
                }

                resultMessage = strResultMsg;       // 문제 없을 경우 등록 관련 결과 메시지로 보여지도록 함.
                strSaveFlag = "Y";

                if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();

                //// 품목의 첨부문서 코드문자열 업데이트
                //attachmentManager.ReloadData(0, new string[] { this.NoticeSeq});
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                Trans.Rollback();
                resultCode = "ER";
                resultMessage = e.Message;
            }
        Exit:
            dbConn.Close();
            if (resultCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(resultMessage), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SearchExec();
            }
            else if (resultCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(resultMessage), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(resultMessage), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        // 2022.05.20. hma 추가(Start)
        #region 증빙문서 저장여부 및 증빙문서건수 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string SaveYn, string DocCnt)
        {
            returnVal = new string[2];
            returnVal[0] = SaveYn;
            returnVal[1] = DocCnt;
        }
        #endregion

        #region WNDWS01_FormClosing(): 증빙문서저장여부 체크해서 증빙문서건수 리턴
        private void WNDWS01_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
                RtnStr("Y", fpSpread1.Sheets[0].Rows.Count.ToString());
            else
                RtnStr("N", "");
        }
        #endregion

        #endregion
    }
}
