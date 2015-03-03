<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClassOb.aspx.cs" Inherits="BCCApplication.Account.ClassOb" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <table style="width:100%;">
        <tr>
            <td>Name: 
                <input id="ObName" type="text" runat="server"/></td>
            <td>URL: 
                <input id="ObURL" type="text" runat="server"/></td>
        </tr>
        <tr>
            <td>Edit Permissions 
                <asp:RadioButtonList ID="EditPerm" runat="server" Font-Size="X-Small" RepeatColumns="2">
                    <asp:ListItem>Only Me</asp:ListItem>
                    <asp:ListItem>Anyone in my Institution</asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td>Concept String: 
                <input id="ObjConceptString" type="text" runat="server"/></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="ObAddStatus" runat="server"></asp:Label>
            </td>
            <td>
                <asp:Button ID="SubmitObj" runat="server" Text="Submit" /><br />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <div id="listContainer">
                    <asp:TreeView ID="DataSet" runat="server" ImageSet="Arrows" >
                        <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                        <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                        <ParentNodeStyle Font-Bold="False" />
                        <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
                    </asp:TreeView>
                 </div>
                Looking for a term not in the classification? <a href="">Click here to suggest a new term.</a>
            </td>
            <td>
                <asp:Button ID="AddTOb" runat="server" Text="Add" />
                <asp:Button ID="RemoveTOb" runat="server" Text="Remove" />
                <p>Double click on the term you want to use from the list or click the Add button.<br /><br />
                    Double click on a term in the Concept String to remove it.<br /><br />
                    Click and drag terms in the Concept String to change the order.
                </p>
            </td>
        </tr>
    </table>
    <br />
&nbsp; 
</asp:Content>
