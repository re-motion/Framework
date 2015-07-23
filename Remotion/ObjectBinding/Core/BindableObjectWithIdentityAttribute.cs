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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Apply to a class to add a reflection-based implementation of <see cref="IBusinessObjectWithIdentity"/> to the class via 
  /// <see cref="BindableObjectWithIdentityMixin"/>. Use this attribute if deriving from <see cref="BindableObjectWithIdentityBase"/> is not possible.
  /// </summary>
  /// <remarks>This attribute adds the <see cref="BindableObjectWithIdentityMixin"/> to its target class. 
  /// Use <see cref="ObjectFactory"/> to instantiate the target class.</remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class BindableObjectWithIdentityAttribute : UsesAttribute
  {
    public BindableObjectWithIdentityAttribute ()
        : base (typeof (BindableObjectWithIdentityMixin))
    {
    }
  }
}
