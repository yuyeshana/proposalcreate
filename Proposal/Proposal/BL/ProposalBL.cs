using Microsoft.AspNetCore.Http.HttpResults;
using Proposal.DAC;
using Proposal.Models;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Proposal.BL
{
    public class ProposalBL
    {
        private readonly ProposalDAC _createDAC = new ProposalDAC();

        public ProposalBL()
        {

        }

        public ProposalBL(string connectionString)
        {
            _createDAC = new ProposalDAC(connectionString);
        }

        /// <summary>
        /// すべてのドロップダウンリストを格納するViewModel
        /// </summary>
        public class DropdownsViewModel
        {
            //提案所属
            public List<SelectListItem> Affiliations { get; set; }

            //部・署
            public List<SelectListItem> Departments { get; set; }

            //課・部門
            public List<SelectListItem> Sections { get; set; }

            //係・担当
            public List<SelectListItem> Subsections { get; set; }

            //提案種類
            public List<SelectListItem> ProposalTypes { get; set; }

            //提案区分
            public List<SelectListItem> ProposalKbn { get; set; }

            //実施
            public List<SelectListItem> KoukaJishiList { get; set; }

            //主務課
            public List<SelectListItem> Shumukas { get; set; }

            //関係課
            public List<SelectListItem> Kankeikas { get; set; }
        }


        /// <summary>
        /// すべてのドロップダウンリストをまとめて取得するメソッド
        /// </summary>
        public DropdownsViewModel GetDropdowns()
        {
            return new DropdownsViewModel
            {
                //所属（部・署，課・部門，係・担当はAjaxで取得）
                Affiliations = GetAffiliations(),
                //提案の種類
                ProposalTypes = GetProposalTypeList(),
                //提案区分
                ProposalKbn = GetProposalKbnList(),
                //実施効果の種類
                KoukaJishiList = GetKoukaJishiSelectList(),
                //主務課
                //Shumukas = GetShumukaList(),
                //関係課
                //Kankeikas = GetKankeikaList(),
            };
        }

        /// <summary>
        /// 提案区分の取得
        /// </summary>
        public List<SelectListItem> GetProposalKbnList()
        {
            var list = _createDAC.GetProposalKbnList();
            list.Insert(0, new SelectListItem { Value = "", Text = "選択してください" });
            return list;
        }

        /// <summary>
        /// 提案の種類の取得
        /// </summary>
        public List<SelectListItem> GetProposalTypeList()
        {
            var list = _createDAC.GetProposalTypeList();
            list.Insert(0, new SelectListItem { Value = "", Text = "選択してください" });
            return list;
        }

        /// <summary>
        /// 提案所属の取得
        /// </summary>
        public List<SelectListItem> GetAffiliations()
        {
            var list = _createDAC.GetAffiliations().Select(a => new SelectListItem { Value = a.Affiliation_id.Trim(), Text = a.Affiliation_name.Trim() }).ToList();
            list.Insert(0, new SelectListItem { Value = "", Text = "選択してください" });
            return list;
        }

        /// <summary>
        /// 所属IDに基づいて部・署のSelectListItemリストを取得
        /// </summary>
        /// <param name="affiliationId">所属ID</param>
        /// <returns>部・署のSelectListItemリスト</returns>
        public List<SelectListItem> GetDepartmentsByAffiliation(string affiliationId)
        {
            var list = _createDAC.GetDepartmentsByAffiliation(affiliationId).Select(d => new SelectListItem { Value = d.Department_id.Trim(), Text = d.Department_name.Trim() }).ToList();
            //list.Insert(0, new SelectListItem { Value = "", Text = "選択してください" });
            return list;
        }

        /// <summary>
        /// 部・署IDに基づいて課・部門のSelectListItemリストを取得
        /// </summary>
        /// <param name="departmentId">部・署ID</param>
        /// <returns>課・部門のSelectListItemリスト</returns>
        public List<SelectListItem> GetSectionsByDepartment(string departmentId)
        {
            var list = _createDAC.GetSectionsByDepartment(departmentId).Select(s => new SelectListItem { Value = s.Section_id.Trim(), Text = s.Section_name.Trim() }).ToList();
            //list.Insert(0, new SelectListItem { Value = "", Text = "選択してください" });
            return list;
        }

        /// <summary>
        /// 課・部門IDに基づいて係・担当のSelectListItemリストを取得
        /// </summary>
        /// <param name="sectionId">課・部門ID</param>
        /// <returns>係・担当のSelectListItemリスト</returns>
        public List<SelectListItem> GetSubsectionsBySection(string sectionId)
        {
            var list = _createDAC.GetSubsectionsBySection(sectionId).Select(s => new SelectListItem { Value = s.Subsection_id.Trim(), Text = s.Subsection_name.Trim() }).ToList();
            //list.Insert(0, new SelectListItem { Value = "", Text = "選択してください" });
            return list;
        }

        /// <summary>
        /// 実施のSelectListItemリストを取得
        /// </summary>
        public List<SelectListItem> GetKoukaJishiSelectList()
        {
            return Enum.GetValues(typeof(KoukaJishi)).Cast<KoukaJishi>().Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = (e.GetType().GetField(e.ToString()).GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description) ?? e.ToString()
            }).OrderBy(x => int.Parse(x.Value)).ToList();
        }

        /// <summary>
        /// 提案書の詳細情報取得
        /// </summary>
        public void GetProposalDetailById(ProposalModel model, ProposalContentModel proposalContent)
        {
            var dataTable = _createDAC.GetProposalDetailById(model.ProposalId);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0]; // 指定IDのデータを取得

                // モデルに設定
                model.ProposalId = row["proposal_id"].ToString();
                model.Status = row["status"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["status"]);
                model.TeianYear = row["proposal_year"].ToString();
                model.TeianDaimei = row["proposal_name"].ToString();
                model.ProposalTypeId = row["proposal_type"].ToString();
                model.ShimeiOrDaihyoumei = row["name"].ToString();

                // 提案者情報
                model.AffiliationId = row["affiliation_id"].ToString();
                model.DepartmentId = row["department_id"].ToString();
                model.SectionId = row["section_id"].ToString();
                model.SubsectionId = row["subsection_id"].ToString();
                model.AffiliationName = row["affiliation_name"].ToString();
                model.DepartmentName = row["department_name"].ToString();
                model.SectionName = row["section_name"].ToString();
                model.SubsectionName = row["subsection_name"].ToString();

                // 第一次審査者情報
                model.FirstReviewerAffiliationId = row["first_reviewer_affiliation_id"].ToString();
                model.FirstReviewerDepartmentId = row["first_reviewer_department_id"].ToString();
                model.FirstReviewerSectionId = row["first_reviewer_section_id"].ToString();
                model.FirstReviewerSubsectionId = row["first_reviewer_subsection_id"].ToString();
                model.FirstReviewerName = row["first_reviewer_name"].ToString();

                // 第一次審査者情報がnullの場合はcheckboxをチェック
                if (string.IsNullOrEmpty(model.FirstReviewerAffiliationId))
                {
                    model.SkipFirstReviewer = true;
                }
                model.EvaluationSectionId = row["evaluation_section_id"].ToString().Trim();
                model.ResponsibleSectionId1 = row["responsible_section_id1"].ToString().Trim();
                model.ResponsibleSectionId2 = row["responsible_section_id2"].ToString().Trim();
                model.ResponsibleSectionId3 = row["responsible_section_id3"].ToString().Trim();
                model.ResponsibleSectionId4 = row["responsible_section_id4"].ToString().Trim();
                model.ResponsibleSectionId5 = row["responsible_section_id5"].ToString().Trim();

                // 提案内容赋值
                proposalContent.GenjyoMondaiten = row["genjyomondaiten"].ToString();
                proposalContent.Kaizenan = row["kaizenan"].ToString();
                proposalContent.KoukaJishi = row["koukaJishi"] == DBNull.Value ? (KoukaJishi?)null : (KoukaJishi)Convert.ToInt32(row["koukaJishi"]);
                proposalContent.Kouka = row["kouka"].ToString();
                // 日付
                model.Createddate = row["created_time"].ToString();


                // 提案区分はグループの場合
                if (row["proposal_kbn"].ToString() == "2")
                {
                    model.ProposalKbnId = "2";
                    // --- グループメンバー情報の取得と設定 ---
                    model.GroupMembers = new List<GroupMemberModel>();

                    // グループ名を設定
                    model.GroupMei = row["group_name"].ToString();

                    int validMemberCount = 0;
                    for (int i = 1; i <= 10; i++)
                    {
                        var m = new GroupMemberModel
                        {
                            AffiliationId = row[$"affiliation_id_{i}"].ToString().Trim(),
                            DepartmentId = row[$"department_id_{i}"].ToString().Trim(),
                            SectionId = row[$"section_id_{i}"].ToString().Trim(),
                            SubsectionId = row[$"subsection_id_{i}"].ToString().Trim(),
                            Name = row[$"name_{i}"].ToString(),
                        };

                        validMemberCount++;
                        model.GroupMembers.Add(m);
                    }
                }
                else
                {
                    model.ProposalKbnId = "1";
                }

            }
        }

        /// <summary>
        /// ユーザー情報を取得し、モデルに設定
        /// </summary>     
        public void GetUserInfoByUserId(ProposalModel model)
        {
            var dataTable = _createDAC.GetUserInfoByUserId(model.UserId);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0]; // 指定IDのデータを取得

                // ID フィールドを設定（既存の値がある場合は上書きしない）
                var affiliationId = row["affiliation_id"].ToString();
                if (!string.IsNullOrEmpty(affiliationId))
                {
                    model.AffiliationId = affiliationId;
                }

                var departmentId = row["department_id"].ToString();
                if (!string.IsNullOrEmpty(departmentId))
                {
                    model.DepartmentId = departmentId;
                }

                var sectionId = row["section_id"].ToString();
                if (!string.IsNullOrEmpty(sectionId))
                {
                    model.SectionId = sectionId;
                }

                var subsectionId = row["subsection_id"].ToString();
                if (!string.IsNullOrEmpty(subsectionId))
                {
                    model.SubsectionId = subsectionId;
                }

                // 表示名フィールドを設定
                model.AffiliationName = row["affiliation_name"].ToString();
                model.DepartmentName = row["department_name"].ToString();
                model.SectionName = row["section_name"].ToString();
                model.SubsectionName = row["subsection_name"].ToString();
            }
        }


        /// <summary>
        /// 提案書詳細登録
        /// </summary>
        public void Insertproposals_detail(ProposalModel basicInfo, ProposalContentModel proposalContent)
        {
            //作成日
            basicInfo.Createddate = DateTime.Now.ToString();

            //提出日
            //一時保存
            if (basicInfo.Status == 1)
            {
                basicInfo.Submissiondate = null;
            }
            //提出
            else if (basicInfo.Status == 11)
            {
                basicInfo.Submissiondate = DateTime.Now.ToString();
            }

            if (basicInfo.ProposalKbnId == "2")
            {
                _createDAC.Insertproposals_detail_GroupInfo(basicInfo, proposalContent);
            }
            else
            {
                _createDAC.SqlInsertproposals_detail(basicInfo, proposalContent);
            }
        }

        /// <summary>
        /// 提案書詳細更新
        /// </summary>
        public void Updateproposals_detail(ProposalModel basicInfo, ProposalContentModel proposalContent)
        {
            //提出日
            //一時保存
            if (basicInfo.Status == 1)
            {
                basicInfo.Submissiondate = null;
            }
            //提出
            else if (basicInfo.Status == 11)
            {
                basicInfo.Submissiondate = DateTime.Now.ToString();
            }

            // 更新前にユーザー情報を取得してaffiliation_idなどを設定
            if (!string.IsNullOrEmpty(basicInfo.UserId))
            {
                GetUserInfoByUserId(basicInfo);
            }

            //提案書詳細更新
            _createDAC.SqlUpdateproposals_detail(basicInfo, proposalContent);
        }

    }
}
