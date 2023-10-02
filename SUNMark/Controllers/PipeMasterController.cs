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
    public class PipeMasterController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
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
                var yearData = DbConnection.GetYearListByCompanyId(Convert.ToInt32(companyId)).Where(x => x.YearVou == yearId).FirstOrDefault();
                if (yearData != null)
                {
                    coilMasterModel.FrRecDt = yearData.StartDate;
                    coilMasterModel.ToRecDt = yearData.EndDate;
                }
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                coilMasterModel.GradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                coilMasterModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                ViewBag.processList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                coilMasterModel.StockYNList = objProductHelper.GetStockYNNew();
                coilMasterModel.NBList = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
                coilMasterModel.SCHList = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);
                coilMasterModel.FinishList = objProductHelper.GetFinishMasterDropdown(companyId,0);
                coilMasterModel.StockYNVou = 1;
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

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby, string od, string feetper, string nb, string sch, string gradeid, string frRecDt, string toRecDt, string companyid, string DoneProc, string NextProc, string stockYN, string finishId)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));

                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/PipeMaster/Index";
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

                SqlParameter[] sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("@SESSID", userId);
                sqlParameters[1] = new SqlParameter("@StockYN", stockYN);
                sqlParameters[2] = new SqlParameter("@FromDt", frRecDt);
                sqlParameters[3] = new SqlParameter("@ToDt", toRecDt);
                DataTable DtStkLed = ObjDBConnection.CallStoreProcedure("RPT_PIPEMASTER_NEW", sqlParameters);

                string whereConditionQuery = string.Empty;
                //if (gridMstId != 48)
                //{
                //    if (!string.IsNullOrWhiteSpace(coilNo))
                //    {
                //        whereConditionQuery += " AND LotMst.LotCoilNo='" + coilNo + "'";
                //    }
                //    if (!string.IsNullOrWhiteSpace(frWidth))
                //    {
                //        whereConditionQuery += " AND LotMst.LotWidth>='" + frWidth + "'";
                //    }
                //    if (!string.IsNullOrWhiteSpace(toWidth))
                //    {
                //        whereConditionQuery += " AND LotMst.LotWidth<='" + toWidth + "'";
                //    }
                //    if (!string.IsNullOrWhiteSpace(frthick))
                //    {
                //        whereConditionQuery += " AND LotMst.LotThick>='" + frthick + "'";
                //    }
                //    if (!string.IsNullOrWhiteSpace(tothick))
                //    {
                //        whereConditionQuery += " AND LotMst.LotThick<='" + tothick + "'";
                //    }
                //    if (!string.IsNullOrWhiteSpace(frWeigth))
                //    {
                //        whereConditionQuery += " AND LotMst.LotQty >='" + frWeigth + "'";
                //    }
                //    if (!string.IsNullOrWhiteSpace(toWeigth))
                //    {
                //        whereConditionQuery += " AND LotMst.LotQty <='" + toWeigth + "'";
                //    }
                //    if (!string.IsNullOrWhiteSpace(gradeid))
                //    {
                //        whereConditionQuery += " AND LotMst.LotGrdMscVou='" + gradeid + "'";
                //    }
                //    if (!string.IsNullOrWhiteSpace(frRecDt))
                //    {
                //        whereConditionQuery += " AND LotMst.LotDt>='" + frRecDt + "'";
                //    }
                //    if (!string.IsNullOrWhiteSpace(toRecDt))
                //    {
                //        whereConditionQuery += " AND LotMst.LotDt<='" + toRecDt + "'";
                //    }
                //    //if (!string.IsNullOrWhiteSpace(DoneProc))
                //    //{
                //    //    whereConditionQuery += " AND LotMst.LotDt ='" + DoneProc + "'";
                //    //}
                //    //if (!string.IsNullOrWhiteSpace(NextProc))
                //    //{
                //    //    whereConditionQuery += " AND LotMst.LotDt<='" + NextProc + "'";
                //    //}
                //}
                //else
                //{
                if (!string.IsNullOrWhiteSpace(od))
                {
                    whereConditionQuery += " AND PipeMst.OD='" + od + "'";
                }
                if (!string.IsNullOrWhiteSpace(feetper))
                {
                    whereConditionQuery += " AND PipeMst.FeetPer='" + feetper + "'";
                }
                if (!string.IsNullOrWhiteSpace(nb))
                {
                    whereConditionQuery += " AND PipeMst.NB='" + nb + "'";
                }
                if (!string.IsNullOrWhiteSpace(sch))
                {
                    whereConditionQuery += " AND PipeMst.SCH='" + sch + "'";
                }
                if (!string.IsNullOrWhiteSpace(gradeid))
                {
                    whereConditionQuery += " AND PipeMst.GrdVou='" + gradeid + "'";
                }
                if (!string.IsNullOrWhiteSpace(DoneProc))
                {
                    whereConditionQuery += " AND PipeMst.DoneProc<='" + DoneProc + "'";
                }
                if (!string.IsNullOrWhiteSpace(NextProc))
                {
                    whereConditionQuery += " AND PipeMst.NextProc='" + NextProc + "'";
                }
                if (!string.IsNullOrWhiteSpace(finishId))
                {
                    whereConditionQuery += " AND PipeMst.NextProc='" + NextProc + "'";
                }
                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, 0, 0, "", 0, 0, whereConditionQuery);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "PipeMaster";
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }

        public IActionResult ExportToExcelPDF(int gridMstId, string searchValue, int type, string od, string feetper, string nb, string sch, string gradeid, string frRecDt, string toRecDt, string companyid, string finishId)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                int userId = Convert.ToInt32(GetIntSession("UserId"));
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                var companyDetails = DbConnection.GetCompanyDetailsById(companyId);

                string whereConditionQuery = string.Empty;
                if (gridMstId != 48)
                {
                    if (!string.IsNullOrWhiteSpace(od))
                    {
                        whereConditionQuery += " AND LotMst.LotOD='" + od + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(feetper))
                    {
                        whereConditionQuery += " AND LotMst.LotFeetPer='" + feetper + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(nb))
                    {
                        whereConditionQuery += " AND LotMst.LotNB='" + nb + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(sch))
                    {
                        whereConditionQuery += " AND LotMst.LotSCH='" + sch + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(gradeid))
                    {
                        whereConditionQuery += " AND LotMst.LotGrdMscVou='" + gradeid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(finishId))
                    {
                        whereConditionQuery += " AND LotMst.LotFinMscVou='" + finishId + "'";
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
                    if (!string.IsNullOrWhiteSpace(od))
                    {
                        whereConditionQuery += " AND PipeMst.OD='" + od + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(feetper))
                    {
                        whereConditionQuery += " AND PipeMst.FeetPer='" + feetper + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(nb))
                    {
                        whereConditionQuery += " AND PipeMst.NB='" + nb + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(sch))
                    {
                        whereConditionQuery += " AND PipeMst.SCH='" + sch + "'";
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
                    if (!string.IsNullOrWhiteSpace(finishId))
                    {
                        whereConditionQuery += " AND PipeMst.FinishVou='" + finishId + "'";
                    }
                }


                getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, userId, 0, "", 0, 1, whereConditionQuery);
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
                    var bytes = PDF(getReportDataModel, "Pipe Register Report", companyDetails.CmpName, Convert.ToInt32(companyId).ToString());
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
