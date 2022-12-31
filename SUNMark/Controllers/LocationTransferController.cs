using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SUNMark.Classes;
using SUNMark.Models;

namespace SUNMark.Controllers
{
    public class LocationTransferController : BaseController
    {

        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();

        public IActionResult Index(int id)
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
                ViewBag.id = id;
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
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

            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            ViewBag.godownList = objProductHelper.GetGoDownMasterDropdown(companyId, administrator);
            ViewBag.locationList = objProductHelper.GetLocationMasterDropdown(companyId, administrator);
            ViewBag.productTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
            ViewBag.lotList = objProductHelper.GetLotMasterDropdown_1(companyId, administrator);
            ViewBag.productList = objProductHelper.GetProductMasterDropdown(companyId);
            ViewBag.companyList= objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

            #endregion
        }
    }
}
