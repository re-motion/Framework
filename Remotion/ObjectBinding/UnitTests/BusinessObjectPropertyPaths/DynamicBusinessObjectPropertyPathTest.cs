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
using Remotion.ObjectBinding.BusinessObjectPropertyPaths;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;
using Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths
{
  [TestFixture]
  public class DynamicBusinessObjectPropertyPathTest
  {
    [Test]
    public void GetIdentifier_ReturnsIdentifier ()
    {
      IBusinessObjectPropertyPath path = DynamicBusinessObjectPropertyPath.Create ("TypeTwoValue.TypeThreeValue.TypeFourValue.IntValue");
      Assert.That (path.Identifier, Is.EqualTo ("TypeTwoValue.TypeThreeValue.TypeFourValue.IntValue"));
    }

    [Test]
    public void GetIsDynamic_ReturnsTrue ()
    {
      IBusinessObjectPropertyPath path = DynamicBusinessObjectPropertyPath.Create ("TypeTwoValue.TypeThreeValue.TypeFourValue.IntValue");
      Assert.That (path.IsDynamic, Is.True);
    }

    [Test]
    public void GetProperties_ThrowsNotSupportedException ()
    {
      IBusinessObjectPropertyPath path = DynamicBusinessObjectPropertyPath.Create ("TypeTwoValue.TypeThreeValue.TypeFourValue.IntValue");
      Assert.That (
          () => path.Properties,
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo ("Properties collection cannot be retrieved for dynamic property paths."));
    }

    [Test]
    public void GetResult_ValidPropertyPath_EndsWithInt ()
    {
      var root = TypeOne.Create();
      IBusinessObjectPropertyPath path = DynamicBusinessObjectPropertyPath.Create ("TypeTwoValue.TypeThreeValue.TypeFourValue.IntValue");

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
      var path = DynamicBusinessObjectPropertyPath.Create ("TypeTwoValue.TypeThreeValue.TypeFourValue");

      var result = path.GetResult (
          (IBusinessObject) root,
          BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties);


      Assert.That (result, Is.InstanceOf<EvaluatedBusinessObjectPropertyPathResult>());
      Assert.That (result.ResultObject, Is.SameAs (root.TypeTwoValue.TypeThreeValue));
      Assert.That (result.ResultProperty.Identifier, Is.EqualTo ("TypeFourValue"));
    }

    [Test]
    public void GetResult_InvalidPropertyPath_PropertyNotFound_ReturnsNullPath ()
    {
      var root = TypeOne.Create();
      var path = DynamicBusinessObjectPropertyPath.Create ("TypeTwoValue.TypeThreeValue.TypeFourValue1.IntValue");

      var result = path.GetResult (
          (IBusinessObject) root,
          BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties);


      Assert.That (result, Is.InstanceOf<NullBusinessObjectPropertyPathResult>());
    }

    [Test]
    public void GetResult_InvalidPropertyPath_NonLastPropertyNotReferenceProperty_ReturnsNullPath ()
    {
      var root = TypeOne.Create();
      var path = DynamicBusinessObjectPropertyPath.Create ("TypeTwoValue.IntValue.TypeFourValue");

      var result = path.GetResult (
          (IBusinessObject) root,
          BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties);


      Assert.That (result, Is.InstanceOf<NullBusinessObjectPropertyPathResult>());
    }
  }
}