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
    public class AnnealingController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        ProductHelpers objProductHelper = new ProductHelpers();

        public IActionResult Index(int id)
        {
            AnnealingMasterModel annealingMasterModel = new AnnealingMasterModel();
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                annealingMasterModel.Vno = GetVoucherNo();
                if (id > 0)
                {
                    SqlParameter[] parameter = new SqlParameter[2];
                    parameter[0] = new SqlParameter("@AnnVou", id);
                    parameter[1] = new SqlParameter("@Flg", 0);
                    DataTable dt = ObjDBConnection.CallStoreProcedure("GetAnnealingMasterById", parameter);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        annealingMasterModel.AnnVou = id;
                        annealingMasterModel.Vno = dt.Rows[0]["AnnVno"].ToString(); ;
                        annealingMasterModel.AnnCmpVou = Convert.ToInt32(dt.Rows[0]["AnnCmpVou"].ToString());
                        annealingMasterModel.Date = Convert.ToDateTime(dt.Rows[0]["AnnDt"]).ToString("yyyy-MM-dd");
                        annealingMasterModel.Shift = dt.Rows[0]["AnnShift"].ToString();
                        annealingMasterModel.MachineNo = dt.Rows[0]["AnnMacno"].ToString();
                        annealingMasterModel.SupEmpVou = dt.Rows[0]["AnnSupEmpVou"].ToString();
                        annealingMasterModel.ManEmpVou = dt.Rows[0]["AnnManEmpVou"].ToString();
                        annealingMasterModel.IssuePrdVou = dt.Rows[0]["AnnIssPrdVou"].ToString();
                        annealingMasterModel.Finish = dt.Rows[0]["AnnFinish"].ToString();
                        annealingMasterModel.FinishVou = dt.Rows[0]["AnnFinishVou"].ToString();
                        annealingMasterModel.Grade = dt.Rows[0]["AnnGrade"].ToString();
                        annealingMasterModel.GradeVou = dt.Rows[0]["AnnGrdVou"].ToString();
                        annealingMasterModel.Width = dt.Rows[0]["AnnWidth"].ToString();
                        annealingMasterModel.Thick = dt.Rows[0]["AnnThick"].ToString();
                        annealingMasterModel.OD = dt.Rows[0]["AnnOD"].ToString();
                        annealingMasterModel.NoOfPipe = dt.Rows[0]["AnnNoOfPipe"].ToString();
                        annealingMasterModel.Weight = dt.Rows[0]["AnnQty"].ToString();
                        annealingMasterModel.RecPrdVou = dt.Rows[0]["AnnRecPrdVou"].ToString();
                        annealingMasterModel.InTime = dt.Rows[0]["AnnInTime"].ToString();
                        annealingMasterModel.OutTime = dt.Rows[0]["AnnOutTime"].ToString();
                        annealingMasterModel.OilLevel = dt.Rows[0]["AnnOilLevel"].ToString();
                        annealingMasterModel.LDOQty = dt.Rows[0]["AnnLDOQty"].ToString();
                        annealingMasterModel.Hours = dt.Rows[0]["AnnHours"].ToString();
                        annealingMasterModel.RPM = dt.Rows[0]["AnnRPM"].ToString();
                        annealingMasterModel.Remarks = dt.Rows[0]["AnnRemarks"].ToString();
                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(annealingMasterModel);
        }

        [HttpPost]
        public IActionResult Index(AnnealingMasterModel annealingMasterModel)
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
                sqlParameter[0] = new SqlParameter("@AnnVou", annealingMasterModel.AnnVou);
                sqlParameter[1] = new SqlParameter("@AnnCmpVou", annealingMasterModel.AnnCmpVou);
                sqlParameter[2] = new SqlParameter("@AnnVno", annealingMasterModel.Vno);
                sqlParameter[3] = new SqlParameter("@AnnDt", annealingMasterModel.Date);
                sqlParameter[4] = new SqlParameter("@AnnShift", annealingMasterModel.Shift);
                sqlParameter[5] = new SqlParameter("@AnnMacNo", annealingMasterModel.MachineNo);
                sqlParameter[6] = new SqlParameter("@AnnSupEmpVou", annealingMasterModel.SupEmpVou);
                sqlParameter[7] = new SqlParameter("@AnnManEmpVou", annealingMasterModel.ManEmpVou);
                sqlParameter[8] = new SqlParameter("@AnnIssPrdVou", annealingMasterModel.IssuePrdVou);
                sqlParameter[9] = new SqlParameter("@AnnFinish", annealingMasterModel.Finish);
                sqlParameter[10] = new SqlParameter("@AnnFinishVou", annealingMasterModel.FinishVou);
                sqlParameter[11] = new SqlParameter("@AnnGrade", annealingMasterModel.Grade);
                sqlParameter[12] = new SqlParameter("@AnnGrdVou", annealingMasterModel.GradeVou);
                sqlParameter[13] = new SqlParameter("@AnnWidth", annealingMasterModel.Width);
                sqlParameter[14] = new SqlParameter("@AnnThick", annealingMasterModel.Thick);
                sqlParameter[15] = new SqlParameter("@AnnOD", annealingMasterModel.OD);
                sqlParameter[16] = new SqlParameter("@AnnNoOfPipe", annealingMasterModel.NoOfPipe);
                sqlParameter[17] = new SqlParameter("@AnnQty", annealingMasterModel.NoOfPipe);
                sqlParameter[18] = new SqlParameter("@AnnRecPrdVou", annealingMasterModel.RecPrdVou);
                sqlParameter[19] = new SqlParameter("@AnnInTime", annealingMasterModel.InTime);
                sqlParameter[20] = new SqlParameter("@AnnOutTime", annealingMasterModel.OutTime);
                sqlParameter[21] = new SqlParameter("@AnnOilLevel", annealingMasterModel.OilLevel);
                sqlParameter[22] = new SqlParameter("@AnnLDOQty", annealingMasterModel.LDOQty);
                sqlParameter[23] = new SqlParameter("@AnnHours", annealingMasterModel.Hours);
                sqlParameter[24] = new SqlParameter("@AnnRPM", annealingMasterModel.RPM);
                sqlParameter[25] = new SqlParameter("@AnnRemarks", annealingMasterModel.Remarks);
                DataTable dt = ObjDBConnection.CallStoreProcedure("InsertAnnealing", sqlParameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int annVou = Convert.ToInt32(dt.Rows[0][0].ToString());
                    if (annVou > 0 && annealingMasterModel.AnnVou > 0)
                    {
                        SetSuccessMessage("Record updated succesfully!");
                    }
                    else if (annVou > 0 && annealingMasterModel.AnnVou <= 0)
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
            ViewBag.productList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "PIPE"); ;
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
                parameter[0] = new SqlParameter("@AnnVou", id);
                DataTable dt = ObjDBConnection.CallStoreProcedure("DeleteAnnealingMaster", parameter);
                SetSuccessMessage("Annealing record deleted succesfully!");
            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("Index", "Annealing", new { id = 0 });
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
                    string currentURL = "/Annealing/Index";
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
                    getReportDataModel.ControllerName = "Annealing";
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
                    var bytes = Excel(getReportDataModel, "Annealing", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Annealing.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Annealing", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Annealing.pdf");
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
                DataTable dtVoucherNo = ObjDBConnection.CallStoreProcedure("GetAnnealingVoucherNo", null);
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
