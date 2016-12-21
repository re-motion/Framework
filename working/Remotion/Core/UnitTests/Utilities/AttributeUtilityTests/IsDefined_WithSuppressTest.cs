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
using Remotion.UnitTests.Utilities.AttributeUtilityTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class IsDefined_WithSuppressTest
  {
    [Test]
    public void IsDefined_NoAttributes ()
    {
      Assert.That (AttributeUtility.IsDefined (typeof (DerivedDerivedDerivedSuppressed),typeof(Attribute), true), Is.False);
    }

    [Test]
    public void IsDefined_WithAttributesAndSuppressed()
    {
      Assert.That (AttributeUtility.IsDefined (typeof (DerivedWithAttributesAndSuppressed), typeof (Attribute), true), Is.True);
    }

    [Test]
    public void IsDefined_WithAttributesAndNoSuppressed ()
    {
      Assert.That (AttributeUtility.IsDefined (typeof (BaseClassWithAttribute), typeof (Attribute), true), Is.True);
    }
  }
}
