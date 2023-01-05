using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SUNMark.Classes;
using SUNMark.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Controllers
{
    public class InTransInwardController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        private readonly IWebHostEnvironment hostingEnvironment;

        public InTransInwardController(IWebHostEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
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
                int administrator = 0;
                InTransInwardModel inwardModel = new InTransInwardModel();
                inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                inwardModel.AccountList = new List<CustomDropDown>();
                inwardModel.AccountList.Add(CommonHelpers.GetDefaultValue());
                inwardModel.AccountList.AddRange(ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0));
                inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "COIL");
                inwardModel.HdGodwonList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

                inwardModel.Inward = new InTransInwardGridModel();
                inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                inwardModel.Inward.CoilTypeList = objProductHelper.GetMainCoilType();
                inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);

                SqlParameter[] sqlParam = new SqlParameter[3];
                sqlParam[0] = new SqlParameter("@InwVou", id);
                sqlParam[1] = new SqlParameter("@Flg", 3);
                sqlParam[2] = new SqlParameter("@CmpVou", companyId);
                DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetInTransInwardDetails", sqlParam);
                if (DtInw != null && DtInw.Rows.Count > 0)
                {
                    inwardModel.InwCmpVou = Convert.ToInt32(DtInw.Rows[0]["InwCmpVou"].ToString());
                    //inwardModel.InwPtyVou = Convert.ToInt32(DtInw.Rows[0]["InwPrdTyp"].ToString());
                    //inwardModel.InwPrdTyp = DtInw.Rows[0]["PrdType"].ToString();
                    inwardModel.InwDt = !string.IsNullOrWhiteSpace(DtInw.Rows[0]["InwDt"].ToString()) ? Convert.ToDateTime(DtInw.Rows[0]["InwDt"].ToString()).ToString("yyyy-MM-dd") : "";
                    //inwardModel.InwETADt = !string.IsNullOrWhiteSpace(DtInw.Rows[0]["InwETADt"].ToString()) ? Convert.ToDateTime(DtInw.Rows[0]["InwETADt"].ToString()).ToString("yyyy-MM-dd") : "";
                    //inwardModel.InwAccVou = Convert.ToInt32(DtInw.Rows[0]["InwAccVou"].ToString());
                    int vno = DbConnection.ParseInt32(DtInw.Rows[0][0].ToString());
                    inwardModel.InwVNo = vno + 1;
                    //inwardModel.Inward.SupCoilNoStr = DtInw.Rows[0]["IntSupCoilNo"].ToString();
                    //inwardModel.Inward.HeatNoStr = DtInw.Rows[0]["IntHeatNo"].ToString();
                }

                if (id > 0)
                {
                    inwardModel.InwVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@InwVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtInwMst = ObjDBConnection.CallStoreProcedure("GetInTransInwardDetails", sqlParameters);
                    if (DtInwMst != null && DtInwMst.Rows.Count > 0)
                    {
                        inwardModel.InwVNo = Convert.ToInt32(DtInwMst.Rows[0]["InwVNo"].ToString());
                        inwardModel.InwDt = !string.IsNullOrWhiteSpace(DtInwMst.Rows[0]["InwDt"].ToString()) ? Convert.ToDateTime(DtInwMst.Rows[0]["InwDt"].ToString()).ToString("yyyy-MM-dd") : "";
                        inwardModel.InwAccVou = Convert.ToInt32(DtInwMst.Rows[0]["InwAccVou"].ToString());
                        inwardModel.InwRefNo = DtInwMst.Rows[0]["InwRefNo"].ToString();
                        inwardModel.InwPtyVou = Convert.ToInt32(DtInwMst.Rows[0]["InwPrdTyp"].ToString());
                        inwardModel.InwPrdTyp = DtInwMst.Rows[0]["PrdType"].ToString();
                        inwardModel.InwRem = DtInwMst.Rows[0]["InwRem"].ToString();
                        inwardModel.InwCmpVou = Convert.ToInt32(DtInwMst.Rows[0]["InwCmpVou"].ToString());
                        inwardModel.InwFrightRt = Convert.ToDecimal(DtInwMst.Rows[0]["InwFrtRt"].ToString());
                        inwardModel.InwPrdVou = Convert.ToInt32(DtInwMst.Rows[0]["InwPrdVou"].ToString());

                        inwardModel.Inward.BillNoCoil = new string[DtInwMst.Rows.Count];
                        inwardModel.Inward.SupCoilNo = new string[DtInwMst.Rows.Count];
                        inwardModel.Inward.HeatNo = new string[DtInwMst.Rows.Count];
                        inwardModel.Inward.IntGrdCoil = new string[DtInwMst.Rows.Count];
                        inwardModel.Inward.IntGdnCoil = new string[DtInwMst.Rows.Count];
                        inwardModel.Inward.CoilNo = new string[DtInwMst.Rows.Count];
                        inwardModel.Inward.IntThickCoil = new decimal[DtInwMst.Rows.Count];
                        inwardModel.Inward.IntWidth = new decimal[DtInwMst.Rows.Count];
                        inwardModel.Inward.IntQtyCoil = new decimal[DtInwMst.Rows.Count];
                        inwardModel.Inward.IntRemksCoil = new string[DtInwMst.Rows.Count];
                        inwardModel.Inward.IntCoilType = new string[DtInwMst.Rows.Count];
                        inwardModel.Inward.IntSCNo = new string[DtInwMst.Rows.Count];
                        inwardModel.Inward.IntPOWidth = new decimal[DtInwMst.Rows.Count];
                        inwardModel.Inward.IntPOThick = new decimal[DtInwMst.Rows.Count];
                        inwardModel.InwETADt = !string.IsNullOrWhiteSpace(DtInw.Rows[0]["InwETADt"].ToString()) ? Convert.ToDateTime(DtInw.Rows[0]["InwETADt"].ToString()).ToString("yyyy-MM-dd") : "";

                        for (int i = 0; i < DtInwMst.Rows.Count; i++)
                        {
                            inwardModel.Inward.BillNoCoil[i] = DtInwMst.Rows[i]["IntBillNo"].ToString();
                            inwardModel.Inward.SupCoilNo[i] = DtInwMst.Rows[i]["IntSupCoilNo"].ToString();
                            inwardModel.Inward.HeatNo[i] = DtInwMst.Rows[i]["IntHeatNo"].ToString();
                            inwardModel.Inward.IntGrdCoil[i] = DtInwMst.Rows[i]["IntGrade"].ToString();
                            inwardModel.Inward.IntGdnCoil[i] = DtInwMst.Rows[i]["IntGdnVou"].ToString();
                            inwardModel.Inward.CoilNo[i] = DtInwMst.Rows[i]["CoilNo"].ToString();
                            inwardModel.Inward.IntThickCoil[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntThick"].ToString());
                            inwardModel.Inward.IntWidth[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntWidth"].ToString());
                            inwardModel.Inward.IntQtyCoil[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntQty"].ToString());
                            if (DtInwMst.Rows[i]["IntRem"].ToString() == "-1")
                            {
                                inwardModel.Inward.IntRemksCoil[i] = "";
                            }
                            else
                            {
                                inwardModel.Inward.IntRemksCoil[i] = DtInwMst.Rows[i]["IntRem"].ToString();
                            }
                            inwardModel.Inward.IntSCNo[i] = DtInwMst.Rows[i]["IntScNo"].ToString();
                            inwardModel.Inward.IntPOWidth[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntPOWidth"].ToString());
                            inwardModel.Inward.IntPOThick[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntPOThick"].ToString());
                            //inwardModel.Inward.IntCoilType[i] = DtInwMst.Rows[i]["IntCoilTypeVou"].ToString();
                        }

                    }
                }
                return View(inwardModel);
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
        public IActionResult Index(long id, InTransInwardModel inwardModel)
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
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                long userId = GetIntSession("UserId");
                int administrator = 0;

                inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                inwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                inwardModel.MainProductList = objProductHelper.GetProductMasterDropdown(companyId);
                inwardModel.HdGodwonList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                if (inwardModel.Inward == null)
                    inwardModel.Inward = new InTransInwardGridModel();

                inwardModel.Inward.CoilTypeList = objProductHelper.GetMainCoilType();
                inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);

                if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwVNo).ToString()) && !string.IsNullOrWhiteSpace(inwardModel.InwDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwAccVou).ToString()) && inwardModel.Inward.IntThickCoil.Length > 0 && inwardModel.Inward.IntWidth.Length > 0 && inwardModel.Inward.IntQtyCoil.Length > 0)
                {
                    int count = inwardModel.Inward.SupCoilNo.ToList().Distinct().Count();
                    if (id == 0)
                    {
                        int instexl = inwardModel.InstExl;                       
                        //int qty1 = inwardModel.Inward.IntQtyCoil.Length;
                        //int supcoil = inwardModel.Inward.SupCoilNo.Length;
                        if (instexl == 2)
                        {
                            if (count != inwardModel.Inward.SupCoilNo.Length)
                            {
                                SetSuccessMessage("Supplier Coil No is Already Exists");
                                ViewBag.FocusType = "1";
                                return View(inwardModel);
                            }
                            else
                            {
                                //Database check
                                for (int p = 0; p < inwardModel.Inward.SupCoilNo.Length; p++)
                                {
                                    if (inwardModel.Inward.SupCoilNo[p] != null)
                                    {
                                        SqlParameter[] sqlParam = new SqlParameter[6];
                                        sqlParam[0] = new SqlParameter("@SupCoil", inwardModel.Inward.SupCoilNo[p]);
                                        sqlParam[1] = new SqlParameter("@cmpvou", companyId);
                                        sqlParam[2] = new SqlParameter("@Type", "INTR");
                                        sqlParam[3] = new SqlParameter("@Vou", "0");
                                        sqlParam[4] = new SqlParameter("@Flg", "1");
                                        sqlParam[5] = new SqlParameter("@MainVou", id);
                                        DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetLotMstDetails1", sqlParam);
                                        if (DtInw != null && DtInw.Columns.Count > 1)
                                        {
                                            SetSuccessMessage("Supplier Coil No is Already Exists");
                                            ViewBag.FocusType = "1";
                                            return View(inwardModel);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (count != inwardModel.Inward.SupCoilNo.Length - 1)
                            {
                                SetSuccessMessage("Supplier Coil No is Already Exists");
                                ViewBag.FocusType = "1";
                                return View(inwardModel);
                            }
                            else
                            {
                                //Database check
                                for (int p = 0; p < inwardModel.Inward.SupCoilNo.Length; p++)
                                {
                                    if (inwardModel.Inward.SupCoilNo[p] != null)
                                    {
                                        SqlParameter[] sqlParam = new SqlParameter[6];
                                        sqlParam[0] = new SqlParameter("@SupCoil", inwardModel.Inward.SupCoilNo[p]);
                                        sqlParam[1] = new SqlParameter("@cmpvou", companyId);
                                        sqlParam[2] = new SqlParameter("@Type", "INTR");
                                        sqlParam[3] = new SqlParameter("@Vou", "0");
                                        sqlParam[4] = new SqlParameter("@Flg", "1");
                                        sqlParam[5] = new SqlParameter("@MainVou", id);
                                        DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetLotMstDetails1", sqlParam);
                                        if (DtInw != null && DtInw.Columns.Count > 1)
                                        {
                                            SetSuccessMessage("Supplier Coil No is Already Exists");
                                            ViewBag.FocusType = "1";
                                            return View(inwardModel);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        //Database check
                        for (int p = 0; p < inwardModel.Inward.SupCoilNo.Length; p++)
                        {
                            if (inwardModel.Inward.SupCoilNo[p] != null)
                            {
                                SqlParameter[] sqlParam = new SqlParameter[6];
                                sqlParam[0] = new SqlParameter("@SupCoil", inwardModel.Inward.SupCoilNo[p]);
                                sqlParam[1] = new SqlParameter("@cmpvou", companyId);
                                sqlParam[2] = new SqlParameter("@Type", "INTR");
                                sqlParam[3] = new SqlParameter("@Vou", p);
                                sqlParam[4] = new SqlParameter("@Flg", "1");
                                sqlParam[5] = new SqlParameter("@MainVou", id);
                                DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetLotMstDetails1", sqlParam);
                                if (DtInw != null && DtInw.Columns.Count > 1)
                                {
                                    SetSuccessMessage("Supplier Coil No is Already Exists");
                                    ViewBag.FocusType = "1";
                                    return View(inwardModel);
                                }
                            }
                        }
                    }
                    SqlParameter[] sqlParameters = new SqlParameter[13];
                    sqlParameters[0] = new SqlParameter("@InwVNo", inwardModel.InwVNo);
                    sqlParameters[1] = new SqlParameter("@InwDt", inwardModel.InwDt);
                    sqlParameters[2] = new SqlParameter("@InwRefNo", inwardModel.InwRefNo);
                    sqlParameters[3] = new SqlParameter("@InwAccVou", inwardModel.InwAccVou);
                    sqlParameters[4] = new SqlParameter("@InwPrdTyp", inwardModel.InwPtyVou);
                    sqlParameters[5] = new SqlParameter("@InwRem", inwardModel.InwRem);
                    sqlParameters[6] = new SqlParameter("@InwVou", id);
                    sqlParameters[7] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[8] = new SqlParameter("@FLG", 1);
                    sqlParameters[9] = new SqlParameter("@CmpVou", inwardModel.InwCmpVou);
                    sqlParameters[10] = new SqlParameter("@InwPrdVou", inwardModel.InwPrdVou);
                    sqlParameters[11] = new SqlParameter("@InwFrtRt", inwardModel.InwFrightRt);
                    sqlParameters[12] = new SqlParameter("@InwETADt", inwardModel.InwETADt);
                    DataTable DtInwMst = ObjDBConnection.CallStoreProcedure("InTransInwardMst_Insert", sqlParameters);
                    if (DtInwMst != null && DtInwMst.Rows.Count > 0)
                    {
                        int masterId = DbConnection.ParseInt32(DtInwMst.Rows[0][0].ToString());
                        if (masterId > 0)
                        {
                            for (int i = 0; i < inwardModel.Inward.IntThickCoil.Length; i++)
                            {

                                if (inwardModel.Inward.IntThickCoil[i] != null && inwardModel.Inward.IntThickCoil[i] != 0 && inwardModel.Inward.IntThickCoil[i] > 0)
                                {
                                    SqlParameter[] parameter = new SqlParameter[26];
                                    parameter[0] = new SqlParameter("@IntInwVou", masterId);
                                    parameter[1] = new SqlParameter("@IntSrNo", (i + 1));
                                    parameter[2] = new SqlParameter("@IntPrdVou", "0");
                                    parameter[3] = new SqlParameter("@IntGrade", inwardModel.Inward.IntGrdCoil[i]);
                                    parameter[4] = new SqlParameter("@IntSpacifi", "");
                                    parameter[5] = new SqlParameter("@IntWidth", inwardModel.Inward.IntWidth[i]);
                                    parameter[6] = new SqlParameter("@IntThick", inwardModel.Inward.IntThickCoil[i]);
                                    parameter[7] = new SqlParameter("@IntQty", inwardModel.Inward.IntQtyCoil[i]);
                                    parameter[8] = new SqlParameter("@IntPcs", "0");
                                    if (inwardModel.Inward.IntRemksCoil[i] == "-1")
                                    {
                                        parameter[9] = new SqlParameter("@IntRem", "");
                                    }
                                    else
                                    {
                                        parameter[9] = new SqlParameter("@IntRem", inwardModel.Inward.IntRemksCoil[i + 1]);
                                    }
                                    parameter[10] = new SqlParameter("@IntCmpVou", inwardModel.InwCmpVou);
                                    parameter[11] = new SqlParameter("@IntPOTVou", "0");
                                    parameter[12] = new SqlParameter("@IntLotVou", "0");
                                    parameter[13] = new SqlParameter("@IntGdnVou", inwardModel.Inward.IntGdnCoil[i + 1]);
                                    parameter[14] = new SqlParameter("@IntLocVou", "0");
                                    parameter[15] = new SqlParameter("@IntSupCoilNo", inwardModel.Inward.SupCoilNo[i + 1]);
                                    parameter[16] = new SqlParameter("@IntHeatNo", inwardModel.Inward.HeatNo[i + 1]);
                                    parameter[17] = new SqlParameter("@IntOD", "0");
                                    parameter[18] = new SqlParameter("@IntLenght", "0");
                                    parameter[19] = new SqlParameter("@IntFinish", "");
                                    parameter[20] = new SqlParameter("@IntProcess", "");
                                    parameter[21] = new SqlParameter("@IntBillNo", inwardModel.Inward.BillNoCoil[i + 1]);
                                    parameter[22] = new SqlParameter("@IntCoilTypeVou", 5);
                                    parameter[23] = new SqlParameter("@IntScNo", inwardModel.Inward.IntSCNo[i + 1]);
                                    parameter[24] = new SqlParameter("@IntPOWidth", inwardModel.Inward.IntPOWidth[i]);
                                    parameter[25] = new SqlParameter("@IntPOThick", inwardModel.Inward.IntPOThick[i]);
                                    DataTable DtInwTrn = ObjDBConnection.CallStoreProcedure("InTransInwardTrn_Insert", parameter);
                                }
                            }
                            int Status = DbConnection.ParseInt32(DtInwMst.Rows[0][0].ToString());
                            if (Status == 0)
                            {
                                inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                inwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                inwardModel.HdGodwonList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

                                if (inwardModel.Inward == null)
                                    inwardModel.Inward = new InTransInwardGridModel();

                                inwardModel.Inward.CoilTypeList = objProductHelper.GetMainCoilType();
                                inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                                inwardModel.InwDt = Convert.ToDateTime(inwardModel.InwDt).ToString("yyyy-MM-dd");
                                SetErrorMessage("Dulplicate Vou.No Details");
                                ViewBag.FocusType = "1";
                                return View(inwardModel);
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Updated Sucessfully");
                                }
                                else
                                {
                                    SetSuccessMessage("Inserted Sucessfully");
                                }
                                return RedirectToAction("Index", "InTransInward", new { id = 0 });
                            }
                        }
                        else
                        {
                            inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                            inwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                            inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                            inwardModel.HdGodwonList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

                            if (inwardModel.Inward == null)
                                inwardModel.Inward = new InTransInwardGridModel();

                            inwardModel.Inward.CoilTypeList = objProductHelper.GetMainCoilType();
                            inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                            inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                            inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                            inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                            inwardModel.InwDt = Convert.ToDateTime(inwardModel.InwDt).ToString("yyyy-MM-dd");
                            SetErrorMessage("Insert error");
                            ViewBag.FocusType = "1";
                            return View(inwardModel);
                        }
                    }
                    else
                    {
                        inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                        inwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                        inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                        inwardModel.HdGodwonList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

                        if (inwardModel.Inward == null)
                            inwardModel.Inward = new InTransInwardGridModel();

                        inwardModel.Inward.CoilTypeList = objProductHelper.GetMainCoilType();
                        inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                        inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                        inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                        inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                        inwardModel.InwDt = Convert.ToDateTime(inwardModel.InwDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(inwardModel);
                    }
                }
                else
                {
                    inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                    inwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                    inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                    inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                    inwardModel.HdGodwonList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

                    if (inwardModel.Inward == null)
                        inwardModel.Inward = new InTransInwardGridModel();

                    inwardModel.Inward.CoilTypeList = objProductHelper.GetMainCoilType();
                    inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                    inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                    inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                    inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                    inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                    inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                    inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                    inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                    inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                    inwardModel.InwDt = Convert.ToDateTime(inwardModel.InwDt).ToString("yyyy-MM-dd");
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "1";
                    return View(inwardModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index");
        }

        public InTransInwardModel GetLastVoucherNo(int companyId)
        {
            InTransInwardModel obj = new InTransInwardModel();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@Cmpvou", companyId);
                DataTable dtNewInwVNo = ObjDBConnection.CallStoreProcedure("GetLatestInTransInwVNo", sqlParameters);
                if (dtNewInwVNo != null && dtNewInwVNo.Rows.Count > 0)
                {
                    int.TryParse(dtNewInwVNo.Rows[0]["InwVNo"].ToString(), out int InwVNo);
                    InwVNo = InwVNo == 0 ? 1 : InwVNo;
                    obj.InwVNo = InwVNo;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return obj;
        }

        public JsonResult GetNBSChValue(string thickpipe, string odpipe)
        {
            PurchaseOrderModel obj = new PurchaseOrderModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(thickpipe) && !string.IsNullOrWhiteSpace(odpipe))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@thickpipe", thickpipe);
                    sqlParameters[1] = new SqlParameter("@odpipe", odpipe);
                    DataTable dtNBSCH = ObjDBConnection.CallStoreProcedure("GetNBSCHDetails", sqlParameters);
                    if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
                    {
                        string OrdNB = dtNBSCH.Rows[0]["OrdNB"].ToString();
                        string OrdSch = dtNBSCH.Rows[0]["OrdSch"].ToString();
                        return Json(new { result = true, ordNb = OrdNB, ordSch = OrdSch });
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { result = false });
        }

        public IActionResult Delete(long id)
        {
            try
            {
                InTransInwardModel inwardModel = new InTransInwardModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@InwVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "1");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetInTransInwardDetails", sqlParameters);
                    if (DtInw != null && DtInw.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtInw.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("InTrasit Inward Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "InTransInward");
        }

        public IActionResult GetDeleteSupCoil(string supcoilno)
        {
            try
            {
                InTransInwardModel inwardModel = new InTransInwardModel();
                if (supcoilno != "")
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("@SupCoil", supcoilno);
                    DataTable DtInw = ObjDBConnection.CallStoreProcedure("DeleteSupCoilMst", sqlParameters);
                    if (DtInw != null && DtInw.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtInw.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            return Json(new { result = true, data = "0" });
                        }
                        else
                        {
                            return Json(new { result = true, data = "1" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View();
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
                    string currentURL = "/InTransInward/Index";
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
                    getReportDataModel.ControllerName = "InTransInward";
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
                    var bytes = Excel(getReportDataModel, "InTrans Inward Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "InTransInward.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "InTrans Inward Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "InTransInward.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult GetSupCoilCheck(string supcoil, string inwvou, int srno1)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@SupCoil", supcoil);
                sqlParameters[1] = new SqlParameter("@cmpvou", companyId);
                sqlParameters[2] = new SqlParameter("@Type", "INTR");
                sqlParameters[3] = new SqlParameter("@Vou", srno1);
                sqlParameters[4] = new SqlParameter("@Flg", "0");
                sqlParameters[5] = new SqlParameter("@MainVou", inwvou);
                DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetLotMstDetails1", sqlParameters);
                if (DtInw != null && DtInw.Columns.Count > 1)
                {
                    return Json(new { result = true, data = "1" });

                }
                else
                {
                    return Json(new { result = true, data = "-1" });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult Excel(IFormFile file, string type)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                long userId = GetIntSession("UserId");
                DataTable dtExcel = new DataTable();
                DataTable dtDefault = new DataTable();
                if (file != null && !string.IsNullOrWhiteSpace(type))
                {
                    var bytes = FileHelper.ConvertIFormFileToBytes(file);
                    string fileName = DateTime.Today.Ticks.ToString() + ".xlsx";
                    if (bytes != null)
                    {
                        var uploads = Path.Combine(hostingEnvironment.WebRootPath, "InTransExcels");
                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }

                        if (FileHelper.FileUpload(uploads, string.Concat(uploads, "/", fileName), bytes))
                        {
                            string defaultFilePath = string.Empty;
                            dtDefault = FileHelper.ReadXLSXFile(string.Concat(hostingEnvironment.WebRootPath, "/InTranscoil.xlsx"));
                            dtExcel = FileHelper.ReadXLSXFile(string.Concat(uploads, "/", fileName));
                        }
                    }
                    if (dtExcel != null && dtExcel.Columns.Count > 0 && dtExcel.Rows.Count > 0)
                    {
                        if (dtExcel.Columns.Count == dtDefault.Columns.Count)
                        {
                            bool isValid = true;
                            for (int i = 0; i < dtDefault.Columns.Count; i++)
                            {
                                if (string.IsNullOrWhiteSpace(dtExcel.Columns[i].ColumnName.Trim().ToString()) || !dtExcel.Columns[i].ColumnName.Trim().ToString().ToLower().Equals(dtDefault.Columns[i].ColumnName.Trim().ToLower()))
                                {
                                    isValid = false;
                                    break;
                                }
                            }
                            if (!isValid)
                            {
                                return Json(new { result = false, message = "Upload excel column not match with sample excel file." });
                            }
                            else
                            {
                                var gradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                DataTable dtGrade = new DataTable();
                                if (gradeList != null && gradeList.Count > 0)
                                {
                                    dtGrade = ListtoDataTableConverter.ToDataTable(gradeList);
                                }
                                List<SelectListItem> notFoundItemList = new List<SelectListItem>();
                                var gridList = SalesHelper.InTranProcessExcel(dtExcel, dtGrade, companyId.ToString(), userId.ToString(), ref notFoundItemList);
                                if (gridList.Count > 75)
                                {
                                    return Json(new { result = false, message = "Please Enter Maximum 75 Rows Only" });
                                }
                                else
                                {
                                    if ((gridList != null && gridList.Count > 0) || (notFoundItemList != null && notFoundItemList.Count > 0))
                                    {
                                        gradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                        return Json(new { result = true, message = "Excel uploaded successfully", gridList = gridList, gradeList = gradeList, notFoundItemList = notFoundItemList });

                                    }
                                    else
                                    {
                                        return Json(new { result = false, message = "Error in uploading excel" });
                                    }

                                }
                            }
                        }
                        else
                        {
                            return Json(new { result = false, message = "Upload excel column not match with sample excel file." });
                        }

                    }
                    else
                    {
                        return Json(new { result = false, message = "Upload excel column not match with sample excel file." });
                    }

                }
                else
                {
                    return Json(new { result = false, message = "Please select file" });
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { result = false, message = "Internal error!" });
        }

        [Route("/InTransInward/GetAccount-List")]
        public IActionResult AccountList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var superList = new List<CustomDropDown>();
            superList.Add(CommonHelpers.GetDefaultValue());
            superList.AddRange(ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0));

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                superList = superList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }

            return Json(new { items = CommonHelpers.BindSelect2Model(superList) });
        }
    }
}
