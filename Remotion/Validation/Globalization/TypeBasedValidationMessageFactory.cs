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
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Globalization
{
  [ImplementationFor(typeof(IValidationMessageFactory), Position = Position, Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class TypeBasedValidationMessageFactory : IValidationMessageFactory
  {
    public const int Position = LocalizedValidationMessageFactory.Position - 1;

    private readonly IMemberInformationGlobalizationService _memberInformationGlobalizationService;

    public TypeBasedValidationMessageFactory (IMemberInformationGlobalizationService memberInformationGlobalizationService)
    {
      ArgumentUtility.CheckNotNull("memberInformationGlobalizationService", memberInformationGlobalizationService);

      _memberInformationGlobalizationService = memberInformationGlobalizationService;
    }

    public ValidationMessage? CreateValidationMessageForPropertyValidator (IPropertyValidator validator, IPropertyInformation validatedProperty)
    {
      ArgumentUtility.CheckNotNull("validator", validator);
      ArgumentUtility.CheckNotNull("validatedProperty", validatedProperty);

      var typeInformation = TypeAdapter.Create(validator.GetType());
      var typeInformationForResourceResolution = typeInformation;

      if (!_memberInformationGlobalizationService.ContainsTypeDisplayName(typeInformation, typeInformationForResourceResolution))
        return null;

      return new DelegateBasedValidationMessage(
          () => _memberInformationGlobalizationService.GetTypeDisplayName(typeInformation, typeInformationForResourceResolution));
    }

    public ValidationMessage? CreateValidationMessageForObjectValidator (IObjectValidator validator, ITypeInformation validatedType)
    {
      ArgumentUtility.CheckNotNull("validator", validator);
      ArgumentUtility.CheckNotNull("validatedType", validatedType);

      var typeInformation = TypeAdapter.Create(validator.GetType());
      var typeInformationForResourceResolution = typeInformation;

      if (!_memberInformationGlobalizationService.ContainsTypeDisplayName(typeInformation, typeInformationForResourceResolution))
        return null;

      return new DelegateBasedValidationMessage(
          () => _memberInformationGlobalizationService.GetTypeDisplayName(typeInformation, typeInformationForResourceResolution));
    }
  }
}
