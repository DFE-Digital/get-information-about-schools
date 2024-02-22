// detect IE 8 or lower
function oldIE() {
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf('MSIE ');
    if (msie > 0) {
        var ieVersion = parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
        return ieVersion <= 8;
    }
    return false;
}
if (oldIE()) {
    document.querySelector('.govuk-header__logotype-text').style.display = 'inline';
}
