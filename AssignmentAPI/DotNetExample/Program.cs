using DotNetExample;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddHttpClient<IAssignmentApiService, AssignmentApiService>();

var serviceProvider = services.BuildServiceProvider();
var assignmentApiClient = serviceProvider.GetRequiredService<IAssignmentApiService>();

var response = await assignmentApiClient.SendAssignment("testAssignmentId");

Console.WriteLine("Successfully sent assignment! Response:");
Console.WriteLine(await response.Content.ReadAsStringAsync());