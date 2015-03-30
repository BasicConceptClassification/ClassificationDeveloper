<%@ Page Language="C#" Title="Classifier Home" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GLAMClass.aspx.cs" Inherits="BCCApplication.Account.GLAMClass" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Content/bootstrap.css" rel="stylesheet" type="text/css"/>
    <style type="text/css">
        #UnClassList {
            width: 187px;
        }

        #UnClassAdminCause {
            width: 187px;
        }

        .auto-style1 {
            width: 307px;
        }

        #RecAddedTerms {
            width: 187px;
        }

        #RecClassObj {
            width: 187px;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <table style="width: 100%;">
            <tr>
                <td>
                    <h2>Welcome, Classifier!</h2>
                    <div>
                        <asp:Label ID="LabelDescription" runat="server"></asp:Label>
                    </div>
                    <div>
                        <h3>Notifications</h3>
                        <asp:Table runat="server" ID="TableNotification" BorderWidth="1" CellPadding="2" GridLines="Both"></asp:Table>
                        <div>
                            <asp:Label runat="server" ID="LabelTableNotification" Visible="false"></asp:Label>
                        </div>
                    </div>
                    <div>
                        <h3>Manage Classifiables</h3>
                        <asp:Button ID="ButtGLAMClassAddNew" runat="server" Text="Add a New GLAM Object" OnClick="AddNew_Click" Font-Size="Small" />
                        <asp:Button ID="ButtGLAMClassClassNow" runat="server" Text="Edit GLAM OBjects" OnClick="ClassNow_Click" Font-Size="Small" />
                        <asp:Button ID="RemoveClass" runat="server" Text="Remove GLAM Objects" OnClick="RemoveClassPage_Click" Font-Size="Small" />
                    </div>
                    <div>
                        <h3>Your Recently Classified</h3>
                        <asp:Table ID="TableRecClassObj" runat="server" CssClass="table table-bordered table-fixed-width"></asp:Table>
                        <div>
                            <asp:Label ID="LabelRecClassObj" runat="server" Visible="false"></asp:Label>
                        </div>
                    </div>
                    <div>
                        <h3>Not Classified</h3>
                        <asp:Table ID="TableUnClassList" runat="server" CssClass="table table-bordered table-fixed-width"></asp:Table>
                        <div>
                            <asp:Label ID="LabelNotClassified" runat="server" Visible="false"></asp:Label>
                        </div>
                    </div>
                    <div>
                        <h3>Not Classified - Special</h3>
                        <asp:Table ID="TableUnClassAdminCause" runat="server" CssClass="table table-bordered table-fixed-width"></asp:Table>
                        <div id="NotClassifiedSpecialNoneExist">
                            <asp:Label ID="LabelNotClassifiedSpecial" runat="server" Visible="false"></asp:Label>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>




