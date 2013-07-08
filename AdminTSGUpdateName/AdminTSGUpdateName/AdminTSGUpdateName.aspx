<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminTSGUpdateName.aspx.cs" Inherits="AdminTSGUpdateName.AdminTSGUpdateName" %>
<%@ Register TagPrefix="PFWeb" TagName="AdminTitleCtrl" Src="_AdminTitleCtrl.ascx" %>
<%@ Register TagPrefix="PFWeb" TagName="AdminHeader" Src="_AdminHeader.ascx" %>
<%@ Register TagPrefix="PFWeb" TagName="AdminMenu" Src="_AdminMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Update Names</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	<link href="PFWebAdmin.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="origForm" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    <table>
        <tr>
            <td colspan="2"><PFWEB:ADMINHEADER id="AdminHdr" runat="server"></PFWEB:ADMINHEADER></td>
        </tr>
        <tr>
            <!-- admin menu custom control -->
		    <td style="width: 150px; vertical-align: text-top;"  ="top"><PFWEB:ADMINMENU id="AdminMenu" selectedLabel="AdminMaster_RepeaterMenu_ctl05_RepeaterMenuItems_ctl05_DivMenuItem" runat="server"></PFWEB:ADMINMENU></td>
            <td class="AdminMain" style="width: 100%; vertical-align: text-top;">
                <table class="UpdateTSGHead">
                    <tr>
                        <td style="vertical-align: text-top;"><a href="AdminTSGUpdateDivision.aspx">Update Plants</a></td>
                        <td style="vertical-align: text-top;"><a href="AdminTSGUpdateSeries.aspx">Update Series</a></td>
                        <td style="vertical-align: text-top;"><a href="AdminTSGUpdateName.aspx">Update Products</a></td>
                    </tr>
                </table>
            </td>
        </tr>

    </table>
    </form>
</body>
</html>
