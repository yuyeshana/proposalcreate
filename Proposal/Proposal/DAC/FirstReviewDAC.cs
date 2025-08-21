using System;
using System.Data;
using System.Data.SqlClient;
using Proposal.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Proposal.DAC
{
    public class FirstReviewDAC
    {
        private readonly string _connectionString;

        public FirstReviewDAC()
        {

        }
        public FirstReviewDAC(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 提案書の第一次審査情報をIDで取得
        /// </summary>
        public DataTable GetFirstReviewContentById(string id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT 
                    proposal_id,
                    status,
                    review_guidance,
                    created_time,
                    updated_time
                FROM 
                    proposal 
                WHERE proposal_id = @id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var adapter = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }

        /// <summary>
        /// 提案書の第一次審査情報を更新
        /// </summary>
        public void SqlUpdateFirstReviewContent(FirstReviewContentModel firstReviewContent)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(@"UPDATE proposal SET
                review_guidance = @review_guidance,
                status = @status,
                updated_time = SYSDATETIME()
            WHERE proposal_id = @proposal_id;", conn);
            cmd.Parameters.AddWithValue("@review_guidance", firstReviewContent.ReviewGuidance ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@status", firstReviewContent.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_id", firstReviewContent.ProposalId);
            cmd.ExecuteNonQuery();
        }
    }
}
