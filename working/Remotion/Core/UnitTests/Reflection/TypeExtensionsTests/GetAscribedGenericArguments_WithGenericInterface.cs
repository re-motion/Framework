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
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;

namespace Remotion.UnitTests.Reflection.TypeExtensionsTests
{
  [TestFixture]
  public class GetAscribedGenericArguments_WithGenericInterface
  {
    [Test]
    public void ClosedGenericType ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));

      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (IGenericInterface<ParameterType>), typeof (IGenericInterface<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));
    }

    [Test]
    public void ClosedGenericType_WithDerivedType ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (DerivedTypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));

      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (IDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));
    }

    [Test]
    public void ClosedGenericType_WithTwoTypeParameters ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<ParameterType, int>), typeof (IGenericInterface<,>)), Is.EqualTo (new Type[] {typeof (ParameterType), typeof (int)}));
    }

    [Test]
    public void OpenGenericType_WithClass ()
    {
      Type[] types = TypeExtensions.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<>), typeof (IGenericInterface<>));
      Assert.That (types.Length, Is.EqualTo (1));
      Assert.That (types[0].Name, Is.EqualTo ("T"));
    }

    [Test]
    public void OpenGenericType_WihtInterface ()
    {
      Type[] types = TypeExtensions.GetAscribedGenericArguments (typeof (IGenericInterface<>), typeof (IGenericInterface<>));
      Assert.That (types.Length, Is.EqualTo (1));
      Assert.That (types[0].Name, Is.EqualTo ("T"));
    }

    [Test]
    public void OpenGenericType_WithTwoTypeParameters ()
    {
      Type[] types = TypeExtensions.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<,>), typeof (IGenericInterface<,>));
      Assert.That (types.Length, Is.EqualTo (2));
      Assert.That (types[0].Name, Is.EqualTo ("T1"));
      Assert.That (types[1].Name, Is.EqualTo ("T2"));
    }

    [Test]
    public void OpenGenericType_WithOneOpenTypeParameter ()
    {
      Type[] types = TypeExtensions.GetAscribedGenericArguments (typeof (TypeWithDerivedOpenGenericInterface<>), typeof (IGenericInterface<,>));
      Assert.That (types.Length, Is.EqualTo (2));
      Assert.That (types[0], Is.EqualTo (typeof (ParameterType)));
      Assert.That (types[1].Name, Is.EqualTo ("T"));
    }

    [Test]
    public void ClosedDerivedGenericType ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (TypeWithDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));
    }

    [Test]
    public void OpenDerivedGenericType_WithClass ()
    {
      Type[] types = TypeExtensions.GetAscribedGenericArguments (typeof (TypeWithDerivedGenericInterface<>), typeof (IGenericInterface<>));
      Assert.That (types.Length, Is.EqualTo (1));
      Assert.That (types[0].Name, Is.EqualTo ("T"));


      Assert.That (TypeExtensions.CanAscribeTo (typeof (IDerivedGenericInterface<>), typeof (IGenericInterface<>)), Is.True);
    }

    [Test]
    public void OpenDerivedGenericType_WithInterface ()
    {
      Type[] types = TypeExtensions.GetAscribedGenericArguments (typeof (IDerivedGenericInterface<>), typeof (IGenericInterface<>));
      Assert.That (types.Length, Is.EqualTo (1));
      Assert.That (types[0].Name, Is.EqualTo ("T"));
    }

    [Test]
    public void NonGenericDerivedGenericType ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (TypeWithDerivedGenericInterface), typeof (IGenericInterface<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));

      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (IDerivedGenericInterface), typeof (IGenericInterface<>)), Is.EqualTo (new Type[] { typeof (ParameterType) }));

      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (IDoubleDerivedGenericInterface), typeof (IGenericInterface<>)), Is.EqualTo (new Type[] { typeof (ParameterType) }));
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = 
      "The type Remotion.UnitTests.Reflection.TypeExtensionsTests.IDoubleInheritingGenericInterface implements "
      + "the given interface type Remotion.UnitTests.Reflection.TypeExtensionsTests.IGenericInterface`1 more than once.")]
    public void TwoSetsOfArguments ()
    {
      TypeExtensions.GetAscribedGenericArguments (typeof (IDoubleInheritingGenericInterface), typeof (IGenericInterface<>));
    }

    [Test]
    public void TwoSetsOfArgumentsOfWhichOneFitsTheRequest ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (IDoubleInheritingGenericInterface), typeof (IGenericInterface<int>)), Is.EqualTo (new Type[] {typeof (int) }));
    }

    [Test]
    public void ClosedGenericDerivedGenericType ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (TypeWithGenericDerivedGenericInterface<int>), typeof (IGenericInterface<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));
    }

    [Test]
    public void OpenGenericDerivedGenericType ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (TypeWithGenericDerivedGenericInterface<>), typeof (IGenericInterface<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'type' has type 'Remotion.UnitTests.Reflection.TypeExtensionsTests.TypeWithBaseInterface' "
        + "when type 'Remotion.UnitTests.Reflection.TypeExtensionsTests.IGenericInterface`1[T]' was expected."
        + "\r\nParameter name: type")]
    public void BaseType ()
    {
      TypeExtensions.GetAscribedGenericArguments (typeof (TypeWithBaseInterface), typeof (IGenericInterface<>));
    }
  }
}
