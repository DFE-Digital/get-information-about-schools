Feature: Tokens

Scenario: Ensure filters are remembered after logging out and back in
Given that I am a logged-in GIAS user
And I have applied one or more filters within GIAS
When I save the selected filters as a filter set
And I log out of GIAS
And I subsequently log back into GIAS using the same user account
Then the system should remember my previously saved filter set
And my saved filter set should be available for me to select and apply
And the saved filter set should retain the filter criteria I previously selected
And applying the saved filter set should return the same filtering results.
