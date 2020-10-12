import GiasOkCancel from "./GiasOkCancel";

class GiasAttachUnload  {
  constructor(options) {
    this.opts = options || {};
    if (this.opts.fields) {
      this.$fields = $(options.fields);
    } else {
      this.$fields = $('#main-content').find(':input')
    }

    if (window.isConfimingChanges){
      this.attachConfirmationExitWarning();
    }
    else {
      this.preventExitAfterEdit();
    }
  }

  setExitStatus(status) {
    this.canExit = status;
  }

  preventExitAfterEdit() {
    // main edit view
    const $content = $('#main-content');
    const self = this;
    let message = 'Any unsaved changes will be lost.';
    let exitUrl;
    let exitElement;
    this.canExit = true;
    const $escapes = $content.find('[type="submit"]').not('[value="cancel"], .removeProprietor, .govuk-error-summary a');

    $escapes.addClass('js-allow-exit');

    const bindExitEvents = function(){
      /// attach field changes
      $content.on('click', '.js-allow-exit', function () {
        self.canExit = true;
        window.setTimeout(function () {
          self.canExit = false;
        }, 100);
      });

      $content.on('click', 'a, button', (e)=> {
        if (!self.canExit) {
          e.preventDefault();
          exitUrl = $(e.target).attr('href');
          exitElement = $(e.target);

          $('<div/>').okCancel({
            title: 'Are you sure you want to leave this page?',
            content: message,
            immediate: true,
            ok: function () {
              self.setExitStatus(true);
              if (typeof exitUrl !== 'undefined') {
                window.location = exitUrl;
              } else {
                exitElement.click();
              }
            }
          });
        }
      });

      $(window).on('beforeunload', (e) => {
        if (!self.canExit) {
          return message;
        }
      });
    }

    this.$fields.on('change', ()=> {
      self.canExit = false;
      bindExitEvents();
    });

    if (window.isDirty) {
      bindExitEvents();
    }
  }

  attachConfirmationExitWarning() {
    // used on confirmation of edit screen

    $('#cancel-edit, .requires-conf-to-leave').okCancel({
      ok: function() {
        window.location = this.el.getAttribute('href');
      },
      title: 'Are you sure you want to leave this page?',
      content: 'Any unsaved changes will be lost',
      triggerEvent: 'click'
    });
  }

}

export default GiasAttachUnload
