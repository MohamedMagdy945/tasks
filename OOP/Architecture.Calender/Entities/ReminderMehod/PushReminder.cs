namespace Architecture.Calender.Entities.ReminderMehod
{
    public class PushReminder : Reminder
    {
        public PushReminder(Mission mission) : base(mission)
        {
        }

        public override void Notify()
        {
            Console.WriteLine($"[PushReminder] Reminder {mission.Owner.Name} for mission '{mission.Title}' status is {mission.Status} at {Time}");
        }
    }
}
