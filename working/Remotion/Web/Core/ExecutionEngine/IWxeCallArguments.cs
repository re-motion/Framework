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
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  ///   The <see cref="IWxeCallArguments"/> interface is used to to collect the parameters for executing a <see cref="WxeFunction"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  ///   The usage pattern is to pass the <see cref="IWxeCallArguments"/> and the <see cref="WxeFunction"/> to the <see cref="IWxePage.ExecuteFunction"/>
  ///   method defined by the <see cref="IWxePage"/>. This methood then invokes the <see cref="Dispatch"/> method, passing the <see cref="IWxeExecutor"/>
  ///   for the <see cref="IWxePage"/> and the <see cref="WxeFunction"/>. It is the <see cref="Dispatch"/> method's responsibility to correctly 
  ///   execute the <see cref="WxeFunction"/> with the help of the <see cref="IWxeExecutor"/> and using the state of this <see cref="IWxeCallArguments"/> object.
  /// </para>
  /// <para>
  ///   Use the <see cref="WxeCallArguments.Default"/> instance exposed on the <see cref="WxeCallArguments"/> type if your usecase is to simply
  ///   invoke a sub-function on your page. If you whish to execute the function with more advanced <see cref="WxeCallOptions"/>, instantiate an 
  ///   instance of the <see cref="WxeCallArguments"/> type. Finally, the <see cref="WxePermaUrlCallArguments"/> type is used if simply wish to
  ///   display a perma-URL in the browser's location-bar.
  /// </para>
  /// <para>
  ///   The <b>WxeGen</b> also allows for a simplified syntax by providing static <b>Call</b> methods on each page that will accept all required 
  ///   parameters (the <see cref="IWxePage"/>, the <see cref="IWxeCallArguments"/>, and additional arguments required by the specific function).
  /// </para>
  /// <note type="inotes">Implement the <see cref="Dispatch"/> method to control the execution of the <see cref="WxeFunction"/>.</note>
  /// </remarks>
  /// <example>
  /// <code escaped="true" lang="C#">
  /// internal class Sample
  /// {
  ///   void OnClick (object sender, EventArgs e)
  ///   {
  ///     try
  ///     {
  ///       IWxeCallArguments callArguments;
  ///       callArguments = WxeCallArguments.Default;                                    
  ///       callArguments = new WxePermaUrlCallArguments ();                                  
  ///       callArguments = new WxePermaUrlCallArguments (true);
  ///       callArguments = new WxeCallArguments ((Control) sender, new WxeCallOptionsExternal ("_blank"));
  ///       callArguments = new WxeCallArguments ((Control) sender, new WxeCallOptionsNoRepost ());
  ///       callArguments = new WxeCallArguments ((Control) sender, new WxeCallOptions ());
  ///       // MyPage.Call (this, callArguments, arg1, arg2, ...);
  ///      }
  ///     catch (WxeIgnorableException) { }
  ///   }
  /// }
  /// </code>
  /// </example>
  public interface IWxeCallArguments
  {
    void Dispatch ([NotNull]IWxeExecutor executor, [NotNull]WxeFunction function);
  }
}