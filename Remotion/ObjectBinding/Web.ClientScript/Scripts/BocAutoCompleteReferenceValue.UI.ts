﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
namespace Remotion.BocAutoCompleteReferenceValue
{

    export interface BocAutoCompleteReferenceValueSearchResult
    {
        Type: string;
    }

    export interface BocAutoCompleteReferenceValueSearchResultWithValueList extends BocAutoCompleteReferenceValueSearchResult
    {
        Type: 'ValueList';
        Values: Remotion.BocAutoCompleteReferenceValue.Item[];
        Context: Nullable<string>;
        HasMoreSearchResults: boolean;
    }

    export interface BocAutoCompleteReferenceValueCacheableSearchResult
    {
        cacheRow: CacheRow;
        hasMoreSearchResults: boolean;
        context: Nullable<string>;
    }

    export type Item = {
        UniqueIdentifier: string;
        DisplayName: string;
        IconUrl: string;
        IsAnnotation: boolean;
    };

    export type UpdateResult = {
        UniqueIdentifier: string;
        DisplayName: string;
    };

    export type FormatResult = {
        html: string;
        class: Nullable<string>;
        isAnnotation: boolean;
    };

    export type Options = {
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
        loadingMatchesMessageDelay: number;
        noDataFoundMessage: string;
        loadingMoreMatchesMessage: string;
        loadedMoreMatchesMessage: string;
        matchContains: boolean;
        cacheLength: number;
        max: number;
        isAutoPostBackEnabled: boolean;
        extraParams: Dictionary<unknown>;
        selectListID: string;
        informationPopUpID: string;
        dropDownButtonID: string;
        dataType: string;
        selectFirst(inputValue: string, searchTerm: Nullable<string>): boolean;
        formatItem(item: Item, value: string, term: string): FormatResult;
        formatPlaceholderElement(placeholderElement: HTMLElement): void;
        formatMatch(item: Item): string | false;
        formatResult?(item: Item): string;
        autoFill: boolean;
        highlight(value: string, term: string): string;
        scroll: boolean;
        repositionInterval: number;
        parse(data: BocAutoCompleteReferenceValueSearchResult): BocAutoCompleteReferenceValueCacheableSearchResult;
        handleRequestError(err: Sys.Net.WebServiceError): void;
        clearRequestError(err?: Sys.Net.WebServiceError): void;
        serviceUrl: string;
        serviceMethodSearch: string;
        serviceMethodSearchExact: string;
    };

    // This type contains the properties that are required to be passed into the .autocomplete() function
    // The type-checker makes sure that all other properties are defaulted (see Object.assign in .autocomplete())
    // Nested types must be defined fully as recursion is not supported
    export type RequiredOptions = {
        nullValue: Options["nullValue"];
        dataType: Options["dataType"];
        searchStringValidationParams: {
            inputRegex?: RegExp;
            dropDownTriggerRegex?: RegExp;
            dropDownTriggerRegexFailedMessage: string;
            getDropDownSearchString?(searchString: string): string;
        },
        loadingMoreMatchesMessage: Options["loadingMoreMatchesMessage"];
        loadedMoreMatchesMessage: Options["loadedMoreMatchesMessage"];
        formatItem: Options["formatItem"];
        formatPlaceholderElement: Options["formatPlaceholderElement"];
        formatMatch: Options["formatMatch"];
        selectListID: Options["selectListID"];
        informationPopUpID: Options["informationPopUpID"];
        dropDownButtonID: Options["dropDownButtonID"];
        parse: Options["parse"];
    };

    export type PositioningOptions = {
        maxWidth: number;
        maxHeight: number;
    };

    export type CollectedOptions = {
        serviceUrl: string;
        serviceMethodSearch: string;
        serviceMethodSearchExact: string;
        params: Dictionary<unknown>;
        searchString: string;
        completionSetCount?: number;
    };

    export type AutoCompleteHTMLElement = HTMLInputElement & AutoCompletePublicApi;

    export type InvalidateResultEventHandler = () => void;
    export type UpdateResultEventHandler = (item: UpdateResult, out: OutBox<UpdateResult>) => void;

    export interface AutoCompletePublicApi {
        invalidateResult(handler: InvalidateResultEventHandler): this;
        updateResult(handler: UpdateResultEventHandler): this;
        flushCache(): this;
        getAutoCompleteSearchParameters(searchString: string, completionSetCount: number): CollectedOptions;
        getAutoCompleteSelectList(): Remotion.BocAutoCompleteReferenceValue.Select;
        getAutoCompleteInformationPopUp(): Remotion.BocAutoCompleteReferenceValue.InformationPopUp;
        hideInformationPopUp(): this;
        clear(): this;
        setOptions(options: Options): this;
        showInformationPopUp(message: string): this;
        unautocomplete(): void;
    }

    // This is used to tell the type checker that a certain variable cannot be null at a certain point
    function NotNullAssert(v: unknown): asserts v is NotNull {}

    // Defines a set of APIs exposed on the input element for testing purposes
    interface AutoCompleteTestApi {
        __getUpdateResultHandler(): Nullable<UpdateResultEventHandler>;
    }

    // Object that contains entries for all APIs that should be removed when unautocomplete is called
    const apisToCleanUp: { [K in keyof (AutoCompletePublicApi & AutoCompleteTestApi)]: null } = {
        invalidateResult: null,
        updateResult: null,
        flushCache: null,
        getAutoCompleteSearchParameters: null,
        getAutoCompleteSelectList: null,
        getAutoCompleteInformationPopUp: null,
        hideInformationPopUp: null,
        clear: null,
        setOptions: null,
        showInformationPopUp: null,
        unautocomplete: null,
        __getUpdateResultHandler: null
    };

    // This is not typed against Options because otherwise the default
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
        loadingMatchesMessageDelay: 1000,
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

    export function createAutoComplete(
        target: HTMLInputElement,
        serviceUrl: string,
        serviceMethodSearch: string,
        serviceMethodSearchExact: string,
        initialOptions: PartialWithRequiredProperties<Options, keyof RequiredOptions>): AutoCompleteHTMLElement {
        // This assignment builds the final options object and also ensures that all the option types are set up correctly
        const options: Options = Object.assign({}, defaultOptions, {
            // re-motion: instead of a single URL property, use separate service URL and service method properties. 
            //           data cannot be inserted directly any more
            serviceUrl: serviceUrl,
            serviceMethodSearch: serviceMethodSearch,
            serviceMethodSearchExact: serviceMethodSearchExact,
        }, initialOptions as RequiredOptions);

        // if highlight is set to false, replace it with a do-nothing function
        options.highlight = options.highlight || function(value) { return value; };

        // if the formatMatch option is not specified, then use formatItem for backwards compatibility
        options.formatMatch = options.formatMatch || options.formatItem;

        createAutoCompleteInternal(target, options);
        return target;
    }

    function createAutoCompleteInternal(input: HTMLInputElement, options: Options): asserts input is AutoCompleteHTMLElement {

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
        input.classList.add(options.inputClass);
        //aria-controls has been removed since it does not provide a meaningful assistance at this point in time (IE 11 / JAWS 18)
        //$input.attr ({ "aria-controls" : options.selectListID });

        // re-motion: Holds the currently executing request. 
        //            If the user types faster than the requests can be answered, the intermediate requests will be discarded.
        let executingRequest: Nullable<Sys.Net.WebRequest> = null;
        let timeout: Optional<number>;
        let autoFillTimeout: Optional<Nullable<number>>;
        let isInvalidated: boolean = false;
        const cache = new Cache(options);
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
            previousValue: input.value,
            lastKeyPressValue: null as Nullable<string>
        };

        const select = new Select(options, input, selectCurrent, requestMoreData, state);
        // Perform initialize on load to ensure that screenreader can recognize the BocAutoCompleteReferenceValue as a combobox.
        // With IE 11 / JAWS 18, the input element is announced as an 'edit' input element until there is a listbox element available as well,
        // Chrome 63 does not show this behavior, so it's just some weird issue that needs to be worked around.
        select.init();

        const informationPopUp = new InformationPopUp(options, input);

        const keyDownHandler = function(event: KeyboardEvent) {
            // track last key pressed
            state.lastKeyPressCode = event.keyCode;
            clearTimeout(timeout);
            if (state.lastKeyPressValue !== null && state.lastKeyPressValue != input.value) {
                invalidateResult();
            }
            state.lastKeyPressValue = input.value;

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
                        onChange(true, input.value);
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
                        onChange(true, input.value);
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
                        onChange(true, input.value);
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
                        onChange(true, input.value);
                    }
                    return;

                case KEY.RETURN:
                case KEY.TAB:
                case KEY.ESC:
                    // re-motion: cancel an already running request
                    stopLoading();
                    abortRequest();
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
                        if (options.isAutoPostBackEnabled) {
                            // stop default on auto-postback since the change-event already takes care of this.
                            event.preventDefault();
                            return;
                        } else if (wasVisible) {
                            // stop default for visible dropdown options since RETURN should simply select the current option without triggering a postback.
                            event.preventDefault();
                            return;
                        } else {
                            // allow default, i.e. regular textbox-behavior when no dropdown options where displayed.
                            return;
                        }
                    } else if (event.keyCode == KEY.TAB) {
                        // allow default, i.e. support TAB-based navigation.
                        return;
                    } else /* ESC */ {
                        event.preventDefault();
                        return;
                    }

                default:
                    // re-motion: cancel an already running request
                    stopLoading();
                    abortRequest();
                    // allow default for remaining keys.
                    return;
            }
        };
        input.addEventListener("keydown", keyDownHandler);

        const keyUpPasteHandler = function(event: KeyboardEvent | ClipboardEvent) { // re-motion
            const handleInput = function() {
                informationPopUp.hide();
                const currentValue = input.value;
                const dropDownDelay = select.visible() ? options.dropDownRefreshDelay : options.dropDownDisplayDelay;

                timeout = setTimeout(
                    function () { 
                        onChange(false, currentValue); 
                    }, 
                    dropDownDelay);
            };

            state.lastKeyPressValue = input.value;

            function isKeyupEvent(event: Event): event is KeyboardEvent {
                return event.type == 'keyup';
            }

            if (isKeyupEvent(event)) {
                const isTextChangeKey =
                       event.keyCode >= KEY.FIRSTTEXTCHARACTER
                    || event.keyCode == KEY.BACKSPACE
                    || event.keyCode == KEY.DEL
                    || event.keyCode == KEY.SPACE;

                const hasValueChanged = input.value != state.previousValue;

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
                        if (input.value != '')
                        {
                            index = select.findItem(input.value);
                        }
                        else
                        {
                            const selectedItem = select.selected(true);
                            const isAnnotationSelected = selectedItem != null && selectedItem.data.IsAnnotation === true;
                            if (isAnnotationSelected)
                            {
                                index = select.findItemPositionWhere (function (data) { return data === selectedItem });
                            }                            
                            else
                            {
                                index = 0;
                            }
                        }

                        select.selectItem (index);
                        
                        if (index != -1)
                            autoFill(input.value, select.selected(true)!.result);
                }, options.selectionUpdateDelay);
            }
        }
        input.addEventListener ('keyup', keyUpPasteHandler);
        input.addEventListener ('paste', keyUpPasteHandler);

        const focusHandler = function() {
            // track whether the field has focus, we shouldn't process any
            // results if the field no longer has focus
            hasFocus = true;
        };
        input.addEventListener("focus", focusHandler);

        const blurHandler = function() {
            hasFocus = false;
            if (!select.visible()) {
                clearTimeout(timeout);
                if (input.value == '')  {
                    options.clearRequestError();
                }
            }

            if (state.mouseDownOnSelect) {
                informationPopUp.hide();
            } else {
                const focusInputAfterSelection = false;
                const isLastKeyPressBeforeBlurHandled = state.lastKeyPressCode == -1;
                if (isLastKeyPressBeforeBlurHandled) {
                    closeDropDownListAndSetValue(input.value, focusInputAfterSelection);
                    updateResult ({ DisplayName : input.value, UniqueIdentifier : options.nullValue });
                } else {
                    clearTimeout(timeout);
                    const lastKeyPressCode = state.lastKeyPressCode;
                    if (state.previousValue !== input.value) {
                        invalidateResult();
                    }
                    acceptInput(lastKeyPressCode, focusInputAfterSelection);
                }
            }
        };
        input.addEventListener("blur", blurHandler);

        let invalidateResultHandler: Nullable<InvalidateResultEventHandler> = null;
        let updateResultHandler: Nullable<UpdateResultEventHandler> = null;
        const publicApi: AutoCompletePublicApi = {
            invalidateResult: function(handler: InvalidateResultEventHandler): AutoCompletePublicApi {
                invalidateResultHandler = handler;
                return this;
            },
            updateResult: function(handler: UpdateResultEventHandler): AutoCompletePublicApi {
                updateResultHandler = handler;
                return this;
            },
            flushCache: function(): AutoCompletePublicApi {
                cache.flush();
                return this;
            },
            getAutoCompleteSearchParameters: function (searchString: string, completionSetCount?: number): CollectedOptions
            {
                const collectedOptions: Partial<CollectedOptions> = {
                    serviceUrl: options.serviceUrl,
                    serviceMethodSearch: options.serviceMethodSearch,
                    serviceMethodSearchExact: options.serviceMethodSearchExact,
                    params : {}
                };
                
                for (const propName in options.extraParams)
                    collectedOptions.params![propName] = options.extraParams[propName];

                collectedOptions.searchString = searchString;
                if(completionSetCount)
                    collectedOptions.completionSetCount = completionSetCount;
    
                return collectedOptions as CollectedOptions;
            },
            getAutoCompleteSelectList: function (): Select
            {
                return select;
            },
            getAutoCompleteInformationPopUp: function (): InformationPopUp
            {
                return informationPopUp;
            },
            hideInformationPopUp: function(): AutoCompletePublicApi {
                informationPopUp.hide();
                return this;
            },
            clear: function(): AutoCompletePublicApi {
                input.value = '';
                acceptCurrent(true, false);
                return this;
            },
            setOptions: function(newOptions: Options): AutoCompletePublicApi {
                Object.assign(options, newOptions);

                return this;
            },
            showInformationPopUp: function (message: string): AutoCompletePublicApi {
                informationPopUp.show(message);
                return this;
            },
            unautocomplete: function(): void {
                informationPopUp.unbind();
                select.unbind();

                for (const publicApi in apisToCleanUp)
                    delete (input as any)[publicApi];

                input.removeEventListener("keydown", keyDownHandler);
                input.removeEventListener("keyup", keyUpPasteHandler);
                input.removeEventListener("paste", keyUpPasteHandler);
                input.removeEventListener("focus", focusHandler);
                input.removeEventListener("blur", blurHandler);
            }
        }
        Object.assign(input, publicApi);

        const testApi: AutoCompleteTestApi = {
            __getUpdateResultHandler: function (){
                return updateResultHandler;
            }
        };
        Object.assign(input, testApi);

        // re-motion: bind onChange to the dropDownButton's click event
        const dropdownButton = document.getElementById(options.dropDownButtonID);
        if (dropdownButton) {
            dropdownButton.addEventListener('mousedown', function() {
                state.mouseDownOnSelect = true;
            });

            dropdownButton.addEventListener('mouseup', function() {
                state.mouseDownOnSelect = false;
            });

            dropdownButton.addEventListener('click', function(event) {
                // re-motion: block event bubbling
                event.stopPropagation();

                if (select.visible()) {
                    acceptInput (state.lastKeyPressCode, true);
                } else {
                    input.focus();
                    onChange(true, input.value);
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

            const term = input.value;
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
            const term = input.value;
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
                    input.value = selectedItem.result;

                updateResult(selectedItem.data);
                return;
            }

            state.previousValue = term;

            if (selectedItem == null) {
                options.clearRequestError();
            }

            if (selectedItem != null) {

                input.value = selectedItem.result;
                updateResult(selectedItem.data);

            } else if (confirmValue && term != '' && !options.isAutoPostBackEnabled) {

                const successHandler = function(term: string, data: Nullable<CacheRowEntry>) {
                    stopLoading();
                    if (data != null) {
                        if (input.value.toLowerCase() == term.toLowerCase()) {
                            input.value = data.result;
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

        function updateResult(item: UpdateResult): void {
            const out: OutBox<Item> = { Value: null };
            if (updateResultHandler !== null)
                updateResultHandler(item, out);
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
            if (invalidateResultHandler !== null)
                invalidateResultHandler();
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

        function requestMoreData(term: string, offset: number, context: Nullable<string>): Promise<BocAutoCompleteReferenceValueCacheableSearchResult> {
            return new Promise((resolve, reject) => {
                const successHandler = (term: string, result: BocAutoCompleteReferenceValueCacheableSearchResult) => {
                    resolve(result);
                };
                const failureHandler = (error: string) => {
                    select.hide();
                    reject();
                };

                requestData(term, offset, context, successHandler, failureHandler);
            });
        }

        function onChange(dropDownTriggered: boolean, currentValue: string): void {
            if (informationPopUp.visible() && currentValue == state.previousValue)
                return;

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

                const successHandler = function (q: string, data: BocAutoCompleteReferenceValueCacheableSearchResult) {
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

                requestData(searchString, 0, null, successHandler, failureHandler);
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
            if (input.value.toLowerCase() != query.toLowerCase())
                return;

            //sValue completely matches the user's input, don't autofill needed
            if (query.toLowerCase() == sValue.toLowerCase())
                return;

            //sValue does not start with the user's input, don't autofill
            if (sValue.length >= query.length && sValue.substring(0, query.length).toLowerCase() != query.toLowerCase())
                return;

            // fill in the value (keep the case the user has typed)
            input.value = input.value + sValue.substring(query.length);
            // select the portion of the value not typed by the user (so the next character will erase)
            Remotion.BocAutoCompleteReferenceValue.Selection(input, query.length, query.length + sValue.length, false);
        };

        function closeDropDownListAndSetValue(value: string, focusInputAfterSelection: boolean): void {
            // re-motion: reset the timer
            if (autoFillTimeout) {
                clearTimeout(autoFillTimeout);
                autoFillTimeout = null;
            }

            informationPopUp.hide();
            hideResults(focusInputAfterSelection);
            input.value = value;
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
                Remotion.BocAutoCompleteReferenceValue.Selection(input, input.value.length, input.value.length, focusInputAfterSelection);
            }
        };

        function resetState(): void {
            state.mouseDownOnSelect = false;
            state.lastKeyPressCode = -1;
            state.lastKeyPressValue = null;
        };

        function receiveData(q: string, data: BocAutoCompleteReferenceValueCacheableSearchResult): void {
            informationPopUp.hide();
            if (data && hasFocus) {
                select.display(data, q);
                if (data.cacheRow.length) {
                    autoFill(q, data.cacheRow[0].result);
                    select.show();
                    if (options.selectFirst (input.value, q)) {
                        select.selectItem (0);
                    }
                } else {
                    select.hide();
                }
            } else {
                acceptCurrent(false, false);
            }
        };

        function requestData(term: string, offset: number, context: Nullable<string>, success: (term: string, result: BocAutoCompleteReferenceValueCacheableSearchResult) => void, failure: (term: string) => void) {
            // re-motion: cancel an already running request
            abortRequest();

            // re-motion: if an async postback is in progress, updating the DOM results in an exception
            const pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
            if (pageRequestManager.get_isInAsyncPostBack()) {
                stopLoading();
                return;
            }

            options.clearRequestError();

            const cachedSearchResult = cache.load(term, offset);
            // receive the cached data
            if (cachedSearchResult && cachedSearchResult.cacheRow.length) {
                success(term, cachedSearchResult);

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
                    completionSetOffset: offset,
                    completionSetCount: options.max,
                    context: context
                };
                for (const propertyName in options.extraParams)
                    params[propertyName] = options.extraParams[propertyName];

                executingRequest = Sys.Net.WebServiceProxy.invoke(options.serviceUrl, options.serviceMethodSearch, false, params,
                                            function(result: BocAutoCompleteReferenceValueSearchResult) {
                                                executingRequest = null;
                                                const parsed = options.parse(result);
                                                cache.add(term, offset, parsed);
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

        function requestDataExact(term: string, success: (term: string, result: Nullable<CacheRowEntry>) => void, failure: (term: string) => void): void {

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
                                                    function (result: Nullable<Item>) {
                                                        executingRequest = null;
                                                        let parsed: Nullable<CacheRowEntry> = null;
                                                        if (result != null) {
                                                            const resultArray: BocAutoCompleteReferenceValueSearchResultWithValueList = { Type: "ValueList", Values: [result], HasMoreSearchResults: false, Context: null };
                                                            const parsedArray = options.parse(resultArray);
                                                            parsed = parsedArray.cacheRow![0];
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

        function startLoading(): void {
            input.classList.add(options.loadingClass);
        };

        function stopLoading(): void {
            input.classList.remove(options.loadingClass);
        };

    };

    export const defaults = defaultOptions;

    export type CacheRowEntry = {
        data: Item;
        value: string;
        result: string;
    };
    export type CacheRow = CacheRowEntry[];

    export class Cache {
        private readonly options: Options;
        private readonly data = new Map<string, BocAutoCompleteReferenceValueCacheableSearchResult>();

        constructor(options: Options) {
            this.options = options;
        }

        public add(q: string, offset: number, value: BocAutoCompleteReferenceValueCacheableSearchResult): void {
            if (this.data.size > this.options.cacheLength) {
                this.flush();
            }

            if (offset === 0) {
                this.data.set(q, value);
            } else {
                const existingData = this.data.get(q);
                if (existingData && existingData.cacheRow.length === offset) {
                    existingData.cacheRow.push(...value.cacheRow);
                    existingData.hasMoreSearchResults = value.hasMoreSearchResults;
                    existingData.context = value.context;
                }
                // ignore offset data that does not append to an existing cache line
            }
        }

        public flush(): void {
            this.data.clear();
        }

        public load(q: string, offset: number): Nullable<BocAutoCompleteReferenceValueCacheableSearchResult> {
            if (offset !== 0)
                return null;

            return this.data.get(q) || null;
        }
    }

    export type SelectConfig = {
        lastKeyPressCode: number;
        mouseDownOnSelect: boolean;
        previousValue: string;
        lastKeyPressValue: Nullable<string>;
    };

    export class Select {
        private readonly options: Options;
        private readonly input: HTMLInputElement;
        private readonly select: (value: boolean) => void;
        private readonly loadMoreData: (term: string, offset: number, context: Nullable<string>) => Promise<BocAutoCompleteReferenceValueCacheableSearchResult>
        private readonly config: SelectConfig;

        private readonly ac_data: WeakMap<HTMLElement, CacheRowEntry> = new WeakMap();

        constructor(
            options: Options,
            input: HTMLInputElement,
            select: (value: boolean) => void,
            loadMoreData: (term: string, offset: number, context: Nullable<string>) => Promise<BocAutoCompleteReferenceValueCacheableSearchResult>,
            config: SelectConfig) {
            this.options = options;
            this.input = input;
            this.select = select;
            this.loadMoreData = loadMoreData;
            this.config = config;
        }

        private CLASSES = {
            ACTIVE: "ac_over"
        };

        private listItems: HTMLElement[] = undefined!;
        private active: number = -1;
        private data: Nullable<CacheRow> = null;
        private term: string = "";
        private context: Nullable<string> = null;
        private needsInit: boolean = true;
        private element: Nullable<HTMLElement> = undefined!;
        private announcementElement: HTMLElement = undefined!;
        private queuedLoadingAnnouncementTimeout: Nullable<number> = null;
        private placeholderItem: Nullable<HTMLLIElement> = null;
        private list: HTMLElement = undefined!;
        private isLoadingMoreData = false;

        // Create results
        public init() {
            if (!this.needsInit)
                return;

            this.element = document.createElement("div");
            this.element.setAttribute('class', this.options.resultsClass);
            this.element.style.position = 'fixed';
            LayoutUtility.Hide(this.element);

            this.announcementElement = document.createElement("span");
            this.announcementElement.role = "status";
            this.announcementElement.classList.add("screenReaderText");
            this.element.appendChild(this.announcementElement);

            this.input.closest('div, td, th, body')!.appendChild(this.element);

            this.input.setAttribute('aria-controls', this.options.selectListID);

            const elementStyle = window.getComputedStyle(this.element);
            this.element.dataset['originalMaxHeight'] = "" + parseInt(elementStyle.maxHeight, 10);
            this.element.dataset['originalMaxWidth'] = "" + parseInt(elementStyle.maxWidth, 10);

            //re-motion: block blur bind as long we scroll dropDown list 
            let revertInputStatusTimeout: Nullable<number> = null;
            const revertInputStatus = () => {
                if (this.config.mouseDownOnSelect) {
                    this.config.mouseDownOnSelect = false;
                    this.input.focus();
                }
            }

            const innerDiv = document.createElement("div");
            this.element.appendChild(innerDiv);
            innerDiv.addEventListener('scroll', () => {
                // If more data is available, the bottom element will be the placeholder. Start loading new elements
                // when the placeholder is getting scrolled into view.
                const heightPerListItem = Math.max(1, innerDiv.clientHeight / this.listItems.length);
                const scrolledToLastRealElement = Math.abs(innerDiv.scrollHeight - innerDiv.clientHeight - innerDiv.scrollTop) <= heightPerListItem;
                if (scrolledToLastRealElement && this.visible()) {
                    this.loadMoreDataIfAvailable();
                }

                this.config.mouseDownOnSelect = true;
                if (revertInputStatusTimeout) 
                    clearTimeout(revertInputStatusTimeout);
                revertInputStatusTimeout = setTimeout(revertInputStatus, 200);
            });
            innerDiv.addEventListener('mousedown', () => {
                this.config.mouseDownOnSelect = true;
                if (revertInputStatusTimeout) 
                    clearTimeout(revertInputStatusTimeout);
                revertInputStatusTimeout = setTimeout(revertInputStatus, 200);
            });

            this.list = document.createElement("ul");
            this.list.setAttribute('role', 'listbox');
            this.list.setAttribute('id', this.options.selectListID);
            innerDiv.appendChild(this.list);

            this.list.addEventListener('mouseover', (event) => {
                const listItemElement = this.target(event);
                if (listItemElement && listItemElement.nodeName && listItemElement.nodeName.toUpperCase() === 'LI')
                {
                    const listElements = Array.from(this.list.querySelectorAll<HTMLElement>("li"));
                    for (const listElement of listElements)
                        listElement.classList.remove(this.CLASSES.ACTIVE);

                    this.active = listElements.indexOf(listItemElement);
                    listItemElement.classList.add(this.CLASSES.ACTIVE);
                    // do not mark as selected.
                }
            });
            this.list.addEventListener('click', (event) => {
                const listElements = Array.from(this.list.querySelectorAll<HTMLElement>("li"));
                for (const listElement of listElements)
                {
                    listElement.classList.remove(this.CLASSES.ACTIVE);
                    listElement.setAttribute('aria-selected', 'false');
                }
                this.active = -1;

                const activeItem = this.target (event);
                if (activeItem)
                {
                    this.active = listElements.indexOf(activeItem);
                    activeItem.classList.add(this.CLASSES.ACTIVE);
                    activeItem.setAttribute('aria-selected', 'true');

                    this.select(true);
                }

                return false;
            });
            this.list.addEventListener('mousedown', () => {
                this.config.mouseDownOnSelect = true;
            });
            this.list.addEventListener('mouseup', () => {
                this.config.mouseDownOnSelect = false;
            });

            // re-motion: clean-up drop-down div during an async postback.
            const beginRequestHandler = () => {                
                Sys.WebForms.PageRequestManager.getInstance().remove_beginRequest(beginRequestHandler);
                this.element!.remove();
                this.element = null;
                this.clearAnnouncements();
                this.needsInit = true;
            }
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);

            this.needsInit = false;
        }

        private target(event: Event): Nullable<HTMLElement> {
            let element = event.target as Nullable<HTMLElement>;
            while (element && element.tagName != "LI")
                element = element.parentNode as Nullable<HTMLElement>;
            return element;
        }

        private moveSelect(step: number, updateInput: boolean): void {
            const position = this.calculatePosition(this.active, step);
            if (this.active === position)
                return;

            this.setSelect (position, updateInput);
        };

        private setSelect(position: number, updateInput: boolean): void {
            for (const listItem of this.listItems)
            {
                listItem.classList.remove(this.CLASSES.ACTIVE);
                listItem.setAttribute('aria-selected', 'false');
            }

            this.setPosition (position);
            if (this.active >= 0) {
                const activeItem = this.listItems[this.active];
                activeItem.setAttribute("aria-selected", "true");
                activeItem.classList.add(this.CLASSES.ACTIVE);

                const result = this.ac_data.get(activeItem)!.result;
                this.input.setAttribute ("aria-activedescendant", activeItem.id);

                if (updateInput)
                    this.input.value = result;

                // re-motion: do not select the text in the input element when moving the drop-down selection 
                // Remotion.BocAutoCompleteReferenceValue.Selection(input, 0, input.value.length);

                const resultsElement = this.element!.querySelector(':scope > div')!;

                if (this.options.scroll) {
                    let offset = 0;
                    this.listItems.slice(0, this.active).forEach(function (el) {
                        offset += el.offsetHeight;
                    });

                    // Calculate the offset for the current item, but use twice the item-height for the scroll's top position.
                    // This will mitigate the risk of cutting off the current item while at the same time offering a bit of a preview for the next item.
                    if ((offset + 2 * activeItem.offsetHeight - resultsElement.scrollTop) > resultsElement.clientHeight) {
                        resultsElement.scrollTop = offset + 2 * activeItem.offsetHeight - resultsElement.clientHeight;
                    } else if (offset < resultsElement.scrollTop) {
                        resultsElement.scrollTop = offset;
                    }
                }

                if (position === this.listItems.length - 1) {
                    this.loadMoreDataIfAvailable();
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

        private loadMoreDataIfAvailable() {
            if (!this.placeholderItem || this.isLoadingMoreData)
                return;

            this.clearAnnouncements();

            const loadingPromise = this.loadMoreData(this.term, this.listItems.length, this.context);
            this.addLoadingAnnouncement();
            this.isLoadingMoreData = true;
            this.placeholderItem.classList.add("ac_placeholder_loading");
            loadingPromise.then(searchResult => {
                this.addListItems(searchResult.cacheRow, searchResult.hasMoreSearchResults);
                this.context = searchResult.context;
                this.isLoadingMoreData = false;
                this.addLoadedAnnouncement();
            }, _ => {
                // Ignore the error and reset to allow triggering a load again. If there has been an error during the
                // request, the select box is closed and an information popup is shown so we don't have to handle it here.
                this.isLoadingMoreData = false;
            });
        }

        private applyPositionToDropDown(): void {
            NotNullAssert(this.element);
            const reference = this.input.closest('.' + this.options.inputAreaClass) as HTMLElement;
            const positionOptions: PositioningOptions = {
                maxWidth: parseInt(this.element.dataset['originalMaxWidth']!),
                maxHeight: parseInt(this.element.dataset['originalMaxHeight']!)
            };
            applyPositionToPopUp(reference, this.element, positionOptions);
        }

        private addListItems(items: CacheRow, hasMoreData: boolean): void {
            NotNullAssert(this.data);
            NotNullAssert(this.element);

            this.removePlaceholder();

            const escapeHelper = document.createElement("div");
            escapeHelper.innerText = this.term;
            const escapedTerm = escapeHelper.innerHTML;

            const initialInsert = this.data.length === 0;
            for (const item of items) {
                const index = this.data.length;
                this.data.push(item);

                const formatResult = this.options.formatItem(item.data, item.value, this.term);
                const li = this.createListItem(formatResult, index, escapedTerm);
                this.list.appendChild(li);
                this.listItems.push(li);
                this.ac_data.set(li, item);
            }

            if (hasMoreData) {
                const placeholder = document.createElement("li");
                placeholder.role = "option";
                placeholder.ariaSelected = "false";
                placeholder.id = `${this.options.selectListID}_placeholder`;

                placeholder.dataset["isAnnotation"] = "true";
                placeholder.addEventListener("click", () => this.loadMoreDataIfAvailable());
                this.options.formatPlaceholderElement(placeholder);

                this.list.appendChild(placeholder);
                this.placeholderItem = placeholder;
            }

            [...this.listItems].forEach((e, i) => {
                e.ariaSetSize = hasMoreData
                    ? "-1"
                    : "" + this.listItems.length;
                e.ariaPosInSet = "" + (i + 1);
            })

            if (initialInsert && this.options.selectFirst(this.input.value, this.term) && this.listItems.length > 0) {
                const activeItem = this.listItems[0];
                this.input.setAttribute("aria-activedescendant", activeItem.id);
                activeItem.classList.add (this.CLASSES.ACTIVE);
                activeItem.setAttribute ("aria-selected", "true");
                this.active = 0;
            }
        }

        private removePlaceholder() {
            if (this.placeholderItem) {
                this.placeholderItem.remove();
                this.placeholderItem = null;
            }
        }

        private createListItem(formatResult: FormatResult, index: number, escapedTerm: Nullable<string>){
            const li = document.createElement("li");
            li.role = "option";
            li.ariaSelected = "false";
            li.id = `${this.options.selectListID}_${index}`;

            li.innerHTML = escapedTerm
                ? this.options.highlight(formatResult.html, escapedTerm)
                : formatResult.html;
            li.classList.add(index % 2 === 0 ? "ac_even" : "ac_odd");

            if (formatResult.class != null)
                li.classList.add(formatResult.class);
            if (formatResult.isAnnotation) {
                li.dataset['isAnnotation'] = 'true';
                li.classList.add('ac_disabled');
            }

            return li;
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
        private _findItemPositionWhere (predicate: (value: CacheRowEntry) => boolean): number {
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

        public display(d: BocAutoCompleteReferenceValueCacheableSearchResult, q: string): void {
            this.init();
            this.data = [];
            this.term = q;
            this.context = d.context;
            this.listItems = [];
            this.list.innerHTML = '';
            this.addListItems(d.cacheRow, d.hasMoreSearchResults);
        }

        public getElement(): Nullable<HTMLElement> {
            return this.element;
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
            this.input.setAttribute("aria-expanded", "false");
            this.input.setAttribute("aria-activedescendant", "");
            this.clearAnnouncements();
            this.element && LayoutUtility.Hide(this.element);
            if (this.listItems) {
                for (const listItem of this.listItems) {
                    listItem.classList.remove(this.CLASSES.ACTIVE);
                    listItem.setAttribute("aria-selected", "false")
                }
            }
            this.active = -1;
        }

        public visible(): boolean {
            return (this.element && LayoutUtility.IsVisible(this.element)) ? true : false;
        }

        public current(): HTMLElement | false {
            return this.visible() && (this.listItems.filter(e => e.getAttribute("aria-selected") === "true")[0] || this.options.selectFirst(this.input.value, null) && this.listItems[0]);
        }

        public show(): void {
            NotNullAssert(this.element);

            // re-motion: scroll dropDown list to value from input
            let selectedItemIndex = -1;
            const inputValue = this.input.value.toLowerCase();
            if (inputValue.length > 0) {
                this.listItems.forEach((el, i) => {
                    const textValue: string = this.ac_data.get(el)!.result;
                    if (textValue.toLowerCase().startsWith (inputValue)) {
                        selectedItemIndex = i;
                        return false;
                    }
                });
            }

            // re-motion: reposition element 
            this.applyPositionToDropDown();
            
            LayoutUtility.Show(this.element);
            this.input.setAttribute("aria-expanded", "true");
            
            // re-motion: reposition element 
            if (this.repositionTimer) 
                clearTimeout(this.repositionTimer);
            const repositionHandler = () => {
                if (this.repositionTimer) {
                    clearTimeout(this.repositionTimer);
                }
                if (this.element && LayoutUtility.IsVisible(this.element)) {
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
                    this.list.scrollTop = selectedItem.offsetTop;
                }
                else {
                    this.list.scrollTop = 0;
                }
            }

        }

        public selected(includeAnnoations: boolean): Nullable<CacheRowEntry> {
            // re-motion: removing the CSS class does not provide any benefits, but prevents us from highlighting the currently selected value
            // on dropDownButton Click
            // Original: const selected = listItems && listItems.filter("." + CLASSES.ACTIVE).removeClass(CLASSES.ACTIVE);
            const selected = this.listItems && this.listItems.filter(e => e.getAttribute("aria-selected") === "true");
            if (!selected || !selected.length && selected.length === 0)
                return null;
            const selectedItem = selected[0];
            if (!includeAnnoations && selectedItem.dataset['isAnnotation'] === 'true')
                return null;
            return this.ac_data.get(selectedItem) || null;
        }

        public emptyList(): void {
            if (this.list) {
                this.list.innerHTML = '';
            }
        }

        // re-motion: returns the index of the item
        public findItem (term: string): number {
            return this.findItemPosition (term, Math.max (this.active, 0));
        }

        // re-motion: returns the index of the item matching the specified predicate
        public findItemPositionWhere (predicate: (entry: CacheRowEntry) => boolean) {
            return this._findItemPositionWhere (predicate);
        }

        // re-motion: selects the item at the specified index
        public selectItem (index: number): void {
            this.setSelect (index, false);
        }

        public unbind(): void {
            this.element && this.element.remove();
        }

        private addLoadingAnnouncement() {
            this.clearQueuedLoadingAnnouncement();
            this.queuedLoadingAnnouncementTimeout = setTimeout(() => {
                if (this.visible()) {
                    this.announcementElement.innerText = this.options.loadingMoreMatchesMessage;
                }
            }, this.options.loadingMatchesMessageDelay);
        }

        private addLoadedAnnouncement() {
            this.clearQueuedLoadingAnnouncement();
            if (this.visible()) {
                this.announcementElement.innerText = this.options.loadedMoreMatchesMessage;
            }
        }

        private clearAnnouncements() {
            this.announcementElement.innerText = "";
        }

        private clearQueuedLoadingAnnouncement() {
            if (this.queuedLoadingAnnouncementTimeout !== null) {
                clearTimeout(this.queuedLoadingAnnouncementTimeout);
                this.queuedLoadingAnnouncementTimeout = null;
            }
        }
    };

    export class InformationPopUp {
        private readonly options: Options;
        private readonly input: HTMLElement;

        constructor(options: Options, input: HTMLElement) {
            this.options = options;
            this.input = input;
        }

        private needsInit: boolean = true;
        private element: Nullable<HTMLDivElement> = null!;
        private repositionTimer: Nullable<number> = null;

        private init(): void {
            if (!this.needsInit)
                return;

            this.element = document.createElement('div') as HTMLDivElement;
            this.element.setAttribute('role', 'alert');
            this.element.setAttribute('aria-atomic', 'true');
            this.element.setAttribute('id', this.options.informationPopUpID);
            this.element.setAttribute('class', this.options.informationPopUpClass);
            this.element.style.position = 'fixed';

            LayoutUtility.Hide(this.element);
            this.input.closest('div, td, th, body')!.appendChild(this.element);

            if (this.input.getAttribute('aria-labelledby') !== null) {
                this.element.setAttribute("aria-labelledby", this.input.getAttribute('aria-labelledby')!);
            }

            const elementStyle = window.getComputedStyle(this.element);
            this.element.dataset['originalMaxHeight'] = "" + parseInt(elementStyle.maxHeight, 10);
            this.element.dataset['originalMaxWidth'] = "" + parseInt(elementStyle.maxWidth, 10);

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

            this.applyPositionToPopUp();
            LayoutUtility.Show(this.element);
                
            if (this.repositionTimer) 
                clearTimeout(this.repositionTimer);

            const repositionHandler = () => {
                if (this.repositionTimer) {
                    clearTimeout(this.repositionTimer);
                }
                if (this.element && LayoutUtility.IsVisible(this.element)) {
                    this.applyPositionToPopUp();
                    this.repositionTimer = setTimeout(repositionHandler, this.options.repositionInterval);
                }
            };
            this.repositionTimer = setTimeout(repositionHandler, this.options.repositionInterval);
        }

        private applyPositionToPopUp(): void {
            NotNullAssert(this.element);

            const reference = this.input.closest('.' + this.options.inputAreaClass) as HTMLElement;
            const positionOptions = {
                maxWidth: parseInt(this.element.dataset['originalMaxWidth']!, 10),
                maxHeight: parseInt(this.element.dataset['originalMaxHeight']!, 10)
            };
            applyPositionToPopUp(reference, this.element, positionOptions);
        }

        public getElement (): Nullable<HTMLElement> {
            return this.element;
        }

        public visible (): boolean {
            return (this.element && LayoutUtility.IsVisible(this.element)) ? true : false;
        }

        public show(message: string): void {
            NotNullAssert(this.element);

            this.init();
            this.element.innerHTML = '';

            const messageDiv = document.createElement("div");
            messageDiv.innerHTML = message;

            this.element.appendChild(messageDiv);
            this.showPopUp();
        }

        public hide(): void {
            if (this.repositionTimer) 
                clearTimeout(this.repositionTimer);
            this.element && LayoutUtility.Hide(this.element);
        }

        public unbind(): void {
            this.element && this.element.remove();
        }
    };

    export function Selection(field: HTMLInputElement, start: number, end: number, focusInputAfterSelection: boolean): void {
        if (field.value.length < 2)
            return;

        if (field.setSelectionRange) {
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

    type SpaceAround = {
        top: number;
        bottom: number;
        left: number;
        right: number;
        spaceVertical: string;
        spaceHorizontal: string;
        space: string;
    };

    function calculateSpaceAround(element: HTMLElement): SpaceAround {
        // re-motion: make sure CSS values are allways numbers, IE can return 'auto'
        function number(num: number | string) {
            return parseInt(num as string) || 0;
        };

        const width = LayoutUtility.GetOuterWidth(element);
        const height = LayoutUtility.GetOuterHeight(element);
        const offset = LayoutUtility.GetOffset(element);

        const scrollTop = number(window.pageYOffset);
        const scrollLeft = number(window.pageXOffset);
        const documentWidth = number(document.documentElement.clientWidth);
        const documentHeight = number(document.documentElement.clientHeight);

        const space: Partial<SpaceAround> = new Object();
        space.top = offset.top - scrollTop;
        space.bottom = documentHeight - ((offset.top + height) - scrollTop);
        space.left = offset.left - scrollLeft;
        space.right = documentWidth - ((offset.left + width) - scrollLeft);

        (space.top > space.bottom) ? space.spaceVertical = 'T' : space.spaceVertical = 'B';
        (space.left > space.right) ? space.spaceHorizontal = 'L' : space.spaceHorizontal = 'R';
        space.space = space.spaceVertical + space.spaceHorizontal;

        return space as SpaceAround;
    };

    function applyPositionToPopUp(reference: HTMLElement, popUp: HTMLElement, options: PositioningOptions): void
    {
        const offset = LayoutUtility.GetOffset(reference);
        const position = calculateSpaceAround(reference);

        const isVisibe = LayoutUtility.IsVisible(popUp);
        let scrollbarGutterValueBackup: string  = "";
        if (!isVisibe)
        {
            popUp.style.width = 'auto'; // clear the width before showing the popUp, otherwise, the popUp expands to 100%
            scrollbarGutterValueBackup = popUp.style.getPropertyValue("scrollbar-gutter");
            popUp.style.setProperty("scrollbar-gutter", "stable")
            LayoutUtility.Show(popUp); // provide initial dimensions to popUp
        }

        const popUpDiv = popUp.querySelector (':scope > div')!;
        let contentHeight = Math.max(0, Math.max(...(Array.from(popUpDiv.children) as HTMLElement[]).map(function (el) { return el.offsetHeight + el.offsetTop; })));

        let contentWidth = parseInt(popUp.dataset['popUpContentWidth']!, 10);
        let scrollbarWidth = parseInt(popUp.dataset['popUpScrollbarWidth']!, 10);
        if (!isVisibe)
        {
            const totalWidthWithScrollbar = LayoutUtility.GetOuterWidth(popUp);

            popUp.style.setProperty("scrollbar-gutter", scrollbarGutterValueBackup);
            const htmlPopUpDiv = popUpDiv as HTMLElement;
            const width = LayoutUtility.GetOuterWidth(popUp);
            contentWidth = Math.max(0, width);

            scrollbarWidth = totalWidthWithScrollbar - width;

            popUp.dataset['popUpContentWidth'] = '' + contentWidth;
            popUp.dataset['popUpScrollbarWidth'] = '' + scrollbarWidth;
        }

        if (!isVisibe)
            LayoutUtility.Hide(popUp);

        const maxHeightSafe = isNaN(options.maxHeight) ? (position.spaceVertical == 'T' ? position.top : position.bottom) : options.maxHeight;

        const requiredHeight = Math.min(contentHeight == 0 ? LayoutUtility.GetOuterHeight(popUp) : contentHeight, maxHeightSafe);
        let topPosition;
        let bottomPosition;
        let maxHeight;
        let elementHeight: number | string;
        if (position.spaceVertical == 'T' && requiredHeight > position.bottom)
        {
            topPosition = 'auto';
            bottomPosition = Math.max(0, position.bottom + LayoutUtility.GetOuterHeight(reference));
            maxHeight = Math.min(position.top, maxHeightSafe);
            elementHeight = Math.round(window.innerHeight - bottomPosition);
        }
        else
        {
            topPosition = Math.max(0, (offset.top - window.pageYOffset) + LayoutUtility.GetOuterHeight(reference));
            bottomPosition = 'auto';
            maxHeight = Math.min(position.bottom, maxHeightSafe);
            elementHeight = Math.round(window.innerHeight - topPosition);
        }

        const scrollLeft = popUpDiv.scrollLeft;
        const scrollTop = popUpDiv.scrollTop;

        if (requiredHeight < maxHeightSafe)
        {
            elementHeight = 'auto';
            maxHeight = '';
        }

        const referenceWidth = LayoutUtility.GetOuterWidth(reference);
        const marginLeft = 30;
        const availableWidth = position.left + referenceWidth - marginLeft;
        const isScrollbarRequired = requiredHeight >= maxHeightSafe;
        // js rounding errors sometimes create linebreaks
        // therefore we add a single pixel for the field to always be wide enough:
        const requiredWidth = contentWidth + 1 + (isScrollbarRequired ? scrollbarWidth : 0);
        const maxWidth = Math.min (isNaN (options.maxWidth) ? referenceWidth : options.maxWidth, availableWidth);
        const maxAllowedWidth = Math.min(requiredWidth, maxWidth)
        const elementWidth = Math.max(referenceWidth, maxAllowedWidth)

        const rightPosition = position.right;

        popUp.style.height = LayoutUtility.FormatPixelProperty(elementHeight);
        popUp.style.maxHeight =  LayoutUtility.FormatPixelProperty(maxHeight);
        popUp.style.top =  LayoutUtility.FormatPixelProperty(topPosition);
        popUp.style.bottom =  LayoutUtility.FormatPixelProperty(bottomPosition);
        popUp.style.left = 'auto';
        popUp.style.right = LayoutUtility.FormatPixelProperty(rightPosition);
        popUp.style.width = LayoutUtility.FormatPixelProperty(elementWidth);
        popUp.style.maxWidth = 'none';

        popUpDiv.scrollLeft = scrollLeft;
        popUpDiv.scrollTop = scrollTop;
    };
}