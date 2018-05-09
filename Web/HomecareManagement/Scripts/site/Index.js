var hostname = "http://" + location.host + "/";
var serverErrorMessage = "伺服器發生問題，請稍後在試。";
function showAlert(type, status, text) {
    var s = "<button type='button' class='close' data-dismiss='alert'>"
          + "<span aria-hidden='true'><i class='fa fa-times'></i></span>"
          + "<span class='sr-only'>Close</span></button>";

    switch (type) {
        case "success": {
            if (status) {
                s = "<div class='alert alert-success alert-dismissible' role='alert'>"
                    + s
                    + "<strong>Success!</strong>&nbsp;&nbsp;" + text
                    + "</div>";
                $("#alert-success").append(s);
            } else {
                $("#alert-success").empty();
            }
            break;
        }
        case "info": {
            if (status) {
                s = "<div class='alert alert-info alert-dismissible' role='alert'>"
                    + s
                    + "<strong>" + "(" + kendo.toString(kendo.parseDate(new Date()), 'yyyy/MM/dd HH:mm:ss') + ")" + "</strong>&nbsp;&nbsp;"
                    + text
                    + "</div>";
                $("#alert-info").append(s);
            } else {
                $("#alert-info").empty();
            }
            break;
        }
        case "warning": {
            if (status) {
                s = "<div class='alert alert-warning alert-dismissible' role='alert'>"
                    + s
                    + "<strong>Warning!</strong>&nbsp;&nbsp;" + text
                    + "</div>";
                $("#alert-warning").append(s);
            } else {
                $("#alert-warning").empty();
            }
            break;
        }
        case "error": {
            if (status) {
                s = "<div class='alert alert-danger alert-dismissible' role='alert'>"
                    + s
                    + "<strong>Warning!</strong>&nbsp;&nbsp;" + text
                    + "</div>";
                $("#alert-danger").append(s);
            } else {
                $("#alert-danger").empty();
            }
            break;
        }
        case "close": {
            $("#alert-success").empty();
            $("#alert-info").empty();
            $("#alert-warning").empty();
            $("#alert-danger").empty();
            break;
        }
    }
}
function showProgressBar(status) {
    if (status) {
        if (!$('#progressBarModal').is(':visible')) {
            $('#progressBarModal').modal('show');
            $("#btnProfileProgressBar").val(1);
        }
    } else {
        setTimeout(function () {
            $('#progressBarModal').modal('hide');
            $("#btnProfileProgressBar").val(0);
            $("body").css("padding-right", "0px");
            $("body").css("margin-right", "-17px");
        }, 1000);
    }
}
//取cookies函数    
function getCookie(name) {
    var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
    if (arr != null) {
        return unescape(arr[2]);
    } else {
        window.location.href = hostname + "/Account/Logout";
    }
    return null;

}
$(document).ready(function () {
    kendo.culture("zh-TW");
});