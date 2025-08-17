using Proposal.DAC;
using Proposal.Models;
using System.Security.Cryptography;
using System.Text;

namespace Proposal.BL
{
    /// <summary>
    /// ログイン処理を行うビジネスロジッククラス
    /// </summary>
    public class LoginBL
    {
        /// <summary>
        /// データアクセスクラス
        /// </summary>
        private readonly LoginDAC _LoginDAC;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionString">データベース接続文字列</param>
        public LoginBL(string connectionString)
        {
            _LoginDAC = new LoginDAC(connectionString);
        }

        /// <summary>
        /// ユーザーアカウントとパスワードの認証を行います
        /// </summary>
        /// <param name="pModel">ログインモデル</param>
        /// <returns>ログイン成功時はユーザーオブジェクト、失敗時はnullを返します</returns>
        public User ValidateUser(LoginModel pModel)
        {
            var user = _LoginDAC.GetUserById(pModel);
            if (user != null)
            {
                // 入力されたパスワードをハッシュ化
                string hashedInputPassword = HashPasswordSHA256(pModel.Password);
                // データベースに保存されているハッシュ化パスワードと比較
                if (user.Password == hashedInputPassword)
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// SHA256アルゴリズムを使用してパスワードをハッシュ化します
        /// </summary>
        /// <param name="password">ハッシュ化対象のパスワード</param>
        /// <returns>Base64エンコードされたハッシュ値</returns>
        public string HashPasswordSHA256(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
