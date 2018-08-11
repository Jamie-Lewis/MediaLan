<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MediaLan.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        body {background: #080707; }
        ul li { display: inline-block; float: left; width: 25%; min-width: 250px;vertical-align:top; border: 1px solid #000; margin: 5px; padding: 5px; background: #fff;}
        ul li img { max-height: 175px; clear:both; }
        ul li:hover {
            border: 1px solid #f00;
            cursor: pointer;
            background:#efefef;
        }
    </style>

    <script src="https://dashif.org/reference/players/javascript/latest/dist/dash.all.debug.js"></script>
<style>
    video {
       width: 100%;
       height: 100%;
    }
</style>
</head>
<body>
    
   <div>
       <video data-dashjs-player preload="none" controls="true">
                <source src="http://localhost:54789/STREAM/Ozzy/manifest.mpd" type="application/dash+xml" />
            </video>
       <%--<video id="videoPlayer" data-dashjs-player src="http://localhost:54789/STREAM/Snowfall/Season%201/SNOWFALL - S01 E01 - Pilot (720p AMZN Web-DL)_video_160x90_250k_dash.mpd" controls></video>--%>
   </div>
    <form id="form1" runat="server">
        
            <asp:placeholder id="ph1" runat="server" />
        
            
    </form>
</body>
</html>

<%--<script>
// setup the video element and attach it to the Dash player
    function setupVideo() {
        var url = "";
        var context = new Dash.di.DashContext();
        var player = new MediaPlayer(context);
        player.startup();
        player.attachView(document.querySelector("#videoplayer"));
        player.attachSource(url);
    }

</script>--%>