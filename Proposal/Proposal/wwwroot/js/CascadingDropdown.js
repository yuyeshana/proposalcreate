$(document).ready(function () {
    // 第一次審査者の所属変更時の処理
    $('#firstReviewerAffiliation').on('change', function () {
        var affiliationId = $(this).val();
        loadDepartments(affiliationId);

        // 下位のドロップダウンをリセット
        $('#firstReviewerDepartment').empty().append('<option value="">選択してください</option>');
        $('#firstReviewerSection').empty().append('<option value="">選択してください</option>');
        $('#firstReviewerSubsection').empty().append('<option value="">選択してください</option>');
    });

    // 第一次審査者の部・署変更時の処理
    $('#firstReviewerDepartment').on('change', function () {
        var departmentId = $(this).val();
        loadSections(departmentId);

        // 下位のドロップダウンをリセット
        $('#firstReviewerSection').empty().append('<option value="">選択してください</option>');
        $('#firstReviewerSubsection').empty().append('<option value="">選択してください</option>');
    });

    // 第一次審査者の課・部門変更時の処理
    $('#firstReviewerSection').on('change', function () {
        var sectionId = $(this).val();
        loadSubsections(sectionId);

        // 下位のドロップダウンをリセット
        $('#firstReviewerSubsection').empty().append('<option value="">選択してください</option>');
    });

    // 所属IDに基づいて部・署を読み込む
    function loadDepartments(affiliationId) {
        if (!affiliationId) {
            $('#firstReviewerDepartment').empty().append('<option value="">選択してください</option>');
            return;
        }

        $.ajax({
            url: '/Proposal/GetDepartmentsByAffiliation',
            type: 'GET',
            data: { affiliationId: affiliationId },
            success: function (data) {
                var departmentSelect = $('#firstReviewerDepartment');
                var currentValue = departmentSelect.data('current-dept');
                departmentSelect.empty();
                departmentSelect.append('<option value="">選択してください</option>');

                $.each(data, function (index, item) {
                    departmentSelect.append('<option value="' + item.value + '">' + item.text + '</option>');
                });

                // 以前選択されていた値が新しいリストに存在する場合は復元
                if (currentValue && data.some(function (item) { return item.value === currentValue; })) {
                    departmentSelect.val(currentValue);
                }

                // 初期値がある場合は設定
                if (window.firstReviewerInitialValues && window.firstReviewerInitialValues.department) {
                    departmentSelect.val(window.firstReviewerInitialValues.department);
                    // 初期値が設定されたら、次のドロップダウンも読み込む
                    loadSections(window.firstReviewerInitialValues.department);
                }
            },
            error: function (xhr, status, error) {
                console.error('部・署の読み込みに失敗しました:', error);
                $('#firstReviewerDepartment').empty().append('<option value="">読み込みエラー</option>');
            }
        });
    }

    // 部・署IDに基づいて課・部門を読み込む
    function loadSections(departmentId) {
        if (!departmentId) {
            $('#firstReviewerSection').empty().append('<option value="">選択してください</option>');
            return;
        }

        $.ajax({
            url: '/Proposal/GetSectionsByDepartment',
            type: 'GET',
            data: { departmentId: departmentId },
            success: function (data) {
                var sectionSelect = $('#firstReviewerSection');
                sectionSelect.empty();
                sectionSelect.append('<option value="">選択してください</option>');

                $.each(data, function (index, item) {
                    sectionSelect.append('<option value="' + item.value + '">' + item.text + '</option>');
                });

                // 初期値がある場合は設定
                if (window.firstReviewerInitialValues && window.firstReviewerInitialValues.section) {
                    sectionSelect.val(window.firstReviewerInitialValues.section);
                    // 初期値が設定されたら、次のドロップダウンも読み込む
                    loadSubsections(window.firstReviewerInitialValues.section);
                }
            },
            error: function (xhr, status, error) {
                console.error('課・部門の読み込みに失敗しました:', error);
                $('#firstReviewerSection').empty().append('<option value="">読み込みエラー</option>');
            }
        });
    }

    // 課・部門IDに基づいて係・担当を読み込む
    function loadSubsections(sectionId) {
        if (!sectionId) {
            $('#firstReviewerSubsection').empty().append('<option value="">選択してください</option>');
            return;
        }

        $.ajax({
            url: '/Proposal/GetSubsectionsBySection',
            type: 'GET',
            data: { sectionId: sectionId },
            success: function (data) {
                var subsectionSelect = $('#firstReviewerSubsection');
                subsectionSelect.empty();
                subsectionSelect.append('<option value="">選択してください</option>');

                $.each(data, function (index, item) {
                    subsectionSelect.append('<option value="' + item.value + '">' + item.text + '</option>');
                });

                // 初期値がある場合は設定
                if (window.firstReviewerInitialValues && window.firstReviewerInitialValues.subsection) {
                    subsectionSelect.val(window.firstReviewerInitialValues.subsection);
                }
            },
            error: function (xhr, status, error) {
                console.error('係・担当の読み込みに失敗しました:', error);
                $('#firstReviewerSubsection').empty().append('<option value="">読み込みエラー</option>');
            }
        });
    }

    // ページ読み込み時に初期値を設定
    if (window.firstReviewerInitialValues && window.firstReviewerInitialValues.affiliation) {
        // 所属が選択されている場合、部・署を読み込む
        loadDepartments(window.firstReviewerInitialValues.affiliation);
    }

    // GroupMembersの初期値を設定
    setupGroupMembersInitialValues();

    // GroupMembersの級聯ドロップダウン処理を設定
    setupGroupMembersCascading();
});

// GroupMembersの級聯ドロップダウンを設定する関数
function setupGroupMembersCascading() {
    // 0-9のメンバーに対して級聯処理を設定
    for (let i = 0; i < 10; i++) {
        // 所属変更時の処理
        $(`#memberAffiliation_${i}`).on('change', function () {
            var affiliationId = $(this).val();
            loadMemberDepartments(i, affiliationId);

            // 下位のドロップダウンをリセット
            $(`#memberDepartment_${i}`).empty().append('<option value="">選択してください</option>');
            $(`#memberSection_${i}`).empty().append('<option value="">選択してください</option>');
            $(`#memberSubsection_${i}`).empty().append('<option value="">選択してください</option>');
        });

        // 部・署変更時の処理
        $(`#memberDepartment_${i}`).on('change', function () {
            var departmentId = $(this).val();
            loadMemberSections(i, departmentId);

            // 下位のドロップダウンをリセット
            $(`#memberSection_${i}`).empty().append('<option value="">選択してください</option>');
            $(`#memberSubsection_${i}`).empty().append('<option value="">選択してください</option>');
        });

        // 課・部門変更時の処理
        $(`#memberSection_${i}`).on('change', function () {
            var sectionId = $(this).val();
            loadMemberSubsections(i, sectionId);

            // 下位のドロップダウンをリセット
            $(`#memberSubsection_${i}`).empty().append('<option value="">選択してください</option>');
        });
    }
}

// メンバーの部・署を読み込む関数
function loadMemberDepartments(memberIndex, affiliationId) {
    if (!affiliationId) {
        return;
    }

    $.ajax({
        url: '/Proposal/GetDepartmentsByAffiliation',
        type: 'GET',
        data: { affiliationId: affiliationId },
        success: function (data) {
            // 現在選択されている値を保存
            var departmentSelect = $(`#memberDepartment_${memberIndex}`);
            var currentValue = departmentSelect.data('current-dept');

            departmentSelect.empty();
            departmentSelect.append('<option value="">選択してください</option>');

            $.each(data, function (index, item) {
                departmentSelect.append('<option value="' + item.value + '">' + item.text + '</option>');
            });

            // 以前選択されていた値が新しいリストに存在する場合は復元
            if (currentValue && data.some(function (item) { return item.value === currentValue; })) {
                departmentSelect.val(currentValue);
            }
        },
        error: function (xhr, status, error) {
            console.error('メンバー部・署の読み込みに失敗しました:', error);
            $(`#memberDepartment_${memberIndex}`).empty().append('<option value="">読み込みエラー</option>');
        }
    });
}

// メンバーの課・部門を読み込む関数
function loadMemberSections(memberIndex, departmentId) {
    if (!departmentId) {
        return;
    }

    $.ajax({
        url: '/Proposal/GetSectionsByDepartment',
        type: 'GET',
        data: { departmentId: departmentId },
        success: function (data) {
            var sectionSelect = $(`#memberSection_${memberIndex}`);
            // 現在選択されている値を保存
            var currentValue = sectionSelect.data('current-section');

            sectionSelect.empty();
            sectionSelect.append('<option value="">選択してください</option>');

            $.each(data, function (index, item) {
                sectionSelect.append('<option value="' + item.value + '">' + item.text + '</option>');
            });

            // 以前選択されていた値が新しいリストに存在する場合は復元
            if (currentValue && data.some(function (item) { return item.value === currentValue; })) {
                sectionSelect.val(currentValue);
            }
        },
        error: function (xhr, status, error) {
            console.error('メンバー課・部門の読み込みに失敗しました:', error);
            $(`#memberSection_${memberIndex}`).empty().append('<option value="">読み込みエラー</option>');
        }
    });
}

// メンバーの係・担当を読み込む関数
function loadMemberSubsections(memberIndex, sectionId) {
    if (!sectionId) {
        return;
    }

    $.ajax({
        url: '/Proposal/GetSubsectionsBySection',
        type: 'GET',
        data: { sectionId: sectionId },
        success: function (data) {
            var subsectionSelect = $(`#memberSubsection_${memberIndex}`);
            // 現在選択されている値を保存
            var currentValue = subsectionSelect.data('current-subsection');

            subsectionSelect.empty();
            subsectionSelect.append('<option value="">選択してください</option>');

            $.each(data, function (index, item) {
                subsectionSelect.append('<option value="' + item.value + '">' + item.text + '</option>');
            });

            // 以前選択されていた値が新しいリストに存在する場合は復元
            if (currentValue && data.some(function (item) { return item.value === currentValue; })) {
                subsectionSelect.val(currentValue);
            }
        },
        error: function (xhr, status, error) {
            console.error('メンバー係・担当の読み込みに失敗しました:', error);
            $(`#memberSubsection_${memberIndex}`).empty().append('<option value="">読み込みエラー</option>');
        }
    });
}

// GroupMembersの初期値を設定する関数
function setupGroupMembersInitialValues() {
    // 0-9のメンバーに対して初期値を設定
    for (let i = 0; i < 10; i++) {
        // 所属に値がある場合、部・署を読み込む
        var affiliationId = $(`#memberAffiliation_${i}`).val();
        if (affiliationId) {
            loadMemberDepartments(i, affiliationId);

            // 部・署に値がある場合、課・部門を読み込む
            setTimeout(function () {
                var departmentId = $(`#memberDepartment_${i}`).val();
                if (departmentId) {
                    loadMemberSections(i, departmentId);

                    // 課・部門に値がある場合、係・担当を読み込む
                    setTimeout(function () {
                        var sectionId = $(`#memberSection_${i}`).val();
                        if (sectionId) {
                            loadMemberSubsections(i, sectionId);
                        }
                    }, 100);
                }
            }, 100);
        }
    }
}
