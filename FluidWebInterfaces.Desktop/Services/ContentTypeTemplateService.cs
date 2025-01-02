using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using FluidWebInterfaces.Desktop.Models;

namespace FluidWebInterfaces.Desktop.Services;

public class ContentTypeTemplateService : IContentTypeTemplateService
{
    public ContentTypeTemplateService()
    {
        Initialize();
    }
    
    private string TemplatesFile => Path.Combine(PersistenceLocations.ApplicationData, "content-type-templates.xml");

    public IEnumerable<ContentTypeTemplate> GetAll()
    {
        XmlDocument document = new();
        document.Load(TemplatesFile);

        List<ContentTypeTemplate> templates = [];

        foreach (XmlElement node in document.DocumentElement!.ChildNodes)
        {
            templates.Add(new ContentTypeTemplate
            {
                ShortName = node.GetAttribute("name"),
                ContentType = node.GetAttribute("content-type")
            });
        }

        return templates;
    }

    private void Initialize()
    {
        if (!File.Exists(TemplatesFile))
        {
            File.WriteAllText(TemplatesFile,
                """
                <ContentTypeTemplates>
                    <Template name="Text" content-type="text/plain"/>
                    <Template name="HTML" content-type="text/html"/>
                    <Template name="CSS" content-type="text/css"/>
                    <Template name="JavaScript" content-type="text/javascript"/>
                    <Template name="JSON" content-type="application/json"/>
                    <Template name="XML" content-type="application/xml"/>
                </ContentTypeTemplates>
                """);
        }
    }
}