using NerdyMishka;
using Xunit;

namespace Tests
{
    public class DotEnvTests
    {
        [Fact]
        public void Test1()
        {
            // TEST_DOUBLE purposely has whitespace before the quotet to test
            // trimming whitespace for values.
            var configValue = @"
TEST_VALUE=x
TEST_DOUBLE_ONE_LINE=""my multiline \ntext""
TEST_SINGLE_ONE_LINE='single'
TEST_DOUBLE= ""woah
I have multiline values
yea!""
TEST_EMPTY=
TEST_JSON={
    ""name"": ""value"",
    ""array"": [
        ""x"",
        ""y"",
        ""z""
    ]
}
  
# yo, this is comment";

            var json = @"{
    ""name"": ""value"",
    ""array"": [
        ""x"",
        ""y"",
        ""z""
    ]
}";
            var multi = @"woah
I have multiline values
yea!";
            var values = DotEnvReader.ReadString(configValue);
            Assert.NotNull(values);

            Assert.Equal("x", values["TEST_VALUE"]);
            Assert.Equal("my multiline \r\ntext", values["TEST_DOUBLE_ONE_LINE"]);
            Assert.Equal("single", values["TEST_SINGLE_ONE_LINE"]);
            Assert.Equal(json, values["TEST_JSON"]);
            Assert.Equal(multi, values["TEST_DOUBLE"]);
            Assert.Null(values["TEST_EMPTY"]);
        }
    }
}
