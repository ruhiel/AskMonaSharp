﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AskMonaSharp;
using System.Threading.Tasks;
using AskMonaSharp.Models;

namespace AskMonaSharpTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestTopicListAsync()
        {
            var result = await new AskMonaClient().TopicsListAsync(category:Category.Science_IT, safe:false, order:Order.Updated, limit:30, offset:0);
            Assert.AreEqual(1, result.status);
        }

        [TestMethod]
        public async Task TestResponsesList()
        {
            var result = await new AskMonaClient().ResponsesList(8659);
            Assert.AreEqual(1, result.status);
        }
    }
}
