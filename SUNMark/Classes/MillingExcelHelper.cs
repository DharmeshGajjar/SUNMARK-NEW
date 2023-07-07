using SUNMark.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Classes
{
    public class MillingExcelHelper
    {
        static ProductHelpers objProductHelper = new ProductHelpers();
        static AccountMasterHelpers ObjAccountMasterHelpers = new AccountMasterHelpers();
        static DbConnection ObjDBConnection = new DbConnection();

        public static void ProcessExcel(DataTable dt, ref List<object> gridList, ref List<object> headerList)
        {
            gridList = new List<object>();
            headerList = new List<object>();
            try
            {
                if (dt != null && dt.Columns.Count > 0)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(dt.Columns[i].ColumnName.ToString()))
                        {
                            headerList.Add(dt.Columns[i].ColumnName.ToString());
                        }
                    }
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        List<object> grid = new List<object>();
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (!string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString()))
                            {
                                grid.Add(dt.Rows[i][j].ToString());
                            }
                            else
                            {
                                grid.Add(string.Empty);
                            }
                        }
                        gridList.Add(grid);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int InsertExcelData(DataTable dt, string scpproductId, string companyId, int sessionCompanyId, ref List<CustomDropDown> notFoundItems, string productId)
        {
            int inserted = 0;
            try
            {
                notFoundItems = new List<CustomDropDown>();
                var gradeList = objProductHelper.GetGradeMasterDropdown(sessionCompanyId, 0);
                var oper1List = ObjAccountMasterHelpers.GetEmployeeCustomDropdown(sessionCompanyId, 0);
                var supervisorList = ObjAccountMasterHelpers.GetSupervisorCustomDropdown(sessionCompanyId, 0);
                var processList = objProductHelper.GetLotPrcTypMasterDropdown(sessionCompanyId, 0);
                var milprocessList = ObjAccountMasterHelpers.GetMachineMasterDropdown(sessionCompanyId, "");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        #region Validation

                        #region Grade
                        string gradeId = string.Empty;
                        string grade = string.Empty;
                        if (gradeList != null && gradeList.Count > 0)
                        {
                            gradeId = gradeList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["GRADE"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                            grade = gradeList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["GRADE"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                        }
                        #endregion

                        #region Operator1
                        string oper1Id = string.Empty;
                        string oper1 = string.Empty;
                        if (oper1List != null && oper1List.Count > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(dt.Rows[i]["OPRETOR NAME1"].ToString()))
                            {
                                oper1Id = oper1List.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["OPRETOR NAME1"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                oper1 = oper1List.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["OPRETOR NAME1"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                            }
                        }
                        #endregion

                        #region Operator2
                        string oper2Id = string.Empty;
                        string oper2 = string.Empty;
                        if (oper1List != null && oper1List.Count > 0)
                        {
                            if(!string.IsNullOrWhiteSpace(dt.Rows[i]["OPRETOR NAME2"].ToString()))
                            {
                                oper2Id = oper1List.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["OPRETOR NAME2"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                oper2 = oper1List.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["OPRETOR NAME2"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                            }
                        }
                        #endregion

                        #region Supervisor
                        string supervisorId = string.Empty;
                        string supervisor = string.Empty;
                        if (supervisorList != null && supervisorList.Count > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(dt.Rows[i]["SUPERVISOR NAME"].ToString()))
                            {
                                supervisorId = supervisorList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["SUPERVISOR NAME"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                supervisor = supervisorList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["SUPERVISOR NAME"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                            }
                        }
                        #endregion

                        #region MillNo
                        string millnoId = string.Empty;
                        string millno = string.Empty;
                        if (milprocessList != null && milprocessList.Count > 0)
                        {
                            millnoId = milprocessList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["MILL NO"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                            millno = milprocessList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["MILL NO"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                        }
                        #endregion

                        #region Next Process
                        string processId = string.Empty;
                        string process = string.Empty;
                        if (processList != null && processList.Count > 0)
                        {
                            processId = processList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["NEXT PROCESS"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                            process = processList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["NEXT PROCESS"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                        }
                        #endregion

                        bool isgradeerror = false, isoper1error = false, isoper2error = false, issupervisorerror = false, ismillnoerror = false, isprocesserror = false;

                        if (string.IsNullOrWhiteSpace(gradeId))
                        {
                            if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value2.Equals(dt.Rows[i]["GRADE"].ToString().Trim())).Count() <= 0)
                            {
                                gradeId = dt.Rows[i]["GRADE"].ToString().Trim();
                                isgradeerror = true;
                            }
                            else
                            {
                                gradeId = "Grade not found";
                            }
                        }
                        else
                        {
                            gradeId = "-";
                        }

                        if (string.IsNullOrWhiteSpace(millnoId) )
                        {
                            if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value2.Equals(dt.Rows[i]["MILL NO"].ToString().Trim())).Count() <= 0)
                            {
                                millnoId = dt.Rows[i]["MILL NO"].ToString().Trim();
                                ismillnoerror = true;
                            }
                            else
                            {
                                millnoId = "Mill No not found";
                            }
                        }
                        else
                        {
                            millnoId = "-";
                        }

                        if (string.IsNullOrWhiteSpace(oper1Id) && oper1Id != "")
                        {
                            if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value2.Equals(dt.Rows[i]["OPRETOR NAME1"].ToString().Trim())).Count() <= 0)
                            {
                                oper1Id = dt.Rows[i]["OPRETOR NAME1"].ToString().Trim();
                                isoper1error = true;
                            }
                            else
                            {
                                oper1Id = "Operator Name not found";
                            }
                        }
                        else
                        {
                            oper1Id = "-";
                        }

                        if (string.IsNullOrWhiteSpace(oper2Id) && oper2Id != "")
                        {
                            if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value2.Equals(dt.Rows[i]["OPRETOR NAME2"].ToString().Trim())).Count() <= 0)
                            {
                                oper2Id = dt.Rows[i]["OPRETOR NAME2"].ToString().Trim();
                                isoper2error = true;
                            }
                            else
                            {
                                oper2Id = "Operator Name not found";
                            }
                        }
                        else
                        {
                            oper2Id = "-";
                        }

                        if (string.IsNullOrWhiteSpace(supervisorId) && supervisorId != "")
                        {
                            if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value2.Equals(dt.Rows[i]["SUPERVISOR NAME"].ToString().Trim())).Count() <= 0)
                            {
                                supervisorId = dt.Rows[i]["SUPERVISOR NAME"].ToString().Trim();
                                issupervisorerror = true;
                            }
                            else
                            {
                                supervisorId = "Supervisor Name not found";
                            }
                        }
                        else
                        {
                            supervisorId = "-";
                        }

                        if (string.IsNullOrWhiteSpace(processId))
                        {
                            if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value2.Equals(dt.Rows[i]["NEXT PROCESS"].ToString().Trim())).Count() <= 0)
                            {
                                processId = dt.Rows[i]["NEXT PROCESS"].ToString().Trim();
                                issupervisorerror = true;
                            }
                            else
                            {
                                processId = "NEXT PROCESS not found";
                            }
                        }
                        else
                        {
                            processId = "-";
                        }


                        if (isgradeerror || isoper1error || isoper2error || issupervisorerror || ismillnoerror || isprocesserror)
                            notFoundItems.Add(new CustomDropDown
                            {
                                //Text = finishId,
                                Value1 = gradeId,
                                Value2 = oper1Id,
                                Value3 = oper2Id,
                                Value4 = supervisorId,
                                Value5 = millnoId,
                                Value6 = processId
                            });


                        #endregion
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }

                if (notFoundItems == null || notFoundItems.Count == 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            #region Insert code

                            #region MillNo
                            string millnoId = string.Empty;
                            if (milprocessList != null && milprocessList.Count > 0)
                            {
                                millnoId = milprocessList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["MILL NO"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                            }
                            string coilno = dt.Rows[i]["COIL NO"].ToString();

                            #endregion

                            SqlParameter[] sqlparameter = new SqlParameter[2];
                            sqlparameter[0] = new SqlParameter("@COILNO", dt.Rows[i]["COIL NO"].ToString());
                            sqlparameter[1] = new SqlParameter("@MACVOU", millnoId);
                            DataTable ds = ObjDBConnection.CallStoreProcedure("GetDataByCoilNoMillingNew", sqlparameter);
                            if (ds != null && ds.Rows.Count > 0)
                            {
                                int status = Convert.ToInt32(ds.Rows[0][0].ToString());
                                if (status != -1)
                                {
                                    string voucherNo = GetVoucherNo(sessionCompanyId);
                                    SqlParameter[] parameter = new SqlParameter[48];
                                    parameter[0] = new SqlParameter("@MilVou", "0");
                                    parameter[1] = new SqlParameter("@MilCmpVou", companyId);
                                    parameter[2] = new SqlParameter("@MilVno", voucherNo);
                                    parameter[3] = new SqlParameter("@MilDt", DateTime.Parse(dt.Rows[i]["DATE"].ToString()));
                                    string shiftid = string.Empty;
                                    if (dt.Rows[i]["SHIFT"].ToString() == "FIRST" || dt.Rows[i]["SHIFT"].ToString() == "First")
                                    {
                                        shiftid = "1";
                                    }
                                    else if (dt.Rows[i]["SHIFT"].ToString() == "SECOND" || dt.Rows[i]["SHIFT"].ToString() == "Second")
                                    {
                                        shiftid = "2";
                                    }
                                    else if (dt.Rows[i]["SHIFT"].ToString() == "BOTH" || dt.Rows[i]["SHIFT"].ToString() == "Both")
                                    {
                                        shiftid = "3";
                                    }

                                    parameter[4] = new SqlParameter("@MilShift", shiftid);
                                    parameter[5] = new SqlParameter("@MilMacNo", millnoId);

                                    #region Operator1
                                    string oper1Id = string.Empty;
                                    if (oper1List != null && oper1List.Count > 0)
                                    {
                                        oper1Id = oper1List.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["OPRETOR NAME1"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                    }

                                    if (!string.IsNullOrWhiteSpace(oper1Id))
                                    {
                                        parameter[6] = new SqlParameter("@MilOpr1EmpVou", oper1Id);
                                    }
                                    else
                                    {
                                        parameter[6] = new SqlParameter("@MilOpr1EmpVou", 0);
                                    }
                                    #endregion

                                    #region Operator2
                                    string oper2Id = string.Empty;
                                    if (oper1List != null && oper1List.Count > 0)
                                    {
                                        oper2Id = oper1List.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["OPRETOR NAME2"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                    }

                                    if (!string.IsNullOrWhiteSpace(oper2Id))
                                    {
                                        parameter[7] = new SqlParameter("@MilOpr2EmpVou", oper2Id);
                                    }
                                    else
                                    {
                                        parameter[7] = new SqlParameter("@MilOpr2EmpVou", 0);
                                    }
                                    #endregion

                                    parameter[8] = new SqlParameter("@MilLotVou", ds.Rows[0][0].ToString());
                                    parameter[9] = new SqlParameter("@MilLotNo", dt.Rows[i]["COIL NO"].ToString());
                                    parameter[10] = new SqlParameter("@MilOD", Convert.ToDecimal(dt.Rows[i]["OD"].ToString()));
                                    parameter[11] = new SqlParameter("@MilLenFeet", Convert.ToDecimal(dt.Rows[i]["LENGTH"].ToString()));
                                    parameter[12] = new SqlParameter("@MilInTime", "");
                                    parameter[13] = new SqlParameter("@MilOutTime", "");
                                    parameter[14] = new SqlParameter("@MilPcs", Convert.ToDecimal(dt.Rows[i]["NOS"].ToString()));
                                    parameter[15] = new SqlParameter("@MilQty", Convert.ToDecimal(dt.Rows[i]["COIL WEIGHT"].ToString()));
                                    parameter[16] = new SqlParameter("@MilRecPrdVou", productId);
                                    parameter[17] = new SqlParameter("@MilScrLenFeet", 0);
                                    parameter[18] = new SqlParameter("@MilScrQty", Convert.ToDecimal(dt.Rows[i]["SCRAP"].ToString()));
                                    parameter[19] = new SqlParameter("@MilScrPrdVou", scpproductId);
                                    parameter[20] = new SqlParameter("@MilTouNo", 0);
                                    parameter[21] = new SqlParameter("@MilWeldSpeed", 0);
                                    parameter[22] = new SqlParameter("@MilWeldAMP", "");
                                    parameter[23] = new SqlParameter("@MilWeldVolt", "");
                                    parameter[24] = new SqlParameter("@MilRem", "");
                                    parameter[25] = new SqlParameter("@MilRead1Thick", "");
                                    parameter[26] = new SqlParameter("@MilRead1OD", "");
                                    parameter[27] = new SqlParameter("@MilRead2Thick", "");
                                    parameter[28] = new SqlParameter("@MilRead2OD", "");

                                    #region Supervisor
                                    string supervisorId = string.Empty;
                                    if (supervisorList != null && supervisorList.Count > 0)
                                    {
                                        supervisorId = supervisorList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["SUPERVISOR NAME"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                    }

                                    if (!string.IsNullOrWhiteSpace(supervisorId))
                                    {
                                        parameter[29] = new SqlParameter("@MilSupEmpVou", supervisorId);
                                    }
                                    else
                                    {
                                        parameter[29] = new SqlParameter("@MilSupEmpVou", 0);
                                    }
                                    #endregion

                                    #region Process
                                    string processId = string.Empty;
                                    if (processList != null && processList.Count > 0)
                                    {
                                        processId = processList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["NEXT PROCESS"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                    }

                                    if (!string.IsNullOrWhiteSpace(processId))
                                    {
                                        parameter[30] = new SqlParameter("@MilNextPrcVou", processId);
                                    }
                                    #endregion

                                    parameter[31] = new SqlParameter("@MilNB", "");
                                    parameter[32] = new SqlParameter("@MilSCH", "");
                                    parameter[33] = new SqlParameter("@MilRecQty", Convert.ToDecimal(dt.Rows[i]["KG"].ToString()));
                                    parameter[34] = new SqlParameter("@MilNBVou", "");
                                    parameter[35] = new SqlParameter("@MilSCHVou", "");
                                    parameter[36] = new SqlParameter("@MilRLPcs", Convert.ToDecimal(dt.Rows[i]["RL PCS"].ToString()));
                                    parameter[37] = new SqlParameter("@MilRLWeight", Convert.ToDecimal(dt.Rows[i]["RL KGS"].ToString()));
                                    parameter[38] = new SqlParameter("@MilPrcVou", 40);
                                    parameter[39] = new SqlParameter("@MIlFinishDt", "");
                                    parameter[40] = new SqlParameter("@MilReason", "");
                                    parameter[41] = new SqlParameter("@MilRemainingWeight", 0);
                                    parameter[42] = new SqlParameter("@MilStopFrTm1", "");
                                    parameter[43] = new SqlParameter("@MilStopToTm1", "");
                                    parameter[44] = new SqlParameter("@MilStopReson1", "");
                                    parameter[45] = new SqlParameter("@MilStopFrTm2", "");
                                    parameter[46] = new SqlParameter("@MilStopToTm2", "");
                                    parameter[47] = new SqlParameter("@MilStopReson2", "");
                                    if (!string.IsNullOrWhiteSpace(voucherNo) && !string.IsNullOrWhiteSpace(companyId) && !string.IsNullOrWhiteSpace(processId) && !string.IsNullOrWhiteSpace(productId) && !string.IsNullOrWhiteSpace(shiftid) && !string.IsNullOrWhiteSpace(millnoId) && !string.IsNullOrWhiteSpace(coilno))
                                    {
                                        DataTable dtmill = ObjDBConnection.CallStoreProcedure("AddMilling", parameter);
                                        if (dtmill != null && dtmill.Rows.Count > 0)
                                        {
                                            int status1 = DbConnection.ParseInt32(dtmill.Rows[0][0].ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {

                            }

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }

                    inserted = 1;


                }
                else
                {
                    inserted = 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return inserted;
        }


        private static string GetVoucherNo(int companyId)
        {
            string returnValue = string.Empty;
            try
            {
                DataTable dtVoucherNo = ObjDBConnection.CallStoreProcedure("GetMillingVoucherNo", null);
                if (dtVoucherNo != null && dtVoucherNo.Rows.Count > 0)
                {
                    returnValue = dtVoucherNo.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return returnValue;
        }

    }
}
