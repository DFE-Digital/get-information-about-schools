//  cookie management
// https://i.imgur.com/KFGpyES.gif

class DfECookieManager {

  getCookie(name) {
    const nameEQ = name + '=';
    let cookies = document.cookie.split(';');
    for (let i = 0, len = cookies.length; i < len; i++) {
      let cookie = cookies[i];
      while (cookie.charAt(0) === ' ') {
        cookie = cookie.substring(1, cookie.length);
      }
      if (cookie.indexOf(nameEQ) === 0) {
        return decodeURIComponent(cookie.substring(nameEQ.length));
      }
    }
    return null;
  }

  setCookie(name, value, options) {
    if (typeof options === 'undefined') {
      options = {};
    }

    let cookieStr = `${name}=${value};path=/`;

    if (options.days) {
      let date = new Date();

      date.setTime(date.getTime() + (options.days * 24 * 60 * 60 * 1000));
      cookieStr = `${cookieStr};expires=${date.toGMTString()}`;
    }

    if (document.location.protocol === 'https:') {
      cookieStr = cookieStr + '; Secure';
    }
    document.cookie = cookieStr;
  }

  cookie(name, value, options) {
    if (typeof value !== 'undefined') {
      if (!value) {
        return this.setCookie(name, '', {days: -1});
      }

      return this.setCookie(name, value, options);
    } else {
      return this.getCookie(name);
    }
  }

}

export default DfECookieManager;
