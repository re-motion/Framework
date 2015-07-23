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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation
{
  /// <summary>
  /// The <see cref="BocBooleanValueResourceSet"/> type is a value object that holds the information required 
  /// for displaying the <see cref="BocBooleanValue"/>'s three states.
  /// </summary>
  public class BocBooleanValueResourceSet
  {
    private readonly string _resourceKey;
    private readonly string _trueIconUrl;
    private readonly string _falseIconUrl;
    private readonly string _nullIconUrl;
    private readonly string _defaultTrueDescription;
    private readonly string _defaultFalseDescription;
    private readonly string _defaultNullDescription;

    public BocBooleanValueResourceSet (
        string resourceKey,
        string trueIconUrl,
        string falseIconUrl,
        string nullIconUrl,
        string defaultTrueDescription,
        string defaultFalseDescription,
        string defaultNullDescription)
    {
      ArgumentUtility.CheckNotNull ("resourceKey", resourceKey);
      ArgumentUtility.CheckNotNullOrEmpty ("trueIconUrl", trueIconUrl);
      ArgumentUtility.CheckNotNullOrEmpty ("falseIconUrl", falseIconUrl);
      ArgumentUtility.CheckNotNullOrEmpty ("nullIconUrl", nullIconUrl);
      ArgumentUtility.CheckNotNullOrEmpty ("defaultTrueDescription", defaultTrueDescription);
      ArgumentUtility.CheckNotNullOrEmpty ("defaultFalseDescription", defaultFalseDescription);
      ArgumentUtility.CheckNotNullOrEmpty ("defaultNullDescription", defaultNullDescription);

      _resourceKey = resourceKey;
      _trueIconUrl = trueIconUrl;
      _falseIconUrl = falseIconUrl;
      _nullIconUrl = nullIconUrl;
      _defaultTrueDescription = defaultTrueDescription;
      _defaultFalseDescription = defaultFalseDescription;
      _defaultNullDescription = defaultNullDescription;
    }

    public string ResourceKey
    {
      get { return _resourceKey; }
    }

    public string TrueIconUrl
    {
      get { return _trueIconUrl; }
    }

    public string FalseIconUrl
    {
      get { return _falseIconUrl; }
    }

    public string NullIconUrl
    {
      get { return _nullIconUrl; }
    }

    public string DefaultTrueDescription
    {
      get { return _defaultTrueDescription; }
    }

    public string DefaultFalseDescription
    {
      get { return _defaultFalseDescription; }
    }

    public string DefaultNullDescription
    {
      get { return _defaultNullDescription; }
    }
  }
}