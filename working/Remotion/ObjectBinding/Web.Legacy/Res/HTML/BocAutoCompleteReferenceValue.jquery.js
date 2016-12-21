// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
/*
* Autocomplete - jQuery plugin 1.1pre
*
* Copyright (c) 2007 Dylan Verheul, Dan G. Switzer, Anjesh Tuladhar, JÃ¶rn Zaefferer
*
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
*
* Revision: $Id: jquery.autocomplete.js 5785 2008-07-12 10:37:33Z joern.zaefferer $
*
*/

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
                // re-motion: clicking this control will display the dropdown list with an assumed input of '' (regardless of textbox value)
                dropDownButtonId: null
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
        search: function(handler) {
            return this.trigger("search", [handler]);
        },
        flushCache: function() {
            return this.trigger("flushCache");
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
            SPACE: 32,
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
            FIRSTTEXTCHARACTER: 48 // 0
        };

        // Create $ object for input element
        var $input = $(input).attr("autocomplete", "off").addClass(options.inputClass);

        // re-motion: Holds the currently executing request. 
        //            If the user types faster than the requests can be answered, the intermediate requests will be discarded.
        var executingRequest = null;
        var timeout;
        var autoFillTimeout;
        // holds the last text the user entered into the input element
        var previousValue = '';
        // re-motion: holds the last valid value of the input element
        var previousValidValue = $input.val();
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
        var lastKeyPressCode = -1;
        var config = {
            mouseDownOnSelect: false
        };
        var select = $.Autocompleter.Select(options, input, selectCurrent, config);

        var blockSubmit;

        // prevent form submit in opera when selecting with return key
        $.browser.opera && $(input.form).bind("submit.autocomplete", function() {
            if (blockSubmit) {
                blockSubmit = false;
                return false;
            }
        });



        // only opera doesn't trigger keydown multiple times while pressed, others don't work with keypress at all
        $input.bind(($.browser.opera ? "keypress" : "keydown") + ".autocomplete", function(event) {
            // track last key pressed
            lastKeyPressCode = event.keyCode;

            clearTimeout(timeout);
            // re-motion: cancel an already running request
            stopLoading();
            abortRequest();

            switch (event.keyCode) {
                case KEY.UP:
                    event.preventDefault();
                    // re-motion: block event bubbling
                    event.stopPropagation();
                    if (select.visible()) {
                        select.prev();
                    } else {
                        onChange(0, true, $input.val());
                    }
                    return;

                case KEY.DOWN:
                    event.preventDefault();
                    // re-motion: block event bubbling
                    event.stopPropagation();
                    if (select.visible()) {
                        select.next();
                    } else {
                        onChange(0, true, $input.val());
                    }
                    return;

                case KEY.PAGEUP:
                    event.preventDefault();
                    // re-motion: block event bubbling
                    event.stopPropagation();
                    if (select.visible()) {
                        select.pageUp();
                    } else {
                        onChange(0, true, $input.val());
                    }
                    return;

                case KEY.PAGEDOWN:
                    event.preventDefault();
                    // re-motion: block event bubbling
                    event.stopPropagation();
                    if (select.visible()) {
                        select.pageDown();
                    } else {
                        onChange(0, true, $input.val());
                    }
                    return;

                // matches also semicolon                              
                case options.multiple && $.trim(options.multipleSeparator) == "," && KEY.COMMA:
                case KEY.RETURN:
                case KEY.TAB:
                case KEY.ESC:
                    // re-motion: block event bubbling
                    event.stopPropagation();
                    var wasVisible = select.visible();
                    config.mouseDownOnSelect = false;

                    var isCancelKey = event.keyCode == KEY.TAB || event.keyCode == KEY.ESC;
                    if (!isCancelKey && selectCurrent()) {
                        //SelectCurrent already does everything that's needed.
                    } else {
                        acceptCurrent(true);
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
                var currentValue = $input.val();
                var dropDownDelay = select.visible() ? options.dropDownRefreshDelay : options.dropDownDisplayDelay;
                timeout = setTimeout(
                    function () { 
                        onChange(0, false, currentValue); 
                    }, 
                    dropDownDelay);
            };

            if (event.type == 'keyup') {
              var isControlKey = event.keyCode < KEY.FIRSTTEXTCHARACTER
                && event.keyCode != KEY.BACKSPACE
                  && event.keyCode != KEY.DEL
                    && event.keyCode != KEY.SPACE;
                var isValueSeparatorKey = options.multiple && $.trim(options.multipleSeparator) == "," && event.keyCode ==  KEY.COMMA;
                if (!isControlKey && !isValueSeparatorKey)
                    handleInput();
            } else if (event.type == 'paste') {
                lastKeyPressCode = KEY.FIRSTTEXTCHARACTER;
                setTimeout(handleInput, 0);
            } else {
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
                            index = select.findItem ($input.val());
                        
                        select.selectItem (index);
                        
                        if (index != -1){
                            autoFill ($input.val(), select.selected().result);
                    }
                }, options.selectionUpdateDelay);
            }
        }).focus(function() {
            // track whether the field has focus, we shouldn't process any
            // results if the field no longer has focus
            hasFocus = true;
        }).blur(function() {
            hasFocus = false;
            if (select.visible() && !config.mouseDownOnSelect) {
                var value = $input.val();
                clearTimeout(timeout);
                timeout = setTimeout(
                    function() {
                        acceptInput (lastKeyPressCode);
                    }, 
                    200);
            }
        }).click(function() {

        }).bind("search", function() {
            // TODO why not just specifying both arguments?
            var fn = (arguments.length > 1) ? arguments[1] : null;
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
                else $input.trigger("updateResult", result.data);
            }
            $.each(trimWords($input.val()), function(i, value) {
                requestData(value, findValueCallback, findValueCallback);
            });
        }).bind("flushCache", function() {
            cache.flush();
        }).bind("setOptions", function() {
            $.extend(options, arguments[1]);
            // if we've updated the data, repopulate
            if ("data" in arguments[1])
                cache.populate();
        }).bind("unautocomplete", function() {
            select.unbind();
            $input.unbind();
            $(input.form).unbind(".autocomplete");
        });

        // re-motion: bind onChange to the dropDownButton's click event
        var dropdownButton = $('#' + options.dropDownButtonId);
        if (dropdownButton.length > 0) {
            dropdownButton.mousedown(function() {
                config.mouseDownOnSelect = true;
            });

            dropdownButton.mouseup(function() {
                config.mouseDownOnSelect = false;
            });

            dropdownButton.click(function(event) {
                // re-motion: block event bubbling
                event.stopPropagation();

                if (select.visible()) {
                    acceptInput (lastKeyPressCode);
                } else {
                    $input.focus();
                    onChange(1, true, $input.val());
                    clearTimeout(timeout);
                }
            });
        }

        function acceptInput(lastKeyPressCode) {
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

            if (isLastKeyPressedNavigationKey) {
                var index = -1;
                if ($input.val() != '')
                    index = select.findItem ($input.val());
                        
                select.selectItem (index);
            }

            if (isLastKeyPressedNavigationKey && selectCurrent()) {
                //SelectCurrent already does everything that's needed.
            } else if (lastKeyPressCode != -1) {
                acceptCurrent(true);
            } else {
                closeDropDownListAndSetValue(previousValidValue);
            }
        };

        // re-motion: allows empty input and invalid input
        function acceptCurrent(confirmValue) {
            var term = $input.val();
            var selectedItem = null;
            if (confirmValue && term != '' && select.visible())
            {
              var itemIndex = select.findItem (term);
              var item = null;
              if (itemIndex != -1)
              {
                select.selectItem (itemIndex);
                selectedItem = select.selected();
              }
            }
            closeDropDownListAndSetValue(term);

            if (previousValidValue == term) {
              return;
            } else {
              previousValue = term;
              previousValidValue = term;
            }

            if (selectedItem != null) {

                $input.val(selectedItem.result);
                $input.trigger("updateResult", selectedItem.data);

            } else if (confirmValue && term != '' && !options.isAutoPostBackEnabled) {

                var successHandler = function(term, data) {
                  if (data != null) {
                      stopLoading();
                      if ($input.val().toLowerCase() == term.toLowerCase()) {
                          previousValue = data.result;
                          previousValidValue = data.result;
                          $input.val(data.result);
                          $input.trigger("updateResult", data.data);
                      }
                  } else {
                      stopLoading();
                      $input.trigger("updateResult", { DisplayName: term, UniqueIdentifier: options.nullValue });
                  }
                };

                var failureHandler = function() {
                    stopLoading();
                    $input.trigger("updateResult", { DisplayName: term, UniqueIdentifier: options.nullValue });
                };

              startLoading();
              $input.trigger("invalidateResult");
              requestDataExact (term, successHandler, failureHandler);

            } else {

                $input.trigger("updateResult", { DisplayName: term, UniqueIdentifier: options.nullValue });

            }
        };

        function selectCurrent() {
            var selected = select.selected();
            if (!selected)
                return false;

            var v = selected.result;

            previousValue = v;
            previousValidValue = v;

            if (options.multiple) {
                var words = trimWords($input.val());
                if (words.length > 1) {
                    v = words.slice(0, words.length - 1).join(options.multipleSeparator) + options.multipleSeparator + v;
                }
                v += options.multipleSeparator;
            }

            closeDropDownListAndSetValue(v);
            $input.trigger("updateResult", selected.data);

            return true;
        }

        // re-motion: use obsolete first parameter to indicate whether the onChange event is triggered by input (0) or the dropdownButton (1)
        function onChange(isDropDown, skipPrevCheck, currentValue) {
            if (lastKeyPressCode == KEY.DEL) {
                select.hide();
                return;
            }

            if (!skipPrevCheck && currentValue == previousValue)
                return;

            previousValue = currentValue;

            currentValue = lastWord(currentValue);
            if (currentValue.length >= options.minChars) {
                startLoading();
                if (!options.matchCase)
                    currentValue = currentValue.toLowerCase();

                var failureHandler = function() {
                    closeDropDownListAndSetValue(previousValidValue);
                }

                // re-motion: if triggered by dropDownButton, get the full list
                if (isDropDown == 1) {
                    requestData('', receiveData, failureHandler);
                } else {
                    requestData(currentValue, receiveData, failureHandler);
                }

            } else {
                stopLoading();
                select.hide();
            }
        };

        function trimWords(value) {
            if (!value) {
                return [""];
            }
            var words = value.split(options.multipleSeparator);
            var result = [];
            $.each(words, function(i, value) {
                if ($.trim(value))
                    result[i] = $.trim(value);
            });
            return result;
        }

        function lastWord(value) {
            if (!options.multiple)
                return value;
            var words = trimWords(value);
            return words[words.length - 1];
        }

        // fills in the input box w/the first match (assumed to be the best match)
        // query: the term entered
        // sValue: the first matching result
        function autoFill(query, sValue) {
            // re-motion: rewritten
            if (!options.autoFill)
                return;

            // if the last user key pressed was backspace or delete, don't autofill
            if (lastKeyPressCode == KEY.BACKSPACE || lastKeyPressCode == KEY.DEL)
                return;

            if (query == '')
                return;

            // autofill in the complete box w/the first match as long as the user hasn't entered in more data
            if (lastWord($input.val()).toLowerCase() != query.toLowerCase())
                return;

            var lastWordInQuery = lastWord(query);
            //sValue completely matches the user's input, don't autofill needed
            if (lastWordInQuery.toLowerCase() == sValue.toLowerCase())
                return;

            //sValue does not start with the user's input, don't autofill
            if (sValue.length >= lastWordInQuery.length && sValue.substring(0, lastWordInQuery.length).toLowerCase() != lastWordInQuery.toLowerCase())
              return;

            // fill in the value (keep the case the user has typed)
            $input.val($input.val() + sValue.substring(lastWordInQuery.length));
            // select the portion of the value not typed by the user (so the next character will erase)
            $.Autocompleter.Selection(input, query.length, query.length + sValue.length);
        };

        function closeDropDownListAndSetValue(value){
            // re-motion: reset the timer
            if (autoFillTimeout) {
                clearTimeout(autoFillTimeout);
                autoFillTimeout = null;
            }

            hideResults();
            $input.val(value);
            resetState();
        }

        function hideResults() {
            if (config.mouseDownOnSelect)
                return;

            var wasVisible = select.visible();
            select.hide();
            clearTimeout(timeout);
            stopLoading();
            if (options.mustMatch) {
                // call search and run callback
                $input.search(
                    function(result) {
                        // if no value found, clear the input box
                        if (!result) {
                            if (options.multiple) {
                                var words = trimWords($input.val()).slice(0, -1);
                                $input.val(words.join(options.multipleSeparator) + (words.length ? options.multipleSeparator : ""));
                            }
                            else
                                $input.val("");
                        }
                    }
                  );
            }
            if (wasVisible)
            // position cursor at end of input field
                $.Autocompleter.Selection(input, $input.val().length, $input.val().length);
        };
            
        function resetState() {
            lastKeyPressCode = -1;
            previousValue = '';
            config.mouseDownOnSelect = false;
        };

        function receiveData(q, data) {
            if (data && hasFocus) {
                stopLoading();
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
                acceptCurrent(false);
            }
        };

        function requestData(term, success, failure) {
            if (!options.matchCase)
                term = term.toLowerCase();

            // re-motion: cancel an already running request
            abortRequest();

            // re-motion: if an async postback is in progress, updating the DOM results in an exception
            var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
            if (pageRequestManager.get_isInAsyncPostBack()) {
                failure(term);
                return;
            }

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
                    searchString: (options.ignoreInput ? '' : lastWord(term)),
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
                                          });
            } else {
                // if we have a failure, we need to empty the list -- this prevents the the [TAB] key from selecting the last successful match
                select.emptyList();
                failure(term);
            }
        };

        function requestDataExact(term, success, failure) {
            if (!options.matchCase)
              term = term.toLowerCase();

            // re-motion: cancel an already running request
            abortRequest();

            // re-motion: if an async postback is in progress, updating the DOM results in an exception
            var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
            if (pageRequestManager.get_isInAsyncPostBack())
            {
              failure(term);
              return;
            }

            var params = {
              searchString: lastWord(term)
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
        minChars: 1,
        // re-motion: modified delay concept
        dropDownDisplayDelay: 400,
        dropDownRefreshDelay: 400,
        selectionUpdateDelay: 400,
        matchCase: false,
        matchSubset: true,
        matchContains: false,
        cacheLength: 10,
        max: 100,
        mustMatch: false,
        isAutoPostBackEnabled: false,
        extraParams: {},
        // re-motion: changed selectFirst from boolean field to function
        selectFirst: function(inputValue, searchTerm) { return true; },
        formatItem: function(row) { return row[0]; },
        formatMatch: null,
        autoFill: false,
        width: 0,
        multiple: false,
        multipleSeparator: ", ",
        highlight: function(value, term) {
            if (term == '')
              return value;
            return value.replace(new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + term.replace(/([\^\$\(\)\[\]\{\}\*\.\+\?\|\\])/gi, "\\$1") + ")(?![^<>]*>)(?![^&;]+;)", "gi"), "<strong>$1</strong>");
        },
        scroll: true,
        scrollHeight: 180,
        repositionInterval: 200
    };

    $.Autocompleter.Cache = function(options) {

        var data = {};
        var length = 0;

        function matchSubset(s, sub) {
            if (!options.matchCase)
                s = s.toLowerCase();
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

            // track all options for minChars = 0
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

                // keep track of minChars zero items
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
                } else
                    if (options.matchSubset) {
                    // re-motion: added check for min-length of q. If q contained only a single character, options.minChars is 0,
                    //            and the cache contains an entry for an empty string, this complete search result would be returned.
                    var minChars = Math.max (options.minChars, 1);
                    for (var i = q.length - 1; i >= minChars; i--) {
                        var c = data[q.substr(0, i)];
                        if (c) {
                            var csub = [];
                            $.each(c, function(i, x) {
                                if (matchSubset(x.result, q)) {
                                    csub[csub.length] = x;
                                }
                            });
                            return csub;
                        }
                    }
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
            element = $("<div/>")
            .hide()
            .addClass(options.resultsClass)
            .css("position", "absolute")
            .appendTo(document.body);

            if (element.width() == 0)
                element.width($(input).outerWidth() + $('#' + options.dropDownButtonId).outerWidth());

            //re-motion: block blur bind as long we scroll dropDown list 
            var revertInputStatusTimeout = null;
            function revertInputStatus() {
                if (config.mouseDownOnSelect) {
                    config.mouseDownOnSelect = false;
                    $(input).focus();
                }
            }
            element.scroll(function() {
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

            list = $("<ul/>").appendTo(element).mouseover(function(event) {
                if (target(event).nodeName && target(event).nodeName.toUpperCase() == 'LI') {
                    active = $("li", list).removeClass(CLASSES.ACTIVE).index(target(event));
                    $(target(event)).addClass(CLASSES.ACTIVE);
                }
            }).click(function(event) {
                $(target(event)).addClass(CLASSES.ACTIVE);
                select();
                // TODO provide option to avoid setting focus again after selection? useful for cleanup-on-focus
                input.focus();
                return false;
            }).mousedown(function() {
                config.mouseDownOnSelect = true;
            }).mouseup(function() {
                config.mouseDownOnSelect = false;
            });

            if (options.width > 0)
                element.css("width", options.width);

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
            setSelect (position, updateInput);
        };

        function setSelect(position, updateInput) {
            listItems.slice(active, active + 1).removeClass(CLASSES.ACTIVE);
            setPosition (position);
            var activeItem = listItems.slice(active, active + 1).addClass(CLASSES.ACTIVE);
            if (active >= 0) {
                var result = $.data(activeItem[0], "ac_data").result;
                if (updateInput)
                  $(input).val(result);

                // re-motion: do not select the text in the input element when moving the drop-down selection 
                //$.Autocompleter.Selection(input, 0, input.value.length);

                var resultsElement = $('.' + options.resultsClass);

                if (options.scroll) {
                    var offset = 0;
                    listItems.slice(0, active).each(function() {
                        offset += this.offsetHeight;
                    });

                    if ((offset + activeItem[0].offsetHeight - resultsElement.scrollTop()) > resultsElement[0].clientHeight) {
                        resultsElement.scrollTop(offset + activeItem[0].offsetHeight - resultsElement.innerHeight());
                    } else if (offset < resultsElement.scrollTop()) {
                        resultsElement.scrollTop(offset);
                    }
                }
            }
        }

        function calculatePosition(currentPosition, step) {
            currentPosition += step;
            if (currentPosition < 0) {
                currentPosition = listItems.size() - 1;
            } else if (currentPosition >= listItems.size()) {
                currentPosition = 0;
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

            var maxHeight = 0;
            var offset = $(input).offset();
            // re-motion: calculate best position where to open dropdown list
            var position = $.Autocompleter.calculateSpaceAround(input);
            var elementHeight = element.height();
            var listHeight = list.outerHeight();

            if (listHeight > options.scrollHeight)
                maxHeight = options.scrollHeight;

            if (position.spaceVertical == 'T' && position.bottom < listHeight) {
                topPosition = 'auto';
                bottomPosition = position.bottom + input.offsetHeight;
                if (options.scrollHeight > position.bottom && options.scrollHeight > position.top) {
                    maxHeight = position.top;
                }
            } else {

                bottomPosition = 'auto';
                topPosition = offset.top + input.offsetHeight;
                if (options.scrollHeight > position.bottom) {
                    maxHeight = position.bottom;
                }
            }
            // evaluate if list is smaller than available space
            if (maxHeight > listHeight && listHeight > 0) {
                maxHeight = listHeight;
            }

            // evaluate if maxHeight has never been set
            if (maxHeight == 0 && listHeight > 0) {
                maxHeight = listHeight;
            }

            // re-motion: need to resize list to specified width in css not in plugin config
            var elementWidth;
            if (options.width > 0) {
                elementWidth = options.width;
            } else if (element.outerWidth() > 0) {
                elementWidth = element.outerWidth();
            } else {
                elementWidth = $(input).outerWidth() + $('#' + options.dropDownButtonId).outerWidth();
            }

            var overflowY = listHeight > maxHeight ? 'scroll' : 'hidden';

            if (maxHeight > 0) {
                element.css({
                    width: elementWidth,
                    left: offset.left,
                    top: topPosition,
                    bottom: bottomPosition,
                    'overflow-y': overflowY,
                    height: maxHeight
                });
            } else {
                element.css({
                    width: elementWidth,
                    left: offset.left,
                    top: topPosition,
                    bottom: bottomPosition,
                    height: options.scrollHeight
                });
            }
        }

        function fillList() {
            list.empty();
            var max = data.length;
            for (var i = 0; i < max; i++) {
                if (!data[i])
                    continue;
                var formatted = options.formatItem(data[i].data, i + 1, max, data[i].value, term);
                if (formatted === false)
                    continue;
                var li = $("<li/>").html(options.highlight(formatted, term)).addClass(i % 2 == 0 ? "ac_even" : "ac_odd").appendTo(list)[0];
                $.data(li, "ac_data", data[i]);
            }
            listItems = list.find("li");
            if (options.selectFirst($(input).val(), term)) {
                listItems.slice(0, 1).addClass(CLASSES.ACTIVE);
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
                if (data[i].result.toLowerCase().indexOf(term.toLowerCase()) != -1) {
                    return i;
                }
            }

            for (var i = startPosition - 1; i >= 0; i--) {
                if (data[i].result.toLowerCase().indexOf(term.toLowerCase()) != -1) {
                    return i;
                }
            }

            return -1;
        }

        return {
            display: function(d, q) {
                init();
                data = d;
                term = q;
                fillList();
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
                element && element.hide();
                listItems && listItems.removeClass(CLASSES.ACTIVE);
                active = -1;
            },
            visible: function() {
                return element && element.is(":visible");
            },
            current: function() {
                return this.visible() && (listItems.filter("." + CLASSES.ACTIVE)[0] || options.selectFirst($(input).val(), null) && listItems[0]);
            },
            show: function() {

                //re-motion: scroll dropDown list to value from input
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
                }
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
            selected: function() {
                // re-motion: removing the CSS class does not provide any benefits, but prevents us from highlighting the currently selected value
                // on dropDownButton Click
                // Original: var selected = listItems && listItems.filter("." + CLASSES.ACTIVE).removeClass(CLASSES.ACTIVE);
                var selected = listItems && listItems.filter("." + CLASSES.ACTIVE);
                return selected && selected.length && $.data(selected[0], "ac_data");
            },
            emptyList: function() {
                list && list.empty();
            },
            // re-motion: returns the index of the item
            findItem: function (term) {
                return findItemPosition (term, Math.max (active, 0));
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

    $.Autocompleter.Selection = function(field, start, end) {
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
        field.focus();
    };

    $.Autocompleter.calculateSpaceAround = function(element) {
        // re-motion: make sure CSS values are allways numbers, IE can return 'auto'
        function number(num) {
            return parseInt(num) || 0;
        };

        var element = $(element);
        // re-motion: check position where to place the element
        var offsetParent = element.offsetParent();
        var pos = element.position();
        // re-motion: position and dimensions of the element
        var top = number(pos.top) + number(element.css('margin-top')); // IE can return 'auto' for margins
        var left = number(pos.left) + number(element.css('margin-left'));
        var width = element.outerWidth();
        var height = element.outerHeight();

        // re-motion: calculate space arrownd the element
        var scrollTop = number($(document).scrollTop());
        var scrollLeft = number($(document).scrollLeft());
        var documentWidth = number($(window).width());
        var documentHeight = number($(window).height());
        var windowRight = scrollLeft + documentWidth;
        var windowBottom = scrollTop + documentHeight;

        var space = new Object();
        space.top = element.offset().top - scrollTop;
        space.bottom = documentHeight - ((element.offset().top + height) - scrollTop);
        space.left = element.offset().left - scrollLeft;
        space.right = documentWidth - ((element.offset().left + width) - scrollLeft);

        (space.top > space.bottom) ? space.spaceVertical = 'T' : space.spaceVertical = 'B';
        (space.left > space.right) ? space.spaceHorizontal = 'L' : space.spaceHorizontal = 'R';
        space.space = space.spaceVertical + space.spaceHorizontal;

        return space;
    }

})(jQuery);