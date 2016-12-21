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
  public class GetAscribedGenericArguments_WithNonGenericInterface
  {
    [Test]
    public void DerivedType ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (DerivedType), typeof (IDerivedInterface)), Is.EqualTo (new Type[0]));
    }

    [Test]
    public void DerivedInterface ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (IDerivedInterface), typeof (IDerivedInterface)), Is.EqualTo (new Type[0]));
    }

    [Test]
    public void DerivedInterfaceFromBaseInterface ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (IDerivedInterface), typeof (IBaseInterface)), Is.EqualTo (new Type[0]));
    }

    [Test]
    public void DerivedTypeFromBaseInterface ()
    {
      Assert.That (TypeExtensions.GetAscribedGenericArguments (typeof (DerivedType), typeof (IBaseInterface)), Is.EqualTo (new Type[0]));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'type' has type 'Remotion.UnitTests.Reflection.TypeExtensionsTests.BaseType' "
        + "when type 'Remotion.UnitTests.Reflection.TypeExtensionsTests.IDerivedInterface' was expected.\r\n"
        + "Parameter name: type")]
    public void BaseType ()
    {
      TypeExtensions.GetAscribedGenericArguments (typeof (BaseType), typeof (IDerivedInterface));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'type' has type 'Remotion.UnitTests.Reflection.TypeExtensionsTests.IBaseInterface' "
        + "when type 'Remotion.UnitTests.Reflection.TypeExtensionsTests.IDerivedInterface' was expected.\r\n"
        + "Parameter name: type")]
    public void BaseInterface ()
    {
      TypeExtensions.GetAscribedGenericArguments (typeof (IBaseInterface), typeof (IDerivedInterface));
    }
  }
}
