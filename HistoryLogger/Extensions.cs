using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HistoryLogger
{
    public static class Extensions
    {
        public static void AddHistoryTable(this ModelBuilder modelBuilder)
        {
            var entityMethod = typeof(ModelBuilder).GetMethods()
                .First(e => e.Name == "Entity");
            entityMethod.MakeGenericMethod(typeof(History))
                .Invoke(modelBuilder, new object[] { });
        }
    }
}