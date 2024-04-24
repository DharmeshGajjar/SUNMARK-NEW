using Microsoft.AspNetCore.Mvc.Rendering;
using SUNMark.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Classes
{
    public static class TransactionGridHelper
    {
        #region Private Methods

        static DbConnection ObjDBConnection = new DbConnection();
        #endregion

        #region Public Methods

        public static List<SelectListItem> GetLayoutList(string url)
        {
            try
            {
                SqlParameter[] sqlParametersNew = new SqlParameter[6];
                sqlParametersNew[0] = new SqlParameter("@GridTransactionId", "0");
                sqlParametersNew[1] = new SqlParameter("@Flg", 3);
                sqlParametersNew[2] = new SqlParameter("@skiprecord", "0");
                sqlParametersNew[3] = new SqlParameter("@pagesize", "0");
                sqlParametersNew[4] = new SqlParameter("@searchvalue", "0");
                sqlParametersNew[5] = new SqlParameter("@URL", url);

                DataTable dt = ObjDBConnection.CallStoreProcedure("GetTransactionGridDetails", sqlParametersNew);
                if (dt != null && dt.Rows.Count > 0)
                {
                    List<SelectListItem> data = new List<SelectListItem>();
                    foreach (DataRow item in dt.Rows)
                    {
                        data.Add(new SelectListItem
                        {
                            Text = Convert.ToString(item["LayoutName"]),
                            Value = Convert.ToString(item["TransactionGridId"])
                        });
                    }
                    return data;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return null;
        }

        public static string GetValue(TransactionGridAddUpdateDataModel dataList, string keyName)
        {
            if (dataList != null && dataList.Data != null && dataList.Data.Count > 0)
            {
                return dataList.Data.Where(x => x.Label == keyName).Select(x => x.Value).FirstOrDefault();
            }
            return null;
        }

        public static string GetDisplayValue(TransactionGridAddUpdateDataModel dataList, string keyName)
        {
            if (dataList != null && dataList.Data != null && dataList.Data.Count > 0)
            {
                return dataList.Data.Where(x => x.Label == keyName).Select(x => x.DisplayValue).FirstOrDefault();
            }
            return null;
        }
        #endregion
    }
}
