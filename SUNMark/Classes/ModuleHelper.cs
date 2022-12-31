using SUNMark.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SUNMark.Models;

namespace SUNMark.Classes
{
    public class ModuleHelper
    {
        DbConnection ObjDBConnection = new DbConnection();

        public ModuleMasterModal BindModuleMasterModel(long id)
        {
            ModuleMasterModal moduleMasterModal = new ModuleMasterModal();
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@ModuleId", id);
            DataSet dataset = ObjDBConnection.GetDataSet("GetModuleMasterById", sqlParameters);
            List<ModuleMasterViewModal> ModuleMasterViewModalList = new List<ModuleMasterViewModal>();

            DataTable dtModuleMaster = dataset.Tables[0];
            DataTable dtModuleMasterChild = dataset.Tables[1];

            if (dtModuleMaster != null && dtModuleMaster.Rows.Count > 0)
            {
                moduleMasterModal.ModuleId = dtModuleMaster.Rows[0]["ModuleId"] != null ? DbConnection.ParseInt32(dtModuleMaster.Rows[0]["ModuleId"].ToString()) : 0;

                moduleMasterModal.Name = dtModuleMaster.Rows[0]["Name"] != null ? dtModuleMaster.Rows[0]["Name"].ToString() : string.Empty;

                moduleMasterModal.Link = dtModuleMaster.Rows[0]["Link"] != null ? dtModuleMaster.Rows[0]["Link"].ToString() : string.Empty;

                moduleMasterModal.Icon = dtModuleMaster.Rows[0]["Icon"] != null ? dtModuleMaster.Rows[0]["Icon"].ToString() : string.Empty;

                moduleMasterModal.IsMaster = Convert.ToBoolean(dtModuleMaster.Rows[0]["IsMaster"]);

                if (!string.IsNullOrEmpty(dtModuleMaster.Rows[0]["ParentFK"].ToString()))
                {
                    moduleMasterModal.ParentFK = Convert.ToInt64(dtModuleMaster.Rows[0]["ParentFK"]);
                }
                else
                {
                    moduleMasterModal.ParentFK = null;
                }
            }

            if (dtModuleMasterChild != null && dtModuleMasterChild.Rows.Count > 0)
            {
                foreach (DataRow drModule in dtModuleMasterChild.Rows)
                {
                    ModuleMasterViewModal dataRoemoduleMasterChild = new ModuleMasterViewModal();

                    dataRoemoduleMasterChild.Name = drModule["Name"] != null ? drModule["Name"].ToString() : string.Empty;

                    dataRoemoduleMasterChild.Link = drModule["Link"] != null ? drModule["Link"].ToString() : string.Empty;

                    dataRoemoduleMasterChild.Icon = drModule["Icon"] != null ? drModule["Icon"].ToString() : string.Empty;

                    dataRoemoduleMasterChild.ModuleId = drModule["ModuleId"] != null ? DbConnection.ParseInt32(drModule["ModuleId"].ToString()) : 0;

                    dataRoemoduleMasterChild.IsMaster = Convert.ToBoolean(drModule["IsMaster"]);

                    dataRoemoduleMasterChild.Position = DbConnection.ParseInt32(drModule["Position"]);

                    dataRoemoduleMasterChild.DashboardPosition = DbConnection.ParseInt32(drModule["DashboardPosition"]);

                    if (!string.IsNullOrEmpty(drModule["ParentFK"].ToString()))
                    {
                        dataRoemoduleMasterChild.ParentFK = Convert.ToInt64(drModule["ParentFK"]);
                    }
                    else
                    {
                        dataRoemoduleMasterChild.ParentFK = null;
                    }

                    ModuleMasterViewModalList.Add(dataRoemoduleMasterChild);
                }

                moduleMasterModal.ModuleMasterViewModalList = ModuleMasterViewModalList;
            }
            return moduleMasterModal;
        }

        public int InsertModuleDetails(ModuleMasterModal moduleModal)
        {
            int isInserted = 0;
            try
            {
                ModuleMasterModal moduleMasterModal = new ModuleMasterModal();

                if (moduleModal != null)
                {
                    if (moduleModal.ModuleId > 0)
                    {
                        moduleMasterModal = BindModuleMasterModel(moduleModal.ModuleId);
                    }
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@ModuleId", moduleModal.ModuleId.ToString());
                    sqlParameters[1] = new SqlParameter("@Name", moduleModal.Name);
                    sqlParameters[2] = new SqlParameter("@Link", moduleModal.Link);
                    sqlParameters[3] = new SqlParameter("@Icon", moduleModal.Icon);
                    sqlParameters[4] = new SqlParameter("@ParentFK", null);
                    sqlParameters[5] = new SqlParameter("@IsMaster", moduleModal.IsMaster ? "1" : "0");

                    DataSet dtUser = ObjDBConnection.GetDataSet("InsertModuleMaster", sqlParameters);
                    int lastInsertedId = 0;

                    if (dtUser.Tables != null && dtUser.Tables.Count > 0 && dtUser.Tables[1] != null && dtUser.Tables[1].Rows.Count > 0)
                    {
                        lastInsertedId = DbConnection.ParseInt32(dtUser.Tables[1].Rows[0][0].ToString());
                    }

                    if (lastInsertedId > 0)
                    {
                        if (moduleMasterModal != null)
                        {
                            if (moduleMasterModal.IsMaster)
                            {
                                if (!moduleModal.IsMaster)
                                {
                                    var isDeleted = this.DeleteChildModuleMaster(moduleMasterModal.ModuleId);
                                }
                            }
                        }

                        if (moduleModal.IsMaster)
                        {
                            if (moduleModal.ModuleMasterViewModalList != null && moduleModal.ModuleMasterViewModalList.Count > 0)
                            {
                                List<ModuleMasterViewModal> ModuleMasterViewModalList = moduleModal.ModuleMasterViewModalList;

                                List<long> newChildModuleId = ModuleMasterViewModalList.Where(m => m.ModuleId != 0).Select(x => x.ModuleId).ToList();

                                if (newChildModuleId != null && newChildModuleId.Count > 0)
                                {
                                    sqlParameters = new SqlParameter[1];
                                    sqlParameters[0] = new SqlParameter("@ModuleId", moduleModal.ModuleId.ToString());

                                    DataTable oldChildModuleList = ObjDBConnection.CallStoreProcedure("GetChildModuleListByModuleId", sqlParameters);
                                    List<long> oldChildModuleId = new List<long>();
                                    if (oldChildModuleList != null && oldChildModuleList.Rows.Count > 0)
                                    {
                                        foreach (DataRow dataRow in oldChildModuleList.Rows)
                                        {
                                            oldChildModuleId.Add(Convert.ToInt64(dataRow["ModuleId"]));
                                        }
                                    }

                                    if (oldChildModuleId != null && oldChildModuleId.Count > 0)
                                    {
                                        List<long> deleteChildModule = oldChildModuleId.Except(newChildModuleId).ToList();

                                        if (deleteChildModule != null && deleteChildModule.Count > 0)
                                        {
                                            foreach (long deleteModuleId in deleteChildModule)
                                            {
                                                var isDeleted = this.DeleteModuleMaster(deleteModuleId);
                                            }
                                        }
                                    }
                                }

                                foreach (ModuleMasterViewModal model in moduleModal.ModuleMasterViewModalList)
                                {
                                    sqlParameters = new SqlParameter[8];
                                    sqlParameters[0] = new SqlParameter("@ModuleId", model.ModuleId.ToString());
                                    sqlParameters[1] = new SqlParameter("@Name", model.Name.ToString());
                                    sqlParameters[2] = new SqlParameter("@Link", model.Link.ToString());
                                    sqlParameters[3] = new SqlParameter("@Icon", model.Icon);
                                    sqlParameters[4] = new SqlParameter("@ParentFK", lastInsertedId.ToString());
                                    sqlParameters[5] = new SqlParameter("@IsMaster", model.IsMaster ? "1" : "0");
                                    sqlParameters[6] = new SqlParameter("@Position", model.Position.ToString());
                                    sqlParameters[7] = new SqlParameter("@DashboardPosition", model.DashboardPosition.ToString());

                                    DataSet childPDTUser = ObjDBConnection.GetDataSet("InsertModuleMaster", sqlParameters);
                                    long chileLastInsertedId = 0; ;
                                    if (childPDTUser != null && childPDTUser.Tables.Count > 0 && childPDTUser.Tables[1] != null && childPDTUser.Tables[1].Rows.Count > 0)
                                    {
                                        chileLastInsertedId = DbConnection.ParseInt32(childPDTUser.Tables[1].Rows[0][0].ToString());
                                        isInserted = DbConnection.ParseInt32(childPDTUser.Tables[0].Rows[0][0].ToString());
                                    }
                                }
                            }
                        }
                        else
                        {
                            return lastInsertedId;
                        }
                    }

                    return isInserted;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return isInserted;
        }

        public int DeleteChildModuleMaster(long Id)
        {
            int returnId = 0;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@ModuleId", Id);
                DataTable dt = ObjDBConnection.CallStoreProcedure("DeleteChildModuleMasterByModuleId", sqlParameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    returnId = DbConnection.ParseInt32(dt.Rows[0][0].ToString());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return returnId;
        }

        public int DeleteModuleMaster(long Id)
        {
            int returnId = 0;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@ModuleId", Id);
                DataTable dt = ObjDBConnection.CallStoreProcedure("DeleteModuleMaster", sqlParameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    returnId = DbConnection.ParseInt32(dt.Rows[0][0].ToString());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return returnId;
        }

        public List<ModuleMasterModal> BindModuleMasterListing(int skipRecord, int pageSize, string searchValue, ref long maxRows)
        {
            List<ModuleMasterModal> moduleMasterModal = new List<ModuleMasterModal>();
            SqlParameter[] sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("@SkipRecord", skipRecord);
            sqlParameters[1] = new SqlParameter("@PageSize", pageSize);
            sqlParameters[2] = new SqlParameter("@SearchValue", searchValue);
            DataSet dsModule = ObjDBConnection.GetDataSet("GetModuleMasterListPageWise", sqlParameters);
            if (dsModule != null && dsModule.Tables.Count > 0)
            {
                DataTable dtModule = dsModule.Tables[0];
                if (dtModule != null && dtModule.Rows.Count > 0)
                {
                    maxRows = Convert.ToInt64(dtModule.Rows[0]["MaxRow"]);

                    foreach (DataRow item in dtModule.Rows)
                    {
                        moduleMasterModal.Add(new ModuleMasterModal
                        {
                            Name = item["Name"] != null ? item["Name"].ToString() : string.Empty,

                            Icon = item["Icon"] != null ? item["Icon"].ToString() : string.Empty,

                            Link = item["Link"] != null ? item["Link"].ToString() : string.Empty,

                            ModuleId = item["ModuleId"] != null ? DbConnection.ParseInt32(item["ModuleId"].ToString()) : 0

                        });
                    }
                }
            }

            return moduleMasterModal;
        }

        public List<UserModuleModal> GetUserModuleList(long userId)
        {
            List<UserModuleModal> userModuleModalsList = new List<UserModuleModal>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@UserId", userId);
                DataSet dsModule = ObjDBConnection.GetDataSet("GetAllModuleListForDesignationAssign", sqlParameters);

                if (dsModule != null && dsModule.Tables.Count > 0)
                {
                    DataTable dtMaster = dsModule.Tables[0];
                    DataTable dtChild = dsModule.Tables[1];
                    DataTable dtAssigned = new DataTable();
                    if (dsModule.Tables.Count > 2)
                        dtAssigned = dsModule.Tables[2];


                    if (dtMaster != null && dtMaster.Rows.Count > 0)
                    {
                        foreach (DataRow master in dtMaster.Rows)
                        {
                            UserModuleModal userModuleModal = new UserModuleModal();
                            userModuleModal.ModuleId = Convert.ToInt64(master["ModuleId"].ToString());
                            userModuleModal.ModuleName = master["Name"].ToString();

                            List<UserSubMenuModal> subMenuModalsList = new List<UserSubMenuModal>();

                            DataRow[] drChild = dtChild.Select("ParentFK = '" + master["ModuleId"].ToString() + "'");
                            if (drChild != null && drChild.Length > 0)
                            {
                                foreach (var child in drChild)
                                {
                                    UserSubMenuModal userSubMenuModal = new UserSubMenuModal();
                                    userSubMenuModal.ModuleId = Convert.ToInt64(child["ModuleId"].ToString());
                                    userSubMenuModal.ModuleName = child["Name"].ToString();
                                    if (dtAssigned != null && dtAssigned.Rows.Count > 0)
                                    {
                                        DataRow[] drFind = dtAssigned.Select("ModuleId = '" + userSubMenuModal.ModuleId + "'");
                                        if (drFind != null && drFind.Length > 0)
                                        {
                                            if (!string.IsNullOrWhiteSpace(drFind[0]["IsAdd"].ToString()) && drFind[0]["IsAdd"].ToString().Equals("True"))
                                            {
                                                userSubMenuModal.IsAdd = true;
                                            }
                                            if (!string.IsNullOrWhiteSpace(drFind[0]["IsEdit"].ToString()) && drFind[0]["IsEdit"].ToString().Equals("True"))
                                            {
                                                userSubMenuModal.IsEdit = true;
                                            }
                                            if (!string.IsNullOrWhiteSpace(drFind[0]["IsDelete"].ToString()) && drFind[0]["IsDelete"].ToString().Equals("True"))
                                            {
                                                userSubMenuModal.IsDelete = true;
                                            }
                                            if (!string.IsNullOrWhiteSpace(drFind[0]["IsList"].ToString()) && drFind[0]["IsList"].ToString().Equals("True"))
                                            {
                                                userSubMenuModal.IsList = true;
                                            }
                                        }
                                    }
                                    subMenuModalsList.Add(userSubMenuModal);
                                }
                            }
                            else
                            {
                                if (dtAssigned != null && dtAssigned.Rows.Count > 0)
                                {
                                    DataRow[] drModule = dtAssigned.Select("ModuleId = '" + master["ModuleId"].ToString() + "'");
                                    if (drModule != null && drModule.Length > 0)
                                    {
                                        foreach (var module in drModule)
                                        {
                                            UserSubMenuModal userSubMenuModal = new UserSubMenuModal();
                                            userSubMenuModal.ModuleId = Convert.ToInt64(module["ModuleId"].ToString());
                                            userSubMenuModal.ModuleName = module["Name"].ToString();

                                            if (!string.IsNullOrWhiteSpace(module["IsAdd"].ToString()) && module["IsAdd"].ToString().Equals("True"))
                                            {
                                                userSubMenuModal.IsAdd = true;
                                            }
                                            if (!string.IsNullOrWhiteSpace(module["IsEdit"].ToString()) && module["IsEdit"].ToString().Equals("True"))
                                            {
                                                userSubMenuModal.IsEdit = true;
                                            }
                                            if (!string.IsNullOrWhiteSpace(module["IsDelete"].ToString()) && module["IsDelete"].ToString().Equals("True"))
                                            {
                                                userSubMenuModal.IsDelete = true;
                                            }
                                            if (!string.IsNullOrWhiteSpace(module["IsList"].ToString()) && module["IsList"].ToString().Equals("True"))
                                            {
                                                userSubMenuModal.IsList = true;
                                            }
                                            subMenuModalsList.Add(userSubMenuModal);
                                        }
                                    }
                                }

                            }
                            userModuleModal.SubMenuModalList = subMenuModalsList;
                            userModuleModalsList.Add(userModuleModal);
                        }
                    }
                }
                return userModuleModalsList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int InsertUserModuleMapping(AssignUserModuleModal assignDesignationModuleModal)
        {
            try
            {
                DataTable dtModuleAssign = new DataTable();
                if (assignDesignationModuleModal != null && assignDesignationModuleModal.AssignedModule != null && assignDesignationModuleModal.AssignedModule.Count > 0)
                {
                    for (int x = 0; x < assignDesignationModuleModal.AssignedModule.Count; x++)
                    {
                        SqlParameter[] parameter = new SqlParameter[7];
                        parameter[0] = new SqlParameter("@UserFK", assignDesignationModuleModal.UserRollId.ToString());
                        parameter[1] = new SqlParameter("@ModuleFK", assignDesignationModuleModal.AssignedModule[x].ModuleId.ToString());
                        parameter[2] = new SqlParameter("@IsAdd", assignDesignationModuleModal.AssignedModule[x].IsAdd.ToString());
                        parameter[3] = new SqlParameter("@IsEdit", assignDesignationModuleModal.AssignedModule[x].IsEdit.ToString());
                        parameter[4] = new SqlParameter("@IsDelete", assignDesignationModuleModal.AssignedModule[x].IsDelete.ToString());
                        parameter[5] = new SqlParameter("@IsList", assignDesignationModuleModal.AssignedModule[x].IsList.ToString());
                        parameter[6] = new SqlParameter("@IsFirstRecord", x == 0 ? true.ToString() : false.ToString());

                        dtModuleAssign = ObjDBConnection.CallStoreProcedure("InsertAssignModuleToUser", parameter);
                    }
                }
                else
                {
                    SqlParameter[] parameter = new SqlParameter[7];
                    parameter[0] = new SqlParameter("@UserFK", assignDesignationModuleModal.UserId.ToString());
                    dtModuleAssign = ObjDBConnection.CallStoreProcedure("DeleteAssignModuleToDesignation", parameter);
                }
                return dtModuleAssign != null && dtModuleAssign.Rows.Count > 0 ? DbConnection.ParseInt32(dtModuleAssign.Rows[0][0].ToString()) : 0;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<DynamicSidebarModel> GetDynamicSidebars(long userId)
        {
            List<DynamicSidebarModel> dynamicSidebarsList = new List<DynamicSidebarModel>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@UserFK", userId);
                DataSet dsDynamicSidebar = ObjDBConnection.GetDataSet("GetDynamicSidebar", parameters);

                DataTable dtDetail = dsDynamicSidebar.Tables[0];
                DataTable dtModule = dsDynamicSidebar.Tables[1];

                if (dtModule == null || dtModule.Rows.Count == 0)
                    dtModule = dtDetail.DefaultView.ToTable(true, "ModuleFK", "Name", "Link", "Icon");

                if (dtModule.Columns.Contains("ModuleFK"))
                    dtModule.Columns["ModuleFK"].ColumnName = "ModuleId";


                if (dtModule != null && dtModule.Rows.Count > 0)
                {
                    for (int i = 0; i < dtModule.Rows.Count; i++)
                    {
                        DataRow[] drFind = dtDetail.Select("ParentFK = '" + dtModule.Rows[i]["ModuleId"].ToString() + "'");
                        if (drFind != null && drFind.Length > 0)
                        {
                            List<DynamicSidebarDetailModel> dynamicSidebarDetailModels = new List<DynamicSidebarDetailModel>();

                            for (int j = 0; j < drFind.Length; j++)
                            {
                                dynamicSidebarDetailModels.Add(new DynamicSidebarDetailModel
                                {
                                    Icons = drFind[j]["Icon"] != null ? drFind[j]["Icon"].ToString() : string.Empty,

                                    Link = drFind[j]["Link"] != null ? drFind[j]["Link"].ToString() : string.Empty,

                                    ModuleId = drFind[j]["ModuleId"] != null ? Convert.ToInt64(drFind[j]["ModuleId"].ToString()) : 0,

                                    ModuleName = drFind[j]["Name"] != null ? drFind[j]["Name"].ToString() : string.Empty,
                                });
                            }

                            dynamicSidebarsList.Add(new DynamicSidebarModel
                            {
                                Icons = dtModule.Rows[i]["Icon"] != null ? dtModule.Rows[i]["Icon"].ToString() : string.Empty,

                                Link = dtModule.Rows[i]["Link"] != null ? dtModule.Rows[i]["Link"].ToString() : string.Empty,

                                ModuleId = dtModule.Rows[i]["ModuleId"] != null ? Convert.ToInt64(dtModule.Rows[i]["ModuleId"].ToString()) : 0,

                                ModuleName = dtModule.Rows[i]["Name"] != null ? dtModule.Rows[i]["Name"].ToString() : string.Empty,

                                sidebarDetail = dynamicSidebarDetailModels
                            });
                        }
                        else
                        {
                            dynamicSidebarsList.Add(new DynamicSidebarModel
                            {
                                Icons = dtModule.Rows[i]["Icon"] != null ? dtModule.Rows[i]["Icon"].ToString() : string.Empty,

                                Link = dtModule.Rows[i]["Link"] != null ? dtModule.Rows[i]["Link"].ToString() : string.Empty,

                                ModuleId = dtModule.Rows[i]["ModuleId"] != null ? Convert.ToInt64(dtModule.Rows[i]["ModuleId"].ToString()) : 0,

                                ModuleName = dtModule.Rows[i]["Name"] != null ? dtModule.Rows[i]["Name"].ToString() : string.Empty,

                                sidebarDetail = new List<DynamicSidebarDetailModel>()
                            });
                        }
                    }
                }


                DataRow[] drMasters = dtDetail.Select("ParentFK is null");

                if (drMasters != null && drMasters.Length > 0)
                {
                    for (int i = 0; i < drMasters.Length; i++)
                    {
                        dynamicSidebarsList.Add(new DynamicSidebarModel
                        {
                            Icons = drMasters[i]["Icon"] != null ? drMasters[i]["Icon"].ToString() : string.Empty,

                            Link = drMasters[i]["Link"] != null ? drMasters[i]["Link"].ToString() : string.Empty,

                            ModuleId = drMasters[i]["ModuleId"] != null ? Convert.ToInt64(drMasters[i]["ModuleId"].ToString()) : 0,

                            ModuleName = drMasters[i]["Name"] != null ? drMasters[i]["Name"].ToString() : string.Empty,

                            sidebarDetail = new List<DynamicSidebarDetailModel>()
                        });
                    }
                }


               // dynamicSidebarsList = dynamicSidebarsList.OrderBy(x => x.ModuleId).ToList();
                return dynamicSidebarsList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        
    }
}
