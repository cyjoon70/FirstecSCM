#region 작성정보
/*********************************************************************/
// 단위업무명 : Lot 분할
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-10-16
// 작성내용 : Lot 분할 처리
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
using FarPoint.Win.Spread.CellType;
using System.Data.SqlClient;

namespace SCM.SCM007
{
	public partial class SCM007P1 : UIForm.FPCOMM1
	{

		#region 변수
		public bool bSave = true;
		public string strSCM_MVMT_NO = string.Empty;
		public string strPO_NO = string.Empty;
		public string strPO_SEQ = string.Empty;
		public decimal dLotSum = 0;
		public DataTable dt = new DataTable();
		public DataTable dtM1 = new DataTable();
		public SCM007 M1 = new SCM007();

		// lot 분할/수정/삭제 팝업화면에서 CUD 발생여부 체크. 발생했다면 Parent Form Reload
		public string strSaveYN = string.Empty;
		#endregion

		#region 생성자
		public SCM007P1()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void SCM007P1_Load(object sender, EventArgs e)
		{
			this.Text = "Lot 분할/수정/삭제";

			SystemBase.Validation.GroupBox_Setting(groupBox1);

			if (bSave == false)
			{
				UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
			}
			else
			{
				UIForm.Buttons.ReButton("111011010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
			}

			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
			SearchExec();
		}
		#endregion
		
		#region 조회
		protected override void SearchExec()
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			int k = 0;

			try
			{

				if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
				{
					string strQuery = " usp_SCM007 ";
					strQuery += " @pTYPE = 'P5' ";
					strQuery += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ",@pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
					strQuery += ",@pSCM_MVMT_NO = '" + strSCM_MVMT_NO + "' ";
					strQuery += ",@pPO_NO = '" + strPO_NO + "' ";
					strQuery += ",@pPO_SEQ = '" + strPO_SEQ + "' ";

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

					if (fpSpread1.Sheets[0].Rows.Count > 0)
					{
						for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
						{
							fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
						}
					}

					// 기 입력된 정보 매칭
					if (dtM1.Rows.Count > 0)
					{
						for (int i = 0; i <= dtM1.Rows.Count - 1; i++)
						{
							if (
								(string.Compare(strPO_NO, dtM1.Rows[i]["PO_NO"].ToString(), true) == 0) &&
								(string.Compare(strPO_SEQ, dtM1.Rows[i]["PO_SEQ"].ToString(), true) == 0)
							   )
							{
																
								// 조회된 행과 기 입력된 행의 수가 같으면
								if (k > 0)
								{
									RCopyExec();
									SetCellStyle(k);
								}

								fpSpread1.Sheets[0].Cells[k, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text = dtM1.Rows[i]["LOT_NO"].ToString();
								fpSpread1.Sheets[0].Cells[k, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수량")].Value = Convert.ToDecimal(dtM1.Rows[i]["RCPT_QTY"]);

								k++;

							}

						}
					}

					if (bSave == false)
					{
						for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
						{
							for (int j = 0; j < fpSpread1.Sheets[0].Columns.Count; j++)
							{
								UIForm.FPMake.grdReMake(fpSpread1, i, j + "|3");
							}
						}
					}
					
					SetControl();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		#endregion

		#region 저장
		protected override void SaveExec()
		{

			txtPO_NO.Focus();
			DialogResult dsMsg;
			dt = new DataTable();

			this.Cursor = Cursors.WaitCursor;

			if (bSave == false)
			{
				MessageBox.Show("수정할 수 없는 상태입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Information);
				this.Cursor = Cursors.Default;
				return;
			}

			// 입고번호가 없는 경우(입고 등록 이전) : 그리드 내용을 DataTable에 넣어주고, 이 것을 이용하여, 구매입고 저장시 T_IN_INFO 테이블에 저장
			// 입고번호가 있는 경우(입고 등록 후) : 바로 T_IN_INFO 테이블에 저장
			if (string.IsNullOrEmpty(strSCM_MVMT_NO))
			{
				if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
				{

					if (CheckPO_Qty() == false)
					{
						this.Cursor = Cursors.Default;
						return;
					}

					if (CheckSame_LotNo() == false)
					{
						MessageBox.Show(SystemBase.Base.MessageRtn("Lot 분할에서는 동일한 Lot No 가 있을 수 없습니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						this.Cursor = Cursors.Default;
						return;
					}

					if (fpSpread1.Sheets[0].Rows.Count > 0)
					{
						dt = (DataTable)fpSpread1.Sheets[0].DataSource;
						dt.Columns.Add("SingleYN", typeof(string));

						if (fpSpread1.Sheets[0].Rows.Count == 1)
						{
							dt.Rows[0]["SingleYN"] = "Y";
						}
						else
						{
							for (int k = 0; k <= dt.Rows.Count - 1; k++)
							{
								dt.Rows[k]["SingleYN"] = "N";
							}
						}
					}

					// Grid CUD 값 초기화
					for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U" ||
							fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
						{
							fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = (fpSpread1.Sheets[0].ActiveRowIndex + 1).ToString();
						}
					}

					strSaveYN = "Y";
					this.DialogResult = DialogResult.OK;
					Close();
				}

				this.Cursor = Cursors.Default;
				return;
			}
			else
			{

				//if (fpSpread1.Sheets[0].Rows.Count > 0)
				//{

				//    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
				//    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				//    SqlCommand cmd = dbConn.CreateCommand();
				//    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

				//    try
				//    {
				//        /////////////////////////////////////////////// 저장 시작 /////////////////////////////////////////////////
				//        //그리드 상단 필수 체크
				//        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
				//        {

				//            if (CheckAllDelete() == true)
				//            {
				//                MessageBox.Show(SystemBase.Base.MessageRtn("적어도 하나 이상의 Lot 번호를 등록해야 합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//                this.Cursor = Cursors.Default;
				//                return;
				//            }

				//            if (CheckPO_Qty() == false)
				//            {
				//                this.Cursor = Cursors.Default;
				//                return;
				//            }

				//            if (CheckSame_LotNo() == false)
				//            {
				//                MessageBox.Show(SystemBase.Base.MessageRtn("Lot 분할에서는 동일한 Lot No 가 있을 수 없습니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//                this.Cursor = Cursors.Default;
				//                return;
				//            }

				//            //행수만큼 처리
				//            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				//            {
				//                string strHead = string.Empty;
				//                string strGbn = "";

				//                if (string.IsNullOrEmpty(strSCM_MVMT_NO) == true)
				//                {
				//                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
				//                }
				//                else
				//                {
				//                    if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text))
				//                    {
				//                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
				//                    }
				//                }

				//                strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

				//                if (strHead.Length > 0)
				//                {
				//                    switch (strHead)
				//                    {
				//                        case "U": strGbn = "U1"; break;
				//                        case "I": strGbn = "I1"; break;
				//                        case "D": strGbn = "D1"; break;
				//                        default: strGbn = ""; break;
				//                    }

				//                    string strSql = " usp_T_IN_INFO_CUDR ";
				//                    strSql += "  @pTYPE        = '" + strGbn + "'";
				//                    strSql += ", @pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
				//                    strSql += ", @pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
				//                    strSql += ", @pSCM_MVMT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM 입고번호")].Text + "' ";
				//                    strSql += ", @pPO_NO       = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "' ";
				//                    strSql += ", @pPO_SEQ      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "' ";
				//                    strSql += ", @pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
				//                    strSql += ", @pITEM_CD     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
				//                    strSql += ", @pTR_TYPE     = 'I' ";
				//                    strSql += ", @pIN_DATE     = '' ";
				//                    strSql += ", @pLOT_NO      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
				//                    strSql += ", @pPROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트 번호")].Text + "' ";
				//                    strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
				//                    strSql += ", @pRCPT_QTY    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수량")].Value + "' ";
				//                    strSql += ", @pDEFECT_QTY  = 0 ";
				//                    strSql += ", @pIN_TRAN_NO  = '' ";
				//                    strSql += ", @pIN_TRAN_SEQ = NULL ";
				//                    strSql += ", @pIN_TRAN_QTY = 0 ";
				//                    strSql += ", @pSTOCK_QTY   = 0 ";
				//                    strSql += ", @pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
				//                    strSql += ", @pEND_YN      = '' ";
				//                    strSql += ", @pREMARK      = '' ";
				//                    strSql += ", @pATT_DOC_CDS = '' ";
				//                    strSql += ", @pUSER_ID     = '" + SystemBase.Base.gstrUserID + "' ";

				//                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
				//                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
				//                    MSGCode = ds.Tables[0].Rows[0][1].ToString();
				//                    if (ERRCode != "OK") { Trans.Rollback(); dLotSum = 0; goto Exit; }	// ER 코드 Return시 점프

				//                }
				//                else
				//                {
				//                    dLotSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수량")].Value);
				//                }
				//            }
				//        }
				//        else
				//        {
				//            Trans.Rollback();
				//            this.Cursor = Cursors.Default;
				//            return;
				//        }

				//        Trans.Commit();
				//        SearchExec();
				//        strSaveYN = "Y";
				//    }
				//    catch (Exception e)
				//    {
				//        SystemBase.Loggers.Log(this.Name, e.ToString());
				//        Trans.Rollback();
				//        ERRCode = "ER";
				//        MSGCode = e.Message;
				//    }
				//Exit:
				//    dbConn.Close();

				//    if (ERRCode == "OK")
				//    {
				//        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
				//        SearchExec();
				//    }
				//    else if (ERRCode == "ER")
				//    {
				//        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//    }
				//    else
				//    {
				//        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				//    }

				//    this.Cursor = Cursors.Default;
				//}

			}

			this.Cursor = Cursors.Default;
		}
		#endregion

		#region Lot 수량 <= 발주수량 체크
		private bool CheckPO_Qty()
		{
			bool bValid = true;
			decimal dSum = 0;
			decimal dReSum = 0;	// 수정 모드에서 입고잔량을 재계산하기 위한 변수

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				{
					if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수량")].Text) == false &&
						string.Compare(fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text, "D", true) != 0)
					{
						dSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수량")].Value);
					}

					if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수량")].Text) == false &&
						string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 LOT 수량")].Text) == false)
					{
						dReSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수량")].Value) -
									Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 Lot 수량")].Value);
					}
				}
			}

			if (string.IsNullOrEmpty(strSCM_MVMT_NO) == true)
			{
				if (dSum > Convert.ToDecimal(txtBAL_QTY.Value))
				{
					DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("Lot 수량이 잔량을 초과합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					bValid = false;
				}
				else
				{
					bValid = true;
				}
			}
			else
			{
				if (dSum != Convert.ToDecimal(txtIN_QTY.Value))
				{
					bValid = false;
					MessageBox.Show(SystemBase.Base.MessageRtn("구매입고처리가 되면, \r\nLot 수량의 합과 입고수량은 일치해야 합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					bValid = true;
				}
			}

			return bValid;
		}
		#endregion

		#region 동일 Lot No 여부 체크
		private bool CheckSame_LotNo()
		{
			bool bValid = true;
			int iCnt = 0;

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				{
					iCnt = 0;

					for (int j = 0; j <= fpSpread1.Sheets[0].Rows.Count - 1; j++)
					{
						if (string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text,
							fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text, true) == 0)
						{
							iCnt++;
						}
					}

					if (iCnt > 1)
					{
						bValid = false;
						break;
					}
					else
					{
						bValid = true;
					}
				}
			}

			return bValid;
		}
		#endregion

		#region 기본 값 설정
		private void SetControl()
		{
			txtPO_NO.Value = strPO_NO;
			txtPO_SEQ.Value = strPO_SEQ;
			txtPROJECT_NO.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트 번호")].Text.ToString();
			txtPROJECT_SEQ.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString();
			txtITEM_CD.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString();
			txtITEM_NM.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text.ToString();
			txtITEM_SPEC.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text.ToString();
			txtPO_QTY.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Text.ToString();
			txtUNIT.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text.ToString();

			txtBAL_QTY.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Text.ToString();

			txtPreIn_qty.Value = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Value) -
								 Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value);

			txtIN_QTY.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Text.ToString();
		}
		#endregion

		#region 입고잔량 조회
		private decimal GetRemQty()
		{
			decimal dReturn = 0;

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				{
					dReturn += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고잔량")].Value);
				}
			}

			return dReturn;
		}
		#endregion

		#region 행복사 버튼 클릭 이벤트
		private void BtnRCopy_Click(object sender, EventArgs e)
		{
			try
			{
				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					SetCellStyle(fpSpread1.Sheets[0].ActiveRowIndex);
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

		#region 행복사 후 숫자 서식 지정
		private void SetCellStyle(int row)
		{
			fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text = "";

			//NumberCellType..
			NumberCellType num = new NumberCellType();

			//소수점 위치 체크를 위한 변수
			int Place = 4;

			//소수점 위치
			num.DecimalPlaces = Place;

			//소수점 구분자
			num.DecimalSeparator = ".";

			//소수점을 표시여부
			num.FixedPoint = true;

			//천단위 구분자
			num.Separator = ",";

			//천단위 구분자 표시 여부
			num.ShowSeparator = true;

			//최대값
			num.MaximumValue = 9999999999;

			//최소값
			num.MinimumValue = -9999999999;

			//원하는 부분(row,column,cell)에 할당
			fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 수량")].CellType = num;
		}
		#endregion

		#region 입고처리된 후 모든 lot 삭제 확인
		private bool CheckAllDelete()
		{
			int iCnt = 0;
			bool bReturn = false;

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				{
					if (string.Compare(fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text, "D", true) == 0)
					{
						iCnt++;
					}
				}
			}

			if (fpSpread1.Sheets[0].Rows.Count == iCnt)
			{
				bReturn = true;
			}
			else
			{
				bReturn = false;
			}


			return bReturn;
		}
		#endregion

	}
}
