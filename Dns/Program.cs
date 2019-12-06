// // //------------------------------------------------------------------------------------------------- 
// // // <copyright file="Program.cs" company="stephbu">
// // // Copyright (c) Steve Butler. All rights reserved.
// // // </copyright>
// // //-------------------------------------------------------------------------------------------------

namespace Dns
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;
    using Dns.ZoneProvider.AP;
    using Ninject;
    using System.Reflection;

    public class Program
    {
        private class Plugins {
            public Contracts.IDnsCache Cache {get;set;}
            public ZoneProvider.BaseZoneProvider ZoneProvider {get;set;}

            public Contracts.IDnsResolver Resolver {get;set;}
        }

        private static DnsServer _dnsServer; // resolver and delegated lookup for unsupported zones;
        private static HttpServer _httpServer;
        private static ManualResetEvent _exit = new ManualResetEvent(false);
        private static ManualResetEvent _exitTimeout = new ManualResetEvent(false);

        private static Plugins plugins = new Plugins();

        public static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            // Ninject DI controlled
            // see bindings.cs for configured bindings
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            plugins.Cache = kernel.Get<Contracts.IDnsCache>();
            plugins.Resolver = kernel.Get<Contracts.IDnsResolver>();
            plugins.ZoneProvider = kernel.Get<ZoneProvider.BaseZoneProvider>();

            // wire up plugin dependencies
            plugins.Resolver.SubscribeTo(plugins.ZoneProvider);

            // TODO: read zone data and select ZoneProvider from configuration
            _dnsServer = new DnsServer();
            _httpServer = new HttpServer();

            _dnsServer.Initialize(plugins.Resolver);
            _httpServer.Initialize("http://+:8080/");
            _httpServer.OnProcessRequest += _httpServer_OnProcessRequest;
            _httpServer.OnHealthProbe += _httpServer_OnHealthProbe;

            plugins.ZoneProvider.Start();
            _dnsServer.Start();
            _httpServer.Start();

            _exit.WaitOne();

            _httpServer.Stop();
            _dnsServer.Stop();
            plugins.ZoneProvider.Stop();

            _exitTimeout.Set();
        }

        static void _httpServer_OnHealthProbe(HttpListenerContext context)
        {
        }

        private static void _httpServer_OnProcessRequest(HttpListenerContext context)
        {
            string rawUrl = context.Request.RawUrl;
            if (rawUrl == "/dump/dnsresolver")
            {
                context.Response.Headers.Add("Content-Type","text/html");
                using (TextWriter writer = context.Response.OutputStream.CreateWriter())
                {
                    plugins.Resolver.DumpHtml(writer);
                }
            }
            else if (rawUrl == "/dump/httpserver")
            {
                context.Response.Headers.Add("Content-Type", "text/html");
                using (TextWriter writer = context.Response.OutputStream.CreateWriter())
                {
                    _httpServer.DumpHtml(writer);
                }
            }
            else if (rawUrl == "/dump/dnsserver")
            {
                context.Response.Headers.Add("Content-Type", "text/html");
                using (TextWriter writer = context.Response.OutputStream.CreateWriter())
                {
                    _dnsServer.DumpHtml(writer);
                }
            }
            else if (rawUrl == "/dump/zoneprovider")
            {
                context.Response.Headers.Add("Content-Type", "text/html");
                using (TextWriter writer = context.Response.OutputStream.CreateWriter())
                {
                    // TODO: Implement Zone Provider dump
                    //plugins.ZoneProvider.DumpHtml(writer);
                }
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _exit.Set();
            _exitTimeout.WaitOne(5000);
        }
    }
}