function getProjectPageContent(element) {

    const projectPath = element.attr('project-path');

    let html = 
`
<div>
    <div style="margin-bottom: 15px">
        <ul class="nav nav-pills">
            <li class="nav-item">
                <a class="nav-link active project-tab" aria-current="page" href="#" id="project-tab-dashboard" loc-text="Dashboard" autofocus>Dashboard</a>
            </li>
            <li class="nav-item">
                <a class="nav-link project-tab" href="#" id="project-tab-components" loc-text="Components">Components</a>
            </li>
            <li class="nav-item">
                <a class="nav-link project-tab" href="#" id="project-tab-issues" loc-text="Issues">Issues</a>
            </li>
        </ul>
    </div>

    <div id="project-content" project-path="${projectPath}">
    </div>
</div>
`;

    var projectFolder = getFolderName(projectPath);
    var subtitle = projectFolder !== `` ? `(${projectFolder})` : ``;

    return {
        "content" : html,
        "name": getFileName(projectPath),
        "subtitle": subtitle,
        "postAction": () => {
            $("#project-tab-dashboard").on("click", function() {
                showTab("project-tab-dashboard", getProjectDashboardContent);
            });
        
            $("#project-tab-components").on("click", function() {
                showTab("project-tab-components", getProjectComponentsContent);
            });
        
            $("#project-tab-issues").on("click", function() {
                showTab("project-tab-issues", getProjectIssuesContent);
            });

            showTab("project-tab-dashboard", getProjectDashboardContent);
        }
    };
}

function showTab(tabId, tabContentGenerator) {
    const projectPath = $('#project-content').attr('project-path');
    const projectData = getProjectData(projectPath);

    let tabResult = tabContentGenerator(projectData);

    $('#project-content').html(tabResult.content);

    tabResult.postAction();

    $('.project-tab').removeClass('active');
    $('#' + tabId).addClass('active');

    $("#project-content").find('[autofocus]').focus();

    localize();
}

var projectsData = undefined;

// Project data is a dictionary of 
//  "project path": {
//      "path": "project path again",
//      "totalStoryPoints": 0,
//      "components": {
//        "name": {
//           "name": "",
//           "kind": "",
//           "relativePath": "",
//           "storyPoints": 0,
//           "ruleInstances": {
//           }    
//        }
//      },
//      "rules": {
//        "id": {
//          "id": "",
//          "encodedId": "",  to be used as html element id
//          "storyPoints": 0,
//          "ruleInstances": {
//          }
//        },  
//      }
//      "charts": {
//          "severity": {
//              "information": 0,
//              "potential": 0,
//              "optional": 0,
//              "mandatory": 0
//          },
//          "category": {
//              "COM": 0,
//              "IDENTITY": 0,
//              "HTTP": 0,
//              ...
//          }
//      }
//  }
function getProjectData(projectPath)
{
    if (projectsData === undefined)
    {
        projectsData = {};
    }

    if (projectsData[projectPath] !== undefined)
    {
        return projectsData[projectPath];
    }

    let projectResult = results.projects.find(project => project.path === projectPath);
    if (projectResult === undefined || projectResult.path === undefined)
    {
        return undefined;
    }

    let severity = {
        "Mandatory": 0,
        "Optional": 0,
        "Potential": 0,
        "Information": 0
    };

    let categories = {};
    let projectComponents = {};
    let projectRules = {};
    let totalStoryPoints = 0;
    let instanceCount = 0;
    const projectDir = getFolderName(projectPath);

    if (projectResult.hasOwnProperty('ruleInstances'))
    {
        let componentCount = 0;
        for (const instance of projectResult.ruleInstances) {
            // we assign integer id since we later refer to these instances in component 
            // instance list which reference same instances, but has different count of them.
            instance.id = instanceCount;

            let componentName = instance.location.path;
            let component = projectComponents[componentName];
            if (component === undefined) {
                component = {
                    "id": "project-component" + componentCount,
                    "name": componentName,
                    "kind": instance.location.kind,
                    "relativePath": getRelativePath(componentName, projectDir),
                    "storyPoints": 0,
                    "ruleInstances": []
                };

                projectComponents[componentName] = component;

                componentCount++;
            }

            const rule = results.rules[instance.ruleId];
            if (rule !== undefined)
            {
                const category = getRuleCategory(rule.id);
                if (category !== undefined)
                {
                    if (categories[category] === undefined)
                    {
                        categories[category] = 0;
                    }

                    categories[category]++;
                }

                severity[rule.severity]++;
                component.storyPoints += rule.effort;

                let projectRule = projectRules[rule.id];
                if (projectRule === undefined) {
                    projectRule = {
                        "id": rule.id,
                        "encodedId": rule.id.replace('.', '-'),
                        "storyPoints": 0,
                        "ruleInstances": []
                    };
    
                    projectRules[rule.id] = projectRule;
                }

                projectRule.storyPoints += rule.effort;
                projectRule.ruleInstances.push(instance);

                totalStoryPoints += rule.effort;
            }

            component.ruleInstances.push(instance);

            instanceCount++;
        }
    }

    let projectData = {
        "path": projectResult.path,
        "totalStoryPoints": totalStoryPoints,
        "totalIssues": Object.keys(projectRules).length,
        "totalIncidents": instanceCount,
        "components": projectComponents,
        "rules": projectRules,
        "charts": {
            "severity": severity,
            "category": categories
        }
    };

    projectsData[projectPath] = projectData;

    return projectData;
}
