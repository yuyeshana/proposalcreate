using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Proposal.BL;
using Proposal.Models;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

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
            var connStr = _configuration.GetConnectionString("ProposalDB");
            _loginBL = new LoginBL(connStr);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// ログイン画面を表示します。既にログインしている場合は適切なページへリダイレクトされます。
        /// </summary>
        /// <returns>
        /// ログインしていない場合はログイン画面を返します。<br/>
        /// ログイン済みかつパスワード未設定の場合は「ChangePassword」画面へリダイレクトします。<br/>
        /// それ以外の場合は「Menu」画面へリダイレクトします。
        /// </returns>
        [HttpGet]
        public IActionResult Login()
        {
            
            var userId = HttpContext.Session.GetString("UserId");
            var SetPass = HttpContext.Session.GetString("SetPass");

            if (!string.IsNullOrEmpty(userId))
            {
                if (SetPass == "1")
                {
                     return RedirectToAction("Menu");
                }
                return RedirectToAction("ChangePassword");
            }
            return View(new LoginModel());
        }

        /// <summary>
        /// ログイン処理を実行します。
        /// <returns>
        /// 入力エラーがある場合はログイン画面を再表示します。<br/>
        /// ユーザー認証に失敗した場合もログイン画面を再表示します。<br/>
        /// 認証成功後、パスワード未設定の場合は「ChangePassword」画面に遷移し、<br/>
        /// それ以外の場合は「Menu」画面に遷移します。
        /// </returns>
        [HttpPost]
        public IActionResult Login(LoginModel model, string action)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _loginBL.ValidateUser(model);
            if (user == null)
            {
                ViewBag.Error = "ユーザーIDまたはパスワードが間違っています。";
                return View(model);
            }
            // 登录成功，设置 session
            HttpContext.Session.SetString("UserId", user.UserId);
            HttpContext.Session.SetString("SetPass", user.Registration_status ? "1" : "0");

            if (!user.Registration_status)
            {
                return RedirectToAction("ChangePassword");
            }
            return RedirectToAction("Menu");
        }




        [Route("proposal/menu")]
        public IActionResult Menu()
        {
            // 检查是否已登录
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                // 未登录，跳转到登录页面
                return RedirectToAction("Login");
            }

            ViewBag.UserKbn = HttpContext.Session.GetString("UserKbn");
            return View("~/Views/ForgetPass/ChangePassword.cshtml");
        }
    }
}
