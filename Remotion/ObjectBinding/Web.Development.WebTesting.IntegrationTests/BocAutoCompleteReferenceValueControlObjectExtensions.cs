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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests;

public static class BocAutoCompleteReferenceValueControlObjectExtensions
{
  public static BocAutoCompleteReferenceValueSearchResultControlObject OpenSearchResults (this BocAutoCompleteReferenceValueControlObject bocAutoComplete, string text)
  {
    bocAutoComplete.FillWith(
        text,
        el => el.SendKeys("\uE015"), // Down arrow
        new WebTestActionOptions { CompletionDetectionStrategy = new NullCompletionDetectionStrategy() });

    var searchBoxScope = RetryUntilTimeout.Run(
        () =>
        {
          var searchId = bocAutoComplete.GetHtmlID() + "_Results";
          var searchBoxScope = bocAutoComplete.Context.RootScope.FindId(searchId);
          if (!searchBoxScope.IsVisible())
            throw new InvalidOperationException("Cannot find the search box.");

          return searchBoxScope;
        });
    var controlObjectContext = bocAutoComplete.Context.CloneForControl(searchBoxScope);

    return new BocAutoCompleteReferenceValueSearchResultControlObject(controlObjectContext);
  }
}
