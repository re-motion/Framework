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
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests;

public class BocAutoCompleteReferenceValueSearchResultControlObject : ControlObject
{
  public class Item : ControlObject
  {
    public Item ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    public string Text => Scope.Text;

    public void Click ()
    {
      // Clicking directly on the LI element does not work in Firefox
      // As such, we click on the nested span and let the click event bubble up
      Scope.FindCss("span").Click();
    }

    public void ScrollIntoView ()
    {
      var driver = ((IWrapsDriver)Scope.Native).WrappedDriver;
      var jsExecutor = (IJavaScriptExecutor)driver;

      var executeScript = jsExecutor.ExecuteScript("arguments[0].scrollIntoView(true);return 1;", (IWebElement)Scope.Native);
    }

    protected override ICompletionDetectionStrategy GetDefaultCompletionDetectionStrategy (ElementScope scope)
    {
      return new NullCompletionDetectionStrategy();
    }
  }

  public BocAutoCompleteReferenceValueSearchResultControlObject ([NotNull] ControlObjectContext context)
      : base(context)
  {
  }

  /// <inheritdoc />
  protected override ICompletionDetectionStrategy GetDefaultCompletionDetectionStrategy (ElementScope scope)
  {
    return new NullCompletionDetectionStrategy();
  }

  public bool IsVisible => Scope.IsVisible(Logger);

  public Item[] GetItems (Predicate<Item[]> predicate)
  {
    return RetryUntilTimeout.Run(
        Logger,
        () =>
        {
          var result = Scope.FindAllCss("li", options: Options.NoWait)
              .Select(el => new Item(Context.CloneForControl(el)))
              .ToArray();

          if (!predicate(result))
            throw new InvalidOperationException("Could not find search results matching the predicate.");

          return result;
        });
  }
}
