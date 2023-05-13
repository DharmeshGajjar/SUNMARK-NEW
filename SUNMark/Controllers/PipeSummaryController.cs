using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SUNMark.Classes;
using SUNMark.Models;

namespace SUNMark.Controllers
{
    public class PipeSummaryController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        public IActionResult Index(long id)
        {
            CoilMasterModel coilMasterModel = new CoilMasterModel();
            try
            {
                bool isreturn = false;
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                long yearId = GetIntSession("YearId");
                int administrator = 0;
                //var yearData = DbConnection.GetYearListByCompanyId(Convert.ToInt32(companyId)).Where(x => x.YearVou == yearId).FirstOrDefault();
                //if (yearData != null)
                //{
                //    coilMasterModel.FrRecDt = yearData.StartDate;
                //    coilMasterModel.ToRecDt = yearData.EndDate;
                //}
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                coilMasterModel.GradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                coilMasterModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                ViewBag.processList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                //coilMasterModel.StockYNList = objProductHelper.GetStockYN();
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(coilMasterModel);
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

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby, string coilNo, string frWidth, string toWidth, string frthick, string tothick, string frWeigth, string toWeigth, string gradeid, string frRecDt, string toRecDt, string companyid,string DoneProc,string NextProc)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));

                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/PipeSummary/Index";
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

                //SqlParameter[] sqlParameters = new SqlParameter[1];
                //sqlParameters[0] = new SqlParameter("@SESSID", userId);
                //DataTable DtStkLed = ObjDBConnection.CallStoreProcedure("RPT_PIPEMASTER", sqlParameters);

                string whereConditionQuery = string.Empty;
                    if (!string.IsNullOrWhiteSpace(coilNo))
                    {
                        whereConditionQuery += " AND PipeMst.CoilNo='" + coilNo + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frWidth))
                    {
                        whereConditionQuery += " AND T1.Width>='" + frWidth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toWidth))
                    {
                        whereConditionQuery += " AND T1.Width<='" + toWidth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frthick))
                    {
                        whereConditionQuery += " AND T1.Thick>='" + frthick + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(tothick))
                    {
                        whereConditionQuery += " AND T1.Thick<='" + tothick + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frWeigth))
                    {
                        whereConditionQuery += " AND T1.Qty >='" + frWeigth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toWeigth))
                    {
                        whereConditionQuery += " AND T1.Qty <='" + toWeigth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(gradeid))
                    {
                        whereConditionQuery += " AND T1.GrdVou='" + gradeid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frRecDt))
                    {
                        whereConditionQuery += " AND T1.RecDt>='" + frRecDt + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toRecDt))
                    {
                        whereConditionQuery += " AND T1.RecDt<='" + toRecDt + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(DoneProc))
                    {
                        whereConditionQuery += " AND T1.DoneProc<='" + DoneProc + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(NextProc))
                    {
                        whereConditionQuery += " AND T1.NextProc<='" + NextProc + "'";
                    }
                
                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, 0, 0, "", 0, 0, whereConditionQuery);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "PipeSummary";
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }

        public IActionResult ExportToExcelPDF(int gridMstId, string searchValue, int type, string coilNo, string frWidth, string toWidth, string frthick, string tothick, string frWeigth, string toWeigth, string gradeid, string frRecDt, string toRecDt, string companyid)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                var companyDetails = DbConnection.GetCompanyDetailsById(companyId);

                string whereConditionQuery = string.Empty;
                if (gridMstId != 48)
                {
                    if (!string.IsNullOrWhiteSpace(coilNo))
                    {
                        whereConditionQuery += " AND LotMst.LotCoilNo='" + coilNo + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frWidth))
                    {
                        whereConditionQuery += " AND LotMst.LotWidth>='" + frWidth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toWidth))
                    {
                        whereConditionQuery += " AND LotMst.LotWidth<='" + toWidth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frthick))
                    {
                        whereConditionQuery += " AND LotMst.LotThick>='" + frthick + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(tothick))
                    {
                        whereConditionQuery += " AND LotMst.LotThick<='" + tothick + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frWeigth))
                    {
                        whereConditionQuery += " AND LotMst.LotWeigth>='" + frWeigth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toWeigth))
                    {
                        whereConditionQuery += " AND LotMst.LotWeigth<='" + toWeigth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(gradeid))
                    {
                        whereConditionQuery += " AND LotMst.LotGrdMscVou='" + gradeid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frRecDt))
                    {
                        whereConditionQuery += " AND LotMst.LotRecDt>='" + frRecDt + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toRecDt))
                    {
                        whereConditionQuery += " AND LotMst.LotRecDt<='" + toRecDt + "'";
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(coilNo))
                    {
                        whereConditionQuery += " AND PipeMst.CoilNo='" + coilNo + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frWidth))
                    {
                        whereConditionQuery += " AND PipeMst.Width>='" + frWidth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toWidth))
                    {
                        whereConditionQuery += " AND PipeMst.Width<='" + toWidth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frthick))
                    {
                        whereConditionQuery += " AND PipeMst.Thick>='" + frthick + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(tothick))
                    {
                        whereConditionQuery += " AND PipeMst.Thick<='" + tothick + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frWeigth))
                    {
                        whereConditionQuery += " AND PipeMst.Qty >='" + frWeigth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toWeigth) && toWeigth != "0")
                    {
                        whereConditionQuery += " AND PipeMst.Qty <='" + toWeigth + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(gradeid))
                    {
                        whereConditionQuery += " AND PipeMst.GrdVou='" + gradeid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frRecDt))
                    {
                        whereConditionQuery += " AND PipeMst.RecDt>='" + frRecDt + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toRecDt))
                    {
                        whereConditionQuery += " AND PipeMst.RecDt<='" + toRecDt + "'";
                    }
                }


                getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, 0, "", 0, 1, whereConditionQuery);
                if (type == 1)
                {
                    var bytes = Excel(getReportDataModel, "Pipe Register Report", companyDetails.CmpName);
                    return File(
                        bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "PipeRegister.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Pipe Register Report", companyDetails.CmpName, "");
                    return File(
                            bytes,
                            "application/pdf",
                            "PipeRegister.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
