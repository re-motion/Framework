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
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Core.Security.Domain;

namespace Remotion.Web.UnitTests.Core.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandMethodPermissionAttributeHelperTest
  {
    [Test]
    public void InitializeWithMethodTypeInstance ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("Show");

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.That (helper.FunctionType, Is.SameAs (typeof (TestFunctionWithThisObject)));
      Assert.That (helper.MethodType, Is.EqualTo (MethodType.Instance));
      Assert.That (helper.SecurableClass, Is.Null);
      Assert.That (helper.MethodName, Is.EqualTo ("Show"));
    }

    [Test]
    public void InitializeWithMethodTypeInstanceAndSecurableClass ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("ShowSpecial", typeof (DerivedSecurableObject));

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.That (helper.FunctionType, Is.SameAs (typeof (TestFunctionWithThisObject)));
      Assert.That (helper.MethodType, Is.EqualTo (MethodType.Instance));
      Assert.That (helper.SecurableClass, Is.SameAs (typeof (DerivedSecurableObject)));
      Assert.That (helper.MethodName, Is.EqualTo ("ShowSpecial"));
    }

    [Test]
    public void InitializeWithMethodTypeInstanceAndMethodEnum ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute (SecurableObject.Method.Show);

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.That (helper.FunctionType, Is.SameAs (typeof (TestFunctionWithThisObject)));
      Assert.That (helper.MethodType, Is.EqualTo (MethodType.Instance));
      Assert.That (helper.SecurableClass, Is.SameAs (typeof (SecurableObject)));
      Assert.That (helper.MethodName, Is.EqualTo ("Show"));
    }


    [Test]
    public void InitializeWithMethodTypeStatic ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = new WxeDemandTargetStaticMethodPermissionAttribute ("Search", typeof (SecurableObject));

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.That (helper.FunctionType, Is.SameAs (typeof (TestFunctionWithThisObject)));
      Assert.That (helper.MethodType, Is.EqualTo (MethodType.Static));
      Assert.That (helper.SecurableClass, Is.SameAs (typeof (SecurableObject)));
      Assert.That (helper.MethodName, Is.EqualTo ("Search"));
    }

    [Test]
    public void InitializeWithMethodTypeStaticAndMethodEnum ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Method.Search);

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.That (helper.FunctionType, Is.SameAs (typeof (TestFunctionWithThisObject)));
      Assert.That (helper.MethodType, Is.EqualTo (MethodType.Static));
      Assert.That (helper.SecurableClass, Is.SameAs (typeof (SecurableObject)));
      Assert.That (helper.MethodName, Is.EqualTo ("Search"));
    }

    [Test]
    public void InitializeWithMethodTypeStaticAndMethodEnumFromBaseClass ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = 
          new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Method.Search, typeof (DerivedSecurableObject));

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.That (helper.FunctionType, Is.SameAs (typeof (TestFunctionWithThisObject)));
      Assert.That (helper.MethodType, Is.EqualTo (MethodType.Static));
      Assert.That (helper.SecurableClass, Is.SameAs (typeof (DerivedSecurableObject)));
      Assert.That (helper.MethodName, Is.EqualTo ("Search"));
    }


    [Test]
    public void InitializeWithMethodTypeConstructor ()
    {
      WxeDemandTargetPermissionAttribute attribute = new WxeDemandCreatePermissionAttribute (typeof (SecurableObject));

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.That (helper.FunctionType, Is.SameAs (typeof (TestFunctionWithThisObject)));
      Assert.That (helper.MethodType, Is.EqualTo (MethodType.Constructor));
      Assert.That (helper.SecurableClass, Is.SameAs (typeof (SecurableObject)));
      Assert.That (helper.MethodName, Is.Null);
    }
  }
}
