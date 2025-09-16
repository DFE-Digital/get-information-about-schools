import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';

const $main = $('#main-content');

function getAppointingBodyAutoInput() {
  const $byId = $("#governorAppointingBodyInput");
  if ($byId.length) return $byId;
  return $("#AppointingBodyId")
    .closest(".govuk-form-group, form, body")
    .find(".autocomplete__wrapper input.autocomplete__input, .autocomplete__wrapper input[type='text']")
    .first();
}

function resolveOptionValueByTexts(texts) {
  const $opts = $("#AppointingBodyId").find("option");
  for (const t of texts) {
    const $m = $opts.filter(function () {
      return $(this).text().trim().toLowerCase() === String(t).trim().toLowerCase();
    }).first();
    if ($m.length) return $m.val();
  }
  for (const t of texts) {
    const lt = String(t).trim().toLowerCase();
    const $m = $opts.filter(function () {
      return $(this).text().trim().toLowerCase().startsWith(lt);
    }).first();
    if ($m.length) return $m.val();
  }
  return null;
}

function selectAppointingBodyByTexts(texts, disableAfter = true) {
  const $select = $("#AppointingBodyId");
  const wasDisabled = $select.prop("disabled");
  if (wasDisabled) $select.prop("disabled", false);

  const val = resolveOptionValueByTexts(texts);
  if (val != null) {
    $select.val(String(val)).trigger("change");
    const $auto = getAppointingBodyAutoInput();
    if ($auto.length) {
      const txt = $select.find(`option[value="${val}"]`).text().trim();
      $auto.val(txt).trigger("input").trigger("change");
      try {
        $auto[0].dispatchEvent(new CustomEvent("autocomplete:confirm", { detail: txt, bubbles: true }));
        $auto[0].dispatchEvent(new CustomEvent("autocomplete:confirm", { detail: { selection: txt }, bubbles: true }));
      } catch (e) {
        $auto.trigger("autocomplete:confirm", txt);
        $auto.trigger("autocomplete:confirm", { selection: txt });
      }
      if (disableAfter) $auto.prop("disabled", true);
    }
  }

  if (disableAfter) {
    $select.prop("disabled", true);
  } else if (wasDisabled) {
    $select.prop("disabled", wasDisabled);
  }
}

$(document).ready(function () {
  if ($("#IsOriginalChairOfTrustees").val() === 'true') {
    selectAppointingBodyByTexts([
      "Appointed by SSAT original signatory member(s)",
      "Appointed by SSAT Original signatory member(s)"
    ], true);
  } else if ($("#IsOriginalSignatoryMember").val() === 'true') {
    selectAppointingBodyByTexts([
      "N/A",
      "Not applicable",
      "Not-applicable"
    ], true);
  }
});

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

$("#IsOriginalSignatoryMember").on('change', function (e) {
  if ($("#IsOriginalSignatoryMember").val() === "true") {
    selectAppointingBodyByTexts([
      "N/A",
      "Not applicable",
      "Not-applicable"
    ], true);
  } else {
    $("#AppointingBodyId").prop("disabled", false);
    const $auto = getAppointingBodyAutoInput();
    if ($auto.length) $auto.prop("disabled", false);
  }
});

$("#IsOriginalChairOfTrustees").on('change', function (e) {
  if ($("#IsOriginalChairOfTrustees").val() === "true") {
    selectAppointingBodyByTexts([
      "Appointed by SSAT original signatory member(s)",
      "Appointed by SSAT Original signatory member(s)"
    ], true);
  } else {
    $("#AppointingBodyId").prop("disabled", false);
    const $auto = getAppointingBodyAutoInput();
    if ($auto.length) $auto.prop("disabled", false);
  }
});

$("#governorEdit").submit(function () {
  $("#AppointingBodyId").prop("disabled", "");
});

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

// Note that these IDs will reflect IDs (either auto-generated or manually-specified) within:
// - Edubase.Web.UI\Views\Shared\EditorTemplates\GovernorViewModel.cshtml
const titleInput = document.getElementById('GovernorTitleId');
const firstNameInput = document.getElementById('FirstName');
const middleNameInput = document.getElementById('MiddleName');
const lastNameInput = document.getElementById('LastName');
const fullNameDiv = document.getElementById('governorFullNameDiv');
const governorFullNameInput = document.getElementById('FullName');

if (titleInput != null) {
  titleInput.addEventListener('input', UpdateFullName);
} else {
  console.warn("title input field not found - automatic update of full name display will be limited");
}
if (firstNameInput != null) {
  firstNameInput.addEventListener('input', UpdateFullName);
} else {
  console.warn("first name input field not found - automatic update of full name display will be limited");
}
if (middleNameInput != null) {
  middleNameInput.addEventListener('input', UpdateFullName);
} else {
  console.warn("middle name input field not found - automatic update of full name display will be limited");
}
if (lastNameInput != null) {
  lastNameInput.addEventListener('input', UpdateFullName);
} else {
  console.warn("last name input field not found - automatic update of full name display will be limited");
}
if (fullNameDiv != null) {
  fullNameDiv.style.display = 'block';
} else {
  console.warn("full name div not found - automatic update of full name display will be limited");
}
if (governorFullNameInput == null) {
  console.warn("full name input not found - automatic update of full name display will be limited");
}

/**
 * See also the server-side rendered C# implementation, whose implementation
 * should be kept synchronised with the browser-side JavaScript implementation:
 * - Edubase.Web.UI.Areas.Governors.Models.GovernorViewModel.FullName
 */
function UpdateFullName() {
  // Do not include the title in the "full name", if the title is not provided
  let title = titleInput.options[titleInput.selectedIndex].text;
  if (["Not recorded", "Not-applicable"].indexOf(title) !== -1) {
    title = "";
  }

  // Join the name parts with only a single space between each name part
  const fullNameValue = [title, firstNameInput.value, middleNameInput.value, lastNameInput.value]
    .filter(part => part)
    .map(part => part.trim())
    .join(" ")
    .trim();

  governorFullNameInput.value = fullNameValue;
}
