﻿using System;
using System.IO;
using Sinch.ServerSdk.Calling.Callbacks.Request;
using Sinch.ServerSdk.Calling.Models;
using Sinch.ServerSdk.IvrMenus;
using Sinch.ServerSdk.Models;

namespace Sinch.ServerSdk.Examples.CallingCallback
{
    class Program
    {
        static void Main(string[] args)
        {
            var iceRequestText = File.ReadAllText("../../ice.txt");

            var sinch = SinchFactory.CreateCallbackResponseFactory(Locale.EnUs);
            
            var evt = sinch.CreateEventReader().ReadJson(iceRequestText);

            if (evt is IIceEvent)
            {
                var iceEvent = evt as IIceEvent;
                Console.WriteLine("Ice Event");
                Console.WriteLine("From: " + iceEvent.Cli);
                Console.WriteLine("To: " + iceEvent.To.Endpoint);

                var menu = sinch.CreateMenuBuilder();
                menu.AddNumberInputMenu("ipt", new TtsPrompt("Hello!"), 4);

                var response = sinch.CreateIceSvamletBuilder();
                var iceResponseText = response.RunMenu("ipt", menu).Body;

               Console.WriteLine(iceResponseText);

                iceResponseText =
                    response
                        .Say("Hello world")
                        .Say("Will connect your call")
                        .SetCookie("mycookie", "myvalue")
                        .ConnectPstn("+46777888999")
                        .WithBridgeTimeout(TimeSpan.FromMinutes(2.5))
                        .WithAnonymousCli()
                        .WithCallTag(CallTag.DefaultBillingTag, "C02-14")
                        .WithCallTag("CustomCallTag", "foo")
                        .WithAnsweringMachineDetection().Body;

                Console.WriteLine(iceResponseText);
            }
        }
    }
}