using System;
using System.Data;
using System.Data.SqlClient;
using Proposal.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Proposal.DAC
{
    public class CreateDAC
    {
        private readonly string _connectionString;

        public CreateDAC()
        {

        }
        public CreateDAC(string connectionString)
        {
            _connectionString = connectionString;
        }

        //提案書詳細登録
        public int SqlInsertproposals_detail(CreateModel basicInfo, ProposalContentModel proposalContent)
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
                    p.proposal_id,
                    p.user_id,
                    p.status,
                    p.proposal_year,
                    p.proposal_name,
                    p.proposal_type,
                    p.affiliation_id,
                    p.department_id,
                    p.section_id,
                    p.subsection_id,
                    p.name,
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
                    p.created_time,
                    p.updated_time
                FROM proposal p
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
                    u.shozoku_id as affiliation_id,
                    u.department_id,
                    u.section_id,
                    u.subsection_id,
                    A.organizations_name as affiliation_name,
                    B.organizations_name as department_name,
                    K.organizations_name as section_name,
                    T.organizations_name as subsection_name
                FROM 
                    [user] u 
                LEFT JOIN
                    organizations A
                ON
                    A.organizations_id = u.shozoku_id
                LEFT JOIN
                    organizations B
                ON
                    B.organizations_id = u.department_id
                LEFT JOIN
                    organizations K
                ON
                    K.organizations_id = u.section_id
                LEFT JOIN
                    organizations T
                ON
                    T.organizations_id = u.subsection_id
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
        public void SqlUpdateproposals_detail(CreateModel basicInfo, ProposalContentModel proposalContent)
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
            cmd.Parameters.AddWithValue("@proposal_id", basicInfo.Id);
            cmd.ExecuteNonQuery();
        }

        public int SqlInsertproposals_detailWithContent(CreateModel basicInfo, ProposalContentModel proposalContent)
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

        public void SqlUpdateproposals_detailWithContent(CreateModel basicInfo, ProposalContentModel proposalContent)
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
            cmd.Parameters.AddWithValue("@proposal_id", basicInfo.Id);
            cmd.ExecuteNonQuery();
        }

      
        // 提案種類（ProposalType）の一覧をデータベースから取得するメソッド
        public List<ProposalType> GetProposalTypes()
        {
            var list = new List<ProposalType>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT kbn, kbn_name FROM proposal_kbn", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProposalType
                        {
                            Kbn = reader["kbn"].ToString(),
                            KbnName = reader["kbn_name"].ToString()
                        });
                    }
                }
            }
            return list;
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
                var cmd = new SqlCommand("SELECT affiliation_id, affiliation_name FROM affiliation", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Affiliation
                        {
                            Id = reader["affiliation_id"].ToString(),
                            Shozoku = reader["affiliation_name"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 部・署をデータベースから取得するメソッド
        public List<Department> GetDepartments()
        {
            var list = new List<Department>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT department_id, department_name FROM department", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Department
                        {
                            Department_id = reader["department_id"].ToString(),
                            Department_name = reader["department_name"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 課・部門をデータベースから取得するメソッド
        public List<Section> GetSections()
        {
            var list = new List<Section>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT section_id, section_name FROM section", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Section
                        {
                            Section_id = reader["section_id"].ToString(),
                            Section_name = reader["section_name"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 係・担当をデータベースから取得するメソッド
        public List<Subsection> GetSubsections()
        {
            var list = new List<Subsection>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT subsection_id, subsection_name FROM subsection", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Subsection
                        {
                            Subsection_id = reader["subsection_id"].ToString(),
                            Subsection_name = reader["subsection_name"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 既存のグループデータを削除するメソッド
        public void DeleteGroupInfo(string proposalId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand("DELETE FROM group_info WHERE proposal_id = @proposalId", conn);
            cmd.Parameters.AddWithValue("@proposalId", proposalId);
            cmd.ExecuteNonQuery();
        }

        // グループデータを挿入するメソッド
        public void InsertGroupInfo(CreateModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            // 既存のグループデータを削除
            var delCmd = new SqlCommand("DELETE FROM group_info WHERE proposal_id = @proposalId", conn);
            delCmd.Parameters.AddWithValue("@proposalId", model.Id);
            delCmd.ExecuteNonQuery();

            // 10人分のメンバー情報を挿入
            var sql = @"INSERT INTO group_info (
                proposal_id,
                group_name,
                affiliation_id_1, department_id_1, section_id_1, subsection_id_1, name_1,
                affiliation_id_2, department_id_2, section_id_2, subsection_id_2, name_2,
                affiliation_id_3, department_id_3, section_id_3, subsection_id_3, name_3,
                affiliation_id_4, department_id_4, section_id_4, subsection_id_4, name_4,
                affiliation_id_5, department_id_5, section_id_5, subsection_id_5, name_5,
                affiliation_id_6, department_id_6, section_id_6, subsection_id_6, name_6,
                affiliation_id_7, department_id_7, section_id_7, subsection_id_7, name_7,
                affiliation_id_8, department_id_8, section_id_8, subsection_id_8, name_8,
                affiliation_id_9, department_id_9, section_id_9, subsection_id_9, name_9,
                affiliation_id_10, department_id_10, section_id_10, subsection_id_10, name_10
            ) VALUES (
                @proposal_id,
                @group_name,
                @affiliation_id_1, @department_id_1, @section_id_1, @subsection_id_1, @name_1,
                @affiliation_id_2, @department_id_2, @section_id_2, @subsection_id_2, @name_2,
                @affiliation_id_3, @department_id_3, @section_id_3, @subsection_id_3, @name_3,
                @affiliation_id_4, @department_id_4, @section_id_4, @subsection_id_4, @name_4,
                @affiliation_id_5, @department_id_5, @section_id_5, @subsection_id_5, @name_5,
                @affiliation_id_6, @department_id_6, @section_id_6, @subsection_id_6, @name_6,
                @affiliation_id_7, @department_id_7, @section_id_7, @subsection_id_7, @name_7,
                @affiliation_id_8, @department_id_8, @section_id_8, @subsection_id_8, @name_8,
                @affiliation_id_9, @department_id_9, @section_id_9, @subsection_id_9, @name_9,
                @affiliation_id_10, @department_id_10, @section_id_10, @subsection_id_10, @name_10
            )";
            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@proposal_id", model.Id);
            cmd.Parameters.AddWithValue("@group_name", model.GroupMei ?? (object)DBNull.Value);
            for (int i = 0; i < 10; i++)
            {
                var m = (model.GroupMembers != null && i < model.GroupMembers.Count) ? model.GroupMembers[i] : null;
                cmd.Parameters.AddWithValue($"@affiliation_id_{i + 1}", m?.AffiliationId ?? "");
                cmd.Parameters.AddWithValue($"@department_id_{i + 1}", m?.DepartmentId ?? "");
                cmd.Parameters.AddWithValue($"@section_id_{i + 1}", m?.SectionId ?? "");
                cmd.Parameters.AddWithValue($"@subsection_id_{i + 1}", m?.SubsectionId ?? "");
                cmd.Parameters.AddWithValue($"@name_{i + 1}", m?.Name ?? "");
            }
            cmd.ExecuteNonQuery();
        }

        // 指定proposal_idでgroup_infoの10人分のメンバー情報を取得（JOINで所属情報も取得）
        public DataTable GetGroupInfoByProposalId(string proposalId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            string sql = @"SELECT 
                g.group_name,
                g.affiliation_id_1, g.department_id_1, g.section_id_1, g.subsection_id_1, g.name_1,
                g.affiliation_id_2, g.department_id_2, g.section_id_2, g.subsection_id_2, g.name_2,
                g.affiliation_id_3, g.department_id_3, g.section_id_3, g.subsection_id_3, g.name_3,
                g.affiliation_id_4, g.department_id_4, g.section_id_4, g.subsection_id_4, g.name_4,
                g.affiliation_id_5, g.department_id_5, g.section_id_5, g.subsection_id_5, g.name_5,
                g.affiliation_id_6, g.department_id_6, g.section_id_6, g.subsection_id_6, g.name_6,
                g.affiliation_id_7, g.department_id_7, g.section_id_7, g.subsection_id_7, g.name_7,
                g.affiliation_id_8, g.department_id_8, g.section_id_8, g.subsection_id_8, g.name_8,
                g.affiliation_id_9, g.department_id_9, g.section_id_9, g.subsection_id_9, g.name_9,
                g.affiliation_id_10, g.department_id_10, g.section_id_10, g.subsection_id_10, g.name_10
            FROM group_info g
            WHERE g.proposal_id = @proposalId";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@proposalId", proposalId);
            using var adapter = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }

        /// <summary>
        /// 获取所有组织架构数据
        /// </summary>
        public List<Organization> GetAllOrganizations()
        {
            var list = new List<Organization>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT organizations_id, organizations_name, organizations_parent_id, created_time 
                    FROM organizations 
                    ORDER BY organizations_id, organizations_name", conn);
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Organization
                        {
                            OrganizationId = reader["organizations_id"].ToString(),
                            OrganizationName = reader["organizations_name"].ToString(),
                            OrganizationParentId = reader["organizations_parent_id"]?.ToString(),
                            OrganizationCreatedTime = Convert.ToDateTime(reader["created_time"])
                        });
                    }
                }
            }
            return list;
        }
    }
}
