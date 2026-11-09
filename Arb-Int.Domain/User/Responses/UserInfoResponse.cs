

using System.Text.Json.Serialization;
using Arb_Int.Domain.User;

public class UserInfoResponse
{
    [JsonPropertyOrder(-1)]
    public Guid Id { get; set; }
    [JsonPropertyOrder(1)]
    public string? FirstName { get; set; }
    [JsonPropertyOrder(2)]
    public string? LastName { get; set; }
    [JsonPropertyOrder(3)]
    public required string UserName { get; set; }
    [JsonPropertyOrder(4)]
    public required string Email { get; set; }
}