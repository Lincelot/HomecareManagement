showProgressBar(true);
$(document).ready(function () {
    var checkedIds = [];
    //gridRptWorkRecord初始化
    $(function () {
        $("#gridRptWorkRecord").kendoGrid({
            //排序
            sortable: true,
            groupable: true,
            filterable: true,
            pageable: true,
            columns: [{
                field: "select",
                title: "<input id='workRecodecCheckAll', type='checkbox', class='checkbox' />",
                template: "<input type='checkbox' class='checkbox' />",
                sortable: false,
                filterable: false,
                width: 32
            }, {
                field: "AttendantID",
                title: "照服員編號",
                hidden: true
            }, {
                field: "EmployerID",
                title: "案主編號",
                hidden: true
            }, {
                field: "EmployerName",
                title: "案主姓名",
                hidden: true
            }, {
                field: "Year_Month",
                title: "日期"
            }, {
                field: "Displayname",
                title: "照服員姓名",
                width: 150
            }, {
                field: "phone",
                title: "手機",
                width: 150
            }, {
                field: "Count",
                title: "筆數",
                width: 70
            }, {
                field: "Worktime",
                title: "時數(分)",
                width: 100
            }, {
                title: "匯出",
                command: [{
                    text: "Export",
                    click: exportFile
                }],
                width: "90px"
            }]
        });
        $("#gridRptWorkRecord").data("kendoGrid").table.on("click", ".checkbox", selectRow);
        //on click of the checkbox:
        function selectRow() {
            debugger;
            var checked = this.checked,
            row = $(this).closest("tr"),
            grid = $("#gridRptWorkRecord").data("kendoGrid"),
            dataItem = grid.dataItem(row);
            var AttendantID = dataItem.AttendantID;
            var EmployerID = dataItem.EmployerID;
            var Year = dataItem.Year;
            var Month = dataItem.Month;
            var EmployerName = dataItem.EmployerName;
            var AttendantName = dataItem.Displayname;
            if (checked) {
                checkedIds.push({
                    AttendantID: AttendantID,
                    EmployerID: EmployerID,
                    Year: Year,
                    Month: Month,
                    EmployerName: EmployerName,
                    AttendantName: AttendantName
                });
            } else {
                for (var i = 0; i < checkedIds.length; i++) {
                    if (checkedIds[i].AttendantID == AttendantID && checkedIds[i].EmployerID == EmployerID
                        && checkedIds[i].Year == Year && checkedIds[i].Month == Month) {
                        checkedIds.splice(i, 1);
                    }
                }
            }
        }
        var checkTab = true;
        $('#workRecodecCheckAll').click(function () {
            debugger;
            if ($(this).prop("checked")) {
                $('input:checkbox').each(function () {
                    if ($(this).attr("id") == "workRecodecCheckAll")
                        checkTab = true;
                    if ($(this).attr("id") == "caseCheckAll")
                        checkTab = false;
                    if (checkTab && $(this).attr("id") != "workRecodecCheckAll") {
                        $(this).attr('checked', true);
                        $(this).prop('checked', true);
                        var checked = this.checked,
                        row = $(this).closest("tr"),
                        grid = $("#gridRptWorkRecord").data("kendoGrid"),
                        dataItem = grid.dataItem(row);
                        var zxc = $(this).attr("id");
                        var AttendantID = dataItem.AttendantID;
                        var EmployerID = dataItem.EmployerID;
                        var Year = dataItem.Year;
                        var Month = dataItem.Month;
                        var EmployerName = dataItem.EmployerName;
                        var AttendantName = dataItem.Displayname;
                        if (checked) {
                            checkedIds.push({
                                AttendantID: AttendantID,
                                EmployerID: EmployerID,
                                Year: Year,
                                Month: Month,
                                EmployerName: EmployerName,
                                AttendantName: AttendantName
                            });
                        } else {
                            for (var i = 0; i < checkedIds.length; i++) {
                                if (checkedIds[i].AttendantID == AttendantID && checkedIds[i].EmployerID == EmployerID
                                    && checkedIds[i].Year == Year && checkedIds[i].Month == Month) {
                                    checkedIds.splice(i, 1);
                                }
                            }
                        }
                    }
                });
            } else {
                $('input:checkbox').each(function () {
                    if ($(this).attr("id") == "workRecodecCheckAll")
                        checkTab = true;
                    if ($(this).attr("id") == "caseCheckAll")
                        checkTab = false;
                    if (checkTab && $(this).attr("id") != "workRecodecCheckAll") {
                        $(this).attr('checked', false);
                        $(this).prop('checked', false);
                        var checked = this.checked,
                        row = $(this).closest("tr"),
                        grid = $("#gridRptWorkRecord").data("kendoGrid"),
                        dataItem = grid.dataItem(row);
                        var AttendantID = dataItem.AttendantID;
                        var EmployerID = dataItem.EmployerID;
                        var Year = dataItem.Year;
                        var Month = dataItem.Month;
                        var EmployerName = dataItem.EmployerName;
                        var AttendantName = dataItem.Displayname;
                        if (checked) {
                            checkedIds.push({
                                AttendantID: AttendantID,
                                EmployerID: EmployerID,
                                Year: Year,
                                Month: Month,
                                EmployerName: EmployerName,
                                AttendantName: AttendantName
                            });
                        } else {
                            for (var i = 0; i < checkedIds.length; i++) {
                                if (checkedIds[i].AttendantID == AttendantID && checkedIds[i].EmployerID == EmployerID
                                    && checkedIds[i].Year == Year && checkedIds[i].Month == Month) {
                                    checkedIds.splice(i, 1);
                                }
                            }
                        }
                    }
                });
            }
        });
        function exportFile(e) {
            e.preventDefault();
            // get the current table row (tr)
            var tr = $(e.target).closest("tr");
            // get the data bound to the current table row
            var data = this.dataItem(tr);
            if (data != null) {
                var url = hostname + "/Supervisor/Report_WorkRecord?AttendantID=" + data.AttendantID + "&EmployerID=" + data.EmployerID + "&Year=" + data.Year + "&Month=" + data.Month + "&EmployerName=" + data.EmployerName;
                window.open(url, '_blank');
            }
        }
    });
    //gridRptCaseServiceRecord初始化
    $(function () {
        $("#gridRptCaseServiceRecord").kendoGrid({
            //排序
            sortable: true,
            groupable: true,
            filterable: true,
            pageable: true,
            columns: [{
                field: "select",
                title: "<input id='caseCheckAll', type='checkbox', class='checkbox' />",
                template: "<input type='checkbox' class='checkbox' />",
                sortable: false,
                filterable: false,
                width: 32
            }, {
                field: "AttendantID",
                title: "照服員編號",
                hidden: true
            }, {
                field: "EmployerID",
                title: "案主編號",
                hidden: true
            }, {
                field: "EmployerName",
                title: "案主姓名",
                hidden: true
            }, {
                field: "Year_Month",
                title: "Date",
                hidden: true
            }, {
                field: "Displayname",
                title: "照服員姓名",
                width: 150
            }, {
                field: "phone",
                title: "手機",
                width: 150
            }, {
                field: "Count",
                title: "筆數",
                width: 70
            }, {
                field: "Worktime",
                title: "時數(分)",
                width: 100
            }, {
                title: "匯出",
                command: [{
                    text: "Export",
                    click: exportFile
                }],
                width: "90px"
            }]
        });
        $("#gridRptCaseServiceRecord").data("kendoGrid").table.on("click", ".checkbox", selectRow);
        //on click of the checkbox:
        function selectRow() {
            var checked = this.checked,
            row = $(this).closest("tr"),
            grid = $("#gridRptCaseServiceRecord").data("kendoGrid"),
            dataItem = grid.dataItem(row);
            var AttendantID = dataItem.AttendantID;
            var EmployerID = dataItem.EmployerID;
            var Year = dataItem.Year;
            var Month = dataItem.Month;
            var EmployerName = dataItem.EmployerName;
            var AttendantName = dataItem.Displayname;
            if (checked) {
                checkedIds.push({
                    AttendantID: AttendantID,
                    EmployerID: EmployerID,
                    Year: Year,
                    Month: Month,
                    EmployerName: EmployerName,
                    AttendantName: AttendantName
                });
            } else {
                for (var i = 0; i < checkedIds.length; i++) {
                    if (checkedIds[i].AttendantID == AttendantID && checkedIds[i].EmployerID == EmployerID
                        && checkedIds[i].Year == Year && checkedIds[i].Month == Month) {
                        checkedIds.splice(i, 1);
                    }
                }
            }
        }
        $('#caseCheckAll').click(function () {
            debugger;
            if ($(this).prop("checked")) {
                $('input:checkbox').each(function () {
                    if ($(this).attr("id") == "caseCheckAll")
                        checkTab = true;
                    if ($(this).attr("id") == "workRecodecCheckAll")
                        checkTab = false;
                    if (checkTab && $(this).attr("id") != "caseCheckAll") {
                        $(this).attr('checked', true);
                        $(this).prop('checked', true);
                        var checked = this.checked,
                row = $(this).closest("tr"),
                grid = $("#gridRptCaseServiceRecord").data("kendoGrid"),
                dataItem = grid.dataItem(row);
                        var AttendantID = dataItem.AttendantID;
                        var EmployerID = dataItem.EmployerID;
                        var Year = dataItem.Year;
                        var Month = dataItem.Month;
                        var EmployerName = dataItem.EmployerName;
                        var AttendantName = dataItem.Displayname;
                        if (checked) {
                            checkedIds.push({
                                AttendantID: AttendantID,
                                EmployerID: EmployerID,
                                Year: Year,
                                Month: Month,
                                EmployerName: EmployerName,
                                AttendantName: AttendantName
                            });
                        } else {
                            for (var i = 0; i < checkedIds.length; i++) {
                                if (checkedIds[i].AttendantID == AttendantID && checkedIds[i].EmployerID == EmployerID
                                    && checkedIds[i].Year == Year && checkedIds[i].Month == Month) {
                                    checkedIds.splice(i, 1);
                                }
                            }
                        }
                    }
                });
            } else {
                $('input:checkbox').each(function () {
                    if ($(this).attr("id") == "caseCheckAll")
                        checkTab = true;
                    if ($(this).attr("id") == "workRecodecCheckAll")
                        checkTab = false;
                    if (checkTab && $(this).attr("id") != "caseCheckAll") {
                        $(this).attr('checked', false);
                        $(this).prop('checked', false);
                        var checked = this.checked,
                row = $(this).closest("tr"),
                grid = $("#gridRptCaseServiceRecord").data("kendoGrid"),
                dataItem = grid.dataItem(row);
                        var AttendantID = dataItem.AttendantID;
                        var EmployerID = dataItem.EmployerID;
                        var Year = dataItem.Year;
                        var Month = dataItem.Month;
                        var EmployerName = dataItem.EmployerName;
                        var AttendantName = dataItem.Displayname;
                        if (checked) {
                            checkedIds.push({
                                AttendantID: AttendantID,
                                EmployerID: EmployerID,
                                Year: Year,
                                Month: Month,
                                EmployerName: EmployerName,
                                AttendantName: AttendantName
                            });
                        } else {
                            for (var i = 0; i < checkedIds.length; i++) {
                                if (checkedIds[i].AttendantID == AttendantID && checkedIds[i].EmployerID == EmployerID
                                    && checkedIds[i].Year == Year && checkedIds[i].Month == Month) {
                                    checkedIds.splice(i, 1);
                                }
                            }
                        }
                    }
                });
            }
        });
        function exportFile(e) {
            e.preventDefault();
            // get the current table row (tr)
            var tr = $(e.target).closest("tr");
            // get the data bound to the current table row
            var data = this.dataItem(tr);
            if (data != null) {
                var url = hostname + "/Supervisor/Report_CaseServiceRecord?AttendantID=" + data.AttendantID + "&EmployerID=" + data.EmployerID + "&Year=" + data.Year + "&Month=" + data.Month + "&EmployerName=" + data.EmployerName + "&AttendantName=" + data.Displayname;
                window.open(url, '_blank');
            }
        }
    });

    //取得督導列表
    $(function () {
        var userUID = getCookie("uid");
        var supervisorID = 0;
        $.ajax({
            type: "POST",
            url: hostname + "/Supervisor/getReportInitData",
            dataType: "json",
            async: true,
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result != null) {
                    var SupervisorList = result;
                    //Supervisor
                    $(function () {
                        if (SupervisorList.length > 0) {
                            for (var i = 0; i < SupervisorList.length; i++) {
                                $("#SupervisorList").append("<li><a href='#" + SupervisorList[i].account_uid + "'>" + SupervisorList[i].displayname + "（" + SupervisorList[i].phone1 + "）</a></li>");
                                if (userUID == SupervisorList[i].account_uid) {
                                    supervisorID = SupervisorList[i].account_uid;
                                    document.getElementById("SupervisorListText").innerHTML = SupervisorList[i].displayname + "（" + SupervisorList[i].phone1 + "）";
                                }
                            }
                            if (supervisorID == 0 && SupervisorList.length > 0) {
                                supervisorID = SupervisorList[0].account_uid;
                                document.getElementById("SupervisorListText").innerHTML = SupervisorList[0].displayname + "（" + SupervisorList[0].phone1 + "）";
                            }
                        }
                    });
                    if (supervisorID != 0) {
                        getGridRptWorkRecord(supervisorID);
                    }
                }
                else {
                    showAlert("error", true, serverErrorMessage);
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

    //Click Event
    $("#SupervisorList").on('click', 'li a', function () {
        document.getElementById("SupervisorListText").innerHTML = $(this).text();
        var Schedule;
        var itemHerf = $(this).attr("href");
        supervisorID = itemHerf.replace("#", "");
        if (supervisorID != 0) {
            getGridRptWorkRecord(supervisorID);
        }
    });
    $("#btnMultiExportWorkRecord_item").on('click', 'li a', function () {
        var type = $(this).attr("href").replace("#", "");
        var mail = $("#rptWorkRecord_export_email").val();
        showAlert("close", true, null);
        if (checkedIds.length > 1 && mail != "") {
            //Tip
            var $btn = $("#btnMultiExportWorkRecord");
            $btn.button('loading');
            showProgressBar(true);
            showAlert("info", true, "資料將在稍後寄送到您指定的電子信箱。");
            var AttendantID = [];
            var EmployerID = [];
            var Year = [];
            var Month = [];
            var EmployerName = [];
            var AttendantName = [];
            for (var i = 0; i < checkedIds.length; i++) {
                AttendantID.push(checkedIds[i].AttendantID);
                EmployerID.push(checkedIds[i].EmployerID);
                Year.push(checkedIds[i].Year);
                Month.push(checkedIds[i].Month);
                EmployerName.push(checkedIds[i].EmployerName);
                AttendantName.push(checkedIds[i].AttendantName)
            }
            //ajax
            $(function () {
                $.ajax({
                    type: "POST",
                    url: hostname + "/Supervisor/Report_WorkRecord",
                    dataType: "json",
                    async: true,
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({
                        AttendantID: AttendantID,
                        EmployerID: EmployerID,
                        Year: Year,
                        Month: Month,
                        EmployerName: EmployerName,
                        AttendantName: AttendantName,
                        lsEmail: mail,
                        Type: type
                    }),
                    success: function (result) {
                        showAlert("close", true, null);
                        switch (result) {
                            default: {
                                showAlert("success", true, "發送成功！" + result);
                                break;
                            }
                            case '-1': {
                                showAlert("error", true, serverErrorMessage);
                                break;
                            }
                            case '100': {
                                showAlert("warning", true, "電子郵件格式不正確！");
                                break;
                            }
                        }
                    },
                    error: function () {
                        showAlert("close", true, null);
                        showAlert("error", true, serverErrorMessage);
                    },
                    complete: function () {
                        showProgressBar(false);
                        $btn.button('reset');
                    }
                });
            });
        } else {
            var errorMsg = [];
            var str = "";
            if (checkedIds.length < 2) {
                errorMsg.push("[請選擇兩項以上]");
            }
            if (mail == "") {
                errorMsg.push("[請輸入email]");
            }
            for (var i = 0; i < errorMsg.length; i++) {
                str += errorMsg[i];
                if (i < errorMsg.length - 1) {
                    str += ",";
                }
            }
            showAlert("warning", true, str);
        }
    });
    $("#btnMultiExportCaseServiceRecord_item").on('click', 'li a', function () {
        var type = $(this).attr("href").replace("#", "");
        var mail = $("#rptCaseServiceRecord_export_email").val();
        showAlert("close", true, null);
        if (checkedIds.length > 1 && mail != "") {
            //Tip
            var $btn = $("#btnMultiExportCaseServiceRecord");
            $btn.button('loading');
            showProgressBar(true);
            showAlert("info", true, "資料將在稍後寄送到您指定的電子信箱。");
            var AttendantID = [];
            var EmployerID = [];
            var Year = [];
            var Month = [];
            var EmployerName = [];
            var AttendantName = [];
            for (var i = 0; i < checkedIds.length; i++) {
                AttendantID.push(checkedIds[i].AttendantID);
                EmployerID.push(checkedIds[i].EmployerID);
                Year.push(checkedIds[i].Year);
                Month.push(checkedIds[i].Month);
                EmployerName.push(checkedIds[i].EmployerName);
                AttendantName.push(checkedIds[i].AttendantName)
            }
            //ajax
            $(function () {
                $.ajax({
                    type: "POST",
                    url: hostname + "/Supervisor/Report_CaseServiceRecord",
                    dataType: "json",
                    async: true,
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({
                        AttendantID: AttendantID,
                        EmployerID: EmployerID,
                        Year: Year,
                        Month: Month,
                        EmployerName: EmployerName,
                        AttendantName: AttendantName,
                        lsEmail: mail,
                        Type: type
                    }),
                    success: function (result) {
                        showAlert("close", true, null);
                        switch (result) {
                            default: {
                                showAlert("success", true, "發送成功！" + result);
                                break;
                            }
                            case '-1': {
                                showAlert("error", true, serverErrorMessage);
                                break;
                            }
                            case '100': {
                                showAlert("warning", true, "電子郵件格式不正確！");
                                break;
                            }
                        }
                    },
                    error: function () {
                        showAlert("close", true, null);
                        showAlert("error", true, serverErrorMessage);
                    },
                    complete: function () {
                        showProgressBar(false);
                        $btn.button('reset');
                    }
                });
            });
        } else {
            var errorMsg = [];
            var str = "";
            if (checkedIds.length < 2) {
                errorMsg.push("[請選擇兩項以上]");
            }
            if (mail == "") {
                errorMsg.push("[請輸入email]");
            }
            for (var i = 0; i < errorMsg.length; i++) {
                str += errorMsg[i];
                if (i < errorMsg.length - 1) {
                    str += ",";
                }
            }
            showAlert("warning", true, str);
        }
    });

    //取得該督導旗下照服員資料
    function getGridRptWorkRecord(supervisorID) {
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json",
                    url: hostname + "/Supervisor/getGridRptWorkRecord"
                },
                parameterMap: function (data, type) {
                    return JSON.stringify({
                        SupervisorID: supervisorID
                    });
                },
                schema: {
                    data: "SupplierSearchResult.SupplierList",
                    type: 'json'
                }
            },
            pageSize: 20,
            group: [{
                field: "Year_Month",
                dir: "asc"
            }, {
                field: "EmployerName",
                dir: "asc"
            }]
        });
        $("#gridRptWorkRecord").data("kendoGrid").setDataSource(dataSource);
        $("#gridRptWorkRecord").data("kendoGrid").refresh();
        $("#gridRptCaseServiceRecord").data("kendoGrid").setDataSource(dataSource);
        $("#gridRptCaseServiceRecord").data("kendoGrid").refresh();
        checkedIds = [];
    }

});