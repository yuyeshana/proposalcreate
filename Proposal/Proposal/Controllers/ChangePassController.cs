using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Proposal.BL;
using Proposal.Models;
using Microsoft.AspNetCore.Http;

namespace Proposal.Controllers
{
    public class ChangePassController : Controller
    {
        private readonly ILogger<ForgetPassController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ChangePassBL _changePassBL;
        private readonly string _connectionString;

        // コンストラクタでロガー、設定ファイル、接続文字列などを初期化
        public ChangePassController(ILogger<ForgetPassController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // appsettings.json から接続文字列を取得
            _connectionString = _configuration.GetConnectionString("ProposalDB");

            // BL（ビジネスロジック）層に接続文字列と設定を渡す
            _changePassBL = new ChangePassBL(_connectionString, _configuration);
        }

        // パスワード変更画面の表示
        [HttpGet]
        [Route("/Login/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View("ChangePassword");
        }

        // 新しいパスワードの更新処理
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePass(ChangePassModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Login");
            }

            // モデルバリデーション失敗 → エラー表示
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ビジネスロジック実行
            var success = _changePassBL.ChangeUserPassword(userId, model.NewPassword, model.ConfirmPassword, out string error);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error); // 共通エラーとして表示
                return View(model);
            }

            // セッション更新
            HttpContext.Session.SetString("SetPass", "1");

            // ログインへリダイレクト（メッセージ付き）
            TempData["Message"] = "パスワードが変更されました。ログインしてください。";
            return RedirectToAction("Index", "Login");
        }
    }
}
