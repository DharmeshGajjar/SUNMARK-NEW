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
    public class PicklingController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        ProductHelpers objProductHelper = new ProductHelpers();

        public IActionResult Index(int id)
        {
            PicklingMasterModel picklingMasterModel = new PicklingMasterModel();
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                picklingMasterModel.Vno = GetVoucherNo();
                if (id > 0)
                {
                    SqlParameter[] parameter = new SqlParameter[2];
                    parameter[0] = new SqlParameter("@PikVou", id);
                    parameter[1] = new SqlParameter("@Flg", 0);
                    DataTable dt = ObjDBConnection.CallStoreProcedure("GetPicklingMasterById", parameter);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        picklingMasterModel.PikVou = id;
                        picklingMasterModel.Vno = dt.Rows[0]["PikVno"].ToString(); ;
                        picklingMasterModel.PikCmpVou = Convert.ToInt32(dt.Rows[0]["PikCmpVou"].ToString());
                        picklingMasterModel.Date = Convert.ToDateTime(dt.Rows[0]["PikDt"]).ToString("yyyy-MM-dd");
                        picklingMasterModel.Shift = dt.Rows[0]["PikShift"].ToString();
                        picklingMasterModel.MachineNo = dt.Rows[0]["PikMacno"].ToString();
                        picklingMasterModel.SupEmpVou = dt.Rows[0]["PikSupEmpVou"].ToString();
                        picklingMasterModel.ManEmpVou = dt.Rows[0]["PikManEmpVou"].ToString();
                        picklingMasterModel.IssuePrdVou = dt.Rows[0]["PikIssPrdVou"].ToString();
                        picklingMasterModel.Finish = dt.Rows[0]["PikFinish"].ToString();
                        picklingMasterModel.FinishVou = dt.Rows[0]["PikFinishVou"].ToString();
                        picklingMasterModel.Grade = dt.Rows[0]["PikGrade"].ToString();
                        picklingMasterModel.GradeVou = dt.Rows[0]["PikGrdVou"].ToString();
                        picklingMasterModel.Width = dt.Rows[0]["PikWidth"].ToString();
                        picklingMasterModel.Thick = dt.Rows[0]["PikThick"].ToString();
                        picklingMasterModel.OD = dt.Rows[0]["PikOD"].ToString();
                        picklingMasterModel.NoOfPipe = dt.Rows[0]["PikPCS"].ToString();
                        picklingMasterModel.Weight = dt.Rows[0]["PikQty"].ToString();
                        picklingMasterModel.RecPrdVou = dt.Rows[0]["PikRecPrdVou"].ToString();
                        picklingMasterModel.InTime = dt.Rows[0]["PikInTime"].ToString();
                        picklingMasterModel.OutTime = dt.Rows[0]["PikOutTime"].ToString();
                        picklingMasterModel.HFQty = dt.Rows[0]["PikHFQty"].ToString();
                        picklingMasterModel.NitricQty = dt.Rows[0]["PikNitricQty"].ToString();
                        picklingMasterModel.LimeQty = dt.Rows[0]["PikLimeQty"].ToString();
                        picklingMasterModel.RPM = dt.Rows[0]["PikRPM"].ToString();
                        picklingMasterModel.Remarks = dt.Rows[0]["PikRemarks"].ToString();
                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(picklingMasterModel);
        }

        [HttpPost]
        public IActionResult Index(PicklingMasterModel picklingMasterModel)
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
                sqlParameter[0] = new SqlParameter("@PikVou", picklingMasterModel.PikVou);
                sqlParameter[1] = new SqlParameter("@PikCmpVou", picklingMasterModel.PikCmpVou);
                sqlParameter[2] = new SqlParameter("@PikVno", picklingMasterModel.Vno);
                sqlParameter[3] = new SqlParameter("@PikDt", picklingMasterModel.Date);
                sqlParameter[4] = new SqlParameter("@PikShift", picklingMasterModel.Shift);
                sqlParameter[5] = new SqlParameter("@PikMacNo", picklingMasterModel.MachineNo);
                sqlParameter[6] = new SqlParameter("@PikSupEmpVou", picklingMasterModel.SupEmpVou);
                sqlParameter[7] = new SqlParameter("@PikManEmpVou", picklingMasterModel.ManEmpVou);
                sqlParameter[8] = new SqlParameter("@PikIssPrdVou", picklingMasterModel.IssuePrdVou);
                sqlParameter[9] = new SqlParameter("@PikFinish", picklingMasterModel.Finish);
                sqlParameter[10] = new SqlParameter("@PikFinishVou", picklingMasterModel.FinishVou);
                sqlParameter[11] = new SqlParameter("@PikGrade", picklingMasterModel.Grade);
                sqlParameter[12] = new SqlParameter("@PikGrdVou", picklingMasterModel.GradeVou);
                sqlParameter[13] = new SqlParameter("@PikWidth", picklingMasterModel.Width);
                sqlParameter[14] = new SqlParameter("@PikThick", picklingMasterModel.Thick);
                sqlParameter[15] = new SqlParameter("@PikOD", picklingMasterModel.OD);
                sqlParameter[16] = new SqlParameter("@PikNoOfPipe", picklingMasterModel.NoOfPipe);
                sqlParameter[17] = new SqlParameter("@PikQty", picklingMasterModel.NoOfPipe);
                sqlParameter[18] = new SqlParameter("@PikRecPrdVou", picklingMasterModel.RecPrdVou);
                sqlParameter[19] = new SqlParameter("@PikInTime", picklingMasterModel.InTime);
                sqlParameter[20] = new SqlParameter("@PikOutTime", picklingMasterModel.OutTime);
                sqlParameter[21] = new SqlParameter("@PikHFQty", picklingMasterModel.HFQty);
                sqlParameter[22] = new SqlParameter("@PikNitricQty", picklingMasterModel.NitricQty);
                sqlParameter[23] = new SqlParameter("@PikLimeQty", picklingMasterModel.LimeQty);
                sqlParameter[24] = new SqlParameter("@PikRPM", picklingMasterModel.RPM);
                sqlParameter[25] = new SqlParameter("@PikRemarks", picklingMasterModel.Remarks);
                DataTable dt = ObjDBConnection.CallStoreProcedure("InsertPickling", sqlParameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int pikVou = Convert.ToInt32(dt.Rows[0][0].ToString());
                    if (pikVou > 0 && picklingMasterModel.PikVou > 0)
                    {
                        SetSuccessMessage("Record updated succesfully!");
                    }
                    else if (pikVou > 0 && picklingMasterModel.PikVou <= 0)
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
                parameter[0] = new SqlParameter("@PikVou", id);
                DataTable dt = ObjDBConnection.CallStoreProcedure("DeletePicklingMaster", parameter);
                SetSuccessMessage("Pickling record deleted succesfully!");
            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("Index", "Pickling", new { id = 0 });
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
                    string currentURL = "/Pickling/Index";
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
                    getReportDataModel.ControllerName = "Pickling";
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
                    var bytes = Excel(getReportDataModel, "Pickling", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Pickling.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Pickling", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Pickling.pdf");
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
    }
}
