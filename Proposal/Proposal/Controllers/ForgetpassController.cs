using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Proposal.BL;
using Proposal.Models;
using Microsoft.AspNetCore.Http;

namespace Proposal.Controllers
{
    public class ForgetPassController : Controller
    {
        private readonly ILogger<ForgetPassController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ForgetPassBL _forgetPassBL;
        private readonly string _connectionString;

        // コンストラクタでロガー、設定ファイル、接続文字列などを初期化
        public ForgetPassController(ILogger<ForgetPassController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // appsettings.json から接続文字列を取得
            _connectionString = _configuration.GetConnectionString("ProposalDB");

            // BL（ビジネスロジック）層に接続文字列と設定を渡す
            _forgetPassBL = new ForgetPassBL(_connectionString, _configuration);
        }

        // パスワード忘れ画面の表示
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View(new ForgetPassModel());
        }

        // 入力されたメールアドレス宛にリセットリンク（仮パスワード）を送信
        [HttpPost]
        public IActionResult SendResetLink(ForgetPassModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ForgetPassword", model);
            }

            var resultMessage = _forgetPassBL.ResetPasswordByEmail(model.Email);

            if (resultMessage == "Success") 
            {
                ViewBag.ResetSuccess = true;
            }
            else
            {
                ViewBag.Message = resultMessage;
            }

            return View("ForgetPassword", model);
        }

    }
}
