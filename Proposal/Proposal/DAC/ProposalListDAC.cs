using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Proposal.Models;

namespace Proposal.DAC
{
    public class ProposalListDAC
    {
        private readonly string _connectionString;

        public ProposalListDAC(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 提案書一覧を取得する
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="year">提案年度</param>
        /// <param name="status">提案状態</param>
        public (List<ProposalList> Items, int TotalCount, int TotalPages) GetProposals(
            string userId, string? year, int? status, DateTime? dateFrom, DateTime? dateTo, int page, int pageSize)
        {
            var items = new List<ProposalList>();
            int totalCount = 0;

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string filterSql = "";
            if (!string.IsNullOrEmpty(year)) filterSql += " AND p.proposal_year = @Year";
            if (status.HasValue) filterSql += " AND p.status = @Status";
            if (dateFrom.HasValue) filterSql += " AND p.submitted_date >= @DateFrom";
            if (dateTo.HasValue) filterSql += " AND p.submitted_date <= @DateTo";

            string sql = $@"
            SELECT COUNT(*)
            FROM ProposalsDB.dbo.proposal p
            LEFT JOIN ProposalsDB.dbo.[user] u ON (p.user_id = u.user_id 
                OR (u.shozoku_id = p.first_reviewer_affiliation_id and u.department_id = p.first_reviewer_department_id and u.section_id = p.first_reviewer_section_id and u.subsection_id = p.first_reviewer_subsection_id)
                OR (u.section_id = p.evaluation_section_id OR u.section_id = p.responsible_section_id1 OR u.section_id = p.responsible_section_id2 OR u.section_id = p.responsible_section_id3 OR u.section_id = p.responsible_section_id4 OR u.section_id = p.responsible_section_id5))
            LEFT JOIN ProposalsDB.dbo.[organizations] o ON u.shozoku_id = o.organizations_id
            LEFT JOIN ProposalsDB.dbo.[proposal_status] s ON p.status = s.status_id
            WHERE u.user_id = @UserId {filterSql};
                SELECT 
                    p.*, 
                    o.organizations_name,
                    s.status_name
                FROM ProposalsDB.dbo.proposal p
                LEFT JOIN ProposalsDB.dbo.[user] u ON (p.user_id = u.user_id 
                    OR (u.shozoku_id = p.first_reviewer_affiliation_id and u.department_id = p.first_reviewer_department_id and u.section_id = p.first_reviewer_section_id and u.subsection_id = p.first_reviewer_subsection_id)
                    OR (u.section_id = p.evaluation_section_id OR u.section_id = p.responsible_section_id1 OR u.section_id = p.responsible_section_id2 OR u.section_id = p.responsible_section_id3 OR u.section_id = p.responsible_section_id4 OR u.section_id = p.responsible_section_id5))
                LEFT JOIN ProposalsDB.dbo.[organizations] o ON u.shozoku_id = o.organizations_id
                LEFT JOIN ProposalsDB.dbo.[proposal_status] s ON p.status = s.status_id
                WHERE u.user_id = @UserId {filterSql}
                ORDER BY p.created_time DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";


            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            if (!string.IsNullOrEmpty(year)) cmd.Parameters.AddWithValue("@Year", year);
            if (status.HasValue) cmd.Parameters.AddWithValue("@Status", status.Value);
            if (dateFrom.HasValue) cmd.Parameters.AddWithValue("@DateFrom", dateFrom.Value);
            if (dateTo.HasValue) cmd.Parameters.AddWithValue("@DateTo", dateTo.Value.Date.AddDays(1).AddTicks(-1));
            cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                totalCount = reader.GetInt32(0);
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    items.Add(new ProposalList
                    {
                        ProposalYear = reader.IsDBNull(reader.GetOrdinal("proposal_year")) ? "" : reader.GetString(reader.GetOrdinal("proposal_year")),
                        ProposalId = reader.IsDBNull(reader.GetOrdinal("proposal_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("proposal_id")),
                        Status = reader.IsDBNull(reader.GetOrdinal("status_name")) ? "" : reader.GetString(reader.GetOrdinal("status_name")),
                        ProposalName = reader.IsDBNull(reader.GetOrdinal("proposal_name")) ? "" : reader.GetString(reader.GetOrdinal("proposal_name")),
                        UserName = reader.IsDBNull(reader.GetOrdinal("name")) ? "" : reader.GetString(reader.GetOrdinal("name")),
                        GroupName = reader.IsDBNull(reader.GetOrdinal("group_name")) ? "" : reader.GetString(reader.GetOrdinal("group_name")),
                        Affiliation = reader.IsDBNull(reader.GetOrdinal("organizations_name")) ? "" : reader.GetString(reader.GetOrdinal("organizations_name")),
                        CreatedTime = reader.IsDBNull(reader.GetOrdinal("created_time")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("created_time")),
                        Point = reader.IsDBNull(reader.GetOrdinal("point")) ? 0 : reader.GetInt32(reader.GetOrdinal("point")),
                        Decision = reader.IsDBNull(reader.GetOrdinal("decision")) ? 0 : reader.GetInt32(reader.GetOrdinal("decision")),
                        AdditionalSubmission = reader.IsDBNull(reader.GetOrdinal("additional_submission")) ? false : reader.GetBoolean(reader.GetOrdinal("additional_submission")),
                        EvaluationSection = reader.IsDBNull(reader.GetOrdinal("evaluation_section_id")) ? "" : reader.GetString(reader.GetOrdinal("evaluation_section_id")),
                        ResponsibleSection1 = reader.IsDBNull(reader.GetOrdinal("responsible_section_id1")) ? "" : reader.GetString(reader.GetOrdinal("responsible_section_id1")),
                        ResponsibleSection2 = reader.IsDBNull(reader.GetOrdinal("responsible_section_id2")) ? "" : reader.GetString(reader.GetOrdinal("responsible_section_id2")),
                        ResponsibleSection3 = reader.IsDBNull(reader.GetOrdinal("responsible_section_id3")) ? "" : reader.GetString(reader.GetOrdinal("responsible_section_id3")),
                        ResponsibleSection4 = reader.IsDBNull(reader.GetOrdinal("responsible_section_id4")) ? "" : reader.GetString(reader.GetOrdinal("responsible_section_id4")),
                        ResponsibleSection5 = reader.IsDBNull(reader.GetOrdinal("responsible_section_id5")) ? "" : reader.GetString(reader.GetOrdinal("responsible_section_id5")),
                        AwardType = reader.IsDBNull(reader.GetOrdinal("award_type")) ? 0 : reader.GetInt32(reader.GetOrdinal("award_type")),
                    });
                }
            }

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            return (items, totalCount, totalPages);
        }

        /// <summary>
        /// 提案状態一覧を取得する
        /// </summary>
        /// <returns>提案状態一覧</returns>
        public List<ProposalStatus> GetProposalStatuses()
        {
            var statuses = new List<ProposalStatus>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = "SELECT status_id, status_name FROM [proposal_status] ORDER BY status_id";

            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                statuses.Add(new ProposalStatus
                {
                    Id = reader.GetInt32(reader.GetOrdinal("status_id")),
                    StatusName = reader.GetString(reader.GetOrdinal("status_name"))
                });
            }

            return statuses;
        }
    }
}
