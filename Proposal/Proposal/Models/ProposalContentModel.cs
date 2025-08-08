using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Proposal.Models
{
    public class ProposalContentModel
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }

        [Required(ErrorMessage = "現状・問題点は必須です")]
        [MaxLength(1000, ErrorMessage = "現状・問題点は1000文字以内で入力してください")]
        public string GenjyoMondaiten { get; set; }

        [Required(ErrorMessage = "改善案は必須です")]
        [MaxLength(1000, ErrorMessage = "改善案は1000文字以内で入力してください")]
        public string Kaizenan { get; set; }

        [Required(ErrorMessage = "効果の種類を選択してください")]
        public KoukaJishi? KoukaJishi { get; set; }

        [Required(ErrorMessage = "効果は必須です")]
        [MaxLength(1000, ErrorMessage = "効果は1000文字以内で入力してください")]
        public string Kouka { get; set; }

        public IFormFile? TenpuFile1 { get; set; }
        public IFormFile? TenpuFile2 { get; set; }
        public IFormFile? TenpuFile3 { get; set; }
        public IFormFile? TenpuFile4 { get; set; }
        public IFormFile? TenpuFile5 { get; set; }

        public string? TenpuFileName1 { get; set; }
        public string? TenpuFileName2 { get; set; }
        public string? TenpuFileName3 { get; set; }
        public string? TenpuFileName4 { get; set; }
        public string? TenpuFileName5 { get; set; }
    }
} 