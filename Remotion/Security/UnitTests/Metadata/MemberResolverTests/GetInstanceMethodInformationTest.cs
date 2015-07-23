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
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests.Metadata.MemberResolverTests
{
  [TestFixture]
  public class GetInstanceMethodInformationTest
  {
    private ReflectionBasedMemberResolver _resolver;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new ReflectionBasedMemberResolver();
    }

    [Test]
    public void Test_MethodWithoutAttributes ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "Save", MemberAffiliation.Instance);

      Assert.That (methodInformation, Is.InstanceOf(typeof(NullMethodInformation)));
    }

    [Test]
    public void Test_CacheForMethodWithoutAttributes ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "Save", MemberAffiliation.Instance);

      Assert.That (_resolver.GetMethodInformation (typeof (SecurableObject), "Save", MemberAffiliation.Instance), Is.SameAs (methodInformation));
    }

    [Test]
    public void Test_MethodWithOneAttribute ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "Load", MemberAffiliation.Instance);
      var expectedMethodInformation = MethodInfoAdapter.Create(typeof (SecurableObject).GetMethod ("Load",new Type[] {}));

      Assert.That (methodInformation, Is.Not.Null);
      Assert.That (methodInformation, Is.EqualTo (expectedMethodInformation));
    }

    [Test]
    public void Test_CacheForMethodWithOneAttribute ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "Load", MemberAffiliation.Instance);

      Assert.That (methodInformation, Is.SameAs(_resolver.GetMethodInformation (typeof (SecurableObject), "Load", MemberAffiliation.Instance)));
    }

    [Test]
    public void Test_OverloadedMethodWithOneAttribute ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "Delete", MemberAffiliation.Instance);
      var expectedMethodInformation = MethodInfoAdapter.Create(typeof (SecurableObject).GetMethod ("Delete", new [] {typeof(int)}));

      Assert.That (methodInformation, Is.Not.Null);
      Assert.That (methodInformation, Is.EqualTo (expectedMethodInformation));
    }

    [Test]
    public void Test_MethodOfDerivedClass ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (DerivedSecurableObject), "Show", MemberAffiliation.Instance);
      var expectedMethodInformation = MethodInfoAdapter.Create(typeof (SecurableObject).GetMethod ("Show"));

      Assert.That (methodInformation, Is.Not.Null);
      Assert.That (methodInformation, Is.EqualTo (expectedMethodInformation));
    }

    [Test]
    public void Test_OverriddenMethodFromBaseMethod ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (DerivedSecurableObject), "Record", MemberAffiliation.Instance);
      var expectedMethodInformation = MethodInfoAdapter.Create(typeof (SecurableObject).GetMethod ("Record"));

      Assert.That (methodInformation, Is.Not.Null);
      Assert.That (methodInformation, Is.EqualTo (expectedMethodInformation));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The method 'Sve' could not be found.\r\nParameter name: methodName")]
    public void Test_NotExistingMethod ()
    {
      _resolver.GetMethodInformation (typeof (SecurableObject), "Sve", MemberAffiliation.Instance);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
      "The DemandPermissionAttribute must not be defined on methods overriden or redefined in derived classes. "
        + "A method 'Send' exists in class 'Remotion.Security.UnitTests.SampleDomain.DerivedSecurableObject' and its base class."
        + "\r\nParameter name: methodName")]
    public void Test_MethodDeclaredOnBaseAndDerivedClass ()
    {
      _resolver.GetMethodInformation (typeof (DerivedSecurableObject), "Send", MemberAffiliation.Instance);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
      "The DemandPermissionAttribute must not be defined on methods overriden or redefined in derived classes. "
        + "A method 'Print' exists in class 'Remotion.Security.UnitTests.SampleDomain.DerivedSecurableObject' and its base class."
        + "\r\nParameter name: methodName")]
    public void Test_OverriddenMethods ()
    {
      _resolver.GetMethodInformation (typeof (DerivedSecurableObject), "Print", MemberAffiliation.Instance);
    }
  }
}