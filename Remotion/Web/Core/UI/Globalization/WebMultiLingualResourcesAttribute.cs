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
using System.Web.UI;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Globalization
{

  /// <summary> Specifies the location of the resources used by a <see cref="Page"/> or <see cref="UserControl"/>. </summary>
  /// <remarks>
  ///   The <see cref="WebMultiLingualResourcesAttribute"/> enhances the globalization system with 
  ///   support for the new compilation model introduced with ASP.NET 2.0.
  /// </remarks>
  /// <example>
  ///   <b>Specifying a resource for ASP.NET 1.1 Web Projects</b>
  ///   <para>
  ///     The following code snippet assumes a resource file named <c>MyForm.resx</c> is located within a folder named
  ///     <c>Globalization</c>, below the project root. The assembly name is <c>MyCompany.MyProject.Client.Web</c>.
  ///     The resource file is set to be an embedded resource. 
  ///   </para><para>
  ///     The <see cref="WebMultiLingualResourcesAttribute"/> is passed the resource's <b>Base Name</b>. It consists
  ///     of the assembly name, followed by a dot and the folder hierarchy below the project root, followed by a dot 
  ///     and the name of the neutral culture's resource file minus the extension.
  ///   </para>
  ///   <note>
  ///     The naming convention for the <b>Base Name</b> is a result of following the rules described in this example. 
  ///     It is possible to create resources with <b>Base Names</b> of an entirely different structure. As long as the
  ///     resource is a part of the web page's assembly, the <see cref="WebMultiLingualResourcesAttribute"/> will
  ///     be able to resolve the resource's <b>Base Names</b>.
  ///   </note>
  ///   <code lang="C#">
  /// using System.Web.UI;
  /// using Remotion.Web.UI.Globalization;
  /// 
  /// namespace MyCompany.MyProject.Client.Web.UI
  /// {
  ///   [WebMultiLingualResources ("MyCompany.MyProject.Client.Web.Globalization.MyForm")]
  ///   public class MyForm : Page
  ///   {
  ///   }
  /// }
  ///   </code>
  /// </example>
  /// 
  /// <example>
  ///   <b>Specifying a resource for ASP.NET 2.0 Web Sites</b>
  ///   <para>
  ///     The following code snippet assumes a resource file named <c>MyForm.resx</c> is located within the special
  ///     folder <c>App_GlobalResources</c> of an ASP.NET Web Site.
  ///   </para><para>
  ///     The <see cref="WebMultiLingualResourcesAttribute"/> is passed the <see cref="Type"/> of the resource. 
  ///     In ASP.NET 2.0 applications, a type is generated for each global resource, and named after the resource's 
  ///     <b>Base Name</b>. Passing the type name ensures compile time verification of the resource's existence.
  ///     It is also possible to specify the <b>Base Name</b> as a string, but this is only recommended for upgrading 
  ///     an existing ASP.NET 1.1 project, since it simplifies the Search and Replace process.
  ///   </para><para>
  ///     In ASP.NET 2.0, the <b>Base Name</b> of global resources consists of the prefix <c>Resources</c>, followed by 
  ///     a dot, followed by the name of the neutral culture's resource file minus the extension.
  ///   </para>
  ///   <note>
  ///     The naming convention for the <b>Base Name</b> is a result of the ASP.NET 2.0 compilation model. A separate
  ///     assembly will be generated for the global resoruces. The <see cref="WebMultiLingualResourcesAttribute"/> 
  ///     uses the <b>System.Web.Compilation.BuildManager.GetType</b> method to resolve the resource.
  ///   </note>
  ///     <b>Base Name</b> as <see cref="Type"/> version:
  ///   <code lang="C#">
  /// using System.Web.UI;
  /// using Remotion.Web.UI.Globalization;
  /// 
  /// [WebMultiLingualResources (typeof (Resources.MyForm))]
  /// public class MyForm : Page
  /// {
  /// }
  ///   </code>
  ///     <b>Base Name</b> as <see cref="String"/> version:
  ///   <code lang="C#">
  /// using System.Web.UI;
  /// using Remotion.Web.UI.Globalization;
  /// 
  /// [WebMultiLingualResources ("Resources.MyForm")]
  /// public class MyForm : Page
  /// {
  /// }
  ///   </code>
  /// </example>
  /// 
  /// <example>
  ///   <b>Upgrading an ASP.NET 1.1 Web Project</b>
  ///   <list type="number">
  ///     <item> 
  ///       Move the resource files form the folder <c>Globalization</c> into the project's root folder. 
  ///     </item>
  ///     <item> 
  ///       Upgrade the project. 
  ///     </item>
  ///     <item>
  ///       Search and Replace any occurance of <see cref="MultiLingualResourcesAttribute"/> with
  ///       <see cref="WebMultiLingualResourcesAttribute"/>. (Necessary for projects implemented before the 
  ///       <see cref="WebMultiLingualResourcesAttribute"/> has become available with verison 1.4 of the
  ///       library. 
  ///     </item>
  ///     <item> 
  ///       Search and Replace any occurance of 
  ///       <c>[WebMultiLingualResourcesAttribute ("MyCompany.MyProject.Client.Web.Globalization.</c> 
  ///       with <c>[WebMultiLingualResources ("Resources.</c>. 
  ///     </item>
  ///   </list>
  /// </example>
  public class WebMultiLingualResourcesAttribute : MultiLingualResourcesAttribute
  {

    public WebMultiLingualResourcesAttribute (string baseName)
        : base (baseName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("baseName", baseName);
      Type type = BuildManager.GetType (baseName, throwOnError: false, ignoreCase: false);
      if (type != null)
        SetResourceAssembly (type.Assembly);
    }

    public WebMultiLingualResourcesAttribute (Type resourceType)
        : base (resourceType.FullName)
    {
      ArgumentUtility.CheckNotNull ("resourceType", resourceType);
      SetResourceAssembly (resourceType.Assembly);
    }

    private static IBuildManager BuildManager
    {
      get { return SafeServiceLocator.Current.GetInstance<IBuildManager>(); }
    }
  }
}
