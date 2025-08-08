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
        public IActionResult ProposalList(int? year, int? status, int page = 1)
        {
            const int pageSize = 10;
            var bl = new ProposalListBL(_connectionString);
            var result = bl.GetProposalList(year, status, page, pageSize);

            ViewBag.Page = page;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedYear = year;
            ViewBag.SelectedStatus = status;
            ViewBag.Years = result.Items
                .Select(p => p.ProposalYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();
            ViewBag.Statuses = bl.GetProposalStatuses();


            return View("~/Views/ProposalList/ProposalList.cshtml", result.Items);
        }

        [HttpGet("setid")]
        public IActionResult SetIdAndGoCreate(string id)
        {
            HttpContext.Session.SetString("Id", id);
            return RedirectToAction("Create", "Create");
        }
    }


}
