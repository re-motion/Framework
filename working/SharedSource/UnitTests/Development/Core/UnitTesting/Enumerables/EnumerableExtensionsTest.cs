// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.FunctionalProgramming;

// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTests.Core.UnitTesting.Enumerables
{
  [TestFixture]
  public class EnumerableExtensionsTest
  {
    [Test]
    public void AsOneTime ()
    {
      var source = new[] { 1, 2, 3 };

      OneTimeEnumerable<int> result = source.AsOneTime();

      Assert.That (result, Is.EqualTo (source));
    }

    [Test]
    public void ForceEnumeration ()
    {
      var wasCalled = false;
      var source = new[] { 7 }.ApplySideEffect (x => wasCalled = true);

      Assert.That (wasCalled, Is.False);
      var result = source.ForceEnumeration();
      Assert.That (wasCalled, Is.True);

      Assert.That (result, Is.EqualTo (source));
    }
  }
}