using System;
using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using FluidWebInterfaces.Desktop.Models;
using FluidWebInterfaces.Desktop.Services;

namespace FluidWebInterfaces.Desktop.ViewModels;

public partial class CreateNewMockEndpointViewModel : ViewModelBase
{
    private readonly IMockEndpointService _mockEndpointService;
    private readonly IContentTypeTemplateService _templateService;

    [ObservableProperty]
    private string _name = "";
    
    [ObservableProperty]
    private string _port = "";

    [ObservableProperty]
    private string _route = "";

    [ObservableProperty]
    private string _responseText = "";

    [ObservableProperty]
    private string _responseType = "";
    
    [ObservableProperty]
    private string _issueMessage = "";

    [ObservableProperty]
    private ObservableCollection<ContentTypeTemplate> _templates = [];

    private ContentTypeTemplate? _selectedTemplate;

    public ContentTypeTemplate? SelectedTemplate
    {
        get => _selectedTemplate;
        set
        {
            _selectedTemplate = value;
            if (_selectedTemplate is not null)
            {
                ResponseType = _selectedTemplate.ContentType;
            }
            OnPropertyChanged();
        }
    }

    public CreateNewMockEndpointViewModel(IMockEndpointService mockEndpointService, IContentTypeTemplateService templateService)
    {
        _mockEndpointService = mockEndpointService;
        _templateService = templateService;
    }

    public override void Initialize(object? parameter = null)
    {
        Templates = new ObservableCollection<ContentTypeTemplate>(_templateService.GetAll());
    }

    public bool TryCreateMockEndpoint()
    {
        IssueMessage = "";
        
        if (Name is "")
        {
            IssueMessage = "Please provide a name for the mock endpoint.";
            return false;
        }
        
        if (Route is "")
        {
            IssueMessage = "Please provide a route for this endpoint.";
            return false;
        }
        
        if (Port is "" || !int.TryParse(Port, out int port))
        {
            IssueMessage = "Please provide a valid port for this endpoint.";
            return false;
        }

        if (port is < 0 or > 65_535)
        {
            IssueMessage = "Please provide a port in between 0 and 65535 (Ideally a port between 1024 and 65535).";
            return false;
        }

        _mockEndpointService.Create(
            new MockEndpoint
            {
                Id = Guid.NewGuid(),
                Name = Name,
                Active = false,
                Port = port,
                Route = Route,
                ResponseText = ResponseText,
                ResponseType = ResponseType
            });

        return true;
    }
}