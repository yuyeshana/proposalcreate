using Proposal.Models;
using System;
using System.Data.SqlClient;

namespace Proposal.DAL
{
    public class ForgetPassDAC
    {
        private readonly string _connectionString;

        public ForgetPassDAC(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetUserByEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT user_id, user_name, password, email_adress FROM [user] WHERE email_adress = @Email", conn);
            cmd.Parameters.AddWithValue("@Email", email);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = reader["user_id"].ToString(),
                    UserName = reader["user_name"].ToString(),
                    Password = reader["password"].ToString(),
                    UserEmail = reader["email_adress"].ToString()
                };
            }

            return null;
        }

        public void UpdateUserPassword(string userId, string hashedPassword)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("UPDATE [user] SET password = @Password, registration_status = 0 WHERE user_id = @UserId", conn);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
            cmd.Parameters.AddWithValue("@UserId", userId);

            cmd.ExecuteNonQuery();
        }
    }
}
