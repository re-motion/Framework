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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web
{
  /// <summary>
  ///   Provides services for business object bound web applications
  /// </summary>
  public interface IBusinessObjectWebUIService : IBusinessObjectService
  {
    IconInfo GetIcon (IBusinessObject obj);
    string GetToolTip (IBusinessObject obj);

    /// <summary>
    /// Returns a <see cref="HelpInfo"/> object for the specified parameters.
    /// </summary>
    /// <param name="control">The <see cref="IBusinessObjectBoundWebControl"/> for which the help-link is to be generated.</param>
    /// <param name="businessObjectClass">The <see cref="IBusinessObjectClass"/> bound to the <paramref name="control"/>'s datasource.</param>
    /// <param name="businessObjectProperty">
    /// The <see cref="IBusinessObjectProperty"/> displayed by the <paramref name="control"/>. Can be <see langword="null" />.
    /// </param>
    /// <param name="businessObject">
    /// The <see cref="IBusinessObject"/> bound to the <paramref name="control"/>. 
    /// Can be <see langword="null" /> if the control's datasource has no value.
    /// </param>
    /// <returns>An instance of the <see cref="HelpInfo"/> type or <see langword="null" /> to not generate a help-link.</returns>
    HelpInfo GetHelpInfo (
        IBusinessObjectBoundWebControl control,
        IBusinessObjectClass businessObjectClass,
        IBusinessObjectProperty businessObjectProperty,
        IBusinessObject businessObject);
  }
}
