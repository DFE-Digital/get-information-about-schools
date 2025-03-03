document.addEventListener("DOMContentLoaded", function () {
  function validateArticleDate() {
    let day = document.getElementById("ArticleDate_Day")?.value.trim();
    let month = document.getElementById("ArticleDate_Month")?.value.trim();
    let year = document.getElementById("ArticleDate_Year")?.value.trim();
    let hour = document.getElementById("ArticleDate_Hour")?.value.trim();
    let minute = document.getElementById("ArticleDate_Minute")?.value.trim();

    let isBlank = !day || !month || !year || !hour || !minute;
    let dateObj = { day, month, year };
    let isValidDate = !isBlank && typeof GiasFilterValidation !== "undefined" && !GiasFilterValidation.validateDate(dateObj);
    let isValidHour = !isNaN(hour) && hour !== "" && +hour >= 0 && +hour <= 23;
    let isValidMinute = !isNaN(minute) && minute !== "" && +minute >= 0 && +minute <= 59;
    let isValidTime = !isBlank && isValidHour && isValidMinute;
    let isValid = isValidDate && isValidTime && !isBlank;

    let dateFormGroup = document.querySelector(".govuk-form-group:has(#ArticleDate_Day)");
    let errorMessage = document.getElementById("article-date-error-message");
    let inputFields = document.querySelectorAll("#ArticleDate_Day, #ArticleDate_Month, #ArticleDate_Year, #ArticleDate_Hour, #ArticleDate_Minute");

    let validationSummary = document.querySelector(".govuk-error-summary");
    let summaryList = validationSummary?.querySelector("ul");
    let summaryMessage = "The date specified is not valid";

    summaryList?.querySelectorAll("li").forEach(li => {
      if (li.textContent.trim() === summaryMessage) {
        li.remove();
      }
    });

    if (!isValid) {
      dateFormGroup?.classList.add("govuk-form-group--error");
      errorMessage?.classList.remove("hidden");
      inputFields.forEach(input => input.classList.add("govuk-input--error"));

      if (summaryList && !summaryList.innerHTML.includes(summaryMessage)) {
        let errorItem = document.createElement("li");
        errorItem.textContent = summaryMessage;
        summaryList.appendChild(errorItem);
        validationSummary?.classList.remove("hidden");
      }
    } else {
      dateFormGroup?.classList.remove("govuk-form-group--error");
      errorMessage?.classList.add("hidden");
      inputFields.forEach(input => input.classList.remove("govuk-input--error"));

      if (summaryList && summaryList.children.length === 0) {
        validationSummary?.classList.add("hidden");
      }
    }

    return isValid;
  }

  document.querySelectorAll("#ArticleDate_Day, #ArticleDate_Month, #ArticleDate_Year, #ArticleDate_Hour, #ArticleDate_Minute")
    .forEach(input => input.addEventListener("change", validateArticleDate));

  document.querySelector("form")?.addEventListener("submit", function (e) {
    if (!validateArticleDate()) {
      e.preventDefault(); // Stop form submission
      document.querySelector(".govuk-error-summary")?.scrollIntoView({ behavior: "smooth" });
    }
  });
});
