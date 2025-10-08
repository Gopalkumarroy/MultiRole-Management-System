
using Sewa360.Models;
using System.Data;
using System.Data.SqlClient;

namespace Sewa360.DBContext
{
    public class ServiceProviderDb
    {
        private readonly IConfiguration _configuration;
        private readonly String _connectionString;

        public ServiceProviderDb(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "Server=DESKTOP-9H4DCH8\\SQLEXPRESS;Database=VrddhiDb;Integrated Security=True;TrustServerCertificate=True;";
        }


        public bool InsertServiceProvider(SericeProvider model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ServiceProviderMstOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@ServiceProvider_Id", model.ServiceProvider_Id);
                cmd.Parameters.AddWithValue("@ServiceProv_Name", model.ServiceProv_Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ServiceProv_Email", model.ServiceProv_Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Service_Phone", model.Service_Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Manager_Id", model.Manager_Id);
                cmd.Parameters.AddWithValue("@ServiceProv_Password", model.ServiceProv_Password);
                cmd.Parameters.AddWithValue("@Manager_Code", model.Manager_Code ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@First_Name", model.First_Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Last_Name", model.Last_Name ?? (object)DBNull.Value);

                con.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public SericeProvider? ValidateServiceProviderLogin(string email, string password)
        {
            SericeProvider? serviceprovider = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ServiceProviderLogin", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceProv_Email", email);
                cmd.Parameters.AddWithValue("@ServiceProv_Password", password);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        serviceprovider = new SericeProvider
                        {
                            ServiceProvider_Id = Convert.ToInt32(reader["ServiceProvider_Id"]),
                            ServiceProv_Email = reader["ServiceProv_Email"].ToString(),
                            First_Name = reader["First_Name"].ToString()
                        };
                    }
                }
            }

            return serviceprovider;
        }

        // Get All or One Service Provider
        public List<SericeProvider> GetServiceProviderList(int? id)
        {
            var serviceProviderList = new List<SericeProvider>();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_ServiceProviderMstOperations", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    cmd.Parameters.AddWithValue("@ServiceProvider_Id", id.HasValue ? (object)id.Value : DBNull.Value);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        serviceProviderList.Add(new SericeProvider
                        {
                            ServiceProvider_Id = Convert.ToInt32(rdr["ServiceProvider_Id"]),
                            Manager_Code = rdr["Manager_Code"].ToString(),
                            First_Name = rdr["First_Name"].ToString(),
                            Last_Name = rdr["Last_Name"].ToString(),
                            Company_Name = rdr["Company_Name"].ToString(),
                            ServiceProv_Email = rdr["ServiceProv_Email"].ToString(),
                            ServiceProv_Password = rdr["ServiceProv_Password"].ToString(),
                            Service_Phone = rdr["Service_Phone"].ToString(),
                            IsActive = rdr["IsActive"] != DBNull.Value && Convert.ToBoolean(rdr["IsActive"]),
                            CreatedOn = rdr["CreatedOn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rdr["CreatedOn"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting service provider list: " + ex.Message, ex);
            }

            return serviceProviderList;
        }

        public List<SericeProvider> GetAllServiceProvider()
        {
            return GetServiceProviderList(null); // Reusing existing method by passing null to get all companies
        }

        //  Star Save KYC
        public bool SaveServiceProviderKYC(ServiceProviderKYC model)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_ServiceProviderKYCOperations", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "INSERT");
            cmd.Parameters.AddWithValue("@ServiceProvider_Id", model.ServiceProvider_Id);
            cmd.Parameters.AddWithValue("@AadhaarDocumentPath", model.AadhaarDocumentPath ?? "");
            cmd.Parameters.AddWithValue("@PanCardDocumentPath", model.PanCardDocumentPath ?? "");
            cmd.Parameters.AddWithValue("@AadhaarNumber", model.AadhaarNumber ?? "");
            cmd.Parameters.AddWithValue("@PanNumber", model.PanNumber ?? "");
            cmd.Parameters.AddWithValue("@AccountHolderName", model.AccountHolderName ?? "");
            cmd.Parameters.AddWithValue("@BankName", model.BankName ?? "");
            cmd.Parameters.AddWithValue("@AccountNumber", model.AccountNumber ?? "");
            cmd.Parameters.AddWithValue("@IFSC", model.IFSC ?? "");
            cmd.Parameters.AddWithValue("@CreatedOn", model.CreatedOn ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy ?? "ServiceProvider");
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive ?? "1");
            cmd.Parameters.AddWithValue("@IsDeleted", model.IsDeleted ?? "0");
            cmd.Parameters.AddWithValue("@KycStatus", model.KycStatus ?? "Pending");

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // Update KYC
        public bool UpdateServiceProviderKYC(ServiceProviderKYC model)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_ServiceProviderKYCOperations", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "UPDATE");
            cmd.Parameters.AddWithValue("@KYC_Id", model.KYC_Id);
            cmd.Parameters.AddWithValue("@AadhaarDocumentPath", model.AadhaarDocumentPath ?? "");
            cmd.Parameters.AddWithValue("@PanCardDocumentPath", model.PanCardDocumentPath ?? "");
            cmd.Parameters.AddWithValue("@AadhaarNumber", model.AadhaarNumber ?? "");
            cmd.Parameters.AddWithValue("@PanNumber", model.PanNumber ?? "");
            cmd.Parameters.AddWithValue("@AccountHolderName", model.AccountHolderName ?? "");
            cmd.Parameters.AddWithValue("@BankName", model.BankName ?? "");
            cmd.Parameters.AddWithValue("@AccountNumber", model.AccountNumber ?? "");
            cmd.Parameters.AddWithValue("@IFSC", model.IFSC ?? "");
            cmd.Parameters.AddWithValue("@ModifiedOn", model.ModifiedOn ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@ModifiedBy", model.ModifiedBy ?? "ServiceProvider");
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive ?? "1");
            cmd.Parameters.AddWithValue("@IsDeleted", model.IsDeleted ?? "0");
            cmd.Parameters.AddWithValue("@KycStatus", model.KycStatus ?? "Pending");

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // Get KYC by ServiceProvider_Id
        public ServiceProviderKYC GetServiceProviderKYCById(int serviceProviderId)
        {
            ServiceProviderKYC model = null;
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_ServiceProviderKYCOperations", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "GET_BY_ID");
            cmd.Parameters.AddWithValue("@ServiceProvider_Id", serviceProviderId);

            conn.Open();
            using SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                model = new ServiceProviderKYC
                {
                    KYC_Id = Convert.ToInt32(rdr["KYC_Id"]),
                    ServiceProvider_Id = Convert.ToInt32(rdr["ServiceProvider_Id"]),
                    AadhaarDocumentPath = rdr["AadhaarDocumentPath"].ToString(),
                    PanCardDocumentPath = rdr["PanCardDocumentPath"].ToString(),
                    AadhaarNumber = rdr["AadhaarNumber"].ToString(),
                    PanNumber = rdr["PanNumber"].ToString(),
                    AccountHolderName = rdr["AccountHolderName"].ToString(),
                    BankName = rdr["BankName"].ToString(),
                    AccountNumber = rdr["AccountNumber"].ToString(),
                    IFSC = rdr["IFSC"].ToString(),
                    KycStatus = rdr["KycStatus"].ToString()
                };
            }

            return model;
        }

        // Update KYC Status (Approve/Reject)
        public bool UpdateKycStatus(int kycId, string status, string modifiedBy)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_ServiceProviderKYCOperations", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "UPDATE_STATUS");
            cmd.Parameters.AddWithValue("@KYC_Id", kycId);
            cmd.Parameters.AddWithValue("@KycStatus", status);
            cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // Get All Submitted KYCs
        public List<ServiceProviderKYC> GetAllServiceProviderKYCs()
        {
            List<ServiceProviderKYC> list = new List<ServiceProviderKYC>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("sp_ServiceProviderKYCOperations", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GET_ALL");

            conn.Open();
            using SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new ServiceProviderKYC
                {
                    KYC_Id = Convert.ToInt32(rdr["KYC_Id"]),
                    ServiceProvider_Id = Convert.ToInt32(rdr["ServiceProvider_Id"]),
                    AadhaarDocumentPath = rdr["AadhaarDocumentPath"].ToString(),
                    PanCardDocumentPath = rdr["PanCardDocumentPath"].ToString(),
                    AadhaarNumber = rdr["AadhaarNumber"].ToString(),
                    PanNumber = rdr["PanNumber"].ToString(),
                    AccountHolderName = rdr["AccountHolderName"].ToString(),
                    BankName = rdr["BankName"].ToString(),
                    AccountNumber = rdr["AccountNumber"].ToString(),
                    IFSC = rdr["IFSC"].ToString(),
                    KycStatus = rdr["KycStatus"].ToString()
                });
            }

            return list;
        }


        // 🔹 Get Service Provider By ID
        public SericeProvider GetServiceProviderById(int id)
        {
            SericeProvider sericeProvider = null;
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ServiceProviderMstOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETBYID");
                cmd.Parameters.AddWithValue("@ServiceProvider_Id", id); // adjust param name as per your SP

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        sericeProvider = new SericeProvider
                        {
                            ServiceProvider_Id = Convert.ToInt32(reader["ServiceProvider_Id"]),
                            First_Name = reader["First_Name"].ToString(),
                            Last_Name = reader["Last_Name"].ToString(),
                            Service_Phone = reader["Service_Phone"].ToString(),
                            ServiceProv_Email = reader["ServiceProv_Email"].ToString(),
                            //CreatedOn = reader["CreatedOn"] as DateTime?,
                            Manager_Code = reader["Manager_Code"].ToString(),
                        };
                    }
                }
            }
            return sericeProvider;
        }


        public int GetServiceProviderCountByManager(int managerId)
        {
            int count = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ServiceProviderMstOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_COUNT_BY_MANAGER");
                cmd.Parameters.AddWithValue("@Manager_Id", managerId);

                con.Open();
                count = (int)cmd.ExecuteScalar();
            }
            return count;
        }



    }
}
