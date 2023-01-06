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
    public class MachineMasterController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        TaxMasterHelpers ObjtaxMasterHelpers = new TaxMasterHelpers();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();

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
                MachineMasterModel machineMasterModel = new MachineMasterModel();
                if (id > 0)
                {
                    machineMasterModel.MacVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@MacVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtMac = ObjDBConnection.CallStoreProcedure("GetMachineDetails", sqlParameters);
                    if (DtMac != null && DtMac.Rows.Count > 0)
                    {
                        machineMasterModel.MacTypeID = Convert.ToInt32(DtMac.Rows[0]["MacMtyMscVou"].ToString());
                        machineMasterModel.MachineType = DtMac.Rows[0]["MacMtyCd"].ToString();
                        machineMasterModel.MacName = DtMac.Rows[0]["MacNm"].ToString();
                        machineMasterModel.MacCd = DtMac.Rows[0]["MacCd"].ToString();
                        machineMasterModel.MacCapHr = Convert.ToInt32(DtMac.Rows[0]["MacCapHr"].ToString());
                        machineMasterModel.MacGdnVou = Convert.ToInt32(DtMac.Rows[0]["MacGdnVou"].ToString());
                        machineMasterModel.GdnName = DtMac.Rows[0]["MacGdnNm"].ToString();
                        machineMasterModel.MacLocVou = Convert.ToInt32(DtMac.Rows[0]["MacLocVou"].ToString());
                        machineMasterModel.LocName = DtMac.Rows[0]["MacLocNm"].ToString();
                        machineMasterModel.MacDesc = DtMac.Rows[0]["MacDesc"].ToString();
                        machineMasterModel.MacS1Opr1EmpVou = Convert.ToInt32(DtMac.Rows[0]["MacS1Opr1EmpVou"].ToString());
                        machineMasterModel.MacS1Opr2EmpVou = Convert.ToInt32(DtMac.Rows[0]["MacS1Opr2EmpVou"].ToString());
                        machineMasterModel.MacS1SuperEmpVou = Convert.ToInt32(DtMac.Rows[0]["MacS1SuperEmpVou"].ToString());
                        machineMasterModel.MacS2Opr1EmpVou = Convert.ToInt32(DtMac.Rows[0]["MacS2Opr1EmpVou"].ToString());
                        machineMasterModel.MacS2Opr2EmpVou = Convert.ToInt32(DtMac.Rows[0]["MacS2Opr2EmpVou"].ToString());
                        machineMasterModel.MacS2SuperEmpVou = Convert.ToInt32(DtMac.Rows[0]["MacS2SuperEmpVou"].ToString());
                        machineMasterModel.MacSizeRngFr = Convert.ToDecimal(DtMac.Rows[0]["MacSizeRngFr"].ToString());
                        machineMasterModel.MacSizeRngTo = Convert.ToDecimal(DtMac.Rows[0]["MacSizeRngTo"].ToString());
                    }
                }
                machineMasterModel.MachineTypeList = ObjtaxMasterHelpers.GetMachineType(companyId);
                machineMasterModel.GodownList = ObjtaxMasterHelpers.GetGodownDropdown(companyId);
                machineMasterModel.LocationList = ObjtaxMasterHelpers.GetLocationDropdown(companyId);
                machineMasterModel.EmployeeListS1Opr1 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId,0);
                machineMasterModel.EmployeeListS1Opr2 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                machineMasterModel.EmployeeSuperList1 = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);
                machineMasterModel.EmployeeListS2Opr1 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                machineMasterModel.EmployeeListS2Opr2 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                machineMasterModel.EmployeeSuperList2 = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);

                return View(machineMasterModel);

            }
            catch (Exception)
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
        public IActionResult Index(long id, MachineMasterModel machineMasterModel)
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
                if (!string.IsNullOrWhiteSpace(machineMasterModel.MacCd) && !string.IsNullOrWhiteSpace(machineMasterModel.MacName) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(machineMasterModel.MacTypeID).ToString()) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(machineMasterModel.MacGdnVou).ToString()))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[20];
                    sqlParameters[0] = new SqlParameter("@MacType", machineMasterModel.MachineType);
                    sqlParameters[1] = new SqlParameter("@MacMtyMscVou", machineMasterModel.MacTypeID);
                    sqlParameters[2] = new SqlParameter("@MacName", machineMasterModel.MacName);
                    sqlParameters[3] = new SqlParameter("@MacCode", machineMasterModel.MacCd);
                    sqlParameters[4] = new SqlParameter("@MacCapHr", machineMasterModel.MacCapHr);
                    sqlParameters[5] = new SqlParameter("@MacGdnVou", machineMasterModel.MacGdnVou);
                    sqlParameters[6] = new SqlParameter("@MacGdnNm", machineMasterModel.GdnName);
                    sqlParameters[7] = new SqlParameter("@MacLocVou", machineMasterModel.MacLocVou);
                    sqlParameters[8] = new SqlParameter("@MacLocNm", machineMasterModel.LocName);
                    sqlParameters[9] = new SqlParameter("@MacS1Opr1EmpVou", machineMasterModel.MacS1Opr1EmpVou);
                    sqlParameters[10] = new SqlParameter("@MacS1Opr2EmpVou", machineMasterModel.MacS1Opr2EmpVou);
                    sqlParameters[11] = new SqlParameter("@MacS1SuperEmpVou", machineMasterModel.MacS1SuperEmpVou);
                    sqlParameters[12] = new SqlParameter("@MacS2Opr1EmpVou", machineMasterModel.MacS2Opr1EmpVou);
                    sqlParameters[13] = new SqlParameter("@MacS2Opr2EmpVou", machineMasterModel.MacS2Opr2EmpVou);
                    sqlParameters[14] = new SqlParameter("@MacS2SuperEmpVou", machineMasterModel.MacS2SuperEmpVou);
                    sqlParameters[15] = new SqlParameter("@MacVou", id);
                    sqlParameters[16] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[17] = new SqlParameter("@FLG", "1");
                    sqlParameters[18] = new SqlParameter("@MacSizeRngFr", machineMasterModel.MacSizeRngFr);
                    sqlParameters[19] = new SqlParameter("@MacSizeRngTo", machineMasterModel.MacSizeRngTo);

                    DataTable DtMachi = ObjDBConnection.CallStoreProcedure("MachineMst_Insert", sqlParameters);
                    if (DtMachi != null && DtMachi.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtMachi.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Code Details");
                            ViewBag.FocusType = "-1";
                            machineMasterModel.MachineTypeList = ObjtaxMasterHelpers.GetMachineType(companyId);
                            machineMasterModel.GodownList = ObjtaxMasterHelpers.GetGodownDropdown(companyId);
                            machineMasterModel.LocationList = ObjtaxMasterHelpers.GetLocationDropdown(companyId);
                            machineMasterModel.EmployeeListS1Opr1 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                            machineMasterModel.EmployeeListS1Opr2 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                            machineMasterModel.EmployeeSuperList1 = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);
                            machineMasterModel.EmployeeListS2Opr1 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                            machineMasterModel.EmployeeListS2Opr2 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                            machineMasterModel.EmployeeSuperList2 = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);
                            return View(machineMasterModel);
                        }
                        else if (status == -2)
                        {
                            SetErrorMessage("Dulplicate Name Details");
                            ViewBag.FocusType = "-2";
                            machineMasterModel.MachineTypeList = ObjtaxMasterHelpers.GetMachineType(companyId);
                            machineMasterModel.GodownList = ObjtaxMasterHelpers.GetGodownDropdown(companyId);
                            machineMasterModel.LocationList = ObjtaxMasterHelpers.GetLocationDropdown(companyId);
                            machineMasterModel.EmployeeListS1Opr1 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                            machineMasterModel.EmployeeListS1Opr2 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                            machineMasterModel.EmployeeSuperList1 = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);
                            machineMasterModel.EmployeeListS2Opr1 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                            machineMasterModel.EmployeeListS2Opr2 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                            machineMasterModel.EmployeeSuperList2 = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);
                            return View(machineMasterModel);
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
                            return RedirectToAction("index", "MachineMaster", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        machineMasterModel.MachineTypeList = ObjtaxMasterHelpers.GetMachineType(companyId);
                        machineMasterModel.GodownList = ObjtaxMasterHelpers.GetGodownDropdown(companyId);
                        machineMasterModel.LocationList = ObjtaxMasterHelpers.GetLocationDropdown(companyId);
                        machineMasterModel.EmployeeListS1Opr1 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                        machineMasterModel.EmployeeListS1Opr2 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                        machineMasterModel.EmployeeSuperList1 = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);
                        machineMasterModel.EmployeeListS2Opr1 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                        machineMasterModel.EmployeeListS2Opr2 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                        machineMasterModel.EmployeeSuperList2 = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);
                        ViewBag.FocusType = "-1";
                        return View(machineMasterModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    machineMasterModel.MachineTypeList = ObjtaxMasterHelpers.GetMachineType(companyId);
                    machineMasterModel.GodownList = ObjtaxMasterHelpers.GetGodownDropdown(companyId);
                    machineMasterModel.LocationList = ObjtaxMasterHelpers.GetLocationDropdown(companyId);
                    machineMasterModel.EmployeeListS1Opr1 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                    machineMasterModel.EmployeeListS1Opr2 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                    machineMasterModel.EmployeeSuperList1 = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);
                    machineMasterModel.EmployeeListS2Opr1 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                    machineMasterModel.EmployeeListS2Opr2 = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);
                    machineMasterModel.EmployeeSuperList2 = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);
                    ViewBag.FocusType = "-1";
                    return View(machineMasterModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new MachineMasterModel());
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
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@MacVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "1");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtMachi = ObjDBConnection.CallStoreProcedure("GetMachineDetails", sqlParameters);
                    if (DtMachi != null && DtMachi.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtMachi.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Machine Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "MachineMaster");
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
                    string currentURL = "/MachineMaster/Index";
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
                    getReportDataModel.ControllerName = "MachineMaster";
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
                    var bytes = Excel(getReportDataModel, "Machine Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Machine.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Machine Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Machine.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Route("/MachineMaster/GetGodown-List")]
        public IActionResult GodownList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var godownList = ObjtaxMasterHelpers.GetGodownDropdown(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                godownList = godownList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(godownList) });
        }

        [Route("/MachineMaster/GetLocation-List")]
        public IActionResult LocationList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var locationList = ObjtaxMasterHelpers.GetLocationDropdown(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                locationList = locationList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(locationList) });
        }

        [Route("/MachineMaster/GetMachineType-List")]
        public IActionResult MachineTypeList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var mtypeList = ObjtaxMasterHelpers.GetMachineType(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                mtypeList = mtypeList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(mtypeList) });
        }

        //[Route("/MachineMaster/GetOperator-List")]
        //public IActionResult OperatorList(string q)
        //{
        //    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
        //    var operList = ObjAccountMasterHelpers.GetOperatorCustomDropdown(companyId, 0);

        //    if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
        //    {
        //        operList = operList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
        //    }
        //    return Json(new { items = CommonHelpers.BindSelect2Model(operList) });
        //}

        //[Route("/MachineMaster/GetSuperVisor-List")]
        //public IActionResult SupervisorList(string q)
        //{
        //    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
        //    var superList = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(companyId, 0);

        //    if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
        //    {
        //        superList = superList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
        //    }
        //    return Json(new { items = CommonHelpers.BindSelect2Model(superList) });
        //}
    }
}
