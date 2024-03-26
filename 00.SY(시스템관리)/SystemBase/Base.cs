using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using PlexityHide.GTP;
using System.Globalization;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SystemBase
{
	/// <summary>
	/// Base�� ���� ��� �����Դϴ�.
	/// </summary>
	public class Base
	{
        public static string gstrFromLoading    = "N";  //From Loading����
        public static string gstrMNUF_CODE      = "";	//��ü�ڵ�
        public static string gstrFCTR_CODE      = "";	//����
        public static string gstrPERIOD_FROM    = "";	//���Ⱓ(From)
        public static string gstrPERIOD_TO      = "";	//���Ⱓ(To)

		public static string gstrDbConn			= "";	//DB ��������
		public static string gstrDbName		    = "";	//db Name
		public static string gstrServerNM		= "";	//���Ӽ���Ip
		public static string gstrServerId       = "";   //���Ӽ��� id
		public static string gstrServerPwd      = "";   //���Ӽ��� pwd
		public static string gstrUserID			= "";	//����� ID ����
		public static string gstrUserPWD		= "";	//����� ��й�ȣ
		public static string gstrUserName		= "";	//����� �̸� ����
        public static string gstrExcelConn      = "";   //EXCEL ��������
		public static string gstrScmAdmin		= "";   //SCM ADMIN ����

		public static string gstrMacAddress		= "";	//����� �ƾ�巹��
        public static string gstrUserIp = "";	//����� ���� IP

		//public static string gstrLodeFormName	= "";	//�ε�����
		public static int gstrFormClosingMsg	= 0;	//ȭ������� �׸��� ���浥��Ÿ Ȯ�� �޼��� ǥ�ÿ��� 0 or 1

        public static string gstrLangCd = "KOR";//�����ڵ�

        public static  System.Drawing.Color gColor1 = Color.PaleTurquoise;	//���հ�
		public static  System.Drawing.Color gColor2 = Color.PaleGreen;		//�Ұ�1
		public static  System.Drawing.Color gColor3 = Color.PaleVioletRed;	//�Ұ�2
		public static  System.Drawing.Color gColor4 = Color.Moccasin;		//�Ұ�3
		public static  System.Drawing.Color gColor5 = Color.PaleGoldenrod;	//�Ұ�4

        public static System.Drawing.Color Color_Update = System.Drawing.Color.FromArgb(115, 181, 223);	  // Update U �÷���
        public static System.Drawing.Color Color_Insert = System.Drawing.Color.FromArgb(224, 245, 89);    // Insert I �÷���
        public static System.Drawing.Color Color_Delete = System.Drawing.Color.FromArgb(228, 120, 58);     // Delete D �÷���
        public static System.Drawing.Color Color_Org = System.Drawing.Color.FromArgb(242, 244, 246);      // ������ 

        public static string gstrControl_OrgData = "";	//��Ʈ�� �������� �ڷ�
        public static string gstrControl_SaveData = "";	//��Ʈ�� �������� �ڷ�

		public static string gstrCOMCD			= "1";  //�����ڵ�
		public static string gstrCOMNM			= "";  //���θ�
		public static string gstrBIZCD			= "";  //������ڵ�
		public static string gstrBIZNM			= "";  //������

		public static string gstrPLANT_CD		= "";  //�����

		public static string gstrREORG_ID		= "";  //�μ�����ID
		public static string gstrDEPT			= "";  //�μ��ڵ�
		public static string gstrDEPTNM			= "";  //�μ���
		public static string gstrMSG_CODE			= "";  //�޼����ڵ�

		public static string gstrDOC_NO		= "";		//������ȣ
		public static string gstrWO_NO		= "";		//workorder no

        // scm���� �߰�
        public static string gstrTRADE_TYPE = ""; //�ŷ�����
        public static string gstrCUST_TYPE = ""; //�ŷ�ó����

		public static string RodeFormName		= "";	//�ε��� �� �̸� ����
		public static string ProgramWhere		= "";	//���� ���α׷� ��ġ
		public static string RodeFormID			= "";	//�ε��� �� ID ����
		public static string RodeFormText		= "";	//�ε��� �� �ѱ۸�
		public static string Query1				= "";	//Query�� ����
		public static string Query2				= "";
		public static int InputBoxHeight;			//�Է�â ���� ����
		public static int GridKind;					//�׸��� ����
		public static int lblHeadColorR;			//
		public static int lblHeadColorG;			//
		public static int lblHeadColorB;			//
		public static int lblLineColorR;			//
		public static int lblLineColorG;			//
		public static int lblLineColorB;			//
		public static int lblSelectColorR;			//
		public static int lblSelectColorG;			//
		public static int lblSelectColorB;			//
		public static int lblSelectFontR;			//
		public static int lblSelectFontG;			//
		public static int lblSelectFontB;			//
		public static int lbl2_1ColorR;			//
		public static int lbl2_1ColorG;			//
		public static int lbl2_1ColorB;			//
		public static int lblColColorR;			//
		public static int lblColColorG;			//
		public static int lblColColorB;			//
		//public static string [] Grid1 = new string[]{"",""};//�׸��� Ÿ��Ʋ�� �迭�� ����
		//public static string [] Grid2;

		public static Form BaseForm = null; //������

		#region �׷�ڽ� ���� �ִ� ��Ʈ�� Reset
		public static void GroupBoxReset(GroupBox groupBox1)
		{// ���� : SystemBase.Base.GroupBoxReset(groupBox1)
			GroupBoxReset2(groupBox1);
		}
		private static void GroupBoxReset2(Control ctls)
		{// ���� : SystemBase.Base.GroupBoxReset(groupBox1)
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   

				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;
						cb.DropDownStyle = ComboBoxStyle.DropDownList;

						if(cb.Items.Count > 0)
							cb.SelectedIndex = 0;
						break;

					case "TextBox":
						c.Text = "";  
						break;
						
					case "NumericUpDown":
						NumericUpDown Nud = (NumericUpDown)c;
						Nud.Value = 1;
						break;

					case "C1TextBox":
						C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
						ctb.Value = null;
						break;

					case "C1NumericEdit":
						C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
						cne.Value = null;
						break;

					case "C1DateEdit":
						C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
						cde.Value = null;
						break;
					case "RadioButton":
						RadioButton rd = (RadioButton)c;
						if(rd.Checked == true)
							rd.Checked = false;
						break;
					case "CheckBox":
						CheckBox ck = (CheckBox)c;
						if(ck.Checked == true)
							ck.Checked = false;
						break;

					case "DateTimePicker":
						DateTimePicker dt = (DateTimePicker)c;
						dt.Value = Convert.ToDateTime(SystemBase.Base.ServerTime(""));
						break;

				}

				//User Contorl �ϰ�쿣 ���ȣ��
				//if(Convert.ToString(c.Tag) == "UC")
					GroupBoxReset2(c);

			}
		}
		#endregion

		#region c1DockingTab ���� �ִ� ��Ʈ�� Reset
		public static void c1DockingTabReset(C1.Win.C1Command.C1DockingTabPage c1DockingTabPage1)
		{// ���� : NamYoung.Common.Etc.GroupBoxReset(groupBox1)
			c1DockingTabReset2(c1DockingTabPage1);
		}

		private static void c1DockingTabReset2(Control ctls)
		{
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   

				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;
						cb.DropDownStyle = ComboBoxStyle.DropDownList;

						if(cb.Items.Count > 0)
							cb.SelectedIndex = 0;
						break;

					case "TextBox":
						c.Text = "";  
						break;
						
					case "NumericUpDown":
						NumericUpDown Nud = (NumericUpDown)c;
						Nud.Value = 1;
						break;

					case "RadioButton":
						RadioButton rd = (RadioButton)c;
						if(rd.Checked == true)
							rd.Checked = false;
						break;

					case "CheckBox":
						CheckBox ck = (CheckBox)c;
						if(ck.Checked == true)
							ck.Checked = false;
						break;

					case "C1TextBox":
						C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
						ctb.Value = null;
						break;

					case "C1NumericEdit":
						C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
						cne.Value = null;
						break;

					case "C1DateEdit":
						C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
						cde.Value = null;
						break;

					case "DateTimePicker":
						DateTimePicker dt = (DateTimePicker)c;
						dt.Value = Convert.ToDateTime(SystemBase.Base.ServerTime(""));
						break;
				}

				//User Contorl �ϰ�쿣 ���ȣ��
				//if(Convert.ToString(c.Tag) == "UC")
					c1DockingTabReset2(c);
			}
		}
		#endregion

		#region TabPage ���� �ִ� ��Ʈ�� Reset
		public static void TabPageReset(TabPage tabPage1)
		{// ���� : NamYoung.Common.Etc.GroupBoxReset(groupBox1)
			TabPageReset2(tabPage1);
		}

		private static void TabPageReset2(Control ctls)
		{
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;
						cb.DropDownStyle = ComboBoxStyle.DropDownList;

						if(cb.Items.Count > 0)
							cb.SelectedIndex = 0;
						break;

					case "TextBox":
						c.Text = "";  
						break;
						
					case "NumericUpDown":
						NumericUpDown Nud = (NumericUpDown)c;
						Nud.Value = 1;
						break;

					case "RadioButton":
						RadioButton rd = (RadioButton)c;
						if(rd.Checked == true)
							rd.Checked = false;
						break;

					case "CheckBox":
						CheckBox ck = (CheckBox)c;
						if(ck.Checked == true)
							ck.Checked = false;
						break;

					case "C1TextBox":
						C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
						ctb.Value = null;
						break;

					case "C1NumericEdit":
						C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
						cne.Value = null;
						break;

					case "C1DateEdit":
						C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
						cde.Value = null;
						break;

					case "DateTimePicker":
						DateTimePicker dt = (DateTimePicker)c;
						dt.Value = Convert.ToDateTime(SystemBase.Base.ServerTime(""));
						break;
				}

				//User Contorl �ϰ�쿣 ���ȣ��
				//if(Convert.ToString(c.Tag) == "UC")
					TabPageReset2(c);

			}
		}
		#endregion

		#region Panel ���� �ִ� ��Ʈ�� Reset
		public static void PanelReset(Panel panel1)
		{// ���� : NamYoung.Common.Etc.PanelReset(panel1)
			PanelReset2(panel1);
		}

		private static void PanelReset2(Control ctls)
		{// ���� : NamYoung.Common.Etc.PanelReset(panel1)
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;
						cb.DropDownStyle = ComboBoxStyle.DropDownList;

						if(cb.Items.Count > 0)
							cb.SelectedIndex = 0;
						break;

					case "TextBox":
						c.Text = "";  
						break;
						
					case "NumericUpDown":
						NumericUpDown Nud = (NumericUpDown)c;
						Nud.Value = 1;
						break;

					case "RadioButton":
						RadioButton rd = (RadioButton)c;
						if(rd.Checked == true)
							rd.Checked = false;
						break;

					case "CheckBox":
						CheckBox ck = (CheckBox)c;
						if(ck.Checked == true)
							ck.Checked = false;
						break;

					case "C1TextBox":
						C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;
						ctb.Value = null;
						break;

					case "C1NumericEdit":
						C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;
						cne.Value = null;
						break;

					case "C1DateEdit":
						C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;
						cde.Value = null;
						break;

					case "DateTimePicker":
						DateTimePicker dt = (DateTimePicker)c;
						dt.Value = Convert.ToDateTime(SystemBase.Base.ServerTime(""));
						break;
				}

				//User Contorl �ϰ�쿣 ���ȣ��
				//if(Convert.ToString(c.Tag) == "UC")
					PanelReset2(c);

			}
		}
		#endregion

		#region Panel ���� �ִ� Radio Checked �� return
		public static string PanelRdoValue(Panel panel1)
		{// ���� : string CheckedText = NamYoung.Common.Etc.PanelRdoValue(panel1)
			string Rtn = "";
			foreach(System.Windows.Forms.Control c in panel1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "RadioButton":
						RadioButton cb = (RadioButton)c;
						if(cb.Checked == true)
							Rtn = cb.Text;
						break;
				}
			}
			return Rtn;
		}

		#endregion

		#region Panel �� Radio Checked
		public static void PanelRdoCheck(Panel panel1, string Value)
		{// ���� : NamYoung.Common.Etc.PanelRdoCheck(panel9,"aaa");
			foreach(System.Windows.Forms.Control c in panel1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "RadioButton":
						RadioButton cb = (RadioButton)c;
						if(cb.Text == Value)
							cb.Checked = true;
						break;
				}
			}
		}
		#endregion

		#region GroupBox �� RadioButton Checked
		public static void GroupBoxRdoCheck(GroupBox groupBox1, string Value)
		{// ���� : SystemBase.Base.GroupBoxRdoCheck(groupBox1,"aaa");
			foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "RadioButton":
						RadioButton cb = (RadioButton)c;
						if(cb.Text == Value)
							cb.Checked = true;
						break;
				}
			}
		}
		#endregion

		#region GroupBox �� RadioButton TAG ������ Checked
		public static void GroupBoxRdoCheck(string TAG, GroupBox groupBox1)
		{// ���� : SystemBase.Base.GroupBoxRdoCheck("aaa",groupBox1);
			foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "RadioButton":
						RadioButton cb = (RadioButton)c;
						if(cb.Tag.ToString() == TAG)
							cb.Checked = true;
						break;
				}
			}
		}
		#endregion

		#region Panel �� CheckBox Text return
		public static string PanelChkValue(Panel panel1)
		{// ���� : string CheckedText = NamYoung.Common.Etc.PanelChkValue(panel1)
			string Rtn = "";
			foreach(System.Windows.Forms.Control c in panel1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "CheckBox":
						CheckBox cb = (CheckBox)c;
						if(cb.Checked == true)
							if(Rtn.Length == 0)
								Rtn = cb.Text;
							else
								Rtn = Rtn +"|"+ cb.Text;
						break;
				}
			}
			return Rtn;
		}
		#endregion

		#region groupBox ���� �ִ� Radio Checked �� return
		public static string GroupBoxRdoValue(GroupBox groupBox1)
		{// ���� : string CheckedText = NamYoung.Common.Etc.GroupBoxRdoValue(panel1)
			string Rtn = "";
			foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "RadioButton":
						RadioButton cb = (RadioButton)c;
						if(cb.Checked == true)
							Rtn = cb.Text;
						break;
				}
			}
			return Rtn;
		}
		#endregion

		#region groupBox ���� �ִ� Radio Checked �� return
		public static string GroupBoxRdoValue(GroupBox groupBox1, bool TAG)
		{// ���� : string CheckedText = NamYoung.Common.Etc.GroupBoxRdoValue(groupBox1, true)
			string Rtn = "";
			if(TAG == true)
			{
				foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
				{   
					switch(c.GetType().Name)
					{
						case "RadioButton":
							RadioButton cb = (RadioButton)c;
							if(cb.Checked == true)
								Rtn = cb.Tag.ToString();
							break;
					}
				}
			}
			else
			{
				foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
				{   
					switch(c.GetType().Name)
					{
						case "RadioButton":
							RadioButton cb = (RadioButton)c;
							if(cb.Checked == true)
								Rtn = cb.Text;
							break;
					}
				}
			}
			return Rtn;
		}
		#endregion

		#region GroupBox ���� �ִ� CheckBox Checked �� return
		public static string GroupBoxChkValue(GroupBox groupBox1)
		{// ���� : string CheckedText = NamYoung.Common.Etc.PanelChkValue(panel1)
			string Rtn = "";
			foreach(System.Windows.Forms.Control c in groupBox1.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "CheckBox":
						CheckBox cb = (CheckBox)c;
						if(cb.Checked == true)
							if(Rtn.Length == 0)
								Rtn = cb.Text;
							else
								Rtn = Rtn +"|"+ cb.Text;
						break;
				}
			}
			return Rtn;
		}
		#endregion

		#region ����� ��Ϲ�ȣ
		public static string BusinessNo(string Txt)
		{// �޸� �ֱ�	NamYoung.Common.Etc.BusinessNo("2120852121");
			string Rtn= Txt;
			if(Txt.Length == 10)
			{
				Rtn = Rtn.Replace("-","");
				string temp = Rtn.ToString().Substring(0,3) +"-"+ Rtn.ToString().Substring(3,2) +"-"+ Rtn.ToString().Substring(5,5);
				Rtn = temp;
			}
			else
				Rtn = Txt;

			return Rtn;
		}
		#endregion

		#region �ֹε�Ϲ�ȣ
		public static string JuminNo(string Txt)
		{// �޸� �ֱ�	NamYoung.Common.Etc.BusinessNo("2120852121");
			string Rtn= Txt;
			if(Txt.Length == 13)
			{
				Rtn = Rtn.Replace("-","");
				string temp = Rtn.ToString().Substring(0,6) +"-"+ Rtn.ToString().Substring(6,7);
				Rtn = temp;
			}
			else
				Rtn = Txt;

			return Rtn;
		}
		#endregion

		#region Panel �� CheckBox Checked
		public static void PanelChkChecked(Panel panel1, string Value)
		{// ���� : NamYoung.Common.Etc.PanelChkChecked(panel11,"aaa-bbb-ccc");

			Value = Value.ToString().Replace("|","-");

			Regex r = new Regex("-");
			string[] s = r.Split(Value);

			foreach(System.Windows.Forms.Control c in panel1.Controls)                           
			{   
				if(c.GetType().Name == "CheckBox")
				{
					CheckBox cb = (CheckBox)c;
					for(int j = 0; j < s.Length; j++)
					{
						if(cb.Text == s[j])
							cb.Checked = true;
					}
				}
			}
		}
		#endregion

		#region GroupBox Exception Check
		public static bool GroupBoxException(GroupBox groupBox1)
		{// ���� : SystemBase.Base.GroupBoxException(groupBox1);
			return GroupBoxException2(groupBox1);
		}

		private static bool GroupBoxException2(Control ctls)
		{// ���� : SystemBase.Base.GroupBoxException(groupBox1);
			bool Rtn = true;
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Pink;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Linen;
						}
						break;

					case "TextBox":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Pink;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Linen;
						}
						break;
				}

				if(Rtn == false)
				{
					c.Focus();
					MessageBox.Show(SystemBase.Base.MessageRtn("SY007"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //�ʼ��׸��� �Է��Ͻʽÿ�.
					return false;
				}

				//User Contorl �ϰ�쿣 ���ȣ��
				//if(Convert.ToString(c.Tag) == "UC")
					Rtn = GroupBoxException2(c);
				
			}
			return Rtn;
		}
		#endregion

		#region GroupBox Exception Check �ű� Exception 2007-05-04

		public static bool GroupBoxExceptions(GroupBox groupBox1)
		{// ���� : SystemBase.Base.GroupBoxExceptions(groupBox1);
			return GroupBoxExceptions2(groupBox1);
		}

		public static bool GroupBoxExceptions(Panel groupBox1)
		{// ���� : SystemBase.Base.GroupBoxExceptions(groupBox1);
			return GroupBoxExceptions2(groupBox1);
		}

		private static bool GroupBoxExceptions2(Control ctls)
		{// ���� : SystemBase.Base.GroupBoxExceptions(groupBox1);
			bool Rtn = true;
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   
				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						//						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "1")
						//						{
						//							c.BackColor = Color.LightCyan;
						//						}
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.LightCyan;
						}
						break;

					case "TextBox":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						//						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "1")
						//						{
						//							c.BackColor = Color.LightCyan;
						//						}

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.LightCyan;
						}
						break;
					case "C1TextBox":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.LightCyan;
						}

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.LightCyan;
						}
						break;
					case "C1NumericEdit":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.LightCyan;
						}

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.LightCyan;
						}
						break;
					case "C1DateEdit":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "1")
						{
							c.BackColor = Color.LightCyan;
						}

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.Linen;
							c.Focus();
							Rtn = false;
						}
						else if(c.Tag != null && c.Text != "" && c.Tag.ToString() == "3")
						{
							c.BackColor = Color.LightCyan;
						}
						break;						
				}

				if(Rtn == false)
				{
					MessageBox.Show(SystemBase.Base.MessageRtn("SY007"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //�ʼ��׸��� �Է��Ͻʽÿ�.
					c.Focus();
					return false;
				}

				//User Contorl �ϰ�쿣 ���ȣ��
				//if(Convert.ToString(c.Tag) == "UC")
					Rtn = GroupBoxExceptions2(c);

			}
			
			return Rtn;
		}
		#endregion

		#region Panel Exception üũ
		public static bool PanelException(Panel panel1)
		{// ���� : NamYoung.Common.Etc.PanelException(panel1)
			return PanelException2(panel1);
		}

		private static bool PanelException2(Control ctls)
		{// ���� : NamYoung.Common.Etc.PanelException(panel1)
			bool Rtn = false;
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{   
				//MessageBox.Show(c.GetType().Name.ToString());

				switch(c.GetType().Name)
				{
					case "ComboBox":
						ComboBox cb = (ComboBox)c;

						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Pink;
							Rtn = true;
						}
						else
						{
							c.BackColor = Color.White;
						}
						break;

					case "TextBox":
						if(c.Tag != null && c.Text == "" && c.Tag.ToString() == "T")
						{
							c.BackColor = Color.Pink;
							Rtn = true;
						}
						else
						{
							c.BackColor = Color.White;
						}
						break;
				}

				//User Contorl �ϰ�쿣 ���ȣ��
				//if(Convert.ToString(c.Tag) == "UC")
					Rtn = PanelException2(c);
			}
			return Rtn;
		}
		#endregion

		#region TextBox Ű KeyPressEvent�� �޸� ó��
		public static void Comma(System.Windows.Forms.KeyPressEventArgs e, System.Windows.Forms.TextBox textBox1)
		{// �޸� �ֱ�	NamYoung.Common.Etc.Comma(e, textBox1);

			if ((Convert.ToInt32(e.KeyChar) > 47 && Convert.ToInt32(e.KeyChar) < 58))
			{   
				if (textBox1.Text.Length > 2)
				{
					string temp = textBox1.Text.Replace(",", "");     // e.KeyChar�� ���ڴ� ���� textBox�� ���� ���°� �ƴմϴ�.
					textBox1.Text = string.Format("{0:#,###}", Convert.ToInt64(temp + e.KeyChar.ToString()));
					textBox1.SelectionStart = textBox1.Text.Length;
					e.Handled = true;
				}
			}
			else if(Convert.ToInt32(e.KeyChar) == 8 )
			{
				if (textBox1.Text.Length > 2)
				{					
					string temp = textBox1.Text.Replace(",", "");     // e.KeyChar�� ���ڴ� ���� textBox�� ���� ���°� �ƴմϴ�.
					textBox1.Text = string.Format("{0:#,###}", Convert.ToInt64(temp));
					textBox1.SelectionStart = textBox1.Text.Length;
					e.Handled = true;
				}
			}
			else        // ���⼭ ���ͳ� �齺���̽� ��� �ٸ� Ű�ڵ忡 ���� ó���� �ϸ�.....
				e.Handled = true;
		}
		#endregion
		
		#region Text KeyPressEventArgs�� �޸� �ֱ� ���� : Comma(e, textBox1, 2);
		public static void Comma(System.Windows.Forms.KeyPressEventArgs e, System.Windows.Forms.TextBox textBox1, int Lan)
		{	// ���� : Comma(e, textBox1, 2);
			if( (Convert.ToInt32(e.KeyChar) > 47 && Convert.ToInt32(e.KeyChar) < 58) || Convert.ToInt32(e.KeyChar) == 8 ||  Convert.ToInt32(e.KeyChar) == 46)
			{   
				if(Convert.ToInt32(e.KeyChar) == 8 && textBox1.SelectionStart == 0)
				{// SelectionStart�� 0�̰� �齺���̽��϶� �ƹ��� ��ȭ ����
				}
				else
				{
					int Focus = textBox1.SelectionStart;
					int MFocus = 0;

					string TxtOr;
					if(Convert.ToInt32(e.KeyChar) == 8 && Focus > 0)	//BackSpace�� ������
					{
						if(textBox1.Text.Substring(textBox1.SelectionStart-1, 1) == ".")
						{
							TxtOr = textBox1.Text.Substring(0, textBox1.Text.Length - Lan -1) + textBox1.Text.Substring(textBox1.Text.Length - Lan -1, textBox1.Text.Length - textBox1.Text.Length + Lan -1);
							MFocus = -1;
						}
						else
							TxtOr = textBox1.Text.Substring(0, Focus-1) + textBox1.Text.Substring(Focus, textBox1.Text.Length - Focus);

					}
					else
						TxtOr = textBox1.Text.Substring(0, Focus) + e.KeyChar.ToString() + textBox1.Text.Substring(Focus, textBox1.Text.Length - Focus);

					if(e.KeyChar.ToString() == ".")
						TxtOr = textBox1.Text.Substring(0, Focus) + textBox1.Text.Substring(Focus, textBox1.Text.Length - Focus);

					string temp = TxtOr.Replace(",", "");

					if(Lan == 1)
						temp = string.Format("{0:#,###.0}", Convert.ToDecimal(temp));
					else if(Lan == 2)
						temp = string.Format("{0:#,###.00}", Convert.ToDecimal(temp));
					else if(Lan == 3)
						temp = string.Format("{0:#,###.000}", Convert.ToDecimal(temp));
					else if(Lan == 4)
						temp = string.Format("{0:#,###.0000}", Convert.ToDecimal(temp));
					else if(Lan == 5)
						temp = string.Format("{0:#,###.00000}", Convert.ToDecimal(temp));
					else if(Lan == 6)
						temp = string.Format("{0:#,###.000000}", Convert.ToDecimal(temp));
					else
						temp = string.Format("{0:#,###}", Convert.ToDecimal(temp));

					if(TxtOr.Length == 1)
						MFocus = MFocus + 0;
					else
						MFocus = MFocus + temp.Length - TxtOr.Length;

					if(temp.Length == 6 && MFocus > 1)
					{
						MFocus = 0;
					}

					int bbb = textBox1.Text.Substring(0, textBox1.SelectionStart).IndexOf(".",0);
					if(bbb > 0)
					{
						if(Convert.ToInt32(e.KeyChar) == 8)
						{
							MFocus = MFocus - 1;
						}
						else
						{
							MFocus = MFocus + 1;
						}
					}

					if(Convert.ToInt32(e.KeyChar) == 8)	//BackSpace�� Ŭ�������� Focus -2
						MFocus = MFocus -2;

					textBox1.Text = temp;

					textBox1.SelectionStart = Focus+1   + MFocus;
					e.Handled = true;
				}
			}
			else
				e.Handled = true;

		}
		#endregion

		#region Text�� �޸� �ֱ�
		public static string Comma2(string Txt)
		{// �޸� �ֱ�	SystemBase.Base.Comma2("55000");
			string Rtn="";
			if(Txt.Length > 2)
			{
				string temp = Txt.ToString().Replace(",", "");     // e.KeyChar�� ���ڴ� ���� textBox�� ���� ���°� �ƴմϴ�.
				Rtn = string.Format("{0:#,###}", Convert.ToInt64(temp));
			}
			else
				Rtn = Txt;

			return Rtn;
			//		if(c1FlexGrid1.Rows.Count > 1)	// ȣ�� ����
			//		c1FlexGrid1.Rows[c1FlexGrid1.Row][10] = NamYoung.Common.Etc.Comma2(c1FlexGrid1[c1FlexGrid1.Row, 10].ToString());
		}
		#endregion

		#region �ֹε�Ϲ�ȣ üũ
		public static bool JuminChk(string juminno)
		{//�ֹε�Ϲ�ȣ üũ
			bool Msg = false;
			string Jumin = juminno.Replace("-","");
			if(Jumin.Length != 13)
			{
				Msg = false;
			}
			else
			{
				int j = 2;
				int Cnt = 0;
				for(int i = 0; i < Jumin.Length-1; i++)
				{
					if(j > 1 && j < 10)
					{
						Cnt = Cnt + (j * Convert.ToInt32(Jumin.Substring(i,1).ToString()));
						j++;
					}
					else
					{
						j = 2;
						Cnt = Cnt + (j * Convert.ToInt32(Jumin.Substring(i,1).ToString()));
						j++;
					}
				}
				int Mod = Cnt % 11;
				Mod = 11 - Mod;
				Mod=Mod % 10;

				if(Mod != Convert.ToInt32(Jumin.Substring(12,1)) )
				{
					Msg = false;
				}
				else
				{
					Msg = true;
				}
			}
			return Msg;

			//			if(JuminChk(Jumin))
			//				MessageBox.Show("����");
		}
		#endregion

		#region �ֹε�Ϲ�ȣ �ߺ�üũ
		public static int JuminNOChk(string juminno)
		{//�ֹε�Ϲ�ȣ �ߺ�üũ
			int Msg = 0;
			string Jumin = juminno.Replace("-","");

			string Query = "Select Count(*) from MemberList Where Replace(Jumin,'-','') = '"+ Jumin.ToString() +"'";
			DataTable dt = DbOpen.NoTranDataTable(Query);

			Msg = Convert.ToInt32(dt.Rows[0][0].ToString());
			return Msg;
		}
		#endregion

		#region ��ȣȭEnCode
		public static string EnCode(string Str)
		{//��ȣȭ	EnCode(textBox2.Text);
			int[] NanSuArr = new int[]{5,7,0,6,1,8,3,4,9,2};
			string RtnStr = "";

			Random rnd = new Random();
			for(int i = 0; i < Str.Length; i++)
			{
				string Tmp = Str.Substring(i,1);
				int NanSu = rnd.Next(9);
				int TmpNanSu = NanSu + 65;
				string FirstStr = Convert.ToChar(TmpNanSu).ToString();

				int SecondMod = (Convert.ToInt32(Convert.ToChar(Tmp.ToString())) % 29) + 65 + NanSuArr[NanSu] ;
				string SecondStr = Convert.ToChar(SecondMod).ToString();

				int Thirdint = ((Convert.ToInt32(Convert.ToChar(Tmp.ToString())) - (Convert.ToInt32(Convert.ToChar(Tmp.ToString())) % 29)) / 29 ) + 76 + NanSuArr[NanSu];
				string ThirdStr = Convert.ToChar(Thirdint).ToString();

				RtnStr = RtnStr + FirstStr.ToString() + SecondStr.ToString() + ThirdStr.ToString();
			}
			return RtnStr;
		}
		#endregion
		
		#region ��ȣȭ DeCode
		public static string DeCode(string Str)
		{//��ȣȭ	DeCode(textBox1.Text);
			int[] NanSuArr = new int[]{5,7,0,6,1,8,3,4,9,2};
			string RtnStr = "";

			//Random rnd = new Random();
			for(int i = 0; i < Str.Length / 3; i++)
			{
				string Tmp1 = Str.Substring(((i+1)*3)-3,1);
				int First = Convert.ToChar(Convert.ToInt32(Convert.ToChar(Tmp1))) - 65;

				string Tmp2 = Str.Substring(((i+1)*3)-2,1);
				int Secondint = Convert.ToChar(Convert.ToInt32(Convert.ToChar(Tmp2))) - 65 - NanSuArr[First] ;

				string Tmp3 = Str.Substring(((i+1)*3)-1,1);
				int Thirdint = Convert.ToChar(Convert.ToInt32(Convert.ToChar(Tmp3))) - 76 - NanSuArr[First];

				RtnStr = RtnStr + Convert.ToChar(Convert.ToInt32(Convert.ToChar( ((Thirdint * 29) + Secondint) ))).ToString();
			}
			return RtnStr;
		}
		#endregion

		#region ��ȣȭ(S) Encode
		public static string  Encode( string strEncode )
		{
			int i = 0;
			int lens = 0;
			int conv = 0;
			string temp = "";
			string temps;

			temp = strEncode;
			lens = strEncode.Length;
			temp = "";

			for( i = 1; i < lens+1; i++ )
			{
				conv = i % 3;
				
				temps = strEncode.Substring(lens-i, 1);

				temp = temp + Convert.ToChar(Convert.ToInt32(Convert.ToChar(temps)) + conv);
			}

			return temp;
		}
		#endregion

		#region ��ȣȭ(S) Decode
		public static string  Decode( string strEncode )
		{
			int i = 0;
			int lens = 0;
			int conv = 0;
			string temp = "";
			string temps;

			temp = strEncode;
			lens = strEncode.Length;
			temp = "";

			for( i = lens; i > 0; i-- )
			{
				conv = i % 3;
				
				temps = strEncode.Substring(i-1, 1);

				temp = temp + Convert.ToChar(Convert.ToInt32(Convert.ToChar(temps)) - conv);
			}

			return temp;
		}
		#endregion

		#region ���� �޼���
		/******************************************************************************************************
		 * 	MessageBox.Show(SystemBase.Base.MessageRtn("P0002")); ��� : ���������� ó���Ǿ����ϴ�.
		 *  MessageBox.Show(SystemBase.Base.MessageRtn("P0002"), "����",MessageBoxButtons.OK);
		 *  MessageBox.Show("�޼���", "����",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		******************************************************************************************************/
		public static string MessageRtn( string MSG_CODE )
		{
			string Msg = "Message Code not Found (CODE : " +  MSG_CODE.Replace("'","'''") + ")";//�޼��� �ڵ尡 �߸� �ԷµǾ��ų� ��ϵ��� �ʾҽ��ϴ�.
			try
			{
				//Msg = "Message Code Fail (CODE : " + MSG_CODE + ")";//�޼��� �ڵ尡 �߸� �ԷµǾ��ų� ��ϵ��� �ʾҽ��ϴ�.
				string Query = "Select MSG_NAME From CO_SYS_MSG(Nolock) Where MSG_CODE = '"+ MSG_CODE.Replace("'","''") +"' ";
				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

				if(dt.Rows.Count > 0)
					Msg = dt.Rows[0][0].ToString();
				else
					Msg = MSG_CODE;
			}
			catch//(Exception e)
			{
				MessageBox.Show("�޼��� �ڵ尡 �߸� �ԷµǾ��ų� ��ϵ��� �ʾҽ��ϴ�.");
			}
			return Msg;
		}
		#endregion

		#region ���� �޼���
		/******************************************************************************************
		 *   - ȣ�� : MessageBox.Show(SystemBase.Base.MessageRtn("SY023", "5||7"));
		 *   - B0010�� ����� �޼��� �� : ||��° Row ||��° Į���� �ʼ��׸��Դϴ�.
		 *   - ��� : 5��° Row 7��° Į���� �ʼ��׸��Դϴ�.
		 * ***************************************************************************************/
		public static string MessageRtn( string MSG_CODE, string ReplaceMsg )
		{
			string Msg = "Message Code Fail (CODE : " + MSG_CODE + ")";
			try
			{
				string Query = "Select MSG_NAME From CO_SYS_MSG(Nolock) Where MSG_CODE = '"+ MSG_CODE.Replace("'","''") +"' ";
				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

				if(dt.Rows.Count > 0)
				{
					Msg = dt.Rows[0][0].ToString();

					ReplaceMsg = ReplaceMsg.Replace("||","#");

					Regex r = new Regex("#");
					string[] s = r.Split(ReplaceMsg);

					for(int i = 0; i < s.Length; i++)
					{
						if(Msg.IndexOf("||", 0) > -1)
						{
							Msg = Msg.Substring(0, Msg.IndexOf("||", 0)) + s[i].ToString() + Msg.Substring(Msg.IndexOf("||", 0)+2, Msg.Length - Msg.IndexOf("||", 0)-2);
						}
					}
				}
				else
				{
					Msg = MSG_CODE;

					ReplaceMsg = ReplaceMsg.Replace("||","#");

					Regex r = new Regex("#");
					string[] s = r.Split(ReplaceMsg);

					for(int i = 0; i < s.Length; i++)
					{
						if(Msg.IndexOf("||", 0) > -1)
						{
							Msg = Msg.Substring(0, Msg.IndexOf("||", 0)) + s[i].ToString() + Msg.Substring(Msg.IndexOf("||", 0)+2, Msg.Length - Msg.IndexOf("||", 0)-2);
						}
					}

				}

			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("MessageRtn ", f.ToString());
				MessageBox.Show("�޼��� �ڵ尡 �߸� �ԷµǾ��ų� ��ϵ��� �ʾҽ��ϴ�.");
			}
			return Msg;
		}
		#endregion

		#region NMȣ��
		public static string CodeName( string SCode, string Name, string Table, string Code, string AddQuery)
		{
			// ���� txtCust_Nm.Text = SystemBase.Base.CodeName("CUST_CD","CUST_NM", "B_CUST_INFO", txtCust_Cd.Text, "");

			string Query = "Select Isnull( " + Name + " ,'') From "+ Table +" (Nolock) Where "+ SCode +" = '"+ Code +"' "+ AddQuery +" ";
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			if(dt.Rows.Count > 0)
			{
				Name = dt.Rows[0][0].ToString();
			}
			else
			{
				Name = "";
			}

			return Name;
		}
		#endregion

		#region ���������翩��
		public static bool CodeCheck( string SCode, string Name, string Table, string Code, string AddQuery)
		{
			// ���� txtCust_Nm.Text = SystemBase.Base.CodeName("CUST_CD","CUST_NM", "B_CUST_INFO", txtCust_Cd.Text, "");

			string Query = "Select count(*) cnt From "+ Table +" (Nolock) Where "+ SCode +" = '"+ Code +"' "+ AddQuery +" ";
			int resultCnt = Convert.ToInt32(SystemBase.DbOpen.NoTranScalar(Query));

			if(resultCnt > 0)
				return true;
			else
				return false;
		}
		#endregion

		#region GroupBoxLang �� �ٱ��� ���� �� �̱۸�� �Է�â ������

		public static void GroupBoxLang(GroupBox groupBox1, string Lang, string FormName)
		{
			string Query = "SELECT SEQ, LBL_SEQ, LBL_NM FROM B_LABEL_NM WHERE LANG_CD = '"+ Lang.ToString() +"' AND FORM_ID = '"+ FormName.ToString() +"' AND GROUP_NM = '"+ groupBox1.Name.ToString() +"' ";
			DataSet ds = SystemBase.DbOpen.NoTranDataSet(Query);

			GroupBoxLang2(ds, groupBox1, Lang, FormName);
		}

		private static void GroupBoxLang2(DataSet ds, Control ctls, string Lang, string FormName)
		{// ���� : SystemBase.Base.GroupBoxLang(groupBox1);
			try
			{
				if(ds.Tables[0].Rows.Count > 0)
				{
					foreach(System.Windows.Forms.Control c in ctls.Controls)                           
					{   
						if(c.GetType().Name == "Label")
						{
							Label lb = (Label)c;
							if(lb.Tag != null && lb.Tag.ToString().Length > 0)
							{
								if(ds.Tables[0].Select("LBL_SEQ="+lb.Tag.ToString()) != null)
								{
									lb.Text = ds.Tables[0].Select("LBL_SEQ="+lb.Tag.ToString())[0][2].ToString();
								}
							}
						}
						else if(c.GetType().Name == "CheckBox")
						{
							CheckBox cb = (CheckBox)c;
							if(cb.Tag != null && cb.Tag.ToString().Length > 0)
							{
								if(ds.Tables[0].Select("LBL_SEQ="+cb.Tag.ToString()) != null)
								{
									cb.Text = ds.Tables[0].Select("LBL_SEQ="+cb.Tag.ToString())[0][2].ToString();
								}
							}
						}
						else if(c.GetType().Name == "RadioButton")
						{
							RadioButton rb = (RadioButton)c;
							if(rb.Tag != null && rb.Tag.ToString().Length > 0)
							{
								if(ds.Tables[0].Select("LBL_SEQ="+rb.Tag.ToString()) != null)
								{
									rb.Text = ds.Tables[0].Select("LBL_SEQ="+rb.Tag.ToString())[0][2].ToString();
								}
							}
						}
						else if(c.GetType().Name == "Button")
						{
							Button bb = (Button)c;
							if(bb.Tag != null && bb.Tag.ToString().Length > 0)
							{
								if(ds.Tables[0].Select("LBL_SEQ="+bb.Tag.ToString()) != null)
								{
									bb.Text = ds.Tables[0].Select("LBL_SEQ="+bb.Tag.ToString())[0][2].ToString();
								}
							}
						}
						else if(c.GetType().Name == "TextBox")
						{
							TextBox tb = (TextBox)c;
							if(tb.Tag != null && tb.Tag.ToString().Length > 0)
							{
								if(tb.Tag.ToString() == "1")
								{	// �ʼ��׸�
									tb.BackColor = Color.LightCyan;
								}
								else if(tb.Tag.ToString() == "2")
								{	// �б�����
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "3")
								{	// �б�����, �ʼ�
									tb.BackColor = Color.LightCyan;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "4")
								{	// �б�����, �ʼ�, ȸ��
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "5")
								{	// �ʼ��׸� ���� �빮�ڸ� ����
									tb.BackColor = Color.LightCyan;
									tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
								}
							}
						}
						else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
						{
							TextBox tb = (TextBox)c;
							if(tb.Tag != null && tb.Tag.ToString().Length > 0)
							{
								if(tb.Tag.ToString() == "1")
								{	// �ʼ��׸�
									tb.BackColor = Color.LightCyan;
								}
								else if(tb.Tag.ToString() == "2")
								{	// �б�����
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "3")
								{	// �б�����, �ʼ�
									tb.BackColor = Color.LightCyan;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "4")
								{	// �б�����, �ʼ�, ȸ��
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "5")
								{	// �ʼ��׸� ���� �빮�ڸ� ����
									tb.BackColor = Color.LightCyan;
									tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
								}
							}
						}
						else if(c.GetType().Name == "ComboBox")
						{
							ComboBox cb = (ComboBox)c;
							if(cb.Tag != null && cb.Tag.ToString().Length > 0)
							{
								if(cb.Tag.ToString() == "1" || cb.Tag.ToString() == "3")
								{
									cb.BackColor = Color.LightCyan;
								}
								else if(cb.Tag.ToString() == "2")
								{
									cb.BackColor = Color.Gainsboro;
									cb.Enabled = false;
								}
							}
						}
						else if(c.GetType().Name == "GroupBox")
						{
							GroupBox cb = (GroupBox)c;

							if(cb.Tag != null && cb.Tag.ToString().Length > 0)
							{
								if(ds.Tables[0].Select("LBL_SEQ="+cb.Tag.ToString()) != null)
								{
									cb.Text = ds.Tables[0].Select("LBL_SEQ="+cb.Tag.ToString())[0][2].ToString();
								}
							}
						}
						else if(c.GetType().Name == "C1DateEdit")
						{
							TextBox tb = (TextBox)c;
							if(tb.Tag != null && tb.Tag.ToString().Length > 0)
							{
								if(tb.Tag.ToString() == "1")
								{	// �ʼ��׸�
									tb.BackColor = Color.LightCyan;
								}
								else if(tb.Tag.ToString() == "2")
								{	// �б�����
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "3")
								{	// �б�����, �ʼ�
									tb.BackColor = Color.LightCyan;
									tb.ReadOnly = true;
								}
								else if(tb.Tag.ToString() == "4")
								{	// �б�����, �ʼ�, ȸ��
									tb.BackColor = Color.Gainsboro;
									tb.ReadOnly = true;
								}
							}
						}

						GroupBoxLang2(ds, c, Lang, FormName);

					}

				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("GroupBoxLang (�ٱ��� �� �̵�� ����)", f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0020"));

			}
		}
		#endregion

		#region GroupBoxLang() GroupBox�� �̱۸�� �Է�â �ʼ��׸� ������

		public static void GroupBoxLang(GroupBox groupBox1)
		{
			GroupBoxLang2(groupBox1);
		}

		private static void GroupBoxLang2(Control ctls)
		{// ���� : txtPlant_NM.Tag = "3";	SystemBase.Base.GroupBoxLang( groupBox1 );	
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{
				if(c.GetType().Name == "TextBox")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// �ʼ��׸�
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
							tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
						}
						else if(tb.Tag.ToString() == "2")
						{	// �б�����
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// �б�����, �ʼ�
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// �б�����, �ʼ�, ȸ��
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}

					}
				}
				else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// �ʼ��׸�
							tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
						}
						else if(tb.Tag.ToString() == "2")
						{	// �б�����
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// �б�����, �ʼ�
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// �б�����, �ʼ�, ȸ��
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}

					}
				}
				else if(c.GetType().Name == "ComboBox")
				{
					ComboBox cb = (ComboBox)c;
					if(cb.Tag != null && cb.Tag.ToString().Length > 0)
					{
						if(cb.Tag.ToString() == "1")
						{	// �ʼ��׸�
							cb.BackColor = Color.LightCyan;
							cb.Enabled = true;
						}
						else if(cb.Tag.ToString() == "2")
						{	// �б�����
							cb.BackColor = Color.Gainsboro;
							cb.Enabled = false;
						}
						else if(cb.Tag.ToString() == "3")
						{	// �б�����, �ʼ�
							cb.BackColor = Color.LightCyan;
							cb.Enabled = false;
						}
						else
						{
							cb.BackColor = Color.White;
							cb.Enabled = true;
						}
					}
				}
				else if(c.GetType().Name == "C1DateEdit")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// �ʼ��׸�
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
						}
						else if(tb.Tag.ToString() == "2")
						{	// �б�����
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// �б�����, �ʼ�
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// �б�����, �ʼ�, ȸ��
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
				}
				else if(c.GetType().Name == "DateTimePicker")
				{
					DateTimePicker dt = (DateTimePicker)c;

					if(dt.Tag != null && dt.Tag.ToString().Length > 0)
					{
						if(dt.Tag.ToString() == "1")
						{	// �ʼ��׸�
							dt.BackColor = Color.LightCyan;
							dt.Enabled = true;
						}
						else if(dt.Tag.ToString() == "2")
						{	// �б�����
							dt.BackColor = Color.Gainsboro;
							dt.Enabled = false;
						}
						else if(dt.Tag.ToString() == "3")
						{	// �б�����, �ʼ�
							dt.BackColor = Color.LightCyan;
							dt.Enabled = false;
						}
						else if(dt.Tag.ToString() == "4")
						{	// �б�����, �ʼ�, ȸ��
							dt.BackColor = Color.Gainsboro;
							dt.Enabled = false;
						}
						else
						{
							dt.BackColor = Color.White;
							dt.Enabled = true;
						}

					}
				}
				else if(c.GetType().Name == "Button")
				{
					Button bt = (Button)c;
					if(bt.Tag != null && bt.Tag.ToString() == "2") bt.Enabled = false;
					else bt.Enabled = true;
				}

				//User Contorl �ϰ�쿣 ���ȣ��
				//if(Convert.ToString(c.Tag) == "UC")
					GroupBoxLang2(c);

			}
		}
		#endregion

		#region PanelLang() Panel�� �̱۸�� �Է�â ������
		public static void PanelLang(Panel panel1)
		{
			PanelLang2(panel1);
		}
		
		private static void PanelLang2(Control ctls)
		{// ���� : txtPlant_NM.Tag = "3";	SystemBase.Base.GroupBoxLang( groupBox1 );	
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{
				if(c.GetType().Name == "TextBox")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// �ʼ��׸�
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
						}
						else if(tb.Tag.ToString() == "2")
						{	// �б�����
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// �б�����, �ʼ�
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// �б�����, �ʼ�, ȸ��
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
						tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
					}
				}
				else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// �ʼ��׸�
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
						}
						else if(tb.Tag.ToString() == "2")
						{	// �б�����
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// �б�����, �ʼ�
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// �б�����, �ʼ�, ȸ��
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}

						tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
					}
				}
				else if(c.GetType().Name == "ComboBox")
				{
					ComboBox cb = (ComboBox)c;
					if(cb.Tag != null && cb.Tag.ToString().Length > 0)
					{
						if(cb.Tag.ToString() == "1")
						{
							cb.BackColor = Color.LightCyan;
							cb.Enabled = true;
						}
						else if(cb.Tag.ToString() == "2")
						{
							cb.BackColor = Color.Gainsboro;
							cb.Enabled = false;
						}
						else if(cb.Tag.ToString() == "3")
						{
							cb.BackColor = Color.LightCyan;
							cb.Enabled = false;
						}
						else if(cb.Tag.ToString() == "4")
						{	// �б�����, �ʼ�, ȸ��
							cb.BackColor = Color.Gainsboro;
							cb.Enabled = false;
						}
						else
						{
							cb.BackColor = Color.White;
							cb.Enabled = true;
						}

					}
				}
				else if(c.GetType().Name == "C1DateEdit")
				{
					TextBox tb = (TextBox)c;
					if(tb.Tag != null && tb.Tag.ToString().Length > 0)
					{
						if(tb.Tag.ToString() == "1")
						{	// �ʼ��׸�
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = false;
						}
						else if(tb.Tag.ToString() == "2")
						{	// �б�����
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "3")
						{	// �б�����, �ʼ�
							tb.BackColor = Color.LightCyan;
							tb.ReadOnly = true;
						}
						else if(tb.Tag.ToString() == "4")
						{	// �б�����, �ʼ�, ȸ��
							tb.BackColor = Color.Gainsboro;
							tb.ReadOnly = true;
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}

					}
				}
				else if(c.GetType().Name == "DateTimePicker")
				{
					DateTimePicker dt = (DateTimePicker)c;

					if(dt.Tag != null && dt.Tag.ToString().Length > 0)
					{
						if(dt.Tag.ToString() == "1")
						{	// �ʼ��׸�
							dt.BackColor = Color.LightCyan;
							dt.Enabled = true;
						}
						else if(dt.Tag.ToString() == "2")
						{	// �б�����
							dt.BackColor = Color.Gainsboro;
							dt.Enabled = false;
						}
						else if(dt.Tag.ToString() == "3")
						{	// �б�����, �ʼ�
							dt.BackColor = Color.LightCyan;
							dt.Enabled = false;
						}
						else if(dt.Tag.ToString() == "4")
						{	// �б�����, �ʼ�, ȸ��
							dt.BackColor = Color.Gainsboro;
							dt.Enabled = false;
						}
						else
						{
							dt.BackColor = Color.White;
							dt.Enabled = true;
						}

					}
				}
				else if(c.GetType().Name == "Button")
				{
					Button bt = (Button)c;
					bt.Enabled = true;
				}

				//User Contorl �ϰ�쿣 ���ȣ��
				//if(Convert.ToString(c.Tag) == "UC")
					PanelLang2(c);

			}
		}
		#endregion

		#region GroupBoxLock �̱۸�� �Է�â ��ü Lock

		public static void GroupBoxLock(GroupBox groupBox1)
		{
			GroupBoxLock2(groupBox1);
		}

		private static void GroupBoxLock2(Control ctls)
		{// ���� : SystemBase.Base.GroupBoxLock( groupBox1 );	
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{
				if(c.GetType().Name == "TextBox")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "ComboBox")
				{
					ComboBox cb = (ComboBox)c;
					cb.BackColor = Color.Gainsboro;
					cb.Enabled = false;
				}
				else if(c.GetType().Name == "C1DateEdit")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "DateTimePicker")
				{
					DateTimePicker dt = (DateTimePicker)c;
					dt.BackColor = Color.Gainsboro;
					dt.Enabled = false;
				}
				else if(c.GetType().Name == "Button")
				{
					Button bt = (Button)c;
					bt.Enabled = false;
				}
				else if(c.GetType().Name == "RadioButton")
				{
					RadioButton rb = (RadioButton)c;
					rb.BackColor = Color.Gainsboro;
					rb.Enabled = false;
				}
				else if(c.GetType().Name == "CheckBox")
				{
					CheckBox rb = (CheckBox)c;
					rb.BackColor = Color.Gainsboro;
					rb.Enabled = false;
				}
				else if(c.GetType().Name == "Panel")
				{
					Panel pl = (Panel)c;
					pl.Enabled = false;
				}

				//User Contorl �ϰ�쿣 ���ȣ��
				//if(Convert.ToString(c.Tag) == "UC")
					GroupBoxLock2(c);

			}
		}
		#endregion

		#region �׷�ڽ� ��� ��Ʈ�Ѹ� ��, ����

		public static void GroupBoxLock(GroupBox groupBox1, bool Lock)
		{
			GroupBoxLock2(groupBox1, Lock);
		}

		private static void GroupBoxLock2(Control ctls, bool Lock)
		{// ���� : SystemBase.Base.GroupBoxLock( groupBox1 , true);	

			if(Lock == true)
			{
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						cb.BackColor = Color.Gainsboro;
						cb.Enabled = false;
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;
						dt.BackColor = Color.Gainsboro;
						dt.Enabled = false;
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = false;
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						rb.BackColor = Color.Gainsboro;
						rb.Enabled = false;
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						rb.BackColor = Color.Gainsboro;
						rb.Enabled = false;
					}
					else if(c.GetType().Name == "Panel")
					{
						Panel pl = (Panel)c;
						pl.Enabled = false;
					}

					GroupBoxLock2(c, Lock);

				}
			}
			else if(Lock == false)
			{	// �̱۸�� ������
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// �б�����
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}

							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// �б�����
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}

							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						if(cb.Tag != null && cb.Tag.ToString().Length > 0)
						{
							if(cb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								cb.BackColor = Color.LightCyan;
								cb.Enabled = true;
							}
							else if(cb.Tag.ToString() == "2")
							{	// �б�����
								cb.BackColor = Color.Gainsboro;
								cb.Enabled = false;
							}
							else if(cb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								cb.BackColor = Color.LightCyan;
								cb.Enabled = false;
							}
							else
							{
								cb.BackColor = Color.White;
								cb.Enabled = true;
							}
						}
						else
						{
							cb.BackColor = Color.White;
							cb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// �б�����
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// �б�����
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// �б�����
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;

						if(dt.Tag != null && dt.Tag.ToString().Length > 0)
						{
							if(dt.Tag.ToString() == "1")
							{	// �ʼ��׸�
								dt.BackColor = Color.LightCyan;
								dt.Enabled = true;
							}
							else if(dt.Tag.ToString() == "2")
							{	// �б�����
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								dt.BackColor = Color.LightCyan;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else
							{
								dt.BackColor = Color.White;
								dt.Enabled = true;
							}
						}
						else
						{
							dt.BackColor = Color.White;
							dt.Enabled = true;
						}
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = true;
					}
					else if(c.GetType().Name == "Panel")
					{
						Panel pl = (Panel)c;
						pl.Enabled = true;
					}

					GroupBoxLock2(c, Lock);

				}
			}
			
		}
		#endregion

		#region �׷�ڽ� ��� ��Ʈ�� T:��  F:�� ���� R:�ʱ�ȭ RD:��Ʈ�� ��� ����Ÿ���� �ʱ�ȭ
 
		public static void GroupBoxLock(GroupBox groupBox1, string Lock)
		{
			GroupBoxLock2(groupBox1, Lock);
		}

		private static void GroupBoxLock2(Control ctls, string Lock)
		{// ���� : SystemBase.Base.GroupBoxLock( groupBox1 , "T"); // T:��  F:�� ���� R:"�ʱ�ȭ" RD:��Ʈ�� ��� ����Ÿ���� �ʱ�ȭ 
			if(Lock == "T")
			{
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						cb.BackColor = Color.Gainsboro;
						cb.Enabled = false;
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.Gainsboro;
						tb.ReadOnly = true;
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;
						dt.BackColor = Color.Gainsboro;
						dt.Enabled = false;
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = false;
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						rb.BackColor = Color.Gainsboro;
						rb.Enabled = false;
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						rb.BackColor = Color.Gainsboro;
						rb.Enabled = false;
					}

					GroupBoxLock2(c, Lock);
			
				}
			}
			else if(Lock == "F")
			{	// ��� �� ����
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.White;
						tb.ReadOnly = false;
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.White;
						tb.ReadOnly = false;
					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						cb.BackColor = Color.White;
						cb.Enabled = true;
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						tb.BackColor = Color.White;
						tb.ReadOnly = false;
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;
						dt.BackColor = Color.White;
						dt.Enabled = true;
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = true;
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						rb.Enabled = true;
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						rb.Enabled = true;
					}

					GroupBoxLock2(c, Lock);


				}
			}
			else if(Lock == "R")
			{	// �ʱ�ȭ �̱۸�� ������
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// �б�����
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}

							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// �б�����
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}

							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						if(cb.Tag != null && cb.Tag.ToString().Length > 0)
						{
							if(cb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								cb.BackColor = Color.LightCyan;
								cb.Enabled = true;
							}
							else if(cb.Tag.ToString() == "2")
							{	// �б�����
								cb.BackColor = Color.Gainsboro;
								cb.Enabled = false;
							}
							else if(cb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								cb.BackColor = Color.LightCyan;
								cb.Enabled = false;
							}
							else
							{
								cb.BackColor = Color.White;
								cb.Enabled = true;
							}
						}
						else
						{
							cb.BackColor = Color.White;
							cb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// �б�����
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// �б�����
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// �б�����
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;

						if(dt.Tag != null && dt.Tag.ToString().Length > 0)
						{
							if(dt.Tag.ToString() == "1")
							{	// �ʼ��׸�
								dt.BackColor = Color.LightCyan;
								dt.Enabled = true;
							}
							else if(dt.Tag.ToString() == "2")
							{	// �б�����
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								dt.BackColor = Color.LightCyan;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else
							{
								dt.BackColor = Color.White;
								dt.Enabled = true;
							}
						}
						else
						{
							dt.BackColor = Color.White;
							dt.Enabled = true;
						}
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = true;
					}

					GroupBoxLock2(c, Lock);

				}
			}
			else if(Lock == "RD")
			{	// �ʱ�ȭ (����Ÿ, ��Ʈ��)
				foreach(System.Windows.Forms.Control c in ctls.Controls)                           
				{
					if(c.GetType().Name == "TextBox")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// �б�����
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}

							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
						tb.Text = "";
					}
					else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
								tb.Text = "0";

							}
							else if(tb.Tag.ToString() == "2")
							{	// �б�����
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = false;
								tb.Text = "0";
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
								tb.Text = "0";
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = false;
								tb.Text = "0";
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.Text = "0";
								tb.ReadOnly = false;
							}


							//tb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;//���� �빮�ڸ� ����
						}
						else
						{
							tb.BackColor = Color.White;
							tb.Text = "0";
							tb.ReadOnly = false;
						}

					}
					else if(c.GetType().Name == "ComboBox")
					{
						ComboBox cb = (ComboBox)c;
						if(cb.Tag != null && cb.Tag.ToString().Length > 0)
						{
							if(cb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								cb.BackColor = Color.LightCyan;
								cb.Enabled = true;
							}
							else if(cb.Tag.ToString() == "2")
							{	// �б�����
								cb.BackColor = Color.Gainsboro;
								cb.Enabled = false;
							}
							else if(cb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								cb.BackColor = Color.LightCyan;
								cb.Enabled = false;
							}
							else
							{
								cb.BackColor = Color.White;
								cb.Enabled = true;
							}
						}
						else
						{
							cb.BackColor = Color.White;
							cb.Enabled = true;
						}
						cb.SelectedIndex = 0;
					}
					else if(c.GetType().Name == "RadioButton")
					{
						RadioButton rb = (RadioButton)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// �б�����
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "CheckBox")
					{
						CheckBox rb = (CheckBox)c;
						if(rb.Tag != null && rb.Tag.ToString().Length > 0)
						{
							if(rb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = true;
							}
							else if(rb.Tag.ToString() == "2")
							{	// �б�����
								rb.BackColor = Color.Gainsboro;
								rb.Enabled = false;
							}
							else if(rb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								rb.BackColor = Color.LightCyan;
								rb.Enabled = false;
							}
							else
							{
								rb.BackColor = Color.White;
								rb.Enabled = true;
							}
						}
						else
						{
							rb.BackColor = Color.White;
							rb.Enabled = true;
						}
					}
					else if(c.GetType().Name == "C1DateEdit")
					{
						TextBox tb = (TextBox)c;
						if(tb.Tag != null && tb.Tag.ToString().Length > 0)
						{
							if(tb.Tag.ToString() == "1")
							{	// �ʼ��׸�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = false;
							}
							else if(tb.Tag.ToString() == "2")
							{	// �б�����
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								tb.BackColor = Color.LightCyan;
								tb.ReadOnly = true;
							}
							else if(tb.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								tb.BackColor = Color.Gainsboro;
								tb.ReadOnly = true;
							}
							else
							{
								tb.BackColor = Color.White;
								tb.ReadOnly = false;
							}
						}
						else
						{
							tb.BackColor = Color.White;
							tb.ReadOnly = false;
						}
						tb.Text = "";
					}
					else if(c.GetType().Name == "DateTimePicker")
					{
						DateTimePicker dt = (DateTimePicker)c;

						if(dt.Tag != null && dt.Tag.ToString().Length > 0)
						{
							if(dt.Tag.ToString() == "1")
							{	// �ʼ��׸�
								dt.BackColor = Color.LightCyan;
								dt.Enabled = true;
							}
							else if(dt.Tag.ToString() == "2")
							{	// �б�����
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "3")
							{	// �б�����, �ʼ�
								dt.BackColor = Color.LightCyan;
								dt.Enabled = false;
							}
							else if(dt.Tag.ToString() == "4")
							{	// �б�����, �ʼ�, ȸ��
								dt.BackColor = Color.Gainsboro;
								dt.Enabled = false;
							}
							else
							{
								dt.BackColor = Color.White;
								dt.Enabled = true;
							}
						}
						else
						{
							dt.BackColor = Color.White;
							dt.Enabled = true;
						}
						dt.Text = "";
					}
					else if(c.GetType().Name == "Button")
					{
						Button bt = (Button)c;
						bt.Enabled = true;
					}

					GroupBoxLock2(c, Lock);

				}
			}
		}

		#endregion

		#region PanelLock �̱۸�� �Է�â ��ü Lock

		public static void PanelLock(Panel panel1)
		{
			PanelLock2(panel1);
		}

		private static void PanelLock2(Control ctls)
		{
			foreach(System.Windows.Forms.Control c in ctls.Controls)                           
			{
				if(c.GetType().Name == "TextBox")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "C1TextBox" || c.GetType().Name == "C1NumericEdit")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "ComboBox")
				{
					ComboBox cb = (ComboBox)c;
					cb.BackColor = Color.Gainsboro;
					cb.Enabled = false;
				}
				else if(c.GetType().Name == "C1DateEdit")
				{
					TextBox tb = (TextBox)c;
					tb.BackColor = Color.Gainsboro;
					tb.ReadOnly = true;
				}
				else if(c.GetType().Name == "DateTimePicker")
				{
					DateTimePicker dt = (DateTimePicker)c;
					dt.BackColor = Color.Gainsboro;
					dt.Enabled = false;
				}
				else if(c.GetType().Name == "Button")
				{
					Button bt = (Button)c;
					bt.Enabled = false;
				}
				else if(c.GetType().Name == "RadioButton")
				{
					RadioButton rb = (RadioButton)c;
					rb.BackColor = Color.Gainsboro;
					rb.Enabled = false;
				}
				else if(c.GetType().Name == "CheckBox")
				{
					CheckBox rb = (CheckBox)c;
					rb.BackColor = Color.Gainsboro;
					rb.Enabled = false;
				}

				//User Contorl �ϰ�쿣 ���ȣ��
				//if(Convert.ToString(c.Tag) == "UC")
					PanelLock2(c);

			}
		}
		#endregion

		#region ���ȣ��
		public static DataTable SelfCall(DataTable dt, string S_ID, string M_ID, string P_ID, string[] GROUP)
		{// ���� : SystemBase.Base.SelfCall(dt, "1", "MY_ID", "PARENT_ID", new string[]{});
			DataTable dtRtn = new DataTable();	//Return�� DataTable

			ArrayList keyColumns = new ArrayList();

			string[] ColumnName = new string[dt.Columns.Count+1];	//Column�� ���庯��
			for(int i=0; i < dt.Columns.Count; i++)
			{
				ColumnName[i] = dt.Columns[i].ColumnName;	//Column�� �迭�� ����
				if((ColumnName[i] == M_ID) || (ColumnName[i] == P_ID))
				{
					dtRtn.Columns.Add(dt.Columns[i].ColumnName, typeof(string));//Column ����
				}
				else
				{
					dtRtn.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);//Column ����
				}
			}
			dtRtn.Columns.Add("TMP_YN", typeof(string));
			
			DataColumn key = dtRtn.Columns.Add("TMP_SEQ", typeof(string));//col.DataType
			keyColumns.Add(key);

			UniqueConstraint unique = new UniqueConstraint((DataColumn[])keyColumns.ToArray(typeof(DataColumn)), true);
			dtRtn.Constraints.Add(unique);

			string SEQ = "";

			int g = 0;
			string TMPSEQ1 = "";
			foreach (DataRow drTemp in dt.Select(M_ID +" = '"+S_ID + "' "))
			{
				g++;

				if(Convert.ToString(g).Length == 1){TMPSEQ1 = "00" + Convert.ToString(g);}
				else if(Convert.ToString(g).Length == 2){TMPSEQ1 = "0" + Convert.ToString(g);}
				else{TMPSEQ1 = Convert.ToString(g);}

				DataRow dr = dtRtn.NewRow();
				for(int i=0; i < dt.Columns.Count; i++)
				{
					dr[ColumnName[i]] = drTemp[ColumnName[i]];//���� ���� ����Ÿ ����
				}
				dr["TMP_YN"] = "Y";

				dr["TMP_SEQ"] = TMPSEQ1;
				SEQ = TMPSEQ1;
				dtRtn.Rows.Add(dr);
			}
			DataTable dtRtnTmp = SystemBase.Base.SelfCallDetail(dt, dtRtn, S_ID, SEQ, M_ID, P_ID, ColumnName);

			DataTable dtRtnOR = TMPSEQ(dtRtnTmp, keyColumns);

			return dtRtnOR;
		}


		public static DataTable SelfCallDetail(DataTable dt, DataTable dtRtn, string S_ID, string SEQ, string M_ID, string P_ID, string[] ColumnName)
		{
			int k = 0;
			foreach (DataRow drTemp in dt.Select(P_ID +" = '"+ S_ID +"'" ))
			{
				if(drTemp[M_ID].ToString() !=  S_ID.ToString() )
				{
					k++;
					DataRow dr = dtRtn.NewRow();
					for(int i=0; i < dt.Columns.Count; i++)
					{
						dr[ColumnName[i]]	= drTemp[ColumnName[i]];
					}
					dr["TMP_YN"] = "N";
				
					string TMPSEQ1, TMPSEQ2 = "";

					if(SEQ.Length == 1){TMPSEQ1 = "00" + SEQ;}
					else if(SEQ.Length == 2){TMPSEQ1 = "0" + SEQ;}
					else{TMPSEQ1 = SEQ;}

					if(Convert.ToString(k).Length == 1){TMPSEQ2 = "00" + Convert.ToString(k);}
					else if(Convert.ToString(k).Length == 2){TMPSEQ2 = "0" + Convert.ToString(k);}
					else{TMPSEQ2 = Convert.ToString(k);}

					dr["TMP_SEQ"] = TMPSEQ1 + TMPSEQ2;

					dtRtn.Rows.Add(dr);
				}
			}

			if(dtRtn.Select(" TMP_YN = 'N' ").Length > 0)
			{
				foreach (DataRow drTemp  in dtRtn.Select(" TMP_YN = 'N' "))
				{
					if( dtRtn.Select( M_ID+ " = '"+ drTemp[M_ID] +"' AND TMP_YN = 'Y' ").Length == 0 && S_ID.ToString() != drTemp[M_ID].ToString())
					{
						drTemp["TMP_YN"] = "Y";
						S_ID = drTemp[M_ID].ToString();
						SEQ = drTemp["TMP_SEQ"].ToString();
						SystemBase.Base.SelfCallDetail(dt, dtRtn, S_ID, SEQ, M_ID, P_ID, ColumnName);
					}
				}
			}

			return dtRtn;
		}


		public static DataTable SelfCall(DataTable dt, string S_ID, string M_ID, string P_ID)
		{	// ���� : SystemBase.Base.SelfCall(dt, "1", "MY_ID", "PARENT_ID");//���ȣ�� �θ�ID�� ���ȣ��
			DataTable dtRtn = new DataTable();	//Return�� DataTable

			ArrayList keyColumns = new ArrayList();

			string[] ColumnName = new string[dt.Columns.Count+1];	//Column�� ���庯��
			for(int i=0; i < dt.Columns.Count; i++)
			{
				ColumnName[i] = dt.Columns[i].ColumnName;	//Column�� �迭�� ����
				if((ColumnName[i] == M_ID) || (ColumnName[i] == P_ID))
				{
					dtRtn.Columns.Add(dt.Columns[i].ColumnName, typeof(string));//Column ����
				}
				else
				{
					dtRtn.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);//Column ����
				}
			}
			dtRtn.Columns.Add("TMP_YN", typeof(string));
			
			DataColumn key = dtRtn.Columns.Add("TMP_SEQ", typeof(string));//col.DataType
			keyColumns.Add(key);

			UniqueConstraint unique = new UniqueConstraint((DataColumn[])keyColumns.ToArray(typeof(DataColumn)), true);
			dtRtn.Constraints.Add(unique);

			string SEQ = "";

			int g = 0;
			string TMPSEQ1 = "";
			foreach (DataRow drTemp in dt.Select(P_ID +" = '"+S_ID + "' " ))
			{
				g++;

				if(Convert.ToString(g).Length == 1){TMPSEQ1 = "00" + Convert.ToString(g);}
				else if(Convert.ToString(g).Length == 2){TMPSEQ1 = "0" + Convert.ToString(g);}
				else{TMPSEQ1 = Convert.ToString(g);}

				DataRow dr = dtRtn.NewRow();
				for(int i=0; i < dt.Columns.Count; i++)
				{
					dr[ColumnName[i]] = drTemp[ColumnName[i]];//���� ���� ����Ÿ ����
				}
				dr["TMP_YN"] = "N";

				dr["TMP_SEQ"] = TMPSEQ1;
				SEQ = TMPSEQ1;
				dtRtn.Rows.Add(dr);
			}
			//S_ID = dtRtn.Rows[0][M_ID].ToString();
			DataTable dtRtnTmp = SystemBase.Base.SelfCallDetail2(dt, dtRtn, S_ID, SEQ, M_ID, P_ID, ColumnName);
			DataTable dtRtnOR = TMPSEQ(dtRtnTmp, keyColumns);

			return dtRtnOR;
		}

		public static DataTable SelfCallDetail2(DataTable dt, DataTable dtRtn, string S_ID, string SEQ, string M_ID, string P_ID, string[] ColumnName)
		{
			//int k = 0;
/*
			foreach (DataRow drTemp in dt.Select(P_ID +" = '"+ S_ID +"'" ))
			{
				if(drTemp[M_ID].ToString() !=  S_ID.ToString() )
				{
					k++;
					DataRow dr = dtRtn.NewRow();
					for(int i=0; i < dt.Columns.Count; i++)
					{
						dr[ColumnName[i]]	= drTemp[ColumnName[i]];
					}
					dr["TMP_YN"] = "N";
				
					string TMPSEQ1, TMPSEQ2 = "";

					if(SEQ.Length == 1){TMPSEQ1 = "00" + SEQ;}
					else if(SEQ.Length == 2){TMPSEQ1 = "0" + SEQ;}
					else{TMPSEQ1 = SEQ;}

					if(Convert.ToString(k).Length == 1){TMPSEQ2 = "00" + Convert.ToString(k);}
					else if(Convert.ToString(k).Length == 2){TMPSEQ2 = "0" + Convert.ToString(k);}
					else{TMPSEQ2 = Convert.ToString(k);}

					dr["TMP_SEQ"] = TMPSEQ1 + TMPSEQ2;

					dtRtn.Rows.Add(dr);
				}
			}
*/
			if(dtRtn.Select(" TMP_YN = 'N' ").Length > 0)
			{
				foreach (DataRow drTemp  in dtRtn.Select(" TMP_YN = 'N' "))
				{
					if( dtRtn.Select( P_ID+ " = '"+ drTemp[M_ID] +"' AND TMP_YN = 'Y' ").Length == 0 && S_ID.ToString() != drTemp[M_ID].ToString())
					{
						drTemp["TMP_YN"] = "Y";
						S_ID = drTemp[M_ID].ToString();
						SEQ = drTemp["TMP_SEQ"].ToString();
						SystemBase.Base.SelfCallDetail(dt, dtRtn, S_ID, SEQ, M_ID, P_ID, ColumnName);
					}
				}
			}

			return dtRtn;
		}


		public static DataTable TMPSEQ(DataTable dt, ArrayList keyColumn)
		{
			DataView dv = new DataView(dt);
			dv.Sort = " TMP_SEQ ASC ";

			DataTable tdt = new DataTable("TmpTable2");
			DataView tdv = new DataView(tdt);

			ArrayList keyColumns = new ArrayList();
			foreach (DataColumn col in dt.Columns)
			{
				string colName = col.ColumnName;
				bool KeyChk = true;
				for(int k = 0; k < keyColumn.Count; k++)
				{
					if(keyColumn[k].ToString() == colName)
					{
						DataColumn key = tdt.Columns.Add(colName, col.DataType);//col.DataType
						keyColumns.Add(key);

						KeyChk = false;
					}
				}
				if(KeyChk)
				{
					tdt.Columns.Add(colName, col.DataType);//col.DataType
				}
			}

			UniqueConstraint unique = new UniqueConstraint(
				(DataColumn[])keyColumns.ToArray(typeof(DataColumn)), true);
			tdt.Constraints.Add(unique);

			Hashtable pivotNames = new Hashtable();
			foreach (DataRowView drv in dv)
			{
				object[] keys = new object[keyColumns.Count];

				for (int i = 0; i < keyColumns.Count; i++)
				{
					string name = ((DataColumn)keyColumns[i]).ColumnName;
					keys[i] = drv[name];
				}
				DataRow drp = tdt.Rows.Find(keys);
				
				if (drp == null)
				{
					drp = tdt.NewRow();
					foreach (DataColumn col in dt.Columns)
					{
						string name = col.ColumnName;
						drp[name] = drv[name];	// Ű�� Į���� Row ����Ÿ ���
					}
					tdt.Rows.Add(drp);
				}
			}
			/**************************************************************************/
			
			DataTable dtRtnTmp = new DataTable();	//Return�� DataTable

			string[] ColumnName = new string[tdt.Columns.Count-2];	//Column�� ���庯��
			for(int i=0; i < tdt.Columns.Count-2; i++)
			{
				ColumnName[i] = tdt.Columns[i].ColumnName;	//Column�� �迭�� ����
				dtRtnTmp.Columns.Add(tdt.Columns[i].ColumnName, tdt.Columns[i].DataType);//Column ����
			}

			DataView dv2 = new DataView(tdt);
			foreach (DataRowView drv in dv2)
			{
				DataRow drp = dtRtnTmp.NewRow();

				for(int k = 0; k < ColumnName.Length; k++)	
				{
					drp[ColumnName[k]] = drv[ColumnName[k]];	// Ű�� Į���� Row ����Ÿ ���
				}
				dtRtnTmp.Rows.Add(drp);

			}
			return dtRtnTmp;
		}

		#endregion

		#region SVTM �����ð� ȣ��  SVTM �����ð� ȣ��	YYMMDD : 2007-12-26  YMD : 20071226  "" : 2007-12-26 16:43:55
		public static string ServerTime(string Kind)
		{// ���� : SystemBase.Base.ServerTime("YMD");
			string RtnMsg = "";
			string Query = " usp_TIME '"+ Kind +"' ";
			DataTable DT = SystemBase.DbOpen.NoTranDataTable(Query);

			if(DT.Rows.Count > 0)
				RtnMsg = DT.Rows[0][0].ToString();

			return RtnMsg;
		}
		#endregion

		#region ������
		public static double MyRound(double value, int digits) 
		{	//���� : MyRound(Convert.ToDouble(textBox1.Text), 1);
			int sign = Math.Sign(value);
			double scale = Math.Pow(10.0, digits);
			double round = Math.Floor(Math.Abs(value) * scale + 0.5);
			return (sign * round / scale);
		}
		#endregion

        #region GridHeadIndex - �׸��� Head Index Return
        public static int GridHeadIndex(string[,] GHIdx, string HeadName)
        {	// fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "TRV NO")].Text
            int RtnGridHeadIndex = 0;
            for (int i = 0; i < GHIdx.Length / 2; i++)
            {
                if (GHIdx[i, 0].Trim().ToUpper() == HeadName.Trim().ToUpper())
                {
                    RtnGridHeadIndex = Convert.ToInt32(GHIdx[i, 1].Trim());
                    return RtnGridHeadIndex;
                }
            }
            return RtnGridHeadIndex;
        }
        #endregion

		#region GTP �׸��� ����
        //public static void GantGridSet(object BaseGantt, int MaxCol, string MinDt, string MaxDt, double StDt, double EdDt, string[] Title, string [] CellType,  int[] Width, string[] HorzAlign, string FontName, float FontSize)
        //{

        //}

		/// <summary>
		/// GTP�׸��弼��
		/// </summary>
		/// <param name="BaseGantt">Gantt1</param>
		/// <param name="MaxCol">��Col��</param>
		/// <param name="MinDt">Dafault�� ������ �ּ�����</param>
		/// <param name="MaxDt">Dafault�� ������ �ִ�����</param>
		/// <param name="StDt">Dafault�� ������ ���糯¥(+-) �ּ�����, double���� DateTime.Add() ��ȣ���� ����</param>
		/// <param name="EdDt">Dafault�� ������ ���糯¥(+-) �ִ�����, double���� DateTime.Add() ��ȣ���� ����</param>
		/// <param name="Title">�������, string �迭����</param>
		/// <param name="CellType">Col�Ӽ�, string �迭����</param>
		/// <param name="Width">Col����, int �迭����</param>
		/// <param name="HorzAlign">����������, string �迭����(L,C,R)</param>
		/// <param name="FontName">��Ʈ string����</param>
		/// <param name="FontSize">��Ʈ������ float����</param>
		public static void GantGridSet(PlexityHide.GTP.Gantt BaseGantt, int MaxCol, string MinDt, string MaxDt, double StDt, double EdDt, string[] Title, string [] CellType,  int[] Width, string[] HorzAlign, string FontName, float FontSize)
		{
			//line, ��¥ ����
			BaseGantt.GridProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;				// BorderStyle
			BaseGantt.DateScaler.CultureInfoDateTimeFormat = new CultureInfo("ko-KR", false ).DateTimeFormat;	// ��¥ ����
			BaseGantt.DateScalerProperties.LowerBound = Convert.ToDateTime(MinDt);								// �ּ�����
			BaseGantt.DateScalerProperties.UpperBound = Convert.ToDateTime(MaxDt);								// �ִ�����
			BaseGantt.DateScalerProperties.ShowWeekNumbers = true;												
			BaseGantt.VerticalDayStripes = true;
			BaseGantt.TodayLine = true;
			BaseGantt.DateScalerProperties.StartTime = DateTime.Now.AddDays(StDt);								// ��Ʈ ��� ������ ��������
			BaseGantt.DateScalerProperties.StopTime = DateTime.Now.AddDays(EdDt);								// ��Ʈ ��� ������ ��������
			BaseGantt.Grid.GridStructure.RowSelect = true;														//Single Select
			
			//�׸��� ���̾ƿ� ����
			//default center����
			PlexityHide.GTP.CellLayout BaseLayout = new PlexityHide.GTP.CellLayout();
			BaseLayout.Name = "New";
			BaseLayout.Font = new System.Drawing.Font(FontName,FontSize);
			BaseLayout.HeaderBackgroundColor = Color.WhiteSmoke;
			BaseLayout.HeaderBackgroundGradiantColor = Color.WhiteSmoke;
			BaseLayout.HorzAlign = System.Drawing.StringAlignment.Center;
			BaseLayout.VertAlign = System.Drawing.StringAlignment.Center;
			BaseLayout.HeaderBackgroundColor = Color.LightGoldenrodYellow;
			//left ����
			PlexityHide.GTP.CellLayout LeftLayout = new PlexityHide.GTP.CellLayout();
			LeftLayout.Name = "Left";
			LeftLayout.Font = new System.Drawing.Font(FontName, FontSize);
			LeftLayout.HeaderBackgroundColor = Color.WhiteSmoke;
			LeftLayout.HeaderBackgroundGradiantColor = Color.WhiteSmoke;
			LeftLayout.HorzAlign = System.Drawing.StringAlignment.Near;
			LeftLayout.VertAlign = System.Drawing.StringAlignment.Center;
			LeftLayout.HeaderBackgroundColor = Color.LightGoldenrodYellow;
			//right ����
			PlexityHide.GTP.CellLayout RightLayout = new PlexityHide.GTP.CellLayout();
			RightLayout.Name = "Right";
			RightLayout.Font = new System.Drawing.Font(FontName, FontSize);
			RightLayout.HeaderBackgroundColor = Color.WhiteSmoke;
			RightLayout.HeaderBackgroundGradiantColor = Color.WhiteSmoke;
			RightLayout.HorzAlign = System.Drawing.StringAlignment.Far;
			RightLayout.VertAlign = System.Drawing.StringAlignment.Center;
			RightLayout.HeaderBackgroundColor = Color.LightGoldenrodYellow;

			int iWidth = 30;	//�׸��� ����

			for (int i = 0; i < MaxCol; i++)
			{	
				switch(CellType[i])
				{
					case "MT" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.MultiText);
						break;
					case "BC" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.BoolCheck);
						break;
					case "CT" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.ComboText);
						break;
					case "TD" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.TimeDate);
						break;
					case "TS" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.TimeSpan);
						break;
					case "CC" :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.CustomCell);
						break;
					default :
						BaseGantt.GridProperties.Columns.InsertNew(i,PlexityHide.GTP.CellType.SingleText);
						break;
				}

				switch(HorzAlign[i])
				{
					case "C" :
						BaseGantt.GridProperties.Columns[i].LayoutName = "New";
						break;
					case "R" :
						BaseGantt.GridProperties.Columns[i].LayoutName = "Right";
						break;
					default :
						BaseGantt.GridProperties.Columns[i].LayoutName = "Left";
						break;
				}
				BaseGantt.GridProperties.Columns[i].Title = Title[i].ToString();
				BaseGantt.GridProperties.Columns[i].Width = Width[i];
				BaseGantt.GridProperties.Columns[i].ReadOnly = true;

				BaseGantt.GridProperties.CellLayouts.Add(BaseLayout);
				BaseGantt.GridProperties.CellLayouts.Add(LeftLayout);
				BaseGantt.GridProperties.CellLayouts.Add(RightLayout);

				iWidth += Width[i];
			}
				
			BaseGantt.GridWidth = iWidth;
        }
		#endregion

		#region �ü����
		public static int TimeCheck(string StartTm, string EndTm, int DayCount)
		{
			int Time = 0;
			string Time_T = "";		//��
			string Time_M = "";		//��
			int Time_S = 0;			//���۽ü�
			int Time_E = 0;			//����ü�

			try
			{
				if(StartTm != "" && EndTm != "")
				{
					//���۽ð�, ����ð��� ������ 0�̴�
					if(DayCount == 0 && StartTm == EndTm)
					{
						Time = 0;
					}
					else
					{
						Time_T = StartTm.Substring(0,2);
						Time_M = StartTm.Substring(2,2);
						Time_S = Convert.ToInt32(Time_T) * 60 + Convert.ToInt32(Time_M);

						Time_T = EndTm.Substring(0,2);
						Time_M = EndTm.Substring(2,2);
						Time_E = Convert.ToInt32(Time_T) * 60 + Convert.ToInt32(Time_M);

						Time = (DayCount * 1440) + Time_E - Time_S;
					}
				}
			}
			catch(Exception f)
			{
				Loggers.Log("TimeCheck", f.Message);
				MessageBox.Show(SystemBase.Base.MessageRtn("SY008","�ü����"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return Time;
		}
		#endregion

		#region ����ù�ȣ ã��
		public static string RoutingNo(string Type)
		{
			string RoutNo = "";

			string Query = "SELECT MINOR_CODE FROM B_COMM_CODE(NOLOCK) WHERE COMM_CODE = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' AND MAJOR_CODE = 'P018' AND REL_CD1 = '"+ Type +"' ";
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			if(dt.Rows.Count > 0)
			{
				RoutNo = dt.Rows[0][0].ToString();
			}
			else
			{
				RoutNo = "";
			}

			return RoutNo;
		}
		#endregion

		#region ���ε�
		
		public static void MDIFORM(string MdiFM, string MenuName, string MenuID, string[] fieldName, string[] fieldValue)
		{	// �ٸ� ������Ʈ �� �ε�
			try
			{
				if(MdiFM.Length > 2)
				{
					string TMdiFM = MdiFM.ToString();
					// ������ . �� �������ڿ��ִ°��� Ŭ�������̸�, �տ����� ��� ���ӽ����̽��� �����Ѵ�.
					//						string NamespaceName	= TMdiFM.Substring(0, TMdiFM.IndexOf(".",0, TMdiFM.Length-1) );
					string NamespaceName	= TMdiFM.Substring(0, TMdiFM.LastIndexOf("."));
					//						string RodeFormName		= TMdiFM.Substring(TMdiFM.IndexOf(".",0, TMdiFM.Length)+1, TMdiFM.Length-TMdiFM.IndexOf(".",0, TMdiFM.Length)-1 );
					string RodeFormName		= TMdiFM.Substring(TMdiFM.LastIndexOf(".") + 1, TMdiFM.Length - TMdiFM.LastIndexOf(".") -1);

					SystemBase.Base.RodeFormName	= TMdiFM.ToString();
					SystemBase.Base.RodeFormID		= MenuID.ToString();
					SystemBase.Base.RodeFormText	= SystemBase.Base.CodeName ("MENU_ID","MENU_NAME","CO_SYS_MENU", MenuID.ToString(),"");
					BaseForm.GetType().GetProperty("StatusBarText").SetValue(BaseForm, MenuID.ToString(), null);

					bool mdiwin = false;
					Form tempForm = null;
					for(int i=0; i < BaseForm.MdiChildren.Length; i++)
					{	// ���� �̹� ���������� �������� ��������
						if(BaseForm.MdiChildren[i].Name == RodeFormName)
						{
							BaseForm.MdiChildren[i].BringToFront();
							tempForm = BaseForm.MdiChildren[i];
							mdiwin = true;
							break;
						}
					}

					if(mdiwin == false)
					{
						Assembly ServiceAssembly = Assembly.LoadFile( ProgramWhere.ToString() +"\\" + NamespaceName.ToString() + ".dll" );
						Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(MdiFM.ToString()));

						for(int i=0; i<fieldName.Length; i++)
						{
							myForm.GetType().GetProperty(fieldName[i]).SetValue(myForm, fieldValue[i], null);
						}

						myForm.MdiParent = BaseForm;
						myForm.Show();
					}
					else
					{
						for(int i=0; i<fieldName.Length; i++)
						{
							tempForm.GetType().GetProperty(fieldName[i]).SetValue(tempForm, fieldValue[i], null);
						}

						tempForm.Activate();
					}

				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(BaseForm.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SY006"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region SHDBNULL

		public static object SH_DBNULL(object val, object defaultValue)
		{
			object objVal = null;

			if(val != null) 
			{
				if(val.Equals(System.DBNull.Value))
				{
					objVal = defaultValue;
				}
				else
				{
					objVal = val;
				}
			}

			return objVal;
		}

		public static object SH_DBNULL(object val, object defaultValue, Type type)
		{
			object objVal = null;

			if(val != null) 
			{
				if(val.Equals(System.DBNull.Value))
				{
					objVal = defaultValue;
				}
				else
				{
					objVal = val;
				}
			}

			if(type.Equals(typeof(Int32)))
				objVal =  Convert.ToInt32(objVal);
			else if(type.Equals(typeof(Int16)))
				objVal =  Convert.ToInt16(objVal);
			else if(type.Equals(typeof(Int64)))
				objVal =  Convert.ToInt64(objVal);
			else if(type.Equals(typeof(decimal)))
				objVal =  Convert.ToDecimal(objVal);
			else if(type.Equals(typeof(DateTime)))
				objVal =  Convert.ToDateTime(objVal);
			else if(type.Equals(typeof(String)))
				objVal =  Convert.ToString(objVal);
			else if(type.Equals(typeof(Double)))
				objVal =  Convert.ToDouble(objVal);

			return objVal;
		}

		#endregion

        #region �� ����� ������ ������ ����
        public static int GetMonthDiff(string date1, string date2)
        {
            int GetMonth = 0;
            int d1 = 0; int d2 = 0;
            // ������ 1�� ���� ���
            DateTime Date1 = Convert.ToDateTime(date1.Substring(0, 7) + "-01");
            DateTime Date2 = Convert.ToDateTime(date2.Substring(0, 7) + "-01");

            d1 = Date1.Month;
            d2 = Date2.Month;

            GetMonth = (d2 - d1) + 1;
            return GetMonth;
        }
        #endregion

        #region ��,�� ���� ���� ���ϱ�
        public static string GetYearMonth(string date1, int InterVal, string Get_Flag)
        {
            string GetYearMonth = "";
            DateTime Date1 = Convert.ToDateTime(date1);

            if (Get_Flag == "Y")
            {
                GetYearMonth = Date1.AddYears(InterVal).ToString();
            }
            else if (Get_Flag == "M")
            {
                GetYearMonth = Convert.ToDateTime(Date1.AddYears(InterVal).ToString()).Year.ToString() + "-12-" + Convert.ToDateTime(Date1.AddYears(InterVal).ToString()).Day.ToString();
            }

            return GetYearMonth;
        }
        #endregion

        #region �������α׷��� �ֻ��� �޴������ ǥ��..
        public static string GetMenuTree(string MenuId)
        {
            string GetMenu = "" ;

            string strSql = " usp_MENU_TREE ";
            strSql += "  @pMENU_ID = '" + MenuId + "' ";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count ; i++)
                {
                    if (i == ds.Tables[0].Rows.Count - 1)
                    {
                        GetMenu += ds.Tables[0].Rows[i]["MENU_NAME"].ToString() ;
                    }
                    else
                    {
                        GetMenu += ds.Tables[0].Rows[i]["MENU_NAME"].ToString() + " > ";
                    }
                }
            }

            return GetMenu;
        }
        #endregion

        #region ���� ��ư���°� ��������
        public static string GetToolbarButtonStatus(string UserID, string PGM_ID)
        {
            string status = "";

            //��ư����
            string strSql = "usp_TOOLBARSET '" + UserID + "','" + PGM_ID + "'";
            DataTable Rowdt = SystemBase.DbOpen.NoTranDataTable(strSql);

            if (Rowdt.Rows.Count > 0)
            {
                for (int i = 0; i < Rowdt.Columns.Count; i++)
                {
                    status += Rowdt.Rows[0][i].ToString();
                }
            }

            return status;
        }
        #endregion

        #region ���� Cell�� ���ϱ�
        public static string ExCol(int Col)
        {
            string strCol = "";

            strCol = Convert.ToChar(Col + 64).ToString();

            return strCol;
        }

        public static string ExcelCol(int Col)
        {
            string strCol = "";

            String columName = "";
            String columName2 = "";
 
            int tmpI = 0;
            int tmpJ = 0;

            if (Col > 26)
            {
                tmpJ = Col % 26;
                tmpI = Col / 26;
 
                if (tmpI > 26)
                {
                    if (tmpJ == 0)
                    {
                        tmpJ = 1;
                        tmpI -= 1;
                    }

                    columName = ExcelCol(tmpI).ToString();
                    columName2 = ExCol(tmpJ).ToString();
                }
                else
                {
                    if (tmpJ == 0)
                    {
                        tmpJ = 1;
                        tmpI -= 1;
                    }
                    columName = ExCol(tmpI).ToString();
                    columName2 = ExCol(tmpJ).ToString();
                }
            }
            else
            {
                columName = ExCol(Col);
            }

            strCol = columName + columName2;

            return strCol;
        }
        #endregion

        #region �ܾ� Ư�� ���� ��ȸ(�λ�/����)
        public static string OverRollSearch(string strType, string strDt, string strInternalCd)
        {
            string strRoll = "N";

            string Query = "usp_H_COMMON 'H015', , @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'@pCOM_CD = '" + strType + "', @pDATE = '" + strDt + "', @pCOM_NM = '" + strInternalCd + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                strRoll = dt.Rows[0][0].ToString();
            }

            return strRoll;
        }
        #endregion

        #region �λ���� ����
        public static string HumanRoll(string strEmpNo)
        {
            string strRoll = "N";

            string Query = "usp_H_COMMON 'H016', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "', @pCOM_CD = '" + strEmpNo + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                strRoll = dt.Rows[0][0].ToString();
            }

            return strRoll;
        }
        #endregion

        #region INI �� �б� ����
        // INI �� �б� 
        public String GetIniValue(String Section, String Key, String iniPath)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniPath);
            return temp.ToString();
        }

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        #endregion

        #region ���� ����� ������ ���� (2014.7.16, ���籤)
        /// <summary>
        /// ���� ����� ������ �����մϴ�. ���� ����� �ʵ尡 2�� �ʿ��ϸ� ù ��°�� Ű, �� ��°�� ���� �˴ϴ�.
        /// </summary>
        /// <param name="query">����</param>
        /// <returns>����</returns>
        public static Dictionary<string, string> CreateDictionary(string query)
        {
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(query);
            Dictionary<string, string> dic = new Dictionary<string, string>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Columns.Count > 1)
                    dic.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
                else
                    dic.Add(dt.Rows[i][0].ToString(), dt.Rows[i][0].ToString());
            }

            return dic;
        }
        #endregion

    }
}
