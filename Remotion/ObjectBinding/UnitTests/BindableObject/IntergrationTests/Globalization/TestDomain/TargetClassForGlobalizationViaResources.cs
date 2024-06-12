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

namespace Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.Globalization.TestDomain
{
  [BindableObject]
  [MultiLingualResources("Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.Globalization.Resources.TargetClassForGlobalizationViaResources")]
  public class TargetClassForGlobalizationViaResources : ITargetClassForGlobalizationViaResources
  {
    public TargetClassForGlobalizationViaResources ()
    {
    }

    public string Property1 { get; set; }

    public string Property2 { get; set; }

    public string PropertyForMixinOverrideTest
    {
      get { return Property1; }
      set { Property1 = value; }
    }

    public string ImplicitImplementedProperty { get; set; }

    string ITargetClassForGlobalizationViaResources.ExplicitImplementedProperty { get; set; }
  }
}
