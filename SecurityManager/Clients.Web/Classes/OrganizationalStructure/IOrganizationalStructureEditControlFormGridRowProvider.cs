// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure
{
  /// <summary>
  /// Closed implementations of the <see cref="IOrganizationalStructureEditControlFormGridRowProvider{TDataEditControl}"/> type can be registered
  /// with the <see cref="IServiceLocator"/> and are then used to provide <see cref="FormGridRowInfo"/> objects for implementations of 
  /// <see cref="BaseEditControl{T}"/>.
  /// </summary>
  /// <typeparam name="TDataEditControl">
  ///   The user control. Can be <see cref="EditUserControl"/>, <see cref="EditGroupControl"/>, <see cref="EditTenantControl"/>,
  ///   <see cref="EditGroupTypeControl"/>, and <see cref="EditPositionControl"/>
  /// </typeparam>
  /// <seealso cref="IFormGridRowProvider"/>
  public interface IOrganizationalStructureEditControlFormGridRowProvider<in TDataEditControl>
      where TDataEditControl: BaseEditControl<TDataEditControl>
  {
    /// <summary>
    /// Returns a sequence of IDs identifying the rows to be hidden in the <paramref name="formGrid"/>.
    /// </summary>
    /// <param name="dataEditControl">The <see cref="BaseEditControl{T}"/> contains the <paramref name="formGrid"/>. Must not be <see langword="null" />.</param>
    /// <param name="formGrid">The <see cref="HtmlTable"/> whose rows will be hidden. Must not be <see langword="null" />.</param>
    /// <param name="formGridManager">The <see cref="FormGridManager"/> of the <paramref name="formGrid"/>. Must not be <see langword="null" />.</param>
    /// <returns>A sequence of <see cref="string"/> objectgs. </returns>
    IEnumerable<string> GetHiddenRows (TDataEditControl dataEditControl, HtmlTable formGrid, FormGridManager formGridManager);

    /// <summary>
    ///   Returns a sequence of <see cref="FormGridRowInfo"/> objects used to constrtuct and then insert new rows into the <paramref name="formGrid"/>.
    /// </summary>
    /// <param name="dataEditControl">The <see cref="BaseEditControl{T}"/> contains the <paramref name="formGrid"/>. Must not be <see langword="null" />.</param>
    /// <param name="formGrid">The <see cref="HtmlTable"/> whose rows will be hidden. Must not be <see langword="null" />.</param>
    /// <param name="formGridManager">The <see cref="FormGridManager"/> of the <paramref name="formGrid"/>. Must not be <see langword="null" />.</param>
    /// <returns> A sequence of <see cref="FormGridRowInfo"/> objects. </returns>
    IEnumerable<FormGridRowInfo> GetAdditionalRows (TDataEditControl dataEditControl, HtmlTable formGrid, FormGridManager formGridManager);
  }
}
