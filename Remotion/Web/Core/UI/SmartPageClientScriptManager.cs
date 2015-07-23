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
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI
{
  /// <summary>
  /// The <see cref="SmartPageClientScriptManager"/> type is the default implementation of the <see cref="ISmartPageClientScriptManager"/> interface.
  /// </summary>
  public class SmartPageClientScriptManager : ClientScriptManagerWrapper, ISmartPageClientScriptManager
  {
    public SmartPageClientScriptManager (ClientScriptManager clientScriptManager)
        : base(clientScriptManager)
    {
    }

    /// <summary>
    /// Returns a string that can be used in a client event to display the abort confirmation of the <see cref="ISmartPage"/> instance. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// When the script is executed, its result will be a boolean value, <c>true</c> specifiying that the user confirmed the abort.
    /// </para><para>
    /// The conditions for showing the abort confirmation are identical to that of the abort confirmation displayed when leaving the page.
    /// If the abort confirmation is disabled or not required, the script will also evaluate <c>true</c>.
    /// </para>
    /// </remarks>
    /// <returns>
    /// A string that, when treated as script on the client, displayes the abort confirmation and results in a boolean value.
    /// </returns>
    public string GetShowAbortConfirmationReference ()
    {
      return "SmartPage_Context.Instance.ShowAbortConfirmation()";
    }
  }
}