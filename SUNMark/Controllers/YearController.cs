using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SUNMark.Classes;
using SUNMark.Models;

namespace SUNMark.Controllers
{
    public class YearController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();

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

                YearMasterModel yearMasterModel = new YearMasterModel();
                yearMasterModel.StartDate = new DateTime(DateTime.Now.Year, 04, 01).ToString("yyyy-MM-dd");
                yearMasterModel.EndDate = new DateTime(DateTime.Now.Year + 1, 03, 31).ToString("yyyy-MM-dd");
                if (id > 0)
                {
                    yearMasterModel.YearVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@YearVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 3);

                    DataTable dtYearMst = ObjDBConnection.CallStoreProcedure("YearMasterInsert", sqlParameters);
                    if (dtYearMst != null && dtYearMst.Rows.Count > 0)
                    {
                        yearMasterModel.StartDate = Convert.ToDateTime(dtYearMst.Rows[0]["StartDt"].ToString()).ToString("yyyy-MM-dd");
                        yearMasterModel.EndDate = Convert.ToDateTime(dtYearMst.Rows[0]["EndDt"].ToString()).ToString("yyyy-MM-dd");
                        yearMasterModel.IsDefault = DbConnection.ParseInt32(dtYearMst.Rows[0]["IsDefault"].ToString());
                        yearMasterModel.CompanyVou = DbConnection.ParseInt32(dtYearMst.Rows[0]["CmpVou"].ToString());
                    }
                }
                int clientid = Convert.ToInt32(GetIntSession("ClientId"));
                int isadministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                yearMasterModel.GetCompanyList = DbConnection.GetCompanyList(clientid,isadministrator);
                return View(yearMasterModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult Index(long id, YearMasterModel yearMasterModel)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int ClientId = Convert.ToInt32(GetIntSession("ClientId"));
                int isadministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                yearMasterModel.GetCompanyList = DbConnection.GetCompanyList(ClientId, isadministrator);
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@CmpVou", yearMasterModel.CompanyVou);
                sqlParameters[1] = new SqlParameter("@StartDt", yearMasterModel.StartDate);
                sqlParameters[2] = new SqlParameter("@EndDt", yearMasterModel.EndDate);
                sqlParameters[3] = new SqlParameter("@IsDefault", yearMasterModel.IsDefault);
                sqlParameters[4] = new SqlParameter("@YearVou", id);
                sqlParameters[5] = new SqlParameter("@Flg", 1);
                DataTable dtUserMst = ObjDBConnection.CallStoreProcedure("YearMasterInsert", sqlParameters);
                if (dtUserMst != null && dtUserMst.Rows.Count > 0)
                {
                    int Status = DbConnection.ParseInt32(dtUserMst.Rows[0][0].ToString());
                    if (Status == -1)
                    {
                        SetErrorMessage("Dulplicate start date and end date");
                        ViewBag.FocusType = "1";
                        return View(yearMasterModel);
                    }
                    else if (Status == -2)
                    {
                        SetErrorMessage("Default year exist for this company");
                        ViewBag.FocusType = "1";
                        return View(yearMasterModel);
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
                        return RedirectToAction("index", "Year", new { id = 0 });
                    }
                }
                else
                {
                    SetErrorMessage("Internal error");
                    ViewBag.FocusType = "1";
                    return View(yearMasterModel);
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
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@YearVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    DataTable dtUserMst = ObjDBConnection.CallStoreProcedure("YearMasterInsert", sqlParameters);
                    if (dtUserMst != null && dtUserMst.Rows.Count > 0)
                    {
                        int value = DbConnection.ParseInt32(dtUserMst.Rows[0][0].ToString());
                        if (value > 0)
                        {
                            SetSuccessMessage("Year Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "Year");
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
                string currentURL = "/Year/Index";
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
                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue,companyId);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "Year";
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
                    var bytes = Excel(getReportDataModel, "Year Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Year.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Year Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Year.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
