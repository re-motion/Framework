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
using System.Web.UI.WebControls;
using Remotion.ServiceLocation;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Contains all the CSS class definitions needed throughout <see cref="BocList"/> rendering.
  /// </summary>
  [ImplementationFor(typeof(BocListCssClassDefinition), Lifetime = LifetimeKind.Singleton)]
  public class BocListCssClassDefinition
  {
    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocTextValue</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    public virtual string Base
    {
      get { return "bocList"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>readOnly</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.readOnly</c> as a selector. </para>
    /// </remarks>
    public virtual string ReadOnly
    {
      get { return CssClassDefinition.ReadOnly; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.disabled</c> as a selector.</para>
    /// </remarks>
    public virtual string Disabled
    {
      get { return CssClassDefinition.Disabled; }
    }

    /// <summary>
    /// Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> when itself and child elements
    /// that are standard browser controls (e.g. input elements) should be styled in the current theme.
    /// </summary>
    /// <remarks> 
    ///   <para> Class: <c>remotion-themed</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class.</para>
    /// </remarks>
    public virtual string Themed
    {
      get { return CssClassDefinition.Themed; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s <c>table</c> tag. </summary>
    /// <remarks> Class: <c>bocListTable</c> </remarks>
    public virtual string Table
    {
      get { return "bocListTable"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s <c>thead</c> tag. </summary>
    /// <remarks> Class: <c>bocListTableHead</c> </remarks>
    public virtual string TableHead
    {
      get { return "bocListTableHead"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s <c>tbody</c> tag. </summary>
    /// <remarks> Class: <c>bocListTableBody</c> </remarks>
    public virtual string TableBody
    {
      get { return "bocListTableBody"; }
    }

    /// <summary> CSS-Class applied to the cells in the <see cref="BocList"/>'s title row. </summary>
    /// <remarks> Class: <c>bocListTitleCell</c> </remarks>
    public virtual string TitleCell
    {
      get { return "bocListTitleCell"; }
    }

    /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s selected data. </summary>
    /// <remarks> Class: <c>bocListDataRow</c> </remarks>
    public virtual string DataRow
    {
      get { return "bocListDataRow"; }
    }

    /// <summary> Gets the CSS-Class applied to a validation row in the <see cref="BocList"/>. </summary>
    /// <remarks> Class: <c>bocListValidationRow</c> </remarks>
    public virtual string ValidationRow
    {
      get { return "bocListValidationRow"; }
    }

    /// <summary> Gets the CSS-Class additionally applied to the <see cref="BocList"/>'s selected data rows. </summary>
    /// <remarks> Class: <c>selected</c> </remarks>
    public string DataRowSelected
    {
      get { return "selected"; }
    }

    /// <summary> Gets the CSS-Class additionally applied to the <see cref="BocList"/>'s even data rows. </summary>
    /// <remarks> Class: <c>odd</c> </remarks>
    public string DataRowOdd
    {
      get { return "odd"; }
    }

    /// <summary> Gets the CSS-Class additionally applied to the <see cref="BocList"/>'s even data rows. </summary>
    /// <remarks> Class: <c>even</c> </remarks>
    public string DataRowEven
    {
      get { return "even"; }
    }

    /// <summary> Gets the CSS-Class applied to the cells in the <see cref="BocList"/>'s odd data rows. </summary>
    /// <remarks> Class: <c>bocListDataCellOdd</c> </remarks>
    public virtual string DataCell
    {
      get { return "bocListDataCell"; }
    }

    /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s title row that contains the row index header. </summary>
    /// <remarks> Class: <c>bocListTitleCellIndex</c> </remarks>
    public virtual string TitleCellIndex
    {
      get { return "bocListTitleCellIndex"; }
    }

    /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s data rows that contains the row index. </summary>
    /// <remarks> Class: <c>bocListDataCellIndex</c> </remarks>
    public virtual string DataCellIndex
    {
      get { return "bocListDataCellIndex"; }
    }

    /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s title row that contains the all rows selector. </summary>
    /// <remarks> Class: <c>bocListTitleCellAllRowsSelector</c> </remarks>
    public virtual string TitleCellSelector
    {
      get { return "bocListTitleCellAllRowsSelector"; }
    }

    /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s data rows that contains the edit mode controls. </summary>
    /// <remarks> Class: <c>bocListDataCellEditModeButtons</c> </remarks>
    public virtual string DataCellEditModeButtons
    {
      get { return "bocListDataCellEditModeButtons"; }
    }

    /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s title row that contains the edit mode controls. </summary>
    /// <remarks> Class: <c>bocListTitleCellEditModeButtons</c> </remarks>
    public virtual string TitleCellEditModeButtons
    {
      get { return "bocListTitleCellEditModeButtons"; }
    }

    /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s data rows that contains the row selector. </summary>
    /// <remarks> Class: <c>bocListDataCellRowSelector</c> </remarks>
    public virtual string DataCellSelector
    {
      get { return "bocListDataCellRowSelector"; }
    }

    /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s data column declaration that represents the validation failure indicator column. </summary>
    /// <remarks> Class: <c>bocListDataColumnDeclarationValidationFailureIndicator</c> </remarks>
    public virtual string DataColumnDeclarationValidationFailureIndicator
    {
      get { return "bocListDataColumnDeclarationValidationFailureIndicator"; }
    }

    /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s title row that contains the validation failure indicator header. </summary>
    /// <remarks> Class: <c>bocListTitleCellValidationFailureIndicator</c> </remarks>
    public virtual string TitleCellValidationFailureIndicator
    {
      get { return "bocListTitleCellValidationFailureIndicator"; }
    }

    /// <summary> Gets the CSS-Class applied to the cell in the <see cref="BocList"/>'s data rows that contains the validation failure indicator. </summary>
    /// <remarks> Class: <c>bocListDataCellValidationFailureIndicator</c> </remarks>
    public virtual string DataCellValidationFailureIndicator
    {
      get { return "bocListDataCellValidationFailureIndicator"; }
    }

    /// <summary> Gets the CSS-Class applied to the content if there is no anchor element. </summary>
    /// <remarks> Class: <c>bocListDataCellContent</c> </remarks>
    public virtual string Content
    {
      get { return "bocListContent"; }
    }

    /// <summary> Gets the CSS-Class applied to the text providing the sorting order's index. </summary>
    /// <remarks> Class: <c>bocListSortingOrder</c> </remarks>
    public virtual string SortingOrder
    {
      get { return "bocListSortingOrder"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s navigator. </summary>
    /// <remarks> Class: <c>bocListNavigator</c> </remarks>
    public virtual string Navigator
    {
      get { return "bocListNavigator"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s list of additional columns. </summary>
    /// <remarks> Class: <c>bocListAvailableViewsList</c> </remarks>
    public virtual string AvailableViewsList
    {
      get { return "bocListAvailableViewsList"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s dropdown list of additional columns. </summary>
    /// <remarks> Class: <c>bocListAvailableViewsListDropDownList</c> </remarks>
    public virtual string AvailableViewsListDropDownList
    {
      get { return "bocListAvailableViewsListDropDownList"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s label for the list of additional columns. </summary>
    /// <remarks> Class: <c>bocListAvailableViewsListLabel</c> </remarks>
    public virtual string AvailableViewsListLabel
    {
      get { return "bocListAvailableViewsListLabel"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s validation error marker icons. </summary>
    /// <remarks> Class: <c>validationErrorMarker</c> </remarks>
    public virtual string ValidationErrorMarker
    {
      get { return "validationErrorMarker"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s validation failure cells. </summary>
    public virtual string ValidationFailureCell
    {
      get { return "bocListValidationFailureCell"; }
    }

    public string ListMenuContainer
    {
      get { return "bocListListMenuContainer"; }
    }

    public string TableBlock
    {
      get { return "bocListTableBlock"; }
    }

    public string TableContainer
    {
      get { return "bocListTableContainer"; }
    }

    public string TableScrollContainer
    {
      get { return "bocListTableScrollContainer"; }
    }

    public string HasMenuBlock
    {
      get { return "hasMenuBlock"; }
    }

    public string HasNavigator
    {
      get { return "hasNavigator"; }
    }

    /// <summary> Gets the CSS-Class applied to BocListRows which have a BocListValidationRow associated with it. </summary>
    public string HasValidationRow
    {
      get { return "hasValidationRow"; }
    }

    public virtual string MenuBlock
    {
      get { return "bocListMenuBlock"; }
    }

    public virtual string CommandText
    {
      get { return "bocListCommandText"; }
    }

    /// <summary> Gets the CSS-Class applied to elements only visible to screen readers. </summary>
    /// <remarks> Class: <c>screenReaderText</c> </remarks>
    public string CssClassScreenReaderText
    {
      get { return CssClassDefinition.ScreenReaderText; }
    }

    public string CellStructureElement
    {
      get { return "bocListCellStructureElement"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocList"/>'s table block. </summary>
    /// <param name="hasMenuBlock"><see langword="true" /> if the list has a menu block, otherwise <see langword="false" />.</param>
    /// <param name="hasNavigator"><see langword="true" /> if the list has a navigation block, otherwise <see langword="false" />.</param>
    public string GetTableBlock (bool hasMenuBlock, bool hasNavigator)
    {
      string cssClass = TableBlock;

      if (hasMenuBlock)
        cssClass = cssClass + " " + HasMenuBlock;

      if (hasNavigator)
        cssClass = cssClass + " " + HasNavigator;

      return cssClass;
    }
  }
}
