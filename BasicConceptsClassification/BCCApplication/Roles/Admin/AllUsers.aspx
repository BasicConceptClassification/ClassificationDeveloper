<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AllUsers.aspx.cs" Inherits="BCCApplication.Roles.Admin.AllUsers" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Users</h1>

    Number of Users Online:
    <asp:Label ID="UsersOnlineLabel" runat="Server" /><br />

    <asp:Panel ID="NavigationPanel" Visible="false" runat="server">
        <table border="0">
            <tr>
                <td>Page
                        <asp:Label ID="CurrentPageLabel" runat="server" />
                    of
                        <asp:Label ID="TotalPagesLabel" runat="server" /></td>
                <td>
                    <asp:LinkButton ID="PreviousButton" Text="< Prev"
                        OnClick="PreviousButton_OnClick" runat="server" /></td>
                <td>
                    <asp:LinkButton ID="NextButton" Text="Next >"
                        OnClick="NextButton_OnClick" runat="server" /></td>
            </tr>
        </table>
    </asp:Panel>

    <asp:DataGrid ID="UserGrid" runat="server"
        CellPadding="2" CellSpacing="1"
        GridLines="Both">
        <HeaderStyle BackColor="darkblue" ForeColor="white" />
    </asp:DataGrid>

</asp:Content>
