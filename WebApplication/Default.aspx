﻿<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication._Default" %>

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
        <div id="Error"></div>
    </div>
    <script type="text/javascript" >
        (function() {

            var exampleSocket = new WebSocket("wss://localhost/WebApplication/threadpool");

            Plotly.plot('chart', [{
                y: [0],
                type: 'line',
                name: 'AvailableWorkerThreads',
                marker: {
                    color: 'rgb(0,6,182)'
                }
            },
            {
                y: [0],
                type: 'line',
                name: 'AvailableCompletionPortThreads',
                marker: {
                    color: 'rgb(68,151,84)'
                }
            },
            {
                y: [0],
                type: 'bar',
                name: 'ms > 200 Slowdown detected',
                marker: {
                    color: 'rgb(228,194,0)'
                }
            },
            {
                y: [0],
                type: 'bar',
                name: 'Error',
                marker: {
                    color: 'rgb(85,0,19)'
                }
            }]);

            var timeoutId = null;

            function setDelayedResponse(data) {
                timeoutId = window.setTimeout(function() {
                    Plotly.extendTraces('chart', 
                        { y: [[data.AvailableWorkerThreads], [data.AvailableCompletionPortThreads], [5], [0]] }, 
                        [0, 1, 2, 3], 
                        120);
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

                var error;

                if (data.Error) {
                    error = 5;
                } else {
                    error = 0;
                }

                Plotly.extendTraces('chart',
                    { y: [[data.AvailableWorkerThreads], [data.AvailableCompletionPortThreads], [0], [error]] },
                    [0, 1, 2, 3],
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
