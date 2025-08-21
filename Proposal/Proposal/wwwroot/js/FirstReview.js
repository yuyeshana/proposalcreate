// ProposalCreate JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // 提案書作成モード以外では基本情報と提案内容は非活性し、送信直前だけ一時的に活性化する
    var page = document.getElementById("currentMode");
    var currentMode = page.dataset.currentMode;
    var form = document.getElementById("proposalForm");
    if (currentMode !== "Create") {
        disableInputs("#baseDiv");
        disableInputs("#teianDiv");
        // 保存送信直前に disabled を外して値をPOSTに乗せる
        if (form) {
            form.addEventListener("submit", function () {
                // ファイル入力は re-enable しない（変更禁止のため）
                enableGroup("#baseDiv");
                enableGroup("#teianDiv");
            });
        }
    }
});

function disableInputs(containerSelector) {
    var container = document.querySelector(containerSelector);
    if (!container) return;

    container.querySelectorAll("input, select, textarea, button").forEach(function (el) {
        if (el.type !== "hidden") {
            el.disabled = true;
        }
    });
}

function enableGroup(containerSelector) {
    var container = document.querySelector(containerSelector);
    if (!container) return;

    container.querySelectorAll("input, select, textarea, button").forEach(function (el) {
        if (el.type === "hidden") return;
        if (el.type !== "hidden") {
            el.disabled = false;
        }
    });
}