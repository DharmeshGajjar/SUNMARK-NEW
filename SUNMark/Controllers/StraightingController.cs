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
    public class StraightingController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        ProductHelpers objProductHelper = new ProductHelpers();

        public IActionResult Index(int id)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                long userId = GetIntSession("UserId");
                StraightingMasterModel straightingMaster = new StraightingMasterModel();
                straightingMaster.Straighting = new StrGridModel();
                straightingMaster.Straighting.RecProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "PIPE");
                straightingMaster.Straighting.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                straightingMaster.Straighting.StatusList = objProductHelper.GetPicklingStatus();
                straightingMaster.Vno = GetVoucherNo();
                if (id > 0)
                {
                    SqlParameter[] parameter = new SqlParameter[2];
                    parameter[0] = new SqlParameter("@PikVou", id);
                    parameter[1] = new SqlParameter("@Flg", 0);
                    DataTable dt = ObjDBConnection.CallStoreProcedure("GetAnnealingMasterById", parameter);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        straightingMaster.StrVou = id;
                        straightingMaster.Vno = dt.Rows[0]["StrVno"].ToString(); ;
                        straightingMaster.StrCmpVou = Convert.ToInt32(dt.Rows[0]["StrCmpVou"].ToString());
                        straightingMaster.Date = Convert.ToDateTime(dt.Rows[0]["StrDt"]).ToString("yyyy-MM-dd");
                        straightingMaster.Shift = dt.Rows[0]["StrShift"].ToString();
                        straightingMaster.MachineNo = dt.Rows[0]["StrMacno"].ToString();
                        straightingMaster.SupEmpVou = dt.Rows[0]["StrSupEmpVou"].ToString();
                        straightingMaster.ManEmpVou = dt.Rows[0]["StrManEmpVou"].ToString();
                        straightingMaster.IssuePrdVou = dt.Rows[0]["StrIssPrdVou"].ToString();
                        straightingMaster.Finish = dt.Rows[0]["StrFinish"].ToString();
                        straightingMaster.FinishVou = dt.Rows[0]["StrFinishVou"].ToString();
                        straightingMaster.NextProc = dt.Rows[0]["StrNextProc"].ToString();
                        straightingMaster.NextPrcVou = dt.Rows[0]["StrNextPrcVou"].ToString();
                        straightingMaster.HFQty = dt.Rows[0]["StrHFQty"].ToString();
                        straightingMaster.Remarks = dt.Rows[0]["StrRemarks"].ToString();

                        straightingMaster.Straighting.Grade = new string[dt.Rows.Count];
                        straightingMaster.Straighting.StrOD = new decimal[dt.Rows.Count];
                        straightingMaster.Straighting.StrThick = new decimal[dt.Rows.Count];
                        straightingMaster.Straighting.StrLength = new decimal[dt.Rows.Count];
                        straightingMaster.Straighting.StrNoOfPipe = new decimal[dt.Rows.Count];
                        straightingMaster.Straighting.StrWeight = new decimal[dt.Rows.Count];
                        straightingMaster.Straighting.RecProduct = new string[dt.Rows.Count];
                        straightingMaster.Straighting.StrInTime = new string[dt.Rows.Count];
                        straightingMaster.Straighting.StrOutTime = new string[dt.Rows.Count];
                        straightingMaster.Straighting.StrCoilNo = new string[dt.Rows.Count];
                        straightingMaster.Straighting.StrStatus = new string[dt.Rows.Count];
                        straightingMaster.Straighting.StrType = new string[dt.Rows.Count];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            straightingMaster.Straighting.Grade[i] = dt.Rows[i]["StrAGrdVou"].ToString();
                            straightingMaster.Straighting.StrOD[i] = Convert.ToDecimal(dt.Rows[i]["StrAOD"].ToString());
                            straightingMaster.Straighting.StrThick[i] = Convert.ToDecimal(dt.Rows[i]["StrAThick"].ToString());
                            straightingMaster.Straighting.StrLength[i] = Convert.ToDecimal(dt.Rows[i]["StrALength"].ToString());
                            straightingMaster.Straighting.StrNoOfPipe[i] = Convert.ToDecimal(dt.Rows[i]["StrANoOfPipe"].ToString());
                            straightingMaster.Straighting.StrWeight[i] = Convert.ToDecimal(dt.Rows[i]["StrAWeight"].ToString());
                            straightingMaster.Straighting.RecProduct[i] = dt.Rows[i]["StrARecPrdVou"].ToString();
                            straightingMaster.Straighting.StrInTime[i] = dt.Rows[i]["StrAInTime"].ToString();
                            straightingMaster.Straighting.StrOutTime[i] = dt.Rows[i]["StrAOutTime"].ToString();
                            straightingMaster.Straighting.StrCoilNo[i] = dt.Rows[i]["StrACoilNo"].ToString();
                            straightingMaster.Straighting.StrStatus[i] = dt.Rows[i]["StrAStatus"].ToString();
                            straightingMaster.Straighting.StrType[i] = dt.Rows[i]["StrAType"].ToString();
                        }
                    }
                }
                return View(straightingMaster);
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(long id, StraightingMasterModel straightingMaster)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(straightingMaster.Vno).ToString()) && !string.IsNullOrWhiteSpace(straightingMaster.Date) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(straightingMaster.StrCmpVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(straightingMaster.MachineNo).ToString()) && straightingMaster.Straighting.RecProduct.Length > 0 && straightingMaster.Straighting.Grade.Length > 0 )
                {
                    SqlParameter[] sqlParameter = new SqlParameter[18];
                    sqlParameter[0] = new SqlParameter("@StrVou", straightingMaster.StrVou);
                    sqlParameter[1] = new SqlParameter("@StrCmpVou", straightingMaster.StrCmpVou);
                    sqlParameter[2] = new SqlParameter("@StrVno", straightingMaster.Vno);
                    sqlParameter[3] = new SqlParameter("@StrDt", straightingMaster.Date);
                    sqlParameter[4] = new SqlParameter("@StrShift", straightingMaster.Shift);
                    sqlParameter[5] = new SqlParameter("@StrMacNo", straightingMaster.MachineNo);
                    sqlParameter[6] = new SqlParameter("@StrSupEmpVou", straightingMaster.SupEmpVou);
                    sqlParameter[7] = new SqlParameter("@StrManEmpVou", straightingMaster.ManEmpVou);
                    sqlParameter[8] = new SqlParameter("@StrIssPrdVou", straightingMaster.IssuePrdVou);
                    sqlParameter[9] = new SqlParameter("@StrFinish", straightingMaster.Finish);
                    sqlParameter[10] = new SqlParameter("@StrFinishVou", straightingMaster.FinishVou);
                    sqlParameter[11] = new SqlParameter("@StrHFQty", straightingMaster.HFQty);
                    sqlParameter[12] = new SqlParameter("@StrNitricQty", straightingMaster.NitricQty);
                    sqlParameter[13] = new SqlParameter("@StrLimeQty", straightingMaster.LimeQty);
                    sqlParameter[14] = new SqlParameter("@StrRemarks", straightingMaster.Remarks);
                    sqlParameter[15] = new SqlParameter("@StrNextPrcVou", straightingMaster.NextPrcVou);
                    sqlParameter[16] = new SqlParameter("@StrNextProc", straightingMaster.NextProc);
                    sqlParameter[17] = new SqlParameter("@Flg", "1");
                    DataTable dt = ObjDBConnection.CallStoreProcedure("InsertStrmst", sqlParameter);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int masterId = DbConnection.ParseInt32(dt.Rows[0][0].ToString());
                        if (masterId > 0)
                        {
                            for (int i = 0; i < straightingMaster.Straighting.StrWeight.Length; i++)
                            {
                                SqlParameter[] sqlParam = new SqlParameter[15];
                                sqlParam[0] = new SqlParameter("@StrAStrVou", masterId);
                                sqlParam[1] = new SqlParameter("@StrCmpVou", straightingMaster.StrCmpVou);
                                sqlParam[2] = new SqlParameter("@StrGrdVou", straightingMaster.Straighting.Grade[i]);
                                sqlParam[3] = new SqlParameter("@StrThick", straightingMaster.Straighting.StrThick[i]);
                                sqlParam[4] = new SqlParameter("@StrOD", straightingMaster.Straighting.StrOD[i]);
                                sqlParam[5] = new SqlParameter("@StrLength", straightingMaster.Straighting.StrLength[i]);
                                sqlParam[6] = new SqlParameter("@StrNoOfPipe", straightingMaster.Straighting.StrNoOfPipe[i]);
                                sqlParam[7] = new SqlParameter("@StrQty", straightingMaster.Straighting.StrWeight[i]);
                                sqlParam[8] = new SqlParameter("@StrRecPrdVou", straightingMaster.Straighting.RecProduct[i]);
                                sqlParam[9] = new SqlParameter("@StrInTime", straightingMaster.Straighting.StrInTime[i + 1]);
                                sqlParam[10] = new SqlParameter("@StrOutTime", straightingMaster.Straighting.StrOutTime[i + 1]);
                                sqlParam[11] = new SqlParameter("@StrCoilNo", straightingMaster.Straighting.StrCoilNo[i + 1]);
                                sqlParam[12] = new SqlParameter("@StrStatus", straightingMaster.Straighting.StrStatus[i]);
                                sqlParam[13] = new SqlParameter("@StrType", straightingMaster.Straighting.StrType[i + 1]);
                                sqlParam[14] = new SqlParameter("@StrSrNo", (i + 1));
                                DataTable dttrn = ObjDBConnection.CallStoreProcedure("InsertStrTrn", sqlParam);
                            }
                            int Status = DbConnection.ParseInt32(dt.Rows[0][0].ToString());
                            if (Status == 0)
                            {
                                SetErrorMessage("Dulplicate Vou.No Details");
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Record updated succesfully!");
                                }
                                else
                                {
                                    SetSuccessMessage("Record inserted succesfully!");
                                }

                            }
                        }
                        else
                        {
                            SetSuccessMessage("Insert error!");
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("Index", new { id = "0" });
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
            ViewBag.employeeList = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0); ;
            ViewBag.supervisorList = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0); ;
            ViewBag.productList = objProductHelper.GetProductMasterDropdown(companyId); ;
            ViewBag.shiftList = objProductHelper.GetShiftNew(); ;
            ViewBag.milprocessList = ObjAccountMasterHelpers.GetMachineMasterDropdown(companyId);
            ViewBag.gradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
            ViewBag.finishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
            ViewBag.nextProcList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
        }

        public IActionResult Delete(int id)
        {
            try
            {
                SqlParameter[] parameter = new SqlParameter[1];
                parameter[0] = new SqlParameter("@StrVou", id);
                DataTable dt = ObjDBConnection.CallStoreProcedure("DeleteStraightingMaster", parameter);
                SetSuccessMessage("Straighting record deleted succesfully!");
            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("Index", "Straighting", new { id = 0 });
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
                    string currentURL = "/Straighting/Index";
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
                    getReportDataModel.ControllerName = "Straighting";
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
                    var bytes = Excel(getReportDataModel, "Straighting", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Straighting.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Pickling", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Straighting.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetVoucherNo()
        {
            string returnValue = string.Empty;
            try
            {
                DataTable dtVoucherNo = ObjDBConnection.CallStoreProcedure("GetPicklingVoucherNo", null);
                if (dtVoucherNo != null && dtVoucherNo.Rows.Count > 0)
                {
                    returnValue = dtVoucherNo.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return returnValue;
        }

        public ActionResult GetSupEmpByShiftMacNo(string shift, string macNo)
        {
            SqlParameter[] parameter = new SqlParameter[2];
            parameter[0] = new SqlParameter("@Macno", macNo);
            parameter[1] = new SqlParameter("@Shift", shift);
            DataTable dt = ObjDBConnection.CallStoreProcedure("GetSupByShiftMacNo", parameter);
            if (dt != null && dt.Rows.Count > 0)
            {
                return Json(new { result = true, Sup1 = dt.Rows[0]["Sup"].ToString(), Opr1 = dt.Rows[0]["Opr2"].ToString(), Opr2 = dt.Rows[0]["Opr2"].ToString() });
            }
            else
            {
                return Json(new { result = false });
            }
        }
        public IActionResult GetLotIssStraightingProduct(int recProdId, int strvou, int gradeId, decimal od, decimal thick, string dt, int gsrno)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                SqlParameter[] sqlParameters = new SqlParameter[8];
                sqlParameters[0] = new SqlParameter("@AnnVou", strvou);
                sqlParameters[1] = new SqlParameter("@RecProd", recProdId);
                sqlParameters[2] = new SqlParameter("@Grade", gradeId);
                sqlParameters[3] = new SqlParameter("@od", od);
                sqlParameters[4] = new SqlParameter("@thick", thick);
                sqlParameters[5] = new SqlParameter("@dt", dt);
                sqlParameters[6] = new SqlParameter("@FLG", "3");
                sqlParameters[7] = new SqlParameter("@GSrNo", gsrno);
                DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetLotIssAnnelProduct", sqlParameters);
                if (DtInw != null)
                {
                    int Status = DbConnection.ParseInt32(DtInw.Rows[0][0].ToString());
                    if (Status == 1)
                    {
                        return Json(new { result = true, data = "1" });
                    }
                    else if (Status == 2)
                    {
                        return Json(new { result = true, data = "2" });
                    }
                    else if (Status == 3)
                    {
                        return Json(new { result = true, data = "3" });
                    }
                    else
                    {
                        string LotPcs = DtInw.Rows[0]["LotPCS"].ToString();
                        string LotQty = DtInw.Rows[0]["LotQty"].ToString();
                        string Length = DtInw.Rows[0]["Length"].ToString();
                        return Json(new { result = true, lotPcs = LotPcs, lotQty = LotQty, length = Length });
                    }

                }
                else
                {
                    return Json(new { result = true, data = "1" });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
