using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SUNMark.Models;

namespace SUNMark.Classes
{
    public class ProductHelpers
    {

        public DbConnection ObjDBConnection = new DbConnection();

        public List<SelectListItem> GetProductMasterDropdown(int companyId)
        {
            List<SelectListItem> productList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@PrdVOU", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                sqlParameters[4] = new SqlParameter("@CmpVou", companyId);
                DataTable DtProductList = ObjDBConnection.CallStoreProcedure("GetProductDetails", sqlParameters);
                if (DtProductList != null && DtProductList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtProductList.Rows.Count; i++)
                    {
                        SelectListItem productItem = new SelectListItem();
                        productItem.Text = DtProductList.Rows[i]["PrdNm"].ToString();
                        productItem.Value = DtProductList.Rows[i]["PrdVou"].ToString();
                        productList.Add(productItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return productList;
        }

        public List<SelectListItem> GetPrdTypeWiseProductDropdown(int companyId, string PrdType)
        {
            List<SelectListItem> productList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@PrdVOU", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                sqlParameters[4] = new SqlParameter("@CmpVou", companyId);
                sqlParameters[5] = new SqlParameter("@PrdType", PrdType);
                DataTable DtProductList = ObjDBConnection.CallStoreProcedure("GetPrdTypeProductDetails", sqlParameters);
                if (DtProductList != null && DtProductList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtProductList.Rows.Count; i++)
                    {
                        SelectListItem productItem = new SelectListItem();
                        productItem.Text = DtProductList.Rows[i]["PrdNm"].ToString();
                        productItem.Value = DtProductList.Rows[i]["PrdVou"].ToString();
                        productList.Add(productItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return productList;
        }


        public List<SelectListItem> GetMasterMenuDropdown(int companyId)
        {
            List<SelectListItem> MenuList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[0];
                DataTable DtMenuList = ObjDBConnection.CallStoreProcedure("GetMasterMenuList", sqlParameters);
                if (DtMenuList != null && DtMenuList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtMenuList.Rows.Count; i++)
                    {
                        SelectListItem menuItem = new SelectListItem();
                        menuItem.Text = DtMenuList.Rows[i]["Name"].ToString();
                        menuItem.Value = DtMenuList.Rows[i]["ModuleID"].ToString();
                        MenuList.Add(menuItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return MenuList;
        }

        public List<CustomDropDown> GetProductCustomDropdown(int companyId)
        {
            List<CustomDropDown> productList = new List<CustomDropDown>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@PrdVOU", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                sqlParameters[4] = new SqlParameter("@CmpVou", companyId);
                DataTable DtProductList = ObjDBConnection.CallStoreProcedure("GetProductDetails", sqlParameters);
                if (DtProductList != null && DtProductList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtProductList.Rows.Count; i++)
                    {
                        CustomDropDown productItem = new CustomDropDown();
                        productItem.Text = DtProductList.Rows[i]["PrdName"].ToString();
                        productItem.Value = DtProductList.Rows[i]["PrdVou"].ToString();
                        productItem.Value1 = DtProductList.Rows[i]["PrdExYN"].ToString();
                        productList.Add(productItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return productList;
        }

        public List<SelectListItem> GetProductYesNo()
        {
            List<SelectListItem> productList = new List<SelectListItem>();
            try
            {
                productList.Add(new SelectListItem
                {
                    Text = "Yes",
                    Value = "0"
                });

                productList.Add(new SelectListItem
                {
                    Text = "No",
                    Value = "1"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return productList;
        }

        public List<SelectListItem> GetCoilType()
        {
            List<SelectListItem> coiltypeList = new List<SelectListItem>();
            try
            {
                coiltypeList.Add(new SelectListItem
                {
                    Text = "Coil",
                    Value = "1"
                });

                coiltypeList.Add(new SelectListItem
                {
                    Text = "Pipe",
                    Value = "2"
                });
                coiltypeList.Add(new SelectListItem
                {
                    Text = "Other",
                    Value = "3"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return coiltypeList;
        }

        public List<SelectListItem> GetShift()
        {
            List<SelectListItem> shifList = new List<SelectListItem>();
            try
            {
                shifList.Add(new SelectListItem
                {
                    Text = "First",
                    Value = "1"
                });

                shifList.Add(new SelectListItem
                {
                    Text = "Second",
                    Value = "2"
                });

                shifList.Add(new SelectListItem
                {
                    Text = "Office",
                    Value = "3"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return shifList;
        }


        public List<SelectListItem> GetShiftNew()
        {
            List<SelectListItem> shifList = new List<SelectListItem>();
            try
            {
                shifList.Add(new SelectListItem
                {
                    Text = "First",
                    Value = "1"
                });

                shifList.Add(new SelectListItem
                {
                    Text = "Second",
                    Value = "2"
                });
                shifList.Add(new SelectListItem
                {
                    Text = "Both",
                    Value = "3"
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return shifList;
        }

        public List<SelectListItem> GetDataType()
        {
            List<SelectListItem> DataTypeList = new List<SelectListItem>();
            try
            {
                DataTypeList.Add(new SelectListItem
                {
                    Text = "Character",
                    Value = "1"
                });

                DataTypeList.Add(new SelectListItem
                {
                    Text = "Numeric",
                    Value = "2"
                });
                DataTypeList.Add(new SelectListItem
                {
                    Text = "Date",
                    Value = "3"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return DataTypeList;
        }

        public List<SelectListItem> GetTextAlign()
        {
            List<SelectListItem> AlignList = new List<SelectListItem>();
            try
            {
                AlignList.Add(new SelectListItem
                {
                    Text = "L",
                    Value = "1"
                });

                AlignList.Add(new SelectListItem
                {
                    Text = "R",
                    Value = "2"
                });
                AlignList.Add(new SelectListItem
                {
                    Text = "C",
                    Value = "3"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return AlignList;
        }

        public List<SelectListItem> GetReportView()
        {
            List<SelectListItem> TypeList = new List<SelectListItem>();
            try
            {
                TypeList.Add(new SelectListItem
                {
                    Text = "View",
                    Value = "1"
                });

                TypeList.Add(new SelectListItem
                {
                    Text = "Report",
                    Value = "2"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return TypeList;
        }


        public List<SelectListItem> GetMenuMasterDropdown()
        {
            List<SelectListItem> MenuList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("@MnuVOU", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                DataTable DtMenuList = ObjDBConnection.CallStoreProcedure("GetMenuMstDetails", sqlParameters);
                if (DtMenuList != null && DtMenuList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtMenuList.Rows.Count; i++)
                    {
                        SelectListItem menuItem = new SelectListItem();
                        menuItem.Text = DtMenuList.Rows[i]["Name"].ToString();
                        menuItem.Value = DtMenuList.Rows[i]["ModuleId"].ToString();
                        MenuList.Add(menuItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return MenuList;
        }

        public List<SelectListItem> GetTrnTypeDropdown()
        {
            List<SelectListItem> MenuList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@skiprecord", 0);
                sqlParameters[1] = new SqlParameter("@pagesize", 0);
                DataTable DtMenuList = ObjDBConnection.CallStoreProcedure("GetTranTypeDetails", sqlParameters);
                if (DtMenuList != null && DtMenuList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtMenuList.Rows.Count; i++)
                    {
                        SelectListItem menuItem = new SelectListItem();
                        menuItem.Text = DtMenuList.Rows[i]["TrnType"].ToString();
                        menuItem.Value = DtMenuList.Rows[i]["SrNo"].ToString();
                        MenuList.Add(menuItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return MenuList;
        }


        public List<SelectListItem> GetStateMasterDropdown(int companyId, int isAdministrator)
        {
            List<SelectListItem> StateList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "STA");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                //if (isAdministrator == 1)
                //    companyId = 0;
                sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                DataTable DtStateList = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtStateList != null && DtStateList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtStateList.Rows.Count; i++)
                    {
                        SelectListItem StateItem = new SelectListItem();
                        StateItem.Text = DtStateList.Rows[i]["MscNm"].ToString();
                        StateItem.Value = DtStateList.Rows[i]["MscVou"].ToString();
                        StateList.Add(StateItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StateList;
        }

        public List<SelectListItem> GetTransMasterDropdown(int companyId, int isAdministrator)
        {
            List<SelectListItem> TransList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "TRN");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                //if (isAdministrator == 1)
                //    companyId = 0;
                sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                DataTable DtTransList = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtTransList != null && DtTransList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtTransList.Rows.Count; i++)
                    {
                        SelectListItem StateItem = new SelectListItem();
                        StateItem.Text = DtTransList.Rows[i]["MscNm"].ToString();
                        StateItem.Value = DtTransList.Rows[i]["MscVou"].ToString();
                        TransList.Add(StateItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return TransList;
        }

        public List<SelectListItem> GetGradeMasterDropdown(int companyId, int isAdministrator)
        {
            List<SelectListItem> GradeList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "GRD");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                //if (isAdministrator == 1)
                //    companyId = 0;
                sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                DataTable DtGradeList = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtGradeList != null && DtGradeList.Rows.Count > 0)
                {
                    SelectListItem GradeItem = new SelectListItem();
                    for (int i = 0; i < DtGradeList.Rows.Count; i++)
                    {
                        SelectListItem GradeItem1 = new SelectListItem();
                        GradeItem1.Text = DtGradeList.Rows[i]["MscNm"].ToString();
                        GradeItem1.Value = DtGradeList.Rows[i]["MscVou"].ToString();
                        GradeList.Add(GradeItem1);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return GradeList;
        }
        public List<SelectListItem> GetQualityMasterDropdown(int companyId, int isAdministrator)
        {
            List<SelectListItem> QualityList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "QLT");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                //if (isAdministrator == 1)
                //    companyId = 0;
                sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                DataTable DtQualityList = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtQualityList != null && DtQualityList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtQualityList.Rows.Count; i++)
                    {
                        SelectListItem QualityItem = new SelectListItem();
                        QualityItem.Text = DtQualityList.Rows[i]["MscNm"].ToString();
                        QualityItem.Value = DtQualityList.Rows[i]["MscVou"].ToString();
                        QualityList.Add(QualityItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return QualityList;
        }
        public List<SelectListItem> GetFinishMasterDropdown(int companyId, int isAdministrator)
        {
            List<SelectListItem> FinishList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "FIN");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                //if (isAdministrator == 1)
                //    companyId = 0;
                sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                DataTable DtFinishList = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtFinishList != null && DtFinishList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtFinishList.Rows.Count; i++)
                    {
                        SelectListItem FinishItem = new SelectListItem();
                        FinishItem.Text = DtFinishList.Rows[i]["MscNm"].ToString();
                        FinishItem.Value = DtFinishList.Rows[i]["MscVou"].ToString();
                        FinishList.Add(FinishItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FinishList;
        }

        public List<SelectListItem> GetLotPrcTypMasterDropdown(int companyId, int isAdministrator, string condi)
        {
            List<SelectListItem> FinishList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[7];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "PRC");
                sqlParameters[2] = new SqlParameter("@Flg", 3);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                //if (isAdministrator == 1)
                //    companyId = 0;
                sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                sqlParameters[6] = new SqlParameter("@Condi", condi);
                DataTable DtFinishList = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtFinishList != null && DtFinishList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtFinishList.Rows.Count; i++)
                    {
                        SelectListItem FinishItem = new SelectListItem();
                        FinishItem.Text = DtFinishList.Rows[i]["MscNm"].ToString();
                        FinishItem.Value = DtFinishList.Rows[i]["MscVou"].ToString();
                        FinishList.Add(FinishItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FinishList;
        }

        public List<SelectListItem> GetCompanyMasterDropdown(long companyId, long isAdministrator)
        {
            List<SelectListItem> CompanyList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@CmpVou", companyId);
                DataTable DtCompany = ObjDBConnection.CallStoreProcedure("GetDepartmentDetail", sqlParameters);
                if (DtCompany != null && DtCompany.Rows.Count > 0)
                {
                    for (int i = 0; i < DtCompany.Rows.Count; i++)
                    {
                        SelectListItem CompanyItem = new SelectListItem();
                        CompanyItem.Text = DtCompany.Rows[i]["DepName"].ToString();
                        CompanyItem.Value = DtCompany.Rows[i]["DepVou"].ToString();
                        CompanyList.Add(CompanyItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return CompanyList;
        }

        public List<SelectListItem> GetGoDownMasterDropdown(int companyId, int isAdministrator)
        {
            List<SelectListItem> GoDownList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@CmpVou", companyId);
                DataTable DtGoDownList = ObjDBConnection.CallStoreProcedure("GetGoDownDetail", sqlParameters);
                if (DtGoDownList != null && DtGoDownList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtGoDownList.Rows.Count; i++)
                    {
                        SelectListItem GoDownItem = new SelectListItem();
                        GoDownItem.Text = DtGoDownList.Rows[i]["GdnNm"].ToString();
                        GoDownItem.Value = DtGoDownList.Rows[i]["GdnVou"].ToString();
                        GoDownList.Add(GoDownItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return GoDownList;
        }

        public List<SelectListItem> GetLocationMasterDropdown(int companyId, int isAdministrator)
        {
            List<SelectListItem> LocationList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@CmpVou", companyId);
                DataTable DtLocationList = ObjDBConnection.CallStoreProcedure("GetLocationDetail", sqlParameters);
                if (DtLocationList != null && DtLocationList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtLocationList.Rows.Count; i++)
                    {
                        SelectListItem LocationItem = new SelectListItem();
                        LocationItem.Text = DtLocationList.Rows[i]["LocNm"].ToString();
                        LocationItem.Value = DtLocationList.Rows[i]["LocVou"].ToString();
                        LocationList.Add(LocationItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return LocationList;
        }

        public List<SelectListItem> GetSupplierMasterDropdown(int companyId, int isAdministrator)
        {
            List<SelectListItem> SupplierList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@CmpVou", companyId);
                DataTable DtSupplierList = ObjDBConnection.CallStoreProcedure("GetAccountDetail", sqlParameters);
                if (DtSupplierList != null && DtSupplierList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtSupplierList.Rows.Count; i++)
                    {
                        SelectListItem SupplierItem = new SelectListItem();
                        SupplierItem.Text = DtSupplierList.Rows[i]["AccNm"].ToString();
                        SupplierItem.Value = DtSupplierList.Rows[i]["AccVou"].ToString();
                        SupplierList.Add(SupplierItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return SupplierList;
        }


        public List<SelectListItem> GetProductMasterDropdown(int companyId, int isAdministrator, String PrdType)
        {
            List<SelectListItem> ProductList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@CmpVou", companyId);
                sqlParameters[1] = new SqlParameter("@flg", 1);
                sqlParameters[2] = new SqlParameter("@PrdTyp", PrdType);
                DataTable DtProductList = ObjDBConnection.CallStoreProcedure("GetProductDetail", sqlParameters);
                if (DtProductList != null && DtProductList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtProductList.Rows.Count; i++)
                    {
                        SelectListItem ProductItem = new SelectListItem();
                        ProductItem.Text = DtProductList.Rows[i]["PrdNm"].ToString();
                        ProductItem.Value = DtProductList.Rows[i]["PrdVou"].ToString();
                        ProductList.Add(ProductItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ProductList;
        }

        public List<SelectListItem> GetLotMasterDropdown(int companyId, int isAdministrator)
        {
            List<SelectListItem> LotList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "LOT");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                //if (isAdministrator == 1)
                //    companyId = 0;
                sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                DataTable DtLotList = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtLotList != null && DtLotList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtLotList.Rows.Count; i++)
                    {
                        SelectListItem LotItem = new SelectListItem();
                        LotItem.Text = DtLotList.Rows[i]["MscNm"].ToString();
                        LotItem.Value = DtLotList.Rows[i]["MscVou"].ToString();
                        LotList.Add(LotItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return LotList;
        }
        public List<SelectListItem> GetVoucherTypeDropdown(string type, int companyId)
        {
            List<SelectListItem> VouTypeList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@VTYTRNMSCCD", type);
                sqlParameters[1] = new SqlParameter("@CmpVou", companyId);
                DataTable DtVouTyList = ObjDBConnection.CallStoreProcedure("GetVoucherTypeDetail", sqlParameters);
                if (DtVouTyList != null && DtVouTyList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtVouTyList.Rows.Count; i++)
                    {
                        SelectListItem VouTypeItem = new SelectListItem();
                        VouTypeItem.Text = DtVouTyList.Rows[i]["VtyType"].ToString();
                        VouTypeItem.Value = DtVouTyList.Rows[i]["VtyVou"].ToString();
                        VouTypeList.Add(VouTypeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return VouTypeList;
        }

        public List<SelectListItem> GetLotMasterDropdown_1(int companyId, int isAdministrator)
        {
            List<SelectListItem> LotList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "LOT");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                //if (isAdministrator == 1)
                //    companyId = 0;
                sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                DataTable DtLotList = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtLotList != null && DtLotList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtLotList.Rows.Count; i++)
                    {
                        SelectListItem LotItem = new SelectListItem();
                        LotItem.Text = DtLotList.Rows[i]["MscNm"].ToString();
                        LotItem.Value = DtLotList.Rows[i]["MscNm"].ToString();
                        LotList.Add(LotItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return LotList;
        }
        public List<SelectListItem> GetLotMasterDropdown_2(int companyId, int isAdministrator)
        {
            List<SelectListItem> LotList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "LOT");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                //if (isAdministrator == 1)
                //    companyId = 0;
                sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                DataTable DtLotList = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtLotList != null && DtLotList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtLotList.Rows.Count; i++)
                    {
                        SelectListItem LotItem = new SelectListItem();
                        LotItem.Text = DtLotList.Rows[i]["MscNm"].ToString();
                        LotItem.Value = DtLotList.Rows[i]["MscVou"].ToString();
                        LotList.Add(LotItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return LotList;
        }

        public List<SelectListItem> GetStockYN()
        {
            List<SelectListItem> StockYNList = new List<SelectListItem>();
            try
            {
                StockYNList.Add(new SelectListItem
                {
                    Text = "Yes",
                    Value = "1"
                });

                StockYNList.Add(new SelectListItem
                {
                    Text = "No",
                    Value = "2"
                });
                StockYNList.Add(new SelectListItem
                {
                    Text = "InProcess",
                    Value = "3"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StockYNList;
        }

        public List<SelectListItem> GetStockYNNew()
        {
            List<SelectListItem> StockYNList = new List<SelectListItem>();
            try
            {
                StockYNList.Add(new SelectListItem
                {
                    Text = "Yes",
                    Value = "1",
                    Selected = true
                });

                StockYNList.Add(new SelectListItem
                {
                    Text = "No",
                    Value = "2"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StockYNList;
        }

        public List<SelectListItem> GetMainCoilType()
        {
            List<SelectListItem> CoilTypeList = new List<SelectListItem>();
            try
            {
                CoilTypeList.Add(new SelectListItem
                {
                    Text = "Slited Coil",
                    Value = "1"
                });
                CoilTypeList.Add(new SelectListItem
                {
                    Text = "Bal. Patta",
                    Value = "2"
                });
                CoilTypeList.Add(new SelectListItem
                {
                    Text = "Scrap",
                    Value = "3"
                });
                CoilTypeList.Add(new SelectListItem
                {
                    Text = "M.Coil",
                    Value = "4"
                });
                CoilTypeList.Add(new SelectListItem
                {
                    Text = "In Trans",
                    Value = "5"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return CoilTypeList;
        }

        public List<SelectListItem> GetPicklingStatus()
        {
            List<SelectListItem> StatusList = new List<SelectListItem>();
            try
            {
                StatusList.Add(new SelectListItem
                {
                    Text = "AP",
                    Value = "1"
                });
                StatusList.Add(new SelectListItem
                {
                    Text = "Final Pickling",
                    Value = "2"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StatusList;
        }

        public List<SelectListItem> GetStraightingStatus()
        {
            List<SelectListItem> StatusList = new List<SelectListItem>();
            try
            {
                StatusList.Add(new SelectListItem
                {
                    Text = "AP",
                    Value = "1"
                });
                StatusList.Add(new SelectListItem
                {
                    Text = "Final Straighting",
                    Value = "2"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StatusList;
        }

        public List<SelectListItem> GetInwardCoilType()
        {
            List<SelectListItem> CoilTypeList = new List<SelectListItem>();
            try
            {
                CoilTypeList.Add(new SelectListItem
                {
                    Text = "Slited Coil",
                    Value = "1"
                });
                CoilTypeList.Add(new SelectListItem
                {
                    Text = "Bal. Patta",
                    Value = "2"
                });
                CoilTypeList.Add(new SelectListItem
                {
                    Text = "Scrap",
                    Value = "3"
                });
                CoilTypeList.Add(new SelectListItem
                {
                    Text = "M.Coil",
                    Value = "4"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return CoilTypeList;
        }


        public List<SelectListItem> GetRecIss()
        {
            List<SelectListItem> RecIssList = new List<SelectListItem>();
            try
            {
                RecIssList.Add(new SelectListItem
                {
                    Text = "All",
                    Value = "1"
                });

                RecIssList.Add(new SelectListItem
                {
                    Text = "Recieve",
                    Value = "2"
                });
                RecIssList.Add(new SelectListItem
                {
                    Text = "Issue",
                    Value = "3"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RecIssList;
        }
        public List<SelectListItem> GetType()
        {
            List<SelectListItem> RecIssList = new List<SelectListItem>();
            try
            {
                RecIssList.Add(new SelectListItem
                {
                    Text = "Split",
                    Value = "1"
                });

                RecIssList.Add(new SelectListItem
                {
                    Text = "Slit",
                    Value = "2"
                });
                RecIssList.Add(new SelectListItem
                {
                    Text = "Thickness",
                    Value = "3"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RecIssList;
        }

        public List<CustomDropDown> GetProductMasterWithCodeDropdown(int companyId)
        {
            List<CustomDropDown> productList = new List<CustomDropDown>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@PrdVOU", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                sqlParameters[4] = new SqlParameter("@CmpVou", companyId);
                DataTable DtProductList = ObjDBConnection.CallStoreProcedure("GetProductDetails", sqlParameters);
                if (DtProductList != null && DtProductList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtProductList.Rows.Count; i++)
                    {
                        CustomDropDown productItem = new CustomDropDown();
                        productItem.Text = DtProductList.Rows[i]["PrdNm"].ToString();
                        productItem.Value = DtProductList.Rows[i]["PrdVou"].ToString();
                        productItem.Value1 = DtProductList.Rows[i]["PrdCd"].ToString();
                        productList.Add(productItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return productList;
        }

        public List<SelectListItem> GetPrTypeMasterDropdown(int companyId, int isAdministrator)
        {
            List<SelectListItem> GradeList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "GRD");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                //if (isAdministrator == 1)
                //    companyId = 0;
                sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                DataTable DtGradeList = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtGradeList != null && DtGradeList.Rows.Count > 0)
                {
                    for (int i = 0; i < DtGradeList.Rows.Count; i++)
                    {
                        SelectListItem GradeItem = new SelectListItem();
                        GradeItem.Text = DtGradeList.Rows[i]["MscNm"].ToString();
                        GradeItem.Value = DtGradeList.Rows[i]["MscVou"].ToString();
                        GradeList.Add(GradeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return GradeList;
        }
        public List<SelectListItem> GetTypeList()
        {
            List<SelectListItem> AlignList = new List<SelectListItem>();
            try
            {
                AlignList.Add(new SelectListItem
                {
                    Text = "Textbox",
                    Value = "1"
                });

                AlignList.Add(new SelectListItem
                {
                    Text = "Dropdown",
                    Value = "2"
                });
                AlignList.Add(new SelectListItem
                {
                    Text = "Radio",
                    Value = "3"
                });
                AlignList.Add(new SelectListItem
                {
                    Text = "Checkbox",
                    Value = "4"
                });
                AlignList.Add(new SelectListItem
                {
                    Text = "Textarea",
                    Value = "5"
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return AlignList;
        }
    }
}
