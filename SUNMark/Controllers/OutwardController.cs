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
using Microsoft.AspNetCore.Http;

namespace SUNMark.Controllers
{
    public class OutwardController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        private readonly IWebHostEnvironment _iwebhostenviroment;

        public OutwardController(IWebHostEnvironment iwebhostenviroment)
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
                OutwardModel outwardModel = new OutwardModel();
                outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                outwardModel.AccountList = new List<CustomDropDown>();
                outwardModel.AccountList.Add(CommonHelpers.GetDefaultValue());
                outwardModel.AccountList.AddRange(ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0));
                outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "Select");

                outwardModel.Outward = new OutwardGridModel();
                outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                outwardModel.Outward.OutACoilTypeList = objProductHelper.GetInwardCoilType();
                SqlParameter[] sqlParam = new SqlParameter[3];
                sqlParam[0] = new SqlParameter("@OutVou", id);
                sqlParam[1] = new SqlParameter("@Flg", 3);
                sqlParam[2] = new SqlParameter("@CmpVou", companyId);
                DataTable DtOut = ObjDBConnection.CallStoreProcedure("GetOutwardDetails", sqlParam);
                if (DtOut != null && DtOut.Rows.Count > 0)
                {
                    outwardModel.OutCmpVou = Convert.ToInt32(DtOut.Rows[0]["OutCmpVou"].ToString());
                    outwardModel.OutPtyVou = Convert.ToInt32(DtOut.Rows[0]["OutPrdTyp"].ToString());
                    outwardModel.OutPrdTyp = DtOut.Rows[0]["PrdType"].ToString();
                    outwardModel.OutDt = !string.IsNullOrWhiteSpace(DtOut.Rows[0]["OutDt"].ToString()) ? Convert.ToDateTime(DtOut.Rows[0]["OutDt"].ToString()).ToString("yyyy-MM-dd") : "";
                    outwardModel.OutAccVou = Convert.ToInt32(DtOut.Rows[0]["OutAccVou"].ToString());
                    int vno = DbConnection.ParseInt32(DtOut.Rows[0][0].ToString());
                    outwardModel.OutVNo = vno + 1;
                    var ptyname = DtOut.Rows[0]["PrdType"].ToString();
                    if (ptyname == "COIL" || ptyname == "Coil")
                    {
                        outwardModel.Outward.SupCoilNoStr = DtOut.Rows[0]["OutASupCoilNo"].ToString();
                        outwardModel.Outward.HeatNoStr = DtOut.Rows[0]["OutAHeatNo"].ToString();
                        outwardModel.Outward.ProfilePictureCoilstr = DtOut.Rows[0]["TCUpload"].ToString();
                    }
                    else if (ptyname == "PIPE" || ptyname == "Pipe" || ptyname == "SEAMLESS PIPE" || ptyname == "seamless pipe")
                    {
                        outwardModel.Outward.ProfilePicturePipestr = DtOut.Rows[0]["TCUpload"].ToString();
                    }
                    else
                    {
                        outwardModel.Outward.ProfilePictureOtherstr = DtOut.Rows[0]["TCUpload"].ToString();
                    }
                }
                if (id > 0)
                {
                    outwardModel.OutVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@OutVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtOutMst = ObjDBConnection.CallStoreProcedure("GetOutwardDetails", sqlParameters);
                    if (DtOutMst != null && DtOutMst.Rows.Count > 0)
                    {
                        outwardModel.OutVNo = Convert.ToInt32(DtOutMst.Rows[0]["OutVNo"].ToString());
                        outwardModel.OutDt = !string.IsNullOrWhiteSpace(DtOutMst.Rows[0]["OutDt"].ToString()) ? Convert.ToDateTime(DtOutMst.Rows[0]["OutDt"].ToString()).ToString("yyyy-MM-dd") : "";
                        outwardModel.OutAccVou = Convert.ToInt32(DtOutMst.Rows[0]["OutAccVou"].ToString());
                        outwardModel.OutRefNo = DtOutMst.Rows[0]["OutRefNo"].ToString();
                        outwardModel.OutBillDt = !string.IsNullOrWhiteSpace(DtOutMst.Rows[0]["OutBillDt"].ToString()) ? Convert.ToDateTime(DtOutMst.Rows[0]["OutBillDt"].ToString()).ToString("yyyy-MM-dd") : "";
                        outwardModel.OutPtyVou = Convert.ToInt32(DtOutMst.Rows[0]["OutPrdTyp"].ToString());
                        outwardModel.OutPOMVou = Convert.ToInt32(DtOutMst.Rows[0]["OutPOMVou"].ToString());
                        outwardModel.OutPrdTyp = DtOutMst.Rows[0]["PrdType"].ToString();
                        outwardModel.OutRem = DtOutMst.Rows[0]["OutRem"].ToString();
                        outwardModel.OutCmpVou = Convert.ToInt32(DtOutMst.Rows[0]["OutCmpVou"].ToString());
                        outwardModel.OutLRNo = DtOutMst.Rows[0]["OutLRNo"].ToString();
                        outwardModel.OutWPNo = DtOutMst.Rows[0]["OutWpSlipNo"].ToString();
                        outwardModel.OutWPWeight = DtOutMst.Rows[0]["OutWpWeight"].ToString();
                        outwardModel.OutVehNo = DtOutMst.Rows[0]["OutVehNo"].ToString();
                        outwardModel.OutTransNm = DtOutMst.Rows[0]["OutTransNm"].ToString();
                        outwardModel.OutTrnVou = Convert.ToInt32(DtOutMst.Rows[0]["OutTrnVou"].ToString());
                        outwardModel.OutFrightRt = Convert.ToDecimal(DtOutMst.Rows[0]["OutFrtRt"].ToString());
                        outwardModel.OutBillNo = DtOutMst.Rows[0]["OutBillNo"].ToString();
                        outwardModel.OutPrdVou = Convert.ToInt32(DtOutMst.Rows[0]["OutPrdVou"].ToString());
                        //outwardModel.OutCoilTypeVou = Convert.ToInt32(DtOutMst.Rows[0]["OutCoilTypeVou"].ToString());
                        //outwardModel.CoilType = DtOutMst.Rows[0]["OutCoilType"].ToString();

                        var ptynm = DtOutMst.Rows[0]["PrdType"].ToString();
                        if (ptynm == "COIL" || ptynm == "Coil")
                        {
                            outwardModel.Outward.ProfilePictureCoil = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.SupCoilNo = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.HeatNo = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAGrdCoil = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAGdnCoil = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutACoilType = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.CoilNo = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAThickCoil = new decimal[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAWidth = new decimal[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAQtyCoil = new decimal[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutARemksCoil = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutACoilPrefix = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutASufix = new string[DtOutMst.Rows.Count];
                        }
                        else if (ptynm == "PIPE" || ptynm == "Pipe" || ptynm == "SEAMLESS PIPE" || ptynm == "seamless pipe")
                        {
                            outwardModel.Outward.ProfilePicturePipe = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAGdnPipe = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.SpecifiName = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAGrdPipe = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAOD = new decimal[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAThickPipe = new decimal[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutALenghtPipe = new decimal[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAPcs = new decimal[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAQtyPipe = new decimal[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAFinshPipe = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAProcePipe = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutARemksPipe = new string[DtOutMst.Rows.Count];
                        }
                        else
                        {
                            outwardModel.Outward.ProfilePictureOther = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAPrdVou = new int[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAGdnOther = new string[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutAQtyOther = new decimal[DtOutMst.Rows.Count];
                            outwardModel.Outward.OutARemksOther = new string[DtOutMst.Rows.Count];
                        }
                        for (int i = 0; i < DtOutMst.Rows.Count; i++)
                        {
                            if (ptynm == "COIL" || ptynm == "Coil")
                            {
                                outwardModel.Outward.ProfilePictureCoil[i] = DtOutMst.Rows[i]["TCUpload"].ToString();
                                outwardModel.Outward.SupCoilNo[i] = DtOutMst.Rows[i]["OutASupCoilNo"].ToString();
                                outwardModel.Outward.HeatNo[i] = DtOutMst.Rows[i]["OutAHeatNo"].ToString();
                                outwardModel.Outward.OutAGrdCoil[i] = DtOutMst.Rows[i]["OutAGrade"].ToString();
                                outwardModel.Outward.OutACoilType[i] = DtOutMst.Rows[i]["OutACoilType"].ToString();
                                outwardModel.Outward.OutAGdnCoil[i] = DtOutMst.Rows[i]["OutAGdnVou"].ToString();
                                outwardModel.Outward.CoilNo[i] = DtOutMst.Rows[i]["CoilNo"].ToString();
                                outwardModel.Outward.OutAThickCoil[i] = Convert.ToDecimal(DtOutMst.Rows[i]["OutAThick"].ToString());
                                outwardModel.Outward.OutAWidth[i] = Convert.ToDecimal(DtOutMst.Rows[i]["OutAWidth"].ToString());
                                outwardModel.Outward.OutAQtyCoil[i] = Convert.ToDecimal(DtOutMst.Rows[i]["OutAQty"].ToString());
                                outwardModel.Outward.OutARemksCoil[i] = DtOutMst.Rows[i]["OutARem"].ToString();
                                outwardModel.Outward.CoilNo[i] = DtOutMst.Rows[i]["OutACoilNo"].ToString();
                                outwardModel.Outward.OutACoilPrefix[i] = DtOutMst.Rows[i]["OutAPrefix"].ToString().Trim();
                                outwardModel.Outward.OutASufix[i] = Convert.ToString(DtOutMst.Rows[i]["OutASufix"].ToString());
                            }
                            else if (ptynm == "PIPE" || ptynm == "Pipe" || ptynm == "SEAMLESS PIPE" || ptynm == "seamless pipe")
                            {
                                outwardModel.Outward.ProfilePicturePipe[i] = DtOutMst.Rows[i]["TCUpload"].ToString();
                                outwardModel.Outward.OutAGdnPipe[i] = DtOutMst.Rows[i]["OutAGdnVou"].ToString();
                                outwardModel.Outward.SpecifiName[i] = DtOutMst.Rows[i]["OutASpacifi"].ToString();
                                outwardModel.Outward.OutAGrdPipe[i] = DtOutMst.Rows[i]["OutAGrade"].ToString();
                                outwardModel.Outward.OutAOD[i] = Convert.ToDecimal(DtOutMst.Rows[i]["OutAOD"].ToString());
                                outwardModel.Outward.OutAThickPipe[i] = Convert.ToDecimal(DtOutMst.Rows[i]["OutAThick"].ToString());
                                outwardModel.Outward.OutALenghtPipe[i] = Convert.ToDecimal(DtOutMst.Rows[i]["OutALenght"].ToString());
                                outwardModel.Outward.OutAPcs[i] = Convert.ToDecimal(DtOutMst.Rows[i]["OutAPcs"].ToString());
                                outwardModel.Outward.OutAQtyPipe[i] = Convert.ToDecimal(DtOutMst.Rows[i]["OutAQty"].ToString());
                                outwardModel.Outward.OutAFinshPipe[i] = DtOutMst.Rows[i]["OutAFinish"].ToString();
                                outwardModel.Outward.OutAProcePipe[i] = DtOutMst.Rows[i]["OutAProcess"].ToString();
                                outwardModel.Outward.OutARemksPipe[i] = DtOutMst.Rows[i]["OutARem"].ToString();
                            }
                            else
                            {
                                outwardModel.Outward.ProfilePictureOther[i] = DtOutMst.Rows[i]["TCUpload"].ToString();
                                outwardModel.Outward.OutAPrdVou[i] = Convert.ToInt32(DtOutMst.Rows[i]["OutAPrdVou"].ToString());
                                outwardModel.Outward.OutAGdnOther[i] = DtOutMst.Rows[i]["OutAGdnVou"].ToString();
                                outwardModel.Outward.OutAQtyOther[i] = Convert.ToDecimal(DtOutMst.Rows[i]["OutAQty"].ToString());
                                outwardModel.Outward.OutARemksOther[i] = DtOutMst.Rows[i]["OutARem"].ToString();
                            }

                        }

                    }
                }
                if (id > 0)
                    TempData["ReturnId"] = Convert.ToString(id);
                return View(outwardModel);
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
        public IActionResult Index(long id, OutwardModel outwardModel)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                #region Upload File pdfUpload1
                
                if (outwardModel.Outward.pdfUpload1 != null)
                {
                    var uniqueFileName = GetUniqueFileName(outwardModel.Outward.pdfUpload1.FileName);
                    var uploads = Path.Combine(_iwebhostenviroment.WebRootPath, "TCUploads");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    outwardModel.Outward.pdfUpload1.CopyTo(new FileStream(filePath, FileMode.Create));
                    outwardModel.Outward.ProfilePictureCoil = new string[] { uniqueFileName };
                    //to do : Save uniqueFileName  to your db table   
                }

                #endregion
                #region Upload File pdfUpload2
                
                if (outwardModel.Outward.pdfUpload2 != null)
                {
                    var uniqueFileName = GetUniqueFileName(outwardModel.Outward.pdfUpload2.FileName);
                    var uploads = Path.Combine(_iwebhostenviroment.WebRootPath, "TCUploads");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    outwardModel.Outward.pdfUpload2.CopyTo(new FileStream(filePath, FileMode.Create));
                    outwardModel.Outward.ProfilePicturePipe = new string[] { uniqueFileName };
                    //to do : Save uniqueFileName  to your db table   
                }

                #endregion
                #region Upload File pdfUpload3
                
                if (outwardModel.Outward.pdfUpload3 != null)
                {
                    var uniqueFileName = GetUniqueFileName(outwardModel.Outward.pdfUpload3.FileName);
                    var uploads = Path.Combine(_iwebhostenviroment.WebRootPath, "TCUploads");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    outwardModel.Outward.pdfUpload3.CopyTo(new FileStream(filePath, FileMode.Create));
                    outwardModel.Outward.ProfilePictureOther = new string[] { uniqueFileName };
                    //to do : Save uniqueFileName  to your db table   
                }

                #endregion
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                long userId = GetIntSession("UserId");
                int administrator = 0;

                outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                outwardModel.MainProductList = objProductHelper.GetProductMasterDropdown(companyId);

                if (outwardModel.Outward == null)
                    outwardModel.Outward = new OutwardGridModel();

                outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                outwardModel.Outward.OutACoilTypeList = objProductHelper.GetInwardCoilType();

                var prdtype = outwardModel.OutPrdTyp;
                if (prdtype == "COIL" || prdtype == "Coil")
                {
                    if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outwardModel.OutVNo).ToString()) && !string.IsNullOrWhiteSpace(outwardModel.OutDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outwardModel.OutAccVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outwardModel.OutPtyVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outwardModel.OutCoilTypeVou).ToString()) && outwardModel.Outward.OutAGrdCoil.Length > 0 && outwardModel.Outward.OutAGdnCoil.Length > 0 && outwardModel.Outward.OutAQtyCoil.Length > 0)
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[22];
                        sqlParameters[0] = new SqlParameter("@OutVNo", outwardModel.OutVNo);
                        sqlParameters[1] = new SqlParameter("@OutDt", outwardModel.OutDt);
                        sqlParameters[2] = new SqlParameter("@OutRefNo", outwardModel.OutRefNo);
                        sqlParameters[3] = new SqlParameter("@OutAccVou", outwardModel.OutAccVou);
                        sqlParameters[4] = new SqlParameter("@OutPrdTyp", outwardModel.OutPtyVou);
                        sqlParameters[5] = new SqlParameter("@OutRem", outwardModel.OutRem);
                        sqlParameters[6] = new SqlParameter("@OutVou", id);
                        sqlParameters[7] = new SqlParameter("@OutPOMVou", outwardModel.OutPOMVou);
                        sqlParameters[8] = new SqlParameter("@OutLRNo", outwardModel.OutLRNo);
                        sqlParameters[9] = new SqlParameter("@OutWPNo", outwardModel.OutWPNo);
                        sqlParameters[10] = new SqlParameter("@OutVehNo", outwardModel.OutVehNo);
                        if (outwardModel.OutTransNm == "Select")
                        {
                            sqlParameters[11] = new SqlParameter("@OutTransNm", "");
                        }
                        else
                        {
                            sqlParameters[11] = new SqlParameter("@OutTransNm", outwardModel.OutTransNm);
                        }
                        sqlParameters[12] = new SqlParameter("@OutBillNo", outwardModel.OutBillNo);
                        sqlParameters[13] = new SqlParameter("@UsrVou", userId);
                        sqlParameters[14] = new SqlParameter("@FLG", 1);
                        sqlParameters[15] = new SqlParameter("@CmpVou", outwardModel.OutCmpVou);
                        sqlParameters[16] = new SqlParameter("@OutPrdVou", outwardModel.OutPrdVou);
                        sqlParameters[17] = new SqlParameter("@OutTrnVou", outwardModel.OutTrnVou);
                        sqlParameters[18] = new SqlParameter("@OutFrtRt", outwardModel.OutFrightRt);
                        sqlParameters[19] = new SqlParameter("@OutCoilTypeVou", outwardModel.OutCoilTypeVou);
                        sqlParameters[20] = new SqlParameter("@OutCoilType", outwardModel.CoilType);
                        sqlParameters[21] = new SqlParameter("@OutWPWeight", outwardModel.OutWPWeight);
                        DataTable DtOutMst = ObjDBConnection.CallStoreProcedure("OutwardMst_Insert", sqlParameters);
                        if (DtOutMst != null && DtOutMst.Rows.Count > 0)
                        {
                            int masterId = DbConnection.ParseInt32(DtOutMst.Rows[0][0].ToString());
                            if (masterId > 0)
                            {
                                for (int i = 0; i < outwardModel.Outward.OutAQtyCoil.Length; i++)
                                {

                                    if (outwardModel.Outward.OutAQtyCoil[i] != null && outwardModel.Outward.OutAQtyCoil[i] != 0 && outwardModel.Outward.OutAQtyCoil[i] > 0)
                                    {
                                        SqlParameter[] parameter = new SqlParameter[25];
                                        parameter[0] = new SqlParameter("@OutAOutVou", masterId);
                                        parameter[1] = new SqlParameter("@OutASrNo", (i + 1));
                                        parameter[2] = new SqlParameter("@OutAPrdVou", "0");
                                        parameter[3] = new SqlParameter("@OutAGrade", outwardModel.Outward.OutAGrdCoil[i]);
                                        parameter[4] = new SqlParameter("@OutASpacifi", "");
                                        parameter[5] = new SqlParameter("@OutAWidth", outwardModel.Outward.OutAWidth[i]);
                                        parameter[6] = new SqlParameter("@OutAThick", outwardModel.Outward.OutAThickCoil[i]);
                                        parameter[7] = new SqlParameter("@OutAQty", outwardModel.Outward.OutAQtyCoil[i]);
                                        parameter[8] = new SqlParameter("@OutAPcs", "0");
                                        if (outwardModel.Outward.OutARemksCoil[i] == "-1")
                                        {
                                            parameter[9] = new SqlParameter("@OutARem", "");
                                        }
                                        else
                                        {
                                            parameter[9] = new SqlParameter("@OutARem", outwardModel.Outward.OutARemksCoil[i]);
                                        }
                                        parameter[10] = new SqlParameter("@OutACmpVou", outwardModel.OutCmpVou);
                                        parameter[11] = new SqlParameter("@OutAPOTVou", "0");
                                        parameter[12] = new SqlParameter("@OutALotVou", "0");
                                        parameter[13] = new SqlParameter("@OutAGdnVou", outwardModel.Outward.OutAGdnCoil[i]);
                                        parameter[14] = new SqlParameter("@OutALocVou", "0");
                                        parameter[15] = new SqlParameter("@OutASupCoilNo", outwardModel.Outward.SupCoilNo[i]);
                                        parameter[16] = new SqlParameter("@OutAHeatNo", outwardModel.Outward.HeatNo[i]);
                                        parameter[17] = new SqlParameter("@OutAOD", "0");
                                        parameter[18] = new SqlParameter("@OutALenght", "0");
                                        parameter[19] = new SqlParameter("@OutAFinish", "");
                                        parameter[20] = new SqlParameter("@OutAProcess", "");
                                        parameter[21] = new SqlParameter("@OutACoilPrefix", outwardModel.Outward.OutACoilPrefix[i + 1]);
                                        parameter[22] = new SqlParameter("@OutACoilNo", outwardModel.Outward.CoilNo[i]);
                                        parameter[23] = new SqlParameter("@OutASufix", outwardModel.Outward.OutASufix[i]);
                                        parameter[24] = new SqlParameter("@TCUpload", outwardModel.Outward.ProfilePictureCoil[i]);
                                        DataTable DtOutTrn = ObjDBConnection.CallStoreProcedure("OutwardTrn_Insert", parameter);
                                    }
                                }
                                int Status = DbConnection.ParseInt32(DtOutMst.Rows[0][0].ToString());
                                if (Status == 0)
                                {
                                    outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                    outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                    outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                    outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                    outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                    outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                                    if (outwardModel.Outward == null)
                                        outwardModel.Outward = new OutwardGridModel();

                                    outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                                    outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                                    SetErrorMessage("Dulplicate Vou.No Details");
                                    ViewBag.FocusType = "1";
                                    return View(outwardModel);
                                }
                                else
                                {
                                    if (id > 0)
                                    {
                                        SetSuccessMessage("Updated Sucessfully");
                                        if (outwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = outwardModel.isPrint;
                                        }
                                    }
                                    else
                                    {
                                        SetSuccessMessage("Inserted Sucessfully");
                                        if (outwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = outwardModel.isPrint;
                                        }
                                    }
                                    if (outwardModel.isPrint == 0)
                                        return RedirectToAction("Index", new { id = 0 });
                                }
                            }
                            else
                            {
                                outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                                if (outwardModel.Outward == null)
                                    outwardModel.Outward = new OutwardGridModel();

                                outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                                outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                                outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                                SetErrorMessage("Insert error");
                                ViewBag.FocusType = "1";
                                return View(outwardModel);
                            }
                        }
                        else
                        {
                            outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                            outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                            outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                            outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                            outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                            outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                            if (outwardModel.Outward == null)
                                outwardModel.Outward = new OutwardGridModel();

                            outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                            outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                            outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                            outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                            outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                            outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                            SetErrorMessage("Please Enter the Value");
                            ViewBag.FocusType = "1";
                            return View(outwardModel);
                        }
                    }
                    else
                    {
                        outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                        outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                        outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                        outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                        outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                        outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                        if (outwardModel.Outward == null)
                            outwardModel.Outward = new OutwardGridModel();

                        outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                        outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                        outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                        outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                        outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                        outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(outwardModel);
                    }
                }
                else if (prdtype == "PIPE" || prdtype == "Pipe" || prdtype == "SEAMLESS PIPE" || prdtype == "seamless pipe")
                {
                    if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outwardModel.OutVNo).ToString()) && !string.IsNullOrWhiteSpace(outwardModel.OutDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outwardModel.OutAccVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outwardModel.OutPtyVou).ToString()) && outwardModel.Outward.OutAGrdPipe.Length > 0 && outwardModel.Outward.OutAGdnPipe.Length > 0 && outwardModel.Outward.SpecifiName.Length > 0 && outwardModel.Outward.OutAFinshPipe.Length > 0 && outwardModel.Outward.OutAProcePipe.Length > 0 && outwardModel.Outward.OutAThickPipe.Length > 0 && outwardModel.Outward.OutAOD.Length > 0 && outwardModel.Outward.OutAPcs.Length > 0 && outwardModel.Outward.OutALenghtPipe.Length > 0 && outwardModel.Outward.OutAQtyPipe.Length > 0)
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[22];
                        sqlParameters[0] = new SqlParameter("@OutVNo", outwardModel.OutVNo);
                        sqlParameters[1] = new SqlParameter("@OutDt", outwardModel.OutDt);
                        sqlParameters[2] = new SqlParameter("@OutRefNo", outwardModel.OutRefNo);
                        sqlParameters[3] = new SqlParameter("@OutAccVou", outwardModel.OutAccVou);
                        sqlParameters[4] = new SqlParameter("@OutPrdTyp", outwardModel.OutPtyVou);
                        sqlParameters[5] = new SqlParameter("@OutRem", outwardModel.OutRem);
                        sqlParameters[6] = new SqlParameter("@OutVou", id);
                        sqlParameters[7] = new SqlParameter("@OutPOMVou", outwardModel.OutPOMVou);
                        sqlParameters[8] = new SqlParameter("@OutLRNo", outwardModel.OutLRNo);
                        sqlParameters[9] = new SqlParameter("@OutWPNo", outwardModel.OutWPNo);
                        sqlParameters[10] = new SqlParameter("@OutVehNo", outwardModel.OutVehNo);
                        sqlParameters[11] = new SqlParameter("@OutTransNm", outwardModel.OutTransNm);
                        sqlParameters[12] = new SqlParameter("@OutBillNo", outwardModel.OutBillNo);
                        sqlParameters[13] = new SqlParameter("@UsrVou", userId);
                        sqlParameters[14] = new SqlParameter("@FLG", 1);
                        sqlParameters[15] = new SqlParameter("@CmpVou", outwardModel.OutCmpVou);
                        sqlParameters[16] = new SqlParameter("@OutPrdVou", outwardModel.OutPrdVou);
                        sqlParameters[17] = new SqlParameter("@OutTrnVou", outwardModel.OutTrnVou);
                        sqlParameters[18] = new SqlParameter("@OutFrtRt", outwardModel.OutFrightRt);
                        sqlParameters[19] = new SqlParameter("@OutCoilTypeVou", outwardModel.OutCoilTypeVou);
                        sqlParameters[20] = new SqlParameter("@OutCoilType", outwardModel.CoilType);
                        sqlParameters[21] = new SqlParameter("@OutWPWeight", outwardModel.OutWPWeight);
                        DataTable DtOutMst = ObjDBConnection.CallStoreProcedure("OutwardMst_Insert", sqlParameters);
                        if (DtOutMst != null && DtOutMst.Rows.Count > 0)
                        {
                            int masterId = DbConnection.ParseInt32(DtOutMst.Rows[0][0].ToString());
                            if (masterId > 0)
                            {
                                for (int i = 0; i < outwardModel.Outward.SpecifiName.Length; i++)
                                {
                                    if (outwardModel.Outward.OutAThickPipe[i] != null && outwardModel.Outward.OutAThickPipe[i] != 0 && outwardModel.Outward.OutAThickPipe[i] > 0)
                                    {
                                        SqlParameter[] parameter = new SqlParameter[25];
                                        parameter[0] = new SqlParameter("@OutAOutVou", masterId);
                                        parameter[1] = new SqlParameter("@OutASrNo", (i + 1));
                                        parameter[2] = new SqlParameter("@OutAPrdVou", 0);
                                        parameter[3] = new SqlParameter("@OutAGrade", outwardModel.Outward.OutAGrdPipe[i]);
                                        parameter[4] = new SqlParameter("@OutASpacifi", outwardModel.Outward.SpecifiName[i]);
                                        parameter[5] = new SqlParameter("@OutAWidth", 0);
                                        parameter[6] = new SqlParameter("@OutAThick", outwardModel.Outward.OutAThickPipe[i]);
                                        parameter[7] = new SqlParameter("@OutAQty", outwardModel.Outward.OutAQtyPipe[i]);
                                        parameter[8] = new SqlParameter("@OutAPcs", outwardModel.Outward.OutAPcs[i]);
                                        if (outwardModel.Outward.OutARemksPipe[i] == "-1")
                                        {
                                            parameter[9] = new SqlParameter("@OutARem", "");
                                        }
                                        else
                                        {
                                            parameter[9] = new SqlParameter("@OutARem", outwardModel.Outward.OutARemksPipe[i]);
                                        }
                                        parameter[10] = new SqlParameter("@OutACmpVou", outwardModel.OutCmpVou);
                                        parameter[11] = new SqlParameter("@OutAPOTVou", 0);
                                        parameter[12] = new SqlParameter("@OutALotVou", 0);
                                        parameter[13] = new SqlParameter("@OutAGdnVou", outwardModel.Outward.OutAGdnPipe[i]);
                                        parameter[14] = new SqlParameter("@OutALocVou", "0");
                                        //:                                        }
                                        parameter[15] = new SqlParameter("@OutASupCoilNo", "");
                                        parameter[16] = new SqlParameter("@OutAHeatNo", "");
                                        parameter[17] = new SqlParameter("@OutAOD", outwardModel.Outward.OutAOD[i]);
                                        parameter[18] = new SqlParameter("@OutALenght", outwardModel.Outward.OutALenghtPipe[i]);
                                        parameter[19] = new SqlParameter("@OutAFinish", outwardModel.Outward.OutAFinshPipe[i]);
                                        parameter[20] = new SqlParameter("@OutAProcess", outwardModel.Outward.OutAProcePipe[i]);
                                        parameter[21] = new SqlParameter("@OutACoilPrefix", "");
                                        parameter[22] = new SqlParameter("@OutACoilNo", "");
                                        parameter[23] = new SqlParameter("@OutASufix", "");
                                        parameter[24] = new SqlParameter("@TCUpload", outwardModel.Outward.ProfilePicturePipe[i]);
                                        DataTable DtOutTrn = ObjDBConnection.CallStoreProcedure("OutwardTrn_Insert", parameter);
                                    }
                                }
                                int Status = DbConnection.ParseInt32(DtOutMst.Rows[0][0].ToString());
                                if (Status == 0)
                                {
                                    outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                    outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                    outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                    outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                    outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                    outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();
                                    if (outwardModel.Outward == null)
                                        outwardModel.Outward = new OutwardGridModel();

                                    outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                                    outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                                    SetErrorMessage("Dulplicate Vou.No Details");
                                    ViewBag.FocusType = "1";
                                    return View(outwardModel);
                                }
                                else
                                {
                                    if (id > 0)
                                    {
                                        SetSuccessMessage("Updated Sucessfully");
                                        if (outwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = outwardModel.isPrint;
                                        }
                                    }
                                    else
                                    {
                                        SetSuccessMessage("Inserted Sucessfully");
                                        if (outwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = outwardModel.isPrint;
                                        }
                                    }
                                    if (outwardModel.isPrint == 0)
                                        return RedirectToAction("Index", "Outward", new { id = 0 });
                                }
                            }
                            else
                            {
                                outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                                if (outwardModel.Outward == null)
                                    outwardModel.Outward = new OutwardGridModel();

                                outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                                outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                                outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                                SetErrorMessage("Insert error");
                                ViewBag.FocusType = "1";
                                return View(outwardModel);
                            }
                        }
                        else
                        {
                            outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                            outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                            outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                            outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                            outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                            outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                            if (outwardModel.Outward == null)
                                outwardModel.Outward = new OutwardGridModel();

                            outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                            outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                            outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                            outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                            outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                            outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                            SetErrorMessage("Please Enter the Value");
                            ViewBag.FocusType = "1";
                            return View(outwardModel);
                        }
                    }
                    else
                    {
                        outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                        outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                        outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                        outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                        outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                        outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                        if (outwardModel.Outward == null)
                            outwardModel.Outward = new OutwardGridModel();

                        outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                        outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                        outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                        outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                        outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                        outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(outwardModel);

                    }

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outwardModel.OutVNo).ToString()) && !string.IsNullOrWhiteSpace(outwardModel.OutDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outwardModel.OutAccVou).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(outwardModel.OutPtyVou).ToString()) && outwardModel.Outward.OutAPrdVou.Length > 0 && outwardModel.Outward.OutAGdnOther.Length > 0 && outwardModel.Outward.OutAQtyOther.Length > 0)
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[22];
                        sqlParameters[0] = new SqlParameter("@OutVNo", outwardModel.OutVNo);
                        sqlParameters[1] = new SqlParameter("@OutDt", outwardModel.OutDt);
                        sqlParameters[2] = new SqlParameter("@OutRefNo", outwardModel.OutRefNo);
                        sqlParameters[3] = new SqlParameter("@OutAccVou", outwardModel.OutAccVou);
                        sqlParameters[4] = new SqlParameter("@OutPrdTyp", outwardModel.OutPtyVou);
                        sqlParameters[5] = new SqlParameter("@OutRem", outwardModel.OutRem);
                        sqlParameters[6] = new SqlParameter("@OutVou", id);
                        sqlParameters[7] = new SqlParameter("@OutPOMVou", outwardModel.OutPOMVou);
                        sqlParameters[8] = new SqlParameter("@OutLRNo", outwardModel.OutLRNo);
                        sqlParameters[9] = new SqlParameter("@OutWPNo", outwardModel.OutWPNo);
                        sqlParameters[10] = new SqlParameter("@OutVehNo", outwardModel.OutVehNo);
                        sqlParameters[11] = new SqlParameter("@OutTransNm", outwardModel.OutTransNm);
                        sqlParameters[12] = new SqlParameter("@OutBillNo", outwardModel.OutBillNo);
                        sqlParameters[13] = new SqlParameter("@UsrVou", userId);
                        sqlParameters[14] = new SqlParameter("@FLG", 1);
                        sqlParameters[15] = new SqlParameter("@CmpVou", outwardModel.OutCmpVou);
                        sqlParameters[16] = new SqlParameter("@OutPrdVou", outwardModel.OutPrdVou);
                        sqlParameters[17] = new SqlParameter("@OutTrnVou", outwardModel.OutTrnVou);
                        sqlParameters[18] = new SqlParameter("@OutFrtRt", outwardModel.OutFrightRt);
                        sqlParameters[19] = new SqlParameter("@OutCoilTypeVou", outwardModel.OutCoilTypeVou);
                        sqlParameters[20] = new SqlParameter("@OutCoilType", outwardModel.CoilType);
                        sqlParameters[21] = new SqlParameter("@OutWPWeight", outwardModel.OutWPWeight);
                        DataTable DtOutMst = ObjDBConnection.CallStoreProcedure("OutwardMst_Insert", sqlParameters);
                        if (DtOutMst != null && DtOutMst.Rows.Count > 0)
                        {
                            int masterId = DbConnection.ParseInt32(DtOutMst.Rows[0][0].ToString());
                            if (masterId > 0)
                            {
                                for (int i = 0; i < outwardModel.Outward.OutAPrdVou.Length; i++)
                                {
                                    if (outwardModel.Outward.OutAQtyOther[i] != null && outwardModel.Outward.OutAQtyOther[i] != 0 && outwardModel.Outward.OutAQtyOther[i] > 0)
                                    {
                                        SqlParameter[] parameter = new SqlParameter[25];
                                        parameter[0] = new SqlParameter("@OutAOutVou", masterId);
                                        parameter[1] = new SqlParameter("@OutASrNo", (i + 1));
                                        parameter[2] = new SqlParameter("@OutAPrdVou", outwardModel.Outward.OutAPrdVou[i]);
                                        parameter[3] = new SqlParameter("@OutAGrade", "");
                                        parameter[4] = new SqlParameter("@OutASpacifi", "");
                                        parameter[5] = new SqlParameter("@OutAWidth", "0");
                                        parameter[6] = new SqlParameter("@OutAThick", "0");
                                        parameter[7] = new SqlParameter("@OutAQty", outwardModel.Outward.OutAQtyOther[i]);
                                        parameter[8] = new SqlParameter("@OutAPcs", "0");
                                        if (outwardModel.Outward.OutARemksOther[i] == "-1")
                                        {
                                            parameter[9] = new SqlParameter("@OutARem", "");
                                        }
                                        else
                                        {
                                            parameter[9] = new SqlParameter("@OutARem", outwardModel.Outward.OutARemksOther[i]);
                                        }
                                        parameter[10] = new SqlParameter("@OutACmpVou", outwardModel.OutCmpVou);
                                        parameter[11] = new SqlParameter("@OutAPOTVou", "0");
                                        parameter[12] = new SqlParameter("@OutALotVou", "0");
                                        parameter[13] = new SqlParameter("@OutAGdnVou", outwardModel.Outward.OutAGdnOther[i]);
                                        //if (outwardModel.Outward.OutALocOther != null && outwardModel.Outward.OutALocOther.Length> 0)
                                        //{
                                        //    parameter[15] = new SqlParameter("@OutALocVou", outwardModel.Outward.OutALocOther[i]);
                                        //}
                                        //else
                                        //{
                                        parameter[14] = new SqlParameter("@OutALocVou", "0");
                                        //}
                                        parameter[15] = new SqlParameter("@OutASupCoilNo", "");
                                        parameter[16] = new SqlParameter("@OutAHeatNo", "");
                                        parameter[17] = new SqlParameter("@OutAOD", "0");
                                        parameter[18] = new SqlParameter("@OutALenght", "0");
                                        parameter[10] = new SqlParameter("@OutAFinish", "0");
                                        parameter[20] = new SqlParameter("@OutAProcess", "0");
                                        parameter[21] = new SqlParameter("@OutACoilPrefix", "");
                                        parameter[22] = new SqlParameter("@OutACoilNo", "");
                                        parameter[23] = new SqlParameter("@OutASufix", "");
                                        parameter[24] = new SqlParameter("@TCUpload", outwardModel.Outward.ProfilePictureOther[i]);
                                        DataTable DtOutTrn = ObjDBConnection.CallStoreProcedure("OutwardTrn_Insert", parameter);
                                    }
                                }
                                int Status = DbConnection.ParseInt32(DtOutMst.Rows[0][0].ToString());
                                if (Status == 0)
                                {
                                    outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                    outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                    outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                    outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                    outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                    outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                    outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                                    if (outwardModel.Outward == null)
                                        outwardModel.Outward = new OutwardGridModel();

                                    outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                    outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                    outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                    outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                                    outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                                    outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                                    SetErrorMessage("Dulplicate Vou.No Details");
                                    ViewBag.FocusType = "1";
                                    return View(outwardModel);
                                }
                                else
                                {
                                    if (id > 0)
                                    {
                                        SetSuccessMessage("Updated Sucessfully");
                                        if (outwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = outwardModel.isPrint;
                                        }
                                    }
                                    else
                                    {
                                        SetSuccessMessage("Inserted Sucessfully");
                                        if (outwardModel.isPrint != 0)
                                        {
                                            TempData["ReturnId"] = Status;
                                            TempData["Savedone"] = "1";
                                            TempData["IsPrint"] = outwardModel.isPrint;
                                        }
                                    }
                                    if (outwardModel.isPrint == 0)
                                        return RedirectToAction("Index", "Outward", new { id = 0 });
                                }
                            }
                            else
                            {
                                outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                                outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                                outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                                outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                                outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                                outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                                outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();
                                if (outwardModel.Outward == null)
                                    outwardModel.Outward = new OutwardGridModel();

                                outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                                outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                                outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                                outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                                outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                                outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                                outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                                outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                                SetErrorMessage("Insert error");
                                ViewBag.FocusType = "1";
                                return View(outwardModel);
                            }
                        }
                        else
                        {
                            outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                            outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                            outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                            outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                            outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                            outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                            outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                            if (outwardModel.Outward == null)
                                outwardModel.Outward = new OutwardGridModel();

                            outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                            outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                            outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                            outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                            outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                            outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                            outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                            outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                            SetErrorMessage("Please Enter the Value");
                            ViewBag.FocusType = "1";
                            return View(outwardModel);
                        }
                    }
                    else
                    {
                        outwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
                        outwardModel.AccountList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0);
                        outwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
                        outwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
                        outwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
                        outwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "");
                        outwardModel.CoilTypeList = objProductHelper.GetInwardCoilType();

                        if (outwardModel.Outward == null)
                            outwardModel.Outward = new OutwardGridModel();

                        outwardModel.Outward.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        outwardModel.Outward.GradeListPipe = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
                        outwardModel.Outward.SpacificationList = ObjAccountMasterHelpers.GetSpacificationDropdown(companyId);
                        outwardModel.Outward.ProductList = objProductHelper.GetProductMasterDropdown(companyId);
                        outwardModel.Outward.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        outwardModel.Outward.GodownPipeList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        outwardModel.Outward.GodownOtherList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
                        outwardModel.Outward.FinishListPipe = objProductHelper.GetFinishMasterDropdown(companyId, administrator);
                        outwardModel.Outward.ProceListPipe = objProductHelper.GetLotPrcTypMasterDropdown(companyId, administrator);
                        outwardModel.Outward.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
                        outwardModel.OutDt = Convert.ToDateTime(outwardModel.OutDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(outwardModel);
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index");
        }

        public OutwardModel GetOrderListByAccount(string OutPOMVou)
        {
            OutwardModel obj = new OutwardModel();
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
                                obj.OutwardList = new List<OutwardGridModel>();
                                for (int i = 0; i < dtUniq.Rows.Count; i++)
                                {
                                    if (prdType == "COIL" || prdType == "Coil")
                                    {
                                        obj.OutwardList.Add(new OutwardGridModel()
                                        {
                                            OutAGrdCoilStr = dtUniq.Rows[i]["OrdGrade"].ToString(),
                                            OutAThickCoilStr = dtUniq.Rows[i]["OrdThick"].ToString(),
                                            OutAWidthStr = dtUniq.Rows[i]["OrdWidth"].ToString(),
                                            OutAQtyCoilStr = dtUniq.Rows[i]["OrdQty"].ToString(),
                                            OutARemksCoilStr = dtUniq.Rows[i]["OrdRem"].ToString()
                                        });
                                    }
                                    else if (prdType == "PIPE" || prdType == "Pipe" || prdType == "SEAMLESS PIPE" || prdType == "seamless pipe")
                                    {
                                        obj.OutwardList.Add(new OutwardGridModel()
                                        {
                                            SpecifiNameStr = dtUniq.Rows[i]["OrdSpacifi"].ToString(),
                                            OutAGrdPipeStr = dtUniq.Rows[i]["OrdGrade"].ToString(),
                                            OutAThickPipeStr = dtUniq.Rows[i]["OrdThick"].ToString(),
                                            OutAODStr = dtUniq.Rows[i]["OrdOD"].ToString(),
                                            OutAPcsStr = dtUniq.Rows[i]["OrdPcs"].ToString(),
                                            OutARemksPipeStr = dtUniq.Rows[i]["OrdRem"].ToString()
                                        });

                                    }
                                    else
                                    {
                                        obj.OutwardList.Add(new OutwardGridModel()
                                        {
                                            OutAPrdVouStr = dtUniq.Rows[i]["OrdPrdVou"].ToString(),
                                            OutAQtyOtherStr = dtUniq.Rows[i]["OrdQty"].ToString(),
                                            OutARemksOtherStr = dtUniq.Rows[i]["OrdRem"].ToString(),
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

        public OutwardModel GetLastVoucherNo(int companyId)
        {
            OutwardModel obj = new OutwardModel();
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
                OutwardModel outwardModel = new OutwardModel();
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
                    DataTable DtOut = ObjDBConnection.CallStoreProcedure("GetOutwardDetails", sqlParameters);
                    if (DtOut != null && DtOut.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtOut.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Outward Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "Outward");
        }
        public IActionResult GetDeleteSupCoil(string supcoilno)
        {
            try
            {
                InTransInwardModel outwardModel = new InTransInwardModel();
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
                    string currentURL = "/Outward/Index";
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
                    getReportDataModel.ControllerName = "Outward";
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
                    var bytes = Excel(getReportDataModel, "Outward Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Outward.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Outward Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Outward.pdf");
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

        public IActionResult GetSupCoilCheck(string supcoil, string outvou, int srno1)
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
                sqlParameters[5] = new SqlParameter("@MainVou", outvou);
                DataTable DtOut = ObjDBConnection.CallStoreProcedure("GetLotMstDetails1", sqlParameters);
                if (DtOut != null && DtOut.Columns.Count > 1)
                {
                    if (DtOut.Rows.Count > 0)
                    {
                        string GdnVou = DtOut.Rows[0]["LotGdnVou"].ToString();
                        string Grade = DtOut.Rows[0]["LotGrade"].ToString();
                        string GrdVou = DtOut.Rows[0]["LotGrdMscVou"].ToString();
                        string Width = DtOut.Rows[0]["LotWidth"].ToString();
                        string Thick = DtOut.Rows[0]["LotThick"].ToString();
                        string Qty = DtOut.Rows[0]["LotQty"].ToString();
                        string HeatNo = DtOut.Rows[0]["LotHeatNo"].ToString();
                        return Json(new { result = true, gdnVou = GdnVou, grade = Grade, grdVou = GrdVou, width = Width, thick = Thick, qty = Qty, heatNo = HeatNo });

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
                //if (DtOut != null && DtOut.Columns.Count > 1)
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

        public IActionResult OutwardPrintDetials(long id, int copyType = 1)
        {
            OutwardPrintDetails obj = new OutwardPrintDetails();

            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                long userId = GetIntSession("UserId");


                //SqlParameter[] Param = new SqlParameter[1];
                //Param[0] = new SqlParameter("@OutVou", id);
                //DataTable DtLabel = ObjDBConnection.CallStoreProcedure("Insert_OutwardLabel", Param);

                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@OutVou", id);
                sqlParameters[1] = new SqlParameter("@Flg", 4);
                sqlParameters[2] = new SqlParameter("@cmpvou", companyId);
                DataTable DtBilty = ObjDBConnection.CallStoreProcedure("GetOutwardDetails", sqlParameters);
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

        [Route("/Outward/GetAccount-List")]
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

        [Route("/Outward/GetTransport-List")]
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
        public IActionResult RawOutwardPrintDetials(long id, int companyid, int copyType = 1)
        {
            try
            {
                OutwardPrintDetails obj = GetDetailsById(id);

                return View(obj);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult OutwardPrintDetials(long id)
        {
            try
            {
                OutwardPrintDetails obj = GetDetailsById(id);

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
                DataTable DtOutMst = ObjDBConnection.CallStoreProcedure("GetNewCoilNoForOutward", sqlParameters);
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

        public IActionResult OutwardSendMail(long id, string email = "")
        {
            try
            {
                OutwardPrintDetails obj = GetDetailsById(id);
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
        public IActionResult OutwardWhatApp(long id, string whatappNo = "")
        {
            try
            {
                OutwardPrintDetails obj = GetDetailsById(id);
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

        public OutwardPrintDetails GetDetailsById(long Id)
        {
            OutwardPrintDetails obj = new OutwardPrintDetails();

            try
            {
                StringBuilder sb = new StringBuilder();
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@OutVou", Id);
                sqlParameters[1] = new SqlParameter("@Flg", 5);
                sqlParameters[2] = new SqlParameter("@cmpvou", companyId);
                DataTable DtOutward = ObjDBConnection.CallStoreProcedure("GetOutwardDetails", sqlParameters);
                if (DtOutward != null && DtOutward.Rows.Count > 0)
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
                    if (DtOutward.Columns.Contains("DepAdd"))
                        CmpAdd = DtOutward.Rows[0]["DepAdd"].ToString();
                    if (DtOutward.Columns.Contains("DepEmail"))
                        CmpEmail = DtOutward.Rows[0]["DepEmail"].ToString();
                    if (DtOutward.Columns.Contains("DepVou"))
                        CmpVou = DtOutward.Rows[0]["DepVou"].ToString();
                    if (DtOutward.Columns.Contains("DepWeb"))
                        CmpWeb = DtOutward.Rows[0]["DepWeb"].ToString();
                    if (DtOutward.Columns.Contains("DepBusLine"))
                        CmpWeb = DtOutward.Rows[0]["DepBusLine"].ToString();

                    Layout = "RawMatOutward";
                    filename = "RawMatOutward.html";

                    using (StreamReader reader = new StreamReader(Path.Combine(path, filename)))
                    {
                        body = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        newbody = body.Replace("#*#*cmpAdd*#*#", CmpAdd.Replace(";", "<br>"));
                        newbody = newbody.Replace("#*#*cmpEmail*#*#", CmpEmail);
                        newbody = newbody.Replace("#*#*cmpWeb*#*#", CmpWeb);

                        string BilDate = DateTime.Parse(DtOutward.Rows[0]["OutDt"].ToString()).ToString("dd-MM-yyyy");
                        newbody = newbody.Replace("#*#*r1*#*#", BilDate);
                        newbody = newbody.Replace("#*#*r2*#*#", DtOutward.Rows[0]["AccNm"].ToString());
                        newbody = newbody.Replace("#*#*r3*#*#", DtOutward.Rows[0]["OutVehNo"].ToString());
                        newbody = newbody.Replace("#*#*r4*#*#", "");
                        newbody = newbody.Replace("#*#*logo*#*#", !string.IsNullOrWhiteSpace(DtOutward.Rows[0]["DepLogo"].ToString()) ? "/Uploads/" + DtOutward.Rows[0]["DepLogo"].ToString() + "" : string.Empty);


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
                        for (int i = 0; i < DtOutward.Rows.Count; i++)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + (i + 1) + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtOutward.Rows[i]["LotCoilNo"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtOutward.Rows[i]["OutAHeatNo"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtOutward.Rows[i]["OutAWidth"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtOutward.Rows[i]["OutAThick"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\">" + DtOutward.Rows[i]["OutAQty"].ToString() + "</td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"center\" style=\"font-size:14px;\"></td>");
                            sb.Append("<td align=\"left\" style=\"font-size:14px;\">" + DtOutward.Rows[i]["OutARem"].ToString() + "</td>");
                            dTotWT += Convert.ToDouble(DtOutward.Rows[i]["OutAQty"].ToString());
                            sb.Append("</tr>");
                        }

                        for (int i = DtOutward.Rows.Count; i < 11; i++)
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