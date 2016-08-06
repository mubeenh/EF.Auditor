using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EF.Auditor.Tests.Infrastructure.Models
{
    public class Magazine : IEntity, IAuditable
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int Frequency { get; set; }
        public string Publisher { get; set; }

        public Magazine()
        {
            this.ID = Guid.NewGuid();
        }

        public virtual Dictionary<string, string> TrackableProperties
        {
            get
            {
                Dictionary<string, string> properties = new Dictionary<string, string>();
                properties.Add("Name", "Name");
                properties.Add("Frequency", "Frequency");
                properties.Add("Publisher", "Publisher");

                return properties;
            }
        }

        public Guid? ParentID
        {
            get
            {
                return null;
            }
        }
    }
}