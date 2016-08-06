using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace EF.Auditor
{
    public class Auditor
    {
        private DbContext AppContext;
        private IAuditorContextAdapter AuditorContext;

        public Auditor(IAuditorContextAdapter auditorContext)
        {
            this.AuditorContext = auditorContext;
            this.AppContext = auditorContext.Context;
        }

        public int LogAndSaveChanges(string user)
        {
            int result = 0;
            List<AuditLog> logs = new List<AuditLog>();
            AppContext.Configuration.ValidateOnSaveEnabled = false;

            using (var scope = new TransactionScope())
            {
                AppContext.ChangeTracker.DetectChanges();

                var auditables = AppContext.ChangeTracker.Entries<IAuditable>();

                Guid transactionID = Guid.NewGuid(); // A unique ID to mark all logs in a single save operation

                foreach (DbEntityEntry<IAuditable> entry in auditables.Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
                {
                    logs.Add(AuditLogger.GetAuditLogForEntry(entry, transactionID, user, AppContext));
                }

                logs.ForEach(e => AuditorContext.AddAuditLogEntity(e));
                
                result = AppContext.SaveChanges();
                scope.Complete();
            }
            return result;
        }
    }
}
