namespace backend.Models
{
    public abstract class BaseModel
    {
        public T BaseObject<T>()
            where T : BaseModel, new()
        {
            T ret = new T();
            foreach(var property in ret.GetType().GetProperties())
                property.SetValue(ret, property.GetValue(this));

            return ret;
        }
    }
}
