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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public static class CompoundStoragePropertyDefinitionObjectMother
  {
    public static CompoundStoragePropertyDefinition CreateWithTwoProperties()
    {
      return new CompoundStoragePropertyDefinition(
          typeof (Tuple<int, string>),
          new[]
          {
              new CompoundStoragePropertyDefinition.NestedPropertyInfo (
                  SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("one"), t => ((Tuple<int, string>) t).Item1), 
              new CompoundStoragePropertyDefinition.NestedPropertyInfo (
                  SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("two"), t => ((Tuple<int, string>) t).Item2), 
          },
          values => Tuple.Create ((int) values[0], (string) values[1]));
    }
  }
}