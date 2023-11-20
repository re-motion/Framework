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

namespace Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata
{
  /// <summary>
  /// Diagnostic metadata attribute names used by various renderers.
  /// </summary>
  public static class DiagnosticMetadataAttributesForObjectBinding
  {
    public static readonly string DisplayName = "data-display-name";
    public static readonly string IsBound = "data-is-bound";
    public static readonly string BoundType = "data-bound-type";
    public static readonly string BoundProperty = "data-bound-property";
    public static readonly string HasPropertyPaths = "data-has-propertypaths";
    public static readonly string BoundPropertyPaths = "data-bound-propertypaths";
    public static readonly string NullIdentifier = "data-null-identifier";
    public static readonly string ValidationFailureSourceBusinessObject = "data-validation-failure-business-object";
    public static readonly string ValidationFailureSourceProperty = "data-validation-failure-property";

    public static readonly string BocBooleanValueIsTriState = "data-bocbooleanvalue-is-tristate";

    public static readonly string BocDateTimeValueHasTimeField = "data-bocdatetimevalue-has-timefield";
    public static readonly string BocDateTimeValueDateField = "data-bocdatetimevalue-datefield";
    public static readonly string BocDateTimeValueTimeField = "data-bocdatetimevalue-timefield";
    public static readonly string BocDateTimeValueTimeFieldHasSeconds = "data-bocdatetimevalue-timefield-has-seconds";

    public static readonly string BocEnumValueStyle = "data-bocenumvalue-style";

    public static readonly string BocListCellContents = "data-boclist-cell-contents";
    public static readonly string BocListCellIndex = "data-boclist-cell-index";
    public static readonly string BocListColumnHasContentAttribute = "data-boclist-column-has-content-attribute";
    public static readonly string BocListColumnIsRowHeader = "data-boclist-column-is-row-header";
    // Note: do not change value without changing usages in JavaScript files.
    public static readonly string BocListHasFakeTableHead = "data-boclist-has-fake-table-head";
    // Note: do not change value without changing usages in JavaScript files.
    public static readonly string BocListIsInitialized = "data-boclist-is-initialized";
    public static readonly string BocListNumberOfPages = "data-boclist-number-of-pages";
    public static readonly string BocListCurrentPageNumber = "data-boclist-current-page-number";
    public static readonly string BocListRowIndex = "data-boclist-row-index";
    public static readonly string BocListIsEditModeActive = "data-boclist-is-edit-mode-active";
    public static readonly string BocListHasNavigationBlock = "data-boclist-has-navigator";
    public static readonly string BocListWellKnownEditCell = "data-boclist-wellknown-cell-edit";
    public static readonly string BocListWellKnownRowDropDownMenuCell = "data-boclist-wellknown-cell-dropdownmenu";
    public static readonly string BocListWellKnownSelectAllControl = "data-boclist-wellknown-selectall-control";
    public static readonly string BocListValidationFailureSourceRow = "data-boclist-validation-failure-source-row";
    public static readonly string BocListValidationFailureSourceColumn = "data-boclist-validation-failure-source-column";
  }
}
