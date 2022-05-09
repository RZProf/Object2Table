using System.Collections.Generic;
using FluentAssertions;
using Object2Table.UnitTests.Models;
using Xunit;

namespace Object2Table.UnitTests
{
    public class ObjectPrinterUnitTest
    {
        [Fact]
        public void TestPrint_WhenSimpleObject_ResultMustBeAsExpected()
        {
            var result = ObjectPrinter.Print(new SimplePerson{Age = 12, Name = "Qoli"});
            const string expected = @"--------------
| Name | Age |
--------------
| Qoli | 12  |
--------------";
            result.Should().Be(expected);
        }
        
        [Fact]
        public void TestPrint_WhenObjectContainsDictionary_ResultMustBeAsExpected()
        {
            var result = ObjectPrinter.Print(new PersonWithMeta
            {
                Age = 11,
                Name = "Qoli",
                Meta = new Dictionary<string, object>{{"FatherName", "Ahmad"}, {"FatherAge", 13}, {"Mother", new SimplePerson{Age = 12}}}
            });
            const string expected = @"----------------------------------------------
|             Meta              | Name | Age |
----------------------------------------------
|  FatherName       Ahmad       | Qoli | 11  |
|  FatherAge          13        |      |     |
|    Mother     --------------  |      |     |
|               | Name | Age |  |      |     |
|               --------------  |      |     |
|               |  -   | 12  |  |      |     |
|               --------------  |      |     |
----------------------------------------------";
            result.Should().Be(expected);
        }
        
        [Fact]
        public void TestPrint_WhenObjectContainsDictionaryArray_ResultMustBeAsExpected()
        {
            var result = ObjectPrinter.Print(new PersonWithMetas
            {
                Age = 11,
                Name = "Qoli",
                Metas = new[]
                {
                    null,
                    new Dictionary<string, object> {{"FatherName", "Ahmad"}, {"FatherAge", 13}, {"Mother", new SimplePerson {Age = 12}}},
                    new Dictionary<string, object> {{"Degree", "Master"}, {"NumberOdCourses", 1300}}
                }
            });
            
            const string expected = @"----------------------------------------------
|             Metas             | Name | Age |
----------------------------------------------
|               -               | Qoli | 11  |
|  FatherName       Ahmad       |      |     |
|  FatherAge          13        |      |     |
|    Mother     --------------  |      |     |
|               | Name | Age |  |      |     |
|               --------------  |      |     |
|               |  -   | 12  |  |      |     |
|               --------------  |      |     |
|       Degree        Master    |      |     |
|   NumberOdCourses    1300     |      |     |
----------------------------------------------";
            result.Should().Be(expected);
        }
    }
}