using System;
using System.Collections.Generic;
using FluidWebInterfaces.Desktop.Models;

namespace FluidWebInterfaces.Desktop.Services;

public interface IContentTypeTemplateService
{
    IEnumerable<ContentTypeTemplate> GetAll();
}