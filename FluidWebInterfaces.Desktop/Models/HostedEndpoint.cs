using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluidWebInterfaces.Desktop.Models;

public class HostedEndpoint
{
    public Guid MockEndpointId;
    private readonly Task _listeningTask;
    private readonly HttpListener _listener = new();
    private bool _shuttingDown = false;

    public HostedEndpoint(ActiveMockEndpoint endpoint)
    {
        MockEndpointId = endpoint.Id;
        _listeningTask = Task.Run(() => HandleClients(endpoint));
    }

    public void Shutdown()
    {
        if (_shuttingDown)
        {
            return;
        }
        
        _shuttingDown = true;
        _listener.Stop();
        _listener.Close();
        ((IDisposable)_listener).Dispose();
    }

    public void HandleClients(ActiveMockEndpoint endpoint)
    {
        if (endpoint.Route[0] is not '/')
        {
            endpoint.Route = $"/{endpoint.Route}";
        }
        if (endpoint.Route[^1] is not '/')
        {
            endpoint.Route = $"{endpoint.Route}/";
        }
        
        endpoint.SaveMockEndpoint();

        _listener.Prefixes.Add($"http://localhost:{endpoint.Port}{endpoint.Route}");
        _listener.Start();
        
        while (!_shuttingDown)
        {
            var context = _listener.GetContext();
            
            if (!endpoint.Active || _shuttingDown)
            {
                context.Response.StatusCode = 404;
                context.Response.Abort();
                continue;
            }
        
            byte[] buffer = Encoding.UTF8.GetBytes(endpoint.ResponseText);
            context.Response.OutputStream.Write(buffer);
            context.Response.ContentType = endpoint.ResponseType;
            context.Response.StatusCode = 200;
            context.Response.OutputStream.Close();
        }
            
        _listener.Stop();
    }
}