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
    public class OpeningStockOtherController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        public IActionResult Index(long id)
        {

            bool isreturn = false;
            INIT(ref isreturn);
            if (isreturn)
                return RedirectToAction("index", "dashboard");

            long userId = GetIntSession("UserId");
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;

            if (id > 0)
            {
                SqlParameter[] sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("@OblVou", id);
                sqlParameters[1] = new SqlParameter("@Flg", 1);
                sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                sqlParameters[3] = new SqlParameter("@OblPrdTyp", "OOBL");
                DataTable DtOBLDetail = ObjDBConnection.CallStoreProcedure("GetOBLMSTDetails", sqlParameters);

                if (DtOBLDetail.Rows.Count > 0)
                {
                    OpeningStokOtherModel objOpeningStock = new OpeningStokOtherModel();
                    objOpeningStock.OblVou = int.Parse(DtOBLDetail.Rows[0]["OblVou"].ToString());

                    objOpeningStock.OblNVno = int.Parse(DtOBLDetail.Rows[0]["OblNVno"].ToString());
                    objOpeningStock.OblDt = !string.IsNullOrWhiteSpace(DtOBLDetail.Rows[0]["OblDt"].ToString()) ? Convert.ToDateTime(DtOBLDetail.Rows[0]["OblDt"].ToString()).ToString("yyyy-MM-dd") : "";

                    objOpeningStock.OblCmpVou = int.Parse(DtOBLDetail.Rows[0]["OblCmpVou"].ToString());
                    objOpeningStock.OblCmpVouList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                    objOpeningStock.OblGdnVou = int.Parse(DtOBLDetail.Rows[0]["OblGdnVou"].ToString());
                    objOpeningStock.OblGdnVouList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

                    objOpeningStock.OblLocVou = int.Parse(DtOBLDetail.Rows[0]["OblLocVou"].ToString());
                    objOpeningStock.OblLocVouList = objProductHelper.GetLocationMasterDropdown(companyId, administrator);

                    objOpeningStock.OblAccVou = int.Parse(DtOBLDetail.Rows[0]["OblAccVou"].ToString());
                    objOpeningStock.OblAccVouList = objProductHelper.GetSupplierMasterDropdown(companyId, administrator);

                    objOpeningStock.LotGrade = int.Parse(DtOBLDetail.Rows[0]["LotGrade"].ToString());
                    objOpeningStock.LotGradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);

                    objOpeningStock.LotFinish = int.Parse(DtOBLDetail.Rows[0]["LotFinMscVou"].ToString());
                    objOpeningStock.LotFinishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);

                    objOpeningStock.OblPrdVou = int.Parse(DtOBLDetail.Rows[0]["OblPrdVou"].ToString());
                    objOpeningStock.OblPrdVouList = objProductHelper.GetProductMasterDropdown(companyId, administrator,"");

                    objOpeningStock.LotSupCoilNo = DtOBLDetail.Rows[0]["LotSupCoilNo"].ToString();

                    objOpeningStock.LotQty = decimal.Parse(DtOBLDetail.Rows[0]["LotQty"].ToString());
                    objOpeningStock.LotWidth = decimal.Parse(DtOBLDetail.Rows[0]["LotWidth"].ToString());
                    objOpeningStock.LotThick = decimal.Parse(DtOBLDetail.Rows[0]["LotThick"].ToString());
                    objOpeningStock.LotInwQty = decimal.Parse(DtOBLDetail.Rows[0]["LotInwQty"].ToString());
                    objOpeningStock.LotInwWidth = decimal.Parse(DtOBLDetail.Rows[0]["LotInwWidth"].ToString());
                    objOpeningStock.LotInwThick = decimal.Parse(DtOBLDetail.Rows[0]["LotInwThick"].ToString());
                    objOpeningStock.LotHeatNo = DtOBLDetail.Rows[0]["LotHeatNo"].ToString();

                    objOpeningStock.LotTypVou = int.Parse(DtOBLDetail.Rows[0]["LotTypVou"].ToString());
                    objOpeningStock.LotTypVouList = objProductHelper.GetLotMasterDropdown(companyId, administrator);
                    objOpeningStock.CoilNo = int.Parse(DtOBLDetail.Rows[0]["LotCoilNo"].ToString().Split('_')[1].ToString());


                    objOpeningStock.LotPrcTypVou = 0;
                    objOpeningStock.LotPrcTypVouList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");

                    objOpeningStock.OblRem = DtOBLDetail.Rows[0]["OblRem"].ToString();
                    return View(objOpeningStock);
                }
                else
                {
                    return View();
                }

            }
            else
            {
                SqlParameter[] sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("@OblVou", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 3);
                sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                sqlParameters[3] = new SqlParameter("@OblPrdTyp", "OOBL");
                DataTable DtOBLDetail = ObjDBConnection.CallStoreProcedure("GetOBLMSTDetails", sqlParameters);

                if (DtOBLDetail.Rows.Count > 0)
                {

                }

                OpeningStokOtherModel objOpeningStock = new OpeningStokOtherModel();
                objOpeningStock.OblVou = 0;
                objOpeningStock.OblNVno = int.Parse(DtOBLDetail.Rows[0]["MaxOblVou"].ToString());
                objOpeningStock.OblDt = string.Empty;

                objOpeningStock.OblCmpVou = 0;
                objOpeningStock.OblCmpVouList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                objOpeningStock.OblGdnVou = 0;
                objOpeningStock.OblGdnVouList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

                objOpeningStock.OblLocVou = 0;
                objOpeningStock.OblLocVouList = objProductHelper.GetLocationMasterDropdown(companyId, administrator);

                objOpeningStock.OblAccVou = 0;
                objOpeningStock.OblAccVouList = objProductHelper.GetSupplierMasterDropdown(companyId, administrator);

                objOpeningStock.LotGrade = 0;
                objOpeningStock.LotGradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);

                objOpeningStock.LotFinish = 0;
                objOpeningStock.LotFinishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);

                objOpeningStock.OblPrdVou = 0;
                objOpeningStock.OblPrdVouList = objProductHelper.GetProductMasterDropdown(companyId, administrator, "");

                objOpeningStock.LotPrcTypVou = 0;
                objOpeningStock.LotPrcTypVouList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator,"");


                objOpeningStock.LotTypVou = 0;
                objOpeningStock.CoilNo = 0;
                objOpeningStock.LotTypVouList = objProductHelper.GetLotMasterDropdown(companyId, administrator);

                return View(objOpeningStock);
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
        }

        [HttpPost]
        public ActionResult Save(OpeningStokModel obj, long id)
        {
            try
            {
                var ID = ViewBag.ID;

                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;

                SqlParameter[] sqlParameters = new SqlParameter[24];
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
                sqlParameters[11] = new SqlParameter("@LotFinMscVou", obj.LotFinish);
                sqlParameters[12] = new SqlParameter("@LotWidth", obj.LotWidth);
                sqlParameters[13] = new SqlParameter("@LotThick", obj.LotThick);
                sqlParameters[14] = new SqlParameter("@LotQty", obj.LotQty);
                sqlParameters[15] = new SqlParameter("@LotInwWidth", obj.LotInwWidth);
                sqlParameters[16] = new SqlParameter("@LotInwThick", obj.LotInwThick);
                sqlParameters[17] = new SqlParameter("@LotInwQty", obj.LotInwQty);
                sqlParameters[18] = new SqlParameter("@PrdTyp", "OOBL");
                sqlParameters[19] = new SqlParameter("@CmpVou", companyId);
                sqlParameters[20] = new SqlParameter("@OblVou", obj.OblVou);
                sqlParameters[21] = new SqlParameter("@LotTypVou", obj.LotTypVou);
                sqlParameters[22] = new SqlParameter("@CoilNo", obj.CoilNo);

                if (obj.OblVou.Equals(0))
                    sqlParameters[23] = new SqlParameter("@Flg", 1);
                else
                    sqlParameters[23] = new SqlParameter("@Flg", 2);

                DataTable DtState = ObjDBConnection.CallStoreProcedure("OBLMST_Insert", sqlParameters);
                if (DtState != null && DtState.Rows.Count > 0)
                {
                    //int status = DbConnection.ParseInt32(DtState.Rows[0][0].ToString());
                    //if (status == -1)
                    //{
                    //    return Json(new { result = false, message = "Duplicate Code" });
                    //}
                    //else if (status == -2)
                    //{
                    //    return Json(new { result = false, message = "Duplicate Name" });
                    //}
                    //else
                    //{
                    //var stateList = objProductHelper.GetStateMasterDropdown(companyId, administrator);
                    //return Json(new { result = true, message = "Inserted successfully" });

                    if (id > 0)
                    {
                        SetSuccessMessage("Update Sucessfully");
                    }
                    else
                    {
                        SetSuccessMessage("Inserted Sucessfully");
                    }
                    return RedirectToAction("index", "OpeningStockOther", new { id = 0 });
                    //}
                }
                else
                {
                    return Json(new { result = false, message = "Please Enter the Value" });
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
                    string currentURL = "/OpeningStockOther/Index";
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
                    getReportDataModel.ControllerName = "OpeningStockOther";
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
                    var bytes = Excel(getReportDataModel, "Opening Stock Other Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "OpeningStockOther.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Opening Stock Other Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "OpeningStockOther.pdf");
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
                            SetSuccessMessage("Opening Stock Other Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "OpeningStockOther");
        }

        [Route("/OpeningStockOther/GetCompany-List")]
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


        [Route("/OpeningStockOther/GetGodown-List")]
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

        [Route("/OpeningStockOther/GetLocation-List")]
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

        [Route("/OpeningStockOther/GetProduct-List")]
        public IActionResult ProductList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            var productList = objProductHelper.GetProductMasterDropdown(companyId, administrator, "");

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                productList = productList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(productList) });
        }


    }
}
