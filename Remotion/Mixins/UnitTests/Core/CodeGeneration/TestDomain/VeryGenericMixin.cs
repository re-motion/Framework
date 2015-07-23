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
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain
{
  public interface IVeryGenericMixin
  {
    string GetMessage<T>(T t);
  }

  public interface IVeryGenericMixin<T1, T2>
  {
    string GenericIfcMethod<T3> (T1 t1, T2 t2, T3 t3);
  }

  public class VeryGenericMixin<[BindToTargetType] TTarget, [BindToConstraints] TNext> : Mixin<TTarget, TNext>, IVeryGenericMixin<TTarget, TNext>, IVeryGenericMixin
    where TTarget : class
    where TNext : class
  {
    public string GenericIfcMethod<T3> (TTarget t1, TNext t2, T3 t3)
    {
      return "IVeryGenericMixin.GenericIfcMethod-" + t3;
    }

    public string GetMessage<T> (T t)
    {
      return GenericIfcMethod (Target, Next, t);
    }
  }

  public interface IUltraGenericMixin
  {
    string GetMessage<T> (T t);
  }

  public interface IADUGMTargetCallDependencies : IBaseType31, IBaseType32, IBT3Mixin4 {}
  public interface IADUGMNextCallDependencies : IBaseType31, IBaseType32, IBT3Mixin4 {}

  public abstract class AbstractDerivedUltraGenericMixin<[BindToConstraints] TTarget, [BindToConstraints] TNext> : VeryGenericMixin<TTarget, TNext>, IUltraGenericMixin
    where TTarget : class, IADUGMTargetCallDependencies
    where TNext : class, IADUGMNextCallDependencies
  {
    protected abstract string AbstractGenericMethod<T>();

    public new string GetMessage<T> (T t)
    {
      return AbstractGenericMethod<T>() + "-" + base.GenericIfcMethod (Target, Next, t);
    }
  }

  [Uses (typeof (AbstractDerivedUltraGenericMixin<,>), AdditionalDependencies = new Type[] { typeof (IBT3Mixin6) })]
  [Uses (typeof (BT3Mixin4))]
  public class ClassOverridingUltraGenericStuff : BaseType3
  {
    [OverrideMixin]
    public string AbstractGenericMethod<T> ()
    {
      return typeof (T).Name;
    }
  }
}
