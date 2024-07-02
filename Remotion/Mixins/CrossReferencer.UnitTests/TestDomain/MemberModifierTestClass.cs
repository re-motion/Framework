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

namespace Remotion.Mixins.CrossReferencer.UnitTests.TestDomain
{
  public abstract class MemberModifierTestClass : IDisposable
  {
    static MemberModifierTestClass ()
    {
    }

    public void PublicMethod ()
    {
    }

    protected string ProtectedProperty { get; set; }

    protected internal event ChangedEventHandler ProtectedInternalEvent;

    internal string InternalField = null;

    private string _privateField;

    public virtual void PublicVirtualMethod ()
    {
    }

    public abstract void PublicAbstractMethod ();

    public void Dispose ()
    {
      throw new NotImplementedException();
    }

    public static int _staticField;

    public static void StaticMethod ()
    {
    }

    public static class NestedStaticClass
    {
    }

    public readonly string _readonlyField;

    public class NestedClass
    {
    }
  }

  public delegate void ChangedEventHandler (object sender, EventArgs e);


  public abstract class SubModifierTestClass : MemberModifierTestClass
  {
    public abstract override void PublicAbstractMethod ();

    public override sealed void PublicVirtualMethod ()
    {
    }
  }
}
