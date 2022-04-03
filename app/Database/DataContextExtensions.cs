using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace backend.Database
{
    public static class DataContextExtensions
    {
        public static IQueryable<T> LoadFields<T>(this DbSet<T> dataContext, string[]? fields = null)
            where T : BaseModel
        {
            if (fields == null)
            {
                var reflectedFields = typeof(T).GetProperties().ToList();

                reflectedFields.RemoveAll(field => !field.GetAccessors().Any(accessor => accessor.IsVirtual));
                reflectedFields.RemoveAll(field => field.Name == "IsDeleted");
                fields = reflectedFields.ConvertAll(tf => tf.Name).ToArray();
            }

            IQueryable<T> res = dataContext;
            foreach (var field in fields)
                res = res.Include(field);

            return res;
        }
    }
}
