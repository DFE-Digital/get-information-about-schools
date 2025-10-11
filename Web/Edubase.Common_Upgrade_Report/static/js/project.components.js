function getProjectComponentsContent(projectData) {
    const projectComponentsContentTemplate = 
`
<div class="row">
    <div class="col-12 col-xl-12 mb-4 mb-lg-0">
        <div class="card">
            <div class="card-body">
                <div id="component-list" class="table-responsive">
                </div>
            </div>
        </div>
    </div>
</div>
`;

    let html = $(projectComponentsContentTemplate);

    const projectPath = projectData.path
    html.find('#component-list').html(generateComponentList(projectPath, projectData));

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

            $('.component-table').each(function() {
                $(this).on("click", "tr", function() {
                    showDetailsForComponentRowList($(this));
                });
            });

            $('#route-inline-help-issues').on("click", function() {
                showPage("help-issues");
            });

            $('#route-inline-help-incidents').on("click", function() {
                showPage("help-incidents");
            });

            $('#route-inline-help-storyPoints').on("click", function() {
                showPage("help-storyPoints");
            });
        }
    };
}

function generateComponentList(projectPath, projectData) {

    let html = 
`<table id="project-components" class="table table-without-top-border" project-path="${projectPath}">
    <thead>
        <tr>
            <th scope="col" class="pr-0"></th>
            <th scope="col" class="w-75" loc-text="Component">Component</th>
            <th scope="col" class="align-center" loc-text="State">State</th>
            <th scope="col" class="align-center" loc-text="Kind">Kind</th>
            <th scope="col" class="align-center">
                <span loc-text="Issues">Issues</span>
                <a id="route-inline-help-issues" href="#" loc-title="WhatAreIssues" title="What are issues?" class="accessible-link-style" autofocus>
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

    if (projectData !== undefined && projectData.components !== undefined)
    {
        let count = 0;
        for (const component of Object.values(projectData.components).sort((a, b) => a.relativePath.toUpperCase().localeCompare(b.relativePath.toUpperCase()))) {
            let componentKind = component.kind;
            let instanceCount = component.ruleInstances.length;
            let storyPoints = component.storyPoints;
            let issueCount = getIssuesCount(component.ruleInstances);
            let state = getState(component.ruleInstances);

            html += 
                `<tr>
                    <td class="pr-0">
                        <button class="expander-button" style="display: inline;" data-toggle="collapse" data-target="#${component.id}" aria-expanded="true" aria-controls="${component.id}">
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-chevron-right"><polyline points="9 18 15 12 9 6"></polyline></svg>
                            <svg class="hidden" xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-chevron-down"><polyline points="6 9 12 15 18 9"></polyline></svg>
                        </button>
                    </td>
                    <td class="table-row-wrap-all">
                        <span style="white-space: normal; word-wrap: break-all;">${component.relativePath}</span>
                    </td>
                    <td class="align-center">${getLocString(state) ?? state}</td>
                    <td class="align-center">${getLocString(componentKind) ?? componentKind}</td>
                    <td class="align-center"><span class="badge badge-pill badge-light badge-with-padding">${issueCount}</span></td>
                    <td class="align-center"><span class="badge badge-pill badge-light badge-with-padding">${instanceCount}</span></td>
                    <td class="align-center"><span class="badge badge-pill badge-light badge-with-padding">${storyPoints}</span></td>
                </tr>`;

            html += getComponentContent(component);

            count++;
        }
    }

    html += '</tbody></table>';

    return html;
}

function getComponentContent(component) {
    let issuesHtml = getComponentIssues(component);
    let ruleInstances = Object.values(component.ruleInstances);
    let issueDetailsHtml = ruleInstances.length > 0 ? getIssueDetails(ruleInstances[0], null) : "";

    let html = 
`
<tr class="without-border">
    <td colspan="7" id="${component.id}" class="collapse" style="border-left: 2px solid #0d6efd;">
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
                    <div id="issue-details-${component.id}" class="card-body">
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

function getComponentIssues(component) {
    let html = 
`
<table class="table table-hover table-without-top-border table-with-narrow-rows component-table">
<thead>
    <tr>
        <th scope="col" class="width-auto align-center">Issue</th>
        <th scope="col" class="w-75">Description</th>
        <th scope="col" class="width-auto align-center">State</th>
    </tr>
</thead>
<tbody>
`;

    let count = 0;
    for (const instance of component.ruleInstances.sort((a, b) => a.ruleId.toUpperCase().localeCompare(b.ruleId.toUpperCase()))) {
        let ruleId = instance.ruleId;
        let rule = results.rules[ruleId];
        
        let description = instance.description ?? rule.label;
        if (description === undefined) {
            description = rule.label === undefined ? "" : rule.label;
        }

        let label = instance.location.label === undefined
            ? description
            : `${description} (${instance.location.label})`;

        let position = getPosition(instance);

        html += 
            `<tr class="${count == 0 ? "table-active" : ""} details-text-size" instance-id="${instance.id}" component-id="${component.id}">
                <td class="width-auto align-center">
                    <span class="badge badge-info badge-with-padding">${ruleId}</span>
                </td>
                <td class="w-75">
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
function showDetailsForComponentRowList(row) {
    const projectPath = $('#project-components').attr('project-path');
    let projectResult = results.projects.find(project => project.path === projectPath);
    let instanceId = row.attr('instance-id');
    let componentId = row.attr('component-id');

    let instance = undefined;
    if (projectResult !== undefined && instanceId !== undefined && instanceId >= 0) {
        instance = projectResult.ruleInstances[instanceId];
    }

    $('#issue-details-' + componentId).html(getIssueDetails(instance));

    let table = row.closest('table');
    table.find('tr').removeClass('table-active');
    row.addClass('table-active');
}