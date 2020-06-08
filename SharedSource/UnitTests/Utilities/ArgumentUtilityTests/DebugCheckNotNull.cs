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

#nullable disable
// ReSharper disable once CheckNamespace

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
#if !DEBUG
  [Ignore ("Skipped unless DEBUG build")]
#endif
  [TestFixture]
  public class DebugCheckNotNull
  {
    [Test]
    public void Nullable_Fail ()
    {
      Assert.That (
          () => ArgumentUtility.DebugCheckNotNull ("arg", (int?) null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Nullable_Succeed ()
    {
      ArgumentUtility.DebugCheckNotNull ("arg", (int?) 1);
    }

    [Test]
    public void Value_Succeed ()
    {
      ArgumentUtility.DebugCheckNotNull ("arg", (int) 1);
    }

    [Test]
    public void Reference_Fail ()
    {
      Assert.That (
          () => ArgumentUtility.DebugCheckNotNull ("arg", (string) null),
          Throws.InstanceOf<ArgumentNullException>());
    }
  }
}