class GiasPolling {
  constructor(opts) {
    this.opts = opts;
    this.init()
  }

  init() {
    const params = window.location.toString().split('?')[1];
    const {pollingUrl} = this.opts;

    const checkStatus = function () {
      // api responses are { state: bool,  redirect: string }
      // state: false = not ready yet, redirect = url to go to once we're ready
      $.ajax({
        url: pollingUrl,
        data: params,
        dataType: 'json',
        error: function () {
          window.location.reload();
        },
        success: function (data) {
          data = JSON.parse(data);
          if (data.status === false) {
            window.setTimeout(checkStatus, 2000);
          } else {
            if (params) {
              window.location = data.redirect + '?' + params;

            } else {
              window.location = data.redirect

            }
          }
        }
      });
    }

    checkStatus();
  }
}

export default GiasPolling;
