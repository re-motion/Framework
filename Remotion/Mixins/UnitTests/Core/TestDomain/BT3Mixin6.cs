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

namespace Remotion.Mixins.UnitTests.Core.TestDomain
{
  public interface IBT3Mixin6TargetCallDependencies : IBaseType31, IBaseType32, IBaseType33, IBT3Mixin4
  {
  }

  public interface IBT3Mixin6NextCallDependencies : IBaseType34, IBT3Mixin4
  {
  }

  public interface IBT3Mixin6 { }

  [Extends(typeof(BaseType3))]
  public class BT3Mixin6<[BindToConstraints] TTarget, [BindToConstraints]TNext> : Mixin<TTarget, TNext>, IBT3Mixin6
      where TTarget : class, IBT3Mixin6TargetCallDependencies
      where TNext : class, IBT3Mixin6NextCallDependencies
  {
  }
}
