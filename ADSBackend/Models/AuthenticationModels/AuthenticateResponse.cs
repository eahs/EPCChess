using ADSBackend.Models;

namespace ADSBackend.Models.AuthenticationModels
{
    public class AuthenticateResponse
    {
        public int MemberId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(Member member, string token)
        {
            MemberId = member.MemberId;
            Email = member.Email;
            Token = token;
        }
    }
}
