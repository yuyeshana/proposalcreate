using Proposal.DAL;
using Proposal.Services;
using System.Security.Cryptography;
using System.Text;

namespace Proposal.BL
{
    public class ForgetPassBL
    {
        private readonly ForgetPassDAC _forgetPassDAC;
        private readonly EmailSender _emailSender;

        // コンストラクタ：DB接続とメール送信の設定を初期化
        public ForgetPassBL(string connectionString, IConfiguration configuration)
        {
            _forgetPassDAC = new ForgetPassDAC(connectionString);

            // appsettings.json からメール設定を読み込む
            var emailConfig = configuration.GetSection("EmailSettings");
            var smtpHost = emailConfig["SmtpHost"];
            var smtpPort = int.Parse(emailConfig["SmtpPort"]);
            var fromEmail = emailConfig["FromEmail"];
            var fromPassword = emailConfig["FromPassword"];

            // メール送信クラスの初期化
            _emailSender = new EmailSender(smtpHost, smtpPort, fromEmail, fromPassword);
        }

        // メールアドレスを元にパスワードをリセットし、メールで通知する
        public string ResetPasswordByEmail(string email)
        {
            // ユーザー情報を取得
            var user = _forgetPassDAC.GetUserByEmail(email);
            if (user == null)
                return "該当するユーザーが見つかりませんでした。";

            // ランダムな仮パスワードを生成
            var plainPassword = GenerateRandomPassword(8);

            // パスワードの更新（※暗号化処理は必要に応じて）
            string hashedPassword = HashPasswordSHA256(plainPassword);
            _forgetPassDAC.UpdateUserPassword(user.UserId, hashedPassword);

            try
            {
                // メール送信内容の作成
                var subject = "【提案システム】パスワード再設定";
                var body = $"ユーザー {user.UserName} 様\n\n新しいパスワード: {plainPassword}\n\nログイン後、すぐにパスワードを変更してください。";

                // メール送信
                _emailSender.Send(user.UserEmail, subject, body);

                // 这里返回特定成功标志字符串
                return "Success";
            }
            catch (Exception ex)
            {
                return "メールの送信に失敗しました：" + ex.Message;
            }
        }

        // ランダムなパスワードを生成
        public string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var data = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);

            var result = new char[length];
            for (int i = 0; i < length; i++)
                result[i] = chars[data[i] % chars.Length];

            return new string(result);
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
