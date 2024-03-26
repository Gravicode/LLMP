using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Connectors.AI.PaLM.Helper
{
    internal static class PaLMInternalTypeConverter
    {
        /// <summary>
        /// Converts the given object value to a string representation using the appropriate CultureInfo.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="culture">The CultureInfo to consider during conversion.</param>
        /// <returns>A string representation of the object value, considering the specified CultureInfo.</returns>
        public static string? ConvertToString(object? value, CultureInfo? culture = null)
        {
            if (value == null) { return null; }

            var sourceType = value.GetType();

            var converterDelegate = GetTypeToStringConverterDelegate(sourceType);

            return converterDelegate == null
                ? value.ToString()
                : converterDelegate(value, culture ?? CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Retrieves a type-to-string converter delegate for the specified source type.
        /// </summary>
        /// <param name="sourceType">The source Type for which to retrieve the type-to-string converter delegate.</param>
        /// <returns>A Func delegate for converting the source type to a string, considering CultureInfo, or null if no suitable converter is found.</returns>
        private static Func<object?, CultureInfo, string?>? GetTypeToStringConverterDelegate(Type sourceType) =>
            s_converters.GetOrAdd(sourceType, static sourceType =>
            {
                // Strings just render as themselves.
                if (sourceType == typeof(string))
                {
                    return (input, cultureInfo) => (string)input!;
                }

                // Look up and use a type converter.
                if (TypeConverterFactory.GetTypeConverter(sourceType) is TypeConverter converter && converter.CanConvertTo(typeof(string)))
                {
                    return (input, cultureInfo) =>
                    {
                        return converter.ConvertToString(context: null, cultureInfo, input);
                    };
                }

                return null;
            });

        /// <summary>Converter functions for converting types to strings.</summary>
        private static readonly ConcurrentDictionary<Type, Func<object?, CultureInfo, string?>?> s_converters = new();
    }
    internal static class TypeConverterFactory
    {
        /// <summary>
        /// Returns a TypeConverter instance for the specified type.
        /// </summary>
        /// <param name="type">The Type of the object to convert.</param>
        /// <returns>A TypeConverter instance if a suitable converter is found, otherwise null.</returns>
        internal static TypeConverter? GetTypeConverter(Type type)
        {
            // In an ideal world, this would use TypeDescriptor.GetConverter. However, that is not friendly to
            // any form of ahead-of-time compilation, as it could end up requiring functionality that was trimmed.
            // Instead, we just use a hard-coded set of converters for the types we know about and then also support
            // types that are explicitly attributed with TypeConverterAttribute.

            if (type == typeof(string)) { return new StringConverter(); }
            if (type == typeof(byte)) { return new ByteConverter(); }
            if (type == typeof(sbyte)) { return new SByteConverter(); }
            if (type == typeof(bool)) { return new BooleanConverter(); }
            if (type == typeof(ushort)) { return new UInt16Converter(); }
            if (type == typeof(short)) { return new Int16Converter(); }
            if (type == typeof(char)) { return new CharConverter(); }
            if (type == typeof(uint)) { return new UInt32Converter(); }
            if (type == typeof(int)) { return new Int32Converter(); }
            if (type == typeof(ulong)) { return new UInt64Converter(); }
            if (type == typeof(long)) { return new Int64Converter(); }
            if (type == typeof(float)) { return new SingleConverter(); }
            if (type == typeof(double)) { return new DoubleConverter(); }
            if (type == typeof(decimal)) { return new DecimalConverter(); }
            if (type == typeof(TimeSpan)) { return new TimeSpanConverter(); }
            if (type == typeof(DateTime)) { return new DateTimeConverter(); }
            if (type == typeof(DateTimeOffset)) { return new DateTimeOffsetConverter(); }
            if (type == typeof(Uri)) { return new UriTypeConverter(); }
            if (type == typeof(Guid)) { return new GuidConverter(); }
            if (type.IsEnum) { return new EnumConverter(type); }

            if (type.GetCustomAttribute<TypeConverterAttribute>() is TypeConverterAttribute tca &&
                Type.GetType(tca.ConverterTypeName, throwOnError: false) is Type converterType &&
                Activator.CreateInstance(converterType) is TypeConverter converter)
            {
                return converter;
            }

            return null;
        }
    }
}
