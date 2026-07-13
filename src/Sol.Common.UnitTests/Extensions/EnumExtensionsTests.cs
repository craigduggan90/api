using Sol.Common.Exceptions;
using Sol.Common.Extensions;
using System.ComponentModel;
using System.Text.Json;

namespace Sol.Common.UnitTests.Extensions;

public static class EnumExtensionsTests
{
    public class GetName
    {
        [Fact]
        public void GetName_ReturnsEnumName()
        {
            const TestEnum value = TestEnum.Option2;
            const string expected = "Option2";
            Assert.Equal(expected, value.GetName());
        }

        [Fact]
        public void GetName_ReturnsFormattedEnumName_WithCustomFunction()
        {
            const TestEnum value = TestEnum.Option2;
            const string expected = "OPTION2";
            Assert.Equal(expected, value.GetName(s => s.ToUpper()));
        }

        [Fact]
        public void GetName_ReturnsFormattedEnumName_WithMethodGroup()
        {
            const TestEnum value = TestEnum.Option2;
            const string expected = "option2";
            Assert.Equal(expected, value.GetName(JsonNamingPolicy.CamelCase.ConvertName));
        }
    }

    public class GetDescription
    {
        [Fact]
        public void GetDescription_ReturnsNull_WhenNoDescriptionAvailable()
        {
            const TestEnum value = TestEnum.Option1;
            Assert.Null(value.GetDescription());
        }

        [Fact]
        public void GetDescription_ReturnsDescriptionAttributeValue_WhenDescriptionAvailable()
        {
            var description = TestEnum.Option2.GetDescription();
            Assert.Equivalent("Option2 Description", description);
        }
    }

    public class GetEnumNamesByValue
    {
        [Fact]
        public void ShouldThrowEnumConversionException_WhenEnumTypeDoesNotHaveFlagsAttribute()
        {
            const TestEnum input = TestEnum.Option1;
            Assert.Throws<EnumConversionException>(() => input.GetEnumNamesByValue());
        }

        [Fact]
        public void ShouldReturnSelectedOptions_WhenEnumTypeNotAssignableToInt32()
        {
            const LongEnum input = LongEnum.Uno | LongEnum.Cuatro;
            string[] expected = ["cero", "uno", "cuatro"];
            var actual = input.GetEnumNamesByValue();
            Assert.Equivalent(expected, actual, true);
        }

        [Fact]
        public void ShouldReturnSelectedOptions_WhenEnumTypeNotAssignableToInt32_AndFormatterProvided()
        {
            const LongEnum input = LongEnum.Uno | LongEnum.Cuatro;
            string[] expected = ["CERO", "UNO", "CUATRO"];
            var actual = input.GetEnumNamesByValue(x => x.ToUpperInvariant());
            Assert.Equivalent(expected, actual, true);
        }

        [Fact]
        public void ShouldReturnSelectedOptions_WhenEnumTypeAssignableToInt32()
        {
            const FlagsEnum input = FlagsEnum.One | FlagsEnum.Four;
            string[] expected = ["zero", "one", "four"];
            var actual = input.GetEnumNamesByValue();
            Assert.Equivalent(expected, actual, true);
        }

        [Fact]
        public void ShouldReturnSelectedOptions_WhenEnumTypeAssignableToInt32_AndFormatterProvided()
        {
            const FlagsEnum input = FlagsEnum.One | FlagsEnum.Four;
            string[] expected = ["ZERO", "ONE", "FOUR"];
            var actual = input.GetEnumNamesByValue(x => x.ToUpperInvariant());
            Assert.Equivalent(expected, actual, true);
        }
    }

    public class GetEnumValuesByName
    {
        [Fact]
        public void ShouldThrowEnumConversionException_WhenEnumTypeNotAssignableToInt32()
        {
            string[] input = ["one", "two", "three"];
            Assert.Throws<EnumConversionException>(() => input.GetEnumValuesByName<LongEnum>());
        }

        [Fact]
        public void ShouldThrowEnumConversionException_WhenEnumTypeDoesNotHaveFlagsAttribute()
        {
            string[] input = ["Option1", "Option2"];
            Assert.Throws<EnumConversionException>(() => input.GetEnumValuesByName<TestEnum>());
        }

        [Fact]
        public void ShouldReturnNull_WhenNoValuesDeclaredInEnum()
        {
            string[] input = ["uno", "dos", "tres"];
            var actual = input.GetEnumValuesByName<FlagsEnum>();
            Assert.Null(actual);
        }

        [Fact]
        public void ShouldReturnExpectedCollection_WhenValidValuesProvided()
        {
            string[] input = ["zero", "two", "four"];
            const FlagsEnum expected = FlagsEnum.Zero | FlagsEnum.Two | FlagsEnum.Four;
            var actual = input.GetEnumValuesByName<FlagsEnum>();
            Assert.Equivalent(expected, actual, true);
        }

        [Fact]
        public void ShouldReturnExpectedCollection_WhenMalformedValueProvided()
        {
            string[] input = ["Zero", "TWO", "Four"];
            const FlagsEnum expected = FlagsEnum.Zero | FlagsEnum.Four;
            var actual = input.GetEnumValuesByName<FlagsEnum>(StringComparer.Ordinal);
            Assert.Equivalent(expected, actual, true);
        }

        [Fact]
        public void ShouldIgnoreValues_WhenNotDeclaredInEnum()
        {
            string[] input = ["zero", "two", "four", "five"];
            const FlagsEnum expected = FlagsEnum.Zero | FlagsEnum.Two | FlagsEnum.Four;
            var actual = input.GetEnumValuesByName<FlagsEnum>();
            Assert.Equivalent(expected, actual, true);
        }
    }
}

internal enum TestEnum
{
    Option1,

    [Description("Option2 Description")]
    Option2
}

[Flags]
internal enum LongEnum : long
{
    Cero = 0,
    Uno = 1,
    Dos = 2,
    Tres = Uno | Dos,
    Cuatro = 4
}

[Flags]
internal enum FlagsEnum
{
    Zero = 0,
    One = 1,
    Two = 2,
    Three = 1 | 2,
    Four = 4
}