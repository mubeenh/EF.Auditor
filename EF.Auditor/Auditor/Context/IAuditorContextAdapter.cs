using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Auditor
{
    public interface IAuditorContextAdapter
    {
        DbContext Context { get; }
        IQueryable<AuditLog> AuditLogSet { get; }
        void AddAuditLogEntity(AuditLog entity);
    }
}
