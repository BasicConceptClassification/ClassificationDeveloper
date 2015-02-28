<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BCCApplication._Default" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Basic Concepts Classification</h1>
        <p>Welcome to the Basic Concepts Classification. Here you can search for items found in the 
            galleries, archieves, and museums. Each Item can be searched for by terms in the classification.</p>
            <form action="SearchResults.aspx" method="get" target="_self">
                Search:
                <br />
                    <div id="listContainer">
                        <asp:TreeView ID="DataSet" runat="server" ImageSet="Arrows" OnSelectedNodeChanged="DataSet_SelectedNodeChanged">
                            <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                            <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                            <ParentNodeStyle Font-Bold="False" />
                            <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
                         </asp:TreeView>
                    </div>
                <input type="button" onclick="location.href = 'SearchResults.aspx'" value='Search' />
            </form>
        </center>
    </div>
</asp:Content>