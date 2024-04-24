using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SUNMark.Classes;
using SUNMark.Models;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using SUNMark.Common;

namespace SUNMark.Controllers
{
    public class GodownTransferController : BaseController
    {
        static string GdmVou = string.Empty;
        DbConnection ObjDBConnection = new DbConnection();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        ProductHelpers objProductHelper = new ProductHelpers();
        private readonly IWebHostEnvironment _iwebhostenviroment;
        public GodownTransferController(IWebHostEnvironment iwebhostenviroment)
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
                GdmVou = "0";
                if (id > 0)
                {
                    GdmVou = Convert.ToString(id);
                    TempData["ReturnId"] = Convert.ToString(id);
                }
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
            ViewBag.gradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
            ViewBag.productList = objProductHelper.GetProductMasterDropdown(companyId);
            ViewBag.processDoneList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator,"");
            ViewBag.productTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
            ViewBag.companyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
            ViewBag.godownList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
            ViewBag.locationList = objProductHelper.GetLocationMasterDropdown(companyId, administrator);
            ViewBag.voucherNo = GetVoucherNo();
        }

        private string GetVoucherNo()
        {
            string returnValue = string.Empty;
            try
            {
                DataTable dtVoucherNo = ObjDBConnection.CallStoreProcedure("GetGodownTransferVoucherNo", null);
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

        public JsonResult AddGodownTransfer(GodownTransferModel godownTransferModel)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                var productTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                var processDoneList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, 0,"");
                string productType = string.Empty;
                if (productTypeList != null && productTypeList.Count > 0)
                {
                    productType = productTypeList.Where(x => x.Value.Equals(godownTransferModel.ProductType)).Select(x => x.Text).FirstOrDefault();
                }
                SqlParameter[] parameter = new SqlParameter[14];
                parameter[0] = new SqlParameter("@GdmVou", godownTransferModel.Vou);
                parameter[1] = new SqlParameter("@GdmVNo", godownTransferModel.Vno);
                parameter[2] = new SqlParameter("@GdmCmpVou", godownTransferModel.CmpVou);
                parameter[3] = new SqlParameter("@GdmRef", godownTransferModel.ChallanNo);
                parameter[4] = new SqlParameter("@GdmDt", godownTransferModel.Date);
                parameter[5] = new SqlParameter("@GdmFrGdnVou", godownTransferModel.FromGodown);
                parameter[6] = new SqlParameter("@GdmToGdnVou", godownTransferModel.ToGodown);
                parameter[7] = new SqlParameter("@GdmPrdTyp", productType);
                parameter[8] = new SqlParameter("@GdmPtyVou", godownTransferModel.ProductType);
                parameter[9] = new SqlParameter("@GdmRem", godownTransferModel.Remarks);
                parameter[10] = new SqlParameter("@GdmLocVou", godownTransferModel.Location);
                parameter[11] = new SqlParameter("@GdmVehNo", godownTransferModel.VehicleNo);
                parameter[12] = new SqlParameter("@GdmMOTrans", godownTransferModel.ModeOfTransport);
                parameter[13] = new SqlParameter("@GdmPurpose", godownTransferModel.Purpose);
                DataTable dt = ObjDBConnection.CallStoreProcedure("GodownMasterAdd", parameter);
                if (dt != null && dt.Rows.Count > 0)
                {
                    int gdmVou = Convert.ToInt32(dt.Rows[0][0].ToString());
                    if (gdmVou > 0)
                    {
                        DataTable dtInsert = new DataTable();
                        dtInsert.Columns.Add("GdtVou");
                        dtInsert.Columns.Add("GdtGdmVou");
                        dtInsert.Columns.Add("GdtSrNo");
                        dtInsert.Columns.Add("GdtPrdVou");
                        dtInsert.Columns.Add("GdtGrade");
                        dtInsert.Columns.Add("GdtWidth");
                        dtInsert.Columns.Add("GdtThick");
                        dtInsert.Columns.Add("GdtQty");
                        dtInsert.Columns.Add("GdtOD");
                        dtInsert.Columns.Add("GdtPcs");
                        dtInsert.Columns.Add("GdtFeetPer");
                        dtInsert.Columns.Add("GdtRem");
                        dtInsert.Columns.Add("GdtFinishVou");
                        dtInsert.Columns.Add("GdtFinish");
                        dtInsert.Columns.Add("GdtCoilNo");
                        dtInsert.Columns.Add("GdtNB");
                        dtInsert.Columns.Add("GdtSCH");

                        if (productType.ToUpper().Equals("COIL"))
                        {
                            if (godownTransferModel.Grade != null && godownTransferModel.Grade.Length > 0 && godownTransferModel.Thick != null && godownTransferModel.Thick.Length > 0 && godownTransferModel.Width != null && godownTransferModel.Width.Length > 0 && godownTransferModel.Qty != null && godownTransferModel.Qty.Length > 0)

                                for (int i = 0; i < godownTransferModel.Grade.Length; i++)
                                {
                                    DataRow dr = dtInsert.NewRow();
                                    dr["GdtVou"] = "0";
                                    dr["GdtGdmVou"] = gdmVou.ToString();
                                    dr["GdtSrNo"] = (i + 1).ToString();
                                    dr["GdtPrdVou"] = null;
                                    dr["GdtGrade"] = godownTransferModel.Grade[i].ToString();
                                    dr["GdtWidth"] = godownTransferModel.Width[i].ToString();
                                    dr["GdtThick"] = godownTransferModel.Thick[i].ToString();
                                    dr["GdtQty"] = godownTransferModel.Qty[i].ToString();
                                    dr["GdtOD"] = "0";
                                    dr["GdtPcs"] = "0";
                                    dr["GdtFeetPer"] = "0";
                                    dr["GdtRem"] = godownTransferModel.GridRemarks[i];
                                    dr["GdtFinishVou"] = null;
                                    dr["GdtFinish"] = null;
                                    dr["GdtCoilNo"] = godownTransferModel.CoilNo[i];
                                    dr["GdtNB"] = "";
                                    dr["GdtSCH"] = "";
                                    dtInsert.Rows.Add(dr);
                                }
                        }
                        else if (productType.ToUpper().Equals("PIPE"))
                        {
                            if (godownTransferModel.Grade != null && godownTransferModel.Grade.Length > 0 && godownTransferModel.Thick != null && godownTransferModel.Thick.Length > 0 && godownTransferModel.OD != null && godownTransferModel.OD.Length > 0 &&
                                godownTransferModel.FeetPer != null && godownTransferModel.FeetPer.Length > 0 &&
                                godownTransferModel.PCS != null && godownTransferModel.PCS.Length > 0 &&
                                godownTransferModel.ProcessDone != null && godownTransferModel.ProcessDone.Length > 0)

                                for (int i = 0; i < godownTransferModel.Grade.Length; i++)
                                {
                                    string processDone = string.Empty;
                                    if (processDoneList != null && processDoneList.Count > 0)
                                    {
                                        processDone = processDoneList.Where(x => x.Value.Equals(godownTransferModel.ProcessDone[i])).Select(x => x.Text).FirstOrDefault();
                                    }
                                    DataRow dr = dtInsert.NewRow();
                                    dr["GdtVou"] = "0";
                                    dr["GdtGdmVou"] = gdmVou.ToString();
                                    dr["GdtSrNo"] = (i + 1).ToString();
                                    dr["GdtPrdVou"] = null;
                                    dr["GdtGrade"] = godownTransferModel.Grade[i].ToString();
                                    dr["GdtWidth"] = "0";
                                    dr["GdtThick"] = godownTransferModel.Thick[i].ToString();
                                    dr["GdtQty"] = "0";
                                    dr["GdtOD"] = godownTransferModel.OD[i].ToString();
                                    dr["GdtPcs"] = godownTransferModel.PCS[i].ToString();
                                    dr["GdtFeetPer"] = godownTransferModel.FeetPer[i].ToString();
                                    dr["GdtRem"] = godownTransferModel.GridRemarks[i];
                                    dr["GdtFinishVou"] = godownTransferModel.ProcessDone[i].ToString();
                                    dr["GdtFinish"] = processDone;
                                    dr["GdtNB"] = godownTransferModel.NB[i].ToString();
                                    dr["GdtSCH"] = godownTransferModel.SCH[i].ToString();
                                    dtInsert.Rows.Add(dr);
                                }
                        }
                        else
                        {
                            if (godownTransferModel.Product != null && godownTransferModel.Product.Length > 0 && godownTransferModel.Qty != null && godownTransferModel.Qty.Length > 0)

                                for (int i = 0; i < godownTransferModel.Product.Length; i++)
                                {
                                    DataRow dr = dtInsert.NewRow();
                                    dr["GdtVou"] = "0";
                                    dr["GdtGdmVou"] = gdmVou.ToString();
                                    dr["GdtSrNo"] = (i + 1).ToString();
                                    dr["GdtPrdVou"] = godownTransferModel.Product[i].ToString();
                                    dr["GdtGrade"] = "";
                                    dr["GdtWidth"] = "0";
                                    dr["GdtThick"] = "0";
                                    dr["GdtQty"] = godownTransferModel.Qty[i].ToString();
                                    dr["GdtOD"] = "0";
                                    dr["GdtPcs"] = "0";
                                    dr["GdtFeetPer"] = "0";
                                    dr["GdtRem"] = godownTransferModel.GridRemarks[i];
                                    dr["GdtFinishVou"] = null;
                                    dr["GdtFinish"] = "";
                                    dr["GdtNB"] = "";
                                    dr["GdtSCH"] = "";
                                    dtInsert.Rows.Add(dr);
                                }
                        }
                        int status = DbConnection.SqlBulkInsertToTable(dtInsert, "gdntrntrn");
                        if (status > 0)
                        {
                            SetSuccessMessage("Godown transfer saved successfully.");
                            if (godownTransferModel.isPrint != 0)
                            {
                                TempData["ReturnId"] = godownTransferModel.Vou;
                                TempData["Savedone"] = "1";
                                TempData["IsPrint"] = godownTransferModel.isPrint;
                            }
                            return Json(new { result = true, message = "Godown transfer saved successfully" });
                        }
                        else
                        {
                            return Json(new { result = false, message = "Godown transfer not saved" });
                        }

                    }
                    else
                    {
                        return Json(new { result = false, message = "Godown transfer not saved" });
                    }
                }
                else
                {
                    return Json(new { result = false, message = "Godown transfer not saved" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
        }

        public ActionResult GetDataById(string id)
        {
            GodownTransferModel godownTransferModel = new GodownTransferModel();
            try
            {
                if (int.Parse(id) > 0)
                {
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] parameter = new SqlParameter[1];
                    parameter[0] = new SqlParameter("@GdmVou", id);
                    GdmVou = id;
                    DataSet ds = ObjDBConnection.GetDataSet("GetGodownTransferDataById", parameter);
                    if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Count == 2)
                    {
                        DataTable dtGdnMst = ds.Tables[0];
                        DataTable dtGdnTrn = ds.Tables[1];
                        if (dtGdnMst != null && dtGdnMst.Rows.Count > 0)
                        {
                            godownTransferModel.Vno = dtGdnMst.Rows[0]["GdmVNo"].ToString();
                            godownTransferModel.Date = Convert.ToDateTime(dtGdnMst.Rows[0]["GdmDt"].ToString()).ToString("yyyy-MM-dd");
                            godownTransferModel.CmpVou = dtGdnMst.Rows[0]["GdmCmpVou"].ToString();
                            godownTransferModel.ChallanNo = dtGdnMst.Rows[0]["GdmRef"].ToString();
                            godownTransferModel.FromGodown = dtGdnMst.Rows[0]["GdmFrGdnVou"].ToString();
                            godownTransferModel.ToGodown = dtGdnMst.Rows[0]["GdmToGdnVou"].ToString();
                            godownTransferModel.ProductType = dtGdnMst.Rows[0]["GdmPtyVou"].ToString();
                            godownTransferModel.ProductTypeString = dtGdnMst.Rows[0]["GdmPrdTyp"].ToString();
                            godownTransferModel.Remarks = dtGdnMst.Rows[0]["GdmRem"].ToString();
                            godownTransferModel.VehicleNo = dtGdnMst.Rows[0]["GdmVehNo"].ToString();
                            godownTransferModel.ModeOfTransport = dtGdnMst.Rows[0]["GdmMOTrans"].ToString();
                            godownTransferModel.Purpose = dtGdnMst.Rows[0]["GdmPurpose"].ToString();
                            godownTransferModel.Location = dtGdnMst.Rows[0]["GdmLocVou"].ToString();
                        }
                        if (dtGdnTrn != null && dtGdnTrn.Rows.Count > 0)
                        {
                            List<JobWorkEntrySlit> jobWorkEntrySlits = new List<JobWorkEntrySlit>();
                            List<JobWorkEntrySplit> jobWorkEntrySplits = new List<JobWorkEntrySplit>();
                            if (dtGdnMst.Rows[0]["GdmPrdTyp"].ToString().ToUpper() == "COIL")
                            {
                                string[] coilList = new string[dtGdnTrn.Rows.Count];
                                string[] gradeList = new string[dtGdnTrn.Rows.Count];
                                string[] thickList = new string[dtGdnTrn.Rows.Count];
                                string[] qtyList = new string[dtGdnTrn.Rows.Count];
                                string[] widthList = new string[dtGdnTrn.Rows.Count];
                                string[] remarksList = new string[dtGdnTrn.Rows.Count];
                                for (int i = 0; i < dtGdnTrn.Rows.Count; i++)
                                {
                                    coilList[i] = dtGdnTrn.Rows[i]["GdtGrade"].ToString();
                                    gradeList[i] = dtGdnTrn.Rows[i]["GdtGrade"].ToString();
                                    thickList[i] = dtGdnTrn.Rows[i]["GdtThick"].ToString();
                                    qtyList[i] = dtGdnTrn.Rows[i]["GdtQty"].ToString();
                                    widthList[i] = dtGdnTrn.Rows[i]["GdtWidth"].ToString();
                                    remarksList[i] = dtGdnTrn.Rows[i]["GdtRem"].ToString();
                                    coilList[i] = dtGdnTrn.Rows[i]["GdtCoilNo"].ToString();
                                }
                                godownTransferModel.CoilNo = coilList;
                                godownTransferModel.Grade = gradeList;
                                godownTransferModel.Thick = thickList;
                                godownTransferModel.Qty = qtyList;
                                godownTransferModel.Width = widthList;
                                godownTransferModel.GridRemarks = remarksList;
                            }
                            else if (dtGdnMst.Rows[0]["GdmPrdTyp"].ToString().ToUpper() == "PIPE")
                            {
                                string[] gradeList = new string[dtGdnTrn.Rows.Count];
                                string[] thickList = new string[dtGdnTrn.Rows.Count];
                                string[] odList = new string[dtGdnTrn.Rows.Count];
                                string[] feetPerList = new string[dtGdnTrn.Rows.Count];
                                string[] pcsList = new string[dtGdnTrn.Rows.Count];
                                string[] processDoneList = new string[dtGdnTrn.Rows.Count];
                                string[] remarksList = new string[dtGdnTrn.Rows.Count];
                                string[] nbList = new string[dtGdnTrn.Rows.Count];
                                string[] schList= new string[dtGdnTrn.Rows.Count];
                                for (int i = 0; i < dtGdnTrn.Rows.Count; i++)
                                {
                                    gradeList[i] = dtGdnTrn.Rows[i]["GdtGrade"].ToString();
                                    thickList[i] = dtGdnTrn.Rows[i]["GdtThick"].ToString();
                                    odList[i] = dtGdnTrn.Rows[i]["GdtOD"].ToString();
                                    feetPerList[i] = dtGdnTrn.Rows[i]["GdtFeetPer"].ToString();
                                    pcsList[i] = dtGdnTrn.Rows[i]["GdtPcs"].ToString();
                                    processDoneList[i] = dtGdnTrn.Rows[i]["GdtFinishVou"].ToString();
                                    remarksList[i] = dtGdnTrn.Rows[i]["GdtRem"].ToString();
                                    var values = GetNBSCHValue(thickList[i], odList[i]);
                                    if(values!=null && values.Length>0)
                                    {
                                        nbList[i] = values[0];
                                        schList[i] = values[0];
                                    }
                                }
                                godownTransferModel.Grade = gradeList;
                                godownTransferModel.Thick = thickList;
                                godownTransferModel.OD = odList;
                                godownTransferModel.FeetPer = feetPerList;
                                godownTransferModel.PCS = pcsList;
                                godownTransferModel.ProcessDone = processDoneList;
                                godownTransferModel.GridRemarks = remarksList;
                                godownTransferModel.NB = nbList;
                                godownTransferModel.SCH= schList;
                                
                            }
                            else
                            {
                                string[] productList = new string[dtGdnTrn.Rows.Count];
                                string[] qtyList = new string[dtGdnTrn.Rows.Count];
                                string[] remarksList = new string[dtGdnTrn.Rows.Count];
                                for (int i = 0; i < dtGdnTrn.Rows.Count; i++)
                                {
                                    productList[i] = dtGdnTrn.Rows[i]["GdtPrdVou"].ToString();
                                    qtyList[i] = dtGdnTrn.Rows[i]["GdtQty"].ToString();
                                    remarksList[i] = dtGdnTrn.Rows[i]["GdtRem"].ToString();
                                }
                                godownTransferModel.Qty = qtyList;
                                godownTransferModel.Product = productList;
                                godownTransferModel.GridRemarks = remarksList;
                            }
                        }

                        return Json(new { result = true, data = godownTransferModel });
                    }
                    else
                    {
                        return Json(new { result = false, message = "Godown transfer not found!" });
                    }
                }
                else
                {
                    return Json(new { result = false, message = "Godown transfer not found!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
        }

        private string[] GetNBSCHValue(string thickpipe,string od)
        {
            string[] values = new string[2];
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@thickpipe", thickpipe);
            sqlParameters[1] = new SqlParameter("@odpipe", od);
            DataTable dtNBSCH = ObjDBConnection.CallStoreProcedure("GetNBSCHDetails", sqlParameters);
            if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
            {
                values[0] = dtNBSCH.Rows[0]["OrdNB"].ToString();
                values[1] = dtNBSCH.Rows[0]["OrdSch"].ToString();
                
            }
            return values;
        }

        public ActionResult GetDataByCoilNo(string coilNo)
        {
            try
            {
                SqlParameter[] parameter = new SqlParameter[2];

                parameter[0] = new SqlParameter("@COILNO", coilNo);
                if (GdmVou == "0")
                {
                    parameter[1] = new SqlParameter("@FLG", 0);
                }
                else
                {
                    parameter[1] = new SqlParameter("@FLG", 1);
                }
                DataSet ds = ObjDBConnection.GetDataSet("GetDataByCoilNo", parameter);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables.Count == 5)
                {
                    List<object> data = new List<object>();
                    DataTable dtLotMst = ds.Tables[0];
                    if (dtLotMst != null && dtLotMst.Rows.Count > 0)
                    {
                        data.Add(dtLotMst.Rows[0]["LotGrade"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotThick"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotWidth"].ToString());
                        data.Add(dtLotMst.Rows[0]["LotQty"].ToString());
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

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby, string gdnVNo)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                if (gridMstId > 0)
                {
                    #region User Rights
                    long userId = GetIntSession("UserId");
                    UserFormRightModel userFormRights = new UserFormRightModel();
                    string currentURL = "/GodownTransfer/Index";
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
                    string whereConditionQuery = string.Empty;
                    if (!string.IsNullOrWhiteSpace(gdnVNo))
                    {
                        whereConditionQuery += " AND GdnTrnMst.GdmVNo='" + gdnVNo + "'";
                    }
                    getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId,0,0,"",0,0, whereConditionQuery);
                    if (getReportDataModel.IsError)
                    {
                        ViewBag.Query = getReportDataModel.Query;
                        return PartialView("_reportView");
                    }
                    getReportDataModel.pageIndex = pageIndex;
					getReportDataModel.ControllerName = "GodownTransfer";
				}
			}
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }
        public IActionResult ExportToExcelPDF(int gridMstId, string searchValue, int type, string gdnVNo)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                var companyDetails = DbConnection.GetCompanyDetailsById(companyId);
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                //getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, 0, "", 0, 1);
                string whereConditionQuery = string.Empty;
                if (!string.IsNullOrWhiteSpace(gdnVNo))
                {
                    whereConditionQuery += " AND GdnTrnMst.GdmVNo='" + gdnVNo + "'";
                }

                getReportDataModel = getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, YearId, "", 0, 1,whereConditionQuery);
                if (type == 1)
                {
                    var bytes = Excel(getReportDataModel, "Godown Transfer Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "GodownTransfer.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Godown Transfer Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "GodownTransfer.pdf");
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
                GodownTransferModel godownTransferModel = new GodownTransferModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@GdmVou", id);
                    sqlParameters[1] = new SqlParameter("@CmpVou", companyId);
                    DataTable dtGdnMst = ObjDBConnection.CallStoreProcedure("GdnMstTrn_Delete", sqlParameters);
                    if (dtGdnMst != null && dtGdnMst.Rows.Count > 0)
                    {
                        int value = DbConnection.ParseInt32(dtGdnMst.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("Godown Transfer not deleted");
                        }
                        else
                        {
                            SetSuccessMessage("Godown Transfer deleted successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "GodownTransfer");
        }
        public GdnTrnPrintDetails GetGdnTrnHtmlData(long id)
        {
            GdnTrnPrintDetails obj = new GdnTrnPrintDetails();

            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                long userId = GetIntSession("UserId");

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@GDMVOU", id);
                DataTable DtInward = ObjDBConnection.CallStoreProcedure("GetGodownTransferDetails", sqlParameters);
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

                    Layout = "GodownTransfer";
                    filename = "GodownTransfer.html";

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        newbody = body.Replace("#*#*cmpAdd*#*#", CmpAdd);
                        newbody = newbody.Replace("#*#*cmpEmail*#*#", CmpEmail);
                        newbody = newbody.Replace("#*#*cmpWeb*#*#", CmpWeb);

                        string BilDate = DateTime.Parse(DtInward.Rows[0]["GdmDt"].ToString()).ToString("dd-MM-yyyy");
                        newbody = newbody.Replace("#*#*r1*#*#", BilDate);
                        string sDepnm = DtInward.Rows[0]["AccName"].ToString();
                        if (DtInward.Rows[0]["Togodown"].ToString() != "")
                            sDepnm = sDepnm + " ( " + DtInward.Rows[0]["Togodown"].ToString() + " ) ";
                        newbody = newbody.Replace("#*#*r2*#*#", sDepnm);
                        newbody = newbody.Replace("#*#*r2A*#*#", DtInward.Rows[0]["frgodown"].ToString());
                        newbody = newbody.Replace("#*#*chalnno*#*#", DtInward.Rows[0]["gdmref"].ToString());
                        newbody = newbody.Replace("#*#*r3*#*#", DtInward.Rows[0]["GdmVehNo"].ToString());
                        newbody = newbody.Replace("#*#*r4*#*#", "");
                        //newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(DtInward.Rows[0]["DepLogo"].ToString()) ? "/Uploads/" + DtInward.Rows[0]["DepLogo"].ToString() + "" : string.Empty);
                        newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(DtInward.Rows[0]["DepLogo"].ToString()) ? "http://piosunmark.pioerp.com/Uploads/" + DtInward.Rows[0]["DepLogo"].ToString() + "" : string.Empty);

                        StringBuilder sb = new StringBuilder();

                        sb.Append("<table border=\"1\" cellpadding=\"5\" cellspacing=\"0\" width=\"100%\" style=\"margin-top:10px;\" > <thead><tr>");//datatable
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:3%;\">SrNo</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:10%;\">Grade</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:20%;\">Coil No./No of Pcs.</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:10%;\">Width/Size</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:10%;\">THK</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:10%;\">Weight</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:20%;\">Remarks</th>");
                        sb.Append("</tr></thead><tbody>");

                        double dTotWT = 0;
                        for (int i = 0; i < DtInward.Rows.Count; i++)
                        {
                            string sRem = DtInward.Rows[i]["GdtRem"].ToString();
                            sRem = sRem == null ? "" : sRem;
                            sb.Append("<tr>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + (i + 1).ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtInward.Rows[i]["GdtGrade"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtInward.Rows[i]["GdtCoilNo"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtInward.Rows[i]["GdtWidth"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtInward.Rows[i]["GdtThick"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtInward.Rows[i]["GdtQty"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + sRem + "</td>");

                            dTotWT += Convert.ToDouble(DtInward.Rows[i]["GdtQty"].ToString());

                            sb.Append("</tr>");
                        }
                        for (int i = 0; i < DtInward.Rows.Count; i++)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("</tr>");
                        }

                        sb.Append("<tr style=\"font-weight:bold; \">");
                        sb.Append("<td></td> <td></td> <td></td> <td></td> <td></td>");
                        sb.Append("<td style=\"text-align:center;font-size:15px;\">" + dTotWT.ToString("") + "</td>");
                        sb.Append("<td></td>");
                        sb.Append("</tr>");

                        sb.Append("</tbody></table>");

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
            return obj;
        }
        public IActionResult GdnTrnPrintDetials(long id, int copyType = 1)
        {
            try
            {
                GdnTrnPrintDetails obj = GetGdnTrnHtmlData(id);

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
        public IActionResult GdnTrnSendMail(long id, string email = "")
        {
            try
            {
                GdnTrnPrintDetails obj = GetGdnTrnHtmlData(id);
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

                bool result = SendEmail(email, "GODOWN TRANSFER REPORT", "Please find attachment", wwwroot);
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
        public IActionResult GdnTrWhatApp(long id, string whatappNo = "")
        {
            try
            {
                GdnTrnPrintDetails obj = GetGdnTrnHtmlData(id);
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

                WhatAppAPIResponse apiResponse = SendWhatAppMessage(whatappNo, "GODOWN TRANSFER REPORT", wwwroot, filenm);
                return Json(new { result = apiResponse.status, message = apiResponse.message });
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
