showProgressBar(true);
$(document).ready(function () {
    //督導編號
    var supervisorID = 0;
    var AttendantID = 0;
    var EmployerID = 0;
    //編輯者編號
    var LastEditer = 0
    var pay = 0;
    var date;
    var start;
    var end;
    var editMode = false;
    var taskID = 0;
    //初始化用陣列
    var SupervisorList = [];
    var AttendantList = [];
    var EmployerList = [];
    var ServiceList = [];
    //儲存已選取服務項目
    var serviceItem = [];
    //防止重複與後台連線
    var isBusy_getSupervisorData = false;
    //初始化
    $(function () {
        //kendo_Scheduler初始化
        $(function () {
            $(function () {
                $("#scheduler").kendoScheduler({
                    date: new Date(),
                    workDayStart: new Date("2015/1/1 06:00"),
                    workDayEnd: new Date("2015/1/1 18:00"),
                    selectable: "cell",
                    editable: {
                        change: false,
                        resize: false,
                        move: false,
                        destroy: false,
                        template: $("#event-template").html()
                    },
                    height: 800,
                    showWorkHours: true,
                    views: [
                        {
                            type: "day",
                            selected: false
                        },
                        {
                            type: "week",
                            selected: false,
                        },
                        {
                            type: "month",
                            selected: true
                        }
                    ],
                    edit: scheduler_editEvent,
                    change: scheduler_changeEvent,
                    resources: [
                        {
                            field: "colorId",
                            title: "colorId",
                            dataSource: [
                                { value: 1, color: "#f8a398" },
                                { value: 2, color: "#51a0ed" },
                                { value: 3, color: "#56ca85" },
                                { value: 4, color: "#8556CA" }
                            ]
                        }
                    ]
                });
            });
            function scheduler_editEvent(e) {
                setTimeout(function () {
                    if (e.event.id != 0) {
                        //編輯           
                        $(function () {
                            editMode = true;
                            var data = e.event;
                            taskID = data.taskId;
                            AttendantID = data.AttendantID;
                            EmployerID = data.EmployerID;
                            pay = data.pay;
                            date = kendo.toString(kendo.parseDate(new Date(data.start)), 'yyyy/MM/dd');
                            start = kendo.toString(kendo.parseDate(new Date(data.start)), 'HH:mm');
                            end = kendo.toString(kendo.parseDate(new Date(data.end)), 'HH:mm');
                            serviceItem = data.serviceItem;
                            summary = data.summary;
                            //設定按鈕顯示&隱藏
                            $(function () {
                                $("#chkRepeat").css("display", "none");
                                $("#btnCheckDelect").css("display", "inline");
                                $("#btnUpdate").css("display", "inline");
                                $("#btnSend").css("display", "none");
                            });
                            //欄位值
                            $(function () {
                                $("#datetimepicker_Date").val(date);
                                $("#datetimepicker_Start").val(start);
                                $("#datetimepicker_End").val(end);
                                cheackWorkDateTime();
                                $("#pay").val(pay);
                                for (var i = 0; i < AttendantList.length; i++) {
                                    if (AttendantID == AttendantList[i].account_uid) {
                                        document.getElementById("AttendantListText").innerHTML = AttendantList[i].displayname + "（" + AttendantList[i].phone1 + "）";
                                        getAttendantJobInfo(AttendantID);
                                        break;
                                    }
                                }
                                for (var i = 0; i < EmployerList.length; i++) {
                                    if (EmployerID == EmployerList[i].account_uid) {
                                        document.getElementById("EmployerListText").innerHTML = EmployerList[i].displayname + "（" + EmployerList[i].phone1 + "）";
                                        getEmployerTime(EmployerID);
                                        break;
                                    }
                                }
                                $("#modalTitle").text("Edit");
                                $("#summary").val(summary);
                                //搜尋服務項目
                                $(function () {
                                    $("#serviceItem input[type=checkbox]").each(function () {
                                        $(this).prop("checked", false);
                                    });
                                    $("#serviceItem input[type=checkbox]").each(function () {
                                        for (var i = 0; i < serviceItem.length; i++) {
                                            if (serviceItem[i] == $(this).val()) {
                                                $(this).prop("checked", true);
                                            }
                                        }
                                    });
                                });
                            });
                            $("#createModal").modal("show");
                        });
                    } else {
                        //新增
                        $("#btnCreate").click();
                    }
                    $("#scheduler").data("kendoScheduler").cancelEvent();
                }, 0);
            }
            function scheduler_changeEvent(e) {
                if (e.events.length > 0 && e.events[0].id != 0) {
                    editMode = true;
                    var data = e.events[0];
                    taskID = data.taskId;
                    AttendantID = data.AttendantID;
                    EmployerID = data.EmployerID;
                    pay = data.pay;
                    date = kendo.toString(kendo.parseDate(new Date(data.start)), "yyyy/MM/dd");
                    start = kendo.toString(kendo.parseDate(new Date(data.start)), 'HH:mm');
                    end = kendo.toString(kendo.parseDate(new Date(data.end)), 'HH:mm');
                    serviceItem = data.serviceItem;
                    summary = data.summary;
                    //設定按鈕顯示&隱藏
                    $(function () {
                        $("#chkRepeat").css("display", "none");
                        $("#btnCheckDelect").css("display", "inline");
                        $("#btnUpdate").css("display", "inline");
                        $("#btnSend").css("display", "none");
                    });
                    //欄位值
                    $(function () {
                        $("#datetimepicker_Date").val(date);
                        $("#datetimepicker_Start").val(start);
                        $("#datetimepicker_End").val(end);
                        cheackWorkDateTime();
                        $("#pay").val(pay);
                        for (var i = 0; i < AttendantList.length; i++) {
                            if (AttendantID == AttendantList[i].account_uid) {
                                document.getElementById("AttendantListText").innerHTML = AttendantList[i].displayname + "（" + AttendantList[i].phone1 + "）";
                                getAttendantJobInfo(AttendantID);
                                break;
                            }
                        }
                        for (var i = 0; i < EmployerList.length; i++) {
                            if (EmployerID == EmployerList[i].account_uid) {
                                document.getElementById("EmployerListText").innerHTML = EmployerList[i].displayname + "（" + EmployerList[i].phone1 + "）";
                                getEmployerTime(EmployerID);
                                break;
                            }
                        }
                        $("#modalTitle").text("Edit");
                        $("#summary").val(summary);
                        //搜尋服務項目
                        $(function () {
                            $("#serviceItem input[type=checkbox]").each(function () {
                                $(this).prop("checked", false);
                            });
                            $("#serviceItem input[type=checkbox]").each(function () {
                                for (var i = 0; i < serviceItem.length; i++) {
                                    if (serviceItem[i] == $(this).val()) {
                                        $(this).prop("checked", true);
                                    }
                                }
                            });
                        });
                    });
                    $("#btnEdit").show();
                } else {
                    $("#btnEdit").hide();
                }
            }
        });
        //kendo_datetimepicker初始化
        $(function () {
            var date = $("#datetimepicker_Date").kendoDatePicker({
                format: "yyyy/MM/dd"
            }).data("kendoTimePicker");
            var start = $("#datetimepicker_Start").kendoTimePicker({
                format: "HH:mm"
            }).data("kendoDatePicker");

            var end = $("#datetimepicker_End").kendoTimePicker({
                format: "HH:mm"
            }).data("kendoDatePicker");
        });
        //取得督導列表&服務項目
        $(function () {
            var userUID = getCookie("uid");
            $.ajax({
                type: "POST",
                url: hostname + "/Supervisor/getScheduleInitialData",
                dataType: "json",
                async: true,
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    if (result != null) {
                        SupervisorList = result["SupervisorList"];
                        ServiceList = result["serviceItem"];
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
                                getSupervisorData(supervisorID);
                            }
                        });
                        //Service
                        $(function () {
                            var ServiceItemName = [];
                            if (ServiceList.length > 0) {
                                //Service Item
                                for (var i = 0; i < ServiceList.length; i++) {
                                    var isRepeat = false;
                                    for (var j = 0; j < ServiceItemName.length; j++) {
                                        if (ServiceItemName[j] == ServiceList[i].service_item_uid) {
                                            isRepeat = true;
                                            break;
                                        }
                                    }
                                    if (!isRepeat) {
                                        var html = "<div class='floatDiv' id='ServiceItemName_" + ServiceList[i].service_item_uid + "'></div>";
                                        $("#serviceItem").append(html);
                                        ServiceItemName.push(ServiceList[i].service_item_uid);
                                    }
                                }
                                //Service check add
                                for (var i = 0; i < ServiceList.length; i++) {
                                    var html = "<div class='checkbox'>";
                                    html += "<label><input type='checkbox' value='" + ServiceList[i].service_uid + "'>";
                                    html += ServiceList[i].service_name;
                                    html += "</label></div>";
                                    $("#ServiceItemName_" + ServiceList[i].fk_service_item_uid).append(html);
                                }
                            }
                        });
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
    });
    //選擇督導
    $("#SupervisorList").on('click', 'li a', function () {
        document.getElementById("SupervisorListText").innerHTML = $(this).text();
        var Schedule;
        var itemHerf = $(this).attr("href");
        supervisorID = itemHerf.replace("#", "");
        getSupervisorData(supervisorID);
    });
    //Form
    $(function () {
        //Open Create
        $("#btnCreate").click(function () {
            $("#btnCheckDelect").css("display", "none");
            $("#btnUpdate").css("display", "none");
            $("#btnSend").css("display", "inline");
            $("#modalTitle").text("Create");
            if (editMode) {
                clearOption();
            }
        });
        //選擇照服員
        $("#AttendantList").on('click', 'li a', function () {
            //rest
            $('#editPay input[type=checkbox]').prop("checked", false);
            $("#pay").attr("readonly", true);
            $("#pay").val("");
            pay = 0;
            document.getElementById("AttendantListText").innerHTML = $(this).text();
            document.getElementById("create_AttendantMsg").innerHTML = "";
            //start
            var itemHerf = $(this).attr("href");
            AttendantID = itemHerf.replace("#", "");
            cheackWorkDateTime();
            if (AttendantID != 0) {
                getAttendantJobInfo(AttendantID);
            } else {
                document.getElementById("create_AttendantMsg").innerHTML = "";
                document.getElementById("create_payMsg").innerHTML = "";
            }
        });
        //選擇案主
        $("#EmployerList").on('click', 'li a', function () {
            document.getElementById("EmployerListText").innerHTML = $(this).text();
            document.getElementById("create_EmployerMsg").innerHTML = "";
            var itemHerf = $(this).attr("href");
            EmployerID = itemHerf.replace("#", "");
            if (EmployerID != 0) {
                getEmployerTime(EmployerID);
            } else {
                document.getElementById("create_EmployerMsg").innerHTML = "";
            }
        });
        //檢查時間是否重疊&格式是否正確
        $(function () {
            $("#datetimepicker_Date").click(function () {
                $("#datetimepicker_Date").data("kendoDatePicker").open("date");
            });
            $("#datetimepicker_Start").click(function () {
                $("#datetimepicker_Start").data("kendoTimePicker").open("time");
            });
            $("#datetimepicker_End").click(function () {
                $("#datetimepicker_End").data("kendoTimePicker").open("time");
            });

            $("#datetimepicker_Date").blur(function () {
                cheackWorkDateTime();
            });
            $("#datetimepicker_Start").blur(function () {
                cheackWorkDateTime();
            });
            $("#datetimepicker_End").blur(function () {
                cheackWorkDateTime();
            });
        });
        $('#editPay').on("change", "input[type='checkbox']", function () {
            if ($(this).is(":checked")) {
                $("#pay").attr("readonly", false);
            }
            else if ($(this).is(":not(:checked)")) {
                $("#pay").attr("readonly", true);
            }
        });
        //Create
        $("#btnSend").click(function () {
            if (checkFormValues() == 0) {
                newScheduleData();
            }
        });
        //Edit
        $("#btnUpdate").click(function () {
            if (checkFormValues() == 0) {
                editScheduleData();
            }
        });
        $("#btnDelect").click(function () {
            delectScheduleData();
        })
        //check value
        function checkFormValues() {
            LastEditer = getCookie("uid");
            serviceItem = [];
            $("#serviceItem input[type=checkbox]").each(function () {
                if ($(this).is(":checked")) {
                    serviceItem.push($(this).val());
                }
            });
            var i = 0;
            var errorMsg = [];
            showAlert("warning", false, "");
            if (document.getElementById("AttendantListText").innerHTML == "---請選擇---") {
                errorMsg.push("[照服員]");
                i++;
            }
            if (document.getElementById("EmployerListText").innerHTML == "---請選擇---") {
                errorMsg.push("[案主]");
                i++;
            }
            if ($("#editPay input[type=checkbox]").is(":checked") && $("#pay").val() == "") {
                errorMsg.push("[時薪]");
                i++;
            }
            if ($("#datetimepicker_Date").val() == "") {
                errorMsg.push("[日期]");
                i++;
            }
            if ($("#datetimepicker_Start").val() == "") {
                errorMsg.push("[開始時間]");
                i++;
            }
            if ($("#datetimepicker_End").val() == "") {
                errorMsg.push("[結束時間]");
                i++;
            }
            if (document.getElementById("create_datetimeMsg").innerHTML != "") {
                errorMsg.push("[日期重複]");
                i++;
            }
            if (serviceItem.length < 1) {
                errorMsg.push("[服務項目]");
                i++;
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
        function newScheduleData() {
            showProgressBar(true);
            var tmp = 0;
            if ($("#editPay input[type=checkbox]").is(":checked")) {
                tmp = $("#pay").val();
            } else {
                tmp = pay;
            }
            $.ajax({
                type: "POST",
                url: hostname + "/Supervisor/setScheduleData",
                dataType: "json",
                data: JSON.stringify({
                    AttendantID: AttendantID,
                    EmployerID: EmployerID,
                    LastEditer: LastEditer,
                    pay: tmp,
                    start: start,
                    end: end,
                    serviceItem: serviceItem,
                    Repeat: $("#chkRepeat input[type=checkbox]").is(":checked"),
                    summary: $("#summary").val()
                }),
                async: true,
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    if (result > 0) {
                        showAlert("info", false, "");
                        showAlert("info", true, "成功！");
                        clearOption();
                    } else {
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
        }
        function editScheduleData() {
            showProgressBar(true);
            var tmp = 0;
            if ($("#editPay input[type=checkbox]").is(":checked")) {
                tmp = $("#pay").val();
            } else {
                tmp = pay;
            }
            $.ajax({
                type: "POST",
                url: hostname + "/Supervisor/setNewScheduleData",
                dataType: "json",
                data: JSON.stringify({
                    taskID: taskID,
                    AttendantID: AttendantID,
                    EmployerID: EmployerID,
                    LastEditer: LastEditer,
                    pay: tmp,
                    start: start,
                    end: end,
                    serviceItem: serviceItem,
                    summary: $("#summary").val()
                }),
                async: true,
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    if (result > 0) {
                        showAlert("info", false, "");
                        showAlert("info", true, "成功！");
                        clearOption();
                    } else {
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
        }
        function delectScheduleData() {
            showAlert("close", true, null);
            $.ajax({
                type: "POST",
                url: hostname + "/Supervisor/delectSchedule",
                dataType: "json",
                data: JSON.stringify({
                    taskId: taskID,
                }),
                async: true,
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    debugger;
                    if (result > 0) {
                        showAlert("info", true, "刪除成功！");
                        clearOption();
                    } else {
                        showAlert("error", true, "此班次已有簽到資料，無法刪除！");
                    }
                },
                error: function () {
                    showAlert("error", true, serverErrorMessage);
                },
                complete: function () {
                    showProgressBar(false);
                }
            });
        }
    });
    function clearOption() {
        document.getElementById("AttendantListText").innerHTML = "---請選擇---";
        document.getElementById("create_AttendantMsg").innerHTML = "";
        document.getElementById("EmployerListText").innerHTML = "---請選擇---";
        document.getElementById("create_EmployerMsg").innerHTML = "";
        $('#editPay input[type=checkbox]').prop("checked", false);
        $("#pay").attr("readonly", true);
        $("#pay").val("");
        document.getElementById("create_payMsg").innerHTML = "";
        $("#datetimepicker_Date").val("");
        $("#datetimepicker_Start").val("");
        $("#datetimepicker_End").val("");
        document.getElementById("create_datetimeMsg").innerHTML = "";
        document.getElementById("create_datetimeMsg2").innerHTML = "";
        $("#serviceItem input[type=checkbox]").each(function () {
            if ($(this).is(":checked")) {
                $(this).prop("checked", false);
            }
        });
        $('#chkRepeat input[type=checkbox]').prop("checked", false);
        $("#chkRepeat").css("display", "block");
        $("#summary").val("");
        getSupervisorData(supervisorID);
        AttendantID = 0;
        EmployerID = 0;
        LastEditer = 0;
        pay = 0;
        date = "";
        start = "";
        end = "";
        editMode = false;
        taskID = 0;
        serviceItem = [];
    }
    function cheackWorkDateTime() {
        $("#create_datetimeMsg").text("");
        $("#create_datetimeMsg2").text("");
        date = $("#datetimepicker_Date").val();
        start = date + " " + $("#datetimepicker_Start").val();
        end = date + " " + $("#datetimepicker_End").val();
        if (date != "" && start != "" && end != "") {
            if (Date.parse(start) > Date.parse(end)) {
                $("#datetimepicker_End").val("");
            } else if (AttendantID != 0) {
                $.ajax({
                    type: "POST",
                    url: hostname + "/Supervisor/cheackWorkDateTime",
                    dataType: "json",
                    data: JSON.stringify({
                        AttendantID: AttendantID,
                        EmployerID: EmployerID,
                        start: start,
                        end: end,
                        editMode: editMode,
                        taskID: taskID
                    }),
                    async: true,
                    contentType: 'application/json; charset=utf-8',
                    success: function (result) {
                        var s1 = "";
                        var s2 = "";
                        if (result["RepeatDate"] > 0) {
                            s1 = "與其他時段重複！";
                            $("#datetimepicker_End").val("");
                        } else if (result["RepeatDate"] == 0) {
                            s1 = "";
                        }
                        if (result["WorkTime_Day"] != -1) {
                            s2 = "當日已排定時數：" + result["WorkTime_Day"] + "小時";
                        }
                        document.getElementById("create_datetimeMsg").innerHTML = s1;
                        document.getElementById("create_datetimeMsg2").innerHTML = s2;
                    },
                    error: function () {
                        showAlert("error", true, serverErrorMessage);
                    },
                    complete: function () {
                        showProgressBar(false);
                    }
                });
            }
        }
    }
    function getAttendantJobInfo(AttendantID) {
        $.ajax({
            type: "POST",
            url: hostname + "/Supervisor/getAttendantWorkInfo",
            dataType: "json",
            data: JSON.stringify({
                AttendantID: AttendantID
            }),
            async: true,
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result["worktime"] > 0) {
                    document.getElementById("create_AttendantMsg").innerHTML = "當月時數：" + result["worktime"] + "小時";
                }
                else if (result["worktime"] == null) {
                    showAlert("error", true, serverErrorMessage);
                } else {
                    document.getElementById("create_AttendantMsg").innerHTML = "";
                }
                if (result["pay"] != "" && result["firstday"] != "") {
                    document.getElementById("create_payMsg").innerHTML = "到職日：" + result["firstday"];
                    if (!editMode) {
                        $("#pay").val(result["pay"]);
                        pay = result["pay"];
                    }
                } else {
                    document.getElementById("create_payMsg").innerHTML = "";
                    $("#pay").val("");
                }
            },
            error: function () {
                showAlert("error", true, serverErrorMessage);
            }
        });
    }
    function getSupervisorData(supervisorID) {
        if (!isBusy_getSupervisorData) {
            isBusy_getSupervisorData = true;
            showProgressBar(true);
            $.ajax({
                type: "POST",
                url: hostname + "/Supervisor/getSupervisorData",
                dataType: "json",
                data: JSON.stringify({
                    account_uid: supervisorID,
                }),
                async: true,
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    if (result != null) {
                        AttendantList = result["AttendantList"];
                        EmployerList = result["EmployerList"];
                        Schedule = result["Schedule"];
                        //AttendantList
                        $(function () {
                            $("#AttendantList").empty();
                            $("#AttendantList").append("<li><a href='#0'>---請選擇---</a></li>");
                            for (var i = 0; i < AttendantList.length; i++) {
                                $("#AttendantList").append("<li><a href='#" + AttendantList[i].account_uid + "'>" + AttendantList[i].displayname + "（" + AttendantList[i].phone1 + "）</a></li>");
                            }
                        });
                        //EmployerList
                        $(function () {
                            $("#EmployerList").empty();
                            $("#EmployerList").append("<li><a href='#0'>---請選擇---</a></li>");
                            for (var i = 0; i < EmployerList.length; i++) {
                                $("#EmployerList").append("<li><a href='#" + EmployerList[i].account_uid + "'>" + EmployerList[i].displayname + "（" + EmployerList[i].phone1 + "）</a></li>");
                            }
                        });
                        //刷新kendoScheduler
                        $(function () {
                            var dataSource = new kendo.data.SchedulerDataSource({
                                data: Schedule,
                                schema: {
                                    model: {
                                        id: "taskId",
                                        fields: {
                                            taskId: { from: "taskId", type: "number" },
                                            ownerId: { from: "ownerId", type: "number" },
                                            title: { from: "title", defaultValue: "No title", validation: { required: true } },
                                            start: { type: "date", from: "start" },
                                            end: { type: "date", from: "end" }
                                        }
                                    },
                                },
                            });
                            $("#scheduler").data("kendoScheduler").setDataSource(dataSource);
                        });
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
                    isBusy_getSupervisorData = false;
                }
            });
        }
    }
    function getEmployerTime(EmployerID) {
        $.ajax({
            type: "POST",
            url: hostname + "/Supervisor/getEmployerTime",
            dataType: "json",
            data: JSON.stringify({
                EmployerID: EmployerID
            }),
            async: true,
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result == "") {
                    document.getElementById("create_EmployerMsg").innerHTML = "";
                    showAlert("error", true, serverErrorMessage);
                } else {
                    document.getElementById("create_EmployerMsg").innerHTML = "當月時數：" + result["minutes"] + "小時<br />"
                    + "總時數：" + result["total"] + "小時<br />"
                    + "(核定：" + result["minutes1"] + "小時+" + "自費：" + result["minutes2"] + "小時)";
                }
            },
            error: function () {
                showAlert("error", true, serverErrorMessage);
            }
        });
    }
});