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
using FluentValidation.Internal;
using FluentValidation.Resources;
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Utilities;

namespace Remotion.Validation.Globalization
{
  public class PropertyRuleDisplayNameStringSource : IStringSource
  {
    private readonly PropertyRule _propertyRule;
    private readonly Type _typeToValidate;
    private readonly IMemberInformationGlobalizationService _globalizationService;
    private readonly string _resourceName;
    private readonly Type _resourceType;

    public PropertyRuleDisplayNameStringSource (
        PropertyRule propertyRule, Type typeToValidate, IMemberInformationGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull ("propertyRule", propertyRule);
      ArgumentUtility.CheckNotNull ("typeToValidate", typeToValidate);
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);

      _propertyRule = propertyRule;
      _typeToValidate = typeToValidate;
      _globalizationService = globalizationService;

      _resourceName = typeToValidate.FullName;
      _resourceType = globalizationService.GetType ();
    }

    public string GetString ()
    {
      var propertyInformation = PropertyInfoAdapter.Create (_propertyRule.GetPropertyInfo ());
      var typeInformation = TypeAdapter.Create (_typeToValidate);

      var localizedPropertyName = _globalizationService.GetPropertyDisplayName (propertyInformation, typeInformation);
      return localizedPropertyName;
    }

    public string ResourceName
    {
      get { return _resourceName; }
    }

    public Type ResourceType
    {
      get { return _resourceType; }
    }
  }
}