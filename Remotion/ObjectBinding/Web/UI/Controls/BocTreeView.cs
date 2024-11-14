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
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Mixins;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls.BocTreeViewImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTreeViewImplementation.Rendering;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Services;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> Object bound tree view. </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocTreeView.xml' path='BocTreeView/Class/*' />
  [DefaultEvent("Click")]
  public class BocTreeView : BusinessObjectBoundWebControl, IBocRenderableControl, IBocTreeView
  {
    // constants

    // types

    // static members
    private static readonly Type[] s_supportedPropertyInterfaces = new Type[]
                                                                   {
                                                                       typeof(IBusinessObjectReferenceProperty)
                                                                   };

    private static readonly object s_clickEvent = new object();
    private static readonly object s_selectionChangedEvent = new object();

    // member fields
    private WebTreeView _treeView;

    /// <summary> The <see cref="IBusinessObject"/> displayed by the <see cref="BocTreeView"/>. </summary>
    private IReadOnlyList<IBusinessObjectWithIdentity>? _value = null;

    private bool _enableTreeNodeCaching = true;
    private Pair[]? _nodesControlState;
    private bool _isRebuildRequired = false;
    private string? _controlServicePath;
    private string? _controlServiceArguments;

    private readonly IRenderingFeatures _renderingFeatures;

    protected IWebServiceFactory WebServiceFactory { get; }

    // construction and destruction

    public BocTreeView ()
        : this(SafeServiceLocator.Current.GetInstance<IRenderingFeatures>(), SafeServiceLocator.Current.GetInstance<IWebServiceFactory>())
    {
    }

    protected BocTreeView ([NotNull] IRenderingFeatures renderingFeatures, [NotNull] IWebServiceFactory webServiceFactory)
    {
      ArgumentUtility.CheckNotNull("renderingFeatures", renderingFeatures);
      ArgumentUtility.CheckNotNull("webServiceFactory", webServiceFactory);

      _treeView = new WebTreeView(this);
      _renderingFeatures = renderingFeatures;
      WebServiceFactory = webServiceFactory;
    }

    // methods and properties

    protected override void CreateChildControls ()
    {
      _treeView.ID = ID + "_Boc_TreeView";
      _treeView.Click += new WebTreeNodeClickEventHandler(TreeView_Click);
      _treeView.SelectionChanged += new WebTreeNodeEventHandler(TreeView_SelectionChanged);
      _treeView.SetEvaluateTreeNodeDelegate(new EvaluateWebTreeNode(EvaluateTreeNode));
      _treeView.SetInitializeRootTreeNodesDelegate(new InitializeRootWebTreeNodes(InitializeRootWebTreeNodes));
      _treeView.SetTreeNodeMenuRenderMethodDelegate(RenderTreeNodeMenu);
      _treeView.EnableTreeNodeControlState = !_enableTreeNodeCaching;
      Controls.Add(_treeView);
    }

    /// <summary> Handles the tree view's <see cref="WebTreeView.Click"/> event. </summary>
    private void TreeView_Click (object sender, WebTreeNodeClickEventArgs e)
    {
      OnClick(e.Node, e.Path);
    }

    /// <summary> Fires the <see cref="Click"/> event. </summary>
    protected virtual void OnClick (WebTreeNode? node, string[] path)
    {
      BocTreeNodeClickEventHandler? handler = (BocTreeNodeClickEventHandler?)Events[s_clickEvent];
      if (handler != null)
      {
        ArgumentUtility.CheckNotNullAndType<BocTreeNode>("node", node!);
        BusinessObjectTreeNode? businessObjectNode = node as BusinessObjectTreeNode;
        BusinessObjectPropertyTreeNode? propertyNode = node as BusinessObjectPropertyTreeNode;

        BocTreeNodeClickEventArgs? e = null;
        if (businessObjectNode != null)
          e = new BocTreeNodeClickEventArgs(businessObjectNode, path);
        else if (propertyNode != null)
          e = new BocTreeNodeClickEventArgs(propertyNode, path);

        handler(this, e!);
      }
    }

    /// <summary> Handles the tree view's <see cref="WebTreeView.SelectionChanged"/> event. </summary>
    private void TreeView_SelectionChanged (object sender, WebTreeNodeEventArgs e)
    {
      OnSelectionChanged(e.Node);
    }

    /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
    protected virtual void OnSelectionChanged (WebTreeNode? node)
    {
      BocTreeNodeEventHandler? handler = (BocTreeNodeEventHandler?)Events[s_selectionChangedEvent];
      if (handler != null)
      {
        ArgumentUtility.CheckNotNullAndType<BocTreeNode>("node", node!);
        BusinessObjectTreeNode? businessObjectNode = node as BusinessObjectTreeNode;
        BusinessObjectPropertyTreeNode? propertyNode = node as BusinessObjectPropertyTreeNode;

        BocTreeNodeEventArgs? e = null;
        if (businessObjectNode != null)
          e = new BocTreeNodeEventArgs(businessObjectNode);
        else if (propertyNode != null)
          e = new BocTreeNodeEventArgs(propertyNode);

        handler(this, e!);
      }
    }

    IEnumerable<string> IControlWithLabel.GetLabelIDs ()
    {
      return GetLabelIDs();
    }

    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls();
      base.OnPreRender(e);
      _treeView.Width = Width;
      _treeView.Height = Height;

      var labelID = GetLabelIDs().FirstOrDefault();
      if (!string.IsNullOrEmpty(labelID))
        _treeView.AssignLabel(labelID);

      CheckControlService();
    }

    private void CheckControlService ()
    {
      if (string.IsNullOrEmpty(ControlServicePath))
        return;

      var virtualServicePath = VirtualPathUtility.GetVirtualPath(this, ControlServicePath);
      WebServiceFactory.CreateJsonService<IBocTreeViewWebService>(virtualServicePath);
    }

    protected override void Render (HtmlTextWriter writer)
    {

      base.Render(writer);
    }

    private void RenderTreeNodeMenu (HtmlTextWriter writer, WebTreeNode node, DropDownMenu menu)
    {
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("node", node);
      ArgumentUtility.CheckNotNull("menu", menu);

      if (!string.IsNullOrEmpty(ControlServicePath))
      {
        var businessObjectWebServiceContext = CreateBusinessObjectWebServiceContext();
        var nodeBusinessObjectProperty = ((node as BusinessObjectTreeNode)?.Property ?? (node as BusinessObjectPropertyTreeNode)?.Property)?.Identifier;
        var nodeBusinessObjectUniqueIdentifier = (node as BusinessObjectTreeNode)?.BusinessObject?.UniqueIdentifier;

        var stringValueParametersDictionary = new Dictionary<string, string?>();
        stringValueParametersDictionary.Add("controlID", ID);
        stringValueParametersDictionary.Add(
            "controlType",
            TypeUtility.GetPartialAssemblyQualifiedName(MixinTypeUtility.GetUnderlyingTargetType(GetType())));
        stringValueParametersDictionary.Add("businessObjectClass", businessObjectWebServiceContext.BusinessObjectClass);
        stringValueParametersDictionary.Add("businessObjectProperty", businessObjectWebServiceContext.BusinessObjectProperty);
        stringValueParametersDictionary.Add("businessObject", businessObjectWebServiceContext.BusinessObjectIdentifier);
        stringValueParametersDictionary.Add("nodeID", node.ItemID);
        stringValueParametersDictionary.Add("nodePath", _treeView.FormatNodePath(node));
        stringValueParametersDictionary.Add("nodeBusinessObjectProperty", nodeBusinessObjectProperty);
        stringValueParametersDictionary.Add("nodeBusinessObject", nodeBusinessObjectUniqueIdentifier);
        stringValueParametersDictionary.Add("arguments", businessObjectWebServiceContext.Arguments);

        menu.SetLoadMenuItemStatus(
            ControlServicePath,
            nameof(IBocTreeViewWebService.GetMenuItemStatusForTreeNode),
            stringValueParametersDictionary);
      }
    }

    private BusinessObjectWebServiceContext CreateBusinessObjectWebServiceContext ()
    {
      return BusinessObjectWebServiceContext.Create(
          DataSource,
          Property,
          ControlServiceArguments);
    }

    /// <summary>
    ///   Sets the tree view to be rebuilded with the current business objects. 
    ///   Must be called before or during the <c>PostBackEvent</c> to affect the tree view.
    /// </summary>
    public void InvalidateTreeNodes ()
    {
      _isRebuildRequired = true;
    }

    public void RefreshTreeNodes ()
    {
      BocTreeNode? selectedNode = (BocTreeNode?)_treeView.SelectedNode;
      string selectedNodePath = _treeView.FormatNodePath(selectedNode);

      InvalidateTreeNodes();
      InitializeRootWebTreeNodes();

      if (!string.IsNullOrEmpty(selectedNodePath))
      {
        string[] pathSegments;
        selectedNode = (BocTreeNode?)_treeView.ParseNodePath(selectedNodePath, out pathSegments);
        if (selectedNode != null)
          selectedNode.IsSelected = true;
      }

      if (selectedNodePath != _treeView.FormatNodePath(selectedNode))
        OnSelectionChanged(selectedNode);
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      base.AddAttributesToRender(writer);
      if (string.IsNullOrEmpty(CssClass) && string.IsNullOrEmpty(Attributes["class"]))
        writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);

      var renderer = CreateRenderer();
      renderer.AddDiagnosticMetadataAttributes(CreateRenderingContext(writer));
    }

    protected virtual IBocTreeViewRenderer CreateRenderer ()
    {
      return ServiceLocator.GetInstance<IBocTreeViewRenderer>();
    }

    protected virtual BocTreeViewRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      Assertion.IsNotNull(Context, "Context must not be null.");

      return new BocTreeViewRenderingContext(Context, writer, this);
    }

    private void InitializeRootWebTreeNodes ()
    {
      if (!_enableTreeNodeCaching || !ControlExistedInPreviousRequest)
        CreateRootTreeNodes();
      else
      {
        if (_isRebuildRequired)
        {
          RebuildTreeNodes();
        }
        else
        {
          Assertion.IsNotNull(_nodesControlState, "_nodesControlState must not be null.");
          LoadNodesControlStateRecursive(_nodesControlState, _treeView.Nodes);
        }
      }
    }

    private void CreateRootTreeNodes ()
    {
      if (Value != null)
      {
        for (int i = 0; i < Value.Count; i++)
        {
          IBusinessObjectWithIdentity? businessObject = (IBusinessObjectWithIdentity?)Value[i];
          Assertion.IsNotNull(businessObject, "The instance at index {0} of 'Value' must not be null.", i);
          BusinessObjectTreeNode node = CreateBusinessObjectNode(null, businessObject);
          _treeView.Nodes.Add(node);
          if (EnableTopLevelExpander)
          {
            if (EnableLookAheadEvaluation)
              node.Evaluate();
            else
              node.IsEvaluated = false;
          }
          else // Top-Level nodes are expanded
          {
            node.EvaluateExpand();
            if (EnableLookAheadEvaluation)
              node.EvaluateChildren();
          }
        }
      }
    }

    private void RebuildTreeNodes ()
    {
      if (!_enableTreeNodeCaching)
        return;

      Assertion.IsNotNull(_nodesControlState, "_nodesControlState must not be null.");

      _treeView.ResetNodes();
      CreateRootTreeNodes();
      ApplyNodesControlStateRecursive(_nodesControlState, _treeView.Nodes);
    }

    private void ApplyNodesControlStateRecursive (Pair[] nodesState, WebTreeNodeCollection nodes)
    {
      for (int i = 0; i < nodesState.Length; i++)
      {
        Pair nodeState = nodesState[i];
        object[] values = (object[])nodeState.First!;
        string itemID = (string)values[0];
        WebTreeNode? node = nodes.Find(itemID);
        if (node != null)
        {
          if (!node.IsEvaluated)
          {
            bool isEvaluated = (bool)values[2];
            if (isEvaluated)
              node.EvaluateExpand();
          }
          if (node.IsEvaluated)
          {
            bool isExpanded = (bool)values[1];
            node.IsExpanded = isExpanded;
            if (node.Children.Count == 0)
              node.IsExpanded = false;
          }
          ApplyNodesControlStateRecursive((Pair[])nodeState.Second!, node.Children);
        }
      }
    }

    private void EvaluateTreeNode (WebTreeNode node)
    {
      ArgumentUtility.CheckNotNullAndType<BocTreeNode>("node", node);

      if (node.IsEvaluated)
        return;

      BusinessObjectTreeNode? businessObjectNode = node as BusinessObjectTreeNode;
      BusinessObjectPropertyTreeNode? propertyNode = node as BusinessObjectPropertyTreeNode;

      if (businessObjectNode != null)
        CreateAndAppendBusinessObjectNodeChildren(businessObjectNode);
      else if (propertyNode != null)
        CreateAndAppendPropertyNodeChildren(propertyNode);
    }

    private void CreateAndAppendBusinessObjectNodeChildren (BusinessObjectTreeNode businessObjectNode)
    {
      IBusinessObjectWithIdentity? businessObject = businessObjectNode.BusinessObject;

      Assertion.IsNotNull(businessObject, "The business object of the node must not be null.");
      BusinessObjectPropertyTreeNodeInfo[] propertyNodeInfos = GetPropertyNodes(businessObjectNode, businessObject);
      if (propertyNodeInfos.Length > 0)
      {
        if (propertyNodeInfos.Length == 1)
          CreateAndAppendBusinessObjectNodes(businessObjectNode, businessObject, propertyNodeInfos[0].Property);
        else
          CreateAndAppendPropertyNodes(businessObjectNode, propertyNodeInfos);
      }
      businessObjectNode.IsEvaluated = true;
    }

    private void CreateAndAppendPropertyNodeChildren (BusinessObjectPropertyTreeNode propertyNode)
    {
      if (propertyNode.ParentNode == null)
      {
        throw new ArgumentException(
            string.Format(
                "BusinessObjectPropertyTreeNode with ItemID '{0}' has no parent node but property nodes cannot be used as root nodes.",
                propertyNode.ItemID));
      }
      if (!(propertyNode.ParentNode is BusinessObjectTreeNode))
      {
        throw new ArgumentException(
            string.Format(
                "BusinessObjectPropertyTreeNode with ItemID '{0}' has parent node of type '{1}' but property node cannot be children of nodes of type '{2}'.",
                propertyNode.ItemID,
                propertyNode.ParentNode.GetType().Name,
                typeof(BusinessObjectTreeNode).Name));
      }

      BusinessObjectTreeNode parentNode = (BusinessObjectTreeNode)propertyNode.ParentNode;
      Assertion.IsNotNull(parentNode.BusinessObject, "The business object of the parent node must not be null.");
      CreateAndAppendBusinessObjectNodes(propertyNode, parentNode.BusinessObject, propertyNode.Property);
      propertyNode.IsEvaluated = true;
    }

    private void CreateAndAppendBusinessObjectNodes (
        BocTreeNode parentNode,
        IBusinessObjectWithIdentity parentBusinessObject,
        IBusinessObjectReferenceProperty parentProperty)
    {
      var children = GetBusinessObjects(parentNode, parentBusinessObject, parentProperty);
      for (int i = 0; i < children.Length; i++)
      {
        IBusinessObjectWithIdentity childBusinessObject = children[i];
        BusinessObjectTreeNode childNode = CreateBusinessObjectNode(parentProperty, childBusinessObject);
        parentNode.Children.Add(childNode);
      }
    }

    private void CreateAndAppendPropertyNodes (
        BusinessObjectTreeNode parentNode,
        BusinessObjectPropertyTreeNodeInfo[] propertyNodeInfos)
    {
      for (int i = 0; i < propertyNodeInfos.Length; i++)
      {
        BusinessObjectPropertyTreeNodeInfo propertyNodeInfo = propertyNodeInfos[i];
        BusinessObjectPropertyTreeNode propertyNode = new BusinessObjectPropertyTreeNode(
            propertyNodeInfo.Property.Identifier,
            propertyNodeInfo.Text,
            propertyNodeInfo.ToolTip,
            propertyNodeInfo.Icon,
            propertyNodeInfo.Property);
        propertyNode.Badge = propertyNodeInfo.Badge;
        propertyNode.IsEvaluated = false;
        parentNode.Children.Add(propertyNode);
      }
    }

    private BusinessObjectTreeNode CreateBusinessObjectNode (
        IBusinessObjectReferenceProperty? property,
        IBusinessObjectWithIdentity businessObject)
    {
      string id = businessObject.UniqueIdentifier;
      WebString text = GetText(businessObject);
      PlainTextString toolTip = GetToolTip(businessObject);
      IconInfo? icon = GetIcon(businessObject, businessObject.BusinessObjectClass.BusinessObjectProvider);
      BusinessObjectTreeNode node = new BusinessObjectTreeNode(id, text, toolTip, icon, property, businessObject);
      node.Badge = GetBadge(businessObject);
      node.IsEvaluated = false;
      return node;
    }

    /// <summary> Overrides the parent control's <c>TagKey</c> property. </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get { return HtmlTextWriterTag.Div; }
    }

    [CanBeNull]
    protected virtual Badge? GetBadge (IBusinessObjectProperty businessObjectProperty, IBusinessObjectWithIdentity businessObject)
    {
      ArgumentUtility.CheckNotNull("businessObjectProperty", businessObjectProperty);
      ArgumentUtility.CheckNotNull("businessObject", businessObject);

      return null;
    }

    [CanBeNull]
    protected virtual Badge? GetBadge (IBusinessObjectWithIdentity businessObject)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);
      return null;
    }

    protected virtual WebString GetText (IBusinessObjectWithIdentity businessObject)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);
      return WebString.CreateFromText(businessObject.GetAccessibleDisplayName());
    }

    protected virtual PlainTextString GetToolTip (IBusinessObjectWithIdentity businessObject)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);
      return PlainTextString.CreateFromText(GetToolTip(businessObject, businessObject.BusinessObjectClass.BusinessObjectProvider));
    }

    [NotNull]
    protected virtual IBusinessObjectWithIdentity[] GetBusinessObjects (
        BocTreeNode parentNode,
        IBusinessObjectWithIdentity parentBusinessObject,
        IBusinessObjectReferenceProperty parentProperty)
    {
      ArgumentUtility.CheckNotNull("parentNode", parentNode);
      ArgumentUtility.CheckNotNull("parentBusinessObject", parentBusinessObject);
      ArgumentUtility.CheckNotNull("parentProperty", parentProperty);

      IList? children = (IList?)parentBusinessObject.GetProperty(parentProperty);
      if (children == null)
        return new IBusinessObjectWithIdentity[0];

      return children.Cast<IBusinessObjectWithIdentity>().ToArray();
    }

    [NotNull]
    protected virtual BusinessObjectPropertyTreeNodeInfo[] GetPropertyNodes (
        BusinessObjectTreeNode parentNode,
        IBusinessObjectWithIdentity parentBusinessObject)
    {
      ArgumentUtility.CheckNotNull("parentNode", parentNode);
      ArgumentUtility.CheckNotNull("parentBusinessObject", parentBusinessObject);
      if (Property == null)
      {
        ArrayList referenceListPropertyInfos = new ArrayList();
        IBusinessObjectProperty[] properties = parentBusinessObject.BusinessObjectClass.GetPropertyDefinitions();
        for (int i = 0; i < properties.Length; i++)
        {
          IBusinessObjectReferenceProperty? referenceProperty = properties[i] as IBusinessObjectReferenceProperty;
          if (referenceProperty != null
              && referenceProperty.IsList
              && referenceProperty.ReferenceClass is IBusinessObjectClassWithIdentity
              && referenceProperty.IsAccessible(parentBusinessObject))
            referenceListPropertyInfos.Add(CreateBusinessObjectPropertyTreeNodeInfo(referenceProperty, parentBusinessObject));
        }
        return (BusinessObjectPropertyTreeNodeInfo[])referenceListPropertyInfos.ToArray(typeof(BusinessObjectPropertyTreeNodeInfo));
      }

      return new[] { CreateBusinessObjectPropertyTreeNodeInfo(Property, parentBusinessObject) };
    }

    private BusinessObjectPropertyTreeNodeInfo CreateBusinessObjectPropertyTreeNodeInfo (IBusinessObjectReferenceProperty property, IBusinessObjectWithIdentity businessObject)
    {
      var businessObjectPropertyTreeNodeInfo = new BusinessObjectPropertyTreeNodeInfo(property);
      businessObjectPropertyTreeNodeInfo.Badge = GetBadge(property, businessObject);

      return businessObjectPropertyTreeNodeInfo;
    }

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocTreeView.xml' path='BocTreeView/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (DataSource == null)
        return;

      IBusinessObjectWithIdentity? value = null;

      if (DataSource.BusinessObject != null)
        value = (IBusinessObjectWithIdentity)DataSource.BusinessObject;

      LoadValueInternal(value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value">
    ///   The <see cref="IReadOnlyList{IBusinessObjectWithIdentity}"/> of objects to load,
    ///   or <see langword="null"/>.
    /// </param>
    /// <param name="interim"> Not used. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocTreeView.xml' path='BocTreeView/LoadUnboundValue/*' />
    public void LoadUnboundValue (IReadOnlyList<IBusinessObjectWithIdentity> value, bool interim)
    {
      LoadValueInternal(value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> 
    ///   The <see cref="IList"/> of objects implementing <see cref="IBusinessObjectWithIdentity"/> to load,
    ///   or <see langword="null"/>. 
    /// </param>
    /// <param name="interim"> Not used. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocTreeView.xml' path='BocTreeView/LoadUnboundValue/*' />
    public void LoadUnboundValueAsList (IList value, bool interim)
    {
      LoadValueInternal(value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="O:Remotion.ObjectBinding.Web.UI.Controls.BocTreeView.LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (object? value, bool interim)
    {
      ValueImplementation = value;
    }

    protected override void LoadControlState (object? savedState)
    {
      object?[] values = (object?[])savedState!;

      base.LoadControlState(values[0]);
      if (_enableTreeNodeCaching)
        _nodesControlState = (Pair[])values[1]!;
    }

    protected override object SaveControlState ()
    {
      object?[] values = new object?[2];

      values[0] = base.SaveControlState();
      if (_enableTreeNodeCaching)
        values[1] = SaveNodesStateRecursive(_treeView.Nodes);

      return values;
    }

    /// <summary> Loads the settings of the <paramref name="nodes"/> from <paramref name="nodesState"/>. </summary>
    private void LoadNodesControlStateRecursive (Pair[] nodesState, WebTreeNodeCollection nodes)
    {
      for (int i = 0; i < nodesState.Length; i++)
      {
        Pair nodeState = nodesState[i];
        object[] values = (object[])nodeState.First!;
        string itemID = (string)values[0];
        bool isExpanded = (bool)values[1];
        bool isEvaluated = (bool)values[2];
        bool isSelected = (bool)values[3];
        string menuID = (string)values[4];
        WebString text = (WebString)values[5];
        PlainTextString toolTip = (PlainTextString)values[6];
        IconInfo icon = (IconInfo)values[7];
        Badge badge = (Badge)values[8];
        bool isBusinessObjectTreeNode = (bool)values[10];

        WebTreeNode node;
        if (isBusinessObjectTreeNode)
        {
          node = new BusinessObjectTreeNode(itemID, text, toolTip, icon, null, null);
          string propertyIdentifier = (string)values[9];
          ((BusinessObjectTreeNode)node).PropertyIdentifier = propertyIdentifier;
        }
        else
          node = new BusinessObjectPropertyTreeNode(itemID, text, toolTip, icon, null);
        node.Badge = badge;
        node.IsExpanded = isExpanded;
        node.IsEvaluated = isEvaluated;
        if (isSelected)
          node.IsSelected = true;
        node.MenuID = menuID;
        nodes.Add(node);

        LoadNodesControlStateRecursive((Pair[])nodeState.Second!, node.Children);
      }
    }

    /// <summary> Saves the settings of the  <paramref name="nodes"/> and returns this control state </summary>
    private Pair[] SaveNodesStateRecursive (WebTreeNodeCollection nodes)
    {
      Pair[] nodesState = new Pair[nodes.Count];
      for (int i = 0; i < nodes.Count; i++)
      {
        WebTreeNode node = (WebTreeNode)nodes[i];
        Pair nodeState = new Pair();
        object?[] values = new object?[11];
        values[0] = node.ItemID;
        values[1] = node.IsExpanded;
        values[2] = node.IsEvaluated;
        values[3] = node.IsSelected;
        values[4] = node.MenuID;
        values[5] = node.Text;
        values[6] = node.ToolTip;
        values[7] = node.Icon;
        values[8] = node.Badge;
        if (node is BusinessObjectTreeNode)
        {
          values[9] = ((BusinessObjectTreeNode)node).PropertyIdentifier;
          values[10] = true;
        }
        else
          values[10] = false;
        nodeState.First = values;
        nodeState.Second = SaveNodesStateRecursive(node.Children);
        nodesState[i] = nodeState;
      }
      return nodesState;
    }

    public override bool SupportsProperty (IBusinessObjectProperty property)
    {
      ArgumentUtility.CheckNotNull("property", property);
      if (!base.SupportsProperty(property))
        return false;
      return ((IBusinessObjectReferenceProperty)property).ReferenceClass is IBusinessObjectClassWithIdentity;
    }

    /// <summary> The <see cref="BocTreeView"/> supports only list properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="true"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return isList;
    }

    /// <summary>
    ///   The <see cref="BocTreeView"/> supports properties of types <see cref="IBusinessObjectReferenceProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing this control.
    /// </summary>
    /// <value> Returns always <see langword="true"/>. </value>
    public override bool UseLabel
    {
      get { return true; }
    }

    /// <summary> The <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to. </summary>
    /// <value>An <see cref="IBusinessObjectReferenceProperty"/> object.</value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new IBusinessObjectReferenceProperty? Property
    {
      get { return (IBusinessObjectReferenceProperty?)base.Property; }
      set
      {
        IBusinessObjectReferenceProperty property = ArgumentUtility.CheckType<IBusinessObjectReferenceProperty>("value", value);
        if (value?.IsList == false)
          throw new ArgumentException("Only properties supporting IList can be assigned to the BocTreeView.", "value");
        base.Property = property;
      }
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <value> A list of <see cref="IBusinessObjectWithIdentity"/> implementations or <see langword="null"/>. </value>
    [Browsable(false)]
    public new IReadOnlyList<IBusinessObjectWithIdentity>? Value
    {
      get
      {
        return _value;
      }
      set
      {
        if (value != null)
          ArgumentUtility.CheckNotNullOrItemsNull("value", value);

        _value = value;
      }
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <value> A list of <see cref="IBusinessObjectWithIdentity"/> implementations or <see langword="null"/>. </value>
    [Browsable(false)]
    public IList? ValueAsList
    {
      get
      {
        var value = Value;

        if (value == null)
          return null;
        else if (value is BusinessObjectListAdapter<IBusinessObjectWithIdentity>)
          return ((BusinessObjectListAdapter<IBusinessObjectWithIdentity>)value).WrappedList;
        else if (value is IList)
          return (IList)value;
        else
          throw new InvalidOperationException("The value only implements the IReadOnlyList<IBusinessObjectWithIdentity> interface. Use the Value property to access the value.");
      }
      set
      {
        if (value == null)
          Value = null;
        else if (value is IReadOnlyList<IBusinessObjectWithIdentity>)
          Value = (IReadOnlyList<IBusinessObjectWithIdentity>)value;
        else
          Value = new BusinessObjectListAdapter<IBusinessObjectWithIdentity>(value);
      }
    }

    /// <summary> Gets or sets the current value when <see cref="Value"/> through polymorphism. </summary>
    /// <value> The value must be of type <see cref="IReadOnlyList{IBusinessObjectWithIdentity}"/>, <see cref="IList"/> or <see cref="IBusinessObjectWithIdentity"/>. </value>
    protected sealed override object? ValueImplementation
    {
      get
      {
        var value = Value;
        if (value is BusinessObjectListAdapter<IBusinessObjectWithIdentity>)
          return ((BusinessObjectListAdapter<IBusinessObjectWithIdentity>)value).WrappedList;
        else
          return value;
      }
      set
      {
        if (value == null)
        {
          Value = null;
        }
        else if (value is IBusinessObjectWithIdentity)
        {
          Value = new[] { (IBusinessObjectWithIdentity)value };
        }
        else if (value is IReadOnlyList<IBusinessObjectWithIdentity>)
        {
          Value = (IReadOnlyList<IBusinessObjectWithIdentity>)value;
        }
        else if (value is IList)
        {
          Value = new BusinessObjectListAdapter<IBusinessObjectWithIdentity>((IList)value);
        }
        else
        {
          throw new ArgumentException(
              string.Format(
                  "Parameter type '{0}' is not supported. Parameters must implement interface IBusinessObjectWithIdentity, IReadOnlyList<IBusinessObjectWithIdentity>, or IList.",
                  value.GetType()),
              "value");
        }
      }
    }

    /// <summary>Gets a flag indicating whether the <see cref="BocTreeView"/> contains a value. </summary>
    public override bool HasValue
    {
      get { return _value != null; }
    }

    /// <summary> Gets the tree nodes displayed by this tree view. </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public WebTreeNodeCollection Nodes
    {
      get { return _treeView.Nodes; }
    }

    /// <summary>Gets the <see cref="WebTreeView"/> used by this <see cref="BocTreeView"/> to render the tree.</summary>
    [Browsable(false)]
    public WebTreeView TreeView
    {
      get { return _treeView; }
    }

    [Category("Behavior")]
    [DefaultValue("")]
    public string? ControlServicePath
    {
      get { return _controlServicePath; }
      set { _controlServicePath = value ?? string.Empty; }
    }

    [Category("Behavior")]
    [DefaultValue("")]
    [Description("Additional arguments passed to the control service.")]
    public string? ControlServiceArguments
    {
      get { return _controlServiceArguments; }
      set { _controlServiceArguments = StringUtility.EmptyToNull(value); }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines whether to show the top level expander and automatically expand the 
    ///   child nodes if the expander is hidden.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("If cleared, the top level expender will be hidden and the child nodes expanded for the top level nodes.")]
    [DefaultValue(true)]
    public bool EnableTopLevelExpander
    {
      get { return _treeView.EnableTopLevelExpander; }
      set { _treeView.EnableTopLevelExpander = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to evaluate the child nodes when expanding a tree node. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("If set, the child nodes will be evaluated when a node is expanded.")]
    [DefaultValue(false)]
    public bool EnableLookAheadEvaluation
    {
      get { return _treeView.EnableLookAheadEvaluation; }
      set { _treeView.EnableLookAheadEvaluation = value; }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines whether to show scroll bars. Requires also a width for the tree view.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("If set, the tree view shows srcoll bars. Requires a witdh in addition to this setting to actually enable the scrollbars.")]
    [DefaultValue(false)]
    public bool EnableScrollBars
    {
      get { return _treeView.EnableScrollBars; }
      set { _treeView.EnableScrollBars = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to group the root nodes by their category. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("If set, the root nodes will be grouped by their category attribute. The order of the child nodes remians unchanged.")]
    [DefaultValue(false)]
    public bool EnableTopLevelGrouping
    {
      get { return _treeView.EnableTopLevelGrouping; }
      set { _treeView.EnableTopLevelGrouping = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to enable word wrapping. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Appearance")]
    [Description("If set, word wrap will be enabled for the tree node's text.")]
    [DefaultValue(false)]
    public bool EnableWordWrap
    {
      get { return _treeView.EnableWordWrap; }
      set { _treeView.EnableWordWrap = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to show the connection lines between the nodes. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Appearance")]
    [Description("If cleared, the tree nodes will not be connected by lines.")]
    [DefaultValue(true)]
    public bool ShowLines
    {
      get { return _treeView.ShowLines; }
      set { _treeView.ShowLines = value; }
    }

    /// <summary> Gets or sets a flag that determines whether the evaluated tree nodes will be cached. </summary>
    /// <remarks> 
    ///   Clear this flag if you want to reload all evaluated tree nodes during each post back. 
    ///   This could be required if the tree must show only the current nodes instead of the nodes that have 
    ///   been in the tree during the first evaluation.
    /// </remarks>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("If cleared, the evaluated tree nodes will be reloaded during each postback.")]
    [DefaultValue(true)]
    public bool EnableTreeNodeCaching
    {
      get { return _enableTreeNodeCaching; }
      set
      {
        _enableTreeNodeCaching = value;
        _treeView.EnableTreeNodeControlState = !value;
      }
    }

    /// <summary>
    /// Gets or sets a flag that determines whether the post back from a node click must be executed synchronously when the tree is rendered within 
    /// an <see cref="System.Web.UI.UpdatePanel"/>.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("True to require a synchronous postback for node clicks within Ajax Update Panels.")]
    [DefaultValue(false)]
    public bool RequiresSynchronousPostBack
    {
      get { return _treeView.RequiresSynchronousPostBack; }
      set { _treeView.RequiresSynchronousPostBack = value; }
    }

    /// <summary> Occurs when a node is clicked. </summary>
    [Category("Action")]
    [Description("Occurs when a node is clicked.")]
    public event BocTreeNodeClickEventHandler Click
    {
      add { Events.AddHandler(s_clickEvent, value); }
      remove { Events.RemoveHandler(s_clickEvent, value); }
    }

    /// <summary> 
    ///   Occurs when the selected node is changed. Fires for both client side changes or change by the 
    ///   <see cref="RefreshTreeNodes"/> method.
    /// </summary>
    [Category("Action")]
    [Description("Occurs when the selected node is changed. Fires for both client side changes and a change by the RefreshTreeNodes method.")]
    public event BocTreeNodeEventHandler SelectionChanged
    {
      add { Events.AddHandler(s_selectionChangedEvent, value); }
      remove { Events.RemoveHandler(s_selectionChangedEvent, value); }
    }

    /// <summary> Gets the currently selected tree node. </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public BocTreeNode? SelectedNode
    {
      get { return (BocTreeNode?)_treeView.SelectedNode; }
    }

    [Obsolete("Context menu is not compatible with keyboard navigation. See RM-9153", false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public BocTreeViewMenuItemProvider? MenuItemProvider
    {
      get { return (BocTreeViewMenuItemProvider?)_treeView.MenuItemProvider; }
      set
      {
        if (_treeView.MenuItemProvider != null)
          ((BocTreeViewMenuItemProvider)_treeView.MenuItemProvider).OwnerControl = null;

        _treeView.MenuItemProvider = value;

        if (_treeView.MenuItemProvider != null)
          ((BocTreeViewMenuItemProvider)_treeView.MenuItemProvider).OwnerControl = this;
      }
    }

    public void SetTreeNodeRenderMethodDelegate (WebTreeNodeRenderMethod treeNodeRenderMethod)
    {
      _treeView.SetTreeNodeRenderMethodDelegate(treeNodeRenderMethod);
    }

    //  public void EnsureTreeNodesCreated()
    //  {
    //    _treeView.EnsureTreeNodesCreated();
    //  }

    string IControlWithDiagnosticMetadata.ControlType
    {
      get { return "BocTreeView"; }
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="BocTreeView"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocTreeView</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBase
    {
      get { return "bocTreeView"; }
    }

    #endregion
  }

  public class BusinessObjectPropertyTreeNodeInfo
  {
    private WebString _text;
    private PlainTextString _toolTip;
    private IconInfo? _icon;
    private Badge? _badge;
    private IBusinessObjectReferenceProperty _property;

    public BusinessObjectPropertyTreeNodeInfo (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull("property", property);
      _text = WebString.CreateFromText(property.DisplayName);
      _toolTip = PlainTextString.Empty;
      _icon = null;
      _property = property;
    }

    public BusinessObjectPropertyTreeNodeInfo (WebString text, PlainTextString toolTip, IconInfo icon, IBusinessObjectReferenceProperty property)
    {
      _text = text;
      _toolTip = toolTip;
      _icon = icon;
      _property = property;
    }

    public WebString Text
    {
      get { return _text; }
      set { _text = value; }
    }

    public PlainTextString ToolTip
    {
      get { return _toolTip; }
      set { _toolTip = value; }
    }

    public IconInfo? Icon
    {
      get { return _icon; }
      set { _icon = value; }
    }

    public Badge? Badge
    {
      get { return _badge; }
      set { _badge = value; }
    }

    public IBusinessObjectReferenceProperty Property
    {
      get { return _property; }
      set { _property = value; }
    }
  }

  /// <summary> Represents the method that handles the <c>Click</c> event raised when clicking on a tree node. </summary>
  public delegate void BocTreeNodeClickEventHandler (object sender, BocTreeNodeClickEventArgs e);

  /// <summary> Provides data for the <c>Click</c> event. </summary>
  public class BocTreeNodeClickEventArgs : BocTreeNodeEventArgs
  {
    private string[] _path;

    public BocTreeNodeClickEventArgs (BusinessObjectTreeNode node, string[] path)
        : base(node)
    {
      _path = path;
    }

    public BocTreeNodeClickEventArgs (BusinessObjectPropertyTreeNode node, string[] path)
        : base(node)
    {
      _path = path;
    }

    /// <summary> The ID path for the clicked node. </summary>
    public string[] Path
    {
      get { return _path; }
    }
  }

  /// <summary> Represents the method that handles events raised by a <see cref="BocTreeNode"/>. </summary>
  public delegate void BocTreeNodeEventHandler (object sender, BocTreeNodeEventArgs e);

  /// <summary> Provides data for the events raised by a <see cref="BocTreeNode"/>. </summary>
  public class BocTreeNodeEventArgs : WebTreeNodeEventArgs
  {
    public BocTreeNodeEventArgs (BusinessObjectTreeNode node)
        : base(ArgumentUtility.CheckNotNull("node", node))
    {
    }

    public BocTreeNodeEventArgs (BusinessObjectPropertyTreeNode node)
        : base(ArgumentUtility.CheckNotNull("node", node))
    {
    }

    public new BocTreeNode Node
    {
      get
      {
        var node = (BocTreeNode?)base.Node;
        Assertion.DebugIsNotNull(node, "node != null");
        return node;
      }
    }

    public BusinessObjectTreeNode? BusinessObjectTreeNode
    {
      get { return Node as BusinessObjectTreeNode; }
    }

    public BusinessObjectPropertyTreeNode? PropertyTreeNode
    {
      get { return Node as BusinessObjectPropertyTreeNode; }
    }
  }
}
