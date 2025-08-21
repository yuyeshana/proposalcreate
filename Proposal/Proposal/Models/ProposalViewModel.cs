using System.ComponentModel.DataAnnotations;

namespace Proposal.Models
{
    public class ProposalViewModel
    {
        //基本情報
        public ProposalModel BasicInfo { get; set; } = new ProposalModel();
        //提案内容
        public ProposalContentModel ProposalContent { get; set; } = new ProposalContentModel();
    }
} 