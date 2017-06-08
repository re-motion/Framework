// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  public static class BindableObjectProviderTestHelper
  {
    /// <summary>
    /// Use this method as a shortcut to retrieve the <see cref="BindableObjectClass"/> for a <see cref="Type"/> 
    /// that has the <see cref="BindableObjectMixinBase{T}"/> applied or is derived from a bindable object base class without first retrieving the 
    /// matching provider.
    /// </summary>
    /// <param name="type">The type to get a <see cref="BindableObjectClass"/> for. This type must have a mixin derived from
    /// <see cref="BindableObjectMixinBase{TBindableObject}"/> configured or it must be derived from a bindable object class class, and it is 
    /// recommended to specify the simple target type rather then the generated mixed type.</param>
    /// <returns>Returns the <see cref="BindableObjectClass"/> for the <paramref name="type"/>.</returns>
    public static BindableObjectClass GetBindableObjectClass (Type type)
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (type);
      return provider.GetBindableObjectClass (type);
    }
  }
}