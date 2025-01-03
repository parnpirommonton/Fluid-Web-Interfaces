using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FluidWebInterfaces.Desktop.Models;
using FluidWebInterfaces.Desktop.Services;

namespace FluidWebInterfaces.Desktop.ViewModels;

public partial class EditMockEndpointViewModel : ViewModelBase
{
    private readonly IMockEndpointService _mockEndpointService;
    private readonly IContentTypeTemplateService _templateService;

    [ObservableProperty]
    private ActiveMockEndpoint? _currentInstance;

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
    private string _title = "";

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

    public EditMockEndpointViewModel(IMockEndpointService mockEndpointService, IContentTypeTemplateService templateService)
    {
        _mockEndpointService = mockEndpointService;
        _templateService = templateService;
    }

    public override void Initialize(object? parameter = null)
    {
        var instance = (ActiveMockEndpoint?)parameter;

        if (instance is null)
        {
            throw new Exception(
                $"Parameter {nameof(instance)} must be provided to view model {nameof(EditMockEndpointViewModel)} when initializing.");
        }

        CurrentInstance = instance;
        
        Templates = new ObservableCollection<ContentTypeTemplate>(_templateService.GetAll());
        var selectedTemplate =
            Templates.SingleOrDefault(template => template.ContentType == CurrentInstance.ResponseType);

        Name = CurrentInstance.Name;
        Port = CurrentInstance.Port.ToString();
        Route = CurrentInstance.Route;
        ResponseText = CurrentInstance.ResponseText;
        ResponseType = CurrentInstance.ResponseType;

        Title = "Editing Active Instance";

        if (selectedTemplate is not null)
        {
            SelectedTemplate = selectedTemplate;
        }
    }

    public bool TryUpdateMockEndpoint()
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

        _mockEndpointService.Update(
            new MockEndpoint
            {
                Id = CurrentInstance!.Id,
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