﻿// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="CharCheckExtensions.cs" company="">
// //   Copyright 2013 Thomas PIERRAIN
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
namespace NFluent
{
    using NFluent.Extensions;
    using NFluent.Helpers;

    /// <summary>
    /// Provides check methods to be executed on a char value.
    /// </summary>
    public static class CharCheckExtensions
    {
        /// <summary>
        /// Checks that the checked <see cref="char"/> is a letter.
        /// </summary>
        /// <param name="check">The chained fluent check.</param>
        /// <exception cref="FluentCheckException">The checked <see cref="char"/> is not a letter.</exception>
        /// <returns>A check link.</returns>
        public static ICheckLink<ICheck<char>> IsALetter(this ICheck<char> check)
        {
            var checkRunner = check as ICheckRunner<char>;
            var runnableCheck = check as IRunnableCheck<char>;

            return checkRunner.ExecuteCheck(
                () =>
                {
                    char checkedChar = runnableCheck.Value;
                    if (!IsALetter(checkedChar))
                    {
                        var errorMessage = FluentMessage.BuildMessage("The {0} is not a letter.").For("char").On(checkedChar).ToString();
                        throw new FluentCheckException(errorMessage);
                    }
                },
                FluentMessage.BuildMessage("The {0} is a letter whereas it must not.").For("char").On(runnableCheck.Value).ToString());
        }

        /// <summary>
        /// Checks that the checked <see cref="char"/> and the given one are the same letter, whatever the case.
        /// </summary>
        /// <param name="check">The chained fluent check.</param>
        /// <param name="otherChar">The other char that.</param>
        /// <exception cref="FluentCheckException">The checked <see cref="char"/> is not the same letter as the expected one, whatever the case.</exception>
        /// <returns>A check link.</returns>
        public static ICheckLink<ICheck<char>> IsSameLetter(this ICheck<char> check, char otherChar)
        {
            var checkRunner = check as ICheckRunner<char>;
            var runnableCheck = check as IRunnableCheck<char>;

            return checkRunner.ExecuteCheck(
                () =>
                {
                    char checkedChar = runnableCheck.Value;
                    
                    if (!IsALetter(checkedChar))
                    {
                        var errorMessage = FluentMessage.BuildMessage("The {0} is not the same letter as the given one (whatever the case).\nThe checked char is not even a letter!").For("char").On(checkedChar).And.WithGivenValue(otherChar).ToString();
                        throw new FluentCheckException(errorMessage);
                    }

                    if (!IsSameCharCaseInsensitive(checkedChar, otherChar))
                    {
                        var errorMessage = FluentMessage.BuildMessage("The {0} is not the same letter as the given one (whatever the case).").For("char").On(checkedChar).And.WithGivenValue(otherChar).ToString();
                        throw new FluentCheckException(errorMessage);
                    }
                },
                FluentMessage.BuildMessage("The {0} is the same letter as the given one (whatever the case), whereas it must not.").For("char").On(runnableCheck.Value).And.WithGivenValue(otherChar).ToString());
        }

        /// <summary>
        /// Checks that the checked <see cref="char"/> is the same letter as the other, but with different case only.
        /// </summary>
        /// <param name="check">The chained fluent check.</param>
        /// <param name="otherChar">The other char that.</param>
        /// <exception cref="FluentCheckException">The checked <see cref="char"/> is not the same as the expected one, or is the same but with the same case.</exception>
        /// <returns>A check link.</returns>
        public static ICheckLink<ICheck<char>> IsSameLetterButWithDifferentCaseAs(this ICheck<char> check, char otherChar)
        {
            var checkRunner = check as ICheckRunner<char>;
            var runnableCheck = check as IRunnableCheck<char>;

            return checkRunner.ExecuteCheck(
                () =>
                    {
                        char checkedChar = runnableCheck.Value;
                        if (!IsALetter(checkedChar) || !IsSameCharCaseInsensitive(checkedChar, otherChar) || HaveSameCase(checkedChar, otherChar))
                        {
                            var errorMessage = FluentMessage.BuildMessage("The {0} is not the same letter but with different case as the given one.").For("char").On(checkedChar).And.WithGivenValue(otherChar).ToString();
                            throw new FluentCheckException(errorMessage);
                        }
                    },
                    FluentMessage.BuildMessage("The {0} is the same letter as the given one but with different case, whereas it must not.").For("char").On(runnableCheck.Value).And.WithGivenValue(otherChar).ToString());
        }

        private static bool IsALetter(char checkedChar)
        {
            return char.IsLetter(checkedChar);
        }

        private static bool IsSameCharCaseInsensitive(char checkedChar, char otherChar)
        {
            return char.ToLower(checkedChar).Equals(char.ToLower(otherChar));
        }

        private static bool HaveSameCase(char checkedChar, char otherChar)
        {
            return (!char.IsLower(checkedChar) || !char.IsUpper(otherChar)) && (!char.IsUpper(checkedChar) || !char.IsLower(otherChar));
        }
    }
}
