using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SUNMark.Classes;
using SUNMark.Common;
using SUNMark.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUNMark.Controllers
{
    public class PicklingController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        ProductHelpers objProductHelper = new ProductHelpers();
        private readonly IWebHostEnvironment _iwebhostenviroment;
        public PicklingController(IWebHostEnvironment iwebhostenviroment)
        {
            _iwebhostenviroment = iwebhostenviroment;
        }
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
                    DataTable dt = ObjDBConnection.CallStoreProcedure("GetPickglinMasterById", parameter);
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


                        List<PikGridModel> List = new List<PikGridModel>();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            PikGridModel Pick = new PikGridModel();
                           Pick.Grade = dt.Rows[i]["PikAGrdVou"].ToString();
                           Pick.PikOD = Convert.ToDecimal(dt.Rows[i]["PikAOD"].ToString() == "" ? "0" : dt.Rows[i]["PikAOD"].ToString());
                           Pick.PikThick = Convert.ToDecimal(dt.Rows[i]["PikAThick"].ToString() == "" ? "0" : dt.Rows[i]["PikAThick"].ToString());
                           Pick.PikLength = Convert.ToDecimal(dt.Rows[i]["PikALength"].ToString() == "" ? "0" : dt.Rows[i]["PikALength"].ToString());
                           Pick.PikNoOfPipe = Convert.ToDecimal(dt.Rows[i]["PikANoOfPipe"].ToString() == "" ? "0" : dt.Rows[i]["PikANoOfPipe"].ToString());
                           Pick.PikWeight = Convert.ToDecimal(dt.Rows[i]["PikAWeight"].ToString() == "" ? "0" : dt.Rows[i]["PikAWeight"].ToString());
                           Pick.RecProduct = dt.Rows[i]["PikARecPrdVou"].ToString();
                           Pick.PikInTime = dt.Rows[i]["PikAInTime"].ToString();
                           Pick.PikOutTime = dt.Rows[i]["PikAOutTime"].ToString();
                           Pick.PikCoilNo = dt.Rows[i]["PikACoilNo"].ToString();
                           Pick.PikStatus = dt.Rows[i]["PikAStatus"].ToString();
                           Pick.PikType = dt.Rows[i]["PikAType"].ToString();
                            List.Add(Pick);
                        }
                        picklingMasterModel.LstPikling = List;
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
                if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(picklingMaster.Vno).ToString()) && !string.IsNullOrWhiteSpace(picklingMaster.Date) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(picklingMaster.PikCmpVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(picklingMaster.MachineNo).ToString())  && picklingMaster.LstPikling.Count > 0 )
                {
                    SqlParameter[] sqlParameter = new SqlParameter[18];
                    sqlParameter[0] = new SqlParameter("@PikVou", picklingMaster.PikVou);
                    sqlParameter[1] = new SqlParameter("@PikCmpVou", picklingMaster.PikCmpVou);
                    sqlParameter[2] = new SqlParameter("@PikVno", picklingMaster.Vno);
                    sqlParameter[3] = new SqlParameter("@PikDt", picklingMaster.Date);
                    sqlParameter[4] = new SqlParameter("@PikShift", picklingMaster.Shift);
                    sqlParameter[5] = new SqlParameter("@PikMacNo", picklingMaster.MachineNo);
                    sqlParameter[6] = new SqlParameter("@PikSupEmpVou", picklingMaster.SupEmpVou);
                    sqlParameter[7] = new SqlParameter("@PikManEmpVou", picklingMaster.ManEmpVou);
                    sqlParameter[8] = new SqlParameter("@PikIssPrdVou", picklingMaster.IssuePrdVou);
                    sqlParameter[9] = new SqlParameter("@PikFinish", picklingMaster.Finish);
                    sqlParameter[10] = new SqlParameter("@PikFinishVou", picklingMaster.FinishVou);
                    sqlParameter[11] = new SqlParameter("@PikHFQty", picklingMaster.HFQty);
                    sqlParameter[12] = new SqlParameter("@PikNitricQty", picklingMaster.NitricQty);
                    sqlParameter[13] = new SqlParameter("@PikLimeQty", picklingMaster.LimeQty);
                    sqlParameter[14] = new SqlParameter("@PikRemarks", picklingMaster.Remarks);
                    sqlParameter[15] = new SqlParameter("@PikNextPrcVou", picklingMaster.NextPrcVou);
                    sqlParameter[16] = new SqlParameter("@PikNextProc", picklingMaster.NextProc);
                    sqlParameter[17] = new SqlParameter("@Flg", "1");
                    DataTable dt = ObjDBConnection.CallStoreProcedure("InsertPickling", sqlParameter);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int masterId = DbConnection.ParseInt32(dt.Rows[0][0].ToString());
                        if (masterId > 0)
                        {
                            for (int i = 0; i < picklingMaster.LstPikling.Count; i++)
                            {
                                SqlParameter[] sqlParam = new SqlParameter[15];
                                sqlParam[0] = new SqlParameter("@PikAPikVou", masterId);
                                sqlParam[1] = new SqlParameter("@PikCmpVou", picklingMaster.PikCmpVou);
                                sqlParam[2] = new SqlParameter("@PikGrdVou", picklingMaster.LstPikling[i].Grade);
                                sqlParam[3] = new SqlParameter("@PikThick", picklingMaster.LstPikling[i].PikThick);
                                sqlParam[4] = new SqlParameter("@PikOD", picklingMaster.LstPikling[i].PikOD);
                                sqlParam[5] = new SqlParameter("@PikLength", picklingMaster.LstPikling[i].PikLength);
                                sqlParam[6] = new SqlParameter("@PikNoOfPipe", picklingMaster.LstPikling[i].PikNoOfPipe);
                                sqlParam[7] = new SqlParameter("@PikQty", picklingMaster.LstPikling[i].PikWeight);
                                sqlParam[8] = new SqlParameter("@PikRecPrdVou", picklingMaster.LstPikling[i].RecProduct);
                                sqlParam[9] = new SqlParameter("@PikInTime", picklingMaster.LstPikling[i].PikInTime);
                                sqlParam[10] = new SqlParameter("@PikOutTime", picklingMaster.LstPikling[i].PikOutTime);
                                sqlParam[11] = new SqlParameter("@PikCoilNo", picklingMaster.LstPikling[i].PikCoilNo);
                                sqlParam[12] = new SqlParameter("@PikStatus", picklingMaster.LstPikling[i].PikStatus);
                                sqlParam[13] = new SqlParameter("@PikType", picklingMaster.LstPikling[i].PikType);
                                sqlParam[14] = new SqlParameter("@PikSrNo", (i + 1));
                                DataTable dttrn = ObjDBConnection.CallStoreProcedure("InsertPicklingTrn", sqlParam);
                            }
                            int Status = DbConnection.ParseInt32(dt.Rows[0][0].ToString());
                            if (Status == 0)
                            {
                                SetErrorMessage("Dulplicate Vou.No Details");
                            }
                            else
                            {
                                if (picklingMaster.isPrint != 0)
                                {
                                    TempData["ReturnId"] = id.ToString();
                                    TempData["Savedone"] = "1";
                                    TempData["IsPrint"] = picklingMaster.isPrint.ToString();
                                    return RedirectToAction("Index", new { id = id });
                                }
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
        public PicklingPrintDetails GetPicklingHtmlData(long id)
        {
            PicklingPrintDetails obj = new PicklingPrintDetails();

            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                long userId = GetIntSession("UserId");

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@PikVou", id);
                DataTable DtInward = ObjDBConnection.CallStoreProcedure("GetPicklingDetailsforPDF", sqlParameters);
                if (DtInward != null && DtInward.Rows.Count > 0)
                {
                    string path = _iwebhostenviroment.WebRootPath + "/Reports";
                    string body = string.Empty;
                    string newbody = string.Empty;
                    string filename = "";
                    //using streamreader for reading my htmltemplate   
                    string CmpCode = string.Empty;
                    string CmpName = string.Empty;
                    string CmpVou = string.Empty;
                    string Layout = string.Empty;
                    string CmpWeb = string.Empty;
                    string CmpAdd = string.Empty;
                    string CmpEmail = string.Empty;

                    Layout = "Pickling";
                    filename = "Pickling.html";

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        newbody = body.Replace("//Address//", DtInward.Rows[0]["DepAdd"].ToString());
                        newbody = newbody.Replace("//Email//", DtInward.Rows[0]["DepEmail"].ToString());
                        newbody = newbody.Replace("//Web//", CmpWeb);
                        string BilDate = DateTime.Parse(DtInward.Rows[0]["PikDt"].ToString()).ToString("dd-MM-yyyy");
                        newbody = newbody.Replace("//Date//", BilDate);
                        newbody = newbody.Replace("//Shift//", DtInward.Rows[0]["PikShift"].ToString());
                        newbody = newbody.Replace("//Logo//", !string.IsNullOrWhiteSpace(DtInward.Rows[0]["DepLogo"].ToString()) ? "http://piosunmark.pioerp.com/Uploads/" + DtInward.Rows[0]["DepLogo"].ToString() + "" : string.Empty);
                        newbody = newbody.Replace("//Nitric//", DtInward.Rows[0]["PikNitricQty"].ToString());
                        newbody = newbody.Replace("//Lime//", DtInward.Rows[0]["PikLimeQty"].ToString());
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < DtInward.Rows.Count; i++)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td align=\"center\" style=\"font-size:16px;\">E+ Stock</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:16px;\">" + DtInward.Rows[i]["PikACoilNo"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:16px;\">" + DtInward.Rows[i]["Grade"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:16px;\">-</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:16px;\">" + DtInward.Rows[i]["PikAOD"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:16px;\">" + DtInward.Rows[i]["PikAThick"].ToString() + "</td>");
                            var length = (Convert.ToDouble(DtInward.Rows[i]["PikALength"]) * 0.3048);
                            sb.Append("<td align=\"center\" style=\"font-size:16px;\">" + length + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:16px;\">" + DtInward.Rows[i]["PikANoOfPipe"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:16px;\">" + DtInward.Rows[i]["PikAWeight"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:16px;\">" + DtInward.Rows[i]["PikAStatus"].ToString() + "</td>");
                            sb.Append("</tr>");
                        }
                        
                        for (int i = 0; i < DtInward.Rows.Count; i++)
                        {
                            sb.Append("<tr align=\"center\"style =\"font-size:16px;\">");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("</tr>");
                        }
                        newbody = newbody.Replace("//First-Table//", sb.ToString());
                        obj.Html = newbody;
                        obj.Id = id.ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return obj;
        }
        public IActionResult PicklingPrintDetials(long id, int copyType = 1)
        {
            try
            {
                PicklingPrintDetails obj = GetPicklingHtmlData(id);

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
         
        public IActionResult PicklingSendMail(long id, string email = "")
        {
            try
            {
                //PicklingPrintDetails obj = GetPicklingHtmlData(id);
                PicklingPrintDetails obj = GetPicklingHtmlData(id);
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

                bool result = SendEmail(email, "Pickling TRANSFER REPORT", "Please find attachment", wwwroot);
                if (result)
                    return Json(new { result = result, message = "Mail Send Sucessfully" });
                else
                    return Json(new { result = result, message = "Internal server error" });


                //return Json(new { result = result, message = "Please check your mail address" });
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public IActionResult PicklingWhatApp(long id, string whatappNo = "")
        {
            try
            {
                PicklingPrintDetails obj = GetPicklingHtmlData(id);
                string wwwroot = string.Empty;
                string filenm = string.Empty;
                string dateTime = DateTime.Now.ToString("ddMMyyyhhmmss");

                wwwroot = _iwebhostenviroment.WebRootPath + "/PrintPDF/" + dateTime + ".pdf";
                //wwwroot = "http://piosunmark.pioerp.com/wwwroot/PrintPDF/" + dateTime + ".pdf";
                filenm = dateTime + ".pdf";
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(obj.Html);
                doc.Margins.Left = 25;
                doc.Save(wwwroot);
                doc.Close();

                WhatAppAPIResponse apiResponse = SendWhatAppMessage(whatappNo, "Pickling TRANSFER REPORT", wwwroot, filenm);
                return Json(new { result = apiResponse.status, message = apiResponse.message });
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
