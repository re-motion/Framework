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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Providers;

namespace Remotion.Validation.UnitTests.Providers
{
  public class TestableValidationAttributesBasedCollectorProvider : ValidationAttributesBasedCollectorProvider
  {
    public new ILookup<Type, IAttributesBasedValidationPropertyRuleReflector> CreatePropertyRuleReflectors (IEnumerable<Type> types)
    {
      return
          types.SelectMany (t => t.GetProperties (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
              .Select (p => new { Type = p.DeclaringType, Property = p })
              .Select (
                  t =>
                      new Tuple<Type, IAttributesBasedValidationPropertyRuleReflector> (
                          t.Type,
                          new ValidationAttributesBasedPropertyRuleReflector (t.Property)))
              .ToLookup (c => c.Item1, c => c.Item2);
    }
  }
}