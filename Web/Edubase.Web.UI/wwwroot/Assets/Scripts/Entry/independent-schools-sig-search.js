import autocompleteSetup from '../GiasSearchForm/autocomplete-setup';

autocompleteSetup.setUp();

const laButton = '<a id="button-la-{0}" class="link-button font-small remove-suggest-la" data-remove="la-{0}">{1}</a>';
const laInput = '<input name="d" value="{0}" id="la-{0}" type="hidden">';
const $save = $('#set-saver');
const $laId = $('#SelectedLocalAuthoritySetId');
const $laTarget = $('#la-id-target');

const changeFn = function() {
  $laTarget.find('[name="d"], .remove-suggest-la').remove();
  const val = this.value;

  if (val.length === 0) {
    return;
  }

  const selectedSet = window.sets.filter(function(set) {
    return  val === set.id;
  })[0];

  let buttons = '';
  let inputs = '';
  $(selectedSet.localAuthorityIds).each(function() {
    const laId = parseInt(this);
    const laName = window.localAuthorities.filter(function(la) {
      return la.id === laId;
    })[0].name;

    const input = laInput.replace(/\{0\}/g, laId);
    const button = laButton.replace('{1}', laName).replace(/\{0\}/g, laId);

    buttons += button;
    inputs += input;
  });
  $laTarget.prepend(buttons);
  $laTarget.append(inputs);

};

if ($laTarget.find('.remove-suggest-la').length > 0) {
  $save.removeClass('hidden');
}
$(window).on('laSelected',
  function(e) {
    if (e.count === 0) {
      $save.addClass('hidden');
    } else {
      $save.removeClass('hidden');
    }
    $laId.off('change');
    $laId.val($('#SelectedLocalAuthoritySetId option:first').val());
    $laId.on('change', changeFn);
  });

$laId.on('change',changeFn);
