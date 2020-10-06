using System;

namespace HistoryLogger
{
    public class LogDataMemberAttribute : Attribute
    {
        public string CustomName;
        public LogDataMemberAttribute()
        {
        }
        public LogDataMemberAttribute(string name)
        {
            CustomName = name;
        }
    }
}