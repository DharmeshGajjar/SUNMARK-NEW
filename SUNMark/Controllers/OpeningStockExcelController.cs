using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SUNMark.Classes;
using SUNMark.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Controllers
{
    public class OpeningStockExcelController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        OpeningStokModel objOpeningStock = new OpeningStokModel();
        AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        private readonly IWebHostEnvironment hostingEnvironment;

        public OpeningStockExcelController(IWebHostEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
        }

        public IActionResult Index()
        {

            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                    return RedirectToAction("index", "dashboard");

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

            int administrator = 0;
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            ViewBag.productTypeList = ObjAccountMasterHelpers.GetPrdTypeDropdown(companyId);
            ViewBag.companyList = objProductHelper.GetCompanyMasterDropdown(companyId, administrator);

            #endregion
        }

        [HttpPost]
        public IActionResult Excel(IFormFile file, string type)
        {
            try
            {
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                long userId = GetIntSession("UserId");
                DataTable dtExcel = new DataTable();
                DataTable dtDefault = new DataTable();
                if (file != null && !string.IsNullOrWhiteSpace(type))
                {
                    var bytes = FileHelper.ConvertIFormFileToBytes(file);
                    string fileName = DateTime.Today.Ticks.ToString() + ".xlsx";
                    if (bytes != null)
                    {
                        var uploads = Path.Combine(hostingEnvironment.WebRootPath, "OpeningStock");
                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }

                        if (FileHelper.FileUpload(uploads, string.Concat(uploads, "/", fileName), bytes))
                        {
                            string defaultFilePath = string.Empty;
                            if (type.ToLower() == "coil")
                            {
                                dtDefault = FileHelper.ReadXLSXFile(string.Concat(hostingEnvironment.WebRootPath, "/openingstockcoil.xlsx"));
                            }
                            else if (type.ToLower() == "pipe")
                            {
                                dtDefault = FileHelper.ReadXLSXFile(string.Concat(hostingEnvironment.WebRootPath, "/openingstockpipe.xlsx"));
                            }
                            else
                            {
                                dtDefault = FileHelper.ReadXLSXFile(string.Concat(hostingEnvironment.WebRootPath, "/openingstockother.xlsx"));
                            }
                            dtExcel = FileHelper.ReadXLSXFile(string.Concat(uploads, "/", fileName));
                        }
                    }
                    if (dtExcel != null && dtExcel.Columns.Count > 0 && dtExcel.Rows.Count > 0)
                    {
                        if (dtExcel.Columns.Count == dtDefault.Columns.Count)
                        {
                            bool isValid = true;
                            for (int i = 0; i < dtDefault.Columns.Count; i++)
                            {
                                if (string.IsNullOrWhiteSpace(dtExcel.Columns[i].ColumnName.Trim().ToString()) || !dtExcel.Columns[i].ColumnName.Trim().ToString().ToLower().Equals(dtDefault.Columns[i].ColumnName.Trim().ToLower()))
                                {
                                    isValid = false;
                                    break;
                                }
                            }
                            if (!isValid)
                            {
                                return Json(new { result = false, message = "Upload excel column not match with sample excel file." });
                            }
                            else
                            {
                                List<object> gridList = new List<object>();
                                List<object> headerList = new List<object>();
                                OpeningStockHelper.ProcessExcel(dtExcel, ref gridList, ref headerList);
                                if ((gridList != null && gridList.Count > 0) || (headerList != null && headerList.Count > 0))
                                {
                                    return Json(new { result = true, message = "Excel uploaded successfully", gridList = gridList, headerList = headerList });

                                }
                                else
                                {
                                    return Json(new { result = false, message = "Error in uploading excel" });
                                }
                            }
                        }
                        else
                        {
                            return Json(new { result = false, message = "Upload excel column not match with sample excel file." });
                        }

                    }
                    else
                    {
                        return Json(new { result = false, message = "Upload excel column not match with sample excel file." });
                    }

                }
                else
                {
                    return Json(new { result = false, message = "Please select file" });
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { result = false, message = "Internal error!" });
        }

        [HttpPost]
        public IActionResult InsertExcel(IFormFile file, string type, string companyId, string productId)
        {
            try
            {
                int sessionCompanyId = Convert.ToInt32(GetIntSession("CompanyId"));
                long userId = GetIntSession("UserId");
                DataTable dtExcel = new DataTable();
                DataTable dtDefault = new DataTable();
                if (file != null && !string.IsNullOrWhiteSpace(type))
                {
                    var bytes = FileHelper.ConvertIFormFileToBytes(file);
                    string fileName = DateTime.Today.Ticks.ToString() + ".xlsx";
                    if (bytes != null)
                    {
                        var uploads = Path.Combine(hostingEnvironment.WebRootPath, "OpeningStock");
                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }

                        if (FileHelper.FileUpload(uploads, string.Concat(uploads, "/", fileName), bytes))
                        {
                            string defaultFilePath = string.Empty;
                            if (type.ToLower() == "coil")
                            {
                                dtDefault = FileHelper.ReadXLSXFile(string.Concat(hostingEnvironment.WebRootPath, "/openingstockcoil.xlsx"));
                            }
                            else if (type.ToLower() == "pipe")
                            {
                                dtDefault = FileHelper.ReadXLSXFile(string.Concat(hostingEnvironment.WebRootPath, "/openingstockpipe.xlsx"));
                            }
                            else
                            {
                                dtDefault = FileHelper.ReadXLSXFile(string.Concat(hostingEnvironment.WebRootPath, "/openingstockother.xlsx"));
                            }
                            dtExcel = FileHelper.ReadXLSXFile(string.Concat(uploads, "/", fileName));
                        }
                    }
                    if (dtExcel != null && dtExcel.Columns.Count > 0 && dtExcel.Rows.Count > 0)
                    {
                        if (dtExcel.Columns.Count == dtDefault.Columns.Count)
                        {
                            bool isValid = true;
                            for (int i = 0; i < dtDefault.Columns.Count; i++)
                            {
                                if (string.IsNullOrWhiteSpace(dtExcel.Columns[i].ColumnName.Trim().ToString()) || !dtExcel.Columns[i].ColumnName.Trim().ToString().ToLower().Equals(dtDefault.Columns[i].ColumnName.Trim().ToLower()))
                                {
                                    isValid = false;
                                    break;
                                }
                            }
                            if (!isValid)
                            {
                                return Json(new { result = false, message = "Upload excel column not match with sample excel file." });
                            }
                            else
                            {
                                List<CustomDropDown> notFoundList = new List<CustomDropDown>();
                                if (OpeningStockHelper.InsertExcelData(dtExcel, type, companyId.ToString(), sessionCompanyId, ref notFoundList, productId) > 0)
                                {
                                    return Json(new { result = true, message = "Excel Uploaded!", notFoundList = notFoundList });
                                }
                                else
                                {
                                    return Json(new { result = false, message = "Error in uploading excel.", notFoundList = notFoundList });
                                }
                            }
                        }
                        else
                        {
                            return Json(new { result = false, message = "Upload excel column not match with sample excel file." });
                        }

                    }
                    else
                    {
                        return Json(new { result = false, message = "Upload excel column not match with sample excel file." });
                    }

                }
                else
                {
                    return Json(new { result = false, message = "Please select file" });
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { result = false, message = "Internal error!" });
        }

        public ActionResult GetProductByType(string type)
        {
            try
            {
                int administrator = 0;
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                return Json(new { result = true, data = objProductHelper.GetProductMasterDropdown(companyId, administrator, type) });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = "Internal Error!" });
            }
        }


    }
}
