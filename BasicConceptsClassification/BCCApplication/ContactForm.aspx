<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ContactForm.aspx.cs" Inherits="BCCApplication.ContactForm" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<div>
    <div>
        <h2>Contact Us</h2>
        <br />
        <table>
            <!-- Name -->
            <tr>
                <td>
                    *Name:</td>
                <td>
                    <asp:TextBox ID="txtName"
                                    runat="server"
                                    Columns="50"></asp:TextBox>
                </td>
            </tr>

            <!-- Email -->
            <tr>
                <td>
                    *Email:</td>
                <td>
                    <asp:TextBox ID="txtEmail"
                                    runat="server"
                                    Columns="50"></asp:TextBox>
                </td>
            </tr>
 
            <!-- Subject -->
            <tr>
                <td>
                    *Subject:</td>
                <td>
                    <asp:DropDownList ID="ddlSubject" runat="server">
                        <asp:ListItem>Ask a question</asp:ListItem>
                        <asp:ListItem>Report a bug</asp:ListItem>
                        <asp:ListItem>Request Account</asp:ListItem>
                        <asp:ListItem>Other</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
 
            <!-- Message -->
            <tr>
                <td>
                    *Message:</td>
                <td>
                    <asp:TextBox ID="txtMessage"
                                    runat="server"
                                    Columns="40"
                                    Rows="6"
                                    TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
 
            <!-- Submit -->
            <tr align="center">
                <td colspan="2">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit"
                        onclick="btnSubmit_Click" />
                </td>
            </tr>
             * You must fill in all fields
            <!-- Results -->
            <tr>
                <td colspan="2">
                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
</div>
</asp:Content>
