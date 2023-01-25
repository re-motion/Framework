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

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Exception is thrown whenever the web service call in <see cref="BocAutoCompleteReferenceValueControlObject.GetSearchServiceResults"/> or
  /// <see cref="BocAutoCompleteReferenceValueControlObject.GetExactSearchServiceResult"/> fails.
  /// </summary>
  public sealed class WebServiceExecutionException : Exception
  {
    private readonly long _readyState;
    private readonly string _responseText;
    private readonly long _status;
    private readonly string _statusText;

    public WebServiceExecutionException (long readyState, [NotNull] string responseText, long status, [NotNull] string statusText)
        : base(
            string.Format(
                "The web service call failed with status '{0} - {1}'. The returned JSON object was: '{2}'.",
                status,
                statusText,
                responseText))
    {
      ArgumentUtility.CheckNotNull("responseText", responseText); // The response body may be empty.
      ArgumentUtility.CheckNotNull("statusText", statusText); // The HTTP statusText is empty for HTTPS responses

      _readyState = readyState;
      _responseText = responseText;
      _status = status;
      _statusText = statusText;
    }

    /// <summary>
    /// The ready-state of the Javascript XmlWebRequest object used to perform the reuqest.
    /// </summary>
    public long ReadyState
    {
      get { return _readyState; }
    }

    /// <summary>
    /// The response body.
    /// </summary>
    public string ResponseText
    {
      get { return _responseText; }
    }

    /// <summary>
    /// The HTTP status code of the response.
    /// </summary>
    public long Status
    {
      get { return _status; }
    }

    /// <summary>
    /// The HTTP status text of the response. Empty if the request is performed via HTTPS.
    /// </summary>
    public string StatusText
    {
      get { return _statusText; }
    }
  }
}
