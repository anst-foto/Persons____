using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Persons.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Persons.Client;

public partial class MainWindowViewModel : ReactiveObject
{
    private HttpClient _client = new();
    
    [Reactive] private int? _id;
    [Reactive] private string? _lastName;
    [Reactive] private string? _firstName;
    
    public ObservableCollection<Person> People { get; } = [];
    [Reactive] private Person? _selectedPerson;
    
    private IObservable<bool> CanDelete => 
        this.WhenAnyValue(x => x.SelectedPerson, 
            x => x.Id,
            (p, i) => p != null || i != null);

    public MainWindowViewModel()
    {
        this.WhenAnyValue(x => x.SelectedPerson)
            .Subscribe(p =>
            {
                Id = p?.Id;
                LastName = p?.LastName;
                FirstName = p?.FirstName;
            });
    }

    [ReactiveCommand]
    private async Task Load()
    {
        const string url = "http://localhost:5056/persons";
        var people = await _client.GetFromJsonAsync<Person[]>(url);
        
        People.Clear();
        foreach (var person in people!)
        {
            People.Add(person);
        }
    }
    
    [ReactiveCommand(CanExecute = nameof(CanDelete))]
    private async Task Delete()
    {
        var url = $"http://localhost:5056/persons/{SelectedPerson!.Id}";
        await _client.DeleteAsync(url);
    }
}