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
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocAutoCompleteReferenceValue"/>.
  /// </summary>
  public class BocAutoCompleteReferenceValueControlObject
      : BocControlObject, IFillableControlObject, ICommandHost, IDropDownMenuHost, IControlObjectWithFormElements
  {
    /// <summary>
    /// Various constants shared by the script and the script-user client-code.
    /// </summary>
    private static class AutoCompleteSearchService
    {
      public const string State = "state";
      public const string Data = "data";
      public const string Success = "success";
      public const string Error = "error";
    }

    public BocAutoCompleteReferenceValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      if (IsReadOnly())
        return Scope.FindChild ("Value").Text; // do not trim

      return Scope.FindChild ("TextValue").Value; // do not trim
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (string text, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);

      return FillWith (text, FinishInput.WithTab, actionOptions);
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (string text, FinishInputWithAction finishInputWith, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);
      ArgumentUtility.CheckNotNull ("finishInputWith", finishInputWith);

      var actualActionOptions = MergeWithDefaultActionOptions (Scope, actionOptions);
      new FillWithAction (this, Scope.FindChild ("TextValue"), text, finishInputWith).Execute (actualActionOptions);
      return UnspecifiedPage();
    }

    /// <inheritdoc/>
    public CommandControlObject GetCommand ()
    {
      var commandScope = Scope.FindChild ("Command");
      return new CommandControlObject (Context.CloneForControl (commandScope));
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject ExecuteCommand (IWebTestActionOptions acitonOptions = null)
    {
      return GetCommand().Click (acitonOptions);
    }

    /// <inheritdoc/>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = Scope.FindChild ("Boc_OptionsMenu");
      return new DropDownMenuControlObject (Context.CloneForControl (dropDownMenuScope));
    }

    /// <summary>
    /// Invokes the associated search service of the represented <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocAutoCompleteReferenceValue"/>
    /// and returns its results.
    /// </summary>
    /// <param name="searchText">Text to search for.</param>
    /// <param name="completionSetCount">Auto completion set count.</param>
    /// <returns>The completion set as list of <see cref="SearchServiceResultItem"/> or an empty list if the completion set has been empty.</returns>
    public IReadOnlyList<SearchServiceResultItem> GetSearchServiceResults ([NotNull] string searchText, int completionSetCount)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("searchText", searchText);

      var inputScopeID = GetInputScopeID();

      var searchServiceRequestScript = CreateAutoCompleteSearchServiceRequest (inputScopeID, searchText, completionSetCount);
      var response = (IReadOnlyDictionary<string, object>) Context.Browser.Driver.ExecuteScript (searchServiceRequestScript, Scope);
      return ParseSearchServiceResponse (response);
    }

    /// <summary>
    /// Invokes the associated exact search service of the represented
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocAutoCompleteReferenceValue"/> and returns its result.
    /// </summary>
    /// <param name="searchText">Text to search for.</param>
    /// <returns>The exact search result as <see cref="SearchServiceResultItem"/> or null if no result has been found.</returns>
    public SearchServiceResultItem GetExactSearchServiceResult ([NotNull] string searchText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("searchText", searchText);

      var inputScopeId = GetInputScopeID();

      var searchServiceRequestScript = CreateAutoCompleteExactSearchServiceRequest (inputScopeId, searchText);
      var response = (IReadOnlyDictionary<string, object>) Context.Browser.Driver.ExecuteScript (searchServiceRequestScript, Scope);
      return ParseSearchServiceResponse (response).SingleOrDefault();
    }


    /// <summary>
    /// JavaScript for calling the auto completion SearchExact web service method.
    /// </summary>
    /// <param name="autoCompleteTextValueInputFieldId">The auto completes input field ID.</param>
    /// <param name="searchText">The search text.</param>
    private string CreateAutoCompleteExactSearchServiceRequest (
        [NotNull] string autoCompleteTextValueInputFieldId,
        [NotNull] string searchText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("autoCompleteTextValueInputFieldId", autoCompleteTextValueInputFieldId);
      ArgumentUtility.CheckNotNullOrEmpty ("searchText", searchText);

      return CreateAutoCompleteSearchServiceRequestScript (autoCompleteTextValueInputFieldId, searchText, "serviceMethodSearchExact", null);
    }

    /// <summary>
    /// JavaScript for calling the auto completion Search web service method.
    /// </summary>
    /// <param name="autoCompleteTextValueInputFieldId">The auto completes input field ID.</param>
    /// <param name="searchText">The search text.</param>
    /// <param name="completionSetCount">The maximum size of the returned auto completion set.</param>
    private string CreateAutoCompleteSearchServiceRequest (
        [NotNull] string autoCompleteTextValueInputFieldId,
        [NotNull] string searchText,
        int completionSetCount)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("autoCompleteTextValueInputFieldId", autoCompleteTextValueInputFieldId);
      ArgumentUtility.CheckNotNullOrEmpty ("searchText", searchText);

      return CreateAutoCompleteSearchServiceRequestScript (autoCompleteTextValueInputFieldId, searchText, "serviceMethodSearch", completionSetCount);
    }

    private string CreateAutoCompleteSearchServiceRequestScript (
        [NotNull] string autoCompleteTextValueInputFieldId,
        [NotNull] string searchText,
        [NotNull] string searchMethod,
        int? completionSetCount)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("autoCompleteTextValueInputFieldId", autoCompleteTextValueInputFieldId);
      ArgumentUtility.CheckNotNullOrEmpty ("searchText", searchText);
      ArgumentUtility.CheckNotNullOrEmpty ("searchMethod", searchMethod);

      var setCompletionSetCountScriptPart = completionSetCount.HasValue
          ? string.Format ("data['completionSetCount'] = {0};", completionSetCount.Value)
          : string.Empty;

      return string.Format (
          @"
CallWebService = function() {{
  var input = $('#{0}');
  var options = input.getAutoCompleteSearchParameters('{1}');
  
  var data = options.params;
  data['searchString'] = options.searchString;
  {2}

  data = Sys.Serialization.JavaScriptSerializer.serialize(data);

  var returnValue = null;
  var request = {{
    async:false,
    type:'POST',
    contentType:'application/json; charset=utf-8',
    url:options.serviceUrl + '/' + options.{3},
    data:data,
    dataType:'json',
    success:function(result, context, methodName){{ returnValue = {{ {4}:'{6}', {5}:result.d }}; }},
    error:function(error, context, methodName){{ returnValue = {{ {4}:'{7}', {5}:error }}; }}
  }};

  $.ajax(request);

  // Normalize communication between JS and C# API: always return an array if succeeded.
  if(returnValue.state === 'success' && !Array.isArray(returnValue.data))
    returnValue.data = [ returnValue.data ];

  return returnValue;
}};
return CallWebService();",
          autoCompleteTextValueInputFieldId,
          searchText,
          setCompletionSetCountScriptPart,
          searchMethod,
          AutoCompleteSearchService.State,
          AutoCompleteSearchService.Data,
          AutoCompleteSearchService.Success,
          AutoCompleteSearchService.Error);
    }

    private IReadOnlyList<SearchServiceResultItem> ParseSearchServiceResponse ([NotNull] IReadOnlyDictionary<string, object> response)
    {
      ArgumentUtility.CheckNotNull ("response", response);

      var state = (string) response[AutoCompleteSearchService.State];
      var data = response[AutoCompleteSearchService.Data];
      switch (state)
      {
        case AutoCompleteSearchService.Success:
          var successData = (IReadOnlyCollection<object>) data;
          return successData.Cast<IDictionary<string, object>>()
              .Where (d => d != null) // empty JSON object (= no result) is converted to null
              .Select (d => new SearchServiceResultItem ((string) d["UniqueIdentifier"], (string) d["DisplayName"], (string) d["IconUrl"]))
              .ToList();

        case AutoCompleteSearchService.Error:
          var errorData = (IDictionary<string, object>) data;
          throw new WebServiceExceutionException (
              (long) errorData["readyState"],
              (string) errorData["responseText"],
              (long) errorData["status"],
              (string) errorData["statusText"]);

        default:
          throw new NotSupportedException (string.Format ("The script returned the unknown state '{0}'.", state));
      }
    }

    /// <summary>
    /// See <see cref="IControlObjectWithFormElements.GetFormElementNames"/>. Returns the input[type=text] (text value) as first element, the
    /// input[type=hidden] (key value) as second element.
    /// </summary>
    ICollection<string> IControlObjectWithFormElements.GetFormElementNames ()
    {
      var htmlID = GetHtmlID();
      return new[] { string.Format ("{0}_TextValue", htmlID), string.Format ("{0}_KeyValue", htmlID) };
    }

    private string GetInputScopeID ()
    {
      return GetHtmlID() + "_TextValue";
    }
  }
}