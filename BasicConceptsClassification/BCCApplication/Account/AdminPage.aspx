<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="BCCApplication.Account.AdminPage" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <div>
            <table style="width:100%">
                <tr>
                    <td>
                        <center>
                            <h1>The Classification</h1>
                        </center>
                    </td>
                </tr>
                <tr>
                    <td>
                        <p>You can edit the Classification by doing the following....</p>
                    </td>
                </tr>
                <tr>
                    <td id="Datalist1">
                         <asp:TreeView ID="DataSet" runat="server" ImageSet="Arrows" OnSelectedNodeChanged="DataSet_SelectedNodeChanged">
                            <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                            <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                            <ParentNodeStyle Font-Bold="False" />
                            <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
                         </asp:TreeView>
                    </td>
                    <td>
                        <ul id="control">
                            <li><a runat="server">Add</a></li>
                            <li><a runat="server">Move</a></li>
                            <li><a runat="server">Rename</a></li>
                            <li><a runat="server">Delete</a></li>
                        </ul>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>