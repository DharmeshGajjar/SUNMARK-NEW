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
                PicklingMasterModel picklingMasterModel = new PicklingMasterModel();
                picklingMasterModel.Pikling = new PikGridModel();
                picklingMasterModel.Pikling.RecProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "PIPE");
                picklingMasterModel.Pikling.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                picklingMasterModel.Pikling.StatusList = objProductHelper.GetPicklingStatus();
                picklingMasterModel.Vno = GetVoucherNo();
                if (id > 0)
                {
                    SqlParameter[] parameter = new SqlParameter[2];
                    parameter[0] = new SqlParameter("@PikVou", id);
                    parameter[1] = new SqlParameter("@Flg", 0);
                    DataTable dt = ObjDBConnection.CallStoreProcedure("GetAnnealingMasterById", parameter);
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
                        picklingMasterModel.NextProc = dt.Rows[0]["PikNextProc"].ToString();
                        picklingMasterModel.NextPrcVou = dt.Rows[0]["PikNextPrcVou"].ToString();
                        picklingMasterModel.HFQty = dt.Rows[0]["PikHFQty"].ToString();
                        picklingMasterModel.Remarks = dt.Rows[0]["PikRemarks"].ToString();

                        picklingMasterModel.Pikling.Grade = new string[dt.Rows.Count];
                        picklingMasterModel.Pikling.PikOD = new decimal[dt.Rows.Count];
                        picklingMasterModel.Pikling.PikThick = new decimal[dt.Rows.Count];
                        picklingMasterModel.Pikling.PikLength = new decimal[dt.Rows.Count];
                        picklingMasterModel.Pikling.PikNoOfPipe = new decimal[dt.Rows.Count];
                        picklingMasterModel.Pikling.PikWeight = new decimal[dt.Rows.Count];
                        picklingMasterModel.Pikling.RecProduct = new string[dt.Rows.Count];
                        picklingMasterModel.Pikling.PikInTime = new string[dt.Rows.Count];
                        picklingMasterModel.Pikling.PikOutTime = new string[dt.Rows.Count];
                        picklingMasterModel.Pikling.PikCoilNo = new string[dt.Rows.Count];
                        picklingMasterModel.Pikling.PikStatus = new string[dt.Rows.Count];
                        picklingMasterModel.Pikling.PikType = new string[dt.Rows.Count];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            picklingMasterModel.Pikling.Grade[i] = dt.Rows[i]["PikAGrdVou"].ToString();
                            picklingMasterModel.Pikling.PikOD[i] = Convert.ToDecimal(dt.Rows[i]["PikAOD"].ToString());
                            picklingMasterModel.Pikling.PikThick[i] = Convert.ToDecimal(dt.Rows[i]["PikAThick"].ToString());
                            picklingMasterModel.Pikling.PikLength[i] = Convert.ToDecimal(dt.Rows[i]["PikALength"].ToString());
                            picklingMasterModel.Pikling.PikNoOfPipe[i] = Convert.ToDecimal(dt.Rows[i]["PikANoOfPipe"].ToString());
                            picklingMasterModel.Pikling.PikWeight[i] = Convert.ToDecimal(dt.Rows[i]["PikAWeight"].ToString());
                            picklingMasterModel.Pikling.RecProduct[i] = dt.Rows[i]["PikARecPrdVou"].ToString();
                            picklingMasterModel.Pikling.PikInTime[i] = dt.Rows[i]["PikAInTime"].ToString();
                            picklingMasterModel.Pikling.PikOutTime[i] = dt.Rows[i]["PikAOutTime"].ToString();
                            picklingMasterModel.Pikling.PikCoilNo[i] = dt.Rows[i]["PikACoilNo"].ToString();
                            picklingMasterModel.Pikling.PikStatus[i] = dt.Rows[i]["PikAStatus"].ToString();
                            picklingMasterModel.Pikling.PikType[i] = dt.Rows[i]["PikAType"].ToString();
                        }
                    }
                }
                return View(picklingMasterModel);
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(long id, PicklingMasterModel picklingMaster)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(picklingMaster.Vno).ToString()) && !string.IsNullOrWhiteSpace(picklingMaster.Date) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(picklingMaster.PikCmpVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(picklingMaster.MachineNo).ToString()) && picklingMaster.Pikling.RecProduct.Length > 0 && picklingMaster.Pikling.Grade.Length > 0 && picklingMaster.Pikling.PikLength.Length > 0 && picklingMaster.Pikling.PikOD.Length > 0 && picklingMaster.Pikling.PikWeight.Length > 0 && picklingMaster.Pikling.PikNoOfPipe.Length > 0 && picklingMaster.Pikling.PikThick.Length > 0)
                {
                    SqlParameter[] sqlParameter = new SqlParameter[16];
                    sqlParameter[0] = new SqlParameter("@AnnVou", picklingMaster.PikVou);
                    sqlParameter[1] = new SqlParameter("@AnnCmpVou", picklingMaster.PikCmpVou);
                    sqlParameter[2] = new SqlParameter("@AnnVno", picklingMaster.Vno);
                    sqlParameter[3] = new SqlParameter("@AnnDt", picklingMaster.Date);
                    sqlParameter[4] = new SqlParameter("@AnnShift", picklingMaster.Shift);
                    sqlParameter[5] = new SqlParameter("@AnnMacNo", picklingMaster.MachineNo);
                    sqlParameter[6] = new SqlParameter("@AnnSupEmpVou", picklingMaster.SupEmpVou);
                    sqlParameter[7] = new SqlParameter("@AnnManEmpVou", picklingMaster.ManEmpVou);
                    sqlParameter[8] = new SqlParameter("@AnnIssPrdVou", picklingMaster.IssuePrdVou);
                    sqlParameter[9] = new SqlParameter("@AnnFinish", picklingMaster.Finish);
                    sqlParameter[10] = new SqlParameter("@AnnFinishVou", picklingMaster.FinishVou);
                    sqlParameter[11] = new SqlParameter("@AnnHFQty", picklingMaster.HFQty);
                    sqlParameter[12] = new SqlParameter("@AnnRemarks", picklingMaster.Remarks);
                    sqlParameter[13] = new SqlParameter("@AnnNextPrcVou", picklingMaster.NextPrcVou);
                    sqlParameter[14] = new SqlParameter("@AnnNextProc", picklingMaster.NextProc);
                    sqlParameter[15] = new SqlParameter("@Flg", "1");
                    DataTable dt = ObjDBConnection.CallStoreProcedure("InsertAnnealing", sqlParameter);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int masterId = DbConnection.ParseInt32(dt.Rows[0][0].ToString());
                        if (masterId > 0)
                        {
                            for (int i = 0; i < picklingMaster.Pikling.PikWeight.Length; i++)
                            {
                                SqlParameter[] sqlParam = new SqlParameter[15];
                                sqlParam[0] = new SqlParameter("@AnnAAnnVou", masterId);
                                sqlParam[1] = new SqlParameter("@AnnCmpVou", picklingMaster.PikCmpVou);
                                sqlParam[2] = new SqlParameter("@AnnGrdVou", picklingMaster.Pikling.Grade[i]);
                                sqlParam[3] = new SqlParameter("@AnnThick", picklingMaster.Pikling.PikThick[i]);
                                sqlParam[4] = new SqlParameter("@AnnOD", picklingMaster.Pikling.PikOD[i]);
                                sqlParam[5] = new SqlParameter("@AnnLength", picklingMaster.Pikling.PikLength[i]);
                                sqlParam[6] = new SqlParameter("@AnnNoOfPipe", picklingMaster.Pikling.PikNoOfPipe[i]);
                                sqlParam[7] = new SqlParameter("@AnnQty", picklingMaster.Pikling.PikWeight[i]);
                                sqlParam[8] = new SqlParameter("@AnnRecPrdVou", picklingMaster.Pikling.RecProduct[i]);
                                sqlParam[9] = new SqlParameter("@AnnInTime", picklingMaster.Pikling.PikInTime[i + 1]);
                                sqlParam[10] = new SqlParameter("@AnnOutTime", picklingMaster.Pikling.PikOutTime[i + 1]);
                                sqlParam[11] = new SqlParameter("@AnnCoilNo", picklingMaster.Pikling.PikCoilNo[i + 1]);
                                sqlParam[12] = new SqlParameter("@AnnStatus", picklingMaster.Pikling.PikStatus[i]);
                                sqlParam[13] = new SqlParameter("@AnnType", picklingMaster.Pikling.PikType[i + 1]);
                                sqlParam[14] = new SqlParameter("@AnnSrNo", (i + 1));
                                DataTable dttrn = ObjDBConnection.CallStoreProcedure("InsertAnnealingTrn", sqlParam);
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
        public IActionResult GetLotIssPiklingProduct(int recProdId, int pikvou, int gradeId, decimal od, decimal thick, string dt, int gsrno)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                SqlParameter[] sqlParameters = new SqlParameter[8];
                sqlParameters[0] = new SqlParameter("@AnnVou", pikvou);
                sqlParameters[1] = new SqlParameter("@RecProd", recProdId);
                sqlParameters[2] = new SqlParameter("@Grade", gradeId);
                sqlParameters[3] = new SqlParameter("@od", od);
                sqlParameters[4] = new SqlParameter("@thick", thick);
                sqlParameters[5] = new SqlParameter("@dt", dt);
                sqlParameters[6] = new SqlParameter("@FLG", "2");
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
