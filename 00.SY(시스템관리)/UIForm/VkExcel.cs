using System;
using System.IO;
using System.Data;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using Office = Microsoft.Office.Core;
using Excel;

namespace UIForm
{    
	public class VkExcel
	{
		#region ����

		private ArrayList arrProcessID = null;
		private Excel.Application excelApp = null;
		private Excel.Workbook excelWorkbook = null;
		private Excel.Sheets excelSheets = null;
		private Excel.Worksheet excelWorksheet = null;

		private object vk_filename;
		private bool vk_app_visible = false;
		private bool bIsOpened = false;
        
		#endregion

		#region ������
		public VkExcel(bool visible)
		{
			this.vk_app_visible = visible;
			this.startExcel();
		}
		#endregion

		#region START EXCEL
		private void startExcel()
		{
			if (this.excelApp == null)
			{
				// ������ȯ�� ����ڰ� �۾� ���� ���� ���� üũ
				this.arrProcessID = new ArrayList();
				Process[] excelProcess = Process.GetProcessesByName("EXCEL");
				for (int i = 0; i < excelProcess.Length; i++)
				{
					this.arrProcessID.Add(excelProcess[i].Id);
				} 

				this.excelApp = new Excel.ApplicationClass();
			}

			// Make Excel Visible
			this.excelApp.Visible = this.vk_app_visible;
		}
		#endregion

		#region STOP EXCEL
		public void stopExcel()
		{
			if (this.excelApp != null)
			{
				//��ȯ�� ���� â��  �ݴ´�.
				Process[] excelProcess = Process.GetProcessesByName("EXCEL");
				for (int i = 0; i < excelProcess.Length; i++)
				{
					if (!this.arrProcessID.Contains(excelProcess[i].Id))
						excelProcess[i].Kill();
				}

				this.excelApp = null;
			}
		}
		#endregion

		#region OPEN FILE
		public bool OpenFile(string fileName)
		{
			vk_filename = fileName;

			try
			{
				this.excelWorkbook = this.excelApp.Workbooks.Open(
					fileName,           // File Name
					0,                  // UpdateLinks
					false,              // ReadOnly
					1,                  // Format
					Missing.Value,      // Password
					Missing.Value,      // WriteResPassword
					true,               // IgnoreReadOnlyRecommended
					Missing.Value,      // Origin
					Missing.Value,      // Delimiter
					true,               // Editable
					false,              // Notify
					Missing.Value,      // Converter
					false,              // AddToMru
					false,              // Local
					false);             // CorruptLoad

				this.bIsOpened = true;
			}
			catch (Exception e)
			{
				this.CloseFile();
				throw new Exception(e.Message);
			}
			return this.bIsOpened;
		}
		#endregion

		#region CLOSE FILE
		public void CloseFile()
		{
			if (this.bIsOpened == true)
			{
				excelWorkbook.Close(
					false,              // SaveChanges
					vk_filename,        // Filename
					false);             // RouteWorkbook

				this.bIsOpened = false;
			}
		}
		#endregion

		#region SAVE FILE
		public void SaveExcel()
		{
			excelWorkbook.Save();
		}
		public void SaveAsExcel(string strPath)
		{
			excelWorkbook.SaveAs(strPath
				, Missing.Value
				, Missing.Value
				, Missing.Value
				, Missing.Value
				, Missing.Value
				, Excel.XlSaveAsAccessMode.xlNoChange
				, Missing.Value
				, Missing.Value
				, Missing.Value
				, Missing.Value
				, Missing.Value);
		}
		#endregion

		#region CREATE EXCEL SHEET
		public void CreateSheet()
		{
			this.excelWorkbook = this.excelApp.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
			this.excelWorksheet = (Excel.Worksheet)excelWorkbook.ActiveSheet;
		}

		#endregion

		#region GET EXCEL SHEET LIST
		public string[] GetExcelSheetLists()
		{
			if (this.excelWorkbook != null)
			{
				excelSheets = excelWorkbook.Worksheets;
			}

			string[] excelSheetNames = new string[excelSheets.Count];

			for (int i = 1; i <= this.excelSheets.Count; i++)
			{
				this.excelWorksheet = (Excel.Worksheet)excelSheets.get_Item((object)i);
				excelSheetNames[i - 1] = excelWorksheet.Name;
			}

			return excelSheetNames;
		}
		#endregion

		#region FIND EXCEL ATP WORKSHEET
		public bool FindExcelWorksheet(string worksheetName)
		{
			bool ATP_SHEET_FOUND = false;

			if (this.excelWorkbook != null)
			{
				excelSheets = excelWorkbook.Worksheets;
			}

			if (this.excelSheets != null)
			{
				// Step thru the worksheet collection and see if ATP sheet is
				// available. If found return true;
				for (int i = 1; i <= this.excelSheets.Count; i++)
				{
					this.excelWorksheet = (Excel.Worksheet)excelSheets.get_Item((object)i);
					if (this.excelWorksheet.Name.Equals(worksheetName))
					{
						this.excelWorksheet.Activate();
						ATP_SHEET_FOUND = true;
						return ATP_SHEET_FOUND;
					}
				}
			}

			return ATP_SHEET_FOUND;
		}
		#endregion

		#region COPY EXCEL APT WORKSHEET
		public bool CopyExcelWorksheet(string origin, string target)
		{
			bool ATP_SHEET_COPIED = false;

			if (FindExcelWorksheet(origin))
			{
				this.excelWorksheet.Copy(Missing.Value, this.excelWorksheet);
				((Excel.Worksheet)this.excelWorksheet.Next).Name = target;

				ATP_SHEET_COPIED = true;
			}

			return ATP_SHEET_COPIED;
		}

		#endregion

		#region GET DATA
		public int GetRowCount()
		{
			return (excelWorksheet.UsedRange).Rows.Count;
		}
		public string GetCellValue(string StartCell)
		{
			Excel.Range range = excelWorksheet.get_Range(StartCell, Type.Missing);
			object value = range.get_Value(Type.Missing);

			if (value == null) return "";
			else return value.ToString();
		}
		public string[] GetRange(string strStartCell, string strEndCell)
		{
			Excel.Range range = excelWorksheet.get_Range(strStartCell, strEndCell);
			Array myvalues = (Array)range.Cells.Value2;
			string[] strArray = ConvertToStringArray(myvalues);

			return strArray;
		}
		public  Excel.Range GetRange_1(string strStartCell, string strEndCell)
		{
			Excel.Range range = excelWorksheet.get_Range(strStartCell, strEndCell);

			return range;
		}

		private string[] ConvertToStringArray(Array values)
		{
			string[] newArray = new string[values.Length];

			int index = 0;
			for (int i = values.GetLowerBound(0); i <= values.GetUpperBound(0); i++)
			{
				for (int j = values.GetLowerBound(1); j <= values.GetUpperBound(1); j++)
				{
					if (values.GetValue(i, j) == null)
					{
						newArray[index] = "";
					}
					else
					{
						newArray[index] = (string)values.GetValue(i, j).ToString();
					}
					index++;
				}
			}
			return newArray;
		}
		#endregion

		#region SET DATA
		public void SetRange(string strStartCell, System.Data.DataTable dtData)
		{
			try
			{
				string[,] excelData = null;
				string strEndCell = string.Empty;
				int intStartCol = 0;
				int intStartRow = 0;
				int intEndCol = 0;
				int intEndRow = 0;

				DataSet ds = new DataSet();                    
				ds.Tables.Add(dtData.Copy());

				intStartCol = Convert.ToInt32(Convert.ToChar(strStartCell.Substring(0, 1)));  // A = 65
				intStartRow = Convert.ToInt32(strStartCell.Substring(1));
				intEndCol = intStartCol + dtData.Columns.Count - 1;
				intEndRow = intStartRow + dtData.Rows.Count - 1;
				strEndCell = Convert.ToChar(intEndCol).ToString() + intEndRow.ToString();
				excelData = this.ConvertToStringArray(dtData);

				Excel.Range range = this.excelWorksheet.get_Range(strStartCell, strEndCell);
				range.Value2 = excelData;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public void SetSelect(string strStartCell, string strEndCell)
		{
			this.excelWorksheet.get_Range(strStartCell, strEndCell).Select();
		}
		public void SetCell(int iRow, int iCol, string strData)
		{
			((Excel.Range)this.excelWorksheet.Cells[iRow, iCol]).Value2 = strData;

		}
		#endregion

		#region SET MACRO ENV
		public void SetMacroEnv()
		{
			string strVBOM = string.Empty;

			// ��ũ�ο��� VB PROJECT�� ������ �� �ֵ��� ��ũ�� ���ȼ����� �������ش�.
			// ����> ��ũ��> ����> �ŷ��� �� �ִ� �Խ���> Visual Basic ������Ʈ�� �����ϰ� �׼����� �� ����(üũ)
			// (���ȼ����� ���������� ������ ��ũ�� ������ ������ �߻��Ѵ�)
			Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser;
			reg = reg.OpenSubKey("Software\\Microsoft\\Office\\" + this.excelApp.Version + "\\Excel\\Security", true);
			strVBOM = Convert.ToString(reg.GetValue("AccessVBOM"));

			//            switch (strVBOM)
			//            {
			//                case "":
			//                case "0":
			//                    // ������ ������ؾ���
			//                    stopExcel();
			//                    reg.SetValue("AccessVBOM", 1, Microsoft.Win32.RegistryValueKind.DWord);
			//                    startExcel();
			//                    break;
			//                case "1":
			//                    break;
			//            }

			reg.Close();
		}
		#endregion

		#region RUN MACRO
		public void RunMacro(string strMacro)
		{
			if (this.excelApp != null)
			{
				this.excelApp.Run(
					strMacro,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value,
					Missing.Value);

			}
		}
		#endregion

		#region DELETE MACRO
		public void DelMacro(string strModule)
		{
			if (this.excelWorkbook != null)
			{
                this.excelWorkbook.VBProject.VBComponents.Remove(
                    this.excelWorkbook.VBProject.VBComponents.Item(strModule));
			}
		}

		public void SetAddRow(string strStartCell, string strEndCell)
		{
			this.excelWorksheet.get_Range(strStartCell, strEndCell).Insert(Excel.XlInsertShiftDirection.xlShiftDown, Type.Missing);
		}
		#endregion

		#region CONVERT TO STRING ARRAY
		private string[,] ConvertToStringArray(System.Data.DataTable dtData)
		{
			string[,] newArray = new string[dtData.Rows.Count, dtData.Columns.Count];

			for (int i = 0; i < dtData.Rows.Count; i++)
			{
				for (int j = 0; j < dtData.Columns.Count; j++)
				{
					newArray[i, j] = dtData.Rows[i][j].ToString();
				}
			}

			return newArray;
		}
		#endregion

		#region AddPicture
		public void AddPicture(String picFullPathName, int ix, int iy, System.Single  dwidth, System.Single dheight)
		{

			excelWorksheet.Shapes.AddPicture(picFullPathName.ToString(), Microsoft.Office.Core.MsoTriState.msoFalse, 
				Microsoft.Office.Core.MsoTriState.msoCTrue, (float)ix, (float)iy, dwidth, dheight);

		}
		#endregion

		#region Cell Merge
		public void  CellMerge(string strRange)
		{
			Excel.Range range2 = excelWorksheet.get_Range(strRange, Missing.Value);

			//			range2.Select();   // ������ �����ϰ�

			range2.Merge(Type.Missing);  

		}

		public void CellBorder(string strRange)
		{
			Excel.Range range2 = excelWorksheet.get_Range(strRange, Missing.Value);

			//			range2.Select();   // ������ �����ϰ�

			range2.Borders.LineStyle = 1;

		}
		#endregion

		#region SHOWEXCEL
		public void ShowExcel(bool visible)
		{
			this.excelApp.Visible = visible;
		}
		#endregion
	}   
}