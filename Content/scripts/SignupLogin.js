function checkcustomer() {
    var phone = $("#customer_phone").val().toString();
    if (phone.length != 10) {
        alert("Enter Valid Phone number");
        $("#customer_phone").css("border-color", "red");
        return false;
    }
    else {
        return true;
    }
}
function checkseller() {
    var phone = $("#seller_phone").val().toString();
    if (phone.length != 10) {
        alert("Enter Valid Phone number");
        $("#seller_phone").css("border-color", "red");
        return false;
    }
    else {
        return true;
    }
}
$(document).ready(function () {
    $("#login").click(function () {
        $(".login").show();
        $(".seller").hide();
        $(".customer").hide();
        $("#login").addClass("click");
        $("#seller").removeClass("click");
        $("#customer").removeClass("click");
    });
    $("#seller").click(function () {
        $(".seller").show();
        $(".login").hide();
        $(".customer").hide();
        $("#seller").addClass("click");
        $("#login").removeClass("click");
        $("#customer").removeClass("click");
    });
    $("#customer").click(function () {
        $(".customer").show();
        $(".login").hide();
        $(".seller").hide();
        $("#customer").addClass("click");
        $("#login").removeClass("click");
        $("#seller").removeClass("click");
    });
    $(".content").click(function () {
        $("#alert").hide();
    });

});