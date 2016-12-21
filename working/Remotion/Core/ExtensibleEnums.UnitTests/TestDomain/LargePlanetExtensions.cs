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
using System.Reflection;

namespace Remotion.ExtensibleEnums.UnitTests.TestDomain
{
  public static class LargePlanetExtensions
  {
    [ExtensibleEnumPosition (4.0)]
    public static Planet Neptune (this ExtensibleEnumDefinition<Planet> definition)
    {
      return new Planet (MethodBase.GetCurrentMethod ());
    }

    [ExtensibleEnumPosition (2.0)]
    public static Planet Jupiter (this ExtensibleEnumDefinition<Planet> definition)
    {
      return new Planet (MethodBase.GetCurrentMethod ());
    }

    [ExtensibleEnumPosition (2.0)]
    public static Planet Saturn (this ExtensibleEnumDefinition<Planet> definition)
    {
      return new Planet (MethodBase.GetCurrentMethod ());
    }

    [ExtensibleEnumPosition (3.0)]
    public static Planet Uranus (this ExtensibleEnumDefinition<Planet> definition)
    {
      return new Planet (MethodBase.GetCurrentMethod ());
    }
  }
}