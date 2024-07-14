



$(document).ready(function () {
    $(".product-view").owlCarousel({
        autoplay: true,
        autoplayTimeout: 3500,
        loop: true,
        margin: 10,
        nav: true,
        responsive: {
            320: {
                items: 1
            },
            600: {
                items: 3
            },
            1000: {
                items: 4
            }
        }
    });
    $(".categories-carousel").owlCarousel({
        autoplay: true,
        autoplayTimeout: 4000,
        loop: true,
        margin: 8,
        nav: true,
        autoHeight: true,
        responsive: {
            320: {
                items: 3
            },
            600: {
                items: 3
            },
            1000: {
                items: 5
            }
        }
    });
    $(".slider_intro").owlCarousel({
        autoplay: true,
        autoplayTimeout: 3500,
        loop: true,
        margin: 3,
        nav: true,
        autoHeight: true,
        responsive: {
            320: {
                items: 1
            },
            600: {
                items: 1
            },
            1000: {
                items: 1
            }
        }
    });
    $('.product_connection').owlCarousel({
        loop: true,
        autoplay: true,
        autoplayTimeout: 2000,
        margin: 2,
        nav: false,
        autoHeight: true,
        responsive: {
            320: {
                items: 5
            },
            600: {
                items: 5
            },
            1000: {
                items: 5
            }
        }
    })
    $(window).on("scroll", function () {
        if ($(this).scrollTop() > 120) {
            $(".header").addClass("is-sticky");
        }
        else {
            $(".header").removeClass("is-sticky");
        }
    });
    $(".header_menu .bx-menu").on("click", function () {
        $(".header_pages").toggle();
    })
    $(".header_menu .bx-dots-horizontal-rounded").on("click", function () {
        $(".header_account").toggle();
    })



    
    




});

