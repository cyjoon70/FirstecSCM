using System;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Threading;

namespace SystemBase
{
	/// <program>
	///   <date>2004-11-15</date><writer>����ȫ</writer>
	/// </program>
	/// <summary>
	/// Application ���� ����ϴ� Message�� �ٱ��� ó���� ���� �������� �����ϴ� CLASS�Դϴ�.
	/// </summary>
	public class LocalMessages
	{
		public LocalMessages()
		{
		}

		/* -----------------------------------------------
			* Attribute
		   ----------------------------------------------- */
		const string LOCALIZATION_RESOURCE_NAME = "Innobank.Common.ApplicationMessages";


		/* -----------------------------------------------
			* Method
		   ----------------------------------------------- */


		/// <program>
		///   <date>2004-11-15</date><writer>����ȫ</writer>
		/// </program>
		/// 
		/// <summary>
		/// �ٱ��� Resource �����κ��� MESSAGE�� �����´�.
		/// </summary>
		/// <param name="strResName">Resource Item Name</param>
		/// <returns></returns>
		public static string getMsgString(string strResName)
		{
			// Create a resource manager to retrieve resources.
			ResourceManager rm = new ResourceManager(LOCALIZATION_RESOURCE_NAME, 
				Assembly.GetExecutingAssembly());

			// Get the culture of the currently executing thread.
			CultureInfo ci = Thread.CurrentThread.CurrentCulture;
        
			// Retrieve the value of the string resource named 
			return rm.GetString(strResName, ci);

			
		}

	}


}
