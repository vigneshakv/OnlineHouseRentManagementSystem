function check() {
    var flag = false;
    var pass = $("#new_password").val().toString().trim();
    var user_password = $("#user_password").val().trim();
    if (pass == user_password) {
        if (pass.length >= 8) {
            flag = true;
        }
        else {
            alert("Password required atleast 8 character");
        }
    }
    else {
        alert("Password fields should be same");
    }
    return flag;
    }

