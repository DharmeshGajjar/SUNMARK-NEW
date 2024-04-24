using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SUNMark.Classes;
using SUNMark.Common;
using SUNMark.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace SUNMark.Controllers
{
	public class JobInwardController : BaseController
	{
		DbConnection ObjDBConnection = new DbConnection();
		ProductHelpers objProductHelper = new ProductHelpers();
		TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
		AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
		private readonly IWebHostEnvironment _iwebhostenviroment;

		public JobInwardController(IWebHostEnvironment iwebhostenviroment)
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
				JobInwardModel jobinwardModel = new JobInwardModel();
				jobinwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
				jobinwardModel.TypeList = objProductHelper.GetType();
				jobinwardModel.AccountList = new List<CustomDropDown>();
				jobinwardModel.AccountList.Add(CommonHelpers.GetDefaultValue());
				jobinwardModel.AccountList.AddRange(ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0));
				jobinwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
				jobinwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeCoilDropdown(companyId);
				jobinwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
				jobinwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "Select");
				jobinwardModel.FrGodownList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

				jobinwardModel.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
				jobinwardModel.NBList = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
				jobinwardModel.SCHList = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);
				jobinwardModel.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
				jobinwardModel.IntCoilTypeList = objProductHelper.GetInwardCoilType();
				jobinwardModel.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
				SqlParameter[] sqlParam = new SqlParameter[3];
				sqlParam[0] = new SqlParameter("@InwVou", id);
				sqlParam[1] = new SqlParameter("@Flg", 3);
				sqlParam[2] = new SqlParameter("@CmpVou", companyId);
				DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetJobInwardDetails", sqlParam);
				if (DtInw != null && DtInw.Rows.Count > 0)
				{
					jobinwardModel.InwCmpVou = Convert.ToInt32(DtInw.Rows[0]["InwCmpVou"].ToString());
					jobinwardModel.InwDt = !string.IsNullOrWhiteSpace(DtInw.Rows[0]["InwDt"].ToString()) ? Convert.ToDateTime(DtInw.Rows[0]["InwDt"].ToString()).ToString("yyyy-MM-dd") : "";
					jobinwardModel.InwAccVou = Convert.ToInt32(DtInw.Rows[0]["InwAccVou"].ToString());
					int vno = DbConnection.ParseInt32(DtInw.Rows[0][0].ToString());
					jobinwardModel.InwVNo = vno + 1;
				}

				if (id > 0)
				{
					jobinwardModel.InwVou = Convert.ToInt32(id);
					SqlParameter[] sqlParameters = new SqlParameter[3];
					sqlParameters[0] = new SqlParameter("@InwVou", id);
					sqlParameters[1] = new SqlParameter("@Flg", 2);
					sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
					DataTable DtInwMst = ObjDBConnection.CallStoreProcedure("GetJobInwardDetails", sqlParameters);
					if (DtInwMst != null && DtInwMst.Rows.Count > 0)
					{
						jobinwardModel.InwVNo = Convert.ToInt32(DtInwMst.Rows[0]["InwVNo"].ToString());
						jobinwardModel.InwDt = !string.IsNullOrWhiteSpace(DtInwMst.Rows[0]["InwDt"].ToString()) ? Convert.ToDateTime(DtInwMst.Rows[0]["InwDt"].ToString()).ToString("yyyy-MM-dd") : "";
						jobinwardModel.InwAccVou = Convert.ToInt32(DtInwMst.Rows[0]["InwAccVou"].ToString());
						jobinwardModel.CtyName = DtInwMst.Rows[0]["City"].ToString();
						jobinwardModel.InwRefNo = DtInwMst.Rows[0]["InwRefNo"].ToString();
						jobinwardModel.InwBillDt = !string.IsNullOrWhiteSpace(DtInwMst.Rows[0]["InwBillDt"].ToString()) ? Convert.ToDateTime(DtInwMst.Rows[0]["InwBillDt"].ToString()).ToString("yyyy-MM-dd") : "";
						jobinwardModel.InwRem = DtInwMst.Rows[0]["InwRem"].ToString();
						jobinwardModel.InwCmpVou = Convert.ToInt32(DtInwMst.Rows[0]["InwCmpVou"].ToString());
						jobinwardModel.InwLRNo = DtInwMst.Rows[0]["InwLRNo"].ToString();
						jobinwardModel.InwWPNo = DtInwMst.Rows[0]["InwWpSlipNo"].ToString();
						jobinwardModel.InwWPWeight = DtInwMst.Rows[0]["InwWpWeight"].ToString();
						jobinwardModel.InwVehNo = DtInwMst.Rows[0]["InwVehNo"].ToString();
						jobinwardModel.InwTransNm = DtInwMst.Rows[0]["InwTransNm"].ToString();
						jobinwardModel.InwTrnVou = Convert.ToInt32(DtInwMst.Rows[0]["InwTrnVou"].ToString());
						jobinwardModel.InwFrightRt = Convert.ToDecimal(DtInwMst.Rows[0]["InwFrtRt"].ToString());
						jobinwardModel.InwBillNo = DtInwMst.Rows[0]["InwBillNo"].ToString();
						jobinwardModel.IssCoilNo= DtInwMst.Rows[0]["InwIssCoilNo"].ToString();
						jobinwardModel.InwFrGdnVou = Convert.ToInt32(DtInwMst.Rows[0]["InwFrGdnVou"].ToString());
						jobinwardModel.InwTypeVou = Convert.ToInt32(DtInwMst.Rows[0]["InwTypeVou"].ToString());
						jobinwardModel.Width = Convert.ToDecimal(DtInwMst.Rows[0]["Width"].ToString());
						jobinwardModel.Qty = Convert.ToDecimal(DtInwMst.Rows[0]["Qty"].ToString());
						jobinwardModel.Thick = Convert.ToDecimal(DtInwMst.Rows[0]["Thick"].ToString());

						List<TransactionGridAddUpdateRootModel> lstobl = new List<TransactionGridAddUpdateRootModel>();
						List<TransactionGridAddUpdateDataModel> dataLIstAnotherModel = new List<TransactionGridAddUpdateDataModel>();

						for (int i = 0; i < DtInwMst.Rows.Count; i++)
						{
							List<TransactionGridAddUpdateModel> dataList = new List<TransactionGridAddUpdateModel>();

							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntGdnVou",
								Value = Convert.ToString(DtInwMst.Rows[i]["IntGdnVou"].ToString())
							});

							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntPrefix",
								Value = !string.IsNullOrWhiteSpace(DtInwMst.Rows[i]["IntPrefix"].ToString()) ? Convert.ToString(DtInwMst.Rows[i]["IntPrefix"].ToString()) : ""
							});

							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntCoilNo",
								Value = Convert.ToString(DtInwMst.Rows[i]["IntCoilNo"].ToString())
							});

							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntSupCoilNo",
								Value = Convert.ToString(DtInwMst.Rows[i]["IntSupCoilNo"].ToString())
							});

							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntHeatNo",
								Value = Convert.ToString(DtInwMst.Rows[i]["IntHeatNo"].ToString())
							});

							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntCoilTypeVou",
								Value = Convert.ToString(DtInwMst.Rows[i]["IntCoilTypeVou"].ToString())
							});
							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntGrade",
								Value = Convert.ToString(DtInwMst.Rows[i]["IntGrade"].ToString())
							});
							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntWidth",
								Value = Convert.ToString(DtInwMst.Rows[i]["IntWidth"].ToString())
							});
							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntThick",
								Value = Convert.ToString(DtInwMst.Rows[i]["IntThick"].ToString())
							});
							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntQty",
								Value = Convert.ToString(DtInwMst.Rows[i]["IntQty"].ToString())
							});
							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntNB",
								Value = !string.IsNullOrWhiteSpace(DtInwMst.Rows[i]["IntNB"].ToString()) ? Convert.ToString(DtInwMst.Rows[i]["IntNB"].ToString()) : ""
							});
							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntSCH",
								Value = !string.IsNullOrWhiteSpace(DtInwMst.Rows[i]["IntSCH"].ToString()) ? Convert.ToString(DtInwMst.Rows[i]["IntSCH"].ToString()) : ""
							});
							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntOD",
								Value = Convert.ToString(DtInwMst.Rows[i]["IntOD"].ToString())
							});
							dataList.Add(new TransactionGridAddUpdateModel
							{
								Label = "IntRem",
								Value = Convert.ToString(DtInwMst.Rows[i]["IntRem"].ToString())
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
						jobinwardModel.Data = jsonString;


					}
				}
				if (id > 0)
					TempData["ReturnId"] = Convert.ToString(id);
				return View(jobinwardModel);
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
		public IActionResult Index(long id, JobInwardModel jobinwardModel)
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

				List<TransactionGridAddUpdateRootModel> transactionList = JsonConvert.DeserializeObject<List<TransactionGridAddUpdateRootModel>>(jobinwardModel.Data);

				jobinwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
				jobinwardModel.TypeList = objProductHelper.GetType();
				jobinwardModel.AccountList = new List<CustomDropDown>();
				jobinwardModel.AccountList.Add(CommonHelpers.GetDefaultValue());
				jobinwardModel.AccountList.AddRange(ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0));
				jobinwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
				jobinwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeCoilDropdown(companyId);
				jobinwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
				jobinwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "Select");
				jobinwardModel.FrGodownList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

				jobinwardModel.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
				jobinwardModel.NBList = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
				jobinwardModel.SCHList = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);
				jobinwardModel.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
				jobinwardModel.IntCoilTypeList = objProductHelper.GetInwardCoilType();
				jobinwardModel.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
				if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(jobinwardModel.InwVNo).ToString()) && !string.IsNullOrWhiteSpace(jobinwardModel.InwDt) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(jobinwardModel.InwAccVou).ToString()))
				{
					SqlParameter[] sqlParameters = new SqlParameter[25];
					sqlParameters[0] = new SqlParameter("@InwVNo", jobinwardModel.InwVNo);
					sqlParameters[1] = new SqlParameter("@InwDt", jobinwardModel.InwDt);
					sqlParameters[2] = new SqlParameter("@InwRefNo", jobinwardModel.InwRefNo);
					sqlParameters[3] = new SqlParameter("@InwAccVou", jobinwardModel.InwAccVou);
					sqlParameters[4] = new SqlParameter("@InwPrdTyp", jobinwardModel.InwPtyVou);
					sqlParameters[5] = new SqlParameter("@InwRem", jobinwardModel.InwRem);
					sqlParameters[6] = new SqlParameter("@InwVou", id);
					sqlParameters[7] = new SqlParameter("@InwPOMVou", jobinwardModel.InwPOMVou);
					sqlParameters[8] = new SqlParameter("@InwLRNo", jobinwardModel.InwLRNo);
					sqlParameters[9] = new SqlParameter("@InwWPNo", jobinwardModel.InwWPNo);
					sqlParameters[10] = new SqlParameter("@InwVehNo", jobinwardModel.InwVehNo);
					if (jobinwardModel.InwTransNm == "Select")
					{
						sqlParameters[11] = new SqlParameter("@InwTransNm", "");
					}
					else
					{
						sqlParameters[11] = new SqlParameter("@InwTransNm", jobinwardModel.InwTransNm);
					}
					sqlParameters[12] = new SqlParameter("@InwBillNo", jobinwardModel.InwBillNo);
					sqlParameters[13] = new SqlParameter("@UsrVou", userId);
					sqlParameters[14] = new SqlParameter("@FLG", 1);
					sqlParameters[15] = new SqlParameter("@CmpVou", jobinwardModel.InwCmpVou);
					sqlParameters[16] = new SqlParameter("@InwPrdVou", jobinwardModel.InwPrdVou);
					sqlParameters[17] = new SqlParameter("@InwTrnVou", jobinwardModel.InwTrnVou);
					sqlParameters[18] = new SqlParameter("@InwFrtRt", jobinwardModel.InwFrightRt);
					sqlParameters[19] = new SqlParameter("@InwCoilTypeVou", jobinwardModel.InwCoilTypeVou);
					sqlParameters[20] = new SqlParameter("@InwCoilType", jobinwardModel.CoilType);
					sqlParameters[21] = new SqlParameter("@InwWPWeight", jobinwardModel.InwWPWeight);
					sqlParameters[22] = new SqlParameter("@InwFrGdnVou", jobinwardModel.InwFrGdnVou);
					sqlParameters[23] = new SqlParameter("@InwIssCoilNo", jobinwardModel.IssCoilNo);
					sqlParameters[24] = new SqlParameter("@InwTypeVou", jobinwardModel.InwTypeVou);
					DataTable DtInwMst = ObjDBConnection.CallStoreProcedure("JobInwardMst_Insert", sqlParameters);
					if (DtInwMst != null && DtInwMst.Rows.Count > 0)
					{
						int masterId = DbConnection.ParseInt32(DtInwMst.Rows[0][0].ToString());
						if (masterId > 0)
						{
							for (int i = 0; i < transactionList[0].MyArray.Count; i++)
							{

								SqlParameter[] parameter = new SqlParameter[19];
								parameter[0] = new SqlParameter("@IntInwVou", masterId);
								parameter[1] = new SqlParameter("@IntSrNo", (i + 1));
								parameter[2] = new SqlParameter("@IntGrade", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntGrade"));
								parameter[3] = new SqlParameter("@IntWidth", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntWidth"));
								parameter[4] = new SqlParameter("@IntThick", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntThick"));
								parameter[5] = new SqlParameter("@IntQty", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntQty"));
								parameter[6] = new SqlParameter("@IntRem", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntRem"));
								parameter[7] = new SqlParameter("@IntCmpVou", companyId);
								parameter[8] = new SqlParameter("@IntLotVou", 0);
								parameter[9] = new SqlParameter("@IntGdnVou", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntGdnVou"));
								parameter[10] = new SqlParameter("@IntSupCoilNo", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntSupCoilNo"));
								parameter[11] = new SqlParameter("@IntHeatNo", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntHeatNo"));
								parameter[12] = new SqlParameter("@IntOD", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntOD"));
								parameter[13] = new SqlParameter("@IntLenght", 0);
								parameter[14] = new SqlParameter("@IntNB", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntNB"));
								parameter[15] = new SqlParameter("@IntSCH", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntSCH"));
								parameter[16] = new SqlParameter("@IntCoilPrefix", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntPrefix"));
								parameter[17] = new SqlParameter("@IntCoilNo", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntCoilNo"));
								parameter[18] = new SqlParameter("@IntCoilTypeVou", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "IntCoilTypeVou"));
								DataTable DtRec = ObjDBConnection.CallStoreProcedure("JobInwardTrn_Insert", parameter);
							}
							int Status = DbConnection.ParseInt32(DtInwMst.Rows[0][0].ToString());
							if (Status == 0)
							{
								SetErrorMessage("Dulplicate Vou.No Details");
							}
							else
							{
								if (id > 0)
								{
									SetSuccessMessage("Record updated succesfully!");
								}
								else
								{
									SetSuccessMessage("Record inserted succesfully!");
								}
								if (jobinwardModel.isPrint != 0)
								{
									TempData["ReturnId"] = id.ToString();
									TempData["Savedone"] = "1";
									TempData["IsPrint"] = jobinwardModel.isPrint.ToString();
									return RedirectToAction("Index", new { id = id });
								}
								

							}
						}
						else
						{
							jobinwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
							jobinwardModel.TypeList = objProductHelper.GetType();
							jobinwardModel.AccountList = new List<CustomDropDown>();
							jobinwardModel.AccountList.Add(CommonHelpers.GetDefaultValue());
							jobinwardModel.AccountList.AddRange(ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0));
							jobinwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
							jobinwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeCoilDropdown(companyId);
							jobinwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
							jobinwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "Select");
							jobinwardModel.FrGodownList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

							jobinwardModel.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
							jobinwardModel.NBList = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
							jobinwardModel.SCHList = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);
							jobinwardModel.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
							jobinwardModel.IntCoilTypeList = objProductHelper.GetInwardCoilType();
							jobinwardModel.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
							jobinwardModel.InwDt = Convert.ToDateTime(jobinwardModel.InwDt).ToString("yyyy-MM-dd");
							SetErrorMessage("Insert error");
							ViewBag.FocusType = "1";
							return View(jobinwardModel);
						}
					}
					else
					{
						jobinwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
						jobinwardModel.TypeList = objProductHelper.GetType();
						jobinwardModel.AccountList = new List<CustomDropDown>();
						jobinwardModel.AccountList.Add(CommonHelpers.GetDefaultValue());
						jobinwardModel.AccountList.AddRange(ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0));
						jobinwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
						jobinwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeCoilDropdown(companyId);
						jobinwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
						jobinwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "Select");
						jobinwardModel.FrGodownList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

						jobinwardModel.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
						jobinwardModel.NBList = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
						jobinwardModel.SCHList = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);
						jobinwardModel.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
						jobinwardModel.IntCoilTypeList = objProductHelper.GetInwardCoilType();
						jobinwardModel.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
						jobinwardModel.InwDt = Convert.ToDateTime(jobinwardModel.InwDt).ToString("yyyy-MM-dd");
						SetErrorMessage("Please Enter the Value");
						ViewBag.FocusType = "1";
						return View(jobinwardModel);
					}
				}
				else
				{
					jobinwardModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
					jobinwardModel.TypeList = objProductHelper.GetType();
					jobinwardModel.AccountList = new List<CustomDropDown>();
					jobinwardModel.AccountList.Add(CommonHelpers.GetDefaultValue());
					jobinwardModel.AccountList.AddRange(ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 0));
					jobinwardModel.TransportList = ObjTaxMasterHelpers.GetAccountCustomDropdown(companyId, 3);
					jobinwardModel.ProductTypeList = ObjAccountMasterHelpers.GetPrdTypeCoilDropdown(companyId);
					jobinwardModel.PurchaseOrderList = ObjAccountMasterHelpers.GetPurchaseOrderDropdown(companyId);
					jobinwardModel.MainProductList = objProductHelper.GetPrdTypeWiseProductDropdown(companyId, "Select");
					jobinwardModel.FrGodownList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);

					jobinwardModel.GradeList = ObjAccountMasterHelpers.GetGradeDropdown(companyId);
					jobinwardModel.NBList = ObjAccountMasterHelpers.GetNBMasterDropdown(companyId);
					jobinwardModel.SCHList = ObjAccountMasterHelpers.GetSCHMasterDropdown(companyId);
					jobinwardModel.GodownCoilList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
					jobinwardModel.IntCoilTypeList = objProductHelper.GetInwardCoilType();
					jobinwardModel.CoilPrefixList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
					jobinwardModel.InwDt = Convert.ToDateTime(jobinwardModel.InwDt).ToString("yyyy-MM-dd");
					SetErrorMessage("Please Enter the Value");
					ViewBag.FocusType = "1";
					return View(jobinwardModel);
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return RedirectToAction("Index", new { id = "0" });
		}

		public JobInwardModel GetLastVoucherNo(int companyId)
		{
			JobInwardModel obj = new JobInwardModel();
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[1];
				sqlParameters[0] = new SqlParameter("@Cmpvou", companyId);
				DataTable dtNewInwVNo = ObjDBConnection.CallStoreProcedure("GetLatestJobInwVNo", sqlParameters);
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

		public IActionResult Delete(long id)
		{
			try
			{
				JobInwardModel jobinwardModel = new JobInwardModel();
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
					DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetJobInwardDetails", sqlParameters);
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
			return RedirectToAction("index", "JobInward");
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
					sqlParameters[1] = new SqlParameter("@Type", "JINW");
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
					string currentURL = "/JobInward/Index";
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
					getReportDataModel.ControllerName = "JobInward";
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
		public IActionResult GetGodownParty(string frgdnvou)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(frgdnvou))
				{
					int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
					SqlParameter[] sqlParameters = new SqlParameter[3];
					sqlParameters[0] = new SqlParameter("@GdnVou", frgdnvou);
					sqlParameters[1] = new SqlParameter("@Flg", 2);
					sqlParameters[2] = new SqlParameter("@cmpvou", companyId);
					DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetGodownDetails", sqlParameters);
					if (DtInw != null && DtInw.Rows.Count > 0)
					{
						string accNm = DtInw.Rows[0]["AccNm"].ToString();
						string accVou = DtInw.Rows[0]["GdnAccVou"].ToString();
						string ctyNm = DtInw.Rows[0]["CtyNm"].ToString();
						return Json(new { result = true, accNm = accNm, ctyNm = ctyNm, accVou = accVou });
					}
					else
					{
						return Json(new { result = false, data = "Godown Not Found" });
					}
				}
				else
				{
					return Json(new { result = false, data = "Godown Not Found" });
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public IActionResult GetCoilNoCheck(string coilno, string inwvou, int srno1, string prefix)
		{
			try
			{
				int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
				SqlParameter[] sqlParameters = new SqlParameter[7];
				sqlParameters[0] = new SqlParameter("@CoilNo", coilno);
				sqlParameters[1] = new SqlParameter("@cmpvou", companyId);
				sqlParameters[2] = new SqlParameter("@Type", "JINW");
				sqlParameters[3] = new SqlParameter("@Vou", srno1);
				sqlParameters[4] = new SqlParameter("@Flg", "0");
				sqlParameters[5] = new SqlParameter("@MainVou", inwvou);
				sqlParameters[6] = new SqlParameter("@Prefix", prefix);
				DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetLotMstDetails2", sqlParameters);
				if (DtInw != null && DtInw.Columns.Count > 1)
				{
					if (DtInw.Rows.Count > 0)
					{
						return Json(new { result = true, data = "1" });
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
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public IActionResult GetSupCoilCheck(string maincoil, string frgdnvou, string dt)
		{
			try
			{
				int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
				SqlParameter[] sqlParameters = new SqlParameter[4];
				sqlParameters[0] = new SqlParameter("@FrGdnVou", frgdnvou);
				sqlParameters[1] = new SqlParameter("@cmpvou", companyId);
				sqlParameters[2] = new SqlParameter("@MainCoil", maincoil);
				sqlParameters[3] = new SqlParameter("@Dt", dt);
				DataTable DtInw = ObjDBConnection.CallStoreProcedure("GetLotMstJobInwDetails1", sqlParameters);
				if (DtInw != null && DtInw.Columns.Count > 1)
				{
					if (DtInw.Rows.Count > 0)
					{
						string Grade = DtInw.Rows[0]["LotGrade"].ToString();
						string GrdVou = DtInw.Rows[0]["LotGrdMscVou"].ToString();
						string Width = DtInw.Rows[0]["LotWidth"].ToString();
						string Thick = DtInw.Rows[0]["LotThick"].ToString();
						string supCoilNo = DtInw.Rows[0]["LotSupCoilNo"].ToString();
						string HeatNo = DtInw.Rows[0]["LotHeatNo"].ToString();
						string weight = DtInw.Rows[0]["LotQty"].ToString();
						return Json(new { result = true, grade = Grade, grdVou = GrdVou, width = Width, thick = Thick, supCoilNo = supCoilNo, heatNo = HeatNo, weight = weight });

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
						return Json(new { result = true, data = "1" });
					}
				}
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

		[Route("/JobInward/GetAccount-List")]
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

		[Route("/JobInward/GetTransport-List")]
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
