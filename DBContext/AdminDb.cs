using Microsoft.Data.SqlClient;
using Sewa360.Areas.Admin.Models;
using Sewa360.Models;
using System.Data;

namespace Sewa360.DBContext
{
    public class AdminDb
    {
        private readonly IConfiguration _configuration;
        public AdminDb(IConfiguration configuration)
        { 
            _configuration = configuration;
        }
        String _connectionString = "Server=DESKTOP-9H4DCH8\\SQLEXPRESS;Database=VrddhiDb;Integrated Security=True;TrustServerCertificate=True;";

        public AdminLogin AdminLogin(string userName, string password)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_AdminLogin", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Admin_Name", SqlDbType.NVarChar, 100).Value = userName ?? (object)DBNull.Value;
                cmd.Parameters.Add("@Admin_Password", SqlDbType.NVarChar, 100).Value = password ?? (object)DBNull.Value;

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new AdminLogin
                        {
                            Admin_Id = Convert.ToInt32(reader["Admin_Id"]),
                            Admin_Name = reader["Admin_Name"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public List<PartnerKYCEntry> GetPartnerKYCList(int managerId)
        {
            List<PartnerKYCEntry> list = new List<PartnerKYCEntry>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetPartnerKYCListByManager", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Manager_Id", managerId);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new PartnerKYCEntry
                    {
                        Partner_Id = Convert.ToInt32(rdr["Partner_Id"]),
                        PartnerName = rdr["PartnerName"].ToString(),
                        Partner_Phone = rdr["Partner_Phone"].ToString(),
                        KycStatus = rdr["KycStatus"]?.ToString() ?? "Pending"
                    });
                }
            }

            return list;
        }

        public int GetPartnerCountByManager(int managerId)
        {
            int count = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_PartnerMstOperations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_PARTNER_COUNT_BY_MANAGER");
                cmd.Parameters.AddWithValue("@Manager_Id", managerId);
                //cmd.Parameters.AddWithValue("@Business", DBNull.Value); // <-- Added this

                con.Open();
                count = (int)cmd.ExecuteScalar();
            }
            return count;
        }



        public (int Approved, int Pending, int Rejected, int NotDone) GetKYCCountsByManager(int managerId)
        {
            int approved = 0, pending = 0, rejected = 0, notDone = 0;

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetKYCCountsByManager", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Manager_Id", managerId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    approved = Convert.ToInt32(reader["KYCApproved"]);
                    pending = Convert.ToInt32(reader["KYCPending"]);
                    rejected = Convert.ToInt32(reader["KYCRejected"]);
                    notDone = Convert.ToInt32(reader["KYCNotDone"]);
                }
            }

            return (approved, pending, rejected, notDone);
        }



    }
}
