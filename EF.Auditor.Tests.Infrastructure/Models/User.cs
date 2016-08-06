using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EF.Auditor.Tests.Infrastructure.Models
{
    public class User : IEntity, IAuditable
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public ICollection<Magazine> Subscriptions { get; set; }
        public Address Address { get; set; }

        public User()
        {
            this.ID = Guid.NewGuid();
        }

        public virtual Dictionary<string, string> TrackableProperties
        {
            get
            {
                Dictionary<string, string> properties = new Dictionary<string, string>();
                properties.Add("Name", "Name");
                properties.Add("Subscriptions", "Subscriptions");
                properties.Add("Address", "Address");

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