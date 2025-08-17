using System.ComponentModel.DataAnnotations;
using Proposal.Models;

namespace Proposal.Services
{
    /// <summary>
    /// 提案書のバリデーションサービス
    /// </summary>
    public class ProposalValidator
    {
        /// <summary>
        /// 提案書の基本情報を検証します
        /// </summary>
        /// <param name="model">検証対象の提案書モデル</param>
        /// <returns>バリデーション結果のコレクション</returns>
        public IEnumerable<ValidationResult> ValidateProposal(ProposalModel model)
        {
            var validationResults = new List<ValidationResult>();

            // 関係課の選択チェック
            if (IsAllResponsibleSectionsEmpty(model))
            {
                validationResults.Add(new ValidationResult("関係課を1つ以上選択してください。", new[] { nameof(ProposalModel.ResponsibleSectionId1) }));
            }

            // グループ提案の場合の追加チェック
            if (IsGroupProposal(model))
            {
                validationResults.AddRange(ValidateGroupProposal(model));
            }

            return validationResults;
        }

        /// <summary>
        /// すべての関係課が空かどうかをチェックします
        /// </summary>
        /// <param name="model">提案書モデル</param>
        /// <returns>すべて空の場合はtrue、そうでなければfalse</returns>
        private bool IsAllResponsibleSectionsEmpty(ProposalModel model)
        {
            return string.IsNullOrEmpty(model.ResponsibleSectionId1)
                && string.IsNullOrEmpty(model.ResponsibleSectionId2)
                && string.IsNullOrEmpty(model.ResponsibleSectionId3)
                && string.IsNullOrEmpty(model.ResponsibleSectionId4)
                && string.IsNullOrEmpty(model.ResponsibleSectionId5);
        }

        /// <summary>
        /// グループ提案かどうかをチェックします
        /// </summary>
        /// <param name="model">提案書モデル</param>
        /// <returns>グループ提案の場合はtrue、そうでなければfalse</returns>
        private bool IsGroupProposal(ProposalModel model)
        {
            return model.ProposalKbnId == "2"; // 2 = グループ
        }

        /// <summary>
        /// グループ提案のバリデーションを実行します
        /// </summary>
        /// <param name="model">提案書モデル</param>
        /// <returns>バリデーション結果のコレクション</returns>
        private IEnumerable<ValidationResult> ValidateGroupProposal(ProposalModel model)
        {
            var validationResults = new List<ValidationResult>();

            // グループメンバーの順序チェック
            validationResults.AddRange(ValidateGroupMembersOrder(model));

            // グループメンバーの必須項目チェック
            validationResults.AddRange(ValidateGroupMembersRequired(model));

            // グループ名の必須チェック
            if (string.IsNullOrWhiteSpace(model.GroupMei))
            {
                validationResults.Add(new ValidationResult("グループ名は必須です。", new[] { nameof(ProposalModel.GroupMei) }));
            }

            return validationResults;
        }

        /// <summary>
        /// グループメンバーの順序をチェックします
        /// </summary>
        /// <param name="model">提案書モデル</param>
        /// <returns>バリデーション結果のコレクション</returns>
        private IEnumerable<ValidationResult> ValidateGroupMembersOrder(ProposalModel model)
        {
            bool foundEmpty = false;
            
            for (int i = 0; i < model.GroupMembers.Count; i++)
            {
                var member = model.GroupMembers[i];
                bool hasAnyData = HasGroupMemberData(member);
                
                if (!hasAnyData)
                {
                    foundEmpty = true;
                }
                else if (foundEmpty)
                {
                    yield return new ValidationResult("グループメンバーは正しい順番でご記入ください。", new[] { $"GroupMembers[{i}]" });
                    break;
                }
            }
        }

        /// <summary>
        /// グループメンバーの必須項目をチェックします
        /// </summary>
        /// <param name="model">提案書モデル</param>
        /// <returns>バリデーション結果のコレクション</returns>
        private IEnumerable<ValidationResult> ValidateGroupMembersRequired(ProposalModel model)
        {
            for (int i = 0; i < model.GroupMembers.Count; i++)
            {
                var member = model.GroupMembers[i];
                
                // 部・署が選択されている場合、他の項目も必須
                if (!string.IsNullOrEmpty(member.DepartmentId))
                {
                    if (string.IsNullOrEmpty(member.SectionId) || 
                        string.IsNullOrEmpty(member.SubsectionId) || 
                        string.IsNullOrEmpty(member.Name))
                    {
                        yield return new ValidationResult(
                            $"グループメンバー{i + 1}の情報をすべて入力してください。", 
                            new[] { $"GroupMembers[{i}]" });
                    }
                }
            }
        }

        /// <summary>
        /// グループメンバーにデータがあるかどうかをチェックします
        /// </summary>
        /// <param name="member">グループメンバー</param>
        /// <returns>データがある場合はtrue、そうでなければfalse</returns>
        private bool HasGroupMemberData(GroupMemberModel member)
        {
            return !string.IsNullOrEmpty(member.DepartmentId) || 
                   !string.IsNullOrEmpty(member.SectionId) || 
                   !string.IsNullOrEmpty(member.SubsectionId) || 
                   !string.IsNullOrEmpty(member.Name);
        }
    }
}
