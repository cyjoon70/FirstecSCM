using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WNDW;
using System.Text.RegularExpressions;

namespace SCM.SCM034
{
	public partial class SCM034 : UIForm.FPCOMM1
	{
		#region 변수

		#endregion

		#region 생성자
		public SCM034()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void SCM034_Load(object sender, EventArgs e)
		{
			SetAuth();

			//필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			//콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cmb1Inspector, "usp_SCM033 @pType='C1', @sCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 3);    // 전진검사원

			//그리드초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			SetInit();
		}

		private void SetInit()
		{
			txtDlvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
			txtDlvDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

			txt1CustCd.Value = SystemBase.Base.gstrUserID;
			txt1CustNm.Value = SystemBase.Base.gstrUserName;
		}

		private void SetAuth()
		{
			if (SystemBase.Base.gstrUserID != "KO132")
			{
				txt1CustCd.Tag = ";2;;";
				btn1Cust.Tag = ";2;;";
			}
		}
		#endregion

		#region SearchExec()
		protected override void SearchExec()
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;

				string strQuery = " usp_SCM033  @pTYPE = 'S1'";
				strQuery += ", @sCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery += ", @sCUST_CD	= '" + txt1CustCd.Text + "' ";  //거래처
				strQuery += ", @sDLV_DT_FR	= '" + txtDlvDtFr.Text + "' ";  //납기일 FR
				strQuery += ", @sDLV_DT_TO	= '" + txtDlvDtTo.Text + "' ";  //납기일 TO
				strQuery += ", @sDLV_REF_FR	= '" + txtDlvRefFr.Text + "' "; //변경납기일 FR
				strQuery += ", @sDLV_REF_TO	= '" + txtDlvRefTo.Text + "' "; //변경납기일 TO
				strQuery += ", @sITEM_CD	= '" + txt1ItemCd.Text + "' ";  //품목
				strQuery += ", @sPROJECT_NO	= '" + txt1PrjNo.Text + "' ";   //프로젝트
				strQuery += ", @sPO_NO		= '" + txt1PoNo.Text + "' ";     //발주번호

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
			}
			catch (Exception e)
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

			fpSpread1.Sheets[0].Rows.Count = 0;

			SetInit();
		}

		#endregion

		#region SaveExec()
		protected override void SaveExec()
		{
			this.Cursor = Cursors.WaitCursor;

			if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
			{
				string ERRCode = "ER", MSGCode = "SY001";   //처리할 내용이 없습니다.
				string strCOST_CENTER = "";

				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

				try
				{
					//행수만큼 처리
					for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
						string strGbn = "";

						if (strHead.Length > 0)
						{
							switch (strHead)
							{
								case "U": strGbn = "U1"; break;
								case "I": strGbn = "I1"; break;
								case "D": strGbn = "D1"; break;
								default: strGbn = ""; break;
							}

							strCOST_CENTER = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "코스트센터코드")].Text;

							string strSql = " usp_ABBD009 '" + strGbn + "'";
							strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";

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
			try
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
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void txt1CustCd_TextChanged(object sender, EventArgs e)
		{
			try
			{
				txt1CustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txt1CustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
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
			try
			{
				string strQuery = " usp_SC_COMM_POPUP @pTYPE = 'S1' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pCUST_CD = '" + SystemBase.Base.gstrUserID + "'";
				string[] strWhere = new string[] { "@pCD", "@pNM" };
				string[] strSearch;

				strSearch = new string[] { txt1ItemCd.Text, txt1ItemNm.Text };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM_COMM_ITEM", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txt1ItemCd.Value = Msgs[0].ToString();
					txt1ItemNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void txt1ItemCd_TextChanged(object sender, EventArgs e)
		{
			try
			{
				txt1ItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txt1ItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 프로젝트 조회
		private void btn1Project_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_SC_COMM_POPUP @pTYPE = 'S2' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "', @pCUST_CD = '" + SystemBase.Base.gstrUserID + "'";
				string[] strWhere = new string[] { "@pCD", "@pNM" };
				string[] strSearch;

				strSearch = new string[] { txt1PrjNo.Text, txt1PrjNm.Text };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("SCM_COMM_PROJ", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txt1PrjNo.Value = Msgs[0].ToString();
					txt1PrjNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void txt1PrjNo_TextChanged(object sender, EventArgs e)
		{
			try
			{
				txt1PrjNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txt1PrjNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "' ");
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion
	}
}
