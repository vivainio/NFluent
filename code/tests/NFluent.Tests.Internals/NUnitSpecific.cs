﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NUnitSpecific.cs" company="">
//   Copyright 2017 Cyrille DUPUYDAUBY
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//         http://www.apache.org/licenses/LICENSE-2.0
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NFluent.Tests
{
    using Helpers;
    using NUnit.Framework;

    public class NUnitSpecific
    {
#if !NETCOREAPP1_1
        [Test]
        public void ExceptionScanTest()
        {
            Check.That(ExceptionHelper.BuildException("Test")).IsInstanceOf<AssertionException>();
            Check.That(ExceptionHelper.BuildInconclusiveException("Test")).IsInstanceOf<InconclusiveException>();
        }
#endif
    }
}
