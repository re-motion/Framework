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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   This <see langword="abstract"/> class provides base functionality usually required by an implementation of
  ///   <see cref="IBusinessObjectReferenceDataSource"/>.
  /// </summary>
  /// <remarks>
  ///   Only members with functionality common to all implementations of <see cref="IBusinessObjectReferenceDataSource"/> 
  ///   have been implemented. The actual implementation of the <see cref="IBusinessObjectBoundEditableControl"/> 
  ///   interface is left to the child class. 
  /// </remarks>
  /// <seealso cref="IBusinessObjectReferenceDataSource"/>
  public abstract class BusinessObjectReferenceDataSourceBase : BusinessObjectDataSource, IBusinessObjectReferenceDataSource
  {
#pragma warning disable 612,618
    /// <summary>
    ///   The <see cref="IBusinessObject"/> accessed through <see cref="ReferenceProperty"/> and provided as 
    ///   the <see cref="BusinessObject"/>.
    /// </summary>
    private IBusinessObject _businessObject;

    /// <summary>
    ///   A flag that is cleared when the <see cref="BusinessObject"/> is loaded from or saved to the
    ///   <see cref="ReferencedDataSource"/>.
    /// </summary>
    private bool _hasBusinessObjectChanged;

    /// <summary>
    ///   A flag that is cleared when the <see cref="BusinessObject"/> is loaded from the 
    ///   <see cref="IBusinessObjectReferenceProperty.CreateDefaultValue"/> API.
    /// </summary>
    private bool _hasBusinessObjectCreated;

    /// <summary>
    ///   See <see cref="IBusinessObjectReferenceDataSource.ReferenceProperty">IBusinessObjectReferenceDataSource.ReferenceProperty</see>
    ///   for information on how to implement this <see langword="abstract"/> property.
    /// </summary>
    public abstract IBusinessObjectReferenceProperty ReferenceProperty { get; }

    /// <summary>
    ///   See <see cref="IBusinessObjectReferenceDataSource.ReferencedDataSource">IBusinessObjectReferenceDataSource.ReferencedDataSource</see>
    ///   for information on how to implement this <see langword="abstract"/> property.
    /// </summary>
    public abstract IBusinessObjectDataSource ReferencedDataSource { get; }

    /// <summary>
    /// Returns a meaningful identifier for the data source to be used in exception messages, etc.
    /// </summary>
    protected abstract string GetDataSourceIdentifier ();

    protected BusinessObjectReferenceDataSourceBase ()
    {
    }

    /// <summary> 
    ///   Loads the <see cref="BusinessObject"/> from the <see cref="ReferencedDataSource"/> using 
    ///   <see cref="ReferenceProperty"/> and populates the bound controls using 
    ///   <see cref="BusinessObjectDataSource.LoadValues"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <remarks>
    ///   For details on <see cref="LoadValue"/>, 
    ///   see <see cref="IBusinessObjectDataSource.LoadValues">IBusinessObjectDataSource.LoadValues</see>.
    /// </remarks>
    /// <seealso cref="IBusinessObjectBoundControl.LoadValue">IBusinessObjectBoundControl.LoadValue</seealso>
    public void LoadValue (bool interim)
    {
      // load value from "parent" data source
      if (HasValidBinding)
      {
        if (interim && HasBusinessObjectCreated)
        {
          // NOP
        }
        else
        {
          if (HasBusinessObjectCreated)
          {
            DeleteBusinessObject();
            HasBusinessObjectCreated = false;
          }

          _businessObject = null;

          if (ReferencedDataSource.BusinessObject != null)
          {
            _businessObject = (IBusinessObject) ReferencedDataSource.BusinessObject.GetProperty (ReferenceProperty);
            if (_businessObject == null && SupportsDefaultValueSemantics)
            {
              _businessObject = ReferenceProperty.CreateDefaultValue (ReferencedDataSource.BusinessObject);
              HasBusinessObjectCreated = true;
            }
          }
          HasBusinessObjectChanged = false;
        }
      }

      // load values into "child" controls
      LoadValues (interim);
    }

    /// <summary> 
    ///   Saves the values from the bound controls using <see cref="BusinessObjectDataSource.SaveValues"/>
    ///   and writes the <see cref="BusinessObject"/> back into the <see cref="ReferencedDataSource"/> using 
    ///   <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
    /// <returns>
    /// <see langword="true"/> if all bound controls have saved their value into the <see cref="BusinessObject"/> 
    /// and the <see cref="BusinessObject"/> was saved back into the bound <see cref="ReferencedDataSource"/>.<see cref="IBusinessObjectDataSource.BusinessObject"/>.
    /// </returns>
    /// <remarks>
    ///   For details on <see cref="SaveValue"/>, 
    ///   see <see cref="IBusinessObjectDataSource.SaveValues">IBusinessObjectDataSource.SaveValues</see>.
    /// </remarks>
    /// <seealso cref="IBusinessObjectBoundEditableControl.SaveValue">IBusinessObjectBoundEditableControl.SaveValue</seealso>
    public bool SaveValue (bool interim)
    {
      if (!HasValidBinding)
        return false;

      if (!interim && IsBusinessObjectSetToDefaultValue())
      {
        DeleteBusinessObject();
        _businessObject = null;
        HasBusinessObjectChanged = true;
        HasBusinessObjectCreated = false;
      }

      // save values from "child" controls
      var hasSavedAllBoundControl = SaveValues (interim);

      // if required, save value into "parent" data source

      if (!RequiresWriteBack)
        return hasSavedAllBoundControl;

      if (IsReadOnlyInDomainModel)
      {
        throw new InvalidOperationException (
            string.Format (
                "The business object of the {0} could not be saved into the domain model because the property '{1}' is read only.",
                GetDataSourceIdentifier(),
                ReferenceProperty.Identifier));
      }

      bool hasSaved;
      if (ReferencedDataSource.BusinessObject == null)
      {
        hasSaved = false;
      }
      else
      {
        ReferencedDataSource.BusinessObject.SetProperty (ReferenceProperty, BusinessObject);
        hasSaved = hasSavedAllBoundControl;
      }
      HasBusinessObjectChanged = false;
      HasBusinessObjectCreated = false;
      return hasSaved;
    }

    /// <summary>
    /// Evaluates whether the <see cref="BusinessObjectReferenceDataSource"/> contains a value that will be written back into the 
    /// <see cref="ReferencedDataSource"/> during <see cref="SaveValue"/>.
    /// </summary>
    /// <returns></returns>
    public bool HasValue ()
    {
      if (BusinessObject == null)
        return false;
      else if (IsBusinessObjectSetToDefaultValue())
        return false;
      else
        return true;
    }

    /// <summary> 
    ///   Gets a flag that is <see langword="true"/> if the <see cref="BusinessObject"/> has been set since the last
    ///   call to <see cref="LoadValue"/> or <see cref="SaveValue"/>.
    /// </summary>
    public bool HasBusinessObjectChanged
    {
      get { return _hasBusinessObjectChanged; }
      private set 
      {
        if (value && HasValidBinding && IsReadOnlyInDomainModel)
        {
          throw new InvalidOperationException (
              string.Format (
                  "The {0} '{1}' could not be marked as changed because the bound property '{2}' is read only.",
                  GetType().Name,
                  GetDataSourceIdentifier(),
                  ReferenceProperty.Identifier));
        }
        _hasBusinessObjectChanged = value;
      }
    }

    /// <summary>
    ///   Gets a flag that is <see langword="true" /> if the <see cref="BusinessObject"/> is loaded from the 
    ///   <see cref="IBusinessObjectReferenceProperty.CreateDefaultValue"/> API. It is cleared once the newly created <see cref="BusinessObject"/> 
    ///   has been saved back into the <see cref="ReferencedDataSource"/>.
    /// </summary>
    public bool HasBusinessObjectCreated
    {
      get { return _hasBusinessObjectCreated; }
      private set { _hasBusinessObjectCreated = value; }
    }

    /// <summary>
    ///   Tests whether the <see cref="IBusinessObjectReferenceDataSource"/> can be bound to the 
    ///   <paramref name="property"/>.
    /// </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> to be tested. Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    ///   <see langword="true"/> if the <paremref name="property"/> is of type 
    ///   <see cref="IBusinessObjectReferenceProperty"/>.
    /// </returns>
    /// <seealso cref="IBusinessObjectBoundControl.SupportsProperty">IBusinessObjectBoundControl.SupportsProperty</seealso>
    public bool SupportsProperty (IBusinessObjectProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      return property is IBusinessObjectReferenceProperty;
    }

    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObject"/> accessed through the <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <value> An <see cref="IBusinessObject"/> or <see langword="null"/>. </value>
    public override sealed IBusinessObject BusinessObject
    {
      get { return _businessObject; }
      set
      {
         if (HasBusinessObjectCreated)
           DeleteBusinessObject();

        _businessObject = value;

        HasBusinessObjectChanged = true;
        HasBusinessObjectCreated = false;
      }
    }

    /// <summary> 
    ///   Gets the <see cref="IBusinessObjectReferenceProperty.ReferenceClass"/> of the <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <value> 
    ///   An <see cref="IBusinessObjectClass"/> or <see langword="null"/> if no <see cref="ReferenceProperty"/> is set.
    /// </value>
    public override IBusinessObjectClass BusinessObjectClass
    {
      get
      {
        IBusinessObjectReferenceProperty property = ReferenceProperty;
        return (property == null) ? null : property.ReferenceClass;
      }
    }

    private bool IsReadOnlyInDomainModel
    {
      get
      {
        var objectDataSource = ReferencedDataSource;
        var referenceProperty = ReferenceProperty;
        Assertion.IsNotNull (objectDataSource, "ReferencedDataSource is null.");
        Assertion.IsNotNull (referenceProperty, "ReferenceProperty is null.");

        return referenceProperty.IsReadOnly (objectDataSource.BusinessObject);
      }
    }

    private bool HasValidBinding
    {
      get { return ReferencedDataSource != null && ReferenceProperty != null; }
    }

    private bool RequiresWriteBack
    {
      get { return (HasBusinessObjectChanged || HasBusinessObjectCreated || ReferenceProperty.ReferenceClass.RequiresWriteBack); }
    }

    private bool SupportsDefaultValueSemantics
    {
      get { return (Mode == DataSourceMode.Edit && ReferenceProperty.SupportsDefaultValue); }
    }

    private bool IsBusinessObjectSetToDefaultValue ()
    {
      if (HasValidBinding && BusinessObject != null && ReferenceProperty.SupportsDefaultValue)
      {
        if (GetBoundControlsWithValidBinding().Any (c => c.HasValue))
          return false;

        var properties = GetBoundControlsWithValidBinding().Select (c => c.Property).Distinct ().ToArray ();
        return ReferenceProperty.IsDefaultValue (ReferencedDataSource.BusinessObject, BusinessObject, properties);
      }
      else
      {
        return false;
      }
    }

    private void DeleteBusinessObject ()
    {
      Assertion.IsNotNull (BusinessObject, "The business object of this reference data source cannot be null when invoking DeleteBusinessObject().");
      if (ReferenceProperty.SupportsDelete)
        ReferenceProperty.Delete (ReferencedDataSource.BusinessObject, BusinessObject);
    }
#pragma warning restore 612,618
  }
}
