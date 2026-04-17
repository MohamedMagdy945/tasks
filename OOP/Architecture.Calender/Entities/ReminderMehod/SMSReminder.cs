
using System.Reflection;

namespace Architecture.Calender.Entities.ReminderMehod
{
    public class SMSReminder : Reminder
    {
        public SMSReminder(Mission mission) : base(mission)
        {
        }

        public override void Notify()
        {
            Console.WriteLine($"[SMS] Reminder {mission.Owner.Name} for mission '{mission.Title}' status is {mission.Status} at {Time}");
        }
    }
}
