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
    public class OpeningStockPipeController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        OpeningStokPipeModel objOpeningStock = new OpeningStokPipeModel();
        public IActionResult Index(long id)
        {

            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                    return RedirectToAction("index", "dashboard");

                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                SqlParameter[] sqlPara = new SqlParameter[4];
                sqlPara[0] = new SqlParameter("@OblVou", id);
                sqlPara[1] = new SqlParameter("@Flg", 4);
                sqlPara[2] = new SqlParameter("@CmpVou", companyId);
                sqlPara[3] = new SqlParameter("@OblPrdTyp", "PIPE");
                DataTable DtOBL = ObjDBConnection.CallStoreProcedure("GetOBLMSTDetails", sqlPara);
                if (DtOBL.Rows.Count > 0)
                {
                    objOpeningStock.OblDt = !string.IsNullOrWhiteSpace(DtOBL.Rows[0]["OblDt"].ToString()) ? Convert.ToDateTime(DtOBL.Rows[0]["OblDt"].ToString()).ToString("yyyy-MM-dd") : "";
                    objOpeningStock.OblCmpVou = int.Parse(DtOBL.Rows[0]["OblCmpVou"].ToString());
                    objOpeningStock.OblGdnVou = int.Parse(DtOBL.Rows[0]["OblGdnVou"].ToString());
                    objOpeningStock.LotHeatNo = DtOBL.Rows[0]["LotHeatNo"].ToString();
                    objOpeningStock.LotFinish = int.Parse(DtOBL.Rows[0]["OblFinMscVou"].ToString());
                    objOpeningStock.Finish = DtOBL.Rows[0]["LotFinish"].ToString();
                    objOpeningStock.OblAccVou = int.Parse(DtOBL.Rows[0]["OblAccVou"].ToString());
                    objOpeningStock.LotGrade = int.Parse(DtOBL.Rows[0]["OblGrdMscVou"].ToString());
                    objOpeningStock.Grade = DtOBL.Rows[0]["LotGrade"].ToString();
                    objOpeningStock.OblPrdVou = int.Parse(DtOBL.Rows[0]["OblPrdVou"].ToString());
                    objOpeningStock.LotType = DtOBL.Rows[0]["LotTyp"].ToString();
                }

                if (id > 0)
                {
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@OblVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 1);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    sqlParameters[3] = new SqlParameter("@OblPrdTyp", "PIPE");
                    DataTable DtOBLDetail = ObjDBConnection.CallStoreProcedure("GetOBLMSTDetails", sqlParameters);

                    if (DtOBLDetail.Rows.Count > 0)
                    {
                        objOpeningStock.OblVou = int.Parse(DtOBLDetail.Rows[0]["OblVou"].ToString());

                        objOpeningStock.OblNVno = int.Parse(DtOBLDetail.Rows[0]["OblNVno"].ToString());
                        objOpeningStock.OblDt = !string.IsNullOrWhiteSpace(DtOBLDetail.Rows[0]["OblDt"].ToString()) ? Convert.ToDateTime(DtOBLDetail.Rows[0]["OblDt"].ToString()).ToString("yyyy-MM-dd") : "";

                        objOpeningStock.OblCmpVou = int.Parse(DtOBLDetail.Rows[0]["OblCmpVou"].ToString());
                        objOpeningStock.OblGdnVou = int.Parse(DtOBLDetail.Rows[0]["OblGdnVou"].ToString());
                        objOpeningStock.OblLocVou = int.Parse(DtOBLDetail.Rows[0]["OblLocVou"].ToString());
                        objOpeningStock.OblAccVou = int.Parse(DtOBLDetail.Rows[0]["OblAccVou"].ToString());
                        objOpeningStock.LotGrade = int.Parse(DtOBLDetail.Rows[0]["LotGrade"].ToString());
                        objOpeningStock.Grade = DtOBLDetail.Rows[0]["Grade"].ToString();
                        objOpeningStock.LotFinish = int.Parse(DtOBLDetail.Rows[0]["LotFinMscVou"].ToString());
                        objOpeningStock.Finish= DtOBLDetail.Rows[0]["Finish"].ToString();
                        objOpeningStock.OblPrdVou = int.Parse(DtOBLDetail.Rows[0]["OblPrdVou"].ToString());
                        objOpeningStock.LotSupCoilNo = DtOBLDetail.Rows[0]["LotSupCoilNo"].ToString();
                        objOpeningStock.LotQty = decimal.Parse(DtOBLDetail.Rows[0]["LotQty"].ToString());
                        objOpeningStock.LotWidth = decimal.Parse(DtOBLDetail.Rows[0]["LotWidth"].ToString());
                        objOpeningStock.LotThick = decimal.Parse(DtOBLDetail.Rows[0]["LotThick"].ToString());
                        objOpeningStock.LotInwQty = decimal.Parse(DtOBLDetail.Rows[0]["LotInwQty"].ToString());
                        objOpeningStock.LotInwWidth = decimal.Parse(DtOBLDetail.Rows[0]["LotInwWidth"].ToString());
                        objOpeningStock.LotInwThick = decimal.Parse(DtOBLDetail.Rows[0]["LotInwThick"].ToString());
                        objOpeningStock.LotHeatNo = DtOBLDetail.Rows[0]["LotHeatNo"].ToString();
                        objOpeningStock.LotNB = DtOBLDetail.Rows[0]["LotNB"].ToString();
                        objOpeningStock.LotSCH = DtOBLDetail.Rows[0]["LotSCH"].ToString();
                        //objOpeningStock.LotPrcTypVou = int.Parse(DtOBLDetail.Rows[0]["LotPrcTypCd"].ToString());
                        //objOpeningStock.CoilNo = string.IsNullOrEmpty(DtOBLDetail.Rows[0]["LotCoilNo"].ToString()) ? 0 : int.Parse(DtOBLDetail.Rows[0]["LotCoilNo"].ToString().Split('_')[1].ToString());
                        objOpeningStock.LotPrcTypVou = 0;
                        
                        objOpeningStock.LotOD = decimal.Parse(DtOBLDetail.Rows[0]["LotOD"].ToString());
                        objOpeningStock.FeetPer = decimal.Parse(DtOBLDetail.Rows[0]["LotFeetPer"].ToString());
                        objOpeningStock.PCS = decimal.Parse(DtOBLDetail.Rows[0]["LotPCS"].ToString());
                        objOpeningStock.LotPrcTypVou = int.Parse(DtOBLDetail.Rows[0]["LotPrcTypCD"].ToString());
                        objOpeningStock.OblRem = DtOBLDetail.Rows[0]["OblRem"].ToString();   
                    }
                }
                objOpeningStock.OblCmpVouList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                objOpeningStock.LotTypVouList = objProductHelper.GetLotMasterDropdown(companyId, administrator);
                objOpeningStock.OblPrdVouList = objProductHelper.GetProductMasterDropdown(companyId, administrator, "PIPE");
                objOpeningStock.LotFinishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                objOpeningStock.OblGdnVouList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                objOpeningStock.OblLocVouList = objProductHelper.GetLocationMasterDropdown(companyId, administrator);
                objOpeningStock.OblAccVouList = objProductHelper.GetSupplierMasterDropdown(companyId, administrator);
                objOpeningStock.LotGradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                objOpeningStock.LotPrcTypVouList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                return View(objOpeningStock);
            }
            catch (Exception ex)
            {
                throw;
            }
            return View();
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
        }

        [HttpPost]
        public ActionResult index(long id, OpeningStokPipeModel obj)
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
                int administrator = 0;
                if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(obj.OblCmpVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(obj.OblGdnVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(obj.OblLocVou).ToString()))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[32];
                    sqlParameters[0] = new SqlParameter("@OblNVno", obj.OblNVno);
                    sqlParameters[1] = new SqlParameter("@OblDt", DateTime.Parse(obj.OblDt));
                    sqlParameters[2] = new SqlParameter("@OblCmpVou", obj.OblCmpVou);
                    sqlParameters[3] = new SqlParameter("@OblGdnVou", obj.OblGdnVou);
                    sqlParameters[4] = new SqlParameter("@OblLocVou", obj.OblLocVou);
                    sqlParameters[5] = new SqlParameter("@OblAccVou", obj.OblAccVou);
                    sqlParameters[6] = new SqlParameter("@OblPrdVou", obj.OblPrdVou);
                    sqlParameters[7] = new SqlParameter("@OblRem", obj.OblRem);
                    sqlParameters[8] = new SqlParameter("@LotSupCoilNo", obj.LotSupCoilNo);
                    sqlParameters[9] = new SqlParameter("@LotHeatNo", obj.LotHeatNo);
                    sqlParameters[10] = new SqlParameter("@LotGrdMscVou", obj.LotGrade);
                    sqlParameters[11] = new SqlParameter("@OblGrade", obj.Grade);
                    sqlParameters[12] = new SqlParameter("@LotFinMscVou", obj.LotFinish);
                    sqlParameters[13] = new SqlParameter("@OblFinish", obj.Finish);
                    sqlParameters[14] = new SqlParameter("@LotWidth", obj.LotWidth);
                    sqlParameters[15] = new SqlParameter("@LotThick", obj.LotThick);
                    sqlParameters[16] = new SqlParameter("@LotQty", obj.LotQty);
                    sqlParameters[17] = new SqlParameter("@LotInwWidth", obj.LotInwWidth);
                    sqlParameters[18] = new SqlParameter("@LotInwThick", obj.LotInwThick);
                    sqlParameters[19] = new SqlParameter("@LotInwQty", obj.LotInwQty);
                    sqlParameters[20] = new SqlParameter("@PrdTyp", "PIPE");
                    sqlParameters[21] = new SqlParameter("@CmpVou", companyId);
                    sqlParameters[22] = new SqlParameter("@OblVou", id);
                    sqlParameters[23] = new SqlParameter("@LotTyp", "");
                    sqlParameters[24] = new SqlParameter("@CoilNo", obj.CoilNo);

                    sqlParameters[25] = new SqlParameter("@LotPrcTypCD", obj.LotPrcTypVou);
                    sqlParameters[26] = new SqlParameter("@LotOD", obj.LotOD);
                    sqlParameters[27] = new SqlParameter("@LotPCS", obj.PCS);
                    sqlParameters[28] = new SqlParameter("@LotFeetPer", obj.FeetPer);

                    if (id == 0)
                        sqlParameters[29] = new SqlParameter("@Flg", 1);
                    else
                        sqlParameters[29] = new SqlParameter("@Flg", 2);
                    sqlParameters[30] = new SqlParameter("@NB", obj.LotNB);
                    sqlParameters[31] = new SqlParameter("@SCH", obj.LotSCH);
                    DataTable DtState = ObjDBConnection.CallStoreProcedure("OBLMST_Insert", sqlParameters);
                    if (DtState != null && DtState.Rows.Count > 0)
                    {
                        if (id > 0)
                        {
                            SetSuccessMessage("Update Sucessfully");
                        }
                        else
                        {
                            SetSuccessMessage("Inserted Sucessfully");
                        }
                        return RedirectToAction("index", "OpeningStockPipe", new { id = 0 });
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        objOpeningStock.OblCmpVouList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                        objOpeningStock.LotTypVouList = objProductHelper.GetLotMasterDropdown(companyId, administrator);
                        objOpeningStock.OblPrdVouList = objProductHelper.GetProductMasterDropdown(companyId, administrator, "PIPE");
                        objOpeningStock.LotFinishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                        objOpeningStock.OblGdnVouList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        objOpeningStock.OblLocVouList = objProductHelper.GetLocationMasterDropdown(companyId, administrator);
                        objOpeningStock.OblAccVouList = objProductHelper.GetSupplierMasterDropdown(companyId, administrator);
                        objOpeningStock.LotGradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                        objOpeningStock.LotPrcTypVouList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    objOpeningStock.OblCmpVouList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                    objOpeningStock.LotTypVouList = objProductHelper.GetLotMasterDropdown(companyId, administrator);
                    objOpeningStock.OblPrdVouList = objProductHelper.GetProductMasterDropdown(companyId, administrator, "PIPE");
                    objOpeningStock.LotFinishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                    objOpeningStock.OblGdnVouList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                    objOpeningStock.OblLocVouList = objProductHelper.GetLocationMasterDropdown(companyId, administrator);
                    objOpeningStock.OblAccVouList = objProductHelper.GetSupplierMasterDropdown(companyId, administrator);
                    objOpeningStock.LotGradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                    objOpeningStock.LotPrcTypVouList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new OpeningStokPipeModel());
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
                    string currentURL = "/OpeningStockPipe/Index";
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
                    getReportDataModel.ControllerName = "OpeningStockPipe";
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
                    var bytes = Excel(getReportDataModel, "Opening Stock Pipe Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "OpeningStock.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Opening Stock Pipe Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "OpeningStock.pdf");
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
                //OpeningStokModel accountMasterModel = new OpeningStokModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@OblVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "2");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtAcc = ObjDBConnection.CallStoreProcedure("GetOBLMSTDetails", sqlParameters);
                    if (DtAcc != null && DtAcc.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtAcc.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Opening Stock Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "OpeningStockPipe");
        }

        public IActionResult GetLotPrecessList()
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                var lotPrcTypVouList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                return Json(new { result = true, data = lotPrcTypVouList });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public OpeningStokModel GetLastVoucherNo(string OblPrdType, int companyId)
        {
            OpeningStokModel obj = new OpeningStokModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(OblPrdType))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@OblPrdType", OblPrdType);
                    sqlParameters[1] = new SqlParameter("@Cmpvou", companyId);
                    DataTable dtNewOrdVNo = ObjDBConnection.CallStoreProcedure("GetLatestOblVNo", sqlParameters);
                    if (dtNewOrdVNo != null && dtNewOrdVNo.Rows.Count > 0)
                    {
                        int.TryParse(dtNewOrdVNo.Rows[0]["OblNVno"].ToString(), out int OblNVno);
                        OblNVno = OblNVno == 0 ? 1 : OblNVno;
                        obj.OblNVno = OblNVno;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return obj;
        }
        public JsonResult GetNBSCH(string thick, string od)
        {
            OpeningStokModel obj = new OpeningStokModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(thick) && !string.IsNullOrWhiteSpace(od))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@thickpipe", thick);
                    sqlParameters[1] = new SqlParameter("@odpipe", od);
                    DataTable dtNBSCH = ObjDBConnection.CallStoreProcedure("GetNBSCHDetails", sqlParameters);
                    if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
                    {
                        string LotNB = dtNBSCH.Rows[0]["OrdNB"].ToString();
                        string LotSCH = dtNBSCH.Rows[0]["OrdSCH"].ToString();
                        return Json(new { result = true, lotNB = LotNB, lotSCH = LotSCH });
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { result = false });
        }

        [Route("/OpeningStockPipe/GetCompany-List")]
        public IActionResult CompanyList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            var companyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                companyList = companyList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(companyList) });
        }


        [Route("/OpeningStockPipe/GetGodown-List")]
        public IActionResult GodownList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            var godownList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                godownList = godownList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(godownList) });
        }

        [Route("/OpeningStockPipe/GetLocation-List")]
        public IActionResult LocationList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            var locationList = objProductHelper.GetLocationMasterDropdown(companyId, administrator);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                locationList = locationList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(locationList) });
        }


        [Route("/OpeningStockPipe/GetLotType List")]
        public IActionResult LotTypeList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            var lottypeList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                lottypeList = lottypeList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(lottypeList) });
        }

        [Route("/OpeningStockPipe/GetProduct-List")]
        public IActionResult ProductList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            var productList = objProductHelper.GetProductMasterDropdown(companyId, administrator, "PIPE");

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                productList = productList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(productList) });
        }

        [Route("/OpeningStockPipe/GetGrade-List")]
        public IActionResult GradeList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            var gradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                gradeList = gradeList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(gradeList) });
        }

        [Route("/OpeningStockPipe/GetFinish-List")]
        public IActionResult FinishList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            var finishist = objProductHelper.GetFinishMasterDropdown(companyId, administrator);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                finishist = finishist.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(finishist) });
        }

    }
}
