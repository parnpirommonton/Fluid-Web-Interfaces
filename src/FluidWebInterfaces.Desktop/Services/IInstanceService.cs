using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluidWebInterfaces.Desktop.Models;

namespace FluidWebInterfaces.Desktop.Services;

public interface IInstanceService
{
    IEnumerable<Instance> GetAll();
    Instance? GetById(Guid id);
    bool DeleteById(Guid id);
    bool Create(Instance instance);
    bool Update(Instance instance);
}