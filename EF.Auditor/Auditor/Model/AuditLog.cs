using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Auditor
{
    public class AuditLog 
    {
        public AuditLog()
        {
            ID = Guid.NewGuid();
        }

        public Guid ID { get; set; }
        public Guid TransactionID { get; set; }
        public string EntityName { get; set; }
        public Guid EntityID { get; set; }
        public Guid? ParentID { get; set; }
        public string User { get; set; }
        public DateTime TimeStamp { get; set; }
        public string ChangeType { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ChangeLog> ChangeLog { get; set; }
    }
}
