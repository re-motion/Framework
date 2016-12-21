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

namespace Remotion.UnitTests.Reflection.TypeExtensionsTests
{
  [TestFixture]
  public class CanAscribe_WithGenericInterface
  {
    [Test]
    public void ClosedGenericInterface ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<ParameterType>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<object>)), Is.False);

      Assert.That (TypeExtensions.CanAscribeTo (typeof (IGenericInterface<ParameterType>), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (IGenericInterface<ParameterType>), typeof (IGenericInterface<ParameterType>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (IGenericInterface<ParameterType>), typeof (IGenericInterface<object>)), Is.False);
    }

    [Test]
    public void ClosedGenericInterface_WithDerivedType ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (DerivedTypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (DerivedTypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<ParameterType>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (DerivedTypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<object>)), Is.False);

      Assert.That (TypeExtensions.CanAscribeTo (typeof (IDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (IDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<ParameterType>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (IDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<object>)), Is.False);
    }

    [Test]
    public void ClosedGenericInterface_WithTwoTypeParameters ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericInterface<ParameterType, int>), typeof (IGenericInterface<,>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericInterface<ParameterType, int>), typeof (IGenericInterface<ParameterType, int>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericInterface<ParameterType, int>), typeof (IGenericInterface<object, int>)), Is.False);
    }

    [Test]
    public void OpenGenericInterface ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericInterface<>), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericInterface<>), typeof (IGenericInterface<ParameterType>)), Is.False);

      Assert.That (TypeExtensions.CanAscribeTo (typeof (IGenericInterface<>), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (IGenericInterface<>), typeof (IGenericInterface<ParameterType>)), Is.False);
    }

    [Test]
    public void OpenGenericInterface_WithTwoTypeParameters ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericInterface<,>), typeof (IGenericInterface<,>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericInterface<,>), typeof (IGenericInterface<ParameterType,int>)), Is.False);
    }

    [Test]
    public void OpenGenericInterface_WithOneOpenTypeParameter ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithDerivedOpenGenericInterface<>), typeof (IGenericInterface<,>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithDerivedOpenGenericInterface<>), typeof (IGenericInterface<ParameterType,int>)), Is.False);
    }

    [Test]
    public void ClosedDerivedGenericInterface ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<ParameterType>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<object>)), Is.False);
    }

    [Test]
    public void OpenDerivedGenericInterface ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithDerivedGenericInterface<>), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithDerivedGenericInterface<>), typeof (IGenericInterface<ParameterType>)), Is.False);

      Assert.That (TypeExtensions.CanAscribeTo (typeof (IDerivedGenericInterface<>), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (IDerivedGenericInterface<>), typeof (IGenericInterface<ParameterType>)), Is.False);
    }

    [Test]
    public void NonGenericDerivedGenericInterface ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithDerivedGenericInterface), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithDerivedGenericInterface), typeof (IGenericInterface<ParameterType>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithDerivedGenericInterface), typeof (IGenericInterface<object>)), Is.False);

      Assert.That (TypeExtensions.CanAscribeTo (typeof (IDerivedGenericInterface), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (IDerivedGenericInterface), typeof (IGenericInterface<ParameterType>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (IDerivedGenericInterface), typeof (IGenericInterface<object>)), Is.False);
    }

    [Test]
    public void ClosedGenericDerivedGenericInterface ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericDerivedGenericInterface<int>), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericDerivedGenericInterface<int>), typeof (IGenericInterface<ParameterType>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericDerivedGenericInterface<int>), typeof (IGenericInterface<object>)), Is.False);
    }

    [Test]
    public void OpenGenericDerivedGenericInterface ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericDerivedGenericInterface<>), typeof (IGenericInterface<>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericDerivedGenericInterface<>), typeof (IGenericInterface<ParameterType>)), Is.True);
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithGenericDerivedGenericInterface<>), typeof (IGenericInterface<object>)), Is.False);
    }

    [Test]
    public void BaseInterface ()
    {
      Assert.That (TypeExtensions.CanAscribeTo (typeof (TypeWithBaseInterface), typeof (IGenericInterface<>)), Is.False);
    }
  }
}
