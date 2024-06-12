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

namespace Remotion.ObjectBinding.UnitTests.TestDomain
{
  public interface IMixinAddingProperty
  {
    string MixedProperty { get; set; }
    string MixedPropertyWithoutLocalization { get; set; }
    string MixedPropertyWithShortNameInLocalization { get; set; }
    string MixedReadOnlyPropertyHavingSetterOnMixin { get; }
    string MixedReadOnlyProperty { get; }
    string ExplicitMixedProperty { get; set; }
    string ExplicitMixedPropertyWithShortNameInLocalization { get; set; }
  }

  public class MixinAddingProperty : BaseOfMixinAddingProperty, IMixinAddingProperty
  {
    private string _mixedProperty;
    private string _mixedPropertyWithShortNameInLocalization;
    private string _mixedPropertyWithoutLocalization;

    public string MixedProperty
    {
      get { return _mixedProperty; }
      set { _mixedProperty = value; }
    }

    public string MixedPropertyWithShortNameInLocalization
    {
      get { return _mixedPropertyWithShortNameInLocalization; }
      set { _mixedPropertyWithShortNameInLocalization = value; }
    }

    public string MixedPropertyWithoutLocalization
    {
      get { return _mixedPropertyWithoutLocalization; }
      set { _mixedPropertyWithoutLocalization = value; }
    }

    public string MixedReadOnlyProperty
    {
      get { return _mixedProperty; }
    }

    public string MixedReadOnlyPropertyHavingSetterOnMixin
    {
      get { return _mixedProperty; }
      set { _mixedProperty = value; }
    }

    string IMixinAddingProperty.ExplicitMixedProperty
    {
      get { return _mixedProperty; }
      set { _mixedProperty = value; }
    }

    string IMixinAddingProperty.ExplicitMixedPropertyWithShortNameInLocalization
    {
      get { return _mixedPropertyWithShortNameInLocalization; }
      set { _mixedPropertyWithShortNameInLocalization = value; }
    }
  }
}
