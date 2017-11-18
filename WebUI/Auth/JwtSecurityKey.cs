using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MassOrderApp.WebUI.Auth
{
    public static class JwtSecurityKey
    {
        public static SymmetricSecurityKey Create(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }

        public const string Issuer = "CorpoExt";
        public const string Audience = "CorpoExt";
    }
}