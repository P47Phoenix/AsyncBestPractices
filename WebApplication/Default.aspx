<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

  
    <div>Thread Count</div>
    <div id="chart"></div>
    <div>
        <div id="DateTime"></div>
        <div id="AvailableWorkerThreads"></div>
        <div id="AvailableCompletionPortThreads"></div>
        <div id="MaxWorkerThreads"></div>
        <div id="MaxCompletionPortThreads"></div>
        <div id="MinWorkerThreads"></div>
        <div id="MinCompletionPortThreads"></div>
    </div>
    <script type="text/javascript" >
        var exampleSocket = new WebSocket("wss://localhost:44303/threadpool");

        Plotly.plot('chart', [{
            y: [0],
            type: 'line',
            name: 'AvailableWorkerThreads'
        },
        {
            y: [0],
            type: 'line',
            name: 'AvailableCompletionPortThreads'
        }]);

        exampleSocket.onmessage = function (event) {
            var data = jQuery.parseJSON(event.data);
            Plotly.extendTraces('chart', { y: [[data.AvailableWorkerThreads], [data.AvailableCompletionPortThreads]] }, [0,1]);

            jQuery.each(data,
                function(index, value) {
                    $('#' + index).text(index + ":" + value);
                });
        }
    </script>


</asp:Content>
