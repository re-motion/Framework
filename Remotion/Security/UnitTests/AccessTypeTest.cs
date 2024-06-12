// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests
{

  [TestFixture]
  public class AccessTypeTest
  {
    [Test]
    public void GetAccessTypeFromEnum ()
    {
      AccessType accessType = AccessType.Get(EnumWrapper.Get(TestAccessTypes.First));

      Assert.That(accessType.Value, Is.EqualTo(EnumWrapper.Get(TestAccessTypes.First)));
    }

    [Test]
    public void GetAccessTypeFromEnumWithoutAccessTypeAttribute ()
    {
      Assert.That(
          () => AccessType.Get(TestAccessTypesWithoutAccessTypeAttribute.First),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Enumerated type 'Remotion.Security.UnitTests.SampleDomain.TestAccessTypesWithoutAccessTypeAttribute' cannot be used as an access type. "
                  + "Valid access types must have the Remotion.Security.AccessTypeAttribute applied.", "accessType"));
    }

    [Test]
    public void GetFromCache ()
    {
      Assert.That(AccessType.Get(TestAccessTypes.First), Is.EqualTo(AccessType.Get(TestAccessTypes.First)));
      Assert.That(AccessType.Get(TestAccessTypes.Second), Is.EqualTo(AccessType.Get(EnumWrapper.Get(TestAccessTypes.Second))));
      Assert.That(AccessType.Get(EnumWrapper.Get(TestAccessTypes.Third)), Is.EqualTo(AccessType.Get(EnumWrapper.Get(TestAccessTypes.Third))));
    }

    [Test]
    public void Test_ToString ()
    {
      EnumWrapper wrapper = EnumWrapper.Get(TestAccessTypes.First);
      AccessType accessType = AccessType.Get(wrapper);

      Assert.That(accessType.ToString(), Is.EqualTo(wrapper.ToString()));
    }

    [Test]
    public void Equatable_Equals_True ()
    {
      Assert.That(AccessType.Get(EnumWrapper.Get(TestAccessTypes.Second)).Equals(AccessType.Get(TestAccessTypes.Second)), Is.True);
    }

    [Test]
    public void Equatable_Equals_False ()
    {
      Assert.That(AccessType.Get(EnumWrapper.Get(TestAccessTypes.Second)).Equals(AccessType.Get(TestAccessTypes.Fourth)), Is.False);
    }

    [Test]
    public void Equals_True ()
    {
      Assert.That(AccessType.Get(EnumWrapper.Get(TestAccessTypes.Second)).Equals((object)AccessType.Get(TestAccessTypes.Second)), Is.True);
    }

    [Test]
    public void Equals_False ()
    {
      Assert.That(AccessType.Get(EnumWrapper.Get(TestAccessTypes.Second)).Equals((object)AccessType.Get(TestAccessTypes.Fourth)), Is.False);
    }

    [Test]
    public void Equals_False_WithDifferentType ()
    {
      Assert.That(AccessType.Get(EnumWrapper.Get(TestAccessTypes.Second)).Equals(EnumWrapper.Get(TestAccessTypes.Second)), Is.False);
    }

    [Test]
    public void Equals_False_WithNull ()
    {
      Assert.That(AccessType.Get(EnumWrapper.Get(TestAccessTypes.Second)).Equals(null), Is.False);
    }

    [Test]
    public void GetHashCode_IsSameForEqualValues ()
    {
      Assert.That(AccessType.Get(TestAccessTypes.Second).GetHashCode(), Is.EqualTo(AccessType.Get(EnumWrapper.Get(TestAccessTypes.Second)).GetHashCode()));
    }
  }
}
