using System;
using System.Text;
using System.Collections.Generic;

namespace PCIBusiness
{
	public class MiscList : BaseList
	{
		public override BaseData NewItem()
		{
			return new MiscData();
		}

		public int ExecQuery(string sqlQuery,string dataClass="",byte loadRows=1)
		{
			sql = sqlQuery;

			if ( loadRows == 0 )
			{
				base.ExecuteSQL(null);
				return 0;
			}

			if ( string.IsNullOrWhiteSpace(dataClass) )
				return base.LoadDataFromSQL();

//	The above creates "MiscData" objects.
//	Below creates objects of type {dataClass} using Reflection.

			string err = "Invalid class name";
			try
			{
				Type classType  = (System.Reflection.Assembly.Load("PCIBusiness")).GetType("PCIBusiness."+dataClass);
				if ( classType != null )
					return base.LoadDataFromSQL(null,0,classType);
			}
			catch (Exception ex)
			{
				err = ex.Message;
			}
			Tools.LogException("MiscList.ExecQuery",err + " (DataClass=" + dataClass + ", SQL=" + sqlQuery + ")");
			return 0;
		}

		public void Add(MiscData dataX)
		{
			if ( objList == null )
				objList = new List<BaseData>();
			objList.Add(dataX);
		}
	}
}
