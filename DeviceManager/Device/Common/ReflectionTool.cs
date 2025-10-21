using System.Collections.Immutable;
using System.Reflection;

namespace DeviceManager
{
    internal static class ReflectionTool
    {
        public static PropertyInfo GetPropertyByName<T>(T obj, string name)
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
        
        /// <summary>
        /// Prints properties ordered by position
        /// </summary>
        public static string PrintProps<T>(T obj)
        {
            return string.Join(", ", GetPropertiesValues(obj)
                .Select(x => x.Key + ": " + x.Value));
        }

        /// <summary>
        /// Prints properties ordered by position (without suppressed)
        /// </summary>
        public static string PrintFilteredProps<T>(T obj)
        {
            return string.Join(", ", GetPropertiesValues(obj)
                .SkipWhile(x => GetPropertyByName(obj, x.Key)
                    .GetCustomAttribute<LoggedAttribute>()!.Position < -99)
                .Select(x => x.Key + ": " + x.Value));
        }

        public static OrderedDictionary<string,object> GetPropertiesValues<T>(T obj)
        {
            var props = GetAnnotatedProperties(obj, typeof(LoggedAttribute));
            var sorted = props
                .OrderBy(x => x.GetCustomAttribute<LoggedAttribute>()!.Position)
                .Select(p => (p.Name, p.GetValue(obj)))
                .ToImmutableList();
            var dict = new OrderedDictionary<string, object>();
            foreach (var prop in sorted)
            {
                dict.Add(prop.Item1, prop.Item2);
            }
            return dict;
        }
        
        public static IEnumerable<PropertyInfo> GetEditableProperties<T>(T obj) where T : class
        {
            var type = obj.GetType();
            var properties = type.GetProperties()
                .Where(x=> Attribute.IsDefined(x, typeof(EditablePropertyAttribute)));
            return properties;
        }
    }
}
