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
    public class StockLedMCoilController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();

        public IActionResult Index(long id)
        {
            StockLedgerModel stockLedgerModel = new StockLedgerModel();
            try
            {
                bool isreturn = false;
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                long yearId = GetIntSession("YearId");
                int administrator = 0;
                var yearData = DbConnection.GetYearListByCompanyId(Convert.ToInt32(companyId)).Where(x => x.YearVou == yearId).FirstOrDefault();
                if (yearData != null)
                {
                    stockLedgerModel.FrDt = yearData.StartDate;
                    stockLedgerModel.ToDt = yearData.EndDate;
                }
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                SqlParameter[] sqlPara = new SqlParameter[0];
                DataTable DtTrnTypeMst = ObjDBConnection.CallStoreProcedure("GetTrnTypeDetails", sqlPara);

                ViewBag.processList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                stockLedgerModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                stockLedgerModel.TrnTypeList = objProductHelper.GetTrnTypeDropdown();
                stockLedgerModel.RecIssList = objProductHelper.GetRecIss();
                stockLedgerModel.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                stockLedgerModel.StockYNList = objProductHelper.GetStockYN();
                stockLedgerModel.GradeList = objProductHelper.GetGradeMasterDropdown(companyId, 0);
                stockLedgerModel.FinishList = objProductHelper.GetFinishMasterDropdown(companyId, 0);
                stockLedgerModel.NBList = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
                stockLedgerModel.SCHList = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(stockLedgerModel);
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
                ViewBag.layoutList = GetGridLayoutDropDown(DbConnection.GridTypeReport, userFormRights.ModuleId);
                ViewBag.pageNoList = GetPageNo();
            }

            #endregion
        }

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby, string companyid, string trnType, string recIssID, string productid, string vNo, string frDt, string toDt, string stockid, string thick, string width, string qty, string od, string nb, string sch, string grade, string finish, string langth, string proc, string coilno)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                string guid = GetStringSession("LoginGUID");
                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/StockLedMCoil/Index";
                userFormRights = GetUserRights(userId, currentURL);
                if (userFormRights == null)
                {
                    SetErrorMessage("You do not have right to access requested page. Please contact admin for more detail.");
                }
                ViewBag.userRight = userFormRights;
                #endregion

                //if (productid == "" || productid == "0" || productid == null)
                //{
                //    SetErrorMessage("Please select Product Type!!!");
                //    return PartialView("_reportView");
                //}

                double startRecord = 0;
                if (pageIndex > 0)
                {
                    startRecord = (pageIndex - 1) * pageSize;
                }

                SqlParameter[] sqlParameters = new SqlParameter[21];
                sqlParameters[0] = new SqlParameter("@CmpVou", companyid);
                if (trnType == "Select")
                {
                    sqlParameters[1] = new SqlParameter("@TrnType", "");
                }
                else
                {
                    sqlParameters[1] = new SqlParameter("@TrnType", trnType);
                }
                sqlParameters[2] = new SqlParameter("@RecIss", recIssID);
                sqlParameters[3] = new SqlParameter("@PrdVou", productid);
                sqlParameters[4] = new SqlParameter("@VNo", vNo);
                sqlParameters[5] = new SqlParameter("@FrDt", frDt);
                sqlParameters[6] = new SqlParameter("@ToDt", toDt);
                sqlParameters[7] = new SqlParameter("@Stock", 0);
                sqlParameters[8] = new SqlParameter("@UserID", userId);
                sqlParameters[9] = new SqlParameter("@Thick", thick);
                sqlParameters[10] = new SqlParameter("@Width", width);
                sqlParameters[11] = new SqlParameter("@Qty", qty);
                sqlParameters[12] = new SqlParameter("@OD", od);
                sqlParameters[13] = new SqlParameter("@NB", nb);
                sqlParameters[14] = new SqlParameter("@Sch", sch);
                sqlParameters[15] = new SqlParameter("@GUID", guid);
                sqlParameters[16] = new SqlParameter("@Grade", grade);
                sqlParameters[17] = new SqlParameter("@Finish", finish);
                sqlParameters[18] = new SqlParameter("@Langth", langth);
                sqlParameters[19] = new SqlParameter("@Proc", proc);
                sqlParameters[20] = new SqlParameter("@Coilno", coilno);
                DataTable DtStkLed = ObjDBConnection.CallStoreProcedure("GetMCoilStockLedgerDetails", sqlParameters);


                string whereConditionQuery = string.Empty;
                if (!string.IsNullOrWhiteSpace(companyid))
                {
                    whereConditionQuery += " AND StkLedger.StkCmpVou='" + companyId + "'";
                }
                if (!string.IsNullOrWhiteSpace(vNo))
                {
                    whereConditionQuery += " AND StkLedger.StkVNo='" + vNo + "'";
                }
                if (!string.IsNullOrWhiteSpace(guid))
                {
                    whereConditionQuery += " AND StkLedger.StkGUID='" + guid + "'";
                }
                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, 0, 0, "", 0, 0, whereConditionQuery);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "StockLedMCoil";
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }

        public IActionResult ExportToExcelPDF(int gridMstId, string searchValue, int type, string companyid, string trnType, string recIssID, string productid, string vNo, string frDt, string toDt, string stockid, string thick, string width, string qty, string od, string nb, string sch, string grade, string finish, string langth, string proc, string coilno)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                string guid = GetStringSession("LoginGUID");
                var companyDetails = DbConnection.GetCompanyDetailsById(companyId);

                SqlParameter[] sqlParameters = new SqlParameter[21];
                sqlParameters[0] = new SqlParameter("@CmpVou", companyid);
                if (trnType == "Select")
                {
                    sqlParameters[1] = new SqlParameter("@TrnType", "");
                }
                else
                {
                    sqlParameters[1] = new SqlParameter("@TrnType", trnType);
                }
                sqlParameters[2] = new SqlParameter("@RecIss", '0');
                sqlParameters[3] = new SqlParameter("@PrdVou", productid);
                sqlParameters[4] = new SqlParameter("@VNo", vNo);
                sqlParameters[5] = new SqlParameter("@FrDt", frDt);
                sqlParameters[6] = new SqlParameter("@ToDt", toDt);
                sqlParameters[7] = new SqlParameter("@Stock", 0);
                sqlParameters[8] = new SqlParameter("@UserID", userId);
                sqlParameters[9] = new SqlParameter("@Thick", thick == null ? 0 : Convert.ToDecimal(thick));
                sqlParameters[10] = new SqlParameter("@Width", width == null ? 0 : Convert.ToDecimal(width));
                sqlParameters[11] = new SqlParameter("@Qty", qty == null ? 0 : Convert.ToDecimal(qty));
                sqlParameters[12] = new SqlParameter("@OD", od == null ? 0 : Convert.ToDecimal(od));
                sqlParameters[13] = new SqlParameter("@NB", nb == null ? 0 : Convert.ToDecimal(nb));
                sqlParameters[14] = new SqlParameter("@Sch", sch == null ? 0 : Convert.ToDecimal(sch));
                sqlParameters[15] = new SqlParameter("@GUID", guid);
                sqlParameters[16] = new SqlParameter("@Grade", grade);
                sqlParameters[17] = new SqlParameter("@Finish", finish);
                sqlParameters[18] = new SqlParameter("@Langth", langth);
                sqlParameters[19] = new SqlParameter("@Proc", proc);
                sqlParameters[20] = new SqlParameter("@CoilNo", coilno);
                DataTable DtStkLed = ObjDBConnection.CallStoreProcedure("GetMCoilStockLedgerDetails", sqlParameters);

                string whereConditionQuery = string.Empty;
                if (!string.IsNullOrWhiteSpace(companyid))
                {
                    whereConditionQuery += " AND StkLedger.StkCmpVou='" + companyId + "'";
                }
                if (!string.IsNullOrWhiteSpace(vNo))
                {
                    whereConditionQuery += " AND StkLedger.StkVNo='" + vNo + "'";
                }
                if (!string.IsNullOrWhiteSpace(guid))
                {
                    whereConditionQuery += " AND StkLedger.StkGUID='" + guid + "'";
                }
                getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, 0, "", 0, 1, whereConditionQuery);
                if (type == 1)
                {
                    var bytes = Excel(getReportDataModel, "M. Coil Stock Ledger Report", companyDetails.CmpName);
                    return File(
                        bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "M-CoilStockLedger.xlsx");
                }
                else
                {
                    //string address = string.Empty;
                    //address += companyDetails.CmpAdd == null ? "" :(companyDetails.CmpAdd + "," ) ;
                    //address +=  frDt != null ?"From Date : " + frDt  + "," : "";
                    //address += address + toDt != null ?"To Date : " + toDt  : "";

                    var bytes = PDF(getReportDataModel, "M.Coil Stock Ledger Report", companyDetails.CmpName, companyid);
                    return File(
                            bytes,
                            "application/pdf",
                            "M-CoilStockLedger.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
