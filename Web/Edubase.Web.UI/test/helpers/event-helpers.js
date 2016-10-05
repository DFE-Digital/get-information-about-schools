var EventHelpers = {

  fireEvent: function(caller, event) {
    if (document.createEventObject) {
      caller.fireEvent(event);
    } else {
      var evt = document.createEvent("HTMLEvents");
      evt.initEvent(event, false, true);
      caller.dispatchEvent(evt);
    }
  }
  
};