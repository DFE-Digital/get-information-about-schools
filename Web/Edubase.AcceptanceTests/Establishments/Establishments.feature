Feature: Establishments

Scenario: Get Establishment by URN
Given Establishment with URN "141491" exists
When Establishment with URN "141491" is requested
Then the Establishment URN is "141491"
And the Establishment Name is "Landau Forte Academy Tamworth Sixth Form1"
And the Establishment Type is "Academy 16 to 19 sponsor led"
