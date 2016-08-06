using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using EF.Auditor.Tests.Infrastructure.Models;

namespace EF.Auditor.Tests
{
    [TestClass]
    public class AuditorLoggingTests : BaseEFTests
    {
        [TestMethod]
        public void TestChangeLoggingForReferenceProperties()
        {
            User user = new User();
            user.Name = "John Smith";
            db.Users.Add(user); //Add a user entity
            db.Commit();

            Assert.IsTrue(db.AuditLogs.Any(e => e.EntityID == user.ID && e.ChangeType == "A")); // Verify the log for user addition

            Magazine m1 = new Magazine()
            {
                Name = "National Geographic",
                Publisher = "National Geographic Society",
                Frequency = 30
            };

            db.Magazines.Add(m1); // Add a magazine entity
            db.Commit();

            Assert.IsTrue(db.AuditLogs.Any(e => e.EntityID == m1.ID && e.ChangeType == "A")); // Verify the log for magazine addition

            User dbUser = db.Users.Find(user.ID);
            Magazine dbMagazine = db.Magazines.Find(m1.ID);

            dbUser.Subscriptions = new List<Magazine>();
            dbUser.Subscriptions.Add(dbMagazine);
            db.Entry(dbUser).State = System.Data.EntityState.Modified;
            db.Commit();

            AuditLog log = db.AuditLogs.FirstOrDefault(e => e.EntityID == dbUser.ID && e.ChangeType == "M"); 
            Assert.IsNotNull(log); // Verify the log for user entity update
            Assert.IsFalse(log.ChangeLog == null || log.ChangeLog.Count == 0);
            Assert.IsTrue(log.ChangeLog.Any(e => e.AttributeName == "Subscriptions")); // Verify the log entry for adding subscription to user entity

        }

        [TestMethod]
        public void TestChangeLoggingForSimpleProperties()
        {
            Magazine m1 = new Magazine()
            {
                Name = "National Geographic",
                Publisher = "National Geographic Society",
                Frequency = 30
            };

            db.Magazines.Add(m1); // Add a magazine entity
            db.Commit();

            Assert.IsTrue(db.AuditLogs.Any(e => e.EntityID == m1.ID && e.ChangeType == "A")); // Verify the log for magazine addition

            Magazine dbMagazine = db.Magazines.Find(m1.ID);
            dbMagazine.Name = "Lonely Planet";
            db.Entry(dbMagazine).State = System.Data.EntityState.Modified;
            db.Commit();

            AuditLog log = db.AuditLogs.FirstOrDefault(e => e.EntityID == dbMagazine.ID && e.ChangeType == "M");
            Assert.IsNotNull(log); // Verify the log for user entity update
            Assert.IsFalse(log.ChangeLog == null || log.ChangeLog.Count == 0);
            Assert.IsTrue(log.ChangeLog.Any(e => e.AttributeName == "Name")); // Verify the log entry for adding subscription to user entity
        }

    }
}
