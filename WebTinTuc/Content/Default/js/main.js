// Hello.
//
// This is The Scripts used for ___________ Theme
//
//

function main() {

    (function () {
        'use strict';


        // Smooth Scrolling
        //==========================================
        $(function () {
            $('a.scroll').click(function () {
                if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
                    var target = $(this.hash);
                    target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
                    if (target.length) {
                        $('html,body').animate({
                            scrollTop: target.offset().top - 50
                        }, 1000);
                        return false;
                    }
                }
            });
        });

        // scroll top
        //==========================================    
        $(window).scroll(function () {
            $(this).scrollTop() > 500 ? $('.totop').fadeIn() : $('.totop').fadeOut();
        });
        $('.totop').click(function () {
            $('html,body').animate({ scrollTop: 0 }, 500);
            return false;
        })

        //
        var url = window.location;
        // var element = $('ul.nav a').filter(function() {
        //     return this.href == url;
        // }).addClass('active').parent().parent().addClass('in').parent();
        var element = $('ul a').filter(function () {
            return this.href == url;
        }).addClass('actived').parent();

        while (true) {
            if (element.is('li')) {
                element = element.parent().addClass('in').parent();
            } else {
                break;
            }
        }

        //click show-form-search
        $('.show-form-search').on('click', function () {
            $('#form-search').fadeIn('200');
            $('body').append('<div class="__div_fixed_search__"></div>');
        });

        $(document).mouseup(function (e) {
            var container = $("#form-search");

            if (!container.is(e.target) // if the target of the click isn't the container...
                && container.has(e.target).length === 0) // ... nor a descendant of the container
            {
                container.fadeOut('200');
                $('.__div_fixed_search__').remove();
            }
        });

        

    }());


}
main();

// sửa lỗi call back facebook location hash
function facebookCallback() {
    if (window.location.hash == '#_=_') {
        history.replaceState
            ? history.replaceState(null, null, window.location.href.split('#')[0])
            : window.location.hash = '';
    }
}
facebookCallback();

// Hàm này trả về query chưa mã hóa
function gup(name, url) {
    if (!url) url = location.href;
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(url);
    return results == null ? null : results[1];

}
//gup('q', 'hxxp://example.com/?q=abc')

// hàm này giải mã rồi
function getQueryParams(qs) {
    qs = qs.split('+').join(' ');

    var params = {},
        tokens,
        re = /[?&]?([^=]+)=([^&]*)/g;

    while (tokens = re.exec(qs)) {
        params[decodeURIComponent(tokens[1])] = decodeURIComponent(tokens[2]);
    }

    return params;
}

function validateRadio(radios) {
    for (i = 0; i < radios.length; ++i) {
        if (radios[i].checked) return true;
    }
    return false;
}

//var query = getQueryParams(document.location.search);
//alert(query.foo);