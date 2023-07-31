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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation;
using Remotion.Validation.Results;

namespace Remotion.Data.DomainObjects.Validation
{
  public class ValidationClientTransactionExtension : ClientTransactionExtensionBase
  {
    private readonly IValidatorProvider _validatorProvider;

    public static string DefaultKey
    {
      get { return typeof(ValidationClientTransactionExtension).GetFullNameChecked(); }
    }

    public ValidationClientTransactionExtension (IValidatorProvider validatorProvider)
        : this(DefaultKey, validatorProvider)
    {
    }

    public ValidationClientTransactionExtension (string key, IValidatorProvider validatorProvider)
        : base(key)
    {
      ArgumentUtility.CheckNotNull("validatorProvider", validatorProvider);

      _validatorProvider = validatorProvider;
    }

    public IValidatorProvider ValidatorProvider
    {
      get { return _validatorProvider; }
    }

    public override void CommitValidate (ClientTransaction clientTransaction, IReadOnlyList<PersistableData> committedData)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("committedData", committedData);

      var validatorCache = new Dictionary<Type, IValidator>();

      var validationResults = Validate(committedData, validatorCache);
      if (!validationResults.Any())
        return;

      var validationFailures = validationResults.SelectMany(vr => vr.Errors).ToArray();
      var invalidDomainObjects = validationFailures.Select(err => err.ValidatedObject).Distinct().Cast<DomainObject>().ToArray();
      var invariantErrorMessage = BuildErrorMessage(validationFailures);

      throw new ExtendedDomainObjectValidationException(
          invalidDomainObjects,
          validationFailures,
          invariantErrorMessage);
    }

    private static string BuildErrorMessage (IEnumerable<ValidationFailure> errors)
    {
      var errorsByValidatedObjects = errors.ToLookup(e => e.ValidatedObject);

      var errorMessage = new StringBuilder("One or more DomainObject contain inconsistent data:");
      errorMessage.AppendLine();
      foreach (var errorByValidatedObject in errorsByValidatedObjects)
      {
        errorMessage.AppendLine();
        AppendValidatedObject(errorMessage, errorByValidatedObject.Key);
        errorMessage.AppendLine();
        errorByValidatedObject
            .SelectMany(f => f.ValidatedProperties.DefaultIfEmpty().Select(p => (f.ValidatedObject, f.ErrorMessage, ValidatedProperty: p)))
            .OrderBy(f => f.ValidatedProperty == null ? 2 : f.ValidatedProperty.Object == f.ValidatedObject ? 1 : 0)
            .ThenBy(f => f.ValidatedProperty?.Property.Name)
            .Aggregate(errorMessage, (sb, f) => AppendErrorMessage(sb, f.ValidatedObject, f.ErrorMessage, f.ValidatedProperty));
      }
      return errorMessage.ToString();
    }

    private static void AppendValidatedObject (StringBuilder errorMessage, object validatedInstance)
    {
      if (validatedInstance is DomainObject domainObject)
        errorMessage.AppendFormat("Object '{0}' with ID '{1}':", domainObject.ID.ClassID, domainObject.ID.Value);
      else
        errorMessage.AppendFormat("Validation error on object of Type '{0}':", validatedInstance.GetType().GetFullNameSafe());
    }

    private static StringBuilder AppendErrorMessage (StringBuilder stringBuilder, object validatedObject, string errorMessage, ValidatedProperty? validatedProperty)
    {
      stringBuilder.Append(" -- ");

      if (validatedProperty == null)
      {
        // NOP
      }
      else if (validatedProperty.Object == validatedObject)
      {
        stringBuilder
            .Append(validatedProperty.Property.Name)
            .Append(": ");
      }
      else if (validatedProperty.Object is DomainObject dependentDomainObject)
      {
        stringBuilder
            .Append(validatedProperty.Property.Name)
            .Append(" on dependent object '").Append(dependentDomainObject.ID.ClassID).Append("' with ID '").Append(dependentDomainObject.ID.Value)
            .Append("': ");
      }
      else
      {
        stringBuilder
            .Append(validatedProperty.Property.Name)
            .Append(" on dependent object of Type '").Append(validatedProperty.Object.GetType().GetFullNameSafe())
            .Append("': ");
      }

      stringBuilder.Append(errorMessage);
      stringBuilder.AppendLine();

      return stringBuilder;
    }

    private List<ValidationResult> Validate (IReadOnlyList<PersistableData> domainObjectsToValidate, Dictionary<Type, IValidator> validatorCache)
    {
      ArgumentUtility.CheckNotNull("domainObjectsToValidate", domainObjectsToValidate);

      var invalidValidationResults = new List<ValidationResult>();

      foreach (var item in domainObjectsToValidate)
      {
        if (item.DomainObjectState.IsDeleted)
          continue;

        Assertion.IsFalse(item.DomainObjectState.IsNotLoadedYet, "No unloaded objects get this far.");
        Assertion.IsFalse(item.DomainObjectState.IsInvalid, "No invalid objects get this far.");

        var validator = _validatorProvider.GetValidator(item.DomainObject.GetPublicDomainObjectType());

        var validationResult = validator.Validate(item.DomainObject);

        if (!validationResult.IsValid)
          invalidValidationResults.Add(validationResult);
      }
      return invalidValidationResults;
    }
  }
}
