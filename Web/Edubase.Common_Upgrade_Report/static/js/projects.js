function getProjectsPageContent() {
    const projectsContentTemplate = 
    `
    <div class="row">
        <div class="col-12 mb-4 mb-lg-0">
            <div class="card">
                <div class="card-body">
                    <div id="project-list" class="table-responsive">
                    </div>
                </div>
            </div>
        </div>
    </div>
    `;

    let html = $(projectsContentTemplate);

    html.find('#project-list').html(generateProjectList());

    return {
        "content" : html,
        "postAction": () => {
            // register project details routing event handlers
            $('.route-project').each(function() {
                $(this).on("click", function() {
                    showPage("project", $(this));
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


function generateProjectList() {
    let projectListHTML = 
`<table class="table table-without-top-border">
    <thead>
    <tr>
        <th scope="col" class="w-75" id="ReportEntry" loc-text="Project">Project</th>
        <th scope="col" class="align-center">
            <span loc-text="Issues">Issues</span>
            <a id="route-inline-help-issues" href="#" loc-title="WhatAreIssues" title="What are issues?" class="accessible-link-style">
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

    let count = 0;
    for (const project of results.projects) {
        let projectName = getFileName(project.path);
        let instanceCount = project.hasOwnProperty('ruleInstances') ? project.ruleInstances.length : 0;
        let issueCount = getIssuesCount(project.ruleInstances);
        let storyPoints = project.hasOwnProperty('storyPoints') ? project.storyPoints : 0;
        var autofocus = count === 0 ? 'autofocus' : '';
        projectListHTML += 
`<tr>
    <td class="w-75">
        <a id="${projectName + count}" href="#" class="route-project accessible-link-style" project-path="${project.path}" ${autofocus}>${projectName}</a>
        <br/>
        <small class="text-muted">${project.path}</small>
    </td>
    <td class="align-center">
        <span class="badge badge-pill badge-light badge-with-padding">${issueCount}</span>
    </td>
    <td class="align-center">
        <span class="badge badge-pill badge-light badge-with-padding">${instanceCount}</span>
    </td>    
    <td class="align-center">
        <span class="badge badge-pill badge-light badge-with-padding">${storyPoints}</span>
    </td>
</tr>`;

        count++;
    }

    projectListHTML += '</tbody></table>';    

    return projectListHTML;
}
