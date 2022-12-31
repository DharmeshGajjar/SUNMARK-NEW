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
            StraightingMasterModel straightingMasterModel = new StraightingMasterModel();
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                straightingMasterModel.Vno = GetVoucherNo();
                if (id > 0)
                {
                    SqlParameter[] parameter = new SqlParameter[2];
                    parameter[0] = new SqlParameter("@StrVou", id);
                    parameter[1] = new SqlParameter("@Flg", 0);
                    DataTable dt = ObjDBConnection.CallStoreProcedure("GetStraightingMasterById", parameter);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        straightingMasterModel.StrVou = id;
                        straightingMasterModel.Vno = dt.Rows[0]["StrVno"].ToString(); ;
                        straightingMasterModel.StrCmpVou = Convert.ToInt32(dt.Rows[0]["StrCmpVou"].ToString());
                        straightingMasterModel.Date = Convert.ToDateTime(dt.Rows[0]["StrDt"]).ToString("yyyy-MM-dd");
                        straightingMasterModel.Shift = dt.Rows[0]["StrShift"].ToString();
                        straightingMasterModel.MachineNo = dt.Rows[0]["StrMacno"].ToString();
                        straightingMasterModel.SupEmpVou = dt.Rows[0]["StrSupEmpVou"].ToString();
                        straightingMasterModel.ManEmpVou = dt.Rows[0]["StrManEmpVou"].ToString();
                        straightingMasterModel.IssuePrdVou = dt.Rows[0]["StrIssPrdVou"].ToString();
                        straightingMasterModel.Finish = dt.Rows[0]["StrFinish"].ToString();
                        straightingMasterModel.FinishVou = dt.Rows[0]["StrFinishVou"].ToString();
                        straightingMasterModel.Grade = dt.Rows[0]["StrGrade"].ToString();
                        straightingMasterModel.GradeVou = dt.Rows[0]["StrGrdVou"].ToString();
                        straightingMasterModel.Width = dt.Rows[0]["StrWidth"].ToString();
                        straightingMasterModel.Thick = dt.Rows[0]["StrThick"].ToString();
                        straightingMasterModel.OD = dt.Rows[0]["StrOD"].ToString();
                        straightingMasterModel.NoOfPipe = dt.Rows[0]["StrPCS"].ToString();
                        straightingMasterModel.Weight = dt.Rows[0]["StrQty"].ToString();
                        straightingMasterModel.RecPrdVou = dt.Rows[0]["StrRecPrdVou"].ToString();
                        straightingMasterModel.InTime = dt.Rows[0]["StrInTime"].ToString();
                        straightingMasterModel.OutTime = dt.Rows[0]["StrOutTime"].ToString();
                        straightingMasterModel.OilLevel = dt.Rows[0]["StrOilLevel"].ToString();
                        straightingMasterModel.LDOQty = dt.Rows[0]["StrLDOQty"].ToString();
                        straightingMasterModel.Hours = dt.Rows[0]["StrHours"].ToString();
                        straightingMasterModel.RPM = dt.Rows[0]["StrRPM"].ToString();
                        straightingMasterModel.Remarks = dt.Rows[0]["StrRemarks"].ToString();
                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(straightingMasterModel);
        }

        [HttpPost]
        public IActionResult Index(StraightingMasterModel straightingMasterModel)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                SqlParameter[] sqlParameter = new SqlParameter[26];
                sqlParameter[0] = new SqlParameter("@StrVou", straightingMasterModel.StrVou);
                sqlParameter[1] = new SqlParameter("@StrCmpVou", straightingMasterModel.StrCmpVou);
                sqlParameter[2] = new SqlParameter("@StrVno", straightingMasterModel.Vno);
                sqlParameter[3] = new SqlParameter("@StrDt", straightingMasterModel.Date);
                sqlParameter[4] = new SqlParameter("@StrShift", straightingMasterModel.Shift);
                sqlParameter[5] = new SqlParameter("@StrMacNo", straightingMasterModel.MachineNo);
                sqlParameter[6] = new SqlParameter("@StrSupEmpVou", straightingMasterModel.SupEmpVou);
                sqlParameter[7] = new SqlParameter("@StrManEmpVou", straightingMasterModel.ManEmpVou);
                sqlParameter[8] = new SqlParameter("@StrIssPrdVou", straightingMasterModel.IssuePrdVou);
                sqlParameter[9] = new SqlParameter("@StrFinish", straightingMasterModel.Finish);
                sqlParameter[10] = new SqlParameter("@StrFinishVou", straightingMasterModel.FinishVou);
                sqlParameter[11] = new SqlParameter("@StrGrade", straightingMasterModel.Grade);
                sqlParameter[12] = new SqlParameter("@StrGrdVou", straightingMasterModel.GradeVou);
                sqlParameter[13] = new SqlParameter("@StrWidth", straightingMasterModel.Width);
                sqlParameter[14] = new SqlParameter("@StrThick", straightingMasterModel.Thick);
                sqlParameter[15] = new SqlParameter("@StrOD", straightingMasterModel.OD);
                sqlParameter[16] = new SqlParameter("@StrNoOfPipe", straightingMasterModel.NoOfPipe);
                sqlParameter[17] = new SqlParameter("@StrQty", straightingMasterModel.NoOfPipe);
                sqlParameter[18] = new SqlParameter("@StrRecPrdVou", straightingMasterModel.RecPrdVou);
                sqlParameter[19] = new SqlParameter("@StrInTime", straightingMasterModel.InTime);
                sqlParameter[20] = new SqlParameter("@StrOutTime", straightingMasterModel.OutTime);
                sqlParameter[21] = new SqlParameter("@StrOilLevel", straightingMasterModel.OilLevel);
                sqlParameter[22] = new SqlParameter("@StrLDOQty", straightingMasterModel.LDOQty);
                sqlParameter[23] = new SqlParameter("@StrHours", straightingMasterModel.Hours);
                sqlParameter[24] = new SqlParameter("@StrRPM", straightingMasterModel.RPM);
                sqlParameter[25] = new SqlParameter("@StrRemarks", straightingMasterModel.Remarks);
                DataTable dt = ObjDBConnection.CallStoreProcedure("InsertStraighting", sqlParameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int annVou = Convert.ToInt32(dt.Rows[0][0].ToString());
                    if (annVou > 0 && straightingMasterModel.StrVou > 0)
                    {
                        SetSuccessMessage("Record updated succesfully!");
                    }
                    else if (annVou > 0 && straightingMasterModel.StrVou <= 0)
                    {
                        SetSuccessMessage("Record inserted succesfully!");
                    }
                }
                else
                {
                    SetErrorMessage("Record not inserted succesfully!");
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
                    var bytes = PDF(getReportDataModel, "Straighting", companyDetails.CmpName);
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
                DataTable dtVoucherNo = ObjDBConnection.CallStoreProcedure("GetStraightingVoucherNo", null);
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
    }
}
