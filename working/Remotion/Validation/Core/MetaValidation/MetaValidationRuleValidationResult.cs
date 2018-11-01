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
using Remotion.Utilities;

namespace Remotion.Validation.MetaValidation
{
  /// <summary>
  /// Encapsulates the result of a validation rule consistency verification.
  /// </summary>
  public sealed class MetaValidationRuleValidationResult
  {
    public static MetaValidationRuleValidationResult CreateValidResult ()
    {
      return new MetaValidationRuleValidationResult (true, null);
    }

    [JetBrains.Annotations.StringFormatMethod ("messageFormat")]
    public static MetaValidationRuleValidationResult CreateInvalidResult (string messageFormat, params object[] args)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("messageFormat", messageFormat);
      ArgumentUtility.CheckNotNull ("args", args);

      return new MetaValidationRuleValidationResult (false, string.Format (messageFormat, args));
    }
    
    private readonly bool _isValid;
    private readonly string _message;

    private MetaValidationRuleValidationResult (bool isValid, string message)
    {
      _isValid = isValid;
      _message = message;
    }

    public bool IsValid
    {
      get { return _isValid; }
    }

    public string Message
    {
      get { return _message; }
    }
  }
}