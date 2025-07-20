using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazorApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MudBlazorApp.Pages
{
    public partial class Country : ComponentBase
    {
        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        protected NavigationManager Navigation { get; set; } = default!;

        [Inject]
        protected HttpClient Http { get; set; } = default!;

        [Inject]
        protected IDialogService DialogService { get; set; } = default!;

        [Parameter]
        public int? Id { get; set; }

        private string _searchString = "";
        protected string searchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                StateHasChanged();
            }
        }
        protected int elementsCount = 0;

        // Inline editing variables
        protected int? editingCountryId = null;
        protected string editingName = "";
        protected int editingId = 0;
        protected bool editingVerified = false;

        protected List<CountryModel> _countries = new List<CountryModel>();
        protected CountryModel? selectedCountry = null;
        protected bool isLoadingDetails = false;
        protected bool showingDetails = false;

        private string _sortOption = "custom";
        protected string SortOption
        {
            get => _sortOption;
            set
            {
                _sortOption = value;
                StateHasChanged();
            }
        }

        protected List<CountryModel> Countries
        {
            get => GetSortedCountries();
            set => _countries = value;
        }

        protected bool IsLoading { get; set; } = true;
        protected string ErrorMessage { get; set; } = string.Empty;

        public async Task OpenAddCountryDialog()
        {
            var parameters = new DialogParameters();
            parameters.Add("ExistingIds", _countries.Select(c => c.Id).ToList());
            var dialog = DialogService.Show<AddCountryDialog>("Add New Country", parameters);
            var result = await dialog.Result;
            if (!result.Canceled && result.Data is CountryModel newCountry)
            {
                _countries.Add(newCountry);
                Snackbar.Add($"New country added with ID {newCountry.Id}", Severity.Success);
                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadCountriesAsync();
        }

        private List<CountryModel> GetSortedCountries()
        {
            if (_countries == null || !_countries.Any())
                return new List<CountryModel>();

            var filteredCountries = _countries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                filteredCountries = filteredCountries.Where(c =>
                    c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    c.Id.ToString().Contains(searchString) ||
                    c.Mode.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            return SortOption switch
            {
                "name_asc" => filteredCountries.OrderBy(c => c.Name).ToList(),
                "name_desc" => filteredCountries.OrderByDescending(c => c.Name).ToList(),
                "id_asc" => filteredCountries.OrderBy(c => c.Id).ToList(),
                "id_desc" => filteredCountries.OrderByDescending(c => c.Id).ToList(),
                "verified_asc" => filteredCountries.OrderBy(c => c.Verified).ToList(),
                "verified_desc" => filteredCountries.OrderByDescending(c => c.Verified).ToList(),
                "last_modified_asc" => filteredCountries.OrderBy(c => c.LastModified).ToList(),
                "last_modified_desc" => filteredCountries.OrderByDescending(c => c.LastModified).ToList(),
                "created_asc" => filteredCountries.OrderBy(c => c.CreatedOn).ToList(),
                "created_desc" => filteredCountries.OrderByDescending(c => c.CreatedOn).ToList(),
                "custom" => filteredCountries.ToList(),
                _ => filteredCountries.ToList()
            };
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Id.HasValue)
            {
                await LoadCountryDetails(Id.Value);
            }
            else
            {
                showingDetails = false;
                selectedCountry = null;
            }
        }

        private async Task LoadCountriesAsync()
        {
            IsLoading = true;
            Countries.Clear();
            ErrorMessage = string.Empty;

            try
            {
                var apiUrl = "http://localhost:5200/api/country";
                var result = await Http.GetFromJsonAsync<List<CountryModel>>(apiUrl);

                if (result != null)
                {
                    _countries = result;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                _countries = new List<CountryModel>
                {
                    new CountryModel { Id = 1, Name = "United States (Mock)", Verified = true, Mode = "Active", CreatedOn = DateTime.Now.AddDays(-30), LastModified = DateTime.Now.AddDays(-5) },
                    new CountryModel { Id = 2, Name = "Canada (Mock)", Verified = true, Mode = "Active", CreatedOn = DateTime.Now.AddDays(-25), LastModified = DateTime.Now.AddDays(-3) },
                    new CountryModel { Id = 3, Name = "Mexico (Mock)", Verified = false, Mode = "Inactive", CreatedOn = DateTime.Now.AddDays(-20), LastModified = DateTime.Now.AddDays(-1) },
                    new CountryModel { Id = 4, Name = "United Kingdom (Mock)", Verified = true, Mode = "Active", CreatedOn = DateTime.Now.AddDays(-15), LastModified = DateTime.Now.AddDays(-2) }
                };

                Snackbar.Add("Using mock data because API is not available", Severity.Warning);

                if (Id.HasValue)
                {
                    var mockCountry = _countries.FirstOrDefault(c => c.Id == Id.Value);
                    if (mockCountry != null)
                    {
                        selectedCountry = mockCountry;
                        showingDetails = true;
                    }
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected void AddNew()
        {
            var newCountry = new CountryModel
            {
                Id = _countries.Count > 0 ? _countries.Max(c => c.Id) + 1 : 1,
                Name = "New Country",
                Verified = false,
                Mode = "Active",
                CreatedOn = DateTime.Now,
                LastModified = DateTime.Now
            };

            _countries.Add(newCountry);
            Snackbar.Add($"New country added with ID {newCountry.Id}", Severity.Success);
            StateHasChanged();
        }

        protected void StartEdit(CountryModel country)
        {
            editingCountryId = country.Id;
            editingName = country.Name;
            editingId = country.Id;
            editingVerified = country.Verified;
            StateHasChanged();
        }

        protected void SaveEdit(CountryModel country)
        {
            if (editingId != country.Id && _countries.Any(c => c.Id == editingId))
            {
                Snackbar.Add($"ID {editingId} already exists. Please choose a different ID.", Severity.Error);
                return;
            }

            country.Id = editingId;
            country.Name = editingName;
            country.Verified = editingVerified;
            country.LastModified = DateTime.Now;

            editingCountryId = null;
            editingName = "";
            editingId = 0;
            editingVerified = false;

            Snackbar.Add($"Country updated successfully", Severity.Success);
            StateHasChanged();
        }

        protected void OnVerifiedChanged(bool value)
        {
            editingVerified = value;
            StateHasChanged();
        }

        protected void CancelEdit()
        {
            editingCountryId = null;
            editingName = "";
            editingId = 0;
            editingVerified = false;
            StateHasChanged();
        }

        protected bool IsEditing(CountryModel country)
        {
            return editingCountryId == country.Id;
        }

        protected void BackToList()
        {
            Navigation.NavigateTo("/country");
        }

        protected async Task LoadCountryDetails(int id)
        {
            try
            {
                isLoadingDetails = true;
                selectedCountry = _countries.FirstOrDefault(c => c.Id == id);

                if (selectedCountry != null)
                {
                    showingDetails = true;
                    Snackbar.Add($"Loaded details for {selectedCountry.Name}", Severity.Success);
                }
                else
                {
                    try
                    {
                        var apiUrl = $"http://localhost:5200/api/country/{id}";
                        selectedCountry = await Http.GetFromJsonAsync<CountryModel>(apiUrl);
                        showingDetails = true;
                    }
                    catch
                    {
                        Snackbar.Add($"Country with ID {id} not found", Severity.Warning);
                        selectedCountry = null;
                        showingDetails = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading details: {ex.Message}", Severity.Error);
                selectedCountry = null;
                showingDetails = false;
            }
            finally
            {
                isLoadingDetails = false;
            }
        }

        protected void ViewDetails(CountryModel country)
        {
            Navigation.NavigateTo($"/country/{country.Id}");
        }

        public class ConfigSettings
        {
            public ConnectionStringsConfig? ConnectionStrings { get; set; }
        }

        public class ConnectionStringsConfig
        {
            public string? DefaultConnection { get; set; }
        }
    }
}
