var currentPage = undefined;

// displays dynamic content for a given page name
function showPage(pageName, data) {

    let page = pages[pageName];
    if (page === undefined || page.getContentAction === undefined) {
        return;
    }

    // Note: result of page content action is an object: 
    // {
    //     "content": some html,
    //     "postAction": optional post action to run,
    //     "name": dynamic page title
    // }
    var pageResult = page.getContentAction(data);

    if (pageResult === undefined || pageResult.content === undefined) {
        return;
    }

    $("#content-container").html(pageResult.content);

    if (pageResult.postAction !== undefined) {
        pageResult.postAction();
    }

    updateBreadcrumbs(pageName, page, pageResult);

    applyPrivacyMode();

    localize();

    applyVisualSettings();

    applyAccessibilitySettings(pageName);

    $("#content-container").find('[autofocus]').focus();

    currentPage = page;
}

function applyAccessibilitySettings(pageName) {
    $("[global-tab]").attr("aria-selected", "false");
    $(`[global-tab="${pageName}"]`).attr("aria-selected", "true");
}

function localize() {
    if (strings === undefined || Object.keys(strings).length === 0) {
        return;
    }

    $("[loc-text]").each(function () {
        let locId = $(this).attr("loc-text");
        if (locId !== undefined && strings[locId] !== undefined) {
            $(this).text(strings[locId]);
        }
    });

    $("[loc-title]").each(function () {
        let locId = $(this).attr("loc-title");
        if (locId !== undefined && strings[locId] !== undefined) {
            $(this).attr('title', strings[locId]);
        }
    });

    $("[loc-aria-label]").each(function () {
        let locId = $(this).attr("loc-aria-label");
        if (locId !== undefined && strings[locId] !== undefined) {
            $(this).attr('aria-label', strings[locId]);
        }
    });
}

function getLocString(locId) {
    return strings[locId];
}

function applyPrivacyMode() {
    let container = $("#privacymode-container");
    if (container.length) {
        container.html(
            `<span class="badge badge-warning" style="margin-left: 5px; padding: 4px;"><span loc-text="PrivacyMode">Privacy mode: </span>${getLocString(results.privacyMode) ?? results.privacyMode}</span>
<a href="${results.privacyModeHelpUrl}" loc-title="WhatIsPrivacyMode" title="What is privacy mode?" class="accessible-link-style">
    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" class="feather feather-help-circle">
        <circle cx="12" cy="12" r="10"></circle>
        <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"></path>
        <line x1="12" y1="17" x2="12.01" y2="17"></line>
    </svg>
</a>`);
    }
}

function applyVisualSettings() {
    if (results.settings.visuals === undefined || results.settings.visuals.elements === undefined) {
        return;
    }

    for (const element of results.settings.visuals.elements) {
        if (element.id === undefined || element.displayName === undefined) {
            continue;
        }

        let htmlElement = $("#" + element.id);
        if (htmlElement.length) {
            htmlElement.text(element.displayName);
        }
    }
}

function updateBreadcrumbs(pageName, page, pageResult) {
    let breadcrumbsHtml = "";

    let breadcrumbs = page.breadcrumbs;
    if (breadcrumbs !== undefined) {
        // add previous breadcrumbs
        for (const breadcrumbPage of Object.keys(breadcrumbs)) {
            var name = strings[breadcrumbs[breadcrumbPage]] ?? breadcrumbs[breadcrumbPage];

            if (results.settings.visuals !== undefined && results.settings.visuals.pages !== undefined) {
                let pageVisuals = results.settings.visuals.pages[breadcrumbPage];
                if (pageVisuals !== undefined && pageVisuals.name !== undefined) {
                    name = pageVisuals.name;
                }
            }

            breadcrumbsHtml += '<li class="breadcrumb-item"><a href="#" class="accessible-link-style" route="' + breadcrumbPage + '">' + name + '</a></li>';
        }
    }

    var name = strings[page.name] ?? page.name ?? pageName;
    var title = strings[page.title] ?? page.title ?? pageResult.name ?? "";

    if (results.settings.visuals !== undefined && results.settings.visuals.pages !== undefined) {
        let pageVisuals = results.settings.visuals.pages[pageName];
        if (pageVisuals !== undefined) {
            if (pageVisuals.name !== undefined) {
                name = pageVisuals.name;
            }

            if (pageVisuals.title !== undefined) {
                title = pageVisuals.title;
            }
        }
    }

    // add active page breadcrumb
    let breadCrumbName = strings[pageResult.name] ?? pageResult.name ?? name;
    breadcrumbsHtml += '<li class="breadcrumb-item active" aria-current="page">' + breadCrumbName + '</li>';

    $("#page-title").html(title);

    if (pageResult.subtitle === undefined) {
        $("#page-subtitle").html('');
    }
    else {
        $("#page-subtitle").html(pageResult.subtitle);
    }

    $("#breadcrumbs-container").html(breadcrumbsHtml);

    // hook up click events for breadcrumbs
    $("#breadcrumbs-container a").each(function () {
        let route = $(this).attr("route");

        $(this).on("click", function () {
            showPage(route, $(this));
        });
    });
}

$(document).ready(function () {
    let title = results.settings === undefined || results.settings.targetDisplayName === undefined
        ? "Upgrade Analysis Report"
        : results.settings.targetDisplayName + " upgrade report";

    document.title = title;
    $("#route-home").text(title);

    let toolName = results.settings === undefined || results.settings.toolName === undefined
        ? undefined
        : results.settings.toolName;

    if (toolName !== undefined) {
        $("#tool-name").text(toolName);

        if (results.settings !== undefined && results.settings.toolUrl !== undefined) {
            $("#tool-name").attr("href", results.settings.toolUrl);
        }
    }

    // global routing event handlers
    $("#route-dashboard").on("click", function () {
        showPage("dashboard");
    });

    $("#route-projects").on("click", function () {
        showPage("projects");
    });

    $("#route-issues").on("click", function () {
        showPage("issues");
    });

    $("#route-help").on("click", function () {
        showPage("help");
    });

    $("#route-help-issues").on("click", function () {
        showPage("help-issues");
    });

    $("#route-help-incidents").on("click", function () {
        showPage("help-incidents");
    });

    $("#route-help-severity").on("click", function () {
        showPage("help-severity");
    });

    $("#route-help-storyPoints").on("click", function () {
        showPage("help-storyPoints");
    });

    // global control event handlers
    $("#sidebar-toggle").on("click", function () {
        const isExpanded = $('#sidebar').width() > 54;

        if (isExpanded) {
            document.getElementById("sidebar").style.width = "54px";
            document.getElementById("main").style.marginLeft = "54px";
        } else {
            document.getElementById("sidebar").style.width = "200px";
            document.getElementById("main").style.marginLeft = "200px";
        }

        $('#sidebar-toggle').toggleClass('sidebar-aligned-right');

        let toggleTitle = getLocString(!isExpanded ? "ShowLessInformation" : "ShowMoreInformation");
        if (toggleTitle !== undefined) {
            $('#sidebar-toggle').attr("title", toggleTitle);
        }
        else {
            $('#sidebar-toggle').attr("title", !isExpanded ? "Show less information" : "Show more information");
        }

        $('#sidebar-toggle svg').toggleClass('hidden');
        $('.hidden-in-small-sidebar').toggleClass('hidden');
    });

    // start with home page content
    showPage("dashboard");
});
