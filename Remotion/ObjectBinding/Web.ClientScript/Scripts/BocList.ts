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
  public SelectRowSelectorControls: Nullable<NodeListOf<HTMLInputElement>>;
  public SelectAllSelectorControls: Nullable<NodeListOf<HTMLInputElement>>;
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
  public SelectorControl: HTMLInputElement;
  public ValidationRow: Nullable<HTMLElement>
  constructor(row: HTMLElement, selectorControl: HTMLInputElement)
  {
    if (row.nodeName !== 'TR')
      throw 'Unexpected element type: \'' + row.nodeName + '\'';

    this.Row = row;
    this.SelectorControl = selectorControl
    this.ValidationRow = this.Row.classList.contains("hasValidationRow")
        ? this.Row.nextSibling as HTMLElement
        : null;
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
  public static InitializeList(bocListOrSelector: CssSelectorOrElement<HTMLElement>, selectRowSelectorControlName: string, selectAllSelectorControlName: string, selection: number, hasClickSensitiveRows: boolean, onSelectionChangedHandler: BocList_SelectionChangedHandler): void
  {
    ArgumentUtility.CheckNotNull("bocListOrSelector", bocListOrSelector);
    const bocList = ElementResolverUtility.ResolveSingle(bocListOrSelector);

    if (BocList.HasDimensions (bocList))
    {
      bocList.classList.add('hasDimensions')
      BocList.FixUpScrolling(bocList);
    }

    BocList.FixupValidationErrorOverflowSize(bocList);

    const selectedRows = new BocList_SelectedRows (selection);
    if (   selectedRows.Selection != BocList._rowSelectionUndefined
        && selectedRows.Selection != BocList._rowSelectionDisabled)
    {
      selectedRows.SelectRowSelectorControls = document.querySelectorAll<HTMLInputElement>('input[name="' + selectRowSelectorControlName + '"]')!;
      selectedRows.SelectAllSelectorControls = document.querySelectorAll<HTMLInputElement>('input[name="' + selectAllSelectorControlName + '"]')!;
      selectedRows.OnSelectionChanged = onSelectionChangedHandler;

      const visualizer = document.querySelectorAll('input[name="' + selectAllSelectorControlName + '"] + span')!;
      visualizer.forEach(checkbox =>
      {
        checkbox.addEventListener('click', function (evt)
        {
          evt.stopPropagation();
          selectedRows.SelectAllSelectorControls![0]!.click();
        });
      });

      selectedRows.SelectRowSelectorControls.forEach (checkBox =>
      {
        selectedRows.DataRowCount++;

        const tableCell = checkBox.parentNode as HTMLElement;
        if (tableCell.nodeName !== 'TD')
          throw 'Unexpected element type: \'' + tableCell.nodeName + '\'';

        const tableRow = tableCell.parentNode as HTMLElement;
        if (tableRow.nodeName !== 'TR')
          throw 'Unexpected element type: \'' + tableRow.nodeName + '\'';

        if (hasClickSensitiveRows)
          BocList.BindRowClickEventHandler (bocList, tableRow, tableCell, checkBox);

        if (checkBox.checked)
        {
          const rowBlock = new BocList_RowBlock(tableRow, checkBox);
          selectedRows.Rows[checkBox.id] = rowBlock;
          selectedRows.Length++;
        }
      });

      BocList.SetSelectAllRowsSelectorOnDemand (selectedRows);
    }
    BocList._selectedRows[bocList.id] = selectedRows;

    selectedRows.OnSelectionChanged (bocList, true);

    // Add diganostic metadata for web testing framework (actually: should only be rendered with IRenderingFeatures.EnableDiagnosticMetadata on)
    bocList.setAttribute('data-boclist-is-initialized', 'true');
  }

  private static BindRowClickEventHandler(bocList: HTMLElement, row: HTMLElement, cell: HTMLElement, selectorControl: HTMLInputElement): void
  {
    row.addEventListener('click', (evt) =>
    {
      const hasSelectionChanged = BocList.OnRowClick (evt, bocList, row, selectorControl);
      if (hasSelectionChanged)
      {
        const selectedRows = BocList._selectedRows[bocList.id]!;
        selectedRows.OnSelectionChanged (bocList, false);
      }
    });

    if (row.classList.contains("hasValidationRow"))
    {
      let validationRow = row.nextSibling as HTMLElement;

      validationRow.addEventListener('click', () => row.dispatchEvent(new Event("click")));
    }
  
    selectorControl.addEventListener('click', function (evt)
    {
      evt.stopPropagation();
      BocList.OnRowSelectorClick();
      row.dispatchEvent (new MouseEvent('click'));
    });
  
      // Enable the entire selector control's cell for click events that mimic the selector control.
    cell.addEventListener('click', function (evt)
    {
      BocList.OnRowSelectorClick();
    });
  
    // Enable the entire row to the left of the selector control's cell for click events that mimic the selector control.
    for (let current: Nullable<Node> = cell; current != null; current = current.previousSibling)
    {
      current.addEventListener('click', function (evt)
      {
        BocList.OnRowSelectorClick();
      });
    }
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
  private static OnRowClick(evt: MouseEvent, bocList: HTMLElement, currentRow: HTMLElement, selectorControl: HTMLInputElement): boolean
  {
    if (BocList._isCommandClick)
    {
      BocList._isCommandClick = false;
      return false;
    }  
  
    const currentRowBlock = new BocList_RowBlock (currentRow, selectorControl);
    const selectedRows = BocList._selectedRows[bocList.id]!;
    const isRowHighlightingEnabled = true
    const isCtrlKeyPress = evt?.ctrlKey === true;
      
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
    const selectedRows = BocList._selectedRows[bocList.id]!;
    selectedRows.Rows[rowBlock.SelectorControl.id] = rowBlock;
    selectedRows.Length++;

    // Select currentRow
    rowBlock.SelectorControl.checked = true;
    if (isRowHighlightingEnabled)
    {
      rowBlock.Row.classList.add(BocList.TrClassNameSelected);
      rowBlock.ValidationRow?.classList.add(BocList.TrClassNameSelected);
    }
    BocList.SetSelectAllRowsSelectorOnDemand (selectedRows);
  }

  //  Unselects all rows in a BocList.
  //  Clears _bocList_selectedRows array and sets _bocList_selectedRowsLength to zero.
  //  bocList: The BocList whose rows should be unselected.
  //  isRowHighlightingEnabled: true to update the row's css-class.
  private static UnselectAllRows(bocList: HTMLElement, isRowHighlightingEnabled: boolean): void
  {
    const selectedRows = BocList._selectedRows[bocList.id]!;
    for (const rowID in selectedRows.Rows)
    {
      const rowBlock = selectedRows.Rows[rowID];
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
    const selectedRows = BocList._selectedRows[bocList.id]!;
    selectedRows.Rows[rowBlock.SelectorControl.id] = null;
    selectedRows.Length--;

    // Unselect currentRow
    rowBlock.SelectorControl.checked = false;
    if (isRowHighlightingEnabled)
    {
      rowBlock.Row.classList.remove(BocList.TrClassNameSelected);
      rowBlock.ValidationRow?.classList.remove(BocList.TrClassNameSelected);
    }

    BocList.ClearSelectAllRowsSelector (selectedRows);
  }

  private static SetSelectAllRowsSelectorOnDemand (selectedRows: BocList_SelectedRows): void
  {
    if (selectedRows.DataRowCount == selectedRows.Length && selectedRows.DataRowCount > 0)
      selectedRows.SelectAllSelectorControls!.forEach (element => { element.checked = true; }); // TODO RM-7711 - Move BocList's Row Selection logic to TypeScript class 'BocList_SelectedRows'.
  }

  private static ClearSelectAllRowsSelector (selectedRows: BocList_SelectedRows): void
  {
    selectedRows.SelectAllSelectorControls!.forEach (element => { element.checked = false; }); // TODO RM-7711 - Move BocList's Row Selection logic to TypeScript class 'BocList_SelectedRows'.
  }

  //  Event handler for the selection selectorControl in the title row.
  //  Applies the checked state of the title's selectorControl to all data rows' selectu=ion selectorControles.
  //  bocList: The BocList to which the selectorControl belongs.
  //  selectRowControlName: The name of the row selector controls.
  //  isRowHighlightingEnabled: true to enable highting of the rows
  public static OnSelectAllSelectorControlClick (bocListOrSelector: CssSelectorOrElement<HTMLElement>, selectAllSelectorControlOrSelector: CssSelectorOrElement<HTMLInputElement>, isRowHighlightingEnabled: boolean): void
  {
    ArgumentUtility.CheckNotNull('bocListOrSelector', bocListOrSelector);
    ArgumentUtility.CheckNotNull('selectAllSelectorControlOrSelector', selectAllSelectorControlOrSelector);

    const bocList = ElementResolverUtility.ResolveSingle(bocListOrSelector);
    const selectAllSelectorControl = ElementResolverUtility.ResolveSingle(selectAllSelectorControlOrSelector);
    const selectedRows = BocList._selectedRows[bocList.id]!;

    if (selectedRows.Selection != BocList.rowSelectionMultiple)
      return;
    //  BocList_SelectRow will increment the length, therefor initialize it to zero.
    if (selectAllSelectorControl.checked)
      selectedRows.Length = 0;

    selectedRows.SelectRowSelectorControls!.forEach (checkBox => // TODO RM-7711 - Move BocList's Row Selection logic to TypeScript class 'BocList_SelectedRows'.
    {
      const tableCell = checkBox.parentNode!;
      if (tableCell.nodeName !== 'TD')
        throw 'Unexpected element type: \'' + tableCell.nodeName + '\'';
  
      const tableRow = tableCell.parentNode as HTMLElement;
      if (tableRow.nodeName !== 'TR')
        throw 'Unexpected element type: \'' + tableRow.nodeName + '\'';
  
      const rowBlock = new BocList_RowBlock (tableRow, checkBox);
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

  public static OnInlineValidationEntryClick(event: MouseEvent)
  {
    event.preventDefault();
    event.stopPropagation();

    const a = event.target as HTMLAnchorElement;
    if (a && a.hash && a.hash.startsWith("#"))
    {
      const targetElement = document.getElementById(a.hash.substring(1));

      // We want to scroll the parent <td> element into view to ensure that the whole cell content is visible after scrolling
      let tdElement = targetElement;
      while (tdElement && tdElement.tagName !== "TD")
        tdElement = tdElement.parentElement;

      if (targetElement && tdElement)
      {
        tdElement.scrollIntoView({ block: "nearest", inline: "nearest" });
        targetElement.focus({ preventScroll: true });
      }
    }
  }

  //  Returns the number of rows selected for the specified BocList
  public static GetSelectionCount(bocListID: string): number
  {
    const selectedRows = BocList._selectedRows[bocListID];
    if (selectedRows == null)
      return 0;
    return selectedRows.Length;
  }

  private static HasDimensions(bocList: HTMLElement): boolean
  {
    const heightFromAttribute = bocList.getAttribute('height');
    if (!TypeUtility.IsNull(heightFromAttribute) && heightFromAttribute != '')
      return true;
  
    const heightFromInlineStyle = bocList.style.height;
    if (!TypeUtility.IsNull(heightFromInlineStyle) && heightFromInlineStyle != '')
      return true;
  
    const widthFromAttribute = bocList.getAttribute('width');
    if (!TypeUtility.IsNull(widthFromAttribute) && widthFromAttribute != '')
      return true;
  
    const widthFromInlineStyle = bocList.style.width;
    if (!TypeUtility.IsNull(widthFromAttribute) && widthFromInlineStyle != '')
      return true;
  
    const referenceHeight = 0;
    const referenceWidth = 0;
    const tempList = document.createElement('div');
    bocList.classList.forEach(className => { tempList.classList.add(className) });
    tempList.style.display = 'none';
  
    // Catch styles applied to pseudo-selectors starting at the first element in the DOM collection
    bocList.parentNode!.insertBefore(tempList, bocList);
  
    try
    {
      if (Math.floor(LayoutUtility.GetHeight(tempList)) > referenceHeight)
        return true;
  
      if (Math.floor(LayoutUtility.GetWidth(tempList)) > referenceWidth)
        return true;
    } 
    finally
    {
      tempList.remove();
    }
  
    // Catch styles applied to pseudo-selectors starting at the last element in the DOM collection
    bocList.parentNode!.insertBefore(tempList, bocList.nextSibling)
  
    try
    {
      if (Math.floor(LayoutUtility.GetHeight(tempList)) > referenceHeight)
        return true;
  
      if (Math.floor(LayoutUtility.GetWidth(tempList)) > referenceWidth)
        return true;
    }
    finally
    {
      tempList.remove();
    }
  
    return false;
  }

  private static FixUpScrolling(bocList: HTMLElement): void
  {
    const tableBlock = bocList.querySelector(':scope > div.bocListTableBlock')!;

    let scrollTimer: Nullable<number> = null;
    const tableContainer = tableBlock.querySelector<HTMLElement>(':scope > div.bocListTableContainer');
    if (!tableContainer) // In the case of an empty BocList we don't have a table container
      return;

    const scrollableContainer = tableContainer.querySelector<HTMLElement>(':scope > div.bocListTableScrollContainer')!;
    let horizontalScroll = 0;
  
    scrollableContainer.addEventListener('scroll', () =>
    {
      const newHorizontalScroll = scrollableContainer.scrollLeft;
      const hasHorizontalScrollUpdated = horizontalScroll != newHorizontalScroll;
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
  
    const resizeInterval = 50;
    const resizeHandler = function ()
    {
      if (!PageUtility.Instance.IsInDom (scrollableContainer))
        return;

      BocList.FixHeaderSize(scrollableContainer);
      BocList.FixHeaderPosition(tableContainer, scrollableContainer);
      setTimeout(resizeHandler, resizeInterval);
    };
    resizeHandler();
  }

  private static FixupValidationErrorOverflowSize(bocList: HTMLElement): void
  {
    const tableBlock = bocList.querySelector(':scope > div.bocListTableBlock')!;
    const tableContainer = tableBlock.querySelector<HTMLElement>(':scope > div.bocListTableContainer');
    if (!tableContainer) // In the case of an empty BocList we don't have a table container
      return;

    const scrollableContainer = tableContainer.querySelector<HTMLElement>(':scope > div.bocListTableScrollContainer')!;

    const resizeInterval = 50;

    let resizeHandler = function ()
    {
      if (!PageUtility.Instance.IsInDom (scrollableContainer))
        return;

      BocList.FixValidationErrorOverflowSize(scrollableContainer);
      setTimeout(resizeHandler, resizeInterval);
    };
    resizeHandler();
  }

  private static FixValidationErrorOverflowSize(scrollableContainer: HTMLElement)
  {
    // Sets the width of the validation error field to be as wide as the visible part of the bocList
    // minus how much space the td on the left requires to align with the error indicator column.
    // This enables sticky behaviour while still allowing for a scrollbar if required.

    const availableWidthInContainer = scrollableContainer.clientWidth;

    const tdBeforeValidationFailureColumn: Nullable<HTMLElement> = scrollableContainer.querySelector('tr.bocListValidationRow td:first-child:not(.bocListValidationFailureCell)');

    let widthBeforeValidationFailureColumn = 0;
    if (tdBeforeValidationFailureColumn != null)
    {
      widthBeforeValidationFailureColumn = Math.max(tdBeforeValidationFailureColumn.clientWidth - scrollableContainer.scrollLeft, 0);
    }

    const overflowContainers = [...scrollableContainer.querySelectorAll<HTMLElement>(':scope tr.bocListValidationRow > td.bocListValidationFailureCell > div')];

    overflowContainers.forEach(container =>
    {
      let parentStyle = window.getComputedStyle(<HTMLElement>container.parentNode);

      let padding = parseFloat(parentStyle.paddingLeft) + parseFloat(parentStyle.paddingRight);
      let border = parseFloat(parentStyle.borderLeftWidth) + parseFloat(parentStyle.borderRightWidth);

      // We use a magic 10px width reduction here to ensure that the determined size is always smaller than the actual size
      // This is needed as the calculation is not quite as precise as necessary as even a 1px offset can lead to weird scrolling behavior
      // See RM-8887 and RM-8994 for more context
      container.style.width = (availableWidthInContainer - widthBeforeValidationFailureColumn - padding - border - 10).toString() + "px";
    });
  }

  private static CreateFakeTableHead(tableContainer: HTMLElement, scrollableContainer: HTMLElement, bocList: HTMLElement): void
  {
    // Add diganostic metadata for web testing framework (actually: should only be rendered with IRenderingFeatures.EnableDiagnosticMetadata on)
    tableContainer.setAttribute('data-boclist-has-fake-table-head', 'true');
  
    const table = scrollableContainer.querySelector(':scope > table')!;
  
    const fakeTable = document.createElement('table');
    fakeTable.setAttribute('role', 'none');
    table.classList.forEach(className => { fakeTable.classList.add(className) });
    fakeTable.setAttribute('cellPadding', '0');
    fakeTable.setAttribute('cellSpacing', '0');
    fakeTable.style.width = '100%';

    const realTableHead = table.querySelector(':scope > thead')!;
    const fakeTableHead = realTableHead.cloneNode(true) as HTMLElement;
    realTableHead.setAttribute('aria-hidden', 'true')
    realTableHead.setAttribute('role', 'none')
    realTableHead.querySelectorAll('*[role]').forEach(element =>
    {
      element.setAttribute('role', 'none');
    });
    fakeTable.append(fakeTableHead);
  
    const fakeTableHeadWidthContainer = document.createElement('div');
    fakeTableHeadWidthContainer.setAttribute('role', 'none');
    fakeTableHeadWidthContainer.append(fakeTable);
  
    const fakeTableHeadContainer = document.createElement('div');
    fakeTableHeadContainer.setAttribute('role', 'none');
    fakeTableHeadContainer.classList.add('bocListFakeTableHead');
    fakeTableHeadContainer.style.display = 'none';
    fakeTableHeadContainer.append (fakeTableHeadWidthContainer);
  
    realTableHead.querySelectorAll('*').forEach(element =>
    {
      element.removeAttribute('id');
      element.setAttribute('tabIndex', '-1');
    });
  
    scrollableContainer.before(fakeTableHeadContainer);
  
    // sync checkboxes
    const checkboxes = fakeTableHead.querySelectorAll<HTMLInputElement>("th input[type=checkbox]");
    checkboxes.forEach(checkbox =>
    {
      checkbox.addEventListener('click', (event) =>
      {
        const checkName = checkbox.getAttribute('name');
        const checkStatus = checkbox.checked;
        document.querySelectorAll('input[name="' + checkName + '"]').forEach(element =>
        {
          (element as HTMLInputElement).checked = checkStatus;
        });
      })
    });
    // BocList_FixHeaderSize() with timeout needs to be called after setup. This is already taken care of at the call-site.
  }

  private static FixHeaderSize(scrollableContainer: HTMLElement): void
  {
    const realTable = scrollableContainer.querySelector<HTMLTableElement>(':scope > table')!;
    const realTableWidth = Math.floor(LayoutUtility.GetWidth(realTable));
    const previousRealTableWidth = parseInt(realTable.dataset.bocListPreviousRealTableWidth!);
    if (previousRealTableWidth == realTableWidth)
      return;
    realTable.dataset.bocListPreviousRealTableWidth = realTableWidth.toString();
  
    const realTableHead = realTable.querySelector('thead')!;
    const realTableHeadRow = realTableHead.children[0];
    const realTableHeadRowChildren = Array.from(realTableHeadRow.children);
  
    const fakeTableHeadContainer = scrollableContainer.parentNode!.querySelector<HTMLTableElement>(':scope > div.bocListFakeTableHead')!;
    const fakeTableHead = fakeTableHeadContainer.querySelector('thead')!;
    const fakeTableHeadRow = fakeTableHead.children[0];
    const fakeTableHeadRowChildren = Array.from(fakeTableHeadRow.children);
  
    // store cell widths in array
    const realTableHeadCellWidths = new Array();
    realTableHeadRowChildren.forEach(function (element: Element, index: number)
    {
      realTableHeadCellWidths[index] = Math.floor(LayoutUtility.GetWidth(element as HTMLElement));
    });
  
    // apply widths to fake header
    fakeTableHeadRowChildren.forEach((element: Element, index: number) =>
    {
      (element as HTMLElement).style.width = realTableHeadCellWidths[index] + 'px';
    });

    const fakeTableHeadWidthContainer = fakeTableHeadContainer.querySelector<HTMLElement> (':scope > div')!;
    fakeTableHeadWidthContainer.style.width = realTableWidth + 'px';
    fakeTableHeadContainer.style.display = 'block';
    const fakeTableHeadContainerHeight = Math.floor(LayoutUtility.GetHeight(fakeTableHeadContainer));
    scrollableContainer.style.top = fakeTableHeadContainerHeight + 'px';
    realTable.style.marginTop = (fakeTableHeadContainerHeight * -1) + 'px';
  }

  private static FixHeaderPosition(tableContainer: HTMLElement, scrollableContainer: HTMLElement): void
  {
    const fakeTableHeadContainer = tableContainer.querySelector(':scope > div.bocListFakeTableHead')!;
    let scrollLeft = scrollableContainer.scrollLeft;
    const previousScrollLeft = parseFloat(scrollableContainer.dataset.bocListPreviousScrollLeft!)
    const fakeTableHeadScrollLeft = fakeTableHeadContainer.scrollLeft;

    const hasScrollMoveFromScrollbar = previousScrollLeft !== scrollLeft;
    const hasScrollMoveFromColumnHeader = previousScrollLeft !== fakeTableHeadScrollLeft;
    if (!hasScrollMoveFromScrollbar && !hasScrollMoveFromColumnHeader)
      return;

    scrollLeft = hasScrollMoveFromColumnHeader ? fakeTableHeadScrollLeft : scrollLeft;
    scrollableContainer.dataset.bocListPreviousScrollLeft = scrollLeft.toString();

    if (hasScrollMoveFromColumnHeader)
      scrollableContainer.scrollLeft = scrollLeft;

    if (hasScrollMoveFromScrollbar)
      fakeTableHeadContainer.scrollLeft = scrollLeft;
  }

  public static InitializeNavigationBlock (pageNumberFieldOrSelector: CssSelectorOrElement<HTMLInputElement>, pageIndexFieldOrSelector: CssSelectorOrElement<HTMLInputElement>): void
  {
  
    ArgumentUtility.CheckNotNull ('pageNumberFieldOrSelector', pageNumberFieldOrSelector);
    ArgumentUtility.CheckNotNull('pageIndexFieldOrSelector', pageIndexFieldOrSelector);

    const pageNumberField = ElementResolverUtility.ResolveSingle(pageNumberFieldOrSelector);
    const pageIndexField = ElementResolverUtility.ResolveSingle(pageIndexFieldOrSelector);

    pageNumberField.addEventListener('change', function () {
  
      const pageNumber = parseInt(pageNumberField.value, 10);
      if (isNaN (pageNumber) || !TypeUtility.IsInteger (pageNumber))
      {
        if (pageNumberField.value.length > 0)
          setTimeout(function () { pageNumberField.focus(); }, 0);
  
        pageNumberField.value = '' + (parseInt(pageIndexField.value, 10) + 1); // TODO RM-7699 - Incorrect style assignments in multiple TypeScript files
        return false;
      }
      else
      {
        pageIndexField.value =  '' + (pageNumber - 1); // TODO RM-7699 - Incorrect style assignments in multiple TypeScript files
        pageIndexField.dispatchEvent(new Event('change'));
        return true;
      }
    });
  
    pageNumberField.addEventListener('keydown', function (event) {
      const enterKey = 13;
      const zeroKey = 48;
      const nineKey = 57;
      const zeroKeyNumBlock = 96;
      const nineKeyNumBlock = 105;
      const f1Key = 112;
      const f12Key = 123;
      const isEnterKey = event.keyCode == enterKey;
      const isControlKey = event.keyCode < zeroKey || event.keyCode >= f1Key && event.keyCode <= f12Key;
      const isNumericKey = event.keyCode >= zeroKey && event.keyCode <= nineKey || event.keyCode >= zeroKeyNumBlock && event.keyCode <= nineKeyNumBlock;
  
      if (isEnterKey)
      {
        pageNumberField.dispatchEvent(new Event("change"));
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
