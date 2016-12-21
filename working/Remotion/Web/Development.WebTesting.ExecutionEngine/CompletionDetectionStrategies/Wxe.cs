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

namespace Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies
{
  /// <summary>
  /// WXE-based <see cref="ICompletionDetectionStrategy"/> implementations which are directly supported by the framework.
  /// </summary>
  public static class Wxe
  {
    public static readonly WxePostBackCompletionDetectionStrategy PostBackCompleted = new WxePostBackCompletionDetectionStrategy (1);

    public static readonly Func<PageObject, WxePostBackInCompletionDetectionStrategy> PostBackCompletedIn =
        po => PostBackCompletedInContext (po.Context);

    public static readonly Func<PageObjectContext, WxePostBackInCompletionDetectionStrategy> PostBackCompletedInContext =
        ctx => new WxePostBackInCompletionDetectionStrategy (ctx, 1);

    public static readonly Func<PageObject, WxePostBackInCompletionDetectionStrategy> PostBackCompletedInParent =
        po => PostBackCompletedInContext (po.Context.ParentContext);

    public static readonly WxeResetCompletionDetectionStrategy Reset = new WxeResetCompletionDetectionStrategy();

    public static readonly Func<PageObject, WxeResetInCompletionDetectionStrategy> ResetIn = po => ResetInContext (po.Context);

    public static readonly Func<PageObjectContext, WxeResetInCompletionDetectionStrategy> ResetInContext =
        ctx => new WxeResetInCompletionDetectionStrategy (ctx);

    public static readonly Func<PageObject, WxeResetInCompletionDetectionStrategy> ResetInParent =
        po => ResetInContext (po.Context.ParentContext);
  }
}