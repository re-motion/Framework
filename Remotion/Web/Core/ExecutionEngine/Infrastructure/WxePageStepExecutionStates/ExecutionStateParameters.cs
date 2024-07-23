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
  /// The <see cref="ExecutionStateParameters"/> type groups a basic set of parameters passed between the individual implementations of the
  /// <see cref="IExecutionState"/> interface during the various state transistions. 
  /// </summary>
  public class ExecutionStateParameters : IExecutionStateParameters
  {
    private readonly WxeFunction _subFunction;
    private readonly NameValueCollection _postBackCollection;

    public ExecutionStateParameters (WxeFunction subFunction, NameValueCollection postBackCollection)
    {
      ArgumentUtility.CheckNotNull("subFunction", subFunction);
      ArgumentUtility.CheckNotNull("postBackCollection", postBackCollection);

      _subFunction = subFunction;
      _postBackCollection = postBackCollection;
    }

    public WxeFunction SubFunction
    {
      get { return _subFunction; }
    }

    public NameValueCollection PostBackCollection
    {
      get { return _postBackCollection; }
    }
  }
}
