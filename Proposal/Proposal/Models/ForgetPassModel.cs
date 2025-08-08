using System.ComponentModel.DataAnnotations;

namespace Proposal.Models
{
    public class ForgetPassModel
    {
        [Required(ErrorMessage = "メールアドレスを入力してください。")]
        [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください。")]
        public string Email { get; set; }
    }
}
