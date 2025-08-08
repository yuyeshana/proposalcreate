using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using Proposal.Models;

namespace Proposal.Models
{

    // 所属
    public class Affiliation
    {
        public string Id { get; set; }
        public string Shozoku { get; set; }
    }
    // 部・署
    public class Department
    {
        public string Department_id { get; set; }
        public string Department_name { get; set; }
    }
    // 課・部門
    public class Section
    {
        public string Section_id { get; set; }
        public string Section_name { get; set; }
    }
    // 係・担当
    public class Subsection
    {
        public string Subsection_id { get; set; }
        public string Subsection_name { get; set; }
    }

    // 提案种类
    public class ProposalType
    {
        public string Kbn { get; set; }
        public string KbnName { get; set; }
    }

    // 主務課
    public class Shumuka
    {
        public string Shumuka_id { get; set; }
        public string Shumuka_name { get; set; }
    }

    // 関係課
    public class Kankeika
    {
        public string Kankeika_id { get; set; }
        public string Kankeika_name { get; set; }
    }

    // 組織架构
    public class Organization
    {
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationParentId { get; set; }
        public DateTime OrganizationCreatedTime { get; set; }
    }

    public enum KoukaJishi
    {
        [Description("選択してください")] Select = 0,
        [Description("実施済")] JisshiZumi = 1,
        [Description("未実施（予想効果）")] MiJisshiYosoKouka = 2
    }

    public class GroupMemberModel
    {
        public string? AffiliationId { get; set; }
        public string? DepartmentId { get; set; }
        public string? SectionId { get; set; }
        public string? SubsectionId { get; set; }
        public string? Name { get; set; }
    }

    public class CreateModel : IValidatableObject
    {
        // 主提案者 所属/部・署/課・部門/係・担当 id
        public string? AffiliationId { get; set; } // 所属id
        public string? DepartmentId { get; set; }  // 部・署id
        public string? SectionId { get; set; }     // 課・部門id
        public string? SubsectionId { get; set; }  // 係・担当id

        // 主提案者 所属/部・署/課・部門/係・担当 名称（只读显示用）
        public string AffiliationName { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public string SubsectionName { get; set; }

        // 提案种类
        [Required(ErrorMessage = "提案の種類を選択してください。")]
        public string ProposalTypeId { get; set; }
        // 提案区分
        [Required(ErrorMessage = "提案の区分を選択してください。")]
        public string ProposalKbnId { get; set; }

        // 用户ID
        public string UserId { get; set; }

        // 提案书编号
        public string? Id { get; set; }
        // 提案状态
        public int? Status { get; set; }
        // 提案年度
        public string TeianYear { get; set; }
        // 提案题名
        [Required(ErrorMessage = "提案題名を入力してください。")]
        [MaxLength(50, ErrorMessage = "50文字以内で入力してください。")]
        public string TeianDaimei { get; set; }
        // 氏名又は代表名
        [Required(ErrorMessage = "氏名又は代表名を入力してください。")]
        [MaxLength(20, ErrorMessage = "20文字以内で入力してください。")]
        public string ShimeiOrDaihyoumei { get; set; }

        // グループ名
        [MaxLength(20, ErrorMessage = "20文字以内で入力してください。")]
        public string? GroupMei { get; set; }

        // グループメンバーリスト
        public List<GroupMemberModel> GroupMembers { get; set; } = new List<GroupMemberModel>();
        // 第一次審査者
        public string? FirstReviewerAffiliationId { get; set; }
        public string? FirstReviewerDepartmentId { get; set; }
        public string? FirstReviewerSectionId { get; set; }
        public string? FirstReviewerSubsectionId { get; set; }
        public string? FirstReviewerUserId { get; set; }
        public string? FirstReviewerName { get; set; }
        public string? FirstReviewerTitle { get; set; }

        // 主務課
        [Required(ErrorMessage = "主務課を選択してください。")]
        public string EvaluationSectionId { get; set; }

        // 関係課
        [Required(ErrorMessage = "関係課を1つ以上選択してください。")]
        public string ResponsibleSectionId1 { get; set; }
        public string? ResponsibleSectionId2 { get; set; }
        public string? ResponsibleSectionId3 { get; set; }
        public string? ResponsibleSectionId4 { get; set; }
        public string? ResponsibleSectionId5 { get; set; }

        //作成日
        public string? Createddate { get; set; }
        //提出日
        public string? Submissiondate { get; set; }

        //第一次審査者を経ずに提出する
        public bool SkipFirstReviewer { get; set; }

        // データチェック
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 関係課を1つ以上選択すること
            if (string.IsNullOrEmpty(ResponsibleSectionId1)
                && string.IsNullOrEmpty(ResponsibleSectionId2)
                && string.IsNullOrEmpty(ResponsibleSectionId3)
                && string.IsNullOrEmpty(ResponsibleSectionId4)
                && string.IsNullOrEmpty(ResponsibleSectionId5))
            {
                yield return new ValidationResult("関係課を1つ以上選択してください。", new[] { nameof(ResponsibleSectionId1) });
            }
            // グループ校验（只有区分为グループ时才校验）
            if (ProposalKbnId == "2") // 2=グループ
            {
                // グループメンバー正しい順番チェック
                bool foundEmpty = false;
                for (int i = 0; i < GroupMembers.Count; i++)
                {
                    var m = GroupMembers[i];
                    bool anyFilled = !string.IsNullOrEmpty(m.DepartmentId) || !string.IsNullOrEmpty(m.SectionId) || !string.IsNullOrEmpty(m.SubsectionId) || !string.IsNullOrEmpty(m.Name);
                    if (!anyFilled)
                    {
                        foundEmpty = true;
                    }
                    else if (foundEmpty)
                    {
                        yield return new ValidationResult("グループメンバーは正しい順番でご記入ください。", new[] { $"GroupMembers[{i}]" });
                        break;
                    }
                    //　所属は選択した場合
                    if (!string.IsNullOrEmpty(m.DepartmentId))
                    {
                        if (string.IsNullOrEmpty(m.SectionId) || string.IsNullOrEmpty(m.SubsectionId) || string.IsNullOrEmpty(m.Name))
                        {
                            yield return new ValidationResult($"グループメンバー{i + 1}の情報をすべて入力してください。", new[] { $"GroupMembers[{i}]" });
                        }
                    }
                }
                // 区分がグループの時のみグループ名必須
                if (string.IsNullOrWhiteSpace(GroupMei))
                {
                    yield return new ValidationResult("グループ名は必須です。", new[] { nameof(GroupMei) });
                }
            }
        }
    }
}