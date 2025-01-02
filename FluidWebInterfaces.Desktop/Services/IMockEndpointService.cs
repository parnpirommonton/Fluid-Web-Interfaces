using System;
using System.Collections.Generic;
using FluidWebInterfaces.Desktop.Models;

namespace FluidWebInterfaces.Desktop.Services;

public interface IMockEndpointService
{
    IEnumerable<MockEndpoint> GetAll();
    MockEndpoint? GetById(Guid id);
    bool DeleteById(Guid id);
    bool Create(MockEndpoint instance);
    bool Update(MockEndpoint instance);
}