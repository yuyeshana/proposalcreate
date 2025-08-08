using Proposal.Models;
using System;
using System.Data.SqlClient;

namespace Proposal.DAL
{
    public class ChangePassDAC
    {
        private readonly string _connectionString;

        public ChangePassDAC(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void UpdatePasswordAndActivate(string userId, string hashedPassword)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("UPDATE [user] SET password = @Password, registration_status = 1 WHERE user_id = @UserId", conn);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
            cmd.Parameters.AddWithValue("@UserId", userId);

            cmd.ExecuteNonQuery();
        }
    }
}
