<%@ Page Title="Classifying a GLAM Object" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClassOb.aspx.cs" Inherits="BCCApplication.Account.ClassOb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1><%: Title %></h1>
    <div>
        <asp:Label ID="LabelDescription" runat="server"></asp:Label>
    </div>
    <asp:RequiredFieldValidator runat="server" ControlToValidate="ObName" CssClass="text-danger" 
        ErrorMessage="The name of the object is required." />
    <asp:RequiredFieldValidator runat="server" ControlToValidate="ObUrl" CssClass="text-danger" 
        ErrorMessage="The URL of the object is required." />
    <h3>GLAM Object Information</h3>
    <table style="width:100%;">
        <tr>
            <td>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="ObName" CssClass="control-label">Name</asp:Label> 
                    <asp:TextBox ID="ObName" runat="server"></asp:TextBox>
                </div>
            </td>
            <td>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="ObURL" CssClass="control-label">URL</asp:Label>
                    <asp:TextBox ID="ObURL" runat="server"></asp:TextBox>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div>
                    <asp:Label runat="server" AssociatedControlID="EditPerm" CssClass="control-label">Set Permissions</asp:Label>    
                    <asp:RadioButtonList ID="EditPerm" runat="server" CssClass="radioButtonList" RepeatDirection="Horizontal">
                        <asp:ListItem Value="GLAM" Selected="True"/>
                        <asp:ListItem Value="OwnerOnly"/>
                    </asp:RadioButtonList>
                </div>
            </td>
            <td>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="ObConcept" CssClass="control-label">Concept String</asp:Label>
                    <asp:TextBox ID="ObConcept" runat="server"></asp:TextBox>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="ObAddStatus" runat="server"></asp:Label>
            </td>
            <td>
        
                <asp:Button ID="SubmitObj" runat="server" Text="Submit" OnClick="SubmitObj_Click" AutoPostBack="false"/><br />
            </td>
        </tr>
    </table>
    <table class="table-top-aligned" style="width:100%;">
        <tr>
            <td>
                <asp:Label ID="LabelNoticationDataSet" runat="server"></asp:Label>
                <div id="listContainer">
                    <asp:TreeView ID="DataSet" runat="server" ImageSet="Arrows"
                        OnTreeNodePopulate="PopulateNode">
                        <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                        <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                        <ParentNodeStyle Font-Bold="False" />
                        <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
                    </asp:TreeView>
                </div>
                Looking for a term not in the classification? <a href="SuggestTerm.aspx">Click here to suggest a new term.</a>
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
</asp:Content>
