const pages = {
    "dashboard": {
        "title": "Dashboard",
        "name": "Dashboard",
        "getContentAction": getDashboardPageContent
    },
    "projects": {
        "title": "Projects",
        "name": "Projects",
        "getContentAction": getProjectsPageContent
    },
    "help": {
        "title": "Help Contents",
        "name": "Help",
        "getContentAction": getHelpPageContent,
    },
    "help-issues": {
        "title": "Issues",
        "name": "Issues",
        "getContentAction": getHelpIssuesPageContent,
        "breadcrumbs": {
            "help": "Help"
        }
    },
    "help-incidents": {
        "title": "Incidents",
        "name": "Incidents",
        "getContentAction": getHelpIncidentsPageContent,
        "breadcrumbs": {
            "help": "Help"
        }
    },
    "help-severity": {
        "title": "Severity",
        "name": "Severity",
        "getContentAction": getHelpSeverityPageContent,
        "breadcrumbs": {
            "help": "Help"
        }
    },
    "help-storyPoints": {
        "title": "Story Points",
        "name": "Story Points",
        "getContentAction": getHelpStoryPointsPageContent,
        "breadcrumbs": {
            "help": "Help"
        }
    },
    "project": {
        "getContentAction": getProjectPageContent,
        "breadcrumbs": {
            "projects": "Projects"
        }
    },
    "issues": {
        "title": "Aggregate Issues",
        "name": "Aggregate Issues",
        "getContentAction": getIssuesPageContent
    }
}
