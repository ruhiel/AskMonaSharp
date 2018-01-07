using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AskMonaSharp;
using System.Threading.Tasks;
using AskMonaSharp.Models;

namespace AskMonaSharpTest
{
    [TestClass]
    public class UnitTest1
    {
        [Ignore]
        public async Task TestTopicListAsync()
        {
            using (var client = new AskMonaClient())
            {
                var result = await client.TopicsListAsync(category: Category.Science_IT, safe: false, order: Order.Updated, limit: 30, offset: 0);
                Assert.AreEqual(1, result.status);
            }
        }

        [Ignore]
        public async Task TestResponsesList()
        {
            using (var client = new AskMonaClient())
            {
                var result = await client.ResponsesList(8659);
                Assert.AreEqual(1, result.status);
            }
        }

        [Ignore]
        public async Task TestProfile()
        {
            using (var client = new AskMonaClient())
            {
                var result = await client.Profile(11268);
                Assert.AreEqual(1, result.status);
            }
        }

        [TestMethod]
        public async Task TestSecretkey()
        {
            using (var client = new AskMonaClient(Environment.GetEnvironmentVariable("AskMonaAPISecretKey")))
            {
                var result1 = await client.Secretkey(
                    int.Parse(Environment.GetEnvironmentVariable("AskMonaAPIAppID")),
                    Environment.GetEnvironmentVariable("AskMonaAPIUser"),
                    Environment.GetEnvironmentVariable("AskMonaAPIPassword"));
                Assert.AreEqual(1, result1.status);

                var result2 = await client.Verify(int.Parse(Environment.GetEnvironmentVariable("AskMonaAPIAppID")), int.Parse(Environment.GetEnvironmentVariable("AskMonaAPIUserID")), result1.secretkey);
                Assert.AreEqual(1, result2.status);
            }
        }
    }
}
