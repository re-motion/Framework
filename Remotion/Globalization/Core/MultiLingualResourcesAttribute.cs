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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  ///   Attribute for specifying the resource container for a type.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Resources linked to a <see cref="Type"/> via the <see cref="MultiLingualResourcesAttribute"/> are resolved using the <see cref="IGlobalizationService"/>, 
  /// retrieved from the application's IoC via the <see cref="ServiceLocator"/>. 
  /// </para>
  /// The localized names of reflection types can be resolved using the 
  /// <see cref="IMemberInformationGlobalizationService"/>, <see cref="IEnumerationGlobalizationService"/>,
  /// and <see cref="T:Remotion.Globalization.ExtensibleEnums.IExtensibleEnumerationGlobalizationService"/>, in which case the following naming rules apply:
  /// <list type="bullet">
  ///   <item>
  ///     <term>Type</term>
  ///     <description>
  ///       For a <b>Type</b>, the default formats for the resource identifier are <c>type:&lt;Namespace&gt;.&lt;TypeName&gt;</c> 
  ///       and <c>type:TypeName</c>.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>Property</term>
  ///     <description>
  ///       For a <b>Property</b>, the default formats for the resource identifier are <c>property:&lt;Namespace&gt;.&lt;TypeName&gt;.&lt;PropertyName&gt;</c> 
  ///       and <c>property:Property</c>.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>Enum</term>
  ///     <description>
  ///       For an <b>Enum Value</b>, the default formats for the resource identifier is <c>&lt;Namespace&gt;.&lt;TypeName&gt;.&lt;EnumValue&gt;</c>.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>Extensible Enum</term>
  ///     <description>
  ///       For an <b>Extensible Enum Value</b>, the default format for the resource identifier uses the <see cref="P:Remotion.Globalization.ExtensibleEnums.IExtensibleEnum.ID"/> 
  ///       property of the <see cref="T:Remotion.Globalization.ExtensibleEnums.IExtensibleEnum"/> object.
  ///     </description>
  ///   </item>
  /// </list>
  /// The <see cref="IObjectWithResources"/> interface can be used to allow custom retrieval of the <see cref="IResourceManager"/> for a type. 
  /// This is used in web controls.
  /// <para>
  /// Use the <see cref="AvailableResourcesLanguagesAttribute"/> to list all cultures for which a localization has been provided in the assembly
  /// to improve performance when retrieving the resources.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = true, Inherited = false)]
  public class MultiLingualResourcesAttribute : Attribute, IResourcesAttribute
  {
    /// <summary> The base name of the resource container </summary>
    private string _baseName;

    private Assembly? _resourceAssembly = null;

    /// <summary> Initalizes an instance. </summary>
    public MultiLingualResourcesAttribute (string baseName)
    {
      SetBaseName(baseName);
    }

    /// <summary>
    ///   Gets the base name of the resource container as specified by the attributes construction.
    /// </summary>
    /// <remarks>
    /// The base name of the resource conantainer to be used by this type
    /// (&lt;assembly&gt;.&lt;path inside project&gt;.&lt;resource file name without extension&gt;).
    /// </remarks>
    public string BaseName
    {
      get { return _baseName; }
    }

    [MemberNotNull(nameof(_baseName))]
    protected void SetBaseName (string baseName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("baseName", baseName);
      _baseName = baseName;
    }

    public Assembly? ResourceAssembly
    {
      get { return _resourceAssembly; }
    }

    protected void SetResourceAssembly (Assembly resourceAssembly)
    {
      ArgumentUtility.CheckNotNull("resourceAssembly", resourceAssembly);
      _resourceAssembly = resourceAssembly;
    }

    public override string? ToString ()
    {
      return BaseName;
    }
  }
}
