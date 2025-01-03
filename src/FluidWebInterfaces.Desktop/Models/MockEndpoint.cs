using System;

namespace FluidWebInterfaces.Desktop.Models;

public class MockEndpoint
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required int Port { get; set; }
    public required string Route { get; set; }
    public required bool Active { get; set; }
    public required string ResponseText { get; set; }
    public required string ResponseType { get; set; }
}