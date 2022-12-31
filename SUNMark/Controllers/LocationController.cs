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
    public class LocationController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        TaxMasterHelpers ObjtaxMasterHelpers = new TaxMasterHelpers();

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
                LocationMasterModel locationMasterModel = new LocationMasterModel();
                if (id > 0)
                {
                    locationMasterModel.LocVou  = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@LocVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtLoc = ObjDBConnection.CallStoreProcedure("GetLocationDetails", sqlParameters);
                    if (DtLoc != null && DtLoc.Rows.Count > 0)
                    {
                        locationMasterModel.LocGdnVou = Convert.ToInt32(DtLoc.Rows[0]["LocGdnVou"].ToString());
                        locationMasterModel.LocName = DtLoc.Rows[0]["LocNm"].ToString();
                        locationMasterModel.LocDesc = DtLoc.Rows[0]["LocDesc"].ToString();
                        if (DtLoc.Rows[0]["LocActYN"].ToString().TrimEnd() == "No")
                        {
                            locationMasterModel.LocActYN = 1;
                        }
                        else
                        {
                            locationMasterModel.LocActYN = 0;
                        }
                        locationMasterModel.ActiveYN = DtLoc.Rows[0]["LocActYN"].ToString();
                    }
                }
                locationMasterModel.GodownList = ObjtaxMasterHelpers.GetGodownDropdown(companyId);
                locationMasterModel.ActiveList = ObjtaxMasterHelpers.GetActiveYesNo();

                return View(locationMasterModel);

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
        public IActionResult Index(long id, LocationMasterModel locationMasterModel)
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
                if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(locationMasterModel.LocGdnVou).ToString()) && !String.IsNullOrWhiteSpace(locationMasterModel.LocName))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("@LocGdnVou", locationMasterModel.LocGdnVou);
                    sqlParameters[1] = new SqlParameter("@LocName", locationMasterModel.LocName);
                    sqlParameters[2] = new SqlParameter("@LocDesc", locationMasterModel.LocDesc);
                    sqlParameters[3] = new SqlParameter("@LocActYN", locationMasterModel.ActiveYN);
                    sqlParameters[4] = new SqlParameter("@LocVou", id);
                    sqlParameters[5] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[6] = new SqlParameter("@FLG", "1");

                    DataTable DtLoc = ObjDBConnection.CallStoreProcedure("LocationMst_Insert", sqlParameters);
                    if (DtLoc != null && DtLoc.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtLoc.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Location");
                            locationMasterModel.GodownList = ObjtaxMasterHelpers.GetGodownDropdown(companyId);
                            locationMasterModel.ActiveList = ObjtaxMasterHelpers.GetActiveYesNo();
                            ViewBag.FocusType = "-1";
                            return View(locationMasterModel);
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
                            return RedirectToAction("index", "Location", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        locationMasterModel.GodownList = ObjtaxMasterHelpers.GetGodownDropdown(companyId);
                        locationMasterModel.ActiveList = ObjtaxMasterHelpers.GetActiveYesNo();
                        ViewBag.FocusType = "-1";
                        return View(locationMasterModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    locationMasterModel.GodownList = ObjtaxMasterHelpers.GetGodownDropdown(companyId);
                    locationMasterModel.ActiveList = ObjtaxMasterHelpers.GetActiveYesNo();
                    ViewBag.FocusType = "-1";
                    return View(locationMasterModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new LocationMasterModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                LocationMasterModel locationMasterModel = new LocationMasterModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@LocVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "1");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtLoc = ObjDBConnection.CallStoreProcedure("GetLocationDetails", sqlParameters);
                    if (DtLoc != null && DtLoc.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtLoc.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Location Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "Location");
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
                    string currentURL = "/Location/Index";
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
                    getReportDataModel.ControllerName = "Location";
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
                    var bytes = Excel(getReportDataModel, "Location Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Location.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Location Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Location.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Route("/Location/GetGodown-List")]
        public IActionResult GodownList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var godownList = ObjtaxMasterHelpers.GetSalesAccountDropdown(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                godownList = godownList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(godownList) });
        }

    }
}
