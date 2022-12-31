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
    public class GodownMasterController : BaseController
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
                GodownMasterModel godownMasterModel = new GodownMasterModel();
                if (id > 0)
                {
                    godownMasterModel.GdnVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@GdnVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtGodown = ObjDBConnection.CallStoreProcedure("GetGodownDetails", sqlParameters);
                    if (DtGodown != null && DtGodown.Rows.Count > 0)
                    {
                        godownMasterModel.GdnName = DtGodown.Rows[0]["GdnNm"].ToString();
                        godownMasterModel.GdnAdd= DtGodown.Rows[0]["GdnAdd"].ToString();
                        godownMasterModel.GdnPhone = DtGodown.Rows[0]["GdnPhone"].ToString();
                        godownMasterModel.GdnAccVou = Convert.ToInt32(DtGodown.Rows[0]["GdnAccVou"].ToString());
                        if (DtGodown.Rows[0]["GdnActYN"].ToString().TrimEnd() == "No")
                        {
                            godownMasterModel.GdnActYN = 1;
                        }
                        else
                        {
                            godownMasterModel.GdnActYN = 0;
                        }
                        godownMasterModel.ActiveYN = DtGodown.Rows[0]["GdnActYN"].ToString();
                    }
                }
                godownMasterModel.AccountList = ObjtaxMasterHelpers.GetSalesAccountDropdown(companyId);
                godownMasterModel.ActiveList = ObjtaxMasterHelpers.GetActiveYesNo();


                return View(godownMasterModel);

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
        public IActionResult Index(long id, GodownMasterModel godownMasterModel)
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
                if (!string.IsNullOrWhiteSpace(godownMasterModel.GdnName) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(godownMasterModel.GdnAccVou).ToString()))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[8];
                    sqlParameters[0] = new SqlParameter("@GdnName", godownMasterModel.GdnName);
                    sqlParameters[1] = new SqlParameter("@GdnAdd", godownMasterModel.GdnAdd);
                    sqlParameters[2] = new SqlParameter("@GdnPhone", godownMasterModel.GdnPhone);
                    sqlParameters[3] = new SqlParameter("@GdnAccVou", godownMasterModel.GdnAccVou);
                    sqlParameters[4] = new SqlParameter("@GdnActYN", godownMasterModel.ActiveYN);
                    sqlParameters[5] = new SqlParameter("@GdnVou", id);
                    sqlParameters[6] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[7] = new SqlParameter("@FLG", "1");

                    DataTable DtAccount = ObjDBConnection.CallStoreProcedure("GodownMst_Insert", sqlParameters);
                    if (DtAccount != null && DtAccount.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtAccount.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Godown");
                            godownMasterModel.AccountList = ObjtaxMasterHelpers.GetSalesAccountDropdown(companyId);
                            godownMasterModel.ActiveList = ObjtaxMasterHelpers.GetActiveYesNo();
                            ViewBag.FocusType = "-1";
                            return View(godownMasterModel);
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
                            return RedirectToAction("index", "GodownMaster", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        godownMasterModel.AccountList = ObjtaxMasterHelpers.GetSalesAccountDropdown(companyId);
                        godownMasterModel.ActiveList = ObjtaxMasterHelpers.GetActiveYesNo();
                        ViewBag.FocusType = "-1";
                        return View(godownMasterModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    godownMasterModel.AccountList = ObjtaxMasterHelpers.GetSalesAccountDropdown(companyId);
                    godownMasterModel.ActiveList = ObjtaxMasterHelpers.GetActiveYesNo();
                    ViewBag.FocusType = "-1";
                    return View(godownMasterModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new GodownMasterModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                CityModel cityModel = new CityModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@GdnVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "1");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("GetGodownDetails", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Godown Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "GodownMaster");
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
                    string currentURL = "/GodownMaster/Index";
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
                    getReportDataModel.ControllerName = "GodownMaster";
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
                    var bytes = Excel(getReportDataModel, "Godown Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Godown.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Godown Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Godown.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [Route("/GodownMaster/GetAccount-List")]
        public IActionResult AccountList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var accList = ObjtaxMasterHelpers.GetSalesAccountDropdown(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                accList = accList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(accList) });
        }

    }
}
