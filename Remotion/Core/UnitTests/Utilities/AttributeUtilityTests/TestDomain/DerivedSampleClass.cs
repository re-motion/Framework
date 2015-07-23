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
using JetBrains.Annotations;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests.TestDomain
{
  public class DerivedSampleClass : SampleClass
  {
    public override string PropertyWithSingleAttribute
    {
      get { return null; }
    }

    protected override string ProtectedPropertyWithAttribute
    {
      get { return null; }
    }

    [Multiple]
    public override string PropertyWithMultipleAttribute
    {
      get { return null; }
    }

    public override string this[int i]
    {
      get { return null; }
    }

    public override string PropertyWithoutGetter
    {
      set { Dev.Null = value; }
    }

    public override string PropertyWithNotInheritedAttribute
    {
      get { return null; }
    }

    [InheritedNotMultiple ("Derived")]
    public override string PropertyWithInheritedNotMultipleAttribute
    {
      get { return null; }
    }

    public override event System.EventHandler EventWithSingleAttribute;
    protected override event System.EventHandler ProtectedEventWithAttribute;
    [Multiple]
    public override event System.EventHandler EventWithMultipleAttribute;
    public override event System.EventHandler EventWithNotInheritedAttribute;
    [InheritedNotMultiple ("Derived")]
    public override event System.EventHandler EventWithInheritedNotMultipleAttribute;

    [UsedImplicitly]
    public override string MethodWithSingleAttribute ()
    {
      return base.MethodWithSingleAttribute ();
    }

    [UsedImplicitly]
    protected override string ProtectedMethodWithAttribute ()
    {
      return base.ProtectedMethodWithAttribute ();
    }

    [Multiple]
    public override string MethodWithMultipleAttribute ()
    {
      return base.MethodWithMultipleAttribute ();
    }

    [UsedImplicitly]
    public override string MethodWithNotInheritedAttribute ()
    {
      return base.MethodWithNotInheritedAttribute ();
    }

    [InheritedNotMultiple ("Derived")]
    public override string MethodWithInheritedNotMultipleAttribute ()
    {
      return base.MethodWithInheritedNotMultipleAttribute ();
    }
  }
}
