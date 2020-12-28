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

; (function($) {

    $.fn.extend({
        autocomplete: function(serviceUrl, serviceMethodSearch, serviceMethodSearchExact, options) {
            var $input = $(this);
            options = $.extend({}, $.Autocompleter.defaults, {
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
            }, options);

            // if highlight is set to false, replace it with a do-nothing function
            options.highlight = options.highlight || function(value) { return value; };

            // if the formatMatch option is not specified, then use formatItem for backwards compatibility
            options.formatMatch = options.formatMatch || options.formatItem;

            return this.each(function() {
                new $.Autocompleter(this, options);
            });
        },
        invalidateResult: function(handler) {
            return this.bind("invalidateResult", handler);
        },
        updateResult: function(handler) {
            return this.bind("updateResult", handler);
        },
        flushCache: function() {
            return this.trigger("flushCache");
        },
        getAutoCompleteSearchParameters: function (searchString, completionSetCount)
        {
          var collectedOptions = {};
          this.trigger("collectOptions", [collectedOptions]);

          collectedOptions.searchString = searchString;
          if(completionSetCount)
            collectedOptions.completionSetCount = completionSetCount;

          return collectedOptions;
        },
        getAutoCompleteSelectList: function ()
        {
          var collectedOptions = {};
          this.trigger("collectElements", [collectedOptions]);
          return collectedOptions.selectList;
        },
        getAutoCompleteInformationPopUp: function ()
        {
          var collectedOptions = {};
          this.trigger("collectElements", [collectedOptions]);
          return collectedOptions.informationPopUp;
        },
        setOptions: function(options) {
            return this.trigger("setOptions", [options]);
        },
        unautocomplete: function() {
            return this.trigger("unautocomplete");
        }
    });

    $.Autocompleter = function(input, options) {

      var KEY = {
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
        var $input = $ (input).addClass(options.inputClass);
        //aria-controls has been removed since it does not provide a meaningful assistance at this point in time (IE 11 / JAWS 18)
        //$input.attr ({ "aria-controls" : options.selectListID });

        // re-motion: Holds the currently executing request. 
        //            If the user types faster than the requests can be answered, the intermediate requests will be discarded.
        var executingRequest = null;
        var timeout;
        var autoFillTimeout;
        var isInvalidated = false;
        var cache = $.Autocompleter.Cache(options);
        var hasFocus = false;
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

        var state = {
            lastKeyPressCode: -1,
            mouseDownOnSelect: false,
            // holds the last text the user entered into the input element
            previousValue: '',
            lastKeyPressValue: null
        };

        var select = $.Autocompleter.Select(options, input, selectCurrent, state);
        // Perform initialize on load to ensure that screenreader can recognize the BocAutoCompleteReferenceValue as a combobox.
        // With IE 11 / JAWS 18, the input element is announced as an 'edit' input element until there is a listbox element available as well,
        // Chrome 63 does not show this behavior, so it's just some weird issue that needs to be worked around.
        select.init();

        var informationPopUp = $.Autocompleter.InformationPopUp(options, input);
        var blockSubmit;

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
                    var wasVisible = select.visible();
                    state.mouseDownOnSelect = false;

                    if (event.keyCode == KEY.RETURN) {
                        var isValueSelected = selectCurrent(false);
                        if (isValueSelected) {
                            //SelectCurrent already does everything that's needed.
                        } else {
                            var selectedItem = select.selected(true);
                            var isAnnotationSelected = selectedItem != null && selectedItem.data.IsAnnotation === true;
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
            var handleInput = function() {
                informationPopUp.hide();
                var currentValue = $input.val();
                var dropDownDelay = select.visible() ? options.dropDownRefreshDelay : options.dropDownDisplayDelay;

                timeout = setTimeout(
                    function () { 
                        onChange(false, currentValue); 
                    }, 
                    dropDownDelay);
            };

            state.lastKeyPressValue = $input.val();

            if (event.type == 'keyup') {
                var isTextChangeKey =
                       event.keyCode >= KEY.FIRSTTEXTCHARACTER
                    || event.keyCode == KEY.BACKSPACE
                    || event.keyCode == KEY.DEL
                    || event.keyCode == KEY.SPACE;

                var hasValueChanged = $input.val() != state.previousValue;

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
                        
                        var index = -1;
                        if ($input.val() != '')
                        {
                          index = select.findItem($input.val());
                        }
                        else
                        {
                          var selectedItem = select.selected(true);
                          var isAnnotationSelected = selectedItem != null && selectedItem.data.IsAnnotation === true;
                          if (isAnnotationSelected)
                            index = select.findItemPositionWhere (function (data) { return data === selectedItem });
                        }

                        select.selectItem (index);
                        
                        if (index != -1)
                          autoFill($input.val(), select.selected(true).result);
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
                var focusInputAfterSelection = false;
                var isLastKeyPressBeforeBlurHandled = state.lastKeyPressCode == -1;
                if (isLastKeyPressBeforeBlurHandled) {
                    closeDropDownListAndSetValue($input.val(), focusInputAfterSelection);
                    updateResult ({ DisplayName : $input.val(), UniqueIdentifier : options.nullValue });
                } else {
                    clearTimeout(timeout);
                    var lastKeyPressCode = state.lastKeyPressCode;
                    invalidateResult();
                    acceptInput(lastKeyPressCode, focusInputAfterSelection);
                }
            }
        }).click(function() {

        }).bind("search", function (eventTarget, eventArguments) {
            var fn = eventArguments;
            function findValueCallback(q, data) {
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
                else updateResult(result.data);
            }
            var value = $.trim($input.val());
            requestData(value, findValueCallback, findValueCallback);
        }).bind("flushCache", function() {
            cache.flush();
        }).bind("collectOptions", function (eventTarget, eventArguments) {
            var publicOptions = {
              serviceUrl: options.serviceUrl,
              serviceMethodSearch: options.serviceMethodSearch,
              serviceMethodSearchExact: options.serviceMethodSearchExact,
              params : {}
            };
            
            for (var propName in options.extraParams)
              publicOptions.params[propName] = options.extraParams[propName];
            
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
            $(input.form).unbind(".autocomplete");
        });

        // re-motion: bind onChange to the dropDownButton's click event
        var dropdownButton = $('#' + options.dropDownButtonID);
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

        function acceptInput(lastKeyPressCode, focusInputAfterSelection) {
            var isLastKeyPressedNavigationKey = false;
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

            var term = $input.val();
            if (isLastKeyPressedNavigationKey) {
                var index = -1;
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
        function acceptCurrent(confirmValue, focusInputAfterSelection) {
            var term = $input.val();
            var selectedItem = null;
            if (confirmValue && term != '' && select.visible())
            {
              var itemIndex = select.findItem (term);
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

                var successHandler = function(term, data) {
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

                var failureHandler = function(termParameter) {
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

        function updateResult(item) {
            var out = { Value: null };
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

        function invalidateResult() {
            $input.trigger("invalidateResult");
            isInvalidated = true;
        }

        function selectCurrent(focusInputAfterSelection) {
            var selected = select.selected(false);
            if (!selected)
                return false;

            closeDropDownListAndSetValue(selected.result, focusInputAfterSelection);
            updateResult(selected.data);

            return true;
        }

        function onChange(dropDownTriggered, currentValue) {
            informationPopUp.hide();

            if (!dropDownTriggered && currentValue == state.previousValue)
                return;

            state.previousValue = currentValue;

            var openFromInput = !dropDownTriggered && options.searchStringValidationParams.inputRegex.test (currentValue);
            var openFromTrigger = dropDownTriggered && options.searchStringValidationParams.dropDownTriggerRegex.test (currentValue);

            if (openFromInput || openFromTrigger) {
                startLoading();
                var searchString = currentValue;
                if (dropDownTriggered)
                    searchString = options.searchStringValidationParams.getDropDownSearchString(searchString);

                var successHandler = function (q, data) {
                    stopLoading();
                    receiveData (q, data);
                    if (!select.visible() && options.noDataFoundMessage) {
                        informationPopUp.show (options.noDataFoundMessage);
                    }
                    else if (dropDownTriggered && select.visible()) {
                        var index = select.findItem(currentValue);
                        select.selectItem(index);
                    }
                };
                var failureHandler = function (termParameter) {
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
        function autoFill(query, sValue) {
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

        function closeDropDownListAndSetValue(value, focusInputAfterSelection){
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

        function hideResults(focusInputAfterSelection) {
            if (state.mouseDownOnSelect)
                return;

            var wasVisible = select.visible();
            select.hide();
            clearTimeout(timeout);
            stopLoading();
            if (wasVisible) {
                // position cursor at end of input field
                $.Autocompleter.Selection(input, $input.val().length, $input.val().length, focusInputAfterSelection);
            }
        };

        function resetState() {
            state.mouseDownOnSelect = false;
            state.lastKeyPressCode = -1;
            state.lastKeyPressValue = null;
        };

        function receiveData(q, data) {
            informationPopUp.hide();
            if (data && hasFocus) {
                select.display(data, q);
                if (data.length) {
                    autoFill(q, data[0].result);
                    select.show();
                    if (options.selectFirst ($input.val(), q)) {
                      select.selectItem (0, false);
                    }
                } else {
                    select.hide();
                }
            } else {
                acceptCurrent(false, false);
            }
        };

        function requestData(term, success, failure) {
            // re-motion: cancel an already running request
            abortRequest();

            // re-motion: if an async postback is in progress, updating the DOM results in an exception
            var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
            if (pageRequestManager.get_isInAsyncPostBack()) {
                stopLoading();
                return;
            }

            options.clearRequestError();

            var data = cache.load(term);
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
                var params = {
                    searchString: term,
                    completionSetCount: options.max
                };
                for (var propertyName in options.extraParams)
                  params[propertyName] = options.extraParams[propertyName];

                executingRequest = Sys.Net.WebServiceProxy.invoke(options.serviceUrl, options.serviceMethodSearch, false, params,
                                          function(result, context, methodName) {
                                              executingRequest = null;
                                              var parsed = options.parse && options.parse(result) || parse(result);
                                              cache.add(term, parsed);
                                              success(term, parsed);
                                          },
                                          function(err, context, methodName) {
                                            executingRequest = null;
                                            var isTimedOut = err.get_timedOut();
                                            var isAborting = !isTimedOut && err.get_statusCode() == -1;
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

        function requestDataExact(term, success, failure) {

            // re-motion: cancel an already running request
            abortRequest();

            // re-motion: if an async postback is in progress, updating the DOM results in an exception
            var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
            if (pageRequestManager.get_isInAsyncPostBack()) {
              failure(term);
              return;
            }

            options.clearRequestError();

            var params = {
              searchString: term
            };
            for (var propertyName in options.extraParams)
              params[propertyName] = options.extraParams[propertyName];

            executingRequest = Sys.Net.WebServiceProxy.invoke(options.serviceUrl, options.serviceMethodSearchExact, false, params,
                                                  function (result, context, methodName) {
                                                      executingRequest = null;
                                                      var parsed = null;
                                                      if (result != null) {
                                                          var resultArray = new Array ( result );
                                                          var parsedArray = options.parse && options.parse(resultArray) || parse(resultArray);
                                                          parsed = parsedArray[0];
                                                      }
                                                      success(term, parsed);
                                                  },
                                                  function (err, context, methodName) {
                                                    executingRequest = null;
                                                    var isTimedOut = err.get_timedOut();
                                                    var isAborting = !isTimedOut && err.get_statusCode() == -1;
                                                    if (!isAborting)
                                                    {
                                                      failure(term);
                                                      options.handleRequestError (err);
                                                    }
                                                  });
        };

        // re-motion: cancel an already running request
        function abortRequest() {
            if (executingRequest != null) {
                var executor = executingRequest.get_executor();
                if (executor.get_started()) {
                    executor.abort();
                }
                executingRequest = null;
            }
        };

        function parse(data) {
            var parsed = [];
            var rows = data.split("\n");
            for (var i = 0; i < rows.length; i++) {
                var row = $.trim(rows[i]);
                if (row) {
                    row = row.split("|");
                    parsed[parsed.length] = {
                        data: row,
                        value: row[0],
                        result: options.formatResult && options.formatResult(row, row[0]) || row[0]
                    };
                }
            }
            return parsed;
        };

        function startLoading() {
            $input.addClass(options.loadingClass);
        };

        function stopLoading() {
            $input.removeClass(options.loadingClass);
        };

    };

    $.Autocompleter.defaults = {
        inputClass: "ac_input",
        resultsClass: "ac_results",
        loadingClass: "ac_loading",
        informationPopUpClass: "ac_informationPopUp",
        inputAreaClass: "ac_content",
        searchStringValidationParams:
          {
            inputRegex: new RegExp("\\S+"),
            dropDownTriggerRegex: new RegExp("\\S+"),
            dropDownTriggerRegexFailedMessage: null,
            getDropDownSearchString: function (searchString) { return searchString; }
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
        selectFirst: function(inputValue, searchTerm) { return true; },
        formatItem: function(row) { return { html : row[0], class : null, isAnnotation : false }; },
        formatMatch: null,
        autoFill: false,
        highlight: function(value, term) {
            if (term == '')
              return value;
            return value.replace(new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + term.replace(/([\^\$\(\)\[\]\{\}\*\.\+\?\|\\])/gi, "\\$1") + ")(?![^<>]*>)(?![^&;]+;)", "gi"), "<strong>$1</strong>");
        },
        scroll: true,
        repositionInterval: 200,
        handleRequestError: function (err) { },
        clearRequestError: function (err) { }
    };

    $.Autocompleter.Cache = function(options) {

        var data = {};
        var length = 0;

        function matchSubset(s, sub) {
            var i = s.indexOf(sub);
            if (options.matchContains == "word") {
                i = s.toLowerCase().search("\\b" + sub.toLowerCase());
            }
            if (i == -1) return false;
            return i == 0 || options.matchContains;
        };

        function add(q, value) {
            if (length > options.cacheLength) {
                flush();
            }
            if (!data[q]) {
                length++;
            }
            data[q] = value;
        }

        function populate() {
            if (!options.data) return false;
            // track the matches
            var stMatchSets = {};
            var nullData = 0;

            // no url was specified, we need to adjust the cache length to make sure it fits the local data store
            if (!options.serviceUrl) options.cacheLength = 1;

            // track all options for empty search strings
            stMatchSets[""] = [];

            // loop through the array and create a lookup structure
            for (var i = 0, ol = options.data.length; i < ol; i++) {
                var rawValue = options.data[i];
                // if rawValue is a string, make an array otherwise just reference the array
                rawValue = (typeof rawValue == "string") ? [rawValue] : rawValue;

                var value = options.formatMatch(rawValue, i + 1, options.data.length);
                if (value === false)
                    continue;

                var firstChar = value.charAt(0).toLowerCase();
                // if no lookup array for this character exists, look it up now
                if (!stMatchSets[firstChar])
                    stMatchSets[firstChar] = [];

                // if the match is a string
                var row = {
                    value: value,
                    data: rawValue,
                    result: options.formatResult && options.formatResult(rawValue) || value
                };

                // push the current match into the set list
                stMatchSets[firstChar].push(row);

                // keep track of empty search string items
                if (nullData++ < options.max) {
                    stMatchSets[""].push(row);
                }
            };

            // add the data items to the cache
            $.each(stMatchSets, function(i, value) {
                // increase the cache size
                options.cacheLength++;
                // add to the cache
                add(i, value);
            });
        }

        // populate any existing data
        setTimeout(populate, 25);

        function flush() {
            data = {};
            length = 0;
        }

        return {
            flush: flush,
            add: add,
            populate: populate,
            load: function(q) {
                if (!options.cacheLength || !length)
                    return null;

                // if the exact item exists, use it
                if (data[q]) {
                    return data[q];
                }
                return null;
            }
        };
    };

    $.Autocompleter.Select = function(options, input, select, config) {
        var CLASSES = {
            ACTIVE: "ac_over"
        };

        var listItems,
    active = -1,
    data = null,
    term = "",
    needsInit = true,
    element,
    list;

        // Create results
        function init() {
            if (!needsInit)
                return;
            element = $("<div role='listbox' />")
            .hide()
            .attr("id", options.selectListID)
            .addClass(options.resultsClass)
            .css("position", "fixed")
            .appendTo($(input).closest('div, td, th, body'));

            options.combobox.attr('aria-owns', options.selectListID);
            var isAria11 = options.combobox[0] !== input;
            if (isAria11) {
              options.combobox.attr('aria-controls', options.selectListID);
          }

            element.data('originalMaxHeight', parseInt(element.css('max-height'), 10));
            element.data('originalMaxWidth', parseInt(element.css('max-width'), 10));

            //re-motion: block blur bind as long we scroll dropDown list 
            var revertInputStatusTimeout = null;
            function revertInputStatus() {
                if (config.mouseDownOnSelect) {
                    config.mouseDownOnSelect = false;
                    $(input).focus();
                }
            }

            var innerDiv = $("<div/>").appendTo (element);
            innerDiv.scroll(function() {
                config.mouseDownOnSelect = true;
                if (revertInputStatusTimeout) 
                    clearTimeout(revertInputStatusTimeout);
                revertInputStatusTimeout = setTimeout(revertInputStatus, 200);
            }).mousedown(function() {
                config.mouseDownOnSelect = true;
                if (revertInputStatusTimeout) 
                    clearTimeout(revertInputStatusTimeout);
                revertInputStatusTimeout = setTimeout(revertInputStatus, 200);
            });

            list = $("<ul/>")
            .appendTo(innerDiv)
            .mouseover(function (event) {
                if (target(event).nodeName && target(event).nodeName.toUpperCase() === 'LI')
                {
                  active = $("li", list).removeClass(CLASSES.ACTIVE).index(target(event));
                  var activeItem = $(target (event));
                  activeItem.addClass(CLASSES.ACTIVE);
                  // do not mark as selected.
                }
            }).click(function(event) {
                active = $("li", list).removeClass(CLASSES.ACTIVE).attr("aria-selected", "false").index(target(event));
                var activeItem = $(target (event));
                activeItem.addClass (CLASSES.ACTIVE);
                activeItem.attr("aria-selected", "true");

                select(false);
                // TODO provide option to avoid setting focus again after selection? useful for cleanup-on-focus
                input.focus();

                return false;
            }).mousedown(function() {
                config.mouseDownOnSelect = true;
            }).mouseup(function() {
                config.mouseDownOnSelect = false;
            });

            // re-motion: clean-up drop-down div during an async postback.
            var beginRequestHandler = function() {                
                Sys.WebForms.PageRequestManager.getInstance().remove_beginRequest(beginRequestHandler);
                element.remove();
                element = null;
                needsInit = true;
            }
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);

            needsInit = false;
        }

        function target(event) {
            var element = event.target;
            while (element && element.tagName != "LI")
                element = element.parentNode;
            // more fun with IE, sometimes event.target is empty, just ignore it then
            if (!element)
                return [];
            return element;
        }

        function moveSelect(step, updateInput) {
            var position = calculatePosition(active, step);
            if (active === position)
              return;

            setSelect (position, updateInput);
        };

        function setSelect(position, updateInput) {
            listItems.removeClass(CLASSES.ACTIVE).attr("aria-selected", "false");
            setPosition (position);
            var activeItem = listItems.slice(active, active + 1);
            activeItem.attr("aria-selected", "true");
            activeItem.addClass(CLASSES.ACTIVE);
            if (active >= 0) {
                var result = $.data(activeItem[0], "ac_data").result;
                var $input = $(input);
                $input.attr ("aria-activedescendant", activeItem.attr("id"));

                if (updateInput)
                  $input.val(result);

                // re-motion: do not select the text in the input element when moving the drop-down selection 
                //$.Autocompleter.Selection(input, 0, input.value.length);

                var resultsElement = element.children('div');

                if (options.scroll) {
                    var offset = 0;
                    listItems.slice(0, active).each(function() {
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

        function calculatePosition(currentPosition, step) {
            currentPosition += step;
            if (currentPosition < 0) {
                currentPosition = 0;
            } else if (currentPosition >= listItems.size()) {
                currentPosition = listItems.size() - 1;
            }
            return currentPosition;
        }

        function setPosition(position) {
            if (position >= listItems.size() || position < 0) {
                active = -1;
            } else {
                active = position;
            }
        }

        function limitNumberOfItems(available) {
            return options.max && options.max < available
      ? options.max
      : available;
        }

        var repositionTimer = null;

        function applyPositionToDropDown() {
          var reference = $(input).closest('.' + options.inputAreaClass);
          var positionOptions = { maxWidth: element.data('originalMaxWidth'), maxHeight: element.data('originalMaxHeight') };
          $.Autocompleter.applyPositionToPopUp(reference, element, positionOptions);
        }

        function fillList() {
            list.empty();
            var max = data.length;
            for (var i = 0; i < max; i++) {
                if (!data[i])
                    continue;
                var item = options.formatItem(data[i].data, i + 1, max, data[i].value, term);
                var termElement = document.createElement("div");
                termElement.innerText = term;
                var termAsHtml = termElement.innerHTML;
                var li = $ ("<li role='option' aria-selected='false' />")
                  .html (options.highlight (item.html, termAsHtml))
                  .attr ("id", options.selectListID + "_" + i)
                  .attr("aria-setsize", max)
                  .attr("aria-posinset", i + 1)
                  .addClass (i % 2 === 0 ? "ac_even" : "ac_odd");
                if (item.class != null)
                  li.addClass(item.class);
                if (item.isAnnotation) {
                  li.data ('isAnnotation', 'true');
                  li.addClass ('ac_disabled');
                }
                li.appendTo (list);
                $.data(li[0], "ac_data", data[i]);
            }
            listItems = list.find("li");
            var $input = $(input);
            if (options.selectFirst($input.val(), term)) {
                var activeItem = listItems.slice(0, 1);
                $input.attr("aria-activedescendant", activeItem.attr("id"));
                activeItem.addClass (CLASSES.ACTIVE);
                activeItem.attr ("aria-selected", "true");
                active = 0;
            }
            element.iFrameShim({top: '0px', left: '0px', width: '100%', height: '100%'});
        }

        // re-motion: Gets the index of first item matching the term. The lookup starts with the active item, 
        //            goes to the end of the list, and if no match was found, tries the opposite direction next.
        function findItemPosition(term, startPosition) {
            if (data == null)
                return -1;

            var max = data.length;
            for (var i = startPosition; i < max; i++) {
                if (data[i].data.IsAnnotation === true)
                    return -1;

                if (data[i].result.toLowerCase().indexOf(term.toLowerCase()) != -1) {
                    return i;
                }
            }

            for (var i = startPosition - 1; i >= 0; i--) {
                if (data[i].data.IsAnnotation === true)
                    return -1;

                if (data[i].result.toLowerCase().indexOf(term.toLowerCase()) != -1) {
                    return i;
                }
            }

            return -1;
        }
        
        // re-motion: Finds the first item where predicate(data) == true.
        function findItemPositionWhere (predicate) {
          if (data == null)
            return -1;

          var max = data.length;
          for (var i = 0; i < max; i++) {
            if (predicate(data[i]) === true) {
              return i;
            }
          }

          return -1;
        }

        return {
            init: function (){
              init();
            },
            display: function(d, q) {
                init();
                data = d;
                term = q;
                fillList();
            },
            getElement: function () {
              return element && element[0];
            },
            next: function() {
                moveSelect(1, true);
            },
            prev: function() {
                moveSelect(-1, true);
            },
            pageUp: function() {
                if (active != 0 && active - 8 < 0) {
                    moveSelect(-active, true);
                } else {
                    moveSelect(-8, true);
                }
            },
            pageDown: function() {
                if (active != listItems.size() - 1 && active + 8 > listItems.size()) {
                    moveSelect(listItems.size() - 1 - active, true);
                } else {
                    moveSelect(8, true);
                }
            },
            hide: function() {
                if (repositionTimer) 
                    clearTimeout(repositionTimer);
                options.combobox.attr("aria-expanded", "false");
                $(input).removeAttr("aria-activedescendant");
                element && element.hide();
                listItems && listItems.removeClass(CLASSES.ACTIVE).attr("aria-selected", "false");
                active = -1;
            },
            visible: function() {
                return (element && element.is(":visible")) ? true : false;
            },
            current: function() {
                return this.visible() && (listItems.filter(".[aria-selected=true]")[0] || options.selectFirst($(input).val(), null) && listItems[0]);
            },
            show: function() {

                // re-motion: scroll dropDown list to value from input
                var selectedItemIndex = -1;
                var inputValue = $(input).val().toLowerCase();
                if (inputValue.length > 0) {
                    listItems.each(function(i) {
                        var textValue = $.data(this, "ac_data").result;
                        if (textValue.toLowerCase().startsWith (inputValue)) {
                            selectedItemIndex = i;
                            return false;
                        }
                    });
                }

                // re-motion: reposition element 
                applyPositionToDropDown();
                
                element.show();
                options.combobox.attr("aria-expanded", "true");
                
                // re-motion: reposition element 
                if (repositionTimer) 
                    clearTimeout(repositionTimer);
                var repositionHandler = function() {
                    if (repositionTimer) {
                        clearTimeout(repositionTimer);
                    }
                    if (element && element.is(':visible')) {
                        applyPositionToDropDown();
                        repositionTimer = setTimeout(repositionHandler, options.repositionInterval);
                    }
                };
                repositionTimer = setTimeout(repositionHandler, options.repositionInterval);

                // re-motion: set selection
                setSelect (selectedItemIndex, false);

                if (options.scroll) {
                    if (selectedItemIndex >= 0) {
                        var selectedItem = listItems[selectedItemIndex];
                        list.scrollTop(selectedItem.offsetTop);
                    }
                    else {
                        list.scrollTop(0);
                    }
                }

            },
            selected: function(includeAnnoations) {
                // re-motion: removing the CSS class does not provide any benefits, but prevents us from highlighting the currently selected value
                // on dropDownButton Click
                // Original: var selected = listItems && listItems.filter("." + CLASSES.ACTIVE).removeClass(CLASSES.ACTIVE);
                var selected = listItems && listItems.filter(".[aria-selected=true]");
                if (!selected || !selected.length && selected.length === 0)
                  return null;
                var selectedItem = selected[0];
                if (!includeAnnoations && $.data(selectedItem, 'isAnnotation') === 'true')
                  return null;
                return $.data(selectedItem, "ac_data");
            },
            emptyList: function() {
                list && list.empty();
            },
            // re-motion: returns the index of the item
            findItem: function (term) {
                return findItemPosition (term, Math.max (active, 0));
            },
            // re-motion: returns the index of the item matching the specified predicate
            findItemPositionWhere: function (predicate) {
              return findItemPositionWhere (predicate);
            },
            // re-motion: selects the item at the specified index
            selectItem: function (index) {
                setSelect (index, false);
            },
            unbind: function() {
                element && element.remove();
            }
        };
    };

    $.Autocompleter.InformationPopUp = function(options, input) {

        var needsInit = true;
        var element = null;
        var repositionTimer = null;

        function init() {
            if (!needsInit)
                return;
            element = $("<div role='alert' aria-atomic='true' />")
            .hide()
            .attr("id", options.informationPopUpID)
            .addClass(options.informationPopUpClass)
            .css("position", "fixed")
            .appendTo($(input).closest('div, td, th, body'));
            if (options.combobox.attr('aria-labelledby') !== undefined) {
              element.attr("aria-labelledby", options.combobox.attr('aria-labelledby'));
            }

            element.data('originalMaxHeight', parseInt(element.css('max-height'), 10));
            element.data('originalMaxWidth', parseInt(element.css('max-width'), 10));

            var beginRequestHandler = function() {
                Sys.WebForms.PageRequestManager.getInstance().remove_beginRequest(beginRequestHandler);
                element.remove();
                element = null;
                needsInit = true;
            };
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);

            needsInit = false;
        }

        function showPopUp() {
            element.iFrameShim({ top: '0px', left: '0px', width: '100%', height: '100%' });
            applyPositionToPopUp();
            element.show();
                
            if (repositionTimer) 
                clearTimeout(repositionTimer);
            var repositionHandler = function() {
                if (repositionTimer) {
                    clearTimeout(repositionTimer);
                }
                if (element && element.is(':visible')) {
                    applyPositionToPopUp();
                    repositionTimer = setTimeout(repositionHandler, options.repositionInterval);
                }
            };
            repositionTimer = setTimeout(repositionHandler, options.repositionInterval);
        }

        function applyPositionToPopUp() {
          var reference = $(input).closest('.' + options.inputAreaClass);
          var positionOptions = { maxWidth: element.data('originalMaxWidth'), maxHeight: element.data('originalMaxHeight') };
          $.Autocompleter.applyPositionToPopUp(reference, element, positionOptions);
        }

        return {
            getElement: function () {
              return element && element[0];
            },
            visible: function () {
              return (element && element.is (":visible")) ? true : false;
            },
            show: function(message) {
                init();
                element.empty();
                element.append($('<div/>').html (message));
                showPopUp();
            },
            hide: function() {
                if (repositionTimer) 
                    clearTimeout(repositionTimer);
                element && element.hide();
            },
            unbind: function() {
                element && element.remove();
            }
        };
    };

    $.Autocompleter.Selection = function(field, start, end, focusInputAfterSelection) {
        if (BrowserUtility.GetIEVersion() > 0 && field !== document.activeElement && !focusInputAfterSelection)
            return;

        if (field.value.length < 2)
            return;

        if (field.createTextRange) {
            var selRange = field.createTextRange();
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

    $.Autocompleter.calculateSpaceAround = function(element) {
        // re-motion: make sure CSS values are allways numbers, IE can return 'auto'
        function number(num) {
            return parseInt(num) || 0;
        };

        var width = element.outerWidth();
        var height = element.outerHeight();
        var offset = element.offset();

        var scrollTop = number($(document).scrollTop());
        var scrollLeft = number($(document).scrollLeft());
        var documentWidth = number($(window).width());
        var documentHeight = number($(window).height());

        var space = new Object();
        space.top = offset.top - scrollTop;
        space.bottom = documentHeight - ((offset.top + height) - scrollTop);
        space.left = offset.left - scrollLeft;
        space.right = documentWidth - ((offset.left + width) - scrollLeft);

        (space.top > space.bottom) ? space.spaceVertical = 'T' : space.spaceVertical = 'B';
        (space.left > space.right) ? space.spaceHorizontal = 'L' : space.spaceHorizontal = 'R';
        space.space = space.spaceVertical + space.spaceHorizontal;

        return space;
    };

    $.Autocompleter.applyPositionToPopUp = function (reference, popUp, options)
    {
      var offset = reference.offset();
      var position = $.Autocompleter.calculateSpaceAround(reference);

      var isVisibe = popUp.is (':visible');
      if (!isVisibe)
      {
        popUp.css('width', 'auto'); // clear the width before showing the popUp, otherwise, the popUp expands to 100%
        popUp.show(); // provide initial dimensions to popUp
      }

      var popUpDiv = popUp.children ('div');
      var contentHeight = Math.max(0, Math.max(popUpDiv.children().map(function () { return this.offsetHeight + this.offsetTop; }).get()));

      var contentWidth = popUp.data('popUpContentWidth');
      if (!isVisibe)
      {
        contentWidth = Math.max(0, Math.max(popUpDiv.children().map(function () { return this.offsetWidth + this.offsetLeft; }).get()));
        popUp.data('popUpContentWidth', contentWidth);
      }

      if (!isVisibe)
        popUp.hide();

      var maxHeightSafe = isNaN(options.maxHeight) ? (position.spaceVertical == 'T' ? position.top : position.bottom) : options.maxHeight;

      var requiredHeight = Math.min(contentHeight == 0 ? popUp.outerHeight() : contentHeight, maxHeightSafe);
      var topPosition;
      var bottomPosition;
      var maxHeight;
      if (position.spaceVertical == 'T' && requiredHeight > position.bottom)
      {
        topPosition = 'auto';
        bottomPosition = Math.max(0, position.bottom + reference.outerHeight());
        maxHeight = Math.min(position.top, maxHeightSafe);
      }
      else
      {
        topPosition = Math.max(0, (offset.top - $(document).scrollTop()) + reference.outerHeight());
        bottomPosition = 'auto';
        maxHeight = Math.min(position.bottom, maxHeightSafe);
      }

      var popUpOuterHeight = popUp.outerHeight();
      var popUpInnerHeight = popUpDiv.innerHeight();

      var scrollLeft = popUpDiv[0].scrollLeft;
      var scrollTop = popUpDiv[0].scrollTop;

      if (requiredHeight > popUpOuterHeight || requiredHeight < popUpInnerHeight)
      {
        //Reset height if content has changed to calculate new height
        popUp.css ({ height : 'auto' });
      }
      var elementHeight = popUp.outerHeight();

      if (requiredHeight < maxHeightSafe)
      {
        elementHeight = 'auto';
        maxHeight = '';
      }

      var availableWidth = position.left + reference.outerWidth();
      var minWidth = reference.outerWidth();
      var maxWidth = Math.min (isNaN (options.maxWidth) ? reference.outerWidth() : options.maxWidth, availableWidth);
      var requiredWidth = contentWidth + 30;
      var elementWidth = Math.max (Math.min (requiredWidth, maxWidth), minWidth);

      var rightPosition = position.right;

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
})(jQuery);