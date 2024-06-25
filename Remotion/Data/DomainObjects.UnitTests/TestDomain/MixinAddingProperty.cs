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

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  public interface IMixinAddingProperty
  {
    string MixedProperty { get; set; }
    string MixedReadOnlyProperty { get; }
    string ExplicitMixedProperty { get; set; }
  }

  public class MixinAddingProperty : DomainObjectMixin<DomainObject>, IMixinAddingProperty
  {
    private string _mixedProperty;

    public string MixedProperty
    {
      get { return _mixedProperty; }
      set { _mixedProperty = value; }
    }

    public string MixedReadOnlyProperty
    {
      get { return _mixedProperty; }
    }

    string IMixinAddingProperty.ExplicitMixedProperty
    {
      get { return _mixedProperty; }
      set { _mixedProperty = value; }
    }
  }
}
