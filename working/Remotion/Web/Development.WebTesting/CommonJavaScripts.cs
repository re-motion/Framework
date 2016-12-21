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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Constants for common JavaScript scripts.
  /// </summary>
  public static class CommonJavaScripts
  {
    /// <summary>
    /// Closes the current window.
    /// </summary>
    // ReSharper disable once ConvertToConstant.Global
    public static readonly string SelfClose = "self.close();";

    /// <summary>
    /// Returns the HTML element's computed background color. You must provide the ElementScope as the first parameter to the JavaScript call.
    /// </summary>
    public static readonly string GetComputedBackgroundColor = CreateGetComputedCssValueScript ("background-color");

    /// <summary>
    /// Returns the HTML element's computed text color. You must provide the ElementScope as the first parameter to the JavaScript call.
    /// </summary>
    public static readonly string GetComputedTextColor = CreateGetComputedCssValueScript ("color");

    /// <summary>
    /// JavaScript for calling the auto completion SearchExact web service method.
    /// </summary>
    /// <param name="autoCompleteTextValueInputFieldId">The auto completes input field ID.</param>
    /// <param name="searchText">The search text.</param>
    public static string CreateAutoCompleteExactSearchServiceRequest (
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
    public static string CreateAutoCompleteSearchServiceRequest (
        [NotNull] string autoCompleteTextValueInputFieldId,
        [NotNull] string searchText,
        int completionSetCount)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("autoCompleteTextValueInputFieldId", autoCompleteTextValueInputFieldId);
      ArgumentUtility.CheckNotNullOrEmpty ("searchText", searchText);

      return CreateAutoCompleteSearchServiceRequestScript (autoCompleteTextValueInputFieldId, searchText, "serviceMethodSearch", completionSetCount);
    }

    /// <summary>
    /// Various constants shared by the script and the script-user client-code.
    /// </summary>
    public static class AutoCompleteSearchService
    {
      public const string State = "state";
      public const string Data = "data";
      public const string Success = "success";
      public const string Error = "error";
    }

    private static string CreateGetComputedCssValueScript ([NotNull] string cssProperty)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("cssProperty", cssProperty);

      return string.Format ("return $(arguments[0]).css('{0}');", cssProperty);
    }

    private static string CreateAutoCompleteSearchServiceRequestScript (
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
  }
}