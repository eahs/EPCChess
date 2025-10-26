
using Microsoft.AspNetCore.Identity;

namespace ADSBackend.Models.Identity
{
    /// <summary>
    /// Represents a role in the application, extending the base IdentityRole with an integer key.
    /// </summary>
    public class ApplicationRole : IdentityRole<int>
    {

    }
}