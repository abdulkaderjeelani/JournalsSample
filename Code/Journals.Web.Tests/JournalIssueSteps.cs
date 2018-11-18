using Journals.Model;
using Journals.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TechTalk.SpecFlow;

namespace Journals.Web.Tests
{
    [Binding]
    public class JournalIssueSteps
    {
        private Journal newIssue = new Journal();

        /*Test behaviour with mock */
        IJournalRepository repository = JournalMocks.MockJournalRepositoryWithContext();

        /*Test behaviour with actual repository */
        //private IJournalRepository repository = new JournalRepository();

        [Given(@"A journal is already created and its id is supplied")]
        public void GivenAJournalIsAlreadyCreatedAndItsIdIsSupplied()
        {
            newIssue.Id = 100;
            newIssue.JournalId = 1;
            newIssue.UserId = 1;
        }

        [Given(@"I enter title, description")]
        public void GivenIEnterTitleDescription()
        {
            newIssue.Title = "my issue title";
            newIssue.Description = "my issue description";
            newIssue.FileName = "dummyfile.pdf";
        }

        [When(@"I press create")]
        public void WhenIPressCreate()
        {
            repository.AddJournalIssue(newIssue);
        }

        [Then(@"the result should be a new issue addded to the journal")]
        public void ThenTheResultShouldBeANewIssueAdddedToTheJournal()
        {
            var issues = repository.GetAllIssuesOfJournal(newIssue.JournalId.Value);
            Assert.IsTrue(issues.Any(j => j.Title == newIssue.Title));
        }
    }
}