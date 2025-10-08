using Microsoft.Data.SqlClient;
using Sewa360.Areas.Admin.Models;
using Sewa360.Models;
using System.Data;

namespace Sewa360.DBContext
{
    public class ManagerDb
    {
        private readonly IConfiguration _configuration;

        public ManagerDb(IConfiguration configuration)
        {
            _configuration = configuration;
        }      
        String _connectionString = "Server=DESKTOP-9H4DCH8\\SQLEXPRESS;Database=VrddhiDb;Integrated Security=True;TrustServerCertificate=True;";

        public void CreateManager(Manager model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Manager_CRUD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Correct Action
                    cmd.Parameters.AddWithValue("@Action", "INSERT");

                    // Match parameters to stored procedure
                    cmd.Parameters.AddWithValue("@Manager_Name", model.Manager_Name ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Manager_Code", model.Manager_Code ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Manager_Phone", model.Manager_Phone ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Manager_Email", model.Manager_Email ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Manager_City", model.Manager_City ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Manager_Password", model.Manager_Password ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CreatedOn", model.CreatedOn ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                    //cmd.Parameters.AddWithValue("@Branch_Id", model.Branch_Id);

                    // Optional output parameter if needed
                    //cmd.Parameters.AddWithValue("@Msg", "Creating Manager");

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public List<Manager> GetAllManagers()
        {
            List<Manager> managers = new List<Manager>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Manager_CRUD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GETALL");

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Manager manager = new Manager
                            {
                                Manager_Id = Convert.ToInt32(reader["Manager_Id"]),
                                Manager_Name = reader["Manager_Name"]?.ToString(),
                                Manager_Password = reader["Manager_Password"]?.ToString(),
                                Manager_Description = reader["Manager_Description"]?.ToString(),
                                Manager_Phone = reader["Manager_Phone"]?.ToString(),
                                Manager_Email = reader["Manager_Email"]?.ToString(),
                                Manager_City = reader["Manager_City"]?.ToString(),
                                Manager_Region = reader["Manager_Region"]?.ToString(),
                                Manager_PostalCode = reader["Manager_PostalCode"]?.ToString(),
                                Manager_Country = reader["Manager_Country"]?.ToString(),
                                Manager_Address = reader["Manager_Address"]?.ToString(),
                                Manager_Department = reader["Manager_Department"]?.ToString(),
                                Manager_Code = reader["Manager_Code"]?.ToString(),
                                Manager_gender = reader["Manager_gender"]?.ToString(),
                                Manager_Type = reader["Manager_Type"]?.ToString(),
                                Manager_Department_Id = reader["Manager_Department_Id"]?.ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                IsDelete = Convert.ToBoolean(reader["IsDelete"]),
                                CreatedOn = reader["CreatedOn"] as DateTime?,
                                CreatedBy = reader["CreatedBy"]?.ToString(),
                                ModifiedOn = reader["ModifiedOn"] as DateTime?,
                                ModifiedBy = reader["ModifiedBy"]?.ToString()
                            };

                            managers.Add(manager);
                        }
                    }
                }
            }
            return managers;
        }

        //  Delete
        public bool DeleteManager(int id, string modifiedBy = "System")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("Manager_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "DELETE");
                    cmd.Parameters.AddWithValue("@Manager_Id", id);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0; // अगर एक या अधिक row delete हुई तो true return होगा
                }
            }
            catch (Exception ex)
            {
                // Log the error if needed
                Console.WriteLine(ex.Message);
                return false; // अगर कोई exception आता है तो false return करें
            }
        }


        public Manager GetManagerByEmail(string email)
        {
            Manager manager = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetManagerByEmail", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            manager = new Manager
                            {
                                Manager_Id = Convert.ToInt32(dr["Manager_Id"]),
                                Manager_Name = dr["Manager_Name"].ToString(),
                                Manager_Email = dr["Manager_Email"].ToString(),
                                Manager_Code = dr["Manager_Code"].ToString()
                            };
                        }
                    }
                }
            }
            return manager;
        }



        public Manager? ValidateManagerLogin(string email, string password)
        {
            Manager manager = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ManagerLogin", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Manager_Email", email);
                cmd.Parameters.AddWithValue("@Manager_Password", password);//

                //cmd.Parameters.AddWithValue("@Manager_Code", managerCode); // ✅ Added

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    manager = new Manager
                    {
                        Manager_Id = Convert.ToInt32(reader["Manager_Id"]),
                        Manager_Email = reader["Manager_Email"].ToString(),
                        Manager_Name = reader["Manager_Name"].ToString(), // ✅ ADD THIS
                        Manager_Code = reader["Manager_Code"].ToString(),
                     // Manager_Password = reader["Manager_Password"].ToString()
                    };
                }
            }

            return manager;
        }


        public Manager GetManagerByCode(string code)
        {
            Manager manager = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetManagerByCode", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Manager_Code", code);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        manager = new Manager
                        {
                            Manager_Id = Convert.ToInt32(reader["Manager_Id"]),
                            Manager_Name = reader["Manager_Name"].ToString(),
                           
                            Manager_Code = reader["Manager_Code"].ToString()
                        };
                    }
                }
            }

            return manager;
        }

        public List<Partner> GetPartnersByManagerId(int managerId)
        {
            List<Partner> partners = new List<Partner>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_PartnerMstOperations", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "SelectByManagerId");
                    cmd.Parameters.AddWithValue("@Manager_Id", managerId);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            partners.Add(new Partner
                            {
                                Partner_Id = Convert.ToInt32(reader["Partner_Id"]),
                                First_Name = reader["First_Name"].ToString(),
                                Last_Name = reader["Last_Name"].ToString(),
                                Partner_Email = reader["Partner_Email"].ToString(),
                                Partner_Phone = reader["Partner_Phone"].ToString(),
                                Partner_Code = reader["Partner_Code"].ToString(),
                                Manager_Id = Convert.ToInt32(reader["Manager_Id"])
                            });
                        }
                    }
                }
            }

            return partners;
        }

        public Manager GetManagerById(int id)
        {
            Manager manager = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetManagerById", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Manager_Id", id);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    manager = new Manager
                    {
                        Manager_Id = Convert.ToInt32(rdr["Manager_Id"]),
                        Manager_Name = rdr["Manager_Name"].ToString(),
                        Manager_Email = rdr["Manager_Email"].ToString(),
                        Manager_Phone = rdr["Manager_Phone"].ToString(),
                        Manager_City = rdr["Manager_City"].ToString(),
                        Manager_Code = rdr["Manager_Code"].ToString(),
                        // add other properties
                    };
                }
            }

            return manager;
        }

        public bool SaveManagerKYC(ManagerKYC model)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_ManagerKYCOperations", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "INSERT");
            AddKYCParameters(cmd, model);

            conn.Open();
            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }

        public bool UpdateManagerKYC(ManagerKYC model)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_ManagerKYCOperations", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "UPDATE");
            cmd.Parameters.AddWithValue("@KYC_Id", model.KYC_Id);
            AddKYCParameters(cmd, model);

            conn.Open();
            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }

        public ManagerKYC GetManagerKYCByManagerId(int managerId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_ManagerKYCOperations", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "GET_BY_MANAGER_ID");
            cmd.Parameters.AddWithValue("@Manager_Id", managerId);

            conn.Open();
            using SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new ManagerKYC
                {
                    KYC_Id = Convert.ToInt32(rdr["KYC_Id"]),
                    Manager_Id = Convert.ToInt32(rdr["Manager_Id"]),
                    AadhaarDocumentPath = rdr["AadhaarDocumentPath"]?.ToString(),
                    PanCardDocumentPath = rdr["PanCardDocumentPath"]?.ToString(),
                    AccountHolderName = rdr["AccountHolderName"]?.ToString(),
                    BankName = rdr["BankName"]?.ToString(),
                    AccountNumber = rdr["AccountNumber"]?.ToString(),
                    IFSC = rdr["IFSC"]?.ToString(),
                    AadhaarNumber = rdr["AadhaarNumber"]?.ToString(),
                    PanNumber = rdr["PanNumber"]?.ToString(),
                    KycStatus = rdr["KycStatus"]?.ToString(),
                    IsActive = rdr["IsActive"]?.ToString(),
                    IsDeleted = rdr["IsDeleted"]?.ToString(),
                    CreatedOn = rdr["CreatedOn"]?.ToString(),
                    CreatedBy = rdr["CreatedBy"]?.ToString(),
                    ModifiedOn = rdr["ModifiedOn"]?.ToString(),
                    ModifiedBy = rdr["ModifiedBy"]?.ToString()
                };
            }

            return null;
        }
        private void AddKYCParameters(SqlCommand cmd, ManagerKYC model)
        {
            cmd.Parameters.AddWithValue("@Manager_Id", model.Manager_Id);
            cmd.Parameters.AddWithValue("@AadhaarDocumentPath", (object?)model.AadhaarDocumentPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PanCardDocumentPath", (object?)model.PanCardDocumentPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AccountHolderName", model.AccountHolderName ?? "");
            cmd.Parameters.AddWithValue("@BankName", model.BankName ?? "");
            cmd.Parameters.AddWithValue("@AccountNumber", model.AccountNumber ?? "");
            cmd.Parameters.AddWithValue("@IFSC", model.IFSC ?? "");
            cmd.Parameters.AddWithValue("@AadhaarNumber", model.AadhaarNumber ?? "");
            cmd.Parameters.AddWithValue("@PanNumber", model.PanNumber ?? "");
            cmd.Parameters.AddWithValue("@KycStatus", model.KycStatus ?? "Pending");
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive ?? "1");
            cmd.Parameters.AddWithValue("@IsDeleted", model.IsDeleted ?? "0");
            cmd.Parameters.AddWithValue("@CreatedOn", (object?)model.CreatedOn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedBy", (object?)model.CreatedBy ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ModifiedOn", (object?)model.ModifiedOn ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ModifiedBy", (object?)model.ModifiedBy ?? DBNull.Value);
        }

        public bool UpdateManagerKycStatus(int kycId, string status, string modifiedBy)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("sp_ManagerKYCOperations", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "UPDATE_STATUS");
                cmd.Parameters.AddWithValue("@KYC_Id", kycId);
                cmd.Parameters.AddWithValue("@KycStatus", status);
                cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }


        public List<ManagerKYC> GetAllManagerKycRecords()
        {
            List<ManagerKYC> list = new List<ManagerKYC>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ManagerKYCOperations", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GET_KYC_SUBMITTED_MANAGERS");

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ManagerKYC
                            {
                                KYC_Id = reader["KYC_Id"] != DBNull.Value ? Convert.ToInt32(reader["KYC_Id"]) : 0,
                                Manager_Id = Convert.ToInt32(reader["Manager_Id"]),
                                //Manager_Name = reader["Manager_Name"]?.ToString(),
                               // Manager_Email = reader["Manager_Email"]?.ToString(),
                                //Manager_Phone = reader["Manager_Phone"]?.ToString(),
                                AadhaarNumber = reader["AadhaarNumber"]?.ToString(),
                                PanNumber = reader["PanNumber"]?.ToString(),
                                AccountHolderName = reader["AccountHolderName"]?.ToString(),
                                BankName = reader["BankName"]?.ToString(),
                                AccountNumber = reader["AccountNumber"]?.ToString(),
                                IFSC = reader["IFSC"]?.ToString(),
                                AadhaarDocumentPath = reader["AadhaarDocumentPath"]?.ToString(),
                                PanCardDocumentPath = reader["PanCardDocumentPath"]?.ToString(),
                                KycStatus = reader["KycStatus"]?.ToString(),
                            });
                        }
                    }
                }
            }

            return list;
        }

       


    }
}
