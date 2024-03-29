﻿using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.IO.Pipelines;
using System.Reflection;

namespace BlazorWasm.Server.Middleware;

internal sealed class ClearTextHttpMultiplexingMiddleware
{
    private readonly ConnectionDelegate _next;

    // HTTP/2 prior knowledge-mode connection preface
    private static readonly byte[] Http2Preface =
    {
        0x50, 0x52, 0x49, 0x20, 0x2a, 0x20, 0x48, 0x54, 0x54, 0x50, 0x2f, 0x32, 0x2e, 0x30, 0x0d, 0x0a, 0x0d, 0x0a,
        0x53, 0x4d, 0x0d, 0x0a, 0x0d, 0x0a
    }; //PRI * HTTP/2.0\r\n\r\nSM\r\n\r\n

    public ClearTextHttpMultiplexingMiddleware(ConnectionDelegate next)
    {
        _next = next;
    }

    private static async Task<bool> HasHttp2Preface(PipeReader input)
    {
        while (true)
        {
            var result = await input.ReadAsync().ConfigureAwait(false);
            try
            {
                int pos = 0;
                foreach (var x in result.Buffer)
                {
                    for (var i = 0; i < x.Span.Length && pos < Http2Preface.Length; i++)
                    {
                        if (Http2Preface[pos] != x.Span[i]) return false;
                        pos++;
                    }
                    if (pos >= Http2Preface.Length) return true;
                }

                if (result.IsCompleted) return false;
            }
            finally
            {
                input.AdvanceTo(result.Buffer.Start);
            }
        }
    }

    private static void SetProtocols(object target, HttpProtocols protocols)
    {
        var field = target.GetType()
            .GetField("_endpointDefaultProtocols", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            // Ignore for https
            return;
        }

        field.SetValue(target, protocols);
    }

    public async Task OnConnectAsync(ConnectionContext context)
    {
        var hasHttp2Preface = await HasHttp2Preface(context.Transport.Input).ConfigureAwait(false);
        SetProtocols(_next.Target!, hasHttp2Preface ? HttpProtocols.Http2 : HttpProtocols.Http1);
        await _next(context).ConfigureAwait(false);
    }
}

