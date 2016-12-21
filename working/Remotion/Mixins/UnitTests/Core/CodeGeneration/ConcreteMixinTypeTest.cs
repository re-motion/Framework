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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Mixins.CodeGeneration;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [TestFixture]
  public class ConcreteMixinTypeTest
  {
    private ConcreteMixinType _concreteMixinType;
    private MethodInfo _nonPublicMethod;
    private MethodInfo _publicMethod;
    private MethodInfo _wrapperOrInterfaceMethod;

    [SetUp]
    public void SetUp ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      _nonPublicMethod = ReflectionObjectMother.GetSomeNonPublicMethod();
      _publicMethod = ReflectionObjectMother.GetSomePublicMethod();
      _wrapperOrInterfaceMethod = ReflectionObjectMother.GetSomeMethod();
      _concreteMixinType = new ConcreteMixinType (
          identifier, 
          typeof (object),
          typeof (IServiceProvider),
          new Dictionary<MethodInfo, MethodInfo> { { _nonPublicMethod, _wrapperOrInterfaceMethod } },
          new Dictionary<MethodInfo, MethodInfo> { { _nonPublicMethod, _wrapperOrInterfaceMethod } });
    }

    [Test]
    public void GetPubliclyCallableMixinMethod ()
    {
      Assert.That (_concreteMixinType.GetPubliclyCallableMixinMethod (_nonPublicMethod), Is.SameAs (_wrapperOrInterfaceMethod));
    }

    [Test]
    public void GetPubliclyCallableMixinMethod_ForPublicMethod ()
    {
      Assert.That (_concreteMixinType.GetPubliclyCallableMixinMethod (_publicMethod), Is.SameAs (_publicMethod));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), ExpectedMessage = "No public wrapper was generated for method 'System.Object.MemberwiseClone'.")]
    public void GetPubliclyCallableMixinMethod_NotFound ()
    {
      var method = typeof (StringBuilder).GetMethod ("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
      _concreteMixinType.GetPubliclyCallableMixinMethod (method);
    }

    [Test]
    public void GetOverrideInterfaceMethod ()
    {
      Assert.That (_concreteMixinType.GetOverrideInterfaceMethod (_nonPublicMethod), Is.SameAs (_wrapperOrInterfaceMethod));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), ExpectedMessage = "No override interface method was generated for method 'System.Object.ToString'.")]
    public void GetOverrideInterfaceMethod_NotFound ()
    {
      var method = typeof (object).GetMethod ("ToString");
      _concreteMixinType.GetOverrideInterfaceMethod (method);
    }

  }
}
