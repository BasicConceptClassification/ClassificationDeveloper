<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GLAMClass.aspx.cs" Inherits="BCCApplication.Account.GLAMClass" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <table style="width: 100%;">
            <tr>
                <td class="auto-style1">
                    <h3>Your Recently Classified</h3>
                    <select id="RecClassObj" name="D1" size="8" runat="server"></select>
                    <div>
                        <asp:Label ID="LabelRecClassObj" runat="server" Visible="false"></asp:Label>
                    </div>
                </td>
                <td>
                    <h2>Welcome, Classifier!</h2>
                    <div>
                        <asp:Label ID="LabelDescription" runat="server"></asp:Label>
                    </div>

                    <h3>Notifications</h3>
                    <asp:Table runat="server" ID="TableNotification" BorderWidth="1" CellPadding="2" GridLines="Both"></asp:Table>
                    <div>
                        <asp:Label runat="server" ID="LabelTableNotification" Visiable="false"></asp:Label>
                    </div>
                    <h3>Manage Classifiables</h3>
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                <h3>Not Classified</h3>
                                <div id="NotClassifiedExists">
                                    <center>
                                            <select id="UnClassList" name="D1" size="5" runat="server"></select><br />
                                            <asp:Button ID="ButtGLAMClassClassNow" runat="server" Text="Classify Now" OnClick="ClassNow_Click" Font-Size="Small" />
                                        </center>
                                </div>
                                <div id="NotClassifiedNoneExist">
                                    <asp:Label ID="LabelNotClassified" runat="server" Visible="false"></asp:Label>
                                </div>
                            </td>
                            <td>
                                <h3>Not Classified - Special</h3>
                                <div id="NotClassifiedSpecialExists">
                                    <center>
                                    <select id="UnClassAdminCause" name="D1" size="5" runat="server"></select><br />
                                    <asp:Button ID="ButtGLAMClassReClassNow" runat="server" Text="Reclassify Now" OnClick="ReClassNow_Click" Font-Size="Small" />

                                    </center>
                                </div>
                                <div id="NotClassifiedSpecialNoneExist">
                                    <asp:Label ID="LabelNotClassifiedSpecial" runat="server" Visible="false"></asp:Label>
                                </div>
                            </td>
                            <td>
                                <center><p>OR</p></center>
                            </td>
                            <td>
                                <center><asp:Button ID="ButtGLAMClassAddNew" runat="server" Text="Add New" OnClick="AddNew_Click" Font-Size="Small"/><br /><br />
                                    <asp:Button ID="RemoveClass" runat="server" Text="Remove" OnClick="RemoveClassPage_Click" Font-Size="Small"/>
                                    </center>
                            </td>
                        </tr>
                    </table>
                    <br />
                </td>
            </tr>
        </table>
        &nbsp;
        
    </div>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="HeadContent">
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


