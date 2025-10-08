using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Sewa360.Models;
using System.Data;

namespace Sewa360.DBContext
{
    public class BranchDb
    {
        private readonly IConfiguration _configuration;
        public BranchDb(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        String _connectionString = "Server=DESKTOP-9H4DCH8\\SQLEXPRESS;Database=VrddhiDb;Integrated Security=True;TrustServerCertificate=True;";


        // Insert Branch
        public void InsertBranch(Branch model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_CompanyBranch", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@Branch_Name", model.Branch_Name ?? (object)DBNull.Value);
                   // cmd.Parameters.AddWithValue("@Branch_Description", model.Branch_Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", model.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Branch_Phone", model.Branch_Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Branch_Email", model.Branch_Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Salary", model.Salary ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Department", model.Department ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Branch_Code", model.Branch_Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Lattitude", model.Lattitude ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Longitude", model.Longitude ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                    cmd.Parameters.AddWithValue("@IsDelete", model.IsDelete);
                    cmd.Parameters.AddWithValue("@CreatedOn", model.CreatedOn ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModifiedBy", model.ModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModifiedOn", model.ModifiedOn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Company_Id", model.Company_Id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting branch: " + ex.Message, ex);
            }
        }

        public List<Branch> GetBranchList(int? id)
        {
            var branchList = new List<Branch>();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_CompanyBranch", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    cmd.Parameters.AddWithValue("@Branch_Id", id.HasValue ? (object)id.Value : DBNull.Value);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        branchList.Add(new Branch
                        {
                            Branch_Id = Convert.ToInt32(rdr["Branch_Id"]),
                            Branch_Name = rdr["Branch_Name"]?.ToString(),
                            //Branch_Description = rdr["Branch_Description"]?.ToString(),
                            Address = rdr["Address"]?.ToString(),
                            Branch_Phone = rdr["Branch_Phone"]?.ToString(),
                            Branch_Email = rdr["Branch_Email"]?.ToString(),
                            //CreatedOn = rdr["CreatedOn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["CreatedOn"]),
                            Salary = rdr["Salary"]?.ToString(),
                            Remarks = rdr["Remarks"]?.ToString(),
                            Department = rdr["Department"]?.ToString(),
                            Branch_Code = rdr["Branch_Code"]?.ToString(),
                            Lattitude = rdr["Lattitude"]?.ToString(),
                            Longitude = rdr["Longitude"]?.ToString(),
                            IsActive = rdr["IsActive"] != DBNull.Value && Convert.ToBoolean(rdr["IsActive"]),
                            IsDelete = rdr["IsDelete"] != DBNull.Value && Convert.ToBoolean(rdr["IsDelete"]),
                            CreatedOn = rdr["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(rdr["CreatedOn"]) : (DateTime?)null,
                            CreatedBy = rdr["CreatedBy"]?.ToString(),
                            ModifiedBy = rdr["ModifiedBy"]?.ToString(),
                            ModifiedOn = rdr["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(rdr["ModifiedOn"]) : (DateTime?)null,
                            Company_Id = rdr["Company_Id"] != DBNull.Value ? Convert.ToInt32(rdr["Company_Id"]) : 0
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting branch list: " + ex.Message, ex);
            }

            return branchList;
        }

        public void DeleteBranch(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_CompanyBranch", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Action", "DELETE");
                    cmd.Parameters.AddWithValue("@Branch_Id", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting branch: " + ex.Message, ex);
            }
        }

        // Update Branch
        
        public bool UpdateBranch(Branch model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CompanyBranch", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Action", "UPDATE");
                        cmd.Parameters.AddWithValue("@Branch_Id", model.Branch_Id);
                        cmd.Parameters.AddWithValue("@Company_Id", model.Company_Id);
                        cmd.Parameters.AddWithValue("@Branch_Name", model.Branch_Name ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch_Code", model.Branch_Code ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch_Phone", model.Branch_Phone ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch_Email", model.Branch_Email ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", model.Address ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Department", model.Department ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Salary", model.Salary ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lattitude", model.Lattitude ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Longitude", model.Longitude ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Optional: log the exception or handle it as needed
                Console.WriteLine("SQL Error: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                // Optional: log or handle other errors
                Console.WriteLine("General Error: " + ex.Message);
                return false;
            }
        }


        public List<Branch> GetAllBranches()
        {
            return GetBranchList(null); // Reusing existing method by passing null to get all companies
        }

    }
}
