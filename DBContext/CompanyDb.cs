using Microsoft.Data.SqlClient;
using Sewa360.Models;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Sewa360.DBContext
{
    public class CompanyDb
    {
        private readonly IConfiguration _configuration;
        public CompanyDb(IConfiguration configuration) 
        {
            configuration = _configuration;

        }
        String _connectionString = "Server=DESKTOP-9H4DCH8\\SQLEXPRESS;Database=VrddhiDb;Integrated Security=True;TrustServerCertificate=True;";


        //  Insert Company
        public void InsertCompany(Company model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_ManageCompany", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@Company_Code", model.Company_Code);
                    cmd.Parameters.AddWithValue("@Company_Name", model.Company_Name);
                    cmd.Parameters.AddWithValue("@Company_Address", model.Company_Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_City", model.Company_City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Country", model.Company_Country ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_State", model.Company_State ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_ZipCode", model.Company_ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Email", model.Company_Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Phone", model.Company_Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Website", model.Company_Website ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_RegistrationDate", model.Company_RegistrationDate ?? (object)DBNull.Value);

                    //cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

                    cmd.Parameters.AddWithValue("@IsActive", model.IsActive ? 1 : 0);

                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Company_IndustryType", model.Company_IndustryType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_TaxId", model.Company_TaxId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Description", model.Company_Description ?? (object)DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting company: " + ex.Message, ex);
            }
        }


        //  Get All or One Company
        public List<Company> GetCompanyList(int? id)
        {
            var companyList = new List<Company>();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_ManageCompany", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    cmd.Parameters.AddWithValue("@Company_Id", id.HasValue ? (object)id.Value : DBNull.Value);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        companyList.Add(new Company
                        {
                            Company_Id = Convert.ToInt32(rdr["Company_Id"]),
                            Company_Code = rdr["Company_Code"].ToString(),
                            Company_Name = rdr["Company_Name"].ToString(),
                            Company_Email = rdr["Company_Email"].ToString(),
                            Company_City = rdr["Company_City"].ToString(),
                            Company_Phone = rdr["Company_Phone"].ToString(),
                            Company_Country = rdr["Company_Country"].ToString(),
                            Company_State = rdr["Company_State"].ToString(),
                            IsActive = rdr["IsActive"] != DBNull.Value && Convert.ToBoolean(rdr["IsActive"]),
                            Company_RegistrationDate = rdr["Company_RegistrationDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["Company_RegistrationDate"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting company list: " + ex.Message, ex);
            }

            return companyList;
        }

        //  Get Company by Id
        public Company EditCompanyById(int id)
        {
            Company model = null;

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_ManageCompany", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    cmd.Parameters.AddWithValue("@Company_Id", id);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        model = new Company
                        {
                            Company_Id = Convert.ToInt32(rdr["Company_Id"]),
                            Company_Code = rdr["Company_Code"].ToString(),
                            Company_Name = rdr["Company_Name"].ToString(),
                            Company_Address = rdr["Company_Address"].ToString(),
                            Company_City = rdr["Company_City"].ToString(),
                            Company_Email = rdr["Company_Email"].ToString(),
                            Company_Phone = rdr["Company_Phone"].ToString(),

                            // IsActive = Convert.ToInt16(rdr["IsActive"]) == 1,

                            IsActive = rdr["IsActive"] != DBNull.Value && Convert.ToBoolean(rdr["IsActive"]),
                            

                            Company_Website = rdr["Company_Website"].ToString(),
                            Company_Country = rdr["Company_Country"].ToString(),
                            Company_Description = rdr["Company_Description"].ToString(),
                            Company_State = rdr["Company_State"].ToString(),
                            Company_IndustryType = rdr["Company_IndustryType"].ToString(),
                            Company_TaxId = rdr["Company_TaxId"].ToString(),
                            Company_ZipCode = rdr["Company_ZipCode"].ToString(),
                            Company_RegistrationDate = rdr["Company_RegistrationDate"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(rdr["Company_RegistrationDate"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting company by ID: " + ex.Message, ex);
            }

            return model;
        }

        //  Update Company
        public void UpdateCompany(Company model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_ManageCompany", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Action", "UPDATE");
                    cmd.Parameters.AddWithValue("@Company_Id", model.Company_Id);
                    cmd.Parameters.AddWithValue("@Company_Code", model.Company_Code);
                    cmd.Parameters.AddWithValue("@Company_Name", model.Company_Name);
                    cmd.Parameters.AddWithValue("@Company_Address", model.Company_Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_City", model.Company_City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Country", model.Company_Country ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_State", model.Company_State ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_ZipCode", model.Company_ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Email", model.Company_Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Phone", model.Company_Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Website", model.Company_Website ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_RegistrationDate", model.Company_RegistrationDate ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                    cmd.Parameters.AddWithValue("@ModifiedBy", "Admin");
                    cmd.Parameters.AddWithValue("@Company_IndustryType", model.Company_IndustryType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_TaxId", model.Company_TaxId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Description", model.Company_Description ?? (object)DBNull.Value);




                    cmd.Parameters.AddWithValue("@IsActive", model.IsActive ? 1 : 0);


                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating company: " + ex.Message, ex);
            }
        }

        // Delete Company
        public void DeleteCompany(int id, string modifiedBy)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_ManageCompany", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Action", "DELETE");
                    cmd.Parameters.AddWithValue("@Company_Id", id);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting company: " + ex.Message, ex);
            }
        }

        public List<Company> GetAllCompanies()
        {
            return GetCompanyList(null); // Reusing existing method by passing null to get all companies
        }


    }
}
