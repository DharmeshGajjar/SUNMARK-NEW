using Microsoft.AspNetCore.Mvc.Rendering;
using SUNMark.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Classes
{
    public static class OpeningStockHelper
    {
        static ProductHelpers objProductHelper = new ProductHelpers();
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

        public static int InsertExcelData(DataTable dt, string type, string companyId, int sessionCompanyId, ref List<CustomDropDown> notFoundItems, string productId)
        {
            int inserted = 0;
            try
            {
                notFoundItems = new List<CustomDropDown>();
                if (type.ToLower() == "coil")
                {
                    var finishList = objProductHelper.GetFinishMasterDropdown(sessionCompanyId, 0);
                    var godownList = objProductHelper.GetGoDownMasterDropdown(sessionCompanyId, 0);
                    var gradeList = objProductHelper.GetGradeMasterDropdown(sessionCompanyId, 0);
                    var partyList = objProductHelper.GetSupplierMasterDropdown(sessionCompanyId, 0);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            string voucherNo = GetVoucherNo(sessionCompanyId);
                            SqlParameter[] sqlParameters = new SqlParameter[29];
                            sqlParameters[0] = new SqlParameter("@OblNVno", voucherNo);
                            sqlParameters[1] = new SqlParameter("@OblDt", DateTime.Parse(dt.Rows[i]["INWARD DATE"].ToString()));
                            sqlParameters[2] = new SqlParameter("@OblCmpVou", companyId);

                            #region Godown
                            string godownId = string.Empty;
                            if (godownList != null && godownList.Count > 0)
                            {
                                godownId = godownList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["LOCATION"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                            }

                            if (!string.IsNullOrWhiteSpace(godownId))
                            {
                                sqlParameters[3] = new SqlParameter("@OblGdnVou", godownId);
                            }
                            #endregion

                            sqlParameters[4] = new SqlParameter("@OblLocVou", string.Empty);

                            #region Party
                            string partyId = string.Empty;
                            if (partyList != null && partyList.Count > 0)
                            {
                                partyId = partyList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["PARTY"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                            }

                            if (!string.IsNullOrWhiteSpace(partyId))
                            {
                                sqlParameters[5] = new SqlParameter("@OblAccVou", partyId);
                            }

                            #endregion

                            sqlParameters[6] = new SqlParameter("@OblPrdVou", productId);
                            sqlParameters[7] = new SqlParameter("@OblRem", string.Empty);
                            sqlParameters[8] = new SqlParameter("@LotSupCoilNo", dt.Rows[i]["PARTY COIL NO"].ToString());
                            sqlParameters[9] = new SqlParameter("@LotHeatNo", dt.Rows[i]["HEAT NO"].ToString());

                            #region Grade
                            string gradeId = string.Empty;
                            string grade = string.Empty;
                            if (gradeList != null && gradeList.Count > 0)
                            {
                                gradeId = gradeList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["GRADE"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                grade = gradeList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["GRADE"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                            }

                            if (!string.IsNullOrWhiteSpace(gradeId))
                            {
                                sqlParameters[10] = new SqlParameter("@LotGrdMscVou", gradeId);
                                sqlParameters[11] = new SqlParameter("@OblGrade", grade);
                            }

                            #endregion

                            #region Finish
                            //string finishId = string.Empty;
                            //string finish = string.Empty;
                            //if (finishList != null && finishList.Count > 0)
                            //{
                            //    finishId = finishList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["FINISH"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                            //    finish = finishList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["FINISH"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                            //}

                            //if (!string.IsNullOrWhiteSpace(finishId))
                            //{
                            //    sqlParameters[12] = new SqlParameter("@LotFinMscVou", finishId);
                            //    sqlParameters[13] = new SqlParameter("@OblFinish", finish);
                            //}
                            //else
                            //{
                            sqlParameters[12] = new SqlParameter("@LotFinMscVou", 0);
                            sqlParameters[13] = new SqlParameter("@OblFinish", "");
                            //}
                            #endregion

                            sqlParameters[14] = new SqlParameter("@LotWidth", dt.Rows[i]["WIDTH"].ToString());
                            sqlParameters[15] = new SqlParameter("@LotThick", dt.Rows[i]["THICK"].ToString());
                            sqlParameters[16] = new SqlParameter("@LotQty", dt.Rows[i]["WEIGHT"].ToString());
                            sqlParameters[17] = new SqlParameter("@LotInwWidth", dt.Rows[i]["WIDTH"].ToString());
                            sqlParameters[18] = new SqlParameter("@LotInwThick", dt.Rows[i]["THICK"].ToString());
                            sqlParameters[19] = new SqlParameter("@LotInwQty", dt.Rows[i]["WEIGHT"].ToString());
                            sqlParameters[20] = new SqlParameter("@PrdTyp", "COIL");
                            sqlParameters[21] = new SqlParameter("@CmpVou", companyId);
                            sqlParameters[22] = new SqlParameter("@OblVou", 0);


                            sqlParameters[23] = new SqlParameter("@LotTyp", string.Empty);
                            string coilano = dt.Rows[i]["COIL NO"].ToString();
                            string temp = string.Empty;
                            int coilnumber = 0;

                            for (int c = 0; c < coilano.Length; c++)
                            {
                                if (Char.IsDigit(coilano[c]))
                                    temp += coilano[c];
                            }

                            if (temp.Length > 0)
                                coilnumber = int.Parse(temp);

                            sqlParameters[24] = new SqlParameter("@LotNo", coilnumber);

                            string _strLotCoilNo = string.Format("{0}", dt.Rows[i]["COIL NO"].ToString());
                            sqlParameters[25] = new SqlParameter("@CoilNo", _strLotCoilNo);
                            sqlParameters[26] = new SqlParameter("@Flg", 1);

                            sqlParameters[27] = new SqlParameter("@RefNo", dt.Rows[i]["MEMO NO"].ToString());
                            sqlParameters[28] = new SqlParameter("@LotOD", dt.Rows[i]["SIZE"].ToString());

                            if (!string.IsNullOrWhiteSpace(gradeId) && !string.IsNullOrWhiteSpace(partyId) && !string.IsNullOrWhiteSpace(godownId))
                            {
                                DataTable DtState = ObjDBConnection.CallStoreProcedure("OBLMST_Insert", sqlParameters);
                                if (DtState != null && DtState.Rows.Count > 0)
                                {
                                    int status = DbConnection.ParseInt32(DtState.Rows[0][0].ToString());
                                }
                            }
                            else
                            {
                                bool isAllow = false;
                                if (string.IsNullOrWhiteSpace(godownId))
                                {
                                    if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value1.Equals(dt.Rows[i]["LOCATION"].ToString().Trim())).Count() <= 0)
                                    {
                                        godownId = dt.Rows[i]["LOCATION"].ToString().Trim();
                                        isAllow = true;
                                    }
                                    else
                                    {
                                        godownId = "Godown not found";
                                    }
                                }
                                else
                                {
                                    godownId = "-";
                                }

                                if (string.IsNullOrWhiteSpace(gradeId))
                                {
                                    if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value2.Equals(dt.Rows[i]["GRADE"].ToString().Trim())).Count() <= 0)
                                    {
                                        gradeId = dt.Rows[i]["GRADE"].ToString().Trim();
                                        isAllow = true;
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

                                if (string.IsNullOrWhiteSpace(partyId))
                                {
                                    if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value3.Equals(dt.Rows[i]["PARTY"].ToString().Trim())).Count() <= 0)
                                    {
                                        partyId = dt.Rows[i]["PARTY"].ToString().Trim();
                                        isAllow = true;
                                    }
                                    else
                                    {
                                        partyId = "Party not found";
                                    }
                                }
                                else
                                {
                                    partyId = "-";
                                }

                                //if (string.IsNullOrWhiteSpace(finishId) && !string.IsNullOrWhiteSpace(dt.Rows[i]["FINISH"].ToString()))
                                //{
                                //    if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Text.Equals(dt.Rows[i]["FINISH"].ToString().Trim())).Count() <= 0)
                                //    {
                                //        finishId = dt.Rows[i]["FINISH"].ToString().Trim();
                                //        isAllow = true;
                                //    }
                                //    else
                                //    {
                                //        finishId = "-";
                                //    }
                                //}
                                //else
                                //{
                                //    finishId = "-";
                                //}


                                if (string.IsNullOrWhiteSpace(godownId) && notFoundItems.Where(x => x.Value1.Equals("Godown not found")).Count() <= 0)
                                {
                                    godownId = "Godown not found";
                                }

                                if (!string.IsNullOrWhiteSpace(gradeId) && notFoundItems.Where(x => x.Value2.Equals("Grade not found")).Count() <= 0)
                                {
                                    gradeId = "Grade not found";
                                }

                                if (string.IsNullOrWhiteSpace(partyId) && notFoundItems.Where(x => x.Value3.Equals("Party not found")).Count() <= 0)
                                {
                                    partyId = "Party not found";
                                }


                                if (!string.IsNullOrWhiteSpace(partyId) || string.IsNullOrWhiteSpace(gradeId) || string.IsNullOrWhiteSpace(godownId))
                                    notFoundItems.Add(new CustomDropDown
                                    {
                                        //Text = finishId,
                                        Value1 = godownId,
                                        Value2 = gradeId,
                                        Value3 = partyId
                                    });
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                    inserted = 1;
                }
                else if (type.ToLower() == "pipe")
                {
                    var finishList = objProductHelper.GetFinishMasterDropdown(sessionCompanyId, 0);
                    var godownList = objProductHelper.GetGoDownMasterDropdown(sessionCompanyId, 0);
                    var gradeList = objProductHelper.GetGradeMasterDropdown(sessionCompanyId, 0);
                    var partyList = objProductHelper.GetSupplierMasterDropdown(sessionCompanyId, 0);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            string voucherNo = string.Empty;
                            SqlParameter[] sqlParameters = new SqlParameter[2];
                            sqlParameters[0] = new SqlParameter("@OblPrdType", "PIPE");
                            sqlParameters[1] = new SqlParameter("@Cmpvou", companyId);
                            DataTable dtNewOrdVNo = ObjDBConnection.CallStoreProcedure("GetLatestOblVNo", sqlParameters);
                            if (dtNewOrdVNo != null && dtNewOrdVNo.Rows.Count > 0)
                            {
                                int.TryParse(dtNewOrdVNo.Rows[0]["OblNVno"].ToString(), out int OblNVno);
                                OblNVno = OblNVno == 0 ? 1 : OblNVno;
                                voucherNo = OblNVno.ToString();
                            }

                            sqlParameters = null;
                            sqlParameters = new SqlParameter[30];
                            sqlParameters[0] = new SqlParameter("@OblNVno", voucherNo);
                            sqlParameters[1] = new SqlParameter("@OblDt", DateTime.Parse(dt.Rows[i]["INWARD DATE"].ToString()));
                            sqlParameters[2] = new SqlParameter("@OblCmpVou", companyId);

                            #region Godown
                            string godownId = string.Empty;
                            if (godownList != null && godownList.Count > 0)
                            {
                                godownId = godownList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["LOCATION"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                            }

                            if (!string.IsNullOrWhiteSpace(godownId))
                            {
                                sqlParameters[3] = new SqlParameter("@OblGdnVou", godownId);
                            }
                            #endregion

                            sqlParameters[4] = new SqlParameter("@OblLocVou", string.Empty);

                            #region Party
                            string partyId = string.Empty;
                            if (partyList != null && partyList.Count > 0)
                            {
                                partyId = partyList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["PARTY"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                            }

                            if (!string.IsNullOrWhiteSpace(partyId))
                            {
                                sqlParameters[5] = new SqlParameter("@OblAccVou", partyId);
                            }
                            #endregion

                            sqlParameters[6] = new SqlParameter("@OblPrdVou", productId);
                            sqlParameters[7] = new SqlParameter("@OblRem", string.Empty);
                            sqlParameters[8] = new SqlParameter("@LotSupCoilNo", dt.Rows[i]["PARTY COIL NO"].ToString());
                            sqlParameters[9] = new SqlParameter("@LotHeatNo", dt.Rows[i]["HEAT NO"].ToString());

                            #region Grade
                            string gradeId = string.Empty;
                            string grade = string.Empty;
                            if (gradeList != null && gradeList.Count > 0)
                            {
                                gradeId = gradeList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["GRADE"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                grade = gradeList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["GRADE"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                            }

                            if (!string.IsNullOrWhiteSpace(gradeId))
                            {
                                sqlParameters[10] = new SqlParameter("@LotGrdMscVou", gradeId);
                                sqlParameters[11] = new SqlParameter("@OblGrade", grade);
                            }

                            #endregion

                            #region Finish
                            string finishId = string.Empty;
                            string finish = string.Empty;
                            if (finishList != null && finishList.Count > 0)
                            {
                                finishId = finishList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["FINISH"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                finish = finishList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["FINISH"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                            }

                            if (!string.IsNullOrWhiteSpace(finishId))
                            {
                                sqlParameters[12] = new SqlParameter("@LotFinMscVou", finishId);
                                sqlParameters[13] = new SqlParameter("@OblFinish", finish);
                            }
                            #endregion

                            sqlParameters[14] = new SqlParameter("@LotWidth", dt.Rows[i]["WIDTH"].ToString());
                            sqlParameters[15] = new SqlParameter("@LotThick", dt.Rows[i]["THICK"].ToString());
                            sqlParameters[16] = new SqlParameter("@LotQty", dt.Rows[i]["WEIGHT"].ToString());
                            sqlParameters[17] = new SqlParameter("@LotInwWidth", dt.Rows[i]["WIDTH"].ToString());
                            sqlParameters[18] = new SqlParameter("@LotInwThick", dt.Rows[i]["THICK"].ToString());
                            sqlParameters[19] = new SqlParameter("@LotInwQty", dt.Rows[i]["WEIGHT"].ToString());
                            sqlParameters[20] = new SqlParameter("@PrdTyp", "PIPE");
                            sqlParameters[21] = new SqlParameter("@CmpVou", companyId);
                            sqlParameters[22] = new SqlParameter("@OblVou", 0);


                            sqlParameters[23] = new SqlParameter("@LotTyp", string.Empty);
                            string coilano = dt.Rows[i]["COIL NO"].ToString();
                            string temp = string.Empty;
                            int coilnumber = 0;

                            for (int c = 0; c < coilano.Length; c++)
                            {
                                if (Char.IsDigit(coilano[i]))
                                    temp += coilano[i];
                            }

                            if (temp.Length > 0)
                                coilnumber = int.Parse(temp);

                            sqlParameters[24] = new SqlParameter("@LotNo", coilnumber);

                            string _strLotCoilNo = string.Format("{0}", dt.Rows[i]["COIL NO"].ToString());
                            sqlParameters[25] = new SqlParameter("@CoilNo", _strLotCoilNo);
                            sqlParameters[26] = new SqlParameter("@Flg", 1);

                            sqlParameters[27] = new SqlParameter("@RefNo", dt.Rows[i]["MEMO NO"].ToString());
                            sqlParameters[28] = new SqlParameter("@LotPCS", dt.Rows[i]["NOS"].ToString());
                            sqlParameters[29] = new SqlParameter("@LotOD", dt.Rows[i]["SIZE"].ToString());

                            if (!string.IsNullOrWhiteSpace(gradeId) && !string.IsNullOrWhiteSpace(finishId) && !string.IsNullOrWhiteSpace(partyId) && !string.IsNullOrWhiteSpace(godownId))
                            {
                                DataTable DtState = ObjDBConnection.CallStoreProcedure("OBLMST_Insert", sqlParameters);
                                if (DtState != null && DtState.Rows.Count > 0)
                                {
                                    int status = DbConnection.ParseInt32(DtState.Rows[0][0].ToString());
                                }
                            }
                            else
                            {
                                bool isAllow = false;
                                if (string.IsNullOrWhiteSpace(godownId))
                                {
                                    if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value1.Equals(dt.Rows[i]["LOCATION"].ToString().Trim())).Count() <= 0)
                                    {
                                        godownId = dt.Rows[i]["LOCATION"].ToString().Trim();
                                        isAllow = true;
                                    }
                                    else
                                    {
                                        godownId = "-";
                                    }
                                }
                                else
                                {
                                    godownId = "-";
                                }

                                if (string.IsNullOrWhiteSpace(gradeId))
                                {
                                    if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value2.Equals(dt.Rows[i]["GRADE"].ToString().Trim())).Count() <= 0)
                                    {
                                        gradeId = dt.Rows[i]["GRADE"].ToString().Trim();
                                        isAllow = true;
                                    }
                                    else
                                    {
                                        gradeId = "-";
                                    }
                                }
                                else
                                {
                                    gradeId = "-";
                                }

                                if (string.IsNullOrWhiteSpace(partyId))
                                {
                                    if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value3.Equals(dt.Rows[i]["PARTY"].ToString().Trim())).Count() <= 0)
                                    {
                                        partyId = dt.Rows[i]["PARTY"].ToString().Trim();
                                        isAllow = true;
                                    }
                                    else
                                    {
                                        partyId = "-";
                                    }
                                }
                                else
                                {
                                    partyId = "-";
                                }

                                if (string.IsNullOrWhiteSpace(finishId))
                                {
                                    if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Text.Equals(dt.Rows[i]["FINISH"].ToString().Trim())).Count() <= 0)
                                    {
                                        finishId = dt.Rows[i]["FINISH"].ToString().Trim();
                                        isAllow = true;
                                    }
                                    else
                                    {
                                        finishId = "-";
                                    }
                                }
                                else
                                {
                                    finishId = "-";
                                }

                                if (string.IsNullOrWhiteSpace(finishId) && notFoundItems.Where(x => x.Text.Equals("Finish not found")).Count() <= 0)
                                {
                                    finishId = "Finish not found";
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(finishId))
                                        isAllow = false;
                                }
                                if (string.IsNullOrWhiteSpace(godownId) && notFoundItems.Where(x => x.Value1.Equals("Godown not found")).Count() <= 0)
                                {
                                    godownId = "Godown not found";
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(godownId))
                                        isAllow = false;
                                }
                                if (string.IsNullOrWhiteSpace(gradeId) && notFoundItems.Where(x => x.Value2.Equals("Grade not found")).Count() <= 0)
                                {
                                    gradeId = "Grade not found";
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(gradeId))
                                        isAllow = false;
                                }

                                if (string.IsNullOrWhiteSpace(partyId) && notFoundItems.Where(x => x.Value3.Equals("Party not found")).Count() <= 0)
                                {
                                    partyId = "Party not found";
                                }
                                else
                                {
                                    isAllow = false;
                                }
                                if (isAllow)
                                    notFoundItems.Add(new CustomDropDown
                                    {
                                        Text = finishId,
                                        Value1 = godownId,
                                        Value2 = gradeId,
                                        Value3 = partyId
                                    });
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    inserted = 1;
                }
                else
                {

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
            string voucherNo = string.Empty;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@OblPrdType", "COIL");
                sqlParameters[1] = new SqlParameter("@Cmpvou", companyId);
                DataTable dtNewOrdVNo = ObjDBConnection.CallStoreProcedure("GetLatestOblVNo", sqlParameters);
                if (dtNewOrdVNo != null && dtNewOrdVNo.Rows.Count > 0)
                {
                    int.TryParse(dtNewOrdVNo.Rows[0]["OblNVno"].ToString(), out int OblNVno);
                    OblNVno = OblNVno == 0 ? 1 : OblNVno;
                    voucherNo = OblNVno.ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return voucherNo;
        }
    }
}
