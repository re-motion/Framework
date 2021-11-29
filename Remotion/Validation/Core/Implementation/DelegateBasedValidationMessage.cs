﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Globalization;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  public sealed class DelegateBasedValidationMessage : ValidationMessage
  {
    private readonly Func<string> _validationMessageProvider;

    public DelegateBasedValidationMessage (Func<string> validationMessageProvider)
    {
      ArgumentUtility.CheckNotNull("validationMessageProvider", validationMessageProvider);

      _validationMessageProvider = validationMessageProvider;
    }

    public override string Format (CultureInfo culture, IFormatProvider? formatProvider, params object?[] parameters)
    {
      ArgumentUtility.CheckNotNull("culture", culture);
      ArgumentUtility.CheckNotNull("parameters", parameters);

      using (new CultureScope(CultureInfo.InvariantCulture, culture))
      {
        var validationMessage = _validationMessageProvider();

        Assertion.IsNotNull(validationMessage, $"Delegate '{_validationMessageProvider}' returned null.");

        return string.Format(formatProvider, validationMessage, parameters);
      }
    }

    public override string ToString ()
    {
      return _validationMessageProvider() ?? $"Delegate '{_validationMessageProvider}' returned null.";
    }
  }
}
