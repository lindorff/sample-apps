using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DotNetExample;

internal interface IAssignmentApiService
{
    Task<HttpResponseMessage> SendAssignment(string assignmentId);
}

internal class AssignmentApiService : IAssignmentApiService
{
    private readonly HttpClient _httpClient;

    public AssignmentApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.lowell.com/test/collection/fi/assignment-api/v1/");
    }

    public async Task<HttpResponseMessage> SendAssignment(string assignmentId)
    {
        // TODO: Use abstractions so that InitializeHttpClient is run for every request implicitly
        await InitializeHttpClient();

        var bodyModel = new PostAssignmentBody()
        {
            Id = assignmentId,
            Reference = "TestReference",
            SubclientId = "123456",
            Type = "Reminder",
            Debtors = new Debtor[]
            {
                new Debtor()
                {
                    City = "TestCity",
                    CompanyName = "TestCompany",
                    Country = "FI",
                    Id = "3440564-7",
                    Nationality = "FI",
                    Phone = "04012341234",
                    Role = "Main",
                    Street = "Test 123",
                    Zip = "12345",
                    Type = "Company",
                    VoluntaryCollectionForbidden = false
                }
            },
            Invoices = new Invoice[] {
                new Invoice() {
                    Description = "Test",
                    DueDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    InterestDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    InvoiceDate = DateTime.Now.Subtract(TimeSpan.FromDays(15)).ToString("yyyy-MM-dd"),
                    InterestRateCode = "FI_Consumer",
                    InvoiceNumber = "123456",
                    OpenPrincipal = new decimal(300.5),
                    ReceivableType = "Sales",
                    ReferenceNumber = "12345667788865321"
                }
            }
        };

        var serializedBodyModel = JsonSerializer.Serialize(bodyModel, options: new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var body = new StringContent(
            serializedBodyModel, Encoding.UTF8, "application/json"
        );

        var resp = await _httpClient.PostAsync(
            "assignments",
            body
        );

        if (!resp.IsSuccessStatusCode) throw new Exception($"POSTing assignment failed: {await resp.Content.ReadAsStringAsync()}");

        return resp;
    }

    private async Task InitializeHttpClient()
    {
        var httpClient = new HttpClient();

        using var request = new ClientCredentialsTokenRequest
        {
            Address = "https://idp.lowell.io/auth/realms/test-clients/protocol/openid-connect/token",
            GrantType = "client_credentials",
            ClientId = "testClientId",
            ClientSecret = "xxxxxx"
        };

        // TODO: Store the token somewhere and re-use it
        var response = await httpClient.RequestClientCredentialsTokenAsync(request);

        if (!response.HttpResponse.IsSuccessStatusCode) throw new Exception($"Fetching token was unsuccessful: {response.Raw}");

        _httpClient.SetBearerToken(response.AccessToken);
    }

    private class PostAssignmentBody
    {
        public string? Id { get; set; }
        public Debtor[]? Debtors { get; set; }
        public Invoice[]? Invoices { get; set; }
        public string? Reference { get; set; }
        public string? SubclientId { get; set; }
        public string? Type { get; set; }
    }

    private class Debtor
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? CompanyName { get; set; }
        public string? Country { get; set; }
        public string? Nationality { get; set; }
        public string? Street { get; set; }
        public string? Zip { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public bool VoluntaryCollectionForbidden { get; set; }
    }

    private class Invoice
    {
        public string? Description { get; set; }
        public string? DueDate { get; set; }
        public string? InterestDate { get; set; }
        public string? InterestRate { get; set; }
        public string? InterestRateCode { get; set; }
        public string? InvoiceDate { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal OpenPrincipal { get; set; }
        public string? ReceivableType { get; set; }
        public string? ReferenceNumber { get; set; }
    }
}
