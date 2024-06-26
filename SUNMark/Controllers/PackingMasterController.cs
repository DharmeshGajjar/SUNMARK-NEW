﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class PackingMasterController : BaseController
    {

        DbConnection ObjDBConnection = new DbConnection();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        ProductHelpers objProductHelper = new ProductHelpers();

        public IActionResult Index(int id, bool IsAdded = false)
        {
            PackingMasterModel packingMasterModel = new PackingMasterModel();
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                packingMasterModel.BundleNo = GetBundleNo();
                packingMasterModel.SrNo = GetSrNo();
                SqlParameter[] parameter = new SqlParameter[1];
                parameter[0] = new SqlParameter("@PkgVou", id);
                DataTable dt = ObjDBConnection.CallStoreProcedure("GetPackingNoById", parameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    packingMasterModel.Vou = id;
                    packingMasterModel.CompanyVou = Convert.ToInt32(dt.Rows[0]["PkgCmpVou"].ToString());
                    packingMasterModel.Type = dt.Rows[0]["PkgType"].ToString();
                    if (id > 0)
                    {
                        packingMasterModel.BundleNo = dt.Rows[0]["PkgBndNo"].ToString();
                        packingMasterModel.SrNo = dt.Rows[0]["PkgVou"].ToString();
                        packingMasterModel.QtyInKg = dt.Rows[0]["PkgQty"].ToString();
                    }


                    packingMasterModel.WONumber = dt.Rows[0]["PkgWoNo"].ToString();
                    packingMasterModel.PONumber = dt.Rows[0]["PkgPoNo"].ToString();
                    packingMasterModel.ProductVou = dt.Rows[0]["PkgPrdVou"].ToString();
                    packingMasterModel.ProductCode = dt.Rows[0]["PkgPrdCd"].ToString();
                    packingMasterModel.SpecificationVou = dt.Rows[0]["PkgSpeMscVou"].ToString();
                    packingMasterModel.FinishVou = dt.Rows[0]["PkgFinMscVou"].ToString();
                    packingMasterModel.Finish = dt.Rows[0]["PkgFinish"].ToString();
                    packingMasterModel.GradeVou = dt.Rows[0]["PkgGrdMscVou"].ToString();
                    packingMasterModel.Grade = dt.Rows[0]["PkgGrade"].ToString();
                    packingMasterModel.Width = dt.Rows[0]["PkgWidth"].ToString();
                    packingMasterModel.Thick = dt.Rows[0]["PkgThick"].ToString();
                    packingMasterModel.OD = dt.Rows[0]["PkgOd"].ToString();
                    packingMasterModel.HeatNumber = dt.Rows[0]["PkgHeatNo"].ToString();
                    packingMasterModel.Qty = dt.Rows[0]["PkgPcs"].ToString();
                    packingMasterModel.FeetInPCS = dt.Rows[0]["PkgFeetPer"].ToString();
                    packingMasterModel.QtyInFeet = dt.Rows[0]["PkgFeet"].ToString();
                    packingMasterModel.QtyInMeter = dt.Rows[0]["PkgMeter"].ToString();
                    packingMasterModel.NoOfCopy = dt.Rows[0]["PkgNoCopy"].ToString();
                    if (id > 0 || IsAdded == true)
                    {
                        packingMasterModel.IsUpdate = true;
                    }
                    packingMasterModel.PackerName = dt.Rows[0]["PkgPkgBy"].ToString();
                    packingMasterModel.QualityCheckerName = dt.Rows[0]["PkgChkBy"].ToString();
                    packingMasterModel.SubBundleNumber = dt.Rows[0]["PkgSubSr"].ToString();
                    packingMasterModel.PackingDate = Convert.ToDateTime(dt.Rows[0]["PkgPkgDt"].ToString()).ToString("yyyy-MM-dd");
                    packingMasterModel.OutDate = !string.IsNullOrEmpty(dt.Rows[0]["PkgOutDt"].ToString()) ? Convert.ToDateTime(dt.Rows[0]["PkgOutDt"].ToString()).ToString("yyyy-MM-dd") : "";
                    packingMasterModel.OutBy = dt.Rows[0]["PkgOutBy"].ToString();

                    //packingMasterModel.NB = dt.Rows[0]["PkgNB"].ToString();
                    //packingMasterModel.SCH = dt.Rows[0]["PkgSch"].ToString();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(packingMasterModel);
        }

        [HttpPost]
        public IActionResult Index(PackingMasterModel packingMasterModel)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                if (ModelState.IsValid)
                {
                    SqlParameter[] parameter = new SqlParameter[30];
                    parameter[0] = new SqlParameter("@PkgVou", packingMasterModel.Vou);
                    parameter[1] = new SqlParameter("@PkgCmpVou", packingMasterModel.CompanyVou);
                    parameter[2] = new SqlParameter("@PkgType", packingMasterModel.Type);
                    parameter[3] = new SqlParameter("@PkgBndNo", packingMasterModel.BundleNo);
                    parameter[4] = new SqlParameter("@PkgSrno", packingMasterModel.SrNo);
                    parameter[5] = new SqlParameter("@PkgWONo", packingMasterModel.WONumber);
                    parameter[6] = new SqlParameter("@PkgPONo", packingMasterModel.PONumber);
                    parameter[7] = new SqlParameter("@PkgPrdVou", packingMasterModel.ProductVou);
                    parameter[8] = new SqlParameter("@PkgPrdCd", packingMasterModel.ProductCode);
                    parameter[9] = new SqlParameter("@PkgSpeMscVou", packingMasterModel.SpecificationVou);
                    parameter[10] = new SqlParameter("@PkgFinMscVou", packingMasterModel.FinishVou);
                    parameter[11] = new SqlParameter("@PkgFinish", packingMasterModel.Finish);
                    parameter[12] = new SqlParameter("@PkgGrdMscVou", packingMasterModel.GradeVou);
                    parameter[13] = new SqlParameter("@PkgGrade", packingMasterModel.Grade);
                    parameter[14] = new SqlParameter("@PkgWidth", packingMasterModel.Width);
                    parameter[15] = new SqlParameter("@PkgThick", packingMasterModel.Thick);
                    parameter[16] = new SqlParameter("@PkgOD", packingMasterModel.OD);
                    parameter[17] = new SqlParameter("@PkgHeatNo", packingMasterModel.HeatNumber);
                    parameter[18] = new SqlParameter("@PkgPcs", packingMasterModel.Qty);
                    parameter[19] = new SqlParameter("@PkgFeetPer", packingMasterModel.FeetInPCS);
                    parameter[20] = new SqlParameter("@PkgFeet", packingMasterModel.QtyInFeet);
                    parameter[21] = new SqlParameter("@PkgMeter", packingMasterModel.QtyInMeter);
                    parameter[22] = new SqlParameter("@PkgQty", packingMasterModel.QtyInKg);
                    parameter[23] = new SqlParameter("@PkgPkgBy", packingMasterModel.PackerName);
                    parameter[24] = new SqlParameter("@PkgChkBy", packingMasterModel.QualityCheckerName);
                    parameter[25] = new SqlParameter("@PkgSubSr", packingMasterModel.SubBundleNumber);
                    parameter[26] = new SqlParameter("@PkgPkgDt", packingMasterModel.PackingDate);
                    parameter[27] = new SqlParameter("@PkgOutDt", packingMasterModel.OutDate);
                    parameter[28] = new SqlParameter("@PkgOutBy", packingMasterModel.OutBy);
                    parameter[29] = new SqlParameter("@PkgNoOfCopy", packingMasterModel.NoOfCopy);

                    //parameter[29] = new SqlParameter("@LotPrdCd", packingMasterModel.ProductCode);
                    DataTable dt = ObjDBConnection.CallStoreProcedure("INSERTPackageMaster", parameter);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int status = Convert.ToInt32(dt.Rows[0][0].ToString());
                        if (status > 0)
                        {
                            SetSuccessMessage("Packing master record inserted successfully");
                            return RedirectToAction("Index", "PackingMaster", new { id = 0, IsAdded = true });
                        }
                        else
                        {
                            SetErrorMessage("Packing master record not inserted!");
                            return View(packingMasterModel);
                        }

                    }
                    else
                    {
                        SetErrorMessage("Packing master record not inserted!");
                        return View(packingMasterModel);
                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(packingMasterModel);
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

            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            ViewBag.companyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator); ;
            ViewBag.productList = objProductHelper.GetProductMasterWithCodeDropdown(companyId);
            ViewBag.specificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
            ViewBag.gradeList = objProductHelper.GetGradeMasterDropdown(companyId, 0);
            ViewBag.finishList = objProductHelper.GetFinishMasterDropdown(companyId, 0);
            ViewBag.employeeList = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
            ViewBag.NBList = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
            ViewBag.SchList = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);

            {
                List<SelectListItem> typeList = new List<SelectListItem>();
                typeList.Add(new SelectListItem
                {
                    Text = "SRM",
                    Value = "SRM"
                });
                typeList.Add(new SelectListItem
                {
                    Text = "SMR",
                    Value = "SMR"
                });
                typeList.Add(new SelectListItem
                {
                    Text = "DMO",
                    Value = "DMO"
                });
                ViewBag.typeList = typeList;
            }

            var list = new List<SelectListItem>
    {
        new SelectListItem{ Text="1", Value = "1", Selected = true },
        new SelectListItem{ Text="2", Value = "2" },
        new SelectListItem{ Text="3", Value = "3" },
        new SelectListItem{ Text="4", Value = "4" },
        new SelectListItem{ Text="5", Value = "5" },
    };
            ViewBag.noofcopy = list;



        }

        private string GetBundleNo()
        {
            try
            {
                DataTable dt = ObjDBConnection.CallStoreProcedure("GetPackingBundleNo", null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return string.Empty;
        }

        private string GetSrNo()
        {
            try
            {
                DataTable dt = ObjDBConnection.CallStoreProcedure("GetPackingSrNo", null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return string.Empty;
        }

        public IActionResult Delete(int id)
        {
            try
            {
                SqlParameter[] parameter = new SqlParameter[1];
                parameter[0] = new SqlParameter("@PkgVou", id);
                DataTable dt = ObjDBConnection.CallStoreProcedure("DeletePackageMaster", parameter);
                SetSuccessMessage("Packing record deleted succesfully!");
            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("Index", "PackingMaster", new { id = 0 });
        }

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby, string SearchType, 
            string SearcBundleNo, string SearcWONumber, string SearcPONumber, string frRecDt, string toRecDt )
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                if (gridMstId > 0)
                {
                    #region User Rights
                    long userId = GetIntSession("UserId");
                    UserFormRightModel userFormRights = new UserFormRightModel();
                    string currentURL = "/PackingMaster/Index";
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
                    string whereConditionQuery = string.Empty;

                    if (!string.IsNullOrWhiteSpace(SearchType))
                    {
                        whereConditionQuery += " AND PkgType='" + SearchType + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(SearcBundleNo))
                    {
                        whereConditionQuery += " AND PkgBndNo='" + SearcBundleNo + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(SearcWONumber))
                    {
                        whereConditionQuery += " AND PkgWoNo='" + SearcWONumber + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(SearcPONumber))
                    {
                        whereConditionQuery += " AND PkgPONo='" + SearcPONumber + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(frRecDt))
                    {
                        whereConditionQuery += " AND PkgPkgDt>='" + frRecDt + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(toRecDt))
                    {
                        whereConditionQuery += " AND PkgPkgDt<='" + toRecDt + "'";
                    }

                    getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, 0, 0, "", 0, 0, whereConditionQuery);
                    if (getReportDataModel.IsError)
                    {
                        ViewBag.Query = getReportDataModel.Query;
                        return PartialView("_reportView");
                    }
                    getReportDataModel.pageIndex = pageIndex;
                    getReportDataModel.ControllerName = "PackingMaster";
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
                    var bytes = Excel(getReportDataModel, "Packing Entry", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "PackingEntry.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Packing Entry", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "PackingEntry.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult GetDataByWONumber(string WONumber)
        {
            try
            {
                PackingMasterModel packingMasterModel = new PackingMasterModel();
                SqlParameter[] parameter = new SqlParameter[1];
                parameter[0] = new SqlParameter("@WONumber", WONumber);
                DataSet ds = ObjDBConnection.GetDataSet("GetDataByWONumberPacking", parameter);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Count == 1)
                {
                    List<object> data = new List<object>();
                    DataTable dtLotMst = ds.Tables[0];
                    if (dtLotMst != null && dtLotMst.Rows.Count > 0)
                    {
                        data.Add(dtLotMst.Rows[0]["OrmPONo"].ToString());
                    }

                    if (data == null || data.Count <= 0)
                    {
                        return Json(new { result = false, message = "Invalid WO No!" });
                    }
                    else
                    {
                        return Json(new { result = true, data = data });
                    }

                }
                else
                {
                    return Json(new { result = false, message = "Invalid WO No!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Invalid WO No!" });
            }
        }

        [HttpPost]
        public ActionResult GetDataByProductCode(string ProductCode)
        {
            try
            {
                PackingMasterModel packingMasterModel = new PackingMasterModel();
                SqlParameter[] parameter = new SqlParameter[1];
                parameter[0] = new SqlParameter("@ProductCode", ProductCode);
                // DataSet ds = ObjDBConnection.GetDataSet("GetDataByProductCodePacking", parameter); //old
                DataSet ds = ObjDBConnection.GetDataSet("GetLotMstDetailsByProductCode", parameter); //changes by jitendra

                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Count == 1)
                {
                    List<object> data = new List<object>();
                    DataTable dtLotMst = ds.Tables[0];
                    if (dtLotMst != null && dtLotMst.Rows.Count > 0)
                    {
                        data.Add(dtLotMst.Rows[0]["MscVou"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotWidth"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotThick"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotOD"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotPCS"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotFeetPer"].ToString());
                        data.Add(Convert.ToDecimal(dtLotMst.Rows[0]["LotPCS"].ToString()) * Convert.ToDecimal(dtLotMst.Rows[0]["LotFeetPer"].ToString()));
                        string b = String.Format("{0:0.00}", Convert.ToDecimal(dtLotMst.Rows[0]["LotPCS"].ToString()) * Convert.ToDecimal(dtLotMst.Rows[0]["LotFeetPer"].ToString()) * Convert.ToDecimal(0.3048));
                        data.Add(b);
                        data.Add(dtLotMst.Rows[0]["LotNB"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotSch"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotNB"].ToString() + " NB X SCH " + dtLotMst.Rows[0]["LotSch"].ToString());
                    }

                    if (data == null || data.Count <= 0)
                    {
                        return Json(new { result = false, message = "Invalid Product Code!" });
                    }
                    else
                    {
                        return Json(new { result = true, data = data });
                    }

                }
                else
                {
                    return Json(new { result = false, message = "Invalid Product Code!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Invalid Product Code!" });
            }
        }

        public ActionResult GetLastPackerAndQualityName()
        {
            try
            {
                PackingMasterModel packingMasterModel = new PackingMasterModel();
                SqlParameter[] parameter = new SqlParameter[1];
                parameter[0] = new SqlParameter("@ProductCode", "");
                DataSet ds = ObjDBConnection.GetDataSet("GetLastPackerAndQualityName", parameter);

                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Count == 1)
                {
                    List<object> data = new List<object>();
                    DataTable dtLotMst = ds.Tables[0];
                    if (dtLotMst != null && dtLotMst.Rows.Count > 0)
                    {
                        data.Add(dtLotMst.Rows[0]["PkgPkgBy"].ToString());
                        data.Add(dtLotMst.Rows[0]["PkgChkBy"].ToString());
                        data.Add(Convert.ToDateTime(dtLotMst.Rows[0]["PkgPkgDt"].ToString()).ToString("yyyy-MM-dd"));


                    }

                    if (data != null && data.Count > 0)
                    {
                        return Json(new { result = true, data = data });
                    }
                    else
                    {
                        data.Add(4);
                        data.Add(5);
                        data.Add(Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd"));
                        return Json(new { result = true, data = data });
                    }

                }
                return Json(new { result = false, message = "Invalid Product Code!" });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Invalid Product Code!" });
            }
        }
    }
}
