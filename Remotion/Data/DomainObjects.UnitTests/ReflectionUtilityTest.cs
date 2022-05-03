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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Utilities;
using File = System.IO.File;
using Is = NUnit.Framework.Is;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ReflectionUtilityTest : StandardMappingTest
  {
    private interface ITestDomainObject : IDomainObject
    {
    }

    private interface IBaseInterfaceWithoutStorageGroupAttribute : IDomainObject
    {
    }

    [DBStorageGroup]
    private interface IInterfaceWithStorageGroupAttributeAndExtendedInterface : IBaseInterfaceWithoutStorageGroupAttribute
    {
    }

    public class DerivedDomainObject : AbstractClass, ITestDomainObject
    {
    }

    private TypeDefinition _typeDefinitionWithMixedProperty;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _typeDefinitionWithMixedProperty =
          TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(ClassWithMixedProperty), typeof(MixinAddingProperty));
    }

    [Test]
    public void GetAssemblyPath ()
    {
      Assert.That(
          ReflectionUtility.GetAssemblyDirectory(typeof(ReflectionUtilityTest).Assembly),
          Is.EqualTo(AppContext.BaseDirectory.TrimEnd('\\')));
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7799: Create out-of-process test infrastructure to replace tests done with app domains")]
#endif
    public void GetAssemblyPath_WithHashInDirectoryName ()
    {
#if NETFRAMEWORK
      string directoryPath = Path.Combine(AppContext.BaseDirectory, "#HashTestPath");
      string originalAssemblyPath = typeof(ReflectionUtilityTest).Assembly.Location;
      string newAssemblyPath = Path.Combine(directoryPath, Path.GetFileName(originalAssemblyPath));

      if (Directory.Exists(directoryPath))
        Directory.Delete(directoryPath, true);

      Directory.CreateDirectory(directoryPath);
      try
      {
        File.Copy(originalAssemblyPath, newAssemblyPath);
        AppDomainRunner.Run(
            delegate (object[] args)
            {
              string directory = (string)args[0];
              string assemblyPath = (string)args[1];

              Assembly assembly = Assembly.LoadFile(assemblyPath);
              Assert.That(Path.GetDirectoryName(assembly.Location), Is.EqualTo(directory));
              Assert.That(ReflectionUtility.GetAssemblyDirectory(assembly), Is.EqualTo(directory));
            },
            directoryPath,
            newAssemblyPath);
      }
      finally
      {
        Directory.Delete(directoryPath, true);
      }
#endif
    }

    [Test]
    public void GetAssemblyPath_FromNonPersistentAssembly ()
    {
      var assemblyMock = new Mock<FakeAssembly>(MockBehavior.Strict);
      AssemblyName fakeAssemblyName = new AssemblyName();
      fakeAssemblyName.Name = "FakeAssembly";

      assemblyMock.Setup(_ => _.GetName(false)).Returns(fakeAssemblyName);
      Assert.That(
          () => ReflectionUtility.GetAssemblyDirectory(assemblyMock.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The code base of assembly 'FakeAssembly' is not set."));
    }

    [Test]
    public void GetAssemblyPath_FromNonLocalUri ()
    {
      var assemblyMock = new Mock<FakeAssembly>(MockBehavior.Strict);
      AssemblyName fakeAssemblyName = new AssemblyName();
      fakeAssemblyName.Name = "FakeAssembly";
      fakeAssemblyName.CodeBase = "http://server/File.ext";

      assemblyMock.Setup(_ => _.GetName(false)).Returns(fakeAssemblyName);
      Assert.That(
          () => ReflectionUtility.GetAssemblyDirectory(assemblyMock.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The code base 'http://server/File.ext' of assembly 'FakeAssembly' is not a local path."));
    }

    [Test]
    public void GetDomainObjectAssemblyDirectory ()
    {
      Assert.That(
          ReflectionUtility.GetConfigFileDirectory(),
          Is.EqualTo(TestContext.CurrentContext.TestDirectory));
    }

    [Test]
    public void IsInheritanceRoot_WithClassAndBaseTypeIsDomainObject_ReturnsTrue ()
    {
      var type = typeof(AbstractClass);
      Assertion.IsFalse(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type));
      Assertion.IsTrue(type.BaseType == typeof(DomainObject));

      Assert.That(ReflectionUtility.IsInheritanceRoot(type), Is.True);
    }

    [Test]
    public void IsInheritanceRoot_WithClassDerivedFromInheritanceRootAndImplementedInterface_ReturnsTrue ()
    {
      var type = typeof(AbstractClass);
      Assertion.IsFalse(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type));
      Assertion.IsTrue(type.BaseType == typeof(DomainObject));

      Assert.That(ReflectionUtility.IsInheritanceRoot(type), Is.True);
    }

    [Test]
    public void IsInheritanceRoot_WithInterfaceAndExtendedInterfaceIsIDomainObject_ReturnsTrue ()
    {
      var type = typeof(ITestDomainObject);
      Assertion.IsFalse(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type));
      Assertion.IsTrue(type.GetInterfaces().Contains(typeof(IDomainObject)));

      Assert.That(ReflectionUtility.IsInheritanceRoot(type), Is.True);
    }

    [Test]
    public void IsInheritanceRoot_WithClassAndTypeHasStorageGroupAttributeApplied_ReturnsTrue ()
    {
      var type = typeof(ClassWithStorageGroupAttributeAndBaseClass);
      Assertion.IsFalse(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type));
      Assertion.IsFalse(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type.BaseType));
      Assertion.IsTrue(type.BaseType.BaseType == typeof(DomainObject));

      Assert.That(ReflectionUtility.IsInheritanceRoot(type), Is.True);
    }

    [Test]
    public void IsInheritanceRoot_WithInterfaceAndTypeHasStorageGroupAttributeApplied_ReturnsTrue ()
    {
      var type = typeof(IInterfaceWithStorageGroupAttributeAndExtendedInterface);
      Assertion.IsFalse(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type));
      Assertion.IsFalse(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(GetDirectlyExtendedInterfaces(type).Single()));
      Assertion.IsTrue(GetDirectlyExtendedInterfaces(GetDirectlyExtendedInterfaces(type).Single()).Contains(typeof(IDomainObject)));

      Assert.That(ReflectionUtility.IsInheritanceRoot(type), Is.True);
    }

    [Test]
    public void IsInheritanceRoot_WithClassThatHasStorageGroupAttributeAppliedAndIsIgnoredForMappingConfiguration_ReturnsFalse ()
    {
      var type = typeof(ClassWithStorageGroupAttributeAndIgnoredForMappingConfigurationAttributeAndBaseClass);
      Assertion.IsTrue(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type));
      Assertion.IsFalse(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type.BaseType));
      Assertion.IsTrue(type.BaseType.BaseType == typeof(DomainObject));

      Assert.That(ReflectionUtility.IsInheritanceRoot(type), Is.False);
    }

    [Test]
    public void IsInheritanceRoot_WithClassThatIsDomainObject_ReturnsFalse ()
    {
      Assert.That(ReflectionUtility.IsInheritanceRoot(typeof(DomainObject)), Is.False);
    }

    [Test]
    public void IsInheritanceRoot_WithClassAndOnlyImmediateBaseTypeIsIgnoredForMappingConfiguration_ReturnsFalse ()
    {
      var type = typeof(DerivedClassInMappingDerivedFromClassNotInMapping);
      Assertion.IsFalse(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type));
      Assertion.IsTrue(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type.BaseType));
      Assertion.IsFalse(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type.BaseType.BaseType));
      Assertion.IsTrue(type.BaseType.BaseType.BaseType == typeof(DomainObject));

      Assert.That(ReflectionUtility.IsInheritanceRoot(type), Is.False);
    }

    [Test]
    public void IsInheritanceRoot_WithClassThatIsIgnoredForMappingConfiguration_ReturnsFalse ()
    {
      var type = typeof(ClassWithIgnoreForMappingConfigurationAttribute);
      Assertion.IsTrue(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type));
      Assertion.IsTrue(type.BaseType == typeof(DomainObject));

      Assert.That(ReflectionUtility.IsInheritanceRoot(typeof(ClassWithIgnoreForMappingConfigurationAttribute)), Is.False);
    }

    [Test]
    public void IsInheritanceRoot_WithClassAndAllBaseTypesAreIgnoredForMappingConfiguration_ReturnsTrue ()
    {
      var type = typeof(ClassWithAllBaseTypesWithIgnoreForMappingConfigurationAttribute);
      Assertion.IsFalse(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type));
      Assertion.IsTrue(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type.BaseType));
      Assertion.IsTrue(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type.BaseType.BaseType));
      Assertion.IsTrue(type.BaseType.BaseType.BaseType == typeof(DomainObject));

      Assert.That(ReflectionUtility.IsInheritanceRoot(typeof(ClassWithAllBaseTypesWithIgnoreForMappingConfigurationAttribute)), Is.True);
    }

    [Test]
    public void IsTypeIgnroedOfrMappingConfiguration_SupportsIDomainObjectInterfaces ()
    {
      var type = typeof(ITestDomainObject);

      Assert.That(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(type), Is.False);
    }

    [Test]
    public void IsRelationProperty_DomainObjectIsNotAssignableFromPropertyType ()
    {
      Assert.That(ReflectionUtility.IsDomainObject(typeof(string)), Is.False);
    }

    [Test]
    public void IsRelationProperty_DomainObjectIsAssignableFromPropertyType ()
    {
      Assert.That(ReflectionUtility.IsDomainObject(typeof(AbstractClass)), Is.True);
    }

    [Test]
    public void GetDeclaringDomainObjectTypeForProperty_NoMixedProperty ()
    {
      var property = PropertyInfoAdapter.Create(typeof(ClassWithMixedProperty).GetProperty("PublicNonMixedProperty"));

      var result = ReflectionUtility.GetDeclaringDomainObjectTypeForProperty(property, _typeDefinitionWithMixedProperty);

      Assert.That(result, Is.SameAs(typeof(ClassWithMixedProperty)));
    }

    [Test]
    public void GetDeclaringDomainObjectTypeForProperty_MixedProperty ()
    {
      var property = PropertyInfoAdapter.Create(typeof(MixinAddingProperty).GetProperty("MixedProperty"));

      var result = ReflectionUtility.GetDeclaringDomainObjectTypeForProperty(property, _typeDefinitionWithMixedProperty);

      Assert.That(result, Is.SameAs(typeof(ClassWithMixedProperty)));
    }

    [Test]
    public void IsMixedProperty_NoMixedProperty_ReturnsFalse ()
    {
      var property = PropertyInfoAdapter.Create(typeof(ClassWithMixedProperty).GetProperty("PublicNonMixedProperty"));

      var result = ReflectionUtility.IsMixedProperty(property, _typeDefinitionWithMixedProperty);

      Assert.That(result, Is.False);
    }

    [Test]
    public void IsMixedProperty_MixedProperty_ReturnsTrue ()
    {
      var property = PropertyInfoAdapter.Create(typeof(MixinAddingProperty).GetProperty("MixedProperty"));

      var result = ReflectionUtility.IsMixedProperty(property, _typeDefinitionWithMixedProperty);

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsRelationType_IObjectList_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsRelationType(typeof(IObjectList<DomainObject>)), Is.True);
    }

    [Test]
    public void IsRelationType_ObjectList_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsRelationType(typeof(ObjectList<DomainObject>)), Is.True);
    }

    [Test]
    public void IsRelationType_DomainObject_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsRelationType(typeof(DomainObject)), Is.True);
    }

    [Test]
    public void IsIObjectList_IObjectListClosedWithDomainObject_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsIObjectList(typeof(IObjectList<DomainObject>)), Is.True);
    }

    [Test]
    public void IsIObjectList_IObjectListClosedWithIDomainObject_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsIObjectList(typeof(IObjectList<IDomainObject>)), Is.True);
    }

    [Test]
    public void IsIObjectList_IObjectListOpen_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsIObjectList(typeof(IObjectList<>)), Is.True);
    }

    [Test]
    public void IsIObjectList_ObjectList_ReturnsFalse ()
    {
      Assert.That(ReflectionUtility.IsIObjectList(typeof(ObjectList<DomainObject>)), Is.False);
    }

    [Test]
    public void IsIObjectList_DerivedFromIObjectList_ReturnsFalse ()
    {
      Assert.That(ReflectionUtility.IsIObjectList(typeof(IDerivedObjectList<DomainObject>)), Is.False);
    }

    [Test]
    public void IsIObjectList_DomainObject_ReturnsFalse ()
    {
      Assert.That(ReflectionUtility.IsIObjectList(typeof(DomainObject)), Is.False);
    }

    [Test]
    public void IsObjectList_ObjectListClosedWithDomainObject_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsObjectList(typeof(ObjectList<DomainObject>)), Is.True);
    }

    [Test]
    public void IsObjectList_DerivedFromObjectList_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsObjectList(typeof(OrderCollection)), Is.True);
    }

    [Test]
    public void IsObjectList_ObjectListOpen_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsObjectList(typeof(ObjectList<>)), Is.True);
    }

    [Test]
    public void IsObjectList_DomainObjectCollection_ReturnsFalse ()
    {
      Assert.That(ReflectionUtility.IsObjectList(typeof(DomainObjectCollection)), Is.False);
    }

    [Test]
    public void IsObjectList_IObjectList_ReturnsFalse ()
    {
      Assert.That(ReflectionUtility.IsObjectList(typeof(IObjectList<DomainObject>)), Is.False);
    }

    [Test]
    public void IsObjectList_DomainObject_ReturnsFalse ()
    {
      Assert.That(ReflectionUtility.IsObjectList(typeof(DomainObject)), Is.False);
    }

    [Test]
    public void IsDomainObject_DomainObject_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsDomainObject(typeof(DomainObject)), Is.True);
    }

    [Test]
    public void IsDomainObject_DerivedFromDomainObject_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsDomainObject(typeof(Order)), Is.True);
    }

    [Test]
    public void IsDomainObject_IDomainObject_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsDomainObject(typeof(IDomainObject)), Is.True);
    }

    [Test]
    public void IsDomainObject_ObjectList_ReturnsFalse ()
    {
      Assert.That(ReflectionUtility.IsDomainObject(typeof(ObjectList<DomainObject>)), Is.False);
    }

    [Test]
    public void IsDomainObject_Object_ReturnsFalse ()
    {
      Assert.That(ReflectionUtility.IsDomainObject(typeof(object)), Is.False);
    }

    [Test]
    public void IsDomainObject_NonIDomainObjectInterface_ReturnsFalse ()
    {
      Assert.That(ReflectionUtility.IsDomainObject(typeof(IDisposable)), Is.False);
    }

    [Test]
    public void GetRelatedObjectTypeFromRelationProperty_ObjectList_ReturnsTypeParameterValue ()
    {
      var propertyInfoStub = new Mock<IPropertyInformation>();
      propertyInfoStub.Setup(_ => _.PropertyType).Returns(typeof(ObjectList<Order>));

      Assert.That(ReflectionUtility.GetRelatedObjectTypeFromRelationProperty(propertyInfoStub.Object), Is.EqualTo(typeof(Order)));
    }

    [Test]
    public void GetRelatedObjectTypeFromRelationProperty_ObjectList_OpenGenericParameter_ReturnsNull ()
    {
      var property = PropertyInfoAdapter.Create(typeof(TestObject<>).GetProperty("ObjectListProperty"));

      Assert.That(ReflectionUtility.GetRelatedObjectTypeFromRelationProperty(property), Is.Null);
    }

    [Test]
    public void GetRelatedObjectTypeFromRelationProperty_IObjectList_ReturnsTypeParameterValue ()
    {
      var propertyInfoStub = new Mock<IPropertyInformation>();
      propertyInfoStub.Setup(_ => _.PropertyType).Returns(typeof(IObjectList<Order>));

      Assert.That(ReflectionUtility.GetRelatedObjectTypeFromRelationProperty(propertyInfoStub.Object), Is.EqualTo(typeof(Order)));
    }

    [Test]
    public void GetRelatedObjectTypeFromRelationProperty_IObjectList_OpenGenericParameter_ReturnsNull ()
    {
      var property = PropertyInfoAdapter.Create(typeof(TestObject<>).GetProperty("IObjectListProperty"));

      Assert.That(ReflectionUtility.GetRelatedObjectTypeFromRelationProperty(property), Is.Null);
    }

    [Test]
    public void GetRelatedObjectTypeFromRelationProperty_IDomainObject_ReturnsPropertyType ()
    {
      var propertyInfoStub = new Mock<IPropertyInformation>();
      propertyInfoStub.Setup(_ => _.PropertyType).Returns(typeof(IDomainObject));

      Assert.That(ReflectionUtility.GetRelatedObjectTypeFromRelationProperty(propertyInfoStub.Object), Is.EqualTo(typeof(IDomainObject)));
    }

    [Test]
    public void GetRelatedObjectTypeFromRelationProperty_DomainObject_ReturnsPropertyType ()
    {
      var propertyInfoStub = new Mock<IPropertyInformation>();
      propertyInfoStub.Setup(_ => _.PropertyType).Returns(typeof(DomainObject));

      Assert.That(ReflectionUtility.GetRelatedObjectTypeFromRelationProperty(propertyInfoStub.Object), Is.EqualTo(typeof(DomainObject)));
    }

    [Test]
    public void GetRelatedObjectTypeFromRelationProperty_Object_ReturnsPropertyType ()
    {
      var propertyInfoStub = new Mock<IPropertyInformation>();
      propertyInfoStub.Setup(_ => _.PropertyType).Returns(typeof(object));

      Assert.That(ReflectionUtility.GetRelatedObjectTypeFromRelationProperty(propertyInfoStub.Object), Is.EqualTo(typeof(object)));
    }

    [Test]
    public void GetRelatedObjectTypeFromRelationProperty_RelationProperty_OpenGenericParameter_ReturnsNull ()
    {
      var property = PropertyInfoAdapter.Create(typeof(TestObject<>).GetProperty("RelationProperty"));

      Assert.That(ReflectionUtility.GetRelatedObjectTypeFromRelationProperty(property), Is.Null);
    }

    [Test]
    public void GetObjectListTypeParameter_ObjectList_ReturnsTypeParameterValue ()
    {
      Assert.That(ReflectionUtility.GetObjectListTypeParameter(typeof(ObjectList<Person>)), Is.EqualTo(typeof(Person)));
    }

    [Test]
    public void GetObjectListTypeParameter_DerivedObjectList_ReturnsTypeParameterValue ()
    {
      Assert.That(ReflectionUtility.GetObjectListTypeParameter(typeof(OrderCollection)), Is.EqualTo(typeof(Order)));
    }

    [Test]
    public void GetObjectListTypeParameter_OpenGeneric_ReturnsNull ()
    {
      Assert.That(ReflectionUtility.GetObjectListTypeParameter(typeof(ObjectList<>)), Is.Null);
    }

    [Test]
    public void GetObjectListTypeParameter_OpenGenericParameter_ReturnsNull ()
    {
      var property = typeof(TestObject<>).GetProperty("ObjectListProperty");

      Assert.That(ReflectionUtility.GetObjectListTypeParameter(property.PropertyType), Is.Null);
    }

    [Test]
    public void GetObjectListTypeParameter_IObjectList_ThrowsArgumentException ()
    {
      Assert.That(
          () => ReflectionUtility.GetObjectListTypeParameter(typeof(IObjectList<>)),
          Throws.ArgumentException.With.Message.StartWith(
              "Parameter 'type' has type 'Remotion.Data.DomainObjects.IObjectList`1[TDomainObject]' when type 'Remotion.Data.DomainObjects.ObjectList`1[T]' was expected."));
    }

    [Test]
    public void GetObjectListTypeParameter_DomainObjectCollection_ThrowsArgumentException ()
    {
      Assert.That(
          () => ReflectionUtility.GetObjectListTypeParameter(typeof(DomainObjectCollection)),
          Throws.ArgumentException.With.Message.StartWith(
              "Parameter 'type' has type 'Remotion.Data.DomainObjects.DomainObjectCollection' when type 'Remotion.Data.DomainObjects.ObjectList`1[T]' was expected."));
    }

    [Test]
    public void GetObjectListTypeParameter_DomainObject_ThrowsArgumentException ()
    {
      Assert.That(
          () => ReflectionUtility.GetObjectListTypeParameter(typeof(DomainObject)),
          Throws.ArgumentException.With.Message.StartWith(
              "Parameter 'type' has type 'Remotion.Data.DomainObjects.DomainObject' when type 'Remotion.Data.DomainObjects.ObjectList`1[T]' was expected."));
    }

    [Test]
    public void GetObjectListTypeParameter_IListOfDomainObject_ThrowsArgumentException ()
    {
      Assert.That(
          () => ReflectionUtility.GetObjectListTypeParameter(typeof(List<DomainObject>)),
          Throws.ArgumentException.With.Message.StartWith(
              "Parameter 'type' has type 'System.Collections.Generic.List`1[Remotion.Data.DomainObjects.DomainObject]' when type 'Remotion.Data.DomainObjects.ObjectList`1[T]' was expected."));
    }

    [Test]
    public void GetIObjectListTypeParameter_ObjectList_ReturnsTypeParameterValue ()
    {
      Assert.That(ReflectionUtility.GetIObjectListTypeParameter(typeof(IObjectList<Person>)), Is.EqualTo(typeof(Person)));
    }

    [Test]
    public void GetIObjectListTypeParameter_DerivedObjectList_ThrowsArgumentException ()
    {
      Assert.That(
          () => ReflectionUtility.GetIObjectListTypeParameter(typeof(IDerivedObjectList<Order>)),
          Throws.ArgumentException.With.Message.StartWith(
              "Parameter 'type' has type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.IDerivedObjectList`1[Remotion.Data.DomainObjects.UnitTests.TestDomain.Order]' "
              + "when type 'Remotion.Data.DomainObjects.IObjectList`1[TDomainObject]' was expected."));
    }

    [Test]
    public void GetIObjectListTypeParameter_OpenGeneric_ReturnsNull ()
    {
      Assert.That(ReflectionUtility.GetIObjectListTypeParameter(typeof(IObjectList<>)), Is.Null);
    }

    [Test]
    public void GetIObjectListTypeParameter_OpenGenericParameter_ReturnsNull ()
    {
      var property = typeof(TestObject<>).GetProperty("IObjectListProperty");

      Assert.That(ReflectionUtility.GetIObjectListTypeParameter(property.PropertyType), Is.Null);
    }

    [Test]
    public void GetIObjectListTypeParameter_ObjectList_ThrowsArgumentException ()
    {
      Assert.That(
          () => ReflectionUtility.GetIObjectListTypeParameter(typeof(ObjectList<>)),
          Throws.ArgumentException.With.Message.StartWith(
              "Parameter 'type' has type 'Remotion.Data.DomainObjects.ObjectList`1[T]' when type 'Remotion.Data.DomainObjects.IObjectList`1[TDomainObject]' was expected."));
    }

    [Test]
    public void GetIObjectListTypeParameter_DomainObjectCollection_ThrowsArgumentException ()
    {
      Assert.That(
          () => ReflectionUtility.GetIObjectListTypeParameter(typeof(DomainObjectCollection)),
          Throws.ArgumentException.With.Message.StartWith(
              "Parameter 'type' has type 'Remotion.Data.DomainObjects.DomainObjectCollection' when type 'Remotion.Data.DomainObjects.IObjectList`1[TDomainObject]' was expected."));
    }

    [Test]
    public void GetIObjectListTypeParameter_DomainObject_ThrowsArgumentException ()
    {
      Assert.That(
          () => ReflectionUtility.GetIObjectListTypeParameter(typeof(DomainObject)),
          Throws.ArgumentException.With.Message.StartWith(
              "Parameter 'type' has type 'Remotion.Data.DomainObjects.DomainObject' when type 'Remotion.Data.DomainObjects.IObjectList`1[TDomainObject]' was expected."));
    }

    [Test]
    public void GetIObjectListTypeParameter_IListOfDomainObject_ThrowsArgumentException ()
    {
      Assert.That(
          () => ReflectionUtility.GetIObjectListTypeParameter(typeof(IList<DomainObject>)),
          Throws.ArgumentException.With.Message.StartWith(
              "Parameter 'type' has type 'System.Collections.Generic.IList`1[Remotion.Data.DomainObjects.DomainObject]' when type 'Remotion.Data.DomainObjects.IObjectList`1[TDomainObject]' was expected."));
    }

    private static IEnumerable<Type> GetDirectlyExtendedInterfaces (Type type)
    {
      var interfaces = type.GetInterfaces();
      return interfaces.Except(interfaces.SelectMany(e => e.GetInterfaces()));
    }

    /// <remarks>
    /// Castle does not support creating a proxy for <see cref="Assembly"/> directly ("The type System.Reflection.Assembly implements ISerializable,
    /// but failed to provide a deserialization constructor"), thus this type is required. <see cref="Assembly"/> defines the needed property
    /// <see cref="Assembly.GetName(bool)"/> as virtual, allowing the type to be
    /// mocked for our purpose.
    /// </remarks>
    public class FakeAssembly : Assembly
    {
    }

    private class TestObject<T>
        where T : DomainObject
    {
      public ObjectList<T> ObjectListProperty { get; }
      public IObjectList<T> IObjectListProperty { get; }
      public T RelationProperty { get; }
    }
  }
}
