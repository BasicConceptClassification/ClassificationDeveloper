<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RemoveClassOb.aspx.cs" Inherits="BCCApplication.Account.RemoveClassOb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 789px;
        }
        .auto-style2 {
            width: 366px;
        }
        #UnClassList {
            width: 349px;
        }
        .auto-style3 {
            width: 200px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <div>
            <asp:Label ID="Notification" runat="server"></asp:Label>
        </div>
       <table style="width:100%;">
                <tr>
                    <td class="auto-style2">
                        <h3> Classifiables: </h3><br />
                        <div>
                            <asp:ListBox id="ClassListBox" name="D1" size="15" runat="server"></asp:ListBox>
                        </div>
                        <div>
                            <asp:Button ID="ButtonGetClassifiableInfo" runat="server" Text="Get Information" OnClick="GetClassifiableInfo_Click"/>
                        </div>
                    </td>
                    <td class="auto-style3">
                        <asp:Label ID="Label1" runat="server" Text="Name:"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="Label2" runat="server" Text="URL:"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="Label3" runat="server" Text="Concept String:"></asp:Label>
                        <br />
                        <br />
                        <br />
                        <br />

                    </td>
                    <td class="auto-style1">
                        <asp:HiddenField ID="SelectIndex" runat="server" />
                        <asp:TextBox ID="SelectName" runat="server" ReadOnly="true"></asp:TextBox> <br />
                        <asp:TextBox ID="SelectURL" runat="server" ReadOnly="true"></asp:TextBox> <br />
                        <asp:TextBox ID="SelectConceptString" runat="server" Width="475px" ReadOnly="true"></asp:TextBox> <br /><br />
                        <asp:Button ID="RemoveClassData" runat="server" Text="Remove" OnClick="RemoveClassFromData_Click"/>
                    </td>
                </tr>
       </table>
    </div>
</asp:Content>
