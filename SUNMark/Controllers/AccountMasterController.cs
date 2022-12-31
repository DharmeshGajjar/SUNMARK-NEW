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
    public class AccountMasterController : BaseController
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
                AccountMasterModel accountMasterModel = new AccountMasterModel();
                if (id > 0)
                {
                    accountMasterModel.AccVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@AccVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtAcc = ObjDBConnection.CallStoreProcedure("GetAccountDetails", sqlParameters);
                    if (DtAcc != null && DtAcc.Rows.Count > 0)
                    {
                        accountMasterModel.AccCd = DtAcc.Rows[0]["AccCd"].ToString();
                        accountMasterModel.AccName = DtAcc.Rows[0]["AccNm"].ToString();
                        accountMasterModel.AccAdd = DtAcc.Rows[0]["AccAdd"].ToString();
                        accountMasterModel.AccCtyVou = Convert.ToInt32(DtAcc.Rows[0]["AccCtyVou"].ToString());
                        accountMasterModel.AccCity = DtAcc.Rows[0]["AccCity"].ToString();
                        accountMasterModel.AccState = DtAcc.Rows[0]["AccState"].ToString();
                        accountMasterModel.AccCountry = DtAcc.Rows[0]["AccCountry"].ToString();
                        accountMasterModel.AccPhone = DtAcc.Rows[0]["AccPhone"].ToString();
                        accountMasterModel.AccMob = DtAcc.Rows[0]["AccMob"].ToString();
                        accountMasterModel.AccEmail = DtAcc.Rows[0]["AccEmail"].ToString();
                        accountMasterModel.AccGST = DtAcc.Rows[0]["AccGST"].ToString();
                        accountMasterModel.AccPAN = DtAcc.Rows[0]["AccPAN"].ToString();
                        accountMasterModel.AccOth1 = DtAcc.Rows[0]["AccRem1"].ToString();
                        accountMasterModel.AccOth2 = DtAcc.Rows[0]["AccRem2"].ToString();
                        accountMasterModel.AccOth3 = DtAcc.Rows[0]["AccRem3"].ToString();
                        accountMasterModel.AccOth4 = DtAcc.Rows[0]["AccRem4"].ToString();
                        accountMasterModel.AccOth5 = DtAcc.Rows[0]["AccRem5"].ToString();
                        accountMasterModel.AccType = Convert.ToInt32(DtAcc.Rows[0]["AccType"].ToString());
                        accountMasterModel.TypeNm = DtAcc.Rows[0]["AccTypeC"].ToString();
                        accountMasterModel.AccConPer1 = DtAcc.Rows[0]["AccConPer1"].ToString();
                        accountMasterModel.AccConPerMob1 = DtAcc.Rows[0]["AccConPerMob1"].ToString();
                        accountMasterModel.AccConPerEmail1 = DtAcc.Rows[0]["AccConPerEmail1"].ToString();
                        accountMasterModel.AccConPer2 = DtAcc.Rows[0]["AccConPer2"].ToString();
                        accountMasterModel.AccConPerMob2 = DtAcc.Rows[0]["AccConPerMob2"].ToString();
                        accountMasterModel.AccConPerEmail2 = DtAcc.Rows[0]["AccConPerEmail2"].ToString();
                        accountMasterModel.AccConPer3 = DtAcc.Rows[0]["AccConPer3"].ToString();
                        accountMasterModel.AccConPerMob3 = DtAcc.Rows[0]["AccConPerMob3"].ToString();
                        accountMasterModel.AccConPerEmail3 = DtAcc.Rows[0]["AccConPerEmail3"].ToString();
                        accountMasterModel.AccConPer4 = DtAcc.Rows[0]["AccConPer4"].ToString();
                        accountMasterModel.AccConPerMob4 = DtAcc.Rows[0]["AccConPerMob4"].ToString();
                        accountMasterModel.AccConPerEmail4 = DtAcc.Rows[0]["AccConPerEmail4"].ToString();
                    }
                }
                accountMasterModel.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                accountMasterModel.TypeList = ObjaccountMasterHelpers.GetAccountType();
                return View(accountMasterModel);
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
        public IActionResult Index(long id, AccountMasterModel accountMasterModel)
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
                if (!string.IsNullOrWhiteSpace(accountMasterModel.AccName) && !string.IsNullOrWhiteSpace(accountMasterModel.AccCd))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[34];
                    sqlParameters[0] = new SqlParameter("@AccName", accountMasterModel.AccName);
                    sqlParameters[1] = new SqlParameter("@AccCd", accountMasterModel.AccCd);
                    sqlParameters[2] = new SqlParameter("@AccAdd", accountMasterModel.AccAdd);
                    sqlParameters[3] = new SqlParameter("@AccCtyVou", accountMasterModel.AccCtyVou);
                    sqlParameters[4] = new SqlParameter("@AccCity", accountMasterModel.AccCity);
                    sqlParameters[5] = new SqlParameter("@AccState", accountMasterModel.AccState);
                    sqlParameters[6] = new SqlParameter("@AccCountry", accountMasterModel.AccCountry);
                    sqlParameters[7] = new SqlParameter("@AccPhone", accountMasterModel.AccPhone);
                    sqlParameters[8] = new SqlParameter("@AccMob", accountMasterModel.AccMob);
                    sqlParameters[9] = new SqlParameter("@AccEmail", accountMasterModel.AccEmail);
                    sqlParameters[10] = new SqlParameter("@AccGST", accountMasterModel.AccGST);
                    sqlParameters[11] = new SqlParameter("@AccPAN", accountMasterModel.AccPAN);
                    sqlParameters[12] = new SqlParameter("@AccOth1", accountMasterModel.AccOth1);
                    sqlParameters[13] = new SqlParameter("@AccOth2", accountMasterModel.AccOth2);
                    sqlParameters[14] = new SqlParameter("@AccOth3", accountMasterModel.AccOth3);
                    sqlParameters[15] = new SqlParameter("@AccOth4", accountMasterModel.AccOth4);
                    sqlParameters[16] = new SqlParameter("@AccOth5", accountMasterModel.AccOth5);
                    sqlParameters[17] = new SqlParameter("@AccVou", id);
                    sqlParameters[18] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[19] = new SqlParameter("@FLG", "1");
                    sqlParameters[20] = new SqlParameter("@AccTyp", accountMasterModel.AccType);
                    sqlParameters[21] = new SqlParameter("@AccType", accountMasterModel.TypeNm);
                    sqlParameters[22] = new SqlParameter("@AccConPer1", accountMasterModel.AccConPer1);
                    sqlParameters[23] = new SqlParameter("@AccConPerMob1", accountMasterModel.AccConPerMob1);
                    sqlParameters[24] = new SqlParameter("@AccConPerEmail1", accountMasterModel.AccConPerEmail1);
                    sqlParameters[25] = new SqlParameter("@AccConPer2", accountMasterModel.AccConPer2);
                    sqlParameters[26] = new SqlParameter("@AccConPerMob2", accountMasterModel.AccConPerMob2);
                    sqlParameters[27] = new SqlParameter("@AccConPerEmail2", accountMasterModel.AccConPerEmail2);
                    sqlParameters[28] = new SqlParameter("@AccConPer3", accountMasterModel.AccConPer3);
                    sqlParameters[29] = new SqlParameter("@AccConPerMob3", accountMasterModel.AccConPerMob3);
                    sqlParameters[30] = new SqlParameter("@AccConPerEmail3", accountMasterModel.AccConPerEmail3);
                    sqlParameters[31] = new SqlParameter("@AccConPer4", accountMasterModel.AccConPer4);
                    sqlParameters[32] = new SqlParameter("@AccConPerMob4", accountMasterModel.AccConPerMob4);
                    sqlParameters[33] = new SqlParameter("@AccConPerEmail4", accountMasterModel.AccConPerEmail4);
                    DataTable DtAccount = ObjDBConnection.CallStoreProcedure("AccountMst_Insert", sqlParameters);
                    if (DtAccount != null && DtAccount.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtAccount.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Account Code");
                            ViewBag.FocusType = "-1";
                            accountMasterModel.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                            return View(accountMasterModel);
                        }
                        else if (status == -2)
                        {
                            SetErrorMessage("Dulplicate Account");
                            ViewBag.FocusType = "-2";
                            accountMasterModel.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                            return View(accountMasterModel);
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
                            return RedirectToAction("index", "AccountMaster", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                        accountMasterModel.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                        return View(accountMasterModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                    accountMasterModel.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                    return View(accountMasterModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new CityModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                AccountMasterModel accountMasterModel = new AccountMasterModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@AccVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "1");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtAcc = ObjDBConnection.CallStoreProcedure("GetAccountDetails", sqlParameters);
                    if (DtAcc != null && DtAcc.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtAcc.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Account Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "AccountMaster");
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
                    string currentURL = "/AccountMaster/Index";
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
                    getReportDataModel.ControllerName = "AccountMaster";
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
                    var bytes = Excel(getReportDataModel, "Account Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Account.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Account Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Account.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public IActionResult AddCity(string name, int stateid, string state)
        //{
        //    try
        //    {

        //        long userId = GetIntSession("UserId");
        //        int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
        //        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(stateid).ToString()))
        //        {
        //            SqlParameter[] sqlParameters = new SqlParameter[6];
        //            sqlParameters[0] = new SqlParameter("@CtyName", name);
        //            sqlParameters[1] = new SqlParameter("@CtyStaVou", stateid);
        //            sqlParameters[2] = new SqlParameter("@CtyState", state);
        //            sqlParameters[3] = new SqlParameter("@CtyVou", "0");
        //            sqlParameters[4] = new SqlParameter("@UsrVou", userId);
        //            sqlParameters[5] = new SqlParameter("@FLG", "1");

        //            DataTable DtCity = ObjDBConnection.CallStoreProcedure("CityMst_Insert", sqlParameters);
        //            if (DtCity != null && DtCity.Rows.Count > 0)
        //            {
        //                int status = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
        //                if (status == -1)
        //                {
        //                    return Json(new { result = false, message = "Duplicate City" });
        //                }
        //                else
        //                {
        //                    var cityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
        //                    return Json(new { result = true, message = "Inserted successfully", data = cityList });
        //                }
        //            }
        //            else
        //            {
        //                return Json(new { result = false, message = "Please Enter the Value" });
        //            }
        //        }
        //        else
        //        {
        //            return Json(new { result = false, message = "Please Enter the Value" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


        //public IActionResult GetCityList()
        //{
        //    try
        //    {
        //        int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
        //        var CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
        //        return Json(new { result = true, data = CityList });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


        [Route("/accountmaster/city-list")]
        public IActionResult CityList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                CityList = CityList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(CityList) });
        }

    }
}
