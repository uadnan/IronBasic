#if !NETFX_45

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    public static class ReflectionHelpers
    {
        public static T GetCustomAttribute<T>(this FieldInfo fieldInfo, bool inherit = false)
            where T : Attribute
        {
            if (fieldInfo == null)
                throw new ArgumentNullException(nameof(fieldInfo));

            var attributes = fieldInfo.GetCustomAttributes(typeof(T), inherit);
            if (attributes.Length > 0)
                return (T)attributes[0];

            return null;
        }
    }
}
#endif