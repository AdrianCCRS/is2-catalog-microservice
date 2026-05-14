#!/usr/bin/env dotnet script
#r "nuget: RestSharp, 107.3.0"

using RestSharp;
using System.Net;

ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

var client = new RestClient("http://localhost:5290");
var request = new RestRequest("/api/v1/catalog/products/bulk/demo", Method.Post);
request.AddHeader("Content-Type", "application/json");

var response = await client.ExecuteAsync(request);

Console.WriteLine($"Status Code: {response.StatusCode}");
Console.WriteLine($"Response: {response.Content}");

if (!response.IsSuccessful)
{
    Console.WriteLine($"Error: {response.ErrorMessage}");
}
