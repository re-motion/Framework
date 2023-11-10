<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="Remotion.Data.DomainObjects.Web.Test.Performance.Form" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Data.DomainObjects.Web.Test.Performance" assembly="Remotion.Data.DomainObjects.Web.Test" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <script type="text/javascript">
    var renderStartTime = new Date().getTime();
    var pageRequestManagerRenderStartTime = 0;
    var asyncBeginRequestTime = 0;
    var asyncEndRequestTime = 0;
    
    function Page_OnEndSyncRequest()
    {
      var renderEndTime = new Date().getTime();
      
      var renderDuration = (renderEndTime - renderStartTime) / 1000;
      document.getElementById('PostBackStatus').innerHTML = String.format('sync postback: {0:N2}s', renderDuration);

      renderStartTime = 0;
    }
    
    function Page_OnBeginAsyncLoading()
    {
      renderStartTime = new Date().getTime();
    }

    function Page_OnBeginAsyncRequest()
    {
      asyncBeginRequestTime = new Date().getTime();
    }

    function Page_OnEndAsyncRequest()
    {
      var renderEndTime = new Date().getTime();
      var requestDuration = (renderStartTime - asyncBeginRequestTime) / 1000;
      var renderDuration = (renderEndTime - renderStartTime) / 1000;
      document.getElementById('PostBackStatus').innerHTML = String.format('async postback: request duration: {0:N2}s, render duration: {1:N2}s', requestDuration, renderDuration);

      renderStartTime = 0;
      pageRequestManagerRenderStartTime = 0;
    }
  </script> 

  <title>Performance Test</title>
  <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
  <style type="text/css">
    html
    {
      height: 100%;
      margin: 0;
      padding: 0;
      overflow: clip;
    }
    body
    {
      height: 100%;
      margin: 0;
      padding: 0;
      overflow: clip;
    }
    form
    {
      height: 100%;
      width: 100%;
    }
    #UpdatePanel
    {
      height: 100%;
    }
  </style>
</head>
<body>
  <div style="height: 100%">
    <form id="TheForm" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true" />
    <asp:UpdatePanel ID="UpdatePanel" runat="server">
      <contenttemplate>
        <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.Data.DomainObjects.Web.Test::Domain.ClassForRelationTest" />
        <remotion:FormGridManager ID="FormGridManager" runat="server" />    
        <remotion:SingleView ID="TheView" runat="server">
          <TopControls>
            <asp:Literal ID="Top" runat="server">Top Controls</asp:Literal>
          </TopControls>
          <View>
            <table id="FormGrid" runat="server">
              <tr>
                <td>
                  <remotion:SmartLabel ID="ItemsLabel" runat="server" ForControl="ItemList" Text="Items" />
                </td>
                <td>
                  <remotion:BocListWithRowMenues ID="ItemList" runat="server" DataSourceControl="CurrentObject" PageSize="100" Selection="Multiple" Index="InitialOrder" Width="100%" Height="15em" RowMenuDisplay=Manual>
                    <FixedColumns>
                      <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Name" ColumnTitle="Name" />
                      <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="ClassWithAllDataTypesMandatory.StringProperty" ColumnTitle="String" />
                      <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="ClassWithAllDataTypesMandatory.DateProperty" ColumnTitle="Date" />
                      <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="ClassWithAllDataTypesMandatory.Int32Property" ColumnTitle="Int32" />
                      <remotion:BocDropDownMenuColumnDefinition Width="16px"/>
                    </FixedColumns>
                  </remotion:BocListWithRowMenues>
                </td>
              </tr>
            </table>
          </View>
          <BottomControls>
            <remotion:WebButton ID="SynchPostBackButton" runat="server" Text="Synchronous PostBack" RequiresSynchronousPostBack="true" />
            <remotion:WebButton ID="AsynchPostBackButton" runat="server" Text="Asynchronous PostBack" RequiresSynchronousPostBack="false" />
            <asp:Literal ID="PostbackStatusLiteral" runat="server">
              <div>              
                Page Status:
                <span id="PostBackStatus"></span>
              </div>
            </asp:Literal>
          </BottomControls>
        </remotion:SingleView>
      </contenttemplate>
    </asp:UpdatePanel>
    </form>
  </div>
  
  <script type="text/javascript">
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_beginRequest(Page_OnBeginAsyncRequest);
    prm.add_pageLoading(Page_OnBeginAsyncLoading);
    prm.add_endRequest(Page_OnEndAsyncRequest);

    document.defaultView.addEventListener('load', Page_OnEndSyncRequest);
  </script>
</body>
</html>
