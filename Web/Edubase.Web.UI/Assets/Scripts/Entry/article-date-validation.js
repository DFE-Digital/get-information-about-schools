document.addEventListener("DOMContentLoaded", function () {

  function isValidDate(dayStr, monthStr, yearStr) {
    const day = parseInt(dayStr, 10);
    const month = parseInt(monthStr, 10);
    const year = parseInt(yearStr, 10);
    if (Number.isNaN(day) || Number.isNaN(month) || Number.isNaN(year)) {
      return false;
    }
    const testDate = new Date(year, month - 1, day);
    return (
      testDate.getFullYear() === year &&
      testDate.getMonth() === (month - 1) &&
      testDate.getDate() === day
    );
  }

  function isValidTime(hourStr, minuteStr) {
    const hour = parseInt(hourStr, 10);
    const minute = parseInt(minuteStr, 10);
    if (Number.isNaN(hour) || Number.isNaN(minute)) {
      return false;
    }
    return hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59;
  }

  function validateArticleDate() {
    const day = document.getElementById("ArticleDate_Day")?.value.trim();
    const month = document.getElementById("ArticleDate_Month")?.value.trim();
    const year = document.getElementById("ArticleDate_Year")?.value.trim();
    const hour = document.getElementById("ArticleDate_Hour")?.value.trim();
    const minute = document.getElementById("ArticleDate_Minute")?.value.trim();

    const isBlank = !day || !month || !year || !hour || !minute;
    const validDate = !isBlank && isValidDate(day, month, year);
    const validTime = !isBlank && isValidTime(hour, minute);
    const isValid = (!isBlank && validDate && validTime);

    const dateFormGroup = document.querySelector(".govuk-form-group:has(#ArticleDate_Day)");
    const errorMessage = document.getElementById("article-date-error-message");
    const inputFields = document.querySelectorAll("#ArticleDate_Day, #ArticleDate_Month, #ArticleDate_Year, #ArticleDate_Hour, #ArticleDate_Minute");

    const validationSummary = document.querySelector(".govuk-error-summary");
    const summaryList = validationSummary?.querySelector("ul");

    let summaryMessage = "";
    if (isBlank) {
      summaryMessage = "All date/time fields are required.";
    } else if (!validDate) {
      summaryMessage = "The date specified is not valid.";
    } else if (!validTime) {
      summaryMessage = "The time specified is not valid.";
    }

    if (summaryList) {
      const dateErrors = summaryList.querySelectorAll("li[data-error='article-date']");
      dateErrors.forEach(li => li.remove());
    }

    if (!isValid) {
      dateFormGroup?.classList.add("govuk-form-group--error");
      inputFields.forEach(i => i.classList.add("govuk-input--error"));

      if (errorMessage) {
        errorMessage.classList.remove("hidden");
        errorMessage.textContent = summaryMessage;
      }

      if (summaryList && summaryMessage) {
        const li = document.createElement("li");
        li.setAttribute("data-error", "article-date"); // 🚀 Identify date errors
        const a = document.createElement("a");
        a.href = "#ArticleDate_Day";
        a.id = "error-summary-ArticleDate_Day-list-item";
        a.classList.add("govuk-error-message"); // 🚀 Makes text red
        a.appendChild(document.createTextNode(summaryMessage));
        li.appendChild(a);
        summaryList.appendChild(li);
        validationSummary.classList.remove("hidden");
      }

    } else {
      dateFormGroup?.classList.remove("govuk-form-group--error");
      inputFields.forEach(i => i.classList.remove("govuk-input--error"));
      errorMessage?.classList.add("hidden");

      if (summaryList && summaryList.querySelectorAll("li").length === 0) {
        validationSummary?.classList.add("hidden");
      }
    }

    return isValid;
  }

  document.querySelectorAll("#ArticleDate_Day, #ArticleDate_Month, #ArticleDate_Year, #ArticleDate_Hour, #ArticleDate_Minute")
    .forEach(input => {
      input.addEventListener("change", validateArticleDate);
      input.addEventListener("blur", validateArticleDate);
    });

  document.querySelector("form")?.addEventListener("submit", function (e) {
    if (!validateArticleDate()) {
      e.preventDefault();
      document.querySelector(".govuk-error-summary")?.scrollIntoView({ behavior: "smooth" });
    }
  });
});
