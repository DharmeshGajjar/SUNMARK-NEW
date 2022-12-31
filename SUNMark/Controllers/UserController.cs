using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SUNMark.Classes;
using SUNMark.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace SUNMark.Controllers
{
    public class UserController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        ProductHelpers objproductHelpers = new ProductHelpers();
        private readonly IWebHostEnvironment hostingEnvironment;

        public UserController(IWebHostEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
        }

        public IActionResult Index(long id)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                long companyid = GetIntSession("CompanyId");
                long ClientId = GetIntSession("ClientId");
                int isadministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                UserMasterModel userMasterModel = new UserMasterModel();
                if (id > 0)
                {
                    userMasterModel.UserVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@UserVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 3);
                    sqlParameters[2] = new SqlParameter("@ClientId", ClientId);

                    DataTable dtUserMst = ObjDBConnection.CallStoreProcedure("UserMasterInsert", sqlParameters);
                    if (dtUserMst != null && dtUserMst.Rows.Count > 0)
                    {
                        userMasterModel.UserDate = Convert.ToDateTime(dtUserMst.Rows[0]["UserUseDt"].ToString());
                        userMasterModel.UserEmpVou  = Convert.ToInt32(dtUserMst.Rows[0]["UserEmpVou"].ToString());
                        userMasterModel.EmpName = dtUserMst.Rows[0]["UserEmpNm"].ToString();
                        userMasterModel.UserId = dtUserMst.Rows[0]["UserId"].ToString();
                        userMasterModel.UserCd = dtUserMst.Rows[0]["UserCd"].ToString();
                        userMasterModel.UserPass = dtUserMst.Rows[0]["UserPass"].ToString();
                        userMasterModel.UserMobNo = dtUserMst.Rows[0]["UserMobNo"].ToString();
                        userMasterModel.UserEmail = dtUserMst.Rows[0]["UserEmail"].ToString();
                        userMasterModel.ProfilePicture = dtUserMst.Rows[0]["UserPhoto"].ToString();
                        userMasterModel.UserRolVou = Convert.ToInt32(dtUserMst.Rows[0]["UserRolVou"].ToString());
                        userMasterModel.UserVou = DbConnection.ParseInt32(dtUserMst.Rows[0]["UserVou"].ToString());
                        if (dtUserMst.Rows[0]["UserActYN"].ToString().TrimEnd() == "No")
                        {
                            userMasterModel.UserActYNVou = 1;
                        }
                        else
                        {
                            userMasterModel.UserActYNVou = 0;
                        }
                        userMasterModel.Active = dtUserMst.Rows[0]["UserActYN"].ToString();
                        //userMasterModel.ClientId = DbConnection.ParseInt32(dtUserMst.Rows[0]["ClientId"].ToString());
                    }
                }
                //userMasterModel.GetClientLIst = DbConnection.GetClientList(Convert.ToInt32(companyid), isadministrator);
                userMasterModel.EmployeeList = ObjAccountMasterHelpers.GetEmployeeCustomDropdown(Convert.ToInt32(companyid), isadministrator);
                userMasterModel.UserRoleLIst = ObjAccountMasterHelpers.GetUserRoleDropdown(Convert.ToInt32(companyid));
                userMasterModel.ActiveYNList = objproductHelpers.GetProductYesNo();
                return View(userMasterModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        public IActionResult Index(long id, UserMasterModel userMasterModel)
        {
            try
            {
                long companyid = GetIntSession("CompanyId");
                int isadministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                userMasterModel.GetClientLIst = DbConnection.GetClientList(Convert.ToInt32(companyid), isadministrator);
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                #region Upload File
                if (userMasterModel.profilePhoto != null)
                {
                    var uniqueFileName = GetUniqueFileName(userMasterModel.profilePhoto.FileName);
                    var uploads = Path.Combine(hostingEnvironment.WebRootPath, "Uploads");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    userMasterModel.profilePhoto.CopyTo(new FileStream(filePath, FileMode.Create));
                    userMasterModel.ProfilePicture = uniqueFileName;
                    //to do : Save uniqueFileName  to your db table   
                }

                #endregion

                SqlParameter[] sqlParameters = new SqlParameter[13];
                sqlParameters[0] = new SqlParameter("@UserEmpVou", userMasterModel.UserEmpVou);
                sqlParameters[1] = new SqlParameter("@UserEmpNm", userMasterModel.EmpName);
                sqlParameters[2] = new SqlParameter("@UserId", userMasterModel.UserId);
                sqlParameters[3] = new SqlParameter("@UserCd", userMasterModel.UserCd);
                sqlParameters[4] = new SqlParameter("@UserPassword", userMasterModel.UserPass);
                sqlParameters[5] = new SqlParameter("@UserEmail", userMasterModel.UserEmail);
                sqlParameters[6] = new SqlParameter("@UserMobNo", userMasterModel.UserMobNo);
                sqlParameters[7] = new SqlParameter("@UserRolVou", userMasterModel.UserRolVou);
                sqlParameters[8] = new SqlParameter("@UserPhoto", userMasterModel.ProfilePicture);
                sqlParameters[9] = new SqlParameter("@UserActYN", userMasterModel.Active);
                sqlParameters[10] = new SqlParameter("@UserVou", id);
                sqlParameters[11] = new SqlParameter("@Flg", 1);
                sqlParameters[12] = new SqlParameter("@ClientId", userMasterModel.ClientId);
                DataTable dtUserMst = ObjDBConnection.CallStoreProcedure("UserMasterInsert", sqlParameters);
                if (dtUserMst != null && dtUserMst.Rows.Count > 0)
                {
                    int Status = DbConnection.ParseInt32(dtUserMst.Rows[0][0].ToString());
                    if (Status == -1)
                    {
                        SetErrorMessage("Dulplicate email address");
                        ViewBag.FocusType = "-1";
                        userMasterModel.EmployeeList = ObjAccountMasterHelpers.GetEmployeeCustomDropdown(Convert.ToInt32(companyid), isadministrator);
                        userMasterModel.UserRoleLIst = ObjAccountMasterHelpers.GetUserRoleDropdown(Convert.ToInt32(companyid));
                        userMasterModel.ActiveYNList = objproductHelpers.GetProductYesNo();
                        return View(userMasterModel);
                    }
                    else if (Status == -2)
                    {
                        SetErrorMessage("Dulplicate username");
                        ViewBag.FocusType = "-2";
                        userMasterModel.EmployeeList = ObjAccountMasterHelpers.GetEmployeeCustomDropdown(Convert.ToInt32(companyid), isadministrator);
                        userMasterModel.UserRoleLIst = ObjAccountMasterHelpers.GetUserRoleDropdown(Convert.ToInt32(companyid));
                        userMasterModel.ActiveYNList = objproductHelpers.GetProductYesNo();
                        return View(userMasterModel);
                    }
                    else if (Status == -3)
                    {
                        SetErrorMessage("Dulplicate usercode");
                        ViewBag.FocusType = "-3";
                        userMasterModel.EmployeeList = ObjAccountMasterHelpers.GetEmployeeCustomDropdown(Convert.ToInt32(companyid), isadministrator);
                        userMasterModel.UserRoleLIst = ObjAccountMasterHelpers.GetUserRoleDropdown(Convert.ToInt32(companyid));
                        userMasterModel.ActiveYNList = objproductHelpers.GetProductYesNo();
                        return View(userMasterModel);
                    }
                    else if (Status == -4)
                    {
                        SetErrorMessage("Dulplicate Employee");
                        ViewBag.FocusType = "-4";
                        userMasterModel.EmployeeList = ObjAccountMasterHelpers.GetEmployeeCustomDropdown(Convert.ToInt32(companyid), isadministrator);
                        userMasterModel.UserRoleLIst = ObjAccountMasterHelpers.GetUserRoleDropdown(Convert.ToInt32(companyid));
                        userMasterModel.ActiveYNList = objproductHelpers.GetProductYesNo();
                        return View(userMasterModel);
                    }
                    else
                    {
                        if (id > 0)
                        {
                            SetSuccessMessage("Update Sucessfully");
                        }
                        else
                        {
                            SetSuccessMessage("Inserted Sucessfully");
                        }
                        return RedirectToAction("index", "User", new { id = 0 });
                    }
                }
                else
                {
                    SetErrorMessage("Internal error");
                    userMasterModel.EmployeeList = ObjAccountMasterHelpers.GetEmployeeCustomDropdown(Convert.ToInt32(companyid), isadministrator);
                    userMasterModel.UserRoleLIst = ObjAccountMasterHelpers.GetUserRoleDropdown(Convert.ToInt32(companyid));
                    userMasterModel.ActiveYNList = objproductHelpers.GetProductYesNo();
                    ViewBag.FocusType = "1";
                    return View(userMasterModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult Delete(long id)
        {
            try
            {
                if (id > 0)
                {
                    long companyid = GetIntSession("CompanyId");
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@UserVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    //sqlParameters[2] = new SqlParameter("@CmpVou", companyid);
                    DataTable dtUserMst = ObjDBConnection.CallStoreProcedure("UserMasterInsert", sqlParameters);
                    if (dtUserMst != null && dtUserMst.Rows.Count > 0)
                    {
                        int value = DbConnection.ParseInt32(dtUserMst.Rows[0][0].ToString());
                        if (value > 0)
                        {
                            SetSuccessMessage("User Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "User");
        }

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            string query = string.Empty;
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/User/Index";
                userFormRights = GetUserRights(userId, currentURL);
                if (userFormRights == null)
                {
                    SetErrorMessage("You do not have right to access requested page. Please contact admin for more detail.");
                }
                ViewBag.userRight = userFormRights;
                #endregion

                double startRecord = 0;
                if (pageIndex > 0)
                {
                    startRecord = (pageIndex - 1) * pageSize;
                }
                long companyid = GetIntSession("CompanyId");
                long ClientId = GetIntSession("ClientId");
                int isadministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, 0, Convert.ToInt32(ClientId), 0, "User", isadministrator);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "User";
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }

        private void INIT(ref bool isReturn)
        {
            #region User Rights
            long userId = GetIntSession("UserId");
            UserFormRightModel userFormRights = new UserFormRightModel();
            string currentURL = GetCurrentURL();
            userFormRights = GetUserRights(userId, currentURL);
            if (userFormRights == null)
            {
                SetErrorMessage("You do not have right to access requested page. Please contact admin for more detail.");
                isReturn = true;
            }
            ViewBag.userRight = userFormRights;

            #endregion

            #region Dynamic Report

            if (userFormRights != null)
            {
                ViewBag.layoutList = GetGridLayoutDropDown(DbConnection.GridTypeView, userFormRights.ModuleId);
                ViewBag.pageNoList = GetPageNo();
            }

            #endregion
        }

        public IActionResult ExportToExcelPDF(int gridMstId, string searchValue, int type)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                var companyDetails = DbConnection.GetCompanyDetailsById(companyId);
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                //getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, 0, "", 0, 1);
                getReportDataModel = getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, YearId, "", 0, 1);
                if (type == 1)
                {
                    var bytes = Excel(getReportDataModel, "User Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "User.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "User Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "User.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private string GetUniqueFileName(string fileName)
        {
            return Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

    }
}
