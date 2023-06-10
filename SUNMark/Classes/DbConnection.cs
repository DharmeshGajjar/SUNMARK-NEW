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
    public class DbConnection
    {

        //<<<<<<< HEAD
        //public static string ConnectionString = "Server=144.91.71.201;Database=SUNMark;User ID=Piosun;Password=pio*123";
        //=======
        //public static string ConnectionString = "Server=144.91.71.201;Database=PIOSUN;User ID=Piosun;Password=pio*123";
        //>>>>>>> 8daeec391f04a8e54ddca2328276bd8948d5a6a1
        //public static string ConnectionString = "Server=144.91.71.201;Database=PIOSUN;User ID=Sun;Password=pio*123";

        public static string ConnectionString = "Server=144.91.71.201;Database=SUNMark;User ID=Sun;Password=pio*123";

        public static int GridTypeView = 1;

        public static int GridTypeReport = 2;

        public static int DefaultPageSize = 10;


        public SqlConnection connection()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = ConnectionString;
            return connection;

        }
        public DataTable CallStoreProcedure(string procedurename, SqlParameter[] parameters)
        {
            DataTable DtLogin = new DataTable();
            SqlCommand Command = new SqlCommand();

            try
            {
                Command.Connection = connection();
                Command.Connection.Open();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = procedurename;
                SqlDataAdapter Sdb = new SqlDataAdapter(Command);
                if (parameters != null && parameters.Length > 0)
                {
                    Sdb.SelectCommand.Parameters.AddRange(parameters);
                }
                Sdb.Fill(DtLogin);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Command.Connection.Close();

            }
            return DtLogin;
        }


        public DataSet GetDataSet(string procedurename, SqlParameter[] parameters)
        {
            DataSet DtLogin = new DataSet();
            SqlCommand Command = new SqlCommand();

            try
            {
                Command.Connection = connection();
                Command.Connection.Open();
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandText = procedurename;
                SqlDataAdapter Sdb = new SqlDataAdapter(Command);
                if (parameters != null && parameters.Length > 0)
                {
                    Sdb.SelectCommand.Parameters.AddRange(parameters);
                }
                Sdb.Fill(DtLogin);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Command.Connection.Close();

            }
            return DtLogin;
        }
        public string GetLatestVoucherNumber(string tableName, int id, int companyId, int yearId = 0)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@tablename", tableName);
                sqlParameters[1] = new SqlParameter("@DepVou", id);
                SqlParameter parameter2 = new SqlParameter();
                parameter2.ParameterName = "@VOUNO";
                parameter2.SqlDbType = SqlDbType.Int;
                parameter2.Value = "0";
                parameter2.Direction = ParameterDirection.Output;
                sqlParameters[2] = parameter2;
                sqlParameters[3] = new SqlParameter("@CmpVou", companyId);
                sqlParameters[4] = new SqlParameter("@YearVou", yearId);
                DataTable dtDetails = CallStoreProcedure("GetLatestVoucherNumber", sqlParameters);
                if (dtDetails != null && dtDetails.Rows.Count > 0)
                {
                    return dtDetails.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return string.Empty;
        }

        public string GetNumericDate(string date)
        {
            return date.Replace("-", "");
        }

        public static int ParseInt32(object value)
        {
            return value != null && value != "" ? Convert.ToInt32(value.ToString()) : 0;
        }

        public static string DynamicDecimalPoints(string value, int points)
        {
            string returnValue = string.Empty;
            try
            {
                string[] splitted = value.ToString().Split('.');
                if (splitted != null && splitted.Length > 0)
                {
                    if (splitted.Length == 1)
                    {
                        returnValue = splitted[0];
                    }
                    else
                    {
                        returnValue = splitted[0];
                        if (points > 0)
                            returnValue += ".";
                        for (int x = 0; x < points; x++)
                        {
                            if (x >= splitted[1].Length)
                            {
                                returnValue += "0";
                            }
                            else
                            {
                                returnValue += splitted[1][x];
                            }
                        }
                    }
                }
                else
                {
                    returnValue = "0";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return returnValue;
        }

        public static string GetFormatDateTime(DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy");
        }

        public static List<SelectListItem> GetClientList(int CompanyId, int isadministrator = 0)
        {
            DbConnection objdb = new DbConnection();
            List<SelectListItem> list = new List<SelectListItem>();
            SqlParameter[] sqlParameters = new SqlParameter[1];
            if (isadministrator == 1)
            {
                CompanyId = 0;
            }
            sqlParameters[0] = new SqlParameter("@CmpVou", CompanyId);
            DataTable dtClient = objdb.CallStoreProcedure("GetClientList", sqlParameters);
            if (dtClient != null && dtClient.Rows.Count > 0)
            {
                foreach (DataRow item in dtClient.Rows)
                {
                    list.Add(new SelectListItem
                    {
                        Text = item["CliName"].ToString(),
                        Value = item["CliVou"].ToString(),
                    });
                }
            }
            return list;
        }

        public static List<SelectListItem> GetCompanyList(int clientId = 0, int isAdministrator = 0)
        {
            DbConnection objdb = new DbConnection();
            List<SelectListItem> list = new List<SelectListItem>();
            int length = 1;
            if (clientId > 0)
            {
                length = 3;
            }
            SqlParameter[] parameters = new SqlParameter[length];
            if (clientId > 0)
            {
                parameters[0] = new SqlParameter("@FLg", 100);
                parameters[1] = new SqlParameter("@CLientId", clientId);
                parameters[2] = new SqlParameter("@IsAdministrator", isAdministrator);
            }
            else
            {

                parameters[0] = new SqlParameter("@FLg", 2);
            }
            DataTable dtClient = objdb.CallStoreProcedure("GetCompanyDetails", parameters);
            if (dtClient != null && dtClient.Rows.Count > 0)
            {
                foreach (DataRow item in dtClient.Rows)
                {
                    list.Add(new SelectListItem
                    {
                        Text = item["CmpCode"].ToString() + " - " + item["CmpName"].ToString(),
                        Value = item["CmpVou"].ToString(),
                    });
                }
            }
            return list;
        }

        public static List<SelectListItem> GetYearList(int companyId, int yearId = 0)
        {
            DbConnection objdb = new DbConnection();
            List<SelectListItem> list = new List<SelectListItem>();

            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@FLg", 100);
            parameters[1] = new SqlParameter("@CmpVou", companyId);
            DataTable dtCompany = objdb.CallStoreProcedure("YearMasterInsert", parameters);
            if (dtCompany != null && dtCompany.Rows.Count > 0)
            {
                foreach (DataRow item in dtCompany.Rows)
                {
                    bool selected = false;
                    if (yearId > 0)
                    {
                        if (item["YearVou"].ToString().Equals(yearId.ToString()))
                        {
                            selected = true;
                        }
                    }
                    else
                    {
                        if (item["IsDefault"].ToString().Equals("1"))
                        {
                            selected = true;
                        }
                    }
                    list.Add(new SelectListItem
                    {
                        Text = Convert.ToDateTime(item["StartDt"]).ToString("yyyy") + " - " + Convert.ToDateTime(item["EndDt"]).ToString("yyyy"),
                        Value = item["YearVou"].ToString(),
                        Selected = selected
                    });
                }
            }
            return list;
        }

        public static List<CompanyLIstModel> GetCompanyYearList(int clientId, int isAdministrator)
        {
            DbConnection objdb = new DbConnection();
            List<CompanyLIstModel> list = new List<CompanyLIstModel>();
            int length = 3;
            SqlParameter[] parameters = new SqlParameter[length];
            parameters[0] = new SqlParameter("@FLg", 100);
            parameters[1] = new SqlParameter("@CLientId", clientId);
            parameters[2] = new SqlParameter("@IsAdministrator", isAdministrator);
            DataTable dtClient = objdb.CallStoreProcedure("GetCompanyDetails", parameters);
            if (dtClient != null && dtClient.Rows.Count > 0)
            {
                foreach (DataRow item in dtClient.Rows)
                {
                    list.Add(new CompanyLIstModel
                    {
                        YearId = item["YearVou"].ToString(),
                        ClientNm = item["CliName"].ToString(),
                        COde = item["CmpCode"].ToString(),
                        Name = item["CmpName"].ToString(),
                        Id = item["CmpVou"].ToString(),
                        StartDate = Convert.ToDateTime(item["StartDt"]).ToString("dd-MM-yyyy"),
                        EndDate = Convert.ToDateTime(item["EndDt"]).ToString("dd-MM-yyyy")
                    });
                }
            }
            return list;
        }

        public static int GetClientIdByCompanyId(int companyId)
        {
            DbConnection objdb = new DbConnection();
            List<SelectListItem> list = new List<SelectListItem>();
            int length = 2;
            SqlParameter[] parameters = new SqlParameter[length];
            parameters[0] = new SqlParameter("@FLg", 2);
            parameters[1] = new SqlParameter("@CmpVou", companyId);
            DataTable dtClient = objdb.CallStoreProcedure("GetCompanyDetails", parameters);
            if (dtClient != null && dtClient.Rows.Count > 0)
            {
                return Convert.ToInt32(dtClient.Rows[0]["ClientId"].ToString());
            }
            return 0;
        }

        public static string ConvertToTwoDecimalValue(string value)
        {
            return !string.IsNullOrWhiteSpace(value) ? String.Format("{0:0.000}", Convert.ToDecimal(value)) : string.Empty;
        }


        public static CompanyMasterModel GetCompanyDetailsById(int id)
        {
            CompanyMasterModel companyMasterModel = new CompanyMasterModel();
            DbConnection objdb = new DbConnection();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@CmpVou", id);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                DataTable DtCompany = objdb.CallStoreProcedure("GetCompanyDetails", sqlParameters);
                if (DtCompany != null && DtCompany.Rows.Count > 0)
                {
                    companyMasterModel.CmpCode = DtCompany.Rows[0]["CmpCode"].ToString();
                    companyMasterModel.CmpName = DtCompany.Rows[0]["CmpName"].ToString();
                    companyMasterModel.ClientId = Convert.ToInt32(DtCompany.Rows[0]["CLientId"].ToString());
                    companyMasterModel.StartDate = !string.IsNullOrWhiteSpace(DtCompany.Rows[0]["CmpStDt"].ToString()) ? Convert.ToDateTime(DtCompany.Rows[0]["CmpStDt"].ToString()).ToString("yyyy-MM-dd") : "";
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return companyMasterModel;
        }

        public static List<YearMasterModel> GetYearListByCompanyId(int companyId)
        {
            DbConnection objdb = new DbConnection();
            List<YearMasterModel> list = new List<YearMasterModel>();

            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@FLg", 100);
            parameters[1] = new SqlParameter("@CmpVou", companyId);
            DataTable dtCompany = objdb.CallStoreProcedure("YearMasterInsert", parameters);
            if (dtCompany != null && dtCompany.Rows.Count > 0)
            {
                foreach (DataRow item in dtCompany.Rows)
                {
                    list.Add(new YearMasterModel
                    {
                        StartDate = Convert.ToDateTime(item["StartDt"]).ToString("yyyy-MM-dd").ToString(),
                        EndDate = Convert.ToDateTime(item["EndDt"]).ToString("yyyy-MM-dd").ToString(),
                        YearVou = Convert.ToInt32(item["YearVou"].ToString()),
                    });
                }
            }
            return list;
        }


        public static int SqlBulkInsertToTable(DataTable dtList, string tableName)
        {
            int IsInserted = 0;
            try
            {
                SqlConnection sqlConnection = new SqlConnection(ConnectionString);

                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                SqlBulkCopy objbulk = new SqlBulkCopy(ConnectionString, SqlBulkCopyOptions.FireTriggers);
                //foreach (DataColumn item in dtList.Columns)
                //{
                //    objbulk.ColumnMappings.Add(item.ColumnName, item.ColumnName);
                //}
                sqlConnection.Open();
                objbulk.DestinationTableName = tableName.Trim();
                objbulk.WriteToServer(dtList);
                sqlConnection.Close();
                IsInserted = 1;
            }
            catch (Exception ex)
            {
                IsInserted = 0;
                throw ex;
            }
            return IsInserted;
        }

        public static DepartmentMasterModel GetDepartmentMasterByCompanyId(int id)
        {
            DepartmentMasterModel departmentMasterModel = new DepartmentMasterModel();
            
            DbConnection objdb = new DbConnection();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@CmpVou", id);
                DataTable DtDepartment = objdb.CallStoreProcedure("GetDepartmentMasterByCompany", sqlParameters);
                if (DtDepartment != null && DtDepartment.Rows.Count > 0)
                {
                    departmentMasterModel.DepCode = DtDepartment.Rows[0]["DepCode"].ToString();
                    departmentMasterModel.DepName = DtDepartment.Rows[0]["DepName"].ToString();
                    departmentMasterModel.DepAdd = DtDepartment.Rows[0]["DepAdd"].ToString();
                    departmentMasterModel.DepCity = DtDepartment.Rows[0]["DepCity"].ToString();
                    departmentMasterModel.DepStateVou = DbConnection.ParseInt32(DtDepartment.Rows[0]["DepStateVou"].ToString());
                    departmentMasterModel.DepPAN = DtDepartment.Rows[0]["DepPAN"].ToString();
                    departmentMasterModel.DepGST = DtDepartment.Rows[0]["DepGST"].ToString();
                    departmentMasterModel.DepPhone2 = DtDepartment.Rows[0]["DepPhone2"].ToString();
                    departmentMasterModel.DepMob = DtDepartment.Rows[0]["DepMobile"].ToString();
                    departmentMasterModel.DepEmail = DtDepartment.Rows[0]["DepEmail"].ToString();
                    departmentMasterModel.DepSTax = DtDepartment.Rows[0]["DepSerTaxNo"].ToString();
                    departmentMasterModel.DepJurd = DtDepartment.Rows[0]["DepJurisd"].ToString();
                    departmentMasterModel.DepBnkNm = DtDepartment.Rows[0]["DepBankNm"].ToString();
                    departmentMasterModel.DepAcNo = DtDepartment.Rows[0]["DepBnkAcNo"].ToString();
                    departmentMasterModel.DepIFSC = DtDepartment.Rows[0]["DepBnkIFSC"].ToString();
                    departmentMasterModel.DepBnkBrnch = DtDepartment.Rows[0]["DepBnkBrnch"].ToString();
                    departmentMasterModel.DepWhtMob = DtDepartment.Rows[0]["DepWhtMob"].ToString();
                    departmentMasterModel.DepBusLine = DtDepartment.Rows[0]["DepBusLine"].ToString();
                    departmentMasterModel.ProfilePicture = DtDepartment.Rows[0]["DepLogo"].ToString();
                    
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return departmentMasterModel;
        }

    }
}
