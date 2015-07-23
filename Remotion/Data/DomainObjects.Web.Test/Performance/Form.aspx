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
      $('#PostBackStatus').html(String.format('sync postback: {0:N2}s', renderDuration));

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
      $('#PostBackStatus').html(String.format('async postback: request duration: {0:N2}s, render duration: {1:N2}s', requestDuration, renderDuration));

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
      overflow: hidden;
    }
    body
    {
      height: 100%;
      margin: 0;
      padding: 0;
      overflow: hidden;
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

    $(window).load(Page_OnEndSyncRequest);
  </script>
</body>
</html>
