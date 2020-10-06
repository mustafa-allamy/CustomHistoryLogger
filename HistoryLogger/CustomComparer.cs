using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HistoryLogger
{
    public class CustomComparer
    {
        public async Task<bool> SaveHistory<T, T1>(T oldObject, T1 newObject,string userId, DbContext context)
        {
            var compareResult = Compare(oldObject, newObject);
            foreach (var result in compareResult)
            {
                var id = typeof(T).GetProperties().FirstOrDefault(x => x.Name == "Id")?.GetValue(oldObject);
                context.Entry(new History()
                {
                    FieldName = result.Name,
                    ItemId = id?.ToString(),
                    ModelName = typeof(T).Name,
                    OriginalValue = result.OldValue.ToString(),
                    ChangedValue = result.NewValue.ToString(),
                    UserId = userId,
                }).State=EntityState.Added;
            }

            await context.SaveChangesAsync();
            return  true;
        }
        private  List<PropertyCompareResult> Compare<T, T1>(T oldObject, T1 newObject)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo[] propertiesT1 = typeof(T1).GetProperties();
            List<PropertyCompareResult> result = new List<PropertyCompareResult>();

            foreach (PropertyInfo p in properties)
            {
                if (p.CustomAttributes.All(ca => ca.AttributeType != typeof(LogDataMemberAttribute)))
                {
                    continue;
                }

                if (propertiesT1.All(x => x.Name != p.Name))
                {
                    continue;
                }
                if (p.GetType().IsClass)
                {
                    var res = Compare(p.GetValue(oldObject),
                        propertiesT1.FirstOrDefault(x => x.Name == p.Name)?.GetValue(newObject));
                    result.AddRange(res);
                }
                object oldValue = p.GetValue(oldObject), newValue = propertiesT1.FirstOrDefault(x => x.Name == p.Name)?.GetValue(newObject);

                if (!object.Equals(oldValue, newValue))
                {
                    var attributeName = p.GetCustomAttribute<LogDataMemberAttribute>();
                    if (string.IsNullOrEmpty(attributeName.CustomName))
                    {
                        result.Add(new PropertyCompareResult(p.Name, oldValue, newValue));

                    }
                    else
                    {
                        result.Add(new PropertyCompareResult(attributeName.CustomName, oldValue, newValue));
                    }
                }
            }

            return result;
        }
    }
    public class PropertyCompareResult
    {
        public string Name { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }

        public PropertyCompareResult(string name, object oldValue, object newValue)
        {
            Name = name;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
