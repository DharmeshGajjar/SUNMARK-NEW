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
    public class DemoController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        AccountMasterHelpers ObjaccountMasterHelpers = new AccountMasterHelpers();

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
                DemoModel Demo = new DemoModel();
                if (id > 0)
                {
                    Demo.intId = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@intId", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtPrd = ObjDBConnection.CallStoreProcedure("GetUserDetails", sqlParameters);
                    if (DtPrd != null && DtPrd.Rows.Count > 0)
                    {
                        Demo.FName = DtPrd.Rows[0]["FName"].ToString();
                        Demo.LName = DtPrd.Rows[0]["LName"].ToString();
                    }
                }
                return View(Demo);

            }
            catch (Exception ex)
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
        public IActionResult Index(long id, DemoModel demo)
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
                if (!string.IsNullOrWhiteSpace(demo.FName) && !string.IsNullOrWhiteSpace(demo.LName))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@Fname", demo.FName.Trim());
                    sqlParameters[1] = new SqlParameter("@LName", demo.LName.Trim());
                    sqlParameters[2] = new SqlParameter("@intId", demo.intId);
                    sqlParameters[3] = new SqlParameter("@FLG", "1");

                    DataTable DtProduct = ObjDBConnection.CallStoreProcedure("Demo_Insert", sqlParameters);
                    if (DtProduct != null && DtProduct.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtProduct.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate First Name ");
                            ViewBag.FocusType = "-1";
                            return View(demo);
                        }
                        else if (status == -2)
                        {
                            SetErrorMessage("Dulplicate Last Name ");
                            ViewBag.FocusType = "-2";
                            return View(demo);
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
                            return RedirectToAction("index", "Demo", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                        return View(demo);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                    return View(demo);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new DemoModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                DemoModel demo = new DemoModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@PrdVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "1");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtProduct = ObjDBConnection.CallStoreProcedure("GetUserDetails", sqlParameters);
                    if (DtProduct != null && DtProduct.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtProduct.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
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
            return RedirectToAction("index", "Demo");
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
                    string currentURL = "/Demo/Index";
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
                    getReportDataModel.ControllerName = "Demo";
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
                    var bytes = Excel(getReportDataModel, "Demo Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Product.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Demo Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Product.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

     


    }
}
