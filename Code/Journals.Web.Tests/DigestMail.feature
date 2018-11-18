Feature: DigestMail
	To notify the users about the new issues of a journal,
	The system should send daily digest email if there are any
	new issues available for their subscribed journals.

@issue_mailer
Scenario: Send mail
	Given A user has already subscribed to some journals
	And I have created a new issue of a journal that user has subscribed to
	When The mail service runs 
	Then the issue should be notified in email


	
@issue_mailer
Scenario: Dont send false mail
	Given A user has already subscribed to some journals
	And I have created a new issue of a journal that user has NOT subscribed to
	When The mail service runs 
	Then the issue should NOT be notified in email

@issue_mailer
Scenario: Dont send duplicate mail
	Given A user has already subscribed to some journals
	And I have created a new issue of a journal that user has subscribed to
	And the mail is already sent for the day
	When The mail service runs
	Then the issue should NOT be notified in email
