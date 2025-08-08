using System.Runtime.InteropServices;

namespace Proposal.Models
{
    public class ProposalStatus
    {
        public int Id { get; set; }
        public string? StatusName { get; set; }
    }


    public class ProposalList
    {
        public string? ProposalYear { get; set; }
        public int ProposalId { get; set; }
        public string? Status { get; set; }
        public string? ProposalName { get; set; }
        public string? UserName { get; set; }
        public string? GroupName { get; set; }
        public string? Affiliation { get; set; }
        public DateTime CreatedTime { get; set; }
        public int? Point { get; set; } 
        public int? Decision { get; set; }
        public bool? AdditionalSubmission { get; set; }
        public string? EvaluationSection { get; set; }
        public string? ResponsibleSection1 { get; set; }
        public string? ResponsibleSection2 { get; set; }
        public string? ResponsibleSection3 { get; set; }
        public string? ResponsibleSection4 { get; set; }
        public string? ResponsibleSection5 { get; set; }
        public int? AwardType { get; set; }
    }
}
