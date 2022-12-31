using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SUNMark.Classes;
using SUNMark.Models;

namespace SUNMark.Controllers
{
    public class ModuleController : BaseController
    {
        ModuleHelper objModuleHelper = new ModuleHelper();
        UserHelpers objUserHelper = new UserHelpers();

        public IActionResult Index(long id)
        {

            ModuleMasterModal moduleMasterModal = new ModuleMasterModal();
            try
            {
                if (id > 0)
                {

                    moduleMasterModal.ModuleId = Convert.ToInt32(id);
                    moduleMasterModal = objModuleHelper.BindModuleMasterModel(id);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(moduleMasterModal);
        }

        [HttpPost]
        public ActionResult Index(ModuleMasterModal moduleMasterModal)
        {
            try
            {
                var isRecordInserted = objModuleHelper.InsertModuleDetails(moduleMasterModal);

                if (isRecordInserted > 0)
                {
                    SetSuccessMessage("Module master inserted successfully");
                    return Json(new { success = true, message = "Module master inserted successfully" });
                }
                else
                {
                    SetErrorMessage("Module master not inserted");
                    return Json(new { success = false, message = "Module master not inserted" });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult GetModuleList()
        {
            try
            {
                Request.Form.TryGetValue("draw", out StringValues draw);
                Request.Form.TryGetValue("length", out StringValues pageSize);
                Request.Form.TryGetValue("start", out StringValues startRecord);
                Request.Form.TryGetValue("search[value]", out StringValues searchText);
                long maxRows = 0;
                var moduleData = objModuleHelper.BindModuleMasterListing(DbConnection.ParseInt32(startRecord), DbConnection.ParseInt32(pageSize), searchText, ref maxRows);
                if (moduleData != null && moduleData.Count > 0)
                {
                    return Json(new
                    {
                        draw = DbConnection.ParseInt32(draw),
                        recordsTotal = DbConnection.ParseInt32(pageSize),
                        recordsFiltered = maxRows,
                        data = moduleData,
                    });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { result = false });
        }

        public IActionResult DeleteModule(long Id)
        {
            try
            {
                var isRecordDeleted = objModuleHelper.DeleteModuleMaster(Id);
                if (isRecordDeleted > 0)
                {
                    SetSuccessMessage("Module not deleted successfully"); ;
                }
                else
                {
                    SetErrorMessage("Module deleted successfully");
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return RedirectToAction("Index", new { Id = 0 });
        }

        public ActionResult AssignUserModule(long Id)
        {
            AssignUserModuleModal assignUserModuleModal = new AssignUserModuleModal();
            try
            {
                int clientId = Convert.ToInt32(GetIntSession("ClientId"));
                int isadministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                assignUserModuleModal.UserRollList = objUserHelper.GetUserRoleMasterDropdown(0);
                assignUserModuleModal.ModuleMasterList = objModuleHelper.GetUserModuleList(Id);
                assignUserModuleModal.UserRollId = Id;
                ViewBag.isAdministrator = isadministrator;
            }
            catch (System.Exception ex)
            {

                throw;
            }
            return View(assignUserModuleModal);
        }


        [HttpPost]
        public ActionResult GetAssignModuleList(AssignUserModuleModal assignDesignationModuleModal)
        {
            try
            {
                var isRecordInserted = objModuleHelper.InsertUserModuleMapping(assignDesignationModuleModal);

                if (isRecordInserted > 0)
                {
                    return Json(new { success = true, message = "Rights updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Rights not updated" });
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public IActionResult GetSidebar()
        {
            try
            {
                long userId = GetIntSession("UserId");
                var dynamicSidebar = objModuleHelper.GetDynamicSidebars(userId);
                return Json(new { result = true, data = dynamicSidebar });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
