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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Fluent selection extension methods.
  /// </summary>
  public static class FluentControlSelectorExtensionsForIntegrationTestsForObjectBinding
  {
    public static FluentControlSelector<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject> AutoCompletes (
        this IControlHost host)
    {
      return new FluentControlSelector<BocAutoCompleteReferenceValueSelector, BocAutoCompleteReferenceValueControlObject>(
          host,
          new BocAutoCompleteReferenceValueSelector());
    }

    public static FluentControlSelector<BocBooleanValueSelector, BocBooleanValueControlObject> BooleanValues (this IControlHost host)
    {
      return new FluentControlSelector<BocBooleanValueSelector, BocBooleanValueControlObject>(host, new BocBooleanValueSelector());
    }

    public static FluentControlSelector<BocCheckBoxSelector, BocCheckBoxControlObject> CheckBoxes (this IControlHost host)
    {
      return new FluentControlSelector<BocCheckBoxSelector, BocCheckBoxControlObject>(host, new BocCheckBoxSelector());
    }

    public static FluentControlSelector<BocDateTimeValueSelector, BocDateTimeValueControlObject> DateTimeValues (this IControlHost host)
    {
      return new FluentControlSelector<BocDateTimeValueSelector, BocDateTimeValueControlObject>(host, new BocDateTimeValueSelector());
    }

    public static FluentControlSelector<BocEnumValueSelector, BocEnumValueControlObject> EnumValues (this IControlHost host)
    {
      return new FluentControlSelector<BocEnumValueSelector, BocEnumValueControlObject>(host, new BocEnumValueSelector());
    }

    public static FluentControlSelector<BocListSelector, BocListControlObject> Lists (this IControlHost host)
    {
      return new FluentControlSelector<BocListSelector, BocListControlObject>(host, new BocListSelector());
    }

    public static FluentControlSelector<BocListAsGridSelector, BocListAsGridControlObject> ListAsGrids (this IControlHost host)
    {
      return new FluentControlSelector<BocListAsGridSelector, BocListAsGridControlObject>(host, new BocListAsGridSelector());
    }

    public static FluentControlSelector<BocMultilineTextValueSelector, BocMultilineTextValueControlObject> MultilineTextValues (
        this IControlHost host)
    {
      return new FluentControlSelector<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>(host, new BocMultilineTextValueSelector());
    }

    public static FluentControlSelector<BocReferenceValueSelector, BocReferenceValueControlObject> ReferenceValues (this IControlHost host)
    {
      return new FluentControlSelector<BocReferenceValueSelector, BocReferenceValueControlObject>(host, new BocReferenceValueSelector());
    }

    public static FluentControlSelector<BocTextValueSelector, BocTextValueControlObject> TextValues (this IControlHost host)
    {
      return new FluentControlSelector<BocTextValueSelector, BocTextValueControlObject>(host, new BocTextValueSelector());
    }

    public static FluentControlSelector<BocTreeViewSelector, BocTreeViewControlObject> TreeViews (this IControlHost host)
    {
      return new FluentControlSelector<BocTreeViewSelector, BocTreeViewControlObject>(host, new BocTreeViewSelector());
    }

    public static FluentControlSelector<WebButtonSelector, WebButtonControlObject> WebButtons (this IControlHost host)
    {
      return new FluentControlSelector<WebButtonSelector, WebButtonControlObject>(host, new WebButtonSelector());
    }
  }
}
