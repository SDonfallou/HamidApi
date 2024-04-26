using bookShareBEnd.Database;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace bookShareBEnd
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string receiverUserId, string message)
        {
            var senderUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Ensure both sender and receiver are authenticated
            if (string.IsNullOrEmpty(senderUserId) || string.IsNullOrEmpty(receiverUserId))
            {
                return;
            }

            var groupName = GetGroupName(senderUserId, receiverUserId);

            // Broadcast the message to all members of the group except the sender
            await Clients.Group(groupName).SendAsync("ReceiveMessage", senderUserId, message);
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Add the user to a group for one-to-one chat based on their user ID
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Remove the user from all groups
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private string GetGroupName(string userId1, string userId2)
        {
            // Sort user IDs to ensure consistent group name for the same pair of users
            var sortedUserIds = string.Compare(userId1, userId2) < 0 ? $"{userId1}_{userId2}" : $"{userId2}_{userId1}";
            return $"Chat_{sortedUserIds}";
        }
    }

}
