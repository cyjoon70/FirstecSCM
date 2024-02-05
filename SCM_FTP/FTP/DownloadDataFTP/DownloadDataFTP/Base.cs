using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DownloadDataFTP
{
    public class Base
    {
        public static string is_key = "4E0174A435C94A17B29DC4C551FE1066ACDF89B7598B42D99C4A2096EA48E925";
        public static string strFTPAddress = "";
        public static string strUsername = "";
        public static string strPassword = "";
        public static string strFTPPath = "";

        //public static string gstrDbConn = @"server=10.10.10.80\dbserver;uid=e2max;pwd=kyg007!;database=kona";	//DB 연결정보

        public static string strDBServer = "";
        public static string strDBLoginID = "";
        public static string strDBPass = "";
        public static string strDBName = "";
        public static string AppFolder = "";

        //#region 암호화(S) Encode
        //public static string Encode(string strEncode)
        //{

        //    string strConEncode = "" ;
        //    int i;

        //    if (is_key == "BAD")
        //    {
        //        strConEncode = strEncode;
        //    }
        //    else
        //    {
        //        for (i = 0; i < strEncode.Length; i++)
        //        {
        //            strConEncode = strConEncode + Convert.ToChar(Convert.ToInt32(Convert.ToChar(strEncode.Substring(i, 1))) + Convert.ToInt32(Convert.ToChar(is_key.Substring(i, 1))) - Convert.ToInt32(Convert.ToChar(is_key.Substring(i + 1, 1))));

        //        }
        //    }

        //    strConEncode = strConEncode + Convert.ToChar(Convert.ToInt32(Convert.ToChar(is_key.Substring(1, 1))));

        //    return strConEncode;


        //}
        //#endregion

        //#region 복호화(S) Decode
        //public static string Decode(string strEncode)
        //{
        //    string strConEncode = "";

        //    int i;

        //    if (is_key == "BAD")
        //    {
        //        strConEncode = strEncode;
        //    }
        //    else
        //    {
        //        for (i = 0; i < strEncode.Length; i++)
        //        {
        //            strConEncode = strConEncode + Convert.ToChar(Convert.ToInt32(Convert.ToChar(strEncode.Substring(i, 1))) + Convert.ToInt32(Convert.ToChar(is_key.Substring(i + 1, 1))) - Convert.ToInt32(Convert.ToChar(is_key.Substring(i, 1))));
        //        }
        //    }

        //    strConEncode = strConEncode.Substring(0, strConEncode.Length - 1);

        //    return strConEncode;

        //}
        //#endregion

        #region 암호화(S) Encode
        public static string Encode(string strEncode)
        {
            int i = 0;
            int lens = 0;
            int conv = 0;
            string temp = "";
            string temps;

            temp = strEncode;
            lens = strEncode.Length;
            temp = "";

            for (i = 1; i < lens + 1; i++)
            {
                conv = i % 3;

                temps = strEncode.Substring(lens - i, 1);

                temp = temp + Convert.ToChar(Convert.ToInt32(Convert.ToChar(temps)) + conv);
            }

            return temp;
        }
        #endregion

        #region 복호화(S) Decode
        public static string Decode(string strEncode)
        {
            int i = 0;
            int lens = 0;
            int conv = 0;
            string temp = "";
            string temps;

            temp = strEncode;
            lens = strEncode.Length;
            temp = "";

            for (i = lens; i > 0; i--)
            {
                conv = i % 3;

                temps = strEncode.Substring(i - 1, 1);

                temp = temp + Convert.ToChar(Convert.ToInt32(Convert.ToChar(temps)) - conv);
            }

            return temp;
        }
        #endregion

        // INI 값 읽기 
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

    }
}
