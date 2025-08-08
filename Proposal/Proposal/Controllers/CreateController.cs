using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Extensions.Configuration;
using Proposal.BL;
using Proposal.DAC;
using Proposal.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;


namespace Proposal.Controllers
{
    public class CreateController : Controller
    {
        private readonly CreateBL _createBL = new CreateBL();
        private readonly IConfiguration _configuration;

        public CreateController(ILogger<LoginController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            var connStr = _configuration.GetConnectionString("ProposalDB");
            _createBL = new CreateBL(connStr);
        }

        [HttpGet]
        [Route("proposal/Create")]
        public IActionResult Create()
        {
            // すべてのドロップダウンリストをBL層の共通メソッドで取得
            ViewBag.Dropdowns = _createBL.GetDropdowns();

            var viewModel = new CreateViewModel();
            //提案書ID
            viewModel.BasicInfo.Id = HttpContext.Session.GetString("Id");

            //ユーザーID取得
            viewModel.BasicInfo.UserId = HttpContext.Session.GetString("UserId");
            viewModel.ProposalContent.Id = viewModel.BasicInfo.Id;
            viewModel.ProposalContent.UserId = viewModel.BasicInfo.UserId;

            //修正・確認の場合
            if (!String.IsNullOrEmpty(viewModel.BasicInfo.Id))
            {
                //提案書情報取得
                _createBL.GetProposalDetailById(viewModel.BasicInfo, viewModel.ProposalContent);
            }

            //新規又は一時保存の場合
            if(viewModel.BasicInfo.Status == null || viewModel.BasicInfo.Status == 1)
            {
                //年度取得（新規作成時のみ）
                if (String.IsNullOrEmpty(viewModel.BasicInfo.Id))
                {
                    viewModel.BasicInfo.TeianYear = DateTime.Now.ToString("ggy年", new CultureInfo("ja-JP") { DateTimeFormat = { Calendar = new JapaneseCalendar() } });
                }

                //所属、部・署、課・部門、係・担当取得
                if (!String.IsNullOrEmpty(viewModel.BasicInfo.UserId))
                {
                    //ユーザー情報取得
                    _createBL.GetUserInfoByUserId(viewModel.BasicInfo);
                }
            }
            else
            {
                // 編集時でもユーザー情報を取得
                if (!String.IsNullOrEmpty(viewModel.BasicInfo.UserId))
                {
                    _createBL.GetUserInfoByUserId(viewModel.BasicInfo);
                }
            }

            // GET：新規作成時は GroupMembers を3人に補完し、編集時は group_info から実際のメンバー数を取得する
            if (viewModel.BasicInfo.GroupMembers == null || viewModel.BasicInfo.GroupMembers.Count < 3)
            {
                int count = Math.Max(viewModel.BasicInfo.GroupMembers?.Count ?? 0, 3);
                while (viewModel.BasicInfo.GroupMembers.Count < count) viewModel.BasicInfo.GroupMembers.Add(new GroupMemberModel());
            }

            return View(viewModel);
        }

        [HttpPost]
        [Route("proposal/Create")]
        public IActionResult Create(CreateViewModel viewModel, string action)
        {
            var model = new CreateModel
            {
                // 基本情報
                Id = viewModel.BasicInfo.Id,
                UserId = viewModel.BasicInfo.UserId,
                TeianYear = viewModel.BasicInfo.TeianYear,
                TeianDaimei = viewModel.BasicInfo.TeianDaimei,
                ProposalTypeId = viewModel.BasicInfo.ProposalTypeId,
                ProposalKbnId = viewModel.BasicInfo.ProposalKbnId,
                AffiliationId = viewModel.BasicInfo.AffiliationId,
                DepartmentId = viewModel.BasicInfo.DepartmentId,
                SectionId = viewModel.BasicInfo.SectionId,
                SubsectionId = viewModel.BasicInfo.SubsectionId,
                AffiliationName = viewModel.BasicInfo.AffiliationName,
                DepartmentName = viewModel.BasicInfo.DepartmentName,
                SectionName = viewModel.BasicInfo.SectionName,
                SubsectionName = viewModel.BasicInfo.SubsectionName,
                ShimeiOrDaihyoumei = viewModel.BasicInfo.ShimeiOrDaihyoumei,
                GroupMei = viewModel.BasicInfo.GroupMei,
                GroupMembers = viewModel.BasicInfo.GroupMembers,
                SkipFirstReviewer = viewModel.BasicInfo.SkipFirstReviewer,
                FirstReviewerAffiliationId = viewModel.BasicInfo.FirstReviewerAffiliationId,
                FirstReviewerDepartmentId = viewModel.BasicInfo.FirstReviewerDepartmentId,
                FirstReviewerSectionId = viewModel.BasicInfo.FirstReviewerSectionId,
                FirstReviewerSubsectionId = viewModel.BasicInfo.FirstReviewerSubsectionId,
                FirstReviewerName = viewModel.BasicInfo.FirstReviewerName,
                FirstReviewerTitle = viewModel.BasicInfo.FirstReviewerTitle,
                EvaluationSectionId = viewModel.BasicInfo.EvaluationSectionId,
                ResponsibleSectionId1 = viewModel.BasicInfo.ResponsibleSectionId1,
                ResponsibleSectionId2 = viewModel.BasicInfo.ResponsibleSectionId2,
                ResponsibleSectionId3 = viewModel.BasicInfo.ResponsibleSectionId3,
                ResponsibleSectionId4 = viewModel.BasicInfo.ResponsibleSectionId4,
                ResponsibleSectionId5 = viewModel.BasicInfo.ResponsibleSectionId5,
                Status = viewModel.BasicInfo.Status,
                Createddate = viewModel.BasicInfo.Createddate,
                Submissiondate = viewModel.BasicInfo.Submissiondate
            };

            //提案内容
            var proposalContent = new ProposalContentModel
            {
                Id = viewModel.ProposalContent.Id,
                UserId = viewModel.ProposalContent.UserId,
                GenjyoMondaiten = viewModel.ProposalContent.GenjyoMondaiten,
                Kaizenan = viewModel.ProposalContent.Kaizenan,
                KoukaJishi = viewModel.ProposalContent.KoukaJishi,
                Kouka = viewModel.ProposalContent.Kouka,
                TenpuFile1 = viewModel.ProposalContent.TenpuFile1,
                TenpuFile2 = viewModel.ProposalContent.TenpuFile2,
                TenpuFile3 = viewModel.ProposalContent.TenpuFile3,
                TenpuFile4 = viewModel.ProposalContent.TenpuFile4,
                TenpuFile5 = viewModel.ProposalContent.TenpuFile5,
                TenpuFileName1 = viewModel.ProposalContent.TenpuFileName1,
                TenpuFileName2 = viewModel.ProposalContent.TenpuFileName2,
                TenpuFileName3 = viewModel.ProposalContent.TenpuFileName3,
                TenpuFileName4 = viewModel.ProposalContent.TenpuFileName4,
                TenpuFileName5 = viewModel.ProposalContent.TenpuFileName5
            };

            //戻る
            if (action == "Menu")
            {
                return View("~/Views/Menu/Menu.cshtml");
            }

            //一時保存
            if (action == "Ichijihozon")
            {
                if (!ValidateModel(model) || !ValidateProposalContent(proposalContent))
                {
                    SetDropdowns();
                    SetShowProposalContentFlagForModelStateError();
                    PreserveUserInputData(viewModel, model, proposalContent);
                    return View(viewModel);
                }

                // ファイル保存処理（最大5つ）
                SaveUploadedFiles(proposalContent);

                //提案状態を作成中に設定
                model.Status = 1;

                //登録又は更新処理
                this.InsertOrUpdate(model, proposalContent);

                return RedirectToAction("ProposalList", "ProposalList");
            }

            //出力
            if (action == "Deryoku")
            {
                if (!ValidateModel(model) || !ValidateProposalContent(proposalContent))
                {
                    SetDropdowns();
                    SetShowProposalContentFlagForModelStateError();
                    PreserveUserInputData(viewModel, model, proposalContent);
                    return View(viewModel);
                }

                var dropdowns = (Proposal.BL.DropdownsViewModel)ViewBag.Dropdowns;
                string proposalTypeName = dropdowns.ProposalTypes.FirstOrDefault(x => x.Value == model.ProposalTypeId)?.Text ?? "";
                string firstReviewerAffiliation = dropdowns.Affiliations.FirstOrDefault(x => x.Value == model.FirstReviewerAffiliationId)?.Text ?? "";
                string firstReviewerDepartment = dropdowns.Departments.FirstOrDefault(x => x.Value == model.FirstReviewerDepartmentId)?.Text ?? "";
                string firstReviewerSection = dropdowns.Sections.FirstOrDefault(x => x.Value == model.FirstReviewerSectionId)?.Text ?? "";

                var csv = new StringBuilder();
                csv.AppendLine("提案年度,提案題名,提案の種類,提案の区分,氏名又は代表名,グループ名,第一次審査者所属,第一次審査者部・署,第一次審査者課・部門,第一次審査者氏名,第一次審査者官職,主務課,関係課,現状・問題点,改善案,効果の種類,効果");
                string koukaJishiText = proposalContent.KoukaJishi.HasValue ? (proposalContent.KoukaJishi.Value.GetType().GetField(proposalContent.KoukaJishi.Value.ToString()).GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description ?? proposalContent.KoukaJishi.Value.ToString()) : "";
                csv.AppendLine($"\"{model.TeianYear}\",\"{model.TeianDaimei}\",\"{proposalTypeName}\",\"{model.ProposalKbnId}\",\"{model.ShimeiOrDaihyoumei}\",\"{model.GroupMei}\",\"{firstReviewerAffiliation}\",\"{firstReviewerDepartment}\",\"{firstReviewerSection}\",\"{model.FirstReviewerName}\",\"{model.FirstReviewerTitle}\",\"{model.EvaluationSectionId}\",\"{model.ResponsibleSectionId1}\",\"{proposalContent.GenjyoMondaiten}\",\"{proposalContent.Kaizenan}\",\"{koukaJishiText}\",\"{proposalContent.Kouka}\"");

                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                var filename = $"提案内容_{DateTime.Now:yyyyMMddHHmmss}.csv";
                return File(bytes, "text/csv", filename);
            }

            //提出
            if (action == "Submit")
            {
                if (!ValidateModel(model) || !ValidateProposalContent(proposalContent))
                {
                    SetDropdowns();
                    SetShowProposalContentFlagForModelStateError();
                    PreserveUserInputData(viewModel, model, proposalContent);
                }

                // ファイル保存処理（最大5つ）
                SaveUploadedFiles(proposalContent);

                //提案状態を審査中に設定
                model.Status = 11;

                //登録又は更新処理
                this.InsertOrUpdate(model, proposalContent);

                return RedirectToAction("ProposalList", "ProposalList");
            }

            // POST：バリデーションエラー时に GroupMembers を3人に补完し、保存时に BL.SaveGroupInfo を呼び出す
            if (!ModelState.IsValid) {
                // 确保 model 和 GroupMembers 不为空
                if (model != null && model.GroupMembers != null)
                {
                    int count = Math.Max(model.GroupMembers.Count, 3);
                    while (model.GroupMembers.Count < count) model.GroupMembers.Add(new GroupMemberModel());
                }
                
                PreserveUserInputData(viewModel, model, proposalContent);
                
                // 设置下拉列表
                SetDropdowns();
                SetShowProposalContentFlagForModelStateError();
                
                return View(viewModel);
            }

            return View(viewModel);
        }

        // ドロップダウンリストをViewBagにセット
        private void SetDropdowns()
        {
            ViewBag.Dropdowns = _createBL.GetDropdowns();
        }

        /// <summary>
        /// 保留用户输入的数据，只确保必要的属性不为空
        /// </summary>
        private void PreserveUserInputData(CreateViewModel viewModel, CreateModel model, ProposalContentModel proposalContent)
        {
            // 确保 viewModel 不为空
            if (viewModel == null)
            {
                viewModel = new CreateViewModel();
            }
            if (viewModel.BasicInfo == null)
            {
                viewModel.BasicInfo = new CreateModel();
            }
            if (viewModel.ProposalContent == null)
            {
                viewModel.ProposalContent = new ProposalContentModel();
            }

            // 确保 GroupMembers 不为空
            if (viewModel.BasicInfo.GroupMembers == null)
            {
                viewModel.BasicInfo.GroupMembers = new List<GroupMemberModel>();
            }

            // 将验证后的数据重新赋值给 viewModel，保留用户输入
            viewModel.BasicInfo.Id = model.Id;
            viewModel.BasicInfo.UserId = model.UserId;
            viewModel.BasicInfo.TeianYear = model.TeianYear;
            viewModel.BasicInfo.TeianDaimei = model.TeianDaimei;
            viewModel.BasicInfo.ProposalTypeId = model.ProposalTypeId;
            viewModel.BasicInfo.ProposalKbnId = model.ProposalKbnId;
            viewModel.BasicInfo.AffiliationId = model.AffiliationId;
            viewModel.BasicInfo.DepartmentId = model.DepartmentId;
            viewModel.BasicInfo.SectionId = model.SectionId;
            viewModel.BasicInfo.SubsectionId = model.SubsectionId;
            viewModel.BasicInfo.AffiliationName = model.AffiliationName;
            viewModel.BasicInfo.DepartmentName = model.DepartmentName;
            viewModel.BasicInfo.SectionName = model.SectionName;
            viewModel.BasicInfo.SubsectionName = model.SubsectionName;
            viewModel.BasicInfo.ShimeiOrDaihyoumei = model.ShimeiOrDaihyoumei;
            viewModel.BasicInfo.GroupMei = model.GroupMei;
            viewModel.BasicInfo.GroupMembers = model.GroupMembers;
            viewModel.BasicInfo.SkipFirstReviewer = model.SkipFirstReviewer;
            viewModel.BasicInfo.FirstReviewerAffiliationId = model.FirstReviewerAffiliationId;
            viewModel.BasicInfo.FirstReviewerDepartmentId = model.FirstReviewerDepartmentId;
            viewModel.BasicInfo.FirstReviewerSectionId = model.FirstReviewerSectionId;
            viewModel.BasicInfo.FirstReviewerSubsectionId = model.FirstReviewerSubsectionId;
            viewModel.BasicInfo.FirstReviewerName = model.FirstReviewerName;
            viewModel.BasicInfo.FirstReviewerTitle = model.FirstReviewerTitle;
            viewModel.BasicInfo.EvaluationSectionId = model.EvaluationSectionId;
            viewModel.BasicInfo.ResponsibleSectionId1 = model.ResponsibleSectionId1;
            viewModel.BasicInfo.ResponsibleSectionId2 = model.ResponsibleSectionId2;
            viewModel.BasicInfo.ResponsibleSectionId3 = model.ResponsibleSectionId3;
            viewModel.BasicInfo.ResponsibleSectionId4 = model.ResponsibleSectionId4;
            viewModel.BasicInfo.ResponsibleSectionId5 = model.ResponsibleSectionId5;
            viewModel.BasicInfo.Status = model.Status;
            viewModel.BasicInfo.Createddate = model.Createddate;
            viewModel.BasicInfo.Submissiondate = model.Submissiondate;

            viewModel.ProposalContent.Id = proposalContent.Id;
            viewModel.ProposalContent.UserId = proposalContent.UserId;
            viewModel.ProposalContent.GenjyoMondaiten = proposalContent.GenjyoMondaiten;
            viewModel.ProposalContent.Kaizenan = proposalContent.Kaizenan;
            viewModel.ProposalContent.KoukaJishi = proposalContent.KoukaJishi;
            viewModel.ProposalContent.Kouka = proposalContent.Kouka;
            viewModel.ProposalContent.TenpuFile1 = proposalContent.TenpuFile1;
            viewModel.ProposalContent.TenpuFile2 = proposalContent.TenpuFile2;
            viewModel.ProposalContent.TenpuFile3 = proposalContent.TenpuFile3;
            viewModel.ProposalContent.TenpuFile4 = proposalContent.TenpuFile4;
            viewModel.ProposalContent.TenpuFile5 = proposalContent.TenpuFile5;
            viewModel.ProposalContent.TenpuFileName1 = proposalContent.TenpuFileName1;
            viewModel.ProposalContent.TenpuFileName2 = proposalContent.TenpuFileName2;
            viewModel.ProposalContent.TenpuFileName3 = proposalContent.TenpuFileName3;
            viewModel.ProposalContent.TenpuFileName4 = proposalContent.TenpuFileName4;
            viewModel.ProposalContent.TenpuFileName5 = proposalContent.TenpuFileName5;
        }

        // ファイル保存処理（最大5つ）
        private void SaveUploadedFiles(ProposalContentModel proposalContent)
        {
            // 确保 proposalContent 不为空
            if (proposalContent == null)
            {
                return;
            }
            
            for (int i = 1; i <= 5; i++)
            {
                var fileProp = proposalContent.GetType().GetProperty($"TenpuFile{i}");
                var file = fileProp?.GetValue(proposalContent) as IFormFile;
                if (file != null && file.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "Proposal/wwwroot/uploads");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploads, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    // ファイル名をモデルにセット
                    var nameProp = proposalContent.GetType().GetProperty($"TenpuFileName{i}");
                    nameProp?.SetValue(proposalContent, fileName);
                }
            }
        }

        // モデルバリデーション実行
        private bool ValidateModel(CreateModel model)
        {
            // 确保 model 不为空
            if (model == null)
            {
                return false;
            }
            
            // カスタムバリデーションを明示的に実行
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            
            // カスタムバリデーション結果をModelStateに追加
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                }
            }
            
            // モデルバリデーション実行
            return ModelState.IsValid;
        }

        /// <summary>
        /// ModelStateに提案内容タブのエラーがある場合、ViewBag.ShowProposalContentをtrueにする
        /// </summary>
        private void SetShowProposalContentFlagForModelStateError()
        {
            // 确保 ModelState 不为空
            if (ModelState == null)
            {
                ViewBag.ShowProposalContent = false;
                return;
            }
            
            // 提案内容tab相关字段
            var teianNaiyouFields = new[] { "GenjyoMondaiten", "Kaizenan", "KoukaJishi", "Kouka", "TenpuFile1", "TenpuFile2", "TenpuFile3", "TenpuFile4", "TenpuFile5", "TenpuFileName1", "TenpuFileName2", "TenpuFileName3", "TenpuFileName4", "TenpuFileName5", "TenpuFilePath1", "TenpuFilePath2", "TenpuFilePath3", "TenpuFilePath4", "TenpuFilePath5" };
            // 只有提案内容有错时才切到提案内容Tab，否则停留在基本情報Tab
            bool hasTeianNaiyouError = ModelState.Keys.Any(k => teianNaiyouFields.Any(f => k.Contains(f)) && ModelState[k].Errors.Count > 0);
            ViewBag.ShowProposalContent = hasTeianNaiyouError;
        }

        // ProposalContentModelの検証メソッドを追加
        private bool ValidateProposalContent(ProposalContentModel proposalContent)
        {
            // 确保 proposalContent 不为空
            if (proposalContent == null)
            {
                return false;
            }
            
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(proposalContent);
            Validator.TryValidateObject(proposalContent, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                }
            }
            return ModelState.IsValid;
        }

        //登録と更新処理
        public void InsertOrUpdate(CreateModel model, ProposalContentModel proposalContent)
        {
            // 确保 model 和 proposalContent 不为空
            if (model == null || proposalContent == null)
            {
                return;
            }
            
            //ユーザーID
            model.UserId = HttpContext.Session.GetString("UserId");
            //登録と更新処理判断
            if (string.IsNullOrEmpty(model.Id))
            {
                //登録処理
                //提案書詳細登録
                model.Id = _createBL.Insertproposals_detail(model, proposalContent).ToString();

                //グループデータを登録（区分がグループの時のみ）
                if (model.ProposalKbnId == "2")
                {
                    _createBL.InsertGroupInfo(model);
                }
            }
            else
            {
                //更新処理
                //提案書詳細更新
                _createBL.Updateproposals_detail(model, proposalContent);

                //グループデータを登録（区分がグループの時のみ）
                if (model.ProposalKbnId == "2")
                {
                    //グループデータを削除
                    _createBL.DeleteGroupInfo(model.Id);

                    //グループデータを登録
                    _createBL.InsertGroupInfo(model);
                }
            }
        }

        /// <summary>
        /// 获取所有组织架构数据
        /// </summary>
        [HttpGet]
        public JsonResult GetAllOrganizations()
        {
            var organizations = _createBL.GetAllOrganizations();
            return Json(organizations);
        }
    }
}
