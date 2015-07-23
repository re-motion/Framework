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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class NullablePropertyAttributeTest
  {
    private class StubNullablePropertyAttribute: NullablePropertyAttribute
    {
      public StubNullablePropertyAttribute()
      {
      }
    }

    private StubNullablePropertyAttribute _attribute;
    private INullablePropertyAttribute _nullable;

    [SetUp]
    public void SetUp()
    {
      _attribute = new StubNullablePropertyAttribute();
      _nullable = _attribute;
    }

    [Test]
    public void GetNullable_FromDefault()
    {
      Assert.That (_attribute.IsNullable, Is.True);
      Assert.That (_nullable.IsNullable, Is.True);
    }

    [Test]
    public void GetNullable_FromExplicitValue()
    {
      _attribute.IsNullable = false;
      Assert.That (_attribute.IsNullable, Is.False);
      Assert.That (_nullable.IsNullable, Is.False);
    }
  }
}
