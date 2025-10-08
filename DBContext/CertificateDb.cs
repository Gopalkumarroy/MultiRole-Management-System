using Sewa360.Areas.Admin.Models;
using System.Data;
using System.Data.SqlClient;

namespace Sewa360.DBContext
{
    public class CertificateDb
    {
        private readonly IConfiguration _configuration;
        public CertificateDb(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        String _connectionString = "Server=DESKTOP-9H4DCH8\\SQLEXPRESS;Database=VrddhiDb;Integrated Security=True;TrustServerCertificate=True;";

        public int InsertCertificate(int batchId, int studentId, int templateId, string certNo, string pdfPath)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Certificates_Operations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@Batch_Id", batchId);
                cmd.Parameters.AddWithValue("@Student_Id", studentId);
                cmd.Parameters.AddWithValue("@Template_Id", templateId);
                cmd.Parameters.AddWithValue("@CertificateNo", certNo);
                cmd.Parameters.AddWithValue("@PdfPath", pdfPath);

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()); // returns new Certificate_Id
            }
        }

        public List<CertificateModel> GetCertificatesByBatch(int batchId)
        {
            List<CertificateModel> list = new List<CertificateModel>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Certificates_Operations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_BY_BATCH");
                cmd.Parameters.AddWithValue("@Batch_Id", batchId);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new CertificateModel
                        {
                            Certificate_Id = Convert.ToInt32(dr["Certificate_Id"]),
                            CertificateNo = dr["CertificateNo"].ToString(),
                            PdfPath = dr["PdfPath"].ToString(),
                            GeneratedAt = Convert.ToDateTime(dr["GeneratedAt"]),
                            StudentName = dr["FullName"].ToString(),
                            RollNo = dr["RollNo"].ToString(),
                            Course = dr["Course"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public CertificateModel GetCertificateById(int certId)
        {
            CertificateModel cert = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Certificates_Operations", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_BY_ID");
                cmd.Parameters.AddWithValue("@Certificate_Id", certId);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        cert = new CertificateModel
                        {
                            Certificate_Id = Convert.ToInt32(dr["Certificate_Id"]),
                            CertificateNo = dr["CertificateNo"].ToString(),
                            PdfPath = dr["PdfPath"].ToString(),
                            GeneratedAt = Convert.ToDateTime(dr["GeneratedAt"])
                        };
                    }
                }
            }
            return cert;
        }
        public void AddStudent(Certificates student)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Certificates_Operation", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@FullName", student.FullName);
                cmd.Parameters.AddWithValue("@Email", student.Email);
                cmd.Parameters.AddWithValue("@RollNo", student.RollNo);
                cmd.Parameters.AddWithValue("@Course", student.Course ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CompanyName", student.CompanyName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@StartDate", student.StartDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", student.EndDate ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public List<Certificates> GetAllCertificates()
        {
            List<Certificates> list = new List<Certificates>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_Certificates_Operation", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_ALL");

                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new Certificates
                        {
                            Student_Id = Convert.ToInt32(rdr["Student_Id"]),
                            FullName = rdr["FullName"].ToString(),
                            //Email = rdr["Email"].ToString(),
                            //RollNo = rdr["RollNo"].ToString(),
                            Course = rdr["Course"].ToString(),
                            CompanyName = rdr["CompanyName"].ToString(),
                            StartDate = rdr["StartDate"] == DBNull.Value ? null : Convert.ToDateTime(rdr["StartDate"]),
                            EndDate = rdr["EndDate"] == DBNull.Value ? null : Convert.ToDateTime(rdr["EndDate"]),
                            CreatedAt = Convert.ToDateTime(rdr["CreatedAt"])
                        });
                    }
                }
            }
            return list;
        }

    }
}
