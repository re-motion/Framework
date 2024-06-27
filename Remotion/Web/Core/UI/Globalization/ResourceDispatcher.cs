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
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Microsoft.Extensions.Logging;
using Remotion.Globalization;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Web.UI.Globalization
{
/// <summary>
///   Provides methods for dispatching the resources inside an IResourceManager container
///   to a control.
/// </summary>
/// <include file='..\..\doc\include\ResourceDispatcher.xml' path='/ResourceDispatcher/Class/example' />
public sealed class ResourceDispatcher
{
  // types

  // static members and constants

  /// <summary> Use this ID to dispatch resources to the control that provides the resource manager. </summary>
  private const string c_thisElementID = "this";

  private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<ResourceDispatcher>();
  private static ArrayList _registeredDispatchTargets = new ArrayList();
  private static readonly WebStringConverter s_webStringConverter = new();

  /// <summary>
  ///   Dispatches resources.
  /// </summary>
  /// <include file='..\..\doc\include\ResourceDispatcher.xml' path='/ResourceDispatcher/Dispatch/remarks' />
  /// <param name="control">
  ///   The control for which resources are to be dispatched. Must not be <see langname="null"/>.
  /// </param>
  /// <param name="resourceManager">
  ///   The resource manager to be used. Must not be <see langname="null"/>.
  /// </param>  
  public static void Dispatch (Control control, IResourceManager resourceManager)
  {
    ArgumentUtility.CheckNotNull("control", control);
    ArgumentUtility.CheckNotNull("resourceManager", resourceManager);

    const string prefix = "auto:";

    var autoElements = ResourceDispatcher.GetResources(resourceManager, prefix);

    ResourceDispatcher.Dispatch(control, autoElements, resourceManager.Name);
  }

  /// <summary>
  ///   Dispatches resources provided by <see cref="IObjectWithResources.GetResourceManager()"/>
  /// </summary>
  /// <param name="control">
  ///   The control for which resources are to be dispatched. Must not be <see langname="null"/>.
  ///   The control and/or one or more of its parents must implement <see cref="IObjectWithResources"/>.
  /// </param>
  /// <param name="throwExceptionIfNoResources"> If true and neither the control nor its parents
  ///   define a resource manager, an InvalidOperationException is thrown. </param>
  public static void Dispatch (Control control, bool throwExceptionIfNoResources)
  {
    IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager(control, false);
    if (resourceManager == null)
    {
      if (throwExceptionIfNoResources)
        throw new InvalidOperationException("Control " + control.UniqueID + " has no resource managers.");
      else
        return;
    }
    Dispatch(control, resourceManager);
  }

  /// <summary>
  ///   Dispatches an IDictonary of elementID/IDictonary pairs to the specified control.
  /// </summary>
  /// <include file='..\..\doc\include\ResourceDispatcher.xml' path='/ResourceDispatcher/DispatchMain/*' />
  public static void Dispatch (Control control, IDictionary<string, IDictionary<string, WebString>> elements, string resourceSource)
  {
    ArgumentUtility.CheckNotNull("control", control);
    ArgumentUtility.CheckNotNull("elements", elements);

    //  Dispatch the resources to the controls
    foreach (var elementsEntry in elements)
    {
      var elementID = elementsEntry.Key;

      Control? targetControl;

      if (elementID == c_thisElementID)
        targetControl = (Control)control;
      else
        targetControl = control.FindControl(elementID);

      if (targetControl == null)
      {
        s_logger.LogWarning("Control '" + control.ToString() + "': No child-control with ID '" + elementID + "' found. ID was read from \"" + resourceSource + "\".");
      }
      else
      {
        //  Pass the value to the control
        var values = elementsEntry.Value;
        IResourceDispatchTarget? resourceDispatchTarget = targetControl as IResourceDispatchTarget;

        if (resourceDispatchTarget != null) //  Control knows how to dispatch
          resourceDispatchTarget.Dispatch(values);
        else
          ResourceDispatcher.DispatchGeneric(targetControl, values);
      }
    }
  }

  /// <summary>
  ///   Dispatches the resources passed in <paramref name="values"/> to the properties of <paramref name="obj"/>.
  /// </summary>
  /// <include file='..\..\doc\include\ResourceDispatcher.xml' path='/ResourceDispatcher/DispatchGeneric/*' />
  public static void DispatchGeneric (object obj, IDictionary<string, WebString> values)
  {
    ArgumentUtility.CheckNotNull("obj", obj);
    ArgumentUtility.CheckNotNull("values", values);

    foreach (var entry in values)
    {
      var propertyName = entry.Key;
      var propertyValue = entry.Value;

      PropertyInfo? property = obj.GetType().GetProperty(propertyName);
      if (property?.PropertyType == typeof(WebString))
      {
        property.SetValue(obj, propertyValue, Array.Empty<object>());
      }
      else if (property?.PropertyType == typeof(PlainTextString))
      {
        property.SetValue(obj, propertyValue.ToPlainTextString(), Array.Empty<object>());
      }
      else if (property?.PropertyType == typeof(string))
      {
        property.SetValue(obj, propertyValue.GetValue(), Array.Empty<object>());
      }
      else if (obj is Control)
      {
        Control control = (Control)obj;
        //  Test for HtmlControl, they can take anything
        HtmlControl? genericHtmlControl = control as HtmlControl;
        if (genericHtmlControl != null)
          genericHtmlControl.Attributes[propertyName] = propertyValue.ToPlainTextString().GetValue();
        else //  Non-HtmlControls require valid property
          s_logger.LogWarning("Control '" + control.ID + "' of type '" + control.GetType().GetFullNameSafe() + "' does not contain a public property '" + propertyName + "'.");
      }
    }
  }

  /// <summary>
  ///   Selects all resources matching the <c>prefix</c> into a HashTable.
  /// </summary>
  /// <param name="resourceManager">
  ///   The <see cref="IResourceManager"/> to select from. Must not be <see langname="null"/>.
  /// </param>
  /// <param name="prefix">
  ///   The filter prefix, can be empty. Must not be <see langname="null"/>.
  /// </param>
  /// <returns>
  ///   Hashtable&lt;string elementID, IDictionary&lt;string property, string value&gt; elementValues&gt;
  /// </returns>
  private static IDictionary<string, IDictionary<string, WebString>> GetResources (IResourceManager resourceManager, string? prefix)
  {
    ArgumentUtility.CheckNotNull("resourceManager", resourceManager);

    if (prefix == null)
      prefix = String.Empty;

    // Hashtable<string elementID, IDictionary<string property, string value> elementValues>
    var elements = new Dictionary<string, IDictionary<string, WebString>>();

    var resources = resourceManager.GetAllStrings(prefix);
    foreach (var resourceEntry in resources)
    {
      //  Compound key: "prfx:elementID:argument"
      //  The argument (including the colon) is optional
      //  resources contain only keys with the prefix "auto" because of the applied filter

      //  Remove the prefix and colon
      var key = resourceEntry.Key.Substring(prefix.Length);

      //  Test for a second colon in the key
      int posColon = key.IndexOf(':');

      if (posColon >= 0)
      {
        //  If one is found, this indicates an argument attached to the elementID

        string elementID = key.Substring(0, posColon);
        string property = key.Substring(posColon + 1);

        //  Now there can be more than one argument provided for a specific element
        //  Create a dictonary for the element,
        //  using the argument as key and the resources' value as the value.

        //  Get the dictonary for the current element
        //  If no dictonary exists, create it and insert it into the elements hashtable.
        if (!elements.TryGetValue(elementID, out var elementValues))
        {
          elementValues = new Dictionary<string, WebString>();
          elements[elementID] = elementValues;
        }

        //  Insert the argument and resource's value into the dictonary for the specified element.
        elementValues.Add(property, (WebString?)s_webStringConverter.ConvertFromString(resourceEntry.Value) ?? WebString.Empty);
      }
    }

    return elements;
  }


  //  construction and disposing

  private ResourceDispatcher ()
  {
  }
}

}
