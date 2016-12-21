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

namespace Remotion.ObjectBinding.UnitTests.TestDomain
{
  [BindableObject]
  public class ClassWithReferenceType<T> : IInterfaceWithReferenceType<T>
  {
    private T _explicitInterfaceScalar;
    private readonly T _readOnlyScalar = default (T);
    private Exception _exception;

    protected string NonPublicProperty { get; set; }

    public ClassWithReferenceType ()
    {
    }

    public T Scalar { get; set; }

    T IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar
    {
      get { return _explicitInterfaceScalar; }
      set { _explicitInterfaceScalar = value; }
    }

    T IInterfaceWithReferenceType<T>.ExplicitInterfaceReadOnlyScalar
    {
      get { return _explicitInterfaceScalar; }
    }

    public T ImplicitInterfaceScalar { get; set; }

    public T ImplicitInterfaceReadOnlyScalar { get; set; }

    public T this[int i]
    {
      get { return Scalar; }
      set { Scalar = value; }
    }

    public T this[int i, DateTime j]
    {
      get { return Scalar; }
      set { Scalar = value; }
    }

    public T this[int i, DateTime j, string k]
    {
      get { return Scalar; }
      set { Scalar = value; }
    }

    public T ReadOnlyScalar
    {
      get { return _readOnlyScalar; }
    }

    public T ReadOnlyNonPublicSetterScalar { get; protected set; }

    [ObjectBinding (Visible = false)]
    public T NotVisibleAttributeScalar { get; set; }

    public T NotVisibleNonPublicGetterScalar { protected get; set; }

    [ObjectBinding (ReadOnly = true)]
    public T ReadOnlyAttributeScalar { get; set; }

    public T[] Array { get; set; }

    public T PropertyWithNoSetter { get { return _explicitInterfaceScalar; } }
    public T PropertyWithNoGetter { set { _explicitInterfaceScalar = value; } }

    private T PrivateProperty { get; set; }

    public void PrepareException (Exception exception)
    {
      _exception = exception;
    }

    public T ThrowingProperty
    {
      get
      {
        if (_exception != null)
          throw _exception;
        return default (T);
      }
      set
      {
        if (_exception != null)
          throw _exception;
      }
    }
  }
}
