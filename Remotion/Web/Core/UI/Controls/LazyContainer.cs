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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  [PersistChildren(true)]
  [ParseChildren(true, "RealControls")]
  public class LazyContainer : Control, INamingContainer, IControl
  {
    // types

    // static members and constants

    // member fields

    private bool _isEnsured;
    private EmptyControlCollection? _emptyControlCollection;
    private PlaceHolder? _placeHolder;
    private IDictionary? _childControlStatesBackUp;
    private bool _hasControlStateLoaded;
    private object? _recursiveViewState;
    private bool _isSavingViewStateRecursive;
    private bool _isLoadingViewStateRecursive;
    private bool _isLazyLoadingEnabled = true;

    // construction and disposing

    public LazyContainer ()
    {
    }

    // methods and properties

    public void Ensure ()
    {
      if (_isEnsured)
        return;

      _isEnsured = true;

      if (_isLazyLoadingEnabled)
      {
        if (!_hasControlStateLoaded && Page != null && Page.IsPostBack)
          throw new InvalidOperationException(string.Format("Cannot ensure LazyContainer '{0}' before its state has been loaded.", ID));

        RestoreChildControlState();
      }

      EnsurePlaceHolderCreated();
      Controls.Add(_placeHolder);
    }

    public bool IsLazyLoadingEnabled
    {
      get { return _isLazyLoadingEnabled; }
      set { _isLazyLoadingEnabled = value; }
    }

    public new IPage? Page
    {
      get { return PageWrapper.CastOrCreate(base.Page); }
    }

    public override ControlCollection Controls
    {
      get
      {
        EnsureChildControls();

        if (_isEnsured)
        {
          return base.Controls;
        }
        else
        {
          if (_emptyControlCollection == null)
            _emptyControlCollection = new EmptyControlCollection(this);
          return _emptyControlCollection;
        }
      }
    }

    [Browsable(false)]
    public ControlCollection RealControls
    {
      get
      {
        EnsureChildControls();

        EnsurePlaceHolderCreated();
        return _placeHolder.Controls;
      }
    }

    [MemberNotNull(nameof(_placeHolder))]
    private void EnsurePlaceHolderCreated ()
    {
      if (_placeHolder == null)
        _placeHolder = new PlaceHolder();
    }

    protected override void CreateChildControls ()
    {
      if (! _isLazyLoadingEnabled)
        Ensure();
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      EnsureChildControls();

      Page!.RegisterRequiresControlState(this);
    }

    protected override void LoadViewState (object? savedState)
    {
      if (_isLoadingViewStateRecursive)
        return;

      if (savedState != null)
      {
        Pair values = (Pair)savedState;
        base.LoadViewState(values.First);
        _recursiveViewState = values.Second;

        if (_isLazyLoadingEnabled)
        {
          _isLoadingViewStateRecursive = true;
          MemberCaller.LoadViewStateRecursive(this, _recursiveViewState!);
          _isLoadingViewStateRecursive = false;
        }
      }
    }

    protected override object? SaveViewState ()
    {
      if (_isSavingViewStateRecursive)
        return null;

      if (_isLazyLoadingEnabled && _isEnsured)
      {
        _isSavingViewStateRecursive = true;
        _recursiveViewState = MemberCaller.SaveViewStateRecursive(this);
        _isSavingViewStateRecursive = false;
      }

      Pair values = new Pair();
      values.First = base.SaveViewState();
      values.Second = _recursiveViewState;

      return values;
    }

    protected override void LoadControlState (object? savedState)
    {
      Triplet values = ArgumentUtility.CheckNotNullAndType<Triplet>("savedState", savedState!);

      base.LoadControlState(savedState);
      bool hasChildControlStatesBackUp = (bool)values.Second!;

      if (hasChildControlStatesBackUp)
      {
        Assertion.DebugIsNotNull(values.Third, "values.Third must not be null if hasChildControlStatesBackUp is true.");
        _childControlStatesBackUp = (IDictionary)values.Third;
      }
      else if (_isLazyLoadingEnabled)
      {
        BackUpChildControlState();
      }

      _hasControlStateLoaded = true;
    }

    private void RestoreChildControlState ()
    {
      MemberCaller.SetChildControlState(this, _childControlStatesBackUp);

      _childControlStatesBackUp = null;
    }

    private void BackUpChildControlState ()
    {
      _childControlStatesBackUp = MemberCaller.GetChildControlState(this);
    }

    protected override object SaveControlState ()
    {
      bool hasChildControlStatesBackUp = _isLazyLoadingEnabled && !_isEnsured;

      Triplet values = new Triplet();
      values.First = base.SaveControlState();
      values.Second = hasChildControlStatesBackUp;
      if (hasChildControlStatesBackUp)
        values.Third = _childControlStatesBackUp;
      else
        values.Third = null;

      return values;
    }

    protected virtual IServiceLocator ServiceLocator
    {
      get { return SafeServiceLocator.Current; }
    }

    private IInternalControlMemberCaller MemberCaller
    {
      get { return ServiceLocator.GetInstance<IInternalControlMemberCaller>(); }
    }
  }
}
