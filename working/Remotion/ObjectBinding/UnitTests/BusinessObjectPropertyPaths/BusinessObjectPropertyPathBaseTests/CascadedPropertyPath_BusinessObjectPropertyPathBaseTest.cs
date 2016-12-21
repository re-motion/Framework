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

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.BusinessObjectPropertyPathBaseTests
{
  [TestFixture]
  public class CascadedPropertyPath_BusinessObjectPropertyPathBaseTest
  {
    private BusinessObjectPropertyPathTestHelper _testHelper;
    private BusinessObjectPropertyPathBase _path;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new BusinessObjectPropertyPathTestHelper();
      _path = new TestableBusinessObjectPropertyPathBase (_testHelper.ReferenceProperty, _testHelper.Property);
    }

    [Test]
    public void GetResult ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnReferencePropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (_testHelper.BusinessObjectWithIdentity);
      }
      _testHelper.ReplayAll();

      var actual = _path.GetResult (
          _testHelper.BusinessObject,
          BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);

      _testHelper.VerifyAll();

      Assert.That (actual, Is.InstanceOf<EvaluatedBusinessObjectPropertyPathResult>());
      Assert.That (actual.ResultObject, Is.SameAs (_testHelper.BusinessObjectWithIdentity));
      Assert.That (actual.ResultProperty, Is.SameAs (_testHelper.Property));
    }

    [Test]
    public void GetResult_WithUnreachableObject_ReturnsNull ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnReferencePropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (null);
      }
      _testHelper.ReplayAll();

      var actual = _path.GetResult (
          _testHelper.BusinessObject,
          BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);

      _testHelper.VerifyAll();

      Assert.That (actual, Is.InstanceOf<NullBusinessObjectPropertyPathResult>());
    }

    [Test]
    public void GetResult_WithUnreachableObject_ThrowsInvalidOperationException ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnReferencePropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectGetProperty (null);
      }
      _testHelper.ReplayAll();

      Assert.That (
          () =>
          _path.GetResult (
              _testHelper.BusinessObject,
              BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry),
          Throws.InvalidOperationException.With.Message
                .EqualTo ("A null value was returned for property #0 of property path 'Identifier'. Cannot evaluate rest of path."));
    }

    [Test]
    public void GetResult_WithAccessDenied_ReturnsNull ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnReferencePropertyIsAccessible (false);
      }
      _testHelper.ReplayAll();

      var actual = _path.GetResult (
          _testHelper.BusinessObject,
          BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);

      _testHelper.VerifyAll();
      Assert.That (actual, Is.InstanceOf<NotAccessibleBusinessObjectPropertyPathResult>());
    }

    [Test]
    public void GetResult_WithAccessDenied_ThrowsInvalidOperationException ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnReferencePropertyIsAccessible (false);
      }
      _testHelper.ReplayAll();

      Assert.That (
          () =>
          _path.GetResult (
              _testHelper.BusinessObject,
              BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry),
          Throws.InvalidOperationException.With.Message
                .EqualTo ("Access was denied to property #0 of property path 'Identifier'. Cannot evaluate rest of path."));
    }

    [Test]
    public void GetResult_WithBusinessObjectPropertyAccessException_ReturnsNull ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnReferencePropertyIsAccessible (true);
        ExpectThrowBusinessObjectPropertyAccessExceptionOnBusinessObjectGetProperty();
      }
      _testHelper.ReplayAll();

      var actual = _path.GetResult (
          _testHelper.BusinessObject,
          BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);

      _testHelper.VerifyAll();
      Assert.That (actual, Is.InstanceOf<NotAccessibleBusinessObjectPropertyPathResult>());
    }

    [Test]
    public void GetResult_WithBusinessObjectPropertyAccessException_ThrowsInvalidOperationException ()
    {
      using (_testHelper.Ordered())
      {
        ExpectOnceOnReferencePropertyIsAccessible (true);
        ExpectThrowBusinessObjectPropertyAccessExceptionOnBusinessObjectGetProperty();
      }
      _testHelper.ReplayAll();

      Assert.That (
          () =>
          _path.GetResult (
              _testHelper.BusinessObject,
              BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue,
              BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry),
          Throws.InvalidOperationException.With.Message
                .EqualTo ("Access was denied to property #0 of property path 'Identifier'. Cannot evaluate rest of path."));
    }

    [Test]
    public void Test_ToString ()
    {
      Assert.That (_path.ToString(), Is.EqualTo (_path.Identifier));
    }

    private void ExpectOnceOnReferencePropertyIsAccessible (bool returnValue)
    {
      _testHelper.ExpectOnceOnIsAccessible (
          _testHelper.BusinessObjectClass,
          _testHelper.BusinessObject,
          _testHelper.ReferenceProperty, returnValue);
    }

    private void ExpectOnceOnBusinessObjectGetProperty (IBusinessObjectWithIdentity businessObejctWithIdentity)
    {
      _testHelper.ExpectOnceOnGetProperty (_testHelper.BusinessObject, _testHelper.ReferenceProperty, businessObejctWithIdentity);
    }

    private void ExpectThrowBusinessObjectPropertyAccessExceptionOnBusinessObjectGetProperty ()
    {
      _testHelper.ExpectThrowOnGetProperty (
          _testHelper.BusinessObject,
          _testHelper.ReferenceProperty,
          new BusinessObjectPropertyAccessException ("The Message", null));
    }
  }
}