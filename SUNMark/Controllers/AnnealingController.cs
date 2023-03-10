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
                AnnealingMasterModel annealingMasterModel = new AnnealingMasterModel();
                annealingMasterModel.Annel = new AnnelGridModel();
                annealingMasterModel.Annel.RecProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "PIPE");
                annealingMasterModel.Annel.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
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
                        annealingMasterModel.NextProc = dt.Rows[0]["AnnNextProc"].ToString();
                        annealingMasterModel.NextPrcVou = dt.Rows[0]["AnnNextPrcVou"].ToString();
                        annealingMasterModel.LDOQty = dt.Rows[0]["AnnLDOQty"].ToString();
                        annealingMasterModel.Remarks = dt.Rows[0]["AnnRemarks"].ToString();

                        annealingMasterModel.Annel.Grade = new string[dt.Rows.Count];
                        annealingMasterModel.Annel.AnnOD = new decimal[dt.Rows.Count];
                        annealingMasterModel.Annel.AnnThick = new decimal[dt.Rows.Count];
                        annealingMasterModel.Annel.AnnLength = new decimal[dt.Rows.Count];
                        annealingMasterModel.Annel.AnnNoOfPipe = new decimal[dt.Rows.Count];
                        annealingMasterModel.Annel.AnnWeight = new decimal[dt.Rows.Count];
                        annealingMasterModel.Annel.RecProduct = new string[dt.Rows.Count];
                        annealingMasterModel.Annel.AnnInTime = new string[dt.Rows.Count];
                        annealingMasterModel.Annel.AnnOutTime = new string[dt.Rows.Count];
                        annealingMasterModel.Annel.AnnCoilNo = new string[dt.Rows.Count];
                        annealingMasterModel.Annel.AnnRPM = new decimal[dt.Rows.Count];
                        annealingMasterModel.Annel.AnnType = new string[dt.Rows.Count];

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            annealingMasterModel.Annel.Grade[i] = dt.Rows[i]["AnnAGrdVou"].ToString();
                            annealingMasterModel.Annel.AnnOD[i] = Convert.ToDecimal(dt.Rows[i]["AnnAOD"].ToString());
                            annealingMasterModel.Annel.AnnThick[i] = Convert.ToDecimal(dt.Rows[i]["AnnAThick"].ToString());
                            annealingMasterModel.Annel.AnnLength[i] = Convert.ToDecimal(dt.Rows[i]["AnnALength"].ToString());
                            annealingMasterModel.Annel.AnnNoOfPipe[i] = Convert.ToDecimal(dt.Rows[i]["AnnANoOfPipe"].ToString());
                            annealingMasterModel.Annel.AnnWeight[i] = Convert.ToDecimal(dt.Rows[i]["AnnAWeight"].ToString());
                            annealingMasterModel.Annel.RecProduct[i] = dt.Rows[i]["AnnARecPrdVou"].ToString();
                            annealingMasterModel.Annel.AnnInTime[i] = dt.Rows[i]["AnnAInTime"].ToString();
                            annealingMasterModel.Annel.AnnOutTime[i] = dt.Rows[i]["AnnAOutTime"].ToString();
                            annealingMasterModel.Annel.AnnCoilNo[i] = dt.Rows[i]["AnnACoilNo"].ToString();
                            annealingMasterModel.Annel.AnnRPM[i] = Convert.ToDecimal(dt.Rows[i]["AnnARPM"].ToString());
                            annealingMasterModel.Annel.AnnType[i] = dt.Rows[i]["AnnAType"].ToString();
                        }
                    }
                }
                return View(annealingMasterModel);
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(long id, AnnealingMasterModel annealingMasterModel)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(annealingMasterModel.Vno).ToString()) && !string.IsNullOrWhiteSpace(annealingMasterModel.Date) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(annealingMasterModel.AnnCmpVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(annealingMasterModel.MachineNo).ToString()) && annealingMasterModel.Annel.RecProduct.Length > 0 && annealingMasterModel.Annel.Grade.Length > 0 && annealingMasterModel.Annel.AnnLength.Length > 0 && annealingMasterModel.Annel.AnnOD.Length > 0 && annealingMasterModel.Annel.AnnWeight.Length > 0 && annealingMasterModel.Annel.AnnNoOfPipe.Length > 0 && annealingMasterModel.Annel.AnnThick.Length > 0)
                {
                    SqlParameter[] sqlParameter = new SqlParameter[16];
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
                    sqlParameter[11] = new SqlParameter("@AnnLDOQty", annealingMasterModel.LDOQty);
                    sqlParameter[12] = new SqlParameter("@AnnRemarks", annealingMasterModel.Remarks);
                    sqlParameter[13] = new SqlParameter("@AnnNextPrcVou", annealingMasterModel.NextPrcVou);
                    sqlParameter[14] = new SqlParameter("@AnnNextProc", annealingMasterModel.NextProc);
                    sqlParameter[15] = new SqlParameter("@Flg", "1");
                    DataTable dt = ObjDBConnection.CallStoreProcedure("InsertAnnealing", sqlParameter);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int masterId = DbConnection.ParseInt32(dt.Rows[0][0].ToString());
                        if (masterId > 0)
                        {
                            for (int i = 0; i < annealingMasterModel.Annel.AnnWeight.Length; i++)
                            {
                                SqlParameter[] sqlParam = new SqlParameter[15];
                                sqlParam[0] = new SqlParameter("@AnnAAnnVou", masterId);
                                sqlParam[1] = new SqlParameter("@AnnCmpVou", annealingMasterModel.AnnCmpVou);
                                sqlParam[2] = new SqlParameter("@AnnGrdVou", annealingMasterModel.Annel.Grade[i]);
                                sqlParam[3] = new SqlParameter("@AnnThick", annealingMasterModel.Annel.AnnThick[i]);
                                sqlParam[4] = new SqlParameter("@AnnOD", annealingMasterModel.Annel.AnnOD[i]);
                                sqlParam[5] = new SqlParameter("@AnnLength", annealingMasterModel.Annel.AnnLength[i]);
                                sqlParam[6] = new SqlParameter("@AnnNoOfPipe", annealingMasterModel.Annel.AnnNoOfPipe[i]);
                                sqlParam[7] = new SqlParameter("@AnnQty", annealingMasterModel.Annel.AnnWeight[i]);
                                sqlParam[8] = new SqlParameter("@AnnRecPrdVou", annealingMasterModel.Annel.RecProduct[i]);
                                sqlParam[9] = new SqlParameter("@AnnInTime", annealingMasterModel.Annel.AnnInTime[i+1]);
                                sqlParam[10] = new SqlParameter("@AnnOutTime", annealingMasterModel.Annel.AnnOutTime[i+1]);
                                sqlParam[11] = new SqlParameter("@AnnCoilNo", annealingMasterModel.Annel.AnnCoilNo[i+1]);
                                sqlParam[12] = new SqlParameter("@AnnRPM", annealingMasterModel.Annel.AnnRPM[i]);
                                sqlParam[13] = new SqlParameter("@AnnType", annealingMasterModel.Annel.AnnType[i+1]);
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
            ViewBag.productList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "PIPE"); ;
            ViewBag.shiftList = objProductHelper.GetShiftNew(); ;
            ViewBag.milprocessList = ObjAccountMasterHelpers.GetMachineMasterDropdown(companyId);
            
            ViewBag.finishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
            ViewBag.nextProcList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
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
        public IActionResult GetLotIssAnnelProduct(int recProdId, int annvou, int gradeId, decimal od, decimal thick, string dt)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                SqlParameter[] sqlParameters = new SqlParameter[7];
                sqlParameters[0] = new SqlParameter("@AnnVou", annvou);
                sqlParameters[1] = new SqlParameter("@RecProd", recProdId);
                sqlParameters[2] = new SqlParameter("@Grade", gradeId);
                sqlParameters[3] = new SqlParameter("@od", od);
                sqlParameters[4] = new SqlParameter("@thick", thick);
                sqlParameters[5] = new SqlParameter("@dt", dt);
                sqlParameters[6] = new SqlParameter("@FLG", "1");
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
