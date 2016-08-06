using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EF.Auditor.Tests.Infrastructure.Models
{
    public class Address : IEntity, IAuditable
    {
        public Guid ID { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string PostCode { get; set; }
        public virtual User User { get; set; }

        public Address()
        {
            this.ID = Guid.NewGuid();
        }

        public virtual Dictionary<string, string> TrackableProperties 
        {
            get
            {
                Dictionary<string, string> properties = new Dictionary<string, string>();
                properties.Add("Line1", "Line 1");
                properties.Add("Line2", "Line 2");
                properties.Add("PostCode", "Post Code");

                return properties;
            }
        }

        public Guid? ParentID
        {
            get
            {
                return this.User.ID;
            }
        }
    }
}