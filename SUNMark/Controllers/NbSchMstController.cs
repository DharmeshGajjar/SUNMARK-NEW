﻿using Microsoft.AspNetCore.Mvc;
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
    public class NbSchMstController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        public IActionResult Index()
        {
            try
            {
                bool isreturn = false;
                long companyId = GetIntSession("CompanyId");
                long userId = GetIntSession("UserId");
                long yearId = GetIntSession("YearId");
                var yearData = DbConnection.GetYearListByCompanyId(Convert.ToInt32(companyId)).Where(x => x.YearVou == yearId).FirstOrDefault();
                //if (yearData != null)
                //{
                //    biltyReportModel.BilFrDate = yearData.StartDate;
                //    biltyReportModel.BilToDate = yearData.EndDate;
                //}
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

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
                ViewBag.layoutList = GetGridLayoutDropDown(DbConnection.GridTypeReport, userFormRights.ModuleId);
                ViewBag.pageNoList = GetPageNo();
            }

            #endregion

        }

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));

                SqlParameter[] sqlParameters = new SqlParameter[0];
                DataTable DtNBSCH = ObjDBConnection.CallStoreProcedure("RPT_NBSCH", sqlParameters);


                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/NbSchMst/Index";
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
                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, 0, 0, "", 0, 0, whereConditionQuery);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "NbSchMst";
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }

        public IActionResult ExportToExcelPDF(int gridMstId, string searchValue, int type, string toDate, string accountId, string toaccountId, string fromDate, string frcityId, string tocityId, string vehicleId, string departmentId, string bilmodeid)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                var companyDetails = DbConnection.GetCompanyDetailsById(companyId);

                string whereConditionQuery = string.Empty;
                getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, 0, "", 0, 1, whereConditionQuery);
                if (type == 1)
                {
                    var bytes = Excel(getReportDataModel, "NBSCH Master Report", companyDetails.CmpName);
                    return File(
                        bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "NBSCH.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "NBSCH Master Report", companyDetails.CmpName, "");
                    return File(
                            bytes,
                            "application/pdf",
                            "NBSCH.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
