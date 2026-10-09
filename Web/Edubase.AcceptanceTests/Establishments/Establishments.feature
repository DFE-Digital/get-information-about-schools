Feature: Establishments

Scenario: Get an Establishment by URN for a back office user
    Given a back office user is signed in
    And an Establishment with URN "141491" exists
    When the user requests the Establishment with URN "141491"
    Then the Establishment URN is "141491"
    And the Establishment Name is "Landau Forte Academy Tamworth Sixth Form1"
    And the Establishment Type is "Academy 16 to 19 sponsor led"

Scenario: Get an Establishment by URN without signing in
    Given a user is not signed in
    And an Establishment with URN "141491" exists
    When the user requests the Establishment with URN "141491"
    Then an error occurs

Scenario: Get an Establishment by URN that does not exist
    Given a back office user is signed in
    And an Establishment with URN "0" exists
    When the user requests the Establishment with URN "0"
    Then the Establishment is not found
