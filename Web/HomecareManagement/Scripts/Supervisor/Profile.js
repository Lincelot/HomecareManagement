$(document).ready(function () {
    //Profile
    $(function () {
        $("#birthday").kendoDatePicker();
        $("#birthday").click(function () {
            $("#birthday").data("kendoDatePicker").open("date");
        });
        //read userinfo
        $(function getUserData() {
            //使用者編號
            var userID = getCookie("uid");
            $.ajax({
                type: "POST",
                url: hostname + "/Account/getUserInfo",
                dataType: "json",
                async: true,
                data: JSON.stringify({
                    uid: userID
                }),
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    if (result != null) {
                        $("#displayname").val(result.displayname);
                        var date = kendo.toString(kendo.parseDate(result.birthday), 'yyyy/MM/dd');
                        $("#birthday").val(date);
                        $("#sex").val(result.sex);
                        if (result.sex == 1) {
                            $("#sex_M").click();
                        } else {
                            $("#sex_F").click();
                        }
                        $("#address").val(result.address);
                        $("#phone1").val(result.phone1);
                        $("#phone2").val(result.phone2);
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
        //edit userinfo    
        $("#btnProfileEdit").click(function () {
            switchInputStatus(false);
        });
        //write userinfo
        $("#btnProfileSend").click(function () {
            showProgressBar(true);
            showAlert("close", true, "");
            var isNull = false;
            var userdata = [];
            userdata.push($("#displayname").val());
            userdata.push($("#birthday").val());
            userdata.push($("#sex").val());
            userdata.push($("#address").val());
            userdata.push($("#phone1").val());
            userdata.push($("#phone2").val());
            for (var i = 0; i < (userdata.length - 1) ; i++) {
                if (userdata[i] == "") {
                    isNull = true;
                    break;
                }
            }
            if (!isNull) {
                //使用者編號
                var userID = getCookie("uid");
                $.ajax({
                    type: "POST",
                    url: hostname + "/Account/setUserInfo",
                    dataType: "json",
                    async: true,
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({
                        displayname: userdata[0],
                        birthday: userdata[1],
                        sex: userdata[2],
                        address: userdata[3],
                        phone1: userdata[4],
                        phone2: userdata[5],
                        account_uid: userID
                    }),
                    success: function (result) {
                        if (result == 1) {
                            showAlert("info", true, "修改成功！");
                            switchInputStatus(true);
                        }
                        else {
                            showAlert("warning", true, "請填寫完整資料！");
                        }
                    },
                    error: function () {
                        showAlert("error", true, serverErrorMessage);
                    }
                });
            } else {
                showAlert("warning", true, "請填寫完整資料！");
            }
            showProgressBar(false);
        });
        //資料欄位驗證
        $(function () {
            //set Sex Value
            $("#sex_M").click(function () {
                $("#sex").val("1");
            });
            $("#sex_F").click(function () {
                $("#sex").val("0");
            });
            //限制資料格式
            $("#phone1").keyup(function (value) {
                e = $("#phone1");
                if (!/^\d+$/.test(value)) {
                    e.val(/^\d+/.exec($(e).val()));
                }
            });
            $("#phone2").keyup(function (value) {
                e = $("#phone2");
                if (!/^\d+$/.test(value)) {
                    e.val(/^\d+/.exec($(e).val()));
                }
            });
            $("#birthday").blur(function () {
                var birthday = $("#birthday").val();
                dateValidationCheck(birthday);
            });
        });
        function dateValidationCheck(str) {
            var re = new RegExp("^([0-9]{4})[./]{1}([0-9]{1,2})[./]{1}([0-9]{1,2})$");
            var strDataValue;
            var infoValidation = true;
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
                infoValidation = false;
            }
            return infoValidation;
        }
        function switchInputStatus(status) {
            if (status) {
                document.getElementById("btnProfileEdit").style.display = "inline";
                document.getElementById("btnProfileSend").style.display = "none";
                document.getElementById("displayname").readOnly = true;
                document.getElementById("address").readOnly = true;
                document.getElementById("phone1").readOnly = true;
                document.getElementById("phone2").readOnly = true;
            }
            else {
                document.getElementById("btnProfileEdit").style.display = "none";
                document.getElementById("btnProfileSend").style.display = "inline";
                document.getElementById("displayname").readOnly = false;
                document.getElementById("address").readOnly = false;
                document.getElementById("phone1").readOnly = false;
                document.getElementById("phone2").readOnly = false;
            }
        }
    });
});