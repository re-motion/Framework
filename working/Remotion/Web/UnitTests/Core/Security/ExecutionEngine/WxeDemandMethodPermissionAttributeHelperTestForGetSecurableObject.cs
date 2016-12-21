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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Core.Security.Domain;

namespace Remotion.Web.UnitTests.Core.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandMethodPermissionAttributeHelperTestForGetSecurableObject
  {
    // types

    // static members

    // member fields
    private WxeDemandTargetMethodPermissionAttribute _attribute;
    private TestFunctionWithThisObjectAsSecondParameter _functionWithThisObjectAsSecondParamter;
    private SecurableObject _securableObject;

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _attribute = new WxeDemandTargetMethodPermissionAttribute ("Read");

      object someObject = new object ();
      _securableObject = new SecurableObject (null);

      _functionWithThisObjectAsSecondParamter = new TestFunctionWithThisObjectAsSecondParameter (someObject, _securableObject);
      _functionWithThisObjectAsSecondParamter.SomeObject = someObject; // Required because in this test the WxeFunction has not started executing.
      _functionWithThisObjectAsSecondParamter.ThisObject = _securableObject; // Required because in this test the WxeFunction has not started executing.
    }

    [Test]
    public void TestWithValidParameterName ()
    {
      _attribute.ParameterName = "ThisObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          _functionWithThisObjectAsSecondParamter.GetType (),
          _attribute);

      Assert.That (helper.GetSecurableObject (_functionWithThisObjectAsSecondParamter), Is.SameAs (_functionWithThisObjectAsSecondParamter.ThisObject));
    }

    [Test]
    public void TestWithDefaultParameter ()
    {
      TestFunctionWithThisObject function = new TestFunctionWithThisObject (_securableObject, null);
      function.ThisObject = _securableObject; // Required because in this test the WxeFunction has not started executing.

      _attribute.ParameterName = null;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          function.GetType (),
          _attribute);

      Assert.That (helper.GetSecurableObject (function), Is.SameAs (function.ThisObject));
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage = "The parameter 'ThisObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
        + " WxeFunction 'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithThisObject' is null.")]
    public void TestWithParameterNull ()
    {
      _attribute.ParameterName = "ThisObject";
      TestFunctionWithThisObject function = new TestFunctionWithThisObject (null, null);

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          function.GetType (),
          _attribute);

      helper.GetSecurableObject (function);
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage = "The parameter 'SomeObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
        + " WxeFunction 'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' does not implement"
        + " interface 'Remotion.Security.ISecurableObject'.")]
    public void TestWithParameterNotImplementingISecurableObject ()
    {
      _attribute.ParameterName = "SomeObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          _functionWithThisObjectAsSecondParamter.GetType (),
          _attribute);

      helper.GetSecurableObject (_functionWithThisObjectAsSecondParamter);
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage =
        "The parameter 'ThisObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to WxeFunction "
        + "'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' is of type "
        + "'Remotion.Web.UnitTests.Core.Security.Domain.SecurableObject', which is not a base type of type "
        + "'Remotion.Web.UnitTests.Core.Security.Domain.OtherSecurableObject'.")]
    public void TestWithParameterNotOfMatchingType ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("Show", typeof (OtherSecurableObject));
      attribute.ParameterName = "ThisObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          _functionWithThisObjectAsSecondParamter.GetType (),
          attribute);

      helper.GetSecurableObject (_functionWithThisObjectAsSecondParamter);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'function' has type 'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithoutPermissions' "
        + "when type 'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithThisObject' was expected.\r\nParameter name: function")]
    public void TestWithInvalidFunctionType ()
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          _attribute);

      helper.GetSecurableObject (new TestFunctionWithoutPermissions());
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage =
        "WxeFunction 'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithoutParameters' "
        + "has a WxeDemandTargetMethodPermissionAttribute applied, but does not define any parameters to supply the 'this-object'.")]
    public void TestWithFunctionWithoutParameters ()
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithoutParameters),
          _attribute);

      helper.GetSecurableObject (new TestFunctionWithoutParameters());
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage =
        "The parameter 'Invalid' specified by the WxeDemandTargetMethodPermissionAttribute "
        + "applied to WxeFunction 'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' is not a valid parameter"
        + " of this function.")]
    public void TestWithInvalidParameterName ()
    {
      _attribute.ParameterName = "Invalid";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          _functionWithThisObjectAsSecondParamter.GetType(),
          _attribute);

      helper.GetSecurableObject (_functionWithThisObjectAsSecondParamter);
    }

    [Test]
    public void TestWithHandle_PointingToSecurableObject_ShouldReturnValue ()
    {
      var attribute = new WxeDemandTargetMethodPermissionAttribute ("Some method", typeof (SecurableObject))
      {
        ParameterName = "HandleWithSecurableObject"
      };
      var helper = new WxeDemandMethodPermissionAttributeHelper (typeof (TestFunctionWithHandleParameter), attribute);
      var function = new TestFunctionWithHandleParameter { HandleWithSecurableObject = new Handle<SecurableObject> (_securableObject) };
      
      var result = helper.GetSecurableObject (function);

      Assert.That (result, Is.SameAs (_securableObject));
    }

    [Test]
    public void TestWithHandle_PointingToNull_ShouldThrow ()
    {
      var attribute = new WxeDemandTargetMethodPermissionAttribute ("Some method", typeof (SecurableObject))
      {
        ParameterName = "HandleWithSecurableObject"
      };
      var helper = new WxeDemandMethodPermissionAttributeHelper (typeof (TestFunctionWithHandleParameter), attribute);
      var function = new TestFunctionWithHandleParameter { HandleWithSecurableObject = new Handle<SecurableObject> (null) };

      Assert.That (
          () => helper.GetSecurableObject (function), 
          Throws.TypeOf<WxeException> ().With.Message.EqualTo (
              "The parameter 'HandleWithSecurableObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to WxeFunction "
              + "'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithHandleParameter' is null."));
    }

    [Test]
    public void TestWithHandle_NullHandle_ShouldThrow ()
    {
      var attribute = new WxeDemandTargetMethodPermissionAttribute ("Some method", typeof (SecurableObject))
      {
        ParameterName = "HandleWithSecurableObject"
      };
      var helper = new WxeDemandMethodPermissionAttributeHelper (typeof (TestFunctionWithHandleParameter), attribute);
      var function = new TestFunctionWithHandleParameter { HandleWithSecurableObject = null };

      Assert.That (
          () => helper.GetSecurableObject (function),
          Throws.TypeOf<WxeException> ().With.Message.EqualTo (
              "The parameter 'HandleWithSecurableObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to WxeFunction "
              + "'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithHandleParameter' is null."));
    }

    [Test]
    public void TestWithHandle_PointingToNonSecurableObject_ShouldThrow ()
    {
      var attribute = new WxeDemandTargetMethodPermissionAttribute ("Some method", typeof (SecurableObject))
      {
        ParameterName = "HandleWithNonSecurableObject"
      };
      var helper = new WxeDemandMethodPermissionAttributeHelper (typeof (TestFunctionWithHandleParameter), attribute);
      var function = new TestFunctionWithHandleParameter { HandleWithNonSecurableObject = new Handle<object> (new object ()) };

      Assert.That (
          () => helper.GetSecurableObject (function),
          Throws.TypeOf<WxeException>().With.Message.EqualTo (
              "The parameter 'HandleWithNonSecurableObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to WxeFunction "
              + "'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithHandleParameter' does not implement interface "
              + "'Remotion.Security.ISecurableObject'."));
    }

    [Test]
    public void TestWithHandle_PointingToSecurableObject_ButIncompatibleParameterDeclaration_ShouldThrow ()
    {
      var attribute = new WxeDemandTargetMethodPermissionAttribute ("Some method", typeof (OtherSecurableObject))
      {
        ParameterName = "HandleWithSecurableObject"
      };
      var helper = new WxeDemandMethodPermissionAttributeHelper (typeof (TestFunctionWithHandleParameter), attribute);
      var function = new TestFunctionWithHandleParameter { HandleWithSecurableObject = new Handle<SecurableObject> (_securableObject) };

      Assert.That (
          () => helper.GetSecurableObject (function),
          Throws.TypeOf<WxeException>().With.Message.EqualTo (
              "The parameter 'HandleWithSecurableObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to WxeFunction "
              + "'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithHandleParameter' is of type "
              + "'Remotion.Web.UnitTests.Core.Security.Domain.SecurableObject', which is not a base type of type "
              + "'Remotion.Web.UnitTests.Core.Security.Domain.OtherSecurableObject'."));
    }
  }
}
