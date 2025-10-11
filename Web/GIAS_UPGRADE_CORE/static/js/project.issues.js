function getProjectIssuesContent(projectData) {
    const projectIssuesContentTemplate = 
`
<div class="row">
    <div class="col-12 col-xl-12 mb-4 mb-lg-0">
        <div class="card">
            <div class="card-body">
                <div id="issue-list" class="table-responsive">
                </div>
            </div>
        </div>
    </div>
</div>
`;

    let html = $(projectIssuesContentTemplate);

    const projectPath = projectData.path
    html.find('#issue-list').html(generateProjectIssueList(projectPath, projectData));

    return {
        "content" : html,
        "postAction": () => {
            // register project details routing event handlers
            $('.expander-button').each(function() {
                $(this).on("click", function() {
                    $(this).children().toggleClass('hidden');

                    highlightExpandedRow($(this));
                });
            });

            $('.issue-table').each(function() {
                $(this).on("click", "tr", function() {
                    showDetailsForIssueRowList($(this));
                });
            });

            $('#route-inline-help-incidents').on("click", function() {
                showPage("help-incidents");
            });

            $('#route-inline-help-storyPoints').on("click", function() {
                showPage("help-storyPoints");
            });

            $('#route-inline-help-severity').on("click", function() {
                showPage("help-severity");
            });
        }
    };
}

function generateProjectIssueList(projectPath, projectData) {

    let html = 
`<table id="project-issues" class="table table-without-top-border" project-path="${projectPath}">
    <thead>
        <tr>
            <th scope="col" class="width-auto align-center" loc-text="Issue">Issue</th>
            <th scope="col" class="w-75" loc-text="Description">Description</th>
            <th scope="col" class="align-center" loc-text="State">State</th>
            <th scope="col" class="align-center">
                <span loc-text="Severity">Severity</span>
                <a id="route-inline-help-severity" href="#" loc-title="IssueSeverity" title="Issue Severity" class="accessible-link-style" autofocus>
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" class="feather feather-help-circle">
                        <circle cx="12" cy="12" r="10"></circle>
                        <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"></path>
                        <line x1="12" y1="17" x2="12.01" y2="17"></line>
                    </svg>
                </a>
            </th>
            <th scope="col" class="align-center">
                <span loc-text="Incidents">Incidents</span>
                <a id="route-inline-help-incidents" href="#" loc-title="WhatAreIncidents" title="What are incidents?" class="accessible-link-style">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" class="feather feather-help-circle">
                        <circle cx="12" cy="12" r="10"></circle>
                        <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"></path>
                        <line x1="12" y1="17" x2="12.01" y2="17"></line>
                    </svg>
                </a>
            </th>
            <th scope="col" class="align-center">
                <span loc-text="StoryPoints">Story Points</span>
                <a id="route-inline-help-storyPoints" href="#" loc-title="WhatAreStoryPoints" title="What are Story Points?" class="accessible-link-style">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" class="feather feather-help-circle">
                        <circle cx="12" cy="12" r="10"></circle>
                        <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"></path>
                        <line x1="12" y1="17" x2="12.01" y2="17"></line>
                    </svg>
                </a>
            </th>
        </tr>
    </thead>
    <tbody>`;

    if (projectData !== undefined && projectData.rules !== undefined)
    {
        let count = 0;
        
        for (const ruleId of Object.keys(projectData.rules).sort()) {
            let rule = results.rules[ruleId];
            let ruleData = projectData.rules[ruleId];
            let state = getState(ruleData.ruleInstances);

            html += 
                `<tr>
                    <td class="width-auto">
                        <button class="expander-button" data-toggle="collapse" data-target="#${ruleData.encodedId}" aria-expanded="true" aria-controls="${ruleData.encodedId}">
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-chevron-right"><polyline points="9 18 15 12 9 6"></polyline></svg>
                            <svg class="hidden" xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-chevron-down"><polyline points="6 9 12 15 18 9"></polyline></svg>
                        </button>
                        <span class="badge badge-info badge-with-padding">${rule.id}</span>
                    </td>
                    <td class="w-75">${rule.label ?? rule.description}</td>
                    <td class="align-center">${state}</td>
                    <td class="align-center">${getLocString(rule.severity) ?? rule.severity}</td>
                    <td class="align-center"><span class="badge badge-pill badge-light badge-with-padding">${ruleData.ruleInstances.length}</span></td>
                    <td class="align-center"><span class="badge badge-pill badge-light badge-with-padding">${ruleData.storyPoints}</span></td>
                </tr>`;

            html += getProjectRuleContent(ruleData);

            count++;
        }
    }

    html += '</tbody></table>';

    return html;
}

function getProjectRuleContent(ruleData, aggregate) {
    let issuesHtml = getProjectRuleIssues(ruleData, aggregate);
    let ruleInstances = Object.values(ruleData.ruleInstances);
    let issueDetailsHtml = ruleInstances.length > 0 ? getIssueDetails(ruleInstances[0], null) : "";

    let html = 
`
<tr class="without-border">
    <td colspan="7" id="${ruleData.encodedId}" class="collapse" style="border-left: 2px solid #0d6efd;">
        <div class="row my-4 mx-0">
            <div class="col-12 col-lg-6 col-md-6 mb-4" style="padding: 0px 4px 0px 4px;">
                <div class="card">
                    <div class="card-body overflow-auto" style="max-height: 400px;">
                        ${issuesHtml}
                    </div>
                </div>
            </div>
            <div class="col-12 col-lg-6 col-md-6" style="padding: 0px 4px 0px 4px;">
                <div class="card">
                    <div id="issue-details-${ruleData.encodedId}" class="card-body">
                        ${issueDetailsHtml}
                    </div>
                </div>
            </div>
        </div>
    </td>
</tr>
`;
    return html;
}

function compareRuleInstances(left, right) {
    if (left.location === undefined || left.location.path === undefined || right.location === undefined || right.location.path === undefined) {
        return 0;
    }
    var order = left.location.path.toUpperCase().localeCompare(right.location.path.toUpperCase());
    if (order != 0) {
        return order;
    }

    var leftLine = left.location.line;
    var rightLine = right.location.line;

    if (leftLine === undefined || rightLine === undefined) {
        return 0;
    }

    if (leftLine < rightLine) {
        return -1;
    }
    else if (leftLine > rightLine) {
        return 1;
    }

    var leftColumn = left.location.column;
    var rightColumn = right.location.column;

    if (leftColumn < rightColumn) {
        return -1;
    }
    else if (leftColumn > rightColumn) {
        return 1;
    }

    return 0;
}

function getProjectRuleIssues(ruleData, aggregate) {
    let html = 
`
<table class="table table-hover table-without-top-border table-with-narrow-rows issue-table">
<thead>
    <tr>
        <th scope="col" class="w-75">Location</th>
        <th scope="col" class="width-auto align-center">State</th>
    </tr>
</thead>
<tbody>
`;

    let count = 0;
    for (const instance of ruleData.ruleInstances.sort(compareRuleInstances)) {
        let description = (aggregate  
            ? instance.location.path
            : getRelativePath(instance.location.path, getFolderName(instance.projectPath))) ?? instance.description;

        let label = instance.location.label === undefined
            ? description
            : `${description} (${instance.location.label})`;

        let position = getPosition(instance);

        html += 
            `<tr class="${count == 0 ? "table-active" : ""} details-text-size" instance-id="${instance.id}" project-path="${instance.projectPath ?? ""}" rule-id="${ruleData.encodedId}">
                <td class="w-75 table-row-wrap-all">
                    <span>${label}</span> <span class="text-info">${position}</span>
                </td>
                <td class="width-auto align-center">${getLocString(instance.state) ?? instance.state}</td>
            </tr>`;

        count++;
    }

    html += '</tbody></table>';
    
    return html;
}

// when project component's issue row selected in the table, show the issue details
function showDetailsForIssueRowList(row) {
    const projectPath = $('#project-issues').attr('project-path') ?? row.attr('project-path');
    let projectResult = results.projects.find(project => project.path === projectPath);
    let instanceId = row.attr('instance-id');
    let encodedRuleId = row.attr('rule-id');

    let instance = undefined;
    if (projectResult !== undefined && instanceId !== undefined && instanceId >= 0) {
        instance = projectResult.ruleInstances[instanceId];
    }

    $('#issue-details-' + encodedRuleId).html(getIssueDetails(instance));

    let table = row.closest('table');
    table.find('tr').removeClass('table-active');
    row.addClass('table-active');
}
