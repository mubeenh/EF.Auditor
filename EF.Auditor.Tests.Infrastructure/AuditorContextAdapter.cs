using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EF.Auditor.Tests.Infrastructure
{
    public class AuditorContextAdapter : IAuditorContextAdapter
    {
        private DbContext _Context;

        public AuditorContextAdapter(DbContext context)
        {
            _Context = context;
        }

        public DbContext Context { get { return _Context; } }

        public IQueryable<AuditLog> AuditLogSet
        {
            get
            {
                return Context.Set<AuditLog>();
            }
        }

        public void AddAuditLogEntity(AuditLog entity)
        {
            Context.Set<AuditLog>().Add(entity);
        }
    }
}