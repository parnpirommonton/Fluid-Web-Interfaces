using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using FluidWebInterfaces.Desktop.Models;

namespace FluidWebInterfaces.Desktop.Services;

public class InstanceService : IInstanceService
{
    public InstanceService()
    {
        Initialize();
    }
    
    private string InstanceFile => Path.Combine(PersistenceLocations.ApplicationData, "active-instances.xml");
    
    public IEnumerable<Instance> GetAll()
    {
        XmlDocument document = new();
        document.Load(InstanceFile);

        List<Instance> instances = [];
        
        foreach (XmlElement node in document.DocumentElement!.ChildNodes)
        {
            instances.Add(
                new Instance
                {
                    Id = Guid.Parse(node.GetAttribute("id")),
                    Name = node.GetAttribute("name"),
                    Active = Convert.ToBoolean(node.GetAttribute("active")),
                    SourcePath = node.GetAttribute("source-path"),
                    CompiledPath = node.GetAttribute("compiled-path"),
                    RestartOnError = Convert.ToBoolean(node.GetAttribute("restart-on-error")),
                    ShowErrorMessages = Convert.ToBoolean(node.GetAttribute("show-error-messages"))
                }
            );
        }

        return instances.AsEnumerable();
    }

    public Instance? GetById(Guid id)
    {
        XmlDocument document = new();
        document.Load(InstanceFile);
        
        var node = (XmlElement?)document.SelectSingleNode($"/Instances/Instance[@id='{id}']");

        if (node is null)
        {
            return null;
        }

        return 
            new Instance
            {
                Id = Guid.Parse(node.GetAttribute("id")),
                Name = node.GetAttribute("name"),
                Active = Convert.ToBoolean(node.GetAttribute("active")),
                SourcePath = node.GetAttribute("source-path"),
                CompiledPath = node.GetAttribute("compiled-path"),
                RestartOnError = Convert.ToBoolean(node.GetAttribute("restart-on-error")),
                ShowErrorMessages = Convert.ToBoolean(node.GetAttribute("show-error-messages"))
            };
    }

    public bool DeleteById(Guid id)
    {
        XmlDocument document = new();
        document.Load(InstanceFile);

        var node = document.SelectSingleNode($"/Instances/Instance[@id='{id}']");
        
        if (node is null)
        {
            return false;
        }

        document.DocumentElement!.RemoveChild(node);
        document.Save(InstanceFile);
        return true;
    }

    public bool Create(Instance instance)
    {
        var existingInstance = GetById(instance.Id);
        
        if (existingInstance is not null)
        {
            return false;
        }
        
        XmlDocument document = new();
        document.Load(InstanceFile);

        var element = document.CreateElement("Instance");
        element.SetAttribute("id", instance.Id.ToString());
        element.SetAttribute("name", instance.Name);
        element.SetAttribute("active", instance.Active.ToString());
        element.SetAttribute("source-path", instance.SourcePath);
        element.SetAttribute("compiled-path", instance.CompiledPath);
        element.SetAttribute("restart-on-error", instance.RestartOnError.ToString());
        element.SetAttribute("show-error-messages", instance.ShowErrorMessages.ToString());
        document.DocumentElement!.AppendChild(element);
        
        document.Save(InstanceFile);

        return true;
    }

    public bool Update(Instance instance)
    {
        XmlDocument document = new();
        document.Load(InstanceFile);

        var node = (XmlElement?)document.SelectSingleNode($"/Instances/Instance[@id='{instance.Id}']");
        
        if (node is null)
        {
            return false;
        }

        node.SetAttribute("name", instance.Name);
        node.SetAttribute("source-path", instance.SourcePath);
        node.SetAttribute("compiled-path", instance.CompiledPath);
        node.SetAttribute("active", instance.Active.ToString());
        node.SetAttribute("restart-on-error", instance.RestartOnError.ToString());
        node.SetAttribute("show-error-messages", instance.ShowErrorMessages.ToString());

        document.Save(InstanceFile);
        return true;
    }

    private void Initialize()
    {
        if (!File.Exists(InstanceFile))
        {
            File.WriteAllText(InstanceFile,
            """
            <Instances>
            </Instances>
            """);
        }
    }
}