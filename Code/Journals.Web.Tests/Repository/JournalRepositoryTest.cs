using Journals.Model;
using Journals.Repository;
using Journals.Repository.DataContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.Entity;
using System.Linq;

namespace Journals.Web.Tests.Repository
{
    [TestClass]
    public class JournalRepositoryTest
    {
        [TestMethod]
        public void GetAll_Returns_All_Journals()
        {
            //Arrange
            IJournalRepository repository = JournalMocks.MockJournalRepositoryWithContext();

            //Act
            var actual = repository.GetAllJournals();
            var expected = JournalMocks.JournalData.Where(j => j.Id > 0 && (j.JournalId == 0 || j.JournalId == null)).ToList();

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Count, expected.Count);
            Assert.IsTrue(actual.All(j => (j.JournalId == 0 || j.JournalId == null) && j.Id > 0));
        }

        [TestMethod]
        public void GetAll_Issues_Returns_OnlyIssues()
        {
            //Arrange
            IJournalRepository repository = JournalMocks.MockJournalRepositoryWithContext();

            int journalID = 2;
            //Act
            var actual = repository.GetAllIssuesOfJournal(journalID);
            var expected = JournalMocks.JournalData.Where(j => j.JournalId == journalID).ToList();

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Count, expected.Count);
            Assert.IsTrue(actual.All(j => j.JournalId == journalID));
        }

        [TestMethod]
        public void AddJournal_Saves_In_Context()
        {
            //Arrange
            Mock<DbSet<Journal>> mockDbSetJournal = JournalMocks.MockDbSetJournal();
            Mock<JournalsContext> mockJournalContext = JournalMocks.MockContext(mockDbSetJournal, null);
            IJournalRepository repository = new JournalRepository(mockJournalContext.Object);

            var jToAdd = new Journal { Id = 100, Description = "Description of 100" };
            //Act
            var result = repository.AddJournal(jToAdd);

            //Assert
            Assert.IsTrue(result.Status);
            Assert.IsTrue(result.Id == JournalMocks.JournalData.Count() + 1);

            //todo: refactor this call to Journal mocks using callback instea of verify, target - any MOQ objects should be abstracted
            mockDbSetJournal.Verify(m => m.Add(jToAdd), Times.Once());
            mockJournalContext.Verify(m => m.SaveChanges(), Times.Once());
        }
    }
}