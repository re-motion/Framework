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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Services
{
  /// <summary>
  /// Represents a DTO for transfering the result of the <see cref="BocAutoCompleteReferenceValue"/> to the client and displaying 
  /// the elements as options in the <see cref="BocAutoCompleteReferenceValue"/>.
  /// </summary>
  /// <remarks>
  /// This type offers setable properties and a default constructor to comply with the .NET 2.0 web service implementation.
  /// Returning a not fully initialized objects will result in unexpected behavior by the <see cref="ISearchAvailableObjectWebService"/>.
  /// </remarks>
  public sealed class BusinessObjectWithIdentityProxy
  {
    private string _uniqueIdentifier;
    private string _displayName;
    private string _iconUrl = "";

    public BusinessObjectWithIdentityProxy ()
    {
    }

    public BusinessObjectWithIdentityProxy (IBusinessObjectWithIdentity obj)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      _uniqueIdentifier = obj.UniqueIdentifier;
      _displayName = obj.GetAccessibleDisplayName();
    }

    public string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
      set { _uniqueIdentifier = ArgumentUtility.CheckNotNullOrEmpty ("value", value); }
    }

    public string DisplayName
    {
      get { return _displayName; }
      set { _displayName = ArgumentUtility.CheckNotNullOrEmpty ("value", value); }
    }

    public string IconUrl
    {
      get { return _iconUrl; }
      set { _iconUrl = value ?? string.Empty; }
    }
  }
}
