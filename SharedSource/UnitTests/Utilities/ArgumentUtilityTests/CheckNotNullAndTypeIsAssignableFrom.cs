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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;

#nullable disable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotNullAndTypeIsAssignableFrom
  {
    [Test]
    public void Fail_Null ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("arg", null, typeof(string)),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Fail_Type ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("arg", typeof(object), typeof(string)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' is a 'System.Object', which cannot be assigned to type 'System.String'.", "arg"));
    }

    [Test]
    public void Succeed ()
    {
      Type result = ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("arg", typeof(string), typeof(object));
      Assert.That(result, Is.SameAs(typeof(string)));
    }
  }
}
