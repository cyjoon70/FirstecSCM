using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

namespace SystemBase
{
	/// <summary>
	/// Admin Program�� �⵿�Ҷ� �ʿ��� ���� ������ �������� Class�̴�.
	/// �����ڰ� Static���� ������� �ֱ� ������ ���α׷� �⵿�� �ڵ����� ��� ���ð���
	/// ������ ������ �Ѵ�.
	/// </summary>
	public class Settings
	{
		//prohibit instance creation
		private Settings()
		{
		}

		static Settings()
		{
			Init();
		}

		/* -----------------------------------------------
			* Attribute
		   ----------------------------------------------- */

		// connection string
		private static string strConnectionString;

		// Log Level;
		private static int intLogLevel;

		// AdminOnly
		private static string strAdminOnly;

		// WorkStartTime
		private static string strWorkStartTime;

		// VinNo ���� ���������ڵ� ������ġ;
		private static int intCkndCdPosInVinNo;

		// VinNo ���� ���������ڵ� ����;
		private static int intCkndCdLenInVinNo;

		// ����
		private static string strLanguage;

		// Company Code
		private static string strCompanyCode;

		/* -----------------------------------------------
			* Method
		   ----------------------------------------------- */

		public static string ConnectionString
		{
			get { return strConnectionString; }
		}

		public static int LogLevel
		{
			get { return intLogLevel; }
			set { intLogLevel = value; }
		}

		public static string AdminOnly
		{
			get { return strAdminOnly; }
			set { strAdminOnly = value; }
		}

		public static string WorkStartTime
		{
			get { return strWorkStartTime; }
			set { strWorkStartTime = value; }
		}

		public static int CkndCdPosInVinNo
		{
			get { return intCkndCdPosInVinNo; }
		}

		public static int CkndCdLenInVinNo
		{
			get { return intCkndCdLenInVinNo; }
		}
		
		public static string Language
		{
			get { return strLanguage; }
			set { strLanguage = value; }
		}
		
		public static string CompanyCode
		{
			get { return strCompanyCode; }
		}

		/* -----------------------------------------------
			* User Define Module
		   ----------------------------------------------- */

		private static void Init()
		{
			try
			{
				// ȯ�漳�� XML File Load
				string configFile = new Settings().GetType().Assembly.Location + ".xml";
				XmlDocument doc = new XmlDocument();
				doc.Load(configFile);

				XmlElement elem = null;

				StringDictionary settings = new StringDictionary();

				// ���� �о� �� node�κ��� �����Ѵ�. �� XML���Ͽ��� Set���� �����ϴ� �κ��� Key, Value�κ��� �����´�.
				XmlNodeList nodes = doc.SelectNodes("//set");

				for (int i = 0; i < nodes.Count; i++)
				{
					elem = (XmlElement)nodes[i];
					string key = elem.GetAttribute("key");
					string val = elem.GetAttribute("value");
					settings.Add(key, val);

					// Xml������ Key, Value���� �α׿� �����Ѵ�.
					Logger.StaticLog("CONFIG: KEY=" + key + ", VALUE=" + val);
				}
				
				// XML File�� ����� ������ Static ������ �����Ѵ�.
				strConnectionString		= settings["ConnectionString"];
				intLogLevel				= Convert.ToInt32(settings["LogLevel"]);
				strAdminOnly			= settings["AdminOnly"];
				strWorkStartTime		= settings["WorkStartTime"];
				intCkndCdPosInVinNo		= Convert.ToInt32(settings["CkndCdPosInVinNo"]) - 1; // String Index �� 0 ���� �����ϹǷ� ���� ��ġ�� -1 �� ��
				intCkndCdLenInVinNo		= Convert.ToInt32(settings["CkndCdLenInVinNo"]);
				strLanguage				= settings["Language"];
				strCompanyCode			= settings["CompanyCode"];
			}
			catch (Exception ex)
			{
				Logger.StaticLog(ex.Message + " " + ex.StackTrace);
			}
		}

		public static bool SaveConfig()
		{
			try
			{
				// ȯ�漳�� XML File Load
				string configFile = new Settings().GetType().Assembly.Location + ".xml";
				
				XmlDocument doc = new XmlDocument();
				
				doc.Load(configFile);

				XmlNodeList nodes = doc.SelectNodes("//set");

				XmlElement elem = null;

				for (int i = 0; i < nodes.Count; i++)
				{
					elem = (XmlElement)nodes[i];
					string key = elem.GetAttribute("key");

					switch(key) 
					{
						case "LogLevel":
							if (intLogLevel.ToString() != elem.GetAttribute("value").ToString())
							{
								elem.SetAttribute("value", intLogLevel.ToString());
								Logger.StaticLog("CHANGE CONFIG: KEY=" + key + ", VALUE=" + intLogLevel.ToString());
							}
							break;

						case "AdminOnly":
							if (strAdminOnly != elem.GetAttribute("value").ToString())
							{
								elem.SetAttribute("value", strAdminOnly);
								Logger.StaticLog("CHANGE CONFIG: KEY=" + key + ", VALUE=" + strAdminOnly);
							}
							break;

						case "Language":
							if (strLanguage != elem.GetAttribute("value").ToString())
							{
								elem.SetAttribute("value", strLanguage);
								Logger.StaticLog("CHANGE CONFIG: KEY=" + key + ", VALUE=" + strLanguage);
							}
							break;
					}

				}
				doc.Save(configFile);
				return true;

			}
			catch (Exception ex)
			{
				Logger.StaticLog(ex.Message + " " + ex.StackTrace);
				return false;
			}
		}


	}
}

