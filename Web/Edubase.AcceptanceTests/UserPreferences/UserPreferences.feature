Feature: User Preferences

@ignore
Scenario: Check that User Preferences have been migrated from Table Storage to SQL Server 
Given the required data extracted from table storage
When the migration runs
Then all the existing UserPreferencesRepository data extracted from the table storage should be migrated to the SQL table.

Scenario: A GIAS user can save a filter set
Given I am a logged-in GIAS user
When I filter records on the establishment search results screen
Then I can save the filter set to create a saved filter

@ignore
Scenario: A GIAS user can apply a saved filter
Given I have a saved filter
When I select My saved filter set on the establishment search results screen
Then the saved filters are applied to the list of establishments

@ignore
Scenario: A GIAS user can delete a saved filter
Given I have a saved filter
When I select the option My saved filter set
Then I can delete my saved filter
