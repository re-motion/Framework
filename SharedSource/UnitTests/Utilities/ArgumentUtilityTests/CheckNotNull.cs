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
using Remotion.Utilities;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotNull
  {
    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Nullable_Fail ()
    {
      ArgumentUtility.CheckNotNull ("arg", (int?) null);
    }

    [Test]
    public void Nullable_Succeed ()
    {
      int? result = ArgumentUtility.CheckNotNull ("arg", (int?) 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void Value_Succeed ()
    {
      int result = ArgumentUtility.CheckNotNull ("arg", (int) 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Reference_Fail ()
    {
      ArgumentUtility.CheckNotNull ("arg", (string) null);
    }

    [Test]
    public void Reference_Succeed ()
    {
      string result = ArgumentUtility.CheckNotNull ("arg", string.Empty);
      Assert.That (result, Is.SameAs (string.Empty));
    }
  }
}