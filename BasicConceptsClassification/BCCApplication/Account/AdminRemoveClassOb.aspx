<%@ Page Title="Admin - Remove Classifiable" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminRemoveClassOb.aspx.cs" Inherits="BCCApplication.Account.AdminRemoveClassOb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
      <link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-3">
            <div>
                <h3>
                    Select By Letter
                </h3>
                <asp:DropDownList 
                    ID="AlphabetDDL" 
                    runat="server" 
                    OnSelectedIndexChanged="AlphabetDDL_SelectedIndexChanged"
                    AutoPostBack="true">
                </asp:DropDownList>
            </div>
            <div>
                <asp:ListBox
                    ID="ClassListBox"
                    runat="server"
                    OnSelectedIndexChanged="ClassListBox_SelectedIndexChanged"
                    AutoPostBack="true">
                </asp:ListBox>
            </div>
        </div>
        <div class="col-md-9">
            <h3>
                Classifiable Information
            </h3>
            <div class="form-horizontal">
                <div class="form-group">
                    <asp:Label AssociatedControlID="TxtBxName" runat="server" CssClass="col-sm-3 control-label">Name</asp:Label>
                    <div class="col-sm-9">
                        <asp:TextBox ID="TxtBxName" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label AssociatedControlID="TxtBxUrl" runat="server" CssClass="col-sm-3 control-label">URL</asp:Label>
                    <div class="col-sm-9">
                        <asp:TextBox ID="TxtBxUrl" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label AssociatedControlID="TxtBxConStr" runat="server" CssClass="col-sm-3 control-label">Concept String</asp:Label>
                    <div class="col-sm-9">
                        <asp:TextBox ID="TxtBxConStr" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-offset-3 col-sm-9">
                        <asp:Button ID="BtnRemove" runat="server" CssClass="btn btn-default" Text="Remove" OnClick="BtnRemove_Click"></asp:Button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
