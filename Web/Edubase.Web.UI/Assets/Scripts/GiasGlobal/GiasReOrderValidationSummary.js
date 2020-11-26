// order validation summary messages in the same order the fields appear on screen
const GiasReOrderValidationSummary = function () {
  const $errorSummary = $('.govuk-error-summary');
  const errorSummaryList = $errorSummary.find('ul');
  const summaryMessageItems = $errorSummary.find('li');
  const inPageErrorFields = $('#main-content').find('.govuk-form-group--error');

  if (inPageErrorFields.length < 2 || window.blockReOrder) {
    return;
  }
  const reorderedMessages = [];

  for (let i = 0, len = inPageErrorFields.length; i < len; i++) {
    // get an array of all error field ids within the error group - looking at you date fields
    const fieldIds = $(inPageErrorFields[i]).find(':input, legend').map((n, el) => {
      return $(el).attr('id')
    }).get();

    for (let j = 0, len2 = fieldIds.length; j < len2; j++) {
      let fieldId = fieldIds[j];
      // pluck the correct error message for the field from a copy of the original error messages
      const errorMessages = $.grep(summaryMessageItems.slice(), (el) => {
        if (fieldId === $(el).find('a').attr('href').replace('#', '')) {
          return el;
        }
      });

      for (let k = 0, len3 = errorMessages.length; k < len3; k++) {
        const errorMessage = errorMessages[k];
        reorderedMessages.push(`<li>${errorMessage.innerHTML}</li>`);
      }
    }
  }

  errorSummaryList.empty().append(reorderedMessages.join(' '));
};


export default GiasReOrderValidationSummary;
