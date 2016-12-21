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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.PropertyReflectorTests
{
  [TestFixture]
  public class Extensibility : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void GetMetadata_AddNewPropertyTypeFromMixin ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");

      using (MixinConfiguration.BuildNew().ForClass (typeof (PropertyReflector)).AddMixin<SimpleReferenceTypePropertyReflectorMixin>().EnterScope())
      {
        PropertyReflector propertyReflector = PropertyReflector.Create (propertyInfo, _businessObjectProvider);

        IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

        Assert.That (businessObjectProperty, Is.TypeOf (typeof (SimpleReferenceTypeProperty)));
        Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Scalar"));
      }
    }

    [Test]
    public void GetMetadata_SupportsPredifinedPropertyTypes ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithAllDataTypes), "Int32");

      using (MixinConfiguration.BuildNew ().ForClass (typeof (PropertyReflector)).AddMixin<SimpleReferenceTypePropertyReflectorMixin> ().EnterScope ())
      {
        PropertyReflector propertyReflector = PropertyReflector.Create (propertyInfo, _businessObjectProvider);

        IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

        Assert.That (businessObjectProperty, Is.TypeOf (typeof (Int32Property)));
        Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Int32"));
      }
    }

    [Test]
    public void GetMetadata_DefaultsToBaseImplementation ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "Scalar");

      using (MixinConfiguration.BuildNew ().ForClass (typeof (PropertyReflector)).AddMixin<SimpleReferenceTypePropertyReflectorMixin> ().EnterScope ())
      {
        PropertyReflector propertyReflector = PropertyReflector.Create (propertyInfo, _businessObjectProvider);

        IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

        Assert.That (businessObjectProperty, Is.TypeOf (typeof (NotSupportedProperty)));
        Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Scalar"));
      }
    }
  }
}
