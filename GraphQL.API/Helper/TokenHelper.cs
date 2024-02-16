using GraphQL.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GraphQL.API.Helper;

public class TokenHelper
{
    public TokenResponse GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("c082a53a-938b-4504-8cff-def4667e8854"));

        var signingCredential= new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
              new Claim(ClaimTypes.Name,user.Username),
              new Claim(ClaimTypes.Email,user.Email)
        };

        if(user.Role is not null)
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
        }

        JwtSecurityToken securityToken = new JwtSecurityToken(
            issuer: "http://localhost:5054",
            audience: "http://localhost:5054",
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: signingCredential);


        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return new TokenResponse(token,DateTime.Now.AddMinutes(5).ToLongDateString());
    }
}
