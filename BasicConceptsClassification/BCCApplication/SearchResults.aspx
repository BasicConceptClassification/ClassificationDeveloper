<%@ Page Title="Search" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SearchResults.aspx.cs" Inherits="SearchResults" %>

<script runat="server">
    private void searchResultprint(object sender, EventArgs e)
    {
        
    }
</script>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Search Results</h1></center>
        <br />
        <center>
            <form action="SearchResults.aspx" method="get">
                Search Results:
                <input type="search" name="BCCSearch" placeholder ="[term1] [term2]"/>
                <input type="button" onclick="location.href='SearchResults.aspx'" value='Search Again' />
            </form>
        </center>
            <br />
        <span>
            1. 
            <b>Abrader</b> <br />
            tool. for. smoothing. <br />
            Source/Stored at: <a href="" target="_blank">http://www.someplace.com</a> <br /><br />
            2. 
            <b>Adobe</b> <br />
            clay. for. building. <br />
            Source/Stored at: <a href="" target="_blank">http://www.someplace.com</a> <br /><br />
            3. 
            <b>Adze Head</b> <br />
            head. of. tool. for. carving. wood. <br />
            Source/Stored at: <a href="" target="_blank">http://www.someplace.com</a> <br /><br />
            4. 
            <b>Adze Blade</b> <br />
            blade. of. tool. for. carving. wood. <br />
            Source/Stored at: <a href="" target="_blank">http://www.someplace.com</a> <br /><br />
            5. 
            <b>Antler Artifact</b> <br />
            artiface. from. antler. <br />
            Source/Stored at: <a href="" target="_blank">http://www.someplace.com</a> <br /><br />
        </span>
    </div>
    <div>

    </div>
</asp:Content>