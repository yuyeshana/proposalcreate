using System;
using System.Data;
using System.Data.SqlClient;
using Proposal.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Proposal.DAC
{
    public class ProposalDAC
    {
        private readonly string _connectionString;

        public ProposalDAC()
        {

        }
        public ProposalDAC(string connectionString)
        {
            _connectionString = connectionString;
        }

        //提案書詳細登録
        public int SqlInsertproposals_detail(ProposalModel basicInfo, ProposalContentModel proposalContent)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(@"INSERT INTO proposal (
                user_id,
                affiliation_id,
                department_id,
                section_id,
                subsection_id,
                proposal_name,
                proposal_type,
                proposal_year,
                status,
                name,
                first_reviewer_affiliation_id,
                first_reviewer_department_id,
                first_reviewer_section_id,
                first_reviewer_subsection_id,
                first_reviewer_name,
                evaluation_section_id,
                responsible_section_id1,
                responsible_section_id2,
                responsible_section_id3,
                responsible_section_id4,
                responsible_section_id5,
                genjyomondaiten,
                kaizenan,
                koukaJishi,
                kouka,
                created_time,
                updated_time
            ) 
            SELECT 
                @user_id,
                u.shozoku_id,
                u.department_id,
                u.section_id,
                u.subsection_id,
                @proposal_name,
                @proposal_type,
                @proposal_year,
                @status,
                @name,
                @first_reviewer_affiliation_id,
                @first_reviewer_department_id,
                @first_reviewer_section_id,
                @first_reviewer_subsection_id,
                @first_reviewer_name,
                @evaluation_section_id,
                @responsible_section_id1,
                @responsible_section_id2,
                @responsible_section_id3,
                @responsible_section_id4,
                @responsible_section_id5,
                @genjyomondaiten,
                @kaizenan,
                @koukaJishi,
                @kouka,
                SYSDATETIME(),
                SYSDATETIME()
            FROM [user] u 
            WHERE u.user_id = @user_id; 
            SELECT SCOPE_IDENTITY();", conn);
            // 基本信息参数
            cmd.Parameters.AddWithValue("@user_id", basicInfo.UserId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_name", basicInfo.TeianDaimei ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_type", basicInfo.ProposalTypeId ?? "");
            cmd.Parameters.AddWithValue("@proposal_year", basicInfo.TeianYear ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@status", basicInfo.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", basicInfo.ShimeiOrDaihyoumei ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@evaluation_section_id", basicInfo.EvaluationSectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id1", basicInfo.ResponsibleSectionId1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id2", basicInfo.ResponsibleSectionId2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id3", basicInfo.ResponsibleSectionId3 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id4", basicInfo.ResponsibleSectionId4 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id5", basicInfo.ResponsibleSectionId5 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_affiliation_id", basicInfo.FirstReviewerAffiliationId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_department_id", basicInfo.FirstReviewerDepartmentId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_section_id", basicInfo.FirstReviewerSectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_subsection_id", basicInfo.FirstReviewerSubsectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_name", basicInfo.FirstReviewerName ?? (object)DBNull.Value);

            // 提案内容参数
            cmd.Parameters.AddWithValue("@genjyomondaiten", proposalContent.GenjyoMondaiten ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@kaizenan", proposalContent.Kaizenan ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@koukaJishi", (object)(proposalContent.KoukaJishi.HasValue ? (int)proposalContent.KoukaJishi.Value : DBNull.Value));
            cmd.Parameters.AddWithValue("@kouka", proposalContent.Kouka ?? (object)DBNull.Value);
            // cmd.Parameters.AddWithValue("@attachmentfilename1", proposalContent.TenpuFileName1 ?? (object)DBNull.Value);
            // cmd.Parameters.AddWithValue("@attachmentfilename2", proposalContent.TenpuFileName2 ?? (object)DBNull.Value);
            // cmd.Parameters.AddWithValue("@attachmentfilename3", proposalContent.TenpuFileName3 ?? (object)DBNull.Value);
            // cmd.Parameters.AddWithValue("@attachmentfilename4", proposalContent.TenpuFileName4 ?? (object)DBNull.Value);
            // cmd.Parameters.AddWithValue("@attachmentfilename5", proposalContent.TenpuFileName5 ?? (object)DBNull.Value);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// 提案書の詳細情報をIDで取得
        /// </summary>
        public DataTable GetProposalDetailById(string id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT 
                    A.organizations_id as subsection_id,
                    B.organizations_id as section_id,
                    C.organizations_id as department_id,
                    D.organizations_id as affiliation_id,
                    A.organizations_name as subsection_name,
                    B.organizations_name as section_name,
                    C.organizations_name as department_name,
                    D.organizations_name as affiliation_name,

                    A1.organizations_id as subsection_id_1,
                    B1.organizations_id as section_id_1,
                    C1.organizations_id as department_id_1,
                    D1.organizations_id as affiliation_id_1,
                    A1.organizations_name as subsection_name_1,
                    B1.organizations_name as section_name_1,
                    C1.organizations_name as department_name_1,
                    D1.organizations_name as affiliation_name_1,
                    p.name_1,

                    A2.organizations_id as subsection_id_2,
                    B2.organizations_id as section_id_2,
                    C2.organizations_id as department_id_2,
                    D2.organizations_id as affiliation_id_2,
                    A2.organizations_name as subsection_name_2, 
                    B2.organizations_name as section_name_2,
                    C2.organizations_name as department_name_2,
                    D2.organizations_name as affiliation_name_2,
                    p.name_2,

                    A3.organizations_id as subsection_id_3,
                    B3.organizations_id as section_id_3,
                    C3.organizations_id as department_id_3,
                    D3.organizations_id as affiliation_id_3,
                    A3.organizations_name as subsection_name_3,
                    B3.organizations_name as section_name_3,
                    C3.organizations_name as department_name_3,
                    D3.organizations_name as affiliation_name_3,
                    p.name_3,

                    A4.organizations_id as subsection_id_4,
                    B4.organizations_id as section_id_4,
                    C4.organizations_id as department_id_4,
                    D4.organizations_id as affiliation_id_4,
                    A4.organizations_name as subsection_name_4,
                    B4.organizations_name as section_name_4,
                    C4.organizations_name as department_name_4,
                    D4.organizations_name as affiliation_name_4,
                    p.name_4,

                    A5.organizations_id as subsection_id_5,
                    B5.organizations_id as section_id_5,
                    C5.organizations_id as department_id_5,
                    D5.organizations_id as affiliation_id_5,
                    A5.organizations_name as subsection_name_5,
                    B5.organizations_name as section_name_5,
                    C5.organizations_name as department_name_5,
                    D5.organizations_name as affiliation_name_5,
                    p.name_5,

                    A6.organizations_id as subsection_id_6,
                    B6.organizations_id as section_id_6,
                    C6.organizations_id as department_id_6,
                    D6.organizations_id as affiliation_id_6,
                    A6.organizations_name as subsection_name_6,
                    B6.organizations_name as section_name_6,
                    C6.organizations_name as department_name_6,
                    D6.organizations_name as affiliation_name_6,
                    p.name_6,

                    A7.organizations_id as subsection_id_7,
                    B7.organizations_id as section_id_7,
                    C7.organizations_id as department_id_7,
                    D7.organizations_id as affiliation_id_7,
                    A7.organizations_name as subsection_name_7,
                    B7.organizations_name as section_name_7,
                    C7.organizations_name as department_name_7,
                    D7.organizations_name as affiliation_name_7,
                    p.name_7,

                    A8.organizations_id as subsection_id_8,
                    B8.organizations_id as section_id_8,
                    C8.organizations_id as department_id_8,
                    D8.organizations_id as affiliation_id_8,
                    A8.organizations_name as subsection_name_8,
                    B8.organizations_name as section_name_8,
                    C8.organizations_name as department_name_8,
                    D8.organizations_name as affiliation_name_8,
                    p.name_8,

                    A9.organizations_id as subsection_id_9,
                    B9.organizations_id as section_id_9,
                    C9.organizations_id as department_id_9,
                    D9.organizations_id as affiliation_id_9,
                    A9.organizations_name as subsection_name_9,
                    B9.organizations_name as section_name_9,
                    C9.organizations_name as department_name_9,
                    D9.organizations_name as affiliation_name_9,
                    p.name_9,

                    A10.organizations_id as subsection_id_10,
                    B10.organizations_id as section_id_10,
                    C10.organizations_id as department_id_10,
                    D10.organizations_id as affiliation_id_10,
                    A10.organizations_name as subsection_name_10,
                    B10.organizations_name as section_name_10,
                    C10.organizations_name as department_name_10,
                    D10.organizations_name as affiliation_name_10,
                    p.name_10,
                    
                    p.proposal_id,
                    p.user_id,
                    p.status,
                    p.proposal_year,
                    p.proposal_name,
                    p.proposal_type,
                    p.proposal_kbn,
                    p.organizations_id,
                    p.name,
                    p.group_name,
                    p.first_reviewer_affiliation_id,
                    p.first_reviewer_department_id,
                    p.first_reviewer_section_id,
                    p.first_reviewer_subsection_id,
                    p.first_reviewer_name,
                    p.evaluation_section_id,
                    p.responsible_section_id1,
                    p.responsible_section_id2,
                    p.responsible_section_id3,
                    p.responsible_section_id4,
                    p.responsible_section_id5,
                    p.genjyomondaiten,
                    p.kaizenan,
                    p.koukaJishi,
                    p.kouka,
                    p.review_guidance,
                    p.created_time,
                    p.updated_time
                FROM 
                    proposal p
                LEFT JOIN organizations A ON A.organizations_id = p.organizations_id
                LEFT JOIN organizations B ON B.organizations_id = A.organizations_parent_id
                LEFT JOIN organizations C ON C.organizations_id = B.organizations_parent_id
                LEFT JOIN organizations D ON D.organizations_id = C.organizations_parent_id
                LEFT JOIN organizations A1 ON A1.organizations_id = p.organizations_id_1
                LEFT JOIN organizations B1 ON B1.organizations_id = A1.organizations_parent_id
                LEFT JOIN organizations C1 ON C1.organizations_id = B1.organizations_parent_id
                LEFT JOIN organizations D1 ON D1.organizations_id = C1.organizations_parent_id
                LEFT JOIN organizations A2 ON A2.organizations_id = p.organizations_id_2
                LEFT JOIN organizations B2 ON B2.organizations_id = A2.organizations_parent_id
                LEFT JOIN organizations C2 ON C2.organizations_id = B2.organizations_parent_id
                LEFT JOIN organizations D2 ON D2.organizations_id = C2.organizations_parent_id
                LEFT JOIN organizations A3 ON A3.organizations_id = p.organizations_id_3
                LEFT JOIN organizations B3 ON B3.organizations_id = A3.organizations_parent_id
                LEFT JOIN organizations C3 ON C3.organizations_id = B3.organizations_parent_id
                LEFT JOIN organizations D3 ON D3.organizations_id = C3.organizations_parent_id
                LEFT JOIN organizations A4 ON A4.organizations_id = p.organizations_id_4
                LEFT JOIN organizations B4 ON B4.organizations_id = A4.organizations_parent_id
                LEFT JOIN organizations C4 ON C4.organizations_id = B4.organizations_parent_id
                LEFT JOIN organizations D4 ON D4.organizations_id = C4.organizations_parent_id
                LEFT JOIN organizations A5 ON A5.organizations_id = p.organizations_id_5
                LEFT JOIN organizations B5 ON B5.organizations_id = A5.organizations_parent_id
                LEFT JOIN organizations C5 ON C5.organizations_id = B5.organizations_parent_id
                LEFT JOIN organizations D5 ON D5.organizations_id = C5.organizations_parent_id
                LEFT JOIN organizations A6 ON A6.organizations_id = p.organizations_id_6
                LEFT JOIN organizations B6 ON B6.organizations_id = A6.organizations_parent_id
                LEFT JOIN organizations C6 ON C6.organizations_id = B6.organizations_parent_id
                LEFT JOIN organizations D6 ON D6.organizations_id = C6.organizations_parent_id
                LEFT JOIN organizations A7 ON A7.organizations_id = p.organizations_id_7
                LEFT JOIN organizations B7 ON B7.organizations_id = A7.organizations_parent_id
                LEFT JOIN organizations C7 ON C7.organizations_id = B7.organizations_parent_id
                LEFT JOIN organizations D7 ON D7.organizations_id = C7.organizations_parent_id
                LEFT JOIN organizations A8 ON A8.organizations_id = p.organizations_id_8
                LEFT JOIN organizations B8 ON B8.organizations_id = A8.organizations_parent_id
                LEFT JOIN organizations C8 ON C8.organizations_id = B8.organizations_parent_id
                LEFT JOIN organizations D8 ON D8.organizations_id = C8.organizations_parent_id
                LEFT JOIN organizations A9 ON A9.organizations_id = p.organizations_id_9
                LEFT JOIN organizations B9 ON B9.organizations_id = A9.organizations_parent_id
                LEFT JOIN organizations C9 ON C9.organizations_id = B9.organizations_parent_id
                LEFT JOIN organizations D9 ON D9.organizations_id = C9.organizations_parent_id
                LEFT JOIN organizations A10 ON A10.organizations_id = p.organizations_id_10
                LEFT JOIN organizations B10 ON B10.organizations_id = A10.organizations_parent_id
                LEFT JOIN organizations C10 ON C10.organizations_id = B10.organizations_parent_id
                LEFT JOIN organizations D10 ON D10.organizations_id = C10.organizations_parent_id
                WHERE p.proposal_id = @id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var adapter = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            
            return dataTable;
        }

        /// <summary>
        /// ユーザー情報をUserIdで取得
        /// </summary>
        public DataTable GetUserInfoByUserId(string UserId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT
                    A.organizations_id as subsection_id,
                    B.organizations_id as section_id,
                    C.organizations_id as department_id,
                    D.organizations_id as affiliation_id,
                    A.organizations_name as subsection_name,
                    B.organizations_name as section_name,
                    C.organizations_name as department_name,
                    D.organizations_name as affiliation_name
                FROM 
                    [user] u 
                LEFT JOIN
                    organizations A
                ON
                    A.organizations_id = u.organizations_id
                LEFT JOIN
                    organizations B
                ON
                    B.organizations_id = A.organizations_parent_id
                LEFT JOIN
                    organizations C
                ON
                    C.organizations_id = B.organizations_parent_id
                LEFT JOIN
                    organizations D
                ON
                    D.organizations_id = C.organizations_parent_id
                WHERE
                    u.user_id = @userid";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userid", UserId);
            using var adapter = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }

        //提案書詳細更新
        public void SqlUpdateproposals_detail(ProposalModel basicInfo, ProposalContentModel proposalContent)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(@"UPDATE proposal SET
                user_id = @user_id,
                status = @status,
                proposal_year = @proposal_year,
                proposal_name = @proposal_name,
                proposal_type = @proposal_type,
                affiliation_id = @affiliation_id,
                department_id = @department_id,
                section_id = @section_id,
                subsection_id = @subsection_id,
                name = @name,
                evaluation_section_id = @evaluation_section_id,
                responsible_section_id1 = @responsible_section_id1,
                responsible_section_id2 = @responsible_section_id2,
                responsible_section_id3 = @responsible_section_id3,
                responsible_section_id4 = @responsible_section_id4,
                responsible_section_id5 = @responsible_section_id5,
                first_reviewer_affiliation_id = @first_reviewer_affiliation_id,
                first_reviewer_department_id = @first_reviewer_department_id,
                first_reviewer_section_id = @first_reviewer_section_id,
                first_reviewer_subsection_id = @first_reviewer_subsection_id,
                first_reviewer_name = @first_reviewer_name,
                genjyomondaiten = @genjyomondaiten,
                kaizenan = @kaizenan,
                koukaJishi = @koukaJishi,
                kouka = @kouka,
                updated_time = SYSDATETIME()
            WHERE proposal_id = @proposal_id;", conn);
            // 基本信息参数
            cmd.Parameters.AddWithValue("@user_id", basicInfo.UserId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@status", basicInfo.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_year", basicInfo.TeianYear ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_name", basicInfo.TeianDaimei ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_type", basicInfo.ProposalTypeId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@affiliation_id", basicInfo.AffiliationId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@department_id", basicInfo.DepartmentId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@section_id", basicInfo.SectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@subsection_id", basicInfo.SubsectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", basicInfo.ShimeiOrDaihyoumei ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@evaluation_section_id", basicInfo.EvaluationSectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id1", basicInfo.ResponsibleSectionId1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id2", basicInfo.ResponsibleSectionId2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id3", basicInfo.ResponsibleSectionId3 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id4", basicInfo.ResponsibleSectionId4 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id5", basicInfo.ResponsibleSectionId5 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_affiliation_id", basicInfo.FirstReviewerAffiliationId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_department_id", basicInfo.FirstReviewerDepartmentId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_section_id", basicInfo.FirstReviewerSectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_subsection_id", basicInfo.FirstReviewerSubsectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_name", basicInfo.FirstReviewerName ?? (object)DBNull.Value);
            // 提案内容参数
            cmd.Parameters.AddWithValue("@genjyomondaiten", proposalContent.GenjyoMondaiten ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@kaizenan", proposalContent.Kaizenan ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@koukaJishi", (object)(proposalContent.KoukaJishi.HasValue ? (int)proposalContent.KoukaJishi.Value : DBNull.Value));
            cmd.Parameters.AddWithValue("@kouka", proposalContent.Kouka ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename1", proposalContent.TenpuFileName1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename2", proposalContent.TenpuFileName2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename3", proposalContent.TenpuFileName3 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename4", proposalContent.TenpuFileName4 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename5", proposalContent.TenpuFileName5 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_id", basicInfo.ProposalId);
            cmd.ExecuteNonQuery();
        }

        public int SqlInsertproposals_detailWithContent(ProposalModel basicInfo, ProposalContentModel proposalContent)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(@"INSERT INTO proposal (
                user_id,
                affiliation_id,
                department_id,
                section_id,
                subsection_id,
                proposal_name,
                proposal_type,
                proposal_year,
                status,
                evaluation_section_id,
                responsible_section_id1,
                responsible_section_id2,
                responsible_section_id3,
                responsible_section_id4,
                responsible_section_id5,
                first_reviewer_affiliation_id,
                first_reviewer_department_id,
                first_reviewer_section_id,
                first_reviewer_subsection_id,
                first_reviewer_name,
                genjyomondaiten,
                kaizenan,
                koukaJishi,
                kouka,
                created_time,
                updated_time,
                attachmentfilename1,
                attachmentfilename2,
                attachmentfilename3,
                attachmentfilename4,
                attachmentfilename5
            ) VALUES (
                @user_id,
                u.shozoku_id,
                u.department_id,
                u.section_id,
                u.subsection_id,
                @proposal_name,
                @proposal_type,
                @proposal_year,
                @status,
                @evaluation_section_id,
                @responsible_section_id1,
                @responsible_section_id2,
                @responsible_section_id3,
                @responsible_section_id4,
                @responsible_section_id5,
                @first_reviewer_affiliation_id,
                @first_reviewer_department_id,
                @first_reviewer_section_id,
                @first_reviewer_subsection_id,
                @first_reviewer_name,
                @genjyomondaiten,
                @kaizenan,
                @koukaJishi,
                @kouka,
                SYSDATETIME(),
                SYSDATETIME(),
                @attachmentfilename1,
                @attachmentfilename2,
                @attachmentfilename3,
                @attachmentfilename4,
                @attachmentfilename5
            ) SELECT SCOPE_IDENTITY();", conn);
            // 基本信息参数
            cmd.Parameters.AddWithValue("@user_id", basicInfo.UserId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_name", basicInfo.TeianDaimei ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_type", basicInfo.ProposalTypeId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_year", basicInfo.TeianYear ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@status", basicInfo.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@evaluation_section_id", basicInfo.EvaluationSectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id1", basicInfo.ResponsibleSectionId1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id2", basicInfo.ResponsibleSectionId2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id3", basicInfo.ResponsibleSectionId3 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id4", basicInfo.ResponsibleSectionId4 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id5", basicInfo.ResponsibleSectionId5 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_affiliation_id", basicInfo.FirstReviewerAffiliationId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_department_id", basicInfo.FirstReviewerDepartmentId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_section_id", basicInfo.FirstReviewerSectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_subsection_id", basicInfo.FirstReviewerSubsectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_name", basicInfo.FirstReviewerName ?? (object)DBNull.Value);
            // 提案内容参数
            cmd.Parameters.AddWithValue("@genjyomondaiten", proposalContent.GenjyoMondaiten ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@kaizenan", proposalContent.Kaizenan ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@koukaJishi", (object)(proposalContent.KoukaJishi.HasValue ? (int)proposalContent.KoukaJishi.Value : DBNull.Value));
            cmd.Parameters.AddWithValue("@kouka", proposalContent.Kouka ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename1", proposalContent.TenpuFileName1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename2", proposalContent.TenpuFileName2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename3", proposalContent.TenpuFileName3 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename4", proposalContent.TenpuFileName4 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename5", proposalContent.TenpuFileName5 ?? (object)DBNull.Value);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public void SqlUpdateproposals_detailWithContent(ProposalModel basicInfo, ProposalContentModel proposalContent)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(@"UPDATE proposal SET
                user_id = @user_id,
                status = @status,
                proposal_year = @proposal_year,
                proposal_name = @proposal_name,
                proposal_type = @proposal_type,
                affiliation_id = @affiliation_id,
                department_id = @department_id,
                section_id = @section_id,
                subsection_id = @subsection_id,
                name = @name,
                evaluation_section_id = @evaluation_section_id,
                responsible_section_id1 = @responsible_section_id1,
                responsible_section_id2 = @responsible_section_id2,
                responsible_section_id3 = @responsible_section_id3,
                responsible_section_id4 = @responsible_section_id4,
                responsible_section_id5 = @responsible_section_id5,
                first_reviewer_affiliation_id = @first_reviewer_affiliation_id,
                first_reviewer_department_id = @first_reviewer_department_id,
                first_reviewer_section_id = @first_reviewer_section_id,
                first_reviewer_subsection_id = @first_reviewer_subsection_id,
                first_reviewer_name = @first_reviewer_name,
                genjyomondaiten = @genjyomondaiten,
                kaizenan = @kaizenan,
                koukaJishi = @koukaJishi,
                kouka = @kouka,
                updated_time = SYSDATETIME()
            WHERE proposal_id = @proposal_id;", conn);
            // 基本信息参数
            cmd.Parameters.AddWithValue("@user_id", basicInfo.UserId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@status", basicInfo.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_year", basicInfo.TeianYear ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_name", basicInfo.TeianDaimei ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_type", basicInfo.ProposalTypeId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@affiliation_id", basicInfo.AffiliationId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@department_id", basicInfo.DepartmentId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@section_id", basicInfo.SectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@subsection_id", basicInfo.SubsectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@name", basicInfo.ShimeiOrDaihyoumei ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@evaluation_section_id", basicInfo.EvaluationSectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id1", basicInfo.ResponsibleSectionId1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id2", basicInfo.ResponsibleSectionId2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id3", basicInfo.ResponsibleSectionId3 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id4", basicInfo.ResponsibleSectionId4 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@responsible_section_id5", basicInfo.ResponsibleSectionId5 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_affiliation_id", basicInfo.FirstReviewerAffiliationId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_department_id", basicInfo.FirstReviewerDepartmentId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_section_id", basicInfo.FirstReviewerSectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_subsection_id", basicInfo.FirstReviewerSubsectionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@first_reviewer_name", basicInfo.FirstReviewerName ?? (object)DBNull.Value);
            // 提案内容参数
            cmd.Parameters.AddWithValue("@genjyomondaiten", proposalContent.GenjyoMondaiten ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@kaizenan", proposalContent.Kaizenan ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@koukaJishi", (object)(proposalContent.KoukaJishi.HasValue ? (int)proposalContent.KoukaJishi.Value : DBNull.Value));
            cmd.Parameters.AddWithValue("@kouka", proposalContent.Kouka ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename1", proposalContent.TenpuFileName1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename2", proposalContent.TenpuFileName2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename3", proposalContent.TenpuFileName3 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename4", proposalContent.TenpuFileName4 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@attachmentfilename5", proposalContent.TenpuFileName5 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@proposal_id", basicInfo.ProposalId);
            cmd.ExecuteNonQuery();
        }

        // 提案区分の一覧をデータベースから取得するメソッド
        public List<SelectListItem> GetProposalKbnList()
        {
            var list = new List<SelectListItem>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT kbn, kbn_name FROM proposal_kbn", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SelectListItem
                        {
                            Value = reader["kbn"].ToString(),
                            Text = reader["kbn_name"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<SelectListItem> GetProposalTypeList()
        {
            var list = new List<SelectListItem>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT type_id, type_name FROM proposal_type", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SelectListItem
                        {
                            Value = reader["type_id"].ToString(),
                            Text = reader["type_name"].ToString()
                        });
                    }
                }
            }
            return list;
        }


        // 提案所属の一覧をデータベースから取得するメソッド
        public List<Affiliation> GetAffiliations()
        {
            var list = new List<Affiliation>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT organizations_id, organizations_name FROM organizations where organizations_parent_id IS NULL", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Affiliation
                        {
                            Affiliation_id = reader["organizations_id"].ToString(),
                            Affiliation_name = reader["organizations_name"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 所属IDに基づいて部・署をデータベースから取得するメソッド
        public List<Department> GetDepartmentsByAffiliation(string affiliationId)
        {
            var list = new List<Department>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT organizations_id, organizations_name FROM organizations WHERE organizations_parent_id = @affiliationId", conn);
                cmd.Parameters.AddWithValue("@affiliationId", affiliationId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Department
                        {
                            Department_id = reader["organizations_id"].ToString(),
                            Department_name = reader["organizations_name"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 部・署IDに基づいて課・部門をデータベースから取得するメソッド
        public List<Section> GetSectionsByDepartment(string departmentId)
        {
            var list = new List<Section>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT organizations_id, organizations_name FROM organizations WHERE organizations_parent_id = @departmentId", conn);
                cmd.Parameters.AddWithValue("@departmentId", departmentId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Section
                        {
                            Section_id = reader["organizations_id"].ToString(),
                            Section_name = reader["organizations_name"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 課・部門IDに基づいて係・担当をデータベースから取得するメソッド
        public List<Subsection> GetSubsectionsBySection(string sectionId)
        {
            var list = new List<Subsection>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT organizations_id, organizations_name FROM organizations WHERE organizations_parent_id = @sectionId", conn);
                cmd.Parameters.AddWithValue("@sectionId", sectionId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Subsection
                        {
                            Subsection_id = reader["organizations_id"].ToString(),
                            Subsection_name = reader["organizations_name"].ToString()
                        });
                    }
                }
            }
            return list;
        }
    }
}
