#region 작성정보
/*********************************************************************/
// 단위업무명 : 납품증빙등록
// 작 성 자 : 이재광
// 작 성 일 : 2014-9-22
// 작성내용 : 납품증빙 관련문서(품질문서) 조회/열람/등록/관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.Model;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Extensions.C1ComboExtension;
using EDocument.Network;
using EDocument.Spread;
using WNDW;

namespace SCM.SCM101
{
	public partial class SCM101 : UIForm.FPCOMM2
	{
		#region 필드
		/// <summary>문서카테고리 코드</summary>
		string docCtgCd = null;

		// 마스터 컬럼
		int colReqPart = -1;
		int colPlantCd = -1;
		int colScmMvmtNo = -1;
		int colPoNo = -1;
		int colPoSeq = -1;
		int colBarCode = -1;
		int colMvmtNoYn = -1;

		// 디테일 컬럼
		int colDocScmMvmtNo = -1;
		int colDocPoNo = -1;
		int colDocPoSeq = -1;
		int colDocBarCode = -1;
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

		/// <summary>현재 선택된 마스터 행</summary>
		int selectedMasterRow = -1;

		/// <summary>문서코드별 문서번호 유무</summary>
		Dictionary<string, string> docNoReqs = null;
		/// <summary>첨부파일목록 파일버튼 관리자</summary>
		FileButtonManager buttonManager;
		/// <summary>첨부문서표시 관리자</summary>
		AttachmentManager attachmentManager;
		#endregion

		#region 생성자
		public SCM101()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트 핸들러
		private void SCM101_Load(object sender, System.EventArgs e)
		{
			// 거래처 지정
			if (SystemBase.Base.gstrUserID == "KO132") // 마스터계정인 경우 거래처 변경 허가
			{
				txtCustCd.Tag = "";
				btnCust.Tag = "";
			}
			else txtCustCd.Value = SystemBase.Base.gstrUserID; // 거래처인 경우 거래처 변경 불가

			// 필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			// 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);// 공장

			// 그리드초기화
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); // 문서종류
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

			// 컬럼 인덱스
			SheetView masterSheet = fpSpread2.ActiveSheet;
			colReqPart = masterSheet.FindHeaderColumnIndex("주문형태");
			colPlantCd = masterSheet.FindHeaderColumnIndex("공장코드");
			colScmMvmtNo = masterSheet.FindHeaderColumnIndex("출고번호");
			colPoNo = masterSheet.FindHeaderColumnIndex("발주번호");
			colPoSeq = masterSheet.FindHeaderColumnIndex("발주순번");
			colBarCode = masterSheet.FindHeaderColumnIndex("바코드");
			colMvmtNoYn = masterSheet.FindHeaderColumnIndex("납품여부");
			SheetView sheet = fpSpread1.ActiveSheet;
			colDocScmMvmtNo = sheet.FindHeaderColumnIndex("출고번호");
			colDocPoNo = sheet.FindHeaderColumnIndex("발주번호");
			colDocPoSeq = sheet.FindHeaderColumnIndex("발주순번");
			colDocBarCode = sheet.FindHeaderColumnIndex("바코드");
			colDocId = sheet.FindHeaderColumnIndex("문서ID");
			colSvrPath = sheet.FindHeaderColumnIndex("서버경로");
			colSvrFnm = sheet.FindHeaderColumnIndex("서버파일명");
			colOrgFnm = sheet.FindHeaderColumnIndex("파일명") + 3; // 파일선택/미리보기/다운로드 버튼 다음이 파일명 컬럼
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

			// 첨부문서표시 관리자 초기화
			attachmentManager = new AttachmentManager(fpSpread2.ActiveSheet, "SPUR", null, "첨부문서코드", "필수문서코드");

			// 기타 세팅
			docNoReqs = SystemBase.Base.CreateDictionary("usp_T_DOC_CODE @pTYPE = 'S2', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"); // 문서번호 필수인 문서종류 정보
			NewExec();
		}
		#endregion

		#region 마스터 조회(출고목록)
		protected override void SearchExec()
		{
			if (!SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) return;

			this.Cursor = Cursors.WaitCursor;

			try
			{
				// 주문형태에 따른 초기화
				docCtgCd = GetSelectedDocCtgCd();
				attachmentManager.DocCtgCd = docCtgCd;

				// 조회
				string query = "usp_SCM101 'S1'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' "
					+ ", @pREQ_PART = '" + (rdoReqPartS.Checked ? "S" : "M") + "'";

				if (!string.IsNullOrEmpty(txtCustCd.Text)) query += ", @pCUST_CD = '" + txtCustCd.Text + "'";
				if (!string.IsNullOrEmpty(txtScmMvmtNo.Text)) query += ", @pSCM_MVMT_NO = '" + txtScmMvmtNo.Text + "'";
				if (!string.IsNullOrEmpty(dteSupplyDtFrom.Text)) query += ", @pSUPPLY_DT_FR = '" + dteSupplyDtFrom.Text + "'";
				if (!string.IsNullOrEmpty(dteSupplyDtTo.Text)) query += ", @pSUPPLY_DT_TO = '" + dteSupplyDtTo.Text + "'";
				if (!string.IsNullOrEmpty(txtItemCd.Text)) query += ", @pITEM_CD = '" + txtItemCd.Text + "'";
				if (rdoAttachYes.Checked) query += ", @pATT_YN = 'Y'";
				else if (rdoAttachNo.Checked) query += ", @pATT_YN = 'N'";

				UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 1, true);
				attachmentManager.PlantCode = cboPlantCd.SelectedValue.ToString();
				attachmentManager.AppendColumns(); 	// 스프레드에 컬럼을 추가하고 문서첨부표시

				selectedMasterRow = -1;
				fpSpread1.ActiveSheet.RowCount = 0;
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

		#region 디테일 조회(첨부문서목록)
		private void fpSpread2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SheetView sheet = fpSpread2.ActiveSheet;
			if (sheet.ActiveRowIndex == selectedMasterRow) return;
			selectedMasterRow = sheet.RowCount > 0 ? sheet.ActiveRowIndex : -1;
			SearchDocument();
		}

		/// <summary>
		/// 첨부문서를 조회해 첨부문서 그리드에 뿌립니다.
		/// </summary>
		private void SearchDocument()
		{
			if (selectedMasterRow < 0)
			{
				fpSpread1.ActiveSheet.RowCount = 0;
				return;
			}

			this.Cursor = Cursors.WaitCursor;

			try
			{
				SheetView masterSheet = fpSpread2.ActiveSheet;
				string query = "usp_T_DOC 'S1'"
					+ ", @pDOC_CTG_CD = '" + docCtgCd + "'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + GetSelectedPlantCd() + "'"
					+ ", @pATT_KEY1 = '" + GetFirstKey() + "'"
					+ ", @pATT_KEY2 = '" + GetSecondKey() + "'"
					+ ", @pATT_KEY3 = '" + GetThirdKey() + "'";
				string barcode = GetFourthKey();
				if (!string.IsNullOrEmpty(barcode)) query += ", @pATT_KEY4 = '" + barcode + "'";

				UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
				buttonManager.UpdateButtons();

				// 문서번호 배경색 업데이트
				SheetView sheet = fpSpread1.ActiveSheet;
				if (!CheckEditable(false))
					for (int row = 0; row < sheet.RowCount; row++)
					{
						sheet.Rows[row].Locked = true;
						sheet.Rows[row].BackColor = EDocument.UIColors.ReadonlyBackground;
					}
				else
					for (int row = 0; row < sheet.RowCount; row++)
						UpdateDocNoCellBackgroundColor(row);
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

		#region 초기화
		protected override void NewExec()
		{
			if (!string.IsNullOrEmpty(SystemBase.Base.gstrPLANT_CD))
				cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
			else cboPlantCd.SelectedIndex = 0;
			txtScmMvmtNo.Text = "";
			dteSupplyDtFrom.Value = DateTime.Now.AddMonths(-1);
			dteSupplyDtTo.Value = DateTime.Now;
			rdoAttachBoth.Checked = true;
			txtItemCd.Text = "";
		}
		#endregion

		#region 행추가
		protected override void RowInsExec()
		{
			SheetView sheet = fpSpread1.ActiveSheet;
			fpSpread1.Focus();

			// 필수문서 자동 추가 처리
			bool added = false;
			if (selectedMasterRow > -1) // 서브항목 단일선택시
				foreach (string docCd in attachmentManager.DocumentColumns.Keys)
				{
					if (attachmentManager.GetCellText(selectedMasterRow, docCd) == "0")
					{
						// 행추가후 자동입력 처리
						UIForm.FPMake.RowInsert(fpSpread1); // 행추가
						int row = sheet.ActiveRowIndex;
						sheet.Cells[row, colDocCd].Value = docCd;
						sheet.Cells[row, colDocNm].Value = docCd;
						sheet.Cells[row, colRegUsrId].Value = SystemBase.Base.gstrUserID;
						sheet.Cells[row, colRegUsrNm].Value = SystemBase.Base.gstrUserName;
						buttonManager.UpdateButtons(row); // 버튼 업데이트
						UpdateDocNoCellBackgroundColor(row); // 문서번호 필수색 업데이트
						added = true;
					}
				}

			// 그냥 행추가 처리
			if (!added)
			{
				// 행추가후 자동입력 처리
				UIForm.FPMake.RowInsert(fpSpread1); // 행추가
				int row = sheet.ActiveRowIndex;
				sheet.Cells[row, colRegUsrId].Value = SystemBase.Base.gstrUserID;
				sheet.Cells[row, colRegUsrNm].Value = SystemBase.Base.gstrUserName;
				buttonManager.UpdateButtons(row); // 버튼 업데이트
			}
		}
		#endregion

		#region 행삭제
		protected override void DelExec()
		{
			SheetView sheet = fpSpread1.ActiveSheet;
			if (sheet.RowCount < 1) return;
			CellRange[] ranges = sheet.GetSelections();
			if (ranges.Length == 0) return;

			// 선택된 행에 대해 루프
			foreach (CellRange range in ranges)
				for (int row = range.Row; row < range.Row + range.RowCount; row++)
					if (!CheckEditable(true)) return;

			base.DelExec();
		}
		#endregion

		#region 저장
		protected override void SaveExec()
		{
			SheetView sheet = fpSpread1.ActiveSheet;
			SheetView masterSheet = fpSpread2.ActiveSheet;
			if (sheet.Rows.Count < 1) return;
			RowDataList attKeys = null;
			if (sheet.CheckRowInserted())
			{
				attKeys = masterSheet.GetCheckedRowData(new int[] { colScmMvmtNo, colPoNo, colPoSeq, colBarCode, colPlantCd });
				if (attKeys == null)
				{
					MessageBox.Show("먼저 첨부하려는 출고품목에 체크를 하십시오.", "첨부파일 저장", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
			}

			//그리드 상단 필수 체크
			if (!SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) return;

			this.Cursor = Cursors.WaitCursor;
			fpSpread1.Focus();

			string resultCode = "WR", resultMessage = "P0000"; //처리할 내용이 없습니다.
			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			try
			{
				List<int> newDocIds = new List<int>(); // 새로 추가된 문서레코드의 ID

				// 모든 체크건에 첨부 저장
				int attIndex = 0;
				do // while AttIndex
				{
					RowData key = attKeys != null ? attKeys[attIndex] : new RowData();
					int insertIndex = 0;

					//행수만큼 처리
					for (int row = 0; row < sheet.RowCount; row++)
					{
						string strHead = fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text;
						if (string.IsNullOrEmpty(strHead)) continue;

						string strGbn = "";
						switch (strHead)
						{
							case "U": strGbn = "U1"; break;
							case "I": strGbn = "I1"; break;
							case "D": strGbn = "D1"; break;
							default: continue;
						}

						string query = "usp_T_DOC '" + strGbn + "'"
							+ ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
						switch (strHead)
						{
							case "D":
								query += ", @pDOC_ID = " + sheet.Cells[row, colDocId].Value;
								break;

							case "U":
								query += ", @pDOC_ID = " + sheet.Cells[row, colDocId].Value
									+ ", @pDOC_CD = '" + sheet.Cells[row, colDocNm].Value + "'"
									+ ", @pDOC_NO = '" + sheet.Cells[row, colDocNo].Text + "'"
									+ ", @pREMARK = '" + sheet.Cells[row, colRemark].Text + "'";
								break;

							case "I":
								// 인서트인 경우 체크된 대상 모두에 첨부
								if (attIndex == 0)
								{
									// 새 파일 첨부 쿼리
									query += ", @pDOC_CTG_CD = '" + docCtgCd + "'"
										+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
										+ ", @pPLANT_CD = '" + key.Values[4] + "'"
										+ ", @pDOC_CD = '" + sheet.Cells[row, colDocNm].Value + "'"
										+ ", @pDOC_NO = '" + sheet.Cells[row, colDocNo].Text + "'"
										+ ", @pREMARK = '" + sheet.Cells[row, colRemark].Text + "'"
										+ ", @pATT_KEY = '" + key.GetKeyCombination(4) + "'"
										+ ", @pATT_KEY1 = '" + key.Values[0] + "'"
										+ ", @pATT_KEY2 = '" + key.Values[1] + "'"
										+ ", @pATT_KEY3 = '" + key.Values[2] + "'";
								}
								else
								{
									// 기존 파일 첨부 쿼리
									query += ", @pDOC_CTG_CD = '" + docCtgCd + "'"
										+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
										+ ", @pPLANT_CD = '" + key.Values[4] + "'"
										+ ", @pDOC_PID = " + newDocIds[insertIndex]
										+ ", @pATT_KEY = '" + key.GetKeyCombination(4) + "'"
										+ ", @pATT_KEY1 = '" + key.Values[0] + "'"
										+ ", @pATT_KEY2 = '" + key.Values[1] + "'"
										+ ", @pATT_KEY3 = '" + key.Values[2] + "'";
								}
								// 공통파라메터
								if (!string.IsNullOrEmpty(key.Values[3])) query += ", @pATT_KEY4 = '" + key.Values[3] + "'";
								break;
						}

						// 문서정보 저장
						DataSet ds = SystemBase.DbOpen.TranDataSet(query, dbConn, Trans);
						resultCode = ds.Tables[0].Rows[0][0].ToString();
						resultMessage = ds.Tables[0].Rows[0][1].ToString();
						if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

						// 새 문서 추가인 경우 파일 업로드 및 정보 업데이트
						if (strHead == "I" && attIndex == 0)
						{
							// 최초로 생성된 첨부파일인 경우만 파일정보 저장
							int newDocId = Convert.ToInt32(ds.Tables[0].Rows[0][2]); // 새로 추가된 문서레코드 ID
							newDocIds.Add(newDocId); // 새 문서레코드 ID 저장
							if (Server.UploadDocumentFile(docCtgCd, sheet.Cells[row, colDocCd].Text, newDocId, Convert.ToDateTime(ds.Tables[0].Rows[0][3]), buttonManager.GetAttachedFilename(row), dbConn, Trans) != Server.UploadResultState.Ok)
							{ Trans.Rollback(); goto Exit; }; // 실패시 롤백

							insertIndex++;
						}
					} // for row

					attIndex++;
				} while (attKeys != null && attIndex < attKeys.Count);
				Trans.Commit();

				// 첨부상태가 변경된 입고품목의 첨부대상키 리스트업
				RowDataList updatedKeys = new RowDataList();
				RowDataList deletedKeys = sheet.GetDeletedRowData(new int[] { colDocScmMvmtNo, colDocPoNo, colDocPoSeq, colDocBarCode }); // 문서가 삭제된 첨부대상키 추출
				if (deletedKeys != null)
				{
					// 삭제된 문서행으로부터 첨부대상키 추출
					foreach (RowData rdata in deletedKeys)
					{
						int row = masterSheet.FindText(new int[] { colScmMvmtNo, colPoNo, colPoSeq, colBarCode }, rdata.Values);
						if (row > -1)
						{
							rdata.Row = row;
							updatedKeys.Add(rdata);
						}
					}
				}
				updatedKeys.Add(sheet.CheckRowInserted() ? masterSheet.GetCheckedRowData(new int[] { colScmMvmtNo, colPoNo, colPoSeq, colBarCode }) : null); // 행추가된 첨부대상키 추출

				// 첨부정보 다시 로드
				if (updatedKeys.Count > 0)
					foreach (RowData key in updatedKeys)
						attachmentManager.ReloadData(key.Row, key.Values);
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
				SearchDocument();
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

		#region 공유기능
		/// <summary>
		/// 지정한 행이 편집 가능한지 확인합니다.
		/// </summary>
		/// <param name="row">확인할 행 인덱스</param>
		/// <param name="showAlert">편집 불가인 경우 경고 메시지를 표시할지 여부</param>
		/// <returns></returns>
		bool CheckEditable(bool showAlert)
		{
			if (fpSpread2.ActiveSheet.Cells[selectedMasterRow, colMvmtNoYn].Text == "Y") // 납품 여부
			{
				if (showAlert) MessageBox.Show("납품이 완료되어 선택한 첨부문서를 삭제할 수 없습니다.", "행삭제", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
			}
			/* // 등록자만 편집 허가용
			else if (fpSpread1.ActiveSheet.Cells[row, colRegUsrId].Text != SystemBase.Base.gstrUserID) // 등록자 여부
			{
				if (showAlert) MessageBox.Show("등록자가 아니므로 해당 항목을 삭제할 수 없습니다.", "행삭제", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
			}
			*/
			return true;
		}

		/// <summary>
		/// 현재 선택된 마스터 항목에 대한 첨부키조합을 구합니다.
		/// </summary>
		/// <param name="procRow">공정목록 행 인덱스</param>
		/// <returns></returns>
		string GetSelectedAttKeyCombination()
		{
			return GetFirstKey() + "/" + GetSecondKey() + "/" + GetThirdKey() + "/" + GetFourthKey(); // 키 = "납품번호/수주번호/수주순번/바코드"
		}

		/// <summary>
		/// 현재 선택된 주문형태로 부터 첨부대상 문서카테고리를 반환합니다.
		/// </summary>
		/// <returns>문서카테고리</returns>
		string GetSelectedDocCtgCd()
		{
			return rdoReqPartM.Checked ? "SPUR" : "SOUT";
		}

		/// <summary>
		/// 현재 선택된 마스터 항목에 대한 공장코드를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetSelectedPlantCd()
		{
			SheetView sheet = fpSpread2.ActiveSheet;
			return sheet.RowCount > 0 ? sheet.Cells[0, colPlantCd].Text : "";
		}

		/// <summary>
		/// 현재 선택된 마스터 그리드의 첫 번째 첨부키를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetFirstKey()
		{
			return selectedMasterRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedMasterRow, colScmMvmtNo].Text : "";
		}

		/// <summary>
		/// 현재 선택된 마스터 그리드의 두 번째 첨부키를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetSecondKey()
		{
			return selectedMasterRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedMasterRow, colPoNo].Text : "";
		}

		/// <summary>
		/// 현재 선택된 마스터 그리드의 세 번째 첨부키를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetThirdKey()
		{
			return selectedMasterRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedMasterRow, colPoSeq].Text : "";
		}

		/// <summary>
		/// 현재 선택된 마스터 그리드의 세 번째 첨부키를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetFourthKey()
		{
			return selectedMasterRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedMasterRow, colBarCode].Text : "";
		}

		/// <summary>
		/// 문서번호셀의 필수여부에 따른 배경색을 업데이트합니다.
		/// </summary>
		/// <param name="row"></param>
		void UpdateDocNoCellBackgroundColor(int row)
		{
			SheetView sheet = fpSpread1.ActiveSheet;
			Cell docNoCell = sheet.Cells[row, colDocNo];
			if (docNoReqs[sheet.Cells[row, colDocCd].Text].ToUpper() == "Y")
				docNoCell.BackColor = SystemBase.Validation.Kind_LightCyan;
			else
				docNoCell.BackColor = Color.White;
		}
		#endregion

		#region 그리드 이벤트 핸들러
		/// <summary>
		/// 디테일 그리드 셀값 변경 핸들러(첨부문서 목록)
		/// </summary>
		protected override void fpSpread1_ChangeEvent(int row, int col)
		{
			try
			{
				// 문서종류
				if (col == colDocNm)
				{
					SheetView sheet = fpSpread1.ActiveSheet;
					sheet.Cells[row, colDocCd].Value = (string)sheet.Cells[row, colDocNm].Value; // 문서코드셀 업데이트
					UpdateDocNoCellBackgroundColor(row); // 문서번호 배경색 업데이트
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

		#region 컨트롤 이벤트 핸들러
		/// <summary>
		/// 공급처 팝업
		/// </summary>
		private void btnCust_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW002 pu = new WNDW002(txtCustCd.Text, "P");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtCustCd.Text = Msgs[1].ToString();
					txtCustNm.Value = Msgs[2].ToString();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		/// <summary>
		/// 품목 팝업
		/// </summary>
		private void btnItem_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
				string[] strSearch = new string[] { txtItemCd.Text, txtItemNm.Text };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
				pu.Width = 500;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					txtItemCd.Text = pu.ReturnValue[1].ToString();
					txtItemNm.Value = pu.ReturnValue[2].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 입고번호 팝업
		/// </summary>
		private void btnMvmtNo_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW019 dialog = new WNDW019();
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					string[] Msgs = dialog.ReturnVal;
					txtScmMvmtNo.Text = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 공급처 코드 입력 처리
		/// </summary>
		private void txtCustCd_TextChanged(object sender, EventArgs e)
		{
			txtCustNm.Value = string.IsNullOrEmpty(txtCustCd.Text) ? "" : SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		/// <summary>
		/// 품목코드 입력
		/// </summary>
		private void txtItemCd_TextChanged(object sender, EventArgs e)
		{
			try
			{
				txtItemNm.Value = string.IsNullOrEmpty(txtItemCd.Text) ? "" : SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

	}

}