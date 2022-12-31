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
    public class VoucherTypeController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
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
                int administrator = 0;
                VoucherTypeModel voucherTypeModel = new VoucherTypeModel();
                if (id > 0)
                {
                    voucherTypeModel.VchVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@VtyVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetVoucherTypeDetails", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        voucherTypeModel.VchType = DtEmp.Rows[0]["VtyType"].ToString();
                        voucherTypeModel.VchTrnMscVou = Convert.ToInt32(DtEmp.Rows[0]["VtyTrnMscVou"].ToString());
                        voucherTypeModel.VchTrnMscCd = DtEmp.Rows[0]["VtyTrnMscCd"].ToString();
                        voucherTypeModel.VchDesc = DtEmp.Rows[0]["VtyDesc"].ToString();
                    }
                }
                voucherTypeModel.TransTypeList = ObjaccountMasterHelpers.GetTranTypeCustomDropdown(companyId, administrator);

                return View(voucherTypeModel);
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
        public IActionResult Index(long id, VoucherTypeModel voucherTypeModel)
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
                int administrator = 0;
                if (!string.IsNullOrWhiteSpace(voucherTypeModel.VchType) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(voucherTypeModel.VchTrnMscVou).ToString()) && !string.IsNullOrWhiteSpace(voucherTypeModel.VchTrnMscCd) && !string.IsNullOrWhiteSpace(voucherTypeModel.VchDesc))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("@VtyTrnMscVou", voucherTypeModel.VchTrnMscVou);
                    sqlParameters[1] = new SqlParameter("@VtyTrnMscCd", voucherTypeModel.VchTrnMscCd);
                    sqlParameters[2] = new SqlParameter("@VtyType", voucherTypeModel.VchType);
                    sqlParameters[3] = new SqlParameter("@VtyDesc", voucherTypeModel.VchDesc);
                    sqlParameters[4] = new SqlParameter("@VtyVou", id);
                    sqlParameters[5] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[6] = new SqlParameter("@FLG", "1");

                    DataTable DtVouTy = ObjDBConnection.CallStoreProcedure("VoucherTypeMst_Insert", sqlParameters);
                    if (DtVouTy != null && DtVouTy.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtVouTy.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Voucher Type");
                            ViewBag.FocusType = "-1";
                            voucherTypeModel.TransTypeList = ObjaccountMasterHelpers.GetTranTypeCustomDropdown(companyId, administrator);
                            return View(voucherTypeModel);
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
                            return RedirectToAction("index", "VoucherType", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        voucherTypeModel.TransTypeList = ObjaccountMasterHelpers.GetTranTypeCustomDropdown(companyId, administrator);
                        return View(voucherTypeModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "1";
                    voucherTypeModel.TransTypeList = ObjaccountMasterHelpers.GetTranTypeCustomDropdown(companyId, administrator);
                    return View(voucherTypeModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new VoucherTypeModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                VoucherTypeModel voucherTypeModel = new VoucherTypeModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@VtyVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "1");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtAcc = ObjDBConnection.CallStoreProcedure("GetVoucherTypeDetails", sqlParameters);
                    if (DtAcc != null && DtAcc.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtAcc.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Voucher Type Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "VoucherType");
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
                    string currentURL = "/VoucherType/Index";
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
                    getReportDataModel.ControllerName = "VoucherType";
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
                    var bytes = Excel(getReportDataModel, "Voucher Type Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "VoucherType.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Voucher Type Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "VoucherType.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult GetTransactionTypeList()
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                var transTypeList = ObjaccountMasterHelpers.GetTranTypeCustomDropdown(companyId, administrator);
                return Json(new { result = true, data = transTypeList });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
