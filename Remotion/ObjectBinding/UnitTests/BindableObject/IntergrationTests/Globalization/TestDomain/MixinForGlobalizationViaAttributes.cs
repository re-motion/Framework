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
using Remotion.Globalization;
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.Globalization.TestDomain
{
  public class MixinForGlobalizationViaAttributes : Mixin<MixinForGlobalizationViaAttributes.ITarget>, IMixinForGlobalizationViaAttributes
  {
    public interface ITarget
    {
      string PropertyForOverrideTarget { get; set; }
      string ImplicitImplementedPropertyForOverrideTarget { get; set; }
    }

    [MultiLingualName ("MixedProperty1 display name from MixinForGlobalizationViaAttributes", "")]
    public string MixedProperty1 { get; set; }

    [MultiLingualName ("MixedProperty2 display name from MixinForGlobalizationViaAttributes", "")]
    public string MixedProperty2 { get; set; }

    [MultiLingualName ("MixedExplicitProperty display name from MixinForGlobalizationViaAttributes", "")]
    string IMixinForGlobalizationViaAttributes.MixedExplicitProperty { get; set; }

    [MultiLingualName ("PropertyForOverrideMixin display name from MixinForGlobalizationViaAttributes", "")]
    public virtual string PropertyForOverrideMixin { get; set; }

    [OverrideTarget]
    protected string PropertyForOverrideTarget { get; set; }

    [OverrideTarget]
    protected string ImplicitImplementedPropertyForOverrideTarget { get; set; }
  }
}