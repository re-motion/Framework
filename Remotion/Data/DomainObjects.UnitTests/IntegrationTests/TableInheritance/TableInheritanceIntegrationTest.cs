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
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.TableInheritance
{
  [TestFixture]
  public class TableInheritanceIntegrationTest : TableInheritanceMappingTest
  {
    private ObjectID _rootFolderID;
    private ObjectID _folder1ID;
    private ObjectID _fileInRootFolderID;
    private ObjectID _fileInFolder1ID;

    private ObjectID _derivedClassWithEntity1ID;
    private ObjectID _derivedClassWithEntity2ID;
    private ObjectID _derivedClassWithEntity3ID;

    private ObjectID _derivedClassWithEntityFromBaseClass1ID;
    private ObjectID _derivedClassWithEntityFromBaseClass2ID;
    private ObjectID _derivedClassWithEntityFromBaseClass3ID;

    public override void SetUp ()
    {
      base.SetUp ();

      _rootFolderID = CreateFolderObjectID ("{1A45A89B-746E-4a9e-AC2C-E960E90C0DAD}");
      _folder1ID = CreateFolderObjectID ("{6B8A65C1-1D49-4dab-97D7-F466F3EAB91E}");
      _fileInRootFolderID = CreateFileObjectID ("{023392E2-AB99-434f-A71F-8A9865D10C8C}");
      _fileInFolder1ID = CreateFileObjectID ("{6108E150-6D3C-4e38-9865-895BD143D180}");

      _derivedClassWithEntity1ID = CreateDerivedClassWithEntityWithHierarchyObjectID ("{137DA04C-2B53-463e-A893-D8B246D6BFA9}");
      _derivedClassWithEntity2ID = CreateDerivedClassWithEntityWithHierarchyObjectID ("{6389C3AB-9E65-4bfb-9321-EC9F50B6A479}");
      _derivedClassWithEntity3ID = CreateDerivedClassWithEntityWithHierarchyObjectID ("{15526A7A-57EC-42c3-95A7-B40E46784846}");

      _derivedClassWithEntityFromBaseClass1ID = CreateDerivedClassWithEntityFromBaseClassWithHierarchyObjectID ("{24F27B35-68F8-4035-A454-33CFC1AF6339}");
      _derivedClassWithEntityFromBaseClass2ID = CreateDerivedClassWithEntityFromBaseClassWithHierarchyObjectID ("{9C730A8A-8F83-4b26-AF40-FB0C3D4DD387}");
      _derivedClassWithEntityFromBaseClass3ID = CreateDerivedClassWithEntityFromBaseClassWithHierarchyObjectID ("{953B2E51-C324-4f86-8FA0-3AFA2A2E4E72}");
    }

    [Test]
    public void LoadObjectsWithSamePropertyNameInDifferentInheritanceBranches ()
    {
      TIFolder rootFolder = _rootFolderID.GetObject<TIFolder> ();
      Assert.That (rootFolder.CreatedAt, Is.EqualTo (new DateTime (2006, 2, 1)));

      TIFile fileInRootFolder = _fileInRootFolderID.GetObject<TIFile> ();
      Assert.That (fileInRootFolder.CreatedAt, Is.EqualTo (new DateTime (2006, 2, 3)));
    }


    [Test]
    public void CompositePatternNavigateOneToMany ()
    {
      TIFolder rootFolder = _rootFolderID.GetObject<TIFolder> ();

      Assert.That (rootFolder.FileSystemItems.Count, Is.EqualTo (2));
      Assert.That (rootFolder.FileSystemItems[0].ID, Is.EqualTo (_fileInRootFolderID));
      Assert.That (rootFolder.FileSystemItems[1].ID, Is.EqualTo (_folder1ID));

      TIFolder folder1 = _folder1ID.GetObject<TIFolder> ();

      Assert.That (folder1.FileSystemItems.Count, Is.EqualTo (1));
      Assert.That (folder1.FileSystemItems[0].ID, Is.EqualTo (_fileInFolder1ID));
    }

    [Test]
    public void CompositePatternNavigateManyToOne ()
    {
      TIFolder folder1 = _folder1ID.GetObject<TIFolder> ();
      Assert.That (folder1.ParentFolder.ID, Is.EqualTo (_rootFolderID));

      TIFile fileInRootFolder = _fileInRootFolderID.GetObject<TIFile> ();
      Assert.That (fileInRootFolder.ParentFolder.ID, Is.EqualTo (_rootFolderID));

      TIFile fileInFolder1 = _fileInFolder1ID.GetObject<TIFile> ();
      Assert.That (fileInFolder1.ParentFolder.ID, Is.EqualTo (_folder1ID));
    }

    [Test]
    public void ObjectHierarchyNavigateOneToMany ()
    {
      DerivedClassWithEntityWithHierarchy derivedClassWithEntity1 = _derivedClassWithEntity1ID.GetObject<DerivedClassWithEntityWithHierarchy>();

      Assert.That (derivedClassWithEntity1.ChildAbstractBaseClassesWithHierarchy.Count, Is.EqualTo (3));
      Assert.That (derivedClassWithEntity1.ChildAbstractBaseClassesWithHierarchy[0].ID, Is.EqualTo (_derivedClassWithEntity3ID));
      Assert.That (derivedClassWithEntity1.ChildAbstractBaseClassesWithHierarchy[1].ID, Is.EqualTo (_derivedClassWithEntity2ID));
      Assert.That (derivedClassWithEntity1.ChildAbstractBaseClassesWithHierarchy[2].ID, Is.EqualTo (_derivedClassWithEntityFromBaseClass1ID));

      Assert.That (derivedClassWithEntity1.ChildDerivedClassesWithEntityWithHierarchy.Count, Is.EqualTo (3));
      Assert.That (derivedClassWithEntity1.ChildDerivedClassesWithEntityWithHierarchy[0].ID, Is.EqualTo (_derivedClassWithEntityFromBaseClass1ID));
      Assert.That (derivedClassWithEntity1.ChildDerivedClassesWithEntityWithHierarchy[1].ID, Is.EqualTo (_derivedClassWithEntity2ID));
      Assert.That (derivedClassWithEntity1.ChildDerivedClassesWithEntityWithHierarchy[2].ID, Is.EqualTo (_derivedClassWithEntity3ID));

      DerivedClassWithEntityWithHierarchy derivedClassWithEntity2 = _derivedClassWithEntity2ID.GetObject<DerivedClassWithEntityWithHierarchy>();
      DerivedClassWithEntityWithHierarchy derivedClassWithEntity3 = _derivedClassWithEntity3ID.GetObject<DerivedClassWithEntityWithHierarchy>();

      Assert.That (derivedClassWithEntity2.ChildAbstractBaseClassesWithHierarchy.Count, Is.EqualTo (1));
      Assert.That (derivedClassWithEntity2.ChildAbstractBaseClassesWithHierarchy[0].ID, Is.EqualTo (_derivedClassWithEntityFromBaseClass2ID));
      Assert.That (derivedClassWithEntity2.ChildDerivedClassesWithEntityWithHierarchy.Count, Is.EqualTo (1));
      Assert.That (derivedClassWithEntity2.ChildDerivedClassesWithEntityWithHierarchy[0].ID, Is.EqualTo (_derivedClassWithEntityFromBaseClass2ID));

      Assert.That (derivedClassWithEntity3.ChildAbstractBaseClassesWithHierarchy.Count, Is.EqualTo (1));
      Assert.That (derivedClassWithEntity3.ChildAbstractBaseClassesWithHierarchy[0].ID, Is.EqualTo (_derivedClassWithEntityFromBaseClass3ID));
      Assert.That (derivedClassWithEntity3.ChildDerivedClassesWithEntityWithHierarchy.Count, Is.EqualTo (1));
      Assert.That (derivedClassWithEntity3.ChildDerivedClassesWithEntityWithHierarchy[0].ID, Is.EqualTo (_derivedClassWithEntityFromBaseClass3ID));

      DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass1 =
          _derivedClassWithEntityFromBaseClass1ID.GetObject<DerivedClassWithEntityFromBaseClassWithHierarchy> ();

      Assert.That (derivedClassWithEntityFromBaseClass1.ChildAbstractBaseClassesWithHierarchy, Is.Empty);
      Assert.That (derivedClassWithEntityFromBaseClass1.ChildDerivedClassesWithEntityWithHierarchy, Is.Empty);
      Assert.That (derivedClassWithEntityFromBaseClass1.ChildDerivedClassesWithEntityFromBaseClassWithHierarchy.Count, Is.EqualTo (2));
      Assert.That (derivedClassWithEntityFromBaseClass1.ChildDerivedClassesWithEntityFromBaseClassWithHierarchy[0].ID, Is.EqualTo (_derivedClassWithEntityFromBaseClass2ID));
      Assert.That (derivedClassWithEntityFromBaseClass1.ChildDerivedClassesWithEntityFromBaseClassWithHierarchy[1].ID, Is.EqualTo (_derivedClassWithEntityFromBaseClass3ID));
    }

    [Test]
    public void ObjectHierarchyNavigateManyToOne ()
    {
      DerivedClassWithEntityWithHierarchy derivedClassWithEntity2 = _derivedClassWithEntity2ID.GetObject<DerivedClassWithEntityWithHierarchy>();

      Assert.That (derivedClassWithEntity2.ParentAbstractBaseClassWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntity1ID));
      Assert.That (derivedClassWithEntity2.ParentDerivedClassWithEntityWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntity1ID));

      DerivedClassWithEntityWithHierarchy derivedClassWithEntity3 = _derivedClassWithEntity3ID.GetObject<DerivedClassWithEntityWithHierarchy>();
      Assert.That (derivedClassWithEntity3.ParentAbstractBaseClassWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntity1ID));
      Assert.That (derivedClassWithEntity3.ParentDerivedClassWithEntityWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntity1ID));

      DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass1 =
          _derivedClassWithEntityFromBaseClass1ID.GetObject<DerivedClassWithEntityFromBaseClassWithHierarchy> ();

      Assert.That (derivedClassWithEntityFromBaseClass1.ParentAbstractBaseClassWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntity1ID));
      Assert.That (derivedClassWithEntityFromBaseClass1.ParentDerivedClassWithEntityWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntity1ID));

      DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass2 =
          _derivedClassWithEntityFromBaseClass2ID.GetObject<DerivedClassWithEntityFromBaseClassWithHierarchy> ();

      Assert.That (derivedClassWithEntityFromBaseClass2.ParentAbstractBaseClassWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntity2ID));
      Assert.That (derivedClassWithEntityFromBaseClass2.ParentDerivedClassWithEntityWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntity2ID));
      Assert.That (derivedClassWithEntityFromBaseClass2.ParentDerivedClassWithEntityFromBaseClassWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntityFromBaseClass1ID));

      DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass3 =
          _derivedClassWithEntityFromBaseClass3ID.GetObject<DerivedClassWithEntityFromBaseClassWithHierarchy> ();

      Assert.That (derivedClassWithEntityFromBaseClass3.ParentAbstractBaseClassWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntity3ID));
      Assert.That (derivedClassWithEntityFromBaseClass3.ParentDerivedClassWithEntityWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntity3ID));
      Assert.That (derivedClassWithEntityFromBaseClass3.ParentDerivedClassWithEntityFromBaseClassWithHierarchy.ID, Is.EqualTo (_derivedClassWithEntityFromBaseClass1ID));
    }

    [Test]
    public void UnidirectionalRelationToClassWithoutDerivation ()
    {
      ObjectID client2 = CreateObjectID (typeof (TIClient), "{58535280-84EC-41d9-9F8F-BCAC64BB3709}");

      DerivedClassWithEntityWithHierarchy derivedClassWithEntity1 = _derivedClassWithEntity1ID.GetObject<DerivedClassWithEntityWithHierarchy>();
      Assert.That (derivedClassWithEntity1.ClientFromAbstractBaseClass.ID, Is.EqualTo (DomainObjectIDs.Client));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        derivedClassWithEntity1 = _derivedClassWithEntity1ID.GetObject<DerivedClassWithEntityWithHierarchy>();
        Assert.That (derivedClassWithEntity1.ClientFromDerivedClassWithEntity.ID, Is.EqualTo (client2));
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass1 =
            _derivedClassWithEntityFromBaseClass1ID.GetObject<DerivedClassWithEntityFromBaseClassWithHierarchy> ();
        Assert.That (derivedClassWithEntityFromBaseClass1.ClientFromDerivedClassWithEntityFromBaseClass.ID, Is.EqualTo (DomainObjectIDs.Client));
      }
    }

    [Test]
    public void UnidirectionalRelationToAbstractClass ()
    {
      DerivedClassWithEntityWithHierarchy derivedClassWithEntity1 = _derivedClassWithEntity1ID.GetObject<DerivedClassWithEntityWithHierarchy>();
      Assert.That (derivedClassWithEntity1.FileSystemItemFromAbstractBaseClass.ID, Is.EqualTo (_rootFolderID));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        derivedClassWithEntity1 = _derivedClassWithEntity1ID.GetObject<DerivedClassWithEntityWithHierarchy>();
        Assert.That (derivedClassWithEntity1.FileSystemItemFromDerivedClassWithEntity.ID, Is.EqualTo (_fileInRootFolderID));
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass1 =
            _derivedClassWithEntityFromBaseClass1ID.GetObject<DerivedClassWithEntityFromBaseClassWithHierarchy> ();

        Assert.That (derivedClassWithEntityFromBaseClass1.FileSystemItemFromDerivedClassWithEntityFromBaseClass.ID, Is.EqualTo (_fileInFolder1ID));
      }
    }

    private ObjectID CreateFolderObjectID (string guid)
    {
      return CreateObjectID (typeof (TIFolder), guid);
    }

    private ObjectID CreateFileObjectID (string guid)
    {
      return CreateObjectID (typeof (TIFile), guid);
    }

    private ObjectID CreateDerivedClassWithEntityWithHierarchyObjectID (string guid)
    {
      return CreateObjectID (typeof (DerivedClassWithEntityWithHierarchy), guid);
    }

    private ObjectID CreateDerivedClassWithEntityFromBaseClassWithHierarchyObjectID (string guid)
    {
      return CreateObjectID (typeof (DerivedClassWithEntityFromBaseClassWithHierarchy), guid);
    }

    private ObjectID CreateObjectID (Type classType, string guid)
    {
      return new ObjectID(classType, new Guid (guid));
    }
  }
}
