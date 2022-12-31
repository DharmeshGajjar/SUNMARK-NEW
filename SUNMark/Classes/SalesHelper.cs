using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SUNMark.Models;

namespace SUNMark.Classes
{
    public static class SalesHelper
    {
        static DbConnection objdbConnection = new DbConnection();
        static CommonHelper objCommon = new CommonHelper();
        static AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();

        public static List<object> ProcessExcel(DataTable dt, DataTable dtGrade, DataTable dtSpecification, string companyId, string userId, ref List<SelectListItem> notFoundItemList)
        {
            List<object> gridList = new List<object>();
            notFoundItemList = new List<SelectListItem>();
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        List<object> grid = new List<object>();
                        grid.Add(dt.Rows[i]["SPEC"].ToString());
                        grid.Add(dt.Rows[i]["Grade"].ToString());
                        grid.Add(dt.Rows[i]["ACTUAL THK"].ToString());
                        grid.Add(dt.Rows[i]["THICK"].ToString());
                        grid.Add(dt.Rows[i]["OD"].ToString());
                        grid.Add(dt.Rows[i]["FEET PER"].ToString());
                        grid.Add(dt.Rows[i]["QTY IN FEET"].ToString());
                        if (!string.IsNullOrWhiteSpace(dt.Rows[i]["QTY IN FEET"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["FEET PER"].ToString()))
                        {
                            grid.Add((Convert.ToInt32(dt.Rows[i]["QTY IN FEET"].ToString())  / Convert.ToInt32(dt.Rows[i]["FEET PER"].ToString())).ToString().Trim());
                        }
                        else
                        {
                            grid.Add("0");
                        }
                        var nbSCh = GetNBSCHDetails(dt.Rows[i]["THICK"].ToString(), dt.Rows[i]["OD"].ToString());
                        if (nbSCh != null && nbSCh.Length > 0)
                        {
                            grid.Add(nbSCh[0].Trim());
                            grid.Add(nbSCh[1].Trim());
                        }
                        else
                        {
                            grid.Add(string.Empty);
                            grid.Add(string.Empty);
                        }

                        DataRow[] drFind = dtGrade.Select("TEXT='" + dt.Rows[i]["Grade"].ToString() + "'");
                        DataRow[] drFind1 = dtSpecification.Select("TEXT='" + dt.Rows[i]["SPEC"].ToString() + "'");
                        if (drFind != null && drFind.Length > 0 && drFind1 != null && drFind1.Length > 0)
                        {
                            gridList.Add(grid);
                        }
                        else
                        {
                            string grade = string.Empty;
                            string specification = string.Empty;
                            if (drFind == null || drFind.Length <= 0)
                            {
                                grade = dt.Rows[i]["Grade"].ToString();
                                if (notFoundItemList != null && notFoundItemList.Count > 0 && notFoundItemList.Where(x => x.Text.ToString().Trim().Equals(grade.Trim())).Count() > 0)
                                {
                                    grade = string.Empty;
                                }
                            }

                            if (drFind1 == null || drFind1.Length <= 0)
                            {
                                specification = dt.Rows[i]["SPEC"].ToString();
                                if (notFoundItemList != null && notFoundItemList.Count > 0 && notFoundItemList.Where(x => x.Value.ToString().Trim().Equals(specification.Trim())).Count() > 0)
                                {
                                    specification = string.Empty;
                                }

                            }
                            if (!string.IsNullOrWhiteSpace(grade) || !string.IsNullOrWhiteSpace(specification))
                                notFoundItemList.Add(new SelectListItem
                                {
                                    Value = specification,
                                    Text = grade
                                });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return gridList;
        }


        public static List<object> InTranProcessExcel(DataTable dt, DataTable dtGrade, string companyId, string userId, ref List<SelectListItem> notFoundItemList)
        {
            List<object> gridList = new List<object>();
            notFoundItemList = new List<SelectListItem>();
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        List<object> grid = new List<object>();
                        grid.Add(dt.Rows[i]["NO."].ToString());
                        grid.Add(dt.Rows[i]["BILL NO"].ToString());
                        grid.Add(dt.Rows[i]["SUP COIL NO"].ToString());
                        grid.Add(dt.Rows[i]["HEAT NO"].ToString());
                        //DataRow[] drFind = dtGrade.Select("TEXT='" + dt.Rows[i]["GRADE"].ToString() + "'");
                        grid.Add(dt.Rows[i]["Grade"].ToString());
                        grid.Add(dt.Rows[i]["WIDTH"].ToString());
                        grid.Add(dt.Rows[i]["THICK"].ToString());
                        grid.Add(dt.Rows[i]["NET WEIGHT"].ToString());
                        grid.Add(dt.Rows[i]["PO WIDTH"].ToString());
                        grid.Add(dt.Rows[i]["PO THICK"].ToString());
                        grid.Add(dt.Rows[i]["SCNO"].ToString());

                        DataRow[] drFind = dtGrade.Select("TEXT='" + dt.Rows[i]["Grade"].ToString() + "'");
                        if (drFind != null && drFind.Length > 0)
                        {
                            gridList.Add(grid);
                        }
                        else
                        {
                            string grade = string.Empty;
                            if (drFind == null || drFind.Length <= 0)
                            {
                                grade = dt.Rows[i]["GRADE"].ToString();
                                if (notFoundItemList != null && notFoundItemList.Count > 0 && notFoundItemList.Where(x => x.Text.ToString().Trim().Equals(grade.Trim())).Count() > 0)
                                {
                                    grade = string.Empty;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return gridList;
        }


        public static string[] GetNBSCHDetails(string thickpipe, string odpipe)
        {
            string[] details = new string[2];
            try
            {
                if (!string.IsNullOrWhiteSpace(thickpipe) && !string.IsNullOrWhiteSpace(odpipe))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@thickpipe", thickpipe);
                    sqlParameters[1] = new SqlParameter("@odpipe", odpipe);
                    DataTable dtNBSCH = objdbConnection.CallStoreProcedure("GetNBSCHDetails", sqlParameters);
                    if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
                    {
                        string OrdNB = dtNBSCH.Rows[0]["OrdNB"].ToString();
                        string OrdSch = dtNBSCH.Rows[0]["OrdSch"].ToString();
                        details[0] = OrdNB;
                        details[1] = OrdSch;
                        return details;
                    } 
                }
                else
                {
                    details[0] = "";
                    details[1] = "";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public static bool AddGradeSpecification(string value, string type, string companyId, string userId)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[9];
                sqlParameters[0] = new SqlParameter("@MscName", value);
                sqlParameters[1] = new SqlParameter("@MscCode", RemoveSpecialCharacters(value));
                sqlParameters[2] = new SqlParameter("@MscType", type);
                sqlParameters[3] = new SqlParameter("@MscPos", "0");
                sqlParameters[4] = new SqlParameter("@MscActYN", "Yes");
                sqlParameters[5] = new SqlParameter("@MscVou", "0");
                sqlParameters[6] = new SqlParameter("@CmpVou", companyId);
                sqlParameters[7] = new SqlParameter("@UsrVou", userId);
                sqlParameters[8] = new SqlParameter("@Flg", 1);

                DataTable DtMscMst = objdbConnection.CallStoreProcedure("MscMst_Insert", sqlParameters);
            }
            catch (Exception ex)
            {

                throw;
            }
            return true;
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
