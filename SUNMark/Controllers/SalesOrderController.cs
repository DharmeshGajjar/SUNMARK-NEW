﻿using Microsoft.AspNetCore.Hosting;
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
    public class SalesOrderController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        OrderHelper ObjOrderHelper = new OrderHelper();

        private readonly IWebHostEnvironment hostingEnvironment;

        public SalesOrderController(IWebHostEnvironment _hostingEnvironment)
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
                PurchaseOrderModel purchaseOrderModel = new PurchaseOrderModel();
                purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();
                purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                purchaseOrderModel.PurchaseOrder.MeasureList = new List<SelectListItem>();
                purchaseOrderModel.PurchaseOrder.EndList = new List<SelectListItem>();
                purchaseOrderModel.PurchaseOrder.PipeList = new List<SelectListItem>();
                purchaseOrderModel.PurchaseOrder.MeasureList.Add(new SelectListItem
                {
                    Text = "FOOT",
                    Value = "FOOT",
                });
                purchaseOrderModel.PurchaseOrder.MeasureList.Add(new SelectListItem
                {
                    Text = "METER",
                    Value = "METER",
                });
                purchaseOrderModel.PurchaseOrder.MeasureList.Add(new SelectListItem
                {
                    Text = "WEIGHT",
                    Value = "WEIGHT",
                });
                purchaseOrderModel.PurchaseOrder.MeasureList.Add(new SelectListItem
                {
                    Text = "PCS",
                    Value = "PCS",
                });

                purchaseOrderModel.PurchaseOrder.EndList.Add(new SelectListItem
                {
                    Text = "PE",
                    Value = "PE",
                });
                purchaseOrderModel.PurchaseOrder.EndList.Add(new SelectListItem
                {
                    Text = "BE",
                    Value = "BE",
                });

                purchaseOrderModel.PurchaseOrder.PipeList.Add(new SelectListItem
                {
                    Text = "Seemless",
                    Value = "Seemless",
                });
                purchaseOrderModel.PurchaseOrder.PipeList.Add(new SelectListItem
                {
                    Text = "Weided",
                    Value = "Weided",
                });

                List<SelectListItem> odList = new List<SelectListItem>();
                DataTable dtNBSCH = ObjDBConnection.CallStoreProcedure("GetODDetails", null);
                if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
                {
                    foreach (DataRow item in dtNBSCH.Rows)
                    {
                        odList.Add(new SelectListItem
                        {
                            Text = item[0].ToString(),
                            Value = item[0].ToString()
                        });
                    }
                }
                ViewBag.odList = odList;
                ViewBag.finishList =  objProductHelper.GetFinishMasterDropdown(companyId, 0);
                ViewBag.nbDropdown = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
                ViewBag.schDropdown = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);
                purchaseOrderModel.PurchaseOrder.RatePerList = new List<SelectListItem>();
                purchaseOrderModel.PurchaseOrder.RatePerList = purchaseOrderModel.PurchaseOrder.MeasureList;
                purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                SqlParameter[] sqlParam = new SqlParameter[4];
                sqlParam[0] = new SqlParameter("@OrmVou", id);
                sqlParam[1] = new SqlParameter("@Type", "SORD");
                sqlParam[2] = new SqlParameter("@Flg", 3);
                sqlParam[3] = new SqlParameter("@CmpVou", companyId);
                DataTable DtOrdMst = ObjDBConnection.CallStoreProcedure("GetPurchaseOrderDetails", sqlParam);
                if (DtOrdMst != null && DtOrdMst.Rows.Count > 0)
                {
                    purchaseOrderModel.OrmVchVou = Convert.ToInt32(DtOrdMst.Rows[0]["OrmVchVou"].ToString());
                    purchaseOrderModel.VchType = DtOrdMst.Rows[0]["OrmVchTyp"].ToString();
                    purchaseOrderModel.OrmCmpVou = Convert.ToInt32(DtOrdMst.Rows[0]["OrmCmpVou"].ToString());
                    purchaseOrderModel.OrmPtyVou = Convert.ToInt32(DtOrdMst.Rows[0]["OrmPtyVou"].ToString());
                    purchaseOrderModel.OrmLastWONo = DtOrdMst.Rows[0]["OrmWONo"].ToString();
                    purchaseOrderModel.PtyName = DtOrdMst.Rows[0]["OrmPtyNm"].ToString();
                    purchaseOrderModel.OrmDt = !string.IsNullOrWhiteSpace(DtOrdMst.Rows[0]["OrmDt"].ToString()) ? Convert.ToDateTime(DtOrdMst.Rows[0]["OrmDt"].ToString()).ToString("yyyy-MM-dd") : "";
                    int vno = DbConnection.ParseInt32(DtOrdMst.Rows[0][0].ToString());
                    purchaseOrderModel.OrmVNo = vno + 1;
                }

                if (id > 0)
                {
                    purchaseOrderModel.OrmVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@OrmVou", id);
                    sqlParameters[1] = new SqlParameter("@Type", "SORD");
                    sqlParameters[2] = new SqlParameter("@Flg", 2);
                    sqlParameters[3] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtOrd = ObjDBConnection.CallStoreProcedure("GetPurchaseOrderDetails", sqlParameters);
                    if (DtOrd != null && DtOrd.Rows.Count > 0)
                    {
                        purchaseOrderModel.OrmVNo = Convert.ToInt32(DtOrd.Rows[0]["OrmVNo"].ToString());
                        purchaseOrderModel.OrmVchVou = Convert.ToInt32(DtOrd.Rows[0]["OrmVchVou"].ToString());
                        purchaseOrderModel.VchType = DtOrd.Rows[0]["OrmVchTyp"].ToString();
                        purchaseOrderModel.OrmDt = !string.IsNullOrWhiteSpace(DtOrd.Rows[0]["OrmDt"].ToString()) ? Convert.ToDateTime(DtOrd.Rows[0]["OrmDt"].ToString()).ToString("yyyy-MM-dd") : "";
                        purchaseOrderModel.PODateString = !string.IsNullOrWhiteSpace(DtOrd.Rows[0]["OrdPoDt"].ToString()) ? Convert.ToDateTime(DtOrd.Rows[0]["OrdPoDt"].ToString()).ToString("yyyy-MM-dd") : "";
                        purchaseOrderModel.OrmAccVou = Convert.ToInt32(DtOrd.Rows[0]["OrmAccVou"].ToString());
                        purchaseOrderModel.OrmSalesPersonVou = Convert.ToInt32(DtOrd.Rows[0]["OrdSalesVou"].ToString());
                        purchaseOrderModel.OrmRefNo = DtOrd.Rows[0]["OrmRefNo"].ToString();
                        purchaseOrderModel.CtyName = DtOrd.Rows[0]["CtyName"].ToString();
                        purchaseOrderModel.OrmDueDt = !string.IsNullOrWhiteSpace(DtOrd.Rows[0]["OrmDueDt"].ToString()) ? Convert.ToDateTime(DtOrd.Rows[0]["OrmDueDt"].ToString()).ToString("yyyy-MM-dd") : "";
                        purchaseOrderModel.OrmPtyVou = Convert.ToInt32(DtOrd.Rows[0]["OrmPtyVou"].ToString());
                        purchaseOrderModel.PtyName = DtOrd.Rows[0]["OrmPtyNm"].ToString();
                        purchaseOrderModel.OrmRem = DtOrd.Rows[0]["OrmRem"].ToString();
                        purchaseOrderModel.OrmWONo = DtOrd.Rows[0]["OrmWONo"].ToString();
                        purchaseOrderModel.OrmPONo = DtOrd.Rows[0]["OrmPONo"].ToString();
                        purchaseOrderModel.OrmCmpVou = Convert.ToInt32(DtOrd.Rows[0]["OrmCmpVou"].ToString());
                        purchaseOrderModel.OrdShipTo = DtOrd.Rows[0]["OrdShipTo"].ToString();
                        purchaseOrderModel.OrdQtyIn = DtOrd.Rows[0]["OrdQtyIn"].ToString();

                        var ptynm = DtOrd.Rows[0]["OrmPtyNm"].ToString();
                        if (ptynm == "COIL" || ptynm == "Coil")
                        {
                            purchaseOrderModel.PurchaseOrder.OrdGrdCoil = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdThickCoil = new decimal[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdWidth = new decimal[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdQtyCoil = new decimal[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdRemksCoil = new string[DtOrd.Rows.Count];
                        }
                        else if (ptynm == "PIPE" || ptynm == "Pipe")
                        {
                            purchaseOrderModel.PurchaseOrder.SpecifiName = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdGrdPipe = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdThickPipe = new decimal[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdActThickPipe = new decimal[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdOD = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.Length = new decimal[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.Quantity = new decimal[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdPcs = new decimal[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdRate = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.RatePer = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.End = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.Pipe = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.SupSrNo = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.SupProduct = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.ItemCode = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdRemksPipe = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdFinish = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdWeight = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdNB = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdSCH = new string[DtOrd.Rows.Count];
                        }
                        else
                        {
                            purchaseOrderModel.PurchaseOrder.OrdPrdVou = new string[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdQtyOther = new decimal[DtOrd.Rows.Count];
                            purchaseOrderModel.PurchaseOrder.OrdRemksOther = new string[DtOrd.Rows.Count];
                        }
                        for (int i = 0; i < DtOrd.Rows.Count; i++)
                        {
                            if (ptynm == "COIL" || ptynm == "Coil")
                            {
                                purchaseOrderModel.PurchaseOrder.OrdGrdCoil[i] = DtOrd.Rows[i]["OrdGrade"].ToString();
                                purchaseOrderModel.PurchaseOrder.OrdThickCoil[i] = Convert.ToDecimal(DtOrd.Rows[i]["OrdThick"].ToString());
                                purchaseOrderModel.PurchaseOrder.OrdWidth[i] = Convert.ToDecimal(DtOrd.Rows[i]["OrdWidth"].ToString());
                                purchaseOrderModel.PurchaseOrder.OrdQtyCoil[i] = Convert.ToDecimal(DtOrd.Rows[i]["OrdQty"].ToString());
                                purchaseOrderModel.PurchaseOrder.OrdRemksCoil[i] = DtOrd.Rows[i]["OrdRem"].ToString();
                            }
                            else if (ptynm == "PIPE" || ptynm == "Pipe")
                            {
                                purchaseOrderModel.PurchaseOrder.SpecifiName[i] = DtOrd.Rows[i]["OrdSpacifi"].ToString();
                                purchaseOrderModel.PurchaseOrder.OrdGrdPipe[i] = DtOrd.Rows[i]["OrdGrade"].ToString();
                                purchaseOrderModel.PurchaseOrder.OrdThickPipe[i] = Convert.ToDecimal(DtOrd.Rows[i]["OrdThick"].ToString());
                                purchaseOrderModel.PurchaseOrder.OrdActThickPipe[i] = Convert.ToDecimal(DtOrd.Rows[i]["OrdActThick"].ToString());
                                purchaseOrderModel.PurchaseOrder.OrdOD[i] = Convert.ToString(DtOrd.Rows[i]["OrdOD"].ToString());
                                purchaseOrderModel.PurchaseOrder.Length[i] = Convert.ToDecimal(DtOrd.Rows[i]["OrdFeetPer"].ToString());
                                purchaseOrderModel.PurchaseOrder.Quantity[i] = Convert.ToDecimal(DtOrd.Rows[i]["OrdFoot"].ToString());
                                purchaseOrderModel.PurchaseOrder.OrdPcs[i] = Convert.ToDecimal(DtOrd.Rows[i]["OrdPcs"].ToString());
                                purchaseOrderModel.PurchaseOrder.OrdRate[i] = DtOrd.Rows[i]["OrdRt"].ToString();
                                purchaseOrderModel.PurchaseOrder.RatePer[i] = DtOrd.Rows[i]["OrdRtPer"].ToString();
                                purchaseOrderModel.PurchaseOrder.End[i] = DtOrd.Rows[i]["OrdEnd"].ToString();
                                purchaseOrderModel.PurchaseOrder.Pipe[i] = DtOrd.Rows[i]["OrdPipe"].ToString();
                                purchaseOrderModel.PurchaseOrder.SupSrNo[i] = DtOrd.Rows[i]["OrdSupSrNo"].ToString();
                                purchaseOrderModel.PurchaseOrder.SupProduct[i] = DtOrd.Rows[i]["OrdSupPrdDesc"].ToString();
                                purchaseOrderModel.PurchaseOrder.ItemCode[i] = DtOrd.Rows[i]["OrdSupPrdCd"].ToString();
                                purchaseOrderModel.PurchaseOrder.OrdRemksPipe[i] = DtOrd.Rows[i]["OrdRem"].ToString();
                                purchaseOrderModel.PurchaseOrder.OrdFinish[i] = DtOrd.Rows[i]["OrdFinish"].ToString();
                                purchaseOrderModel.PurchaseOrder.OrdWeight[i] = DtOrd.Rows[i]["OrdWeight"].ToString();
                                purchaseOrderModel.PurchaseOrder.OrdNB[i] = DtOrd.Rows[i]["OrdNB"].ToString();
                                purchaseOrderModel.PurchaseOrder.OrdSCH[i] = DtOrd.Rows[i]["OrdSCH"].ToString();
                            }
                            else
                            {
                                purchaseOrderModel.PurchaseOrder.OrdPrdVou[i] = DtOrd.Rows[i]["OrdPrdVou"].ToString();
                                purchaseOrderModel.PurchaseOrder.OrdQtyOther[i] = Convert.ToDecimal(DtOrd.Rows[i]["OrdQty"].ToString());
                                purchaseOrderModel.PurchaseOrder.OrdRemksOther[i] = DtOrd.Rows[i]["OrdRem"].ToString();
                            }

                        }

                    }
                }
                return View(purchaseOrderModel);
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
        public IActionResult Index(long id, PurchaseOrderModel purchaseOrderModel)
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
                purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                if (purchaseOrderModel.PurchaseOrder == null)
                    purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                //if (id == 0)
                //{
                //    purchaseOrderModel.OrmVNo = DbConnection.ParseInt32(ObjDBConnection.GetLatestVoucherNumber("OrdMst", Convert.ToInt32(id), companyId, 0));

                //}
                var prdtype = purchaseOrderModel.PtyName;
                if (prdtype == "COIL" || prdtype == "Coil")
                {
                    if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(purchaseOrderModel.OrmVNo).ToString()) && !string.IsNullOrWhiteSpace(purchaseOrderModel.OrmDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(purchaseOrderModel.OrmAccVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(purchaseOrderModel.OrmPtyVou).ToString()) && purchaseOrderModel.PurchaseOrder.OrdGrdCoil.Length > 0 && purchaseOrderModel.PurchaseOrder.OrdThickCoil.Length > 0 && purchaseOrderModel.PurchaseOrder.OrdWidth.Length > 0 && purchaseOrderModel.PurchaseOrder.OrdQtyCoil.Length > 0)
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[21];
                        sqlParameters[0] = new SqlParameter("@OrmVchTyp", purchaseOrderModel.VchType);
                        sqlParameters[1] = new SqlParameter("@OrmVchVou", purchaseOrderModel.OrmVchVou);
                        sqlParameters[2] = new SqlParameter("@OrmVNo", purchaseOrderModel.OrmVNo);
                        sqlParameters[3] = new SqlParameter("@OrmDt", purchaseOrderModel.OrmDt);
                        sqlParameters[4] = new SqlParameter("@OrmRefNo", purchaseOrderModel.OrmRefNo);
                        sqlParameters[5] = new SqlParameter("@OrmAccVou", purchaseOrderModel.OrmAccVou);
                        sqlParameters[6] = new SqlParameter("@OrmDueDt", purchaseOrderModel.OrmDueDt);
                        sqlParameters[7] = new SqlParameter("@OrmPtyVou", purchaseOrderModel.OrmPtyVou);
                        sqlParameters[8] = new SqlParameter("@OrmPtyNm", purchaseOrderModel.PtyName);
                        sqlParameters[9] = new SqlParameter("@OrmRem", purchaseOrderModel.OrmRem);
                        sqlParameters[10] = new SqlParameter("@OrmWONo", purchaseOrderModel.OrmWONo);
                        sqlParameters[11] = new SqlParameter("@OrmPONo", purchaseOrderModel.OrmPONo);
                        sqlParameters[12] = new SqlParameter("@OrmVou", id);
                        sqlParameters[13] = new SqlParameter("@UsrVou", userId);
                        sqlParameters[14] = new SqlParameter("@FLG", 1);
                        sqlParameters[15] = new SqlParameter("@CmpVou", purchaseOrderModel.OrmCmpVou);
                        sqlParameters[16] = new SqlParameter("@OrdType", "SORD");
                        sqlParameters[17] = new SqlParameter("@OrdPoDt", purchaseOrderModel.PODate);
                        sqlParameters[18] = new SqlParameter("@OrdSalesVou", purchaseOrderModel. OrmSalesPersonVou);
                        sqlParameters[19] = new SqlParameter("@OrdQtyIn", purchaseOrderModel.OrdQtyIn);
                        sqlParameters[20] = new SqlParameter("@OrdShipTo", purchaseOrderModel.OrdShipTo);

                        DataTable DtOrdMst = ObjDBConnection.CallStoreProcedure("PurOrderMst_Insert", sqlParameters);
                        if (DtOrdMst != null && DtOrdMst.Rows.Count > 0)
                        {
                            int masterId = DbConnection.ParseInt32(DtOrdMst.Rows[0][0].ToString());

                            //int masterid = ObjOrderHelper.InsertOrderMst(purchaseOrderModel, Convert.ToInt32(id), Convert.ToInt32(userId));
                            if (masterId > 0)
                            {
                                for (int i = 0; i < purchaseOrderModel.PurchaseOrder.OrdGrdCoil.Length; i++)
                                {
                                    SqlParameter[] parameter = new SqlParameter[28];
                                    parameter[0] = new SqlParameter("@OrdOrmVou", masterId);
                                    parameter[1] = new SqlParameter("@OrdSrNo", (i + 1));
                                    parameter[2] = new SqlParameter("@OrdPrdVou", "0");
                                    parameter[3] = new SqlParameter("@OrdGrade", purchaseOrderModel.PurchaseOrder.OrdGrdCoil[i]);
                                    parameter[4] = new SqlParameter("@OrdSpacifi", "");
                                    parameter[5] = new SqlParameter("@OrdWidth", purchaseOrderModel.PurchaseOrder.OrdWidth[i]);
                                    parameter[6] = new SqlParameter("@OrdThick", purchaseOrderModel.PurchaseOrder.OrdThickCoil[i]);
                                    parameter[7] = new SqlParameter("@OrdActThick", "0");
                                    parameter[8] = new SqlParameter("@OrdQty", purchaseOrderModel.PurchaseOrder.OrdQtyCoil[i]);
                                    parameter[9] = new SqlParameter("@OrdPenQty", purchaseOrderModel.PurchaseOrder.OrdQtyCoil[i]);
                                    parameter[10] = new SqlParameter("@OrdOD", "0");
                                    parameter[11] = new SqlParameter("@OrdPcs", "0");
                                    parameter[12] = new SqlParameter("@OrdFoot", "0");
                                    parameter[13] = new SqlParameter("@OrdPenPcs", "0");
                                    parameter[14] = new SqlParameter("@OrdFeetPer", "0");
                                    if (purchaseOrderModel.PurchaseOrder.OrdRemksCoil[i] == "-1")
                                    {
                                        parameter[15] = new SqlParameter("@OrdRem", "");
                                    }
                                    else
                                    {
                                        parameter[15] = new SqlParameter("@OrdRem", purchaseOrderModel.PurchaseOrder.OrdRemksCoil[i]);
                                    }

                                    parameter[16] = new SqlParameter("@OrdSupSrno", "");
                                    parameter[17] = new SqlParameter("@OrdSupPrdDesc", "");
                                    parameter[18] = new SqlParameter("@OrdSupPrdCd", "");
                                    parameter[19] = new SqlParameter("@OrdMasUnit", "");
                                    parameter[20] = new SqlParameter("@OrdRtPer", "");
                                    parameter[21] = new SqlParameter("@OrdRt", "0");
                                    parameter[22] = new SqlParameter("@OrdEnd", "");
                                    parameter[23] = new SqlParameter("@OrdPipe", "");
                                    parameter[24] = new SqlParameter("@OrdFinish", "");
                                    parameter[25] = new SqlParameter("@OrdWeight", "");
                                    parameter[26] = new SqlParameter("@OrdNB", "");
                                    parameter[27] = new SqlParameter("@OrdSCH", "");
                                    DataTable DtOrdTrn = ObjDBConnection.CallStoreProcedure("PurOrderTrn_Insert", parameter);
                                }
                                int Status = DbConnection.ParseInt32(DtOrdMst.Rows[0][0].ToString());
                                if (Status == 0)
                                {
                                    purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                                    purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                                    purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                                    purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                                    if (purchaseOrderModel.PurchaseOrder == null)
                                        purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                                    purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                                    purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                                    SetErrorMessage("Dulplicate Vou.No Details");
                                    ViewBag.FocusType = "1";
                                    return View(purchaseOrderModel);
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
                                    return RedirectToAction("Index", "SalesOrder", new { id = 0 });
                                }
                            }
                            else
                            {
                                purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                                purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                                purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                                purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                                if (purchaseOrderModel.PurchaseOrder == null)
                                    purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                                purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                                purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                                SetErrorMessage("Insert error");
                                ViewBag.FocusType = "1";
                                return View(purchaseOrderModel);
                            }
                        }
                        else
                        {
                            int Status = DbConnection.ParseInt32(DtOrdMst.Rows[0][0].ToString());
                            if (Status == -1)
                            {
                                purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                                purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                                purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                                purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                                if (purchaseOrderModel.PurchaseOrder == null)
                                    purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                                purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                                purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                                SetErrorMessage("Duplicate Voucher No NOt Allowed");
                                ViewBag.FocusType = "1";
                                return View(purchaseOrderModel);

                            }
                            else if (Status == -2)
                            {
                                purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                                purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                                purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                                purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                                if (purchaseOrderModel.PurchaseOrder == null)
                                    purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                                purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                                purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                                SetErrorMessage("Duplicate Work Order NOt Allowed");
                                ViewBag.FocusType = "-2";
                                return View(purchaseOrderModel);
                            }
                        }
                    }
                    else
                    {
                        purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                        purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                        purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                        purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                        if (purchaseOrderModel.PurchaseOrder == null)
                            purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                        purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                        purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                        purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                        purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(purchaseOrderModel);
                    }
                }
                else if (prdtype == "PIPE" || prdtype == "Pipe")
                {
                    if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(purchaseOrderModel.OrmVNo).ToString()) && !string.IsNullOrWhiteSpace(purchaseOrderModel.OrmDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(purchaseOrderModel.OrmAccVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(purchaseOrderModel.OrmPtyVou).ToString()) && purchaseOrderModel.PurchaseOrder.OrdGrdPipe != null && purchaseOrderModel.PurchaseOrder.OrdGrdPipe.Length > 0 && purchaseOrderModel.PurchaseOrder.SpecifiName != null && purchaseOrderModel.PurchaseOrder.SpecifiName.Length > 0 && purchaseOrderModel.PurchaseOrder.OrdThickPipe.Length > 0 && purchaseOrderModel.PurchaseOrder.OrdOD.Length > 0 && purchaseOrderModel.PurchaseOrder.OrdPcs.Length > 0 && purchaseOrderModel.PurchaseOrder.OrdNB.Length > 0 && purchaseOrderModel.PurchaseOrder.OrdSCH.Length > 0)
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[21];
                        sqlParameters[0] = new SqlParameter("@OrmVchTyp", purchaseOrderModel.VchType);
                        sqlParameters[1] = new SqlParameter("@OrmVchVou", purchaseOrderModel.OrmVchVou);
                        sqlParameters[2] = new SqlParameter("@OrmVNo", purchaseOrderModel.OrmVNo);
                        sqlParameters[3] = new SqlParameter("@OrmDt", purchaseOrderModel.OrmDt);
                        sqlParameters[4] = new SqlParameter("@OrmRefNo", purchaseOrderModel.OrmRefNo);
                        sqlParameters[5] = new SqlParameter("@OrmAccVou", purchaseOrderModel.OrmAccVou);
                        sqlParameters[6] = new SqlParameter("@OrmDueDt", purchaseOrderModel.OrmDueDt);
                        sqlParameters[7] = new SqlParameter("@OrmPtyVou", purchaseOrderModel.OrmPtyVou);
                        sqlParameters[8] = new SqlParameter("@OrmPtyNm", purchaseOrderModel.PtyName);
                        sqlParameters[9] = new SqlParameter("@OrmRem", purchaseOrderModel.OrmRem);
                        sqlParameters[10] = new SqlParameter("@OrmWONo", purchaseOrderModel.OrmWONo);
                        sqlParameters[11] = new SqlParameter("@OrmPONo", purchaseOrderModel.OrmPONo);
                        sqlParameters[12] = new SqlParameter("@OrmVou", id);
                        sqlParameters[13] = new SqlParameter("@UsrVou", userId);
                        sqlParameters[14] = new SqlParameter("@FLG", 1);
                        sqlParameters[15] = new SqlParameter("@CmpVou", purchaseOrderModel.OrmCmpVou);
                        sqlParameters[16] = new SqlParameter("@OrdType", "SORD");
                        sqlParameters[17] = new SqlParameter("@OrdPoDt", purchaseOrderModel.PODate);
                        sqlParameters[18] = new SqlParameter("@OrdSalesVou", purchaseOrderModel.OrmSalesPersonVou);
                        sqlParameters[19] = new SqlParameter("@OrdQtyIn", purchaseOrderModel.OrdQtyIn);
                        sqlParameters[20] = new SqlParameter("@OrdShipTo", purchaseOrderModel.OrdShipTo);


                        DataTable DtOrdMst = ObjDBConnection.CallStoreProcedure("PurOrderMst_Insert", sqlParameters);
                        if (DtOrdMst != null && DtOrdMst.Rows.Count > 0)
                        {
                            int masterId = DbConnection.ParseInt32(DtOrdMst.Rows[0][0].ToString());

                            //int masterid = ObjOrderHelper.InsertOrderMst(purchaseOrderModel, Convert.ToInt32(id), Convert.ToInt32(userId));
                            if (masterId > 0)
                            {
                                for (int i = 0; i < purchaseOrderModel.PurchaseOrder.SpecifiName.Length; i++)
                                {
                                    SqlParameter[] parameter = new SqlParameter[28];
                                    parameter[0] = new SqlParameter("@OrdOrmVou", masterId);
                                    parameter[1] = new SqlParameter("@OrdSrNo", (i + 1));
                                    parameter[2] = new SqlParameter("@OrdPrdVou", "0");
                                    parameter[3] = new SqlParameter("@OrdGrade", purchaseOrderModel.PurchaseOrder.OrdGrdPipe[i]);
                                    parameter[4] = new SqlParameter("@OrdSpacifi", purchaseOrderModel.PurchaseOrder.SpecifiName[i]);
                                    parameter[5] = new SqlParameter("@OrdWidth", "0");
                                    parameter[6] = new SqlParameter("@OrdThick", purchaseOrderModel.PurchaseOrder.OrdThickPipe[i]);
                                    parameter[7] = new SqlParameter("@OrdActThick", purchaseOrderModel.PurchaseOrder.OrdActThickPipe[i]);
                                    parameter[8] = new SqlParameter("@OrdQty", "0");
                                    parameter[9] = new SqlParameter("@OrdPenQty", "0");
                                    parameter[10] = new SqlParameter("@OrdOD", purchaseOrderModel.PurchaseOrder.OrdOD[i]);
                                    parameter[11] = new SqlParameter("@OrdPcs", purchaseOrderModel.PurchaseOrder.OrdPcs[i]);
                                    parameter[12] = new SqlParameter("@OrdPenPcs", purchaseOrderModel.PurchaseOrder.OrdPcs[i]);
                                    parameter[13] = new SqlParameter("@OrdFeetPer", purchaseOrderModel.PurchaseOrder.Length[i]);
                                    parameter[14] = new SqlParameter("@OrdFoot", purchaseOrderModel.PurchaseOrder.Quantity[i]);
                                    if (purchaseOrderModel.PurchaseOrder.OrdRemksPipe[i] == "-1")
                                    {
                                        parameter[15] = new SqlParameter("@OrdRem", "");
                                    }
                                    else
                                    {
                                        parameter[15] = new SqlParameter("@OrdRem", purchaseOrderModel.PurchaseOrder.OrdRemksPipe[i]);
                                    }

                                    if (purchaseOrderModel.PurchaseOrder.OrdRate[i] == "-1")
                                    {
                                        parameter[16] = new SqlParameter("@OrdRt", "0");
                                    }
                                    else
                                    {
                                        parameter[16] = new SqlParameter("@OrdRt", purchaseOrderModel.PurchaseOrder.OrdRate[i]);
                                    }

                                    if (purchaseOrderModel.PurchaseOrder.RatePer[i] == "-1")
                                    {
                                        parameter[17] = new SqlParameter("@OrdRtPer", "0");
                                    }
                                    else
                                    {
                                        parameter[17] = new SqlParameter("@OrdRtPer", purchaseOrderModel.PurchaseOrder.RatePer[i]);
                                    }

                                    if (purchaseOrderModel.PurchaseOrder.SupSrNo[i] == "-1")
                                    {
                                        parameter[18] = new SqlParameter("@OrdSupSrno", "0");
                                    }
                                    else
                                    {
                                        parameter[18] = new SqlParameter("@OrdSupSrno", purchaseOrderModel.PurchaseOrder.SupSrNo[i]);
                                    }

                                    if (purchaseOrderModel.PurchaseOrder.SupProduct[i] == "-1")
                                    {
                                        parameter[19] = new SqlParameter("@OrdSupPrdDesc", "");
                                    }
                                    else
                                    {
                                        parameter[19] = new SqlParameter("@OrdSupPrdDesc", purchaseOrderModel.PurchaseOrder.SupProduct[i]);
                                    }

                                    if (purchaseOrderModel.PurchaseOrder.ItemCode[i] == "-1")
                                    {
                                        parameter[20] = new SqlParameter("@OrdSupPrdCd", "");
                                    }
                                    else
                                    {
                                        parameter[20] = new SqlParameter("@OrdSupPrdCd", purchaseOrderModel.PurchaseOrder.ItemCode[i]);
                                    }
                                    parameter[21] = new SqlParameter("@OrdEnd", purchaseOrderModel.PurchaseOrder.End[i]);
                                    parameter[22] = new SqlParameter("@OrdPipe", purchaseOrderModel.PurchaseOrder.Pipe[i]);
                                    parameter[23] = new SqlParameter("@OrdFinish", purchaseOrderModel.PurchaseOrder.OrdFinish[i]);
                                    parameter[24] = new SqlParameter("@OrdWeight", purchaseOrderModel.PurchaseOrder.OrdWeight[i]);
                                    parameter[25] = new SqlParameter("@OrdNB", purchaseOrderModel.PurchaseOrder.OrdNB[i]);
                                    parameter[26] = new SqlParameter("@OrdSch", purchaseOrderModel.PurchaseOrder.OrdSCH[i]);
                                    parameter[27] = new SqlParameter("@OrdMasUnit", "");
                                    DataTable DtOrdTrn = ObjDBConnection.CallStoreProcedure("PurOrderTrn_Insert", parameter);
                                }
                                int Status = DbConnection.ParseInt32(DtOrdMst.Rows[0][0].ToString());
                                if (Status == 0)
                                {
                                    purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                                    purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                                    purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                                    purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                                    if (purchaseOrderModel.PurchaseOrder == null)
                                        purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                                    purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                                    purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                                    SetErrorMessage("Dulplicate Vou.No Details");
                                    ViewBag.FocusType = "1";
                                    return View(purchaseOrderModel);
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
                                    return RedirectToAction("Index", "SalesOrder", new { id = 0 });
                                }
                            }
                            else
                            {
                                int Status = DbConnection.ParseInt32(DtOrdMst.Rows[0][0].ToString());
                                if (Status == -1)
                                {
                                    purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                                    purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                                    purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                                    purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                                    if (purchaseOrderModel.PurchaseOrder == null)
                                        purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                                    purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                                    purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                                    SetErrorMessage("Duplicate Voucher No NOt Allowed");
                                    ViewBag.FocusType = "1";
                                    return View(purchaseOrderModel);

                                }
                                else if (Status == -2)
                                {
                                    purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                                    purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                                    purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                                    purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                                    if (purchaseOrderModel.PurchaseOrder == null)
                                        purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                                    purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                                    purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                                    SetErrorMessage("Duplicate Work Order NOt Allowed");
                                    ViewBag.FocusType = "-2";
                                    return View(purchaseOrderModel);
                                }
                            }
                        }
                        else
                        {
                            purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                            purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                            purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                            purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                            if (purchaseOrderModel.PurchaseOrder == null)
                                purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                            purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                            purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                            purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                            purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                            SetErrorMessage("Please Enter the Value");
                            ViewBag.FocusType = "1";
                            return View(purchaseOrderModel);
                        }
                    }
                    else
                    {
                        purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                        purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                        purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                        purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                        if (purchaseOrderModel.PurchaseOrder == null)
                            purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                        purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                        purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                        purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                        purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(purchaseOrderModel);

                    }

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(purchaseOrderModel.OrmVNo).ToString()) && !string.IsNullOrWhiteSpace(purchaseOrderModel.OrmDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(purchaseOrderModel.OrmAccVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(purchaseOrderModel.OrmPtyVou).ToString()) && purchaseOrderModel.PurchaseOrder.OrdPrdVou.Length > 0 && purchaseOrderModel.PurchaseOrder.OrdQtyOther.Length > 0)
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[17];
                        sqlParameters[0] = new SqlParameter("@OrmVchTyp", purchaseOrderModel.VchType);
                        sqlParameters[1] = new SqlParameter("@OrmVchVou", purchaseOrderModel.OrmVchVou);
                        sqlParameters[2] = new SqlParameter("@OrmVNo", purchaseOrderModel.OrmVNo);
                        sqlParameters[3] = new SqlParameter("@OrmDt", purchaseOrderModel.OrmDt);
                        sqlParameters[4] = new SqlParameter("@OrmRefNo", purchaseOrderModel.OrmRefNo);
                        sqlParameters[5] = new SqlParameter("@OrmAccVou", purchaseOrderModel.OrmAccVou);
                        sqlParameters[6] = new SqlParameter("@OrmDueDt", purchaseOrderModel.OrmDueDt);
                        sqlParameters[7] = new SqlParameter("@OrmPtyVou", purchaseOrderModel.OrmPtyVou);
                        sqlParameters[8] = new SqlParameter("@OrmPtyNm", purchaseOrderModel.PtyName);
                        sqlParameters[9] = new SqlParameter("@OrmRem", purchaseOrderModel.OrmRem);
                        sqlParameters[10] = new SqlParameter("@OrmWONo", purchaseOrderModel.OrmWONo);
                        sqlParameters[11] = new SqlParameter("@OrmPONo", purchaseOrderModel.OrmPONo);
                        sqlParameters[12] = new SqlParameter("@OrmVou", id);
                        sqlParameters[13] = new SqlParameter("@UsrVou", userId);
                        sqlParameters[14] = new SqlParameter("@FLG", 1);
                        sqlParameters[15] = new SqlParameter("@CmpVou", purchaseOrderModel.OrmCmpVou);
                        sqlParameters[16] = new SqlParameter("@OrdType", "SORD");
                        DataTable DtOrdMst = ObjDBConnection.CallStoreProcedure("PurOrderMst_Insert", sqlParameters);
                        if (DtOrdMst != null && DtOrdMst.Rows.Count > 0)
                        {
                            int masterId = DbConnection.ParseInt32(DtOrdMst.Rows[0][0].ToString());

                            //int masterid = ObjOrderHelper.InsertOrderMst(purchaseOrderModel, Convert.ToInt32(id), Convert.ToInt32(userId));
                            if (masterId > 0)
                            {
                                for (int i = 0; i < purchaseOrderModel.PurchaseOrder.OrdPrdVou.Length; i++)
                                {
                                    SqlParameter[] parameter = new SqlParameter[28];
                                    parameter[0] = new SqlParameter("@OrdOrmVou", masterId);
                                    parameter[1] = new SqlParameter("@OrdSrNo", (i + 1));
                                    parameter[2] = new SqlParameter("@OrdPrdVou", purchaseOrderModel.PurchaseOrder.OrdPrdVou[i]);
                                    parameter[3] = new SqlParameter("@OrdGrade", "");
                                    parameter[4] = new SqlParameter("@OrdSpacifi", "");
                                    parameter[5] = new SqlParameter("@OrdWidth", "0");
                                    parameter[6] = new SqlParameter("@OrdThick", "0");
                                    parameter[7] = new SqlParameter("@OrdActThick", "0");
                                    parameter[8] = new SqlParameter("@OrdQty", purchaseOrderModel.PurchaseOrder.OrdQtyOther[i]);
                                    parameter[9] = new SqlParameter("@OrdPenQty", purchaseOrderModel.PurchaseOrder.OrdQtyOther[i]);
                                    parameter[10] = new SqlParameter("@OrdOD", "0");
                                    parameter[11] = new SqlParameter("@OrdPcs", "0");
                                    parameter[12] = new SqlParameter("@OrdPenPcs", "0");
                                    parameter[13] = new SqlParameter("@OrdFoot", "0");
                                    parameter[14] = new SqlParameter("@OrdFeetPer", "0");
                                    if (purchaseOrderModel.PurchaseOrder.OrdRemksOther[i] == "-1")
                                    {
                                        parameter[15] = new SqlParameter("@OrdRem", "");
                                    }
                                    else
                                    {
                                        parameter[15] = new SqlParameter("@OrdRem", purchaseOrderModel.PurchaseOrder.OrdRemksOther[i]);
                                    }
                                    parameter[16] = new SqlParameter("@OrdSupSrno", "");
                                    parameter[17] = new SqlParameter("@OrdSupPrdDesc", "");
                                    parameter[18] = new SqlParameter("@OrdSupPrdCd", "");
                                    parameter[19] = new SqlParameter("@OrdMasUnit", "");
                                    parameter[20] = new SqlParameter("@OrdRtPer", "");
                                    parameter[21] = new SqlParameter("@OrdRt", "0");
                                    parameter[22] = new SqlParameter("@OrdEnd", "");
                                    parameter[23] = new SqlParameter("@OrdPipe", "");
                                    parameter[24] = new SqlParameter("@OrdFinish", "");
                                    parameter[25] = new SqlParameter("@OrdWeight", "");
                                    parameter[26] = new SqlParameter("@OrdNB", "");
                                    parameter[27] = new SqlParameter("@OrdSCH", "");
                                    DataTable DtOrdTrn = ObjDBConnection.CallStoreProcedure("PurOrderTrn_Insert", parameter);
                                }
                                int Status = DbConnection.ParseInt32(DtOrdMst.Rows[0][0].ToString());
                                if (Status == 0)
                                {
                                    purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                                    purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                                    purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                                    if (purchaseOrderModel.PurchaseOrder == null)
                                        purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                                    purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                                    purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                                    SetErrorMessage("Dulplicate Vou.No Details");
                                    ViewBag.FocusType = "1";
                                    return View(purchaseOrderModel);
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
                                    return RedirectToAction("Index", "SalesOrder", new { id = 0 });
                                }
                            }
                            else
                            {
                                int Status = DbConnection.ParseInt32(DtOrdMst.Rows[0][0].ToString());
                                if (Status == -1)
                                {
                                    purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                                    purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                                    purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                                    purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                                    if (purchaseOrderModel.PurchaseOrder == null)
                                        purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                                    purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                                    purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                                    SetErrorMessage("Duplicate Voucher No NOt Allowed");
                                    ViewBag.FocusType = "1";
                                    return View(purchaseOrderModel);

                                }
                                else if (Status == -2)
                                {
                                    purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                                    purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                                    purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                                    purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                                    if (purchaseOrderModel.PurchaseOrder == null)
                                        purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                                    purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                                    purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                                    SetErrorMessage("Duplicate Work Order NOt Allowed");
                                    ViewBag.FocusType = "-2";
                                    return View(purchaseOrderModel);
                                }
                            }
                        }
                        else
                        {
                            purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                            purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                            purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                            purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                            if (purchaseOrderModel.PurchaseOrder == null)
                                purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                            purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                            purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                            purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                            purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                            SetErrorMessage("Please Enter the Value");
                            ViewBag.FocusType = "1";
                            return View(purchaseOrderModel);
                        }
                    }
                    else
                    {
                        purchaseOrderModel.VoucherTypeList = objProductHelper.GetVoucherTypeDropdown("SORD", companyId);
                        purchaseOrderModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                        purchaseOrderModel.SalesPersonList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 5);
                        purchaseOrderModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        purchaseOrderModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

                        if (purchaseOrderModel.PurchaseOrder == null)
                            purchaseOrderModel.PurchaseOrder = new PurchaseOrderGridModel();

                        purchaseOrderModel.PurchaseOrder.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        purchaseOrderModel.PurchaseOrder.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        purchaseOrderModel.PurchaseOrder.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                        purchaseOrderModel.PurchaseOrder.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                        purchaseOrderModel.OrmDt = Convert.ToDateTime(purchaseOrderModel.OrmDt).ToString("yyyy-MM-dd");
                        purchaseOrderModel.OrmDueDt = Convert.ToDateTime(purchaseOrderModel.OrmDueDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(purchaseOrderModel);
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index");
        }

        public PurchaseOrderModel GetLastVoucherNo(string OrmVchVou, int companyId)
        {
            PurchaseOrderModel obj = new PurchaseOrderModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(OrmVchVou) && !string.IsNullOrWhiteSpace(OrmVchVou))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@OrmVchVou", OrmVchVou);
                    sqlParameters[1] = new SqlParameter("@Cmpvou", companyId);
                    DataTable dtNewOrdVNo = ObjDBConnection.CallStoreProcedure("GetLatestOrdVNo", sqlParameters);
                    if (dtNewOrdVNo != null && dtNewOrdVNo.Rows.Count > 0)
                    {
                        int.TryParse(dtNewOrdVNo.Rows[0]["OrmVNo"].ToString(), out int OrmVNo);
                        OrmVNo = OrmVNo == 0 ? 1 : OrmVNo;
                        obj.OrmVNo = OrmVNo;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return obj;
        }

        public JsonResult GetNBSChValue(string nb, string sch)
        {
            PurchaseOrderModel obj = new PurchaseOrderModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(nb) && !string.IsNullOrWhiteSpace(nb))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@NB", nb);
                    sqlParameters[1] = new SqlParameter("@SCH", sch);
                    DataTable dtNBSCH = ObjDBConnection.CallStoreProcedure("GetOdByNBSCH", sqlParameters);
                    if (dtNBSCH != null && dtNBSCH.Rows.Count > 0)
                    {
                        return Json(new { result = true, ordOd = dtNBSCH.Rows[0][0] });
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
                MachineMasterModel machineMasterModel = new MachineMasterModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("@OrmVou", id);
                    sqlParameters[1] = new SqlParameter("@Type", "SORD");
                    sqlParameters[2] = new SqlParameter("@Flg", "1");
                    sqlParameters[3] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[4] = new SqlParameter("@pagesize", "0");
                    sqlParameters[5] = new SqlParameter("@searchvalue", "");
                    sqlParameters[6] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtOrd = ObjDBConnection.CallStoreProcedure("GetPurchaseOrderDetails", sqlParameters);
                    if (DtOrd != null && DtOrd.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtOrd.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Sales Order Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "SalesOrder");
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
                    string currentURL = "/SalesOrder/Index";
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
                    getReportDataModel.ControllerName = "SalesOrder";
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
                    var bytes = Excel(getReportDataModel, "Sales Order Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "SalesOrder.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Purchase Order Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "SalesOrder.pdf");
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
                var accountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId,0);
                return Json(new { result = true, data = accountList });
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
                        var uploads = Path.Combine(hostingEnvironment.WebRootPath, "SalesOrderExcels");
                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }

                        if (FileHelper.FileUpload(uploads, string.Concat(uploads, "/", fileName), bytes))
                        {
                            string defaultFilePath = string.Empty;
                            if (type.ToLower() == "coil")
                            {
                                dtDefault = FileHelper.ReadXLSXFile(string.Concat(hostingEnvironment.WebRootPath, "/coil.xlsx"));
                            }
                            else if (type.ToLower() == "pipe")
                            {
                                dtDefault = FileHelper.ReadXLSXFile(string.Concat(hostingEnvironment.WebRootPath, "/pipe.xlsx"));
                            }
                            else
                            {
                                dtDefault = FileHelper.ReadXLSXFile(string.Concat(hostingEnvironment.WebRootPath, "/other.xlsx"));
                            }
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
                                var specificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                DataTable dtGrade = new DataTable();
                                DataTable dtSpecification = new DataTable();
                                if (gradeList != null && gradeList.Count > 0)
                                {
                                    dtGrade = ListtoDataTableConverter.ToDataTable(gradeList);
                                }
                                if (gradeList != null && gradeList.Count > 0)
                                {
                                    dtSpecification = ListtoDataTableConverter.ToDataTable(specificationList);
                                }
                                List<SelectListItem> notFoundItemList = new List<SelectListItem>();
                                var gridList = SalesHelper.ProcessExcel(dtExcel, dtGrade, dtSpecification, companyId.ToString(), userId.ToString(), ref notFoundItemList);
                                if ((gridList != null && gridList.Count > 0) || (notFoundItemList != null && notFoundItemList.Count > 0))
                                {
                                    gradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    specificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    return Json(new { result = true, message = "Excel uploaded successfully", gridList = gridList, gradeList = gradeList, specificationList = specificationList, notFoundItemList = notFoundItemList });

                                }
                                else
                                {
                                    return Json(new { result = false, message = "Error in uploading excel" });
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

        [Route("/SalesOrder/GetAccount-List")]
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
