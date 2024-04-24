using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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
    public class TransactionGridController : BaseController
    {
        #region Private Variables

        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers ObjProductHelpers = new ProductHelpers();
        #endregion

        #region Constructor


        #endregion

        #region Public Methods

        [HttpGet]
        public IActionResult Index(int? id)
        {
            TransactionGridModel transactionGridModel = new TransactionGridModel();
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                if (id.HasValue && id.Value > 0)
                {
                    SqlParameter[] sqlParametersNew = new SqlParameter[5];
                    sqlParametersNew[0] = new SqlParameter("@GridTransactionId", id.Value);
                    sqlParametersNew[1] = new SqlParameter("@Flg", 2);
                    sqlParametersNew[2] = new SqlParameter("@skiprecord", "0");
                    sqlParametersNew[3] = new SqlParameter("@pagesize", "0");
                    sqlParametersNew[4] = new SqlParameter("@searchvalue", "0");

                    DataSet DsGridMaster = ObjDBConnection.GetDataSet("GetTransactionGridDetails", sqlParametersNew);
                    if (DsGridMaster != null && DsGridMaster.Tables != null && DsGridMaster.Tables.Count > 0)
                    {
                        DataTable dtMst = DsGridMaster.Tables[0];
                        DataTable dtDetails = DsGridMaster.Tables[1];
                        if (dtMst != null && dtMst.Rows.Count > 0)
                        {
                            transactionGridModel.FKMenuId = int.Parse(Convert.ToString(dtMst.Rows[0]["FKMenuId"]));
                            transactionGridModel.GridTransactionId = int.Parse(Convert.ToString(dtMst.Rows[0]["TransactionGridId"]));
                            transactionGridModel.LayoutName = Convert.ToString(dtMst.Rows[0]["LayoutName"]);
                            transactionGridModel.TableId = Convert.ToString(dtMst.Rows[0]["TableId"]);
                            transactionGridModel.Style = Convert.ToString(dtMst.Rows[0]["Style"]);
                            transactionGridModel.TransactionList = new List<TransactionGridTransactionModel>();

                            if (dtDetails != null && dtDetails.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dtDetails.Rows)
                                {
                                    transactionGridModel.TransactionList.Add(new TransactionGridTransactionModel
                                    {
                                        Align = int.Parse(Convert.ToString(dr["Align"])),
                                        CanGridYN = Convert.ToString(dr["CanGrow"]).ToLower() == "true" ? true : false,
                                        HideYN = Convert.ToString(dr["Hide"]).ToLower() == "true" ? true : false,
                                        TotalYN = Convert.ToString(dr["Total"]).ToLower() == "true" ? true : false,
                                        Decimal = int.Parse(Convert.ToString(dr["Decimal"])),
                                        Position = int.Parse(Convert.ToString(dr["Position"])),
                                        Width = int.Parse(Convert.ToString(dr["Width"])),
                                        Type = int.Parse(Convert.ToString(dr["Type"])),
                                        FieldName = Convert.ToString(dr["FieldName"]),
                                        Style = Convert.ToString(dr["Style"]),
                                        SupressValue = Convert.ToString(dr["SupressValue"]),
                                        TableHeaderName = Convert.ToString(dr["HeaderName"]),
                                        TransactionId = int.Parse(Convert.ToString(dr["TransactionGridTransactionId"])),
                                    });
                                }
                            }
                        }
                    }
                }

                transactionGridModel.AlignmentList = ObjProductHelpers.GetTextAlign();
                transactionGridModel.MenuList = ObjProductHelpers.GetMenuMasterDropdown();
                transactionGridModel.TypeList = ObjProductHelpers.GetTypeList();
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(transactionGridModel);
        }

        [HttpPost]
        public IActionResult Index(TransactionGridModel transactionGridModel)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                string gridXML = string.Empty;

                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@TransactionId", transactionGridModel.GridTransactionId);
                sqlParameters[1] = new SqlParameter("@FKMenuId", transactionGridModel.FKMenuId);
                sqlParameters[2] = new SqlParameter("@LayoutName", transactionGridModel.LayoutName);
                sqlParameters[3] = new SqlParameter("@Style", transactionGridModel.Style);
                sqlParameters[4] = new SqlParameter("@TableId", transactionGridModel.TableId);
                sqlParameters[5] = new SqlParameter("@GridDetails", GetDetailXML(transactionGridModel));
                DataTable dtSetGrid = ObjDBConnection.CallStoreProcedure("AddUpdateTransactionGrid", sqlParameters);
                if (dtSetGrid != null && dtSetGrid.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dtSetGrid.Rows[0][0]) > 0)
                        SetSuccessMessage("Data saved successfully");
                    else
                        SetErrorMessage("Data not saved");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                transactionGridModel.AlignmentList = ObjProductHelpers.GetTextAlign();
                transactionGridModel.MenuList = ObjProductHelpers.GetMenuMasterDropdown();
                transactionGridModel.TypeList = ObjProductHelpers.GetTypeList();
            }
            return RedirectToAction("Index", new { id = "0" });
        }

        [HttpPost]
        public IActionResult GetData()
        {
            StringValues draw, orderColumn, orderDirection, startRecord, pageSize = "10", searchText = string.Empty;
            List<TransactionGridDetailModel> detailList = new List<TransactionGridDetailModel>();
            int totalRecord = 0;
            try
            {
                Request.Form.TryGetValue("draw", out draw);
                Request.Form.TryGetValue("order[0][column]", out orderColumn);
                Request.Form.TryGetValue("order[0][dir]", out orderDirection);
                Request.Form.TryGetValue("start", out startRecord);
                Request.Form.TryGetValue("length", out pageSize);
                Request.Form.TryGetValue("search[value]", out searchText);

                string searchValue = !string.IsNullOrWhiteSpace(searchText.ToString()) ? searchText.ToString() : null;

                SqlParameter[] sqlParametersNew = new SqlParameter[5];
                sqlParametersNew[0] = new SqlParameter("@GridTransactionId", 0);
                sqlParametersNew[1] = new SqlParameter("@Flg", 2);
                sqlParametersNew[2] = new SqlParameter("@skiprecord", startRecord.ToString());
                sqlParametersNew[3] = new SqlParameter("@pagesize", pageSize.ToString());
                sqlParametersNew[4] = new SqlParameter("@searchvalue", searchValue);

                DataTable DtGridMaster = ObjDBConnection.CallStoreProcedure("GetTransactionGridDetails", sqlParametersNew);
                if (DtGridMaster != null && DtGridMaster.Rows.Count > 0)
                {
                    totalRecord = int.Parse(Convert.ToString(DtGridMaster.Rows[0]["MaxRows"]));
                    for (int i = 0; i < DtGridMaster.Rows.Count; i++)
                    {
                        TransactionGridDetailModel transactionModel = new TransactionGridDetailModel();
                        transactionModel.GridTransactionId = DbConnection.ParseInt32(DtGridMaster.Rows[i]["TransactionGridId"].ToString());
                        transactionModel.FKMenuId = DbConnection.ParseInt32(DtGridMaster.Rows[i]["FKMenuId"].ToString());
                        transactionModel.MenuName = DtGridMaster.Rows[i]["MenuName"].ToString();
                        transactionModel.LayoutName = DtGridMaster.Rows[i]["LayoutName"].ToString();
                        transactionModel.Style = DtGridMaster.Rows[i]["Style"].ToString();
                        detailList.Add(transactionModel);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new
            {
                draw = DbConnection.ParseInt32(draw),
                recordsTotal = DbConnection.ParseInt32(pageSize),
                recordsFiltered = totalRecord,
                data = detailList != null ? detailList : new List<TransactionGridDetailModel>(),
            });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                SqlParameter[] sqlParametersNew = new SqlParameter[5];
                sqlParametersNew[0] = new SqlParameter("@GridTransactionId", id);
                sqlParametersNew[1] = new SqlParameter("@Flg", 1);
                sqlParametersNew[2] = new SqlParameter("@skiprecord", "0");
                sqlParametersNew[3] = new SqlParameter("@pagesize", "0");
                sqlParametersNew[4] = new SqlParameter("@searchvalue", "0");

                DataTable dtSetGrid = ObjDBConnection.CallStoreProcedure("GetTransactionGridDetails", sqlParametersNew);
                if (dtSetGrid != null && dtSetGrid.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dtSetGrid.Rows[0][0]) > 0)
                        SetSuccessMessage("Data deleted successfully");
                    else
                        SetErrorMessage("Data not deleted");
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("Index", new { id = "0" });
        }

        [HttpGet]
        public JsonResult GetGridDetailsById(int Id)
        {
            TransactionGridModel transactionGridModel = new TransactionGridModel();
            try
            {
                if (Id > 0)
                {
                    SqlParameter[] sqlParametersNew = new SqlParameter[5];
                    sqlParametersNew[0] = new SqlParameter("@GridTransactionId", Id);
                    sqlParametersNew[1] = new SqlParameter("@Flg", 2);
                    sqlParametersNew[2] = new SqlParameter("@skiprecord", "0");
                    sqlParametersNew[3] = new SqlParameter("@pagesize", "0");
                    sqlParametersNew[4] = new SqlParameter("@searchvalue", "0");

                    DataSet DsGridMaster = ObjDBConnection.GetDataSet("GetTransactionGridDetails", sqlParametersNew);
                    if (DsGridMaster != null && DsGridMaster.Tables != null && DsGridMaster.Tables.Count > 0)
                    {
                        DataTable dtMst = DsGridMaster.Tables[0];
                        DataTable dtDetails = DsGridMaster.Tables[1];
                        if (dtMst != null && dtMst.Rows.Count > 0)
                        {
                            transactionGridModel.FKMenuId = int.Parse(Convert.ToString(dtMst.Rows[0]["FKMenuId"]));
                            transactionGridModel.GridTransactionId = int.Parse(Convert.ToString(dtMst.Rows[0]["TransactionGridId"]));
                            transactionGridModel.LayoutName = Convert.ToString(dtMst.Rows[0]["LayoutName"]);
                            transactionGridModel.Style = Convert.ToString(dtMst.Rows[0]["Style"]);
                            transactionGridModel.TableId = Convert.ToString(dtMst.Rows[0]["TableId"]);
                            transactionGridModel.TransactionList = new List<TransactionGridTransactionModel>();

                            if (dtDetails != null && dtDetails.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dtDetails.Rows)
                                {
                                    transactionGridModel.TransactionList.Add(new TransactionGridTransactionModel
                                    {
                                        Align = int.Parse(Convert.ToString(dr["Align"])),
                                        CanGridYN = Convert.ToString(dr["CanGrow"]).ToLower() == "true" ? true : false,
                                        HideYN = Convert.ToString(dr["Hide"]).ToLower() == "true" ? true : false,
                                        TotalYN = Convert.ToString(dr["Total"]).ToLower() == "true" ? true : false,
                                        Decimal = int.Parse(Convert.ToString(dr["Decimal"])),
                                        Position = int.Parse(Convert.ToString(dr["Position"])),
                                        Width = int.Parse(Convert.ToString(dr["Width"])),
                                        Type = int.Parse(Convert.ToString(dr["Type"])),
                                        FieldName = Convert.ToString(dr["FieldName"]),
                                        Style = Convert.ToString(dr["Style"]),
                                        SupressValue = Convert.ToString(dr["SupressValue"]),
                                        TableHeaderName = Convert.ToString(dr["HeaderName"]),
                                        TransactionId = int.Parse(Convert.ToString(dr["TransactionGridTransactionId"])),
                                    });
                                }
                            }
                            transactionGridModel.IsTotal = transactionGridModel.TransactionList.Where(x => x.TotalYN).Count() > 0 ? true : false;
                            transactionGridModel.TransactionList = transactionGridModel.TransactionList.OrderBy(x => x.Position).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(transactionGridModel);
        }
        #endregion

        #region Private Methods

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

        private string GetDetailXML(TransactionGridModel transactionGridModel)
        {
            string xml = "<PARAMS>";
            if (transactionGridModel != null && transactionGridModel.TransactionList != null && transactionGridModel.TransactionList.Count > 0)
            {
                foreach (var item in transactionGridModel.TransactionList)
                {
                    if (!item.Decimal.HasValue)
                        item.Decimal = 0;

                    xml = string.Concat(xml, "<PARAMETER HeaderName='", item.TableHeaderName, "' FieldName='", item.FieldName, "' Position='", item.Position, "' Width='", item.Width, "' Decimal='", item.Decimal, "' Align='" + item.Align + "' Total='", item.TotalYN, "' Hide='", item.HideYN, "' CanGrow='", item.CanGridYN, "' SupressValue='", item.SupressValue, "' Style='", item.Style, "' Type='", item.Type, "'></PARAMETER>");
                }
                xml = string.Concat(xml, " </PARAMS>");
            }
            return xml;
        }
        #endregion
    }
}
