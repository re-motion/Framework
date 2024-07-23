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
using System.Collections.Specialized;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates
{
  /// <summary>
  /// The <see cref="PreparingRedirectToSubFunctionStateParameters"/> group the parameters necessary for setting up the the redirect to a sub-function URL.
  /// </summary>
  public class PreparingRedirectToSubFunctionStateParameters : ExecutionStateParameters
  {
    private readonly WxePermaUrlOptions _permaUrlOptions;

    public PreparingRedirectToSubFunctionStateParameters (
        WxeFunction subFunction, NameValueCollection postBackCollection, WxePermaUrlOptions permaUrlOptions)
        : base(subFunction, postBackCollection)
    {
      ArgumentUtility.CheckNotNull("permaUrlOptions", permaUrlOptions);
      _permaUrlOptions = permaUrlOptions;
    }

    public WxePermaUrlOptions PermaUrlOptions
    {
      get { return _permaUrlOptions; }
    }
  }
}
