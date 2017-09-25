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
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.TestSite.Infrastructure;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.GenericPages
{
  /// <summary>
  /// Provides a simple way to implement <see cref="IGenericTestPage{TOptions}"/>.
  /// </summary>
  public abstract class SimpleGenericTestPage<TControl> : IGenericTestPage<GenericTestOptions>
      where TControl : BusinessObjectBoundWebControl, new()
  {
    protected SimpleGenericTestPage ()
    {
    }

    public abstract string DisplayName { get; }

    public abstract string DomainProperty { get; }

    public abstract string PropertyIdentifier { get; }

    /// <inheritdoc />
    public void AddParameters (GenericTestPageParameterCollection parameterCollection, GenericTestOptions options)
    {
      parameterCollection.Add (TestConstants.DisplayNameSelectorID, DisplayName, "Hidden" + DisplayName, options.HtmlID);
      parameterCollection.Add (
          TestConstants.DomainPropertySelectorID,
          DomainProperty,
          "Hidden" + DomainProperty,
          options.HtmlID,
          options.CorrectDomainProperty,
          options.IncorrectDomainProperty);
    }

    /// <inheritdoc />
    Control IGenericTestPage<GenericTestOptions>.CreateControl (GenericTestOptions options)
    {
      return CreateControl (options);
    }

    /// <summary>
    /// Creates a new <typeparamref name="TControl"/> and sets the HTML id.
    /// </summary>
    public virtual TControl CreateControl (GenericTestOptions options)
    {
      return new TControl { ID = options.LocalID, DataSourceControl = options.DataSource, PropertyIdentifier = PropertyIdentifier };
    }
  }
}