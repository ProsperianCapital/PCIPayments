﻿using System;
using System.IO;
using System.Xml;
using System.Globalization;

namespace PCIBusiness
{
	public static class Tools
	{
//		Date formats:
//		 1	 dd/mm/yyyy                (31/12/2009)
//		 2	 yyyy/mm/dd                (2006/04/22)
//		 3	 DD 3-char Month Abbr YYYY (22 Sep 2008)
//		 4	 DD full Month Name YYYY   (19 August 2003)
//		 5	 yyyymmdd
//		 6  DayName DD MonthName YYYY (Saturday 13 October 2010)
//		 7  YYYY-MM-DD
//		 8	 Day DD 3-char Month Abbr YYYY (Fri 22 Sep 2008)
//		19	 yyyy/mm/dd (for SQL)

//		Time formats:
//		 1  HH:mm:ss                  (17:03:54)
//		 2  HHmmss                    (170354)
//		 3  HH:mm                     (17:03)
//		 4  at HH:mm                  (at 17:03)
//		11  HH:mm:ss                  Hard-code to 00:00:00
//		12  HH:mm:ss                  Hard-code to 23:59:59

	public static string DecimalToString(decimal theValue,byte decimalPlaces=2)
	{
		return System.Math.Round(theValue,decimalPlaces).ToString();
	}

	public static string IntToDecimal(int theValue,byte theFormat)
	{
		string tmp = theValue.ToString().Trim();

		if ( theFormat == 1 )
			return tmp + ".00";
		else if ( theFormat == 2 && tmp.Length == 0 )
			return "0.00";
		else if ( theFormat == 2 && tmp.Length == 1 )
			return "0.0" + tmp;
		else if ( theFormat == 2 && tmp.Length == 2 )
			return "0." + tmp;
		else if ( theFormat == 2 )
			return tmp.Substring(0,tmp.Length-2) + "." + tmp.Substring(tmp.Length-2);
		
		return tmp;
	}

	public static string ObjectToString(Object theValue)
	{
		if ( theValue == null ) return "";
		return theValue.ToString().Trim().Replace("<","").Replace(">","");
	}

	public static string NullToString(string theValue)
	{
		if ( string.IsNullOrWhiteSpace(theValue) ) return "";
		return theValue.Trim();
	}

	public static string CompressedString(string theValue)
	{
		if ( string.IsNullOrWhiteSpace(theValue) ) return "";
        theValue = theValue.Trim();
        while ( theValue.Contains("  ") )
            theValue = theValue.Replace("  "," ");
		return theValue;
	}

	public static int StringToInt(string theValue)
	{
		try
		{
			int ret = System.Convert.ToInt32(theValue);
			return ret;
		}
		catch
		{ }
		return 0;
	}

	public static DateTime StringToDate(string dd,string mm,string yy)
	{
		DateTime ret = Constants.C_NULLDATE();
		try
		{
			ret = new DateTime(System.Convert.ToInt32(yy), System.Convert.ToInt32(mm), System.Convert.ToInt32(dd));
		}
		catch
		{
			ret = Constants.C_NULLDATE();
		}
		return ret;
	}

	public static DateTime StringToDate(string theDate,byte dateFormat)
		{
			DateTime ret = Constants.C_NULLDATE();
			string   dd  = "";
			string   mm  = "";
			string   yy  = "";
			string   hh  = "";
			string   mi  = "";
			string   ss  = "";

			theDate = theDate.Trim();

			if ( dateFormat == 1 && theDate.Length == 10 )
			{
				dd = theDate.Substring(0,2);	
				mm = theDate.Substring(3,2);	
				yy = theDate.Substring(6,4);	
			}
			else if ( dateFormat == 2 && theDate.Length == 10 )
			{
				dd = theDate.Substring(8,2);	
				mm = theDate.Substring(5,2);	
				yy = theDate.Substring(0,4);	
			}
			else if ( dateFormat == 13 && ( theDate.Length == 16 || theDate.Length == 19 ) )
			{
				dd = theDate.Substring(0,2);	
				mm = theDate.Substring(3,2);	
				yy = theDate.Substring(6,4);	
				hh = theDate.Substring(11,2);	
				mi = theDate.Substring(14,2);
				if ( theDate.Length == 19 )
					ss = theDate.Substring(17,2);
			}

			try
			{
				if ( ss.Length == 2 )
					ret = new DateTime(System.Convert.ToInt32(yy), System.Convert.ToInt32(mm), System.Convert.ToInt32(dd), System.Convert.ToInt32(hh), System.Convert.ToInt32(mi), System.Convert.ToInt32(ss));
				else if ( hh.Length == 2 )
					ret = new DateTime(System.Convert.ToInt32(yy), System.Convert.ToInt32(mm), System.Convert.ToInt32(dd), System.Convert.ToInt32(hh), System.Convert.ToInt32(mi), 0);
				else
					ret = new DateTime(System.Convert.ToInt32(yy), System.Convert.ToInt32(mm), System.Convert.ToInt32(dd));
			}
			catch
			{
				ret = Constants.C_NULLDATE();
			}
			return ret;
		}

		public static string DateToSQL(DateTime whatDate,byte timeFormat,bool quotes=true)
		{
			return DateToString(whatDate,19,timeFormat,quotes);
		}

		public static string DateToString(DateTime whatDate,byte dateFormat,byte timeFormat=0,bool quotes=false)
		{
			string theDate = "" ;
			string theTime = "" ;

			if ( whatDate.CompareTo(Constants.C_NULLDATE()) <= 0 && dateFormat == 19 ) // for SQL
				return "NULL";

			if ( whatDate.CompareTo(Constants.C_NULLDATE()) <= 0 )
				return "";

			if ( dateFormat == 1 )        // DD/MM/YYYY
				theDate = whatDate.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture);
			else if  ( dateFormat ==  2 ) // YYYY/MM/DD
				theDate = whatDate.ToString("yyyy/MM/dd",CultureInfo.InvariantCulture);
			else if  ( dateFormat ==  3 ) // DD MonthAbbr YYYY
				theDate = whatDate.ToString("dd MMM yyyy");
			else if  ( dateFormat ==  4 ) // DD MonthName YYYY
				theDate = whatDate.ToString("dd MMMMMMMMM yyyy");
			else if  ( dateFormat ==  5 ) // YYYYMMDD
				theDate = whatDate.ToString("yyyyMMdd");
			else if  ( dateFormat ==  6 ) // Saturday 13 October 2010
				theDate = whatDate.ToString("ddddddddd dd MMMMMMMMM yyyy");
			else if  ( dateFormat ==  7 ) // 2010-07-25
				theDate = whatDate.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
			else if  ( dateFormat ==  8 ) // Sat 13 Oct 2010
				theDate = whatDate.ToString("ddd dd MMM yyyy");
			else if  ( dateFormat == 19 ) // YYYY/MM/DD (for SQL)
				theDate = whatDate.ToString("yyyy/MM/dd",CultureInfo.InvariantCulture);

			if ( timeFormat == 1 && ( dateFormat == 6 || dateFormat == 8 ) )
				theTime = "at " + whatDate.ToString("HH:mm:ss",CultureInfo.InvariantCulture);
			else if ( timeFormat == 1 )  // HH:MM:SS
				theTime = whatDate.ToString("HH:mm:ss",CultureInfo.InvariantCulture);
			else if ( timeFormat == 2 )  // HHMMSS
				theTime = whatDate.ToString("HHmmss");
			else if ( timeFormat == 3 )  // HH:MM
				theTime = whatDate.ToString("HH:mm",CultureInfo.InvariantCulture);
			else if ( timeFormat == 4 )  // at HH:MM
				theTime = "at " + whatDate.ToString("HH:mm",CultureInfo.InvariantCulture);
			else if ( timeFormat == 11 )  // 00:00:00
				theTime = "00:00:00";
			else if ( timeFormat == 12 )  // 23:59:59
				theTime = "23:59:59";

			return ( quotes ? "'" : "" ) + (theDate + " " + theTime).Trim() + ( quotes ? "'" : "" );
		}

		public static bool OpenDB(ref DBConn dbConn)
		{
			if ( dbConn == null )
				dbConn = new DBConn();
			return dbConn.Open();
		}

		public static void CloseDB(ref DBConn dbConn)
		{
         if ( dbConn != null )
         {
            try
            {
               dbConn.Close();
               dbConn.Dispose();
            }
            catch { }
         }
         dbConn = null;
		}

		public static string MixedCase(string str)
		{
			int j;
			int k;
			if ( str == null )
				return "";

			try
			{
				str = str.Trim().ToLower();
				if ( str.Length == 0 )
					return "";
				while ( str.IndexOf("  ") >= 0 )
					str = str.Replace("  "," ");
				str = str.Substring(0,1).ToUpper() + str.Substring(1);
				j   = 0;
				k   = str.IndexOf(" ",0);
				while ( k > j )
				{
					str = str.Substring(0,k+1) + str.Substring(k+1,1).ToUpper() + str.Substring(k+2);
					j   = k + 1;
					k   = str.IndexOf(" ",j);
				}
			}
			catch (Exception ex)
			{
				LogException("Tools.MixedCase","str=" + str,ex);
			}
			return str;
		}

		public static string XMLNode(XmlDocument xmlDoc,string xmlTag)
		{
			try
			{
				string ret = xmlDoc.SelectSingleNode("//"+xmlTag).InnerText;
				return ret.Trim();
			}
			catch
			{ }
			return "";
		}

		public static string XMLSafe(string str)
		{
      // Converts a string to safe format for XML:
      // (1) Removes leading and trailing spaces
      // (2) Replaces illegal chars, which are & < > " '
		//	Note that both \ and / are allowed in XML
    
			if ( string.IsNullOrWhiteSpace(str) )
				return "";
			str = str.Trim();
         str = str.Replace("'","`");
         str = str.Replace("\"","`");
         str = str.Replace("<","[");
         str = str.Replace(">","]");
         str = str.Replace("&"," and ");
         str = str.Replace("  and "," and ");
         str = str.Replace(" and  "," and ");
         return str;
		}

		public static string XMLString(string str)
		{
      // Converts an XML string for use in an SQL statement:
      // (1) Removes leading and trailing spaces
      // (2) Replaces each single quote with a \"
      // (3) Puts a single quote at front and back
    
			if ( string.IsNullOrWhiteSpace(str) )
				return "''";
			str = str.Trim();
//       str = "'" + str.Replace("'","\'") + "'" ;
         str = "'" + str.Replace("'","\"") + "'" ;
         return str ;
		}

		public static string DBString(string str,int maxLength=0)
		{
      // Converts a string for use in an SQL statement:
      // (1) Removes leading and trailing spaces
      // (2) Replaces each single quote with TWO single quotes
      // (3) Removes "<" and ">" characters (no embedded HTML)
      // (4) Puts a single quote at front and back
    
			if ( string.IsNullOrWhiteSpace(str) )
				return "''";

			str = str.Trim();
			if ( str.ToUpper() == "NULL" ) // Leave 'NULL' unchanged, no quotes
				return "NULL";
			str = str.Replace("<","").Replace(">","");

			if ( maxLength > 0 && maxLength < str.Length )
				str = str.Substring(0,maxLength);

         str = "'" + str.Replace("'","''") + "'";
         return str ;
		}

		public static int TimeDifference(string time1,string time2)
		{
			try
			{
				DateTime d1 = System.Convert.ToDateTime("1999/12/31 "+time1);
				DateTime d2 = System.Convert.ToDateTime("1999/12/31 "+time2);
				TimeSpan mn = d2 - d1;
				return (int) mn.TotalMinutes;
			}
			catch { }
			return 0;
		}

		public static int DateDifference(DateTime dt1,DateTime dt2)
		{
			try
			{
				TimeSpan mn = dt2 - dt1;
				return (int) mn.Days;
			}
			catch { }
			return 0;
		}

		public static string TimeAdd(string theTime,int minutes)
		{
			try
			{
				DateTime dt = System.Convert.ToDateTime("1999/12/31 " + theTime);
				TimeSpan mn = new TimeSpan(0,minutes,0);
				dt = dt.Add(mn);
				return Tools.DateToString(dt,0,3,false);
			}
			catch { }
			return ""; 
		}

		private static void LogWrite(string settingName, string component, string msg)
		{
		// Use this routine to log internal errors ...
			int          k;
			string       fName   = "";
			FileStream   fHandle = null;
			StreamWriter fOut    = null;

			try
			{
				fName = Tools.ConfigValue(settingName);
				if ( fName == null )
					fName = "";
				else
					fName = fName.Trim();
				if ( fName.Length < 1 )
					fName = "C:\\Temp\\PCILogFile.txt";

				k = fName.LastIndexOf(".");
				if ( k < 1 )
				{
					fName = fName + ".txt";
					k     = fName.LastIndexOf(".");
				}
				fName = fName.Substring(0,k) + "-" + DateToString(System.DateTime.Now,7) + fName.Substring(k);

				if ( File.Exists(fName) )
					fHandle = File.Open(fName, FileMode.Append);
				else
					fHandle = File.Open(fName, FileMode.Create);
				fOut = new StreamWriter(fHandle,System.Text.Encoding.Default);
				fOut.WriteLine( "[v" + SystemDetails.AppVersion + ", " + Tools.DateToString(System.DateTime.Now,1,1,false) + "] " + component + " : " + msg);
			}
			catch (Exception ex)
			{
				if ( settingName.Length > 0 ) // To prevent recursion ...
					LogWrite("","Tools.LogWrite","fName = '" + fName + "', Error = " + ex.Message);
			}
			finally
			{
				if ( fOut != null )
					fOut.Close();
				fOut    = null;
			 	fHandle = null;
			}
		}

		public static void LogException(string component, string msg, Exception ex=null)
		{
		// Use this routine to log error messages
			if ( ex == null )
				msg = "Non-exception error" + ( msg.Length == 0 ? "" : " ("+msg+")" );
			else
				msg = ex.Message + ( msg.Length == 0 ? "" : " ("+msg+")" ) + " : [" + ex.ToString() + "]";
			LogWrite("LogFileErrors",component,msg);
		}

		public static void LogInfo(string component, string msg, byte severity=10)
		{
		// Use this routine to log debugging/info messages
		//	To decide which messages to write, adjust the severity below
		//	Calling routines must supply a severity between 0-255 (default 10)
			if ( severity > 0 )
				LogWrite("LogFileInfo",component,msg);
		}

		public static bool CheckEMail(string email)
		{
		//	Simple, quick check ...
			email = NullToString(email);
			if ( email.Length < 6 || email.Contains("(") || email.Contains(")") || email.Contains("<") || email.Contains(">") || email.Contains(" ") )
				return false;
			return email.Contains("@") && email.Contains(".");
		}

		/*
		public static byte CheckCreditCardNumber(ref string ccNumber,byte cardType)
		{
		//	VISA numbers             start with 4
		//	MasterCard numbers       start with 2 or 5
		// American Express numbers start with 34 or 37
		// Diner's Club numbers     start with various

			ccNumber = ccNumber.Trim().Replace(" ","");

			if ( cardType == (byte)Constants.CreditCardType.Visa )
			{
				if ( ccNumber.Length < 13 || ccNumber.Length > 19 )
					return 3;
				if ( ccNumber.Substring(0,1) != "4" )
					return 6;
			}

			else if ( cardType == (byte)Constants.CreditCardType.MasterCard )
			{
				if ( ccNumber.Length != 16 )
					return 33;
				if ( ccNumber.Substring(0,1) != "2" && ccNumber.Substring(0,1) != "5" )
					return 36;
			}

			else if ( cardType == (byte)Constants.CreditCardType.AmericanExpress )
			{
				if ( ccNumber.Length != 15 )
					return 63;
				if ( ccNumber.Substring(0,2) != "34" && ccNumber.Substring(0,2) != "37" )
					return 66;
			}

			else if ( cardType == (byte)Constants.CreditCardType.DinersClub )
			{
				if ( ccNumber.Length < 14 || ccNumber.Length > 16 )
					return 93;
			}

			else if ( ccNumber.Length < 12 || ccNumber.Length > 20 )
				return 23;

			try
			{
				ulong ccNo = System.Convert.ToUInt64(ccNumber);
			}
			catch
			{
				return 99;
			}

			return 0;
		}
		*/

		public static DateTime IDNumberToDate(string idNumber)
		{
			try
			{
				string year    = System.DateTime.Now.Year.ToString();
				int    century = System.Convert.ToInt32(year.Substring(0,2));
	
				if ( idNumber.Substring(0,2).CompareTo(year.Substring(2,2)) > 0 ) 
					century = century - 1;

				return Tools.StringToDate(idNumber.Substring(4,2),idNumber.Substring(2,2),century.ToString()+idNumber.Substring(0,2));
			}
			catch
			{ }
			return Constants.C_NULLDATE();
		}

		public static byte CheckDate(string dd,string mm,string yy,ref DateTime theDate)
		{
			try
			{
				if ( dd.Length == 0 && mm.Length == 0 && yy.Length == 0 )
					return 244;
				theDate = new DateTime(Convert.ToInt32(yy),Convert.ToInt32(mm),Convert.ToInt32(dd));
				return 0;
			}
			catch
			{ }
			return 255;
		}

		public static int CalcAge(DateTime dateOfBirth,DateTime theDate)
		{
			if ( theDate == null || theDate <= Constants.C_NULLDATE() )
				theDate = System.DateTime.Now;

			if ( dateOfBirth <= Constants.C_NULLDATE() || dateOfBirth >= theDate )
				return 0;

			int diff = theDate.Year - dateOfBirth.Year;
			 
			if ( dateOfBirth.Month < theDate.Month )
				return diff;
			if ( dateOfBirth.Month > theDate.Month )
				return diff - 1;
			if ( dateOfBirth.Day > theDate.Day )
				return diff - 1;
			return diff;
		}

		public static string ConfigValue(string configName)
		{
			try
			{
				string ret = System.Configuration.ConfigurationManager.AppSettings[configName.Trim()].ToString();
				return ret;
			}
			catch
			{ }
			return "";
		}

		public static int DeleteFiles(string fileSpec,short ageDays=0,short beforeHour=0,short afterHour=0)
		{
			int deleted = 0;

			try
			{
				if ( beforeHour > 0 && beforeHour < 24 && System.DateTime.Now.Hour >= beforeHour )
					return -5;
				if ( afterHour  > 0 && afterHour  < 24 && System.DateTime.Now.Hour <  afterHour )
					return -10;

				string folder = Tools.ConfigValue("ReportFolder");

				if ( ! Directory.Exists(folder) )
					return -15;
				string[] files = Directory.GetFiles(folder,fileSpec);
				if ( files.Length < 1 )
					return -20;

				if ( ageDays < 1 )
					ageDays = 7;

				foreach ( string fileName in files )
					if ( File.GetLastWriteTime(fileName).AddDays(ageDays) < DateTime.Now ) // More "x" days old
					{
						try
						{
							File.Delete(fileName);
							deleted++;
						}
						catch { }
					}
			}
			catch (Exception ex)
			{
				Tools.LogException("Tool.DeleteFiles","",ex);
			}
			return deleted;
		}

		public static string CreateFile(int userCode,ref StreamWriter fileStream,string fileExtension="csv")
		{
			FileStream fileHandle;
			string     fileName      = "";
			string     fileNameFixed = "";

			if ( NullToString(fileExtension).Length < 1 )
				fileExtension = ".csv";
			else if ( ! fileExtension.StartsWith(".") )
				fileExtension = "." + fileExtension;

			try
			{
				fileStream    = null;
				fileNameFixed = Tools.FixFolderName(ConfigValue("ReportFolder"));
				fileNameFixed = fileNameFixed + userCode.ToString() + "-" + DateToString(DateTime.Now,5) + "-";

				for ( int k = 1 ; k < 999999 ; k++ )
				{
					fileName = fileNameFixed + k.ToString().PadLeft(6,'0') + fileExtension;
					if ( ! File.Exists(fileName) )
						break;
				}
				fileHandle = File.Open(fileName, FileMode.Create);
				fileStream = new StreamWriter(fileHandle,System.Text.Encoding.Default);
				return fileName;
			}
			catch (Exception ex)
			{
				LogException("Tools.CreateFile","UserCode=" + userCode.ToString(),ex);
			}
			return "";
		}

		public static string FixFolderName(string folder)
		{
			if ( folder == null )
				return "";
			folder = folder.Trim();
			if ( folder.Length < 1 )
				return "";
			return ( folder.EndsWith("\\") ? folder : folder + "\\" );
		}

		public static string ConciseName(string theName)
		{
			string ret;
			string ch;
			int    k;

			if ( theName == null )
				return "";

			theName = theName.Trim().Replace(" ","").ToUpper();
			ret     = "";

			for ( k = 0 ; k < theName.Length ; k++ )
			{
				ch = theName.Substring(k,1);
				if ( ch.CompareTo("A") >= 0 && ch.CompareTo("Z") <= 0 )
					ret = ret + ch;
				else if ( ch.CompareTo("0") >= 0 && ch.CompareTo("9") <= 0 )
					ret = ret + ch;
			}
			return ret;
		}

//	Generic "Valid" stuff

		public static string SplitString(string str,short lineLength=100)
		{
			int    k;
			string ret = "";
			str        = NullToString(str).Replace("  "," ");
			if ( lineLength < 10 )
				lineLength = 100;

			while ( str.Length > lineLength )
			{
				k   = (str.Substring(lineLength)).IndexOf(" ");
				if ( k < 0 )
					break;
				k   = k + lineLength;
				ret = ret + str.Substring(0,k) + Constants.C_HTMLBREAK();
				str = str.Substring(k+1).Trim();
			}
			return ret + str;
		}
	}
}