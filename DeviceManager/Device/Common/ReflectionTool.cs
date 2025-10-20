using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace DeviceManager
{
    internal static class ReflectionTool
    {
        public static PropertyInfo GetPropertyByName<T>(T obj, string name) where T : class
        {
            var type = obj.GetType();
            var properties = type.GetProperties().First(x => x.Name == name);
            return properties;
        }

        public static IEnumerable<PropertyInfo> GetAnnotatedProperties<T>(T obj, Type atr)
        {
            var type = obj.GetType();
            var properties = type.GetProperties()
                .Where(x => Attribute.IsDefined(x, atr));
            return properties;

        }

        public static string PrintProps<T>(T obj)
        {
            var text = new StringBuilder();
            var props = GetAnnotatedProperties(obj, typeof(LoggedAttribute));            
            props = props.OrderBy(x => x.GetCustomAttribute<LoggedAttribute>()!.Position);
            var list = props.ToImmutableList();
            for (int i = 0; i < list.Count; i++)
            {
                text.Append(list[i].Name + ": ");
                text.Append(list[i].GetValue(obj));
                if (i < list.Count - 1) {
                    text.Append(", ");
                }
            }
            return text.ToString();
            
        }

    }
}
