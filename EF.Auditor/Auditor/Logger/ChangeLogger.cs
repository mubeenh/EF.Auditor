using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Auditor
{
    public static class ChangeLogger
    {
        public static ICollection<ChangeLog> GetChangeLogForEntry(DbEntityEntry<IAuditable> entry, DbContext context)
        {
            ICollection<ChangeLog> changes = new List<ChangeLog>();

            // Select only properties marked 'Trackable'
            var props = entry.Entity.TrackableProperties;
            
            if (props == null || props.Count() == 0) // if no properties are marked, then nothing to log here
            {
                return null;
            }

            ObjectStateEntry objectStateEntry = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            var edmMembers = objectStateEntry.EntitySet.ElementType.Members;

            if (entry.State == System.Data.EntityState.Added) // If its a newly added entity
            {
                foreach (string propertyName in props.Keys.ToList())
                {
                    EdmMember member = edmMembers.First(e => e.Name == propertyName);
                    string propertyNameDescription = props[propertyName];

                    if (member.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty)
                    {
                        var referenceEntry = entry.Member(member.Name) as DbReferenceEntry;
                        if (referenceEntry != null)
                        {
                            ChangeLog log = new ChangeLog()
                            {
                                AttributeName = propertyName,
                                AttributeDescription = propertyNameDescription,
                                NewValue = SerializeToString(referenceEntry.CurrentValue),
                                OriginalValue = null,
                            };
                            changes.Add(log);
                        }
                        continue;
                    }
                    else
                    {
                        var dbMemberEntry = entry.Member(member.Name) as DbPropertyEntry;
                        ChangeLog log = new ChangeLog()
                        {
                            AttributeName = propertyName,
                            AttributeDescription = propertyNameDescription,
                            NewValue = dbMemberEntry.CurrentValue == null ? "Null" : dbMemberEntry.CurrentValue.ToString(),
                            OriginalValue = null,
                        };
                        changes.Add(log);
                    }
                }
            }
            else if (entry.State == System.Data.EntityState.Modified)
            {
                foreach (string propertyName in props.Keys.ToList()) // If its an existing modified entity
                {
                    EdmMember member = edmMembers.First(e => e.Name == propertyName);
                    string propertyNameDescription = props[propertyName];

                    if (member.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty)
                    {
                        var referenceEntry = entry.Member(member.Name);
                        if (referenceEntry != null)
                        {
                            if (referenceEntry is DbCollectionEntry)
                            {
                                ICollection<IEntity> originalValue = new List<IEntity>();
                                Type itemType;
                                foreach (var item in ((DbCollectionEntry)referenceEntry).Query())
                                {
                                    itemType = item.GetType();
                                    originalValue.Add(item as IEntity);
                                }
                                dynamic currentValue = referenceEntry.CurrentValue;
                                
                                foreach (var item in currentValue)
                                {
                                    if (!originalValue.Any(e => e.ID == item.ID)) // Item was added
                                    {
                                        ChangeLog log = new ChangeLog()
                                        {
                                            AttributeName = propertyName,
                                            AttributeDescription = propertyNameDescription,
                                            NewValue = SerializeToString(item),
                                            OriginalValue = "Null",
                                        };
                                        changes.Add(log);
                                    }
                                }

                                foreach (var item in originalValue)
                                {
                                    bool found = false;
                                    foreach (var currentItem in currentValue)
                                    {
                                        if (currentItem.ID == item.ID)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }

                                    if (!found)
                                    {
                                        ChangeLog log = new ChangeLog()
                                        {
                                            AttributeName = propertyName,
                                            AttributeDescription = propertyNameDescription,
                                            NewValue = "Null",
                                            OriginalValue = SerializeToString(item),
                                        };
                                        changes.Add(log);
                                    }
                                }
                            }
                            else if (referenceEntry is DbReferenceEntry)
                            {
                                object originalValue = null;
                                foreach (var item in ((DbReferenceEntry)referenceEntry).Query())
                                {
                                    originalValue = item;
                                    break;
                                }

                                if (originalValue == referenceEntry.CurrentValue)
                                {
                                    continue;
                                }

                                ChangeLog log = new ChangeLog()
                                {
                                    AttributeName = propertyName,
                                    AttributeDescription = propertyNameDescription,
                                    NewValue = SerializeToString(referenceEntry.CurrentValue),
                                    OriginalValue = SerializeToString(originalValue),
                                };
                                changes.Add(log);
                            }
                        }
                        continue;
                    }
                    else
                    {
                        var dbMemberEntry = entry.Member(member.Name) as DbPropertyEntry;
                        if (Equals(dbMemberEntry.OriginalValue, dbMemberEntry.CurrentValue))
                        {
                            // Member entry isn't a property entry or it isn't modified.
                            continue;
                        }

                        ChangeLog log = new ChangeLog()
                        {
                            AttributeName = propertyName,
                            AttributeDescription = propertyNameDescription,
                            NewValue = dbMemberEntry.CurrentValue == null ? "Null" : dbMemberEntry.CurrentValue.ToString(),
                            OriginalValue = dbMemberEntry.OriginalValue == null ? "Null" : dbMemberEntry.OriginalValue.ToString(),
                        };
                        changes.Add(log);
                        continue;
                    }
                }
            }

            return changes.Count() == 0 ? null : changes;
        }

        private static string SerializeToString(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();
            var allProperties = obj.GetType().GetProperties();

            if (allProperties.Any(e => e.Name == "Name")) // If entity has a name, just log the name
            {
                builder.Append("Name: ");
                builder.Append(obj.GetType().GetProperty("Name").GetValue(obj));
            }
            else // log all suitable properties if name was not found
            {
                foreach (var property in obj.GetType().GetProperties())
                {
                    if (property.PropertyType == typeof(String)
                        || property.PropertyType == typeof(DateTime)
                        || property.PropertyType == typeof(int))
                    {
                        if (builder.Length != 0)
                        {
                            builder.Append(", ");
                        }
                        builder.Append(property.Name);
                        builder.Append(": ");
                        builder.Append(obj.GetType().GetProperty(property.Name).GetValue(obj));
                    }
                }
            }

            return builder.ToString();
        }

    }
}
