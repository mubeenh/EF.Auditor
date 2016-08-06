using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Auditor
{
    public static class AuditLogger
    {
        public static AuditLog GetAuditLogForEntry(DbEntityEntry<IAuditable> entry, Guid transactionID, string user, DbContext context)
        {
            AuditLog log = null;
            switch (entry.State)
            {
                case System.Data.EntityState.Added:
                    log = new AuditLog()
                    {
                        User = user,
                        TransactionID = transactionID,
                        EntityID = entry.Entity.ID,
                        ParentID = entry.Entity.ParentID,
                        EntityName = entry.Entity.GetType().Name.Split('_').First(), // EF appends extra info to name for tracked entities
                        TimeStamp = DateTime.Now,
                        ChangeType = "A",
                        Description = "Added",
                        ChangeLog = null
                    };
                    break;
                case System.Data.EntityState.Deleted:
                    log = new AuditLog()
                    {
                        User = user,
                        TransactionID = transactionID,
                        EntityID = entry.Entity.ID,
                        ParentID = entry.Entity.ParentID,
                        EntityName = entry.Entity.GetType().Name.Split('_').First(), // EF appends extra info to name for tracked entities
                        TimeStamp = DateTime.Now,
                        ChangeType = "D",
                        Description = "Deleted",
                        ChangeLog = null
                    };
                    break;
                case System.Data.EntityState.Modified:
                    log = new AuditLog()
                    {
                        User = user,
                        TransactionID = transactionID,
                        EntityID = entry.Entity.ID,
                        ParentID = entry.Entity.ParentID,
                        EntityName = entry.Entity.GetType().Name.Split('_').First(),
                        TimeStamp = DateTime.Now,
                        ChangeType = "M",
                        Description = "Modified",
                        ChangeLog  = ChangeLogger.GetChangeLogForEntry(entry, context)
                    };
                    break;                    
            }
            return log;
        }
    }
}
