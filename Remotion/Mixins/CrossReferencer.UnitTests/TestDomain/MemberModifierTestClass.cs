// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;

namespace MixinXRef.UnitTests.TestDomain
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

    public virtual void PublicVirtualMethod() {}

    public abstract void PublicAbstractMethod();

    public void Dispose ()
    {
      throw new NotImplementedException ();
    }

    public static int _staticField;

    public static void StaticMethod ()
    {}

    public static class NestedStaticClass
    {}

    public readonly string _readonlyField;

    public class NestedClass {}

  }
  public delegate void ChangedEventHandler(object sender, EventArgs e);


  public abstract class SubModifierTestClass : MemberModifierTestClass
  {
    public abstract override void PublicAbstractMethod();

    public override sealed void PublicVirtualMethod()
    {
    }
  }
  
}