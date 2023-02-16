import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';
import GiasOkCancel from '../GiasModules/GiasModals/GiasOkCancel';
const $main = $('#main-content');

const unloadHandler = new GiasAttachUnload({
  fields: $main.find(':input').not(".choose-governor"),
});

$("#governorEdit").submit(function() {
  $("#save-button").attr("disabled", true);
  return true;
});

const $day = $('#ReplaceGovernorViewModel_AppointmentEndDate_Day');
const $month = $('#ReplaceGovernorViewModel_AppointmentEndDate_Month');
const $year = $('#ReplaceGovernorViewModel_AppointmentEndDate_Year');

function checkAndUpdateDate() {
  if (Number($day.val()) > 0 && Number($month.val()) > 0 && Number($year.val()) > 0) {
    const date = new Date($year.val(), $month.val() - 1, $day.val(), 0, 0, 0, 0);
    date.setDate(date.getDate() + 1);
    $('#AppointmentStartDate_Day').val(date.getDate());
    $('#AppointmentStartDate_Month').val(date.getMonth() + 1);
    $('#AppointmentStartDate_Year').val(date.getFullYear());
  }
}

$day.blur(checkAndUpdateDate);
$month.blur(checkAndUpdateDate);
$year.blur(checkAndUpdateDate);

checkAndUpdateDate();

if (document.getElementById('IsHistoric') && document.getElementById('IsHistoric').value === 'True') {
  $('.warning-message').addClass('hidden');
  const $endDateFields = $('#AppointmentEndDate_Day, #AppointmentEndDate_Month, #AppointmentEndDate_Year');
  const warningShown = false;

  $endDateFields.on('change', function (e) {
    if (!warningShown) {
      $('.warning-message').removeClass('hidden');
      $endDateFields.off('change');
    }
  });
}

$('.choose-governor').on('change', function () {
  unloadHandler.setExitStatus(true);

  let queryParameters = {};
  const queryString = location.search.substring(1);
  const re = /([^&=]+)=([^&]*)/g;
  let m;

  while (m = re.exec(queryString)) {
    queryParameters[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
  }
  queryParameters['gid2'] = $(this).val();
  queryParameters['d'] = $("#ReplaceGovernorViewModel_AppointmentEndDate_Day").val();
  queryParameters['m'] = $("#ReplaceGovernorViewModel_AppointmentEndDate_Month").val();
  queryParameters['y'] = $("#ReplaceGovernorViewModel_AppointmentEndDate_Year").val();
  queryParameters['rag'] = $("#ReinstateAsGovernor").prop("checked");

  location.search = $.param(queryParameters);
});

if ($("#remove-shared-governor").length > 0) {
  let dismissed = false;
  const form = $('form#removal-form');

  form.on('submit', function (e) {
    if (!dismissed) {
      e.preventDefault();
      $(this).okCancel({
        ok: function () {
          dismissed = true;
          $(e.target).find("#remove-shared-governor").click();
        },
        okLabel: "Proceed",
        immediate: true,
        idPrefix: 'close-continue',
        title: 'Are you sure you want to remove this role?',
        content: 'If you do, the role will also be removed from the multi-academy trust and any academies within the trust that share the role.'
      });
      $(this).removeData('okCancel');
    }
  });

}

const titleInput = document.getElementById('governorTitleInput');
const firstNameInput = document.getElementById('FirstName');
const middleNameInput = document.getElementById('MiddleName');
const lastNameInput = document.getElementById('LastName');
const fullNameDiv = document.getElementById('governorFullNameDiv');

if (titleInput != null) {
  titleInput.addEventListener('change', UpdateFullName);
}
if (firstNameInput != null) {
  firstNameInput.addEventListener('change', UpdateFullName);
}
if (middleNameInput != null) {
  middleNameInput.addEventListener('change', UpdateFullName);
}
if (lastNameInput != null) {
  lastNameInput.addEventListener('change', UpdateFullName);
}
if (fullNameDiv != null) {
  fullNameDiv.style.display = 'block';
}

function UpdateFullName() {
  const fullNameInput = document.getElementById('governorFullNameInput');
  let fullname = '';

  if (titleInput.value != null) {
    fullname += titleInput.options[titleInput.selectedIndex].text + ' ';
  }

  if (firstNameInput.value != null && firstNameInput.value != '') {
    fullname += firstNameInput.value + ' ';
  }

  if (middleNameInput.value != null && middleNameInput.value != '') {
    fullname += middleNameInput.value + ' ';
  }

  if (lastNameInput.value != null) {
    fullname += lastNameInput.value;
  }

  fullNameInput.value = fullname;
}
