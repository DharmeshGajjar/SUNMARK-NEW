using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Classes
{
    public class AccountGroupHelpers
    {
        public DbConnection ObjDBConnection = new DbConnection();
         

        public List<SelectListItem> GetOpositeGroupDropdown(int companyId)
        {
            List<SelectListItem> OpositeGroup = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@AgrVOU", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                sqlParameters[4] = new SqlParameter("@CmpVou", companyId);
                DataTable DtOpositeGroup = ObjDBConnection.CallStoreProcedure("GetAccountGroupDetails", sqlParameters);
                if (DtOpositeGroup != null && DtOpositeGroup.Rows.Count > 0)
                {
                    for (int i = 0; i < DtOpositeGroup.Rows.Count; i++)
                    {
                        SelectListItem OpositeGroupItem = new SelectListItem();
                        OpositeGroupItem.Text = DtOpositeGroup.Rows[i]["AgrName"].ToString();
                        OpositeGroupItem.Value = DtOpositeGroup.Rows[i]["AgrVou"].ToString();
                        OpositeGroup.Add(OpositeGroupItem);
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }
            return OpositeGroup;
        }
        public List<SelectListItem> GetGroupType()
        {
            List<SelectListItem> GroupType = new List<SelectListItem>();
            try
            {
                GroupType.Add(new SelectListItem
                {
                    Text = "Tranding Account",
                    Value = "1"
                });
                GroupType.Add(new SelectListItem
                {
                    Text = "Profit & Loss",
                    Value = "2"
                });
                GroupType.Add(new SelectListItem
                {
                    Text = "Balance Sheet",
                    Value = "3"
                });
            }
            catch (Exception ex)
            {
                throw;
            }
            return GroupType;
        }

        public List<SelectListItem> GetSummaryYesNo()
        {
            List<SelectListItem> SummaryYesNo = new List<SelectListItem>();
            try
            {
                SummaryYesNo.Add(new SelectListItem
                {
                    Text = "No",
                    Value = "1"
                });
                SummaryYesNo.Add(new SelectListItem
                {
                    Text = "Yes",
                    Value = "2"
                });
            }
            catch(Exception ex)
            {
                throw;
            }
            return SummaryYesNo;
        }

        public List<SelectListItem> GetCategory()
        {
            List<SelectListItem> Category = new List<SelectListItem>();
            try
            {
                Category.Add(new SelectListItem
                {
                    Text = "Cash/Bank",
                    Value = "1"
                });
                Category.Add(new SelectListItem
                {
                    Text = "Receivable",
                    Value = "2"
                });
                Category.Add(new SelectListItem
                {
                    Text = "Payable",
                    Value = "3"
                });
                Category.Add(new SelectListItem
                {
                    Text = "Others",
                    Value = "4"
                });
            }
            catch (Exception ex)
            {
                throw;
            }
            return Category;
        }

        public List<SelectListItem> GetAccountCrDr()
        {
            List<SelectListItem> AccountCrDr = new List<SelectListItem>();
            try
            {
                AccountCrDr.Add(new SelectListItem
                {
                    Text = "Cr",
                    Value = "1"
                });
                AccountCrDr.Add(new SelectListItem
                {
                    Text = "Dr",
                    Value = "2"
                });
            }
            catch(Exception ex)
            {
                throw;
            }
            return AccountCrDr;
        }
    }
}
