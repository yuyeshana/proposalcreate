using System.ComponentModel.DataAnnotations;

namespace Proposal.Models
{
    /// <summary>
    /// ログイン画面で使用するモデルクラス
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// ユーザーID
        /// </summary>
        [Required(ErrorMessage = "ユーザーIDは必須です。")]
        public string ?UserId { get; set; }
        
        /// <summary>
        /// パスワード
        /// </summary>
        [Required(ErrorMessage = "パスワードは必須です。")]
        public string ?Password { get; set; }
    }
}
