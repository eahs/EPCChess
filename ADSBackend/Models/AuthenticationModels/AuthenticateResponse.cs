
using ADSBackend.Models;

namespace ADSBackend.Models.AuthenticationModels
{
    /// <summary>
    /// Represents an authentication response.
    /// </summary>
    public class AuthenticateResponse
    {
        /// <summary>
        /// Gets or sets the member ID.
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the authentication token.
        /// </summary>
        public string Token { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateResponse"/> class.
        /// </summary>
        /// <param name="member">The member being authenticated.</param>
        /// <param name="token">The authentication token.</param>
        public AuthenticateResponse(Member member, string token)
        {
            MemberId = member.MemberId;
            Email = member.Email;
            Token = token;
        }
    }
}