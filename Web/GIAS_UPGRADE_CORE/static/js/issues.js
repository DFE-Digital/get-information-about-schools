function getIssuesPageContent() {
    const issuesContentTemplate = 
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

    let html = $(issuesContentTemplate);

    html.find('#issue-list').html(generateIssueList());

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

function generateIssueList() {

    let html = 
`<table id="issues" class="table table-without-top-border">
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

    let aggregateData = getAggregateIssuesData();

    if (aggregateData !== undefined)
    {
        let count = 0;
        for (const ruleId of Object.keys(aggregateData).sort((a, b) => a.toUpperCase().localeCompare(b.toUpperCase()))) {
            let rule = results.rules[ruleId];
            let ruleData = aggregateData[ruleId];
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

            html += getProjectRuleContent(ruleData, true);

            count++;
        }
    }

    html += '</tbody></table>';

    return html;
}

var aggregateIssuesData = undefined;

// Aggregate issues data is a dictionary of rules across all projects
//      {
//        "id": {
//          "id": "",
//          "encodedId": "",  to be used as html element id
//          "storyPoints": 0,
//          "projectPath": "",
//          "ruleInstances": {
//          }
//        }
//      }
function getAggregateIssuesData()
{
    if (aggregateIssuesData !== undefined)
    {
        return aggregateIssuesData;
    }
    
    aggregateIssuesData = {};

    for (const projectResult of results.projects)
    {
        if (projectResult.hasOwnProperty('ruleInstances'))
        {
            let instanceCount = 0;
            for (const instance of projectResult.ruleInstances) {
                // we assign integer id since we later refer to these instances in component 
                // instance list which reference same instances, but has different count of them.
                instance.id = instanceCount;

                const rule = results.rules[instance.ruleId];
                if (rule !== undefined)
                {
                    let projectRule = aggregateIssuesData[rule.id];
                    if (projectRule === undefined) {
                        projectRule = {
                            "id": rule.id,
                            "encodedId": rule.id.replace('.', '-'),
                            "storyPoints": 0,
                            "projectPath": projectResult.path,
                            "ruleInstances": []
                        };
        
                        aggregateIssuesData[rule.id] = projectRule;
                    }

                    projectRule.storyPoints += rule.effort;
                    projectRule.ruleInstances.push(instance);
                }
                instanceCount++;
            }
        }
    }

    return aggregateIssuesData;
}