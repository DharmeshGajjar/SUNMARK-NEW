using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SUNMark.Classes;
using SUNMark.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Controllers
{
    public class CoilRegisterController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        private readonly IWebHostEnvironment _iwebhostenviroment;

        public CoilRegisterController(IWebHostEnvironment iwebhostenviroment)
        {
            _iwebhostenviroment = iwebhostenviroment;
        }

        public IActionResult Index()
        {
            CoilRegisterPrintDetails coilMasterModel = new CoilRegisterPrintDetails();
            try
            {
                bool isreturn = false;
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                long yearId = GetIntSession("YearId");
                int administrator = 0;
                var yearData = DbConnection.GetYearListByCompanyId(Convert.ToInt32(companyId)).Where(x => x.YearVou == yearId).FirstOrDefault();
                if (yearData != null)
                {
                    coilMasterModel.FrDt = yearData.StartDate;
                    coilMasterModel.ToDt = yearData.EndDate;
                }
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                coilMasterModel.GradeList = objProductHelper.GetGradeMasterDropdown(companyId, administrator);
                coilMasterModel.CompanyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(coilMasterModel);

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
                ViewBag.layoutList = GetGridLayoutDropDown(DbConnection.GridTypeReport, userFormRights.ModuleId);
                ViewBag.pageNoList = GetPageNo();
            }

            #endregion
        }
    }
}
