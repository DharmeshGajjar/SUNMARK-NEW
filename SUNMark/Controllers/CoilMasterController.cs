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
    public class CoilMasterController : BaseController
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
                coilMasterModel.GodownList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                coilMasterModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                coilMasterModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                coilMasterModel.StockYNList = objProductHelper.GetStockYN();
                coilMasterModel.CoilTypeList = objProductHelper.GetMainCoilType();
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

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby, string coilNo, string frWidth, string toWidth, string frthick, string tothick, string frWeigth, string toWeigth, string gradeid, string frRecDt, string toRecDt, string frIssDt, string toIssDt, string stockYNid, string godownid, string companyid, string accountid, string coiltype, string supcoilno)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                string guid = GetStringSession("LoginGUID");
                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/CoilMaster/Index";
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

                SqlParameter[] sqlParameters = new SqlParameter[14];
                sqlParameters[0] = new SqlParameter("@SESSID", guid);
                sqlParameters[1] = new SqlParameter("@coilNumber", coilNo);
                sqlParameters[2] = new SqlParameter("@frWidth", frWidth);
                sqlParameters[3] = new SqlParameter("@toWidth", toWidth);
                sqlParameters[4] = new SqlParameter("@frthick", frthick);
                sqlParameters[5] = new SqlParameter("@tothick", tothick);
                sqlParameters[6] = new SqlParameter("@frWeigth", frWeigth);
                sqlParameters[7] = new SqlParameter("@toWeigth", toWeigth);
                sqlParameters[8] = new SqlParameter("@gradeid", gradeid);
                sqlParameters[9] = new SqlParameter("@frRecDt", frRecDt);
                sqlParameters[10] = new SqlParameter("@toRecDt", toRecDt);
                sqlParameters[11] = new SqlParameter("@godownid", godownid);
                sqlParameters[12] = new SqlParameter("@coiltyp", coiltype);
                sqlParameters[13] = new SqlParameter("@supcoil", supcoilno);

                DataTable DtStkLed = ObjDBConnection.CallStoreProcedure("RPT_COILMASTER_NEW", sqlParameters);

                string whereConditionQuery = string.Empty;
                if (gridMstId != 35)
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
                    if (!string.IsNullOrWhiteSpace(godownid))
                    {
                        whereConditionQuery += " AND LotMst.LotGdnVou='" + godownid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(companyid))
                    {
                        whereConditionQuery += " AND LotMst.LotCmpVou='" + companyId + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(accountid))
                    {
                        whereConditionQuery += " AND LotMst.LotAccVou='" + accountid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(coiltype))
                    {
                        whereConditionQuery += " AND LotMst.LotCoilType='" + coiltype + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(supcoilno))
                    {
                        whereConditionQuery += " AND LotMst.LotSupCoilNo='" + supcoilno + "'";
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(frIssDt))
                    {
                        whereConditionQuery += " AND CoilMst.IssDt>='" + frIssDt + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toIssDt))
                    {
                        whereConditionQuery += " AND CoilMst.IssDt<='" + toIssDt + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(stockYNid))
                    {
                        if (stockYNid == "1")
                        {
                            whereConditionQuery += " AND CoilMst.Process = 'YES'";
                        }
                        else if (stockYNid == "2")
                        {
                            whereConditionQuery += " AND CoilMst.Process = 'NO'";
                        }
                        else if (stockYNid == "3")
                        {
                            whereConditionQuery += " AND CoilMst.Process = 'INP'";
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(companyid))
                    {
                        whereConditionQuery += " AND CoilMst.CmpVou='" + companyid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(godownid))
                    {
                        whereConditionQuery += " AND CoilMst.Godown In (Select GdnNm From GdnMst Where GdnVou ='" + godownid + "')";
                    }
                    if (!string.IsNullOrWhiteSpace(coiltype))
                    {
                        whereConditionQuery += " AND CoilMst.CoilType ='" + coiltype + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(accountid))
                    {
                        whereConditionQuery += " AND CoilMst.AccVou='" + accountid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(coilNo))
                    {
                        whereConditionQuery += " AND CoilMst.CoilNo='" + coilNo + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(guid))
                    {
                        whereConditionQuery += " AND CoilMst.SessID='" + guid + "'";
                    }
                }
                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, 0, 0, "", 0, 0, whereConditionQuery);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "CoilMaster";
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
                string guid = GetStringSession("LoginGUID");
                //SqlParameter[] sqlParameters = new SqlParameter[1];
                //sqlParameters[0] = new SqlParameter("@SESSID", userId);
                //DataTable DtStkLed = ObjDBConnection.CallStoreProcedure("RPT_COILMASTER", sqlParameters);

                string whereConditionQuery = string.Empty;
                if (gridMstId != 35)
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
                    if (!string.IsNullOrWhiteSpace(frIssDt))
                    {
                        whereConditionQuery += " AND LotMst.LotIssDt>='" + frIssDt + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toIssDt))
                    {
                        whereConditionQuery += " AND LotMst.LotIssDt<='" + toIssDt + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(stockYNid))
                    {
                        whereConditionQuery += " AND LotMst.LotWeight='" + stockYNid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(godownid))
                    {
                        whereConditionQuery += " AND LotMst.LotGdnVou='" + godownid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(companyid))
                    {
                        whereConditionQuery += " AND LotMst.LotCmpVou='" + companyid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(accountid))
                    {
                        whereConditionQuery += " AND LotMst.LotAccVou='" + accountid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(coiltype))
                    {
                        whereConditionQuery += " AND LotMst.LotCoilType='" + coiltype + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(supcoilno))
                    {
                        whereConditionQuery += " AND LotMst.LotSupCoilNo='" + supcoilno + "'";
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(frIssDt))
                    {
                        whereConditionQuery += " AND CoilMst.IssDt>='" + frIssDt + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toIssDt))
                    {
                        whereConditionQuery += " AND CoilMst.IssDt<='" + toIssDt + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(stockYNid))
                    {
                        if (stockYNid == "1")
                        {
                            whereConditionQuery += " AND CoilMst.Process = 'YES'";
                        }
                        else if (stockYNid == "2")
                        {
                            whereConditionQuery += " AND CoilMst.Process = 'NO'";
                        }
                        else if (stockYNid == "3")
                        {
                            whereConditionQuery += " AND CoilMst.Process = 'INP'";
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(companyid))
                    {
                        whereConditionQuery += " AND CoilMst.CmpVou='" + companyid + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(godownid))
                    {
                        whereConditionQuery += " AND CoilMst.Godown In (Select GdnNm From GdnMst Where GdnVou ='" + godownid + "')";
                    }
                    if (!string.IsNullOrWhiteSpace(coiltype))
                    {
                        whereConditionQuery += " AND CoilMst.CoilType ='" + coiltype + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(accountid))
                    {
                        whereConditionQuery += " AND CoilMst.AccVou='" + accountid + "'";
                    }
                    //if (!string.IsNullOrWhiteSpace(coiltype))
                    //{
                    //    whereConditionQuery += " AND CoilMst.CoilType='" + coiltype + "'";
                    //}
                    if (!string.IsNullOrWhiteSpace(guid))
                    {
                        whereConditionQuery += " AND CoilMst.SessID='" + guid + "'";
                    }
                }


                getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, userId, 0, "", 0, 1,whereConditionQuery);
                if (type == 1)
                {
                    var bytes = Excel(getReportDataModel, "Coil Register Report", companyDetails.CmpName);
                    return File(
                        bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "CoilRegister.xlsx");
                }
                else
                {
                    //string address = string.Empty;
                    //address += companyDetails.CmpAdd == null ? "" : (companyDetails.CmpAdd + ",");
                    //address += frRecDt != null ? "From Date : " + frRecDt + "," : "";
                    //address += address + toRecDt != null ? "To Date : " + toRecDt : "";

                    var bytes = PDF(getReportDataModel, "Coil Register Report", companyDetails.CmpName, companyid);
                    return File(
                            bytes,
                            "application/pdf",
                            "CoilRegister.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
