using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SUNMark.Classes;
using SUNMark.Models;

namespace SUNMark.Controllers
{
    public class PipeMasterController : BaseController
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
                CoilMasterModel coilmasterModel = new CoilMasterModel();
                if (id > 0)
                {
                    //coilmasterModel.LotVou = Convert.ToInt32(id);
                    //SqlParameter[] sqlParameters = new SqlParameter[3];
                    //sqlParameters[0] = new SqlParameter("@LotVou", id);
                    //sqlParameters[1] = new SqlParameter("@Flg", 2);
                    //sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    //DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetVoucherTypeDetails", sqlParameters);
                    //if (DtEmp != null && DtEmp.Rows.Count > 0)
                    //{
                    //    coilmasterModel.VchType = DtEmp.Rows[0]["VchType"].ToString();
                    //    coilmasterModel.VchTrnMscVou = Convert.ToInt32(DtEmp.Rows[0]["VchTrnMscVou"].ToString());
                    //    coilmasterModel.VchDesc = DtEmp.Rows[0]["VchDesc"].ToString();
                    //}
                }
                //coilmasterModel.CoilTypeList = objProductHelper.GetCoilType();
                //coilmasterModel.GradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                //coilmasterModel.QualityList = objProductHelper.GetQualityMasterDropdown(companyId, administrator);
                //coilmasterModel.FinishList = objProductHelper.GetFinishMasterDropdown(companyId, administrator);

                return View(coilmasterModel);
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
    }
}
