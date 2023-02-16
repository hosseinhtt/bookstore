$(document).ready(function () {
   
    $.post("/Home/getPro")
        .done(function (res) {

            $("#products").empty();

            for (var item in res) {
                $("#products").append(



                    '<div class="swiper-slide box">' +
                    '<div class="icons">' +
                    '<a  id="' + res[item].pkID + '" class="fas fa-plus" onclick="addtocard(this.id)" href="javascript:void(0)"  ></a>' +
                    '<a id="' + res[item].pkID + '" href="javascript:void(0)" class="fas fa-heart" onclick="addtofav(this.id)"></a>' +
                    '<a href="#" class="fas fa-eye"></a>' +
                    '</div>' +
                    '<div class="image">' +
                    ' <img src="/Assets/image/' + res[item].Image + '" />' +
                    '</div>' +
                    '<div class="content">' +
                    '<h3>' + res[item].Name + '</h3 > ' +
                    '<div class="price"> ' + res[item].Price + " " + ' تومان</div>' +

                    '</div>' +
                    '</div>' +





                    '<div class="swiper-button-next"></div>' +
                    '<div class="swiper-button-prev"></div>'




                );
            }
        });



    var a = localStorage.getItem("basket");

        
    $.post("/Home/getBasket", { basket: a})

    

        .done(function (res) {
            var tp = 0;
            var myObj = JSON.parse(res.results);


            for (var item in res.res) {
                

                $("#shopingList").append(

                    '<tr>'+
                       
                    '<td>'+
                    '<div class="item">'+
                    '<div class="item-front">' +
                    '<img src="/Assets/image/' + res.res[item].Image + '" />' +
                    '</div>'+
                   
                    '</div>'+
                    '<p>' + res.res[item].Name + '<span class="itemNum"></span></p>'+
                         
                    '</td>'+
                    '<td>' + res.res[item].Price + ' </td>'+
                    '<td>' +
                    '<input type="number" class="quantity" value="' + myObj[res.res[item].pkID] +'" min="1" readonly/>' +
                    '<a href="#" class="remove">حذف</a>'+
                    '</td>'+
                    ' <td class="itemTotal">' + res.res[item].Price +" " + 'تومان</td>'+
                       
                    '</tr>'






                );
            }
        })
        .fail(function () {
            swal("what happened?", "info")
        });


});

function addtocard(a) {

    var old = localStorage.getItem("basket");

    if (old != null) {

        var neww = old + "," + a;
    }
    else {

        neww = a;
    }
    localStorage.setItem("basket", neww);

    //var cart = [];
    //var itemToAdd = {
    //    a,
    //    count: 1,
    //};
    //if (localStorage.getItem('basket')) {
    //    cart = JSON.parse(localStorage.getItem('basket'));
    //    let item = cart.find(el => el.a === a);
    //    if (!item) {
    //        cart.push(itemToAdd);
    //    } else {
    //        item.count++;
    //    }
    //} else {
    //    cart = [itemToAdd];
    //}
    //localStorage.setItem('cart', JSON.stringify(cart))
    //console.log(localStorage.getItem('cart'))
      
    //let cart = {};
    //if (localStorage.getItem('basket')) {
    //    cart = JSON.parse(localStorage.getItem('basket'));
    //}
    //cart[a] = (cart[a] || 0) + 1;
    //localStorage.setItem('basket', JSON.stringify(basket));
    //console.log(localStorage.getItem('basket'));



    updatebadge();

}

function updatebadge() {

    var items = localStorage.getItem("basket").split(",");

    document.getElementById("shop_badge").innerHTML = items.length;

}


function addtofav(b) {


    var old = localStorage.getItem("fav");

    if (old != null) {

        var neww = old + "," + b;
    }
    else {

        neww = b;
    }
    localStorage.setItem("fav", neww);

    updateFav();

}

function updateFav() {
    var items = localStorage.getItem("fav").split(",");

    document.getElementById("fav_badge").innerHTML = items.length;
}

function getlogin() {
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.post("/Home/check_login", { phone: $("#phone").val(), psw: $("#psw").val(), __RequestVerificationToken: token })

        .done(function (res) {

            switch (res) {
                case 1: window.location.href = "Profile"
                    swal("حساب ایجاد شد", {
                    buttons: false,
                    timer: 2000,
                });
                    
                    //$("#deactive").toggleClass("login-form-container deactive");
                    break;
                case 2: window.location.href = "Index"
                    swal("خوش آمدید", {
                    buttons: false,
                    timer: 2000,
                });
                    break;
                case 3: swal("پسورد اشتباه است", {
                    buttons: false,
                    timer: 2000,
                    
                });


            }
        })
        .fail(function () {
            swal("خطا در برقراری ارتباط", "info")
        });

}