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
using Remotion.Web.Resources;

namespace Remotion.Web
{
  /// <summary>
  /// Defines methods for creating objects that implement <see cref="IResourceUrl"/>. 
  /// Use <see cref="T:Remotion.Development.Web.UnitTesting.Resources.FakeResourceUrlFactory"/> for unit testing.
  /// </summary>
  /// <seealso cref="ResourceUrlFactory"/>
  /// <seealso cref="T:Remotion.Development.Web.UnitTesting.Resources.FakeResourceUrlFactory"/>
 public interface IResourceUrlFactory
  {
    /// <summary>
    /// Creates an <see cref="IResourceUrl"/> object that is independent of the selected <see cref="ResourceTheme"/>.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The default implementation (<see cref="ResourceUrl"/>) uses the URL 
    ///     &lt;resource root&gt;/&lt;definingType.Assembly&gt;/&lt;ResourceType&gt;/relativeUrl.
    ///   </para><para>
    ///     The <b>resource root</b> is loaded from the application configuration, <see cref="ResourceRoot"/>, and
    ///     defaults to <c>/&lt;AppDir&gt;/res</c>, e.g. <c>/WebApplication/res/Remotion.Web/Html/Utilities.js</c>.
    ///   </para><para>
    ///     During design time, the <b>resource root</b> is mapped to the environment variable
    ///     <c>REMOTIONRESOURCES</c>, or if the variable does not exist, <c>C:\Remotion.Resources</c>.
    ///   </para>
    /// </remarks>
    /// <seealso cref="IResourcePathBuilder"/>
    IResourceUrl CreateResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl);

    /// <summary>
    /// Creates an <see cref="IResourceUrl"/> object that depends on the selected <see cref="ResourceTheme"/>.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     It is the reponsibilty of the implementation to provide the <see cref="ResourceTheme"/>.
    ///   </para><para>
    ///     The default implementation (<see cref="ThemedResourceUrl"/>) uses the URL 
    ///     &lt;resource root&gt;/&lt;definingType.Assembly&gt;/&lt;ResourceTheme&gt;/&lt;ResourceType&gt;/relativeUrl.
    ///   </para><para>
    ///     The <b>resource root</b> is loaded from the application configuration, <see cref="ResourceRoot"/>, and
    ///     defaults to <c>/&lt;AppDir&gt;/res</c>, e.g. <c>/WebApplication/res/Remotion.Web/NovaBlue/Image/Help.gif</c>.
    ///   </para><para>
    ///     During design time, the <b>resource root</b> is mapped to the environment variable
    ///     <c>REMOTIONRESOURCES</c>, or if the variable does not exist, <c>C:\Remotion.Resources</c>.
    ///   </para>
    /// </remarks>
    /// <seealso cref="IResourcePathBuilder"/>
    IResourceUrl CreateThemedResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl);
  }
}
