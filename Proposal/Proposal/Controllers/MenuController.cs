using Microsoft.AspNetCore.Mvc;

namespace Proposal.Controllers
{
    public class MenuController : Controller
    {

        //ログアウト
        [HttpGet]
        [Route("proposal/Menu")]
        public IActionResult Menu()
        {
            //提案書IDセッション削除
            HttpContext.Session.Remove("Id");
            return View("~/Views/Menu/Menu.cshtml");
        }

        //ログアウト
        [HttpGet]
        [Route("proposal/logout")]
        public IActionResult Logout()
        {
            // 清除session
            HttpContext.Session.Clear();
            return View("~/Views/Login/Login.cshtml");
        }

        ////申請書一覧
        //[Route("proposal/list")]
        //public IActionResult List()
        //{
        //    return View("~/Views/List/List.cshtml");
        //}

        //申請書作成
        [Route("proposal/create")]
        public IActionResult Create ()
        {
            // 返回列表视图
            return View("~/Views/Create/Create.cshtml");
        }

        //一次審査入力
        [Route("proposal/ichijishinnsanyuryoku")]
        public IActionResult Ichijishinnsanyuryoku()
        {
            // 返回列表视图
            return View("~/Views/Ichijishinnsanyuryoku/Ichijishinnsanyuryoku.cshtml");
        }

        //二次審査入力
        [Route("proposal/nichijishinnsanyuryoku")]
        public IActionResult Nichijishinnsanyuryoku()
        {
            // 返回列表视图
            return View("~/Views/Nichijishinnsanyuryoku/Nichijishinnsanyuryoku.cshtml");
        }

        //各種設定・メンテナンス
        [Route("proposal/kakusyusettei")]
        public IActionResult Kakusyusettei()
        {
            // 返回列表视图
            return View("~/Views/Kakusyusettei/Kakusyusettei.cshtml");
        }

        //三次審査入力
        [Route("proposal/sanjishinnsanyuryoku")]
        public IActionResult Sanjishinnsanyuryoku()
        {
            // 返回列表视图
            return View("~/Views/Sanjishinnsanyuryoku/Sanjishinnsanyuryoku.cshtml");
        }

        //過去事情の検索・照会
        [Route("proposal/kensaku")]
        public IActionResult Kensaku()
        {
            // 返回列表视图
            return View("~/Views/Kensaku/Kensaku.cshtml");
        }
    }
}
