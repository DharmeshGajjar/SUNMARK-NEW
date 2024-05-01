using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SUNMark.Classes;
using SUNMark.Common;
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
    public class OutFactoryController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        private readonly IWebHostEnvironment _iwebhostenviroment;

        public OutFactoryController(IWebHostEnvironment iwebhostenviroment)
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
                OutFactoryModel outFactory = new OutFactoryModel();
                outFactory.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                outFactory.AccountList = new List<CustomDropDown>();
                outFactory.AccountList.Add(CommonHelpers.GetDefaultValue());
                outFactory.AccountList.AddRange(ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0));
                outFactory.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                outFactory.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                outFactory.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                outFactory.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "COIL");

                outFactory.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                outFactory.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                outFactory.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                outFactory.FinishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                outFactory.ProcessList = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator, "");
                SqlParameter[] sqlParam = new SqlParameter[3];
                sqlParam[0] = new SqlParameter("@OutVou", id);
                sqlParam[1] = new SqlParameter("@Flg", 3);
                sqlParam[2] = new SqlParameter("@CmpVou", companyId);
                DataTable DtOut = ObjDBConnection.CallStoreProcedure("GetOutFactoryDetails", sqlParam);
                if (DtOut != null && DtOut.Rows.Count > 0)
                {
                    outFactory.OutCmpVou = Convert.ToInt32(DtOut.Rows[0]["OutCmpVou"].ToString());
                    outFactory.OutPtyVou = Convert.ToInt32(DtOut.Rows[0]["OutPrdTyp"].ToString());
                    outFactory.OutPrdTyp = DtOut.Rows[0]["PrdType"].ToString();
                    outFactory.OutDt = !string.IsNullOrWhiteSpace(DtOut.Rows[0]["OutDt"].ToString()) ? Convert.ToDateTime(DtOut.Rows[0]["OutDt"].ToString()).ToString("yyyy-MM-dd") : "";
                    outFactory.OutAccVou = Convert.ToInt32(DtOut.Rows[0]["OutAccVou"].ToString());
                    int vno = DbConnection.ParseInt32(DtOut.Rows[0][0].ToString());
                    outFactory.OutVNo = vno + 1;
                    var ptyname = DtOut.Rows[0]["PrdType"].ToString();
                }
                if (id > 0)
                {
                    outFactory.OutVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@OutVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtOutMst = ObjDBConnection.CallStoreProcedure("GetOutFactoryDetails", sqlParameters);
                    if (DtOutMst != null && DtOutMst.Rows.Count > 0)
                    {
                        outFactory.OutVNo = Convert.ToInt32(DtOutMst.Rows[0]["OutVNo"].ToString());
                        outFactory.OutDt = !string.IsNullOrWhiteSpace(DtOutMst.Rows[0]["OutDt"].ToString()) ? Convert.ToDateTime(DtOutMst.Rows[0]["OutDt"].ToString()).ToString("yyyy-MM-dd") : "";
                        outFactory.OutAccVou = Convert.ToInt32(DtOutMst.Rows[0]["OutAccVou"].ToString());
                        outFactory.OutRefNo = DtOutMst.Rows[0]["OutRefNo"].ToString();
                        outFactory.OutBillDt = !string.IsNullOrWhiteSpace(DtOutMst.Rows[0]["OutBillDt"].ToString()) ? Convert.ToDateTime(DtOutMst.Rows[0]["OutBillDt"].ToString()).ToString("yyyy-MM-dd") : "";
                        outFactory.OutPtyVou = Convert.ToInt32(DtOutMst.Rows[0]["OutPrdTyp"].ToString());
                        outFactory.OutPOMVou = Convert.ToInt32(DtOutMst.Rows[0]["OutPOMVou"].ToString());
                        outFactory.OutPrdTyp = DtOutMst.Rows[0]["PrdType"].ToString();
                        outFactory.OutRem = DtOutMst.Rows[0]["OutRem"].ToString();
                        outFactory.OutCmpVou = Convert.ToInt32(DtOutMst.Rows[0]["OutCmpVou"].ToString());
                        outFactory.OutLRNo = DtOutMst.Rows[0]["OutLRNo"].ToString();
                        outFactory.OutWPNo = DtOutMst.Rows[0]["OutWpSlipNo"].ToString();
                        outFactory.OutWPWeight = DtOutMst.Rows[0]["OutWpWeight"].ToString();
                        outFactory.OutVehNo = DtOutMst.Rows[0]["OutVehNo"].ToString();
                        outFactory.OutTransNm = DtOutMst.Rows[0]["OutTransNm"].ToString();
                        outFactory.OutTrnVou = Convert.ToInt32(DtOutMst.Rows[0]["OutTrnVou"].ToString());
                        outFactory.OutFrightRt = Convert.ToDecimal(DtOutMst.Rows[0]["OutFrtRt"].ToString());
                        outFactory.OutBillNo = DtOutMst.Rows[0]["OutBillNo"].ToString();
                        outFactory.OutPrdVou = Convert.ToInt32(DtOutMst.Rows[0]["OutPrdVou"].ToString());
                        //outFactory.OutCoilTypeVou = Convert.ToInt32(DtOutMst.Rows[0]["OutCoilTypeVou"].ToString());
                        //outFactory.CoilType = DtOutMst.Rows[0]["OutCoilType"].ToString();

                        var ptynm = DtOutMst.Rows[0]["PrdType"].ToString();

                        List<TransactionGridAddUpdateRootModel> lstobl = new List<TransactionGridAddUpdateRootModel>();
                        List<TransactionGridAddUpdateDataModel> dataLIstAnotherModel = new List<TransactionGridAddUpdateDataModel>();

                        for (int i = 0; i < DtOutMst.Rows.Count; i++)
                        {
                            List<TransactionGridAddUpdateModel> dataList = new List<TransactionGridAddUpdateModel>();

                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OutAGdnVou",
                                Value = Convert.ToString(DtOutMst.Rows[i]["OutAGdnVou"].ToString())
                            });
                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OutACoilNo",
                                Value = Convert.ToString(DtOutMst.Rows[i]["OutACoilNo"].ToString())
                            });
                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OutASupCoilNo",
                                Value = Convert.ToString(DtOutMst.Rows[i]["OutASupCoilNo"].ToString())
                            });
                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OutAHeatNo",
                                Value = Convert.ToString(DtOutMst.Rows[i]["OutAHeatNo"].ToString())
                            });
                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OutAGrdVou",
                                Value = Convert.ToString(DtOutMst.Rows[i]["OutAGrdVou"].ToString())
                            });
                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OutAWidth",
                                Value = Convert.ToString(DtOutMst.Rows[i]["OutAWidth"].ToString())
                            });
                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OutAThick",
                                Value = Convert.ToString(DtOutMst.Rows[i]["OutAThick"].ToString())
                            });
                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OutAQty",
                                Value = Convert.ToString(DtOutMst.Rows[i]["OutAQty"].ToString())
                            });
                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OutARem",
                                Value = Convert.ToString(DtOutMst.Rows[i]["OutARem"].ToString())
                            });
                            dataLIstAnotherModel.Add(new TransactionGridAddUpdateDataModel
                            {
                                Data = dataList
                            });
                        }
                        lstobl.Add(new TransactionGridAddUpdateRootModel
                        {
                            MyArray = dataLIstAnotherModel
                        });

                        var jsonString = JsonConvert.SerializeObject(lstobl);
                        outFactory.Data = jsonString;

                    }
                }
                if (id > 0)
                    TempData["ReturnId"] = Convert.ToString(id);
                return View(outFactory);
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                //Transaction dynamic grid
                string currentURL = GetCurrentURL();
                ViewBag.LayoutListNew = TransactionGridHelper.GetLayoutList(currentURL);
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
        public IActionResult Index(long id, OutFactoryModel outFactory)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                //#region Upload File pdfUpload1

                //if (outFactory.OutFactory.pdfUpload1 != null)
                //{
                //    var uniqueFileName = GetUniqueFileName(outFactory.OutFactory.pdfUpload1.FileName);
                //    var uploads = Path.Combine(_iwebhostenviroment.WebRootPath, "TCUploads");
                //    if (!Directory.Exists(uploads))
                //    {
                //        Directory.CreateDirectory(uploads);
                //    }
                //    var filePath = Path.Combine(uploads, uniqueFileName);
                //    outFactory.OutFactory.pdfUpload1.CopyTo(new FileStream(filePath, FileMode.Create));
                //    outFactory.OutFactory.ProfilePictureCoil = new string[] { uniqueFileName };
                //    //to do : Save uniqueFileName  to your db table   
                //}

                //#endregion
                //#region Upload File pdfUpload2

                //if (outFactory.OutFactory.pdfUpload2 != null)
                //{
                //    var uniqueFileName = GetUniqueFileName(outFactory.OutFactory.pdfUpload2.FileName);
                //    var uploads = Path.Combine(_iwebhostenviroment.WebRootPath, "TCUploads");
                //    if (!Directory.Exists(uploads))
                //    {
                //        Directory.CreateDirectory(uploads);
                //    }
                //    var filePath = Path.Combine(uploads, uniqueFileName);
                //    outFactory.OutFactory.pdfUpload2.CopyTo(new FileStream(filePath, FileMode.Create));
                //    outFactory.OutFactory.ProfilePicturePipe = new string[] { uniqueFileName };
                //    //to do : Save uniqueFileName  to your db table   
                //}

                //#endregion

                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                long userId = GetIntSession("UserId");
                int administrator = 0;

                outFactory.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                outFactory.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                outFactory.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                outFactory.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                outFactory.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                outFactory.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "COIL");

                List<TransactionGridAddUpdateRootModel> transactionList = JsonConvert.DeserializeObject<List<TransactionGridAddUpdateRootModel>>(outFactory.Data);

                if (outFactory.OutFactory == null)
                    outFactory.OutFactory = new OutFactoryGridModel();

                outFactory.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                outFactory.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

                var prdtype = outFactory.OutPrdTyp;
                if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outFactory.OutVNo).ToString()) && !string.IsNullOrWhiteSpace(outFactory.OutDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outFactory.OutAccVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outFactory.OutPtyVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outFactory.OutCoilTypeVou).ToString()))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[22];
                    sqlParameters[0] = new SqlParameter("@OutVNo", outFactory.OutVNo);
                    sqlParameters[1] = new SqlParameter("@OutDt", outFactory.OutDt);
                    sqlParameters[2] = new SqlParameter("@OutRefNo", outFactory.OutRefNo);
                    sqlParameters[3] = new SqlParameter("@OutAccVou", outFactory.OutAccVou);
                    sqlParameters[4] = new SqlParameter("@OutPrdTyp", outFactory.OutPtyVou);
                    sqlParameters[5] = new SqlParameter("@OutRem", outFactory.OutRem);
                    sqlParameters[6] = new SqlParameter("@OutVou", id);
                    sqlParameters[7] = new SqlParameter("@OutPOMVou", outFactory.OutPOMVou);
                    sqlParameters[8] = new SqlParameter("@OutLRNo", outFactory.OutLRNo);
                    sqlParameters[9] = new SqlParameter("@OutWPNo", outFactory.OutWPNo);
                    sqlParameters[10] = new SqlParameter("@OutVehNo", outFactory.OutVehNo);
                    if (outFactory.OutTransNm == "Select")
                    {
                        sqlParameters[11] = new SqlParameter("@OutTransNm", "");
                    }
                    else
                    {
                        sqlParameters[11] = new SqlParameter("@OutTransNm", outFactory.OutTransNm);
                    }
                    sqlParameters[12] = new SqlParameter("@OutBillNo", outFactory.OutBillNo);
                    sqlParameters[13] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[14] = new SqlParameter("@FLG", 1);
                    sqlParameters[15] = new SqlParameter("@CmpVou", outFactory.OutCmpVou);
                    sqlParameters[16] = new SqlParameter("@OutPrdVou", outFactory.OutPrdVou);
                    sqlParameters[17] = new SqlParameter("@OutTrnVou", outFactory.OutTrnVou);
                    sqlParameters[18] = new SqlParameter("@OutFrtRt", outFactory.OutFrightRt);
                    sqlParameters[19] = new SqlParameter("@OutCoilTypeVou", outFactory.OutCoilTypeVou);
                    sqlParameters[20] = new SqlParameter("@OutCoilType", outFactory.CoilType);
                    sqlParameters[21] = new SqlParameter("@OutWPWeight", outFactory.OutWPWeight);
                    DataTable DtOutMst = ObjDBConnection.CallStoreProcedure("OutFactoryMst_Insert", sqlParameters);
                    if (DtOutMst != null && DtOutMst.Rows.Count > 0)
                    {
                        int masterId = DbConnection.ParseInt32(DtOutMst.Rows[0][0].ToString());
                        if (masterId > 0)
                        {
                            for (int i = 0; i < transactionList[0].MyArray.Count; i++)
                            {
                                SqlParameter[] parameter = new SqlParameter[25];
                                parameter[0] = new SqlParameter("@OutAOutVou", masterId);
                                parameter[1] = new SqlParameter("@OutASrNo", (i + 1));
                                parameter[2] = new SqlParameter("@OutAPrdVou", "0");
                                parameter[3] = new SqlParameter("@OutAGrdVou", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OutAGrdVou"));
                                parameter[4] = new SqlParameter("@OutASpaVou", "0");
                                parameter[5] = new SqlParameter("@OutAWidth", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OutAWidth"));
                                parameter[6] = new SqlParameter("@OutAThick", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OutAThick"));
                                parameter[7] = new SqlParameter("@OutAQty", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OutAQty"));
                                parameter[8] = new SqlParameter("@OutAPcs", "0");
                                parameter[9] = new SqlParameter("@OutARem", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OutARem"));
                                parameter[10] = new SqlParameter("@OutACmpVou", outFactory.OutCmpVou);
                                parameter[11] = new SqlParameter("@OutAPOTVou", "0");
                                parameter[12] = new SqlParameter("@OutALotVou", "0");
                                parameter[13] = new SqlParameter("@OutAGdnVou", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OutAGdnVou"));
                                parameter[14] = new SqlParameter("@OutALocVou", "0");
                                parameter[15] = new SqlParameter("@OutASupCoilNo", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OutASupCoilNo"));
                                parameter[16] = new SqlParameter("@OutAHeatNo", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OutAHeatNo"));
                                parameter[17] = new SqlParameter("@OutAOD", "0");
                                parameter[18] = new SqlParameter("@OutALenght", "0");
                                parameter[19] = new SqlParameter("@OutAFinVou", "");
                                parameter[20] = new SqlParameter("@OutAPrcVou", "");
                                parameter[21] = new SqlParameter("@OutACoilPrefix", "");
                                parameter[22] = new SqlParameter("@OutACoilNo", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OutACoilNo"));
                                parameter[23] = new SqlParameter("@OutASufix", "0");
                                parameter[24] = new SqlParameter("@TCUpload", "");
                                DataTable DtOutTrn = ObjDBConnection.CallStoreProcedure("OutFactoryTrn_Insert", parameter);
                            }
                            int Status = DbConnection.ParseInt32(DtOutMst.Rows[0][0].ToString());
                            if (Status == 0)
                            {
                                outFactory.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                outFactory.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                outFactory.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                outFactory.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                outFactory.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                outFactory.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "COIL");
                                outFactory.CoilTypeList = objProductHelper.GetInwardCoilType();

                                if (outFactory.OutFactory == null)
                                    outFactory.OutFactory = new OutFactoryGridModel();

                                outFactory.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                outFactory.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                outFactory.OutDt = Convert.ToDateTime(outFactory.OutDt).ToString("yyyy-MM-dd");
                                SetErrorMessage("Dulplicate Vou.No Details");
                                ViewBag.FocusType = "1";
                                return View(outFactory);
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Updated Sucessfully");
                                    if (outFactory.isPrint != 0)
                                    {
                                        TempData["ReturnId"] = Status;
                                        TempData["Savedone"] = "1";
                                        TempData["IsPrint"] = outFactory.isPrint;
                                    }
                                }
                                else
                                {
                                    SetSuccessMessage("Inserted Sucessfully");
                                    if (outFactory.isPrint != 0)
                                    {
                                        TempData["ReturnId"] = Status;
                                        TempData["Savedone"] = "1";
                                        TempData["IsPrint"] = outFactory.isPrint;
                                    }
                                }
                                if (outFactory.isPrint == 0)
                                    return RedirectToAction("Index", new { id = 0 });
                            }
                        }
                        else
                        {
                            outFactory.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                            outFactory.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                            outFactory.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                            outFactory.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            outFactory.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                            outFactory.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "COIL");
                            outFactory.CoilTypeList = objProductHelper.GetInwardCoilType();

                            if (outFactory.OutFactory == null)
                                outFactory.OutFactory = new OutFactoryGridModel();

                            outFactory.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            outFactory.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            outFactory.OutDt = Convert.ToDateTime(outFactory.OutDt).ToString("yyyy-MM-dd");
                            SetErrorMessage("Insert error");
                            ViewBag.FocusType = "1";
                            return View(outFactory);
                        }
                    }
                    else
                    {
                        outFactory.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                        outFactory.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                        outFactory.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                        outFactory.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        outFactory.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                        outFactory.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "COIL");
                        outFactory.CoilTypeList = objProductHelper.GetInwardCoilType();

                        if (outFactory.OutFactory == null)
                            outFactory.OutFactory = new OutFactoryGridModel();

                        outFactory.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        outFactory.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        outFactory.OutDt = Convert.ToDateTime(outFactory.OutDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(outFactory);
                    }
                }
                else
                {
                    outFactory.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                    outFactory.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                    outFactory.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                    outFactory.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                    outFactory.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                    outFactory.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "COIL");
                    outFactory.CoilTypeList = objProductHelper.GetInwardCoilType();

                    if (outFactory.OutFactory == null)
                        outFactory.OutFactory = new OutFactoryGridModel();

                    outFactory.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                    outFactory.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                    outFactory.OutDt = Convert.ToDateTime(outFactory.OutDt).ToString("yyyy-MM-dd");
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "1";
                    return View(outFactory);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index");
        }

        public OutFactoryModel GetOrderListByAccount(string OutPOMVou)
        {
            OutFactoryModel obj = new OutFactoryModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(OutPOMVou))
                {
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@OrmVou", OutPOMVou);
                    sqlParameters[1] = new SqlParameter("@Type", "PORD");
                    sqlParameters[2] = new SqlParameter("@Flg", 2);
                    sqlParameters[3] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtOutMst = ObjDBConnection.CallStoreProcedure("GetPurchaseOrderDetails", sqlParameters);
                    if (DtOutMst != null && DtOutMst.Rows.Count > 0)
                    {
                        DataRow[] drFind = DtOutMst.Select("OrmVou=" + OutPOMVou);
                        if (drFind != null && drFind.Length > 0)
                        {
                            DataTable dtUniq = drFind.CopyToDataTable();
                            if (dtUniq != null && dtUniq.Rows.Count > 0)
                            {
                                obj.OutAccVou = Convert.ToInt32(dtUniq.Rows[0]["OrmAccVou"].ToString());
                                obj.CtyName = dtUniq.Rows[0]["CtyName"].ToString();
                                obj.OutPtyVou = Convert.ToInt32(dtUniq.Rows[0]["OrmPtyVou"].ToString());
                                obj.OutPrdTyp = dtUniq.Rows[0]["OrmPtyNm"].ToString();
                                var prdType = dtUniq.Rows[0]["OrmPtyNm"].ToString();
                                obj.OutFactoryList = new List<OutFactoryGridModel>();
                                for (int i = 0; i < dtUniq.Rows.Count; i++)
                                {
                                    if (prdType == "COIL" || prdType == "Coil")
                                    {
                                        obj.OutFactoryList.Add(new OutFactoryGridModel()
                                        {
                                            OutAGrade = Convert.ToInt32(dtUniq.Rows[i]["OrdAGrdVou"].ToString()),
                                            OutAThick = Convert.ToDecimal(dtUniq.Rows[i]["OrdThick"].ToString()),
                                            OutAWidth = Convert.ToDecimal(dtUniq.Rows[i]["OrdWidth"].ToString()),
                                            OutAQty = Convert.ToDecimal(dtUniq.Rows[i]["OrdQty"].ToString()),
                                            OutARem = dtUniq.Rows[i]["OrdRem"].ToString()
                                        });
                                    }
                                    else if (prdType == "PIPE" || prdType == "Pipe" || prdType == "SEAMLESS PIPE" || prdType == "seamless pipe")
                                    {
                                        obj.OutFactoryList.Add(new OutFactoryGridModel()
                                        {
                                            OutASpaVou = Convert.ToInt32(dtUniq.Rows[i]["OutASpaVou"].ToString()),
                                            OutAGrade = Convert.ToInt32(dtUniq.Rows[i]["OrdAGrdVou"].ToString()),
                                            OutAThick = Convert.ToDecimal(dtUniq.Rows[i]["OrdAThick"].ToString()),
                                            OutAOD = Convert.ToDecimal(dtUniq.Rows[i]["OrdAOD"].ToString()),
                                            OutAPcs = Convert.ToDecimal(dtUniq.Rows[i]["OrdAPcs"].ToString()),
                                            OutARem = dtUniq.Rows[i]["OrdARem"].ToString()
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

        public OutFactoryModel GetLastVoucherNo(int companyId)
        {
            OutFactoryModel obj = new OutFactoryModel();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@Cmpvou", companyId);
                DataTable dtNewOutVNo = ObjDBConnection.CallStoreProcedure("GetLatestOutVNo", sqlParameters);
                if (dtNewOutVNo != null && dtNewOutVNo.Rows.Count > 0)
                {
                    int.TryParse(dtNewOutVNo.Rows[0]["OutVNo"].ToString(), out int OutVNo);
                    OutVNo = OutVNo == 0 ? 1 : OutVNo;
                    obj.OutVNo = OutVNo;
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
                OutFactoryModel outFactory = new OutFactoryModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@OutVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "1");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtOut = ObjDBConnection.CallStoreProcedure("GetOutFactoryDetails", sqlParameters);
                    if (DtOut != null && DtOut.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtOut.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("OutFactory Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "OutFactory");
        }
        public IActionResult GetDeleteSupCoil(string supcoilno)
        {
            try
            {
                InTransInwardModel outFactory = new InTransInwardModel();
                if (supcoilno != "")
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@SupCoil", supcoilno);
                    sqlParameters[1] = new SqlParameter("@Type", "INW");
                    DataTable DtOut = ObjDBConnection.CallStoreProcedure("DeleteSupCoilMst", sqlParameters);
                    if (DtOut != null && DtOut.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtOut.Rows[0][0].ToString());
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
                    string currentURL = "/OutFactory/Index";
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
                    getReportDataModel.ControllerName = "OutFactory";
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
                    var bytes = Excel(getReportDataModel, "OutFactory Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "OutFactory.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "OutFactory Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "OutFactory.pdf");
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

        public IActionResult GetOrderDetail(int outcmpvou)
        {
            try
            {
                var purchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(outcmpvou);
                return Json(new { result = true, data = purchaseOrderList });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult GetSupCoilCheck(string coilno, int srno1, string outvou)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@SupCoil", coilno);
                sqlParameters[1] = new SqlParameter("@cmpvou", companyId);
                sqlParameters[2] = new SqlParameter("@Type", "OUT");
                sqlParameters[3] = new SqlParameter("@Vou", srno1);
                sqlParameters[4] = new SqlParameter("@Flg", "0");
                sqlParameters[5] = new SqlParameter("@MainVou", outvou);
                DataTable DtOut = ObjDBConnection.CallStoreProcedure("GetLotMstDetails1", sqlParameters);
                if (DtOut != null && DtOut.Columns.Count > 1)
                {
                    if (DtOut.Rows.Count > 0)
                    {
                        string Grade = DtOut.Rows[0]["LotGrade"].ToString();
                        string GrdVou = DtOut.Rows[0]["LotGrdMscVou"].ToString();
                        string Width = DtOut.Rows[0]["LotWidth"].ToString();
                        string Thick = DtOut.Rows[0]["LotThick"].ToString();
                        string Qty = DtOut.Rows[0]["LotQty"].ToString();
                        string HeatNo = DtOut.Rows[0]["LotHeatNo"].ToString();
                        string SupCoilNo = DtOut.Rows[0]["LotSupCoilNo"].ToString();
                        return Json(new { result = true, grade = Grade, grdVou = GrdVou, width = Width, thick = Thick, qty = Qty, heatNo = HeatNo, supCoilNo = SupCoilNo });

                    }
                    else
                    {
                        return Json(new { result = true, data = "1" });
                    }
                }
                else
                {
                    int status = DbConnection.ParseInt32(DtOut.Rows[0][0].ToString());
                    if (status == 1)
                    {
                        return Json(new { result = true, data = "1" });
                    }
                    else
                    {
                        return Json(new { result = true, data = "-1" });
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult OutFactoryPrintDetials(long id, int copyType = 1)
        {
            OutFactoryPrintDetails obj = new OutFactoryPrintDetails();

            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                long userId = GetIntSession("UserId");


                //SqlParameter[] Param = new SqlParameter[1];
                //Param[0] = new SqlParameter("@OutVou", id);
                //DataTable DtLabel = ObjDBConnection.CallStoreProcedure("Insert_OutFactoryLabel", Param);

                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@OutVou", id);
                sqlParameters[1] = new SqlParameter("@Flg", 4);
                sqlParameters[2] = new SqlParameter("@cmpvou", companyId);
                DataTable DtBilty = ObjDBConnection.CallStoreProcedure("GetOutFactoryDetails", sqlParameters);
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
                            string BilDate = DateTime.Parse(DtBilty.Rows[0]["OutDt"].ToString()).ToString("dd-MM-yyyy");
                            string newbody = body.Replace("#*#*r1*#*#", BilDate);
                            newbody = body.Replace("#*#*r2*#*#", DtBilty.Rows[0]["LotCoilNo"].ToString());
                            newbody = newbody.Replace("#*#*r1*#*#", BilDate);
                            newbody = newbody.Replace("#*#*r3*#*#", DtBilty.Rows[0]["AccNm"].ToString());
                            newbody = newbody.Replace("#*#*r4*#*#", DtBilty.Rows[0]["OutVehNo"].ToString());
                            newbody = newbody.Replace("#*#*r5*#*#", DtBilty.Rows[0]["LotSupCoilNo"].ToString());
                            newbody = newbody.Replace("#*#*r6*#*#", DtBilty.Rows[0]["LotGrade"].ToString());
                            newbody = newbody.Replace("#*#*r7*#*#", (Math.Round(decimal.Parse((DtBilty.Rows[0]["LotWidth"].ToString())))).ToString("0.00"));
                            newbody = newbody.Replace("#*#*r8*#*#", (Math.Round(decimal.Parse((DtBilty.Rows[0]["LotThick"].ToString())))).ToString("0.00"));
                            newbody = newbody.Replace("#*#*r9*#*#", (Math.Round(decimal.Parse((DtBilty.Rows[0]["LotQty"].ToString())))).ToString());
                            newbody = newbody.Replace("#*#*r10*#*#", DtBilty.Rows[0]["LotHeatNo"].ToString());
                            newbody = newbody.Replace("#*#*r11*#*#", DtBilty.Rows[0]["OutBillNo"].ToString());
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

        [Route("/OutFactory/GetAccount-List")]
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

        [Route("/OutFactory/GetTransport-List")]
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
        public IActionResult RawOutFactoryPrintDetials(long id, int companyid, int copyType = 1)
        {
            try
            {
                OutFactoryPrintDetails obj = GetDetailsById(id);

                return View(obj);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult OutFactoryPrintDetials(long id)
        {
            try
            {
                OutFactoryPrintDetails obj = GetDetailsById(id);

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
                //sqlParameters[0] = new SqlParameter("@OutVou", id);
                DataTable DtOutMst = ObjDBConnection.CallStoreProcedure("GetNewCoilNoForOutFactory", sqlParameters);
                if (DtOutMst != null && DtOutMst.Rows.Count > 0)
                {
                    CoilNo = DtOutMst.Rows[0]["NewLotNo"].ToString();
                }
                return Json(new { data = CoilNo });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult OutFactorySendMail(long id, string email = "")
        {
            try
            {
                OutFactoryPrintDetails obj = GetDetailsById(id);
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
        public IActionResult OutFactoryWhatApp(long id, string whatappNo = "")
        {
            try
            {
                OutFactoryPrintDetails obj = GetDetailsById(id);
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

        public OutFactoryPrintDetails GetDetailsById(long Id)
        {
            OutFactoryPrintDetails obj = new OutFactoryPrintDetails();

            try
            {
                StringBuilder sb = new StringBuilder();
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@OutVou", Id);
                sqlParameters[1] = new SqlParameter("@Flg", 5);
                sqlParameters[2] = new SqlParameter("@cmpvou", companyId);
                DataTable DtOutFactory = ObjDBConnection.CallStoreProcedure("GetOutFactoryDetails", sqlParameters);
                if (DtOutFactory != null && DtOutFactory.Rows.Count > 0)
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
                    if (DtOutFactory.Columns.Contains("DepAdd"))
                        CmpAdd = DtOutFactory.Rows[0]["DepAdd"].ToString();
                    if (DtOutFactory.Columns.Contains("DepEmail"))
                        CmpEmail = DtOutFactory.Rows[0]["DepEmail"].ToString();
                    if (DtOutFactory.Columns.Contains("DepVou"))
                        CmpVou = DtOutFactory.Rows[0]["DepVou"].ToString();
                    if (DtOutFactory.Columns.Contains("DepWeb"))
                        CmpWeb = DtOutFactory.Rows[0]["DepWeb"].ToString();
                    if (DtOutFactory.Columns.Contains("DepBusLine"))
                        CmpWeb = DtOutFactory.Rows[0]["DepBusLine"].ToString();

                    Layout = "RawMatOutFactory";
                    filename = "RawMatOutFactory.html";

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        newbody = body.Replace("#*#*cmpAdd*#*#", CmpAdd.Replace(";", "<br>"));
                        newbody = newbody.Replace("#*#*cmpEmail*#*#", CmpEmail);
                        newbody = newbody.Replace("#*#*cmpWeb*#*#", CmpWeb);

                        string BilDate = DateTime.Parse(DtOutFactory.Rows[0]["OutDt"].ToString()).ToString("dd-MM-yyyy");
                        newbody = newbody.Replace("#*#*r1*#*#", BilDate);
                        newbody = newbody.Replace("#*#*r2*#*#", DtOutFactory.Rows[0]["AccNm"].ToString());
                        newbody = newbody.Replace("#*#*r3*#*#", DtOutFactory.Rows[0]["OutVehNo"].ToString());
                        newbody = newbody.Replace("#*#*r4*#*#", "");
                        newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(DtOutFactory.Rows[0]["DepLogo"].ToString()) ? "/Uploads/" + DtOutFactory.Rows[0]["DepLogo"].ToString() + "" : string.Empty);


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
                        for (int i = 0; i < DtOutFactory.Rows.Count; i++)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + (i + 1) + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtOutFactory.Rows[i]["LotCoilNo"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtOutFactory.Rows[i]["OutAHeatNo"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtOutFactory.Rows[i]["OutAWidth"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtOutFactory.Rows[i]["OutAThick"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtOutFactory.Rows[i]["OutAQty"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"left\" style=\"font-size:14px;\">" + DtOutFactory.Rows[i]["OutARem"].ToString() + "</td>");
                            dTotWT += Convert.ToDouble(DtOutFactory.Rows[i]["OutAQty"].ToString());
                            sb.Append("</tr>");
                        }

                        for (int i = DtOutFactory.Rows.Count; i < 11; i++)
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
        private string GetUniqueFileName(string fileName)
        {
            return Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }
        [HttpPost]
        public IActionResult GeneratePdf(IFormFile pdfFilePath)
        {
            try
            {
                var uniqueFileName = GetUniqueFileName(pdfFilePath.FileName);
                var previewPdf = Path.Combine(_iwebhostenviroment.WebRootPath, "PreviewPdf");
                if (!Directory.Exists(previewPdf))
                {
                    Directory.CreateDirectory(previewPdf);
                }
                var filePath = Path.Combine(previewPdf, uniqueFileName);
                pdfFilePath.CopyTo(new FileStream(filePath, FileMode.Create));
                return Json("/PreviewPdf/" + uniqueFileName);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
