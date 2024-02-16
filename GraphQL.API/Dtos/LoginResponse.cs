using GraphQL.API.Models;

namespace GraphQL.API.Dtos;

public record LoginResponse(string Token, string TokenExpire, User User);
