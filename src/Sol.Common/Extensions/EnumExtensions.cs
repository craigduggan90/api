using Sol.Common.Exceptions;
using Sol.Common.Formatters;
using System.ComponentModel;
using System.Globalization;

namespace Sol.Common.Extensions;

/// <summary>Extension methods for base <see cref="Enum"/> types.</summary>
public static class EnumExtensions
{
    /// <summary>Gets the name value of an enum entry.</summary>
    /// <param name="value">The enum entry.</param>
    /// <typeparam name="TEnum">The type of enum of which `value` is an entry.</typeparam>
    /// <returns>The name if available; otherwise null.</returns>
    public static string? GetName<TEnum>(this TEnum value)
        where TEnum : struct, Enum
        => Enum.GetName(value);

    /// <summary>Gets the name value of an enum entry formatted using the given method.</summary>
    /// <param name="value">The enum entry.</param>
    /// <param name="formatter">The value formatter.</param>
    /// <typeparam name="TEnum">The type of enum of which `value` is an entry.</typeparam>
    /// <returns>
    /// The enum option name if available, otherwise the output of <c>ToString()</c>, formatted using the given method.
    /// </returns>
    public static string GetName<TEnum>(this TEnum value, Func<string, string> formatter)
        where TEnum : struct, Enum
    {
        var name = Enum.GetName(value) ?? value.ToString();
        return formatter.Invoke(name);
    }

    /// <summary>Gets the Description attribute value of an enum entry.</summary>
    /// <param name="value">The enum entry.</param>
    /// <returns>The description if available; otherwise null.</returns>
    public static string? GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attributes = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .OfType<DescriptionAttribute>()
            .ToArray() ?? [];

        return attributes.Any()
            ? attributes.First().Description
            : null;
    }

    /// <summary>Get the entry names of selected options in a Flags enum.</summary>
    /// <param name="enum">The value from which to extract the name collection.</param>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    public static IEnumerable<string> GetEnumNamesByValue<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
        => @enum.GetEnumNamesByValue(StringFormatters.CamelCaseFormatter);

    /// <summary>Get the entry names of selected options in a Flags enum.</summary>
    /// <param name="enum">The value from which to extract the name collection.</param>
    /// <param name="formatter">Function used to format the entry names.</param>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    public static IEnumerable<string> GetEnumNamesByValue<TEnum>(this TEnum @enum, Func<string, string> formatter)
        where TEnum : struct, Enum
    {
        if (!Attribute.IsDefined(typeof(TEnum), typeof(FlagsAttribute)))
            throw new EnumConversionException($"{typeof(TEnum).Name} is not a Flags enum.");

        return Enum.GetValues<TEnum>()
            .Where(option => @enum.HasFlag(option))
            .Select(option => option.GetName(formatter));
    }

    /// <summary>Get the flags enum value representing a given selection of enum entry names.</summary>
    /// <param name="names">The entry names to include in value generation.</param>
    /// <typeparam name="TEnum">The type of the enum to which values are converted.</typeparam>
    public static TEnum? GetEnumValuesByName<TEnum>(this IEnumerable<string> names)
        where TEnum : struct, Enum
        => names.GetEnumValuesByName<TEnum>(StringComparer.OrdinalIgnoreCase);

    /// <summary>Get the flags enum value representing a given selection of enum entry names.</summary>
    /// <param name="names">The entry names to include in value generation.</param>
    /// <param name="comparerType">The string comparer to use when evaluating names against available values.</param>
    /// <typeparam name="TEnum">The type of the enum to which values are converted.</typeparam>
    public static TEnum? GetEnumValuesByName<TEnum>(this IEnumerable<string> names, StringComparer comparerType)
        where TEnum : struct, Enum
    {
        if (!Enum.GetUnderlyingType(typeof(TEnum)).IsAssignableTo(typeof(int)))
            throw new EnumConversionException($"{typeof(TEnum).Name} is not assignable to Int32.");

        if (!Attribute.IsDefined(typeof(TEnum), typeof(FlagsAttribute)))
            throw new EnumConversionException($"{typeof(TEnum).Name} is not a Flags enum.");

        var result = Enum.GetValues<TEnum>().Where(option => names.Contains(option.GetName(), comparerType))
            .Select(option => Convert.ToInt32(option, CultureInfo.InvariantCulture))
            .Aggregate<int, int?>(null, (current, option) => (current ?? 0) | option);

        return result is null
            ? null
            : (TEnum)Enum.ToObject(typeof(TEnum), result);
    }
}