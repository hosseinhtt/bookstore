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
                    ' <img src="/Assets/image/' + res[item].Image +'" />' +
                    '</div>' +
                    '<div class="content">' +
                    '<h3>' + res[item].Name +'</h3 > ' +
                    '<div class="price"> ' + res[item].Price + " " +' تومان</div>' +

                    '</div>' +
                    '</div>' +



                    

                    '<div class="swiper-button-next"></div>' +
                    '<div class="swiper-button-prev"></div>'




                );
            }
        })







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