function throttle(fn, delay) {
  var t = null;
  return function () {
    var context = this;
    var args = arguments;
    clearTimeout(t);
    t = setTimeout(function () {
      fn.apply(context, args);
    }, delay);
  };
}

export default throttle;
