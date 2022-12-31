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
    public class OpeningStockController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        OpeningStokModel objOpeningStock = new OpeningStokModel();
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
                sqlPara[3] = new SqlParameter("@OblPrdTyp", "COIL");
                DataTable DtOBL = ObjDBConnection.CallStoreProcedure("GetOBLMSTDetails", sqlPara);
                if (DtOBL.Rows.Count > 0)
                {
                    objOpeningStock.OblDt = !string.IsNullOrWhiteSpace(DtOBL.Rows[0]["OblDt"].ToString()) ? Convert.ToDateTime(DtOBL.Rows[0]["OblDt"].ToString()).ToString("yyyy-MM-dd") : "";
                    objOpeningStock.OblCmpVou = int.Parse(DtOBL.Rows[0]["OblCmpVou"].ToString());
                    objOpeningStock.OblRefNo = DtOBL.Rows[0]["OblRefNo"].ToString();
                    objOpeningStock.OblGdnVou = int.Parse(DtOBL.Rows[0]["OblGdnVou"].ToString());
                    objOpeningStock.LotHeatNo = DtOBL.Rows[0]["LotHeatNo"].ToString();
                    objOpeningStock.LotFinish = int.Parse(DtOBL.Rows[0]["OblFinMscVou"].ToString());
                    objOpeningStock.Finish = DtOBL.Rows[0]["LotFinish"].ToString();
                    objOpeningStock.OblAccVou = int.Parse(DtOBL.Rows[0]["OblAccVou"].ToString());
                    objOpeningStock.LotGrade = int.Parse(DtOBL.Rows[0]["OblGrdMscVou"].ToString());
                    objOpeningStock.Grade = DtOBL.Rows[0]["LotGrade"].ToString();
                    objOpeningStock.OblPrdVou = int.Parse(DtOBL.Rows[0]["OblPrdVou"].ToString());
                    //objOpeningStock.CoilTypeVou = int.Parse(DtOBL.Rows[0]["LotCoilTypeVou"].ToString());
                    //objOpeningStock.CoilType = DtOBL.Rows[0]["LotCoilType"].ToString();
                }


                if (id > 0)
                {
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@OblVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 1);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    sqlParameters[3] = new SqlParameter("@OblPrdTyp", "COIL");
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
                        objOpeningStock.Grade = DtOBLDetail.Rows[0]["Grade"].ToString();
                        objOpeningStock.LotGrade = int.Parse(DtOBLDetail.Rows[0]["LotGrade"].ToString());
                        objOpeningStock.LotFinish = int.Parse(DtOBLDetail.Rows[0]["LotFinMscVou"].ToString());
                        objOpeningStock.OblPrdVou = int.Parse(DtOBLDetail.Rows[0]["OblPrdVou"].ToString());
                        objOpeningStock.LotSupCoilNo = DtOBLDetail.Rows[0]["LotSupCoilNo"].ToString();
                        objOpeningStock.LotQty = decimal.Parse(DtOBLDetail.Rows[0]["LotQty"].ToString());
                        objOpeningStock.LotWidth = decimal.Parse(DtOBLDetail.Rows[0]["LotWidth"].ToString());
                        objOpeningStock.LotThick = decimal.Parse(DtOBLDetail.Rows[0]["LotThick"].ToString());
                        objOpeningStock.LotInwQty = decimal.Parse(DtOBLDetail.Rows[0]["LotInwQty"].ToString());
                        objOpeningStock.LotInwWidth = decimal.Parse(DtOBLDetail.Rows[0]["LotInwWidth"].ToString());
                        objOpeningStock.LotInwThick = decimal.Parse(DtOBLDetail.Rows[0]["LotInwThick"].ToString());
                        objOpeningStock.LotHeatNo = DtOBLDetail.Rows[0]["LotHeatNo"].ToString();
                        objOpeningStock.LotType = DtOBLDetail.Rows[0]["LotTyp"].ToString();
                        objOpeningStock.CoilNo = int.Parse(DtOBLDetail.Rows[0]["LotNo"].ToString());
                        objOpeningStock.OblRem = DtOBLDetail.Rows[0]["OblRem"].ToString();
                        objOpeningStock.CoilType = DtOBLDetail.Rows[0]["LotCoilType"].ToString();
                        objOpeningStock.LotSuffix = DtOBLDetail.Rows[0]["OblSuffix"].ToString();
                        objOpeningStock.OblRefNo = DtOBLDetail.Rows[0]["OblRefNo"].ToString();
                        objOpeningStock.CoilTypeVou = int.Parse(DtOBLDetail.Rows[0]["LotCoilTypeVou"].ToString());
                        objOpeningStock.LotOD = decimal.Round(decimal.Parse(DtOBLDetail.Rows[0]["LotWidth"].ToString()) / Convert.ToDecimal("3.14"),2, MidpointRounding.AwayFromZero);
                    }
                }
                objOpeningStock.OblCmpVouList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                objOpeningStock.OblGdnVouList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                objOpeningStock.OblLocVouList = objProductHelper.GetLocationMasterDropdown(companyId, administrator);
                objOpeningStock.OblAccVouList = objProductHelper.GetSupplierMasterDropdown(companyId, administrator);
                objOpeningStock.LotGradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                objOpeningStock.LotFinishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                objOpeningStock.OblPrdVouList = objProductHelper.GetProductMasterDropdown(companyId, administrator,"COIL");
                objOpeningStock.LotTypVouList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                objOpeningStock.CoilTypeList = objProductHelper.GetMainCoilType();
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
        public IActionResult Index(long id, OpeningStokModel openingStokModel)
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

                SqlParameter[] sqlParameters = new SqlParameter[37];
                sqlParameters[0] = new SqlParameter("@OblNVno", openingStokModel.OblNVno);
                sqlParameters[1] = new SqlParameter("@OblDt", DateTime.Parse(openingStokModel.OblDt));
                sqlParameters[2] = new SqlParameter("@OblCmpVou", openingStokModel.OblCmpVou);
                sqlParameters[3] = new SqlParameter("@OblGdnVou", openingStokModel.OblGdnVou);
                sqlParameters[4] = new SqlParameter("@OblLocVou", openingStokModel.OblLocVou);
                sqlParameters[5] = new SqlParameter("@OblAccVou", openingStokModel.OblAccVou);
                sqlParameters[6] = new SqlParameter("@OblPrdVou", openingStokModel.OblPrdVou);
                sqlParameters[7] = new SqlParameter("@OblRem", openingStokModel.OblRem);
                sqlParameters[8] = new SqlParameter("@LotSupCoilNo", openingStokModel.LotSupCoilNo);
                sqlParameters[9] = new SqlParameter("@LotHeatNo", openingStokModel.LotHeatNo);
                sqlParameters[10] = new SqlParameter("@LotGrdMscVou", openingStokModel.LotGrade);
                sqlParameters[11] = new SqlParameter("@OblGrade", openingStokModel.Grade);
                sqlParameters[12] = new SqlParameter("@LotFinMscVou", openingStokModel.LotFinish);
                sqlParameters[13] = new SqlParameter("@OblFinish", openingStokModel.Finish);
                sqlParameters[14] = new SqlParameter("@LotWidth", openingStokModel.LotWidth);
                sqlParameters[15] = new SqlParameter("@LotThick", openingStokModel.LotThick);
                sqlParameters[16] = new SqlParameter("@LotQty", openingStokModel.LotQty);
                sqlParameters[17] = new SqlParameter("@LotInwWidth", openingStokModel.LotInwWidth);
                sqlParameters[18] = new SqlParameter("@LotInwThick", openingStokModel.LotInwThick);
                sqlParameters[19] = new SqlParameter("@LotInwQty", openingStokModel.LotInwQty);
                sqlParameters[20] = new SqlParameter("@PrdTyp", "COIL");
                sqlParameters[21] = new SqlParameter("@CmpVou", companyId);
                sqlParameters[22] = new SqlParameter("@OblVou", id);


                sqlParameters[23] = new SqlParameter("@LotTyp", openingStokModel.LotType);
                if (openingStokModel.LotType == "Select")
                {
                    if (openingStokModel.LotSuffix != "" && openingStokModel.LotSuffix != null)
                    {
                        string _strLotCoilNo = string.Format("{0}-{1}", openingStokModel.CoilNo, openingStokModel.LotSuffix);
                        sqlParameters[24] = new SqlParameter("@CoilNo", _strLotCoilNo);
                    }
                    else
                    {
                        string _strLotCoilNo = string.Format("{0}", openingStokModel.CoilNo);
                        sqlParameters[24] = new SqlParameter("@CoilNo", _strLotCoilNo);
                    }
                }
                else
                {
                    if (openingStokModel.LotSuffix != "" && openingStokModel.LotSuffix != null)
                    {
                        string _strLotCoilNo = string.Format("{0}-{1}-{2}", openingStokModel.LotType, openingStokModel.CoilNo, openingStokModel.LotSuffix);
                        sqlParameters[24] = new SqlParameter("@CoilNo", _strLotCoilNo);
                    }
                    else
                    {
                        string _strLotCoilNo = string.Format("{0}-{1}", openingStokModel.LotType, openingStokModel.CoilNo);
                        sqlParameters[24] = new SqlParameter("@CoilNo", _strLotCoilNo);
                    }
                }
                sqlParameters[25] = new SqlParameter("@LotPrcTypCD", "0");
                sqlParameters[26] = new SqlParameter("@LotOD", openingStokModel.LotOD);
                sqlParameters[27] = new SqlParameter("@LotPCS", "0");
                sqlParameters[28] = new SqlParameter("@LotFeetPer", "0");
                if (id == 0)
                    sqlParameters[29] = new SqlParameter("@Flg", 1);
                else
                    sqlParameters[29] = new SqlParameter("@Flg", 2);
                sqlParameters[30] = new SqlParameter("@LotNo", openingStokModel.CoilNo);
                sqlParameters[31] = new SqlParameter("@RefNo", openingStokModel.OblRefNo);
                sqlParameters[32] = new SqlParameter("@NB", "");
                sqlParameters[33] = new SqlParameter("@SCH", "");
                sqlParameters[34] = new SqlParameter("@CoilType", openingStokModel.CoilType);
                sqlParameters[35] = new SqlParameter("@CoilTypeVou", openingStokModel.CoilTypeVou);
                sqlParameters[36] = new SqlParameter("@CoilSuffix", openingStokModel.LotSuffix);
                DataTable DtState = ObjDBConnection.CallStoreProcedure("OBLMST_Insert", sqlParameters);
                if (DtState != null && DtState.Rows.Count > 0)
                {
                    int status = DbConnection.ParseInt32(DtState.Rows[0][0].ToString());
                    if (status == -1)
                    {
                        SetErrorMessage("Dulplicate Coil No");
                        ViewBag.FocusType = "-1";
                        openingStokModel.OblCmpVouList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                        openingStokModel.OblGdnVouList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        openingStokModel.OblLocVouList = objProductHelper.GetLocationMasterDropdown(companyId, administrator);
                        openingStokModel.OblAccVouList = objProductHelper.GetSupplierMasterDropdown(companyId, administrator);
                        openingStokModel.LotGradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                        openingStokModel.LotFinishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                        openingStokModel.OblPrdVouList = objProductHelper.GetProductMasterDropdown(companyId, administrator, "COIL");
                        openingStokModel.LotTypVouList = objProductHelper.GetLotMasterDropdown(companyId, administrator);
                        openingStokModel.CoilTypeList = objProductHelper.GetMainCoilType();

                        return View(openingStokModel);

                    }
                    else
                    {
                        if (status.Equals(0))
                        {
                            SetSuccessMessage("Inserted Sucessfully");
                        }
                        else
                        {
                            SetSuccessMessage("Updated Sucessfully");
                        }
                        return RedirectToAction("index", "OpeningStock", new { id = 0 });
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                    openingStokModel.OblCmpVouList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                    openingStokModel.OblGdnVouList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                    openingStokModel.OblLocVouList = objProductHelper.GetLocationMasterDropdown(companyId, administrator);
                    openingStokModel.OblAccVouList = objProductHelper.GetSupplierMasterDropdown(companyId, administrator);
                    openingStokModel.LotGradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                    openingStokModel.LotFinishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                    openingStokModel.OblPrdVouList = objProductHelper.GetProductMasterDropdown(companyId, administrator, "COIL");
                    openingStokModel.LotTypVouList = objProductHelper.GetLotMasterDropdown(companyId, administrator);
                    openingStokModel.CoilTypeList = objProductHelper.GetMainCoilType();
                    //userRight.IsDelete=false
                    return View(openingStokModel);
                }

            }
            catch (Exception ex)
            {
                throw;
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
                    string currentURL = "/OpeningStock/Index";
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
                    getReportDataModel.ControllerName = "OpeningStock";
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
                    var bytes = Excel(getReportDataModel, "Opening Stock Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "OpeningStock.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Opening Stock Report", companyDetails.CmpName);
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
                OpeningStokModel openingStokModel = new OpeningStokModel();
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
            return RedirectToAction("index", "OpeningStock");
        }

        public JsonResult GetOD(string width)
        {
            OpeningStokModel obj = new OpeningStokModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(width))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("@width", width);
                    DataTable dtNBSCH = ObjDBConnection.CallStoreProcedure("GetODDetails", sqlParameters);
                    if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
                    {
                        string LotOD = dtNBSCH.Rows[0]["od"].ToString();
                        return Json(new { result = true, lotOD = LotOD });
                    }
                    else
                    {
                        decimal lotwidth = Convert.ToDecimal(width)/Convert.ToDecimal("3.14");
                        return Json(new { result = true, lotOD = (lotwidth.ToString("0.##")) });
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { result = false });
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

        [Route("/OpeningStock/GetCompany-List")]
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


        [Route("/OpeningStock/GetGodown-List")]
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

        [Route("/OpeningStock/GetLocation-List")]
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

        [Route("/OpeningStock/GetLotType List")]
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


        [Route("/OpeningStock/GetSupplier-List")]
        public IActionResult SuppierList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            var supplierList = objProductHelper.GetSupplierMasterDropdown(companyId, administrator);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                supplierList = supplierList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(supplierList) });
        }

        [Route("/OpeningStock/GetProduct-List")]
        public IActionResult ProductList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            var productList = objProductHelper.GetProductMasterDropdown(companyId, administrator, "COIL");

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                productList = productList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(productList) });
        }

        [Route("/OpeningStock/GetGrade-List")]
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

        [Route("/OpeningStock/GetFinish-List")]
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
