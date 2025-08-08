using System.ComponentModel.DataAnnotations;

namespace Proposal.Models
{
    public class CreateViewModel
    {
        public CreateModel BasicInfo { get; set; } = new CreateModel();
        public ProposalContentModel ProposalContent { get; set; } = new ProposalContentModel();
    }
} 