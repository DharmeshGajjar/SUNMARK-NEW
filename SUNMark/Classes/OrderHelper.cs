using SUNMark.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Classes
{
    public class OrderHelper
    {
        DbConnection ObjDBConnection = new DbConnection();

        public int InsertOrderMst(PurchaseOrderModel purchaseOrderModel,int id,int userId)
        {
            int masterId = 0;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[13];
                sqlParameters[0] = new SqlParameter("@OrmVchTyp", purchaseOrderModel.VchType);
                sqlParameters[1] = new SqlParameter("@OrmVchVou", purchaseOrderModel.OrmVchVou);
                sqlParameters[2] = new SqlParameter("@OrmVNo", purchaseOrderModel.OrmVNo);
                sqlParameters[3] = new SqlParameter("@OrmDt", purchaseOrderModel.OrmDt);
                sqlParameters[4] = new SqlParameter("@OrmRefNo", purchaseOrderModel.OrmRefNo);
                sqlParameters[5] = new SqlParameter("@OrmAccVou", purchaseOrderModel.OrmAccVou);
                sqlParameters[6] = new SqlParameter("@OrmDueDt", purchaseOrderModel.OrmDueDt);
                sqlParameters[7] = new SqlParameter("@OrmPtyVou", purchaseOrderModel.OrmPtyVou);
                sqlParameters[8] = new SqlParameter("@OrmPtyNm", purchaseOrderModel.PtyName);
                sqlParameters[9] = new SqlParameter("@OrmRem", purchaseOrderModel.OrmRem);
                sqlParameters[10] = new SqlParameter("@OrmVou", id);
                sqlParameters[11] = new SqlParameter("@UsrVou", userId);
                sqlParameters[12] = new SqlParameter("@FLG", 1);
                DataTable DtOrdMst = ObjDBConnection.CallStoreProcedure("PurOrderMst_Insert", sqlParameters);
                if (DtOrdMst != null && DtOrdMst.Rows.Count > 0)
                {
                    masterId = DbConnection.ParseInt32(DtOrdMst.Rows[0][0].ToString());
                }
            }
            catch (Exception ex) 
            {
                throw;
            }
            return masterId;
        }

    }
}
