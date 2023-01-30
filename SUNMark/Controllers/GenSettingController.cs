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
    public class GenSettingController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
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
                GenSettingModel GenSettingModel = new GenSettingModel();
                if (true)
                {
                    GenSettingModel.GenVou = Convert.ToInt64(1);
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@Flg", 2);
                    sqlParameters[1] = new SqlParameter("@GenCmpVou", companyId);
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetSetGenSettingDetails", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        GenSettingModel.GenVou = Convert.ToInt32(DtEmp.Rows[0]["GenVou"].ToString());
                        GenSettingModel.GenEmail = DtEmp.Rows[0]["GenEmail"].ToString();
                        GenSettingModel.GenPass = DtEmp.Rows[0]["GenPass"].ToString();
                        GenSettingModel.GenSMTP = Convert.ToInt32(DtEmp.Rows[0]["GenSMTP"].ToString());
                        GenSettingModel.GenWhtMob = DtEmp.Rows[0]["GenWhtMob"].ToString();
                        GenSettingModel.GenTokenID = DtEmp.Rows[0]["GenTokenID"].ToString();
                        GenSettingModel.GenInstID = DtEmp.Rows[0]["GenInstID"].ToString();
                    }
                }

                return View(GenSettingModel);
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
        public IActionResult Index(long id, GenSettingModel genSettingModel)
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
                if (!string.IsNullOrWhiteSpace(genSettingModel.GenEmail) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(genSettingModel.GenVou).ToString()))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@CtyName", GenSettingModel.CtyName);
                    sqlParameters[1] = new SqlParameter("@CtyStaVou", GenSettingModel.CtyStaVou);
                    sqlParameters[2] = new SqlParameter("@CtyState", GenSettingModel.State);
                    sqlParameters[3] = new SqlParameter("@CtyVou", id);
                    sqlParameters[4] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[5] = new SqlParameter("@FLG", "1");

                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("CityMst_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate City Details");
                            ViewBag.FocusType = "-1";
                            GenSettingModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(GenSettingModel);
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
                            return RedirectToAction("index", "City", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                        GenSettingModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(GenSettingModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                    GenSettingModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(GenSettingModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new GenSettingModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                GenSettingModel GenSettingModel = new GenSettingModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@CtyName", "");
                    sqlParameters[1] = new SqlParameter("@CtyStaVou", "0");
                    sqlParameters[2] = new SqlParameter("@CtyState", "");
                    sqlParameters[3] = new SqlParameter("@CtyVou", id);
                    sqlParameters[4] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[5] = new SqlParameter("@FLG", "2");
                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("CityMst_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("City Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "City");
        }
    }
}
