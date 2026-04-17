using Architecture.Calender.Entities;
using Architecture.Calender.status;
using Architecture.status.Type;

namespace Architecture.Calender.Factory
{
    public class MissionFactory
    {
        private static int currentId = 1;

        public static Mission CreateMission(string title, string description)
        {
            var mission = new Mission
            {
                Id = currentId++,
                Description = description,
                Status = MissionStatus.Pending,
                Type = MissionType.Low

            };
            return mission;
        }
    }
}
