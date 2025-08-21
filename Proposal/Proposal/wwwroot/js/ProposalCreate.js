// ProposalCreate JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Tabボタンとコンテンツエリアを取得
    const btnBase = document.getElementById('btnBase');
    const btnTeian = document.getElementById('btnTeian');
    const btnFirstReview = document.getElementById('btnFirstReview');

    // Tabボタンにクリックイベントを追加
    btnBase.addEventListener('click', () => showDiv('base'));
    btnTeian.addEventListener('click', () => showDiv('teian'));
    btnFirstReview.addEventListener('click', () => showDiv('firstReview'));

    // 初期表示で基本情報Tabを表示
    showDiv('base');

    // 提案の区分の変更を監視
    const teianKbnSelect = document.getElementById('teianKbn');
    if (teianKbnSelect) {
        teianKbnSelect.addEventListener('change', toggleGroupSection);
        // 初期状態でグループセクションの表示/非表示を設定
        toggleGroupSection();
    }
});

// Tab切り替え関数（他のJSファイルから呼び出される）
function showDiv(divName) {
    const btnBase = document.getElementById('btnBase');
    const btnTeian = document.getElementById('btnTeian');
    const btnFirstReview = document.getElementById('btnFirstReview');
    const baseDiv = document.getElementById('baseDiv');
    const teianDiv = document.getElementById('teianDiv');
    const firstReviewDiv = document.getElementById('firstReviewDiv');

    // すべてのTabボタンからactiveクラスを削除
    btnBase.classList.remove('active');
    btnTeian.classList.remove('active');
    btnFirstReview.classList.remove('active');

    // すべてのコンテンツエリアを非表示
    baseDiv.style.display = 'none';
    teianDiv.style.display = 'none';
    firstReviewDiv.style.display = 'none';

    // div名に応じて対応するコンテンツを表示し、対応するボタンをアクティブ化
    if (divName === 'base') {
        baseDiv.style.display = 'block';
        btnBase.classList.add('active');
    } else if (divName === 'teian') {
        teianDiv.style.display = 'block';
        btnTeian.classList.add('active');
    } else if (divName === 'firstReview') {
        firstReviewDiv.style.display = 'block';
        btnFirstReview.classList.add('active');
    }
}

// メンバーを削除する関数
function removeMember(button) {
    const memberDiv = button.closest('.member-item');
    if (memberDiv) {
        memberDiv.remove();
        reorderMembers();
    }
}

// メンバーを追加する関数
function addMember() {
    const container = document.getElementById('groupMembersContainer');
    const hiddenMembers = container.querySelectorAll('.member-item[style*="display: none"]');
    
    if (hiddenMembers.length > 0) {
        const firstHiddenMember = hiddenMembers[0];
        firstHiddenMember.style.display = 'block';
        reorderMembers();
    }
}

// メンバーを並び替える関数
function reorderMembers() {
    const container = document.getElementById('groupMembersContainer');
    const visibleMembers = container.querySelectorAll('.member-item[style*="display: block"], .member-item:not([style*="display: none"])');
    
    visibleMembers.forEach((member, index) => {
        const titleElement = member.querySelector('h6');
        if (titleElement) {
            titleElement.innerHTML = `<i class="fas fa-user me-2" style="color: #136AA8;"></i>メンバー${index + 1}`;
        }
        
        // data-member-index属性を更新
        member.setAttribute('data-member-index', index);
        
        // フォームフィールドのname属性とIDを更新
        const affiliationSelect = member.querySelector('select[name*="AffiliationId"]');
        const departmentSelect = member.querySelector('select[name*="DepartmentId"]');
        const sectionSelect = member.querySelector('select[name*="SectionId"]');
        const subsectionSelect = member.querySelector('select[name*="SubsectionId"]');
        const nameInput = member.querySelector('input[name*="Name"]');
        
        if (affiliationSelect) {
            affiliationSelect.name = `GroupMembers[${index}].AffiliationId`;
            affiliationSelect.id = `memberAffiliation_${index}`;
        }
        if (departmentSelect) {
            departmentSelect.name = `GroupMembers[${index}].DepartmentId`;
            departmentSelect.id = `memberDepartment_${index}`;
        }
        if (sectionSelect) {
            sectionSelect.name = `GroupMembers[${index}].SectionId`;
            sectionSelect.id = `memberSection_${index}`;
        }
        if (subsectionSelect) {
            subsectionSelect.name = `GroupMembers[${index}].SubsectionId`;
            subsectionSelect.id = `memberSubsection_${index}`;
        }
        if (nameInput) nameInput.name = `GroupMembers[${index}].Name`;
    });
    
    // 並び替え後に級聯ドロップダウンを再設定
    if (typeof setupGroupMembersCascading === 'function') {
        setupGroupMembersCascading();
    }
    
    // 並び替え後に初期値を再設定
    if (typeof setupGroupMembersInitialValues === 'function') {
        setTimeout(function() {
            setupGroupMembersInitialValues();
        }, 200);
    }
}

// グループセクションの表示/非表示を切り替える関数
function toggleGroupSection() {
    const teianKbnSelect = document.getElementById('teianKbn');
    const groupSection = document.getElementById('groupSection');
    const shimeiLabel = document.getElementById('shimeiLabel');
    
    if (teianKbnSelect && groupSection) {
        const selectedValue = teianKbnSelect.value;
        
        if (selectedValue === '2') { // グループ提案の場合
            groupSection.style.display = 'block';
            if (shimeiLabel) {
                shimeiLabel.innerHTML = '代表者氏名 <span class="required-badge">必須</span>';
            }
            // グループセクションが表示された後に初期値を設定
            setTimeout(function() {
                if (typeof setupGroupMembersInitialValues === 'function') {
                    setupGroupMembersInitialValues();
                }
            }, 100);
        } else { // 個人提案の場合
            groupSection.style.display = 'none';
            if (shimeiLabel) {
                shimeiLabel.innerHTML = '氏名又は代表名 <span class="required-badge">必須</span>';
            }
        }
    }
}
