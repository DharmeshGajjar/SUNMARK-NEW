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
                GenSettingModel genSettingModel = new GenSettingModel();
                if (true)
                {
                    genSettingModel.GenVou = Convert.ToInt64(1);
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@Flg", 2);
                    sqlParameters[1] = new SqlParameter("@GenCmpVou", companyId);
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetSetGenSettingDetails", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        genSettingModel.GenVou = Convert.ToInt32(DtEmp.Rows[0]["GenVou"].ToString());
                        genSettingModel.GenCmpVou = Convert.ToInt32(DtEmp.Rows[0]["GenCmpVou"].ToString());
                        genSettingModel.GenEmail = DtEmp.Rows[0]["GenEmail"].ToString();
                        genSettingModel.GenPass = DtEmp.Rows[0]["GenPass"].ToString();
                        genSettingModel.GenSMTP = Convert.ToInt32(DtEmp.Rows[0]["GenSMTP"].ToString());
                        genSettingModel.GenWhtMob = DtEmp.Rows[0]["GenWhtMob"].ToString();
                        genSettingModel.GenTokenID = DtEmp.Rows[0]["GenTokenID"].ToString();
                        genSettingModel.GenInstID = DtEmp.Rows[0]["GenInstID"].ToString();
                        genSettingModel.GenHost = DtEmp.Rows[0]["GenHost"].ToString();
                        genSettingModel.GenSkruApi = DtEmp.Rows[0]["GenSkruAPI"].ToString();
                        genSettingModel.GenSURL = DtEmp.Rows[0]["GenSURL"].ToString();
                    }
                }

                return View(genSettingModel);
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
                    SqlParameter[] sqlParameters = new SqlParameter[11];
                    sqlParameters[0] = new SqlParameter("@GenEmail", genSettingModel.GenEmail);
                    sqlParameters[1] = new SqlParameter("@GenPass", genSettingModel.GenPass);
                    sqlParameters[2] = new SqlParameter("@GenSMTP", genSettingModel.GenSMTP);
                    sqlParameters[3] = new SqlParameter("@GenWhtMob", genSettingModel.GenWhtMob);
                    sqlParameters[4] = new SqlParameter("@GenTokenID", genSettingModel.GenTokenID);
                    sqlParameters[5] = new SqlParameter("@GenInstID", genSettingModel.GenInstID);
                    sqlParameters[6] = new SqlParameter("@Flg", 1);
                    sqlParameters[7] = new SqlParameter("@GenCmpVou", companyId);
                    sqlParameters[8] = new SqlParameter("@GenHost", genSettingModel.GenHost);
                    sqlParameters[9] = new SqlParameter("@GenSkruAPI", genSettingModel.GenSkruApi);
                    sqlParameters[10] = new SqlParameter("@GenSURL", genSettingModel.GenSURL);

                    DataTable Dt = ObjDBConnection.CallStoreProcedure("GetSetGenSettingDetails", sqlParameters);
                    SetSuccessMessage("Saved Sucessfully");
                    return RedirectToAction("index", "GenSetting");
                    
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
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
                GenSettingModel genSettingModel = new GenSettingModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@Flg", 3);
                    sqlParameters[1] = new SqlParameter("@GenCmpVou", companyId);
                    DataTable Dt = ObjDBConnection.CallStoreProcedure("GetSetGenSettingDetails", sqlParameters);
                    if (Dt != null && Dt.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(Dt.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("General Setting Deleted Successfully");
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
