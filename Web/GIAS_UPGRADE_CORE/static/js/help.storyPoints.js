function getHelpStoryPointsPageContent() {
    const contentTemplate = 
`
<div style="max-width: 800px">
    <h5 style="margin-top: 40px" loc-text="WhatAreStoryPoints">What are Story Points?</h5>
    <p loc-text="StoryPointsDescription1">Story Points are an abstract metric commonly used in Scrum Agile software development methodology to estimate the level of effort needed to implement a feature or change. They are based on a modified Fibonacci sequence.</p>

    <p loc-text="StoryPointsDescription2">In a similar manner, Windup uses story points to express the level of effort needed to migrate particular application constructs, and in a sum, the application as a whole. It does not necessarily translate to man-hours, but the value should be consistent across tasks.</p>

    <h5 style="margin-top: 40px" loc-text="HowStoryPointsCalculated">How are Story Points calculated?</h5>
    <p loc-text="StoryPointsDescription3">Story points are a unit of measure for expressing an issue's overall size. They help teams estimate how much work is required to complete a task. The higher the story point value, the more effort the team believes the issue will take to complete.</p>

    <table class="table table-without-top-border">
        <thead>
            <tr>
                <th loc-text="LevelOfEffort">Level of Effort</th>
                <th class="align-center" loc-text="StoryPoints">Story Points</th>
                <th loc-text="Description">Description</th>
            </tr>
        </thead>
        <tr>
            <td loc-text="Trivial">Trivial</td>
            <td class="align-center">1</td>
            <td loc-text="TrivialDescription">The migration is a trivial change or a simple library swap with no or minimal API changes.</td>
        </tr>
        <tr>
            <td loc-text="Complex">Complex</td>
            <td class="align-center">3</td>
            <td loc-text="ComplexDescription">The changes required for the migration task are complex, but have a documented solution.</td>
        </tr>
        <tr>
            <td loc-text="Redesign">Redesign</td>
            <td class="align-center">5</td>
            <td loc-text="RedesignDescription">The migration task requires a redesign or a complete library change, with significant API changes.</td>
        </tr>
        <tr>
            <td loc-text="Rearchitecture">Rearchitecture</td>
            <td class="align-center">7</td>
            <td loc-text="RearchitectureDescription">The migration requires a complete rearchitecture of the component or subsystem.</td>
        </tr>
        <tr>
            <td loc-text="Unknown">Unknown</td>
            <td class="align-center">13</td>
            <td loc-text="UnknownDescription">The migration solution is not known and may need a complete rewrite.</td>
        </tr>
    </table>
</div>
`;

    return {
        "content": contentTemplate
    };
}
