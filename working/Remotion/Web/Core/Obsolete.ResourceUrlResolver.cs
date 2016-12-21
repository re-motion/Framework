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
using System.Reflection;
using System.Web;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Resources;
using Remotion.Web.UI.Controls;

namespace Remotion.Web
{
  /// <summary> Utility methods for URL resolving. </summary>
  [Obsolete ("Use IResourceUrlFactory instead. (Version 1.13.198)")]
  public static class ResourceUrlResolver
  {
    /// <summary>
    ///   Returns the physical URL of a resource item.
    /// </summary>
    /// <param name="control"> 
    ///   The current <see cref="Control"/>. Currently, this parameter is only used to detect design time.
    /// </param>
    /// <param name="context"> The current <see cref="HttpContext"/>. </param>
    /// <param name="definingType"> The type that this resource item is associated with. </param>
    /// <param name="resourceType"> The resource type (image, static html, etc.) </param>
    /// <param name="relativeUrl"> The relative URL of the item. </param>
    [Obsolete ("Use IResourceUrlFactory.CreateResourceUrl(...) instead. (Version 1.13.197)", true)]
    public static string GetResourceUrl (IControl control, HttpContextBase context, Type definingType, ResourceType resourceType, string relativeUrl)
    {
      throw new NotImplementedException("Use IResourceUrlFactory.CreateResourceUrl(...) instead. (Version 1.13.197)");
    }

    /// <summary>
    ///   Returns the physical URL of a resource item.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Uses the URL &lt;resource root&gt;/&lt;definingType.Assembly&gt;/&lt;ResourceType&gt;/relativeUrl.
    ///   </para><para>
    ///     The <b>resource root</b> is loaded from the application configuration,
    ///     <see cref="Remotion.Web.Configuration.WebConfiguration.Resources">WebConfiguration.Resources</see>, and 
    ///     defaults to <c>/&lt;AppDir&gt;/res</c>, e.g. <c>/WebApplication/res/Remotion.Web/Image/Help.gif</c>.
    ///   </para><para>
    ///     During design time, the <b>resource root</b> is mapped to the environment variable
    ///     <c>REMOTIONRESOURCES</c>, or if the variable does not exist, <c>C:\Remotion.Resources</c>.
    ///   </para>
    /// </remarks>
    /// <param name="control"> 
    ///   The current <see cref="Control"/>. This parameter is only used to detect design time.
    /// </param>
    /// <param name="definingType"> 
    ///   The type that this resource item is associated with. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="resourceType"> The resource type (image, static html, etc.) Must not be <see langword="null"/>. </param>
    /// <param name="relativeUrl"> The resource file name. Must not be <see langword="null"/> or empty.</param>
    [Obsolete ("Use IResourceUrlFactory.CreateResourceUrl(...) instead. (Version 1.13.197)")]
    public static string GetResourceUrl ([CanBeNull] IControl control, Type definingType, ResourceType resourceType, string relativeUrl)
    {
      ArgumentUtility.CheckNotNull ("definingType", definingType);
      ArgumentUtility.CheckNotNull ("resourceType", resourceType);
      ArgumentUtility.CheckNotNull ("relativeUrl", relativeUrl);

      var factory = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>();
      return factory.CreateResourceUrl (definingType, resourceType, relativeUrl).GetUrl();
    }

    /// <summary>
    ///   Returns the physical URL of a resource item.
    /// </summary>
    /// <seealso cref="IResourceUrlResolver"/>.
    /// <param name="control"> 
    ///   The current <see cref="Control"/>. This parameter is only used to detect design time.
    /// </param>
    /// <param name="definingType"> 
    ///   The type that this resource item is associated with. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="resourceType"> The resource type (image, static html, etc.) Must not be <see langword="null"/>. </param>
    /// <param name="theme">The <see cref="ResourceTheme"/> to which the resource belongs.</param>
    /// <param name="relativeUrl"> The resource file name. Must not be <see langword="null"/> or empty.</param>
    [Obsolete ("Use IResourceUrlFactory.CreateThemedResourceUrl(...) instead. (Version 1.13.197)", true)]
    public static string GetResourceUrl (IControl control, Type definingType, ResourceType resourceType, ResourceTheme theme, string relativeUrl)
    {
      throw new NotImplementedException("Use IResourceUrlFactory.CreateThemedResourceUrl(...) instead. (Version 1.13.197)");
    }

    /// <summary> Returns the root folder for all resources belonging to the <paramref name="assembly"/>. </summary>
    /// <param name="isDesignMode"> <see langword="true"/> if the application is in design mode. </param>
    /// <param name="assembly">The <paramref name="assembly"/> for which a ressource is being resolved.</param>
    /// <returns> 
    ///   The folder where the resources are expected to be for the <paramref name="assembly"/>. 
    ///   Always ends on a slash.
    /// </returns>
    [Obsolete ("Use IResourceUrlFactory.CreateResourceUrl(...) instead. (Version 1.13.197)")]
    public static string GetAssemblyRoot (bool isDesignMode, Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      var builder = SafeServiceLocator.Current.GetInstance<IResourcePathBuilder>();
      return builder.BuildAbsolutePath (assembly);
    }

    /// <summary> Returns the root folder for all resources. </summary>
    /// <param name="isDesignMode"> <see langword="true"/> if the application is in design mode. </param>
    /// <returns> 
    ///   The folder where the resources are expected to be. Ends on a slash unless the root folder is an 
    ///   empty string.
    /// </returns>
    [Obsolete ("Use Control.ResolveClientUrl(\"~/\") instead. (Version 1.13.198)", true)]
    public static string GetRoot (bool isDesignMode)
    {
      throw new NotImplementedException ("Use Control.ResolveClientUrl(\"~/\") instead. (Version 1.13.198)");
    }
  }
}