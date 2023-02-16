function goLogin() {
    var pn = $('#pn').val();
    var pass = $('#pass').val();

    var token = $('input[name="__RequestVerificationToken"]').val();

    $.post("/Home/loginCheck", { pn: pn, pass: pass, __RequestVerificationToken: token })

        .done(function (res) {

            switch (res) {
                case 1: window.location.href = "recept"
                    break;
                case 2: swal("عملیات ناموفق", "پسورد اشتباه است", "error")
                    break;
                case 3: swal("عملیات ناموفق", "چنین کاربری وچود ندارد", "error")

            }


        })

        .fail(function () {

        })

        .always(function () {

        });



}