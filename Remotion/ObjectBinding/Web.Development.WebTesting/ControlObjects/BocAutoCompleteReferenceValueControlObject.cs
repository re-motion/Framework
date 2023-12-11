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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;
using Remotion.Web.Development.WebTesting.WebTestActions;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocAutoCompleteReferenceValue"/>.
  /// </summary>
  public class BocAutoCompleteReferenceValueControlObject
      : BocControlObject,
          IFillableControlObject,
          IDropDownMenuHost,
          IControlObjectWithFormElements,
          ISupportsValidationErrors,
          ISupportsValidationErrorsForReadOnly,
          IControlObjectWithText
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

    private class SelectAutoCompleteAction : WebTestAction
    {
      private const string c_updateResultScript = "arguments[0].__getUpdateResultHandler()({{DisplayName: '{0}', UniqueIdentifier: '{1}' }}, {{ Value: null }});";

      private string _actionName = "BocAutoCompleteReferenceValueControlObject_SelectAutoCompleteAction";

      [NotNull]
      private readonly SearchServiceResultItem _autoCompleteResultItem;

      [NotNull]
      private readonly FinishInputWithAction _finishInputWith;

      public SelectAutoCompleteAction (
          [NotNull] ControlObject control,
          [NotNull] ElementScope scope,
          [NotNull] SearchServiceResultItem autoCompleteResultItem,
          [NotNull] FinishInputWithAction finishInputWith)
          : base(control, scope)
      {
        ArgumentUtility.CheckNotNull("autoCompleteResultItem", autoCompleteResultItem);
        ArgumentUtility.CheckNotNull("finishInputWith", finishInputWith);

        _autoCompleteResultItem = autoCompleteResultItem;
        _finishInputWith = finishInputWith;
      }

      protected override string ActionName
      {
        get { return _actionName; }
      }

      protected override void ExecuteInteraction (ElementScope scope)
      {
        var displayName = _autoCompleteResultItem.DisplayName;

        scope.FillInWithFixed(displayName, FinishInput.Promptly);

        var executor = JavaScriptExecutor.GetJavaScriptExecutor(scope);

        JavaScriptExecutor.ExecuteVoidStatement(
            executor,
            string.Format(c_updateResultScript, displayName, _autoCompleteResultItem.UniqueIdentifier),
            scope.Native);

        _finishInputWith(scope);
      }
    }

    public BocAutoCompleteReferenceValueControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <inheritdoc cref="IFillableControlObject" />
    public string GetText ()
    {
      return Scope.FindChild("TextValue").Value; // do not trim
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (string text, IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNull("text", text);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver);

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      return FillWith(text, FinishInput.WithTab, actionOptions);
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (string text, FinishInputWithAction finishInputWith, IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNull("text", text);
      ArgumentUtility.CheckNotNull("finishInputWith", finishInputWith);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver);

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      var actualActionOptions = MergeWithDefaultActionOptions(Scope, actionOptions);
      ExecuteAction(new FillWithAction(this, Scope.FindChild("TextValue"), text, finishInputWith), actualActionOptions);
      return UnspecifiedPage();
    }

    /// <inheritdoc/>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = Scope.FindChild("Boc_OptionsMenu");
      return new DropDownMenuControlObject(Context.CloneForControl(dropDownMenuScope));
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
      ArgumentUtility.CheckNotNullOrEmpty("searchText", searchText);

      var inputScopeID = GetInputScopeID();

      var searchServiceRequestScript = CreateAutoCompleteSearchServiceRequest(inputScopeID, searchText, completionSetCount);
      var response = (IReadOnlyDictionary<string, object>)Context.Browser.Driver.ExecuteScript(searchServiceRequestScript, Scope);
      return ParseSearchServiceResponse(
          response,
          static data =>
          {
            Assertion.IsTrue(
                data.Count == 1,
                "The returned search service response must have a single item as its data, here {0} items are contained.",
                data.Count);
            return ParseSearchServiceResultVariants((IReadOnlyDictionary<string, object>)data.Single());
          });
    }

    /// <summary>
    /// Invokes the associated exact search service of the represented
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocAutoCompleteReferenceValue"/> and returns its result.
    /// </summary>
    /// <param name="searchText">Text to search for.</param>
    /// <returns>The exact search result as <see cref="SearchServiceResultItem"/> or null if no result has been found.</returns>
    public SearchServiceResultItem? GetExactSearchServiceResult ([NotNull] string searchText)
    {
      ArgumentUtility.CheckNotNullOrEmpty("searchText", searchText);

      var inputScopeId = GetInputScopeID();

      var searchServiceRequestScript = CreateAutoCompleteExactSearchServiceRequest(inputScopeId, searchText);
      var response = (IReadOnlyDictionary<string, object>)Context.Browser.Driver.ExecuteScript(searchServiceRequestScript, Scope);
      return ParseSearchServiceResponse(
          response,
          static data =>
          {
            Assertion.IsTrue(
                data.Count == 1,
                "The returned search service response must have a single item as its data, here {0} items are contained.",
                data.Count);
            return ParseSearchServiceResultItem((IReadOnlyDictionary<string, object>)data.Single());
          });
    }

    /// <summary>
    /// Selects the first auto-completion match for the given <paramref name="filter" />.
    /// </summary>
    /// <param name="filter">The filter used to constrain the possible options.</param>
    /// <param name="actionOptions">See <see cref="IWebTestActionOptions"/> for more information.</param>
    /// <exception cref="WebTestException"><paramref name="filter"/> cannot be matched with an auto-completion result.</exception>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    public UnspecifiedPageObject SelectFirstMatch ([NotNull] string filter, [CanBeNull] IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNull("filter", filter);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver);

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      return SelectFirstMatch(filter, FinishInput.WithTab, actionOptions);
    }

    /// <summary>
    /// Selects the first auto-completion match for the given <paramref name="filter" />.
    /// Afterwards, it executes the given <paramref name="finishInputWith"/> action.
    /// </summary>
    /// <param name="filter">The filter used to constrain the possible options.</param>
    /// <param name="finishInputWith">What to do after the text has been filled in (see <see cref="FinishInput"/> for supported default options).</param>
    /// <param name="actionOptions">See <see cref="IWebTestActionOptions"/> for more information.</param>
    /// <exception cref="WebTestException"><paramref name="filter"/> cannot be matched with an auto-completion result.</exception>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    public UnspecifiedPageObject SelectFirstMatch (
        [NotNull] string filter,
        [NotNull] FinishInputWithAction finishInputWith,
        [CanBeNull] IWebTestActionOptions? actionOptions = null)
    {
      ArgumentUtility.CheckNotNull("filter", filter);
      ArgumentUtility.CheckNotNull("finishInputWith", finishInputWith);

      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver);

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      var firstAutoCompleteResult = GetFirstAutoCompleteResult(filter);
      var textField = Scope.FindChild("TextValue");

      var actualActionOptions = MergeWithDefaultActionOptions(Scope, actionOptions);
      ExecuteAction(new SelectAutoCompleteAction(this, textField, firstAutoCompleteResult, finishInputWith), actualActionOptions);

      return UnspecifiedPage();
    }

    public IReadOnlyList<string> GetValidationErrors ()
    {
      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      return GetValidationErrors(Scope.FindChild("TextValue"));
    }

    public IReadOnlyList<string> GetValidationErrorsForReadOnly ()
    {
      return GetValidationErrorsForReadOnly(GetLabeledElementScope());
    }

    protected override ElementScope GetLabeledElementScope ()
    {
      return Scope.FindChild("TextValue");
    }

    private SearchServiceResultItem GetFirstAutoCompleteResult ([NotNull] string filter)
    {
      ArgumentUtility.CheckNotNullOrEmpty("filter", filter);

      var results = GetSearchServiceResults(filter, 2);

      if (results.Count == 0)
        throw AssertionExceptionUtility.CreateExpectationException(Driver, "No matches were found for the specified filter: '{0}'.", filter);

      return results.First();
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
      ArgumentUtility.CheckNotNullOrEmpty("autoCompleteTextValueInputFieldId", autoCompleteTextValueInputFieldId);
      ArgumentUtility.CheckNotNullOrEmpty("searchText", searchText);

      return CreateAutoCompleteSearchServiceRequestScript(autoCompleteTextValueInputFieldId, searchText, "serviceMethodSearchExact", null);
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
      ArgumentUtility.CheckNotNullOrEmpty("autoCompleteTextValueInputFieldId", autoCompleteTextValueInputFieldId);
      ArgumentUtility.CheckNotNullOrEmpty("searchText", searchText);

      return CreateAutoCompleteSearchServiceRequestScript(autoCompleteTextValueInputFieldId, searchText, "serviceMethodSearch", completionSetCount);
    }

    private string CreateAutoCompleteSearchServiceRequestScript (
        [NotNull] string autoCompleteTextValueInputFieldId,
        [NotNull] string searchText,
        [NotNull] string searchMethod,
        int? completionSetCount)
    {
      ArgumentUtility.CheckNotNullOrEmpty("autoCompleteTextValueInputFieldId", autoCompleteTextValueInputFieldId);
      ArgumentUtility.CheckNotNullOrEmpty("searchText", searchText);
      ArgumentUtility.CheckNotNullOrEmpty("searchMethod", searchMethod);

      var setCompletionSetCountScriptPart = completionSetCount.HasValue
          ? string.Format("data['completionSetCount'] = {0};", completionSetCount.Value)
          : string.Empty;

      return $@"
CallWebService = function() {{
  var input = document.getElementById('{autoCompleteTextValueInputFieldId}');
  var options = input.getAutoCompleteSearchParameters('{searchText}');
  
  var data = options.params;
  data['searchString'] = options.searchString;
  {setCompletionSetCountScriptPart}

  data = Sys.Serialization.JavaScriptSerializer.serialize(data);

  var returnValue = null;
  // We use XMLHttpRequest here instead of Sys.Net.WebServiceProxy.invoke because we want sync execution of the web request
  var request = new XMLHttpRequest();
  request.open('POST', options.serviceUrl + '/' + options.{searchMethod}, false);
  request.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
  var failedHandler = function() {{ 
    returnValue = {{ 
      {AutoCompleteSearchService.State}:'{AutoCompleteSearchService.Error}',
      {AutoCompleteSearchService.Data}: {{
        readyState: request.readyState,
        responseText: request.responseText,
        status: request.status,
        statusText: request.statusText
      }}
    }};
  }}
  request.onload = function() {{
    if (request.status >= 200 && request.status <= 299)
      returnValue = {{ {AutoCompleteSearchService.State}:'{AutoCompleteSearchService.Success}', {AutoCompleteSearchService.Data}: JSON.parse(request.response).d }};
    else
      failedHandler();
  }};
  request.onerror = failedHandler;
  request.send(data);

  // Normalize communication between JS and C# API: always return an array if succeeded.
  if(returnValue.{AutoCompleteSearchService.State} === '{AutoCompleteSearchService.Success}' && !Array.isArray(returnValue.{AutoCompleteSearchService.Data}))
    returnValue.data = [ returnValue.{AutoCompleteSearchService.Data} ];

  return returnValue;
}};
return CallWebService();";
    }

    private static T ParseSearchServiceResponse<T> (
        [NotNull] IReadOnlyDictionary<string, object> response,
        [NotNull] Func<IReadOnlyCollection<object>, T> successParser)
    {
      ArgumentUtility.CheckNotNull("response", response);

      var state = (string)response[AutoCompleteSearchService.State];
      var data = response[AutoCompleteSearchService.Data];
      switch (state)
      {
        case AutoCompleteSearchService.Success:
          return successParser((IReadOnlyCollection<object>)data);

        case AutoCompleteSearchService.Error:
          var errorData = (IDictionary<string, object>)data;
          throw new WebServiceExecutionException(
              (long)errorData["readyState"],
              (string)errorData["responseText"],
              (long)errorData["status"],
              (string)errorData["statusText"]);

        default:
          throw new NotSupportedException(string.Format("The script returned the unknown state '{0}'.", state));
      }
    }

    private static IReadOnlyList<SearchServiceResultItem> ParseSearchServiceResultVariants (IReadOnlyDictionary<string, object> data) =>
        (data["Type"] as string) switch
        {
            "ValueList" => ParseSearchServiceResultItems(((IReadOnlyCollection<object?>)data["Values"]).Cast<IReadOnlyDictionary<string, object>>()),
            var unknown => throw new InvalidOperationException($"The result variant '{unknown}' is not supported."),
        };

    private static IReadOnlyList<SearchServiceResultItem> ParseSearchServiceResultItems (IEnumerable<IReadOnlyDictionary<string, object>?> data) =>
        data.Select(ParseSearchServiceResultItem).OfType<SearchServiceResultItem>().ToArray();

    [return: NotNullIfNotNull("data")]
    private static SearchServiceResultItem? ParseSearchServiceResultItem (IReadOnlyDictionary<string, object>? data) =>
        data switch
        {
            { } => new((string)data["UniqueIdentifier"], (string)data["DisplayName"], (string)data["IconUrl"]),
            null => null,
        };

    /// <summary>
    /// See <see cref="IControlObjectWithFormElements.GetFormElementNames"/>. Returns the input[type=text] (text value) as first element, the
    /// input[type=hidden] (key value) as second element.
    /// </summary>
    ICollection<string> IControlObjectWithFormElements.GetFormElementNames ()
    {
      return new[] { GetInputScopeID(), string.Format("{0}_KeyValue", GetHtmlID()) };
    }

    private string GetInputScopeID ()
    {
      return GetHtmlID() + "_TextValue";
    }
  }
}
