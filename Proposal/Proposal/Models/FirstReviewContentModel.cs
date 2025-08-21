using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Proposal.Models
{
    public class FirstReviewContentModel
    {
        public string? ProposalId { get; set; }
        public string? UserId { get; set; }
        public int? Status { get; set; }
        [Required(ErrorMessage = "審査指導・回答は必須です")]
        [MaxLength(2000, ErrorMessage = "審査指導・回答は2000文字以内で入力してください")]
        public string ReviewGuidance { get; set; }
    }
} 