using Architecture.Calender.Entities;
using Architecture.Calender.Entities.ReminderMehod;
using Architecture.Calender.status;

namespace Architecture.Calender
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var user = new User { Id = 1, Name = "Mohamed Mahmoud" };

            var mission = new Mission { Id = 1, Title = "Finish OOP Task" };
            user.repositoryMission.Add(mission);

            var emailReminder = new EmailReminder(mission) { Time = DateTime.Now };
            var smsReminder = new SMSReminder(mission) { Time = DateTime.Now.AddMinutes(30) };

            mission.AttachReminder(emailReminder);
            mission.AttachReminder(smsReminder);

            mission.ChangeStatus(MissionStatus.InProgress);
        }
    }
}
