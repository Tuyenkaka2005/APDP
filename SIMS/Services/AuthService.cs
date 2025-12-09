using SIMS.Data;
using SIMS.Helpers;
using Microsoft.EntityFrameworkCore;

namespace SIMS.Services
{
    public class AuthService
    {
        private readonly SIMSContext _context;

        public AuthService(SIMSContext context)
        {
            _context = context;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return null;

            if (PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }
    }
}
