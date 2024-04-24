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
    public class ProcessTransferController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        ProductHelpers objProductHelper = new ProductHelpers();

        public IActionResult Index(int id)
        {
            ProcessTransferModel processTransferModel = new ProcessTransferModel();
            try
            {
                bool isreturn = false;
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                long userId = GetIntSession("UserId");

                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                processTransferModel.PrcVNo = GetVoucherNo();
                processTransferModel.GradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                

                if (id > 0)
                {
                    SqlParameter[] sqlpara = new SqlParameter[1];
                    sqlpara[0] = new SqlParameter("@PrcVou", id);
                    DataTable dtProcess = ObjDBConnection.CallStoreProcedure("ProcessTransfer_Select", sqlpara);


                    if (dtProcess != null && dtProcess.Rows.Count > 0)
                    {
                        processTransferModel.PrcVou = id;
                        processTransferModel.PrcVNo = dtProcess.Rows[0]["PrcVNo"].ToString();
                        processTransferModel.PrcCmpCdn = Convert.ToInt32(dtProcess.Rows[0]["PrcCmpCdn"]);
                        processTransferModel.Date = Convert.ToDateTime(dtProcess.Rows[0]["PrcDt"]).ToString("yyyy-MM-dd");
                        processTransferModel.PrcCurPrcVou = Convert.ToInt32(dtProcess.Rows[0]["PrcCurPrcVou"]);
                        processTransferModel.PrcFinMscVou = Convert.ToInt32(dtProcess.Rows[0]["PrcFinMscVou"]);
                        processTransferModel.PrcGrdMscVou = Convert.ToInt32(dtProcess.Rows[0]["PrcGrdMscVou"]);
                        processTransferModel.PrcLangth = Convert.ToDecimal(dtProcess.Rows[0]["PrcLanght"]);
                        processTransferModel.PrcNtxPrcVou = Convert.ToInt32(dtProcess.Rows[0]["prcNxtPrcVou"]);
                        processTransferModel.PrcPrdVou = Convert.ToInt32(dtProcess.Rows[0]["PrcPrdVou"]);
                        processTransferModel.PrcSCH = dtProcess.Rows[0]["PrcSCH"].ToString();
                        processTransferModel.PrcQty = Convert.ToDecimal(dtProcess.Rows[0]["PrcQty"]);
                        
                        processTransferModel.PrcNB = dtProcess.Rows[0]["PrcNB"].ToString();
                        processTransferModel.PrcOD = Convert.ToDecimal(dtProcess.Rows[0]["PrcOD"]);
                        processTransferModel.PrcPCS = Convert.ToDecimal(dtProcess.Rows[0]["PrcPCS"]);
                        processTransferModel.PrcThick = Convert.ToDecimal(dtProcess.Rows[0]["PrcThick"]);
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return View(processTransferModel);
        }
        public IActionResult Delete(int id)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                long userId = GetIntSession("UserId");
                SqlParameter[] parameter = new SqlParameter[1];
                parameter[0] = new SqlParameter("@PrcVou", id);
                DataTable dt = ObjDBConnection.CallStoreProcedure("DeletePrcTrn", parameter);
                SetSuccessMessage("Process Transfer record deleted succesfully!");
            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("Index", "ProcessTransfer", new { id = 0 });
        }

        [HttpPost]
        public IActionResult Index(long Id, ProcessTransferModel processTransferModel)
        {

            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                long userId = GetIntSession("UserId");
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                //if (ModelState.IsValid)
                //{
                SqlParameter[] parameter = new SqlParameter[17];
                parameter[0] = new SqlParameter("@PrcVou", Id);
                parameter[1] = new SqlParameter("@PrcCmpCdN", processTransferModel.PrcCmpCdn);
                parameter[2] = new SqlParameter("@PrcVNo", processTransferModel.PrcVNo);
                parameter[3] = new SqlParameter("@PrcDt", processTransferModel.Date);
                parameter[4] = new SqlParameter("@PrcCurPrcVou",processTransferModel.PrcCurPrcVou);
                parameter[5] = new SqlParameter("@PrcPrdVou", processTransferModel.PrcPrdVou);
                parameter[6] = new SqlParameter("@PrcGrdMscVou", processTransferModel.PrcGrdMscVou);
                parameter[7] = new SqlParameter("@PrcFinMscVou", processTransferModel.PrcFinMscVou);
                parameter[8] = new SqlParameter("@PrcNB", processTransferModel.PrcNB);
                parameter[9] = new SqlParameter("@prcSCH", processTransferModel.PrcSCH);
                parameter[10] = new SqlParameter("@PrcOD", processTransferModel.PrcOD);
                parameter[11] = new SqlParameter("@prcThick", processTransferModel.PrcThick);
                parameter[12] = new SqlParameter("@PrcLangth", processTransferModel.PrcLangth);
                parameter[13] = new SqlParameter("@prcQty", processTransferModel.PrcQty);
                parameter[14] = new SqlParameter("@prcPCS", processTransferModel.PrcPCS);
                parameter[15] = new SqlParameter("@prcNxtPrcVou", processTransferModel.PrcNtxPrcVou);
                parameter[16] = new SqlParameter("@PrcUsr", userId);


                DataTable dt = ObjDBConnection.CallStoreProcedure("ProcessTransfer_Insert", parameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (Id != 0)
                    {
                        SetSuccessMessage("Processor Transfer Update successfully");
                        return RedirectToAction("Index", "ProcessTransfer", new { id = 0 });
                    }
                    else
                    {
                        int status = Convert.ToInt32(dt.Rows[0][0].ToString());
                        if (status > 0)
                        {
                            SetSuccessMessage("Processor Transfer record inserted successfully");
                            return RedirectToAction("Index", "ProcessTransfer", new { id = 0 });
                        }
                        else
                        {
                            SetErrorMessage("Duplicate Voucher No!...");
                            return View(processTransferModel);
                        }
                    }

                }
                else
                {
                    SetErrorMessage("Process Transfer record not inserted!");
                    return View(processTransferModel);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return View(processTransferModel);
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
                    string currentURL = "/ProcessTransfer/Index";
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
                    getReportDataModel.ControllerName = "ProcessTransfer";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }
        public JsonResult GetQtyPCS(int PrcId, decimal PrcThick, long prdId, long gradeId, long finishId, string nb, string sch, decimal od, decimal langth)
        {
            ProcessTransferModel obj = new ProcessTransferModel();
            try
            {
                if (PrcId != 0  && PrcThick!=0)
                {
                    SqlParameter[] sqlParameters = new SqlParameter[9];
                    sqlParameters[0] = new SqlParameter("@PrcId", PrcId);
                    sqlParameters[1] = new SqlParameter("@PrcThick", PrcThick);
                    sqlParameters[2] = new SqlParameter("@prdId", prdId);
                    sqlParameters[3] = new SqlParameter("@gradeId", gradeId);
                    sqlParameters[4] = new SqlParameter("@finishId", finishId);
                    sqlParameters[5] = new SqlParameter("@nb", nb);
                    sqlParameters[6] = new SqlParameter("@sch", sch);
                    sqlParameters[7] = new SqlParameter("@od", od);
                    sqlParameters[8] = new SqlParameter("@langth", langth);
                    DataTable dtQtyPCS = ObjDBConnection.CallStoreProcedure("GetQTYPCS", sqlParameters);
                    if (dtQtyPCS != null && dtQtyPCS.Rows.Count > 0)
                    {
                        string LotQTY = dtQtyPCS.Rows[0][0].ToString().Trim();
                        string LotPCS = dtQtyPCS.Rows[0][1].ToString().Trim();
                        return Json(new { result = true,LotQTY,LotPCS});
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { result = false });
        }

        public ActionResult GetDataByCoilNo(string coilNo, string mildt, string MacId)
        {
            try
            {
                ProcessTransferModel processTransferModel = new ProcessTransferModel();
                SqlParameter[] parameter = new SqlParameter[2];
                parameter[0] = new SqlParameter("@COILNO", coilNo);
                parameter[1] = new SqlParameter("@MACVOU", MacId);
                DataSet ds = ObjDBConnection.GetDataSet("GetDataByCoilNoMilling", parameter);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Count == 6)
                {
                    List<object> data = new List<object>();
                    int IsValidOD = 1;
                    DataTable dtLotMst = ds.Tables[0];
                    DataTable dtMacMst = ds.Tables[4];
                    DataTable dtNBSCH = ds.Tables[5];
                    if (dtLotMst != null && dtLotMst.Rows.Count > 0)
                    {
                        data.Add(dtLotMst.Rows[0]["LotQty"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotVou"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotGrade"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotThick"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotWidth"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotQty"].ToString());
                        data.Add(Convert.ToDateTime(dtLotMst.Rows[0]["LotDt"].ToString()).ToString("yyyy-MM-dd"));

                        if (Convert.ToDateTime(Convert.ToDateTime(dtLotMst.Rows[0]["LotDt"].ToString()).ToString("yyyy-MM-dd")) > Convert.ToDateTime(mildt))
                        {
                            return Json(new { result = false, message = "Lot Date Must Be Less Than Milling Date!" });
                        }
                        if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
                        {
                            data.Add(dtNBSCH.Rows[0]["NbsNB"].ToString());
                            data.Add(dtNBSCH.Rows[0]["NbsSch"].ToString());
                        }
                        else
                        {
                            data.Add("");
                            data.Add("");
                        }
                        data.Add(dtLotMst.Rows[0]["LotRemaining"].ToString());
                        if (dtLotMst.Rows[0]["LotRemaining"].ToString() == "")
                        {
                            return Json(new { result = false, message = "Invalid Coil No!" });
                        }
                        else if (Convert.ToDecimal(dtLotMst.Rows[0]["LotRemaining"].ToString()) <= 0)
                        {
                            return Json(new { result = false, message = "Remaining coil weight is not sufficiant to make a process!" });
                        }
                    }
                    string MilMaxOD = "";
                    string MilMinOD = "";

                    if (dtMacMst != null && dtMacMst.Rows.Count > 0)
                    {
                        if (!(Convert.ToDecimal(dtLotMst.Rows[0]["LotOD"].ToString()) >= Convert.ToDecimal(dtMacMst.Rows[0]["MACSIZERNGFR"].ToString()) && Convert.ToDecimal(dtLotMst.Rows[0]["LotOD"].ToString()) <= Convert.ToDecimal(dtMacMst.Rows[0]["MACSIZERNGTO"].ToString())))
                        {
                            IsValidOD = 0;
                            //return Json(new { result = false, message = "Coil OD Size is Not Range to this Machine!" });
                        }
                        MilMaxOD = Convert.ToString(Convert.ToDecimal(dtMacMst.Rows[0]["MACSIZERNGTO"].ToString()));
                        MilMinOD = Convert.ToString(Convert.ToDecimal(dtMacMst.Rows[0]["MACSIZERNGFR"].ToString()));
                    }
                    else
                    {
                        return Json(new { result = false, message = "Invalid Coil No!" });
                    }
                    if (data == null || data.Count <= 0)
                    {
                        return Json(new { result = false, message = "Invalid Coil No!" });
                    }
                    else
                    {
                        return Json(new { result = true, data = data, IsValidOD = IsValidOD, MilMaxOD = MilMaxOD, MilMinOD = MilMinOD });
                    }

                }
                else
                {
                    return Json(new { result = false, message = "Invalid Coil No!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
        }

        private string GetVoucherNo()
        {
            string returnValue = string.Empty;
            try
            {
                DataTable dtVoucherNo = ObjDBConnection.CallStoreProcedure("GetProcessTransferNo", null);
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
            ViewBag.companyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
            ViewBag.employeeList = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
            ViewBag.supervisorList = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);
            ViewBag.productList = objProductHelper.GetProductMasterDropdown(companyId);
            ViewBag.finishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
            ViewBag.shiftList = objProductHelper.GetShiftNew();
            ViewBag.processList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "Process Transfer");
            ViewBag.milprocessList = ObjAccountMasterHelpers.GetMachineMasterDropdown(companyId, "MILLING");
            ViewBag.nbList = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
            ViewBag.schList = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);

        }


    }
}
