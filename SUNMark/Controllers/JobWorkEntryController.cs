using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SUNMark.Classes;
using SUNMark.Common;
using SUNMark.Models;

namespace SUNMark.Controllers
{
    public class JobWorkEntryController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        private readonly IWebHostEnvironment _iwebhostenviroment;

        public JobWorkEntryController(IWebHostEnvironment iwebhostenviroment)
        {
            _iwebhostenviroment = iwebhostenviroment;
        }

        public IActionResult Index(long id)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                ViewBag.id = id;
                ViewBag.vNo = GetVoucherJobNo()[1];
                ViewBag.jobNo = GetVoucherJobNo()[0];
                ViewBag.nbDropdown = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
                ViewBag.schDropdown = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);
                ViewBag.godownList = objProductHelper.GetGoDownMasterDropdown(companyId, 0);

                List<SelectListItem> odList = new List<SelectListItem>();
                DataTable dtNBSCH = ObjDBConnection.CallStoreProcedure("GetODDetails", null);
                if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
                {
                    foreach (DataRow item in dtNBSCH.Rows)
                    {
                        odList.Add(new SelectListItem
                        {
                            Text = item[0].ToString(),
                            Value = item[0].ToString()
                        });
                    }
                }
                ViewBag.odList = odList;
                if (id > 0)
                    TempData["ReturnId"] = Convert.ToString(id);
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
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
            ViewBag.lottypeList = objProductHelper.GetLotMasterDropdown_2(companyId, administrator);
            ViewBag.shiftList = objProductHelper.GetShiftNew(); ;
            ViewBag.operatorList = ObjAccountMasterHelpers.GetOperatorCustomDropdown_New(companyId, 0);
            //   ViewBag.machineList = ObjAccountMasterHelpers.GetMachineMasterDropdown(companyId);
        }

        private string[] GetVoucherJobNo()
        {
            string[] returnValue = new string[2];
            try
            {
                DataTable dtVoucherNo = ObjDBConnection.CallStoreProcedure("GetJobVoucherJobNumber", null);
                if (dtVoucherNo != null && dtVoucherNo.Rows.Count > 0)
                {
                    returnValue[0] = dtVoucherNo.Rows[0][0].ToString();
                    returnValue[1] = dtVoucherNo.Rows[0][1].ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return returnValue;
        }

        public ActionResult GetDataByCoilNo(string coilNo)
        {
            try
            {
                SqlParameter[] parameter = new SqlParameter[1];
                parameter[0] = new SqlParameter("@COILNO", coilNo);
                DataSet ds = ObjDBConnection.GetDataSet("GetDataByCoilNo", parameter);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Count == 5)
                {
                    List<object> data = new List<object>();
                    DataTable dtLotMst = ds.Tables[0];
                    DataTable dtDepartment = ds.Tables[1];
                    DataTable dtGodown = ds.Tables[2];
                    DataTable dtProduct = ds.Tables[3];
                    if (dtLotMst != null && dtLotMst.Rows.Count > 0)
                    {
                        data.Add(Convert.ToDateTime(dtLotMst.Rows[0]["LotDt"].ToString()).ToString("yyyy-MM-dd"));
                        data.Add(dtLotMst.Rows[0]["LotFinish"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotGrade"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotGdnVou"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotVou"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotThick"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotQty"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotWidth"].ToString());
                    }
                    else
                    {
                        data.Add("");
                        data.Add("");
                        data.Add("");
                        data.Add("");
                        data.Add("");
                        data.Add("");
                        data.Add("");
                        data.Add("");

                    }
                    if (dtDepartment != null && dtDepartment.Rows.Count > 0)
                    {
                        data.Add(dtDepartment.Rows[0]["DEPNAME"].ToString());
                        data.Add(dtDepartment.Rows[0]["DEPCODE"].ToString());
                    }
                    else
                    {
                        data.Add("");
                        data.Add("");
                    }
                    if (dtGodown != null && dtGodown.Rows.Count > 0)
                    {
                        data.Add(dtGodown.Rows[0]["AccNm"].ToString());
                    }
                    else
                    {
                        data.Add("");
                    }
                    if (dtProduct != null && dtProduct.Rows.Count > 0)
                    {
                        data.Add(dtProduct.Rows[0]["PRDNM"].ToString());
                    }
                    else
                    {
                        data.Add("");
                    }
                    if (data == null || data.Count <= 0)
                    {
                        return Json(new { result = false, message = "Invalid Coil No!" });
                    }
                    else
                    {
                        return Json(new { result = true, data = data });
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

        public ActionResult GetMachineListByJobType(string jobType)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                return Json(new { data = ObjAccountMasterHelpers.GetMachineMasterDropdown(companyId, jobType), result = true });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
        }

        public ActionResult AddJobWorkEntry(JobWorkEntryModel jobWorkEntryModel)
        {
            try
            {
                var jobtype = jobWorkEntryModel.JobType.ToString();
                if (jobtype == "1")
                {
                    if (jobWorkEntryModel.JobType != null && jobWorkEntryModel.JobType.Length > 0 && jobWorkEntryModel.JobType == "1" && jobWorkEntryModel.JobWorkEntrySplitsList != null && jobWorkEntryModel.JobWorkEntrySplitsList.Count > 0)
                    {
                        int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                        SqlParameter[] parameter = new SqlParameter[20];
                        parameter[0] = new SqlParameter("@JOBVOU", jobWorkEntryModel.JobId);
                        parameter[1] = new SqlParameter("@JOBCMPVOU", companyId);
                        parameter[2] = new SqlParameter("@JOBTYPE", jobWorkEntryModel.JobType);
                        parameter[3] = new SqlParameter("@JOBVNO", jobWorkEntryModel.VNo);
                        parameter[4] = new SqlParameter("@JOBDT", jobWorkEntryModel.Date);
                        parameter[5] = new SqlParameter("@JOBCOLVOU", jobWorkEntryModel.IssueCoilNoId);
                        parameter[6] = new SqlParameter("@JOBISSCOILNO", jobWorkEntryModel.IssueCoilNo.ToString().ToUpper());
                        parameter[7] = new SqlParameter("@JOBGDNVOU", jobWorkEntryModel.GodownVou);
                        parameter[8] = new SqlParameter("@JOBQTY", jobWorkEntryModel.Qty);
                        parameter[9] = new SqlParameter("@JOBWIDTH", jobWorkEntryModel.Width);
                        parameter[10] = new SqlParameter("@JOBREM", jobWorkEntryModel.Remarks);
                        parameter[11] = new SqlParameter("@JOBACTQTY", jobWorkEntryModel.ActualQty);
                        parameter[12] = new SqlParameter("@JOBACTWIDTH", jobWorkEntryModel.ActualWidth);
                        parameter[13] = new SqlParameter("@JOBCOMDT", !string.IsNullOrWhiteSpace(jobWorkEntryModel.CompleteDate) ? Convert.ToDateTime(jobWorkEntryModel.CompleteDate).ToString("yyyy-MM-dd") : null);
                        parameter[14] = new SqlParameter("@JOBNO", jobWorkEntryModel.JobNo);
                        parameter[15] = new SqlParameter("@JOBLOTTYPVOU", jobWorkEntryModel.LotTypeVou);
                        parameter[16] = new SqlParameter("@JOBTYPEN", jobWorkEntryModel.JobTypeN);
                        parameter[17] = new SqlParameter("@JOBShift", jobWorkEntryModel.JobShift);
                        parameter[18] = new SqlParameter("@JOBMacNo", jobWorkEntryModel.JobMacNo);
                        parameter[19] = new SqlParameter("@JOBOprEmpVou", jobWorkEntryModel.JobOprMscVou);
                        DataTable dt = ObjDBConnection.CallStoreProcedure("JOBMST_INSERT", parameter);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            int jobVou = Convert.ToInt32(dt.Rows[0][0].ToString());
                            if (jobVou > 0)
                            {
                                parameter = new SqlParameter[9];
                                if (jobWorkEntryModel.JobWorkEntrySplitsList != null && jobWorkEntryModel.JobWorkEntrySplitsList.Count > 0)
                                {
                                    foreach (var item in jobWorkEntryModel.JobWorkEntrySplitsList)
                                    {
                                        parameter[0] = new SqlParameter("@JotCmpVou", companyId);
                                        parameter[1] = new SqlParameter("@JotJobVou", jobVou);
                                        parameter[2] = new SqlParameter("@JotLotVou", 0);
                                        parameter[3] = new SqlParameter("@JotFrmQty", item.FormulaWt);
                                        parameter[4] = new SqlParameter("@JotQty", item.VendorWt);
                                        parameter[5] = new SqlParameter("@JotWidth", 0);
                                        parameter[6] = new SqlParameter("@JotRem", item.Remarks);
                                        parameter[7] = new SqlParameter("@JotRecCoilNo", item.RecCoilNo);
                                        parameter[8] = new SqlParameter("@JotGdnMscVou", item.Godown);
                                        ObjDBConnection.CallStoreProcedure("JobTrn_Insert", parameter);
                                    }
                                }
                                SetSuccessMessage("Job saved successfully.");
                                return Json(new { result = true, message = "Job saved successfully" });
                            }
                            else
                            {
                                return Json(new { result = false, message = "Job work not saved" });
                            }
                        }
                        else
                        {
                            return Json(new { result = false, message = "Job work not saved" });
                        }
                    }
                    else
                    {
                        return Json(new { result = false, message = "Please Entry Value" });
                    }
                }
                else
                {
                    if (jobWorkEntryModel.JobType != null && jobWorkEntryModel.JobType.Length > 0 && jobWorkEntryModel.JobType == "2" && jobWorkEntryModel.JobWorkEntrySlitsList != null && jobWorkEntryModel.JobWorkEntrySlitsList.Count > 0)
                    {
                        int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                        SqlParameter[] parameter = new SqlParameter[20];
                        parameter[0] = new SqlParameter("@JOBVOU", jobWorkEntryModel.JobId);
                        parameter[1] = new SqlParameter("@JOBCMPVOU", companyId);
                        parameter[2] = new SqlParameter("@JOBTYPE", jobWorkEntryModel.JobType);
                        parameter[3] = new SqlParameter("@JOBVNO", jobWorkEntryModel.VNo);
                        parameter[4] = new SqlParameter("@JOBDT", jobWorkEntryModel.Date);
                        parameter[5] = new SqlParameter("@JOBCOLVOU", jobWorkEntryModel.IssueCoilNoId);
                        parameter[6] = new SqlParameter("@JOBISSCOILNO", jobWorkEntryModel.IssueCoilNo.ToUpper());
                        parameter[7] = new SqlParameter("@JOBGDNVOU", jobWorkEntryModel.GodownVou);
                        parameter[8] = new SqlParameter("@JOBQTY", jobWorkEntryModel.Qty);
                        parameter[9] = new SqlParameter("@JOBWIDTH", jobWorkEntryModel.Width);
                        parameter[10] = new SqlParameter("@JOBREM", jobWorkEntryModel.Remarks);
                        parameter[11] = new SqlParameter("@JOBACTQTY", jobWorkEntryModel.ActualQty);
                        parameter[12] = new SqlParameter("@JOBACTWIDTH", jobWorkEntryModel.ActualWidth);
                        parameter[13] = new SqlParameter("@JOBCOMDT", !string.IsNullOrWhiteSpace(jobWorkEntryModel.CompleteDate) ? Convert.ToDateTime(jobWorkEntryModel.CompleteDate).ToString("yyyy-MM-dd") : null);
                        parameter[14] = new SqlParameter("@JOBNO", jobWorkEntryModel.JobNo);
                        parameter[15] = new SqlParameter("@JOBLOTTYPVOU", jobWorkEntryModel.LotTypeVou);
                        parameter[16] = new SqlParameter("@JOBTYPEN", jobWorkEntryModel.JobTypeN);
                        parameter[17] = new SqlParameter("@JOBShift", jobWorkEntryModel.JobShift);
                        parameter[18] = new SqlParameter("@JOBMacNo", jobWorkEntryModel.JobMacNo);
                        parameter[19] = new SqlParameter("@JOBOprEmpVou", jobWorkEntryModel.JobOprMscVou);
                        DataTable dt = ObjDBConnection.CallStoreProcedure("JOBMST_INSERT", parameter);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            int jobVou = Convert.ToInt32(dt.Rows[0][0].ToString());
                            if (jobVou > 0)
                            {
                                parameter = new SqlParameter[13];
                                if (jobWorkEntryModel.JobWorkEntrySlitsList != null && jobWorkEntryModel.JobWorkEntrySlitsList.Count > 0)
                                {
                                    foreach (var item in jobWorkEntryModel.JobWorkEntrySlitsList)
                                    {
                                        parameter[0] = new SqlParameter("@JotCmpVou", companyId);
                                        parameter[1] = new SqlParameter("@JotJobVou", jobVou);
                                        parameter[2] = new SqlParameter("@JotLotVou", 0);
                                        parameter[3] = new SqlParameter("@JotFrmQty", item.FormulaWt);
                                        parameter[4] = new SqlParameter("@JotQty", item.ActualWt);
                                        parameter[5] = new SqlParameter("@JotWidth", item.Width);
                                        parameter[6] = new SqlParameter("@JotRem", item.Remarks);
                                        parameter[7] = new SqlParameter("@JotType", item.Type);
                                        parameter[8] = new SqlParameter("@JotRecCoilNo", item.RecCoilNo);
                                        parameter[9] = new SqlParameter("@JotNB", item.NB);
                                        parameter[10] = new SqlParameter("@JotSCH", item.SCH);
                                        parameter[11] = new SqlParameter("@JotGdnMscVou", item.Godown);
                                        parameter[12] = new SqlParameter("@JotOD", item.OD);
                                        ObjDBConnection.CallStoreProcedure("JobTrn_Insert", parameter);
                                    }
                                }
                                SetSuccessMessage("Job saved successfully.");
                                if (jobWorkEntryModel.isPrint != 0)
                                {
                                    TempData["ReturnId"] = jobVou;
                                    TempData["Savedone"] = "1";
                                    TempData["IsPrint"] = jobWorkEntryModel.isPrint;
                                }

                                return Json(new { result = true, message = "Job saved successfully" });
                            }
                            else
                            {
                                return Json(new { result = false, message = "Job work not saved" });
                            }
                        }
                        else
                        {
                            return Json(new { result = false, message = "Job work not saved" });
                        }
                    }
                    else
                    {
                        return Json(new { result = false, message = "Please Entry Value" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
        }

        public ActionResult GetDataById(string id)
        {
            JobWorkEntryModel jobWorkEntryModel = new JobWorkEntryModel();
            try
            {
                if (int.Parse(id) > 0)
                {
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));

                    SqlParameter[] para = new SqlParameter[1];
                    para[0] = new SqlParameter("@JOBVOU", id);
                    DataTable DtJob = ObjDBConnection.CallStoreProcedure("GetJobWorkCoilNo", para);

                    SqlParameter[] parameter = new SqlParameter[2];
                    parameter[0] = new SqlParameter("@FLG", 1);
                    parameter[1] = new SqlParameter("@JOBVOU", id);

                    DataSet ds = ObjDBConnection.GetDataSet("GetDataByCoilNo", parameter);
                    if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Count == 7)
                    {
                        DataTable dtJobMst = ds.Tables[0];
                        DataTable dtJobTrn = ds.Tables[1];
                        DataTable dtLotMst = ds.Tables[2];
                        DataTable dtDepartment = ds.Tables[3];
                        DataTable dtGodown = ds.Tables[4];
                        DataTable dtCoilNo = ds.Tables[5];
                        DataTable dtProduct = ds.Tables[6];
                        if (dtJobMst != null && dtJobMst.Rows.Count > 0)
                        {
                            jobWorkEntryModel.JobType = dtJobMst.Rows[0]["JobTyp"].ToString() == "1" ? "SPLIT" : "SLIT";
                            jobWorkEntryModel.VNo = dtJobMst.Rows[0]["JobVNo"].ToString();
                            jobWorkEntryModel.Date = Convert.ToDateTime(dtJobMst.Rows[0]["JobDt"].ToString()).ToString("yyyy-MM-dd");
                            jobWorkEntryModel.IssueCoilNo = dtJobMst.Rows[0]["CoilNo"].ToString();
                            jobWorkEntryModel.JobNo = dtJobMst.Rows[0]["JobNo"].ToString();
                            jobWorkEntryModel.CompleteDate = !string.IsNullOrWhiteSpace(dtJobMst.Rows[0]["JobComDt"].ToString()) ? Convert.ToDateTime(dtJobMst.Rows[0]["JobComDt"].ToString()).ToString("yyyy-MM-dd") : null;
                            jobWorkEntryModel.ActualQty = dtJobMst.Rows[0]["JobActQty"].ToString();
                            jobWorkEntryModel.ActualWidth = dtJobMst.Rows[0]["JobActWidth"].ToString();
                            jobWorkEntryModel.Remarks = dtJobMst.Rows[0]["JobRem"].ToString();
                            jobWorkEntryModel.GodownVou = dtJobMst.Rows[0]["JobGdnVou"].ToString();
                            jobWorkEntryModel.Qty = dtJobMst.Rows[0]["JobQty"].ToString();
                            jobWorkEntryModel.Width = dtJobMst.Rows[0]["JobWidth"].ToString();
                            jobWorkEntryModel.IssueCoilNoId = dtJobMst.Rows[0]["JobColVou"].ToString();
                            jobWorkEntryModel.LotTypeVou = Convert.ToInt32(dtJobMst.Rows[0]["JobLotTypVou"].ToString());
                            jobWorkEntryModel.JobTypeN = dtJobMst.Rows[0]["JobTypN"].ToString();
                            jobWorkEntryModel.JobShift = dtJobMst.Rows[0]["JobShift"].ToString();
                            jobWorkEntryModel.JobOprMscVou = dtJobMst.Rows[0]["JobOprEmpVou"].ToString();
                            jobWorkEntryModel.JobMacNo = dtJobMst.Rows[0]["JobMacNo"].ToString();
                        }
                        if (dtJobTrn != null && dtJobTrn.Rows.Count > 0)
                        {
                            List<JobWorkEntrySlit> jobWorkEntrySlits = new List<JobWorkEntrySlit>();
                            List<JobWorkEntrySplit> jobWorkEntrySplits = new List<JobWorkEntrySplit>();
                            if (jobWorkEntryModel.JobType == "SPLIT")
                            {
                                for (int i = 0; i < dtJobTrn.Rows.Count; i++)
                                {
                                    jobWorkEntrySplits.Add(new JobWorkEntrySplit
                                    {
                                        FormulaWt = dtJobTrn.Rows[i]["JotFrmQty"].ToString(),
                                        Remarks = dtJobTrn.Rows[i]["JotRem"].ToString(),
                                        VendorWt = dtJobTrn.Rows[i]["JotQty"].ToString(),
                                        CoilNo = dtCoilNo.Rows[i]["LotCoilNo"].ToString(),
                                        Godown = dtJobTrn.Rows[i]["JotGdnMscVou"].ToString(),
                                        RecCoilNo = dtJobTrn.Rows[i]["JotRecCoilNo"].ToString(),
                                    });
                                }
                            }
                            else
                            {
                                for (int i = 0; i < dtJobTrn.Rows.Count; i++)
                                {
                                    jobWorkEntrySlits.Add(new JobWorkEntrySlit
                                    {
                                        FormulaWt = dtJobTrn.Rows[i]["JotFrmQty"].ToString(),
                                        Remarks = dtJobTrn.Rows[i]["JotRem"].ToString(),
                                        Width = dtJobTrn.Rows[i]["JotWidth"].ToString(),
                                        ActualWt = dtJobTrn.Rows[i]["JotQty"].ToString(),
                                        CoilNo = dtCoilNo.Rows[i]["LotCoilNo"].ToString(),
                                        Godown = dtJobTrn.Rows[i]["JotGdnMscVou"].ToString(),
                                        RecCoilNo = dtJobTrn.Rows[i]["JotRecCoilNo"].ToString(),
                                        NB = dtJobTrn.Rows[i]["JotNB"].ToString(),
                                        SCH = dtJobTrn.Rows[i]["JotSCH"].ToString(),
                                        Type = dtJobTrn.Rows[i]["JotType"].ToString(),
                                        OD = dtJobTrn.Rows[i]["JotOD"].ToString(),
                                    });
                                }
                            }
                            jobWorkEntryModel.JobWorkEntrySlitsList = jobWorkEntrySlits;
                            jobWorkEntryModel.JobWorkEntrySplitsList = jobWorkEntrySplits;

                        }

                        if (dtLotMst != null && dtLotMst.Rows.Count > 0)
                        {
                            jobWorkEntryModel.RecDate = (Convert.ToDateTime(dtLotMst.Rows[0]["LotDt"].ToString()).ToString("yyyy-MM-dd"));
                            jobWorkEntryModel.Finish = dtLotMst.Rows[0]["LotFinish"].ToString();
                            jobWorkEntryModel.Grade = dtLotMst.Rows[0]["LotGrade"].ToString();
                            jobWorkEntryModel.Thick = dtLotMst.Rows[0]["LotThick"].ToString();
                        }
                        if (dtDepartment != null && dtDepartment.Rows.Count > 0)
                        {
                            jobWorkEntryModel.CompanyName = dtDepartment.Rows[0]["DEPNAME"].ToString();
                            jobWorkEntryModel.CompanyCode = dtDepartment.Rows[0]["DEPCODE"].ToString();
                        }
                        if (dtGodown != null && dtGodown.Rows.Count > 0)
                        {
                            jobWorkEntryModel.JobWorker = dtGodown.Rows[0]["AccNm"].ToString();
                        }
                        if (dtProduct != null && dtProduct.Rows.Count > 0)
                        {
                            jobWorkEntryModel.ProductName = dtProduct.Rows[0]["PRDNM"].ToString();
                        }
                        return Json(new { result = true, data = jobWorkEntryModel });
                    }
                    else
                    {
                        return Json(new { result = false, message = "Job work not found!" });
                    }
                }
                else
                {
                    return Json(new { result = false, message = "Job work not found!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
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
                    string currentURL = "/JobWorkEntry/Index";
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
                    getReportDataModel.ControllerName = "JobWorkEntry";
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
                    var bytes = Excel(getReportDataModel, "Job Work Entry", companyDetails.CmpName);
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

        public IActionResult Delete(long id)
        {
            try
            {
                if (id > 0)
                {
                    SqlParameter[] sqlPar = new SqlParameter[2];
                    sqlPar[0] = new SqlParameter("@compdt", "");
                    sqlPar[1] = new SqlParameter("@jobvou", id);
                    DataTable dtJob = ObjDBConnection.CallStoreProcedure("GetJobWorkCoilDetails", sqlPar);
                    if (dtJob != null && dtJob.Rows.Count > 0)
                    {
                        SetErrorMessage("Can Not Deleted Job work");
                    }
                    else
                    {
                        long userId = GetIntSession("UserId");
                        int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                        SqlParameter[] sqlParameters = new SqlParameter[2];
                        sqlParameters[0] = new SqlParameter("@JobVou", id);
                        sqlParameters[1] = new SqlParameter("@CmpVou", companyId);
                        DataTable dtJobMst = ObjDBConnection.CallStoreProcedure("JobMstTrn_Delete", sqlParameters);
                        if (dtJobMst != null && dtJobMst.Rows.Count > 0)
                        {
                            int value = DbConnection.ParseInt32(dtJobMst.Rows[0][0].ToString());
                            if (value == -2)
                            {
                                SetErrorMessage("Can Not Deleted Job work");
                            }
                            else
                            {
                                SetSuccessMessage("Job work deleted successfully");
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "JobWorkEntry");
        }

        public ActionResult GetNBSCH(string nb, string sch)
        {
            List<SelectListItem> nbList = new List<SelectListItem>();
            try
            {
                if (!string.IsNullOrWhiteSpace(nb) && !string.IsNullOrWhiteSpace(nb))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@NB", nb);
                    sqlParameters[1] = new SqlParameter("@SCH", sch);
                    DataTable dtNBSCH = ObjDBConnection.CallStoreProcedure("GetOdByNBSCH", sqlParameters);
                    if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
                    {
                        return Json(new { result = true, ordOd = dtNBSCH.Rows[0][0] });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
            return Json(new { result = false, message = "" });
        }

        public ActionResult GetNBSCHDetails(string thick, string od)
        {
            List<SelectListItem> nbList = new List<SelectListItem>();
            try
            {
                if (!string.IsNullOrWhiteSpace(thick) && !string.IsNullOrWhiteSpace(od))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@od", od);
                    sqlParameters[1] = new SqlParameter("@thick", thick);
                    DataTable dtNBSCH = ObjDBConnection.CallStoreProcedure("GetThickODValueByNBSCH", sqlParameters);
                    if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
                    {
                        return Json(new { result = true, nbText = dtNBSCH.Rows[0][0], schText = dtNBSCH.Rows[0][1] });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
            return Json(new { result = false, message = "" });
        }

        public ActionResult GetCoilDetails(string compdt, string jobvno)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(compdt) && !string.IsNullOrWhiteSpace(jobvno))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@compdt", compdt);
                    sqlParameters[1] = new SqlParameter("@jobvou", jobvno);
                    DataTable dtJob = ObjDBConnection.CallStoreProcedure("GetJobWorkCoilDetails", sqlParameters);
                    if (dtJob != null && dtJob.Rows.Count > 0)
                    {
                        return Json(new { result = true, data = "-1" });
                    }
                    else
                    {
                        return Json(new { result = true, data = "1" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
            return Json(new { result = false, message = "" });
        }

        public ActionResult GetCoilNoDetails(string coilno)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(coilno))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("@coilno", coilno);
                    DataTable dtJob = ObjDBConnection.CallStoreProcedure("GetJobWorkCoilNoDetails", sqlParameters);
                    if (dtJob != null && dtJob.Rows.Count > 0)
                    {
                        return Json(new { result = true, data = "-1" });
                    }
                    else
                    {
                        return Json(new { result = true, data = "1" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
            return Json(new { result = false, message = "" });
        }
        public IActionResult JobWorkPrintDetials(long id, int copyType = 1)
        {
            try
            {
                JobWorkPrintDetails obj = GetJobWorkPrintData(id);

                string wwwroot = string.Empty;
                string filePath = "/PrintPDF/" + id + ".pdf";
                wwwroot = _iwebhostenviroment.WebRootPath + filePath;
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(obj.Html);
                doc.Save(wwwroot);
                doc.Close();
                return Json(filePath);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public ActionResult GetCheckCompDtCoil(string isscoil)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(isscoil))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("@SupCoil", isscoil);
                    DataTable dtJob = ObjDBConnection.CallStoreProcedure("GetCheckJobWorkCoil", sqlParameters);
                    if (dtJob != null && dtJob.Rows.Count > 0)
                    {
                        int status = Convert.ToInt32(dtJob.Rows[0][0].ToString());
                        if (status == 1)
                        {
                            return Json(new { result = true, data = "1" });
                        }
                        else
                        {
                            return Json(new { result = true, data = "-1" });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
            return Json(new { result = false, message = "" });
        }



        public IActionResult JobWorkSendMail(long id, string email = "")
        {
            try
            {
                JobWorkPrintDetails obj = GetJobWorkPrintData(id);
                string wwwroot = string.Empty;
                string dateTime = DateTime.Now.ToString("ddMMyyyhhmmss");

                wwwroot = _iwebhostenviroment.WebRootPath + "/PrintPDF/" + dateTime + ".pdf";
                //var render = new IronPdf.HtmlToPdf();
                //using var doc = render.RenderHtmlAsPdf(obj.Html);
                //doc.SaveAs(wwwroot);

                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(obj.Html);
                doc.Save(wwwroot);
                doc.Close();

                bool result = SendEmail(email, "SLITTING MACHINE REPORT", "Please find attachment", wwwroot);
                if (result)
                    return Json(new { result = result, message = "Please check your mail address" });
                else
                    return Json(new { result = result, message = "Internal server error" });

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public IActionResult JobWorkWhatApp(long id, string whatappNo = "")
        {
            try
            {
                JobWorkPrintDetails obj = GetJobWorkPrintData(id);
                string wwwroot = string.Empty;
                string dateTime = DateTime.Now.ToString("ddMMyyyhhmmss");

                wwwroot = _iwebhostenviroment.WebRootPath + "/PrintPDF/" + dateTime + ".pdf";
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(obj.Html);
                doc.Margins.Left = 25;
                doc.Save(wwwroot);
                doc.Close();

                WhatAppAPIResponse apiResponse = SendWhatAppMessage(whatappNo, "SLITTING MACHINE REPORT", wwwroot);
                return Json(new { result = apiResponse.status, message = apiResponse.message });
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public JobWorkPrintDetails GetJobWorkPrintData(long id)
        {
            try
            {
                JobWorkPrintDetails obj = new JobWorkPrintDetails();
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                long userId = GetIntSession("UserId");

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@JOBVOU", id);
                DataTable DtBilty = ObjDBConnection.CallStoreProcedure("GetJobWorkSlitingDetails", sqlParameters);
                if (DtBilty != null && DtBilty.Rows.Count > 0)
                {
                    string path = _iwebhostenviroment.WebRootPath + "/Reports";
                    string body = string.Empty;
                    string filename = "";
                    //using streamreader for reading my htmltemplate   
                    string CmpCode = string.Empty;
                    string CmpName = string.Empty;
                    string CmpVou = string.Empty;
                    string Layout = string.Empty;

                    Layout = "SlittingMachine";
                    filename = "SlittingMachine.html";

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        for (int i = 0; i < DtBilty.Rows.Count; i++)
                        {
                            string BilDate = DateTime.Parse(DtBilty.Rows[0]["JobDt"].ToString()).ToString("dd-MM-yyyy");
                            string newbody = body.Replace("#*#*date*#*#", BilDate);
                            newbody = body.Replace("#*#*coilno*#*#", DtBilty.Rows[0]["CoilNo"].ToString());
                            newbody = newbody.Replace("#*#*party*#*#", DtBilty.Rows[0]["AccNm"].ToString());
                            newbody = newbody.Replace("#*#*date*#*#", BilDate);
                            newbody = newbody.Replace("#*#*opename*#*#", DtBilty.Rows[0]["opename"].ToString());
                            newbody = newbody.Replace("#*#*partycoil*#*#", DtBilty.Rows[0]["SupCoilNo"].ToString());
                            newbody = newbody.Replace("#*#*grade*#*#", DtBilty.Rows[0]["Grade"].ToString());
                            newbody = newbody.Replace("#*#*partywt*#*#", (Math.Round(decimal.Parse((DtBilty.Rows[0]["JobQty"].ToString())))).ToString("0"));
                            newbody = newbody.Replace("#*#*width*#*#", (Math.Round(decimal.Parse((DtBilty.Rows[0]["JobWidth"].ToString())))).ToString("0.0"));
                            newbody = newbody.Replace("#*#*thk*#*#", (Math.Round(decimal.Parse((DtBilty.Rows[0]["Thick"].ToString())))).ToString("0.00"));
                            newbody = newbody.Replace("#*#*actwt*#*#", (Math.Round(decimal.Parse((DtBilty.Rows[0]["JobActQty"].ToString())))).ToString("0"));
                            newbody = newbody.Replace("#*#*diff*#*#", (Math.Round(decimal.Parse((DtBilty.Rows[0]["Diff"].ToString())))).ToString("0"));

                            StringBuilder sb = new StringBuilder();

                            sb.Append("<table border=\"1\" cellpadding=\"5\" cellspacing=\"0\" width=\"100%\" style=\"margin-top:10px;\" > <thead><tr>");//datatable
                            sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:45px;border-left:none;\" >Sr.No.</th>");
                            sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:175px;\" >SM COIL NO.</th>");
                            sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:70px;\" >WIDTH</th>");
                            sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:70px;\" >THK</th>");
                            sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:70px;\" >WEIGHT</th>");
                            sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;border-right:none;\" >REMARKS</th>");
                            sb.Append("</tr>");

                            sb.Append("</thead><tbody>");

                            double dTotWT = 0;
                            for (int j = 0; j < DtBilty.Rows.Count; j++)
                            {
                                sb.Append("<tr>");

                                sb.Append("<td align=\"center\" style=\"font-size:14px;border-left:none;\">" + (j + 1) + "</td>");
                                sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtBilty.Rows[j]["LotCoilNo"].ToString() + "</td>");
                                sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtBilty.Rows[j]["LotWidth"].ToString() + "</td>");
                                sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtBilty.Rows[j]["LotThick"].ToString() + "</td>");
                                sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtBilty.Rows[j]["LotQty"].ToString() + "</td>");
                                sb.Append("<td align=\"center\" style=\"font-size:14px;border-right:none;\">" + DtBilty.Rows[j]["JobRem"].ToString() + "</td>");

                                dTotWT += Convert.ToDouble(DtBilty.Rows[j]["LotQty"].ToString());

                                sb.Append("</tr>");
                            }
                            for (int j = DtBilty.Rows.Count; j < 19; j++)
                            {
                                sb.Append("<tr>");

                                sb.Append("<td align=\"center\" style=\"font-size:14px;border-left:none;\">" + (j + 1) + "</td>");
                                sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                                sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                                sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                                sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                                sb.Append("<td align=\"center\" style=\"font-size:14px;border-right:none;\"></td>");

                                sb.Append("</tr>");
                            }

                            sb.Append("</tbody></table>");

                            newbody = newbody.Replace("#*#*sdate*#*#", BilDate);
                            //newbody = newbody.Replace("#*#*r16*#*#", dTotWT.ToString("") + " kg");
                            newbody = newbody.Replace("#*#*patta*#*#", DtBilty.Rows[i]["BalPatta"].ToString());
                            newbody = newbody.Replace("#*#*scrap*#*#", DtBilty.Rows[i]["Scrap"].ToString());
                            //newbody = newbody.Replace("#*#*r19*#*#", "");

                            newbody = newbody.Replace("#*#*datatable-keytable*#*#", sb.ToString());

                            newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(DtBilty.Rows[0]["DepLogo"].ToString()) ? "/Uploads/" + DtBilty.Rows[0]["DepLogo"].ToString() + "" : string.Empty);
                            //newbody = newbody.Replace("#*#*logo*#*#", "http://localhost:6954/Uploads/0026.jpeg");

                            obj.Html = newbody;
                            obj.Id = id.ToString();
                        }
                    }
                }
                return obj;
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult SetTempData(int tempDataValue)
        {
            TempData["ReturnId"] = tempDataValue;
            return Json(true);
        }
    }
}