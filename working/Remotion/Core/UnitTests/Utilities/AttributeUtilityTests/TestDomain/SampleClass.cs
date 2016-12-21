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
  [Inherited]
  public class SampleClass
  {
    [Inherited]
    [UsedImplicitly]
    public virtual string PropertyWithSingleAttribute
    {
      get { return null; }
    }

    [Inherited]
    [UsedImplicitly]
    protected virtual string ProtectedPropertyWithAttribute
    {
      get { return null; }
    }

    [Multiple]
    [UsedImplicitly]
    public virtual string PropertyWithMultipleAttribute
    {
      get { return null; }
    }

    [Inherited]
    [UsedImplicitly]
    public virtual string this[int i]
    {
      get { return null; }
    }

    [UsedImplicitly]
    public virtual string this[string s]
    {
      get { return null; }
    }

    [Inherited]
    [UsedImplicitly]
    public virtual string PropertyWithoutGetter
    {
      set { Dev.Null = value; }
    }

    [NotInherited]
    [UsedImplicitly]
    public virtual string PropertyWithNotInheritedAttribute
    {
      get { return null; }
    }

    [InheritedNotMultiple ("Base")]
    [UsedImplicitly]
    public virtual string PropertyWithInheritedNotMultipleAttribute
    {
      get { return null; }
    }

    [Inherited]
    [UsedImplicitly]
    public virtual event EventHandler EventWithSingleAttribute;

    [Inherited]
    [UsedImplicitly]
    protected virtual event EventHandler ProtectedEventWithAttribute;

    [Multiple]
    [UsedImplicitly]
    public virtual event EventHandler EventWithMultipleAttribute;

    [NotInherited]
    [UsedImplicitly]
    public virtual event EventHandler EventWithNotInheritedAttribute;

    [InheritedNotMultiple ("Base")]
    [UsedImplicitly]
    public virtual event EventHandler EventWithInheritedNotMultipleAttribute;

    [Inherited]
    [UsedImplicitly]
    public virtual string MethodWithSingleAttribute ()
    {
      return null;
    }

    [Inherited]
    [UsedImplicitly]
    protected virtual string ProtectedMethodWithAttribute ()
    {
      return null;
    }

    [Multiple]
    [UsedImplicitly]
    public virtual string MethodWithMultipleAttribute ()
    {
      return null;
    }

    [NotInherited]
    [UsedImplicitly]
    public virtual string MethodWithNotInheritedAttribute ()
    {
      return null;
    }

    [InheritedNotMultiple ("Base")]
    [UsedImplicitly]
    public virtual string MethodWithInheritedNotMultipleAttribute()
    {
      return null;
    }
  }
}
