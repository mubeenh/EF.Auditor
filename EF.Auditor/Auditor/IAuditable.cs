using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Auditor
{
    public interface IAuditable
    {
        Guid ID { get; set; }
        Dictionary<string, string> TrackableProperties { get; }
        Guid? ParentID { get; }
    }
}
