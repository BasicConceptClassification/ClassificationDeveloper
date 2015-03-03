<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GLAMClass.aspx.cs" Inherits="BCCApplication.Account.GLAMClass" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        
            <br />
            <table style="width:100%;">
                <tr>
                    <td class="auto-style1"><p>Recently Added Terms</p>
                        <select id="RecAddedTerms" name="D1" size="5" runat="server"></select><br /><br />
                        <p>Your Recently Classified</p>
                        <select id="RecClassTerms" name="D1" size="5" runat="server"></select>
                    </td>
                    <td><center><h1>Welcome, Classifier!</h1>
                        <p>Search for items below, add new or classify items waiting to be classified! (And other instructions.)</p>
                        <p>Search: <asp:TextBox ID="TextBox2" runat="server" placeholder="(term1)(term2)"></asp:TextBox>
                            <asp:Button ID="Button1" runat="server" Text="Search" OnClick="Button1_Click"/>
                        </p>
                        <form action="SearchResults.aspx" method="get" target="_self">
                        </form>
                        </center>
                        <table style="width:100%;">
                            <tr>
                                <td><center><h3>Not Classified: </h3>
                                    <select id="UnClassList" name="D1" size="5" runat="server"></select><br />
                                    <asp:Button ID="ButtGLAMClassClassNow" runat="server" Text="Classify Now" OnClick="ClassNow_Click" Font-Size="Small" /></center>
                                </td>
                                <td><center>OR</center></td>
                                <td><center><asp:Button ID="ButtGLAMClassAddNew" runat="server" Text="Add New" OnClick="AddNew_Click" Font-Size="Small"/></center></td>
                            </tr>
                        </table>
                        <br />                       
                    </td>
                </tr>
            </table>
            &nbsp;
        
    </div>
</asp:Content>

<asp:Content ID="Content1" runat="server" contentplaceholderid="HeadContent">
    <style type="text/css">
        #UnClassList {
            width: 187px;
        }
        .auto-style1 {
            width: 307px;
        }
        #RecAddedTerms {
            width: 187px;
        }
        #RecClassTerms {
            width: 187px;
        }
    </style>
</asp:Content>


