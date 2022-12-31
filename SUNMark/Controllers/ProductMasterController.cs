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
    public class ProductMasterController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        AccountMasterHelpers ObjaccountMasterHelpers = new AccountMasterHelpers();

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
                ProductMasterModel productMasterModel = new ProductMasterModel();
                SqlParameter[] sqlPara = new SqlParameter[3];
                sqlPara[0] = new SqlParameter("@PrdVou", id);
                sqlPara[1] = new SqlParameter("@Flg", 3);
                sqlPara[2] = new SqlParameter("@CmpVou", companyId);
                DataTable DtPrdMst = ObjDBConnection.CallStoreProcedure("GetProductDetails", sqlPara);
                if (DtPrdMst != null && DtPrdMst.Rows.Count > 0)
                {
                    productMasterModel.PrdTypVou = Convert.ToInt32(DtPrdMst.Rows[0]["PrdPtyMscVou"].ToString());
                    productMasterModel.PrdType = DtPrdMst.Rows[0]["PrdPtyMscVou"].ToString();
                    productMasterModel.PrdMscUntVou = Convert.ToInt32(DtPrdMst.Rows[0]["PrdUntMscVou"].ToString());
                    productMasterModel.PrdUnit = DtPrdMst.Rows[0]["PrdUntMscVou"].ToString();
                }
                if (id > 0)
                {
                    productMasterModel.PrdVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@PrdVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtPrd = ObjDBConnection.CallStoreProcedure("GetProductDetails", sqlParameters);
                    if (DtPrd != null && DtPrd.Rows.Count > 0)
                    {
                        productMasterModel.PrdTypVou = Convert.ToInt32(DtPrd.Rows[0]["PrdPtyMscVou"].ToString());
                        productMasterModel.PrdCd = DtPrd.Rows[0]["PrdCd"].ToString();
                        productMasterModel.PrdNm = DtPrd.Rows[0]["PrdNM"].ToString();
                        productMasterModel.PrdMscUntVou = Convert.ToInt32(DtPrd.Rows[0]["PrdUntMscVou"].ToString());
                        productMasterModel.PrdDesc = DtPrd.Rows[0]["PrdDesc"].ToString();
                    }
                }
                productMasterModel.UnitList = ObjaccountMasterHelpers.GetUnitDropdown(companyId);
                productMasterModel.TypeList = ObjaccountMasterHelpers.GetPrdTypeDropdown(companyId);
                return View(productMasterModel);

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
        public IActionResult Index(long id, ProductMasterModel productMasterModel)
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
                if (!string.IsNullOrWhiteSpace(productMasterModel.PrdCd) && !string.IsNullOrWhiteSpace(productMasterModel.PrdNm) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(productMasterModel.PrdTypVou).ToString()))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[10];
                    sqlParameters[0] = new SqlParameter("@PrdType", productMasterModel.PrdType);
                    sqlParameters[1] = new SqlParameter("@PrdPtyMscVou", productMasterModel.PrdTypVou);
                    sqlParameters[2] = new SqlParameter("@PrdName", productMasterModel.PrdNm);
                    sqlParameters[3] = new SqlParameter("@PrdCode", productMasterModel.PrdCd);
                    sqlParameters[4] = new SqlParameter("@PrdUntMscVou", productMasterModel.PrdMscUntVou);
                    sqlParameters[5] = new SqlParameter("@PrdUntNm", productMasterModel.PrdUnit);
                    sqlParameters[6] = new SqlParameter("@PrdDesc", productMasterModel.PrdDesc);
                    sqlParameters[7] = new SqlParameter("@PrdVou", id);
                    sqlParameters[8] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[9] = new SqlParameter("@FLG", "1");

                    DataTable DtProduct = ObjDBConnection.CallStoreProcedure("ProductMst_Insert", sqlParameters);
                    if (DtProduct != null && DtProduct.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtProduct.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Code Details");
                            ViewBag.FocusType = "-1";
                            productMasterModel.UnitList = ObjaccountMasterHelpers.GetUnitDropdown(companyId);
                            productMasterModel.TypeList = ObjaccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            return View(productMasterModel);
                        }
                        else if (status == -2)
                        {
                            SetErrorMessage("Dulplicate Name Details");
                            ViewBag.FocusType = "-2";
                            productMasterModel.UnitList = ObjaccountMasterHelpers.GetUnitDropdown(companyId);
                            productMasterModel.TypeList = ObjaccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            return View(productMasterModel);
                        }

                        else
                        {
                            if (id > 0)
                            {
                                SetSuccessMessage("Update Sucessfully");
                            }
                            else
                            {
                                SetSuccessMessage("Inserted Sucessfully");
                            }
                            return RedirectToAction("index", "ProductMaster", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        productMasterModel.UnitList = ObjaccountMasterHelpers.GetUnitDropdown(companyId);
                        productMasterModel.TypeList = ObjaccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        ViewBag.FocusType = "-1";
                        return View(productMasterModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    productMasterModel.UnitList = ObjaccountMasterHelpers.GetUnitDropdown(companyId);
                    productMasterModel.TypeList = ObjaccountMasterHelpers.GetPrdTypeDropdown(companyId);
                    ViewBag.FocusType = "-1";
                    return View(productMasterModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new ProductMasterModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                ProductMasterModel productMasterModel = new ProductMasterModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@PrdVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "1");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtProduct = ObjDBConnection.CallStoreProcedure("GetProductDetails", sqlParameters);
                    if (DtProduct != null && DtProduct.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtProduct.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Product Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "ProductMaster");
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
                    string currentURL = "/ProductMaster/Index";
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
                    getReportDataModel.ControllerName = "ProductMaster";
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
                    var bytes = Excel(getReportDataModel, "Product Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Product.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Product Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Product.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public IActionResult GetProductTypeList()
        //{
        //    try
        //    {
        //        int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
        //        var typeList = ObjaccountMasterHelpers.GetPrdTypeDropdown(companyId);
        //        return Json(new { result = true, data = typeList });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public IActionResult GetUnitList()
        //{
        //    try
        //    {
        //        int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
        //        var unitList = ObjaccountMasterHelpers.GetUnitDropdown(companyId);
        //        return Json(new { result = true, data = unitList });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        [Route("/ProductMaster/GetUnit-List")]
        public IActionResult UnitList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var unitList = ObjaccountMasterHelpers.GetUnitDropdown(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                unitList = unitList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(unitList) });
        }

        [Route("/ProductMaster/GetProductType-List")]
        public IActionResult ProductTypeList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var productTypeList = ObjaccountMasterHelpers.GetPrdTypeDropdown(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                productTypeList = productTypeList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(productTypeList) });
        }


    }
}
