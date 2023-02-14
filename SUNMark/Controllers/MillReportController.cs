using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SUNMark.Classes;
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
    public class MillReportController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        private readonly IWebHostEnvironment _iwebhostenviroment;

        public MillReportController(IWebHostEnvironment iwebhostenviroment)
        {
            _iwebhostenviroment = iwebhostenviroment;
        }

        public IActionResult Index(long id)
        {
            StockLedgerModel stockLedgerModel = new StockLedgerModel();
            try
            {
                bool isreturn = false;
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                long yearId = GetIntSession("YearId");
                int administrator = 0;
                var yearData = DbConnection.GetYearListByCompanyId(Convert.ToInt32(companyId)).Where(x => x.YearVou == yearId).FirstOrDefault();
                if (yearData != null)
                {
                    stockLedgerModel.Dt = yearData.StartDate;
                }
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                stockLedgerModel.ShiftList = objProductHelper.GetShiftNew(); ;
                stockLedgerModel.MachineList = ObjAccountMasterHelpers.GetMachineMasterDropdown(companyId);
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(stockLedgerModel);
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
                ViewBag.layoutList = GetGridLayoutDropDown(DbConnection.GridTypeReport, userFormRights.ModuleId);
                ViewBag.pageNoList = GetPageNo();
            }

            #endregion
        }

        public IActionResult MillingPrintDetials(string dt, int machine, int shift)
        {
            try
            {
                MillingPrintDetails obj = GetMillPrintData(dt, machine, shift);

                string wwwroot = string.Empty;
                string filePath = "/PrintPDF/" + "Mill-" + dt + ".pdf";
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
        public MillingPrintDetails GetMillPrintData(string dt, int machine, int shift)
        {
            try
            {
                MillingPrintDetails obj = new MillingPrintDetails();
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                long userId = GetIntSession("UserId");

                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@Dt", dt);
                sqlParameters[0] = new SqlParameter("@Machine", machine);
                sqlParameters[0] = new SqlParameter("@Shift", machine);
                DataTable DtBilty = ObjDBConnection.CallStoreProcedure("GetMillingReport", sqlParameters);
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

                    Layout = "MillRpt";
                    filename = "MillRpt.html";

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        for (int i = 0; i < DtBilty.Rows.Count; i++)
                        {
                            string BilDate = DateTime.Parse(DtBilty.Rows[0]["MilDt"].ToString()).ToString("dd-MM-yyyy");
                            string newbody = body.Replace("#*#*Dt*#*#", BilDate);
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

                            //newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(DtBilty.Rows[0]["DepLogo"].ToString()) ? "/Uploads/" + DtBilty.Rows[0]["DepLogo"].ToString() + "" : string.Empty);
                            newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(DtBilty.Rows[0]["DepLogo"].ToString()) ? "http://piosunmark.pioerp.com/Uploads/" + DtBilty.Rows[0]["DepLogo"].ToString() + "" : string.Empty);


                            obj.Html = newbody;
                            //obj.Id = id.ToString();
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

    }
}
