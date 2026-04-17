
using Architecture.Calender.status;
using Architecture.status.Type;
using System.Net.NetworkInformation;

namespace Architecture.Calender.Entities
{
    public class Mission
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public MissionType Type { get; set; }
        public MissionStatus Status { get; set; }
        public User Owner { get; set; }



        private List<Reminder> reminders = new List<Reminder>();

        public void AttachReminder(Reminder reminder)
        {
            reminders.Add(reminder);
        }

        public void ChangeStatus(MissionStatus newStatus)
        {
            Status = newStatus;
            NotifyReminders();
        }
        private void NotifyReminders()
        {
            foreach (var reminder in reminders)
            {
                reminder.Notify();
            }
        }


        public override bool Equals(object obj)
        {
            if (obj is Mission other)
                return this.Id == other.Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
