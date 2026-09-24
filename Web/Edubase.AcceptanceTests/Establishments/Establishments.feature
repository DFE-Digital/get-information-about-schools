Feature: Establishments

Scenario: Get Establishment by URN
Given a back office user is signed in
And an Establishment with URN "141491" exists
When the user requests the Establishment with URN "141491"
Then the Establishment URN is "141491"
And the Establishment Name is "Landau Forte Academy Tamworth Sixth Form1"
And the Establishment Type is "Academy 16 to 19 sponsor led"
