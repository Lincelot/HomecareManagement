$(document).ready(function () {
    $("#btnLogin").click(function () {
        showProgressBar(true);
    });
    $("#btnReset").click(function () {
        $("#username").val("");
        $("#password").val("");
    });
});