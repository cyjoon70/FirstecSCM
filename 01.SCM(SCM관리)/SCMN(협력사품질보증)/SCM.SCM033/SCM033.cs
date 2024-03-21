using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WNDW;

/// <summary>
/// 검사의뢰/취소
/// </summary>
namespace SCM.SCM033
{
	public partial class SCM033 : UIForm.FPCOMM2_2T
	{
		#region 변수
		
		#endregion

		#region 생성자
		public SCM033()
		{
			InitializeComponent();
		}


		#endregion

		#region Form Load
		private void SCM033_Load(object sender, EventArgs e)
		{
			SetAuth();
			
			//필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);

			//콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cmb1Inspector, "usp_SCM033 @pType='C1', @sCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 3);	// 전진검사원
			SystemBase.ComboMake.C1Combo(cmb2AccRej, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'Q004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 3);		// 합격/불합격
			SystemBase.ComboMake.C1Combo(cmb2Status, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'SC120', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 3);		// 진행상태

			//그리드 콤보박스 세팅			
			G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "전진검사원")] = SystemBase.ComboMake.ComboOnGrid("usp_SCM033 @pType='C1', @sCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 0);
			G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "판정")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q004', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);

			//그리드초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

			SetInit();
		}

		private void SetInit()
		{
			txtDlvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
			txtDlvDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

			txtReqFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
			txtReqTo.Value = SystemBase.Base.ServerTime("YYMMDD");

			txt1CustCd.Value = SystemBase.Base.gstrUserID;
			txt1CustNm.Value = SystemBase.Base.gstrUserName;

			txt2CustCd.Value = SystemBase.Base.gstrUserID;
			txt2CustNm.Value = SystemBase.Base.gstrUserName;

			c1DockingTab1.SelectedIndex = 0;
		}

		private void SetAuth()
		{
			if (SystemBase.Base.gstrUserID != "KO132")
			{
				txt1CustCd.Tag = ";2;;";
				txt2CustCd.Tag = ";2;;";
				btn1Cust.Tag = ";2;;";
				btn2Cust.Tag = ";2;;";
			}
		}
		#endregion

		#region SearchExec()
		protected override void SearchExec()
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
				if (c1DockingTab1.SelectedIndex == 0)
				{
					string strQuery = " usp_SCM033  @pTYPE = 'S1'";
					strQuery += ", @sCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ", @sCUST_CD	= '" + txt1CustCd.Text + "' ";	//거래처
					strQuery += ", @sDLV_DT_FR	= '" + txtDlvDtFr.Text + "' ";	//납기일 FR
					strQuery += ", @sDLV_DT_TO	= '" + txtDlvDtTo.Text + "' ";	//납기일 TO
					strQuery += ", @sDLV_REF_FR	= '" + txtDlvRefFr.Text +"' ";	//변경납기일 FR
					strQuery += ", @sDLV_REF_TO	= '" + txtDlvRefTo.Text + "' ";	//변경납기일 TO
					strQuery += ", @sITEM_CD	= '" + txt1ItemCd.Text +"' ";	//품목
					strQuery += ", @sPROJECT_NO	= '" + txt1PrjNo.Text +"' ";	//프로젝트
					strQuery += ", @sPO_NO		= '" + txt1PoNo.Text +"' ";     //발주번호

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
				}
				else
				{
					string strQuery = " usp_SCM033  @pTYPE = 'S2'";
					strQuery += ", @sCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ", @sCUST_CD	= '" + txt2CustCd.Text + "' ";			//거래처
					strQuery += ", @sREQ_DT_FR	= '" + txtReqFr.Text +"' ";				//검사요청일 FR
					strQuery += ", @sREQ_DT_TO	= '" + txtReqTo.Text + "' ";			//검사요청일 TO
					strQuery += ", @sITEM_CD	= '" + txt2ItemCd.Text +"' ";			//품목
					strQuery += ", @sPROJECT_NO	= '" + txt2PrjNo.Text +"' ";			//프로젝트
					strQuery += ", @sPO_NO		= '" + txt2PoNo.Text +"' ";				//발주번호
					strQuery += ", @sPROC_STATUS= '" + cmb2Status.SelectedValue +"' ";	//진행상태
					strQuery += ", @sACC_REJ	= '" + cmb2AccRej.SelectedValue + "' "; //합격 / 불합격

					UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);

					if (fpSpread2.Sheets[0].Rows.Count > 0)
					{
						for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
						{
							if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "상태")].Text == "검사완료"
								|| fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰 취소")].Text == "True")
							{
								UIForm.FPMake.grdReMake(fpSpread2, i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰 취소") + "|3");
							}
						}
					}
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.ToString());
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}
		#endregion

		#region NewExec()
		protected override void NewExec()
		{
			SetAuth();

			SystemBase.Validation.GroupBox_Reset(groupBox1);
			SystemBase.Validation.GroupBox_Reset(groupBox2);

			fpSpread1.Sheets[0].Rows.Count = 0;
			fpSpread2.Sheets[0].Rows.Count = 0;

			SetInit();
		}

		#endregion

		#region SaveExec()
		protected override void SaveExec()
		{
			if (c1DockingTab1.SelectedIndex == 0)
			{
				if (string.IsNullOrEmpty(cmb1Inspector.SelectedValue.ToString()))
				{
					MessageBox.Show("전진검사원을 선택해주세요.", "필수값 입력", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
					SaveReqReady();
			}				
			else
				SaveReqAlready();
		}

		private void SaveReqReady()
		{
			this.Cursor = Cursors.WaitCursor;

			if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
			{
				string ERRCode = "ER", MSGCode = "SY001" /*처리할 내용이 없습니다.*/, MSGErr = string.Empty, InsSeq = string.Empty;    

				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

				try
				{
					//행수만큼 처리
					for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
						string strGbn = string.Empty;
						string strReqTF = string.Empty;
						string strQty = string.Empty;
						string strReqDt = string.Empty;
						string strLast = string.Empty;
						string strProc = string.Empty;
						string strCust = string.Empty;
						double dValidQty = 0;

						if (strHead.Length > 0)
						{
							switch (strHead)
							{
								case "U": strGbn = "U1"; break;
								case "I": strGbn = "I1"; break;
								case "D": strGbn = "D1"; break;
								default: strGbn = ""; break;
							}

							strReqTF = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text;
							if (strReqTF == "False")
							{
								Trans.Rollback();
								ERRCode = "IN-ER";
								MSGErr = (i + 1).ToString() + " 행의 [선택] 값을 체크해주세요.";
								goto Exit;
							}

							strQty = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰 수량")].Text;
							if (string.IsNullOrEmpty(strQty) || Convert.ToDouble(strQty) <= 0)
							{
								Trans.Rollback();
								ERRCode = "IN-ER";
								MSGErr = (i + 1).ToString() + " 행의 [검사의뢰 수량] 값은 0보다 큰 값을 입력해주세요.";
								goto Exit;
							}

							dValidQty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰가능수량")].Value);
							if (dValidQty < Convert.ToDouble(strQty))
							{
								Trans.Rollback();
								ERRCode = "IN-ER";
								MSGErr = (i + 1).ToString() + " 행의 [검사의뢰 수량] 값은 [검사의뢰 가능수량]보다 작거나 같아야 합니다.";
								goto Exit;
							}

							strReqDt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사요청일")].Text;
							if (string.IsNullOrEmpty(strReqDt))
							{
								Trans.Rollback();
								ERRCode = "IN-ER";
								MSGErr = (i + 1).ToString() + " 행의 [검사요청일] 값을 입력해주세요.";
								goto Exit;
							}

							strLast = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최종")].Text;
							strProc = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정")].Text;
							strCust = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "고객")].Text;
							if (strLast == "False" && strProc == "False" && strCust == "False")
							{
								Trans.Rollback();
								ERRCode = "IN-ER";
								MSGErr = (i + 1).ToString() + " 행의 [최종/공정/고객] 중 적어도 하나의 값을 체크해주세요.";
								goto Exit;
							}

							if (strLast == "True") 
								strLast = "Y";
							else
								strLast = "N";

							if (strProc == "True") 
								strProc = "Y";
							else
								strProc = "N";

							if (strCust == "True") 
								strCust = "Y";
							else
								strCust = "N";

							string strSql = " usp_SCM033 @pTYPE		= 'I1'";
							strSql = strSql + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "'";
							strSql = strSql + ", @pPO_NO			= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "'";
							strSql = strSql + ", @pPO_SEQ			= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "'";
							strSql = strSql + ", @pINSP_REQ_DT		= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사요청일")].Text + "'";
							strSql = strSql + ", @pINSP_REQ_QTY		= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰 수량")].Text + "'";
							strSql = strSql + ", @pCHK_FINAL		= '" + strLast + "'";
							strSql = strSql + ", @pCHK_PROC			= '" + strProc + "'";
							strSql = strSql + ", @pCHK_CUSTOMER		= '" + strCust + "'";
							strSql = strSql + ", @pOUT_INSPECTOR	= '" + cmb1Inspector.SelectedValue.ToString() + "'";

							DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
							ERRCode = ds.Tables[0].Rows[0][0].ToString();
							MSGCode = ds.Tables[0].Rows[0][1].ToString();
							InsSeq = ds.Tables[0].Rows[0][3].ToString();

							if (ERRCode != "OK")
							{
								Trans.Rollback(); goto Exit; 
							}   // ER 코드 Return시 점프
							else
							{
								strSql = "";
								strSql = " usp_SCM033 @pTYPE		= 'U2'";
								strSql = strSql + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "'";
								strSql = strSql + ", @pPO_NO		= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "'";
								strSql = strSql + ", @pPO_SEQ		= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "'";
								strSql = strSql + ", @pINS_SEQ		= '" + InsSeq + "'";
								strSql = strSql + ", @pFILES_NO		= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "임시파일번호")].Text + "'";

								DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
								ERRCode = ds2.Tables[0].Rows[0][0].ToString();
								MSGCode = ds2.Tables[0].Rows[0][1].ToString();

								if (ERRCode != "OK")
								{
									Trans.Rollback(); goto Exit;
								}   // ER 코드 Return시 점프
							}

							Trans.Commit();
						}
					}
				}
				catch
				{
					Trans.Rollback();
					MSGCode = "SY002";  //에러가 발생하여 데이터 처리가 취소되었습니다.
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
				else if (ERRCode == "IN-ER")
				{
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGErr), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}

			this.Cursor = Cursors.Default;
		}

		private void SaveReqAlready()
		{
			this.Cursor = Cursors.WaitCursor;

			if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread2, this.Name, "fpSpread2", true) == true))// 그리드 필수항목 체크 
			{
				string ERRCode = "ER", MSGCode = "SY001" /*처리할 내용이 없습니다.*/, MSGErr = string.Empty;
				string strCancel = string.Empty;

				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

				try
				{
					//행수만큼 처리
					for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
					{
						string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;
						string strGbn = string.Empty;

						if (strHead.Length > 0)
						{
							switch (strHead)
							{
								case "U": strGbn = "U1"; break;
								case "I": strGbn = "I1"; break;
								case "D": strGbn = "D1"; break;
								default: strGbn = ""; break;
							}

							strCancel = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰 취소")].Text;
							if (strCancel == "True")
								strCancel = "Y";
							else
								strCancel = "N";

							string strSql = " usp_SCM033 @pTYPE		= 'U1'";
							strSql = strSql + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "'";
							strSql = strSql + ", @pPO_NO			= '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발주번호")].Text + "'";
							strSql = strSql + ", @pPO_SEQ			= '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발주순번")].Text + "'";
							strSql = strSql + ", @pINS_SEQ			= '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰 순번")].Text + "'";
							strSql = strSql + ", @pINSP_YN			= '" + strCancel + "'";

							DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
							ERRCode = ds.Tables[0].Rows[0][0].ToString();
							MSGCode = ds.Tables[0].Rows[0][1].ToString();

							if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프
						}
					}
					Trans.Commit();
				}
				catch
				{
					Trans.Rollback();
					MSGCode = "SY002";  //에러가 발생하여 데이터 처리가 취소되었습니다.
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
				else if (ERRCode == "IN-ER")
				{
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGErr), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}

			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 거래처 조회
		private void btn1Cust_Click(object sender, EventArgs e)
		{
			SetCust(0);
		}

		private void btn2Cust_Click(object sender, EventArgs e)
		{
			SetCust(1);
		}

		private void txt1CustCd_TextChanged(object sender, EventArgs e)
		{
			SetCustByText(0);
		}

		private void txt2CustCd_TextChanged(object sender, EventArgs e)
		{
			SetCustByText(1);
		}

		private void SetCust(int id)
		{
			try
			{
				if (id == 0)
				{
					WNDW002 pu = new WNDW002(txt1CustCd.Text, "P");
					pu.ShowDialog();
					if (pu.DialogResult == DialogResult.OK)
					{
						string[] Msgs = pu.ReturnVal;

						txt1CustCd.Text = Msgs[1].ToString();
						txt1CustNm.Value = Msgs[2].ToString();
					}
				}
				else
				{
					WNDW002 pu = new WNDW002(txt2CustCd.Text, "P");
					pu.ShowDialog();
					if (pu.DialogResult == DialogResult.OK)
					{
						string[] Msgs = pu.ReturnVal;

						txt2CustCd.Text = Msgs[1].ToString();
						txt2CustNm.Value = Msgs[2].ToString();
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SetCustByText(int id)
		{
			try
			{
				if (id == 0)
					txt1CustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txt1CustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
				else
					txt2CustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txt2CustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 품목 조회
		private void btn1Item_Click(object sender, EventArgs e)
		{
			SetItem(0);
		}

		private void btn2Item_Click(object sender, EventArgs e)
		{
			SetItem(1);
		}

		private void txt1ItemCd_TextChanged(object sender, EventArgs e)
		{
			SetItemByText(0);
		}

		private void txt2ItemCd_TextChanged(object sender, EventArgs e)
		{
			SetItemByText(1);
		}

		private void SetItem(int id)
		{
			try
			{
				string strQuery = " usp_SC_COMM_POPUP @pTYPE = 'S1' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pCUST_CD = '" + txt1CustCd.Text + "'";
				string[] strWhere = new string[] { "@pCD", "@pNM" };
				string[] strSearch;

				if (id == 0)
					strSearch = new string[] { txt1ItemCd.Text, txt1ItemNm.Text };
				else
					strSearch = new string[] { txt2ItemCd.Text, txt2ItemNm.Text };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM_COMM_ITEM", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					if (id == 0)
					{
						txt1ItemCd.Value = Msgs[0].ToString();
						txt1ItemNm.Value = Msgs[1].ToString();
					}
					else
					{
						txt2ItemCd.Value = Msgs[0].ToString();
						txt2ItemNm.Value = Msgs[1].ToString();
					}						
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SetItemByText(int id)
		{
			try
			{
				if (id == 0)
					txt1ItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txt1ItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
				else
					txt2ItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txt2ItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			
		}
		#endregion

		#region 프로젝트 조회
		private void btn1Project_Click(object sender, EventArgs e)
		{
			SetProject(0);
		}

		private void btn2Project_Click(object sender, EventArgs e)
		{
			SetProject(1);
		}

		private void txt1PrjNo_TextChanged(object sender, EventArgs e)
		{
			SetProjectByText(0);
		}

		private void txt2PrjNo_TextChanged(object sender, EventArgs e)
		{
			SetProjectByText(1);
		}

		private void SetProject(int id)
		{
			try
			{
				string strQuery = " usp_SC_COMM_POPUP @pTYPE = 'S2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pCUST_CD = '" + txt1CustCd.Text + "'";
				string[] strWhere = new string[] { "@pCD", "@pNM" };
				string[] strSearch;

				if (id == 0)
					strSearch = new string[] { txt1PrjNo.Text, txt1PrjNm.Text };
				else
					strSearch = new string[] { txt2PrjNo.Text, txt2PrjNm.Text };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM_COMM_PROJ", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					if (id == 0)
					{
						txt1PrjNo.Value = Msgs[0].ToString();
						txt1PrjNm.Value = Msgs[1].ToString();
					}
					else
					{
						txt2PrjNo.Value = Msgs[0].ToString();
						txt2PrjNm.Value = Msgs[1].ToString();
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SetProjectByText(int id)
		{
			try
			{
				if (id == 0)
					txt1PrjNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txt1PrjNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
				else
					txt2PrjNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txt2PrjNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}
		#endregion

		#region 증빙파일 등록 및 조회
		protected override void fpButtonClick(int Row, int Column)
		{
			string strPoNo = string.Empty;
			string strPoSeq = string.Empty;
			string strFileNo = string.Empty;

			try
			{
				// 첨부파일
				if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "증빙_2"))
				{
					strPoNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Value.ToString();
					strPoSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Value.ToString();
					strFileNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "임시파일번호")].Value.ToString();

					// 첨부파일 팝업 띄움.
					WNDWS01 pu = new WNDWS01("", strPoNo, strPoSeq, "", "", "", true, strFileNo, "검사의뢰", "SCMRE");
					pu.ShowDialog();

					// 2022.05.20. hma 수정(Start): 지출증빙 팝업 리턴값 체크하여 증빙건수 Refresh 처리 
					string[] Msgs = pu.ReturnVal;
					if (Msgs != null && Msgs[0] == "Y")
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙")].Value = Msgs[0];
					else
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙")].Value = "";

				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		protected override void fpButtonClick2(int Row, int Column)
		{
			string strPoNo = string.Empty;
			string strPoSeq = string.Empty;
			string strInspSeq = string.Empty;

			try
			{
				// 첨부파일
				if (Column == SystemBase.Base.GridHeadIndex(GHIdx2, "증빙_2"))
				{
					strPoNo = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "발주번호")].Value.ToString();
					strPoSeq = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "발주순번")].Value.ToString();
					strInspSeq = fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰 순번")].Value.ToString();

					// 첨부파일 팝업 띄움.
					WNDWS01 pu = new WNDWS01(strPoNo + "/" + strPoSeq + "/" + strInspSeq, strPoNo, strPoSeq, strInspSeq, "", "", false, "", "검사의뢰", "SCMRE");
					pu.ShowDialog();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

	}
}
