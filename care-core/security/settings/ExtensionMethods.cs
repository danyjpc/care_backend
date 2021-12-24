using System.Collections.Generic;
using System.Linq;
using care_core.dto.auth;

namespace care_core.security.settings
{
    public static class ExtensionMethods
    {
        public static IEnumerable<TokenDto> WithoutPasswords(this IEnumerable<TokenDto> token)
        {
            return token.Select(x => x.WithoutPassword());
        }

        public static TokenDto WithoutPassword(this TokenDto token)
        {
            return token;
        }
    }
}