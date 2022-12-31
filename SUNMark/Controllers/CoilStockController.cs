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
    public class CoilStockController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        private readonly IWebHostEnvironment _iwebhostenviroment;

        public CoilStockController(IWebHostEnvironment iwebhostenviroment)
        {
            _iwebhostenviroment = iwebhostenviroment;
        }

        public IActionResult Index()
        {
            CoilMasterModel coilMasterModel = new CoilMasterModel();
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
                    coilMasterModel.FrRecDt = yearData.StartDate;
                    coilMasterModel.ToRecDt = yearData.EndDate;
                }
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                coilMasterModel.GradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                coilMasterModel.GodownList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                coilMasterModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                coilMasterModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                coilMasterModel.StockYNList = objProductHelper.GetStockYN();
                coilMasterModel.CoilTypeList = objProductHelper.GetMainCoilType();
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(coilMasterModel);

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

        public IActionResult CoilStockPrintDetials(string todt, int companyId)
        {
            CoilStockPrintDetails obj = new CoilStockPrintDetails();

            try
            {
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                long userId = GetIntSession("UserId");

                SqlParameter[] sqlPara = new SqlParameter[2];
                sqlPara[0] = new SqlParameter("@TODT", todt);
                sqlPara[1] = new SqlParameter("@CmpVou", companyId);
                DataTable DtGrd = ObjDBConnection.CallStoreProcedure("GEN_GRADEWISE_THICK_COLUMNER", sqlPara);


                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@CmpVou", companyId);
                DataSet ds = ObjDBConnection.GetDataSet("GETCOILSTOCK_REPORT", sqlParameters);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Count == 7)
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

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        if (dtCmpMst != null && dtCmpMst.Rows.Count > 0)
                        {
                            string newbody = body.Replace("#*#*Title*#*#", "Coil Stock Report");
                            if (companyId != 0)
                            {
                                newbody = newbody.Replace("#*#*cmpAdd*#*#", dtCmpMst.Rows[0]["DepAdd"].ToString());
                                newbody = newbody.Replace("#*#*cmpEmail*#*#", dtCmpMst.Rows[0]["DepEmail"].ToString());
                                newbody = newbody.Replace("#*#*cmpPhone*#*#", dtCmpMst.Rows[0]["DepMob"].ToString());
                                newbody = newbody.Replace("#*#*cmpGST*#*#", dtCmpMst.Rows[0]["DepGST"].ToString());
                            }
                            else
                            {
                                newbody = newbody.Replace("#*#*cmpAdd*#*#", dtCmpMst.Rows[0]["CmpName"].ToString());
                                newbody = newbody.Replace("#*#*cmpEmail*#*#", "");
                                newbody = newbody.Replace("#*#*cmpPhone*#*#", "");
                                newbody = newbody.Replace("#*#*cmpGST*#*#", "");
                            }

                            for (int i = 0; i < dtGrade.Rows.Count; i++)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append("<table border=\"1\" cellpadding=\"5\" cellspacing=\"0\" width=\"100%\" style=\"margin-top:10px;\" > <thead><tr>");//
                                sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black; width:5px;\" >"+ dtGrade.Rows[i]["MscNm"].ToString() + "</th></tr>");
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
                                sqlParam[0] = new SqlParameter("@Grade", dtGrade.Rows[i]["AccVou"].ToString());
                                DataTable DtData = ObjDBConnection.CallStoreProcedure("GETCOILSTOCK_REPORT1", sqlParam);
                                if (DtData != null && DtData.Rows.Count > 0)
                                {
                                    for (int s = 0; s < dtHead.Rows.Count; s++)
                                    {
                                        sb.Append("<tr>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtData.Rows[s]["InvNo"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col1"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col2"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col3"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col4"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col5"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col6"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col7"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col8"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col9"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col10"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col11"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col12"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col13"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col14"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col15"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col16"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col17"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col18"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col19"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + dtHead.Rows[s]["Col20"].ToString() + "</td>");
                                        sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                                        sb.Append("</tr>");

                                    }
                                }
                                sb.Append("</tbody></table>");
                                newbody = newbody.Replace("#*#*datatable-keytable*#*#", sb.ToString());
                            }
                            if (companyId != 0) 
                            {
                                newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(dtCmpMst.Rows[0]["DepLogo"].ToString()) ? "<img src='/Uploads/" + dtCmpMst.Rows[0]["DepLogo"].ToString() + "' style='max-width:100 %;max-height: 100px; ' />" : string.Empty);
                            }
                            else
                            {
                                newbody = newbody.Replace("#*#*logo*#*#", string.Empty);
                            }

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

    }
}
