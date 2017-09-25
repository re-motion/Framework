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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.TestSite.Infrastructure;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  /// <summary>
  /// Represents options that are passed to <see cref="IGenericTestPage{TOptions}"/>.
  /// </summary>
  public class GenericTestOptions
  {
    private readonly string _localID;
    private readonly string _htmlID;
    private readonly string _dataSource;
    private readonly string _correctDomainProperty;
    private readonly string _incorrectDomainProperty;

    public GenericTestOptions (
        [NotNull] string localID,
        [NotNull] string htmlID,
        [NotNull] string dataSource,
        [NotNull] string correctDomainProperty,
        [NotNull] string incorrectDomainProperty)
    {
      ArgumentUtility.CheckNotNull ("localID", localID);
      ArgumentUtility.CheckNotNull ("htmlID", htmlID);
      ArgumentUtility.CheckNotNull ("correctDomainProperty", correctDomainProperty);
      ArgumentUtility.CheckNotNull ("incorrectDomainProperty", incorrectDomainProperty);

      _localID = localID;
      _htmlID = htmlID;
      _dataSource = dataSource;
      _correctDomainProperty = correctDomainProperty;
      _incorrectDomainProperty = incorrectDomainProperty;
    }

    public string LocalID
    {
      get { return _localID; }
    }

    public string HtmlID
    {
      get { return _htmlID; }
    }

    public string DataSource
    {
      get { return _dataSource; }
    }

    public string CorrectDomainProperty
    {
      get { return _correctDomainProperty; }
    }

    public string IncorrectDomainProperty
    {
      get { return _incorrectDomainProperty; }
    }
  }
}