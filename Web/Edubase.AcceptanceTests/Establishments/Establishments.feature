Feature: Establishments

Scenario: Get Establishment by URN
Given Establishment with URN "123" exists
When Establishment "123" is requested
Then the Establishment URN is "123"
And the Establishment Name is not empty
And the Establishment Type is not empty
