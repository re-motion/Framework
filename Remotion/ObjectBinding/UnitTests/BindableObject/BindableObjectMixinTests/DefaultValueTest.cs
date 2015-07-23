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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class DefaultValueTest
  {
    public class DefaultValueTrueMixin : Mixin<BindableObjectMixin>
    {
      [OverrideTarget]
      public bool IsDefaultValue (PropertyBase property, object nativeValue)
      {
        return true;
      }
    }

    [Test]
    public void GetProperty_NormallyReturnsNonNull ()
    {
      ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>> (ParamList.Empty);
      IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

      Assert.That (instanceAsIBusinessObject.GetProperty ("Scalar"), Is.Not.Null);
      Assert.That (instanceAsIBusinessObject.GetProperty ("Scalar"), Is.EqualTo (instance.Scalar));
    }

    [Test]
    public void GetProperty_ReturnsNull_WhenDefaultValueTrue ()
    {
      ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>> (ParamList.Empty);
      IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

      Assert.That(instanceAsIBusinessObject.GetProperty ("Scalar"), Is.EqualTo(0));
    }

    [Test]
    public void GetProperty_ReturnsNonNull_WhenDefaultValueTrueOnList ()
    {
      ClassWithValueType<int> instance = ObjectFactory.Create<ClassWithValueType<int>> (ParamList.Empty);
      IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

      Assert.That (instanceAsIBusinessObject.GetProperty ("List"), Is.Not.Null);
      Assert.That (instanceAsIBusinessObject.GetProperty ("List"), Is.EqualTo (instance.List));
    }
  }
}
