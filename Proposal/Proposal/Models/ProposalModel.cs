using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace Proposal.Models
{
    /// <summary>
    /// 提提案書画面のモードを表す列挙型
    /// </summary>
    public enum ProposalMode
    {
        Create,
        FirstReview,
        SecondReview,
        Approved,
        Rejected
    }

    /// <summary>
    /// 所属情報を表すモデル
    /// </summary>
    public class Affiliation
    {
        /// <summary>
        /// 所属ID
        /// </summary>
        public string Affiliation_id { get; set; } = string.Empty;

        /// <summary>
        /// 所属名
        /// </summary>
        public string Affiliation_name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 部・署情報を表すモデル
    /// </summary>
    public class Department
    {
        /// <summary>
        /// 部・署ID
        /// </summary>
        public string Department_id { get; set; } = string.Empty;

        /// <summary>
        /// 部・署名
        /// </summary>
        public string Department_name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 課・部門情報を表すモデル
    /// </summary>
    public class Section
    {
        /// <summary>
        /// 課・部門ID
        /// </summary>
        public string Section_id { get; set; } = string.Empty;

        /// <summary>
        /// 課・部門名
        /// </summary>
        public string Section_name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 係・担当情報を表すモデル
    /// </summary>
    public class Subsection
    {
        /// <summary>
        /// 係・担当ID
        /// </summary>
        public string Subsection_id { get; set; } = string.Empty;

        /// <summary>
        /// 係・担当名
        /// </summary>
        public string Subsection_name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 組織構造情報を表すモデル
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// 組織ID
        /// </summary>
        public string OrganizationId { get; set; } = string.Empty;

        /// <summary>
        /// 組織名
        /// </summary>
        public string OrganizationName { get; set; } = string.Empty;

        /// <summary>
        /// 親組織ID
        /// </summary>
        public string OrganizationParentId { get; set; } = string.Empty;

        /// <summary>
        /// 組織作成日時
        /// </summary>
        public DateTime OrganizationCreatedTime { get; set; }
    }

    /// <summary>
    /// 効果の実施状況を表す列挙型
    /// </summary>
    public enum KoukaJishi
    {
        /// <summary>
        /// 選択してください
        /// </summary>
        [Description("選択してください")]
        None = -1,

        /// <summary>
        /// 実施済
        /// </summary>
        [Description("実施済")]
        JisshiZumi = 1,

        /// <summary>
        /// 未実施（予想効果）
        /// </summary>
        [Description("未実施（予想効果）")]
        MiJisshiYosoKouka = 0
    }

    /// <summary>
    /// グループメンバー情報を表すモデル
    /// </summary>
    public class GroupMemberModel
    {
        /// <summary>
        /// 所属ID
        /// </summary>
        public string? AffiliationId { get; set; }

        /// <summary>
        /// 部・署ID
        /// </summary>
        public string? DepartmentId { get; set; }

        /// <summary>
        /// 課・部門ID
        /// </summary>
        public string? SectionId { get; set; }

        /// <summary>
        /// 係・担当ID
        /// </summary>
        public string? SubsectionId { get; set; }

        /// <summary>
        /// 氏名
        /// </summary>
        public string? Name { get; set; }
    }

    /// <summary>
    /// 提案書作成・編集用の基本情報モデル
    /// </summary>
    public class ProposalModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProposalModel()
        {
            // GroupMembersを10個の空の要素で初期化
            GroupMembers = new List<GroupMemberModel>();
            for (int i = 0; i < 10; i++)
            {
                GroupMembers.Add(new GroupMemberModel());
            }
        }

        #region 主提案者の所属情報

        /// <summary>
        /// 主提案者の所属ID
        /// </summary>
        public string? AffiliationId { get; set; }

        /// <summary>
        /// 主提案者の部・署ID
        /// </summary>
        public string? DepartmentId { get; set; }

        /// <summary>
        /// 主提案者の課・部門ID
        /// </summary>
        public string? SectionId { get; set; }

        /// <summary>
        /// 主提案者の係・担当ID
        /// </summary>
        public string? SubsectionId { get; set; }

        /// <summary>
        /// 主提案者の所属名（表示用）
        /// </summary>
        public string AffiliationName { get; set; } = string.Empty;

        /// <summary>
        /// 主提案者の部・署名（表示用）
        /// </summary>
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// 主提案者の課・部門名（表示用）
        /// </summary>
        public string SectionName { get; set; } = string.Empty;

        /// <summary>
        /// 主提案者の係・担当名（表示用）
        /// </summary>
        public string SubsectionName { get; set; } = string.Empty;

        #endregion

        #region 提案の基本情報

        /// <summary>
        /// 提案の種類ID
        /// </summary>
        [Required(ErrorMessage = "提案の種類を選択してください。")]
        public string ProposalTypeId { get; set; } = string.Empty;

        /// <summary>
        /// 提案の区分ID
        /// </summary>
        [Required(ErrorMessage = "提案の区分を選択してください。")]
        public string ProposalKbnId { get; set; } = string.Empty;

        /// <summary>
        /// ユーザーID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 提案書ID
        /// </summary>
        public string? ProposalId { get; set; }

        /// <summary>
        /// 提案の状態
        /// null:新規作成
        /// 1:作成中
        /// 2:一次審査中
        /// 11:審査中
        /// 12:審査済
        /// 13:審査不合格
        /// 14:審査合格
        /// 15:審査合格（一次審査者経由）
        /// 16:審査合格（一次審査者経由）
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 提案年度
        /// </summary>
        public string TeianYear { get; set; } = string.Empty;

        /// <summary>
        /// 提案題名
        /// </summary>
        [Required(ErrorMessage = "提案題名を入力してください。")]
        [MaxLength(50, ErrorMessage = "50文字以内で入力してください。")]
        public string TeianDaimei { get; set; } = string.Empty;

        /// <summary>
        /// 氏名又は代表名
        /// </summary>
        [Required(ErrorMessage = "氏名又は代表名を入力してください。")]
        [MaxLength(20, ErrorMessage = "20文字以内で入力してください。")]
        public string ShimeiOrDaihyoumei { get; set; } = string.Empty;

        /// <summary>
        /// グループ名
        /// </summary>
        [MaxLength(20, ErrorMessage = "20文字以内で入力してください。")]
        public string? GroupMei { get; set; }

        /// <summary>
        /// グループメンバーリスト
        /// </summary>
        public List<GroupMemberModel> GroupMembers { get; set; } = new List<GroupMemberModel>();

        #endregion

        #region 第一次審査者情報

        /// <summary>
        /// 第一次審査者の所属ID
        /// </summary>
        public string? FirstReviewerAffiliationId { get; set; }

        /// <summary>
        /// 第一次審査者の部・署ID
        /// </summary>
        public string? FirstReviewerDepartmentId { get; set; }

        /// <summary>
        /// 第一次審査者の課・部門ID
        /// </summary>
        public string? FirstReviewerSectionId { get; set; }

        /// <summary>
        /// 第一次審査者の係・担当ID
        /// </summary>
        public string? FirstReviewerSubsectionId { get; set; }

        /// <summary>
        /// 第一次審査者の氏名
        /// </summary>
        public string? FirstReviewerName { get; set; }

        /// <summary>
        /// 第一次審査者の官職
        /// </summary>
        public string? FirstReviewerTitle { get; set; }

        #endregion

        #region 主務課・関係課情報

        /// <summary>
        /// 主務課ID
        /// </summary>
        [Required(ErrorMessage = "主務課を選択してください。")]
        public string EvaluationSectionId { get; set; } = string.Empty;

        /// <summary>
        /// 関係課1のID
        /// </summary>
        [Required(ErrorMessage = "関係課を1つ以上選択してください。")]
        public string ResponsibleSectionId1 { get; set; } = string.Empty;

        /// <summary>
        /// 関係課2のID
        /// </summary>
        public string? ResponsibleSectionId2 { get; set; }

        /// <summary>
        /// 関係課3のID
        /// </summary>
        public string? ResponsibleSectionId3 { get; set; }

        /// <summary>
        /// 関係課4のID
        /// </summary>
        public string? ResponsibleSectionId4 { get; set; }

        /// <summary>
        /// 関係課5のID
        /// </summary>
        public string? ResponsibleSectionId5 { get; set; }

        #endregion

        #region 日付情報

        /// <summary>
        /// 作成日
        /// </summary>
        public string? Createddate { get; set; }

        /// <summary>
        /// 提出日
        /// </summary>
        public string? Submissiondate { get; set; }

        #endregion

        #region その他の設定

        /// <summary>
        /// 第一次審査者を経ずに提出するフラグ
        /// </summary>
        public bool SkipFirstReviewer { get; set; }

        #endregion
    }
}