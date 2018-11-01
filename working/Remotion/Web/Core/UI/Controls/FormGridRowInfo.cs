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
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
/// <summary>
///   The information for the automatic creation of a new <see cref="FormGridManager.FormGridRow"/>. 
/// </summary>
public class FormGridRowInfo
{
  /// <summary> The possible positions for inserting the new row relative to a given ID. </summary>
  public enum RowPosition
  {
    /// <summary> Place the row before the row containing the ID. </summary>
    BeforeRowWithID,

    /// <summary> Place the row after the row containing the ID. </summary>
    AfterRowWithID
  }

  /// <summary> The possible layouts for the new <see cref="FormGridManager.FormGridRow"/>. </summary>
  public enum RowType
  {
    /// <summary> Label and Control will be placed into the same row. </summary>
    ControlInRowWithLabel,

    /// <summary> The Control will be placed in a seperat row, following the Label's row. </summary>
    ControlInRowAfterLabel
  }

  /// <summary> The control to the inserted into the row. </summary>
  private Control _control;

  /// <summary>
  ///   The <see cref="RowType"/> for the new <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  private RowType _newRowType;

  /// <summary>
  ///   The <see cref="RowPosition"/> for inserting the new 
  ///   <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  private RowPosition _positionInFormGrid;

  /// <summary>
  ///   The row used as a point of reference for inserting the new 
  ///   <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  private string _releatedRowID;

  /// <summary> 
  ///   Initiliazes a new instance of the <see cref="FormGridRowInfo"/> class with all 
  ///   required information.
  /// </summary>
  /// <param name="control"> The control to the inserted into the row. Must not be <see langword="null" />.</param>
  /// <param name="newRowType">
  ///   The <see cref="RowType"/> for the new <see cref="FormGridManager.FormGridRow"/>.
  /// </param>
  /// <param name="relatedRowID">
  ///   The row used as a point of reference for inserting the new <see cref="FormGridManager.FormGridRow"/>. Can be <see langword="null" />.
  /// </param>
  /// <param name="positionInFormGrid">
  ///   The <see cref="RowPosition"/> for inserting the new 
  ///   <see cref="FormGridManager.FormGridRow"/>.
  /// </param>
  public FormGridRowInfo (
      Control control, 
      RowType newRowType,
      string relatedRowID,
      RowPosition positionInFormGrid)
  {
    ArgumentUtility.CheckNotNull ("control", control);

    _control = control;
    _newRowType = newRowType;
    _positionInFormGrid = positionInFormGrid;
    _releatedRowID = relatedRowID;
  }

  /// <summary> Gets the control to the inserted into the row.  </summary>
  public Control Control
  {
    get { return _control; }
  }

  /// <summary>
  ///   Gets the <see cref="RowType"/> for the new <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  public RowType NewRowType
  {
    get { return _newRowType; }
  }

  /// <summary> 
  ///   Gets the row used as a point of reference when inserting the new 
  ///   <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  public string ReleatedRowID
  {
    get { return _releatedRowID; }
  }
 
  /// <summary> 
  ///   Gets the <see cref="RowPosition"/> for inserting the new 
  ///   <see cref="FormGridManager.FormGridRow"/>.
  /// </summary>
  public RowPosition PositionInFormGrid
  {
    get { return _positionInFormGrid; }
  }
}

}
