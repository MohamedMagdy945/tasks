namespace Architecture.Calender.Entities.ReminderMehod
{
    public class EmailReminder : Reminder
    {
        public EmailReminder(Mission mission) : base(mission)
        {
        }

        public override void Notify()
        {
            Console.WriteLine($"[Email] Reminder {mission.Owner.Name} for mission '{mission.Title}' status is {mission.Status} at {Time}");
        }
    }
}
