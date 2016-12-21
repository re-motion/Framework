﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Utilities;

namespace Remotion.Reflection
{
  public struct ActionInvoker : IActionInvoker
  {
    private readonly DelegateSelector _delegateSelector;



    public ActionInvoker (DelegateSelector delegateSelector)
    {
      _delegateSelector = delegateSelector;

    }

    private Type[] GetValueTypes (Type[] valueTypes)
    {
      return valueTypes;
    }

    private object[] GetValues (object[] values)
    {
      return values;
    }

    public void Invoke (Type[] valueTypes, object[] values)
    {
      InvokerUtility.CheckInvokeArguments (valueTypes, values);
      GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public void Invoke (object[] values)
    {
      Type[] valueTypes = InvokerUtility.GetValueTypes (values);
      GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public Delegate GetDelegate (params Type[] parameterTypes)
    {
      return GetDelegate (ActionUtility.MakeClosedType (parameterTypes));
    }

    public TDelegate GetDelegate<TDelegate> ()
    {
      return (TDelegate) (object) GetDelegate (typeof (TDelegate));
    }

    public Delegate GetDelegate (Type delegateType)
    {
      return _delegateSelector (delegateType);
    }

    public void With ()
    {
      GetDelegateWith () ();
    }

    public Action GetDelegateWith ()
    {
      return GetDelegate<Action> ();
    }

    public void With<A1> (A1 a1)
    {
      GetDelegateWith<A1> () (a1);
    }

    public Action<A1> GetDelegateWith<A1> ()
    {
      return GetDelegate<Action<A1>> ();
    }

    public void With<A1, A2> (A1 a1, A2 a2)
    {
      GetDelegateWith<A1, A2> () (a1, a2);
    }

    public Action<A1, A2> GetDelegateWith<A1, A2> ()
    {
      return GetDelegate<Action<A1, A2>> ();
    }

    public void With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      GetDelegateWith<A1, A2, A3> () (a1, a2, a3);
    }

    public Action<A1, A2, A3> GetDelegateWith<A1, A2, A3> ()
    {
      return GetDelegate<Action<A1, A2, A3>> ();
    }

    public void With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      GetDelegateWith<A1, A2, A3, A4> () (a1, a2, a3, a4);
    }

    public Action<A1, A2, A3, A4> GetDelegateWith<A1, A2, A3, A4> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4>> ();
    }

    public void With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      GetDelegateWith<A1, A2, A3, A4, A5> () (a1, a2, a3, a4, a5);
    }

    public Action<A1, A2, A3, A4, A5> GetDelegateWith<A1, A2, A3, A4, A5> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6> () (a1, a2, a3, a4, a5, a6);
    }

    public Action<A1, A2, A3, A4, A5, A6> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (a1, a2, a3, a4, a5, a6, a7);
    }

    public Action<A1, A2, A3, A4, A5, A6, A7> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6, A7>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> () (a1, a2, a3, a4, a5, a6, a7, a8);
    }

    public Action<A1, A2, A3, A4, A5, A6, A7, A8> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6, A7, A8>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> () (a1, a2, a3, a4, a5, a6, a7, a8, a9);
    }

    public Action<A1, A2, A3, A4, A5, A6, A7, A8, A9> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6, A7, A8, A9>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
    }

    public Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
    }

    public Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
    }

    public Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
    }

    public Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
    }

    public Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15);
    }

    public Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);
    }

    public Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17);
    }

    public Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> ()
    {
      return GetDelegate<Action<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17>> ();
    }
  }

  public struct ActionInvoker<TFixedArg1> : IActionInvoker
  {
    private readonly DelegateSelector _delegateSelector;

    private readonly TFixedArg1 _fixedArg1;

    public ActionInvoker (DelegateSelector delegateSelector, TFixedArg1 fixedArg1)
    {
      _delegateSelector = delegateSelector;
      _fixedArg1 = fixedArg1;
    }

    private Type[] GetValueTypes (Type[] valueTypes)
    {
      Type[] fixedArgTypes = new Type[] { typeof (TFixedArg1) };
      return ArrayUtility.Combine (fixedArgTypes, valueTypes);
    }

    private object[] GetValues (object[] values)
    {
      object[] fixedArgs = new object[] { _fixedArg1 };
      return ArrayUtility.Combine (fixedArgs, values);
    }

    public void Invoke (Type[] valueTypes, object[] values)
    {
      InvokerUtility.CheckInvokeArguments (valueTypes, values);
      GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public void Invoke (object[] values)
    {
      Type[] valueTypes = InvokerUtility.GetValueTypes (values);
      GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public Delegate GetDelegate (params Type[] parameterTypes)
    {
      return GetDelegate (ActionUtility.MakeClosedType (parameterTypes));
    }

    public TDelegate GetDelegate<TDelegate> ()
    {
      return (TDelegate) (object) GetDelegate (typeof (TDelegate));
    }

    public Delegate GetDelegate (Type delegateType)
    {
      return _delegateSelector (delegateType);
    }

    public void With ()
    {
      GetDelegateWith () (_fixedArg1);
    }

    public Action<TFixedArg1> GetDelegateWith ()
    {
      return GetDelegate<Action<TFixedArg1>> ();
    }

    public void With<A1> (A1 a1)
    {
      GetDelegateWith<A1> () (_fixedArg1, a1);
    }

    public Action<TFixedArg1, A1> GetDelegateWith<A1> ()
    {
      return GetDelegate<Action<TFixedArg1, A1>> ();
    }

    public void With<A1, A2> (A1 a1, A2 a2)
    {
      GetDelegateWith<A1, A2> () (_fixedArg1, a1, a2);
    }

    public Action<TFixedArg1, A1, A2> GetDelegateWith<A1, A2> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2>> ();
    }

    public void With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      GetDelegateWith<A1, A2, A3> () (_fixedArg1, a1, a2, a3);
    }

    public Action<TFixedArg1, A1, A2, A3> GetDelegateWith<A1, A2, A3> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3>> ();
    }

    public void With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      GetDelegateWith<A1, A2, A3, A4> () (_fixedArg1, a1, a2, a3, a4);
    }

    public Action<TFixedArg1, A1, A2, A3, A4> GetDelegateWith<A1, A2, A3, A4> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4>> ();
    }

    public void With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      GetDelegateWith<A1, A2, A3, A4, A5> () (_fixedArg1, a1, a2, a3, a4, a5);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5> GetDelegateWith<A1, A2, A3, A4, A5> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6> () (_fixedArg1, a1, a2, a3, a4, a5, a6);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17);
    }

    public Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> ()
    {
      return GetDelegate<Action<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17>> ();
    }
  }

  public struct ActionInvoker<TFixedArg1, TFixedArg2> : IActionInvoker
  {
    private readonly DelegateSelector _delegateSelector;

    private readonly TFixedArg1 _fixedArg1;
    private readonly TFixedArg2 _fixedArg2;

    public ActionInvoker (DelegateSelector delegateSelector, TFixedArg1 fixedArg1, TFixedArg2 fixedArg2)
    {
      _delegateSelector = delegateSelector;
      _fixedArg1 = fixedArg1;
      _fixedArg2 = fixedArg2;
    }

    private Type[] GetValueTypes (Type[] valueTypes)
    {
      Type[] fixedArgTypes = new Type[] { typeof (TFixedArg1), typeof (TFixedArg2) };
      return ArrayUtility.Combine (fixedArgTypes, valueTypes);
    }

    private object[] GetValues (object[] values)
    {
      object[] fixedArgs = new object[] { _fixedArg1, _fixedArg2 };
      return ArrayUtility.Combine (fixedArgs, values);
    }

    public void Invoke (Type[] valueTypes, object[] values)
    {
      InvokerUtility.CheckInvokeArguments (valueTypes, values);
      GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public void Invoke (object[] values)
    {
      Type[] valueTypes = InvokerUtility.GetValueTypes (values);
      GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public Delegate GetDelegate (params Type[] parameterTypes)
    {
      return GetDelegate (ActionUtility.MakeClosedType (parameterTypes));
    }

    public TDelegate GetDelegate<TDelegate> ()
    {
      return (TDelegate) (object) GetDelegate (typeof (TDelegate));
    }

    public Delegate GetDelegate (Type delegateType)
    {
      return _delegateSelector (delegateType);
    }

    public void With ()
    {
      GetDelegateWith () (_fixedArg1, _fixedArg2);
    }

    public Action<TFixedArg1, TFixedArg2> GetDelegateWith ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2>> ();
    }

    public void With<A1> (A1 a1)
    {
      GetDelegateWith<A1> () (_fixedArg1, _fixedArg2, a1);
    }

    public Action<TFixedArg1, TFixedArg2, A1> GetDelegateWith<A1> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1>> ();
    }

    public void With<A1, A2> (A1 a1, A2 a2)
    {
      GetDelegateWith<A1, A2> () (_fixedArg1, _fixedArg2, a1, a2);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2> GetDelegateWith<A1, A2> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2>> ();
    }

    public void With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      GetDelegateWith<A1, A2, A3> () (_fixedArg1, _fixedArg2, a1, a2, a3);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3> GetDelegateWith<A1, A2, A3> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3>> ();
    }

    public void With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      GetDelegateWith<A1, A2, A3, A4> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4> GetDelegateWith<A1, A2, A3, A4> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4>> ();
    }

    public void With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      GetDelegateWith<A1, A2, A3, A4, A5> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5> GetDelegateWith<A1, A2, A3, A4, A5> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17);
    }

    public Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17>> ();
    }
  }

  public struct ActionInvoker<TFixedArg1, TFixedArg2, TFixedArg3> : IActionInvoker
  {
    private readonly DelegateSelector _delegateSelector;

    private readonly TFixedArg1 _fixedArg1;
    private readonly TFixedArg2 _fixedArg2;
    private readonly TFixedArg3 _fixedArg3;

    public ActionInvoker (DelegateSelector delegateSelector, TFixedArg1 fixedArg1, TFixedArg2 fixedArg2, TFixedArg3 fixedArg3)
    {
      _delegateSelector = delegateSelector;
      _fixedArg1 = fixedArg1;
      _fixedArg2 = fixedArg2;
      _fixedArg3 = fixedArg3;
    }

    private Type[] GetValueTypes (Type[] valueTypes)
    {
      Type[] fixedArgTypes = new Type[] { typeof (TFixedArg1), typeof (TFixedArg2), typeof (TFixedArg3) };
      return ArrayUtility.Combine (fixedArgTypes, valueTypes);
    }

    private object[] GetValues (object[] values)
    {
      object[] fixedArgs = new object[] { _fixedArg1, _fixedArg2, _fixedArg3 };
      return ArrayUtility.Combine (fixedArgs, values);
    }

    public void Invoke (Type[] valueTypes, object[] values)
    {
      InvokerUtility.CheckInvokeArguments (valueTypes, values);
      GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public void Invoke (object[] values)
    {
      Type[] valueTypes = InvokerUtility.GetValueTypes (values);
      GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public Delegate GetDelegate (params Type[] parameterTypes)
    {
      return GetDelegate (ActionUtility.MakeClosedType (parameterTypes));
    }

    public TDelegate GetDelegate<TDelegate> ()
    {
      return (TDelegate) (object) GetDelegate (typeof (TDelegate));
    }

    public Delegate GetDelegate (Type delegateType)
    {
      return _delegateSelector (delegateType);
    }

    public void With ()
    {
      GetDelegateWith () (_fixedArg1, _fixedArg2, _fixedArg3);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3> GetDelegateWith ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3>> ();
    }

    public void With<A1> (A1 a1)
    {
      GetDelegateWith<A1> () (_fixedArg1, _fixedArg2, _fixedArg3, a1);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1> GetDelegateWith<A1> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1>> ();
    }

    public void With<A1, A2> (A1 a1, A2 a2)
    {
      GetDelegateWith<A1, A2> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2> GetDelegateWith<A1, A2> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2>> ();
    }

    public void With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      GetDelegateWith<A1, A2, A3> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3> GetDelegateWith<A1, A2, A3> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3>> ();
    }

    public void With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      GetDelegateWith<A1, A2, A3, A4> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4> GetDelegateWith<A1, A2, A3, A4> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4>> ();
    }

    public void With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      GetDelegateWith<A1, A2, A3, A4, A5> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5> GetDelegateWith<A1, A2, A3, A4, A5> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16>> ();
    }

    public void With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17)
    {
      GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17);
    }

    public Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> ()
    {
      return GetDelegate<Action<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17>> ();
    }
  }

}