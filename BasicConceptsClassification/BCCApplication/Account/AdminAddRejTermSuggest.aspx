<%@ Page Title="Add/Reject Suggested Terms" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminAddRejTermSuggest.aspx.cs" Inherits="BCCApplication.Account.AdminAddRejTermSuggest" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <h1><%: Title %></h1>
        <div class ="td">
            <center>
                <h2>Suggested Terms:</h2><br />
                <asp:ListBox ID="ListBoxClass" runat="server" Height="200px" Width="300px"></asp:ListBox>
                <br />
                <asp:Button ID="Update_SuggTerm" runat="server" OnClick="Update_SuggTerm_Click" Text="Update" />
            </center>
        </div>
        <table>
            <tr>
                <td>
                    Term Name:</td>
                <td>
                    <asp:TextBox ID="txtTermName"
                                    runat="server"
                                    Columns="50"
                                    ReadOnly="true"></asp:TextBox>
                </td>
            </tr> 
            <tr>
                <td>
                   Parent Term String:</td>
                <td>
                    <asp:TextBox ID="txtParentString"
                                    runat="server"
                                    Columns="50"
                                    ReadOnly="true"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Reason:</td>
                <td>
                    <asp:TextBox ID="txtMessage"
                                    runat="server"
                                    Columns="40"
                                    Rows="6"
                                    TextMode="MultiLine"
                                    ReadOnly="true"></asp:TextBox>
                    <asp:HiddenField ID="curNoticationIndex" runat="server" />
                </td>
            </tr>
            <tr align="center">
                <td colspan="2">
                    <asp:Button ID="btnAccept" runat="server" Text="Accept and Remove Notification"
                        onclick="btnAccept_Click" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnReject" runat="server" Text="Reject and Remove Notification"
                        onclick="btnReject_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                </td>
            </tr>
            </table>
    </div>
</asp:Content>
