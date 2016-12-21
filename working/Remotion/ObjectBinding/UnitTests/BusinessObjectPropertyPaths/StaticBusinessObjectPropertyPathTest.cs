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
using Remotion.ObjectBinding.BusinessObjectPropertyPaths;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;
using Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths
{
  [TestFixture]
  public class StaticBusinessObjectPropertyPathTest
  {
    [Test]
    public void GetIdentifier_ReturnsIdentifier ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (TypeOne));
      var typeOneClass = provider.GetBindableObjectClass (typeof (TypeOne));
      IBusinessObjectPropertyPath path = StaticBusinessObjectPropertyPath.Parse ("TypeTwoValue.TypeThreeValue.TypeFourValue.IntValue", typeOneClass);

      Assert.That (path.Identifier, Is.EqualTo ("TypeTwoValue.TypeThreeValue.TypeFourValue.IntValue"));
    }

    [Test]
    public void GetIsDynamic_ReturnsFalse ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (TypeOne));
      var typeOneClass = provider.GetBindableObjectClass (typeof (TypeOne));
      IBusinessObjectPropertyPath path = StaticBusinessObjectPropertyPath.Parse ("TypeTwoValue", typeOneClass);

      Assert.That (path.IsDynamic, Is.False);
    }

    [Test]
    public void GetProperties_ThrowsNotSupportedException ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (TypeOne));
      var typeOneClass = provider.GetBindableObjectClass (typeof (TypeOne));

      var expectedProperties = new[]
                               {
                                   typeOneClass.GetPropertyDefinition ("TypeTwoValue"),
                                   provider.GetBindableObjectClass (typeof (TypeTwo)).GetPropertyDefinition ("TypeThreeValue"),
                                   provider.GetBindableObjectClass (typeof (TypeThree)).GetPropertyDefinition ("TypeFourValue"),
                                   provider.GetBindableObjectClass (typeof (TypeFour)).GetPropertyDefinition ("IntValue"),
                               };
      IBusinessObjectPropertyPath path = StaticBusinessObjectPropertyPath.Parse ("TypeTwoValue.TypeThreeValue.TypeFourValue.IntValue", typeOneClass);

      Assert.That (() => path.Properties, Is.EqualTo (expectedProperties));
    }

    [Test]
    public void GetResult_ValidPropertyPath_EndsWithInt ()
    {
      var root = TypeOne.Create();
      IBusinessObjectPropertyPath path = StaticBusinessObjectPropertyPath.Parse (
          "TypeTwoValue.TypeThreeValue.TypeFourValue.IntValue",
          ((IBusinessObject) root).BusinessObjectClass);

      var result = path.GetResult (
          (IBusinessObject) root,
          BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties);


      Assert.That (result, Is.InstanceOf<EvaluatedBusinessObjectPropertyPathResult>());
      Assert.That (result.ResultObject, Is.SameAs (root.TypeTwoValue.TypeThreeValue.TypeFourValue));
      Assert.That (
          result.ResultProperty,
          Is.SameAs (((IBusinessObject) root.TypeTwoValue.TypeThreeValue.TypeFourValue).BusinessObjectClass.GetPropertyDefinition ("IntValue")));
    }

    [Test]
    public void GetResult_ValidPropertyPath_EndsWithReferenceProperty ()
    {
      var root = TypeOne.Create();
      var path = StaticBusinessObjectPropertyPath.Parse (
          "TypeTwoValue.TypeThreeValue.TypeFourValue",
          ((IBusinessObject) root).BusinessObjectClass);

      var result = path.GetResult (
          (IBusinessObject) root,
          BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties);


      Assert.That (result, Is.InstanceOf<EvaluatedBusinessObjectPropertyPathResult>());
      Assert.That (result.ResultObject, Is.SameAs (root.TypeTwoValue.TypeThreeValue));
      Assert.That (result.ResultProperty.Identifier, Is.EqualTo ("TypeFourValue"));
    }

    [Test]
    public void Parse_InvalidPropertyPath_PropertyNotFound_ThrowsParseException ()
    {
      var root = TypeOne.Create();
      Assert.That (
          () => StaticBusinessObjectPropertyPath.Parse ("TypeTwoValue.TypeThreeValue.TypeFourValue1", ((IBusinessObject) root).BusinessObjectClass),
          Throws.TypeOf<ParseException>());
    }

    [Test]
    public void Parse_InvalidPropertyPath_NonLastPropertyNotReferenceProperty_ThrowsParseException ()
    {
      var root = TypeOne.Create();
      Assert.That (
          () => StaticBusinessObjectPropertyPath.Parse ("TypeTwoValue.IntValue.TypeFourValue", ((IBusinessObject) root).BusinessObjectClass),
          Throws.TypeOf<ParseException>());
    }

    [Test]
    public void Create_FromProperties ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (TypeOne));

      var properties = new[]
                       {
                           provider.GetBindableObjectClass (typeof (TypeOne)).GetPropertyDefinition ("TypeTwoValue"),
                           provider.GetBindableObjectClass (typeof (TypeTwo)).GetPropertyDefinition ("TypeThreeValue"),
                           provider.GetBindableObjectClass (typeof (TypeThree)).GetPropertyDefinition ("TypeFourValue"),
                           provider.GetBindableObjectClass (typeof (TypeFour)).GetPropertyDefinition ("IntValue"),
                       };

      var path = StaticBusinessObjectPropertyPath.Create (properties);

      Assert.That (path.Properties, Is.EqualTo (properties));
      Assert.That (path.Identifier, Is.EqualTo ("TypeTwoValue.TypeThreeValue.TypeFourValue.IntValue"));
    }

    [Test]
    public void Create_FromSingleProperty ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (TypeOne));

      var properties = new[] { provider.GetBindableObjectClass (typeof (TypeOne)).GetPropertyDefinition ("TypeTwoValue") };

      var path = StaticBusinessObjectPropertyPath.Create (properties);

      Assert.That (path.Properties, Is.EqualTo (properties));
      Assert.That (path.Identifier, Is.EqualTo ("TypeTwoValue"));
    }

    [Test]
    public void Create_ContainsNonReferenceProperty_ThrowsArgumentException ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (TypeOne));

      var properties = new[]
                       {
                           provider.GetBindableObjectClass (typeof (TypeOne)).GetPropertyDefinition ("TypeTwoValue"),
                           provider.GetBindableObjectClass (typeof (TypeTwo)).GetPropertyDefinition ("IntValue"),
                           provider.GetBindableObjectClass (typeof (TypeThree)).GetPropertyDefinition ("TypeFourValue"),
                           provider.GetBindableObjectClass (typeof (TypeFour)).GetPropertyDefinition ("IntValue"),
                       };

      Assert.That (
          () => StaticBusinessObjectPropertyPath.Create (properties),
          Throws.ArgumentException.With.Message.StartsWith (
              "Property #1 ('IntValue') is not of type IBusinessObjectReferenceProperty. Every property except the last property must be a reference property."));
    }

    [Test]
    public void Create_NextPropertyNotPartOfClass_ThrowsArgumentException ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (TypeOne));

      var properties = new[]
                       {
                           provider.GetBindableObjectClass (typeof (TypeOne)).GetPropertyDefinition ("TypeTwoValue"),
                           provider.GetBindableObjectClass (typeof (TypeThree)).GetPropertyDefinition ("TypeFourValue"),
                           provider.GetBindableObjectClass (typeof (TypeFour)).GetPropertyDefinition ("IntValue"),
                       };

      Assert.That (
          () => StaticBusinessObjectPropertyPath.Create (properties),
          Throws.ArgumentException.With.Message.StartsWith (
              "Property #1 ('TypeFourValue') is not part of the previous business object class "
              + "'Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.TestDomain.TypeTwo, Remotion.ObjectBinding.UnitTests'."
              + " The property path must form a continuous chain."));
    }
  }
}