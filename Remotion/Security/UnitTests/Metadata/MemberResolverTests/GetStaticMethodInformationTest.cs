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
  public class GetStaticMethodInformationTest
  {
    private ReflectionBasedMemberResolver _resolver;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new ReflectionBasedMemberResolver ();
    }

    [Test]
    public void Test_MethodWithoutAttribute ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "CheckPermissions", MemberAffiliation.Static);

      Assert.That (methodInformation, Is.InstanceOf (typeof(NullMethodInformation)));
    }

    [Test]
    public void Test_CacheForMethodWithoutAttributes ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "CheckPermissions", MemberAffiliation.Static);

      Assert.That (_resolver.GetMethodInformation (typeof (SecurableObject), "CheckPermissions", MemberAffiliation.Static), Is.SameAs (methodInformation));
    }

    [Test]
    public void Test_MethodWithOneAttribute ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "CreateForSpecialCase", MemberAffiliation.Static);
      var expectedMethodInformation = MethodInfoAdapter.Create(typeof (SecurableObject).GetMethod ("CreateForSpecialCase"));

      Assert.That (methodInformation, Is.Not.Null);
      Assert.That (methodInformation, Is.EqualTo (expectedMethodInformation));
    }

    [Test]
    public void Test_CacheForMethodWithOneAttribut ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "CreateForSpecialCase", MemberAffiliation.Static);

      Assert.That (_resolver.GetMethodInformation (typeof (SecurableObject), "CreateForSpecialCase", MemberAffiliation.Static), Is.SameAs (methodInformation));
    }

    [Test]
    public void Test_OverloadedMethodWithOneAttributes ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (SecurableObject), "IsValid", MemberAffiliation.Static);
      var expectedMethodInformation = MethodInfoAdapter.Create(typeof (SecurableObject).GetMethod ("IsValid", new [] {typeof(SecurableObject)}));

      Assert.That (methodInformation, Is.Not.Null);
      Assert.That (methodInformation, Is.EqualTo (expectedMethodInformation));
    }

    [Test]
    public void Test_MethodOfDerivedClass ()
    {
      var methodInformation = _resolver.GetMethodInformation (typeof (DerivedSecurableObject), "CreateForSpecialCase", MemberAffiliation.Static);
      var expectedMethodInformation = MethodInfoAdapter.Create(typeof (SecurableObject).GetMethod ("CreateForSpecialCase"));

      Assert.That (methodInformation, Is.Not.Null);
      Assert.That (methodInformation, Is.EqualTo (expectedMethodInformation));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The method 'Sve' could not be found.\r\nParameter name: methodName")]
    public void Test_NotExistingMethod ()
    {
      _resolver.GetMethodInformation (typeof (SecurableObject), "Sve", MemberAffiliation.Static);
    }
  }
}