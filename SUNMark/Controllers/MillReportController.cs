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
                sqlParameters[1] = new SqlParameter("@Machine", machine);
                sqlParameters[2] = new SqlParameter("@Shift", shift);
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
                        string BilDate = DateTime.Parse(DtBilty.Rows[0]["MilDt"].ToString()).ToString("dd-MM-yyyy");
                        string newbody = body.Replace("#*#*Dt*#*", BilDate);
                        newbody = newbody.Replace("#*#*Oper*#*", DtBilty.Rows[0]["Operator1"].ToString() + "/" + DtBilty.Rows[0]["Operator2"].ToString());
                        newbody = newbody.Replace("#*#*MillNo*#*", DtBilty.Rows[0]["MacNm"].ToString());
                        newbody = newbody.Replace("#*#*Shift*#*", DtBilty.Rows[0]["MilShiftName"].ToString());
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < DtBilty.Rows.Count; i++)
                        {
                           
                            sb.Append("<tr>");

                            sb.Append("<td rowspan=\"3\">" + DtBilty.Rows[i]["MilLotNo"].ToString() + "</td>");
                            sb.Append("<td rowspan=\"3\">#*#*r2*#*</td>");
                            sb.Append("<td rowspan=\"3\">" + DtBilty.Rows[i]["LotGrade"].ToString() + "</td>");
                            sb.Append("<td style=\"height: 40px;\">Coil</td>");
                            sb.Append("<td style=\"width: 5%;\">" + DtBilty.Rows[i]["LotInwWidth"].ToString() + "</td>");
                            sb.Append("<td style=\"width: 5%;\">" + DtBilty.Rows[i]["LotInwThick"].ToString() + "</td>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td rowspan=\"3\" style=\"width: 5%;\">" + DtBilty.Rows[i]["MilLenFeet"].ToString() + "</td>");
                            sb.Append("<td rowspan=\"3\">" + DtBilty.Rows[i]["MilQty"].ToString() + "</td>");
                            sb.Append("<td rowspan=\"3\">" + DtBilty.Rows[i]["MilInTime"].ToString() + "</td>");
                            sb.Append("<td rowspan=\"3\">" + DtBilty.Rows[i]["MilOutTime"].ToString() + "</td>");
                            sb.Append("<td rowspan=\"3\">" + DtBilty.Rows[i]["MilPCS"].ToString() + "</td>");
                            sb.Append("<td rowspan=\"3\">" + DtBilty.Rows[i]["MilRecQty"].ToString() + "</td>");
                            sb.Append("<td>3 TO 10</td>");
                            sb.Append("<td style=\"width: 5%;\">#*#*r12*#*</td>");
                            sb.Append("<td style=\"width: 5%;\">#*#*r13*#*</td>");
                            sb.Append("<td rowspan=\"3\">" + DtBilty.Rows[i]["MilScrQty"].ToString() + "</td>");
                            sb.Append("<td style=\"width: 3%;\">" + DtBilty.Rows[i]["MilTouNo"].ToString() + "</td>");
                            sb.Append("<td style=\"width: 3%;\">" + DtBilty.Rows[i]["MilWeldSpeed"].ToString() + "</td>");
                            sb.Append("<td style=\"width: 3%;\">" + DtBilty.Rows[i]["MilWeldAMP"].ToString() + "</td>");
                            sb.Append("<td style=\"width: 3%;\">" + DtBilty.Rows[i]["MilWeldVolt"].ToString() + "</td>");
                            sb.Append("<td rowspan=\"3\">" + DtBilty.Rows[i]["MilRem"].ToString() + "</td>");

                            sb.Append("</tr>");

                            sb.Append("<tr>");
                            sb.Append("<td rowspan=\"2\">ACTUAL</td>");
                            sb.Append("<td rowspan=\"2\">#*#*r16*#*</td>");
                            sb.Append("<td rowspan=\"2\">#*#*r17*#*</td>");
                            sb.Append("<td rowspan=\"2\">" + DtBilty.Rows[i]["MilOD"].ToString() + "</td>");
                            sb.Append("<td style=\"height: 40px;\">10 TO 18</td>");
                            sb.Append("<td style=\"width: 3%;\">#*#*r27*#*</td>");
                            sb.Append("<td style=\"width: 3%;\">#*#*r28*#*</td>");
                            sb.Append("<td rowspan=\"2\">#*#*r19*#*</td>");
                            sb.Append("<td rowspan=\"2\">#*#*r20*#*</td>");
                            sb.Append("<td rowspan=\"2\">#*#*r21*#*</td>");
                            sb.Append("<td rowspan=\"2\">#*#*r22*#*</td>");
                            sb.Append("</tr>");

                            sb.Append("<tr>");
                            sb.Append("<td style=\"width: 6%;height: 40px;\">18 TO 19.5</td>");
                            sb.Append("<td style=\"width: 3%;\">#*#*r29*#*</td>");
                            sb.Append("<td style=\"width: 3%;\">#*#*r30*#*</td>");
                            sb.Append("</tr>");

                            
                        }
                        newbody = newbody.Replace("#*#*datatable-keytable-Main*#*#", sb.ToString());

                        ////newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(DtBilty.Rows[0]["DepLogo"].ToString()) ? "/Uploads/" + DtBilty.Rows[0]["DepLogo"].ToString() + "" : string.Empty);
                        //newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(DtBilty.Rows[0]["DepLogo"].ToString()) ? "http://piosunmark.pioerp.com/Uploads/" + DtBilty.Rows[0]["DepLogo"].ToString() + "" : string.Empty);


                        obj.Html = newbody;
                        //obj.Id = id.ToString();
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
