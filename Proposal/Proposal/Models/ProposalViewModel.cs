using System.ComponentModel.DataAnnotations;

namespace Proposal.Models
{
    public class ProposalViewModel
    {
        //基本情報
        public ProposalModel BasicInfo { get; set; } = new ProposalModel();
        //提案内容
        public ProposalContentModel ProposalContent { get; set; } = new ProposalContentModel();
        //第一次審査
        public FirstReviewContentModel FirstReviewContent { get; set; } = new FirstReviewContentModel();
        public ProposalMode Mode { get; set; }

        public void PreserveUserInputData(
        ProposalModel basic,
        ProposalContentModel content,
        FirstReviewContentModel? first = null)
        {
            BasicInfo = basic ?? new ProposalModel();
            ProposalContent = content ?? new ProposalContentModel();
            FirstReviewContent = first ?? new FirstReviewContentModel();

            // 常に10人分の枠を確保
            EnsureGroupSize(10);
            SetModeFromStatus();
        }

        private void EnsureGroupSize(int size)
        {
            BasicInfo.GroupMembers ??= new List<GroupMemberModel>();
            while (BasicInfo.GroupMembers.Count < size)
                BasicInfo.GroupMembers.Add(new GroupMemberModel());
        }

        /// <summary>
        /// 基本情報の Status から画面モードを決定
        /// </summary>
        public void SetModeFromStatus()
        {
            int? s = BasicInfo?.Status;

            if (s == null || s == 1 || s == 9)
            {
                Mode = ProposalMode.Create;          // 作成モード
            }
            else if (s == 11 || s == 2)
            {
                Mode = ProposalMode.FirstReview;     // 一次審査モード
            }
            else
            {
                Mode = ProposalMode.Create;          // デフォルト
            }
        }
    }
} 