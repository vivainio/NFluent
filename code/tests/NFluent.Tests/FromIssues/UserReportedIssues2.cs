﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="UserReportedIssues2.cs" company="NFluent">
//   Copyright 2018 Thomas PIERRAIN & Cyrille DUPUYDAUBY
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace NFluent.Tests.FromIssues
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
#if DOTNET_45
    using System.Threading.Tasks;
#endif
    using Helpers;
    using NFluent.Helpers;
    using NUnit.Framework;

    [TestFixture]
    public class UserReportedIssues2
    {
        // GH #292
        [Test]
        public void Awkward_behaviour_with_NFluent()
        {
            var toBeChecked = new object[]
            {
                1,
                2,
                3,
                4
            };

            IEnumerable<int> expected = Enumerable.Range(1, 4);
            
            CollectionAssert.AreEquivalent(expected, toBeChecked);  // OK 
            CollectionAssert.AreEqual(expected, toBeChecked);       // OK
            Check.That(toBeChecked).IsEquivalentTo(expected);      // KO ;-(

        }

        // GH #292
        [Test]
        public void Exception()
        {
            IEnumerable<char> e1 = "test";
            IEnumerable<char> e2 = "test2";

            Check.ThatCode(() =>  Check.That(e1).ContainsExactly(e2)).IsAFailingCheck();
        }

        // GH #290
        [Test]
        public void IsEquivalentToShouldWorkOnDictionaries()
        {
            var dict = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };
            var expected = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };
            Check.That(dict).IsEqualTo(expected);
            Check.That(dict).IsEquivalentTo(expected);
#if DOTNET_45
            var value = new ReadOnlyDictionary<string, object>(dict);
            Check.That(value).IsEqualTo(expected);
            Check.That(value).IsEquivalentTo(expected);
#endif
        }

        // GH #286
        [Test]
        public void IsNotZeroFailsForDecimalCloseToZero()
        {
            Check.That(0.12m).IsNotZero();
            Check.That(long.MaxValue).IsNotZero();
        }

        // GH #269 error sor some timespan
        [Test]
        public void IssueWithTimeSpanMinVal()
        {
            Check.That(TimeSpan.MinValue).IsEqualTo(TimeSpan.MinValue); // Ok with NFluent 2.2 but Fails with NFluent 2.3 !
            Check.That(TimeSpan.MinValue).IsLessThan(TimeSpan.MinValue + TimeSpan.FromTicks(1));
            Check.That(TimeSpan.MaxValue).IsGreaterThan(TimeSpan.MaxValue - TimeSpan.FromTicks(1));
        }

        // GH #266 Error using ContainsOnlyElementsThatMatch
        [Test]
        public void IssuesWithEnumerables()
        {
            var integers = new[] { 2, 6 };
// those checks succeed (at least one even entry)
            Check.That(integers).ContainsOnlyElementsThatMatch(x => x % 2 == 0);
        }
        // GH #261 issues with strings with brackets
        [Test]
        public void TestBrackets() {
            const string haystack = "Hello, {LeMonde}";
            const string needle = "{World}";
            Check.ThatCode(()=>
            Check.That(haystack).Contains(needle)).IsAFailingCheck(); // This will intentionally fail
        }

        // GH #258
        [Test]
        public void IsNotNullShouldSupportAs()
        {
            Check.ThatCode(() =>
                Check.That((string) null)
                    .As("foo")
                    .IsNotNull()).IsAFailingCheckWithMessage("", 
                "The checked [foo] must not be null.");
        }

        // GH #257
        [Test]
        public void
            ShouldSupportMultidimensionalArray()
        {
            var array = Array.CreateInstance(typeof(int), new []{2,2}, new []{-1, -1});
            var otherArray = new int[2, 2];
            var val = 0;
            for (var i = 0; i < 2; i++)
            {
                for(var j = 0; j < 2; j++)
                {
                    array.SetValue(val, i-1, j-1);
                    otherArray[i, j] = val;
                    val++;
                }
            }
            var myClass = new  {Property = array};
            Check.That(myClass).HasFieldsWithSameValues(myClass);
            var myOther = new {Property = otherArray};
            myOther.Property[1, 1] = 5;
            Check.ThatCode(() => Check.That(myClass).HasFieldsWithSameValues(myOther)).IsAFailingCheckWithMessage("", 
                "The checked value's field 'Property[1,1]' does not have the expected value.", 
                "The checked value's field 'Property[1,1]':",
                "\t[3]",
                "The expected value's field 'Property[1,1]':",
                "\t[5]");
        }

        class MyType{
            public int[,] Property { get; set; }
        }
        // GH #254
        [Test]
        public void HasElementThatMatchesShouldHandleArrays()
        {
            IEnumerable<string> randomWords = new [] { "yes", "foo", "bar" };
            Check.That(randomWords).HasElementThatMatches((_) => _.StartsWith("ye"));
        }        

        [Test]
        public void ContainsOnlyElementsThatMatchShouldHandleArrays()
        {
            IEnumerable<int> positiveNumbers = new [] { 4, 8, 7 };
            Check.That(positiveNumbers).ContainsOnlyElementsThatMatch((_) => _ >= 0);
        }     

        // GH #143
        [Test]
        public void ThrowShouldCaptureTypeParameters()
        {
            Check.ThatCode(() => throw new MyException<InvalidOperationException>())
                .Throws<MyException<InvalidOperationException>>();
        }

        private class MyException<TU> : Exception where TU : Exception
        {

        }

        // GH #244
        [Test]
        public void Test_Enum_That_Should_Be_In_Error_But_Is_Not()
        {
            var testClass1 = new TestClass
            {
                IntProperty = 10,
                TestEnumProperty = TestEnum.Test2
            };

            var testClass2 = new TestClass
            {
                IntProperty = 10,
                TestEnumProperty = TestEnum.Test3
            };
            Check.ThatCode(() =>
                Check.That(testClass2).HasFieldsWithSameValues(testClass1)).IsAFailingCheck();
        }

        public class TestClass
        {
            public int IntProperty { get; set; }

            public TestEnum TestEnumProperty { get; set; }
        }

        public enum TestEnum
        {
            Test1,
            Test2,
            Test3,
            Test4
        }

    // GH #238
        public class NTest
        {
            private class TestCase
            {
                public List<int> Items { get; set; } = new List<int>();
            }

            [Test]
            public void Test()
            {
                TestCase a = new TestCase();
                TestCase b = new TestCase();

                a.Items.Add(1);
                a.Items.Add(2);
                a.Items.Add(3);

                b.Items.Add(1);
                b.Items.Add(2);
                b.Items.Add(3);

                Check.That(a).Considering().Public.Properties.IsEqualTo(b);
            }
        }


        // GH #231
        [Test]
        public void ShouldNotMixUpCheckedAndExpected()
        {
            double d = 1.0d;
            Check.ThatCode(()=> Check.That(d).IsEqualTo(2.0)).IsAFailingCheckWithMessage(
                "", 
                "The checked value is different from the expected one.", 
                "The checked value:", 
                "\t[1]", 
                "The expected value:", 
                "\t[2]");
        }

        // GH #219
        public class Parent
        {
            public virtual string AutoProperty { get; set; }
        }

        public class Child : Parent
        {
            public override string AutoProperty { get; set; }
        }

        // GH #215
        public class MyClass
        {
            public ISubClass SubClass { get; set; }
        }

        public interface ISubClass
        {
            int MyProperty { get; set; }
        }

        public class SubClass : ISubClass
        {
            public int MyProperty { get; set; }
        }

        private static double DecimalValue => 0.95000000000000006d;

        private class BaseClass
        {
            public BaseClass(string id)
            {
                this.Id = id;
            }

            public string Id { get; }
        }

        private class SomeClass : BaseClass
        {
            public SomeClass(string id) : base(id)
            {
            }

            public string Salt => Id;
            public string Other { get; set; }
            public Dictionary<string, string> Values { get; set; }
        }

        [Test]
        public void CollectionTest()
        {
            var a = new List<int> {1, 2};
            var b = new List<int> {3, 4};

            // List contains references to a and b
            var list1 = new List<List<int>> {a, b};
            Check.That(list1).ContainsExactly(a, b); // OK
            Check.That(list1).ContainsExactly(new List<List<int>> {a, b}); // OK

            // List contains new instances of lists same as a and b
            var list2 = new List<List<int>>
            {
                new List<int> {1, 2}, // new instance, same as a
                new List<int> {3, 4} // new instance, same as b
            };
            Check.That(list2).ContainsExactly(a, b); // Fail
            Check.That(list2).ContainsExactly(new List<List<int>> {a, b}); // Fail
        }

        // GH #212
        [Test(Description = "Issue #212")]
        public void CollectionWithNumeric()
        {
            var expected = new[] {1, 2};
            var actual = new uint[] {1, 2};
            Check.That(actual).ContainsExactly(expected);
        }

        // GH #205 
        [Test]
        public void IsEqualTo_should_provide_details_and_suggest_isCloseTo()
        {
            using (new CultureSession("en-US"))
            {
                Check.ThatCode(() => Check.That(DecimalValue*(1<<16)).IsEqualTo(0.95d*(1<<16))).IsAFailingCheckWithMessage("",
                    "The checked value is different from the expected one, with a difference of 7.3E-12. You may consider using IsCloseTo() for comparison.",
                    "The checked value:",
                    "\t[62259.2]",
                    "The expected value:",
                    "\t[62259.2]");

                Check.ThatCode(() => Check.That(0.9500001f*(1<<16)).IsEqualTo(0.95f*(1<<16))).IsAFailingCheckWithMessage("",
                    "The checked value is different from the expected one, with a difference of 0.0078. You may consider using IsCloseTo() for comparison.",
                    "The checked value:",
                    "\t[62259.21]",
                    "The expected value:",
                    "\t[62259.2]");
                Check.ThatCode(() => Check.That(100001f).IsEqualTo(100000f)).IsAFailingCheckWithMessage("",
                    "The checked value is different from the expected one, with a difference of 1.",
                    "The checked value:",
                    "\t[100001]",
                    "The expected value:",
                    "\t[100000]");
                
                Check.ThatCode(() => Check.That(100000001d).IsEqualTo(100000000d)).IsAFailingCheckWithMessage("",
                    "The checked value is different from the expected one, with a difference of 1.",
                    "The checked value:",
                    "\t[100000001]",
                    "The expected value:",
                    "\t[100000000]");
                
            }
        }

        [Test]
        public void should_not_provide_diff_value_when_at_limit()
        {
            var refValue = 10000;
            var edgeValue = refValue + 0.0001 * refValue;
            Check.ThatCode(() => Check.That((float)edgeValue).IsEqualTo(refValue)).
                IsAFailingCheckWithMessage(	"", 
                    "The checked value is different from the expected one.", 
                    "The checked value:",
                    "\t[10001]",
                    "The expected value:", 
                    "\t[10000]");

            Check.ThatCode(() => Check.That(edgeValue).IsEqualTo(refValue)).
                IsAFailingCheckWithMessage(	"", 
                    "The checked value is different from the expected one.", 
                    "The checked value:",
                    "\t[10001]",
                    "The expected value:", 
                    "\t[10000]");
        }

        [Test]
        public void should_recognize_autoproperty_readonly_values()
        {
            var someClass = new SomeClass("Hello")
            {
                Other = "world",
                Values = new Dictionary<string, string> {["key1"] = "value1"}
            };
            Check.That(someClass).HasFieldsWithSameValues(new
            {
                Other = "world",
                Values = new Dictionary<string, string> {["key1"] = "value1"}
            });
        }

        [Test]
        // GH #226
        public void SupportForWildcards()
        {
            var actualString =
                $"Events with validFrom date in the future aren't supported yet by the platform. Overall Save will be discarded due to event(s){Environment.NewLine}[FondsCurrencyChanged: validFrom=2019-02-13-230000000, createdAt=2018-02-14-102858250]";

            Check.That(actualString).Matches(
                $"Events with validFrom date in the future aren't supported yet by the platform. Overall Save will be discarded due to event\\(s\\){Environment.NewLine}\\[FondsCurrencyChanged: validFrom=.*, createdAt=2018-02-14-.*");
        }

        [Test]
        public void Test()
        {
            var first = new MyClass();
            var second = new MyClass();

            Check.That(first).HasFieldsWithSameValues(second);
        }

        [Test]
        public void TestMethod()
        {
            // Arrange
            var autoPropertyValue = "I am a test.";
            var childOne = new Child {AutoProperty = autoPropertyValue};

            // Act
            var childTwo = new Child {AutoProperty = autoPropertyValue};

            // Assert
            Check.That(childOne).HasFieldsWithSameValues(childTwo);
        }

#if DOTNET_45
        [Test]
        public void reproduce_issue_204()
        {
            Check.ThatAsyncCode(() => this.Execute(5)).Throws<Exception>();
        }

        private Task<int> Execute(int i)
        {
            if (i == 5)
            {
                throw new Exception("bad");
            }

            return Task.FromResult(i);
        }
#endif
    }
}