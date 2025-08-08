using System.ComponentModel.DataAnnotations;

namespace Proposal.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "ユーザーIDは必須です。")]
        public string ?UserId { get; set; }
        [Required(ErrorMessage = "パスワードは必須です。")]
        public string ?Password { get; set; }
    }
}
