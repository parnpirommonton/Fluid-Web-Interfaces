using System;
using FluidWebInterfaces.Desktop.Models;
using FluidWebInterfaces.Desktop.Services;

namespace FluidWebInterfaces.Desktop.Mapping;

public static class MockEndpointMapping
{
    public static void MapFromMockEndpoint(this ActiveMockEndpoint activeMockEndpoint,
        MockEndpoint mockEndpoint)
    {
        activeMockEndpoint.Id = mockEndpoint.Id;
        activeMockEndpoint.Name = mockEndpoint.Name;
        activeMockEndpoint.Port = mockEndpoint.Port;
        activeMockEndpoint.Route = mockEndpoint.Route;
        activeMockEndpoint.Active = mockEndpoint.Active;
        activeMockEndpoint.ResponseText = mockEndpoint.ResponseText;
        activeMockEndpoint.ResponseType = mockEndpoint.ResponseType;
    }

    public static ActiveMockEndpoint ActivateMockEndpoint(this MockEndpoint mockEndpoint, HostingService hostingService)
    {
        return new ActiveMockEndpoint(
            hostingService,
            mockEndpoint.Id,
            mockEndpoint.Name,
            mockEndpoint.Port,
            mockEndpoint.Route,
            mockEndpoint.Active,
            mockEndpoint.ResponseText,
            mockEndpoint.ResponseType);
    }

    public static MockEndpoint ToMockEndpoint(this ActiveMockEndpoint mockEndpoint)
    {
        return new MockEndpoint
        {
            Id = mockEndpoint.Id,
            Name = mockEndpoint.Name,
            Port = mockEndpoint.Port,
            Route = mockEndpoint.Route,
            Active = mockEndpoint.Active,
            ResponseText = mockEndpoint.ResponseText,
            ResponseType = mockEndpoint.ResponseType
        };
    }
}