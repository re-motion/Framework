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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Utilities;
using Remotion.Validation;
using Remotion.Validation.Results;
using Remotion.Validation.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  public class ValidationClientTransactionExtension : ClientTransactionExtensionBase
  {
    private readonly IValidatorBuilder _validatorBuilder;
    private readonly Func<Type, IValidator> _validatorBuilderFunc;

    public static string DefaultKey
    {
      get { return typeof (ValidationClientTransactionExtension).FullName; }
    }

    public ValidationClientTransactionExtension (IValidatorBuilder validatorBuilder)
        : this (DefaultKey, validatorBuilder)
    {
    }

    public ValidationClientTransactionExtension (string key, IValidatorBuilder validatorBuilder)
        : base (key)
    {
      ArgumentUtility.CheckNotNull ("validatorBuilder", validatorBuilder);

      _validatorBuilder = validatorBuilder;

      // Optimized for memory allocations
      _validatorBuilderFunc = _validatorBuilder.BuildValidator;
    }

    public IValidatorBuilder ValidatorBuilder
    {
      get { return _validatorBuilder; }
    }

    public override void CommitValidate (ClientTransaction clientTransaction, ReadOnlyCollection<PersistableData> committedData)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("committedData", committedData);

      var validatorCache = new Dictionary<Type, IValidator>();

      List<ValidationResult> invariantCultureInvalidValidationResults;
      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        invariantCultureInvalidValidationResults = Validate (committedData, validatorCache);
      }

      if (!invariantCultureInvalidValidationResults.Any())
        return;

      var invalidDomainObjects =
          invariantCultureInvalidValidationResults.SelectMany (vr => vr.Errors)
              .Select (err => err.GetValidatedInstance())
              .Distinct()
              .Cast<DomainObject>();

      var objectsToRevalidate = committedData.Where (cd => invalidDomainObjects.Contains (cd.DomainObject)).ToList().AsReadOnly();
      var localizedInvalidValidationResults = Validate (objectsToRevalidate, validatorCache);
      var invariantErrorMessage = BuildErrorMesage (invariantCultureInvalidValidationResults.SelectMany (vr => vr.Errors));

      throw new ExtendedDomainObjectValidationException (
          invalidDomainObjects,
          localizedInvalidValidationResults.SelectMany (e => e.Errors),
          invariantErrorMessage);
    }

    private static string BuildErrorMesage (IEnumerable<ValidationFailure> errors)
    {
      var errorsByValidatedObjects = errors.ToLookup (e => e.GetValidatedInstance ());

      var errorMessage = new StringBuilder ("One or more DomainObject contain inconsistent data:\r\n\r\n");
      foreach (var errorByValidatedObject in errorsByValidatedObjects)
      {
        errorMessage.AppendLine (GetKeyText (errorByValidatedObject.Key));
        errorMessage.AppendLine (
            string.Join (
                "\r\n",
                errorByValidatedObject.Select (t => " -- " + t.Property.Name + ": " + t.ErrorMessage)));
        errorMessage.AppendLine ();
      }
      return errorMessage.ToString ();
    }

    private static string GetKeyText (object validatedInstance)
    {
      var domainObject = validatedInstance as DomainObject;
      if (domainObject != null)
        return string.Format ("Object '{0}' with ID '{1}':", domainObject.ID.ClassID, domainObject.ID.Value);
      return string.Format ("Validation error on object of Type '{0}':", validatedInstance.GetType ().FullName);
    }

    private List<ValidationResult> Validate (ReadOnlyCollection<PersistableData> domainObjectsToValidate, Dictionary<Type, IValidator> validatorCache)
    {
      ArgumentUtility.CheckNotNull ("domainObjectsToValidate", domainObjectsToValidate);

      var invalidValidationResults = new List<ValidationResult> ();

      foreach (var item in domainObjectsToValidate)
      {
        if (item.DomainObjectState == StateType.Deleted)
          continue;

        Assertion.IsTrue (
            item.DomainObjectState != StateType.NotLoadedYet && item.DomainObjectState != StateType.Invalid,
            "No unloaded or invalid objects get this far.");

        var validator = validatorCache.GetOrCreateValue (item.DomainObject.GetPublicDomainObjectType(), _validatorBuilderFunc);
        
        var validationResult = validator.Validate (item.DomainObject);

        // C# compiler 7.2 already provides caching for anonymous method.
        foreach (var validationFailure in validationResult.Errors.Where (vr => vr.GetValidatedInstance () == null))
          validationFailure.SetValidatedInstance (item.DomainObject);

        if (!validationResult.IsValid)
          invalidValidationResults.Add (validationResult);
      }
      return invalidValidationResults;
    }
  }
}