using Microsoft.AspNetCore.SignalR;

namespace BusReservation.API.Hubs
{
    public class SeatsHub : Hub
    {
        // Client calls this to receive updates for a given schedule in a group
        public Task JoinScheduleGroup(string scheduleId)
            => Groups.AddToGroupAsync(Context.ConnectionId, $"schedule-{scheduleId}");

        public Task LeaveScheduleGroup(string scheduleId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"schedule-{scheduleId}");
    }
}
