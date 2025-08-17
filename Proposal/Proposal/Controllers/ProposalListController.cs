using Microsoft.AspNetCore.Mvc;
using Proposal.BL;
using Proposal.Models;
using System.Linq;

namespace Proposal.Controllers
{
    public class ProposalListController : Controller
    {
        private readonly string _connectionString;

        public ProposalListController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ProposalDB");
        }

        [HttpGet]
        public IActionResult Index(string? year, int? status, DateTime? dateFrom, DateTime? dateTo, int page = 1)
        {
            const int pageSize = 10;
            var bl = new ProposalListBL(_connectionString);
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //提案書一覧を取得
            var result = bl.GetProposalList(userId, year, status, dateFrom, dateTo, page, pageSize);

            ViewBag.Page = page;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedYear = year;
            ViewBag.SelectedStatus = status;
            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");
            //提案年度一覧を取得
            ViewBag.Years = result.Items
                .Select(p => p.ProposalYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();
            //提案状態一覧を取得
            ViewBag.Statuses = bl.GetProposalStatuses();


            return View("ProposalList", result.Items);
        }

        /// <summary>
        /// 提案書作成画面に遷移する
        /// </summary>
        /// <param name="id">提案書ID</param>
        /// <returns>提案書作成画面</returns>
        public IActionResult showCreate(string id)
        {
            return RedirectToAction("Index", "Proposal", new { proposalid = id });
        }
    }


}
