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
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Is = NUnit.Framework.Is;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.PropertyReflectorTests
{
  [TestFixture]
  public class ReferenceType : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private BindableObjectClass _bindableObjectClass;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();
      _bindableObjectClass = new BindableObjectClass (
          typeof (ClassWithReferenceType<SimpleReferenceType>),
          _businessObjectProvider,
          SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>(),
          new PropertyBase[0]);
    }

    [Test]
    public void GetMetadata_WithScalar ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Scalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadOnlyScalar ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "ReadOnlyScalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyAttributeScalar ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "ReadOnlyAttributeScalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyAttributeScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyNonPublicSetterScalar ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "ReadOnlyNonPublicSetterScalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyNonPublicSetterScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithArray ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Array");
      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Array"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadWriteExplicitInterfaceScalar ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), 
          typeof (IInterfaceWithReferenceType<SimpleReferenceType>), "ExplicitInterfaceScalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ExplicitInterfaceScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False); 
    }

    [Test]
    public void GetMetadata_WithReadOnlyExplicitInterfaceScalar ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), 
          typeof (IInterfaceWithReferenceType<SimpleReferenceType>), "ExplicitInterfaceReadOnlyScalar");
      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ExplicitInterfaceReadOnlyScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadWriteImplicitInterfaceScalar_FromImplementation ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>),
          typeof (IInterfaceWithReferenceType<SimpleReferenceType>), "ImplicitInterfaceScalar");
      PropertyReflector propertyReflector = PropertyReflector.Create (propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ImplicitInterfaceScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadOnlyImplicitInterfaceScalar_FromReadWriteImplementation ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>),
          typeof (IInterfaceWithReferenceType<SimpleReferenceType>), "ImplicitInterfaceReadOnlyScalar");
      PropertyReflector propertyReflector = PropertyReflector.Create (propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (SimpleReferenceType)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ImplicitInterfaceReadOnlyScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadWriteMixedProperty ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (
              MixinTypeUtility.GetConcreteMixedType (typeof (ClassWithMixedProperty)),
              typeof (IMixinAddingProperty),
              "MixedProperty");
      Assertion.IsTrue (propertyInfo is MixinIntroducedPropertyInformation);

      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (string)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("MixedProperty"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadOnlyMixedProperty ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (
          MixinTypeUtility.GetConcreteMixedType (typeof (ClassWithMixedProperty)),
          typeof (IMixinAddingProperty),
          "MixedReadOnlyProperty");
      Assertion.IsTrue (propertyInfo is MixinIntroducedPropertyInformation);

      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (string)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("MixedReadOnlyProperty"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyMixedPropertyHavingSetterOnMixin ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (
          MixinTypeUtility.GetConcreteMixedType (typeof (ClassWithMixedProperty)),
          typeof (IMixinAddingProperty),
          "MixedReadOnlyPropertyHavingSetterOnMixin");
      Assertion.IsTrue (propertyInfo is MixinIntroducedPropertyInformation);

      PropertyReflector propertyReflector = PropertyReflector.Create(propertyInfo, _businessObjectProvider);

      Assert.That (GetUnderlyingType (propertyReflector), Is.SameAs (typeof (string)));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOf (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("MixedReadOnlyPropertyHavingSetterOnMixin"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_ForSealedBusinessObject_WithExistingMixin ()
    {
      var mixinTargetType = typeof (ManualBusinessObject);
      var businessObjectType = typeof (SealedBindableObject);
      Assertion.IsTrue (mixinTargetType.IsAssignableFrom (businessObjectType));

      using (MixinConfiguration.BuildNew()
          .AddMixinToClass (
              MixinKind.Extending,
              mixinTargetType,
              typeof (MixinStub),
              MemberVisibility.Public,
              Enumerable.Empty<Type>(),
              Enumerable.Empty<Type>())
          .EnterScope())
      {
        IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SealedBindableObject>), "Scalar");

        PropertyReflector propertyReflector = PropertyReflector.Create (propertyInfo, _businessObjectProvider);

        var referenceProperty = (IBusinessObjectReferenceProperty) propertyReflector.GetMetadata();
        Assert.That (() => referenceProperty.SupportsSearchAvailableObjects, Throws.Nothing);
      }
    }

    [Test]
    public void GetMetadata_ForValueType ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<ValueTypeBindableObject>), "Scalar");

      PropertyReflector propertyReflector = PropertyReflector.Create (propertyInfo, _businessObjectProvider);

      var referenceProperty = (IBusinessObjectReferenceProperty) propertyReflector.GetMetadata();
      Assert.That (() => referenceProperty.SupportsSearchAvailableObjects, Throws.Nothing);
    }
  }
}
