$(document).ready(function () {
    $('.req').focusout(function () {
        if (!$(this).val().trim()) {
            $(this).addClass("emp");
        }
        else {
            $(this).removeClass("emp");
        }
    });
    $('select, .add').focusout(function () {
        if (!$(this).val().trim()) {
            $(this).addClass("emp");
        }
        else {
            $(this).removeClass("emp");
        }
    });
    $("#b1").click(function () {
        $("#D1").show();
        $("#D2").hide();
        $("#D3").hide();
        $("#D4").hide();
        $("#b1").addClass("btn-outline-primary").removeClass("btn-primary");
        $("#b2").removeClass("btn-outline-primary").addClass("btn-primary");
        $("#b3").removeClass("btn-outline-primary").addClass("btn-primary");
        $("#b4").removeClass("btn-outline-primary").addClass("btn-primary");
    })
    $("#b2").click(function () {
        $("#D2").show();
        $("#D1").hide();
        $("#D3").hide();
        $("#D4").hide();
        $("#b2").addClass("btn-outline-primary").removeClass("btn-primary");
        $("#b1").removeClass("btn-outline-primary").addClass("btn-primary");
        $("#b3").removeClass("btn-outline-primary").addClass("btn-primary");
        $("#b4").removeClass("btn-outline-primary").addClass("btn-primary");
    })
    $("#b3").click(function () {
        $("#D3").show();
        $("#D2").hide();
        $("#D1").hide();
        $("#D4").hide();
        $("#b3").addClass("btn-outline-primary").removeClass("btn-primary");
        $("#b1").removeClass("btn-outline-primary").addClass("btn-primary");
        $("#b2").removeClass("btn-outline-primary").addClass("btn-primary");
        $("#b4").removeClass("btn-outline-primary").addClass("btn-primary");
    })
    $("#b4").click(function () {
        $("#D4").show();
        $("#D2").hide();
        $("#D3").hide();
        $("#D1").hide();
        $("#b4").addClass("btn-outline-primary").removeClass("btn-primary");
        $("#b1").removeClass("btn-outline-primary").addClass("btn-primary");
        $("#b2").removeClass("btn-outline-primary").addClass("btn-primary");
        $("#b3").removeClass("btn-outline-primary").addClass("btn-primary");
    })
})
function TDate() {
    var UserDate = $("#available_date").val().toString();
    if (new Date(UserDate).getDate() < new Date().getDate())
    {
        alert("The Date must be Greater or Equal to today date");
        $("#available_date").val("");
    }
    return;
}

function ImgPre(input) {

    var x = input;
    if ('files' in x) {
        if (x.files.length == 0) {
            $('.carousel-inner').empty();
            alert("file size is 0");
        }
        else {
            $('.carousel-inner').empty();
            for (let i = 0; (i < x.files.length) && (i < 3); i++) {
                if (i == 0) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        imgstring = "<div class=\"carousel-item active\">" + "<img src=" + "\"" + e.target.result + "\"" + "class=\"d-block col-10\" height=\"200\" alt=\"No Images\"> </div> </div>";
                        $(".carousel-inner").wrapInner(imgstring);
                    }
                    reader.readAsDataURL(x.files[0]);
                }
                else {
                    var readeritem = new FileReader();
                    readeritem.onload = function (e) {
                        imgstring = "<div class=\"carousel-item\">" + "<img src=" + "\"" + e.target.result + "\"" + "class=\"d-block col-10\" height=\"200\" alt=\"No Images\"> </div> </div>";
                        $(".carousel-item").after(imgstring);
                    }
                    readeritem.readAsDataURL(x.files[i]);
                }
                
            }
        }

    }
}
function foralert() {
    let flag = 1;
    $('.req').each(function () {
        if (!$(this).val().trim()) {
            $(this).addClass("emp");
            flag = 0;
            return 0;
        }
        else {
            $(this).removeClass("emp");
        }
    });
    $('select').each(function () {
        if (!$(this).val()) {
            $(this).addClass("emp");
            flag = 0;
            return 0;
        }
        else {
            $(this).removeClass("emp");
        }
    });
    $('.add').each(function () {
        if (!$(this).val()) {
            $(this).addClass("emp");
            flag = 0;
            return 0;
        }
        else {
            $(this).removeClass("emp");
        }
    });
    return flag;
}
function validateCheck() {
    if (foralert() === 0) {
        alert("Please Fill The Fields");
        return false;
    }
    else {
        return true;
    }
    
}
/*
    if (input.files[0]) {
        var uploadimg = new FileReader();
        uploadimg.onload = function (displayimg) {
            $("#imgPre").attr('src', displayimg.target.result);
        }
        uploadimg.readAsDataURL(input.files[0]);
    }
*/