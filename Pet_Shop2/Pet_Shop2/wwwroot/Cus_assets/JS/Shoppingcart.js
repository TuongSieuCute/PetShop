$(document).ready(function () {
    /*window.addEventListener("load", function (event) {
        $.ajax({
            url: '/ShoppingCart/QuantityCart',
            type: "GET",
            success: function (data) {
                console.log(data);
                $("#cart-container").html(data);
            },
            error: function () {
                console.log("Lỗi cập nhật số lượng giỏ hàng");
            }
        });
    });*/
    $(".addproduct").on("click", function (e) {
        //input_quantity_product

        e.preventDefault();
        var id = $(this).data("id");

        var quantity = 1;
        var tQuantity = $(".input_quantity_product").val();

        if (!isNaN(tQuantity) && tQuantity != '') {
            quantity = parseInt(tQuantity);
        }


        $.ajax({
            url: "/ShoppingCart/AddToCart",
            type: "POST",
            dataType: "json",
            data: { productID: id, amount: quantity },
            success: function (data) {
                //alert(data);
                if (data.success) {


                    Toastify({

                        text: "Bạn vừa đặt thành công sản phẩm",
                        close: true,
                        duration: 2000,
                        style: {
                            background: "#28a745",
                        },
                        offset: {
                            x: 0, // horizontal axis - can be a number or a string indicating unity. eg: '2em'
                            y: 70 // vertical axis - can be a number or a string indicating unity. eg: '2em'
                        }

                    }).showToast();
                    /*setTimeout(function () {
                        location.reload();
                    }, 3000); */

                }
                else {
                    location.reload();
                }

            }
        });
        //Cập nhật lại số lượng sản phẩm giỏ hàng
        $.ajax({
            url: '/ShoppingCart/QuantityCart',
            type: "GET",
            success: function (data) {
                console.log(data);
                $("#cart-container").html(data);
            },
            error: function () {
                console.log("Lỗi cập nhật số lượng giỏ hàng");
            }
        });
    });
})
