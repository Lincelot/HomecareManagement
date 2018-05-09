showProgressBar(true);
$(document).ready(function () {
    var mode = "Admin-tablist-Service";
    var isEditMode = false;
    var editUID = 0;
    //Init
    $(function () {
        resetForm(null);
        //set Service_Item Raido Button
        $(function () {
            var lsService_Item = [];
            $.ajax({
                type: "POST",
                url: hostname + "/Admin/getService_ItemData",
                dataType: "json",
                async: true,
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    lsService_Item = result;
                },
                error: function () {
                    showAlert("error", true, serverErrorMessage);
                },
                complete: function () {
                    if (lsService_Item.length > 0) {
                        for (var i = 0; i < lsService_Item.length; i++) {
                            var html = "<div class='radio'><label>";
                            html += "<input type='radio' name='lsService_Item' value='" + lsService_Item[i].service_uid + "'>";
                            html += lsService_Item[i].service_name;
                            html += "</label></div>";
                            $("#Admin-Service_Item").append(html);
                        }
                    }
                    showProgressBar(false);
                }
            });
        });
        //set Equipment User ComboBox
        $(function () {
            $("#Admin-Equipment-User").kendoComboBox({
                filter: "Contains",
                height: 400,
                dataSource: {
                    transport: {
                        read: {
                            type: "POST",
                            dataType: 'json',
                            contentType: "application/json; charset=utf-8",
                            url: hostname + "/Admin/getFormEquipmentUserData"
                        }
                    },
                    group: { field: "levelName" }
                },
                dataTextField: "showName",
                dataValueField: "account_uid"
            });
        });
        //Grid Service
        $(function () {
            $("#gridService").kendoGrid({
                dataSource: {
                    transport: {
                        read: {
                            dataType: "json",
                            type: "POST",
                            url: hostname + "/Admin/getGridServiceInitData"
                        }
                    },
                    group: {
                        field: "service_item_name",
                        dir: "asc"
                    }
                },
                //排序
                sortable: true,
                columns: [{
                    field: "service_uid",
                    title: "服務編號",
                    hidden: true
                }, {
                    field: "service_name",
                    title: "服務名稱",
                    width: 150
                }, {
                    field: "service_edit_time",
                    title: "異動時間",
                    width: 150
                }, {
                    field: "service_item_uid",
                    title: "項目編號",
                    hidden: true
                }, {
                    field: "service_item_name",
                    title: "項目名稱",
                    hidden: true
                }, {
                    field: "service_edit_time",
                    title: "異動時間",
                    hidden: true
                }, {
                    title: "動作",
                    command: {
                        text: "編輯",
                        click: editItem
                    },
                    width: "90px"
                }]
            });
        });
        //Grid Equipment
        $(function () {
            $("#gridEquipment").kendoGrid({
                dataSource: {
                    transport: {
                        read: {
                            dataType: "json",
                            type: "POST",
                            url: hostname + "/Admin/getGridEquipmentInitData"
                        }
                    },
                    group: {
                        field: "typeName",
                        dir: "asc"
                    },
                    pageSize: 20
                },
                //排序
                sortable: true,
                pageable: {
                    refresh: false,
                    pageSizes: true,
                    buttonCount: 5
                },
                columns: [{
                    field: "EquipmentUID",
                    title: "編號",
                    hidden: true
                }, {
                    field: "account_uid",
                    title: "使用者編號",
                    hidden: true
                }, {
                    field: "typeName",
                    title: "類型",
                    hidden: true
                }, {
                    field: "displayname",
                    title: "姓名",
                    width: 150
                }, {
                    field: "phone",
                    title: "電話",
                    width: 150
                }, {
                    field: "MACAddress",
                    title: "識別碼",
                    width: 150
                }, {
                    field: "summary",
                    title: "備註",
                    width: 150
                }, {
                    field: "edit_time",
                    title: "編輯時間",
                    width: 150
                }, {
                    field: "status",
                    title: "狀態",
                    width: 70
                }, {
                    title: "動作",
                    command: {
                        text: "編輯",
                        click: editItem
                    },
                    width: "90px"
                }]
            });
        });
        //Grid License
        $(function () {
            $("#gridLicense").kendoGrid({
                dataSource: {
                    transport: {
                        read: {
                            dataType: "json",
                            type: "POST",
                            url: hostname + "/Admin/getGridLicenseInitData"
                        }
                    },
                    pageSize: 20
                },
                //排序
                sortable: true,
                pageable: {
                    refresh: false,
                    pageSizes: true,
                    buttonCount: 5
                },
                columns: [{
                    field: "LicenseUID",
                    title: "編號",
                    hidden: true
                }, {
                    field: "name",
                    title: "名稱",
                    width: 150
                }, {
                    field: "summary",
                    title: "備註",
                    width: 150
                }, {
                    field: "edit_time",
                    title: "編輯時間",
                    width: 150
                }, {
                    title: "動作",
                    command: {
                        text: "編輯",
                        click: editItem
                    },
                    width: "90px"
                }]
            });


        });
        function editItem(e) {
            e.preventDefault();
            // get the current table row (tr)
            var tr = $(e.target).closest("tr");
            // get the data bound to the current table row
            var data = this.dataItem(tr);
            switch (mode) {
                case "Admin-tablist-Service": {
                    resetForm("Edit");
                    editUID = data.service_uid;
                    $("#Admin-Service_Item > .radio :input").each(function () {
                        if ($(this).val() == data.service_item_uid) {
                            $(this).prop("checked", true);
                        }
                    });
                    $("#Admin-ServiceName").val(data.service_name);
                    if (data.service_isdelete == 1) {
                        $('#Admin-Service-Hide input[type=checkbox]').prop("checked", true);
                    }
                    $("#modal-Admin").modal("show");
                    break;
                }
                case "Admin-tablist-Equipment": {
                    resetForm("Edit")
                    editUID = data.EquipmentUID;
                    $("#Admin-Equipment-User").data("kendoComboBox").value(data.account_uid);
                    $("#Admin-Equipment-MACAddress").val(data.MACAddress);
                    $("#Admin-Equipment-type > .radio :input").each(function () {
                        if ($(this).val() == data.type) {
                            $(this).prop("checked", true);
                        }
                    });
                    $("#Admin-Equipment-Summary").val(data.summary);
                    if (data.isdelete == 0) {
                        $("#Admin-Equipment-Status input[type=checkbox]").prop("checked", true);
                    } else {
                        $("#Admin-Equipment-Status input[type=checkbox]").prop("checked", false);
                    }
                    $("#modal-Admin").modal("show");
                    break;
                }
                case "Admin-tablist-License": {
                    resetForm("Edit")
                    editUID = data.LicenseUID;
                    $("#Admin-License-Name").val(data.name);
                    $("#Admin-License-Summary").val(data.summary);
                    $("#modal-Admin").modal("show");
                    break;
                }
            }
            isEditMode = true;
        }
    });
    //Click Event
    $(function () {
        //switch mode
        $(".nav-tabs a").click(function () {
            mode = $(this).context.id;
            resetForm(null);
        });
        $("#btnOpenCreate").click(function () {
            switch (mode) {
                case "Admin-tablist-Equipment": {
                    resetForm("Create")
                    $("#modal-Admin").modal("show");
                    break;
                }
                case "Admin-tablist-License": {
                    resetForm("Create")
                    $("#modal-Admin").modal("show");
                    break;
                }
            }
        });
        $("#btnUpdate").click(function () {
            if (checkColumn() == 0) {
                showAlert("close", true, "");
                showProgressBar(true);
                switch (mode) {
                    case "Admin-tablist-Service": {
                        var uid = editUID;
                        var service_Item = $("#Admin-Service_Item > .radio :checked").val();
                        var serviceName = $("#Admin-ServiceName").val();
                        var status = $('#Admin-Service-Hide input[type=checkbox]').is(':checked');
                        if (service_Item == null) {
                            service_Item = 0;
                        }
                        //ajax
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Admin/setServiceNameAndStatus",
                                dataType: "json",
                                data: JSON.stringify({
                                    uid: uid,
                                    service_Item: service_Item,
                                    serviceName: serviceName,
                                    status: status
                                }),
                                async: true,
                                contentType: 'application/json; charset=utf-8',
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
                    case "Admin-tablist-Equipment": {
                        var uid = editUID;
                        var account_uid = $("#Admin-Equipment-User").data("kendoComboBox").value();
                        var mac = $("#Admin-Equipment-MACAddress").val();
                        var type = $("#Admin-Equipment-type > .radio :checked").val();
                        var summary = $("#Admin-Equipment-Summary").val();
                        var status = $("#Admin-Equipment-Status input[type=checkbox]").is(":checked");
                        //ajax
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Admin/setOldEquipment",
                                dataType: "json",
                                data: JSON.stringify({
                                    uid: uid,
                                    account_uid: account_uid,
                                    mac: mac,
                                    type: type,
                                    summary: summary,
                                    status: status
                                }),
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                success: function (result) {
                                    if (result > 0) {
                                        showAlert("info", true, "編輯成功！");
                                        resetForm("Clear");
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
                    case "Admin-tablist-License": {
                        var uid = editUID;
                        var name = $("#Admin-License-Name").val();
                        var summary = $("#Admin-License-Summary").val();
                        //ajax
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Admin/setOldLicense",
                                dataType: "json",
                                data: JSON.stringify({
                                    uid: uid,
                                    name: name,
                                    summary: summary
                                }),
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                success: function (result) {
                                    if (result > 0) {
                                        showAlert("info", true, "編輯成功！");
                                        resetForm("Clear");
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
        $("#btnCreate").click(function () {
            if (checkColumn() == 0) {
                showAlert("close", true, "");
                showProgressBar(true);
                switch (mode) {
                    case "Admin-tablist-Equipment": {
                        var account_uid = $("#Admin-Equipment-User").data("kendoComboBox").value();
                        var mac = $("#Admin-Equipment-MACAddress").val();
                        var type = $("#Admin-Equipment-type > .radio :checked").val();
                        var summary = $("#Admin-Equipment-Summary").val();
                        var status = $("#Admin-Equipment-Status input[type=checkbox]").is(":checked");
                        //ajax
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Admin/setNewEquipment",
                                dataType: "json",
                                data: JSON.stringify({
                                    account_uid: account_uid,
                                    mac: mac,
                                    type: type,
                                    summary: summary,
                                    status: status
                                }),
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                success: function (result) {
                                    if (result > 0) {
                                        showAlert("info", true, "新增成功！");
                                        resetForm("Clear");
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
                    case "Admin-tablist-License": {
                        var name = $("#Admin-License-Name").val();
                        var summary = $("#Admin-License-Summary").val();
                        //ajax
                        $(function () {
                            $.ajax({
                                type: "POST",
                                url: hostname + "/Admin/setNewLicense",
                                dataType: "json",
                                data: JSON.stringify({
                                    name: name,
                                    summary: summary
                                }),
                                async: true,
                                contentType: 'application/json; charset=utf-8',
                                success: function (result) {
                                    if (result > 0) {
                                        showAlert("info", true, "新增成功！");
                                        resetForm("Clear");
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
            showAlert("close", true, "");
            showProgressBar(true);
            switch (mode) {
                case "Admin-tablist-Equipment": {
                    //ajax
                    $(function () {
                        $.ajax({
                            type: "POST",
                            url: hostname + "/Admin/setDelEquipment",
                            dataType: "json",
                            data: JSON.stringify({
                                uid: editUID
                            }),
                            async: true,
                            contentType: 'application/json; charset=utf-8',
                            success: function (result) {
                                if (result > 0) {
                                    showAlert("info", true, "刪除成功！");
                                    resetForm("Clear");
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
                case "Admin-tablist-License": {
                    //ajax
                    $(function () {
                        $.ajax({
                            type: "POST",
                            url: hostname + "/Admin/setDelLicense",
                            dataType: "json",
                            data: JSON.stringify({
                                uid: editUID
                            }),
                            async: true,
                            contentType: 'application/json; charset=utf-8',
                            success: function (result) {
                                if (result > 0) {
                                    showAlert("info", true, "刪除成功！");
                                    resetForm("Clear");
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
        });
    });
    function checkColumn() {
        showAlert("close", true, "");
        var errorMsg = [];
        var i = 0;
        switch (mode) {
            case "Admin-tablist-Service": {
                var service_Item = $("#Admin-Service_Item > .radio :checked").val();
                var serviceName = $("#Admin-ServiceName").val();
                var status = $('#Admin-Service-Hide input[type=checkbox]').is(':checked');
                if (serviceName == "") {
                    errorMsg.push("[服務名稱]");
                    i++;
                }
                if (!status && service_Item == null) {
                    errorMsg.push("[服務類型]");
                    i++;
                }
                break;
            }
            case "Admin-tablist-Equipment": {
                var account_uid = $("#Admin-Equipment-User").data("kendoComboBox").value();
                var mac = $("#Admin-Equipment-MACAddress").val();
                var type = $("#Admin-Equipment-type > .radio :checked").val();
                if (type == null) {
                    errorMsg.push("[類型]");
                    i++;
                }
                if (account_uid == "") {
                    errorMsg.push("[使用者]");
                    i++;
                }
                if (mac == "") {
                    errorMsg.push("[識別碼]");
                    i++;
                }
                break;
            }
            case "Admin-tablist-License": {
                var name = $("#Admin-License-Name").val();
                if (name == "") {
                    errorMsg.push("[名稱]");
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
    function resetForm(type) {
        editUID = 0;
        $("#modal-Admin-Title").text("");
        $("#btnOpenCreate").hide();
        $("#btnCheckDelete").hide();
        $("#btnUpdate").hide();
        $("#btnCreate").hide();
        $(".serviceTable").hide();
        $(".equipmentTable").hide();
        $(".licenseTable").hide();
        if (isEditMode || type == "Clear") {
            //Admin-tablist-Service
            $("#Admin-ServiceName").val("");
            $('#Admin-Service-Hide input[type=checkbox]').prop("checked", false);
            $("#Admin-Service_Item > .radio :checked").prop("checked", false);
            //Admin-tablist-Equipment
            $("#Admin-Equipment-type > .radio :checked").prop("checked", false);
            $("#Admin-Equipment-User").data("kendoComboBox").text("");
            $("#Admin-Equipment-MACAddress").val("");
            $("#Admin-Equipment-Status input[type=checkbox]").prop("checked", false);
            $("#Admin-Equipment-Summary").val("");
            //Admin-tablist-License
            $("#Admin-License-Name").val("");
            $("#Admin-License-Summary").val("");
            isEditMode = false;
        }
        if (type == "Create") {
            $("#modal-Admin-Title").text("Create");
            $("#btnCreate").show();
        } else if (type == "Edit") {
            $("#modal-Admin-Title").text("Edit");
            $("#btnUpdate").show();
        }
        switch (mode) {
            case "Admin-tablist-Service": {
                $(".serviceTable").show();
                break;
            }
            case "Admin-tablist-Equipment": {
                $(".equipmentTable").show();
                $("#btnOpenCreate").show();
                if (type == "Edit") {
                    $("#btnCheckDelete").show();
                }
                break;
            }
            case "Admin-tablist-License": {
                $(".licenseTable").show();
                $("#btnOpenCreate").show();
                if (type == "Edit") {
                    $("#btnCheckDelete").show();
                }
                break;
            }
        }
    }
    function refreshData() {
        $('#gridService').data('kendoGrid').dataSource.read();
        $('#gridService').data('kendoGrid').refresh();
        $('#gridEquipment').data('kendoGrid').dataSource.read();
        $('#gridEquipment').data('kendoGrid').refresh();
        $('#gridLicense').data('kendoGrid').dataSource.read();
        $('#gridLicense').data('kendoGrid').refresh();
    }
});