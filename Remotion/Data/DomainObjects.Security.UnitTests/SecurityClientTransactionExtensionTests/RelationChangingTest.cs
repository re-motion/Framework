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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Security.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Reflection;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.Security.UnitTests.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class RelationChangingTest : TestBase
  {
    private TestHelper _testHelper;
    private IClientTransactionExtension _extension;
    private PropertyInfo _propertyInfo;
    private IMethodInformation _setMethodInformation;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new TestHelper ();
      _extension = new SecurityClientTransactionExtension ();
      _propertyInfo = typeof (SecurableObject).GetProperty ("Parent");

      _setMethodInformation = MethodInfoAdapter.Create(_propertyInfo.GetSetMethod());
      
      _testHelper.SetupSecurityIoCConfiguration ();
    }

    public override void TearDown ()
    {
      _testHelper.TearDownSecurityIoCConfiguration ();

      base.TearDown();
    }

    [Test]
    public void Test_AccessGranted_DowsNotThrow ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      var endPointDefinition = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");

      _extension.RelationChanging (_testHelper.Transaction, securableObject, endPointDefinition, null, null);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_ThrowsPermissionDeniedException ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      var endPointDefinition = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");

      _extension.RelationChanging (_testHelper.Transaction, securableObject, endPointDefinition, null, null);
    }

    [Test]
    public void Test_AccessGranted_WithNonPublicAccessor_DowsNotThrow ()
    {
      var propertyInfo =
          typeof (SecurableObject).GetProperty ("NonPublicRelationPropertyWithCustomPermission", BindingFlags.NonPublic | BindingFlags.Instance);
      var setMethodInformation = MethodInfoAdapter.Create(propertyInfo.GetSetMethod (true));
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      var endPointDefinition = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".NonPublicRelationPropertyWithCustomPermission");

      _extension.RelationChanging (_testHelper.Transaction, securableObject, endPointDefinition, null, null);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_WithNonPublicAccessor_ThrowsPermissionDeniedException ()
    {
      var propertyInfo =
          typeof (SecurableObject).GetProperty ("NonPublicRelationPropertyWithCustomPermission", BindingFlags.NonPublic | BindingFlags.Instance);
      var setMethodInformation = MethodInfoAdapter.Create(propertyInfo.GetSetMethod (true));
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      var endPointDefinition = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".NonPublicRelationPropertyWithCustomPermission");

      _extension.RelationChanging (_testHelper.Transaction, securableObject, endPointDefinition, null, null);
    }

    [Test]
    public void Test_AccessGranted_WithMissingAccessor_DowsNotThrow ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (new NullMethodInformation());
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Edit, true);
      _testHelper.ReplayAll();

      var endPointDefinition = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".RelationPropertyWithMissingSetAccessor");

      _extension.RelationChanging (_testHelper.Transaction, securableObject, endPointDefinition, null, null);

      _testHelper.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_WithMissingAccessor_ThrowsPermissionDeniedException ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (new NullMethodInformation());
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Edit, false);
      _testHelper.ReplayAll();

      var endPointDefinition = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".RelationPropertyWithMissingSetAccessor");

      _extension.RelationChanging (_testHelper.Transaction, securableObject, endPointDefinition, null, null);
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection_DoesNotPerformSecurityCheck ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.ReplayAll ();
      var endPointDefinition = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");

      using (SecurityFreeSection.Activate())
      {
        _extension.RelationChanging (_testHelper.Transaction, securableObject, endPointDefinition, null, null);
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject_DoesNotPerformSecurityCheck ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      _testHelper.ReplayAll ();
      var endPointDefinition = nonSecurableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (NonSecurableObject).FullName + ".Parent");

      _extension.RelationChanging (_testHelper.Transaction, nonSecurableObject, endPointDefinition, null, null);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSide_RecursiveSecurity_ChecksAccessOnNestedCall ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject newObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.ExecuteInScope (() => securableObject.OtherParent = _testHelper.CreateSecurableObject ());
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);

      var endPointDefinition = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");

      HasAccessDelegate hasAccess = delegate
      {
        securableObject.OtherParent = newObject;
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      _extension.RelationChanging (_testHelper.Transaction, securableObject, endPointDefinition, null, null);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSideSetNull_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject newObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered ())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);

        var childrenSetMethodInformation = new NullMethodInformation ();
        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (childrenSetMethodInformation, GeneralAccessTypes.Edit);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (newObject, GeneralAccessTypes.Edit, true);
      }
      _testHelper.ReplayAll ();

      _testHelper.Transaction.ExecuteInScope (() => securableObject.Parent = newObject);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSideSetNewValue_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject oldObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.ExecuteInScope (() => securableObject.Parent = oldObject);
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered ())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);

        var childrenSetMethodInformation = new NullMethodInformation ();
        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (childrenSetMethodInformation, GeneralAccessTypes.Edit);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (oldObject, GeneralAccessTypes.Edit, true);
      }
      _testHelper.ReplayAll ();

      _testHelper.Transaction.ExecuteInScope (() => securableObject.Parent = null);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ManySideAdd_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject newObject = _testHelper.CreateSecurableObject ();
      var childrenPropertyInfo = typeof (SecurableObject).GetProperty ("Children");
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (MethodInfoAdapter.Create(childrenPropertyInfo.GetGetMethod()), TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);

        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.Second);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (newObject, TestAccessTypes.Second, true);

        var childrenSetMethodInformation = new NullMethodInformation();
        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (childrenSetMethodInformation, GeneralAccessTypes.Edit);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Edit, true);
      }
      _testHelper.ReplayAll ();

      _testHelper.Transaction.ExecuteInScope (() => securableObject.Children.Add (newObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ManySideRemove_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject oldObject = _testHelper.CreateSecurableObject ();
      var childrenPropertyInfo = typeof (SecurableObject).GetProperty ("Children");
      
      _testHelper.Transaction.ExecuteInScope (() => securableObject.Children.Add (oldObject));
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered ())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (MethodInfoAdapter.Create(childrenPropertyInfo.GetGetMethod()), TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);

        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.Second);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (oldObject, TestAccessTypes.Second, true);

        var childrenSetMethodInformation = new NullMethodInformation();
        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (childrenSetMethodInformation, GeneralAccessTypes.Edit);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Edit, true);
      }
      _testHelper.ReplayAll ();

      _testHelper.Transaction.ExecuteInScope (() => securableObject.Children.Remove (oldObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithActiveTransactionMatchingTransactionPassedAsArgument_DoesNotCreateScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      using (var scope = _testHelper.Transaction.EnterNonDiscardingScope())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccessWithMatchingScope (securableObject, scope);
        _testHelper.ReplayAll();

        var endPointDefinition = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");
        _extension.RelationChanging (_testHelper.Transaction, securableObject, endPointDefinition, null, null);
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithActiveTransactionNotMatchingTransactionPassedAsArgument_CreatesScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll();

      var endPointDefinition = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        _extension.RelationChanging (_testHelper.Transaction, securableObject, endPointDefinition, null, null);
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithInactiveTransaction_CreatesScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_setMethodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      var endPointDefinition = securableObject.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (SecurableObject).FullName + ".Parent");

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionTestHelper.MakeInactive (_testHelper.Transaction))
        {
          _extension.RelationChanging (_testHelper.Transaction, securableObject, endPointDefinition, null, null);
        }
      }

      _testHelper.VerifyAll ();
    }
  }
}
