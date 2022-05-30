// // This file is part of the re-motion Core Framework (www.re-motion.org)
// // Copyright (c) rubicon IT GmbH, www.rubicon.eu
// //
// // The re-motion Core Framework is free software; you can redistribute it
// // and/or modify it under the terms of the GNU Lesser General Public License
// // as published by the Free Software Foundation; either version 2.1 of the
// // License, or (at your option) any later version.
// //
// // re-motion is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU Lesser General Public License for more details.
// //
// // You should have received a copy of the GNU Lesser General Public License
// // along with re-motion; if not, see http://www.gnu.org/licenses.
// //
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class NullableLengthConstrainedPropertyAttributeTest
  {
    private class TestableNullableLengthConstrainedPropertyAttribute : NullableLengthConstrainedPropertyAttribute
    {
    }

    private TestableNullableLengthConstrainedPropertyAttribute _attribute;
    private ILengthConstrainedPropertyAttribute _lengthAttribute;

    [SetUp]
    public void SetUp ()
    {
      _attribute = new TestableNullableLengthConstrainedPropertyAttribute();
      _lengthAttribute = _attribute;
    }

    [Test]
    public void GetInterfaceMaximumLength_FromDefault ()
    {
      Assert.That(_lengthAttribute.MaximumLength, Is.Null);
    }

    [Test]
    public void GetInterfaceMaximumLength_FromExplicitValue ()
    {
      _attribute.MaximumLength = 42;

      Assert.That(_lengthAttribute.MaximumLength, Is.EqualTo(42));
    }

    [Test]
    [Obsolete]
    public void GetPublicMaximumLength_FromDefault ()
    {
      Assert.That(
          () => _attribute.MaximumLength,
          Throws.InvalidOperationException.With.Message.EqualTo("The MaximumLength property is null. Use HasMaximumLength to check the property before accessing its value."));
    }

    [Test]
    [Obsolete]
    public void GetPublicMaximumLength_FromExplicitValue ()
    {
      _attribute.MaximumLength = 42;

      Assert.That(_attribute.MaximumLength, Is.EqualTo(42));
    }

    [Test]
    public void GetHasMaximumLength_FromDefault ()
    {
      Assert.That(_attribute.HasMaximumLength, Is.False);
    }

    [Test]
    public void GetHasMaximumLength_FromExplicitValue ()
    {
      _attribute.MaximumLength = 43;

      Assert.That(_attribute.HasMaximumLength, Is.True);
    }
  }
}
