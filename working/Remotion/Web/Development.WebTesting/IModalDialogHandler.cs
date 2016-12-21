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
using OpenQA.Selenium;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Responsible for dealing with a modal browser dialog (a browser alert). Modal browser dialogs must be handled before an action is able to finish,
  /// as all DOM interaction is completely blocked.
  /// </summary>
  public interface IModalDialogHandler
  {
    /// <summary>
    /// Accepts or cancels the modal browser dialog.
    /// </summary>
    /// <exception cref="NoAlertPresentException">Thrown if no modal browser dialog is present.</exception>
    void HandleModalDialog ([NotNull] PageObjectContext context);
  }
}