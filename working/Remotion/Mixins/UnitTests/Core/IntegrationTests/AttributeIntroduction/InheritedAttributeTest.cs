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

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.AttributeIntroduction
{
  [TestFixture]
  public class InheritedAttributeTest
  {
    [Test]
    public void AttributeNotFoundFromDerivedMixin ()
    {
      var obj = ObjectFactory.Create<TargetTypeWithDerivedMixin> ();
      Assert.That (obj.GetType ().IsDefined (typeof (InheritableAttribute), true), Is.True);
    }

    [Test]
    public void AttributeFoundFromBaseMixin ()
    {
      var obj = ObjectFactory.Create<TargetTypeWithBaseMixin> ();
      Assert.That (obj.GetType ().IsDefined (typeof (InheritableAttribute), true), Is.True);
    }

    [AttributeUsage (AttributeTargets.All, Inherited = true)]
    public class InheritableAttribute : Attribute { }

    [Inheritable]
    public class BaseMixinType
    { }

    public class DerivedMixinType : BaseMixinType
    { }

    [Uses (typeof (BaseMixinType))]
    public class TargetTypeWithBaseMixin
    { }

    [Uses (typeof (DerivedMixinType))]
    public class TargetTypeWithDerivedMixin
    { }
  }
}