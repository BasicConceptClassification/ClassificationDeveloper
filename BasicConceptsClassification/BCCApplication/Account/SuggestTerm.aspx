<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SuggestTerm.aspx.cs" Inherits="BCCApplication.Account.SuggestTerm" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <h1>Suggest New Term</h1>
        * You must fill in all fields
        <table>
            <tr>
                <td>
                    *Term Name:</td>
                <td>
                    <asp:TextBox ID="txtTermName"
                                    runat="server"
                                    Columns="50"></asp:TextBox>
                </td>
            </tr> 
            <tr>
                <td>
                   *Parent Term String:</td>
                <td>
                    <asp:TextBox ID="txtParentString"
                                    runat="server"
                                    Columns="50"></asp:TextBox>
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
                                    TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
            <tr align="center">
                <td colspan="2">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit"
                        onclick="btnSubmit_Click" />
                </td>
            </tr>
             * You must fill in all fields
            <tr>
                <td colspan="2">
                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                </td>
            </tr>
            </table>
    </div>
</asp:Content>
