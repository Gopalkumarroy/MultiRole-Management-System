using Microsoft.Data.SqlClient;
using Sewa360.Areas.Admin.Models;
using Sewa360.Models;
using System.Data;

namespace Sewa360.DBContext
{
    public class PartnerDb
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public PartnerDb(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "Server=DESKTOP-9H4DCH8\\SQLEXPRESS;Database=VrddhiDb;Integrated Security=True;TrustServerCertificate=True;";
        }

        // 🔹 Get All
        public List<Partner> GetAllPartners()
        {
            List<Partner> list = new List<Partner>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PartnerMstOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETALL");
               

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Partner
                        {
                            First_Name = reader["First_Name"].ToString(),
                            Last_Name = reader["Last_Name"].ToString(),
                            CreatedOn = reader["CreatedOn"] as DateTime?,
                            Manager_Code = reader["Manager_Code"].ToString(),
                            Partner_Id = Convert.ToInt32(reader["Partner_Id"]),
                            Partner_Name = reader["Partner_Name"].ToString(),
                            Partner_City = reader["Partner_City"].ToString(),
                            Partner_Code = reader["Partner_Code"].ToString(),
                           
                            Partner_Password = reader["Partner_Password"].ToString(),
                            Partner_IDproof = reader["Partner_IDproof"].ToString(),
                            Partner_Phone = reader["Partner_Phone"].ToString(),
                            Partner_Email = reader["Partner_Email"].ToString(),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        });
                    }
                }
            }
            return list;
        }

        // 🔹 Get By ID
        public Partner GetPartnerById(int id)
        {
            Partner partner = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PartnerMstOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETBYID");
                cmd.Parameters.AddWithValue("@Partner_Id", id);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        partner = new Partner
                        {
                            Partner_Id = Convert.ToInt32(reader["Partner_Id"]),
                            First_Name = reader["First_Name"].ToString(),
                            Last_Name = reader["Last_Name"].ToString(),
                            Partner_City = reader["Partner_City"].ToString(),
                            Partner_Code = reader["Partner_Code"].ToString(),
                            CreatedOn = reader["CreatedOn"] as DateTime?,
                            Manager_Code = reader["Manager_Code"].ToString(),
                            Partner_Password = reader["Partner_Password"].ToString(),
                            Partner_IDproof = reader["Partner_IDproof"].ToString(),
                            Partner_Phone = reader["Partner_Phone"].ToString(),
                            Partner_Email = reader["Partner_Email"].ToString(),
                            
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        };
                    }
                }
            }
            return partner;
        }

        public bool InsertPartner(Partner partner)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PartnerMstOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@Partner_Id", partner.Partner_Id);
                cmd.Parameters.AddWithValue("@Partner_Name", partner.Partner_Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Partner_Email", partner.Partner_Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Partner_Phone", partner.Partner_Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Manager_Id", partner.Manager_Id);
                cmd.Parameters.AddWithValue("@Partner_Password", partner.Partner_Password);
                cmd.Parameters.AddWithValue("@Manager_Code", partner.Manager_Code ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@First_Name", partner.First_Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Last_Name", partner.Last_Name ?? (object)DBNull.Value);

                //cmd.Parameters.AddWithValue("@Business", partner.Business ?? (object)DBNull.Value);

                con.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }





        //  Update
        public void UpdatePartner(Partner partner)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PartnerMstOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@Partner_Id", partner.Partner_Id);
                cmd.Parameters.AddWithValue("@Partner_Name", partner.Partner_Name);
                cmd.Parameters.AddWithValue("@Partner_City", partner.Partner_City);
                cmd.Parameters.AddWithValue("@Partner_Code", partner.Partner_Code);
                cmd.Parameters.AddWithValue("@Partner_IDproof", partner.Partner_IDproof);
                cmd.Parameters.AddWithValue("@Partner_Phone", partner.Partner_Phone ?? "");
                cmd.Parameters.AddWithValue("@Partner_Email", partner.Partner_Email ?? "");
                cmd.Parameters.AddWithValue("@ModifiedBy", partner.ModifiedBy ?? "System");
                cmd.Parameters.AddWithValue("@Partner_Password", partner.Partner_Password);

               // cmd.Parameters.AddWithValue("@Business", partner.Business ?? (object)DBNull.Value);


                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //  Delete
        public bool DeletePartner(int id, string modifiedBy = "System")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_PartnerMstOperations", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "DELETE");
                    cmd.Parameters.AddWithValue("@Partner_Id", id);
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

        //Reading the value of partner from database and binding it to the database by Gopal
        public List<Partner> GetPartnersByManagerCode(string managerCode)
        {
            List<Partner> partners = new List<Partner>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_PartnerMstOperations", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_MANAGER_CODE");
                    cmd.Parameters.AddWithValue("@Manager_Code", managerCode);

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
                                Partner_Name = reader["Partner_Name"].ToString(),
                                Manager_Code = reader["Manager_Code"].ToString(),
                                Partner_Email = reader["Partner_Email"].ToString(),
                                Partner_Phone = reader["Partner_Phone"].ToString(),

                                // Business = reader["Business"]?.ToString() ?? "",
                                // Partner_Password = reader["Partner_Password"].ToString(),

                                CreatedOn = reader["CreatedOn"] == DBNull.Value
    ? (DateTime?)null
    : Convert.ToDateTime(reader["CreatedOn"])
                                // Add other properties as needed
                            });
                        }
                    }
                }
            }

            return partners;
        }


        public bool AddPartner(Partner partner)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PartnerMstOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@Partner_Id", partner.Partner_Id);
                cmd.Parameters.AddWithValue("@First_Name", partner.First_Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Last_Name", partner.Last_Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Partner_Code", partner.Partner_Code ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Partner_Email", partner.Partner_Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Partner_Phone", partner.Partner_Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Partner_Password", partner.Partner_Password ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Manager_Code", partner.Manager_Code ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", partner.CreatedBy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ModifiedBy", partner.ModifiedBy ?? (object)DBNull.Value);

                //cmd.Parameters.AddWithValue("@Manager_Code", partner.Manager_Code ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@IsActive", partner.IsActive);
                cmd.Parameters.AddWithValue("@IsDelete", partner.IsDelete);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    // Optional: log the error here
                    return false;
                }
            }
        }


        // New method to get partners by Manager_Id
        public List<Partner> GetPartnersByManagerId(int managerId)
        {
            List<Partner> partners = new List<Partner>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetPartnersByManagerId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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
                                Partner_Email = reader["Email"].ToString(),
                                Partner_Phone = reader["Contact"].ToString(),
                                Partner_Password = reader["Password"].ToString(),
                                Manager_Id = Convert.ToInt32(reader["Manager_Id"]),
                                Business = reader["Business"].ToString(),
                                Manager_Code = reader["Manager_Code"].ToString()
                            });
                        }
                    }
                }
            }

            return partners;
        }

        public Partner? ValidatePartnerLogin(string email, string password)
        {
            Partner? partner = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PartnerLogin", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Partner_Email", email);
                cmd.Parameters.AddWithValue("@Partner_Password", password);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        partner = new Partner
                        {
                            Partner_Id = Convert.ToInt32(reader["Partner_Id"]),
                            Partner_Email = reader["Partner_Email"].ToString(),
                            First_Name = reader["First_Name"].ToString()
                        };
                    }
                }
            }

            return partner;
        }


        public bool SaveKYC(PartnerKYC model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_PartnerKYCOperations", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@Partner_Id", model.Partner_Id);
                    cmd.Parameters.AddWithValue("@AadhaarDocumentPath", (object?)model.AadhaarDocumentPath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PanCardDocumentPath", (object?)model.PanCardDocumentPath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountHolderName", model.AccountHolderName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankName", model.BankName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AadhaarNumber", model.AadhaarNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PanNumber", model.PanNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountNumber", model.AccountNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IFSC", model.IFSC ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", string.IsNullOrEmpty(model.IsActive) ? 1 : Convert.ToInt16(model.IsActive));
                    cmd.Parameters.AddWithValue("@IsDeleted", string.IsNullOrEmpty(model.IsDeleted) ? 0 : Convert.ToInt16(model.IsDeleted));
                    cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedOn", string.IsNullOrEmpty(model.CreatedOn)
                                                            ? DateTime.Now
                                                            : Convert.ToDateTime(model.CreatedOn));
                    cmd.Parameters.AddWithValue("@KycStatus", model.KycStatus ?? "Pending");

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving Partner KYC: " + ex.Message, ex);
            }
        }

        public PartnerKYC GetPartnerKYCByPartnerId(int partnerId)
        {
            PartnerKYC model = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PartnerKYCOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // ✅ Set the new action and pass partnerId as parameter
                cmd.Parameters.AddWithValue("@Action", "SELECTBYPARTNERID");
                cmd.Parameters.AddWithValue("@Partner_Id", partnerId);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    model = new PartnerKYC
                    {
                        KYC_Id = Convert.ToInt32(rdr["KYC_Id"]),
                        Partner_Id = Convert.ToInt32(rdr["Partner_Id"]),
                        AadhaarDocumentPath = rdr["AadhaarDocumentPath"]?.ToString(),
                        PanCardDocumentPath = rdr["PanCardDocumentPath"]?.ToString(),
                        AadhaarNumber = rdr["AadhaarNumber"]?.ToString(),
                        PanNumber = rdr["PanNumber"]?.ToString(),
                        AccountHolderName = rdr["AccountHolderName"]?.ToString(),
                        BankName = rdr["BankName"]?.ToString(),
                        AccountNumber = rdr["AccountNumber"]?.ToString(),
                        IFSC = rdr["IFSC"]?.ToString(),
                        KycStatus = rdr["KycStatus"]?.ToString(),
                        IsActive = rdr["IsActive"]?.ToString(),
                        IsDeleted = rdr["IsDeleted"]?.ToString(),
                        CreatedBy = rdr["CreatedBy"]?.ToString(),
                        CreatedOn = rdr["CreatedOn"]?.ToString()
                    };
                }
            }

            return model;
        }


        public bool UpdateKYC(PartnerKYC model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PartnerKYCOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@KYC_Id", model.KYC_Id);
                cmd.Parameters.AddWithValue("@AadhaarDocumentPath", model.AadhaarDocumentPath ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PanCardDocumentPath", model.PanCardDocumentPath ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AccountHolderName", model.AccountHolderName);
                cmd.Parameters.AddWithValue("@BankName", model.BankName);
                cmd.Parameters.AddWithValue("@AccountNumber", model.AccountNumber);
                cmd.Parameters.AddWithValue("@IFSC", model.IFSC);
                cmd.Parameters.AddWithValue("@ModifiedBy", model.ModifiedBy);
                cmd.Parameters.AddWithValue("@ModifiedOn", model.ModifiedOn);
                cmd.Parameters.AddWithValue("@AadhaarNumber", model.AadhaarNumber);
                cmd.Parameters.AddWithValue("@PanNumber", model.PanNumber);
                cmd.Parameters.AddWithValue("@IsActive", 1);
                cmd.Parameters.AddWithValue("@IsDeleted", 0);
                cmd.Parameters.AddWithValue("@KycStatus", model.KycStatus);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public bool UpdateKycStatus(int kycId, string status, string modifiedBy)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_PartnerKYCOperations", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Action", "UPDATESTATUS");
                    cmd.Parameters.AddWithValue("@KYC_Id", kycId);
                    cmd.Parameters.AddWithValue("@KycStatus", status);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                    cmd.Parameters.AddWithValue("@ModifiedOn", DateTime.Now);

                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating KYC status: " + ex.Message);
            }
        }


        public List<PartnerKYC> GetAllKycRecords()
        {
            var list = new List<PartnerKYC>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PartnerKYCOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "SELECT");

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new PartnerKYC
                    {
                        KYC_Id = Convert.ToInt32(rdr["KYC_Id"]),
                        Partner_Id = Convert.ToInt32(rdr["Partner_Id"]),
                        AadhaarNumber = rdr["AadhaarNumber"]?.ToString(),
                        PanNumber = rdr["PanNumber"]?.ToString(),
                        AccountHolderName = rdr["AccountHolderName"]?.ToString(),
                        BankName = rdr["BankName"]?.ToString(),
                        AccountNumber = rdr["AccountNumber"]?.ToString(),
                        IFSC = rdr["IFSC"]?.ToString(),
                        AadhaarDocumentPath = rdr["AadhaarDocumentPath"].ToString(),
                        PanCardDocumentPath = rdr["PanCardDocumentPath"].ToString(),
                        KycStatus = rdr["KycStatus"].ToString()
                    });
                }
            }
            return list;
        }
       


    }
}
