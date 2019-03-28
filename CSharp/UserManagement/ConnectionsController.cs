using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public class ConnectionsController : ApiController
    {
        public ConnectionsController(
            IUserCache userCache,
            IUserRepository userRepository)
        {
            UserCache = userCache;
            UserRepository = userRepository;
        }

        public IUserCache UserCache { get; }

        public IUserRepository UserRepository { get; }

        public IHttpActionResult Post(string userId, string otherUserId)
        {

            var response = LookupUser(userId);

            switch (response.Kind)
            {
                case UserResponse.ResponseKind.NotFound:
                    return BadRequest("User not found.");
                case UserResponse.ResponseKind.Invalid:
                    return BadRequest("Invalid user ID.");
            }

            var user = response.User;

            var otherResponse = LookupUser(otherUserId);
            switch (otherResponse.Kind)
            {
                case UserResponse.ResponseKind.NotFound:
                    return BadRequest("Other user not found.");
                case UserResponse.ResponseKind.Invalid:
                    return BadRequest("Invalid ID for other user.");
            }

            var otherUser = otherResponse.User;

            user.Connect(otherUser);
            UserRepository.Update(user);

            return Ok(otherUser);
        }

        private class UserResponse
        {
            public User User { get; }
            public ResponseKind Kind { get; }

            public UserResponse(User user, ResponseKind kind)
            {
                User = user;
                Kind = kind;
            }

            public enum ResponseKind
            {
                Ok,
                NotFound,
                Invalid
            }
        }
       
        private UserResponse LookupUser(string userId )
        {
            var user = UserCache.Find(userId);

            if (user != null)
                return new UserResponse(user, UserResponse.ResponseKind.Ok);

            if (int.TryParse(userId, out var userInt))
            {
                user = UserRepository.ReadUser(userInt);
                return user != null
                    ? new UserResponse(user, UserResponse.ResponseKind.Ok)
                    : new UserResponse(null, UserResponse.ResponseKind.NotFound);
            }
            else
            {
                return new UserResponse(null, UserResponse.ResponseKind.Invalid);
            }
        }
    }
}
