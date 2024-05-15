using System;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping;

public abstract class PropertyDefinitionBase
{
  private IStoragePropertyDefinition? _storagePropertyDefinition;

  protected PropertyDefinitionBase (string propertyName, IPropertyInformation propertyInfo, Type propertyType, bool isNullable, int? maxLength)
  {
    ArgumentUtility.CheckNotNullOrEmpty(nameof(propertyName), propertyName);
    ArgumentUtility.CheckNotNull(nameof(propertyInfo), propertyInfo);
    ArgumentUtility.CheckNotNull(nameof(propertyType), propertyType);

    PropertyName = propertyName;
    PropertyInfo = propertyInfo;
    PropertyType = propertyType;
    IsNullablePropertyType = NullableTypeUtility.IsNullableType(propertyInfo.PropertyType);
    IsNullable = isNullable;
    MaxLength = maxLength;
  }

  public string PropertyName { get; }

  public bool HasStoragePropertyDefinitionBeenSet => _storagePropertyDefinition != null;

  public virtual IStoragePropertyDefinition StoragePropertyDefinition
  {
    get
    {
      Assertion.IsNotNull(_storagePropertyDefinition, GetStoragePropertyDefinitionIsNullErrorMessage());
      return _storagePropertyDefinition;
    }
  }

  public IPropertyInformation PropertyInfo { get; }

  public Type PropertyType { get; }

  /// <summary>
  /// Caches the information on whether the property's .NET type is nullable.
  /// </summary>
  internal bool IsNullablePropertyType { get; }

  /// <summary>
  /// Gets a flag describing whether the property's value is required for persistence.
  /// </summary>
  public bool IsNullable { get; }

  /// <summary>
  /// Gets the maximum length of the property's value when the value is persisted.
  /// </summary>
  public int? MaxLength { get; }

  public void SetStorageProperty (IStoragePropertyDefinition storagePropertyDefinition)
  {
    ArgumentUtility.CheckNotNull("storagePropertyDefinition", storagePropertyDefinition);

    _storagePropertyDefinition = storagePropertyDefinition;
  }

  public override string ToString ()
  {
    return GetType().GetFullNameSafe() + ": " + PropertyName;
  }

  protected virtual string GetStoragePropertyDefinitionIsNullErrorMessage ()
  {
    return $"StoragePropertyDefinition has not been set for property '{PropertyName}' of type '{GetTypeID()}'.";
  }

  protected abstract string GetTypeID ();
}
