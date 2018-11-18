Feature: JournalIssue
	In order to support multiple issues of a journal
	The publisher should be allowed to add multiple issues for a journal
	

@Create_Journal_Issue
Scenario: Add an issue to a journal
	Given A journal is already created and its id is supplied
	And I enter title, description
	When I press create
	Then the result should be a new issue addded to the journal
