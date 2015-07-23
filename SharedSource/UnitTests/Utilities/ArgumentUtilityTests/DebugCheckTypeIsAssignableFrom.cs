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
#if !DEBUG
  [Ignore ("Skipped unless DEBUG build")]
#endif
  [TestFixture]
  public class DebugCheckTypeIsAssignableFrom
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' is a 'System.Object', which cannot be assigned to type 'System.String'.\r\nParameter name: arg")]
    public void Fail ()
    {
      ArgumentUtility.DebugCheckTypeIsAssignableFrom ("arg", typeof (object), typeof (string));
    }

    [Test]
    public void Succeed_Null ()
    {
      ArgumentUtility.DebugCheckTypeIsAssignableFrom ("arg", null, typeof (object));
    }

    [Test]
    public void Succeed ()
    {
      ArgumentUtility.DebugCheckTypeIsAssignableFrom ("arg", typeof (string), typeof (object));
    }
  }
}