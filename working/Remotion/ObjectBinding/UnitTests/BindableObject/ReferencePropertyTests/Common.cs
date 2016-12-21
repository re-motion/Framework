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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class Common : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
    }

    [Test]
    public void Initialize_WithMissmatchedConcreteType_ThrowsInvalidOperationExceptionWhenConcreteTypeIsFirstUsed ()
    {
      PropertyBase.Parameters parameters = CreateParameters (
          _businessObjectProvider,
          GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), "Scalar"),
          typeof (SimpleBusinessObjectClass),
          typeof (ClassWithAllDataTypes),
          null,
          false,
          false);
      var referenceProperty = new ReferenceProperty (parameters);

      Assert.That (
          () => referenceProperty.SupportsSearchAvailableObjects,
          Throws.InvalidOperationException.With.Message.EqualTo (
              "The concrete type must be assignable to the underlying type 'Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass'.\r\n"
              + "Concrete type: Remotion.ObjectBinding.UnitTests.TestDomain.ClassWithAllDataTypes"));
    }

    [Test]
    public void Initialize_WithConcreteTypeNotImplementingIBusinessObject_ThrowsInvalidOperationExceptionWhenConcreteTypeIsFirstUsed ()
    {
      PropertyBase.Parameters parameters = CreateParameters (
          _businessObjectProvider,
          GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), "Scalar"),
          typeof (SimpleBusinessObjectClass),
          typeof (SimpleBusinessObjectClass),
          null,
          false,
          false);
      var referenceProperty = new ReferenceProperty (parameters);

      Assert.That (
          () => referenceProperty.SupportsSearchAvailableObjects,
          Throws.InvalidOperationException.With.Message.EqualTo (
              "The concrete type must implement the IBusinessObject interface.\r\n"
              + "Concrete type: Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass"));
    }
  }
}
