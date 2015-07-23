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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
  public class MixinAddingPersistentProperties : DomainObjectMixin<BindableDomainObjectWithMixedPersistentProperties>, IMixinAddingPersistentProperties
  {
    public DateTime MixedProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "MixedProperty"].GetValue<DateTime>(); }
      set { Properties[typeof (MixinAddingPersistentProperties), "MixedProperty"].SetValue (value); }
    }

    [MemberVisibility (MemberVisibility.Public)]
    public DateTime PublicMixedProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "PublicMixedProperty"].GetValue<DateTime> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), "PublicMixedProperty"].SetValue (value); }
    }

    [MemberVisibility (MemberVisibility.Private)]
    public DateTime PrivateMixedProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "PrivateMixedProperty"].GetValue<DateTime> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), "PrivateMixedProperty"].SetValue (value); }
    }

    [DBColumn ("ExplicitMixedProperty")]
    [StorageClass(StorageClass.Persistent)]
    DateTime IMixinAddingPersistentProperties.ExplicitMixedProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), typeof (IMixinAddingPersistentProperties).FullName + ".ExplicitMixedProperty"].GetValue<DateTime> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), typeof (IMixinAddingPersistentProperties).FullName + ".ExplicitMixedProperty"].SetValue (value); }
    }
  }
}
