using Proposal.DAC;
using Proposal.Models;
using System.Security.Cryptography;
using System.Text;

namespace Proposal.BL
{
    public class LoginBL
    {
        private readonly LoginDAC _LoginDAC;

        public LoginBL(string connectionString)
        {
            _LoginDAC = new LoginDAC(connectionString);
        }

        /// <summary>
        /// 验证用户账号密码
        /// </summary>
        /// <returns>登录成功返回用户对象，否则返回 null</returns>
        public User ValidateUser(LoginModel pModel)
        {
            var user = _LoginDAC.GetUserById(pModel);
            if (user != null)
            {
                // 把输入的密码做哈希
                string hashedInputPassword = HashPasswordSHA256(pModel.Password);
                // 和数据库存的哈希密码比对
                if (user.Password == hashedInputPassword)
                {
                    return user;
                }
            }
            return null;
        }

        public string HashPasswordSHA256(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
