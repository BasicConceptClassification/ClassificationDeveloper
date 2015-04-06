<%@ Page Title="Modifying the Classification" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="BCCApplication.Account.AdminPage" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <%@ Register Assembly="Goldtect.ASTreeView" Namespace="Goldtect" TagPrefix="astv" %>

    <link rel="stylesheet" href="../Scripts/astreeview/astreeview.css" type="text/css" />
    <link rel="stylesheet" href="../Scripts/astreeview/contextmenu.css" type="text/css" />
    <script src="../Scripts/astreeview/astreeview.min.js" type="text/javascript"></script>
    <script src="../Scripts/astreeview/contextmenu.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        //Move term 
        function dndCompletedHandler(elem, newParent) {
            document.getElementById("<%=MoveTermTextBox.ClientID %>").value = elem.getAttribute("treeNodeValue");
            document.getElementById("<%=MoveTermUnderTextBox.ClientID %>").value = newParent.getAttribute("treeNodeValue");
            var movebutton = document.getElementById("<%=MoveAddButton.ClientID %>");
            movebutton.click();
        }

    </script>

    <div>
        <h1><%: Title %></h1>
        <asp:Label ID="LabelDescription" runat="server"></asp:Label>
    </div>
    <div>
        <astv:ASTreeView ID="astvMyTree"
            runat="server"
            BasePath="~/Scripts/astreeview/"
            DataTableRootNodeValue="0"
            EnableRoot="false"
            EnableNodeSelection="true"
            EnableCheckbox="false"
            EnableDragDrop="true"
            EnableTreeLines="true"
            EnableNodeIcon="true"
            EnableCustomizedNodeIcon="false"
            AutoPostBack="false"
            EnableContextMenu="true"
            EnableDebugMode="false"
            EnableAjaxOnEditDelete="true"
            OnNodeDragAndDropCompletedScript="dndCompletedHandler( elem, newParent )"
            EditNodeProvider="~/Account/AjaxItemProvider/EditItemProvider.aspx"
            DeleteNodeProvider="~/Account/AjaxItemProvider/DeleteItemProvider.aspx"
            AddNodeProvider="~/Account/AjaxItemProvider/AddItemProvider.aspx"
            OnNodeEditedScript="editedHandler(elem)"
            EnableMultiLineEdit="false"
            EnableEscapeInput="false" />
    </div>
    <div style="display: none">

        <asp:TextBox ID="MoveTermTextBox" runat="server"></asp:TextBox>

        <asp:TextBox ID="MoveTermUnderTextBox" runat="server"></asp:TextBox>

        <asp:Button ID="MoveAddButton" runat="server" Text="MOVE" OnClick="MoveAddButton_Click" />

    </div>

</asp:Content>
