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
    public class MillingController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        ProductHelpers objProductHelper = new ProductHelpers();

        public IActionResult Index(int id)
        {
            MillingMasterModel millingMasterModel = new MillingMasterModel();
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                SqlParameter[] sqlpara = new SqlParameter[2];
                sqlpara[0] = new SqlParameter("@MilVou", id);
                sqlpara[1] = new SqlParameter("@Flg", 1);
                DataTable dtMill = ObjDBConnection.CallStoreProcedure("GetMillingMasterById", sqlpara);
                if (dtMill != null && dtMill.Rows.Count > 0)
                {
                    millingMasterModel.CompanyVou = dtMill.Rows[0]["MilCmpVou"].ToString();
                    millingMasterModel.Date = Convert.ToDateTime(dtMill.Rows[0]["MilDt"].ToString()).ToString("yyyy-MM-dd");
                    millingMasterModel.ShiftVou = dtMill.Rows[0]["MilShift"].ToString();
                    //millingMasterModel.MillNo = dtMill.Rows[0]["MilMacNo"].ToString();
                    millingMasterModel.Operator1 = dtMill.Rows[0]["MilOpr1EmpVou"].ToString();
                    millingMasterModel.Operator2 = dtMill.Rows[0]["MilOpr2EmpVou"].ToString();
                    millingMasterModel.Supervisor = dtMill.Rows[0]["MilSupEmpVou"].ToString();
                    millingMasterModel.FeetPer = dtMill.Rows[0]["MilLenFeet"].ToString();
                    millingMasterModel.RecPrdVou = dtMill.Rows[0]["MilRecPrdVou"].ToString();
                    //millingMasterModel.ScrapPipeProductVou = dtMill.Rows[0]["MilScrPrdVou"].ToString();
                    millingMasterModel.ProcessVou = dtMill.Rows[0]["MilNextPrcVou"].ToString();
                    millingMasterModel.PrcVou = dtMill.Rows[0]["MilPrcVou"].ToString();
                }

                millingMasterModel.Vno = GetVoucherNo();
                if (id > 0)
                {
                    SqlParameter[] parameter = new SqlParameter[2];
                    parameter[0] = new SqlParameter("@MilVou", id);
                    parameter[1] = new SqlParameter("@Flg", 0);
                    DataTable dt = ObjDBConnection.CallStoreProcedure("GetMillingMasterById", parameter);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        millingMasterModel.Vou = id;
                        millingMasterModel.CompanyVou = dt.Rows[0]["MilCmpVou"].ToString();
                        millingMasterModel.Vno = dt.Rows[0]["MilVno"].ToString();
                        millingMasterModel.Date = Convert.ToDateTime(dt.Rows[0]["MilDt"].ToString()).ToString("yyyy-MM-dd");
                        millingMasterModel.ShiftVou = dt.Rows[0]["MilShift"].ToString();
                        millingMasterModel.MillNo = dt.Rows[0]["MilMacNo"].ToString();
                        millingMasterModel.Operator1 = dt.Rows[0]["MilOpr1EmpVou"].ToString();
                        millingMasterModel.Operator2 = dt.Rows[0]["MilOpr2EmpVou"].ToString();
                        millingMasterModel.IssueCoilNo = dt.Rows[0]["MilLotNo"].ToString();
                        millingMasterModel.IssueCoilNoVou = dt.Rows[0]["MilLotVou"].ToString();
                        millingMasterModel.OD = dt.Rows[0]["MilOD"].ToString();
                        millingMasterModel.FeetPer = dt.Rows[0]["MilLenFeet"].ToString();
                        millingMasterModel.InTime = dt.Rows[0]["MilInTime"].ToString();
                        millingMasterModel.OutTime = dt.Rows[0]["MilOutTime"].ToString();
                        millingMasterModel.PCS = dt.Rows[0]["MilPCS"].ToString();
                        millingMasterModel.Weight = dt.Rows[0]["MilQty"].ToString();
                        millingMasterModel.RecPrdVou = dt.Rows[0]["MilRecPrdVou"].ToString();
                        millingMasterModel.ScrapLength = dt.Rows[0]["MilScrLenFeet"].ToString();
                        millingMasterModel.ScrapWeight = dt.Rows[0]["MilScrQty"].ToString();
                        millingMasterModel.ScrapPipeProductVou = dt.Rows[0]["MilScrPrdVou"].ToString();
                        millingMasterModel.NoOfTourch = dt.Rows[0]["MilTouNo"].ToString();
                        millingMasterModel.WeldingSpeed = Convert.ToDecimal(dt.Rows[0]["MilWeldSpeed"].ToString());
                        millingMasterModel.AMP = dt.Rows[0]["MilWeldAMP"].ToString();
                        millingMasterModel.Voltage = dt.Rows[0]["MilWeldVolt"].ToString();
                        millingMasterModel.Remarks = dt.Rows[0]["MilRem"].ToString();
                        millingMasterModel.Read1OD = dt.Rows[0]["MilRead1OD"].ToString();
                        millingMasterModel.Read1Thick = dt.Rows[0]["MilRead1Thick"].ToString();
                        millingMasterModel.Read2OD = dt.Rows[0]["MilRead2OD"].ToString();
                        millingMasterModel.Read2Thick = dt.Rows[0]["MilRead2Thick"].ToString();
                        millingMasterModel.Grade = dt.Rows[0]["LotGrade"].ToString();
                        millingMasterModel.Thick = dt.Rows[0]["LotThick"].ToString();
                        millingMasterModel.Width = dt.Rows[0]["LotWidth"].ToString();
                        millingMasterModel.ProcessVou = dt.Rows[0]["MilNextPrcVou"].ToString();
                        millingMasterModel.NB = dt.Rows[0]["MilNB"].ToString();
                        millingMasterModel.NbVou = dt.Rows[0]["MilNBVou"].ToString();
                        millingMasterModel.SCH = dt.Rows[0]["MilSCH"].ToString();
                        millingMasterModel.SCHVou = dt.Rows[0]["MilSCHVou"].ToString();
                        millingMasterModel.PCSWeight = dt.Rows[0]["MilRecQty"].ToString();
                        millingMasterModel.Supervisor = dt.Rows[0]["MilSupEmpVou"].ToString();
                        millingMasterModel.RLPCS = dt.Rows[0]["MilRLPcs"].ToString();
                        millingMasterModel.Supervisor = dt.Rows[0]["MilRLWeight"].ToString();
                        millingMasterModel.PrcVou = dt.Rows[0]["MilPrcVou"].ToString();
                        millingMasterModel.FinishDate = !string.IsNullOrWhiteSpace(dt.Rows[0]["MilFinishDt"].ToString()) ? Convert.ToDateTime(dt.Rows[0]["MilFinishDt"].ToString()).ToString("yyyy-MM-dd") : null;
                        millingMasterModel.Reason = dt.Rows[0]["MilReason"].ToString();
                        millingMasterModel.RemainingWeight = (Convert.ToDecimal(dt.Rows[0]["RemainingWeight"].ToString()) + Convert.ToDecimal(dt.Rows[0]["MilRecQty"].ToString()) + Convert.ToDecimal(dt.Rows[0]["MilScrQty"].ToString())).ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(millingMasterModel);
        }

        [HttpPost]
        public IActionResult Index(MillingMasterModel millingMasterModel)
        {

            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                //if (ModelState.IsValid)
                //{
                    SqlParameter[] parameter = new SqlParameter[42];
                    parameter[0] = new SqlParameter("@MilVou", millingMasterModel.Vou);
                    parameter[1] = new SqlParameter("@MilCmpVou", millingMasterModel.CompanyVou);
                    parameter[2] = new SqlParameter("@MilVno", millingMasterModel.Vno);
                    parameter[3] = new SqlParameter("@MilDt", millingMasterModel.Date);
                    parameter[4] = new SqlParameter("@MilShift", millingMasterModel.ShiftVou);
                    parameter[5] = new SqlParameter("@MilMacNo", millingMasterModel.MillNo);
                    parameter[6] = new SqlParameter("@MilOpr1EmpVou", millingMasterModel.Operator1);
                    parameter[7] = new SqlParameter("@MilOpr2EmpVou", millingMasterModel.Operator2);
                    parameter[8] = new SqlParameter("@MilLotVou", millingMasterModel.IssueCoilNoVou);
                    parameter[9] = new SqlParameter("@MilLotNo", millingMasterModel.IssueCoilNo);
                    parameter[10] = new SqlParameter("@MilOD", millingMasterModel.OD);
                    parameter[11] = new SqlParameter("@MilLenFeet", millingMasterModel.FeetPer);
                    parameter[12] = new SqlParameter("@MilInTime", millingMasterModel.InTime);
                    parameter[13] = new SqlParameter("@MilOutTime", millingMasterModel.OutTime);
                    parameter[14] = new SqlParameter("@MilPcs", millingMasterModel.PCS);
                    parameter[15] = new SqlParameter("@MilQty", millingMasterModel.Weight);
                    parameter[16] = new SqlParameter("@MilRecPrdVou", millingMasterModel.RecPrdVou);
                    parameter[17] = new SqlParameter("@MilScrLenFeet", millingMasterModel.ScrapLength);
                    parameter[18] = new SqlParameter("@MilScrQty", millingMasterModel.ScrapWeight);
                    parameter[19] = new SqlParameter("@MilScrPrdVou", millingMasterModel.ScrapPipeProductVou);
                    parameter[20] = new SqlParameter("@MilTouNo", millingMasterModel.NoOfTourch);
                    parameter[21] = new SqlParameter("@MilWeldSpeed", millingMasterModel.WeldingSpeed);
                    parameter[22] = new SqlParameter("@MilWeldAMP", millingMasterModel.AMP);
                    parameter[23] = new SqlParameter("@MilWeldVolt", millingMasterModel.Voltage);
                    parameter[24] = new SqlParameter("@MilRem", millingMasterModel.Remarks);
                    parameter[25] = new SqlParameter("@MilRead1Thick", millingMasterModel.Read1Thick);
                    parameter[26] = new SqlParameter("@MilRead1OD", millingMasterModel.Read1OD);
                    parameter[27] = new SqlParameter("@MilRead2Thick", millingMasterModel.Read2Thick);
                    parameter[28] = new SqlParameter("@MilRead2OD", millingMasterModel.Read2OD);
                    parameter[29] = new SqlParameter("@MilSupEmpVou", millingMasterModel.Supervisor);
                    parameter[30] = new SqlParameter("@MilNextPrcVou", millingMasterModel.ProcessVou);
                    parameter[31] = new SqlParameter("@MilNB", millingMasterModel.NB);
                    parameter[32] = new SqlParameter("@MilSCH", millingMasterModel.SCH);
                    parameter[33] = new SqlParameter("@MilRecQty", millingMasterModel.PCSWeight);
                    parameter[34] = new SqlParameter("@MilNBVou", millingMasterModel.NbVou);
                    parameter[35] = new SqlParameter("@MilSCHVou", millingMasterModel.SCHVou);
                    parameter[36] = new SqlParameter("@MilRLPcs", millingMasterModel.RLPCS);
                    parameter[37] = new SqlParameter("@MilRLWeight", millingMasterModel.RLWeight);
                    parameter[38] = new SqlParameter("@MilPrcVou", millingMasterModel.PrcVou);
                    parameter[39] = new SqlParameter("@MIlFinishDt", millingMasterModel.FinishDate);
                    parameter[40] = new SqlParameter("@MilReason", millingMasterModel.Reason);
                    parameter[41] = new SqlParameter("@MilRemainingWeight", millingMasterModel.RemainingWeight);
                    DataTable dt = ObjDBConnection.CallStoreProcedure("AddMilling", parameter);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int status = Convert.ToInt32(dt.Rows[0][0].ToString());
                        if (status > 0)
                        {
                            SetSuccessMessage("Milling master record inserted successfully");
                            return RedirectToAction("Index", "Milling", new { id = 0 });
                        }
                        else
                        {
                            SetErrorMessage("Milling master record not inserted!");
                            return View(millingMasterModel);
                        }

                    }
                    else
                    {
                        SetErrorMessage("Milling master record not inserted!");
                        return View(millingMasterModel);
                    }

                //}
                //else
                //{

                //}
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(millingMasterModel);
        }

        public IActionResult Delete(int id)
        {
            try
            {
                SqlParameter[] parameter = new SqlParameter[1];
                parameter[0] = new SqlParameter("@MilVou", id);
                DataTable dt = ObjDBConnection.CallStoreProcedure("DeleteMillingMaster", parameter);
                SetSuccessMessage("Milling record deleted succesfully!");
            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("Index", "Milling", new { id = 0 });
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
            ViewBag.processList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
            ViewBag.milprocessList = ObjAccountMasterHelpers.GetMachineMasterDropdown(companyId);
            ViewBag.nbList = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
            ViewBag.schList = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);

        }

        public ActionResult GetDataByCoilNo(string coilNo, string mildt,string MacId)
        {
            try
            {
                MillingMasterModel millingMasterModel = new MillingMasterModel();
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

                        if (Convert.ToDateTime(Convert.ToDateTime(dtLotMst.Rows[0]["LotDt"].ToString()).ToString("yyyy-MM-dd"))  >= Convert.ToDateTime(mildt))
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

                    
                    if (dtMacMst != null && dtMacMst.Rows.Count > 0)
                    {
                        if (!(Convert.ToDecimal(dtLotMst.Rows[0]["LotOD"].ToString()) >= Convert.ToDecimal(dtMacMst.Rows[0]["MACSIZERNGFR"].ToString()) && Convert.ToDecimal(dtLotMst.Rows[0]["LotOD"].ToString()) <= Convert.ToDecimal(dtMacMst.Rows[0]["MACSIZERNGTO"].ToString())))
                        {
                            IsValidOD = 0;
                            //return Json(new { result = false, message = "Coil OD Size is Not Range to this Machine!" });
                        }
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
                        return Json(new { result = true, data = data, IsValidOD = IsValidOD });
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
                DataTable dtVoucherNo = ObjDBConnection.CallStoreProcedure("GetMillingVoucherNo", null);
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
                    string currentURL = "/Milling/Index";
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
                    getReportDataModel.ControllerName = "Milling";
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
                    var bytes = Excel(getReportDataModel, "Milling", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "JobWorkEntry.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Job Work Entry", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "JobWorkEntry.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public JsonResult GetNBSCH(string thick, string od)
        {
            OpeningStokModel obj = new OpeningStokModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(thick) && !string.IsNullOrWhiteSpace(od))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@thickpipe", thick);
                    sqlParameters[1] = new SqlParameter("@odpipe", od);
                    DataTable dtNBSCH = ObjDBConnection.CallStoreProcedure("GetNBSCHDetails", sqlParameters);
                    if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
                    {
                        string LotNB = dtNBSCH.Rows[0]["OrdNB"].ToString();
                        string LotSCH = dtNBSCH.Rows[0]["OrdSCH"].ToString();
                        return Json(new { result = true, lotNB = LotNB, lotSCH = LotSCH });
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { result = false });
        }

        public ActionResult GetSupEmpByShiftMacNo(string shift, string macNo)
        {
            SqlParameter[] parameter = new SqlParameter[2];
            parameter[0] = new SqlParameter("@Macno", macNo);
            parameter[1] = new SqlParameter("@Shift", shift);
            DataTable dt = ObjDBConnection.CallStoreProcedure("GetSupByShiftMacNo", parameter);
            if (dt != null && dt.Rows.Count > 0)
            {
                return Json(new { result = true, Sup1 = dt.Rows[0]["Sup"].ToString(), Opr1 = dt.Rows[0]["Opr2"].ToString(), Opr2 = dt.Rows[0]["Opr2"].ToString(), FrSize = dt.Rows[0]["FromSize"].ToString(), ToSize = dt.Rows[0]["ToSize"].ToString() });
            }
            else
            {
                return Json(new { result = false });
            }
        }

        public ActionResult GetMachineWiseMilling(string macNo)
        {
            SqlParameter[] parameter = new SqlParameter[1];
            parameter[0] = new SqlParameter("@Macno", macNo);
            DataTable dt = ObjDBConnection.CallStoreProcedure("GetMachineWiseMilling", parameter);
            if (dt != null && dt.Columns.Count > 1)
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@Macno", macNo);
                param[1] = new SqlParameter("@LotVou", dt.Rows[0]["MilLotVou"].ToString());
                DataTable db = ObjDBConnection.CallStoreProcedure("GetMachine-LotWiseQty", param);
                if (db != null && db.Rows.Count > 0)
                {
                    return Json(new { result = true, Opr1EmpVou = dt.Rows[0]["MilOpr1EmpVou"].ToString(), Opr1EMpNm = dt.Rows[0]["Operator1"].ToString(), Opr2EmpVou = dt.Rows[0]["MilOpr2EmpVou"].ToString(), Opr2EMpNm = dt.Rows[0]["Operator2"].ToString(), LotVou = dt.Rows[0]["MilLotVou"].ToString(), LotNo = dt.Rows[0]["MilLotNo"].ToString(), OD = dt.Rows[0]["MilOD"].ToString(), LenFeet = dt.Rows[0]["MilLenFeet"].ToString(), InTime = dt.Rows[0]["MilInTime"].ToString(), OutTime = dt.Rows[0]["MilOutTime"].ToString(), Pcs = dt.Rows[0]["MilPCS"].ToString(), Qty = dt.Rows[0]["MilQty"].ToString(), Product = dt.Rows[0]["MilRecPrdVou"].ToString(), ScrLenFeet = dt.Rows[0]["MilScrLenFeet"].ToString(), ScrQty = dt.Rows[0]["MilScrQty"].ToString(), ScrPrdVou = dt.Rows[0]["MilScrPrdVou"].ToString(), ScrapPrd = dt.Rows[0]["ScrapPrd"].ToString(), TouNo = dt.Rows[0]["MilTouNo"].ToString(), WeldSpeed = dt.Rows[0]["MilWeldSpeed"].ToString(), WeldAMP = dt.Rows[0]["MilWeldAMP"].ToString(), WeldVolt = dt.Rows[0]["MilWeldVolt"].ToString(), Rem = dt.Rows[0]["MilRem"].ToString(), RedThick1 = dt.Rows[0]["MilRead1Thick"].ToString(), RedOD1 = dt.Rows[0]["MilRead1OD"].ToString(), RedThick2 = dt.Rows[0]["MilRead2Thick"].ToString(), RedOD2 = dt.Rows[0]["MilRead2OD"].ToString(), SupEmpVou = dt.Rows[0]["MilSupEmpVou"].ToString(), Supervisor = dt.Rows[0]["Supervisor"].ToString(), NextPrcVou = dt.Rows[0]["MilNextPrcVou"].ToString(), NextProce = dt.Rows[0]["NextProce"].ToString(), Nb = dt.Rows[0]["MilNB"].ToString(), NBVou = dt.Rows[0]["MilNBVou"].ToString(), Sch = dt.Rows[0]["MilSCH"].ToString(), SCHVou = dt.Rows[0]["MilSCHVou"].ToString(), RlPcs = dt.Rows[0]["MilRlPcs"].ToString(), RLWeight = dt.Rows[0]["MilRlWeight"].ToString(), PrcVou = dt.Rows[0]["MilPrcVou"].ToString(), CurrProce = dt.Rows[0]["CurrProce"].ToString(), Grade = dt.Rows[0]["LotGrade"].ToString(), Width = dt.Rows[0]["LotWidth"].ToString(), Thick = dt.Rows[0]["LotThick"].ToString(), LotQty = dt.Rows[0]["LotQty"].ToString(), MilQtyS = db.Rows[0]["MilQtyS"].ToString(), MilPcsS = db.Rows[0]["MilPcsS"].ToString(), MilScrQty = db.Rows[0]["ScrQty"].ToString() });
                }
                else
                {
                    return Json(new { result = false });
                }
            }
            else
            {
                return Json(new { result = false });
            }
        }

    }
}
