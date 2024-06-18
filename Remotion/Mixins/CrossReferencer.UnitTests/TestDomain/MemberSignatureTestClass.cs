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
using MixinXRef.UnitTests.Formatting;
using Remotion.Collections;

namespace MixinXRef.UnitTests.TestDomain
{

  public class MemberSignatureTestClass : IExplicitInterface
  {
    protected MemberSignatureTestClass (int Param1, string Param2, Remotion.Reflection.IActionInvoker Param3)
    {}

    public void PublicMethod ()
    {
    }

    protected string ProtectedProperty { get; set; }

    protected internal event ChangedEventHandler ProtectedInternalEvent;

    internal string InternalField = null;

    private string _privateField;

    public virtual void PublicVirtualMethod()
    {
    }

    public class NestedClass
    { }

    public class NestedClassWithInterfaceAndInheritance : GenericTarget<string, int>, IDisposable
    {
      public void Dispose ()
      {
        throw new NotImplementedException();
      }
    }

    public interface INestedInterface : IDisposable, ICloneable
    {
      float Calculate ();  
    }

    public enum NestedEnumeration {}

    public struct NestedStruct : IDisposable {
      public void Dispose ()
      {
        throw new NotImplementedException();
      }
    }

    public void Dispose ()
    {
      throw new NotImplementedException();
    }

    public long MethodWithParams (int intParam, string stringParam, AssemblyResolver assemblyBuilderParam) { return 0; }

    public long MethodWithNestedType (NestedClassWithInterfaceAndInheritance param) { return 0; }

    public long MethodWithNestedGenericType (NestedGenericType<NestedClassWithInterfaceAndInheritance> param) { return 0; }

    protected MultiDictionary<string, int> _dictionary;

    public class NestedGenericType<T>
    {
    }

    public interface INestedExplicitInterface : IExplicitInterface
    {
      
    }

    public class SubClass :IExplicitInterface
    {
      string IExplicitInterface.Version ()
      {
        throw new NotImplementedException();
      }

      int IExplicitInterface.Count
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      event EventHandler IExplicitInterface.eventHandler
      {
        add { throw new NotImplementedException(); }
        remove { throw new NotImplementedException(); }
      }
    }

    // explicit interface
    string IExplicitInterface.Version ()
    {
      throw new NotImplementedException();
    }

    int IExplicitInterface.Count
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    event EventHandler IExplicitInterface.eventHandler
    {
      add { throw new NotImplementedException(); }
      remove { throw new NotImplementedException(); }
    }
  }


  public interface IExplicitInterface
  {
    string Version ();
    int Count { get; set; }
    event EventHandler eventHandler;
  }
}