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
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Page object representing an arbitrary ASP.NET WebForms page.
  /// </summary>
  public class WebFormsTestPageObject : PageObject, IWebFormsPageObject
  {
    public WebFormsTestPageObject (PageObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the completion detection strategy when a control object's action is navigating to another page.
    /// </summary>
    public virtual ICompletionDetectionStrategy NavigationCompletionDetectionStrategy
    {
      get { throw new NotSupportedException ("The WebFormsPageObject does not support deterministic waiting for navigation completion yet."); }
    }

    /// <summary>
    /// Returns the completion detection strategy when a control object's action is triggering a post back.
    /// </summary>
    public virtual ICompletionDetectionStrategy PostBackCompletionDetectionStrategy
    {
      get { throw new NotSupportedException ("The WebFormsPageObject does not support deterministic waiting for post back completion yet."); }
    }
  }
}