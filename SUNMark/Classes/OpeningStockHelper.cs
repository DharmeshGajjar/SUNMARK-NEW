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
                    var productList = objProductHelper.GetProductMasterWithCodeDropdown(sessionCompanyId);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            #region Validation

                            #region Godown
                            string godownId = string.Empty;
                            if (godownList != null && godownList.Count > 0)
                            {
                                godownId = godownList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["LOCATION"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                            }
                            #endregion

                            #region Party
                            string partyId = string.Empty;
                            if (partyList != null && partyList.Count > 0)
                            {
                                partyId = partyList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["PARTY"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                            }

                            #endregion

                            #region Grade
                            string gradeId = string.Empty;
                            string grade = string.Empty;
                            if (gradeList != null && gradeList.Count > 0)
                            {
                                gradeId = gradeList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["GRADE"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                grade = gradeList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["GRADE"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                            }
                            #endregion

                            #region Product
                            string prdId = string.Empty;
                            if (productList != null && productList.Count > 0)
                            {
                                if (!string.IsNullOrWhiteSpace(Convert.ToString(dt.Rows[i]["PRODUCT"])))
                                {
                                    prdId = productList.Where(x => x.Value1.ToLower().Trim().ToString().Equals(dt.Rows[i]["PRODUCT"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                }
                            }
                            #endregion

                            bool isgodownerror = false, isgradeerror = false, ispartyerror = false, isproducterror = false;

                            if (string.IsNullOrWhiteSpace(prdId) && !string.IsNullOrWhiteSpace(Convert.ToString(dt.Rows[i]["PRODUCT"])))
                            {
                                if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value1.Equals(dt.Rows[i]["PRODUCT"].ToString().Trim())).Count() <= 0)
                                {
                                    prdId = dt.Rows[i]["PRODUCT"].ToString().Trim();
                                    isproducterror = true;
                                }
                                else
                                {
                                    prdId = "Product not found";
                                }
                            }
                            else
                            {
                                prdId = "-";
                            }


                            if (string.IsNullOrWhiteSpace(godownId))
                            {
                                if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value1.Equals(dt.Rows[i]["LOCATION"].ToString().Trim())).Count() <= 0)
                                {
                                    godownId = dt.Rows[i]["LOCATION"].ToString().Trim();
                                    isgodownerror = true;
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

                            if (string.IsNullOrWhiteSpace(partyId))
                            {
                                if (notFoundItems.Count <= 0 || notFoundItems.Where(x => x.Value3.Equals(dt.Rows[i]["PARTY"].ToString().Trim())).Count() <= 0)
                                {
                                    partyId = dt.Rows[i]["PARTY"].ToString().Trim();
                                    ispartyerror = true;
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

                            //if (isprefixerror)
                            //{
                            //    prefixId = "Coil Prefix not found";
                            //}
                            //if (isgodownerror)
                            //{
                            //    godownId = "Godown not found";
                            //}

                            //if (isgradeerror)
                            //{
                            //    gradeId = "Grade not found";
                            //}

                            //if (ispartyerror)
                            //{
                            //    partyId = "Party not found";
                            //}


                            if (ispartyerror || isgradeerror || isgodownerror || isproducterror)
                                notFoundItems.Add(new CustomDropDown
                                {
                                    //Text = finishId,
                                    Value1 = godownId,
                                    Value2 = gradeId,
                                    Value3 = partyId,
                                    Value4 = prdId
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

                                string voucherNo = GetVoucherNo(sessionCompanyId);
                                SqlParameter[] sqlParameters = new SqlParameter[38];
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

                                
                                #region Product
                                string prdId = string.Empty;
                                if (productList != null && productList.Count > 0)
                                {
                                    prdId = productList.Where(x => x.Value1.ToLower().Trim().ToString().Equals(dt.Rows[i]["PRODUCT"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                }

                                if (!string.IsNullOrWhiteSpace(prdId))
                                {
                                    sqlParameters[6] = new SqlParameter("@OblPrdVou", prdId);
                                }

                                #endregion
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

                                sqlParameters[12] = new SqlParameter("@LotFinMscVou", 0);
                                sqlParameters[13] = new SqlParameter("@OblFinish", "");
                                sqlParameters[14] = new SqlParameter("@LotWidth", Convert.ToDecimal(dt.Rows[i]["WIDTH"].ToString()));
                                sqlParameters[15] = new SqlParameter("@LotThick", Convert.ToDecimal(dt.Rows[i]["THICK"].ToString()));
                                sqlParameters[16] = new SqlParameter("@LotQty", Convert.ToDecimal(dt.Rows[i]["WEIGHT"].ToString()));
                                sqlParameters[17] = new SqlParameter("@LotInwWidth", Convert.ToDecimal(dt.Rows[i]["WIDTH"].ToString()));
                                sqlParameters[18] = new SqlParameter("@LotInwThick", Convert.ToDecimal(dt.Rows[i]["THICK"].ToString()));
                                sqlParameters[19] = new SqlParameter("@LotInwQty", Convert.ToDecimal(dt.Rows[i]["WEIGHT"].ToString()));
                                sqlParameters[20] = new SqlParameter("@PrdTyp", "COIL");
                                sqlParameters[21] = new SqlParameter("@CmpVou", sessionCompanyId);
                                sqlParameters[22] = new SqlParameter("@OblVou", 0);


                                sqlParameters[23] = new SqlParameter("@LotTyp", dt.Rows[i]["COIL PREFIX"].ToString());
                                
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

                                if (!string.IsNullOrWhiteSpace(dt.Rows[i]["COIL PREFIX"].ToString()))
                                {
                                    string prefix = dt.Rows[i]["COIL PREFIX"].ToString();
                                    _strLotCoilNo = prefix + "-" + _strLotCoilNo;
                                }
                                    
                                sqlParameters[25] = new SqlParameter("@Flg", 1);
                                sqlParameters[26] = new SqlParameter("@RefNo", dt.Rows[i]["MEMO NO"].ToString());
                                sqlParameters[27] = new SqlParameter("@LotOD", dt.Rows[i]["SIZE"].ToString());
                                sqlParameters[28] = new SqlParameter("@LotPrcTypCD", 0);
                                sqlParameters[29] = new SqlParameter("@LotPCS", 0);
                                sqlParameters[30] = new SqlParameter("@LotFeetPer", 0);
                                sqlParameters[31] = new SqlParameter("@NB", "");
                                sqlParameters[32] = new SqlParameter("@SCH", "");

                                string coiltype = string.Empty;
                                if (dt.Rows[i]["COIL TYPE"].ToString() == "1")
                                {
                                    coiltype = "Slited Coil";
                                }
                                else if (dt.Rows[i]["COIL TYPE"].ToString() == "2")
                                {
                                    coiltype = "Bal. Patta";
                                }
                                else if (dt.Rows[i]["COIL TYPE"].ToString() == "3")
                                {
                                    coiltype = "Scrap";
                                }
                                else if (dt.Rows[i]["COIL TYPE"].ToString() == "4")
                                {
                                    coiltype = "M.Coil";
                                }
                                else if (dt.Rows[i]["COIL TYPE"].ToString() == "5")
                                {
                                    coiltype = "In Trans";
                                }
                                sqlParameters[33] = new SqlParameter("@CoilType", coiltype);
                                sqlParameters[34] = new SqlParameter("@CoilTypeVou", dt.Rows[i]["COIL TYPE"].ToString());

                                if (!string.IsNullOrWhiteSpace(dt.Rows[i]["SUFIX"].ToString()))
                                {
                                    _strLotCoilNo = _strLotCoilNo + "-" + dt.Rows[i]["SUFIX"].ToString();
                                    sqlParameters[35] = new SqlParameter("@CoilSuffix", dt.Rows[i]["SUFIX"].ToString());
                                }
                                else
                                {
                                    sqlParameters[35] = new SqlParameter("@CoilSuffix", "");
                                }

                                sqlParameters[36] = new SqlParameter("@CoilNo", _strLotCoilNo);
                                sqlParameters[37] = new SqlParameter("@NextProc", 0);


                                if (!string.IsNullOrWhiteSpace(gradeId) && !string.IsNullOrWhiteSpace(partyId) && !string.IsNullOrWhiteSpace(godownId))
                                {
                                    DataTable DtState = ObjDBConnection.CallStoreProcedure("OBLMST_Insert", sqlParameters);
                                    if (DtState != null && DtState.Rows.Count > 0)
                                    {
                                        int status = DbConnection.ParseInt32(DtState.Rows[0][0].ToString());
                                    }
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
                else if (type.ToLower() == "pipe")
                {
                    var finishList = objProductHelper.GetFinishMasterDropdown(sessionCompanyId, 0);
                    var godownList = objProductHelper.GetGoDownMasterDropdown(sessionCompanyId, 0);
                    var gradeList = objProductHelper.GetGradeMasterDropdown(sessionCompanyId, 0);
                    var partyList = objProductHelper.GetSupplierMasterDropdown(sessionCompanyId, 0);
                    var processList = objProductHelper.GetLotPrcTypMasterDropdown(sessionCompanyId, 0,"");
                    var nextprocessList = objProductHelper.GetLotPrcTypMasterDropdown(sessionCompanyId, 0,"");

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
                            sqlParameters = new SqlParameter[38];
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
                                if (Char.IsDigit(coilano[c]))
                                    temp += coilano[c];
                            }

                            if (temp.Length > 0)
                                coilnumber = int.Parse(temp);

                            sqlParameters[24] = new SqlParameter("@LotNo", coilnumber);

                            string _strLotCoilNo = string.Format("{0}", dt.Rows[i]["COIL NO"].ToString());
                            sqlParameters[25] = new SqlParameter("@Flg", 1);
                            

                            sqlParameters[26] = new SqlParameter("@RefNo", dt.Rows[i]["MEMO NO"].ToString());
                            sqlParameters[27] = new SqlParameter("@LotOD", dt.Rows[i]["SIZE"].ToString());
                            
                            #region Process
                            string processId = string.Empty;
                            string process = string.Empty;
                            if (processList != null && processList.Count > 0)
                            {
                                processId = processList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["PROCESS DONE"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                process = processList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["PROCESS DONE"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                            }

                            if (!string.IsNullOrWhiteSpace(processId))
                            {
                                sqlParameters[28] = new SqlParameter("@LotPrcTypCD", processId);
                            }
                            #endregion

                            sqlParameters[29] = new SqlParameter("@LotPCS", dt.Rows[i]["NOS"].ToString());
                            sqlParameters[30] = new SqlParameter("@LotFeetPer", dt.Rows[i]["LENGTH"].ToString());

                            sqlParameters[31] = new SqlParameter("@NB", dt.Rows[i]["NB"].ToString());
                            sqlParameters[32] = new SqlParameter("@SCH", dt.Rows[i]["SCH"].ToString());

                            sqlParameters[33] = new SqlParameter("@CoilType", "");
                            sqlParameters[34] = new SqlParameter("@CoilTypeVou", 0);
                            sqlParameters[35] = new SqlParameter("@CoilSuffix", 0);
                            sqlParameters[36] = new SqlParameter("@CoilNo", _strLotCoilNo);

                            #region NextProcess
                            string nxtprocessId = string.Empty;
                            string nxtprocess = string.Empty;
                            if (nextprocessList != null && nextprocessList.Count > 0)
                            {
                                nxtprocessId = nextprocessList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["NEXT PROCESS"].ToString().Trim().ToLower())).Select(x => x.Value).FirstOrDefault();
                                nxtprocess = nextprocessList.Where(x => x.Text.ToLower().Trim().ToString().Equals(dt.Rows[i]["NEXT PROCESS"].ToString().Trim().ToLower())).Select(x => x.Text).FirstOrDefault();
                            }

                            if (!string.IsNullOrWhiteSpace(nxtprocessId))
                            {
                                sqlParameters[37] = new SqlParameter("@NextProc", nxtprocessId);
                            }
                            #endregion

                            if (!string.IsNullOrWhiteSpace(gradeId) && !string.IsNullOrWhiteSpace(finishId) && !string.IsNullOrWhiteSpace(godownId))
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
