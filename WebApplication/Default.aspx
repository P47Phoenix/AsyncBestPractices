<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

  
    <div>Thread Count</div>
    <div id="chart"></div>
    <div>
        <div id="EventGap"></div>
        <div id="DateTime"></div>
        <div id="AvailableWorkerThreads"></div>
        <div id="AvailableCompletionPortThreads"></div>
        <div id="MaxWorkerThreads"></div>
        <div id="MaxCompletionPortThreads"></div>
        <div id="MinWorkerThreads"></div>
        <div id="MinCompletionPortThreads"></div>
    </div>
    <script type="text/javascript" >
        (function() {

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
            },
            {
                y: [0],
                type: 'bar',
                name: 'ms > 200 Slowdown detected',
                marker: {
                    color: 'rgb(85,0,19)'
                }
            }]);

            var timeoutId = null;

            function setDelayedResponse(data) {
                timeoutId = window.setTimeout(function() {
                    Plotly.extendTraces('chart', { y: [[data.AvailableWorkerThreads], [data.AvailableCompletionPortThreads], [5]] }, [0,1,2], 120);
                }, 200);

            }


            function epochMilliseconds() {
                var d = new Date();
                return Math.floor(d.getTime());
            }

            var lastMilliseconds;
            var lastGap = 0;
            exampleSocket.onmessage = function(event) {
                if (timeoutId) {
                    window.clearTimeout(timeoutId);
                }
                var milliseconds = epochMilliseconds();
                var data = jQuery.parseJSON(event.data);
                Plotly.extendTraces('chart',
                    { y: [[data.AvailableWorkerThreads], [data.AvailableCompletionPortThreads], [0]] },
                    [0, 1, 2],
                    120);
                
                jQuery.each(data,
                    function(index, value) {
                        $('#' + index).text(index + ":" + value);
                    });
                if (lastMilliseconds) {
                    var gap = milliseconds - lastMilliseconds;
                    if (lastGap < gap) {
                        lastGap = gap;
                        $('#EventGap').text('Largest slowdown Milliseconds:' + (gap));
                    }
                }
                lastMilliseconds = milliseconds;
                setDelayedResponse(data);
            }
            })();
    </script>


</asp:Content>
