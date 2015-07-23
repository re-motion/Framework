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

namespace Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.Globalization.TestDomain
{
  public interface ITargetClassForGlobalizationViaAttributes
  {
    // Localization of interface properties is currently not supported
    //[MultiLingualName ("ImplicitImplementedProperty display name from interface ITargetClassForGlobalizationViaAttributes", "")]
    string ImplicitImplementedProperty { get; set; }

    // Localization of interface properties is currently not supported
    //[MultiLingualName ("ExplicitImplementedProperty display name from interface ITargetClassForGlobalizationViaAttributes", "")]
    string ExplicitImplementedProperty { get; set; }

    // Localization of interface properties is currently not supported
    //[MultiLingualName ("ImplicitImplementedPropertyForOverrideTarget display name from interface ITargetClassForGlobalizationViaAttributes", "")]
    string ImplicitImplementedPropertyForOverrideTarget { get; set; }
  }
}