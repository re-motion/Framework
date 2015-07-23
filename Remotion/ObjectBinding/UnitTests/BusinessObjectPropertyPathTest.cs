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
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BusinessObjectPropertyPathTest
  {
    [Test]
    [Obsolete]
    public void GetValue_True_True ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultStub = MockRepository.GenerateStub<IBusinessObjectPropertyPathResult>();
      resultStub.Stub (_ => _.GetValue()).Return (100);

      var pathStub = MockRepository.GenerateStub<IBusinessObjectPropertyPath>();
      pathStub.Stub (
          _ => _.GetResult (
              businessObjectStub,
              BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry))
              .Return (resultStub);

      var actual = pathStub.GetValue (businessObjectStub, true, true);
      Assert.That (actual, Is.EqualTo (100));
    }

    [Test]
    [Obsolete]
    public void GetValue_True_False ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultStub = MockRepository.GenerateStub<IBusinessObjectPropertyPathResult>();
      resultStub.Stub (_ => _.GetValue()).Return (100);

      var pathStub = MockRepository.GenerateStub<IBusinessObjectPropertyPath>();
      pathStub.Stub (
          _ => _.GetResult (
              businessObjectStub,
              BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties))
              .Return (resultStub);

      var actual = pathStub.GetValue (businessObjectStub, true, false);
      Assert.That (actual, Is.EqualTo (100));
    }

    [Test]
    [Obsolete]
    public void GetValue_False_True ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultStub = MockRepository.GenerateStub<IBusinessObjectPropertyPathResult>();
      resultStub.Stub (_ => _.GetValue()).Return (100);

      var pathStub = MockRepository.GenerateStub<IBusinessObjectPropertyPath>();
      pathStub.Stub (
          _ => _.GetResult (
              businessObjectStub,
              BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry))
              .Return (resultStub);

      var actual = pathStub.GetValue (businessObjectStub, false, true);
      Assert.That (actual, Is.EqualTo (100));
    }

    [Test]
    [Obsolete]
    public void GetValue_False_False ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultStub = MockRepository.GenerateStub<IBusinessObjectPropertyPathResult>();
      resultStub.Stub (_ => _.GetValue()).Return (100);

      var pathStub = MockRepository.GenerateStub<IBusinessObjectPropertyPath>();
      pathStub.Stub (
          _ => _.GetResult (
              businessObjectStub,
              BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties))
              .Return (resultStub);

      var actual = pathStub.GetValue (businessObjectStub, false, false);
      Assert.That (actual, Is.EqualTo (100));
    }

    [Test]
    [Obsolete]
    public void GetString ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultStub = MockRepository.GenerateStub<IBusinessObjectPropertyPathResult>();
      resultStub.Stub (_ => _.GetString ("format")).Return ("value");

      var pathStub = MockRepository.GenerateStub<IBusinessObjectPropertyPath>();
      pathStub.Stub (
          _ => _.GetResult (
              businessObjectStub,
              BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry))
              .Return (resultStub);

      var actual = pathStub.GetString (businessObjectStub, "format");
      Assert.That (actual, Is.EqualTo ("value"));
    }

    [Test]
    [Obsolete]
    public void GetBusinessObject_True_True ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultStub = MockRepository.GenerateStub<IBusinessObjectPropertyPathResult>();
      resultStub.Stub (_ => _.ResultObject).Return (resultObjectStub);

      var pathStub = MockRepository.GenerateStub<IBusinessObjectPropertyPath>();
      pathStub.Stub (
          _ => _.GetResult (
              businessObjectStub,
              BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry))
              .Return (resultStub);

      var actual = pathStub.GetBusinessObject (businessObjectStub, true, true);
      Assert.That (actual, Is.SameAs (resultObjectStub));
    }

    [Test]
    [Obsolete]
    public void GetBusinessObject_True_False ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultStub = MockRepository.GenerateStub<IBusinessObjectPropertyPathResult>();
      resultStub.Stub (_ => _.ResultObject).Return (resultObjectStub);

      var pathStub = MockRepository.GenerateStub<IBusinessObjectPropertyPath>();
      pathStub.Stub (
          _ => _.GetResult (
              businessObjectStub,
              BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties))
              .Return (resultStub);

      var actual = pathStub.GetBusinessObject (businessObjectStub, true, false);
      Assert.That (actual, Is.SameAs (resultObjectStub));
    }

    [Test]
    [Obsolete]
    public void GetBusinessObject_False_True ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultStub = MockRepository.GenerateStub<IBusinessObjectPropertyPathResult>();
      resultStub.Stub (_ => _.ResultObject).Return (resultObjectStub);

      var pathStub = MockRepository.GenerateStub<IBusinessObjectPropertyPath>();
      pathStub.Stub (
          _ => _.GetResult (
              businessObjectStub,
              BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry))
              .Return (resultStub);

      var actual = pathStub.GetBusinessObject (businessObjectStub, false, true);
      Assert.That (actual, Is.SameAs (resultObjectStub));
    }

    [Test]
    [Obsolete]
    public void GetBusinessObject_False_False ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var resultStub = MockRepository.GenerateStub<IBusinessObjectPropertyPathResult>();
      resultStub.Stub (_ => _.ResultObject).Return (resultObjectStub);

      var pathStub = MockRepository.GenerateStub<IBusinessObjectPropertyPath>();
      pathStub.Stub (
          _ => _.GetResult (
              businessObjectStub,
              BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties))
              .Return (resultStub);

      var actual = pathStub.GetBusinessObject (businessObjectStub, false, false);
      Assert.That (actual, Is.SameAs (resultObjectStub));
    }

    [Test]
    [Obsolete]
    public void BusinessObjectProvider_CreatePropertyPath ()
    {
      IBusinessObjectProvider provider = null;
      var propertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();
      propertyStub.Stub (_ => _.ReflectedClass).Return (MockRepository.GenerateStub<IBusinessObjectClass>());
      propertyStub.ReflectedClass.Stub (_ => _.GetPropertyDefinition (Arg<string>.Is.Anything)).Return (propertyStub);
      IBusinessObjectProperty[] properties = new[] { propertyStub };

      var path = provider.CreatePropertyPath (properties);

      Assert.That (path, Is.InstanceOf<StaticBusinessObjectPropertyPath>());
      Assert.That (path.Properties, Is.EqualTo (properties));
    }
  }
}