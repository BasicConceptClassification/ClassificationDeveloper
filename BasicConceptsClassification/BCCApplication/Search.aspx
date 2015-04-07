<%@ Page Title="Search Aid" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="True" Inherits="Search" Codebehind="Search.aspx.cs" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <%@ Register Assembly="Goldtect.ASTreeView" Namespace="Goldtect" TagPrefix="astv" %>
    <link rel="stylesheet" href="../Scripts/astreeview/astreeview.css" type="text/css" />
    <link rel="stylesheet" href="../Scripts/astreeview/contextmenu.css" type="text/css" />
    <script src="../Scripts/astreeview/astreeview.min.js" type="text/javascript"></script>
    <script src="../Scripts/astreeview/contextmenu.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function nodeSelectHandler(elem) {
            var selectedNode = elem.parentNode.getAttribute("TreeNodeValue");
            document.getElementById("<%=TextBox2.ClientID %>").value += selectedNode;
        }
    </script>
    <div>
        <h2><%: Title %></h2>
        <div>
            <asp:Label ID="LabelDescription" runat="server"></asp:Label>
        </div>
        <center>
            <asp:Label ID="SearchPrompt" runat="server" CssClass="control-label">Search for:</asp:Label>
            <asp:TextBox ID="TextBox2" runat="server" CssClass="input-xxlarge search-query" placeholder="(Search)(by)(Terms)"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" Text="Jump to Search page" OnClick="Button1_Click" CssClass="btn"/>
        </center>
    </div>
    <br />
    <div>
        <asp:Label ID="LabelNoticationDataSet" runat="server"></asp:Label>
        <astv:ASTreeView ID="astvMyTree"
            runat="server"
            BasePath="~/Scripts/astreeview/"
            DataTableRootNodeValue="0"
            EnableRoot="true"
            EnableNodeSelection="true"
            EnableCheckbox="false"
            EnableDragDrop="false"
            EnableTreeLines="true"
            EnableNodeIcon="true"
            EnableCustomizedNodeIcon="false"
            EnableDebugMode="false"
            EnableContextMenuAdd="false"
            EnableParentNodeExpand="true"
            OnNodeSelectedScript="nodeSelectHandler(elem);" />
    </div>
</asp:Content>
