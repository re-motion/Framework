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
  public struct FuncInvoker<TResult> : IFuncInvoker<TResult>
  {
    private readonly DelegateSelector _delegateSelector;



    public FuncInvoker (DelegateSelector delegateSelector)
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

    public TResult Invoke (Type[] valueTypes, object[] values)
    {
      InvokerUtility.CheckInvokeArguments (valueTypes, values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public TResult Invoke (object[] values)
    {
      Type[] valueTypes = InvokerUtility.GetValueTypes (values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public Delegate GetDelegate (params Type[] parameterTypes)
    {
      return GetDelegate (FuncUtility.MakeClosedType (typeof (TResult), parameterTypes));
    }

    public TDelegate GetDelegate<TDelegate> ()
    {
      return (TDelegate) (object) GetDelegate (typeof (TDelegate));
    }

    public Delegate GetDelegate (Type delegateType)
    {
      return _delegateSelector (delegateType);
    }

    public TResult With ()
    {
      return GetDelegateWith () ();
    }

    public Func<TResult> GetDelegateWith ()
    {
      return GetDelegate<Func<TResult>> ();
    }

    public TResult With<A1> (A1 a1)
    {
      return GetDelegateWith<A1> () (a1);
    }

    public Func<A1, TResult> GetDelegateWith<A1> ()
    {
      return GetDelegate<Func<A1, TResult>> ();
    }

    public TResult With<A1, A2> (A1 a1, A2 a2)
    {
      return GetDelegateWith<A1, A2> () (a1, a2);
    }

    public Func<A1, A2, TResult> GetDelegateWith<A1, A2> ()
    {
      return GetDelegate<Func<A1, A2, TResult>> ();
    }

    public TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      return GetDelegateWith<A1, A2, A3> () (a1, a2, a3);
    }

    public Func<A1, A2, A3, TResult> GetDelegateWith<A1, A2, A3> ()
    {
      return GetDelegate<Func<A1, A2, A3, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      return GetDelegateWith<A1, A2, A3, A4> () (a1, a2, a3, a4);
    }

    public Func<A1, A2, A3, A4, TResult> GetDelegateWith<A1, A2, A3, A4> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5> () (a1, a2, a3, a4, a5);
    }

    public Func<A1, A2, A3, A4, A5, TResult> GetDelegateWith<A1, A2, A3, A4, A5> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6> () (a1, a2, a3, a4, a5, a6);
    }

    public Func<A1, A2, A3, A4, A5, A6, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (a1, a2, a3, a4, a5, a6, a7);
    }

    public Func<A1, A2, A3, A4, A5, A6, A7, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> () (a1, a2, a3, a4, a5, a6, a7, a8);
    }

    public Func<A1, A2, A3, A4, A5, A6, A7, A8, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, A8, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> () (a1, a2, a3, a4, a5, a6, a7, a8, a9);
    }

    public Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
    }

    public Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
    }

    public Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
    }

    public Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
    }

    public Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
    }

    public Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15);
    }

    public Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);
    }

    public Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> () (a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17);
    }

    public Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> ()
    {
      return GetDelegate<Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, TResult>> ();
    }
  }

  public struct FuncInvoker<TFixedArg1, TResult> : IFuncInvoker<TResult>
  {
    private readonly DelegateSelector _delegateSelector;

    private readonly TFixedArg1 _fixedArg1;

    public FuncInvoker (DelegateSelector delegateSelector, TFixedArg1 fixedArg1)
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

    public TResult Invoke (Type[] valueTypes, object[] values)
    {
      InvokerUtility.CheckInvokeArguments (valueTypes, values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public TResult Invoke (object[] values)
    {
      Type[] valueTypes = InvokerUtility.GetValueTypes (values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public Delegate GetDelegate (params Type[] parameterTypes)
    {
      return GetDelegate (FuncUtility.MakeClosedType (typeof (TResult), parameterTypes));
    }

    public TDelegate GetDelegate<TDelegate> ()
    {
      return (TDelegate) (object) GetDelegate (typeof (TDelegate));
    }

    public Delegate GetDelegate (Type delegateType)
    {
      return _delegateSelector (delegateType);
    }

    public TResult With ()
    {
      return GetDelegateWith () (_fixedArg1);
    }

    public Func<TFixedArg1, TResult> GetDelegateWith ()
    {
      return GetDelegate<Func<TFixedArg1, TResult>> ();
    }

    public TResult With<A1> (A1 a1)
    {
      return GetDelegateWith<A1> () (_fixedArg1, a1);
    }

    public Func<TFixedArg1, A1, TResult> GetDelegateWith<A1> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, TResult>> ();
    }

    public TResult With<A1, A2> (A1 a1, A2 a2)
    {
      return GetDelegateWith<A1, A2> () (_fixedArg1, a1, a2);
    }

    public Func<TFixedArg1, A1, A2, TResult> GetDelegateWith<A1, A2> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, TResult>> ();
    }

    public TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      return GetDelegateWith<A1, A2, A3> () (_fixedArg1, a1, a2, a3);
    }

    public Func<TFixedArg1, A1, A2, A3, TResult> GetDelegateWith<A1, A2, A3> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      return GetDelegateWith<A1, A2, A3, A4> () (_fixedArg1, a1, a2, a3, a4);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, TResult> GetDelegateWith<A1, A2, A3, A4> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5> () (_fixedArg1, a1, a2, a3, a4, a5);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, TResult> GetDelegateWith<A1, A2, A3, A4, A5> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6> () (_fixedArg1, a1, a2, a3, a4, a5, a6);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> () (_fixedArg1, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17);
    }

    public Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> ()
    {
      return GetDelegate<Func<TFixedArg1, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, TResult>> ();
    }
  }

  public struct FuncInvoker<TFixedArg1, TFixedArg2, TResult> : IFuncInvoker<TResult>
  {
    private readonly DelegateSelector _delegateSelector;

    private readonly TFixedArg1 _fixedArg1;
    private readonly TFixedArg2 _fixedArg2;

    public FuncInvoker (DelegateSelector delegateSelector, TFixedArg1 fixedArg1, TFixedArg2 fixedArg2)
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

    public TResult Invoke (Type[] valueTypes, object[] values)
    {
      InvokerUtility.CheckInvokeArguments (valueTypes, values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public TResult Invoke (object[] values)
    {
      Type[] valueTypes = InvokerUtility.GetValueTypes (values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public Delegate GetDelegate (params Type[] parameterTypes)
    {
      return GetDelegate (FuncUtility.MakeClosedType (typeof (TResult), parameterTypes));
    }

    public TDelegate GetDelegate<TDelegate> ()
    {
      return (TDelegate) (object) GetDelegate (typeof (TDelegate));
    }

    public Delegate GetDelegate (Type delegateType)
    {
      return _delegateSelector (delegateType);
    }

    public TResult With ()
    {
      return GetDelegateWith () (_fixedArg1, _fixedArg2);
    }

    public Func<TFixedArg1, TFixedArg2, TResult> GetDelegateWith ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TResult>> ();
    }

    public TResult With<A1> (A1 a1)
    {
      return GetDelegateWith<A1> () (_fixedArg1, _fixedArg2, a1);
    }

    public Func<TFixedArg1, TFixedArg2, A1, TResult> GetDelegateWith<A1> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, TResult>> ();
    }

    public TResult With<A1, A2> (A1 a1, A2 a2)
    {
      return GetDelegateWith<A1, A2> () (_fixedArg1, _fixedArg2, a1, a2);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, TResult> GetDelegateWith<A1, A2> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, TResult>> ();
    }

    public TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      return GetDelegateWith<A1, A2, A3> () (_fixedArg1, _fixedArg2, a1, a2, a3);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, TResult> GetDelegateWith<A1, A2, A3> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      return GetDelegateWith<A1, A2, A3, A4> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, TResult> GetDelegateWith<A1, A2, A3, A4> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, TResult> GetDelegateWith<A1, A2, A3, A4, A5> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> () (_fixedArg1, _fixedArg2, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17);
    }

    public Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, TResult>> ();
    }
  }

  public struct FuncInvoker<TFixedArg1, TFixedArg2, TFixedArg3, TResult> : IFuncInvoker<TResult>
  {
    private readonly DelegateSelector _delegateSelector;

    private readonly TFixedArg1 _fixedArg1;
    private readonly TFixedArg2 _fixedArg2;
    private readonly TFixedArg3 _fixedArg3;

    public FuncInvoker (DelegateSelector delegateSelector, TFixedArg1 fixedArg1, TFixedArg2 fixedArg2, TFixedArg3 fixedArg3)
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

    public TResult Invoke (Type[] valueTypes, object[] values)
    {
      InvokerUtility.CheckInvokeArguments (valueTypes, values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public TResult Invoke (object[] values)
    {
      Type[] valueTypes = InvokerUtility.GetValueTypes (values);
      return (TResult) GetDelegate (GetValueTypes (valueTypes)).DynamicInvoke (GetValues (values));
    }

    public Delegate GetDelegate (params Type[] parameterTypes)
    {
      return GetDelegate (FuncUtility.MakeClosedType (typeof (TResult), parameterTypes));
    }

    public TDelegate GetDelegate<TDelegate> ()
    {
      return (TDelegate) (object) GetDelegate (typeof (TDelegate));
    }

    public Delegate GetDelegate (Type delegateType)
    {
      return _delegateSelector (delegateType);
    }

    public TResult With ()
    {
      return GetDelegateWith () (_fixedArg1, _fixedArg2, _fixedArg3);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, TResult> GetDelegateWith ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, TResult>> ();
    }

    public TResult With<A1> (A1 a1)
    {
      return GetDelegateWith<A1> () (_fixedArg1, _fixedArg2, _fixedArg3, a1);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, TResult> GetDelegateWith<A1> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, TResult>> ();
    }

    public TResult With<A1, A2> (A1 a1, A2 a2)
    {
      return GetDelegateWith<A1, A2> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, TResult> GetDelegateWith<A1, A2> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, TResult>> ();
    }

    public TResult With<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      return GetDelegateWith<A1, A2, A3> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, TResult> GetDelegateWith<A1, A2, A3> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      return GetDelegateWith<A1, A2, A3, A4> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, TResult> GetDelegateWith<A1, A2, A3, A4> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, TResult> GetDelegateWith<A1, A2, A3, A4, A5> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, TResult>> ();
    }

    public TResult With<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8, A9 a9, A10 a10, A11 a11, A12 a12, A13 a13, A14 a14, A15 a15, A16 a16, A17 a17)
    {
      return GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> () (_fixedArg1, _fixedArg2, _fixedArg3, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17);
    }

    public Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, TResult> GetDelegateWith<A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17> ()
    {
      return GetDelegate<Func<TFixedArg1, TFixedArg2, TFixedArg3, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, TResult>> ();
    }
  }

}