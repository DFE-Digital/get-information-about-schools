function getHelpIncidentsPageContent() {
    const contentTemplate = 
`
<div style="max-width: 800px">
    <h5 style="margin-top: 40px" loc-text="WhatIsAnIncident">What is an incident?</h5>
    <p loc-text="IncidentDescription1">
        Incident is an example of detected rule at a specific location (code file, binary etc). There could be many incidents of a given issue. 
    </p>
    <p loc-text="IncidentDescription2">
        Each incident contains an issue ID, location and (most of the times) snippet that caused the incident to be detected.
    </p>
</div>
`;

    return {
        "content": contentTemplate
    };
}
