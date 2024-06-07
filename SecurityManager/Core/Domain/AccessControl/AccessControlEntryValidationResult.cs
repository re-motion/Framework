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
using Remotion.Globalization;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// The <see cref="AccessControlEntryValidationResult"/> type collects validation state for the <see cref="AccessControlEntry"/> type.
  /// </summary>
  public class AccessControlEntryValidationResult
  {
    private readonly HashSet<AccessControlEntryValidationError> _errors = new HashSet<AccessControlEntryValidationError>();

    public AccessControlEntryValidationResult ()
    {
    }

    public bool IsValid
    {
      get { return _errors.Count == 0; }
    }

    public AccessControlEntryValidationError[] GetErrors ()
    {
      return _errors.OrderBy(e => (int)e).ToArray();
    }

    public void SetError (AccessControlEntryValidationError error)
    {
      _errors.Add(error);
    }

    public string GetErrorMessage ()
    {
      StringBuilder errorMessageBuilder = new StringBuilder(_errors.Count * 100);
      errorMessageBuilder.Append("The access control entry is in an invalid state:");
      var enumerationGlobalizationService = SafeServiceLocator.Current.GetInstance<IEnumerationGlobalizationService>();
      foreach (var error in GetErrors())
      {
        errorMessageBuilder.AppendLine();
        errorMessageBuilder.Append("  ");

        var displayName = enumerationGlobalizationService.GetEnumerationValueDisplayName(error);
        errorMessageBuilder.Append(displayName);
      }

      return errorMessageBuilder.ToString();
    }
  }
}
