using Microsoft.AspNetCore.Mvc;
using SUNMark.Classes;
using SUNMark.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Controllers
{
    public class PartialViewController : BaseController
    {
        DbConnection objDbConnection = new DbConnection();
        [HttpPost]
        public IActionResult MscMstPartialViewAdd(string name, string code, int position, string type, int id, string activeyn)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(code))
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[9];
                    sqlParameters[0] = new SqlParameter("@MscName", name);
                    sqlParameters[1] = new SqlParameter("@MscCode", code);
                    sqlParameters[2] = new SqlParameter("@MscType", type);
                    sqlParameters[3] = new SqlParameter("@MscPos", position);
                    sqlParameters[4] = new SqlParameter("@MscActYN", activeyn);
                    sqlParameters[5] = new SqlParameter("@MscVou", id);
                    sqlParameters[6] = new SqlParameter("@CmpVou", companyId);
                    sqlParameters[7] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[8] = new SqlParameter("@Flg", 1);

                    DataTable DtMscMst = objDbConnection.CallStoreProcedure("MscMst_Insert", sqlParameters);
                    if (DtMscMst != null && DtMscMst.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtMscMst.Rows[0][0].ToString());
                        if (type == "DSG")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/Designation/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Designation inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/Designation/Index" });
                                }
                            }
                        }
                        else if (type == "UNT")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/UnitMaster/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Unit inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/UnitMaster/Index" });
                                }
                            }
                        }
                        else if (type == "GRD")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/GradeMaster/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Grade inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/GradeMaster/Index" });
                                }
                            }
                        }
                        else if (type == "STA")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/StateMaster/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("State inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/StateMaster/Index" });
                                }
                            }
                        }
                        else if (type == "COU")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/CountryMaster/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Country inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/CountryMaster/Index" });
                                }
                            }
                        }
                        else if (type == "TRN")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/Transaction/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Transaction Type inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/Transaction/Index" });
                                }
                            }
                        }
                        else if (type == "FIN")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/FinishMaster/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Finish inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/FinishMaster/Index" });
                                }
                            }
                        }
                        else if (type == "MTY")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/MachineType/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Machine Type inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/MachineType/Index" });
                                }
                            }
                        }
                        else if (type == "SPE")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/Spacification/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Spacification inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/Spacification/Index" });
                                }
                            }
                        }
                        else if (type == "PTY")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/ProductType/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Product Type inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/ProductType/Index" });
                                }
                            }
                        }
                        else if (type == "LOT")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/LotType/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Lot Type inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/LotType/Index" });
                                }
                            }
                        }
                        else if (type == "ROL")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/UserRoll/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("User Roll inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/UserRoll/Index" });
                                }
                            }
                        }
                        else if (type == "PRC")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/ProcessType/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Process Type inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/ProcessType/Index" });
                                }
                            }
                        }
                        else if (type == "DEP")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/DepMst/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Department inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/DepMst/Index" });
                                }
                            }
                        }
                        else if (type == "DEL")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/DeliveryAt/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Delivery inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/DeliveryAt/Index" });
                                }
                            }
                        }
                    }

                }
                else
                {
                    return Json(new { result = false, Message = "Please Enter the Value" });
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { result = false });
        }
    }
}
