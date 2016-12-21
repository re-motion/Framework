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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;
using File = System.IO.File;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ReflectionUtilityTest : StandardMappingTest
  {
    private ClassDefinition _classDefinitionWithMixedProperty;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _classDefinitionWithMixedProperty = 
          ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (ClassWithMixedProperty), typeof (MixinAddingProperty));
    }

    [Test]
    [Obsolete]
    public void GetPropertyName ()
    {
      PropertyInfo propertyInfo = typeof (DerivedClassWithDifferentProperties).GetProperty ("Int32");

      Assert.That (
          ReflectionUtility.GetPropertyName (propertyInfo),
          Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithDifferentProperties.Int32"));
    }

    [Test]
    public void GetAssemblyPath ()
    {
      Assert.That (
          ReflectionUtility.GetAssemblyDirectory (typeof (ReflectionUtilityTest).Assembly),
          Is.EqualTo (AppDomain.CurrentDomain.BaseDirectory.TrimEnd ('\\')));
    }

    [Test]
    public void GetAssemblyPath_WithHashInDirectoryName ()
    {
      string directoryPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "#HashTestPath");
      string originalAssemblyPath = typeof (ReflectionUtilityTest).Assembly.Location;
      string newAssemblyPath = Path.Combine (directoryPath, Path.GetFileName (originalAssemblyPath));

      if (Directory.Exists (directoryPath))
        Directory.Delete (directoryPath, true);

      Directory.CreateDirectory (directoryPath);
      try
      {
        File.Copy (originalAssemblyPath, newAssemblyPath);
        AppDomainRunner.Run (
            delegate (object[] args)
            {
              string directory = (string) args[0];
              string assemblyPath = (string) args[1];

              Assembly assembly = Assembly.LoadFile (assemblyPath);
              Assert.That (Path.GetDirectoryName (assembly.Location), Is.EqualTo (directory));
              Assert.That (ReflectionUtility.GetAssemblyDirectory (assembly), Is.EqualTo (directory));
            },
            directoryPath,
            newAssemblyPath);
      }
      finally
      {
        Directory.Delete (directoryPath, true);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The assembly's code base 'http://server/File.ext' is not a local path.")]
    public void GetAssemblyPath_FromNonLocalUri ()
    {
      MockRepository mockRepository = new MockRepository();
      _Assembly assemblyMock = mockRepository.StrictMock<_Assembly>();

      SetupResult.For (assemblyMock.EscapedCodeBase).Return ("http://server/File.ext");
      mockRepository.ReplayAll();

      ReflectionUtility.GetAssemblyDirectory (assemblyMock);
    }

    [Test]
    public void GetDomainObjectAssemblyDirectory ()
    {
      Assert.That (
          ReflectionUtility.GetConfigFileDirectory(),
          Is.EqualTo (Path.GetDirectoryName (new Uri (typeof (DomainObject).Assembly.EscapedCodeBase).AbsolutePath)));
    }

    [Test]
    public void IsInheritanceRoot_BaseTypeIsDomainObject ()
    {
      var type = typeof (AbstractClass);
      Assertion.IsFalse (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type));
      Assertion.IsTrue (type.BaseType == typeof (DomainObject));

      Assert.That (ReflectionUtility.IsInheritanceRoot (type), Is.True);
    }

    [Test]
    public void IsInheritanceRoot_TypeHasStorageGroupAttributeApplied_ReturnsTrue ()
    {
      var type = typeof (ClassWithStorageGroupAttributeAndBaseClass);
      Assertion.IsFalse (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type));
      Assertion.IsFalse (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type.BaseType));
      Assertion.IsTrue (type.BaseType.BaseType == typeof (DomainObject));

      Assert.That (ReflectionUtility.IsInheritanceRoot (type), Is.True);
    }

    [Test]
    public void IsInheritanceRoot_TypeHasStorageGroupAttributeAppliedAndIsIgnoredForMappingConfiguration_ReturnsTrue ()
    {
      var type = typeof (ClassWithStorageGroupAttributeAndIgnoredForMappingConfigurationAttributeAndBaseClass);
      Assertion.IsTrue (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type));
      Assertion.IsFalse (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type.BaseType));
      Assertion.IsTrue (type.BaseType.BaseType == typeof (DomainObject));

      Assert.That (ReflectionUtility.IsInheritanceRoot (type), Is.False);
    }

    [Test]
    public void IsInheritanceRoot_TypeIsDomainObject_ReturnsFalse ()
    {
      Assert.That (ReflectionUtility.IsInheritanceRoot (typeof (DomainObject)), Is.False);
    }

    [Test]
    public void IsInheritanceRoot_OnlyImmediateBaseTypeIsIgnoredForMappingConfiguration_ReturnsFalse ()
    {
      var type = typeof (DerivedClassInMappingDerivedFromClassNotInMapping);
      Assertion.IsFalse (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type));
      Assertion.IsTrue (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type.BaseType));
      Assertion.IsFalse (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type.BaseType.BaseType));
      Assertion.IsTrue (type.BaseType.BaseType.BaseType == typeof (DomainObject));

      Assert.That (ReflectionUtility.IsInheritanceRoot (type), Is.False);
    }

    [Test]
    public void IsInheritanceRoot_TypeIsIgnoredForMappingConfiguration_ReturnsFalse ()
    {
      var type = typeof (ClassWithIgnoreForMappingConfigurationAttribute);
      Assertion.IsTrue (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type));
      Assertion.IsTrue (type.BaseType == typeof (DomainObject));

      Assert.That (ReflectionUtility.IsInheritanceRoot (typeof (ClassWithIgnoreForMappingConfigurationAttribute)), Is.False);
    }

    [Test]
    public void IsInheritanceRoot_AllBaseTypesAreIgnoredForMappingConfiguration_ReturnsTrue ()
    {
      var type = typeof (ClassWithAllBaseTypesWithIgnoreForMappingConfigurationAttribute);
      Assertion.IsFalse (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type));
      Assertion.IsTrue (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type.BaseType));
      Assertion.IsTrue (ReflectionUtility.IsTypeIgnoredForMappingConfiguration (type.BaseType.BaseType));
      Assertion.IsTrue (type.BaseType.BaseType.BaseType == typeof (DomainObject));

      Assert.That (ReflectionUtility.IsInheritanceRoot (typeof (ClassWithAllBaseTypesWithIgnoreForMappingConfigurationAttribute)), Is.True);
    }

    [Test]
    public void IsRelationProperty_DomainObjectIsNotAssignableFromPropertyType ()
    {
      Assert.That (ReflectionUtility.IsDomainObject (typeof (string)), Is.False);
    }

    [Test]
    public void IsRelationProperty_DomainObjectIsAssignableFromPropertyType ()
    {
      Assert.That (ReflectionUtility.IsDomainObject (typeof (AbstractClass)), Is.True);
    }

    [Test]
    public void GetDeclaringDomainObjectTypeForProperty_NoMixedProperty ()
    {
      var property = PropertyInfoAdapter.Create(typeof (ClassWithMixedProperty).GetProperty ("PublicNonMixedProperty"));

      var result = ReflectionUtility.GetDeclaringDomainObjectTypeForProperty (property, _classDefinitionWithMixedProperty);

      Assert.That (result, Is.SameAs (typeof (ClassWithMixedProperty)));
    }

    [Test]
    public void GetDeclaringDomainObjectTypeForProperty_MixedProperty ()
    {
      var property = PropertyInfoAdapter.Create(typeof (MixinAddingProperty).GetProperty ("MixedProperty"));

      var result = ReflectionUtility.GetDeclaringDomainObjectTypeForProperty (property, _classDefinitionWithMixedProperty);

      Assert.That (result, Is.SameAs (typeof (ClassWithMixedProperty)));
    }

    [Test]
    public void IsMixedProperty_NoMixedProperty_ReturnsFalse ()
    {
      var property = PropertyInfoAdapter.Create(typeof (ClassWithMixedProperty).GetProperty ("PublicNonMixedProperty"));

      var result = ReflectionUtility.IsMixedProperty (property, _classDefinitionWithMixedProperty);

      Assert.That (result, Is.False);
    }

    [Test]
    public void IsMixedProperty_MixedProperty_ReturnsTrue ()
    {
      var property = PropertyInfoAdapter.Create(typeof (MixinAddingProperty).GetProperty ("MixedProperty"));

      var result = ReflectionUtility.IsMixedProperty (property, _classDefinitionWithMixedProperty);

      Assert.That (result, Is.True);
    }
  }
}