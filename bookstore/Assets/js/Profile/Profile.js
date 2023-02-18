$(document).ready(function () {




});
function makeEdit() {
    $(".editable").css("background-color", "#fff");
}


function register() {
    
    var fname = $('#fname').val();
    var lname = $('#lname').val();
    var phone = $('#phone').val();
    var email = $('#email').val();
    var password = $('#password').val();
    var address = $('#address').val();

    var token = $('input[name="__RequestVerificationToken"]').val();


    $.post("/Home/updateUser", { fname: fname, lname: lname, phone: phone, email: email, password: password, address: address, __RequestVerificationToken: token })

        .done(function (res) {
            switch (res) {
                case 1:
                    swal("تغییرات انجام شد", {
                        buttons: false,
                        timer: 2000,
                    });

                    break;
                case 2:
                    swal("عملیات ناموفق", {
                        buttons: false,
                        timer: 2000,
                    });
                    break;
            }
        });









}


