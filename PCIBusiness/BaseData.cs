using System;
using System.Collections.Generic;
using System.Text;

namespace PCIBusiness
{
	public abstract class BaseData : StdDisposable
	{
		protected int    returnCode;
		protected string returnMessage;
		protected string returnData;
		protected string sql;
		protected DBConn dbConn;

      public override void Close()
      {
      // This will automatically be called by the base class destructor (StdDisposable).

		//	Clean up the derived class
			CleanUp();

		//	Clean up the base class
			Tools.CloseDB(ref dbConn);
			dbConn        = null;
			returnCode    = 0;
			returnMessage = "";
			returnData    = "";
		}

		public virtual void CleanUp()
		{
		//	This method can be overridden in the derived class to CLEAN UP stuff - not to initialize in the beginning
		//	Nothing here, so can completely override it in the derived class
		}

		public abstract void LoadData(DBConn dbConn);

		protected int ExecuteSQLUpdate(bool closeConnection=true,bool getReturnCodes=true)
		{
			if ( ExecuteSQL(null) == 0 && getReturnCodes )
			{
				returnCode    = dbConn.ColLong  ("ReturnCode");
				returnMessage = dbConn.ColString("ReturnMessage");
				returnData    = dbConn.ColString("ReturnData");
			}
			if (closeConnection)
				Tools.CloseDB(ref dbConn);
			return returnCode;
		}

		protected int ExecuteSQL (object[][] parms          =null,
		                          bool       eofIsError     =true,
		                          bool       closeConnection=true)
		{
			returnCode    = 0;
			returnMessage = "";
			returnData    = "";

			if ( ! Tools.OpenDB(ref dbConn) )
			{
				returnCode    = 1;
				returnMessage = "[BaseData.ExecuteSQL] Unable to connect to SQL database <1>";
			}
			else if ( ! dbConn.Execute(sql,closeConnection,parms) )
			{
				returnCode    = 2;
				returnMessage = "[BaseData.ExecuteSQL] SQL execution failed <2>";
			}
			else if ( dbConn.EOF && eofIsError )
			{
				returnCode    = 3;
				returnMessage = "[BaseData.ExecuteSQL] SQL successfully executed, but no data returned <3>";
			}
			return returnCode;
		}

		public virtual int RowNumber // Override if needed
		{
			set { }
			get { return 0; }
		}

		public string Message 
		{
			get { return Tools.NullToString(returnMessage); }
		}

		public string SQLData
		{
			get { return Tools.NullToString(returnData); }
		}
	}
}
