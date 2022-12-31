using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SUNMark.Classes;
using SUNMark.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Controllers
{
    public class EmployeeMasterController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        AccountMasterHelpers ObjaccountMasterHelpers = new AccountMasterHelpers();
        private readonly IWebHostEnvironment hostingEnvironment;

        public EmployeeMasterController(IWebHostEnvironment _hostingEnvironment)
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
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                EmployeeMasterModel employeeMasterModel = new EmployeeMasterModel();
                if (id > 0)
                {
                    employeeMasterModel.EmpVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@EmpVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetEmployeeDetails", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        employeeMasterModel.EmpCode = DtEmp.Rows[0]["EmpCd"].ToString();
                        employeeMasterModel.EmpName = DtEmp.Rows[0]["EmpNm"].ToString();
                        employeeMasterModel.EmpDsgMscId = Convert.ToInt32(DtEmp.Rows[0]["EmpDsgMscVou"].ToString());
                        employeeMasterModel.EmpDepMscId = Convert.ToInt32(DtEmp.Rows[0]["EmpDepMscVou"].ToString());
                        if (DtEmp.Rows[0]["EmpShift"].ToString().TrimEnd() == "First")
                        {
                            employeeMasterModel.EmpShtId = 1;
                        }
                        else if (DtEmp.Rows[0]["EmpShift"].ToString().TrimEnd() == "Second")
                        {
                            employeeMasterModel.EmpShtId = 2;
                        }
                        else if (DtEmp.Rows[0]["EmpShift"].ToString().TrimEnd() == "Office")
                        {
                            employeeMasterModel.EmpShtId = 3;
                        }
                        else
                        {
                            employeeMasterModel.EmpShtId = 0;
                        }
                        employeeMasterModel.EmpShift = DtEmp.Rows[0]["EmpShift"].ToString();
                        if (DtEmp.Rows[0]["EmpActYN"].ToString().TrimEnd() == "No")
                        {
                            employeeMasterModel.EmpActId = 1;
                        }
                        else
                        {
                            employeeMasterModel.EmpActId = 0;
                        }
                        employeeMasterModel.ActiveYN = DtEmp.Rows[0]["EmpActYN"].ToString();
                        employeeMasterModel.EmpMobile = DtEmp.Rows[0]["EmpMob"].ToString();
                        employeeMasterModel.EmpEmail = DtEmp.Rows[0]["EmpEmail"].ToString();
                        employeeMasterModel.ProfilePicture = DtEmp.Rows[0]["EmpPhoto"].ToString();
                        employeeMasterModel.UserID = DtEmp.Rows[0]["UserID"].ToString();
                    }
                }
                employeeMasterModel.EmpActiveList = objProductHelper.GetProductYesNo();
                employeeMasterModel.EmpShiftList = objProductHelper.GetShift();
                employeeMasterModel.DesignationList = ObjaccountMasterHelpers.GetDesignationDropdown(companyId);
                employeeMasterModel.DepartmentMscList = ObjaccountMasterHelpers.GetDepartmentMscDropdown(companyId);

                return View(employeeMasterModel);

            }
            catch (Exception)
            {

                throw;
            }
            return View();
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

        [HttpPost]
        public IActionResult Index(long id, EmployeeMasterModel employeeMasterModel)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                #region Upload File
                if (employeeMasterModel.profilePhoto != null)
                {
                    var uniqueFileName = GetUniqueFileName(employeeMasterModel.profilePhoto.FileName);
                    var uploads = Path.Combine(hostingEnvironment.WebRootPath, "Uploads");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    employeeMasterModel.profilePhoto.CopyTo(new FileStream(filePath, FileMode.Create));
                    employeeMasterModel.ProfilePicture = uniqueFileName;
                    //to do : Save uniqueFileName  to your db table   
                }

                #endregion

                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                if (!string.IsNullOrWhiteSpace(employeeMasterModel.EmpCode) && !string.IsNullOrWhiteSpace(employeeMasterModel.EmpName) && !string.IsNullOrWhiteSpace(employeeMasterModel.EmpShift) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(employeeMasterModel.EmpDsgMscId).ToString()))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[14];
                    sqlParameters[0] = new SqlParameter("@EmpDsgNm", employeeMasterModel.EmpDsgNm);
                    sqlParameters[1] = new SqlParameter("@EmpDsgMscVou", employeeMasterModel.EmpDsgMscId);
                    sqlParameters[2] = new SqlParameter("@EmpName", employeeMasterModel.EmpName);
                    sqlParameters[3] = new SqlParameter("@EmpCode", employeeMasterModel.EmpCode);
                    sqlParameters[4] = new SqlParameter("@EmpShift", employeeMasterModel.EmpShift);
                    sqlParameters[5] = new SqlParameter("@EmpMob", employeeMasterModel.EmpMobile);
                    sqlParameters[6] = new SqlParameter("@EmpEmail", employeeMasterModel.EmpEmail);
                    sqlParameters[7] = new SqlParameter("@EmpActYN", employeeMasterModel.ActiveYN);
                    sqlParameters[8] = new SqlParameter("@EmpPhoto", employeeMasterModel.ProfilePicture);
                    sqlParameters[9] = new SqlParameter("@EmpVou", id);
                    sqlParameters[10] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[11] = new SqlParameter("@FLG", "1");
                    sqlParameters[12] = new SqlParameter("@EmpDepMscVou", employeeMasterModel.EmpDepMscId);
                    sqlParameters[13] = new SqlParameter("@EmpDepNm", employeeMasterModel.EmpDepNm);

                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("EmployeeMst_Insert", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtEmp.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Employee Code");
                            ViewBag.FocusType = "-1";
                            employeeMasterModel.EmpActiveList = objProductHelper.GetProductYesNo();
                            employeeMasterModel.EmpShiftList = objProductHelper.GetShift();
                            employeeMasterModel.DesignationList = ObjaccountMasterHelpers.GetDesignationDropdown(companyId);
                            employeeMasterModel.DepartmentMscList = ObjaccountMasterHelpers.GetDepartmentMscDropdown(companyId);
                            return View(employeeMasterModel);
                        }
                        else if (status == -2)
                        {
                            SetErrorMessage("Dulplicate Employee");
                            ViewBag.FocusType = "-2";
                            employeeMasterModel.EmpActiveList = objProductHelper.GetProductYesNo();
                            employeeMasterModel.EmpShiftList = objProductHelper.GetShift();
                            employeeMasterModel.DesignationList = ObjaccountMasterHelpers.GetDesignationDropdown(companyId);
                            employeeMasterModel.DepartmentMscList = ObjaccountMasterHelpers.GetDepartmentMscDropdown(companyId);
                            return View(employeeMasterModel);
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
                            return RedirectToAction("index", "EmployeeMaster", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        employeeMasterModel.EmpActiveList = objProductHelper.GetProductYesNo();
                        employeeMasterModel.EmpShiftList = objProductHelper.GetShift();
                        employeeMasterModel.DesignationList = ObjaccountMasterHelpers.GetDesignationDropdown(companyId);
                        employeeMasterModel.DepartmentMscList = ObjaccountMasterHelpers.GetDepartmentMscDropdown(companyId);
                        ViewBag.FocusType = "-1";
                        return View(employeeMasterModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    employeeMasterModel.EmpActiveList = objProductHelper.GetProductYesNo();
                    employeeMasterModel.EmpShiftList = objProductHelper.GetShift();
                    employeeMasterModel.DesignationList = ObjaccountMasterHelpers.GetDesignationDropdown(companyId);
                    employeeMasterModel.DepartmentMscList = ObjaccountMasterHelpers.GetDepartmentMscDropdown(companyId);
                    ViewBag.FocusType = "-1";
                    return View(employeeMasterModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new EmployeeMasterModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                MscMstPartialViewModel mscMstPartialViewModel = new MscMstPartialViewModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@EmpVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "1");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtEmpMst = ObjDBConnection.CallStoreProcedure("GetEmployeeDetails", sqlParameters);
                    if (DtEmpMst != null && DtEmpMst.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtEmpMst.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Employee Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "EmployeeMaster");
        }

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                if (gridMstId > 0)
                {
                    #region User Rights
                    long userId = GetIntSession("UserId");
                    UserFormRightModel userFormRights = new UserFormRightModel();
                    string currentURL = "/EmployeeMaster/Index";
                    userFormRights = GetUserRights(userId, currentURL);
                    if (userFormRights == null)
                    {
                        SetErrorMessage("You do not have right to access requested page. Please contact admin for more detail.");
                    }
                    ViewBag.userRight = userFormRights;
                    #endregion

                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));

                    double startRecord = 0;
                    if (pageIndex > 0)
                    {
                        startRecord = (pageIndex - 1) * pageSize;
                    }
                    getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId);
                    if (getReportDataModel.IsError)
                    {
                        ViewBag.Query = getReportDataModel.Query;
                        return PartialView("_reportView");
                    }
                    getReportDataModel.pageIndex = pageIndex;
                    getReportDataModel.ControllerName = "EmployeeMaster";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
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
                    var bytes = Excel(getReportDataModel, "Employee Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Employee.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Employee Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Employee.pdf");
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

        [Route("/EmployeeMaster/designation-list")]
        public IActionResult DesignationList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var desgList = ObjaccountMasterHelpers.GetDesignationDropdown(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                desgList = desgList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(desgList) });
        }

        [Route("/EmployeeMaster/GetDepartmentMsc-List")]
        public IActionResult DepartmentList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var departList = ObjaccountMasterHelpers.GetDepartmentMscDropdown(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                departList = departList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(departList) });
        }
    }
}
