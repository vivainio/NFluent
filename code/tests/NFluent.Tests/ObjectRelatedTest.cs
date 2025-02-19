﻿// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ObjectRelatedTest.cs" company="">
// //   Copyright 2013 Cyrille DUPUYDAUBY, Thomas PIERRAIN
// //   Licensed under the Apache License, Version 2.0 (the "License");
// //   you may not use this file except in compliance with the License.
// //   You may obtain a copy of the License at
// //       http://www.apache.org/licenses/LICENSE-2.0
// //   Unless required by applicable law or agreed to in writing, software
// //   distributed under the License is distributed on an "AS IS" BASIS,
// //   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// //   See the License for the specific language governing permissions and
// //   limitations under the License.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using System;

namespace NFluent.Tests
{
    using NFluent.Helpers;
    using NUnit.Framework;

    [TestFixture]
    public class ObjectRelatedTest
    {
        [Test]
        public void IsSameReferenceAsWorks()
        {
            var test = new object();
            Check.That(test).IsSameReferenceAs(test);
        }

        [Test]
        public void IsSameReferenceThanWorks()
        {
            var test = new object();
#pragma warning disable CS0618
            Check.That(test).IsSameReferenceThan(test);
#pragma warning restore CS0618
        }

        [Test]
        public void IsSameReferenceThanDoesNotLoseOriginalTypeForOtherCheck()
        {
            var numbers = new[] { 1, 4, 42 };
            Check.That(numbers).IsSameReferenceAs(numbers).And.Contains(42);
        }

        [Test]
        public void IsSameReferenceFailsProperly()
        {
            Check.ThatCode(() =>
            {
                Check.That(new object()).IsSameReferenceAs(new object());
            })
            .IsAFailingCheckWithMessage("",
                    "The checked object must be the same instance as the given one.",
                    "The checked object:",
                    "\t[System.Object]",
                    "The expected object: same instance as",
                    "\t[System.Object]");
        }

        [Test]
        public void IsDistinctWorks()
        {
            Check.That(new object()).IsDistinctFrom(new object());
        }
        
        [Test]
        public void IsDistinctFromDoesNotLoseOriginalTypeForOtherCheck()
        {
            var numbers = new[] { 1, 4, 42 };
            var otherNumbers = new[] { 7, 8, 9 };
            Check.That(numbers).IsDistinctFrom(otherNumbers).And.Contains(42);
        }

        [Test]
        public void IsNullWorks()
        {
            Check.That((object)null).IsNull();
        }

        [Test]
        public void IsNullWorksWithNullable()
        {
            Check.That((int?)null).IsNull();
        }

        [Test]
        public void IsNullDoesNotLoseOriginalTypeForOtherCheck()
        {
            var values = new[] { 0, 1, 2 };
            Check.That(values).Not.IsNull().And.HasSize(3);
        }

        [Test]
        public void IsNotNullWorksWithNullable()
        {
            Mood? goodMood = new Mood();
            Check.That(goodMood).IsNotNull();
        }

        [Test]
        public void IsNotNullThrowsExceptionWithNullNullable()
        {
            Check.ThatCode(() =>
            {
                Check.That((Mood?)null).IsNotNull();
            })
            .IsAFailingCheckWithMessage("",
                    "The checked nullable value is null whereas it must not.");
        }

        [Test]
        public void IsNotNullDoesNotLoseOriginalTypeForOtherCheck()
        {
            var values = new[] { 0, 1, 2 };
            Check.That(values).IsNotNull().And.HasSize(3);
        }

        [Test]
        public void NotIsNullWorksWithNullable()
        {
            Mood? goodMood = new Mood();
            Check.That(goodMood).Not.IsNull();
        }

        [Test]
        public void NotIsNotNullWorksWithNullable()
        {
            Check.That((Mood?)null).Not.IsNotNull();
        }
        
        [Test]
        public void NotIsNotNullThrowsWithNonNullNullable()
        {
            Mood? goodMood = new Mood();

            Check.ThatCode(() =>
            {
                Check.That(goodMood).Not.IsNotNull();
            })
            .IsAFailingCheckWithMessage("",
                    "The checked nullable value must be null.",
                    "The checked nullable value:",
                    "\t[NFluent.Tests.Mood]");
        }
        
        [Test]
        public void NotIsNullThrowsExceptionWithNullNullable()
        {
            Check.ThatCode(() =>
            {
                Check.That((Mood?)null).Not.IsNull();
            })
            .IsAFailingCheckWithMessage("", "The checked nullable value is null whereas it must not.");
        }

        [Test]
        public void IsNullFailsProperly()
        {
            Check.ThatCode(() =>
            {
                Check.That(new object()).IsNull();
            })
            .IsAFailingCheckWithMessage("",
                    "The checked object must be null.",
                    "The checked object:",
                    "\t[System.Object]");
        }

        [Test]
        public void IsNotNullWork()
        {
            Check.That(new object()).IsNotNull();
        }

        [Test]
        public void IsNotNullFailsProperly()
        {
            Check.ThatCode(() =>
            {
                Check.That((object)null).IsNotNull();
            })
            .IsAFailingCheckWithMessage("",
                    "The checked object must not be null.");
        }

        [Test]
        public void IsDistinctFailsProperly()
        {
            var test = new object();

            Check.ThatCode(() =>
            {
                Check.That(test).IsDistinctFrom(test);
            })
            .IsAFailingCheckWithMessage("",
                    "The checked object must be an instance distinct from the given one.",
                    "The checked object:",
                    "\t[System.Object]",
                    "The expected object: distinct from",
                    "\t[System.Object]");
        }

        [Test]
        public void NegatedSameWorks()
        {
            var test = new object();
            Check.That(test).Not.IsDistinctFrom(test);
            Check.That(new object()).Not.IsSameReferenceAs(new object());
        }

        [Test]
        public void HasSameValueAsFailsWithCorrectMessage()
        {
            var mySelf = new Person() { Name = "dupdob" };
            var myClone = new PersonEx() { Name = "tpierrain" };
            Check.ThatCode(() => {
                    Check.That(myClone).HasSameValueAs(mySelf);
                })
                .ThrowsAny()
                .WithMessage(
                    Environment.NewLine+ "The checked value is different from the expected one." + Environment.NewLine + "The checked value:" + Environment.NewLine + "\t[NFluent.Tests.ObjectRelatedTest+PersonEx] of type: [NFluent.Tests.ObjectRelatedTest+PersonEx]" + Environment.NewLine + "The expected value: equals to (using operator==)" + Environment.NewLine + "\t[NFluent.Tests.ObjectRelatedTest+Person] of type: [NFluent.Tests.ObjectRelatedTest+Person]");
            Check.ThatCode(() => {
                    Check.That(myClone).Not.HasSameValueAs(new Person() { Name = "tpierrain"});
                })
                .IsAFailingCheckWithMessage("",
                    "The checked value is equal to the given one whereas it must not.",
                    "The expected value: different from (using !operator==)",
                    "\t[NFluent.Tests.ObjectRelatedTest+Person] of type: [NFluent.Tests.ObjectRelatedTest+Person]");
        }

        [Test]
        public void HasDifferentValueAsFailsWithCorrectMessage()
        {
            var mySelf = new Person() { Name = "dupdob" };
            var myClone = new PersonEx() { Name = "dupdob" };

            Check.ThatCode(() => {
                    Check.That((object)null).HasDifferentValueThan((object)null);
                })
                .IsAFailingCheckWithMessage("",
                    "The checked object is equal to the expected one whereas it must not.",
                    "The expected object: different from (using operator!=)",
                    "\t[null]");

            Check.ThatCode(() => {
                Check.That(myClone).HasDifferentValueThan(mySelf);
            })
                .IsAFailingCheckWithMessage("",
                    "The checked value is equal to the expected one whereas it must not.",
                    "The expected value: different from (using operator!=)",
                    "\t[NFluent.Tests.ObjectRelatedTest+Person] of type: [NFluent.Tests.ObjectRelatedTest+Person]");
           
            Check.ThatCode(value: () => {
                    Check.That(myClone).Not.HasDifferentValueThan(new Person() {Name = "test"});
                })
                .IsAFailingCheckWithMessage("",
                    "The checked value is different from the given one.",
                    "The checked value:",
                    "\t[NFluent.Tests.ObjectRelatedTest+PersonEx]", 
                    "The expected value: equals to (using operator==)",
                    "\t[NFluent.Tests.ObjectRelatedTest+Person]");
        }

        private class Person
        {
            public string Name { get; set; }
            // ReSharper disable once UnusedMember.Local
            public string Surname { get; set; }

        }
        private class PersonEx
        {
            public string Name { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Surname { get; set; }

            public static bool operator ==(PersonEx person1, Person person2)
            {
                return person1?.Name == person2?.Name;
            }
            public static bool operator !=(PersonEx person1, Person person2)
            {
                return person1?.Name != person2?.Name;
            }
            public static bool operator ==(Person person1, PersonEx person2)
            {
                return person1?.Name == person2?.Name;
            }
            public static bool operator !=(Person person1, PersonEx person2)
            {
                return person1?.Name != person2?.Name;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is PersonEx))
                    return false;
#pragma warning disable 252,253
                return (PersonEx) obj == this;
#pragma warning restore 252,253
            }

            public override int GetHashCode()
            {
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                return this.Name.GetHashCode()
                       // ReSharper disable once NonReadonlyMemberInGetHashCode
                    + this.Surname.GetHashCode();
            }
        }
    }
}
