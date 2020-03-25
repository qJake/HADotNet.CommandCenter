using System;
using System.IdentityModel.Tokens.Jwt;

namespace HADotNet.CommandCenter.Utils
{
    public static class JwtHelper
    {
        public static bool IsTokenValid(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                {
                    return false;
                }

                var jwt = handler.ReadJwtToken(token);

                return jwt.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }
    }
}
