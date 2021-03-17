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

//  BocListe.ts contains client side scripts used by BocList.

class BocList_SelectedRows
{
  public Selection: number;
  public Length: number;
  public Rows: Dictionary<Nullable<BocList_RowBlock>>;
  public SelectRowSelectorControls: Nullable<JQuery>;
  public SelectAllSelectorControls: Nullable<JQuery>;
  public DataRowCount: number;

  constructor(selection: number)
  {
    this.Selection = selection;
    this.Length = 0;
    this.Rows = {};
    this.SelectRowSelectorControls = null;
    this.SelectAllSelectorControls = null;
    this.DataRowCount = 0;
  }

  public Clear(): void
  {
    this.Length = 0;
    this.Rows = {};
  }

  OnSelectionChanged(boclist: HTMLElement, flag: boolean): void
  {
  }
}

class BocList_RowBlock
{
  public Row: HTMLElement;
  public SelectorControl: JQuery;

  constructor(row: HTMLElement, selectorControl: JQuery)
  {
    if (row.nodeName !== 'TR')
      throw 'Unexpected element type: \'' + row.nodeName + '\'';

    this.Row = row;
    this.SelectorControl = selectorControl
  }
}

type BocList_SelectionChangedHandler = (boclist: HTMLElement, flag: boolean) => void

class BocList
{
  //  A flag that indicates that the OnClick event for an anchor tag (command) has been raised
  //  prior to the row's OnClick event.
  private static _isCommandClick = false;

  //  A flag that indicates that the OnClick event for a selection selectorControl has been raised
  //  prior to the row's OnClick event.
  private static _isRowSelectorClickExecuting = false;

  private static _rowSelectionUndefined = -1;
  private static _rowSelectionDisabled = 0;

  private static _selectedRows: Dictionary<BocList_SelectedRows> = {};

  //  The css class used for rows in their selected state.
  private static TrClassNameSelected = 'selected';

  private static _rowSelectionSingleCheckBox = 1;
  private static _rowSelectionSingleRadioButton = 2;
  private static rowSelectionMultiple = 3;

  //  Initializes the class names of the css classes used to format the table cells.
  //  Call this method once in a startup script.
  public static InitializeGlobals(): void
  {
    BocList._isCommandClick = false;
    BocList._isRowSelectorClickExecuting = false;
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
  public static InitializeList(bocList: HTMLElement, selectRowSelectorControlName: string, selectAllSelectorControlName: string, selection: number, hasClickSensitiveRows: boolean, onSelectionChangedHandler: BocList_SelectionChangedHandler): void
  {
    if (BocList.HasDimensions (bocList))
    {
      $(bocList).addClass('hasDimensions');
      BocList.FixUpScrolling($(bocList));
    }

    var selectedRows = new BocList_SelectedRows (selection);
    if (   selectedRows.Selection != BocList._rowSelectionUndefined
        && selectedRows.Selection != BocList._rowSelectionDisabled)
    {
      selectedRows.SelectRowSelectorControls = $('input[name="' + selectRowSelectorControlName + '"]');
      selectedRows.SelectAllSelectorControls = $('input[name="' + selectAllSelectorControlName + '"]');
      selectedRows.OnSelectionChanged = onSelectionChangedHandler;

      selectedRows.SelectRowSelectorControls.each (function (this: JQuery)
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
          BocList.BindRowClickEventHandler (bocList, tableRow, tableCell, this);
    
        if (this.checked)
        {
          var rowBlock = new BocList_RowBlock(tableRow, this);
          selectedRows.Rows[this.id] = rowBlock;
          selectedRows.Length++;
        }
      });

      BocList.SetSelectAllRowsSelectorOnDemand (selectedRows);
    }
    BocList._selectedRows[bocList.id] = selectedRows;

    selectedRows.OnSelectionChanged (bocList, true);

    // Add diganostic metadata for web testing framework (actually: should only be rendered with IRenderingFeatures.EnableDiagnosticMetadata on)
    $(bocList).attr('data-boclist-is-initialized', 'true');
  }

  private static BindRowClickEventHandler(bocList: HTMLElement, row: HTMLElement, cell: string, selectorControl: JQuery): void
  {
    $(row).click(function (evt)
    {
      var hasSelectionChanged = BocList.OnRowClick (evt, bocList, row, selectorControl);
      if (hasSelectionChanged)
      {
        var selectedRows = BocList._selectedRows[bocList.id]!;
        selectedRows.OnSelectionChanged (bocList, false);
      }
    });
  
    $(selectorControl).click(function (evt)
    {
      evt.stopPropagation();
      BocList.OnRowSelectorClick();
      $(row).trigger ('click');
    });
  
      // Enable the entire selector control's cell for click events that mimic the selector control.
    $(cell).click(function (evt)
    {
      BocList.OnRowSelectorClick();
    });
  
    // Enable the entire row to the left of the selector control's cell for click events that mimic the selector control.
    $(cell).prevAll().click(function (evt)
    {
      BocList.OnRowSelectorClick();
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
  private static OnRowClick(evt: JQueryEventObject, bocList: HTMLElement, currentRow: HTMLElement, selectorControl: JQuery): boolean
  {
    if (BocList._isCommandClick)
    {
      BocList._isCommandClick = false;
      return false;
    }  
  
    var currentRowBlock = new BocList_RowBlock (currentRow, selectorControl);
    var selectedRows = BocList._selectedRows[bocList.id]!;
    var isRowHighlightingEnabled = true
    var isCtrlKeyPress = false;
    if (evt)
      isCtrlKeyPress = evt.ctrlKey;
      
    if (   selectedRows.Selection == BocList._rowSelectionUndefined
        || selectedRows.Selection == BocList._rowSelectionDisabled)
    {
      return false;
    }
      
    if (isCtrlKeyPress || BocList._isRowSelectorClickExecuting)
    {
      //  Is current row selected?
      if (selectedRows.Rows[selectorControl.id] != null)
      {
        //  Remove currentRow from list and unselect it
        BocList.UnselectRow (bocList, currentRowBlock, isRowHighlightingEnabled);
      }
      else
      {
        if (  (   selectedRows.Selection == BocList._rowSelectionSingleCheckBox
               || selectedRows.Selection == BocList._rowSelectionSingleRadioButton)
            && selectedRows.Length > 0)
        {
          //  Unselect all rows and clear the list
          BocList.UnselectAllRows (bocList, isRowHighlightingEnabled);
        }
        //  Add currentRow to list and select it
        BocList.SelectRow (bocList, currentRowBlock, isRowHighlightingEnabled);
      }
    }
    else // cancel previous selection and select a new row
    {
      if (selectedRows.Length > 0)
      {
        //  Unselect all rows and clear the list
        BocList.UnselectAllRows (bocList, isRowHighlightingEnabled);
      }
      //  Add currentRow to list and select it
      BocList.SelectRow (bocList, currentRowBlock, isRowHighlightingEnabled);
    }
    try
    {
      selectorControl.focus();
    }
    catch (e)
    {
    }  
    BocList._isRowSelectorClickExecuting = false;
    return true;
  }

  //  Selects a row.
  //  Adds the row to the _bocList_selectedRows array and increments _bocList_selectedRowsLength.
  //  bocList: The BocList to which the row belongs.
  //  rowBlock: The row to be selected.
  //  isRowHighlightingEnabled: true to mark the selected rows via css-class
  private static SelectRow(bocList: HTMLElement, rowBlock: BocList_RowBlock, isRowHighlightingEnabled: boolean): void
  {
    //  Add currentRow to list  
    var selectedRows = BocList._selectedRows[bocList.id]!;
    selectedRows.Rows[rowBlock.SelectorControl.id] = rowBlock;
    selectedRows.Length++;

    // Select currentRow
    rowBlock.SelectorControl.checked = true;
    if (isRowHighlightingEnabled)
      $(rowBlock.Row).addClass(BocList.TrClassNameSelected);

    BocList.SetSelectAllRowsSelectorOnDemand (selectedRows);
  }

  //  Unselects all rows in a BocList.
  //  Clears _bocList_selectedRows array and sets _bocList_selectedRowsLength to zero.
  //  bocList: The BocList whose rows should be unselected.
  //  isRowHighlightingEnabled: true to update the row's css-class.
  private static UnselectAllRows(bocList: HTMLElement, isRowHighlightingEnabled: boolean): void
  {
    var selectedRows = BocList._selectedRows[bocList.id]!;
    for (var rowID in selectedRows.Rows)
    {
      var rowBlock = selectedRows.Rows[rowID];
      if (rowBlock != null)
      {
        BocList.UnselectRow (bocList, rowBlock, isRowHighlightingEnabled);
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
  private static UnselectRow(bocList: HTMLElement, rowBlock: BocList_RowBlock, isRowHighlightingEnabled: boolean): void
  {
    //  Remove currentRow from list
    var selectedRows = BocList._selectedRows[bocList.id]!;
    selectedRows.Rows[rowBlock.SelectorControl.id] = null;
    selectedRows.Length--;

    // Unselect currentRow
    rowBlock.SelectorControl.checked = false;
    if (isRowHighlightingEnabled)
      $(rowBlock.Row).removeClass(BocList.TrClassNameSelected);

    BocList.ClearSelectAllRowsSelector (selectedRows);
  }

  private static SetSelectAllRowsSelectorOnDemand (selectedRows: BocList_SelectedRows): void
  {
    if (selectedRows.DataRowCount == selectedRows.Length && selectedRows.DataRowCount > 0)
      selectedRows.SelectAllSelectorControls!.each (function (this: JQuery) { this.checked = true; }); // TODO RM-7711 - Move BocList's Row Selection logic to TypeScript class 'BocList_SelectedRows'.
  }

  private static ClearSelectAllRowsSelector (selectedRows: BocList_SelectedRows): void
  {
    selectedRows.SelectAllSelectorControls!.each (function (this: JQuery) { this.checked = false; }); // TODO RM-7711 - Move BocList's Row Selection logic to TypeScript class 'BocList_SelectedRows'.
  }

  //  Event handler for the selection selectorControl in the title row.
  //  Applies the checked state of the title's selectorControl to all data rows' selectu=ion selectorControles.
  //  bocList: The BocList to which the selectorControl belongs.
  //  selectRowControlName: The name of the row selector controls.
  //  isRowHighlightingEnabled: true to enable highting of the rows
  public static OnSelectAllSelectorControlClick (bocList: HTMLElement, selectAllSelectorControl: JQuery, isRowHighlightingEnabled: boolean): void
  {
    var selectedRows = BocList._selectedRows[bocList.id]!;

    if (selectedRows.Selection != BocList.rowSelectionMultiple)
      return;
    //  BocList_SelectRow will increment the length, therefor initialize it to zero.
    if (selectAllSelectorControl.checked)
      selectedRows.Length = 0;

    selectedRows.SelectRowSelectorControls!.each (function (this: JQuery) // TODO RM-7711 - Move BocList's Row Selection logic to TypeScript class 'BocList_SelectedRows'.
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
        BocList.SelectRow (bocList, rowBlock, isRowHighlightingEnabled);
      else
        BocList.UnselectRow (bocList, rowBlock, isRowHighlightingEnabled);
    });
    
    if (! selectAllSelectorControl.checked)
      selectedRows.Length = 0;
  
    selectedRows.OnSelectionChanged (bocList, false);
  }

  //  Event handler for the selection selectorControl in a data row.
  //  Sets the _bocList_isSelectorControlClick flag.
  private static OnRowSelectorClick(): void
  {
    BocList._isRowSelectorClickExecuting = true;
  }

  //  Event handler for the anchor tags (commands) in a data row.
  //  Sets the BocList._isCommandClick flag.
  public static OnCommandClick(): void
  {
    BocList._isCommandClick = true;
  }

  //  Returns the number of rows selected for the specified BocList
  public static GetSelectionCount(bocListID: string): number
  {
    var selectedRows = BocList._selectedRows[bocListID];
    if (selectedRows == null)
      return 0;
    return selectedRows.Length;
  }

  private static HasDimensions(bocList: HTMLElement): boolean
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

  private static FixUpScrolling(bocList: JQuery): void
  {
    var tableBlock = bocList.children('div.bocListTableBlock').first();

    var scrollTimer: Nullable<number> = null;
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
        BocList.FixHeaderPosition(tableContainer, scrollableContainer);
  
        // Final update after scrolling has finished to ensure propper layout.
        if (scrollTimer)
          clearTimeout(scrollTimer);
        scrollTimer = setTimeout(function () { BocList.FixHeaderPosition (tableContainer, scrollableContainer); }, 50);
      }
    });
  
    BocList.CreateFakeTableHead(tableContainer, scrollableContainer, bocList);
  
    var resizeInterval = 50;
    var resizeHandler = function ()
    {
      if (!PageUtility.Instance.IsInDom (scrollableContainer[0]))
        return;
  
      BocList.FixHeaderSize(scrollableContainer);
      BocList.FixHeaderPosition(tableContainer, scrollableContainer);
      setTimeout(resizeHandler, resizeInterval);
    };
    resizeHandler();
  }

  private static CreateFakeTableHead(tableContainer: JQuery, scrollableContainer: JQuery, bocList: JQuery): void
  {
    if (BrowserUtility.GetIEVersion() > 0)
    {
      // For Internet Explorer + JAWS 2018ff, the tabindex-attribute on the table root will break a table with a scrollable header part.
      tableContainer.removeAttr('tabindex');
      var captionLabelIDs = bocList.attr('aria-labelledby');
      var lastCaptionLabelID = captionLabelIDs.split(' ').slice(-1)[0];
      var captionElement = document.getElementById (lastCaptionLabelID)!;
      var tableCaption = $ ('<span/>')
        .attr ({
          'tabindex' : 0,
          'aria-label': captionElement.innerText,
          'aria-hidden' : 'true'
        })
        .addClass ('screenReaderText');
      tableContainer.before (tableCaption);
    }
    // Add diganostic metadata for web testing framework (actually: should only be rendered with IRenderingFeatures.EnableDiagnosticMetadata on)
    tableContainer.attr('data-boclist-has-fake-table-head', 'true');
  
    var table = scrollableContainer.children('table').first();
  
    var fakeTable = $('<table/>');
    fakeTable.attr({
        'role' : 'none',
        'class' : table.attr('class'), 
        cellPadding: 0,
        cellSpacing: 0
      });
    fakeTable.css({ width: '100%' });
  
    var realTableHead = table.children('thead').first();
    var fakeTableHead = realTableHead.clone(true, true);
    realTableHead.attr({
      'aria-hidden': 'true',
      'role': 'none'
    });
    realTableHead.find ('*[role]').attr ({ 'role' : 'none' });
    fakeTable.append(fakeTableHead);
  
    var fakeTableHeadWidthContainer = $('<div/>').attr ({ 'role' : 'none' });
    fakeTableHeadWidthContainer.append(fakeTable);
  
    var fakeTableHeadContainer = $('<div/>').attr({ 'role': 'none', 'class': 'bocListFakeTableHead' });
    fakeTableHeadContainer.hide();
    fakeTableHeadContainer.append (fakeTableHeadWidthContainer);
  
    realTableHead.find('*').each(function (this: HTMLElement) { $(this).removeAttr('id').attr({ tabIndex: -1 }).attr({ tabIndex: -1 }); });
  
    scrollableContainer.before(fakeTableHeadContainer);
  
    // sync checkboxes
    var checkboxes = fakeTableHead.find("th input:checkbox");
    checkboxes.click(function (this: HTMLElement)
    {
      var checkName = $(this).attr('name');
      var checkStatus = $(this).prop('checked');
      $('input[name="' + checkName + '"]').prop('checked', checkStatus);
    });
    // BocList_FixHeaderSize() with timeout needs to be called after setup. This is already taken care of at the call-site.
  }

  private static FixHeaderSize(scrollableContainer: JQuery): void
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
    realTableHeadRowChildren.each(function (this: HTMLElement, index)
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

  private static FixHeaderPosition(tableContainer: JQuery, scrollableContainerJQuery: JQuery): void
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

  public static InitializeNavigationBlock (pageNumberField: JQuery, pageIndexField: JQuery): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsObject ('pageNumberField', pageNumberField);
    ArgumentUtility.CheckNotNullAndTypeIsObject('pageIndexField', pageIndexField);
  
    pageNumberField.bind('change', function () {
  
      var pageNumber = parseInt(pageNumberField.val(), 10);
      if (isNaN (pageNumber) || !TypeUtility.IsInteger (pageNumber))
      {
        if (pageNumberField.val().length > 0)
          setTimeout(function () { pageNumberField.focus(); }, 0);
  
        pageNumberField.val('' + (parseInt(pageIndexField.val(), 10) + 1)); // TODO RM-7699 - Incorrect style assignments in multiple TypeScript files
        return false;
      }
      else
      {
        pageIndexField.val ('' + (pageNumber - 1)); // TODO RM-7699 - Incorrect style assignments in multiple TypeScript files
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
}

function BocList_InitializeGlobals(): void
{
  BocList.InitializeGlobals();
}

function BocList_InitializeList(bocList: HTMLElement, selectRowSelectorControlName: string, selectAllSelectorControlName: string, selection: number, hasClickSensitiveRows: boolean, onSelectionChangedHandler: { (boclist: HTMLElement, flag: boolean): void }): void
{
  BocList.InitializeList(bocList, selectRowSelectorControlName, selectAllSelectorControlName, selection, hasClickSensitiveRows, onSelectionChangedHandler)
}

function BocList_OnSelectAllSelectorControlClick(bocList: HTMLElement, selectAllSelectorControl: JQuery, isRowHighlightingEnabled: boolean): void
{
  BocList.OnSelectAllSelectorControlClick(bocList, selectAllSelectorControl, isRowHighlightingEnabled)
}

function BocList_OnCommandClick(): void
{
  BocList.OnCommandClick()
}

function BocList_GetSelectionCount (bocListID: string): number
{
  return BocList.GetSelectionCount (bocListID)
}

function BocListNavigationBlock_Initialize(pageNumberField: JQuery, pageIndexField: JQuery): void
{
  BocList.InitializeNavigationBlock (pageNumberField, pageIndexField)
}
