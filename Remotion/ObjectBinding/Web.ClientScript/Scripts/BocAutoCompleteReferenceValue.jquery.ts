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

// Original license header
//
// Autocomplete - jQuery plugin 1.1pre
// 
// Copyright (c) 2007 Dylan Verheul, Dan G. Switzer, Anjesh Tuladhar, Jörn Zaefferer
// 
// Dual licensed under the MIT and GPL licenses:
//   http://www.opensource.org/licenses/mit-license.php
//   http://www.gnu.org/licenses/gpl.html
// 
// Revision: $Id: jquery.autocomplete.js 5785 2008-07-12 10:37:33Z joern.zaefferer $

// ************************************************
// Significantly modified for re-motion
// ************************************************

type BocAutoCompleteReferenceValue_Item = {
    UniqueIdentifier: string;
    DisplayName: string;
    IconUrl: string;
    IsAnnotation: boolean;
};

type BocAutoCompleteReferenceValue_UpdateResult = {
    UniqueIdentifier: string;
    DisplayName: string;
};

type BocAutoCompleteReferenceValue_FormatResult = {
    html: string;
    class: Nullable<string>;
    isAnnotation: boolean;
};

type BocAutoCompleteReferenceValue_Options = {
    nullValue: string;
    inputClass: string;
    resultsClass: string;
    loadingClass: string;
    informationPopUpClass: string;
    inputAreaClass: string;
    searchStringValidationParams:
    {
      inputRegex: RegExp;
      dropDownTriggerRegex: RegExp;
      dropDownTriggerRegexFailedMessage: string;
      getDropDownSearchString(searchString: string): string;
    };
    dropDownDisplayDelay: number;
    dropDownRefreshDelay: number;
    selectionUpdateDelay: number;
    noDataFoundMessage: string;
    matchContains: boolean;
    cacheLength: number;
    max: number;
    isAutoPostBackEnabled: boolean;
    extraParams: Dictionary<unknown>;
    combobox: JQuery;
    selectListID: string;
    informationPopUpID: string;
    dropDownButtonID: string;
    dataType: string;
    selectFirst(inputValue: string, searchTerm: Nullable<string>): boolean;
    formatItem(item: BocAutoCompleteReferenceValue_Item, index: number, length: number, value: string, term: string): BocAutoCompleteReferenceValue_FormatResult;
    formatMatch(item: BocAutoCompleteReferenceValue_Item, index: number, length: number): string | false;
    formatResult?(item: BocAutoCompleteReferenceValue_Item): string;
    autoFill: boolean;
    highlight(value: string, term: string): string;
    scroll: boolean;
    repositionInterval: number;
    parse(data: BocAutoCompleteReferenceValue_Item[]): BocAutoCompleteReferenceValue_CacheRow;
    handleRequestError(err: Sys.Net.WebServiceError): void;
    clearRequestError(err?: Sys.Net.WebServiceError): void;
    serviceUrl: string;
    serviceMethodSearch: string;
    serviceMethodSearchExact: string;
    data: Nullable<BocAutoCompleteReferenceValue_Item[]>;
};

// This type contains the properties that are required to be passed into the .autocomplete() function
// The type-checker makes sure that all other properties are defaulted (see Object.assign in .autocomplete())
// Nested types must be defined fully as recursion is not supported
type BocAutoCompleteReferenceValue_RequiredOptions = {
    nullValue: BocAutoCompleteReferenceValue_Options["nullValue"];
    dataType: BocAutoCompleteReferenceValue_Options["dataType"];
    searchStringValidationParams: {
        inputRegex?: RegExp;
        dropDownTriggerRegex?: RegExp;
        dropDownTriggerRegexFailedMessage: string;
        getDropDownSearchString?(searchString: string): string;
    },
    formatItem: BocAutoCompleteReferenceValue_Options["formatItem"];
    formatMatch: BocAutoCompleteReferenceValue_Options["formatMatch"];
    parse: BocAutoCompleteReferenceValue_Options["parse"];
    combobox: BocAutoCompleteReferenceValue_Options["combobox"];
};

type BocAutoCompleteReferenceValue_PositioningOptions = {
    maxWidth: number;
    maxHeight: number;
};

interface JQueryStatic {
    Autocompleter: BocAutoCompleteReferenceValue_AutocompleterStatic;

    data(el: Element, key: "ac_data"): BocAutoCompleteReferenceValue_CacheRowEntry;
    data(el: Element, key: "ac_data", value: BocAutoCompleteReferenceValue_CacheRowEntry): void;
}

interface JQuery {
    autocomplete(this: JQuery, serviceUrl: string, serviceMethodSearch: string, serviceMethodSearchExact: string, options: PartialWithRequiredProperties<BocAutoCompleteReferenceValue_Options, keyof BocAutoCompleteReferenceValue_RequiredOptions>): JQuery;
    invalidateResult(this: JQuery, handler: BocAutoCompleteReferenceValue_InvalidateResultEventHandler): JQuery;
    updateResult(this: JQuery, handler: BocAutoCompleteReferenceValue_UpdateResultEventHandler): JQuery;
    flushCache(this: JQuery): JQuery;
    getAutoCompleteSearchParameters(this: JQuery, searchString: string, completionSetCount: number): BocAutoCompleteReferenceValue_CollectedOptions;
    getAutoCompleteSelectList(this: JQuery): BocAutoCompleteReferenceValue_Select;
    getAutoCompleteInformationPopUp(this: JQuery): BocAutoCompleteReferenceValue_InformationPopUp;
    setOptions(options: BocAutoCompleteReferenceValue_Options): JQuery;
    unautocomplete(): JQuery;

    bind(ev: "invalidateResult", handler: BocAutoCompleteReferenceValue_InvalidateResultEventHandler): JQuery;
    bind(ev: "updateResult", handler: BocAutoCompleteReferenceValue_UpdateResultEventHandler): JQuery;
    bind(ev: "search", handler: BocAutoCompleteReferenceValue_SearchEventHandler): JQuery;
    bind(ev: "flushCache", handler: BocAutoCompleteReferenceValue_FlushCacheEventHandler): JQuery;
    bind(ev: "collectOptions", handler: BocAutoCompleteReferenceValue_CollectOptionsEventHandler): JQuery;
    bind(ev: "setOptions", handler: BocAutoCompleteReferenceValue_SetOptionsEventHandler): JQuery;
    bind(ev: "collectElements", handler: BocAutoCompleteReferenceValue_CollectElementsEventHandler): JQuery;
    bind(ev: "showInformationPopUp", handler: BocAutoCompleteReferenceValue_ShowInformationPopupEventHandler): JQuery;
    bind(ev: "hideInformationPopUp", handler: BocAutoCompleteReferenceValue_HideInformationPopupEventHandler): JQuery;
    bind(ev: "unautocomplete", handler: BocAutoCompleteReferenceValue_UnautocompleteEventHandler): JQuery;

    trigger(ev: "invalidateResult", args: EventParameters<BocAutoCompleteReferenceValue_InvalidateResultEventHandler>): JQuery;
    trigger(ev: "updateResult", args: EventParameters<BocAutoCompleteReferenceValue_UpdateResultEventHandler>): JQuery;
    trigger(ev: "search", args: EventParameters<BocAutoCompleteReferenceValue_SearchEventHandler>): JQuery;
    trigger(ev: "flushCache", args: EventParameters<BocAutoCompleteReferenceValue_FlushCacheEventHandler>): JQuery;
    trigger(ev: "collectOptions", args: EventParameters<BocAutoCompleteReferenceValue_CollectOptionsEventHandler>): JQuery;
    trigger(ev: "setOptions", args: EventParameters<BocAutoCompleteReferenceValue_SetOptionsEventHandler>): JQuery;
    trigger(ev: "collectElements", args: EventParameters<BocAutoCompleteReferenceValue_CollectElementsEventHandler>): JQuery;
    trigger(ev: "showInformationPopUp", args: EventParameters<BocAutoCompleteReferenceValue_ShowInformationPopupEventHandler>): JQuery;
    trigger(ev: "hideInformationPopUp", args: EventParameters<BocAutoCompleteReferenceValue_HideInformationPopupEventHandler>): JQuery;
    trigger(ev: "unautocomplete", args: EventParameters<BocAutoCompleteReferenceValue_UnautocompleteEventHandler>): JQuery;
}

type BocAutoCompleteReferenceValue_InvalidateResultEventHandler = (ev: JQueryEventObject) => void;
type BocAutoCompleteReferenceValue_UpdateResultEventHandler = (ev: JQueryEventObject, item: BocAutoCompleteReferenceValue_UpdateResult, out: OutBox<BocAutoCompleteReferenceValue_UpdateResult>) => void;
type BocAutoCompleteReferenceValue_SearchEventHandler = (ev: JQueryEventObject, fn?: (result: Optional<Autocomplete_Cache_RowEntry>) => void) => void;
type BocAutoCompleteReferenceValue_FlushCacheEventHandler = (ev: JQueryEventObject) => void;
type BocAutoCompleteReferenceValue_CollectOptionsEventHandler = (ev: JQueryEventObject, options: Partial<BocAutoCompleteReferenceValue_CollectedOptions>) => void;
type BocAutoCompleteReferenceValue_SetOptionsEventHandler = (ev: JQueryEventObject, options: Partial<BocAutoCompleteReferenceValue_Options>) => void;
type BocAutoCompleteReferenceValue_CollectElementsEventHandler = (ev: JQueryEventObject, options: Partial<BocAutoCompleteReferenceValue_CollectedElements>) => asserts options is BocAutoCompleteReferenceValue_CollectedElements;
type BocAutoCompleteReferenceValue_ShowInformationPopupEventHandler = (ev: JQueryEventObject, data: { message: string }) => void;
type BocAutoCompleteReferenceValue_HideInformationPopupEventHandler = (ev: JQueryEventObject) => void;
type BocAutoCompleteReferenceValue_UnautocompleteEventHandler = (ev: JQueryEventObject) => void;

type EventParameters<T extends AnyFunction> = Parameters<T> extends [any, ...infer TRest] ? TRest : never;
type JQueryFunctions = { [K in keyof JQuery]: JQuery[K] extends Function ? JQuery[K] : never };

type BocAutoCompleteReferenceValue_AutocompleterStatic = {
    new(target: HTMLElement, options: BocAutoCompleteReferenceValue_Options): unknown;
    defaults: DeepPartial<BocAutoCompleteReferenceValue_Options>;
    Cache: typeof BocAutoCompleteReferenceValue_Cache;
    Select: typeof BocAutoCompleteReferenceValue_Select;
    InformationPopUp: typeof BocAutoCompleteReferenceValue_InformationPopUp;
    Selection(field: HTMLTextAreaElement, start: number, end: number, focusInputAfterSelection: boolean): void;
    calculateSpaceAround(element: JQuery): BocAutoCompleteReferenceValue_SpaceAround;
    applyPositionToPopUp(reference: JQuery, popUp: JQuery, options: BocAutoCompleteReferenceValue_PositioningOptions): void;
};

type BocAutoCompleteReferenceValue_CollectedElements = {
    selectList: BocAutoCompleteReferenceValue_Select;
    informationPopUp: BocAutoCompleteReferenceValue_InformationPopUp;
};

type BocAutoCompleteReferenceValue_CollectedOptions = {
    serviceUrl: string;
    serviceMethodSearch: string;
    serviceMethodSearchExact: string;
    params: Dictionary<unknown>;
    searchString: string;
    completionSetCount?: number;
};

// This is used to tell the type checker that a certain variable cannot be null at a certain point
function NotNullAssert(v: unknown): asserts v is NotNull {}

// The following type declarations are fixed for IE
// If support for IE is removed also remove all "todo IE"s
interface HTMLTextAreaElement {
    createTextRange?(): {
        collapse(toStart?: boolean): void;
        moveStart(unit: "character" | "sentence" | "textedit" | "word", numberOfUnits?: number): number;
        moveEnd(unit: "character" | "sentence" | "textedit" | "word", numberOfUnits?: number): number;
        select(): void;
    };
}

; (function($) {
    // This is not typed against BocAutoCompleteReferenceValue_Options because otherwise the default
    // applying in .autocomplete() would not work correctly. Regardless, incorrect typing will raise an error.
    const defaultOptions = {
        inputClass: "ac_input",
        resultsClass: "ac_results",
        loadingClass: "ac_loading",
        informationPopUpClass: "ac_informationPopUp",
        inputAreaClass: "ac_content",
        searchStringValidationParams:
          {
            inputRegex: new RegExp("\\S+"),
            dropDownTriggerRegex: new RegExp("\\S+"),
            getDropDownSearchString: function (searchString: string): string { return searchString; }
          },
        // re-motion: modified delay concept
        dropDownDisplayDelay: 400,
        dropDownRefreshDelay: 400,
        selectionUpdateDelay: 400,
        noDataFoundMessage: '',
        matchContains: false,
        cacheLength: 10,
        max: 100,
        isAutoPostBackEnabled: false,
        extraParams: {},
        // re-motion: changed selectFirst from boolean field to function
        selectFirst: function(inputValue: string, searchTerm: Nullable<string>): boolean { return true; },
        autoFill: false,
        highlight: function(value: string, term: string): string {
            if (term == '')
              return value;
            return value.replace(new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + term.replace(/([\^\$\(\)\[\]\{\}\*\.\+\?\|\\])/gi, "\\$1") + ")(?![^<>]*>)(?![^&;]+;)", "gi"), "<strong>$1</strong>");
        },
        scroll: true,
        repositionInterval: 200,
        handleRequestError: function (err: Sys.Net.WebServiceError): void { },
        clearRequestError: function (err: Sys.Net.WebServiceError): void { }
    };

    $.fn.extend(<JQueryFunctions>{
        autocomplete: function(
            this: JQuery,
            serviceUrl: string,
            serviceMethodSearch: string,
            serviceMethodSearchExact: string,
            initialOptions: PartialWithRequiredProperties<BocAutoCompleteReferenceValue_Options, keyof BocAutoCompleteReferenceValue_RequiredOptions>): JQuery {
            const $input = $(this);
            // This assignment builds the final options object and also ensures that all the option types are set up correctly
            const options: BocAutoCompleteReferenceValue_Options = Object.assign({}, defaultOptions, {
                // re-motion: instead of a single URL property, use separate service URL and service method properties. 
                //           data cannot be inserted directly any more
                serviceUrl: serviceUrl,
                serviceMethodSearch: serviceMethodSearch,
                serviceMethodSearchExact: serviceMethodSearchExact,
                data: null,
                combobox: null,
                selectListID: null,
                informationPopUpID: null,
                // re-motion: clicking this control will display the dropdown list with an assumed input of '' (regardless of textbox value)
                dropDownButtonID: null
            }, initialOptions as BocAutoCompleteReferenceValue_RequiredOptions);

            // if highlight is set to false, replace it with a do-nothing function
            options.highlight = options.highlight || function(value) { return value; };

            // if the formatMatch option is not specified, then use formatItem for backwards compatibility
            options.formatMatch = options.formatMatch || options.formatItem;

            return this.each(function(this: HTMLElement) {
                new $.Autocompleter(this, options);
            });
        },
        invalidateResult: function(this: JQuery, handler: BocAutoCompleteReferenceValue_InvalidateResultEventHandler): JQuery {
            return this.bind("invalidateResult", handler);
        },
        updateResult: function(this: JQuery, handler: BocAutoCompleteReferenceValue_UpdateResultEventHandler): JQuery {
            return this.bind("updateResult", handler);
        },
        flushCache: function(this: JQuery): JQuery {
            return this.trigger("flushCache");
        },
        getAutoCompleteSearchParameters: function (this: JQuery, searchString: string, completionSetCount?: number): BocAutoCompleteReferenceValue_CollectedOptions
        {
          const collectedOptions: Partial<BocAutoCompleteReferenceValue_CollectedOptions> = {};
          this.trigger("collectOptions", collectedOptions);

          collectedOptions.searchString = searchString;
          if(completionSetCount)
            collectedOptions.completionSetCount = completionSetCount;

          return collectedOptions as BocAutoCompleteReferenceValue_CollectedOptions;
        },
        getAutoCompleteSelectList: function (this: JQuery): BocAutoCompleteReferenceValue_Select
        {
          const collectedOptions: Partial<BocAutoCompleteReferenceValue_CollectedElements> = {};
          this.trigger("collectElements", [collectedOptions]);
          return collectedOptions.selectList!;
        },
        getAutoCompleteInformationPopUp: function (this: JQuery): BocAutoCompleteReferenceValue_InformationPopUp
        {
          const collectedOptions: Partial<BocAutoCompleteReferenceValue_CollectedElements>  = {};
          this.trigger("collectElements", [collectedOptions]);
          return collectedOptions.informationPopUp!;
        },
        setOptions: function(options: BocAutoCompleteReferenceValue_Options): JQuery {
            return this.trigger("setOptions", [options]);
        },
        unautocomplete: function(): JQuery {
            return this.trigger("unautocomplete");
        }
    });

    $.Autocompleter = <any>function(input: HTMLTextAreaElement, options: BocAutoCompleteReferenceValue_Options) {

      const KEY = {
            SHIFT: 16,
            CAPSLOCK: 20,
            CTRL: 17,
            ALT: 18,
            UP: 38,
            DOWN: 40,
            DEL: 46,
            TAB: 9,
            RETURN: 13,
            ESC: 27,
            COMMA: 188,
            PAGEUP: 33,
            PAGEDOWN: 34,
            BACKSPACE: 8,
            SPACE: 32,
            FIRSTTEXTCHARACTER: 48 // 0
        };

        // Create $ object for input element
        const $input = Remotion.jQuery (input).addClass(options.inputClass);
        //aria-controls has been removed since it does not provide a meaningful assistance at this point in time (IE 11 / JAWS 18)
        //$input.attr ({ "aria-controls" : options.selectListID });

        // re-motion: Holds the currently executing request. 
        //            If the user types faster than the requests can be answered, the intermediate requests will be discarded.
        let executingRequest: Nullable<Sys.Net.WebRequest> = null;
        let timeout: Optional<number>;
        let autoFillTimeout: Optional<Nullable<number>>;
        let isInvalidated: boolean = false;
        const cache = new BocAutoCompleteReferenceValue_Cache(options);
        let hasFocus: boolean = false;
        try
        {
          hasFocus = TypeUtility.IsDefined(window.document.activeElement) && window.document.activeElement == input;
        }
        catch (e)
        {
          //IE9 can throw an unspecified error when inside an iframe because the activeElement is not yet initialzied
          //See also SmartPage.js:270
          //The input-element will be focused later in the page life cycle.
        }

        const state = {
            lastKeyPressCode: -1,
            mouseDownOnSelect: false,
            // holds the last text the user entered into the input element
            previousValue: '',
            lastKeyPressValue: null
        };

        const select = new BocAutoCompleteReferenceValue_Select(options, input, selectCurrent, state);
        // Perform initialize on load to ensure that screenreader can recognize the BocAutoCompleteReferenceValue as a combobox.
        // With IE 11 / JAWS 18, the input element is announced as an 'edit' input element until there is a listbox element available as well,
        // Chrome 63 does not show this behavior, so it's just some weird issue that needs to be worked around.
        select.init();

        const informationPopUp = new BocAutoCompleteReferenceValue_InformationPopUp(options, input);
        let blockSubmit: Optional<boolean>;

        $input.bind("keydown.autocomplete", function(event) {
            // track last key pressed
            state.lastKeyPressCode = event.keyCode;
            clearTimeout(timeout);
            // re-motion: cancel an already running request
            stopLoading();
            abortRequest();
            if (state.lastKeyPressValue !== null && state.lastKeyPressValue != $input.val()) {
                invalidateResult();
            }
            state.lastKeyPressValue = $input.val();

            switch (event.keyCode)
            {
                case KEY.UP:
                    event.preventDefault();
                    // re-motion: block event bubbling
                    event.stopPropagation();
                    if (select.visible()) {
                        options.clearRequestError();
                        select.prev();
                    } else {
                        onChange(true, $input.val());
                    }
                    return;

                case KEY.DOWN:
                    event.preventDefault();
                    // re-motion: block event bubbling
                    event.stopPropagation();
                    if (select.visible()) {
                        options.clearRequestError();
                        select.next();
                    } else {
                        onChange(true, $input.val());
                    }
                    return;

                case KEY.PAGEUP:
                    event.preventDefault();
                    // re-motion: block event bubbling
                    event.stopPropagation();
                    if (select.visible()) {
                        options.clearRequestError();
                        select.pageUp();
                    } else {
                        onChange(true, $input.val());
                    }
                    return;

                case KEY.PAGEDOWN:
                    event.preventDefault();
                    // re-motion: block event bubbling
                    event.stopPropagation();
                    if (select.visible()) {
                        options.clearRequestError();
                        select.pageDown();
                    } else {
                        onChange(true, $input.val());
                    }
                    return;

                case KEY.RETURN:
                case KEY.TAB:
                case KEY.ESC:
                    // re-motion: block event bubbling
                    event.stopPropagation();
                    const wasVisible = select.visible();
                    state.mouseDownOnSelect = false;

                    if (event.keyCode == KEY.RETURN) {
                        const isValueSelected = selectCurrent(false);
                        if (isValueSelected) {
                            //SelectCurrent already does everything that's needed.
                        } else {
                            const selectedItem = select.selected(true);
                            const isAnnotationSelected = selectedItem != null && selectedItem.data.IsAnnotation === true;
                            if (!isAnnotationSelected) {
                                acceptCurrent(true, false);
                            }
                        }
                    } else {
                        acceptCurrent(true, false);
                    }

                    if (event.keyCode == KEY.RETURN) {
                        if (wasVisible) {
                            // stop default to prevent a form submit, Opera needs special handling
                            blockSubmit = true;
                            return false;
                        } else {
                            return true;
                        }
                    } else if (event.keyCode == KEY.TAB) {
                        return true;
                    } else /* ESC */ {
                        return false;
                    }

                default:
                    return;
            }
        }).bind ('keyup paste', function(event) { // re-motion
            const handleInput = function() {
                informationPopUp.hide();
                const currentValue = $input.val();
                const dropDownDelay = select.visible() ? options.dropDownRefreshDelay : options.dropDownDisplayDelay;

                timeout = setTimeout(
                    function () { 
                        onChange(false, currentValue); 
                    }, 
                    dropDownDelay);
            };

            state.lastKeyPressValue = $input.val();

            if (event.type == 'keyup') {
                const isTextChangeKey =
                       event.keyCode >= KEY.FIRSTTEXTCHARACTER
                    || event.keyCode == KEY.BACKSPACE
                    || event.keyCode == KEY.DEL
                    || event.keyCode == KEY.SPACE;

                const hasValueChanged = $input.val() != state.previousValue;

                if (isTextChangeKey) {
                    clearTimeout(timeout);
                }

                if (isTextChangeKey && hasValueChanged) {
                    invalidateResult();
                    handleInput();
                }
            } else if (event.type == 'paste') {
                clearTimeout(timeout);
                invalidateResult();
                state.lastKeyPressCode = KEY.FIRSTTEXTCHARACTER;
                setTimeout(handleInput, 0);
            } else {
                clearTimeout(timeout);
                throw 'Unexpected event match occurred.';
            }

            if (autoFillTimeout) {
              clearTimeout(autoFillTimeout);
              autoFillTimeout = null;
            }
            
            if (options.autoFill) {
                autoFillTimeout = setTimeout (
                    function() {
                        if (!select.visible())
                            return;
                        
                        let index = -1;
                        if ($input.val() != '')
                        {
                          index = select.findItem($input.val());
                        }
                        else
                        {
                          const selectedItem = select.selected(true);
                          const isAnnotationSelected = selectedItem != null && selectedItem.data.IsAnnotation === true;
                          if (isAnnotationSelected)
                            index = select.findItemPositionWhere (function (data) { return data === selectedItem });
                        }

                        select.selectItem (index);
                        
                        if (index != -1)
                          autoFill($input.val(), select.selected(true)!.result);
                }, options.selectionUpdateDelay);
            }
        }).focus(function() {
            // track whether the field has focus, we shouldn't process any
            // results if the field no longer has focus
            hasFocus = true;
        }).blur(function() {
            hasFocus = false;
            if (!select.visible()) {
                clearTimeout(timeout);
                if ($input.val() == '')  {
                    options.clearRequestError();
                }
            }

            if (state.mouseDownOnSelect) {
                informationPopUp.hide();
            } else {
                const focusInputAfterSelection = false;
                const isLastKeyPressBeforeBlurHandled = state.lastKeyPressCode == -1;
                if (isLastKeyPressBeforeBlurHandled) {
                    closeDropDownListAndSetValue($input.val(), focusInputAfterSelection);
                    updateResult ({ DisplayName : $input.val(), UniqueIdentifier : options.nullValue });
                } else {
                    clearTimeout(timeout);
                    const lastKeyPressCode = state.lastKeyPressCode;
                    invalidateResult();
                    acceptInput(lastKeyPressCode, focusInputAfterSelection);
                }
            }
        }).click(function() {

        }).bind("search", function (eventTarget, eventArguments) {
            var fn = eventArguments;
            function findValueCallback(q: string, data?: Nullable<Autocomplete_Cache_Row>) {
                var result;
                if (data && data.length) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].result.toLowerCase() == q.toLowerCase()) {
                            result = data[i];
                            break;
                        }
                    }
                }
                if (typeof fn == "function") fn(result);
                else updateResult(result.data); // todo dead code?
            }
            var value = $.trim($input.val());
            requestData(value, findValueCallback, findValueCallback);
        }).bind("flushCache", function() {
            cache.flush();
        }).bind("collectOptions", function (eventTarget, eventArguments) {
            const publicOptions: Partial<BocAutoCompleteReferenceValue_CollectedOptions> = {
              serviceUrl: options.serviceUrl,
              serviceMethodSearch: options.serviceMethodSearch,
              serviceMethodSearchExact: options.serviceMethodSearchExact,
              params : {}
            };
            
            for (const propName in options.extraParams)
              publicOptions.params![propName] = options.extraParams[propName];
            
            $.extend(eventArguments, publicOptions);
        }).bind("setOptions", function (eventTarget, eventArguments) {
            $.extend(options, eventArguments);
            // if we've updated the data, repopulate
            if ("data" in eventArguments)
              cache.populate();
        }).bind("collectElements", function (eventTarget, eventArguments) {
            eventArguments.selectList = select;
            eventArguments.informationPopUp = informationPopUp;
        }).bind("showInformationPopUp", function (eventTarget, eventArguments) {
            informationPopUp.show(eventArguments.message);
        }).bind("hideInformationPopUp", function() {
            informationPopUp.hide();
        }).bind("unautocomplete", function() {
            informationPopUp.unbind();
            select.unbind();
            $input.unbind();
            $(input.form!).unbind(".autocomplete");
        });

        // re-motion: bind onChange to the dropDownButton's click event
        const dropdownButton = $('#' + options.dropDownButtonID);
        if (dropdownButton.length > 0) {
            dropdownButton.mousedown(function() {
                state.mouseDownOnSelect = true;
            });

            dropdownButton.mouseup(function() {
                state.mouseDownOnSelect = false;
            });

            dropdownButton.click(function(event) {
                // re-motion: block event bubbling
                event.stopPropagation();

                if (select.visible()) {
                    acceptInput (state.lastKeyPressCode, true);
                } else {
                    $input.focus();
                    onChange(true, $input.val());
                    clearTimeout(timeout);
                }
            });
        }

        function acceptInput(lastKeyPressCode: number, focusInputAfterSelection: boolean): void {
            let isLastKeyPressedNavigationKey = false;
            switch (lastKeyPressCode) {
                case KEY.UP:
                case KEY.DOWN:
                case KEY.PAGEUP:
                case KEY.PAGEDOWN:
                    isLastKeyPressedNavigationKey = true;
                    break;
                default:
                    isLastKeyPressedNavigationKey = false;
                    break;
            }

            const term = $input.val();
            if (isLastKeyPressedNavigationKey) {
                let index = -1;
                if (term != '')
                    index = select.findItem(term);
                        
                select.selectItem (index);
            }

            if (isLastKeyPressedNavigationKey && selectCurrent(focusInputAfterSelection)) {
                //SelectCurrent already does everything that's needed.
            } else if (lastKeyPressCode != -1) {
                acceptCurrent(true, focusInputAfterSelection);
            } else {
                closeDropDownListAndSetValue(state.previousValue, focusInputAfterSelection);
            }
        };

        // re-motion: allows empty input and invalid input
        function acceptCurrent(confirmValue: boolean, focusInputAfterSelection: boolean): void {
            const term = $input.val();
            let selectedItem = null;
            if (confirmValue && term != '' && select.visible())
            {
              const itemIndex = select.findItem (term);
              if (itemIndex != -1)
              {
                select.selectItem (itemIndex);
                selectedItem = select.selected(false);
              }
            }
            closeDropDownListAndSetValue(term, focusInputAfterSelection);

            if (state.previousValue == term && selectedItem != null) {

                if (confirmValue)
                  $input.val(selectedItem.result);

                updateResult(selectedItem.data);
                return;
            }

            state.previousValue = term;

            if (selectedItem == null) {
              options.clearRequestError();
            }

            if (selectedItem != null) {

                $input.val(selectedItem.result);
                updateResult(selectedItem.data);

            } else if (confirmValue && term != '' && !options.isAutoPostBackEnabled) {

                const successHandler = function(term: string, data: Nullable<BocAutoCompleteReferenceValue_CacheRowEntry>) {
                  stopLoading();
                  if (data != null) {
                      if ($input.val().toLowerCase() == term.toLowerCase()) {
                          $input.val(data.result);
                          updateResult(data.data);
                      }
                  } else {
                      updateResult({ DisplayName: term, UniqueIdentifier: options.nullValue });
                  }
                };

                const failureHandler = function(termParameter: string) {
                    stopLoading();
                    updateResult({ DisplayName: termParameter, UniqueIdentifier: options.nullValue });
                };

                if (isInvalidated) {
                    startLoading();
                    requestDataExact(term, successHandler, failureHandler);
                }

            } else {

              updateResult({ DisplayName: term, UniqueIdentifier: options.nullValue });

            }
        };

        function updateResult(item: BocAutoCompleteReferenceValue_UpdateResult): void {
            const out: OutBox<BocAutoCompleteReferenceValue_Item> = { Value: null };
            $input.trigger("updateResult", [item, out]);
            if (out.Value === null) {
              // When combining an auto-postback with a button click, it is possible that $input no longer points to a textfield that is still
              // part of the page. In this event, the 'updateResult' operation is not wired up, resulting in a NOP and out.Value will not have
              // been initialized.
              return;
            }

            state.previousValue = out.Value.DisplayName;
            isInvalidated = false;
          };

        function invalidateResult(): void {
            $input.trigger("invalidateResult");
            isInvalidated = true;
        }

        function selectCurrent(focusInputAfterSelection: boolean): boolean {
            const selected = select.selected(false);
            if (!selected)
                return false;

            closeDropDownListAndSetValue(selected.result, focusInputAfterSelection);
            updateResult(selected.data);

            return true;
        }

        function onChange(dropDownTriggered: boolean, currentValue: string): void {
            informationPopUp.hide();

            if (!dropDownTriggered && currentValue == state.previousValue)
                return;

            state.previousValue = currentValue;

            const openFromInput = !dropDownTriggered && options.searchStringValidationParams.inputRegex.test (currentValue);
            const openFromTrigger = dropDownTriggered && options.searchStringValidationParams.dropDownTriggerRegex.test (currentValue);

            if (openFromInput || openFromTrigger) {
                startLoading();
                let searchString = currentValue;
                if (dropDownTriggered)
                    searchString = options.searchStringValidationParams.getDropDownSearchString(searchString);

                const successHandler = function (q: string, data: Nullable<BocAutoCompleteReferenceValue_CacheRow>) {
                    stopLoading();
                    receiveData (q, data);
                    if (!select.visible() && options.noDataFoundMessage) {
                        informationPopUp.show (options.noDataFoundMessage);
                    }
                    else if (dropDownTriggered && select.visible()) {
                        const index = select.findItem(currentValue);
                        select.selectItem(index);
                    }
                };
                const failureHandler = function (termParameter: string) {
                    stopLoading();
                    closeDropDownListAndSetValue(state.previousValue, false);
                };

                requestData(searchString, successHandler, failureHandler);
            } else {
                stopLoading();
                select.hide();
                if (dropDownTriggered)
                  informationPopUp.show(options.searchStringValidationParams.dropDownTriggerRegexFailedMessage);
            }
        };

        // fills in the input box w/the first match (assumed to be the best match)
        // query: the term entered
        // sValue: the first matching result
        function autoFill(query: string, sValue: string): void {
            // re-motion: rewritten
            if (!options.autoFill)
                return;

            // if the last user key pressed was backspace or delete, don't autofill
            if (state.lastKeyPressCode == KEY.BACKSPACE || state.lastKeyPressCode == KEY.DEL)
                return;

            if (query == '')
                return;

            // autofill in the complete box w/the first match as long as the user hasn't entered in more data
            if ($input.val().toLowerCase() != query.toLowerCase())
                return;

            //sValue completely matches the user's input, don't autofill needed
            if (query.toLowerCase() == sValue.toLowerCase())
                return;

            //sValue does not start with the user's input, don't autofill
            if (sValue.length >= query.length && sValue.substring(0, query.length).toLowerCase() != query.toLowerCase())
              return;

            // fill in the value (keep the case the user has typed)
            $input.val($input.val() + sValue.substring(query.length));
            // select the portion of the value not typed by the user (so the next character will erase)
            $.Autocompleter.Selection(input, query.length, query.length + sValue.length, false);
        };

        function closeDropDownListAndSetValue(value: string, focusInputAfterSelection: boolean): void {
            // re-motion: reset the timer
            if (autoFillTimeout) {
                clearTimeout(autoFillTimeout);
                autoFillTimeout = null;
            }

            informationPopUp.hide();
            hideResults(focusInputAfterSelection);
            $input.val(value);
            resetState();
        }

        function hideResults(focusInputAfterSelection: boolean): void {
            if (state.mouseDownOnSelect)
                return;

            const wasVisible = select.visible();
            select.hide();
            clearTimeout(timeout);
            stopLoading();
            if (wasVisible) {
                // position cursor at end of input field
                $.Autocompleter.Selection(input, $input.val().length, $input.val().length, focusInputAfterSelection);
            }
        };

        function resetState(): void {
            state.mouseDownOnSelect = false;
            state.lastKeyPressCode = -1;
            state.lastKeyPressValue = null;
        };

        function receiveData(q: string, data: Nullable<BocAutoCompleteReferenceValue_CacheRow>): void {
            informationPopUp.hide();
            if (data && hasFocus) {
                select.display(data, q);
                if (data.length) {
                    autoFill(q, data[0].result);
                    select.show();
                    if (options.selectFirst ($input.val(), q)) {
                      select.selectItem (0);
                    }
                } else {
                    select.hide();
                }
            } else {
                acceptCurrent(false, false);
            }
        };

        function requestData(term: string, success: (term: string, result: Nullable<BocAutoCompleteReferenceValue_CacheRow>) => void, failure: (term: string) => void) {
            // re-motion: cancel an already running request
            abortRequest();

            // re-motion: if an async postback is in progress, updating the DOM results in an exception
            const pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
            if (pageRequestManager.get_isInAsyncPostBack()) {
                stopLoading();
                return;
            }

            options.clearRequestError();

            const data = cache.load(term);
            // recieve the cached data
            if (data && data.length) {
                success(term, data);

                // re-motion: if a webservice url and a method name have been supplied, try loading the data now
            } else if ((typeof options.serviceUrl == "string") && (options.serviceUrl.length > 0)
                        && (typeof options.serviceMethodSearch == "string") && (options.serviceMethodSearch.length > 0)) {

                // re-motion: replaced jQuery AJAX call with .NET call because of the following problem:
                //           when extending the parameter list with the necessary arguments for the web service method call,
                //           the JSON object is serialized to "key=value;" format, but the service expects JSON format ("{ key: value, ... }")
                //           see http://encosia.com/2008/06/05/3-mistakes-to-avoid-when-using-jquery-with-aspnet-ajax/ 
                //           under "JSON, objects, and strings: oh my!" for details.
                const params: Dictionary<unknown> = {
                    searchString: term,
                    completionSetCount: options.max
                };
                for (const propertyName in options.extraParams)
                  params[propertyName] = options.extraParams[propertyName];

                executingRequest = Sys.Net.WebServiceProxy.invoke(options.serviceUrl, options.serviceMethodSearch, false, params,
                                          function(result: BocAutoCompleteReferenceValue_Item[]) {
                                              executingRequest = null;
                                              const parsed = options.parse && options.parse(result) || parse(result);
                                              cache.add(term, parsed);
                                              success(term, parsed);
                                          } as any,
                                          function(err: Sys.Net.WebServiceError) {
                                            executingRequest = null;
                                            const isTimedOut = err.get_timedOut();
                                            const isAborting = !isTimedOut && err.get_statusCode() == -1;
                                            if (!isAborting)
                                            {
                                              failure(term);
                                              options.handleRequestError (err);
                                            }
                                          });
            } else {
                // if we have a failure, we need to empty the list -- this prevents the the [TAB] key from selecting the last successful match
                select.emptyList();
                failure(term);
            }
        };

        function requestDataExact(term: string, success: (term: string, result: Nullable<BocAutoCompleteReferenceValue_CacheRowEntry>) => void, failure: (term: string) => void): void {

            // re-motion: cancel an already running request
            abortRequest();

            // re-motion: if an async postback is in progress, updating the DOM results in an exception
            const pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
            if (pageRequestManager.get_isInAsyncPostBack()) {
              failure(term);
              return;
            }

            options.clearRequestError();

            const params: Dictionary<unknown> = {
              searchString: term
            };
            for (const propertyName in options.extraParams)
              params[propertyName] = options.extraParams[propertyName];

            executingRequest = Sys.Net.WebServiceProxy.invoke(options.serviceUrl, options.serviceMethodSearchExact, false, params,
                                                  function (result: Nullable<BocAutoCompleteReferenceValue_Item>) {
                                                      executingRequest = null;
                                                      let parsed: Nullable<BocAutoCompleteReferenceValue_CacheRowEntry> = null;
                                                      if (result != null) {
                                                          const resultArray = new Array ( result );
                                                          const parsedArray = options.parse && options.parse(resultArray) || parse(resultArray);
                                                          parsed = parsedArray[0];
                                                      }
                                                      success(term, parsed);
                                                  } as any,
                                                  function (err: Sys.Net.WebServiceError) {
                                                    executingRequest = null;
                                                    const isTimedOut = err.get_timedOut();
                                                    const isAborting = !isTimedOut && err.get_statusCode() == -1;
                                                    if (!isAborting)
                                                    {
                                                      failure(term);
                                                      options.handleRequestError (err);
                                                    }
                                                  });
        };

        // re-motion: cancel an already running request
        function abortRequest(): void {
            if (executingRequest != null) {
                const executor = executingRequest.get_executor();
                if (executor.get_started()) {
                    executor.abort();
                }
                executingRequest = null;
            }
        };

        function parse(data: string): Autocomplete_Cache_Row<string[]> {
            const parsed: Autocomplete_Cache_Row<string[]> = [];
            const rows = data.split("\n");
            for (var i = 0; i < rows.length; i++) {
                var row = $.trim(rows[i]);
                if (row) {
                    const rowParts = row.split("|");
                    parsed[parsed.length] = {
                        data: rowParts,
                        value: rowParts[0],
                        result: options.formatResult && options.formatResult(rowParts, rowParts[0]) || rowParts[0]
                    };
                }
            }
            return parsed;
        };

        function startLoading(): void {
            $input.addClass(options.loadingClass);
        };

        function stopLoading(): void {
            $input.removeClass(options.loadingClass);
        };

    };

    $.Autocompleter.defaults = defaultOptions;
})(Remotion.jQuery);

type BocAutoCompleteReferenceValue_CacheRowEntry = {
    data: BocAutoCompleteReferenceValue_Item;
    value: string;
    result: string;
};
type BocAutoCompleteReferenceValue_CacheRow = BocAutoCompleteReferenceValue_CacheRowEntry[];
type BocAutoCompleteReferenceValue_CacheLookup = { [firstChar: string]: BocAutoCompleteReferenceValue_CacheRow | undefined };

class BocAutoCompleteReferenceValue_Cache {
    private readonly options: BocAutoCompleteReferenceValue_Options;

    private data: BocAutoCompleteReferenceValue_CacheLookup = {};
    private length: number = 0;
    public add(q: string, value: BocAutoCompleteReferenceValue_CacheRow): void {
        if (this.length > this.options.cacheLength) {
            this.flush();
        }
        if (!this.data[q]) {
            this.length++;
        }
        this.data[q] = value;
    }

    public populate(): Optional<boolean> {
        if (!this.options.data) return false;
        // track the matches
        const stMatchSets: Dictionary<BocAutoCompleteReferenceValue_CacheRow> = {};
        let nullData = 0;

        // no url was specified, we need to adjust the cache length to make sure it fits the local data store
        if (!this.options.serviceUrl) this.options.cacheLength = 1;

        // track all options for empty search strings
        stMatchSets[""] = [];

        // loop through the array and create a lookup structure
        for (let i = 0, ol = this.options.data.length; i < ol; i++) {
            let rawValue = this.options.data[i];
            // if rawValue is a string, make an array otherwise just reference the array
            rawValue = (typeof rawValue == "string") ? [rawValue] : rawValue;

            const value = this.options.formatMatch(rawValue, i + 1, this.options.data.length);
            if (value === false)
                continue;

            const firstChar = value.charAt(0).toLowerCase();
            // if no lookup array for this character exists, look it up now
            if (!stMatchSets[firstChar])
                stMatchSets[firstChar] = [];

            // if the match is a string
            const row: BocAutoCompleteReferenceValue_CacheRowEntry = {
                value: value,
                data: rawValue,
                result: this.options.formatResult && this.options.formatResult(rawValue) || value
            };

            // push the current match into the set list
            stMatchSets[firstChar]!.push(row);

            // keep track of empty search string items
            if (nullData++ < this.options.max) {
                stMatchSets[""].push(row);
            }
        };

        // add the data items to the cache
        for (const [key, value] of Object.entries(stMatchSets)) {
            // increase the cache size
            this.options.cacheLength++;
            // add to the cache
            this.add(key, value!);
        }
    }

    public flush(): void {
        this.data = {};
        this.length = 0;
    }

    public load(q: string): Nullable<BocAutoCompleteReferenceValue_CacheRow> {
        if (!this.options.cacheLength || !this.length)
            return null;

        // if the exact item exists, use it
        if (this.data[q]) {
            return this.data[q]!;
        }
        return null;
    }

    constructor(options: BocAutoCompleteReferenceValue_Options) {
        this.options = options;

        // populate any existing data
        setTimeout(this.populate.bind(this), 25);
    }
}
Remotion.jQuery.Autocompleter.Cache = BocAutoCompleteReferenceValue_Cache;

type BocAutoCompleteReferenceValue_SelectConfig = {
    lastKeyPressCode: number;
    mouseDownOnSelect: boolean;
    previousValue: string;
    lastKeyPressValue: Nullable<string>;
};

class BocAutoCompleteReferenceValue_Select {
    private readonly options: BocAutoCompleteReferenceValue_Options;
    private readonly input: HTMLElement;
    private readonly select: (value: boolean) => void;
    private readonly config: BocAutoCompleteReferenceValue_SelectConfig;

    constructor(options: BocAutoCompleteReferenceValue_Options, input: HTMLElement, select: (value: boolean) => void, config: BocAutoCompleteReferenceValue_SelectConfig) {
        this.options = options;
        this.input = input;
        this.select = select;
        this.config = config;
    }

    private CLASSES = {
        ACTIVE: "ac_over"
    };

    private listItems: JQuery = undefined!;
    private active: number = -1;
    private data: Nullable<BocAutoCompleteReferenceValue_CacheRow> = null;
    private term: string = "";
    private needsInit: boolean = true;
    private element: Nullable<JQuery> = undefined!;
    private list: JQuery = undefined!;

    // Create results
    public init() {
        if (!this.needsInit)
            return;
        this.element = Remotion.jQuery("<div role='listbox' />")
        .hide()
        .attr("id", this.options.selectListID)
        .addClass(this.options.resultsClass)
        .css("position", "fixed")
        .appendTo(Remotion.jQuery(this.input).closest('div, td, th, body'));

        this.options.combobox.attr('aria-owns', this.options.selectListID);
        const isAria11 = this.options.combobox[0] !== this.input;
        if (isAria11) {
            this.options.combobox.attr('aria-controls', this.options.selectListID);
        }

        this.element.data('originalMaxHeight', parseInt(this.element.css('max-height'), 10));
        this.element.data('originalMaxWidth', parseInt(this.element.css('max-width'), 10));

        //re-motion: block blur bind as long we scroll dropDown list 
        let revertInputStatusTimeout: Nullable<number> = null;
        const revertInputStatus = () => {
            if (this.config.mouseDownOnSelect) {
                this.config.mouseDownOnSelect = false;
                Remotion.jQuery(this.input).focus();
            }
        }

        const innerDiv = Remotion.jQuery("<div/>").appendTo (this.element);
        innerDiv.scroll(() => {
            this.config.mouseDownOnSelect = true;
            if (revertInputStatusTimeout) 
                clearTimeout(revertInputStatusTimeout);
            revertInputStatusTimeout = setTimeout(revertInputStatus, 200);
        }).mousedown(() => {
            this.config.mouseDownOnSelect = true;
            if (revertInputStatusTimeout) 
                clearTimeout(revertInputStatusTimeout);
            revertInputStatusTimeout = setTimeout(revertInputStatus, 200);
        });

        this.list = Remotion.jQuery("<ul/>")
        .appendTo(innerDiv)
        .mouseover((event) => {
            if (this.target(event).nodeName && this.target(event).nodeName.toUpperCase() === 'LI')
            {
                this.active = Remotion.jQuery("li", this.list).removeClass(this.CLASSES.ACTIVE).index(this.target(event));
                const activeItem = Remotion.jQuery(this.target (event));
                activeItem.addClass(this.CLASSES.ACTIVE);
                // do not mark as selected.
            }
        }).click((event) => {
            this.active = Remotion.jQuery("li", this.list).removeClass(this.CLASSES.ACTIVE).attr("aria-selected", "false").index(this.target(event));
            const activeItem = Remotion.jQuery(this.target (event));
            activeItem.addClass (this.CLASSES.ACTIVE);
            activeItem.attr("aria-selected", "true");

            this.select(true);

            return false;
        }).mousedown(() => {
            this.config.mouseDownOnSelect = true;
        }).mouseup(() => {
            this.config.mouseDownOnSelect = false;
        });

        // re-motion: clean-up drop-down div during an async postback.
        const beginRequestHandler = () => {                
            Sys.WebForms.PageRequestManager.getInstance().remove_beginRequest(beginRequestHandler);
            this.element!.remove();
            this.element = null;
            this.needsInit = true;
        }
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);

        this.needsInit = false;
    }

    private target(event: Event): HTMLElement {
        let element = event.target as Nullable<HTMLElement>;
        while (element && element.tagName != "LI")
            element = element.parentNode as Nullable<HTMLElement>;
        // more fun with IE, sometimes event.target is empty, just ignore it then
        if (!element)
            return [] as any; // todo IE
        return element;
    }

    private moveSelect(step: number, updateInput: boolean): void {
        const position = this.calculatePosition(this.active, step);
        if (this.active === position)
            return;

        this.setSelect (position, updateInput);
    };

    private setSelect(position: number, updateInput: boolean): void {
        this.listItems.removeClass(this.CLASSES.ACTIVE).attr("aria-selected", "false");
        this.setPosition (position);
        const activeItem = this.listItems.slice(this.active, this.active + 1);
        activeItem.attr("aria-selected", "true");
        activeItem.addClass(this.CLASSES.ACTIVE);
        if (this.active >= 0) {
            const result = Remotion.jQuery.data(activeItem[0], "ac_data").result;
            const $input = Remotion.jQuery(this.input);
            $input.attr ("aria-activedescendant", activeItem.attr("id"));

            if (updateInput)
                $input.val(result);

            // re-motion: do not select the text in the input element when moving the drop-down selection 
            //$.Autocompleter.Selection(input, 0, input.value.length);

            const resultsElement = this.element!.children('div');

            if (this.options.scroll) {
                let offset = 0;
                this.listItems.slice(0, this.active).each(function (this: HTMLElement) {
                    offset += this.offsetHeight;
                });

                // Calculate the offset for the current item, but use twice the item-height for the scroll's top position.
                // This will mitigate the risk of cutting off the current item while at the same time offering a bit of a preview for the next item.
                if ((offset + 2 * activeItem[0].offsetHeight - resultsElement.scrollTop()) > resultsElement[0].clientHeight) {
                    resultsElement.scrollTop(offset + 2 * activeItem[0].offsetHeight - resultsElement[0].clientHeight);
                } else if (offset < resultsElement.scrollTop()) {
                    resultsElement.scrollTop(offset);
                }
            }
        }
    }

    private calculatePosition(currentPosition: number, step: number): number {
        currentPosition += step;
        if (currentPosition < 0) {
            currentPosition = 0;
        } else if (currentPosition >= this.listItems.length) {
            currentPosition = this.listItems.length - 1;
        }
        return currentPosition;
    }

    private setPosition(position: number): void {
        if (position >= this.listItems.length || position < 0) {
            this.active = -1;
        } else {
            this.active = position;
        }
    }

    private limitNumberOfItems(available: number): number {
        return this.options.max && this.options.max < available
            ? this.options.max
            : available;
    }

    private repositionTimer: Nullable<number> = null;

    private applyPositionToDropDown(): void {
        NotNullAssert(this.element);
        const reference = Remotion.jQuery(this.input).closest('.' + this.options.inputAreaClass);
        const positionOptions: BocAutoCompleteReferenceValue_PositioningOptions = { maxWidth: this.element.data('originalMaxWidth'), maxHeight: this.element.data('originalMaxHeight') };
        Remotion.jQuery.Autocompleter.applyPositionToPopUp(reference, this.element, positionOptions);
    }

    private fillList(): void {
        NotNullAssert(this.data);
        NotNullAssert(this.element);

        this.list.empty();
        const max = this.data.length;
        for (let i = 0; i < max; i++) {
            if (!this.data[i])
                continue;
            const item = this.options.formatItem(this.data[i].data, i + 1, max, this.data[i].value, this.term);
            const termElement = document.createElement("div");
            termElement.innerText = this.term;
            const termAsHtml = termElement.innerHTML;
            const li = Remotion.jQuery ("<li role='option' aria-selected='false' />")
                .html (this.options.highlight (item.html, termAsHtml))
                .attr ("id", this.options.selectListID + "_" + i)
                .attr("aria-setsize", max)
                .attr("aria-posinset", i + 1)
                .addClass (i % 2 === 0 ? "ac_even" : "ac_odd");
            if (item.class != null)
                li.addClass(item.class);
            if (item.isAnnotation) {
                li.data ('isAnnotation', 'true');
                li.addClass ('ac_disabled');
            }
            li.appendTo (this.list);
            Remotion.jQuery.data(li[0], "ac_data", this.data[i]);
        }
        this.listItems = this.list.find("li");
        const $input = Remotion.jQuery(this.input);
        if (this.options.selectFirst($input.val(), this.term)) {
            const activeItem = this.listItems.slice(0, 1);
            $input.attr("aria-activedescendant", activeItem.attr("id"));
            activeItem.addClass (this.CLASSES.ACTIVE);
            activeItem.attr ("aria-selected", "true");
            this.active = 0;
        }
        this.element.iFrameShim({top: '0px', left: '0px', width: '100%', height: '100%'});
    }

    // re-motion: Gets the index of first item matching the term. The lookup starts with the active item, 
    //            goes to the end of the list, and if no match was found, tries the opposite direction next.
    private findItemPosition(term: string, startPosition: number): number {
        if (this.data == null)
            return -1;

        const max = this.data.length;
        for (let i = startPosition; i < max; i++) {
            if (this.data[i].data.IsAnnotation === true)
                return -1;

            if (this.data[i].result.toLowerCase().indexOf(term.toLowerCase()) != -1) {
                return i;
            }
        }

        for (let i = startPosition - 1; i >= 0; i--) {
            if (this.data[i].data.IsAnnotation === true)
                return -1;

            if (this.data[i].result.toLowerCase().indexOf(term.toLowerCase()) != -1) {
                return i;
            }
        }

        return -1;
    }
    
    // re-motion: Finds the first item where predicate(data) == true.
    private _findItemPositionWhere (predicate: (value: BocAutoCompleteReferenceValue_CacheRowEntry) => boolean): number {
        if (this.data == null)
        return -1;

        const max = this.data.length;
        for (let i = 0; i < max; i++) {
        if (predicate(this.data[i]) === true) {
            return i;
        }
        }

        return -1;
    }

    public display(d: BocAutoCompleteReferenceValue_CacheRow, q: string): void {
        this.init();
        this.data = d;
        this.term = q;
        this.fillList();
    }

    public getElement(): Nullable<HTMLElement> {
        return this.element && this.element[0];
    }

    public next(): void {
        this.moveSelect(1, true);
    }

    public prev(): void {
        this.moveSelect(-1, true);
    }

    public pageUp(): void {
        if (this.active != 0 && this.active - 8 < 0) {
            this.moveSelect(-this.active, true);
        } else {
            this.moveSelect(-8, true);
        }
    }

    public pageDown(): void {
        if (this.active != this.listItems.length - 1 && this.active + 8 > this.listItems.length) {
            this.moveSelect(this.listItems.length - 1 - this.active, true);
        } else {
            this.moveSelect(8, true);
        }
    }

    public hide() {
        if (this.repositionTimer) 
            clearTimeout(this.repositionTimer);
        this.options.combobox.attr("aria-expanded", "false");
        Remotion.jQuery(this.input).removeAttr("aria-activedescendant");
        this.element && this.element.hide();
        this.listItems && this.listItems.removeClass(this.CLASSES.ACTIVE).attr("aria-selected", "false");
        this.active = -1;
    }

    public visible(): boolean {
        return (this.element && this.element.is(":visible")) ? true : false;
    }

    public current(): HTMLElement | false {
        return this.visible() && (this.listItems.filter(".[aria-selected=true]")[0] || this.options.selectFirst(Remotion.jQuery(this.input).val(), null) && this.listItems[0]);
    }

    public show(): void {
        NotNullAssert(this.element);

        // re-motion: scroll dropDown list to value from input
        let selectedItemIndex = -1;
        const inputValue = Remotion.jQuery(this.input).val().toLowerCase();
        if (inputValue.length > 0) {
            this.listItems.each(function(this: HTMLElement, i) {
                const textValue: string = Remotion.jQuery.data(this, "ac_data").result;
                if (textValue.toLowerCase().startsWith (inputValue)) {
                    selectedItemIndex = i;
                    return false;
                }
            });
        }

        // re-motion: reposition element 
        this.applyPositionToDropDown();
        
        this.element.show();
        this.options.combobox.attr("aria-expanded", "true");
        
        // re-motion: reposition element 
        if (this.repositionTimer) 
            clearTimeout(this.repositionTimer);
        const repositionHandler = () => {
            if (this.repositionTimer) {
                clearTimeout(this.repositionTimer);
            }
            if (this.element && this.element.is(':visible')) {
                this.applyPositionToDropDown();
                this.repositionTimer = setTimeout(repositionHandler, this.options.repositionInterval);
            }
        };
        this.repositionTimer = setTimeout(repositionHandler, this.options.repositionInterval);

        // re-motion: set selection
        this.setSelect (selectedItemIndex, false);

        if (this.options.scroll) {
            if (selectedItemIndex >= 0) {
                const selectedItem = this.listItems[selectedItemIndex];
                this.list.scrollTop(selectedItem.offsetTop);
            }
            else {
                this.list.scrollTop(0);
            }
        }

    }

    public selected(includeAnnoations: boolean): Nullable<BocAutoCompleteReferenceValue_CacheRowEntry> {
        // re-motion: removing the CSS class does not provide any benefits, but prevents us from highlighting the currently selected value
        // on dropDownButton Click
        // Original: const selected = listItems && listItems.filter("." + CLASSES.ACTIVE).removeClass(CLASSES.ACTIVE);
        const selected = this.listItems && this.listItems.filter(".[aria-selected=true]");
        if (!selected || !selected.length && selected.length === 0)
            return null;
        const selectedItem = selected[0];
        if (!includeAnnoations && Remotion.jQuery.data(selectedItem, 'isAnnotation') === 'true')
            return null;
        return Remotion.jQuery.data(selectedItem, "ac_data");
    }

    public emptyList(): void {
        this.list && this.list.empty();
    }

    // re-motion: returns the index of the item
    public findItem (term: string): number {
        return this.findItemPosition (term, Math.max (this.active, 0));
    }

    // re-motion: returns the index of the item matching the specified predicate
    public findItemPositionWhere (predicate: (entry: BocAutoCompleteReferenceValue_CacheRowEntry) => boolean) {
        return this._findItemPositionWhere (predicate);
    }

    // re-motion: selects the item at the specified index
    public selectItem (index: number): void {
        this.setSelect (index, false);
    }

    public unbind(): void {
        this.element && this.element.remove();
    }
};
Remotion.jQuery.Autocompleter.Select = BocAutoCompleteReferenceValue_Select;

class BocAutoCompleteReferenceValue_InformationPopUp {
    private readonly options: BocAutoCompleteReferenceValue_Options;
    private readonly input: HTMLElement;

    constructor(options: BocAutoCompleteReferenceValue_Options, input: HTMLElement) {
        this.options = options;
        this.input = input;
    }

    private needsInit: boolean = true;
    private element: Nullable<JQuery> = null!;
    private repositionTimer: Nullable<number> = null;

    private init(): void {
        if (!this.needsInit)
            return;
        this.element = Remotion.jQuery("<div role='alert' aria-atomic='true' />")
        .hide()
        .attr("id", this.options.informationPopUpID)
        .addClass(this.options.informationPopUpClass)
        .css("position", "fixed")
        .appendTo(Remotion.jQuery(this.input).closest('div, td, th, body'));
        if (this.options.combobox.attr('aria-labelledby') !== undefined) {
            this.element.attr("aria-labelledby", this.options.combobox.attr('aria-labelledby'));
        }

        this.element.data('originalMaxHeight', parseInt(this.element.css('max-height'), 10));
        this.element.data('originalMaxWidth', parseInt(this.element.css('max-width'), 10));

        const beginRequestHandler = () => {
            Sys.WebForms.PageRequestManager.getInstance().remove_beginRequest(beginRequestHandler);
            this.element!.remove();
            this.element = null;
            this.needsInit = true;
        };
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);

        this.needsInit = false;
    }

    private showPopUp(): void {
        NotNullAssert(this.element);

        this.element.iFrameShim({ top: '0px', left: '0px', width: '100%', height: '100%' });
        this.applyPositionToPopUp();
        this.element.show();
            
        if (this.repositionTimer) 
            clearTimeout(this.repositionTimer);
        const repositionHandler = () => {
            if (this.repositionTimer) {
                clearTimeout(this.repositionTimer);
            }
            if (this.element && this.element.is(':visible')) {
                this.applyPositionToPopUp();
                this.repositionTimer = setTimeout(repositionHandler, this.options.repositionInterval);
            }
        };
        this.repositionTimer = setTimeout(repositionHandler, this.options.repositionInterval);
    }

    private applyPositionToPopUp(): void {
        NotNullAssert(this.element);

        const reference = Remotion.jQuery(this.input).closest('.' + this.options.inputAreaClass);
        const positionOptions = { maxWidth: this.element.data('originalMaxWidth'), maxHeight: this.element.data('originalMaxHeight') };
        Remotion.jQuery.Autocompleter.applyPositionToPopUp(reference, this.element, positionOptions);
    }

    public getElement (): Nullable<HTMLElement> {
        return this.element && this.element[0];
    }

    public visible (): boolean {
        return (this.element && this.element.is (":visible")) ? true : false;
    }

    public show(message: string): void {
        NotNullAssert(this.element);

        this.init();
        this.element.empty();
        this.element.append(Remotion.jQuery('<div/>').html (message));
        this.showPopUp();
    }

    public hide(): void {
        if (this.repositionTimer) 
            clearTimeout(this.repositionTimer);
        this.element && this.element.hide();
    }

    public unbind(): void {
        this.element && this.element.remove();
    }
};
Remotion.jQuery.Autocompleter.InformationPopUp = BocAutoCompleteReferenceValue_InformationPopUp;

Remotion.jQuery.Autocompleter.Selection = function(field: HTMLTextAreaElement, start: number, end: number, focusInputAfterSelection: boolean): void {
    if (BrowserUtility.GetIEVersion() > 0 && field !== document.activeElement && !focusInputAfterSelection)
        return;

    if (field.value.length < 2)
        return;

    if (field.createTextRange) {
        const selRange = field.createTextRange();
        selRange.collapse(true);
        selRange.moveStart("character", start);
        selRange.moveEnd("character", end);
        selRange.select();
    } else if (field.setSelectionRange) {
        field.setSelectionRange(start, end);
    } else {
        if (field.selectionStart) {
            field.selectionStart = start;
            field.selectionEnd = end;
        }
    }

    if (focusInputAfterSelection) {
        field.focus();
    }
};

type BocAutoCompleteReferenceValue_SpaceAround = {
    top: number;
    bottom: number;
    left: number;
    right: number;
    spaceVertical: string;
    spaceHorizontal: string;
    space: string;
};

Remotion.jQuery.Autocompleter.calculateSpaceAround = function(element: JQuery): BocAutoCompleteReferenceValue_SpaceAround {
    // re-motion: make sure CSS values are allways numbers, IE can return 'auto'
    function number(num: number | string) {
        return parseInt(num as string) || 0;
    };

    const width = element.outerWidth();
    const height = element.outerHeight();
    const offset = element.offset();

    const scrollTop = number(Remotion.jQuery(document).scrollTop());
    const scrollLeft = number(Remotion.jQuery(document).scrollLeft());
    const documentWidth = number(Remotion.jQuery(window).width());
    const documentHeight = number(Remotion.jQuery(window).height());

    const space: Partial<BocAutoCompleteReferenceValue_SpaceAround> = new Object();
    space.top = offset.top - scrollTop;
    space.bottom = documentHeight - ((offset.top + height) - scrollTop);
    space.left = offset.left - scrollLeft;
    space.right = documentWidth - ((offset.left + width) - scrollLeft);

    (space.top > space.bottom) ? space.spaceVertical = 'T' : space.spaceVertical = 'B';
    (space.left > space.right) ? space.spaceHorizontal = 'L' : space.spaceHorizontal = 'R';
    space.space = space.spaceVertical + space.spaceHorizontal;

    return space as BocAutoCompleteReferenceValue_SpaceAround;
};

Remotion.jQuery.Autocompleter.applyPositionToPopUp = function (reference: JQuery, popUp: JQuery, options: BocAutoCompleteReferenceValue_PositioningOptions): void
{
    const offset = reference.offset();
    const position = Remotion.jQuery.Autocompleter.calculateSpaceAround(reference);

    const isVisibe = popUp.is (':visible');
    if (!isVisibe)
    {
        popUp.css('width', 'auto'); // clear the width before showing the popUp, otherwise, the popUp expands to 100%
        popUp.show(); // provide initial dimensions to popUp
    }

    const popUpDiv = popUp.children ('div');
    let contentHeight = Math.max(0, Math.max(popUpDiv.children().map(function (this: HTMLElement) { return this.offsetHeight + this.offsetTop; }).get()));

    let contentWidth = popUp.data('popUpContentWidth');
    if (!isVisibe)
    {
        contentWidth = Math.max(0, Math.max(popUpDiv.children().map(function (this: HTMLElement) { return this.offsetWidth + this.offsetLeft; }).get()));
        popUp.data('popUpContentWidth', contentWidth);
    }

    if (!isVisibe)
        popUp.hide();

    const maxHeightSafe = isNaN(options.maxHeight) ? (position.spaceVertical == 'T' ? position.top : position.bottom) : options.maxHeight;

    const requiredHeight = Math.min(contentHeight == 0 ? popUp.outerHeight() : contentHeight, maxHeightSafe);
    let topPosition;
    let bottomPosition;
    let maxHeight;
    if (position.spaceVertical == 'T' && requiredHeight > position.bottom)
    {
        topPosition = 'auto';
        bottomPosition = Math.max(0, position.bottom + reference.outerHeight());
        maxHeight = Math.min(position.top, maxHeightSafe);
    }
    else
    {
        topPosition = Math.max(0, (offset.top - Remotion.jQuery(document).scrollTop()) + reference.outerHeight());
        bottomPosition = 'auto';
        maxHeight = Math.min(position.bottom, maxHeightSafe);
    }

    const popUpOuterHeight = popUp.outerHeight();
    const popUpInnerHeight = popUpDiv.innerHeight();

    const scrollLeft = popUpDiv[0].scrollLeft;
    const scrollTop = popUpDiv[0].scrollTop;

    if (requiredHeight > popUpOuterHeight || requiredHeight < popUpInnerHeight)
    {
        //Reset height if content has changed to calculate new height
        popUp.css ({ height : 'auto' });
    }
    let elementHeight: number | string = popUp.outerHeight();

    if (requiredHeight < maxHeightSafe)
    {
        elementHeight = 'auto';
        maxHeight = '';
    }

    const availableWidth = position.left + reference.outerWidth();
    const minWidth = reference.outerWidth();
    const maxWidth = Math.min (isNaN (options.maxWidth) ? reference.outerWidth() : options.maxWidth, availableWidth);
    const requiredWidth = contentWidth + 30;
    const elementWidth = Math.max (Math.min (requiredWidth, maxWidth), minWidth);

    const rightPosition = position.right;

    popUp.css ({
        height : elementHeight,
        'max-height' : maxHeight,
        top : topPosition,
        bottom : bottomPosition,
        left : 'auto',
        right : rightPosition,
        width : elementWidth,
        'max-width' : 'none'
    });

    popUpDiv[0].scrollLeft = scrollLeft;
    popUpDiv[0].scrollTop = scrollTop;
};
