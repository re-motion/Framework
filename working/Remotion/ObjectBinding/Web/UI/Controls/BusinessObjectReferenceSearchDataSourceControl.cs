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
using System.ComponentModel;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   <b>BusinessObjectReferenceSearchDataSourceControl</b> is used to supply an <see cref="IBusinessObjectClass"/>
  ///   to an <see cref="IBusinessObjectBoundWebControl"/> used for displaying a search result.
  /// </summary>
  /// <remarks>
  ///   Since a search result is usually an <see cref="IBusinessObject"/> list without an actual parent object
  ///   to connect to the data source, normal data binding is not possible. By using the 
  ///   <b>BusinessObjectReferenceSearchDataSourceControl</b> it is possible to provide the meta data to the bound
  ///   controls dispite the lack of a parent object.
  /// </remarks>
  public class BusinessObjectReferenceSearchDataSourceControl: BusinessObjectReferenceDataSourceControl
  {
    /// <summary> Multiplicity is always supported. </summary>
    /// <param name="isList"> Not evaluated. </param>
    /// <returns> Always <see langword="true"/>. </returns>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return true;
    }

    /// <summary> Not supported by <see cref="BusinessObjectReferenceSearchDataSourceControl"/>. </summary>
    /// <param name="interim"> Not evaluated. </param>
    public override void LoadValues (bool interim)
    {
      throw new NotSupportedException ("Use BusinessObjectReferenceDataSourceControl for actual data.");
    }

    /// <summary>
    ///   Gets or sets the current <see cref="DataSourceMode"/> of this 
    ///   <see cref="BusinessObjectReferenceSearchDataSourceControl"/>.
    /// </summary>
    /// <value> <see cref="DataSourceMode.Search"/>. </value>
    /// <exception cref="NotSupportedException"> Thrown upon an attempt to set a value other than <see cref="DataSourceMode.Search"/>. </exception>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [DefaultValue (DataSourceMode.Search)]
    public override DataSourceMode Mode
    {
      get { return DataSourceMode.Search; }
      set { if (value != DataSourceMode.Search) throw new NotSupportedException ("BusinessObjectReferenceSearchDataSourceControl supports only DataSourceMode.Search."); }
    }
  }
}
