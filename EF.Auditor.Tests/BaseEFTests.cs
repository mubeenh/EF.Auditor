using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EF.Auditor.Tests.Infrastructure;

namespace EF.Auditor.Tests
{
    [TestClass]
    public class BaseEFTests
    {
        protected TestDbContext db;

        public BaseEFTests()
        {
            Initialize();
        }
        protected virtual void Initialize()
        {
            db = new TestDbContext();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            db.Database.Delete();
            db.Database.Create();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            db.Dispose();
        }
    }
}
