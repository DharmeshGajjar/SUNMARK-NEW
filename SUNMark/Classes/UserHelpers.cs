using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Classes
{
    public class UserHelpers
    {
        DbConnection ObjDBConnection = new DbConnection();

        public List<SelectListItem> GetUserMasterDropDown(int clientId, int isAdministrator = 0)
        {
            try
            {
                List<SelectListItem> userMasterModel = new List<SelectListItem>();
                SqlParameter[] parameters = new SqlParameter[1];
                if (isAdministrator == 1)
                {
                    clientId = 0;
                }
                parameters[0] = new SqlParameter("@Clientid", clientId);
                DataTable dtUser = ObjDBConnection.CallStoreProcedure("GetUserMasterList", parameters);
                if (dtUser != null && dtUser.Rows.Count > 0)
                {
                    if (dtUser != null && dtUser.Rows.Count > 0)
                    {
                        foreach (DataRow item in dtUser.Rows)
                        {
                            userMasterModel.Add(new SelectListItem
                            {
                                Text = item["UserId"] != null ? item["UserId"].ToString() : string.Empty,

                                Value = item["UserVou"] != null ? item["UserVou"].ToString() : string.Empty,

                            });
                        }
                    }
                }

                return userMasterModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<SelectListItem> GetUserRoleMasterDropdown(int CompanyId)
        {
            List<SelectListItem> UserRoleList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "ROL");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtRole = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtRole != null && DtRole.Rows.Count > 0)
                {
                    for (int i = 0; i < DtRole.Rows.Count; i++)
                    {
                        SelectListItem UserRoleItem = new SelectListItem();
                        UserRoleItem.Text = DtRole.Rows[i]["MScNm"].ToString();
                        UserRoleItem.Value = DtRole.Rows[i]["MscVou"].ToString();
                        UserRoleList.Add(UserRoleItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return UserRoleList;
        }

    }
}
