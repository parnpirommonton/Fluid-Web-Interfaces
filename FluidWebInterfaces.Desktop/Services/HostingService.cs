using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluidWebInterfaces.Desktop.Models;

namespace FluidWebInterfaces.Desktop.Services;

public class HostingService
{
    private List<HostedEndpoint> _runningEndpoints = [];

    public void AddEndpoint(ActiveMockEndpoint endpoint)
    {
        var hostedEndpoint = _runningEndpoints.SingleOrDefault(
            hostedEndpoint => hostedEndpoint.MockEndpointId == endpoint.Id);

        if (hostedEndpoint is null)
        {
            _runningEndpoints.Add(new HostedEndpoint(endpoint));
        }
    }

    public void ShutdownEndpointById(Guid id)
    {
        var hostedEndpoint = _runningEndpoints.SingleOrDefault(endpoint => endpoint.MockEndpointId == id);

        if (hostedEndpoint is not null)
        {
            hostedEndpoint.Shutdown();
            _runningEndpoints.Remove(hostedEndpoint);
        }
    }

    public void ShutdownAllInstances()
    {
        foreach (var hostedEndpoint in _runningEndpoints)
        {
            hostedEndpoint.Shutdown();
        }
    }
}