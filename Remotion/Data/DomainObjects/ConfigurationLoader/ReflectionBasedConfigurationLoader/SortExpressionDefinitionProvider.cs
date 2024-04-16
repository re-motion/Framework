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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Default implementation of the <see cref="ISortExpressionDefinitionProvider"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(ISortExpressionDefinitionProvider), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  public class SortExpressionDefinitionProvider : ISortExpressionDefinitionProvider
  {
    public SortExpressionDefinitionProvider ()
    {
    }

    public SortExpressionDefinition? GetSortExpression (
        IPropertyInformation propertyInfo,
        TypeDefinition referencedTypeDefinition,
        string? sortExpressionText)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull("referencedTypeDefinition", referencedTypeDefinition);

      if (sortExpressionText == null)
        return null;

      try
      {
        var parser = new SortExpressionParser(referencedTypeDefinition);
        return parser.Parse(sortExpressionText);
      }
      catch (MappingException ex)
      {
        var result = MappingValidationResult.CreateInvalidResultForProperty(propertyInfo, ex.Message);
        Assertion.DebugIsNotNull(result.Message, "result.Message != null");
        throw new MappingException(result.Message, ex);
      }
    }
  }
}
