using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using SUNMark.Classes;
using SUNMark.Models;

namespace SUNMark.Controllers
{
    public class CoilJobRegiController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        public IActionResult Index()
        {
            CoilJobRegiModel coilJob = new CoilJobRegiModel();
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
                    coilJob.FrDt = yearData.StartDate;
                    coilJob.ToDt = yearData.EndDate;
                }
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                coilJob.GradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                coilJob.PrTypeList = objProductHelper.GetPrTypeMasterDropdown(companyId, administrator);

                //INIT(ref isreturn);
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(coilJob);

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

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby, string frDt, string toDt, string gradeid, string prType)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));

                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/InwRegi/Index";
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
                    whereConditionQuery += " AND JobMst.JobComDt>='" + frDt + "'";
                if (!string.IsNullOrWhiteSpace(toDt))
                    whereConditionQuery += " AND JobMst.JobComDt<='" + toDt + "'";
                if (!string.IsNullOrWhiteSpace(gradeid))
                    whereConditionQuery += " AND JotLotVou = (select LotVou from LotMst where LotMst.LotGrdMscVou='" + gradeid + "' AND LotVou=JotLotVou) ";
                if (!string.IsNullOrWhiteSpace(prType))
                    whereConditionQuery += " AND JobTrn.JotType='" + prType + "'";

                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, 0, 0, "", 0, 0, whereConditionQuery);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "CoilJobRegi";
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }

        public IActionResult ExportToExcelPDF(int gridMstId, string searchValue, int type, string companyid)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                int userId = Convert.ToInt32(GetIntSession("UserId"));
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                var companyDetails = DbConnection.GetCompanyDetailsById(companyId);

                getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, userId, 0, "", 0, 1, "");
                if (type == 1)
                {
                    var bytes = Excel(getReportDataModel, "Coil Job Register Report", companyDetails.CmpName);
                    return File(
                        bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "CoilJobRegister.xlsx");
                }
                else
                {
                    //string address = string.Empty;
                    //address += companyDetails.CmpAdd == null ? "" : (companyDetails.CmpAdd + ",");
                    //address += frRecDt != null ? "From Date : " + frRecDt + "," : "";
                    //address += address + toRecDt != null ? "To Date : " + toRecDt : "";

                    var bytes = PDF(getReportDataModel, "Coil Job Register Report", companyDetails.CmpName, companyId.ToString());
                    return File(
                            bytes,
                            "application/pdf",
                            "CoilJobRegister.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}