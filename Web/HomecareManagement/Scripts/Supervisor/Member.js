$(document).ready(function () {
    var isUsernameRepeat = false;
    var supervisorID = 0;
    var editAccountID = 0;
    var lsAttendant = [];
    var lsEmployer = [];
    var lsSupervisor = [];
    var Grid_lsAttendant = [];
    var Grid_lsEmployer = [];
    var Grid_lsSupervisor = [];
    var isEditMode = false;
    //初始化
    $(function () {
        $("#Member-Birthday").kendoDatePicker();
        $("#Member-Attendant-Firstday").kendoDatePicker();
        $("#Member-Supervisor-Firstday").kendoDatePicker();
        //縣市
        $('#dk_tw_citySelector').dk_tw_citySelector('#Member-county', '#Member-district', '#zipcode');
        //Grid Init for 照服員
        $(function () {
            $("#gridAttendant").kendoGrid({
                //排序
                sortable: true,
                pageable: {
                    pageSizes: true,
                    buttonCount: 5
                },
                columns: [{
                    field: "account_uid",
                    title: "編號",
                    hidden: true
                }, {
                    field: "username",
                    title: "帳號",
                    width: 150
                }, {
                    field: "supervisorName",
                    title: "主責督導",
                    hidden: true
                }, {
                    field: "displayname",
                    title: "姓名",
                    width: 150
                }, {
                    field: "phone1",
                    title: "主要電話",
                    width: 150
                }, {
                    command: {
                        text: "編輯",
                        click: gridEditClick
                    },
                    width: "90px"
                }],
                detailTemplate: "<h5>帳號： #: username #</h5>"
                              + "<h5>姓名： #: displayname #</h5>"
                              + "<h5>生日： #: birthday #</h5>"
                              + "<h5>性別： #: sex #</h5>"
                              + "<h5>地址： #: address #</h5>"
                              + "<h5>主要電話： #: phone1 #</h5>"
                              + "<h5>備用電話： #: phone2 #</h5>"
                              + "<h5>主責督導： #: supervisorName #</h5>"
                              + "<h5>時薪： #: pay #</h5>"
                              + "<h5>到職日： #: firstday #</h5>"
                              + "<h5>證照： #: lsLicense #</h5>"
                              + "<h5>備註： #: summary #</h5>"
            });
        });
        //Grid Init for 督導
        $(function () {
            $("#gridSupervisor").kendoGrid({
                //排序
                sortable: true,
                pageable: {
                    pageSizes: true,
                    buttonCount: 5
                },
                columns: [{
                    field: "account_uid",
                    title: "編號",
                    hidden: true
                }, {
                    field: "username",
                    title: "帳號",
                    width: 150
                }, {
                    field: "displayname",
                    title: "姓名",
                    width: 150
                }, {
                    field: "phone1",
                    title: "主要電話",
                    width: 150
                }, {
                    command: {
                        text: "編輯",
                        click: gridEditClick
                    },
                    width: "90px"
                }],
                detailTemplate: "<h5>帳號： #: username #</h5>"
                              + "<h5>姓名： #: displayname #</h5>"
                              + "<h5>生日： #: birthday #</h5>"
                              + "<h5>性別： #: sex #</h5>"
                              + "<h5>戶籍地址： #: address #</h5>"
                              + "<h5>主要電話： #: phone1 #</h5>"
                              + "<h5>備用電話： #: phone2 #</h5>"
                              + "<h5>證照： #: lsLicense #</h5>"
                              + "<h5>到職日： #: firstday #</h5>"
                              + "<h5>在職訓練： #: lsTrain #</h5>"
                              + "<h5>專業背景： #: proBG #</h5>"
                              + "<h5>學歷： #: eduBG #</h5>"
            });
        });
        //Grid Init for 案主
        $(function () {
            $("#gridEmployer").kendoGrid({
                //排序
                sortable: true,
                pageable: {
                    pageSizes: true,
                    buttonCount: 5
                },
                columns: [{
                    field: "account_uid",
                    title: "編號",
                    hidden: true
                }, {
                    field: "username",
                    title: "帳號",
                    width: 150
                }, {
                    field: "supervisorName",
                    title: "主責督導",
                    hidden: true
                }, {
                    field: "displayname",
                    title: "姓名",
                    width: 150
                }, {
                    field: "phone1",
                    title: "主要電話",
                    width: 150
                }, {
                    command: {
                        text: "編輯",
                        click: gridEditClick
                    },
                    width: "90px"
                }],
                detailTemplate: "<h5>帳號： #: username #</h5>"
                              + "<h5>姓名： #: displayname #</h5>"
                              + "<h5>生日： #: birthday #</h5>"
                              + "<h5>性別： #: sex #</h5>"
                              + "<h5>通訊地址： #: address #</h5>"
                              + "<h5>主要電話： #: phone1 #</h5>"
                              + "<h5>備用電話： #: phone2 #</h5>"
                              + "<h5>主責督導： #: supervisorName #</h5>"
                              + "<h5>失能程度： #: info_employer_item1_uid #</h5>"
                              + "<h5>經濟程度： #: info_employer_item2_uid #</h5>"
                              + "<h5>案主身份： #: info_employer_item3_uid_str #</h5>"
                              + "<h5>緊急連絡人1： #: emg1_displayname #</h5>"
                              + "<h5>主要電話： #: emg1_phone1 #</h5>"
                              + "<h5>備用電話： #: emg1_phone2 #</h5>"
                              + "<h5>緊急連絡人2： #: emg2_displayname #</h5>"
                              + "<h5>主要電話： #: emg2_phone1 #</h5>"
                              + "<h5>備用電話： #: emg2_phone2 #</h5>"
                              + "<h5>核定時數： #: minutes1 #（分）</h5>"
                              + "<h5>自費時數： #: minutes2 #（分）</h5>"
                              + "<h5>備註： #: summary #</h5>"
            });
        });
        //set Data
        refreshData();
    });
    //Event-驗證資料是否填寫正確
    $(function () {
        //僅能輸入數字&英文
        var rule_1 = /[^\w]/ig;
        //僅能輸入數字
        var rule_2 = /^\d+/;
        $("#Member-Username").blur(function () {
            var username = $(this).val();
            username = username.replace(rule_1, "");
            $("#Member-Username").val(username);
            isUsernameRepeat = true;
            if (username.length < 8) {
                $("#Member-Username_error").text("帳號最少八位！");
            } else if (!isEditMode) {
                $.ajax({
                    type: "POST",
                    url: hostname + "/Supervisor/checkRepeat",
                    dataType: "json",
                    async: true,
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({
                        username: username
                    }),
                    success: function (result) {
                        if (result == 1) {
                            isUsernameRepeat = true;
                        } else {
                            isUsernameRepeat = false;
                        }
                    },
                    error: function () {
                        isUsernameRepeat = true;
                    },
                    complete: function () {
                        if (isUsernameRepeat) {
                            $("#Member-Username_error").text("已被使用！");
                        } else {
                            $("#Member-Username_error").text(null);
                        }
                    }
                });
            }
        });
        $("#Member-Password").blur(function () {
            var password = $(this).val();
            password = password.replace(rule_1, "");
            $("#Member-Password").val(password);
            if (password.length < 8 || (password.length == 0 && !isEditMode)) {
                $("#Member-Password_error").text("密碼最少八位！");
            } else {
                $("#Member-Password_error").text(null);
            }

        });
        $("#Member-Displayname").blur(function () {
            var displayname = $(this).val();
            if (displayname.length < 1) {
                $("#Member-Displayname_error").text("請輸入姓名！");
            } else {
                $("#Member-Displayname_error").text(null);
            }
        });
        $("#Member-Phone1").keyup(function () {
            var phone = $(this).val();
            phone = rule_2.exec(phone)
            $(this).val(phone);
        });
        $("#Member-Phone2").keyup(function () {
            var phone = $(this).val();
            phone = rule_2.exec(phone)
            $(this).val(phone);
        });
    });
    //Event-Click
    $(function () {
        //顯示日曆
        $("#Member-Birthday").click(function () {
            $("#Member-Birthday").data("kendoDatePicker").open("date");
        });
        $("#Member-Attendant-Firstday").click(function () {
            $("#Member-Attendant-Firstday").data("kendoDatePicker").open("date");
        });
        $("#Member-Supervisor-Firstday").click(function () {
            $("#Member-Supervisor-Firstday").data("kendoDatePicker").open("date");
        });
        //依照職位顯示不同表單
        $("#Member-Level").change(function () {
            //不設定延遲的話無法拿到其值
            setTimeout(function () {
                var level = $("#Member-Level > .btn.active > #gender").val();
                switch (level) {
                    default: {
                        $(".Member-Other").hide();
                        $(".Member-Attendant").hide();
                        $(".Member-Employer").hide();
                        $(".Member-Supervisor").hide();
                        break;
                    }
                    case "2": {
                        $(".Member-Other").hide();
                        $(".Member-Attendant").hide();
                        $(".Member-Employer").hide();
                        $(".Member-Supervisor").show();
                        break;
                    }
                    case "3": {
                        $(".Member-Other").show();
                        $(".Member-Attendant").show();
                        $(".Member-Employer").hide();
                        $(".Member-Supervisor").hide();
                        break;
                    }
                    case "4": {
                        $(".Member-Other").show();
                        $(".Member-Attendant").hide();
                        $(".Member-Employer").show();
                        $(".Member-Supervisor").hide();
                        break;
                    }
                }
            }, 100);
        });
        //選擇督導後儲存至變數
        $("#Member-SupervisorList").on('click', 'li a', function () {
            $("#Member-SupervisorListText").text($(this).text());
            var itemHerf = $(this).attr("href");
            supervisorID = itemHerf.replace("#", "");
        });
        $("#btnOpenCreate").click(function () {
            resetForm("Create");
            $("#modal-Member").modal("show");
        });
        $("#btnCreate").click(function () {
            var level = $("#Member-Level > .btn.active > #gender").val();
            if (checkColumn(level) == 0) {
                showProgressBar(true);
                showAlert("close", true, "");
                //必填
                var user = $("#Member-Username").val();
                var pwd = $("#Member-Password").val();
                var name = $("#Member-Displayname").val();
                var phone1 = $("#Member-Phone1").val();
                //選填
                var level = $("#Member-Level > .btn.active > #gender").val();
                var birthday = $("#Member-Birthday").val();
                var sex = $("#Member-Sex > .btn.active > #gender").val();
                var address = $("#Member-county").val() + $("#Member-district").val() + $("#Member-Address").val();
                var phone2 = $("#Member-Phone2").val();
                switch (level) {
                    //Other
                    default: {
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Supervisor/setNewMember_Other",
                                dataType: "json",
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                data: JSON.stringify({
                                    username: user,
                                    password: pwd,
                                    displayname: name,
                                    level: level,
                                    birthday: birthday,
                                    sex: sex,
                                    address: address,
                                    phone1: phone1,
                                    phone2: phone2
                                }),
                                success: function (result) {
                                    showAlert("close", true, "");
                                    if (result == 2) {
                                        showAlert("info", true, "新增成功！");
                                        resetForm(null);
                                        refreshData();
                                    }
                                    else {
                                        showAlert("warning", true, "請確認資料是否正確！");
                                    }
                                },
                                error: function () {
                                    showAlert("error", true, serverErrorMessage);
                                },
                                complete: function () {
                                    showProgressBar(false);
                                }
                            });
                        });
                        break;
                    }
                    case "2": {
                        //必填
                        var firstday = $("#Member-Supervisor-Firstday").val();
                        //選填
                        var lsL = [];
                        $("#Member-Supervisor-License :checked").each(function () {
                            lsL.push($(this).val());
                        });
                        var lsT = [];
                        $("#Member-Supervisor-Train :checked").each(function () {
                            lsT.push($(this).val());
                        });
                        var proBG = $("#Member-Supervisor-proBG").val();
                        var eduBG = $("#Member-Supervisor-eduBG > .radio :checked").val();
                        //ajax
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Supervisor/setNewMember_Supervisor",
                                dataType: "json",
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                data: JSON.stringify({
                                    username: user,
                                    password: pwd,
                                    displayname: name,
                                    level: level,
                                    birthday: birthday,
                                    sex: sex,
                                    address: address,
                                    phone1: phone1,
                                    phone2: phone2,
                                    firstday: firstday,
                                    lsLicense: lsL,
                                    lsTrain: lsT,
                                    proBG: proBG,
                                    eduBG: eduBG
                                }),
                                success: function (result) {
                                    if (result > 2) {
                                        showAlert("info", true, "新增成功！");
                                        resetForm(null);
                                        refreshData();
                                    }
                                    else {
                                        showAlert("warning", true, "請確認資料是否正確！");
                                    }
                                },
                                error: function () {
                                    showAlert("error", true, serverErrorMessage);
                                },
                                complete: function () {
                                    showProgressBar(false);
                                }
                            });
                        });
                        break;
                    }
                        //照服員
                    case "3": {
                        //必填
                        var pay = $("#Member-Attendant-Pay").val();
                        var firstday = $("#Member-Attendant-Firstday").val();
                        //選填
                        var ls = [];
                        $("#Member-Attendant-License :checked").each(function () {
                            ls.push($(this).val());
                        });
                        var summary = $("#Member-Attendant-Summary").val();
                        //ajax
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Supervisor/setNewMember_Attendant",
                                dataType: "json",
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                data: JSON.stringify({
                                    username: user,
                                    password: pwd,
                                    displayname: name,
                                    level: level,
                                    birthday: birthday,
                                    sex: sex,
                                    address: address,
                                    phone1: phone1,
                                    phone2: phone2,
                                    supervisorID: supervisorID,
                                    pay: pay,
                                    firstday: firstday,
                                    lsLicense: ls,
                                    summary: summary
                                }),
                                success: function (result) {
                                    if (result > 2) {
                                        showAlert("info", true, "新增成功！");
                                        resetForm(null);
                                        refreshData();
                                    }
                                    else {
                                        showAlert("warning", true, "請確認資料是否正確！");
                                    }
                                },
                                error: function () {
                                    showAlert("error", true, serverErrorMessage);
                                },
                                complete: function () {
                                    showProgressBar(false);
                                }
                            });
                        });
                        break;
                    }
                        //案主
                    case "4": {
                        //必填
                        var item1 = $("#Member-Employer-item1 > .radio :checked").val();
                        var item2 = $("#Member-Employer-item2 > .radio :checked").val();
                        var lsitme3 = [];
                        $("#Member-Employer-item3 :checked").each(function () {
                            lsitme3.push($(this).val());
                        });
                        var sub = $("#Member-Employer-Sub > .radio :checked").val();
                        var min1 = $("#Member-Employer-Minutes1").val();
                        var min2 = $("#Member-Employer-Minutes2").val();
                        //選填
                        var emg1_Name = $("#Member-Employer-emg1_Displayname").val();
                        var emg1_Phone1 = $("#Member-Employer-emg1_Phone1").val();
                        var emg1_Phone2 = $("#Member-Employer-emg1_Phone2").val();
                        var emg2_Name = $("#Member-Employer-emg2_Displayname").val();
                        var emg2_Phone1 = $("#Member-Employer-emg2_Phone1").val();
                        var emg2_Phone2 = $("#Member-Employer-emg2_Phone2").val();
                        var summary = $("#Member-Employer-Summary").val();
                        //ajax
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Supervisor/setNewMember_Employer",
                                dataType: "json",
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                data: JSON.stringify({
                                    username: user,
                                    password: pwd,
                                    displayname: name,
                                    level: level,
                                    birthday: birthday,
                                    sex: sex,
                                    address: address,
                                    phone1: phone1,
                                    phone2: phone2,
                                    supervisorID: supervisorID,
                                    info_employer_sub_uid: sub,
                                    info_employer_item1_uid: item1,
                                    info_employer_item2_uid: item2,
                                    info_employer_item3_uid: lsitme3,
                                    emg1_displayname: emg1_Name,
                                    emg1_phone1: emg1_Phone1,
                                    emg1_phone2: emg1_Phone2,
                                    emg2_displayname: emg2_Name,
                                    emg2_phone1: emg2_Phone1,
                                    emg2_phone2: emg2_Phone2,
                                    minutes1: min1,
                                    minutes2: min2,
                                    summary: summary
                                }),
                                success: function (result) {
                                    if (result > 2) {
                                        showAlert("info", true, "新增成功！");
                                        resetForm(null);
                                        refreshData();
                                    }
                                    else {
                                        showAlert("warning", true, "請確認資料是否正確！");
                                    }
                                },
                                error: function () {
                                    showAlert("error", true, serverErrorMessage);
                                },
                                complete: function () {
                                    showProgressBar(false);
                                }
                            });
                        });
                        break;
                    }
                }
            }
        });
        $("#btnUpdate").click(function () {
            var level = $("#Member-Level > .btn.active > #gender").val();
            if (checkColumn(level) == 0) {
                showProgressBar(true);
                showAlert("close", true, "");
                //必填
                var user = $("#Member-Username").val();
                var pwd = $("#Member-Password").val();
                var name = $("#Member-Displayname").val();
                var phone1 = $("#Member-Phone1").val();
                //選填
                var level = $("#Member-Level > .btn.active > #gender").val();
                var birthday = $("#Member-Birthday").val();
                var sex = $("#Member-Sex > .btn.active > #gender").val();
                var address = $("#Member-county").val() + $("#Member-district").val() + $("#Member-Address").val();
                var phone2 = $("#Member-Phone2").val();
                switch (level) {
                    //Other
                    default: {
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Supervisor/setMember_Other",
                                dataType: "json",
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                data: JSON.stringify({
                                    account_uid: editAccountID,
                                    username: user,
                                    password: pwd,
                                    displayname: name,
                                    level: level,
                                    birthday: birthday,
                                    sex: sex,
                                    address: address,
                                    phone1: phone1,
                                    phone2: phone2
                                }),
                                success: function (result) {
                                    if (result > 0) {
                                        showAlert("info", true, "編輯成功！");
                                        resetForm(null);
                                        refreshData();
                                    }
                                    else {
                                        showAlert("warning", true, "請確認資料是否正確！");
                                    }
                                },
                                error: function () {
                                    showAlert("error", true, serverErrorMessage);
                                },
                                complete: function () {
                                    showProgressBar(false);
                                }
                            });
                        });
                        break;
                    }
                    case "2": {
                        //必填
                        var firstday = $("#Member-Supervisor-Firstday").val();
                        //選填
                        var lsL = [];
                        $("#Member-Supervisor-License :checked").each(function () {
                            lsL.push($(this).val());
                        });
                        var lsT = [];
                        $("#Member-Supervisor-Train :checked").each(function () {
                            lsT.push($(this).val());
                        });
                        var proBG = $("#Member-Supervisor-proBG").val();
                        var eduBG = $("#Member-Supervisor-eduBG > .radio :checked").val();
                        //ajax
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Supervisor/setMember_Supervisor",
                                dataType: "json",
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                data: JSON.stringify({
                                    account_uid: supervisorID,
                                    username: user,
                                    password: pwd,
                                    displayname: name,
                                    level: level,
                                    birthday: birthday,
                                    sex: sex,
                                    address: address,
                                    phone1: phone1,
                                    phone2: phone2,
                                    firstday: firstday,
                                    lsLicense: lsL,
                                    lsTrain: lsT,
                                    proBG: proBG,
                                    eduBG: eduBG
                                }),
                                success: function (result) {
                                    if (result > 0) {
                                        showAlert("info", true, "編輯成功！");
                                        resetForm(null);
                                        refreshData();
                                    }
                                    else {
                                        showAlert("warning", true, "請確認資料是否正確！");
                                    }
                                },
                                error: function () {
                                    showAlert("error", true, serverErrorMessage);
                                },
                                complete: function () {
                                    showProgressBar(false);
                                }
                            });
                        });
                        break;
                    }
                        //照服員
                    case "3": {
                        //必填
                        var pay = $("#Member-Attendant-Pay").val();
                        var firstday = $("#Member-Attendant-Firstday").val();
                        //選填
                        var ls = [];
                        $("#Member-Attendant-License :checked").each(function () {
                            ls.push($(this).val());
                        });
                        var summary = $("#Member-Attendant-Summary").val();
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Supervisor/setMember_Attendant",
                                dataType: "json",
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                data: JSON.stringify({
                                    account_uid: editAccountID,
                                    username: user,
                                    password: pwd,
                                    displayname: name,
                                    level: level,
                                    birthday: birthday,
                                    sex: sex,
                                    address: address,
                                    phone1: phone1,
                                    phone2: phone2,
                                    supervisorID: supervisorID,
                                    pay: pay,
                                    firstday: firstday,
                                    lsLicense: ls,
                                    summary: summary
                                }),
                                success: function (result) {
                                    if (result > 0) {
                                        showAlert("info", true, "編輯成功！");
                                        resetForm(null);
                                        refreshData();
                                    }
                                    else {
                                        showAlert("warning", true, "請確認資料是否正確！");
                                    }
                                },
                                error: function () {
                                    showAlert("error", true, serverErrorMessage);
                                },
                                complete: function () {
                                    showProgressBar(false);
                                }
                            });
                        });
                        break;
                    }
                        //案主
                    case "4": {
                        //必填
                        var item1 = $("#Member-Employer-item1 > .radio :checked").val();
                        var item2 = $("#Member-Employer-item2 > .radio :checked").val();
                        var lsitme3 = [];
                        $("#Member-Employer-item3 :checked").each(function () {
                            lsitme3.push($(this).val());
                        });
                        var sub = $("#Member-Employer-Sub > .radio :checked").val();
                        var min1 = $("#Member-Employer-Minutes1").val();
                        var min2 = $("#Member-Employer-Minutes2").val();
                        //選填
                        var emg1_Name = $("#Member-Employer-emg1_Displayname").val();
                        var emg1_Phone1 = $("#Member-Employer-emg1_Phone1").val();
                        var emg1_Phone2 = $("#Member-Employer-emg1_Phone2").val();
                        var emg2_Name = $("#Member-Employer-emg2_Displayname").val();
                        var emg2_Phone1 = $("#Member-Employer-emg2_Phone1").val();
                        var emg2_Phone2 = $("#Member-Employer-emg2_Phone2").val();
                        var summary = $("#Member-Employer-Summary").val();
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Supervisor/setMember_Employer",
                                dataType: "json",
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                data: JSON.stringify({
                                    account_uid: editAccountID,
                                    username: user,
                                    password: pwd,
                                    displayname: name,
                                    level: level,
                                    birthday: birthday,
                                    sex: sex,
                                    address: address,
                                    phone1: phone1,
                                    phone2: phone2,
                                    supervisorID: supervisorID,
                                    info_employer_sub_uid: sub,
                                    info_employer_item1_uid: item1,
                                    info_employer_item2_uid: item2,
                                    info_employer_item3_uid: lsitme3,
                                    emg1_displayname: emg1_Name,
                                    emg1_phone1: emg1_Phone1,
                                    emg1_phone2: emg1_Phone2,
                                    emg2_displayname: emg2_Name,
                                    emg2_phone1: emg2_Phone1,
                                    emg2_phone2: emg2_Phone2,
                                    minutes1: min1,
                                    minutes2: min2,
                                    summary: summary
                                }),
                                success: function (result) {
                                    if (result > 0) {
                                        showAlert("info", true, "編輯成功！");
                                        resetForm(null);
                                        refreshData();
                                    }
                                    else {
                                        showAlert("warning", true, "請確認資料是否正確！");
                                    }
                                },
                                error: function () {
                                    showAlert("error", true, serverErrorMessage);
                                },
                                complete: function () {
                                    showProgressBar(false);
                                }
                            });
                        });
                        break;
                    }
                }
            }
        });
        $("#btnCheckDelete").click(function () {
            $("#deleteModal").modal("show");
        });
        $("#btnDelete").click(function () {
            $.ajax({
                type: "POST",
                url: hostname + "/Supervisor/setMemberlevel",
                dataType: "json",
                async: true,
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    account_uid: editAccountID
                }),
                success: function (result) {
                    if (result == 1) {
                        showAlert("info", true, "刪除成功！");
                        resetForm(null);
                        refreshData();
                    }
                    else {
                        showAlert("warning", true, serverErrorMessage);
                    }
                },
                error: function () {
                    showAlert("error", true, serverErrorMessage);
                },
                complete: function () {
                    showProgressBar(false);
                }
            });
        });
        $("#btnGridExport").click(function () {
            var data;
            var rows_lsAttendant = [{
                cells: [
                  { value: "帳號" },
                  { value: "姓名" },
                  { value: "生日" },
                  { value: "性別" },
                  { value: "地址" },
                  { value: "主要電話" },
                  { value: "備用電話" },
                  { value: "主責督導" },
                  { value: "時薪" },
                  { value: "到職日" },
                  { value: "證照" },
                  { value: "備註" }
                ]
            }];
            var rows_lsEmployer = [{
                cells: [
                  { value: "帳號" },
                  { value: "姓名" },
                  { value: "生日" },
                  { value: "性別" },
                  { value: "地址" },
                  { value: "主要電話" },
                  { value: "備用電話" },
                  { value: "主責督導" },
                  { value: "失能程度" },
                  { value: "經濟程度" },
                  { value: "案主身份" },
                  { value: "緊急連絡人1" },
                  { value: "主要電話" },
                  { value: "備用電話" },
                  { value: "緊急連絡人2" },
                  { value: "主要電話" },
                  { value: "備用電話" },
                  { value: "核定時數" },
                  { value: "自費時數" },
                  { value: "備註" }
                ]
            }];
            var rows_lsSupervisor = [{
                cells: [
                  { value: "帳號" },
                  { value: "姓名" },
                  { value: "生日" },
                  { value: "性別" },
                  { value: "地址" },
                  { value: "主要電話" },
                  { value: "備用電話" },
                  { value: "證照" },
                  { value: "到職日" },
                  { value: "在職訓練" },
                  { value: "專業背景" },
                  { value: "學歷" }
                ]
            }];
            //using fetch, so we can process the data when the request is successfully completed
            data = Grid_lsAttendant;
            for (var i = 0; i < data.length; i++) {
                //push single row for every record
                rows_lsAttendant.push({
                    cells: [
                      { value: data[i].username },
                      { value: data[i].displayname },
                      { value: data[i].birthday },
                      { value: data[i].sex },
                      { value: data[i].address },
                      { value: data[i].phone1 },
                      { value: data[i].phone2 },
                      { value: data[i].supervisorName },
                      { value: data[i].pay },
                      { value: data[i].firstday },
                      { value: data[i].lsLicense },
                      { value: data[i].summary }
                    ]
                })
            }
            data = Grid_lsEmployer;
            for (var i = 0; i < data.length; i++) {
                //push single row for every record
                rows_lsEmployer.push({
                    cells: [
                      { value: data[i].username },
                      { value: data[i].displayname },
                      { value: data[i].birthday },
                      { value: data[i].sex },
                      { value: data[i].address },
                      { value: data[i].phone1 },
                      { value: data[i].phone2 },
                      { value: data[i].supervisorName },
                      { value: data[i].info_employer_item1_uid },
                      { value: data[i].info_employer_item2_uid },
                      { value: data[i].info_employer_item3_uid_str },
                      { value: data[i].emg1_displayname },
                      { value: data[i].emg1_phone1 },
                      { value: data[i].emg1_phone2 },
                      { value: data[i].emg2_displayname },
                      { value: data[i].emg2_phone1 },
                      { value: data[i].emg2_phone2 },
                      { value: data[i].minutes1 },
                      { value: data[i].minutes2 },
                      { value: data[i].summary }
                    ]
                })
            }
            data = Grid_lsSupervisor;
            for (var i = 0; i < data.length; i++) {
                //push single row for every record
                rows_lsSupervisor.push({
                    cells: [
                      { value: data[i].username },
                      { value: data[i].displayname },
                      { value: data[i].birthday },
                      { value: data[i].sex },
                      { value: data[i].address },
                      { value: data[i].phone1 },
                      { value: data[i].phone2 },
                      { value: data[i].lsLicense },
                      { value: data[i].firstday },
                      { value: data[i].lsTrain },
                      { value: data[i].proBG },
                      { value: data[i].eduBG }
                    ]
                })
            }
            var workbook = new kendo.ooxml.Workbook({
                sheets: [{
                    columns: [
                      // Column settings (width)
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true }
                    ],
                    // Title of the sheet
                    title: "Attendant",
                    // Rows of the sheet
                    rows: rows_lsAttendant
                }, {
                    columns: [
                      // Column settings (width)
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true }
                    ],
                    // Title of the sheet
                    title: "Employer",
                    // Rows of the sheet
                    rows: rows_lsEmployer
                }, {
                    columns: [
                      // Column settings (width)
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true },
                      { autoWidth: true }
                    ],
                    // Title of the sheet
                    title: "Supervisor",
                    // Rows of the sheet
                    rows: rows_lsSupervisor
                }]
            });
            kendo.saveAs({
                dataURI: workbook.toDataURL(),
                fileName: "Member.xlsx"
            });

            switch ("") {
                case "": {
                    break;
                }
            }
        });
    });
    function gridEditClick(e) {
        e.preventDefault();
        // get the current table row (tr)
        var tr = $(e.target).closest("tr");
        // get the data bound to the current table row
        var data = this.dataItem(tr);
        resetForm("Edit");
        //暫存該筆資料使用者的編號以供刪除用
        editAccountID = data.account_uid;
        //將當筆資料填入欄位中
        $(function () {
            var type = 0;
            if (lsAttendant.length > 0 && type == 0) {
                for (var i = 0; i < lsAttendant.length; i++) {
                    if (lsAttendant[i].account_uid == data.account_uid) {
                        data = lsAttendant[i];
                        type = 3;
                        break;
                    }
                }
            }
            if (lsEmployer.length > 0 && type == 0) {
                for (var i = 0; i < lsEmployer.length; i++) {
                    if (lsEmployer[i].account_uid == data.account_uid) {
                        data = lsEmployer[i];
                        type = 4;
                        break;
                    }
                }
            }
            if (lsSupervisor.length > 0 && type == 0) {
                for (var i = 0; i < lsSupervisor.length; i++) {
                    if (lsSupervisor[i].account_uid == data.account_uid) {
                        data = lsSupervisor[i];
                        type = 2;
                        break;
                    }
                }
            }
            //必填
            $("#Member-Username").val(data.username);
            $("#Member-Displayname").val(data.displayname);
            $("#Member-Level > .btn.active").removeClass("active");
            $("#Member-Level > .btn > #gender").each(function () {
                if ($(this).val() == type) {
                    $(this).click();
                }
            });
            $("#Member-Sex > .btn.active").removeClass("active");
            $("#Member-Sex > .btn > #gender").each(function () {
                if ($(this).val() == data.sex) {
                    $(this).click();
                }
            });
            $("#Member-Phone1").val(data.phone1);
            //選填
            $("#Member-Birthday").val(data.birthday);
            $("#Member-county > option").each(function () {
                var county = this.value;
                if (data.address.indexOf(county) != -1) {
                    data.address = data.address.replace(county, "");
                    $('#Member-county').val(county).change();
                }
            });
            $("#Member-district > option").each(function () {
                var district = this.value;
                if (data.address.indexOf(district) != -1) {
                    data.address = data.address.replace(district, "");
                    $('#Member-district').val(district).change();
                }
            });
            $("#Member-Address").val(data.address);
            $("#Member-Phone2").val(data.phone2);
            switch (type) {
                case 2: {
                    supervisorID = data.account_uid;
                    for (var i = 0; i < data.lsLicense.length; i++) {
                        $("#Member-Supervisor-License :input").each(function () {
                            if ($(this).val() == data.lsLicense[i]) {
                                $(this).prop("checked", true);
                            }
                        });
                    }
                    $("#Member-Supervisor-Firstday").val(data.firstday);
                    for (var i = 0; i < data.lsTrain.length; i++) {
                        $("#Member-Supervisor-Train :input").each(function () {
                            if ($(this).val() == data.lsTrain[i]) {
                                $(this).prop("checked", true);
                            }
                        });
                    }
                    $("#Member-Supervisor-proBG").val(data.proBG);
                    $("#Member-Supervisor-eduBG > .radio :input").each(function () {
                        if ($(this).val() == data.eduBG) {
                            $(this).prop("checked", true);
                        }
                    });
                    break;
                }
                case 3: {
                    supervisorID = data.supervisorID;
                    $("#Member-SupervisorListText").text(getSupervisorNameAndPhone(supervisorID));
                    $("#Member-Attendant-Pay").val(data.pay);
                    $("#Member-Attendant-Firstday").val(data.firstday);
                    for (var i = 0; i < data.lsLicense.length; i++) {
                        $("#Member-Attendant-License :input").each(function () {
                            if ($(this).val() == data.lsLicense[i]) {
                                $(this).prop("checked", true);
                            }
                        });
                    }
                    $("#Member-Attendant-Summary").val(data.summary);
                    break;
                }
                case 4: {
                    supervisorID = data.supervisorID;
                    $("#Member-SupervisorListText").text(getSupervisorNameAndPhone(supervisorID));
                    $("#Member-Employer-emg1_Displayname").val(data.emg1_displayname);
                    $("#Member-Employer-emg1_Phone1").val(data.emg1_phone1);
                    $("#Member-Employer-emg1_Phone2").val(data.emg1_phone2);
                    $("#Member-Employer-emg2_Displayname").val(data.emg2_displayname);
                    $("#Member-Employer-emg2_Phone1").val(data.emg2_phone1);
                    $("#Member-Employer-emg2_Phone2").val(data.emg2_phone2);
                    $("#Member-Employer-Minutes1").val(data.minutes1);
                    $("#Member-Employer-Minutes2").val(data.minutes2);
                    $("#Member-Employer-item1 > .radio :input").each(function () {
                        if ($(this).val() == data.info_employer_item1_uid) {
                            $(this).prop("checked", true);
                        }
                    });
                    $("#Member-Employer-item2 > .radio :input").each(function () {
                        if ($(this).val() == data.info_employer_item2_uid) {
                            $(this).prop("checked", true);
                        }
                    });
                    for (var i = 0; i < data.info_employer_item3_uid.length; i++) {
                        $("#Member-Employer-item3 :input").each(function () {
                            if ($(this).val() == data.info_employer_item3_uid[i]) {
                                $(this).prop("checked", true);
                            }
                        });
                    }
                    $("#Member-Employer-Sub > .radio :input").each(function () {
                        if ($(this).val() == data.info_employer_sub_uid) {
                            $(this).prop("checked", true);
                        }
                    });
                    $("#Member-Employer-Summary").val(data.summary);
                    break;
                }
            }
        });
        isEditMode = true;
        $("#modal-Member").modal("show");
        function getSupervisorNameAndPhone(uid) {
            var str = "";
            for (var i = 0; i < lsSupervisor.length; i++) {
                if (lsSupervisor[i].account_uid == uid) {
                    str = lsSupervisor[i].displayname + "（" + lsSupervisor[i].phone1 + "）";
                }
            }
            return str;
        }
    }
    //日期格式驗證
    function dateCheck(str) {
        var re = new RegExp("^([0-9]{4})[./]{1}([0-9]{1,2})[./]{1}([0-9]{1,2})$");
        var strDataValue;
        var infoValidation;
        if ((strDataValue = re.exec(str)) != null) {
            var i;
            i = parseFloat(strDataValue[1]);
            if (i <= 0 || i > 9999) { /*年*/
                infoValidation = false;
            }
            i = parseFloat(strDataValue[2]);
            if (i <= 0 || i > 12) { /*月*/
                infoValidation = false;
            }
            i = parseFloat(strDataValue[3]);
            if (i <= 0 || i > 31) { /*日*/
                infoValidation = false;
            }
        } else {
            infoValidation = true;
        }
        return infoValidation;
    }
    function checkColumn(level) {
        showAlert("warning", false, "");
        var errorMsg = [];
        var i = 0;
        var user = $("#Member-Username").val();
        var pwd = $("#Member-Password").val();
        var name = $("#Member-Displayname").val();
        var phone = $("#Member-Phone1").val();
        if (user == "" || isUsernameRepeat) {
            errorMsg.push("[帳號]");
            i++;
        }
        if ((pwd == "" || pwd.length < 8) && !isEditMode) {
            errorMsg.push("[密碼]");
            i++;
        }
        if (name == "") {
            errorMsg.push("[姓名]");
            i++;
        }
        if (phone == "") {
            errorMsg.push("[主要電話]");
            i++;
        }
        switch (level) {
            case "2": {
                var firstday = $("#Member-Supervisor-Firstday").val();
                if (firstday == "") {
                    errorMsg.push("[到職日]");
                    i++;
                }
                break;
            }
            case "3": {
                //照服員
                var pay = $("#Member-Attendant-Pay").val();
                var firstday = $("#Member-Attendant-Firstday").val();
                if (supervisorID == 0) {
                    errorMsg.push("[督導]");
                    i++;
                }
                if (pay == "") {
                    errorMsg.push("[時薪]");
                    i++;
                }
                if (firstday == "") {
                    errorMsg.push("[到職日]");
                    i++;
                }
                break;
            }
            case "4": {
                //案主
                var item1 = $("#Member-Employer-item1 > .radio :checked").val();
                var item2 = $("#Member-Employer-item2 > .radio :checked").val();
                var item3 = [];
                $("#Member-Employer-item3 :checked").each(function () {
                    item3.push($(this).val());
                });
                var sub = $("#Member-Employer-Sub > .radio :checked").val();
                var min1 = $("#Member-Employer-Minutes1").val();
                var min2 = $("#Member-Employer-Minutes2").val();
                if (supervisorID == 0) {
                    errorMsg.push("[督導]");
                    i++;
                }
                if (min1 == "") {
                    errorMsg.push("[核定時數]");
                    i++;
                }
                if (min2 == "") {
                    errorMsg.push("[自費時數]");
                    i++;
                }
                if (item1 == null) {
                    errorMsg.push("[失能程度]");
                    i++;
                }
                if (item2 == null) {
                    errorMsg.push("[經濟程度]");
                    i++;
                }
                if (item3 == null) {
                    errorMsg.push("[案主身份]");
                    i++;
                }
                if (sub == null) {
                    errorMsg.push("[補助身份]");
                    i++;
                }
                break;
            }
        }
        var str = "";
        for (var i = 0; i < errorMsg.length; i++) {
            str += errorMsg[i];
            if (i < errorMsg.length - 1) {
                str += ",";
            }
        }
        if (i > 0) {
            showAlert("warning", true, "請檢查以下欄位：" + str);
        }
        return i;
    }
    function resetForm(mode) {
        $("#btnCheckDelete").hide();
        $("#btnUpdate").hide();
        $("#btnCreate").hide();
        var level = $("#Member-Level > .btn.active > #gender").val();
        switch (level) {
            default: {
                $(".Member-Other").hide();
                $(".Member-Attendant").hide();
                $(".Member-Employer").hide();
                $(".Member-Supervisor").hide();
                break;
            }
            case "2": {
                $(".Member-Other").hide();
                $(".Member-Attendant").hide();
                $(".Member-Employer").hide();
                $(".Member-Supervisor").show();
                break;
            }
            case "3": {
                $(".Member-Other").show();
                $(".Member-Attendant").show();
                $(".Member-Employer").hide();
                $(".Member-Supervisor").hide();
                break;
            }
            case "4": {
                $(".Member-Other").show();
                $(".Member-Attendant").hide();
                $(".Member-Employer").show();
                $(".Member-Supervisor").hide();
                break;
            }
        }
        if (mode == "Create") {
            $("#Member-Password").attr("placeholder", "最少八位");
            $("#modal-Member-Title").text("Create");
            $("#btnCreate").show();
            $(".Member-Level").show();
        } else if (mode == "Edit") {
            $("#Member-Password").attr("placeholder", "不更改則留空");
            $("#modal-Member-Title").text("Edit");
            $("#btnCheckDelete").show();
            $("#btnUpdate").show();
            $(".Member-Level").hide();
        }
        if (mode != "Create" || isEditMode) {
            $("#Member-Username_error").text(null);
            $("#Member-Password_error").text(null);
            $("#Member-Displayname_error").text(null);
            //必填
            $("#Member-Username").val("");
            $("#Member-Password").val("");
            $("#Member-Displayname").val("");
            $("#Member-Level > .btn.active").removeClass("active");
            $("#Member-Level > .btn > #gender").each(function () {
                if ($(this).val() == "2") {
                    $(this).click();
                }
            });
            $("#Member-Sex > .btn.active").removeClass("active");
            $("#Member-Sex > .btn > #gender").each(function () {
                if ($(this).val() == "0") {
                    $(this).click();
                }
            });
            $("#Member-Phone1").val("");
            //選填
            $("#Member-Birthday").val("");
            $('#Member-county').val("").change();
            $("#Member-Address").val("");
            $("#Member-Phone2").val("");
            //督導值
            supervisorID = 0;
            $("#Member-SupervisorListText").text("---請選擇---");
            //照服員
            $("#Member-Attendant-Pay").val("");
            $("#Member-Attendant-Firstday").val("");
            $("#Member-Attendant-License :checked").each(function () {
                $(this).prop("checked", false);
            });
            $("#Member-Attendant-Summary").val("");
            //案主
            $("#Member-Employer-emg1_Displayname").val("");
            $("#Member-Employer-emg1_Phone1").val("");
            $("#Member-Employer-emg1_Phone2").val("");
            $("#Member-Employer-emg2_Displayname").val("");
            $("#Member-Employer-emg2_Phone1").val("");
            $("#Member-Employer-emg2_Phone2").val("");
            $("#Member-Employer-Minutes1").val("");
            $("#Member-Employer-Minutes2").val("");
            $("#Member-Employer-item1 > .radio :checked").prop("checked", false);
            $("#Member-Employer-item2 > .radio :checked").prop("checked", false);
            $("#Member-Employer-item3 :checked").each(function () {
                $(this).prop("checked", false);
            });
            $("#Member-Employer-Sub > .radio :checked").prop("checked", false);
            $("#Member-Employer-Summary").val("");
            //督導
            $("#Member-Supervisor-License :checked").each(function () {
                $(this).prop("checked", false);
            });
            $("#Member-Supervisor-Firstday").val("");
            $("#Member-Supervisor-Train :checked").each(function () {
                $(this).prop("checked", false);
            });
            $("#Member-Supervisor-proBG").val("");
            $("#Member-Supervisor-eduBG > .radio :checked").prop("checked", false);
            //暫存UID
            editAccountID = 0;
            isEditMode = false;
        }
    }
    function refreshData() {
        showProgressBar(true);
        lsAttendant = [];
        lsEmployer = [];
        lsSupervisor = [];
        var lsInfo_employer_item1 = [];
        var lsInfo_employer_item2 = [];
        var lsInfo_employer_item3 = [];
        var lsLicense = [];
        var lsSub = [];
        $.ajax({
            type: "POST",
            url: hostname + "/Supervisor/getMemberInitData",
            async: true,
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                lsAttendant = result["memberlist"]["lsAttendant"];
                lsEmployer = result["memberlist"]["lsEmployer"];
                lsSupervisor = result["memberlist"]["lsSupervisor"];
                Grid_lsAttendant = $.extend(true, [], lsAttendant);
                Grid_lsEmployer = $.extend(true, [], lsEmployer);
                Grid_lsSupervisor = $.extend(true, [], lsSupervisor);
                lsInfo_employer_item1 = result["InfoItem"]["lsInfo_employer_item1"];
                lsInfo_employer_item2 = result["InfoItem"]["lsInfo_employer_item2"];
                lsInfo_employer_item3 = result["InfoItem"]["lsInfo_employer_item3"];
                lsLicense = result["InfoItem"]["lsLicense"];
                lsSub = result["InfoItem"]["lsSub"];
            },
            error: function () {
                showAlert("error", true, serverErrorMessage);
            },
            complete: function () {
                $("#Member-SupervisorList").empty();
                $("#Member-Employer-item1").empty();
                $("#Member-Employer-item2").empty();
                $("#Member-Employer-item3").empty();
                $("#Member-Employer-Sub").empty();
                $("#Member-Attendant-License").empty();
                $("#Member-Supervisor-License").empty();
                //取的督導列表
                $(function () {
                    if (Grid_lsSupervisor != null) {
                        $("#Member-SupervisorList").append("<li><a href='#0'>---請選擇---</a></li>");
                        for (var i = 0; i < Grid_lsSupervisor.length; i++) {
                            $("#Member-SupervisorList").append("<li><a href='#" + Grid_lsSupervisor[i].account_uid + "'>"
                                                              + Grid_lsSupervisor[i].displayname + "（" + Grid_lsSupervisor[i].phone1 + "）</a></li>");
                        }
                    }
                    else {
                        showAlert("error", true, serverErrorMessage);
                    }
                });
                //set lsInfo_employer_item1(案主-失能程度)
                $(function () {
                    if (lsInfo_employer_item1.length > 0) {
                        for (var i = 0; i < lsInfo_employer_item1.length; i++) {
                            var html = "<div class='radio'><label>";
                            html += "<input type='radio' name='lsInfo_employer_item1' value='" + lsInfo_employer_item1[i].uid + "'>";
                            html += lsInfo_employer_item1[i].name;
                            html += "</label></div>";
                            $("#Member-Employer-item1").append(html);
                        }
                    }
                });
                //set lsInfo_employer_item2(案主-經濟程度)
                $(function () {
                    if (lsInfo_employer_item2.length > 0) {
                        for (var i = 0; i < lsInfo_employer_item2.length; i++) {
                            var html = "<div class='radio'><label>";
                            html += "<input type='radio' name='lsInfo_employer_item2' value='" + lsInfo_employer_item2[i].uid + "'>";
                            html += lsInfo_employer_item2[i].name;
                            html += "</label></div>";
                            $("#Member-Employer-item2").append(html);
                        }
                    }
                });
                //set lsInfo_employer_item3(案主-案主身份)
                $(function () {
                    if (lsInfo_employer_item3.length > 0) {
                        for (var i = 0; i < lsInfo_employer_item3.length; i++) {
                            var html = "<div class='checkbox'>";
                            html += "<label><input type='checkbox' value='" + lsInfo_employer_item3[i].uid + "'>";
                            html += lsInfo_employer_item3[i].name;
                            html += "</label></div>";
                            $("#Member-Employer-item3").append(html);
                        }
                    }
                });
                //set lsLicense(照服員-證照)
                $(function () {
                    if (lsLicense.length > 0) {
                        for (var i = 0; i < lsLicense.length; i++) {
                            var html = "<div class='checkbox'>";
                            html += "<label><input type='checkbox' value='" + lsLicense[i].uid + "'>";
                            html += lsLicense[i].name;
                            html += "</label></div>";
                            $("#Member-Attendant-License").append(html);
                        }
                        for (var i = 0; i < lsLicense.length; i++) {
                            var html = "<div class='checkbox'>";
                            html += "<label><input type='checkbox' value='" + lsLicense[i].uid + "'>";
                            html += lsLicense[i].name;
                            html += "</label></div>";
                            $("#Member-Supervisor-License").append(html);
                        }
                    }
                });
                //set lsSub(案主-主要身分)
                $(function () {
                    if (lsSub.length > 0) {
                        for (var i = 0; i < lsSub.length; i++) {
                            var html = "<div class='radio'><label>";
                            html += "<input type='radio' name='lsInfo_employer_sub' value='" + lsSub[i].uid + "'>";
                            html += lsSub[i].name;
                            html += "</label></div>";
                            $("#Member-Employer-Sub").append(html);
                        }
                    }
                });
                //set 0 or 1 to boy or girl
                $(function () {
                    for (var i = 0; i < Grid_lsAttendant.length; i++) {
                        if (Grid_lsAttendant[i]["sex"] == "0") {
                            Grid_lsAttendant[i]["sex"] = "女";
                        } else if (Grid_lsAttendant[i]["sex"] == "1") {
                            Grid_lsAttendant[i]["sex"] = "男";
                        }
                    }
                    for (var i = 0; i < Grid_lsEmployer.length; i++) {
                        if (Grid_lsEmployer[i]["sex"] == "0") {
                            Grid_lsEmployer[i]["sex"] = "女";
                        } else if (Grid_lsEmployer[i]["sex"] == "1") {
                            Grid_lsEmployer[i]["sex"] = "男";
                        }
                    }
                    for (var i = 0; i < Grid_lsSupervisor.length; i++) {
                        if (Grid_lsSupervisor[i]["sex"] == "0") {
                            Grid_lsSupervisor[i]["sex"] = "女";
                        } else if (Grid_lsSupervisor[i]["sex"] == "1") {
                            Grid_lsSupervisor[i]["sex"] = "男";
                        }
                    }
                });
                //set License Name
                $(function () {
                    for (var i = 0; i < Grid_lsAttendant.length; i++) {
                        var str = "";
                        for (var j = 0; j < Grid_lsAttendant[i].lsLicense.length; j++) {
                            str += getLicenseName(Grid_lsAttendant[i].lsLicense[j]);
                            if (j < Grid_lsAttendant[i].lsLicense.length - 1) {
                                str += ",";
                            }
                        }
                        Grid_lsAttendant[i].lsLicense = str;
                    }
                    for (var i = 0; i < Grid_lsSupervisor.length; i++) {
                        var str = "";
                        for (var j = 0; j < Grid_lsSupervisor[i].lsLicense.length; j++) {
                            str += getLicenseName(Grid_lsSupervisor[i].lsLicense[j]);
                            if (j < Grid_lsSupervisor[i].lsLicense.length - 1) {
                                str += ",";
                            }
                        }
                        Grid_lsSupervisor[i].lsLicense = str;
                    }
                });
                //set lsInfo_employer_item1 Name
                $(function () {
                    for (var i = 0; i < Grid_lsEmployer.length; i++) {
                        var str = getlsInfoEmployerItemName(1, Grid_lsEmployer[i].info_employer_item1_uid);
                        Grid_lsEmployer[i].info_employer_item1_uid = str;
                    }
                });
                //set lsInfo_employer_item2 Name
                $(function () {
                    for (var i = 0; i < Grid_lsEmployer.length; i++) {
                        var str = getlsInfoEmployerItemName(2, Grid_lsEmployer[i].info_employer_item2_uid);
                        Grid_lsEmployer[i].info_employer_item2_uid = str;
                    }
                });
                //set lsInfo_employer_item2 Name
                $(function () {
                    for (var i = 0; i < Grid_lsEmployer.length; i++) {
                        var str = getlsInfoEmployerItemName(3, Grid_lsEmployer[i].info_employer_item3_uid);
                        Grid_lsEmployer[i].info_employer_item3_uid = str;
                    }
                });
                //set Train Name
                $(function () {
                    for (var i = 0; i < Grid_lsSupervisor.length; i++) {
                        var str = "";
                        for (var j = 0; j < Grid_lsSupervisor[i].lsTrain.length; j++) {
                            switch (Grid_lsSupervisor[i].lsTrain[j]) {
                                case 1: {
                                    str += "基礎訓練";
                                    break;
                                }
                                case 2: {
                                    str += "進階訓練";
                                    break;
                                }
                                case 3: {
                                    str += "成長訓練";
                                    break;
                                }
                            }
                            if (j != Grid_lsSupervisor[i].lsTrain.length - 1) {
                                str += ",";
                            }
                        }
                        Grid_lsSupervisor[i].lsTrain = str;
                    }
                });
                //set eduBG
                $(function () {
                    for (var i = 0; i < Grid_lsSupervisor.length; i++) {
                        var str = "";
                        switch (Grid_lsSupervisor[i].eduBG) {
                            case 0: {
                                str = "無";
                                break;
                            }
                            case 1: {
                                str = "國小";
                                break;
                            }
                            case 2: {
                                str = "國中";
                                break;
                            }
                            case 3: {
                                str = "高中";
                                break;
                            }
                            case 4: {
                                str = "大學含以上";
                                break;
                            }
                        }
                        Grid_lsSupervisor[i].eduBG = str;
                    }
                });

                //set 照服員 Grid
                $(function () {
                    if (Grid_lsAttendant != null) {
                        var dataSource = new kendo.data.DataSource({
                            data: Grid_lsAttendant,
                            pageSize: 20,
                            group: [{
                                field: "supervisorName",
                                dir: "asc"
                            }]
                        });
                        $('#gridAttendant').data("kendoGrid").setDataSource(dataSource)
                        $('#gridAttendant').data('kendoGrid').dataSource.read();
                        $('#gridAttendant').data('kendoGrid').refresh();
                    }
                });
                //set 案主 Grid
                $(function () {
                    if (Grid_lsEmployer != null) {
                        var dataSource = new kendo.data.DataSource({
                            data: Grid_lsEmployer,
                            pageSize: 20,
                            group: [{
                                field: "supervisorName",
                                dir: "asc"
                            }]
                        });
                        $('#gridEmployer').data("kendoGrid").setDataSource(dataSource)
                        $('#gridEmployer').data('kendoGrid').dataSource.read();
                        $('#gridEmployer').data('kendoGrid').refresh();
                    }
                });
                //set 督導 Grid
                $(function () {
                    if (Grid_lsSupervisor != null) {
                        var dataSource = new kendo.data.DataSource({
                            data: Grid_lsSupervisor,
                            pageSize: 20
                        });
                        $('#gridSupervisor').data("kendoGrid").setDataSource(dataSource)
                        $('#gridSupervisor').data('kendoGrid').dataSource.read();
                        $('#gridSupervisor').data('kendoGrid').refresh();
                    }
                });
                showProgressBar(false);
            }
        });
        function getLicenseName(uid) {
            var str = "";
            if (uid != 0) {
                for (var i = 0; i < lsLicense.length; i++) {
                    if (lsLicense[i].uid == uid) {
                        str = lsLicense[i].name;
                        break;
                    }
                }
            }
            return str;
        }
        function getlsInfoEmployerItemName(type, uid) {
            var str = "";
            if (uid != 0) {
                switch (type) {
                    case 1: {
                        for (var i = 0; i < lsInfo_employer_item1.length; i++) {
                            if (lsInfo_employer_item1[i].uid == uid) {
                                str = lsInfo_employer_item1[i].name;
                                break;
                            }
                        }
                        break;
                    }
                    case 2: {
                        for (var i = 0; i < lsInfo_employer_item2.length; i++) {
                            if (lsInfo_employer_item2[i].uid == uid) {
                                str = lsInfo_employer_item2[i].name;
                                break;
                            }
                        }
                        break;
                    }
                    case 3: {
                        for (var i = 0; i < lsInfo_employer_item3.length; i++) {
                            if (lsInfo_employer_item3[i].uid == uid) {
                                str = lsInfo_employer_item3[i].name;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            return str;
        }
    }
});