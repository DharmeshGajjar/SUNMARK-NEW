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
    public class MillingViewController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();

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

                stockLedgerModel.ShiftList = objProductHelper.GetShiftNew();
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

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby, string frDt, string toDt, string shiftid)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                string guid = GetStringSession("LoginGUID");
                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/MillingView/Index";
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

                string whereConditionQuery = string.Empty;
                if (!string.IsNullOrWhiteSpace(frDt))
                {
                    whereConditionQuery += " AND MilMst.MilDt>='" + frDt + "'";
                }
                if (!string.IsNullOrWhiteSpace(toDt))
                {
                    whereConditionQuery += " AND MilMst.MilDt<='" + toDt + "'";
                }
                if (!string.IsNullOrWhiteSpace(shiftid))
                {
                    whereConditionQuery += " AND MilMst.MilShift='" + shiftid + "'";
                }

                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, 0, 0, "", 0, 0, whereConditionQuery);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "MillingView";
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }

        public IActionResult ExportToExcelPDF(int gridMstId, string searchValue, int type, string frDt, string toDt, string shiftId)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                long userId = GetIntSession("UserId");
                int companyid = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                string guid = GetStringSession("LoginGUID");
                var companyDetails = DbConnection.GetCompanyDetailsById(companyid);

                string whereConditionQuery = string.Empty;
                if (!string.IsNullOrWhiteSpace(frDt))
                {
                    whereConditionQuery += " AND MilMst.MilDt>='" + frDt + "'";
                }
                if (!string.IsNullOrWhiteSpace(toDt))
                {
                    whereConditionQuery += " AND MilMst.MilDt<='" + toDt + "'";
                }
                if (!string.IsNullOrWhiteSpace(shiftId))
                {
                    whereConditionQuery += " AND MilMst.MilShift='" + shiftId + "'";
                }

                getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyid, 0, 0, "", 0, 1, whereConditionQuery);
                if (type == 1)
                {
                    var bytes = Excel(getReportDataModel, "Milling View Report", companyDetails.CmpName);
                    return File(
                        bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "MillView.xlsx");
                }
                else
                {
                    //string address = string.Empty;
                    //address += companyDetails.CmpAdd == null ? "" :(companyDetails.CmpAdd + "," ) ;
                    //address +=  frDt != null ?"From Date : " + frDt  + "," : "";
                    //address += address + toDt != null ?"To Date : " + toDt  : "";

                    var bytes = PDF(getReportDataModel, "Milling View Report", companyDetails.CmpName, "");
                    return File(
                            bytes,
                            "application/pdf",
                            "MillView.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
