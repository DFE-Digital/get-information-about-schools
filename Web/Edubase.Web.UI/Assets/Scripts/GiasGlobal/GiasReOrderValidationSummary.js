
const GiasReOrderValidationSummary = function() {
  const $errorSummary = $('.govuk-error-summary');
  const errorSummaryList = $errorSummary.find('ul');
  const summaryMessageItems = $errorSummary.find('li');
  const inPageErrorFields = $('#main-content').find('.govuk-form-group--error');

  if (inPageErrorFields.length < 2) {
    return;
  }
  const reorderedMessages = [];

  for (let i = 0, len = inPageErrorFields.length; i < len; i++){
    const fieldId = $(inPageErrorFields[i]).find(':input').eq(0).attr('id');
    const errorMessage = $.grep(summaryMessageItems.slice(), (el) => {
      return fieldId.indexOf($(el).find('a').attr('href').replace('#', '')) === 0;
    })[0];

    reorderedMessages.push(`<li>${errorMessage.innerHTML}</li>`);
  }

  errorSummaryList.empty().append(reorderedMessages.join(' '));
};


export default GiasReOrderValidationSummary;
