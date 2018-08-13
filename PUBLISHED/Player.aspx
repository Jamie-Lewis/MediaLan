<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Player.aspx.cs" Inherits="MediaLan.Player" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://dashif.org/reference/players/javascript/latest/dist/dash.all.debug.js"></script>
    <link href="Styles.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:PlaceHolder ID="ph1" runat="server" />
        </div>
    </form>
</body>
</html>

<script>
    var vid = document.getElementById("videoPlayer");
    if (vid != null) {
        vid.onplay = function () {
            //alert("The video has started to play");
        };
        
        vid.onprogress = function (elem, ev) {
            //console.log("Current Time: " + vid.currentTime + ", Ready State: " + vid.readyState);
            //alert("Downloading video");
        };

        var ts = localStorage.getItem("ts_<%=Request.QueryString["key"]%>");
        if (!ts) {
            if (ts > 1 && confirm('do you want to start from saved position?\n\nContinue playing from: ' + ts)) {
                vid.currentTime = ts;
            } else {
                localStorage.setItem("ts_<%=Request.QueryString["key"]%>", null);
            }
        }

        window.onunload = function () {
            if (vid.currentTime > 1) {
                localStorage.setItem("ts_<%=Request.QueryString["key"]%>", vid.currentTime);
            }
        }
    }
</script>
