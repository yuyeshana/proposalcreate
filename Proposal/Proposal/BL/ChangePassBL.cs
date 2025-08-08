using Proposal.DAL;
using System.Security.Cryptography;
using System.Text;

namespace Proposal.BL
{
    public class ChangePassBL
    {
        private readonly ChangePassDAC _changePassDAC;
        private string? connectionString;
        private IConfiguration configuration;

        public ChangePassBL(string? connectionString, IConfiguration configuration)
        {
            this.connectionString = connectionString;
            this.configuration = configuration;
            _changePassDAC = new ChangePassDAC(connectionString);
        }

        // パスワード変更処理（本人が設定）
        public bool ChangeUserPassword(string userId, string newPassword, string confirmPassword, out string error)
        {
            error = string.Empty;

            // 入力チェック
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                error = "すべての項目を入力してください。";
                return false;
            }

            if (newPassword != confirmPassword)
            {
                error = "パスワードが一致しません。";
                return false;
            }

            // パスワードの更新（※暗号化処理は必要に応じて）
            string hashedPassword = HashPasswordSHA256(newPassword);
            _changePassDAC.UpdatePasswordAndActivate(userId, hashedPassword);

            return true;
        }

        // SHA256によるパスワードハッシュ化処理
        public string HashPasswordSHA256(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
