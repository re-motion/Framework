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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates
{
  /// <summary>
  /// The <see cref="PreProcessingSubFunctionStateParameters"/> group the parameters necessary for setting up the execution of a sub-function.
  /// </summary>
  public class PreProcessingSubFunctionStateParameters : IExecutionStateParameters
  {
    private readonly IWxePage _page;
    private readonly WxeFunction _subFunction;
    private readonly WxePermaUrlOptions _permaUrlOptions;

    public PreProcessingSubFunctionStateParameters (IWxePage page, WxeFunction subFunction, WxePermaUrlOptions permaUrlOptions)
    {
      ArgumentUtility.CheckNotNull("page", page);
      ArgumentUtility.CheckNotNull("subFunction", subFunction);
      ArgumentUtility.CheckNotNull("permaUrlOptions", permaUrlOptions);

      _page = page;
      _subFunction = subFunction;
      _permaUrlOptions = permaUrlOptions;
    }

    public IWxePage Page
    {
      get { return _page; }
    }

    public WxeFunction SubFunction
    {
      get { return _subFunction; }
    }

    public WxePermaUrlOptions PermaUrlOptions
    {
      get { return _permaUrlOptions; }
    }
  }
}
