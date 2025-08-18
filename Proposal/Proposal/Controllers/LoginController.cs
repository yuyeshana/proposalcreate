using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Proposal.BL;
using Proposal.Models;

namespace Proposal.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration _configuration;
        private readonly LoginBL _loginBL;

        public LoginController(ILogger<LoginController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            var connStr = _configuration.GetConnectionString("ProposalDB") 
                ?? throw new InvalidOperationException("ProposalDB connection string not found");
            _loginBL = new LoginBL(connStr);
        }

        /// <summary>
        /// ログイン画面を表示する
        /// </summary>
        /// <returns>ログイン画面</returns>
        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var setPass = HttpContext.Session.GetString("SetPass");

            if (!string.IsNullOrEmpty(userId))
            {
                if (setPass == "1")
                {
                    return RedirectToAction("Index", "ProposalList");
                }
                return RedirectToAction("ChangePassword");
            }
            return View("Login", new LoginModel());
        }

        /// <summary>
        /// ログイン処理を実行します。
        /// </summary>
        /// <param name="model">ログインモデル</param>
        /// <param name="action">アクション名</param>
        /// <returns>
        /// 入力エラーがある場合はログイン画面を再表示します。<br/>
        /// ユーザー認証に失敗した場合もログイン画面を再表示します。<br/>
        /// 認証成功後、パスワード未設定の場合は「ChangePassword」画面に遷移し、<br/>
        /// それ以外の場合は「Menu」画面に遷移します。
        /// </returns>
        [HttpPost]
        public IActionResult Index(LoginModel model, string action)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            var user = _loginBL.ValidateUser(model);
            if (user == null)
            {
                ViewBag.Error = "ユーザーIDまたはパスワードが間違っています。";
                return View("Login", model);
            }

            // ログイン成功、セッションを設定
            HttpContext.Session.SetString("UserId", user.UserId);
            HttpContext.Session.SetString("SetPass", user.Registration_status ? "1" : "0");

            if (!user.Registration_status)
            {
                return RedirectToAction("ChangePassword");
            }
            return RedirectToAction("Index", "proposalList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Logout")]
        public IActionResult Logout_Post()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");
            return RedirectToAction(nameof(Index));
        }
    }
}
