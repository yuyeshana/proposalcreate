using System;
using System.Data.SqlClient;
using Proposal.Models;

namespace Proposal.DAC
{
    public class LoginDAC
    {
        private readonly string _connectionString;

        public LoginDAC(string connectionString)
        {
            _connectionString = connectionString;
        }
        public User GetUserById(LoginModel pModel)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            string sql = "SELECT user_id,  password, registration_status FROM [user] WHERE user_id=@userId";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userId", pModel.UserId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = reader["user_id"].ToString(),
                    Password = reader["password"].ToString(),
                    Registration_status = reader.GetBoolean(reader.GetOrdinal("registration_status"))
                };
            }
            return null;
        }
    }
}
