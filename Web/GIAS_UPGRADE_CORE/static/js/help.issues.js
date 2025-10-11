function getHelpIssuesPageContent() {
    const contentTemplate = 
`
<div style="max-width: 800px">
    <h5 style="margin-top: 40px" loc-text="WhatIsAnIssue">What is an issue?</h5>
    <p loc-text="IssueDescription1">
        Issues are detected unique encounters of rules that might have to be addressed to re-platform an application to Azure. Each issue (rule) has its own unique ID, severity and story points. 
    </p>
    <p loc-text="IssueDescription2">There could be multiple incidents for each issue in one or more locations like code files or binaries.</p>
</div>
`;

    return {
        "content": contentTemplate
    };
}
