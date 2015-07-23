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
namespace Remotion.Web.Development.WebTesting.PageObjects
{
  /// <summary>
  /// Interface for all page objects representing an arbitrary ASP.NET WebForms page.
  /// </summary>
  /// <remarks>
  /// Actually, this interface does not belong to the base framework, but should live in an assembly referenced by all control object libraries
  /// written for the ASP.NET WebForms technology stack. However, IWebFormsPageObject is currently the only such entity and to save overhead we
  /// decided to not create such a "WebTesting.WebForms" assembly yet.
  /// </remarks>
  public interface IWebFormsPageObject
  {
    /// <summary>
    /// Returns the completion detection strategy when a control object's action is navigating to another page.
    /// </summary>
    ICompletionDetectionStrategy NavigationCompletionDetectionStrategy { get; }

    /// <summary>
    /// Returns the completion detection strategy when a control object's action is triggering a post back.
    /// </summary>
    ICompletionDetectionStrategy PostBackCompletionDetectionStrategy { get; }
  }
}