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
//using PuppeteerSharp;
//using PuppeteerSharp.Media;
using System.Net.Http;

namespace SUNMark.Controllers
{
    public class PrintController : Controller
    {

        DbConnection ObjDBConnection = new DbConnection();
        private readonly IWebHostEnvironment _iwebhostenviroment;

        public PrintController(IWebHostEnvironment iwebhostenviroment)
        {
            _iwebhostenviroment = iwebhostenviroment;
        }
        
        public IActionResult CoilStockPrintDetails(string todt, int companyid)
        {

            CoilStockPrintDetails obj = new CoilStockPrintDetails();

            try
            {
                //int YearId = Convert.ToInt32(GetIntSession("YearId"));
                //long userId = GetIntSession("UserId");

                SqlParameter[] sqlPara = new SqlParameter[2];
                sqlPara[0] = new SqlParameter("@TODT", todt);
                sqlPara[1] = new SqlParameter("@CmpVou", companyid);
                DataTable DtGrd = ObjDBConnection.CallStoreProcedure("GEN_GRADEWISE_THICK_COLUMNER_NEW", sqlPara);


                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@CmpVou", companyid);
                DataSet ds = ObjDBConnection.GetDataSet("GETCOILSTOCK_REPORT", sqlParameters);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Count == 3)
                {
                    DataTable dtCmpMst = ds.Tables[0];
                    DataTable dtGrade = ds.Tables[1];
                    DataTable dtHead = ds.Tables[2];

                    string path = _iwebhostenviroment.WebRootPath + "/Reports";
                    string body = string.Empty;
                    string filename = "";
                    //using streamreader for reading my htmltemplate   
                    string CmpCode = string.Empty;
                    string CmpName = string.Empty;
                    string CmpVou = string.Empty;
                    string Layout = string.Empty;

                    Layout = "CoilStock";
                    filename = "CoilStock.html";

                    StringBuilder sb = new StringBuilder();

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        if (dtCmpMst != null && dtCmpMst.Rows.Count > 0)
                        {
                            string newbody = body.Replace("#*#*Title*#*#", "Coil Stock Report");
                            newbody = newbody.Replace("#*#*SubTitle*#*#", "Grade Wise Thickness Columner - As On Date : " + Convert.ToDateTime(todt).ToString("dd-MM-yyyy"));
                            newbody = newbody.Replace("#*#*RptDate*#*#", Convert.ToDateTime(DateTime.Now).ToString("dd-MM-yyyy"));
                            if (companyid != 0)
                            {
                                newbody = newbody.Replace("#*#*cmpName*#*#", dtCmpMst.Rows[0]["DepName"].ToString());
                                newbody = newbody.Replace("#*#*cmpAdd*#*#", dtCmpMst.Rows[0]["DepAdd"].ToString());
                                newbody = newbody.Replace("#*#*cmpEmail*#*#", dtCmpMst.Rows[0]["DepEmail"].ToString());
                                newbody = newbody.Replace("#*#*cmpPhone*#*#", dtCmpMst.Rows[0]["DepMobile"].ToString());
                                newbody = newbody.Replace("#*#*cmpGST*#*#", dtCmpMst.Rows[0]["DepGST"].ToString());
                            }
                            else
                            {
                                newbody = newbody.Replace("#*#*cmpName*#*#", dtCmpMst.Rows[0]["CmpName"].ToString());
                                newbody = newbody.Replace("#*#*cmpAdd*#*#", "");
                                newbody = newbody.Replace("#*#*cmpEmail*#*#", "");
                                newbody = newbody.Replace("#*#*cmpPhone*#*#", "");
                                newbody = newbody.Replace("#*#*cmpGST*#*#", "");
                            }
                            for (int i = 0; i < dtGrade.Rows.Count; i++)
                            {

                                sb.Append("<table border=\"1\" cellpadding=\"5\" cellspacing=\"0\" width=\"100%\" style=\"margin-top:10px;\" > <thead><tr style=\"background-color:lightgray\">");//
                                sb.Append("<th align=\"left\" style=\"font-family:Verdana;font-size:15px;color:black;\" colspan=\"22\" >" + dtGrade.Rows[i]["MscNm"].ToString() + " - COIL STOCK </th></tr>");
                                sb.Append("</thead><tbody>");
                                for (int p = 0; p < dtHead.Rows.Count; p++)
                                {
                                    sb.Append("<tr>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">SIZE</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm1"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm2"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm3"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm4"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm5"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm6"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm7"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm8"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm9"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm10"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm11"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm12"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm13"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm14"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm15"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm16"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm17"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm18"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm19"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm20"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">TOTAL</td>");
                                    sb.Append("</tr>");
                                }

                                SqlParameter[] sqlParam = new SqlParameter[1];
                                sqlParam[0] = new SqlParameter("@GRDVOU", dtGrade.Rows[i]["AccVou"].ToString());
                                DataTable DtData = ObjDBConnection.CallStoreProcedure("GETCOILSTOCK_REPORT1", sqlParam);
                                if (DtData != null && DtData.Rows.Count > 0)
                                {
                                    for (int s = 0; s < DtData.Rows.Count; s++)
                                    {
                                        sb.Append("<tr>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["InvNo"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col1"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col2"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col3"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col4"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col5"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col6"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col7"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col8"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col9"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col10"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col11"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col12"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col13"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col14"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col15"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col16"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col17"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col18"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col19"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col20"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["NetAmt"].ToString() + "</td>");
                                        sb.Append("</tr>");

                                    }
                                }
                                sb.Append("</tbody></table>");

                            }
                            newbody = newbody.Replace("#*#*datatable-keytable*#*#", sb.ToString());

                            if (companyid != 0)
                            {
                                newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(dtCmpMst.Rows[0]["DepLogo"].ToString()) ? "/Uploads/" + dtCmpMst.Rows[0]["DepLogo"].ToString() + "" : string.Empty);

                            }
                            else
                            {
                                newbody = newbody.Replace("#*#*logo*#*#", string.Empty);
                            }
                            //newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(dtCmpMst.Rows[0]["DepLogo"].ToString()) ? "<img src='/Uploads/" + dtCmpMst.Rows[0]["DepLogo"].ToString() + "' style='max-width:100 %;max-height: 100px; ' />" : string.Empty);

                            obj.Html = newbody;
                            obj.ToDt = todt.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(obj);
        }
        public IActionResult CoilRegisterPrintDetails(string frdt, string todt, int companyid, int gradeid, decimal frwidth, decimal towidth, decimal frthick, decimal tothick, decimal frqty, decimal toqty)
        {

            CoilRegisterPrintDetails obj = new CoilRegisterPrintDetails();

            try
            {
                //int YearId = Convert.ToInt32(GetIntSession("YearId"));
                //long userId = GetIntSession("UserId");

                SqlParameter[] sqlPara = new SqlParameter[10];
                sqlPara[0] = new SqlParameter("@FrDT", frdt);
                sqlPara[1] = new SqlParameter("@ToDT", todt);
                sqlPara[2] = new SqlParameter("@FrWidth", frwidth);
                sqlPara[3] = new SqlParameter("@ToWidth", towidth);
                sqlPara[4] = new SqlParameter("@FrThick", frthick);
                sqlPara[5] = new SqlParameter("@ToThick", tothick);
                sqlPara[6] = new SqlParameter("@FrQty", frqty);
                sqlPara[7] = new SqlParameter("@ToQty", toqty);
                sqlPara[8] = new SqlParameter("@CmpVou", companyid);
                sqlPara[9] = new SqlParameter("@GrdVou", gradeid);
                DataTable DtGrd = ObjDBConnection.CallStoreProcedure("RPT_COILREGISTER", sqlPara);


                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@CmpVou", 12);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                DataTable DtCom = ObjDBConnection.CallStoreProcedure("GetCompanyDetails", sqlParameters);
                if (DtGrd != null && DtGrd.Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    string path = _iwebhostenviroment.WebRootPath + "/Reports";
                    string body = string.Empty;
                    string filename = "";
                    //using streamreader for reading my htmltemplate   
                    string CmpCode = string.Empty;
                    string CmpName = string.Empty;
                    string CmpVou = string.Empty;
                    string Layout = string.Empty;
                    string option = string.Empty;

                    Layout = "CoilRegister";
                    filename = "CoilRegister.html";

                    if (frdt != null)
                    {
                        option = option + " From Date :" + Convert.ToDateTime(frdt).ToString("dd-MM-yyyy");
                    }
                    if (todt != null)
                    {
                        option = option + " To Date :" + Convert.ToDateTime(todt).ToString("dd-MM-yyyy");
                    }
                        

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        string newbody = body.Replace("#*#*Title*#*#", "Coil Register Report");
                        newbody = newbody.Replace("#*#*SubTitle*#*#", option);
                        newbody = newbody.Replace("#*#*RptDate*#*#", Convert.ToDateTime(DateTime.Now).ToString("dd-MM-yyyy"));
                        if (companyid != 0)
                        {
                            newbody = newbody.Replace("#*#*cmpName*#*#", DtGrd.Rows[0]["DepName"].ToString());
                            newbody = newbody.Replace("#*#*cmpAdd*#*#", DtGrd.Rows[0]["DepAdd"].ToString());
                            newbody = newbody.Replace("#*#*cmpEmail*#*#", DtGrd.Rows[0]["DepEmail"].ToString());
                            newbody = newbody.Replace("#*#*cmpPhone*#*#", DtGrd.Rows[0]["DepMobile"].ToString());
                            newbody = newbody.Replace("#*#*cmpGST*#*#", DtGrd.Rows[0]["DepGST"].ToString());
                        }
                        else
                        {
                            newbody = newbody.Replace("#*#*cmpName*#*#", DtCom.Rows[0]["CmpName"].ToString());
                            newbody = newbody.Replace("#*#*cmpAdd*#*#", "");
                            newbody = newbody.Replace("#*#*cmpEmail*#*#", "");
                            newbody = newbody.Replace("#*#*cmpPhone*#*#", "");
                            newbody = newbody.Replace("#*#*cmpGST*#*#", "");
                        }
                        sb.Append("<table border=\"1\" cellpadding=\"5\" cellspacing=\"0\" width=\"100%\" style=\"margin-top:10px;border-collapse: collapse;border - style: none;\" > <thead>");//
                        
                        sb.Append("<tr>");
                        sb.Append("<th align=\"center\" style=\"font-size:14px;\">Sr.No</th>");
                        sb.Append("<th align=\"center\" style=\"font-size:14px;\">Coil No</th>"); 
                        sb.Append("<th align=\"center\" style=\"font-size:14px;\">Grade</th>");
                        sb.Append("<th align=\"center\" style=\"font-size:14px;\">Width</th>");
                        sb.Append("<th align=\"center\" style=\"font-size:14px;\">Thick</th>");
                        sb.Append("<th align=\"center\" style=\"font-size:14px;\">Weight</th>");                        
                        sb.Append("</tr>");
                        sb.Append("</thead><tbody>");

                        //string srno = "<ul style='list-style:none;padding:0;margin:0;'>",
                        //    coilno = "<ul style='list-style:none;padding:0;margin:0;'>",
                        //    grade = "<ul style='list-style:none;padding:0;margin:0;'>",
                        //    width = "<ul style='list-style:none;padding:0;margin:0;'>",
                        //    thick = "<ul style='list-style:none;padding:0;margin:0;'>",
                        //    weight = "<ul style='list-style:none;padding:0;margin:0;'>";

                        for (int i = 0; i < DtGrd.Rows.Count; i++)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + i + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtGrd.Rows[i]["LotCoilNo"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtGrd.Rows[i]["LotGrade"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtGrd.Rows[i]["LotWidth"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtGrd.Rows[i]["LotThick"].ToString() + "</td>");
                            sb.Append("<td align=\"right\" style=\"font-size:14px;\">" + DtGrd.Rows[i]["LotQty"].ToString() + "</td>");
                            sb.Append("</tr>");
                        }
                        
                        Decimal TotQty = Convert.ToDecimal(DtGrd.Compute("SUM(LotQty)", string.Empty));
                        string GrdTot = TotQty.ToString();
                        if (!string.IsNullOrEmpty(GrdTot))
                        {
                            if (Convert.ToDecimal(GrdTot) <= 0)
                                GrdTot = string.Empty;
                            else
                                GrdTot = GrdTot;
                        }
                        else
                            GrdTot = string.Empty;

                        sb.Append("<tr>");
                        sb.Append("<td align=\"right\" style=\"font-size:14px;\" colspan=\"5\">Total</td>");
                        sb.Append("<td align=\"right\" style=\"font-size:14px;\">"+ GrdTot + "</th>");
                        sb.Append("</tr>");
                        sb.Append("</tbody></table>");


                        newbody = newbody.Replace("#*#*datatable*#*#", sb.ToString());
                        newbody = newbody.Replace("#*#*r7*#*#", GrdTot);
                        //if (companyid != 0)
                        //{
                        //    newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(DtGrd.Rows[0]["DepLogo"].ToString()) ? "/Uploads/" + DtGrd.Rows[0]["DepLogo"].ToString() + "" : string.Empty);

                        //}
                        //else
                        //{
                        //    newbody = newbody.Replace("#*#*logo*#*#", string.Empty);
                        //}
                        //newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(dtCmpMst.Rows[0]["DepLogo"].ToString()) ? "<img src='/Uploads/" + dtCmpMst.Rows[0]["DepLogo"].ToString() + "' style='max-width:100 %;max-height: 100px; ' />" : string.Empty);

                        obj.Html = newbody;
                        obj.ToDt = todt.ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(obj);
        }

        public IActionResult ProductionDailySummDetails(string todt, int companyid)
        {
            CoilStockPrintDetails obj = new CoilStockPrintDetails();

            try
            {
                //int YearId = Convert.ToInt32(GetIntSession("YearId"));
                //long userId = GetIntSession("UserId");

                SqlParameter[] sqlPara = new SqlParameter[2];
                sqlPara[0] = new SqlParameter("@TODT", todt);
                sqlPara[1] = new SqlParameter("@CmpVou", companyid);
                DataTable DtGrd = ObjDBConnection.CallStoreProcedure("GEN_GRADEWISE_THICK_COLUMNER", sqlPara);


                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@CmpVou", companyid);
                DataSet ds = ObjDBConnection.GetDataSet("GETCOILSTOCK_REPORT", sqlParameters);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Count == 3)
                {
                    DataTable dtCmpMst = ds.Tables[0];
                    DataTable dtGrade = ds.Tables[1];
                    DataTable dtHead = ds.Tables[2];

                    string path = _iwebhostenviroment.WebRootPath + "/Reports";
                    string body = string.Empty;
                    string filename = "";
                    //using streamreader for reading my htmltemplate   
                    string CmpCode = string.Empty;
                    string CmpName = string.Empty;
                    string CmpVou = string.Empty;
                    string Layout = string.Empty;

                    Layout = "CoilStock";
                    filename = "CoilStock.html";

                    StringBuilder sb = new StringBuilder();

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        if (dtCmpMst != null && dtCmpMst.Rows.Count > 0)
                        {
                            string newbody = body.Replace("#*#*cmpAdd*#*#", dtCmpMst.Rows[0]["DepAdd"].ToString());
                            newbody = newbody.Replace("#*#*cmpEmail*#*#", dtCmpMst.Rows[0]["DepEmail"].ToString());
                            for (int i = 0; i < dtGrade.Rows.Count; i++)
                            {

                                sb.Append("<table border=\"1\" cellpadding=\"5\" cellspacing=\"0\" width=\"100%\" style=\"margin-top:10px;\" > <thead><tr style=\"background-color:lightgray\">");//
                                sb.Append("<th align=\"left\" style=\"font-family:Verdana;font-size:15px;color:black;\" colspan=\"22\" >" + dtGrade.Rows[i]["MscNm"].ToString() + " - COIL STOCK </th></tr>");
                                sb.Append("</thead><tbody>");
                                for (int p = 0; p < dtHead.Rows.Count; p++)
                                {
                                    sb.Append("<tr>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">SIZE</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm1"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm2"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm3"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm4"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm5"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm6"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm7"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm8"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm9"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm10"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm11"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm12"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm13"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm14"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm15"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm16"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm17"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm18"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm19"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[p]["ColAccNm20"].ToString() + "</td>");
                                    sb.Append("<td align=\"center\" style=\"font-size:14px;\">TOTAL</td>");
                                    sb.Append("</tr>");
                                }

                                SqlParameter[] sqlParam = new SqlParameter[1];
                                sqlParam[0] = new SqlParameter("@GRDVOU", dtGrade.Rows[i]["AccVou"].ToString());
                                DataTable DtData = ObjDBConnection.CallStoreProcedure("GETCOILSTOCK_REPORT1", sqlParam);
                                if (DtData != null && DtData.Rows.Count > 0)
                                {
                                    for (int s = 0; s < DtData.Rows.Count; s++)
                                    {
                                        sb.Append("<tr>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["InvNo"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col1"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col2"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col3"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col4"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col5"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col6"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col7"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col8"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col9"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col10"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col11"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col12"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col13"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col14"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col15"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col16"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col17"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col18"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col19"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["Col20"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["NetAmt"].ToString() + "</td>");
                                        sb.Append("</tr>");

                                    }
                                }
                                sb.Append("</tbody></table>");

                            }
                            newbody = newbody.Replace("#*#*datatable-keytable*#*#", sb.ToString());

                            newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(dtCmpMst.Rows[0]["DepLogo"].ToString()) ? "<img src='/Uploads/" + dtCmpMst.Rows[0]["DepLogo"].ToString() + "' style='max-width:100 %;max-height: 100px; ' />" : string.Empty);

                            obj.Html = newbody;
                            obj.ToDt = todt.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(obj);
        }

        public IActionResult RawInwardPDFDetials(long id,  int companyid, int copyType = 1)
        {
            try
            {
                var Renderer = new IronPdf.ChromePdfRenderer();

                Renderer.RenderingOptions.SetCustomPaperSizeInInches(8.27, 11.69);
                //Renderer.RenderingOptions.PrintHtmlBackgrounds = true;
                Renderer.RenderingOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Landscape;
                //Renderer.RenderingOptions.EnableJavaScript = true;
                //Renderer.RenderingOptions.RenderDelay = 50; // in milliseconds
                //Renderer.RenderingOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Screen;
                Renderer.RenderingOptions.Zoom = 100;
                //Renderer.RenderingOptions.CreatePdfFormsFromHtml = true;

                // Supports margin customization!
                Renderer.RenderingOptions.MarginTop = 0;  //millimeters
                Renderer.RenderingOptions.MarginLeft = 10;  //millimeters
                Renderer.RenderingOptions.MarginRight = 10;  //millimeters
                Renderer.RenderingOptions.MarginBottom = 0;  //millimeters

                // Can set FirstPageNumber if you have a coverpage
                Renderer.RenderingOptions.FirstPageNumber = 1; // use 2 if a coverpage will be appended

                // Create a PDF from a URL or local file path
                var pdf = Renderer.RenderUrlAsPdf("http://piosunmark.pioerp.com/Print/RawInwardPrintDetials?id=" + id + "&companyid=" + companyid + "&copyType=" + copyType);

                //var pdf = Renderer.RenderHtmlAsPdf("Inward/RawInwardPrintDetials?id=" + id + "&copyType=" + copyType);
                //var pdf = Renderer.RenderUrlAsPdf("https://localhost:44317/Print/RawInwardPrintDetials?id=" + id + "&companyid=" + companyid + "&copyType=" + copyType);


                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@CmpVou", companyid);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                DataTable DtCom = ObjDBConnection.CallStoreProcedure("GetCompanyDetails", sqlParameters);
                if (DtCom != null && DtCom.Rows.Count > 0)
                {

                    string companynm = DtCom.Rows[0]["CmpName"].ToString().Substring(0, 4);
                    string path = _iwebhostenviroment.WebRootPath + "/PrintPDF";
                    string filnm = companynm + "-" + id + ".pdf";
                    string pdfpath = path + "/" + companynm + "-" + id + ".pdf";
                    // Export to a file or Stream
                    pdf.SaveAs(pdfpath);

                    if (System.IO.File.Exists(pdfpath))
                    {
                        //return Redirect("https://localhost:44317/PrintPdf/"+ filnm); // local url
                        return Redirect("http://piosunmark.pioerp.com/PrintPdf/" + filnm); // online url
                        //return File(System.IO.File.OpenRead(pdfpath), "application/octet-stream", Path.GetFileName(pdfpath));   
                    }

                    return NotFound();

                }

                return NotFound();
            }
            catch (Exception ex)
            {
                string path = _iwebhostenviroment.WebRootPath + "/PrintPDF";
                using var sw = new StreamWriter(path + "/error.text");
                sw.WriteLine(ex.Message);
                throw;
            }
            return View();
        }

        public IActionResult RawInwardPrintDetials(long id, int companyid, int copyType = 1)
        {
            InwardPrintDetails obj = new InwardPrintDetails();

            try
            {
                StringBuilder sb = new StringBuilder();

                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@InwVou", id);
                sqlParameters[1] = new SqlParameter("@Flg", 5);
                sqlParameters[2] = new SqlParameter("@cmpvou", companyid);
                DataTable DtInward = ObjDBConnection.CallStoreProcedure("GetInwardDetails", sqlParameters);
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
                    if (DtInward.Columns.Contains("DepAdd"))
                        CmpAdd = DtInward.Rows[0]["DepAdd"].ToString();
                    if (DtInward.Columns.Contains("DepEmail"))
                        CmpEmail = DtInward.Rows[0]["DepEmail"].ToString();
                    if (DtInward.Columns.Contains("DepVou"))
                        CmpVou = DtInward.Rows[0]["DepVou"].ToString();
                    if (DtInward.Columns.Contains("DepWeb"))
                        CmpWeb = DtInward.Rows[0]["DepWeb"].ToString();
                    if (DtInward.Columns.Contains("DepBusLine"))
                        CmpWeb = DtInward.Rows[0]["DepBusLine"].ToString();

                    Layout = "RawMatInward";
                    filename = "RawMatInward.html";

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        newbody = body.Replace("#*#*cmpAdd*#*#", CmpAdd.Replace(";", "<br>"));
                        newbody = newbody.Replace("#*#*cmpEmail*#*#", CmpEmail);
                        newbody = newbody.Replace("#*#*cmpWeb*#*#", CmpWeb);

                        string BilDate = DateTime.Parse(DtInward.Rows[0]["InwDt"].ToString()).ToString("dd-MM-yyyy");
                        newbody = newbody.Replace("#*#*r1*#*#", BilDate);
                        newbody = newbody.Replace("#*#*r2*#*#", DtInward.Rows[0]["AccNm"].ToString());
                        newbody = newbody.Replace("#*#*r3*#*#", DtInward.Rows[0]["InwVehNo"].ToString());
                        newbody = newbody.Replace("#*#*r4*#*#", "");
                        newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(DtInward.Rows[0]["DepLogo"].ToString()) ? "/Uploads/" + DtInward.Rows[0]["DepLogo"].ToString() + "" : string.Empty);


                        sb.Append("<table border=\"1\" cellpadding=\"5\" cellspacing=\"0\" width=\"100%\" style=\"margin-top:10px;\" > <thead><tr>");//datatable
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black; width:5px;\" rowspan=\"2\">SR<br />NO.</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black;\" rowspan=\"2\">COIL No.</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black;\" rowspan=\"2\">HEAT No.</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black;\" colspan=\"3\">AS PER INVOICE</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black;\" colspan=\"3\">ACTUAL</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black;\" rowspan=\"2\">REMARKS</th>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black;\">WIDTH</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black;\">THK</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black;\">WEIGHT</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black;width:9%;\">WIDTH</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black;\">THK</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:14px;color:black;\">WEIGHT</th>");
                        sb.Append("</tr></thead><tbody>");

                        double dTotWT = 0;
                        for (int i = 0; i < DtInward.Rows.Count; i++)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + (i + 1) + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtInward.Rows[i]["LotCoilNo"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtInward.Rows[i]["IntHeatNo"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtInward.Rows[i]["IntWidth"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtInward.Rows[i]["IntThick"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtInward.Rows[i]["IntQty"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"left\" style=\"font-size:14px;\">" + DtInward.Rows[i]["IntRem"].ToString() + "</td>");
                            dTotWT += Convert.ToDouble(DtInward.Rows[i]["IntQty"].ToString());
                            sb.Append("</tr>");
                        }

                        for (int i = DtInward.Rows.Count; i < 11; i++)
                        {
                            sb.Append("<tr style=\"height:31px;\">");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"left\" style=\"font-size:14px;\"></td>");
                            sb.Append("</tr>");
                        }

                        sb.Append("<tr>");
                        sb.Append("<td colspan=\"4\" style=\"border-right:none;\"></td>");
                        sb.Append("<td style=\"text-align:right;font-size:15px;padding-right:15px;border-left:none;font-weight:bold;\">TOTAL</td>");
                        sb.Append("<td style=\"text-align:center;font-size:15px;\">" + dTotWT.ToString("") + "</td>");
                        sb.Append("<td></td> <td></td> <td></td> <td></td>");
                        sb.Append("</tr>");

                        sb.Append("</tbody></table>");

                        newbody = newbody.Replace("#*#*r15*#*#", dTotWT.ToString(""));
                        newbody = newbody.Replace("#*#*r16*#*#", dTotWT.ToString("") + " kg");
                        newbody = newbody.Replace("#*#*r17*#*#", "");
                        newbody = newbody.Replace("#*#*r18*#*#", "");
                        newbody = newbody.Replace("#*#*r19*#*#", "");

                        newbody = newbody.Replace("#*#*datatable-keytable*#*#", sb.ToString());

                        obj.Html = newbody;
                        obj.Id = id.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(obj);
        }


    }
}