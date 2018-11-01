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

namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   The <see cref="IBusinessObjectClass"/> interface provides functionality for defining the <b>class</b> of an 
  ///   <see cref="IBusinessObject"/>. 
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The <see cref="IBusinessObjectClass"/> interface provides the list of <see cref="IBusinessObjectProperty"/> instances
  ///     available by an <see cref="IBusinessObject"/> of this <see cref="IBusinessObjectClass"/>'s type. 
  ///   </para><para>
  ///     It also provides services for accessing class specific meta data.
  ///   </para>
  /// </remarks>
  public interface IBusinessObjectClass
  {
    /// <summary> Gets the type name as presented to the user. </summary>
    /// <returns> The human readable identifier of this type. </returns>
    /// <remarks> The result of this method may depend on the current culture. </remarks>
    string GetDisplayName ();

    /// <summary> Returns the <see cref="IBusinessObjectProperty"/> for the passed <paramref name="propertyIdentifier"/>. </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> uniquely identifying an <see cref="IBusinessObjectProperty"/> in this <see cref="IBusinessObjectClass"/>.
    /// </param>
    /// <returns>
    ///   Returns the <see cref="IBusinessObjectProperty"/> 
    ///   or <see langword="null" /> if the <see cref="IBusinessObjectProperty"/> does not exist on this <see cref="IBusinessObjectClass"/>. 
    ///  </returns>
    IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier);

    /// <summary> 
    ///   Returns the <see cref="IBusinessObjectProperty"/> instances defined for this business object class.
    /// </summary>
    /// <returns> An array of <see cref="IBusinessObjectProperty"/> instances. Must not be <see langword="null"/>. </returns>
    IBusinessObjectProperty[] GetPropertyDefinitions ();

    /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this business object class. </summary>
    /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type.
    ///   <note type="inotes">
    ///     Must not return <see langword="null"/>.
    ///   </note>
    /// </value>
    IBusinessObjectProvider BusinessObjectProvider { get; }

    /// <summary>
    ///   Gets a flag that specifies whether a referenced object of this business object class needs to be 
    ///   written back to its container if some of its values have changed.
    /// </summary>
    /// <value> <see langword="true"/> if the <see cref="IBusinessObject"/> must be reassigned to its container. </value>
    /// <example>
    ///   The following pseudo code shows how this value affects the binding behaviour.
    ///   <code><![CDATA[
    ///   Address address = person.Address;
    ///   address.City = "Vienna";
    ///   // the RequiresWriteBack property of the 'Address' business object class specifies 
    ///   // whether the following statement is required:
    ///   person.Address = address;
    ///   ]]></code>
    /// </example>
    bool RequiresWriteBack { get; }

    /// <summary> Gets the identifier (i.e. the type name) for this business object class. </summary>
    /// <value> 
    ///   A string that uniquely identifies the business object class within the business object model. 
    /// </value>
    string Identifier { get; }
  }
}
