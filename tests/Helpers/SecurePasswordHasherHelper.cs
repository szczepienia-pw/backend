using backend.Helpers;
using Microsoft.Extensions.Options;

namespace backend_tests.Helpers
{
    public static class SecurePasswordHasherHelper
    {
        public static SecurePasswordHasher Hasher { get; set; } = new SecurePasswordHasher(
            Options.Create(new HasherSettings() {SaltSize = 16, HashSize = 20})
        );
    }
}
