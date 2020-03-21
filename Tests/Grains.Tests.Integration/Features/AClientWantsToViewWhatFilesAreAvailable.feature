Feature: AClientWantsToViewWhatFilesAreAvailable

@VideoSearcher
Scenario: A person wants to view what movies are available
	Given I have 10 valid movies
	And I have 15 invalid movies
	When I view the available movies
	Then I see the 10 valid movies
