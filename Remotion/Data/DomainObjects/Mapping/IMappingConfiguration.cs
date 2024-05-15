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
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Defines an interface for classes holding mapping configuration metadata.
  /// </summary>
  public interface IMappingConfiguration
  {
    ClassDefinition[] GetTypeDefinitions ();
    bool ContainsTypeDefinition (Type classType);
    ClassDefinition GetTypeDefinition (Type classType);
    ClassDefinition GetTypeDefinition (Type classType, Func<Type, Exception> missingTypeDefinitionExceptionFactory);
    bool ContainsClassDefinition (string classID);
    ClassDefinition GetClassDefinition (string classID);
    ClassDefinition GetClassDefinition (string classID, Func<string, Exception> missingClassDefinitionExceptionFactory);

    bool ResolveTypes { get; }

    IMemberInformationNameResolver NameResolver { get; }
    TupleDefinition[] GetTupleDefinitions ();
    bool ContainsTupleDefinition (Type tupleType);
    TupleDefinition GetTupleDefinition (Type tupleType);
    TupleDefinition GetTupleDefinition (Type tupleType, Func<Type, Exception> missingTypeDefinitionExceptionFactory);
    bool ContainsTupleDefinition (string tupleID);
    TupleDefinition GetTupleDefinition (string tupleID);
    TupleDefinition GetTupleDefinition (string tupleID, Func<string, Exception> missingClassDefinitionExceptionFactory);
  }
}
