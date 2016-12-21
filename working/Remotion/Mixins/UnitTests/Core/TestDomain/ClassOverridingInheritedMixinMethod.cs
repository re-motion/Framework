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
  [Uses (typeof (MixinWithInheritedMethod))]
  public class ClassOverridingInheritedMixinMethod
  {
    [OverrideMixin]
    public string ProtectedInheritedMethod ()
    {
      return "ClassOverridingInheritedMixinMethod.ProtectedInheritedMethod";
    }

    [OverrideMixin]
    public string ProtectedInternalInheritedMethod ()
    {
      return "ClassOverridingInheritedMixinMethod.ProtectedInternalInheritedMethod";
    }

    [OverrideMixin]
    public string PublicInheritedMethod ()
    {
      return "ClassOverridingInheritedMixinMethod.PublicInheritedMethod";
    }
  }

  public class BaseMixinWithInheritedMethod : Mixin<object>
  {
    protected virtual string ProtectedInheritedMethod ()
    {
      return "BaseMixinWithInheritedMethod.ProtectedInheritedMethod";
    }

    protected internal virtual string ProtectedInternalInheritedMethod ()
    {
      return "BaseMixinWithInheritedMethod.ProtectedInternalInheritedMethod";
    }

    public virtual string PublicInheritedMethod ()
    {
      return "BaseMixinWithInheritedMethod.PublicInheritedMethod";
    }
  }

  public class MixinWithInheritedMethod : BaseMixinWithInheritedMethod
  {
    public string InvokeInheritedMethods ()
    {
      return ProtectedInheritedMethod () + "-" + ProtectedInternalInheritedMethod() + "-" + PublicInheritedMethod();
    }
  }
}
