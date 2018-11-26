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

//  BocListe.js contains client side scripts used by BocList.

//  The css class used for rows in their selected state.
var _bocList_TrClassNameSelected = 'selected';

//  Associative array: <BocList ID>, <BocList_SelectedRows>
var _bocList_selectedRows = new Object();

//  A flag that indicates that the OnClick event for an anchor tag (command) has been raised
//  prior to the row's OnClick event.
var _bocList_isCommandClick = false;

//  A flag that indicates that the OnClick event for a selection selectorControl has been raised
//  prior to the row's OnClick event.
var _bocList_isRowSelectorClickExecuting = false;

var _bocList_rowSelectionUndefined = -1;
var _bocList_rowSelectionDisabled = 0;
var _bocList_rowSelectionSingleCheckBox = 1;
var _bocList_rowSelectionSingleRadioButton = 2;
var _bocList_rowSelectionMultiple = 3;

function BocList_SelectedRows (selection)
{
  this.Selection = selection;
  //  Associative Array: <SelectorControl ID>, <BocList_RowBlock>
  this.Length = 0;
  this.Rows = new Object();
  this.Clear = function ()
  {
    this.Length = 0;
    this.Rows = new Object();
  };
  this.SelectRowSelectorControls = null;
  this.SelectAllSelectorControls = null;
  this.DataRowCount = 0;
  this.OnSelectionChanged = function () {};
}

function BocList_RowBlock (row, selectorControl)
{
  if (row.nodeName !== 'TR')
    throw 'Unexpected element type: \'' + row.nodeName + '\'';

  this.Row = row;
  this.SelectorControl = selectorControl;
}

//  Initializes the class names of the css classes used to format the table cells.
//  Call this method once in a startup script.
function BocList_InitializeGlobals ()
{
  _bocList_isCommandClick = false;
  _bocList_isRowSelectorClickExecuting = false;
}

//  Initalizes an individual BocList's List. The initialization synchronizes the selection state 
//  arrays with the BocList's selected rows.
//  Call this method once for each BocList on the page.
//  bocList: The BocList to which the row belongs.
//  selectRowSelectorControlName: The name of the row selector controls.
//  selectAllSelectorControlName: The name of the select-all selector control.
//  selection: The RowSelection enum value defining the selection mode (disabled/single/multiple)
//  hasClickSensitiveRows: true if the click event handler is bound to the data rows.
//  onSelectionChangedHandler: A function to be invoked when the BocList's selection changes. 
//                             First argument is the BocList, second argument is a flag indicating whether the callback is invoked during initialization.
function BocList_InitializeList(bocList, selectRowSelectorControlName, selectAllSelectorControlName, selection, hasClickSensitiveRows, onSelectionChangedHandler)
{
  if (BocList_HasDimensions (bocList))
  {
    $(bocList).addClass('hasDimensions');
    BocList_FixUpScrolling($(bocList));
  }

  var selectedRows = new BocList_SelectedRows (selection);
  if (   selectedRows.Selection != _bocList_rowSelectionUndefined
      && selectedRows.Selection != _bocList_rowSelectionDisabled)
  {
    selectedRows.SelectRowSelectorControls = $('input[name="' + selectRowSelectorControlName + '"]');
    selectedRows.SelectAllSelectorControls = $('input[name="' + selectAllSelectorControlName + '"]');
    selectedRows.OnSelectionChanged = onSelectionChangedHandler;

    selectedRows.SelectRowSelectorControls.each (function ()
    {
      selectedRows.DataRowCount++;

      var checkBox = this;

      var tableCell = checkBox.parentNode;
      if (tableCell.nodeName !== 'TD')
        throw 'Unexpected element type: \'' + tableCell.nodeName + '\'';

      var tableRow = tableCell.parentNode;
      if (tableRow.nodeName !== 'TR')
        throw 'Unexpected element type: \'' + tableRow.nodeName + '\'';

      if (hasClickSensitiveRows)
        BocList_BindRowClickEventHandler (bocList, tableRow, tableCell, this);
  
      if (this.checked)
      {
        var rowBlock = new BocList_RowBlock (tableRow, this);
        selectedRows.Rows[this.id] = rowBlock;
        selectedRows.Length++;
      }
    });

    BocList_SetSelectAllRowsSelectorOnDemand (selectedRows);
  }
  _bocList_selectedRows[bocList.id] = selectedRows;

  selectedRows.OnSelectionChanged (bocList, true);

  // Add diganostic metadata for web testing framework (actually: should only be rendered with IRenderingFeatures.EnableDiagnosticMetadata on)
  $(bocList).attr('data-boclist-is-initialized', 'true');
}

function BocList_BindRowClickEventHandler(bocList, row, cell, selectorControl)
{
  $(row).click(function (evt)
  {
    var hasSelectionChanged = BocList_OnRowClick (evt, bocList, row, selectorControl);
    if (hasSelectionChanged)
    {
      var selectedRows = _bocList_selectedRows[bocList.id];
      selectedRows.OnSelectionChanged (bocList, false);
    }
  });

  $(selectorControl).click(function (evt)
  {
    evt.stopPropagation();
    BocList_OnRowSelectorClick();
    $(row).trigger ('click');
  });

    // Enable the entire selector control's cell for click events that mimic the selector control.
  $(cell).click(function (evt)
  {
    BocList_OnRowSelectorClick();
  });

  // Enable the entire row to the left of the selector control's cell for click events that mimic the selector control.
  $(cell).prevAll().click(function (evt)
  {
    BocList_OnRowSelectorClick();
  });
}

//  Event handler for a table row in the BocList. 
//  Selects/unselects a row/all rows depending on its selection state,
//      whether CTRL has been pressed and if _bocList_isSelectorControlClick is true.
//  Aborts the execution if _bocList_isCommandClick or _bocList_isSelectorControlClick is true.
//  evt: The jQuery event object representing the click event
//  bocList: The BocList to which the row belongs.
//  currentRow: The row that fired the click event.
//  selectorControl: The selection selectorControl in this row.
//  returns: true if the row selection has changed.
function BocList_OnRowClick (evt, bocList, currentRow, selectorControl)
{
  if (_bocList_isCommandClick)
  {
    _bocList_isCommandClick = false;
    return false;
  }  

  var currentRowBlock = new BocList_RowBlock (currentRow, selectorControl);
  var selectedRows = _bocList_selectedRows[bocList.id];
  var isRowHighlightingEnabled = true
  var isCtrlKeyPress = false;
  if (evt)
    isCtrlKeyPress = evt.ctrlKey;
    
  if (   selectedRows.Selection == _bocList_rowSelectionUndefined
      || selectedRows.Selection == _bocList_rowSelectionDisabled)
  {
    return false;
  }
    
  if (isCtrlKeyPress || _bocList_isRowSelectorClickExecuting)
  {
    //  Is current row selected?
    if (selectedRows.Rows[selectorControl.id] != null)
    {
      //  Remove currentRow from list and unselect it
      BocList_UnselectRow (bocList, currentRowBlock, isRowHighlightingEnabled);
    }
    else
    {
      if (  (   selectedRows.Selection == _bocList_rowSelectionSingleCheckBox
             || selectedRows.Selection == _bocList_rowSelectionSingleRadioButton)
          && selectedRows.Length > 0)
      {
        //  Unselect all rows and clear the list
        BocList_UnselectAllRows (bocList, isRowHighlightingEnabled);
      }
      //  Add currentRow to list and select it
      BocList_SelectRow (bocList, currentRowBlock, isRowHighlightingEnabled);
    }
  }
  else // cancel previous selection and select a new row
  {
    if (selectedRows.Length > 0)
    {
      //  Unselect all rows and clear the list
      BocList_UnselectAllRows (bocList, isRowHighlightingEnabled);
    }
    //  Add currentRow to list and select it
    BocList_SelectRow (bocList, currentRowBlock, isRowHighlightingEnabled);
  }
  try
  {
    selectorControl.focus();
  }
  catch (e)
  {
  }  
  _bocList_isRowSelectorClickExecuting = false;
  return true;
}

//  Selects a row.
//  Adds the row to the _bocList_selectedRows array and increments _bocList_selectedRowsLength.
//  bocList: The BocList to which the row belongs.
//  rowBlock: The row to be selected.
//  isRowHighlightingEnabled: true to mark the selected rows via css-class
function BocList_SelectRow (bocList, rowBlock, isRowHighlightingEnabled)
{
  //  Add currentRow to list  
  var selectedRows = _bocList_selectedRows[bocList.id];
  selectedRows.Rows[rowBlock.SelectorControl.id] = rowBlock;
  selectedRows.Length++;

  // Select currentRow
  rowBlock.SelectorControl.checked = true;
  if (isRowHighlightingEnabled)
    $ (rowBlock.Row).addClass (_bocList_TrClassNameSelected);

  BocList_SetSelectAllRowsSelectorOnDemand (selectedRows);
}

//  Unselects all rows in a BocList.
//  Clears _bocList_selectedRows array and sets _bocList_selectedRowsLength to zero.
//  bocList: The BocList whose rows should be unselected.
//  isRowHighlightingEnabled: true to update the row's css-class.
function BocList_UnselectAllRows (bocList, isRowHighlightingEnabled)
{
  var selectedRows = _bocList_selectedRows[bocList.id];
  for (var rowID in selectedRows.Rows)
  {
    var rowBlock = selectedRows.Rows[rowID];
    if (rowBlock != null)
    {
      BocList_UnselectRow (bocList, rowBlock, isRowHighlightingEnabled);
    }
  }
  
  //  Start over with a new array
  selectedRows.Clear();
}

//  Unselects a row.
//  Removes the row frin the _bocList_selectedRows array and decrements _bocList_selectedRowsLength.
//  bocList: The BocList to which the row belongs.
//  rowBlock: The row to be unselected.
//  isRowHighlightingEnabled: true to update the row's css-class.
function BocList_UnselectRow (bocList, rowBlock, isRowHighlightingEnabled)
{
  //  Remove currentRow from list
  var selectedRows = _bocList_selectedRows[bocList.id];
  selectedRows.Rows[rowBlock.SelectorControl.id] = null;
  selectedRows.Length--;
    
  // Unselect currentRow
  rowBlock.SelectorControl.checked = false;
  if (isRowHighlightingEnabled)
    $(rowBlock.Row).removeClass(_bocList_TrClassNameSelected);

  BocList_ClearSelectAllRowsSelector (selectedRows);
}

function BocList_SetSelectAllRowsSelectorOnDemand (selectedRows)
{
  if (selectedRows.DataRowCount == selectedRows.Length && selectedRows.DataRowCount > 0)
    selectedRows.SelectAllSelectorControls.each (function () { this.checked = true; });
}

function BocList_ClearSelectAllRowsSelector (selectedRows)
{
  selectedRows.SelectAllSelectorControls.each (function () { this.checked = false; });
}

//  Event handler for the selection selectorControl in the title row.
//  Applies the checked state of the title's selectorControl to all data rows' selectu=ion selectorControles.
//  bocList: The BocList to which the selectorControl belongs.
//  selectRowControlName: The name of the row selector controls.
//  isRowHighlightingEnabled: true to enable highting of the rows
function BocList_OnSelectAllSelectorControlClick(bocList, selectAllSelectorControl, isRowHighlightingEnabled)
{
  var selectedRows = _bocList_selectedRows[bocList.id];

  if (selectedRows.Selection != _bocList_rowSelectionMultiple)
    return;
  //  BocList_SelectRow will increment the length, therefor initialize it to zero.
  if (selectAllSelectorControl.checked)
    selectedRows.Length = 0;

  selectedRows.SelectRowSelectorControls.each (function ()
  {
    var checkBox = this;

    var tableCell = checkBox.parentNode;
    if (tableCell.nodeName !== 'TD')
      throw 'Unexpected element type: \'' + tableCell.nodeName + '\'';

    var tableRow = tableCell.parentNode;
    if (tableRow.nodeName !== 'TR')
      throw 'Unexpected element type: \'' + tableRow.nodeName + '\'';

    var rowBlock = new BocList_RowBlock (tableRow, checkBox);
    if (selectAllSelectorControl.checked)
      BocList_SelectRow (bocList, rowBlock, isRowHighlightingEnabled);
    else
      BocList_UnselectRow (bocList, rowBlock, isRowHighlightingEnabled);
  });
  
  if (! selectAllSelectorControl.checked)
    selectedRows.Length = 0;

  selectedRows.OnSelectionChanged (bocList, false);
}

//  Event handler for the selection selectorControl in a data row.
//  Sets the _bocList_isSelectorControlClick flag.
function BocList_OnRowSelectorClick()
{
  _bocList_isRowSelectorClickExecuting = true;
}

//  Event handler for the anchor tags (commands) in a data row.
//  Sets the _bocList_isCommandClick flag.
function BocList_OnCommandClick()
{
  _bocList_isCommandClick = true;
}

//  Returns the number of rows selected for the specified BocList
function BocList_GetSelectionCount (bocListID)
{
  var selectedRows = _bocList_selectedRows[bocListID];
  if (selectedRows == null)
    return 0;
  return selectedRows.Length;
}

function BocList_HasDimensions(bocList)
{
  var heightFromAttribute = $(bocList).attr('height');
  if (TypeUtility.IsDefined(heightFromAttribute) && heightFromAttribute != '')
    return true;

  var heightFromInlineStyle = bocList.style.height;
  if (TypeUtility.IsDefined(heightFromInlineStyle) && heightFromInlineStyle != '')
    return true;

  var widthFromAttribute = $(bocList).attr('width');
  if (TypeUtility.IsDefined(widthFromAttribute) && widthFromAttribute != '')
    return true;

  var widthFromInlineStyle = bocList.style.width;
  if (TypeUtility.IsDefined(widthFromInlineStyle) && widthFromInlineStyle != '')
    return true;

  var referenceHeight = 0;
  var referenceWidth = 0;
  var tempList = $("<div/>").attr("class", $(bocList).prop("class")).css("display", "none");

  // Catch styles applied to pseudo-selectors starting at the first element in the DOM collection
  tempList.insertBefore($(bocList));

  try
  {
    if (tempList.height() > referenceHeight)
      return true;

    if (tempList.width() > referenceWidth)
      return true;
  } 
  finally
  {
    tempList.remove();
  }

  // Catch styles applied to pseudo-selectors starting at the last element in the DOM collection
  tempList.insertAfter($(bocList));

  try
  {
    if (tempList.height() > referenceHeight)
      return true;

    if (tempList.width() > referenceWidth)
      return true;
  }
  finally
  {
    tempList.remove();
  }

  return false;
}

function BocList_FixUpScrolling(bocList)
{
  var tableBlock = bocList.children('div.bocListTableBlock').first();

  var scrollTimer = null;
  var tableContainer = tableBlock.children('div.bocListTableContainer').first();
  var scrollableContainer = tableContainer.children('div.bocListTableScrollContainer').first();
  var horizontalScroll = 0;

  scrollableContainer.bind('scroll', function (event)
  {
    var newHorizontalScroll = scrollableContainer.scrollLeft();
    var hasHorizontalScrollUpdated = horizontalScroll != newHorizontalScroll;
    horizontalScroll = newHorizontalScroll;

    if (hasHorizontalScrollUpdated)
    {
      // Continuous refresh.
      BocList_FixHeaderPosition(tableContainer, scrollableContainer);

      // Final update after scrolling has finished to ensure propper layout.
      if (scrollTimer)
        clearTimeout(scrollTimer);
      scrollTimer = setTimeout(function () { BocList_FixHeaderPosition (tableContainer, scrollableContainer); }, 50);
    }
  });

  BocList_CreateFakeTableHead(tableContainer, scrollableContainer);

  var resizeInterval = 50;
  var resizeHandler = function ()
  {
    if (!PageUtility.Instance.IsInDom (scrollableContainer[0]))
      return;

    BocList_FixHeaderSize(scrollableContainer);
    BocList_FixHeaderPosition(tableContainer, scrollableContainer);
    setTimeout(resizeHandler, resizeInterval);
  };
  resizeHandler();
}

function BocList_CreateFakeTableHead(tableContainer, scrollableContainer)
{
  // Add diganostic metadata for web testing framework (actually: should only be rendered with IRenderingFeatures.EnableDiagnosticMetadata on)
  tableContainer.attr('data-boclist-has-fake-table-head', 'true');

  var table = scrollableContainer.children('table').first();

  var fakeTable = $('<table/>');
  fakeTable.attr({
      'role' : 'presentation',
      'class' : table.attr('class'), 
      cellPadding: 0,
      cellSpacing: 0
    });
  fakeTable.css({ width: '100%' });

  var realTableHead = table.children('thead').first();
  var fakeTableHead = realTableHead.clone(true, true);
  realTableHead.attr({
    'aria-hidden': 'true'
  });
  fakeTable.append(fakeTableHead);

  var fakeTableHeadWidthContainer = $('<div/>');
  fakeTableHeadWidthContainer.append(fakeTable);

  var fakeTableHeadContainer = $('<div/>').attr({ 'class': 'bocListFakeTableHead' });
  fakeTableHeadContainer.hide();
  fakeTableHeadContainer.append (fakeTableHeadWidthContainer);

  realTableHead.find('*').each(function () { $(this).removeAttr('id').attr({ tabIndex: -1 }).attr({ tabIndex: -1 }); });

  scrollableContainer.before(fakeTableHeadContainer);

  // sync checkboxes
  var checkboxes = fakeTableHead.find("th input:checkbox");
  checkboxes.click(function ()
  {
    var checkName = $(this).attr('name');
    var checkStatus = $(this).prop('checked');
    $('input[name="' + checkName + '"]').prop('checked', checkStatus);
  });
  // BocList_FixHeaderSize() with timeout needs to be called after setup. This is already taken care of at the call-site.
}

function BocList_FixHeaderSize(scrollableContainer)
{
  var realTable = scrollableContainer.children('table').first();
  var realTableWidth = realTable[0].offsetWidth;
  var previousRealTableWidth = realTable.data("bocListPreviousRealTableWidth");
  if (previousRealTableWidth == realTableWidth)
    return;
  realTable.data("bocListPreviousRealTableWidth", realTableWidth);

  var realTableHead = realTable.eq(0).find('thead');
  var realTableHeadRow = realTableHead.children().eq(0);
  var realTableHeadRowChildren = realTableHeadRow.children();

  var fakeTableHeadContainer = scrollableContainer.parent().children('div.bocListFakeTableHead').first();
  var fakeTableHead = fakeTableHeadContainer.find('thead');
  var fakeTableHeadRow = fakeTableHead.children().eq(0);
  var fakeTableHeadRowChildren = fakeTableHeadRow.children();

  // store cell widths in array
  var realTableHeadCellWidths = new Array();
  realTableHeadRowChildren.each(function (index)
  {
    realTableHeadCellWidths[index] = $(this).width();
  });

  // apply widths to fake header
  fakeTableHeadRowChildren.width(function (index, itemWidth)
  {
    var width = realTableHeadCellWidths[index];
    return width;
  });

  var fakeTableHeadWidthContainer = fakeTableHeadContainer.children ('div').first();
  fakeTableHeadWidthContainer.width(realTableWidth);
  var fakeTableHeadContainerHeight = fakeTableHeadContainer.height();
  scrollableContainer.css({ top: fakeTableHeadContainerHeight});
  realTable.css({ 'margin-top': fakeTableHeadContainerHeight * -1 });

  fakeTableHeadContainer.show();
}

function BocList_FixHeaderPosition(tableContainer, scrollableContainerJQuery)
{
  var scrollableContainer = scrollableContainerJQuery[0];
  var fakeTableHeadContainer = tableContainer.children('div.bocListFakeTableHead').first()[0];
  var scrollLeft = scrollableContainer.scrollLeft;
  var previousScrollLeft = scrollableContainerJQuery.data("bocListPreviousScrollLeft");
  var fakeTableHeadScrollLeft = fakeTableHeadContainer.scrollLeft;

  var hasScrollMoveFromScrollbar = previousScrollLeft !== scrollLeft;
  var hasScrollMoveFromColumnHeader = previousScrollLeft !== fakeTableHeadScrollLeft;
  if (!hasScrollMoveFromScrollbar && !hasScrollMoveFromColumnHeader)
    return;

  scrollLeft = hasScrollMoveFromColumnHeader ? fakeTableHeadScrollLeft : scrollLeft;
  scrollableContainerJQuery.data("bocListPreviousScrollLeft", scrollLeft);

  if (hasScrollMoveFromColumnHeader)
    scrollableContainer.scrollLeft = scrollLeft;

  if (hasScrollMoveFromScrollbar)
    fakeTableHeadContainer.scrollLeft = scrollLeft;
}

function BocListNavigationBlock_Initialize(pageNumberField, pageIndexField)
{
  ArgumentUtility.CheckNotNullAndTypeIsObject ('pageNumberField', pageNumberField);
  ArgumentUtility.CheckNotNullAndTypeIsObject('pageIndexField', pageIndexField);

  pageNumberField.bind('change', function () {

    var pageNumber = parseInt(pageNumberField.val(), 10);
    if (isNaN (pageNumber) || !TypeUtility.IsInteger (pageNumber))
    {
      if (pageNumberField.val().length > 0)
        setTimeout(function () { pageNumberField.focus(); }, 0);

      pageNumberField.val(parseInt(pageIndexField.val(), 10) + 1);
      return false;
    }
    else
    {
      pageIndexField.val (pageNumber - 1);
      pageIndexField.trigger('change');
      return true;
    }
  });

  pageNumberField.bind('keydown', function (event) {
    var enterKey = 13;
    var zeroKey = 48;
    var nineKey = 57;
    var zeroKeyNumBlock = 96;
    var nineKeyNumBlock = 105;
    var f1Key = 112;
    var f12Key = 123;
    var isEnterKey = event.keyCode == enterKey;
    var isControlKey = event.keyCode < zeroKey || event.keyCode >= f1Key && event.keyCode <= f12Key;
    var isNumericKey = event.keyCode >= zeroKey && event.keyCode <= nineKey || event.keyCode >= zeroKeyNumBlock && event.keyCode <= nineKeyNumBlock;

    if (isEnterKey)
    {
      pageNumberField.trigger("change");
      event.cancelBubble = true;
      event.stopPropagation();
      return false;
    }
    else if (event.altKey || event.ctrlKey || isControlKey || isNumericKey)
    {
      return true;
    }
    else
    {
      return false;
    }
  });
}