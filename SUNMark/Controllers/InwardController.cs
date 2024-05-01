using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SUNMark.Classes;
using SUNMark.Models;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using PageSize = iTextSharp.text.PageSize;
using Paragraph = iTextSharp.text.Paragraph;
using Rectangle = iTextSharp.text.Rectangle;
using System.Web;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using SUNMark.Common;

namespace SUNMark.Controllers
{
    public class InwardController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        private readonly IWebHostEnvironment _iwebhostenviroment;

        public InwardController(IWebHostEnvironment iwebhostenviroment)
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
                int administrator = 0;
                InwardModel inwardModel = new InwardModel();
                inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                inwardModel.AccountList = new List<CustomDropDown>();
                inwardModel.AccountList.Add(CommonHelpers.GetDefaultValue());
                inwardModel.AccountList.AddRange(ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0));
                inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "Select");

                inwardModel.Inward = new InwardGridModel();
                inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                inwardModel.Inward.IntCoilTypeList = objProductHelper.GetInwardCoilType();
                SqlParameter[] sqlParam = new SqlParameter[3];
                sqlParam[0] = new SqlParameter("@InwVou", id);
                sqlParam[1] = new SqlParameter("@Flg", 3);
                sqlParam[2] = new SqlParameter("@CmpVou", companyId);
                DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetInwardDetails", sqlParam);
                if (DtInw != null && DtInw.Rows.Count > 0)
                {
                    inwardModel.InwCmpVou = Convert.ToInt32(DtInw.Rows[0]["InwCmpVou"].ToString());
                    inwardModel.InwPtyVou = Convert.ToInt32(DtInw.Rows[0]["InwPrdTyp"].ToString());
                    inwardModel.InwPrdTyp = DtInw.Rows[0]["PrdType"].ToString();
                    inwardModel.InwDt = !string.IsNullOrWhiteSpace(DtInw.Rows[0]["InwDt"].ToString()) ? Convert.ToDateTime(DtInw.Rows[0]["InwDt"].ToString()).ToString("yyyy-MM-dd") : "";
                    inwardModel.InwAccVou = Convert.ToInt32(DtInw.Rows[0]["InwAccVou"].ToString());
                    int vno = DbConnection.ParseInt32(DtInw.Rows[0][0].ToString());
                    inwardModel.InwVNo = vno + 1;
                    var ptynm = DtInw.Rows[0]["PrdType"].ToString();
                    if (ptynm == "COIL" || ptynm == "Coil")
                    {
                        inwardModel.Inward.SupCoilNoStr = DtInw.Rows[0]["IntSupCoilNo"].ToString();
                        inwardModel.Inward.HeatNoStr = DtInw.Rows[0]["IntHeatNo"].ToString();
                    }
                }

                if (id > 0)
                {
                    inwardModel.InwVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@InwVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtInwMst = ObjDBConnection.CallStoreProcedure("GetInwardDetails", sqlParameters);
                    if (DtInwMst != null && DtInwMst.Rows.Count > 0)
                    {
                        inwardModel.InwVNo = Convert.ToInt32(DtInwMst.Rows[0]["InwVNo"].ToString());
                        inwardModel.InwDt = !string.IsNullOrWhiteSpace(DtInwMst.Rows[0]["InwDt"].ToString()) ? Convert.ToDateTime(DtInwMst.Rows[0]["InwDt"].ToString()).ToString("yyyy-MM-dd") : "";
                        inwardModel.InwAccVou = Convert.ToInt32(DtInwMst.Rows[0]["InwAccVou"].ToString());
                        inwardModel.InwRefNo = DtInwMst.Rows[0]["InwRefNo"].ToString();
                        inwardModel.InwBillDt = !string.IsNullOrWhiteSpace(DtInwMst.Rows[0]["InwBillDt"].ToString()) ? Convert.ToDateTime(DtInwMst.Rows[0]["InwBillDt"].ToString()).ToString("yyyy-MM-dd") : "";
                        inwardModel.InwPtyVou = Convert.ToInt32(DtInwMst.Rows[0]["InwPrdTyp"].ToString());
                        inwardModel.InwPOMVou = Convert.ToInt32(DtInwMst.Rows[0]["InwPOMVou"].ToString());
                        inwardModel.InwPrdTyp = DtInwMst.Rows[0]["PrdType"].ToString();
                        inwardModel.InwRem = DtInwMst.Rows[0]["InwRem"].ToString();
                        inwardModel.InwCmpVou = Convert.ToInt32(DtInwMst.Rows[0]["InwCmpVou"].ToString());
                        inwardModel.InwLRNo = DtInwMst.Rows[0]["InwLRNo"].ToString();
                        inwardModel.InwWPNo = DtInwMst.Rows[0]["InwWpSlipNo"].ToString();
                        inwardModel.InwWPWeight = DtInwMst.Rows[0]["InwWpWeight"].ToString();
                        inwardModel.InwVehNo = DtInwMst.Rows[0]["InwVehNo"].ToString();
                        inwardModel.InwTransNm = DtInwMst.Rows[0]["InwTransNm"].ToString();
                        inwardModel.InwTrnVou = Convert.ToInt32(DtInwMst.Rows[0]["InwTrnVou"].ToString());
                        inwardModel.InwFrightRt = Convert.ToDecimal(DtInwMst.Rows[0]["InwFrtRt"].ToString());
                        inwardModel.InwBillNo = DtInwMst.Rows[0]["InwBillNo"].ToString();
                        inwardModel.InwPrdVou = Convert.ToInt32(DtInwMst.Rows[0]["InwPrdVou"].ToString());
                        //inwardModel.InwCoilTypeVou = Convert.ToInt32(DtInwMst.Rows[0]["InwCoilTypeVou"].ToString());
                        //inwardModel.CoilType = DtInwMst.Rows[0]["InwCoilType"].ToString();

                        var ptynm = DtInwMst.Rows[0]["PrdType"].ToString();
                        if (ptynm == "COIL" || ptynm == "Coil")
                        {
                            inwardModel.Inward.BillNoCoil = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.SupCoilNo = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.HeatNo = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntGrdCoil = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntGdnCoil = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntCoilType = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.CoilNo = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntThickCoil = new decimal[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntWidth = new decimal[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntQtyCoil = new decimal[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntRemksCoil = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntCoilPrefix = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntSufix = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntCoilType = new string[DtInwMst.Rows.Count];
                        }
                        else if (ptynm == "PIPE" || ptynm == "Pipe" || ptynm == "SEAMLESS PIPE" || ptynm == "seamless pipe")
                        {
                            inwardModel.Inward.BillNoPipe = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntGdnPipe = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.SpecifiName = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntGrdPipe = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntOD = new decimal[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntThickPipe = new decimal[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntLenghtPipe = new decimal[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntPcs = new decimal[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntQtyPipe = new decimal[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntFinshPipe = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntProcePipe = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntRemksPipe = new string[DtInwMst.Rows.Count];
                        }
                        else
                        {
                            inwardModel.Inward.BillNoOther = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntPrdVou = new int[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntGdnOther = new string[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntQtyOther = new decimal[DtInwMst.Rows.Count];
                            inwardModel.Inward.IntRemksOther = new string[DtInwMst.Rows.Count];
                        }
                        for (int i = 0; i < DtInwMst.Rows.Count; i++)
                        {
                            if (ptynm == "COIL" || ptynm == "Coil")
                            {
                                inwardModel.Inward.BillNoCoil[i] = DtInwMst.Rows[i]["IntBillNo"].ToString();
                                inwardModel.Inward.SupCoilNo[i] = DtInwMst.Rows[i]["IntSupCoilNo"].ToString();
                                inwardModel.Inward.HeatNo[i] = DtInwMst.Rows[i]["IntHeatNo"].ToString();
                                inwardModel.Inward.IntGrdCoil[i] = DtInwMst.Rows[i]["IntGrade"].ToString();
                                inwardModel.Inward.IntCoilType[i] = DtInwMst.Rows[i]["IntCoilType"].ToString();
                                inwardModel.Inward.IntGdnCoil[i] = DtInwMst.Rows[i]["IntGdnVou"].ToString();
                                inwardModel.Inward.CoilNo[i] = DtInwMst.Rows[i]["CoilNo"].ToString();
                                inwardModel.Inward.IntThickCoil[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntThick"].ToString());
                                inwardModel.Inward.IntWidth[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntWidth"].ToString());
                                inwardModel.Inward.IntQtyCoil[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntQty"].ToString());
                                inwardModel.Inward.IntRemksCoil[i] = DtInwMst.Rows[i]["IntRem"].ToString();
                                inwardModel.Inward.CoilNo[i] = DtInwMst.Rows[i]["IntCoilNo"].ToString();
                                inwardModel.Inward.IntCoilPrefix[i] = DtInwMst.Rows[i]["IntPrefix"].ToString().Trim();
                                inwardModel.Inward.IntSufix[i] = Convert.ToString(DtInwMst.Rows[i]["IntSufix"].ToString());
                                inwardModel.Inward.IntCoilType[i] = DtInwMst.Rows[i]["IntCoilTypeVou"].ToString();
                            }
                            else if (ptynm == "PIPE" || ptynm == "Pipe" || ptynm == "SEAMLESS PIPE" || ptynm == "seamless pipe")
                            {
                                inwardModel.Inward.BillNoPipe[i] = DtInwMst.Rows[i]["IntBillNo"].ToString();
                                inwardModel.Inward.IntGdnPipe[i] = DtInwMst.Rows[i]["IntGdnVou"].ToString();
                                inwardModel.Inward.SpecifiName[i] = DtInwMst.Rows[i]["IntSpacifi"].ToString();
                                inwardModel.Inward.IntGrdPipe[i] = DtInwMst.Rows[i]["IntGrade"].ToString();
                                inwardModel.Inward.IntOD[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntOD"].ToString());
                                inwardModel.Inward.IntThickPipe[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntThick"].ToString());
                                inwardModel.Inward.IntLenghtPipe[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntLenght"].ToString());
                                inwardModel.Inward.IntPcs[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntPcs"].ToString());
                                inwardModel.Inward.IntQtyPipe[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntQty"].ToString());
                                inwardModel.Inward.IntFinshPipe[i] = DtInwMst.Rows[i]["IntFinish"].ToString();
                                inwardModel.Inward.IntProcePipe[i] = DtInwMst.Rows[i]["IntProcess"].ToString();
                                inwardModel.Inward.IntRemksPipe[i] = DtInwMst.Rows[i]["IntRem"].ToString();
                            }
                            else
                            {
                                inwardModel.Inward.BillNoOther[i] = DtInwMst.Rows[i]["IntBillNo"].ToString();
                                inwardModel.Inward.IntPrdVou[i] = Convert.ToInt32(DtInwMst.Rows[i]["IntPrdVou"].ToString());
                                inwardModel.Inward.IntGdnOther[i] = DtInwMst.Rows[i]["IntGdnVou"].ToString();
                                inwardModel.Inward.IntQtyOther[i] = Convert.ToDecimal(DtInwMst.Rows[i]["IntQty"].ToString());
                                inwardModel.Inward.IntRemksOther[i] = DtInwMst.Rows[i]["IntRem"].ToString();
                            }

                        }

                    }
                }
                if (id > 0)
                    TempData["ReturnId"] = Convert.ToString(id);
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
        public IActionResult Index(long id, InwardModel inwardModel)
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
                inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                inwardModel.MainProductList = objProductHelper.GetProductMasterDropdown(companyId);

                if (inwardModel.Inward == null)
                    inwardModel.Inward = new InwardGridModel();

                inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                inwardModel.Inward.IntCoilTypeList = objProductHelper.GetInwardCoilType();

                var prdtype = inwardModel.InwPrdTyp;
                if (prdtype == "COIL" || prdtype == "Coil")
                {
                    if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwVNo).ToString()) && !string.IsNullOrWhiteSpace(inwardModel.InwDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwAccVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwPtyVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwCoilTypeVou).ToString()) && inwardModel.Inward.IntGrdCoil.Length > 0 && inwardModel.Inward.IntGdnCoil.Length > 0 && inwardModel.Inward.IntQtyCoil.Length > 0)
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[22];
                        sqlParameters[0] = new SqlParameter("@InwVNo", inwardModel.InwVNo);
                        sqlParameters[1] = new SqlParameter("@InwDt", inwardModel.InwDt);
                        sqlParameters[2] = new SqlParameter("@InwRefNo", inwardModel.InwRefNo);
                        sqlParameters[3] = new SqlParameter("@InwAccVou", inwardModel.InwAccVou);
                        sqlParameters[4] = new SqlParameter("@InwPrdTyp", inwardModel.InwPtyVou);
                        sqlParameters[5] = new SqlParameter("@InwRem", inwardModel.InwRem);
                        sqlParameters[6] = new SqlParameter("@InwVou", id);
                        sqlParameters[7] = new SqlParameter("@InwPOMVou", inwardModel.InwPOMVou);
                        sqlParameters[8] = new SqlParameter("@InwLRNo", inwardModel.InwLRNo);
                        sqlParameters[9] = new SqlParameter("@InwWPNo", inwardModel.InwWPNo);
                        sqlParameters[10] = new SqlParameter("@InwVehNo", inwardModel.InwVehNo);
                        if (inwardModel.InwTransNm == "Select")
                        {
                            sqlParameters[11] = new SqlParameter("@InwTransNm", "");
                        }
                        else
                        {
                            sqlParameters[11] = new SqlParameter("@InwTransNm", inwardModel.InwTransNm);
                        }
                        sqlParameters[12] = new SqlParameter("@InwBillNo", inwardModel.InwBillNo);
                        sqlParameters[13] = new SqlParameter("@UsrVou", userId);
                        sqlParameters[14] = new SqlParameter("@FLG", 1);
                        sqlParameters[15] = new SqlParameter("@CmpVou", inwardModel.InwCmpVou);
                        sqlParameters[16] = new SqlParameter("@InwPrdVou", inwardModel.InwPrdVou);
                        sqlParameters[17] = new SqlParameter("@InwTrnVou", inwardModel.InwTrnVou);
                        sqlParameters[18] = new SqlParameter("@InwFrtRt", inwardModel.InwFrightRt);
                        sqlParameters[19] = new SqlParameter("@InwCoilTypeVou", inwardModel.InwCoilTypeVou);
                        sqlParameters[20] = new SqlParameter("@InwCoilType", inwardModel.CoilType);
                        sqlParameters[21] = new SqlParameter("@InwWPWeight", inwardModel.InwWPWeight);
                        DataTable DtInwMst = ObjDBConnection.CallStoreProcedure("InwardMst_Insert", sqlParameters);
                        if (DtInwMst != null && DtInwMst.Rows.Count > 0)
                        {
                            int masterId = DbConnection.ParseInt32(DtInwMst.Rows[0][0].ToString());
                            if (masterId > 0)
                            {
                                for (int i = 0; i < inwardModel.Inward.IntQtyCoil.Length; i++)
                                {

                                    if (inwardModel.Inward.IntQtyCoil[i] != null && inwardModel.Inward.IntQtyCoil[i] != 0 && inwardModel.Inward.IntQtyCoil[i] > 0)
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
                                            parameter[9] = new SqlParameter("@IntRem", inwardModel.Inward.IntRemksCoil[i]);
                                        }
                                        parameter[10] = new SqlParameter("@IntCmpVou", inwardModel.InwCmpVou);
                                        parameter[11] = new SqlParameter("@IntPOTVou", "0");
                                        parameter[12] = new SqlParameter("@IntLotVou", "0");
                                        parameter[13] = new SqlParameter("@IntGdnVou", inwardModel.Inward.IntGdnCoil[i]);
                                        parameter[14] = new SqlParameter("@IntLocVou", "0");
                                        parameter[15] = new SqlParameter("@IntSupCoilNo", inwardModel.Inward.SupCoilNo[i]);
                                        parameter[16] = new SqlParameter("@IntHeatNo", inwardModel.Inward.HeatNo[i]);
                                        parameter[17] = new SqlParameter("@IntOD", "0");
                                        parameter[18] = new SqlParameter("@IntLenght", "0");
                                        parameter[19] = new SqlParameter("@IntFinish", "");
                                        parameter[20] = new SqlParameter("@IntProcess", "");
                                        parameter[21] = new SqlParameter("@IntBillNo", inwardModel.Inward.BillNoCoil[i]);
                                        parameter[22] = new SqlParameter("@IntCoilPrefix", inwardModel.Inward.IntCoilPrefix[i + 1]);
                                        parameter[23] = new SqlParameter("@IntCoilNo", inwardModel.Inward.CoilNo[i]);
                                        parameter[24] = new SqlParameter("@IntSufix", inwardModel.Inward.IntSufix[i]);
                                        parameter[25] = new SqlParameter("@IntCoilTypeVou", inwardModel.Inward.IntCoilType[i]);
                                        DataTable DtInwTrn = ObjDBConnection.CallStoreProcedure("InwardTrn_Insert", parameter);
                                    }
                                }
                                int Status = DbConnection.ParseInt32(DtInwMst.Rows[0][0].ToString());
                                if (Status == 0)
                                {
                                    inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                    inwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                    inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                    inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                    inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                    inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                                    if (inwardModel.Inward == null)
                                        inwardModel.Inward = new InwardGridModel();

                                    inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                                    inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
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
                                        if (inwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = inwardModel.isPrint;
                                        }
                                    }
                                    else
                                    {
                                        SetSuccessMessage("Inserted Sucessfully");
                                        if (inwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = inwardModel.isPrint;
                                        }
                                    }
                                    if (inwardModel.isPrint == 0)
                                        return RedirectToAction("Index", new { id = 0 });
                                }
                            }
                            else
                            {
                                inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                inwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                                if (inwardModel.Inward == null)
                                    inwardModel.Inward = new InwardGridModel();

                                inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                                inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
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
                            inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                            inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                            inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                            inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                            if (inwardModel.Inward == null)
                                inwardModel.Inward = new InwardGridModel();

                            inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                            inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                            inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                            inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                            inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
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
                        inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                        inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                        inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                        inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                        if (inwardModel.Inward == null)
                            inwardModel.Inward = new InwardGridModel();

                        inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                        inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                        inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                        inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                        inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                        inwardModel.InwDt = Convert.ToDateTime(inwardModel.InwDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(inwardModel);
                    }
                }
                else if (prdtype == "PIPE" || prdtype == "Pipe" || prdtype == "SEAMLESS PIPE" || prdtype == "seamless pipe")
                {
                    if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwVNo).ToString()) && !string.IsNullOrWhiteSpace(inwardModel.InwDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwAccVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwPtyVou).ToString()) && inwardModel.Inward.IntGrdPipe.Length > 0 && inwardModel.Inward.IntGdnPipe.Length > 0 && inwardModel.Inward.SpecifiName.Length > 0 && inwardModel.Inward.IntFinshPipe.Length > 0 && inwardModel.Inward.IntProcePipe.Length > 0 && inwardModel.Inward.IntThickPipe.Length > 0 && inwardModel.Inward.IntOD.Length > 0 && inwardModel.Inward.IntPcs.Length > 0 && inwardModel.Inward.IntLenghtPipe.Length > 0 && inwardModel.Inward.IntQtyPipe.Length > 0)
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[22];
                        sqlParameters[0] = new SqlParameter("@InwVNo", inwardModel.InwVNo);
                        sqlParameters[1] = new SqlParameter("@InwDt", inwardModel.InwDt);
                        sqlParameters[2] = new SqlParameter("@InwRefNo", inwardModel.InwRefNo);
                        sqlParameters[3] = new SqlParameter("@InwAccVou", inwardModel.InwAccVou);
                        sqlParameters[4] = new SqlParameter("@InwPrdTyp", inwardModel.InwPtyVou);
                        sqlParameters[5] = new SqlParameter("@InwRem", inwardModel.InwRem);
                        sqlParameters[6] = new SqlParameter("@InwVou", id);
                        sqlParameters[7] = new SqlParameter("@InwPOMVou", inwardModel.InwPOMVou);
                        sqlParameters[8] = new SqlParameter("@InwLRNo", inwardModel.InwLRNo);
                        sqlParameters[9] = new SqlParameter("@InwWPNo", inwardModel.InwWPNo);
                        sqlParameters[10] = new SqlParameter("@InwVehNo", inwardModel.InwVehNo);
                        sqlParameters[11] = new SqlParameter("@InwTransNm", inwardModel.InwTransNm);
                        sqlParameters[12] = new SqlParameter("@InwBillNo", inwardModel.InwBillNo);
                        sqlParameters[13] = new SqlParameter("@UsrVou", userId);
                        sqlParameters[14] = new SqlParameter("@FLG", 1);
                        sqlParameters[15] = new SqlParameter("@CmpVou", inwardModel.InwCmpVou);
                        sqlParameters[16] = new SqlParameter("@InwPrdVou", inwardModel.InwPrdVou);
                        sqlParameters[17] = new SqlParameter("@InwTrnVou", inwardModel.InwTrnVou);
                        sqlParameters[18] = new SqlParameter("@InwFrtRt", inwardModel.InwFrightRt);
                        sqlParameters[19] = new SqlParameter("@InwCoilTypeVou", inwardModel.InwCoilTypeVou);
                        sqlParameters[20] = new SqlParameter("@InwCoilType", inwardModel.CoilType);
                        sqlParameters[21] = new SqlParameter("@InwWPWeight", inwardModel.InwWPWeight);
                        DataTable DtInwMst = ObjDBConnection.CallStoreProcedure("InwardMst_Insert", sqlParameters);
                        if (DtInwMst != null && DtInwMst.Rows.Count > 0)
                        {
                            int masterId = DbConnection.ParseInt32(DtInwMst.Rows[0][0].ToString());
                            if (masterId > 0)
                            {
                                for (int i = 0; i < inwardModel.Inward.SpecifiName.Length; i++)
                                {
                                    if (inwardModel.Inward.IntThickPipe[i] != null && inwardModel.Inward.IntThickPipe[i] != 0 && inwardModel.Inward.IntThickPipe[i] > 0)
                                    {
                                        SqlParameter[] parameter = new SqlParameter[26];
                                        parameter[0] = new SqlParameter("@IntInwVou", masterId);
                                        parameter[1] = new SqlParameter("@IntSrNo", (i + 1));
                                        parameter[2] = new SqlParameter("@IntPrdVou", 0);
                                        parameter[3] = new SqlParameter("@IntGrade", inwardModel.Inward.IntGrdPipe[i]);
                                        parameter[4] = new SqlParameter("@IntSpacifi", inwardModel.Inward.SpecifiName[i]);
                                        parameter[5] = new SqlParameter("@IntWidth", 0);
                                        parameter[6] = new SqlParameter("@IntThick", inwardModel.Inward.IntThickPipe[i]);
                                        parameter[7] = new SqlParameter("@IntQty", inwardModel.Inward.IntQtyPipe[i]);
                                        parameter[8] = new SqlParameter("@IntPcs", inwardModel.Inward.IntPcs[i]);
                                        if (inwardModel.Inward.IntRemksPipe[i] == "-1")
                                        {
                                            parameter[9] = new SqlParameter("@IntRem", "");
                                        }
                                        else
                                        {
                                            parameter[9] = new SqlParameter("@IntRem", inwardModel.Inward.IntRemksPipe[i]);
                                        }
                                        parameter[10] = new SqlParameter("@IntCmpVou", inwardModel.InwCmpVou);
                                        parameter[11] = new SqlParameter("@IntPOTVou", 0);
                                        parameter[12] = new SqlParameter("@IntLotVou", 0);
                                        parameter[13] = new SqlParameter("@IntGdnVou", inwardModel.Inward.IntGdnPipe[i]);
                                        parameter[14] = new SqlParameter("@IntLocVou", "0");
                                        //:                                        }
                                        parameter[15] = new SqlParameter("@IntSupCoilNo", "");
                                        parameter[16] = new SqlParameter("@IntHeatNo", "");
                                        parameter[17] = new SqlParameter("@IntOD", inwardModel.Inward.IntOD[i]);
                                        parameter[18] = new SqlParameter("@IntLenght", inwardModel.Inward.IntLenghtPipe[i]);
                                        parameter[19] = new SqlParameter("@IntFinish", inwardModel.Inward.IntFinshPipe[i]);
                                        parameter[20] = new SqlParameter("@IntProcess", inwardModel.Inward.IntProcePipe[i]);
                                        parameter[21] = new SqlParameter("@IntBillNo", inwardModel.Inward.BillNoPipe[i]);
                                        parameter[22] = new SqlParameter("@IntCoilPrefix", "");
                                        parameter[23] = new SqlParameter("@IntCoilNo", "");
                                        parameter[24] = new SqlParameter("@IntSufix", "");
                                        parameter[25] = new SqlParameter("@IntCoilTypeVou", 0);
                                        DataTable DtInwTrn = ObjDBConnection.CallStoreProcedure("InwardTrn_Insert", parameter);
                                    }
                                }
                                int Status = DbConnection.ParseInt32(DtInwMst.Rows[0][0].ToString());
                                if (Status == 0)
                                {
                                    inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                    inwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                    inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                    inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                    inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                    inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();
                                    if (inwardModel.Inward == null)
                                        inwardModel.Inward = new InwardGridModel();

                                    inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                                    inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
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
                                        if (inwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = inwardModel.isPrint;
                                        }
                                    }
                                    else
                                    {
                                        SetSuccessMessage("Inserted Sucessfully");
                                        if (inwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = inwardModel.isPrint;
                                        }
                                    }
                                    if (inwardModel.isPrint == 0)
                                        return RedirectToAction("Index", "Inward", new { id = 0 });
                                }
                            }
                            else
                            {
                                inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                inwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                                if (inwardModel.Inward == null)
                                    inwardModel.Inward = new InwardGridModel();

                                inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                                inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
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
                            inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                            inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                            inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                            inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                            if (inwardModel.Inward == null)
                                inwardModel.Inward = new InwardGridModel();

                            inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                            inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                            inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                            inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                            inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
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
                        inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                        inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                        inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                        inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                        if (inwardModel.Inward == null)
                            inwardModel.Inward = new InwardGridModel();

                        inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                        inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                        inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                        inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                        inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                        inwardModel.InwDt = Convert.ToDateTime(inwardModel.InwDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(inwardModel);

                    }

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwVNo).ToString()) && !string.IsNullOrWhiteSpace(inwardModel.InwDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwAccVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(inwardModel.InwPtyVou).ToString()) && inwardModel.Inward.IntPrdVou.Length > 0 && inwardModel.Inward.IntGdnOther.Length > 0 && inwardModel.Inward.IntQtyOther.Length > 0)
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[22];
                        sqlParameters[0] = new SqlParameter("@InwVNo", inwardModel.InwVNo);
                        sqlParameters[1] = new SqlParameter("@InwDt", inwardModel.InwDt);
                        sqlParameters[2] = new SqlParameter("@InwRefNo", inwardModel.InwRefNo);
                        sqlParameters[3] = new SqlParameter("@InwAccVou", inwardModel.InwAccVou);
                        sqlParameters[4] = new SqlParameter("@InwPrdTyp", inwardModel.InwPtyVou);
                        sqlParameters[5] = new SqlParameter("@InwRem", inwardModel.InwRem);
                        sqlParameters[6] = new SqlParameter("@InwVou", id);
                        sqlParameters[7] = new SqlParameter("@InwPOMVou", inwardModel.InwPOMVou);
                        sqlParameters[8] = new SqlParameter("@InwLRNo", inwardModel.InwLRNo);
                        sqlParameters[9] = new SqlParameter("@InwWPNo", inwardModel.InwWPNo);
                        sqlParameters[10] = new SqlParameter("@InwVehNo", inwardModel.InwVehNo);
                        sqlParameters[11] = new SqlParameter("@InwTransNm", inwardModel.InwTransNm);
                        sqlParameters[12] = new SqlParameter("@InwBillNo", inwardModel.InwBillNo);
                        sqlParameters[13] = new SqlParameter("@UsrVou", userId);
                        sqlParameters[14] = new SqlParameter("@FLG", 1);
                        sqlParameters[15] = new SqlParameter("@CmpVou", inwardModel.InwCmpVou);
                        sqlParameters[16] = new SqlParameter("@InwPrdVou", inwardModel.InwPrdVou);
                        sqlParameters[17] = new SqlParameter("@InwTrnVou", inwardModel.InwTrnVou);
                        sqlParameters[18] = new SqlParameter("@InwFrtRt", inwardModel.InwFrightRt);
                        sqlParameters[19] = new SqlParameter("@InwCoilTypeVou", inwardModel.InwCoilTypeVou);
                        sqlParameters[20] = new SqlParameter("@InwCoilType", inwardModel.CoilType);
                        sqlParameters[21] = new SqlParameter("@InwWPWeight", inwardModel.InwWPWeight);
                        DataTable DtInwMst = ObjDBConnection.CallStoreProcedure("InwardMst_Insert", sqlParameters);
                        if (DtInwMst != null && DtInwMst.Rows.Count > 0)
                        {
                            int masterId = DbConnection.ParseInt32(DtInwMst.Rows[0][0].ToString());
                            if (masterId > 0)
                            {
                                for (int i = 0; i < inwardModel.Inward.IntPrdVou.Length; i++)
                                {
                                    if (inwardModel.Inward.IntQtyOther[i] != null && inwardModel.Inward.IntQtyOther[i] != 0 && inwardModel.Inward.IntQtyOther[i] > 0)
                                    {
                                        SqlParameter[] parameter = new SqlParameter[26];
                                        parameter[0] = new SqlParameter("@IntInwVou", masterId);
                                        parameter[1] = new SqlParameter("@IntSrNo", (i + 1));
                                        parameter[2] = new SqlParameter("@IntPrdVou", inwardModel.Inward.IntPrdVou[i]);
                                        parameter[3] = new SqlParameter("@IntGrade", "");
                                        parameter[4] = new SqlParameter("@IntSpacifi", "");
                                        parameter[5] = new SqlParameter("@IntWidth", "0");
                                        parameter[6] = new SqlParameter("@IntThick", "0");
                                        parameter[7] = new SqlParameter("@IntQty", inwardModel.Inward.IntQtyOther[i]);
                                        parameter[8] = new SqlParameter("@IntPcs", "0");
                                        if (inwardModel.Inward.IntRemksOther[i] == "-1")
                                        {
                                            parameter[9] = new SqlParameter("@IntRem", "");
                                        }
                                        else
                                        {
                                            parameter[9] = new SqlParameter("@IntRem", inwardModel.Inward.IntRemksOther[i]);
                                        }
                                        parameter[10] = new SqlParameter("@IntCmpVou", inwardModel.InwCmpVou);
                                        parameter[11] = new SqlParameter("@IntPOTVou", "0");
                                        parameter[12] = new SqlParameter("@IntLotVou", "0");
                                        parameter[13] = new SqlParameter("@IntGdnVou", inwardModel.Inward.IntGdnOther[i]);
                                        //if (inwardModel.Inward.IntLocOther != null && inwardModel.Inward.IntLocOther.Length> 0)
                                        //{
                                        //    parameter[15] = new SqlParameter("@IntLocVou", inwardModel.Inward.IntLocOther[i]);
                                        //}
                                        //else
                                        //{
                                        parameter[14] = new SqlParameter("@IntLocVou", "0");
                                        //}
                                        parameter[15] = new SqlParameter("@IntSupCoilNo", "");
                                        parameter[16] = new SqlParameter("@IntHeatNo", "");
                                        parameter[17] = new SqlParameter("@IntOD", "0");
                                        parameter[18] = new SqlParameter("@IntLenght", "0");
                                        parameter[10] = new SqlParameter("@IntFinish", "0");
                                        parameter[20] = new SqlParameter("@IntProcess", "0");
                                        parameter[21] = new SqlParameter("@IntBillNo", inwardModel.Inward.BillNoOther[i]);
                                        parameter[22] = new SqlParameter("@IntCoilPrefix", "");
                                        parameter[23] = new SqlParameter("@IntCoilNo", "");
                                        parameter[24] = new SqlParameter("@IntSufix", "");
                                        parameter[25] = new SqlParameter("@IntCoilTypeVou", 0);
                                        DataTable DtInwTrn = ObjDBConnection.CallStoreProcedure("InwardTrn_Insert", parameter);
                                    }
                                }
                                int Status = DbConnection.ParseInt32(DtInwMst.Rows[0][0].ToString());
                                if (Status == 0)
                                {
                                    inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                    inwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                    inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                    inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                    inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                    inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                                    if (inwardModel.Inward == null)
                                        inwardModel.Inward = new InwardGridModel();

                                    inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                    inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                                    inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
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
                                        if (inwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = inwardModel.isPrint;
                                        }
                                    }
                                    else
                                    {
                                        SetSuccessMessage("Inserted Sucessfully");
                                        if (inwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = inwardModel.isPrint;
                                        }
                                    }
                                    if (inwardModel.isPrint == 0)
                                        return RedirectToAction("Index", "Inward", new { id = 0 });
                                }
                            }
                            else
                            {
                                inwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                inwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();
                                if (inwardModel.Inward == null)
                                    inwardModel.Inward = new InwardGridModel();

                                inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                                inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
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
                            inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                            inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                            inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                            inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                            if (inwardModel.Inward == null)
                                inwardModel.Inward = new InwardGridModel();

                            inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                            inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                            inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                            inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                            inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
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
                        inwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                        inwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        inwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                        inwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                        inwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                        if (inwardModel.Inward == null)
                            inwardModel.Inward = new InwardGridModel();

                        inwardModel.Inward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        inwardModel.Inward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        inwardModel.Inward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                        inwardModel.Inward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                        inwardModel.Inward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        inwardModel.Inward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                        inwardModel.Inward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator,"");
                        inwardModel.Inward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                        inwardModel.InwDt = Convert.ToDateTime(inwardModel.InwDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(inwardModel);
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index");
        }

        public InwardModel GetOrderListByAccount(string InwPOMVou)
        {
            InwardModel obj = new InwardModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(InwPOMVou))
                {
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@OrmVou", InwPOMVou);
                    sqlParameters[1] = new SqlParameter("@Type", "PORD");
                    sqlParameters[2] = new SqlParameter("@Flg", 2);
                    sqlParameters[3] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtInwMst = ObjDBConnection.CallStoreProcedure("GetPurchaseOrderDetails", sqlParameters);
                    if (DtInwMst != null && DtInwMst.Rows.Count > 0)
                    {
                        DataRow[] drFind = DtInwMst.Select("OrmVou=" + InwPOMVou);
                        if (drFind != null && drFind.Length > 0)
                        {
                            DataTable dtUniq = drFind.CopyToDataTable();
                            if (dtUniq != null && dtUniq.Rows.Count > 0)
                            {
                                obj.InwAccVou = Convert.ToInt32(dtUniq.Rows[0]["OrmAccVou"].ToString());
                                obj.CtyName = dtUniq.Rows[0]["CtyName"].ToString();
                                obj.InwPtyVou = Convert.ToInt32(dtUniq.Rows[0]["OrmPtyVou"].ToString());
                                obj.InwPrdTyp = dtUniq.Rows[0]["OrmPtyNm"].ToString();
                                var prdType = dtUniq.Rows[0]["OrmPtyNm"].ToString();
                                obj.InwardList = new List<InwardGridModel>();
                                for (int i = 0; i < dtUniq.Rows.Count; i++)
                                {
                                    if (prdType == "COIL" || prdType == "Coil")
                                    {
                                        obj.InwardList.Add(new InwardGridModel()
                                        {
                                            IntGrdCoilStr = dtUniq.Rows[i]["OrdGrade"].ToString(),
                                            IntThickCoilStr = dtUniq.Rows[i]["OrdThick"].ToString(),
                                            IntWidthStr = dtUniq.Rows[i]["OrdWidth"].ToString(),
                                            IntQtyCoilStr = dtUniq.Rows[i]["OrdQty"].ToString(),
                                            IntRemksCoilStr = dtUniq.Rows[i]["OrdRem"].ToString()
                                        });
                                    }
                                    else if (prdType == "PIPE" || prdType == "Pipe" || prdType == "SEAMLESS PIPE" || prdType == "seamless pipe")
                                    {
                                        obj.InwardList.Add(new InwardGridModel()
                                        {
                                            SpecifiNameStr = dtUniq.Rows[i]["OrdSpacifi"].ToString(),
                                            IntGrdPipeStr = dtUniq.Rows[i]["OrdGrade"].ToString(),
                                            IntThickPipeStr = dtUniq.Rows[i]["OrdThick"].ToString(),
                                            IntODStr = dtUniq.Rows[i]["OrdOD"].ToString(),
                                            IntPcsStr = dtUniq.Rows[i]["OrdPcs"].ToString(),
                                            IntRemksPipeStr = dtUniq.Rows[i]["OrdRem"].ToString()
                                        });

                                    }
                                    else
                                    {
                                        obj.InwardList.Add(new InwardGridModel()
                                        {
                                            IntPrdVouStr = dtUniq.Rows[i]["OrdPrdVou"].ToString(),
                                            IntQtyOtherStr = dtUniq.Rows[i]["OrdQty"].ToString(),
                                            IntRemksOtherStr = dtUniq.Rows[i]["OrdRem"].ToString(),
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return obj;
        }

        public InwardModel GetLastVoucherNo(int companyId)
        {
            InwardModel obj = new InwardModel();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@Cmpvou", companyId);
                DataTable dtNewInwVNo = ObjDBConnection.CallStoreProcedure("GetLatestInwVNo", sqlParameters);
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
                InwardModel inwardModel = new InwardModel();
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
                    DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetInwardDetails", sqlParameters);
                    if (DtInw != null && DtInw.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtInw.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Inward Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "Inward");
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
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@SupCoil", supcoilno);
                    sqlParameters[1] = new SqlParameter("@Type", "INW");
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
                    string currentURL = "/Inward/Index";
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
                    getReportDataModel.ControllerName = "Inward";
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
                    var bytes = Excel(getReportDataModel, "Inward Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Inward.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Inward Report", companyDetails.CmpName, companyId.ToString());
                    return File(
                          bytes,
                          "application/pdf",
                          "Inward.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult GetAccountList()
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                var accountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                return Json(new { result = true, data = accountList });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult GetProductList(string prdtype)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                var mainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, prdtype);
                return Json(new { result = true, data = mainProductList });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult GetOrderDetail(int inwcmpvou)
        {
            try
            {
                var purchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(inwcmpvou);
                return Json(new { result = true, data = purchaseOrderList });
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
                sqlParameters[2] = new SqlParameter("@Type", "INW");
                sqlParameters[3] = new SqlParameter("@Vou", srno1);
                sqlParameters[4] = new SqlParameter("@Flg", "0");
                sqlParameters[5] = new SqlParameter("@MainVou", inwvou);
                DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetLotMstDetails1", sqlParameters);
                if (DtInw != null && DtInw.Columns.Count > 1)
                {
                    if (DtInw.Rows.Count > 0)
                    {
                        string GdnVou = DtInw.Rows[0]["LotGdnVou"].ToString();
                        string Grade = DtInw.Rows[0]["LotGrade"].ToString();
                        string GrdVou = DtInw.Rows[0]["LotGrdMscVou"].ToString();
                        string Width = DtInw.Rows[0]["LotWidth"].ToString();
                        string Thick = DtInw.Rows[0]["LotThick"].ToString();
                        string Qty = DtInw.Rows[0]["LotQty"].ToString();
                        string HeatNo = DtInw.Rows[0]["LotHeatNo"].ToString();
                        return Json(new { result = true, gdnVou = GdnVou, grade = Grade, grdVou = GrdVou, width = Width, thick = Thick, qty = Qty, heatNo = HeatNo });

                    }
                    else
                    {
                        return Json(new { result = true, data = "1" });
                    }
                }
                else
                {
                    int status = DbConnection.ParseInt32(DtInw.Rows[0][0].ToString());
                    if (status == 1)
                    {
                        return Json(new { result = true, data = "1" });
                    }
                    else
                    {
                        return Json(new { result = true, data = "-1" });
                    }
                }
                //if (DtInw != null && DtInw.Columns.Count > 1)
                //{
                //    return Json(new { result = true, data = "1" });
                //}
                //else
                //{
                //    return Json(new { result = true, data = "-1" });
                //}
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult InwardPrintDetials(long id, int copyType = 1)
        {
            InwardPrintDetails obj = new InwardPrintDetails();

            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                long userId = GetIntSession("UserId");


                //SqlParameter[] Param = new SqlParameter[1];
                //Param[0] = new SqlParameter("@InwVou", id);
                //DataTable DtLabel = ObjDBConnection.CallStoreProcedure("Insert_InwardLabel", Param);

                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@InwVou", id);
                sqlParameters[1] = new SqlParameter("@Flg", 4);
                sqlParameters[2] = new SqlParameter("@cmpvou", companyId);
                DataTable DtBilty = ObjDBConnection.CallStoreProcedure("GetInwardDetails", sqlParameters);
                if (DtBilty != null && DtBilty.Rows.Count > 0)
                {
                    string path = _iwebhostenviroment.WebRootPath + "/Label";
                    string body = string.Empty;
                    string filename = "";
                    //using streamreader for reading my htmltemplate   
                    string CmpCode = string.Empty;
                    string CmpName = string.Empty;
                    string CmpVou = string.Empty;
                    string Layout = string.Empty;
                    //if (DtBilty.Columns.Contains("DepCode"))
                    //    CmpCode = DtBilty.Rows[0]["DepCode"].ToString();
                    //if (DtBilty.Columns.Contains("DepName"))
                    //    CmpName = DtBilty.Rows[0]["DepName"].ToString();
                    //if (DtBilty.Columns.Contains("LotCmpVou"))
                    //    CmpVou = DtBilty.Rows[0]["LotCmpVou"].ToString();

                    Layout = "Labelindex";
                    filename = "Labelindex.html";

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        for (int i = 0; i < DtBilty.Rows.Count; i++)
                        {
                            string BilDate = DateTime.Parse(DtBilty.Rows[0]["InwDt"].ToString()).ToString("dd-MM-yyyy");
                            string newbody = body.Replace("#*#*r1*#*#", BilDate);
                            newbody = body.Replace("#*#*r2*#*#", DtBilty.Rows[0]["LotCoilNo"].ToString());
                            newbody = newbody.Replace("#*#*r1*#*#", BilDate);
                            newbody = newbody.Replace("#*#*r3*#*#", DtBilty.Rows[0]["AccNm"].ToString());
                            newbody = newbody.Replace("#*#*r4*#*#", DtBilty.Rows[0]["InwVehNo"].ToString());
                            newbody = newbody.Replace("#*#*r5*#*#", DtBilty.Rows[0]["LotSupCoilNo"].ToString());
                            newbody = newbody.Replace("#*#*r6*#*#", DtBilty.Rows[0]["LotGrade"].ToString());
                            newbody = newbody.Replace("#*#*r7*#*#", (Math.Round(decimal.Parse((DtBilty.Rows[0]["LotWidth"].ToString())))).ToString("0.00"));
                            newbody = newbody.Replace("#*#*r8*#*#", (Math.Round(decimal.Parse((DtBilty.Rows[0]["LotThick"].ToString())))).ToString("0.00"));
                            newbody = newbody.Replace("#*#*r9*#*#", (Math.Round(decimal.Parse((DtBilty.Rows[0]["LotQty"].ToString())))).ToString());
                            newbody = newbody.Replace("#*#*r10*#*#", DtBilty.Rows[0]["LotHeatNo"].ToString());
                            newbody = newbody.Replace("#*#*r11*#*#", DtBilty.Rows[0]["InwBillNo"].ToString());
                            newbody = newbody.Replace("#*#*r12*#*#", !string.IsNullOrWhiteSpace(DtBilty.Rows[0]["DepLogo"].ToString()) ? "<img src='/Uploads/" + DtBilty.Rows[0]["DepLogo"].ToString() + "' style='max-width:100 %;max-height: 100px; ' />" : string.Empty);

                            obj.Html = newbody;
                            obj.Id = id.ToString();
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

        [Route("/Inward/GetAccount-List")]
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

        [Route("/Inward/GetTransport-List")]
        public IActionResult TransportList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var superList = new List<CustomDropDown>();
            superList.Add(CommonHelpers.GetDefaultValue());
            superList.AddRange(ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3));

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                superList = superList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }

            return Json(new { items = CommonHelpers.BindSelect2Model(superList) });
        }
        public IActionResult RawInwardPrintDetials(long id, int companyid, int copyType = 1)
        {
            try
            {
                InwardPrintDetails obj = GetDetailsById(id);

                return View(obj);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult InwardPrintDetials(long id)
        {
            try
            {
                InwardPrintDetails obj = GetDetailsById(id);

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

        public IActionResult GetNewCoilNo()
        {
            try
            {
                string CoilNo = "";
                SqlParameter[] sqlParameters = new SqlParameter[0];
                //sqlParameters[0] = new SqlParameter("@InwVou", id);
                DataTable DtInwMst = ObjDBConnection.CallStoreProcedure("GetNewCoilNoForInward", sqlParameters);
                if (DtInwMst != null && DtInwMst.Rows.Count > 0)
                {
                    CoilNo = DtInwMst.Rows[0]["NewLotNo"].ToString();
                }
                return Json(new { data = CoilNo });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult InwardSendMail(long id, string email = "")
        {
            try
            {
                InwardPrintDetails obj = GetDetailsById(id);
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

                bool result = SendEmail(email, "INWARD REPORT", "Please find attachment", wwwroot);
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
        public IActionResult InwardWhatApp(long id, string whatappNo = "")
        {
            try
            {
                InwardPrintDetails obj = GetDetailsById(id);
                string wwwroot = string.Empty;
                string filenm = string.Empty;
                string dateTime = DateTime.Now.ToString("ddMMyyyhhmmss");

                wwwroot = _iwebhostenviroment.WebRootPath + "/PrintPDF/" + dateTime + ".pdf";
                filenm = dateTime + ".pdf";
                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(obj.Html);
                doc.Margins.Left = 25;
                doc.Save(wwwroot);
                doc.Close();

                WhatAppAPIResponse apiResponse = SendWhatAppMessage(whatappNo, "INWARD REPORT", wwwroot, filenm);
                return Json(new { result = apiResponse.status, message = apiResponse.message });
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public InwardPrintDetails GetDetailsById(long Id)
        {
            InwardPrintDetails obj = new InwardPrintDetails();

            try
            {
                StringBuilder sb = new StringBuilder();
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@InwVou", Id);
                sqlParameters[1] = new SqlParameter("@Flg", 5);
                sqlParameters[2] = new SqlParameter("@cmpvou", companyId);
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
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black; width:5px;\" rowspan=\"2\">SR<br />NO.</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;\" rowspan=\"2\">COIL No.</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;\" rowspan=\"2\">HEAT No.</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;\" colspan=\"3\">AS PER INVOICE</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;\" colspan=\"3\">ACTUAL</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;\" rowspan=\"2\">REMARKS</th>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;\">WIDTH</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;\">THK</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;\">WEIGHT</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;width:9%;\">WIDTH</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;\">THK</th>");
                        sb.Append("<th align=\"center\" style=\"font-family:Verdana;font-size:15px;color:black;\">WEIGHT</th>");
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
                        obj.Id = Id.ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return obj;
        }

    }
}