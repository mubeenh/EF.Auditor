using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Auditor
{
    public class ChangeLog
    {
        public ChangeLog()
        {
            ID = Guid.NewGuid();
        }

        public Guid ID { get; set; }
        public string AttributeName { get; set; }
        public string AttributeDescription { get; set; }
        public string OriginalValue { get; set; }
        public string NewValue { get; set; }
    }
}
