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
    public class InwRegiController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        //Chirag Changes

        public IActionResult Index(long id)
        {
            InwRegiModel inwRegiModel = new InwRegiModel();
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
                    inwRegiModel.FrRecDt = yearData.StartDate;
                    inwRegiModel.ToRecDt = yearData.EndDate;
                }
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                inwRegiModel.GradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                inwRegiModel.GodownList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwRegiModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                inwRegiModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                inwRegiModel.StockYNList = objProductHelper.GetStockYN();
                inwRegiModel.CoilTypeList = objProductHelper.GetMainCoilType();
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(inwRegiModel);
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

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby, string coilNo, string frWidth, string toWidth, string frthick, string tothick, string frWeigth, string toWeigth, string gradeid, string frRecDt, string toRecDt, string frIssDt, string toIssDt, string stockYNid, string godownid, string companyid, string accountid, string coiltype, string supcoilno)
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

                //SqlParameter[] sqlParameters = new SqlParameter[1];
                //sqlParameters[0] = new SqlParameter("@SESSID", userId);
                //DataTable DtStkLed = ObjDBConnection.CallStoreProcedure("RPT_INWARDREGISTER", sqlParameters);

                string whereConditionQuery = string.Empty;
                if (!string.IsNullOrWhiteSpace(coilNo))
                {
                    whereConditionQuery += " AND LotMst.LotCoilNo='" + coilNo + "'";
                }
                if (!string.IsNullOrWhiteSpace(gradeid))
                {
                    whereConditionQuery += " AND LotMst.LotGrdMscVou='" + gradeid + "'";
                }
                if (!string.IsNullOrWhiteSpace(frRecDt))
                {
                    whereConditionQuery += " AND LotMst.LotDt>='" + frRecDt + "'";
                }
                if (!string.IsNullOrWhiteSpace(toRecDt))
                {
                    whereConditionQuery += " AND LotMst.LotDt<='" + toRecDt + "'";
                }
                if (!string.IsNullOrWhiteSpace(companyid))
                {
                    whereConditionQuery += " AND LotMst.LotCmpVou='" + companyId + "'";
                }
                if (!string.IsNullOrWhiteSpace(accountid))
                {
                    whereConditionQuery += " AND LotMst.LotAccVou='" + accountid + "'";
                }
                if (!string.IsNullOrWhiteSpace(supcoilno))
                {
                    whereConditionQuery += " AND LotMst.LotSupCoilNo='" + supcoilno + "'";
                }

                //if (!string.IsNullOrWhiteSpace(frWidth))
                //{
                //    whereConditionQuery += " AND LotMst.LotWidth>='" + frWidth + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(toWidth))
                //{
                //    whereConditionQuery += " AND LotMst.LotWidth<='" + toWidth + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(frthick))
                //{
                //    whereConditionQuery += " AND LotMst.LotThick>='" + frthick + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(tothick))
                //{
                //    whereConditionQuery += " AND LotMst.LotThick<='" + tothick + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(frWeigth))
                //{
                //    whereConditionQuery += " AND LotMst.LotQty >='" + frWeigth + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(toWeigth))
                //{
                //    whereConditionQuery += " AND LotMst.LotQty <='" + toWeigth + "'";
                //}

                //if (!string.IsNullOrWhiteSpace(frIssDt))
                //{
                //    whereConditionQuery += " AND LotMst.LotIssDt>='" + frIssDt + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(toIssDt))
                //{

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
                    whereConditionQuery += " AND LotMst.LotQty >='" + frWeigth + "'";
                }
                if (!string.IsNullOrWhiteSpace(toWeigth))
                {
                    whereConditionQuery += " AND LotMst.LotQty <='" + toWeigth + "'";
                }
                if (!string.IsNullOrWhiteSpace(gradeid))
                {
                    whereConditionQuery += " AND LotMst.LotGrdMscVou='" + gradeid + "'";
                }
                if (!string.IsNullOrWhiteSpace(frRecDt))
                {
                    whereConditionQuery += " AND LotMst.LotDt>='" + frRecDt + "'";
                }
                if (!string.IsNullOrWhiteSpace(toRecDt))
                {
                    whereConditionQuery += " AND LotMst.LotDt<='" + toRecDt + "'";
                }
                //if (!string.IsNullOrWhiteSpace(frIssDt))
                //{
                //    whereConditionQuery += " AND LotMst.LotIssDt>='" + frIssDt + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(toIssDt))
                //{
                //    whereConditionQuery += " AND LotMst.LotIssDt<='" + toIssDt + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(stockYNid))
                //{
                //    if (stockYNid == "1")
                //    {
                //        whereConditionQuery += " AND isnull(LotMst.LotIssDt,'') = ''";
                //    }
                //    else if (stockYNid == "2")
                //    {
                //        whereConditionQuery += " AND isnull(LotMst.LotIssDt,'') <> ''";
                //    }
                //    else if (stockYNid == "3")
                //    {
                //        whereConditionQuery += " AND LotMst.LotComYN = '1'";
                //    }
                //}
                //if (!string.IsNullOrWhiteSpace(godownid))
                //{
                //    whereConditionQuery += " AND LotMst.LotGdnVou='" + godownid + "'";
                //}

                //if (!string.IsNullOrWhiteSpace(coiltype))
                //{
                //    whereConditionQuery += " AND LotMst.LotCoilType='" + coiltype + "'";
                //}
                if (!string.IsNullOrWhiteSpace(godownid))
                {
                    whereConditionQuery += " AND InwTrn.IntGdnVou='" + godownid + "'";
                }
                if (!string.IsNullOrWhiteSpace(companyid))
                {
                    //whereConditionQuery += " AND InwMst.InwCmpVou='" + companyId + "'";
                    whereConditionQuery += " AND InwMst.InwCmpVou= (select top 1 DepVou from DepartmentMst where DepCmpCdN=" + companyId + ")";
                }
                if (!string.IsNullOrWhiteSpace(accountid))
                {
                    whereConditionQuery += " AND InwMst.InwAccVou='" + accountid + "'";
                }
                if (!string.IsNullOrWhiteSpace(coiltype))
                {
                    whereConditionQuery += " AND InwTrn.IntCoilType='" + coiltype + "'";
                }
                if (!string.IsNullOrWhiteSpace(supcoilno))
                {
                    whereConditionQuery += " AND InwTrn.IntSupCoilNo='" + supcoilno + "'";
                }

                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, 0, 0, "", 0, 0, whereConditionQuery);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "InwRegi";
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }

        public IActionResult ExportToExcelPDF(int gridMstId, string searchValue, int type, string coilNo, string frWidth, string toWidth, string frthick, string tothick, string frWeigth, string toWeigth, string gradeid, string frRecDt, string toRecDt, string frIssDt, string toIssDt, string stockYNid, string godownid, string companyid, string accountid, string coiltype, string supcoilno)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                int userId = Convert.ToInt32(GetIntSession("UserId"));
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                var companyDetails = DbConnection.GetCompanyDetailsById(companyId);

                //SqlParameter[] sqlParameters = new SqlParameter[1];
                //sqlParameters[0] = new SqlParameter("@SESSID", userId);
                //DataTable DtStkLed = ObjDBConnection.CallStoreProcedure("RPT_INWARDREGISTER", sqlParameters);

                string whereConditionQuery = string.Empty;
                //DataTable DtStkLed = ObjDBConnection.CallStoreProcedure("RPT_COILMASTER", sqlParameters);

                //if (gridMstId != 35)
                //{
                if (!string.IsNullOrWhiteSpace(coilNo))
                {
                    whereConditionQuery += " AND LotMst.LotCoilNo='" + coilNo + "'";
                }
                if (!string.IsNullOrWhiteSpace(gradeid))
                {
                    whereConditionQuery += " AND LotMst.LotGrdMscVou='" + gradeid + "'";
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
                //if (!string.IsNullOrWhiteSpace(frWeigth))
                //{
                //    whereConditionQuery += " AND LotMst.LotWeigth>='" + frWeigth + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(toWeigth))
                //{
                //    whereConditionQuery += " AND LotMst.LotWeigth<='" + toWeigth + "'";
                //}
                if (!string.IsNullOrWhiteSpace(gradeid))
                {
                    whereConditionQuery += " AND LotMst.LotGrdMscVou='" + gradeid + "'";
                }
                if (!string.IsNullOrWhiteSpace(frRecDt))
                {
                    whereConditionQuery += " AND InwMst.InwDt>='" + frRecDt + "'";
                }
                if (!string.IsNullOrWhiteSpace(toRecDt))
                {
                    whereConditionQuery += " AND InwMst.InwDt<='" + toRecDt + "'";
                }
                //if (!string.IsNullOrWhiteSpace(frIssDt))
                //{
                //    whereConditionQuery += " AND LotMst.LotIssDt>='" + frIssDt + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(toIssDt))
                //{
                //    whereConditionQuery += " AND LotMst.LotIssDt<='" + toIssDt + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(stockYNid))
                //{
                //    whereConditionQuery += " AND LotMst.LotWeight='" + stockYNid + "'";
                //}
                if (!string.IsNullOrWhiteSpace(godownid))
                {
                    whereConditionQuery += " AND InwTrn.IntGdnVou='" + godownid + "'";
                }
                if (!string.IsNullOrWhiteSpace(companyid))
                {
                    //whereConditionQuery += " AND InwMst.InwCmpVou='" + companyid + "'";
                    whereConditionQuery += " AND InwMst.InwCmpVou= (select top 1 DepVou from DepartmentMst where DepCmpCdN=" + companyId + ")";
                }
                if (!string.IsNullOrWhiteSpace(accountid))
                {
                    whereConditionQuery += " AND InwMst.InwAccVou='" + accountid + "'";
                }
                if (!string.IsNullOrWhiteSpace(coiltype))
                {
                    whereConditionQuery += " AND InwTrn.IntCoilType='" + coiltype + "'";
                }
                if (!string.IsNullOrWhiteSpace(supcoilno))
                {
                    whereConditionQuery += " AND InwTrn.IntSupCoilNo='" + supcoilno + "'";
                }
                if (!string.IsNullOrWhiteSpace(frRecDt))
                {
                    whereConditionQuery += " AND LotMst.LotDt>='" + frRecDt + "'";
                }
                if (!string.IsNullOrWhiteSpace(toRecDt))
                {
                    whereConditionQuery += " AND LotMst.LotDt<='" + toRecDt + "'";
                }
                if (!string.IsNullOrWhiteSpace(companyid))
                {
                    whereConditionQuery += " AND LotMst.LotCmpVou='" + companyId + "'";
                }
                if (!string.IsNullOrWhiteSpace(accountid))
                {
                    whereConditionQuery += " AND LotMst.LotAccVou='" + accountid + "'";
                }
                if (!string.IsNullOrWhiteSpace(supcoilno))
                {
                    whereConditionQuery += " AND LotMst.LotSupCoilNo='" + supcoilno + "'";
                }

                //if (!string.IsNullOrWhiteSpace(frWidth))
                //{
                //    whereConditionQuery += " AND LotMst.LotWidth>='" + frWidth + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(toWidth))
                //{
                //    whereConditionQuery += " AND LotMst.LotWidth<='" + toWidth + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(frthick))
                //{
                //    whereConditionQuery += " AND LotMst.LotThick>='" + frthick + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(tothick))
                //{
                //    whereConditionQuery += " AND LotMst.LotThick<='" + tothick + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(frWeigth))
                //{
                //    whereConditionQuery += " AND LotMst.LotQty >='" + frWeigth + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(toWeigth))
                //{
                //    whereConditionQuery += " AND LotMst.LotQty <='" + toWeigth + "'";
                //}

                //if (!string.IsNullOrWhiteSpace(frIssDt))
                //{
                //    whereConditionQuery += " AND LotMst.LotIssDt>='" + frIssDt + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(toIssDt))
                //{
                //    whereConditionQuery += " AND LotMst.LotIssDt<='" + toIssDt + "'";
                //}
                //if (!string.IsNullOrWhiteSpace(stockYNid))
                //{
                //    if (stockYNid == "1")
                //    {
                //        whereConditionQuery += " AND isnull(LotMst.LotIssDt,'') = ''";
                //    }
                //    else if (stockYNid == "2")
                //    {
                //        whereConditionQuery += " AND isnull(LotMst.LotIssDt,'') <> ''";
                //    }
                //    else if (stockYNid == "3")
                //    {
                //        whereConditionQuery += " AND LotMst.LotComYN = '1'";
                //    }
                //}
                //if (!string.IsNullOrWhiteSpace(godownid))
                //{
                //    whereConditionQuery += " AND LotMst.LotGdnVou='" + godownid + "'";
                //}

                //if (!string.IsNullOrWhiteSpace(coiltype))
                //{
                //    whereConditionQuery += " AND LotMst.LotCoilType='" + coiltype + "'";
                //}

                getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, userId, 0, "", 0, 1, whereConditionQuery);
                if (type == 1)
                {
                    var bytes = Excel(getReportDataModel, "Inward Register Report", companyDetails.CmpName);
                    return File(
                        bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "InwardRegister.xlsx");
                }
                else
                {
                    //string address = string.Empty;
                    //address += companyDetails.CmpAdd == null ? "" : (companyDetails.CmpAdd + ",");
                    //address += frRecDt != null ? "From Date : " + frRecDt + "," : "";
                    //address += address + toRecDt != null ? "To Date : " + toRecDt : "";

                    var bytes = PDF(getReportDataModel, "Inward Register Report", companyDetails.CmpName, companyId.ToString());
                    return File(
                            bytes,
                            "application/pdf",
                            "InwardRegister.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
