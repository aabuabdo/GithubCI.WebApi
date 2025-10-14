using Xunit;

namespace GithubCI.WebApi.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void PassingTest()
        {
            Assert.True(true);
        }

        [Fact]
        public void FailingTest()
        {
            Assert.True(false); // هذا راح يفشل التست
        }
    }
}
