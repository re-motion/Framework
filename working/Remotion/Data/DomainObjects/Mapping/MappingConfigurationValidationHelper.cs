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
using System.Text;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Provides functionality helping with validation of the different parts of a <see cref="MappingConfiguration"/>.
  /// </summary>
  public class MappingConfigurationValidationHelper
  {
    private readonly IMappingValidatorFactory _mappingValidatorFactory;
    private readonly IPersistenceModelValidatorFactory _persistenceModelValidatorFactory;

    public MappingConfigurationValidationHelper (
        IMappingValidatorFactory mappingValidatorFactory, 
        IPersistenceModelValidatorFactory persistenceModelValidatorFactory)
    {
      ArgumentUtility.CheckNotNull ("mappingValidatorFactory", mappingValidatorFactory);
      ArgumentUtility.CheckNotNull ("persistenceModelValidatorFactory", persistenceModelValidatorFactory);
      
      _mappingValidatorFactory = mappingValidatorFactory;
      _persistenceModelValidatorFactory = persistenceModelValidatorFactory;
    }

    public void VerifyPersistenceModelApplied (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.StorageEntityDefinition == null)
      {
        var message = String.Format ("The persistence model loader did not assign a storage entity to class '{0}'.", classDefinition.ID);
        throw new InvalidOperationException (message);
      }

      foreach (PropertyDefinition propDef in classDefinition.MyPropertyDefinitions)
      {
        if (propDef.StorageClass == StorageClass.Persistent && propDef.StoragePropertyDefinition == null)
        {
          var message = String.Format (
              "The persistence model loader did not assign a storage property to property '{0}' of class '{1}'.",
              propDef.PropertyName,
              classDefinition.ID);
          throw new InvalidOperationException (message);
        }
      }

      foreach (var derivedClass in classDefinition.DerivedClasses)
        VerifyPersistenceModelApplied (derivedClass);
    }

    public void ValidateClassDefinitions (IEnumerable<ClassDefinition> typeDefinitions)
    {
      ArgumentUtility.CheckNotNull ("typeDefinitions", typeDefinitions);

      var typeDefinitionValidator = _mappingValidatorFactory.CreateClassDefinitionValidator();
      AnalyzeMappingValidationResults (typeDefinitionValidator.Validate (typeDefinitions));
    }

    public void ValidatePropertyDefinitions (IEnumerable<ClassDefinition> typeDefinitions)
    {
      ArgumentUtility.CheckNotNull ("typeDefinitions", typeDefinitions);

      var propertyDefinitionValidator = _mappingValidatorFactory.CreatePropertyDefinitionValidator();
      AnalyzeMappingValidationResults (propertyDefinitionValidator.Validate (typeDefinitions));
    }

    public void ValidateRelationDefinitions (IEnumerable<RelationDefinition> relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      var relationDefinitionValidator = _mappingValidatorFactory.CreateRelationDefinitionValidator();
      AnalyzeMappingValidationResults (relationDefinitionValidator.Validate (relationDefinitions));
    }

    public void ValidateSortExpression (IEnumerable<RelationDefinition> relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      var sortExpressionValidator = _mappingValidatorFactory.CreateSortExpressionValidator();
      AnalyzeMappingValidationResults (sortExpressionValidator.Validate (relationDefinitions));
    }

    public void ValidatePersistenceMapping (ClassDefinition rootClass)
    {
      ArgumentUtility.CheckNotNull ("rootClass", rootClass);

      var validator = _persistenceModelValidatorFactory.CreatePersistenceMappingValidator (rootClass);
      var classDefinitionsToValidate = new[] { rootClass }.Concat (rootClass.GetAllDerivedClasses());
      AnalyzeMappingValidationResults (validator.Validate (classDefinitionsToValidate));
    }

    public void AnalyzeMappingValidationResults (IEnumerable<MappingValidationResult> mappingValidationResults)
    {
      ArgumentUtility.CheckNotNull ("mappingValidationResults", mappingValidationResults);

      var mappingValidationResultsArray = mappingValidationResults.ToArray();
      if (mappingValidationResultsArray.Any())
        throw CreateMappingException (mappingValidationResultsArray);
    }

    public void ValidateDuplicateClassIDs (IEnumerable<ClassDefinition> classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      var duplicateGroups = from cd in classDefinitions
                            group cd by cd.ID
                            into cdGroups
                            where cdGroups.Count () > 1
                            select cdGroups;
      foreach (var duplicateGroup in duplicateGroups)
      {
        var duplicates = duplicateGroup.ToArray ();
        throw CreateMappingException (
            "Class '{0}' and '{1}' both have the same class ID '{2}'. Use the ClassIDAttribute to define unique IDs for these "
            + "classes. The assemblies involved are '{3}' and '{4}'.",
            duplicates[0].ClassType.FullName,
            duplicates[1].ClassType.FullName,
            duplicates[0].ID,
            duplicates[0].ClassType.Assembly.FullName,
            duplicates[1].ClassType.Assembly.FullName);
      }
    }

    private MappingException CreateMappingException (IEnumerable<MappingValidationResult> mappingValidationResults)
    {
      var messages = new StringBuilder ();
      foreach (var validationResult in mappingValidationResults)
      {
        if (messages.Length > 0)
          messages.AppendLine (new string ('-', 10));
        messages.AppendLine (validationResult.Message);
      }

      return CreateMappingException (messages.ToString ().Trim ());
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (String.Format (message, args));
    }
  }
}