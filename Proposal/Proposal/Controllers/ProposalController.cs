using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Proposal.BL;
using Proposal.DAC;
using Proposal.Models;
using Proposal.Services;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Proposal.Controllers
{
    /// <summary>
    /// 提案書作成・編集機能を提供するコントローラー
    /// </summary>
    public class ProposalController : Controller
    {
        // フィールド
        private readonly ProposalBL _createBL;
        private readonly IConfiguration _configuration;
        private readonly ProposalValidator _proposalValidator;
        private ProposalViewModel _viewModel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="configuration">アプリケーション設定</param>
        /// <param name="proposalValidator">提案書バリデーター</param>
        public ProposalController(ILogger<ProposalController> logger, IConfiguration configuration, ProposalValidator proposalValidator)
        {
            _configuration = configuration;
            _proposalValidator = proposalValidator;
            var connectionString = _configuration.GetConnectionString("ProposalDB");
            _createBL = new ProposalBL(connectionString);
            _viewModel = new ProposalViewModel();
        }

        /// <summary>
        /// 提案書作成・編集画面を表示します（GET）
        /// </summary>
        /// <param name="proposalid">提案書ID（編集時のみ）</param>
        /// <returns>提案書作成・編集画面</returns>
        [HttpGet]
        public IActionResult Index(string proposalid)
        {
            // セッションからユーザーIDを取得
            string userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Login");
            }

            // ドロップダウンリストを取得してViewBagに設定
            ViewBag.Dropdowns = _createBL.GetDropdowns();

            // ビューモデルを初期化
            _viewModel = new ProposalViewModel();

            // 提案書IDとユーザーIDを設定
            _viewModel.BasicInfo.ProposalId = proposalid;
            _viewModel.BasicInfo.UserId = userId;
            _viewModel.ProposalContent.ProposalId = proposalid;
            _viewModel.ProposalContent.UserId = userId;


            // 編集・確認の場合
            if (!string.IsNullOrEmpty(_viewModel.BasicInfo.ProposalId))
            {
                // 提案書の詳細情報を取得
                _createBL.GetProposalDetailById(_viewModel);

            }
            // 新規作成の場合
            else
            {
                _viewModel.BasicInfo.TeianYear = DateTime.Now.ToString("ggy年", new CultureInfo("ja-JP") { DateTimeFormat = { Calendar = new JapaneseCalendar() } });

                // 提案者情報を取得
                _createBL.GetUserInfoByUserId(_viewModel.BasicInfo);
            }

            return View("Proposal", _viewModel);
        }

        /// <summary>
        /// 所属IDに基づいて部・署リストを取得するAPI
        /// </summary>
        /// <param name="affiliationId">所属ID</param>
        /// <returns>部・署のSelectListItemリスト</returns>
        [HttpGet]
        public JsonResult GetDepartmentsByAffiliation(string affiliationId)
        {
            try
            {
                var departments = _createBL.GetDepartmentsByAffiliation(affiliationId);
                return Json(departments);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 部・署IDに基づいて課・部門リストを取得するAPI
        /// </summary>
        /// <param name="departmentId">部・署ID</param>
        /// <returns>課・部門のSelectListItemリスト</returns>
        [HttpGet]
        public JsonResult GetSectionsByDepartment(string departmentId)
        {
            try
            {
                var sections = _createBL.GetSectionsByDepartment(departmentId);
                return Json(sections);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 課・部門IDに基づいて係・担当リストを取得するAPI
        /// </summary>
        /// <param name="sectionId">課・部門ID</param>
        /// <returns>係・担当のSelectListItemリスト</returns>
        [HttpGet]
        public JsonResult GetSubsectionsBySection(string sectionId)
        {
            try
            {
                var subsections = _createBL.GetSubsectionsBySection(sectionId);
                return Json(subsections);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }



        [HttpPost]
        public IActionResult Index(ProposalViewModel viewModel, string action)
        {
            // アクションに応じた処理を実行
            switch (action)
            {
                //一時保存
                case "TemporarySave":
                    return HandleTemporarySave(viewModel);

                //CSV出力
                case "Output":
                //return HandleExport(basicInfoModel, proposalContentModel, viewModel);

                //提出
                case "Submit":
                //return HandleSubmit(basicInfoModel, proposalContentModel, viewModel);

                default:
                    // バリデーションエラー時の処理
                    if (!ModelState.IsValid)
                    {
                        //return HandleValidationError(basicInfoModel, proposalContentModel, viewModel);
                    }
                    return View("~/Views/Proposal/Proposal.cshtml", viewModel);
            }
        }

        /// <summary>
        /// 一時保存処理を実行します
        /// </summary>
        /// <param name="basicInfoModel">基本情報モデル</param>
        /// <param name="proposalContentModel">提案内容モデル</param>
        /// <param name="viewModel">ビューモデル</param>
        /// <returns>処理結果</returns>
        private IActionResult HandleTemporarySave(ProposalViewModel viewModel)
        {
            // バリデーション実行
            // if (!ValidateModel(basicInfoModel) || !ValidateProposalContent(proposalContentModel))
            // {
            //     SetDropdowns();
            //     SetShowProposalContentFlagForModelStateError();
            //     PreserveUserInputData(viewModel, basicInfoModel, proposalContentModel);
            //     return View("~/Views/Proposal/Proposal.cshtml", viewModel);
            // }

            // アップロードファイルを保存
            SaveUploadedFiles(viewModel.ProposalContent);

            // 提案状態を作成中に設定
            //basicInfoModel.Status = 1;

            // データベースに登録または更新
            //InsertOrUpdate(basicInfoModel, proposalContentModel);

            return RedirectToAction("Index", "ProposalList");
        }

        /// <summary>
        /// アップロードされたファイルを保存します（最大5つ）
        /// </summary>
        /// <param name="proposalContentModel">提案内容モデル</param>
        private void SaveUploadedFiles(ProposalContentModel proposalContentModel)
        {
            // 最大5つのファイルを処理
            for (int fileIndex = 1; fileIndex <= 5; fileIndex++)
            {
                var fileProperty = proposalContentModel.GetType().GetProperty($"TenpuFile{fileIndex}");
                var uploadedFile = fileProperty?.GetValue(proposalContentModel) as IFormFile;
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    // アップロードディレクトリの作成
                    var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Proposal/wwwroot/uploads");
                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }

                    // ファイル名の生成（GUID + 拡張子）
                    var fileExtension = Path.GetExtension(uploadedFile.FileName);
                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadDirectory, fileName);

                    // ファイルの保存
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadedFile.CopyTo(fileStream);
                    }

                    // ファイル名をモデルに設定
                    var fileNameProperty = proposalContentModel.GetType().GetProperty($"TenpuFileName{fileIndex}");
                    fileNameProperty?.SetValue(proposalContentModel, fileName);
                }
            }
        }



        /// <summary>
        /// CSV出力処理を実行します
        /// </summary>
        /// <param name="basicInfoModel">基本情報モデル</param>
        /// <param name="proposalContentModel">提案内容モデル</param>
        /// <param name="viewModel">ビューモデル</param>
        /// <returns>CSVファイル</returns>
        private IActionResult HandleExport(ProposalModel basicInfoModel, ProposalContentModel proposalContentModel, ProposalViewModel viewModel)
        {
            // バリデーション実行
            if (!ValidateModel(basicInfoModel) || !ValidateProposalContent(proposalContentModel))
            {
                //SetDropdowns();
                SetShowProposalContentFlagForModelStateError();
                PreserveUserInputData(viewModel, basicInfoModel, proposalContentModel);
                return View("~/Views/Proposal/Proposal.cshtml", viewModel);
            }

            // ドロップダウンリストから表示名を取得
            var dropdowns = (Proposal.BL.ProposalBL.DropdownsViewModel)ViewBag.Dropdowns;
            string proposalTypeName = dropdowns.ProposalTypes.FirstOrDefault(x => x.Value == basicInfoModel.ProposalTypeId)?.Text ?? "";
            string firstReviewerAffiliation = dropdowns.Affiliations.FirstOrDefault(x => x.Value == basicInfoModel.FirstReviewerAffiliationId)?.Text ?? "";
            string firstReviewerDepartment = dropdowns.Departments.FirstOrDefault(x => x.Value == basicInfoModel.FirstReviewerDepartmentId)?.Text ?? "";
            string firstReviewerSection = dropdowns.Sections.FirstOrDefault(x => x.Value == basicInfoModel.FirstReviewerSectionId)?.Text ?? "";

            // CSVデータを作成
            var csv = new StringBuilder();
            csv.AppendLine("提案年度,提案題名,提案の種類,提案の区分,氏名又は代表名,グループ名,第一次審査者所属,第一次審査者部・署,第一次審査者課・部門,第一次審査者氏名,第一次審査者官職,主務課,関係課,現状・問題点,改善案,効果の種類,効果");

            // 効果の種類の表示名を取得
            string koukaJishiText = proposalContentModel.KoukaJishi.HasValue
                ? (proposalContentModel.KoukaJishi.Value.GetType().GetField(proposalContentModel.KoukaJishi.Value.ToString())
                    .GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description ?? proposalContentModel.KoukaJishi.Value.ToString())
                : "";

            csv.AppendLine($"\"{basicInfoModel.TeianYear}\",\"{basicInfoModel.TeianDaimei}\",\"{proposalTypeName}\",\"{basicInfoModel.ProposalKbnId}\",\"{basicInfoModel.ShimeiOrDaihyoumei}\",\"{basicInfoModel.GroupMei}\",\"{firstReviewerAffiliation}\",\"{firstReviewerDepartment}\",\"{firstReviewerSection}\",\"{basicInfoModel.FirstReviewerName}\",\"{basicInfoModel.FirstReviewerTitle}\",\"{basicInfoModel.EvaluationSectionId}\",\"{basicInfoModel.ResponsibleSectionId1}\",\"{proposalContentModel.GenjyoMondaiten}\",\"{proposalContentModel.Kaizenan}\",\"{koukaJishiText}\",\"{proposalContentModel.Kouka}\"");

            // CSVファイルを返却
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var filename = $"提案内容_{DateTime.Now:yyyyMMddHHmmss}.csv";
            return File(bytes, "text/csv", filename);
        }

        /// <summary>
        /// 提出処理を実行します
        /// </summary>
        /// <param name="basicInfoModel">基本情報モデル</param>
        /// <param name="proposalContentModel">提案内容モデル</param>
        /// <param name="viewModel">ビューモデル</param>
        /// <returns>処理結果</returns>
        private IActionResult HandleSubmit(ProposalModel basicInfoModel, ProposalContentModel proposalContentModel, ProposalViewModel viewModel)
        {
            // バリデーション実行
            if (!ValidateModel(basicInfoModel) || !ValidateProposalContent(proposalContentModel))
            {
                //SetDropdowns();
                SetShowProposalContentFlagForModelStateError();
                PreserveUserInputData(viewModel, basicInfoModel, proposalContentModel);
                return View("~/Views/Proposal/Proposal.cshtml", viewModel);
            }

            // アップロードファイルを保存
            SaveUploadedFiles(proposalContentModel);

            // 提案状態を審査中に設定
            basicInfoModel.Status = 11;

            // データベースに登録または更新
            InsertOrUpdate(basicInfoModel, proposalContentModel);

            return RedirectToAction("Index", "ProposalList");
        }

        /// <summary>
        /// ユーザー入力データを保持し、必要なプロパティが空でないことを確保します
        /// </summary>
        /// <param name="viewModel">ビューモデル</param>
        /// <param name="basicInfoModel">基本情報モデル</param>
        /// <param name="proposalContentModel">提案内容モデル</param>
        private void PreserveUserInputData(ProposalViewModel viewModel, ProposalModel basicInfoModel, ProposalContentModel proposalContentModel)
        {
            // ビューモデルのnullチェックと初期化
            if (viewModel == null)
            {
                viewModel = new ProposalViewModel();
            }
            if (viewModel.BasicInfo == null)
            {
                viewModel.BasicInfo = new ProposalModel();
            }
            if (viewModel.ProposalContent == null)
            {
                viewModel.ProposalContent = new ProposalContentModel();
            }

            // グループメンバーのnullチェックと10人に補完
            if (viewModel.BasicInfo.GroupMembers == null)
            {
                viewModel.BasicInfo.GroupMembers = new List<GroupMemberModel>();
            }

            // グループメンバーを10人に補完
            while (viewModel.BasicInfo.GroupMembers.Count < 10)
            {
                viewModel.BasicInfo.GroupMembers.Add(new GroupMemberModel());
            }

            // 基本情報をビューモデルに再設定
            viewModel.BasicInfo.ProposalId = basicInfoModel?.ProposalId;
            viewModel.BasicInfo.UserId = basicInfoModel?.UserId;
            viewModel.BasicInfo.TeianYear = basicInfoModel?.TeianYear;
            viewModel.BasicInfo.TeianDaimei = basicInfoModel?.TeianDaimei;
            viewModel.BasicInfo.ProposalTypeId = basicInfoModel?.ProposalTypeId;
            viewModel.BasicInfo.ProposalKbnId = basicInfoModel?.ProposalKbnId;
            viewModel.BasicInfo.AffiliationId = basicInfoModel?.AffiliationId;
            viewModel.BasicInfo.DepartmentId = basicInfoModel?.DepartmentId;
            viewModel.BasicInfo.SectionId = basicInfoModel?.SectionId;
            viewModel.BasicInfo.SubsectionId = basicInfoModel?.SubsectionId;
            viewModel.BasicInfo.AffiliationName = basicInfoModel?.AffiliationName;
            viewModel.BasicInfo.DepartmentName = basicInfoModel?.DepartmentName;
            viewModel.BasicInfo.SectionName = basicInfoModel?.SectionName;
            viewModel.BasicInfo.SubsectionName = basicInfoModel?.SubsectionName;
            viewModel.BasicInfo.ShimeiOrDaihyoumei = basicInfoModel?.ShimeiOrDaihyoumei;
            viewModel.BasicInfo.GroupMei = basicInfoModel?.GroupMei;
            // グループメンバーを設定し、10人に補完
            viewModel.BasicInfo.GroupMembers = basicInfoModel?.GroupMembers ?? new List<GroupMemberModel>();
            while (viewModel.BasicInfo.GroupMembers.Count < 10)
            {
                viewModel.BasicInfo.GroupMembers.Add(new GroupMemberModel());
            }
            viewModel.BasicInfo.SkipFirstReviewer = basicInfoModel?.SkipFirstReviewer ?? false;
            viewModel.BasicInfo.FirstReviewerAffiliationId = basicInfoModel?.FirstReviewerAffiliationId;
            viewModel.BasicInfo.FirstReviewerDepartmentId = basicInfoModel?.FirstReviewerDepartmentId;
            viewModel.BasicInfo.FirstReviewerSectionId = basicInfoModel?.FirstReviewerSectionId;
            viewModel.BasicInfo.FirstReviewerSubsectionId = basicInfoModel?.FirstReviewerSubsectionId;
            viewModel.BasicInfo.FirstReviewerName = basicInfoModel?.FirstReviewerName;
            viewModel.BasicInfo.FirstReviewerTitle = basicInfoModel?.FirstReviewerTitle;
            viewModel.BasicInfo.EvaluationSectionId = basicInfoModel?.EvaluationSectionId;
            viewModel.BasicInfo.ResponsibleSectionId1 = basicInfoModel?.ResponsibleSectionId1;
            viewModel.BasicInfo.ResponsibleSectionId2 = basicInfoModel?.ResponsibleSectionId2;
            viewModel.BasicInfo.ResponsibleSectionId3 = basicInfoModel?.ResponsibleSectionId3;
            viewModel.BasicInfo.ResponsibleSectionId4 = basicInfoModel?.ResponsibleSectionId4;
            viewModel.BasicInfo.ResponsibleSectionId5 = basicInfoModel?.ResponsibleSectionId5;
            viewModel.BasicInfo.Status = basicInfoModel?.Status;
            viewModel.BasicInfo.Createddate = basicInfoModel?.Createddate;
            viewModel.BasicInfo.Submissiondate = basicInfoModel?.Submissiondate;

            // 提案内容をビューモデルに再設定
            viewModel.ProposalContent.ProposalId = proposalContentModel?.ProposalId;
            viewModel.ProposalContent.UserId = proposalContentModel?.UserId;
            viewModel.ProposalContent.GenjyoMondaiten = proposalContentModel?.GenjyoMondaiten;
            viewModel.ProposalContent.Kaizenan = proposalContentModel?.Kaizenan;
            viewModel.ProposalContent.KoukaJishi = proposalContentModel?.KoukaJishi;
            viewModel.ProposalContent.Kouka = proposalContentModel?.Kouka;
            viewModel.ProposalContent.TenpuFile1 = proposalContentModel?.TenpuFile1;
            viewModel.ProposalContent.TenpuFile2 = proposalContentModel?.TenpuFile2;
            viewModel.ProposalContent.TenpuFile3 = proposalContentModel?.TenpuFile3;
            viewModel.ProposalContent.TenpuFile4 = proposalContentModel?.TenpuFile4;
            viewModel.ProposalContent.TenpuFile5 = proposalContentModel?.TenpuFile5;
            viewModel.ProposalContent.TenpuFileName1 = proposalContentModel?.TenpuFileName1;
            viewModel.ProposalContent.TenpuFileName2 = proposalContentModel?.TenpuFileName2;
            viewModel.ProposalContent.TenpuFileName3 = proposalContentModel?.TenpuFileName3;
            viewModel.ProposalContent.TenpuFileName4 = proposalContentModel?.TenpuFileName4;
            viewModel.ProposalContent.TenpuFileName5 = proposalContentModel?.TenpuFileName5;
        }



        /// <summary>
        /// 基本情報モデルの検証を実行します
        /// </summary>
        /// <param name="basicInfoModel">検証対象の基本情報モデル</param>
        /// <returns>検証が成功した場合はtrue、失敗した場合はfalse</returns>
        private bool ValidateModel(ProposalModel basicInfoModel)
        {
            if (basicInfoModel == null)
            {
                return false;
            }

            // 標準バリデーションを実行
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(basicInfoModel);
            Validator.TryValidateObject(basicInfoModel, validationContext, validationResults, true);

            // カスタムバリデーションを実行
            var customValidationResults = _proposalValidator.ValidateProposal(basicInfoModel);
            validationResults.AddRange(customValidationResults);

            // バリデーション結果をModelStateに追加
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                }
            }

            return ModelState.IsValid;
        }

        /// <summary>
        /// 提案内容モデルの検証を実行します
        /// </summary>
        /// <param name="proposalContentModel">検証対象の提案内容モデル</param>
        /// <returns>検証が成功した場合はtrue、失敗した場合はfalse</returns>
        private bool ValidateProposalContent(ProposalContentModel proposalContentModel)
        {
            if (proposalContentModel == null)
            {
                return false;
            }

            // カスタムバリデーションを実行
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(proposalContentModel);
            Validator.TryValidateObject(proposalContentModel, validationContext, validationResults, true);

            // バリデーション結果をModelStateに追加
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                }
            }

            return ModelState.IsValid;
        }

        /// <summary>
        /// ModelStateに提案内容タブのエラーがある場合、ViewBag.ShowProposalContentをtrueに設定します
        /// </summary>
        private void SetShowProposalContentFlagForModelStateError()
        {
            if (ModelState == null)
            {
                ViewBag.ShowProposalContent = false;
                return;
            }

            // 提案内容タブ関連フィールドの定義
            var proposalContentFields = new[]
            {
                "GenjyoMondaiten", "Kaizenan", "KoukaJishi", "Kouka",
                "TenpuFile1", "TenpuFile2", "TenpuFile3", "TenpuFile4", "TenpuFile5",
                "TenpuFileName1", "TenpuFileName2", "TenpuFileName3", "TenpuFileName4", "TenpuFileName5",
                "TenpuFilePath1", "TenpuFilePath2", "TenpuFilePath3", "TenpuFilePath4", "TenpuFilePath5"
            };

            // 提案内容にエラーがあるかチェック
            bool hasProposalContentError = ModelState.Keys.Any(key =>
                proposalContentFields.Any(field => key.Contains(field)) &&
                ModelState[key].Errors.Count > 0);

            ViewBag.ShowProposalContent = hasProposalContentError;
        }

        /// <summary>
        /// 提案書データの登録または更新処理を実行します
        /// </summary>
        /// <param name="basicInfoModel">基本情報モデル</param>
        /// <param name="proposalContentModel">提案内容モデル</param>
        public void InsertOrUpdate(ProposalModel basicInfoModel, ProposalContentModel proposalContentModel)
        {
            if (basicInfoModel == null || proposalContentModel == null)
            {
                return;
            }

            // セッションからユーザーIDを取得
            basicInfoModel.UserId = HttpContext.Session.GetString("UserId");

            // 新規登録か更新かの判定
            if (string.IsNullOrEmpty(basicInfoModel.ProposalId))
            {
                // 新規登録処理
                basicInfoModel.ProposalId = _createBL.Insertproposals_detail(basicInfoModel, proposalContentModel).ToString();

                // グループ提案の場合、グループ情報も登録
                if (basicInfoModel.ProposalKbnId == "2")
                {
                    //_createBL.InsertGroupInfo(basicInfoModel);
                }
            }
            else
            {
                // 更新処理
                _createBL.Updateproposals_detail(basicInfoModel, proposalContentModel);

                // グループ提案の場合、グループ情報も更新
                if (basicInfoModel.ProposalKbnId == "2")
                {
                    // 既存のグループデータを削除
                    // _createBL.DeleteGroupInfo(basicInfoModel.ProposalId);

                    // 新しいグループデータを登録
                    //_createBL.InsertGroupInfo(basicInfoModel);
                }
            }
        }
    }
}
