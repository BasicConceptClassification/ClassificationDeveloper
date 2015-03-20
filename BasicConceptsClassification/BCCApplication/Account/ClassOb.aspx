﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClassOb.aspx.cs" Inherits="BCCApplication.Account.ClassOb" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <asp:RequiredFieldValidator runat="server" ControlToValidate="ObName" CssClass="text-danger" 
        ErrorMessage="The name of the object is required." />
    <asp:RequiredFieldValidator runat="server" ControlToValidate="ObUrl" CssClass="text-danger" 
        ErrorMessage="The URL of the object is required." />
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
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="EditPerm" CssClass="control-label">Edit Permissions</asp:Label>    
                    <asp:RadioButtonList ID="EditPerm" runat="server" Font-Size="X-Small" RepeatColumns="2">
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
                <asp:Button ID="SubmitObj" runat="server" Text="Submit" OnClick="SubmitObj_Click" /><br />
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
