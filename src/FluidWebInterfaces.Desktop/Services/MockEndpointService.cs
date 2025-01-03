using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using FluidWebInterfaces.Desktop.Models;

namespace FluidWebInterfaces.Desktop.Services;

public class MockEndpointService : IMockEndpointService
{
    public MockEndpointService()
    {
        Initialize();
    }
    
    private string EndpointFile => Path.Combine(PersistenceLocations.ApplicationData, "active-endpoints.xml");
    
    public IEnumerable<MockEndpoint> GetAll()
    {
        XmlDocument document = new();
        document.Load(EndpointFile);

        List<MockEndpoint> mockEndpoints = [];

        foreach (XmlElement node in document.DocumentElement!.ChildNodes)
        {
            mockEndpoints.Add(new MockEndpoint
            {
                Id = Guid.Parse(node.GetAttribute("id")),
                Name = node.GetAttribute("name"),
                Port = int.Parse(node.GetAttribute("port")),
                Route = node.GetAttribute("route"),
                ResponseText = node.GetAttribute("text"),
                ResponseType = node.GetAttribute("type"),
                Active = Convert.ToBoolean(node.GetAttribute("active"))
            });
        }

        return mockEndpoints;
    }

    public MockEndpoint? GetById(Guid id)
    {
        XmlDocument document = new();
        document.Load(EndpointFile);

        var node = (XmlElement?)document.SelectSingleNode($"/MockEndpoints/MockEndpoint[@id='{id}']");

        if (node is null)
        {
            return null;
        }

        return new MockEndpoint
        {
            Id = Guid.Parse(node.GetAttribute("id")),
            Name = node.GetAttribute("name"),
            Port = int.Parse(node.GetAttribute("port")),
            Route = node.GetAttribute("route"),
            ResponseText = node.GetAttribute("text"),
            ResponseType = node.GetAttribute("type"),
            Active = Convert.ToBoolean(node.GetAttribute("active"))
        };
    }

    public bool DeleteById(Guid id)
    {
        XmlDocument document = new();
        document.Load(EndpointFile);

        var node = document.SelectSingleNode($"/MockEndpoints/MockEndpoint[@id='{id}']");
        
        if (node is null)
        {
            return false;
        }

        document.DocumentElement!.RemoveChild(node);
        document.Save(EndpointFile);
        return true;
    }

    public bool Create(MockEndpoint instance)
    {
        XmlDocument document = new();
        document.Load(EndpointFile);
        
        var existingEndpoint = document.SelectSingleNode($"/MockEndpoints/MockEndpoint[@id='{instance.Id}']");
        
        if (existingEndpoint is not null)
        {
            return false;
        }

        var element = document.CreateElement("MockEndpoint");
        
        element.SetAttribute("id", instance.Id.ToString());
        element.SetAttribute("name", instance.Name);
        element.SetAttribute("port", instance.Port.ToString());
        element.SetAttribute("route", instance.Route);
        element.SetAttribute("active", instance.Active.ToString());
        element.SetAttribute("text", instance.ResponseText);
        element.SetAttribute("type", instance.ResponseType);
        
        document.DocumentElement!.AppendChild(element);
        
        document.Save(EndpointFile);

        return true;
    }

    public bool Update(MockEndpoint instance)
    {
        XmlDocument document = new();
        document.Load(EndpointFile);

        var element = (XmlElement?)document.SelectSingleNode($"/MockEndpoints/MockEndpoint[@id='{instance.Id}']");
        
        if (element is null)
        {
            return false;
        }
        
        element.SetAttribute("id", instance.Id.ToString());
        element.SetAttribute("name", instance.Name);
        element.SetAttribute("port", instance.Port.ToString());
        element.SetAttribute("route", instance.Route);
        element.SetAttribute("active", instance.Active.ToString());
        element.SetAttribute("text", instance.ResponseText);
        element.SetAttribute("type", instance.ResponseType);
        
        document.Save(EndpointFile);
        
        return true;
    }

    private void Initialize()
    {
        if (!File.Exists(EndpointFile))
        {
            File.WriteAllText(EndpointFile,
                """
                <MockEndpoints>
                </MockEndpoints>
                """);
        }
    }
}