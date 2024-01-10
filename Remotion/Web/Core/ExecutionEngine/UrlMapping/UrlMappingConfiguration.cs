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
using System.Collections;
using System.Xml.Serialization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine.UrlMapping
{

/// <summary> Contains the configuration data for the URL mapping system of the execution engine. </summary>
/// <include file='..\..\doc\include\ExecutionEngine\UrlMapping\UrlMappingConfiguration.xml' path='UrlMappingConfiguration/Class/*' />
[XmlType(UrlMappingConfiguration.ElementName, Namespace = UrlMappingConfiguration.SchemaUri)]
public class UrlMappingConfiguration
{
  /// <summary> The name of the root element. </summary>
  /// <remarks> <c>mapping</c> </remarks>
  public const string ElementName = "urlMapping";

  /// <summary> The namespace of the mapping's schema. </summary>
  /// <remarks> <c>http://www.re-motion.org/Web/ExecutionEngine/UrlMapping/1.0</c> </remarks>
  public const string SchemaUri = "http://www.re-motion.org/Web/ExecutionEngine/UrlMapping/1.0";

  private static readonly DoubleCheckedLockingContainer<UrlMappingConfiguration> s_current =
      new DoubleCheckedLockingContainer<UrlMappingConfiguration>(CreateConfig);

  /// <summary> Gets the current <see cref="UrlMappingConfiguration"/>. </summary>
  public static UrlMappingConfiguration Current
  {
    get { return s_current.Value; }
  }

  private static UrlMappingConfiguration CreateConfig ()
  {
    var urlMappingLoader = new UrlMappingLoader(
        SafeServiceLocator.Current.GetInstance<IUrlMappingFileFinder>(),
        SafeServiceLocator.Current.GetInstance<IUrlMappingFileLoader>());

    return urlMappingLoader.CreateUrlMappingConfiguration();
  }

  /// <summary> Sets the current <see cref="UrlMappingConfiguration"/>. </summary>
  public static void SetCurrent (UrlMappingConfiguration mappingConfiguration)
  {
    s_current.Value = mappingConfiguration;
  }

  private string? _applicationName = null;
  private UrlMappingCollection _mappings = new UrlMappingCollection();

  public UrlMappingConfiguration ()
  {
  }

  /// <summary> Gets the application name that is specified in the XML configuration file.  </summary>
  [XmlAttribute("application")]
  public string? ApplicationName
  {
    get { return _applicationName; }
  }

  [XmlElement("add")]
  public UrlMappingCollection Mappings
  {
    get { return _mappings; }
  }
}

[XmlType("add", Namespace = UrlMappingConfiguration.SchemaUri)]
public class UrlMappingEntry
{
  private string? _id  = null;
  private string _functionTypeName = null!;
  private Type _functionType = null!;
  private string _resource = null!;

  public UrlMappingEntry ()
  {
  }

  public UrlMappingEntry (string? id, Type functionType, string resource)
  {
    ID = id;
    FunctionType = functionType;
    Resource = resource;
  }

  public UrlMappingEntry (string? id, string functionTypeName, string resource)
  {
    ID = id;
    FunctionTypeName = functionTypeName;
    Resource = resource;
  }

  public UrlMappingEntry (Type functionType, string resource)
      : this(null, functionType, resource)
  {
  }

  public UrlMappingEntry (string functionTypeName, string resource)
      : this(null, functionTypeName, resource)
  {
  }

  /// <summary> An optional ID for the <see cref="UrlMappingEntry"/>. </summary>
  [XmlAttribute("id")]
  public string? ID
  {
    get
    {
      return _id;
    }
    set
    {
      _id = StringUtility.EmptyToNull(value);
    }
  }

  /// <summary>
  ///   The <see cref="Type"/> name of the <see cref="WxeFunction"/> identified by the <see cref="Resource"/>. 
  ///   Must not be <see langword="null"/> or empty. 
  /// </summary>
  /// <value> A valid type name. </value>
  /// <remarks>
  ///   Valid type names include the classic .net syntax and typenames using the abbreviated form as defined by the
  ///   <see cref="TypeUtility.ParseAbbreviatedTypeName">TypeUtility.ParseAbbreviatedTypeName</see> method.
  ///   In ASP.NET 2.0, it is possible to use functions located in the <b>App_Code</b> assembly by not specifying an
  ///   assembly name.
  /// </remarks>
  [XmlAttribute("type")]
  public string FunctionTypeName
  {
    get
    {
      return _functionTypeName;
    }
    set
    {
      ArgumentUtility.CheckNotNullOrEmpty("FunctionTypeName", value);
      FunctionType = WebTypeUtility.GetType(value, true)!;
    }
  }

  /// <summary> 
  ///   The <see cref="Type"/> of the <see cref="WxeFunction"/> identified by the <see cref="Resource"/>. 
  ///   Must not be <see langword="null"/>. 
  /// </summary>
  /// <value> A <see cref="Type"/> derived from the <see cref="WxeFunction"/> type. </value>
  [XmlIgnore]
  public Type FunctionType
  {
    get
    {
      return _functionType;
    }
    set
    {
      ArgumentUtility.CheckNotNull("FunctionType", value);
      if (! typeof(WxeFunction).IsAssignableFrom(value))
        throw new ArgumentException(string.Format("The FunctionType '{0}' must be derived from WxeFunction.", value), "FunctionType");
      _functionType = value;
      _functionTypeName = _functionType.GetAssemblyQualifiedNameChecked();
    }
  }

  /// <summary> 
  ///   The path associated with the <see cref="FunctionType"/>. Must not be <see langword="null"/> or empty. 
  /// </summary>
  /// <value> A virtual path, relative to the application root. Will always start with <c>~/</c>. </value>
  [XmlAttribute("resource")]
  public string Resource
  {
    get
    {
      return _resource;
    }
    set
    {
      ArgumentUtility.CheckNotNull("Resource", value);
      value = value!.Trim();
      ArgumentUtility.CheckNotNullOrEmpty("Resource", value);
      if (value.StartsWith("/") || value.IndexOf(":") != -1)
        throw new ArgumentException(string.Format("No absolute paths are allowed. Resource: '{0}'", value), "Resource");
      if (! value.StartsWith("~/"))
        value = "~/" + value;
      _resource = value;
    }
  }
}

public class UrlMappingCollection: CollectionBase
{
  public UrlMappingCollection ()
  {
  }

  public UrlMappingEntry this[int index]
  {
    get { return (UrlMappingEntry)List[index]!; } // TODO RM-8118: not null assertion
    set { List[index] = value; }
  }

  public UrlMappingEntry? this[string path]
  {
    get { return Find(path); }
  }

  public UrlMappingEntry? this[Type functionType]
  {
    get { return Find(functionType); }
  }

  public int Add (UrlMappingEntry entry)
  {
    return List.Add(entry);
  }

  public void Remove (UrlMappingEntry entry)
  {
    if (entry != null)
      List.Remove(entry);
  }

  protected virtual void ValidateNewValue (object? value)
  {
    UrlMappingEntry entry = ArgumentUtility.CheckNotNullAndType<UrlMappingEntry>("value", value!);
    base.OnValidate(entry);
    if (Find(entry.Resource) != null)
      throw new ArgumentException(string.Format("The mapping already contains an entry for the following resource: '{0}'.", entry.Resource), "value");
    if (FindByID(entry.ID) != null)
      throw new ArgumentException(string.Format("The mapping already contains an entry for the following ID: '{0}'.", entry.ID), "value");
  }

  protected override void OnInsert (int index, object? value)
  {
    ValidateNewValue(value);
    base.OnInsert(index, value);
  }

  protected override void OnSet (int index, object? oldValue, object? newValue)
  {
    ValidateNewValue(newValue);
    base.OnSet(index, oldValue, newValue);
  }

  /// <summary> Finds the mapping for the specified <paramref name="path"/>. </summary>
  /// <param name="path"> The path to look-up. </param>
  /// <returns> 
  ///   A <see cref="Type"/> or <see langword="null"/> if the <paramref name="path"/> does not map to a 
  ///   <see cref="WxeFunction"/>.
  /// </returns>
  public Type? FindType (string path)
  {
    UrlMappingEntry? entry = Find(path);
    if (entry == null)
      return null;
    return entry.FunctionType;
  }

  /// <summary> Finds the mapping for the specified <paramref name="type"/>. </summary>
  /// <param name="type"> The name of the <see cref="Type"/> to look-up. </param>
  /// <returns> 
  ///   A <see cref="string"/> or <see langword="null"/> if the <paramref name="type"/> is not mapped to a path.
  /// </returns>
  public string? FindResource (Type? type)
  {
    UrlMappingEntry? entry = Find(type);
    if (entry == null)
      return null;
    return entry.Resource;
  }

  /// <summary> Finds the mapping for the specified <paramref name="typeName"/>. </summary>
  /// <param name="typeName"> The name of the <see cref="Type"/> to look-up. </param>
  /// <returns> 
  ///   A <see cref="string"/> or <see langword="null"/> if the <paramref name="typeName"/> is not mapped
  ///   to a path.
  /// </returns>
  public string? FindResource (string typeName)
  {
    if (string.IsNullOrEmpty(typeName))
      return null;
    Type type = WebTypeUtility.GetType(typeName, throwOnError: true)!;
    return FindResource(type);
  }

  /// <summary> Finds the mapping for the specified <paramref name="path"/>. </summary>
  /// <param name="path"> The path to look-up. </param>
  /// <returns> 
  ///   A <see cref="UrlMappingEntry"/> or <see langword="null"/> if the <paramref name="path"/> does not map to a 
  ///   <see cref="WxeFunction"/>.
  /// </returns>
  public UrlMappingEntry? Find (string? path)
  {
    if (string.IsNullOrEmpty(path))
      return null;
    for (int i = 0; i < Count; i++)
    {
      if (string.Compare(this[i].Resource, path, true) == 0)
        return this[i];
    }
    return null;
  }

  /// <summary> Finds the mapping for the specified <paramref name="functionType"/>. </summary>
  /// <param name="functionType"> The <see cref="Type"/> to look-up. </param>
  /// <returns> 
  ///   A <see cref="UrlMappingEntry"/> or <see langword="null"/> if the <paramref name="functionType"/> is not mapped
  ///   to a path.
  /// </returns>
  public UrlMappingEntry? Find (Type? functionType)
  {
    if (functionType == null)
      return null;
    for (int i = 0; i < Count; i++)
    {
      if (this[i].FunctionType == functionType)
        return this[i];
    }
    return null;
  }

  /// <summary> Finds the mapping for the specified <paramref name="id"/>. </summary>
  /// <param name="id"> The ID to look-up. Must not be <see langword="null"/> or empty. </param>
  /// <returns> 
  ///   A <see cref="UrlMappingEntry"/> or <see langword="null"/> if the <paramref name="id"/> could not be found.
  /// </returns>
  public UrlMappingEntry? FindByID (string? id)
  {
    if (string.IsNullOrEmpty(id))
      return null;
    for (int i = 0; i < Count; i++)
    {
      if (this[i].ID == id)
        return this[i];
    }
    return null;
  }
}

}
