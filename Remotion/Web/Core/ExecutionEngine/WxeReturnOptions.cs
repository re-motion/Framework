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
using System.Diagnostics.CodeAnalysis;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// The <see cref="WxeReturnOptions"/> determine whether a <see cref="WxeFunction"/> executed with the 
  /// <see cref="IWxeExecutor"/>.<see cref="IWxeExecutor.ExecuteFunctionExternalByRedirect"/> method will return to its calling 
  /// <see cref="WxeFunction"/>. Use the <see cref="Null"/> value if you do not wish to return to the caller.
  /// </summary>
  public sealed class WxeReturnOptions : INullObject
  {
    public static readonly WxeReturnOptions Null = new WxeReturnOptions(false, null);

    private readonly bool _isReturning;
    private readonly NameValueCollection? _callerUrlParameters;

    public WxeReturnOptions ()
      : this(true, new NameValueCollection())
    {
    }

    public WxeReturnOptions (NameValueCollection callerUrlParameters)
      : this(true, ArgumentUtility.CheckNotNull("callerUrlParameters", callerUrlParameters))
    {
    }

    private WxeReturnOptions (bool isReturning, NameValueCollection? callerUrlParameters)
    {
      _isReturning = isReturning;
      _callerUrlParameters = callerUrlParameters;
    }

    [MemberNotNullWhen(true, nameof(CallerUrlParameters))]
    public bool IsReturning
    {
      get { return _isReturning; }
    }

    public NameValueCollection? CallerUrlParameters
    {
      get { return _callerUrlParameters; }
    }

    bool INullObject.IsNull
    {
      get { return !_isReturning; }
    }
  }
}
