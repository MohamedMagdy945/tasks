
using Architecture.Calender.Entities;
using System.Reflection;

namespace Architecture.Calender.Repository
{
    public class RepositoryMission : IRepositoryMission
    {
        private List<Mission> missions;
        private User  owner;

        public RepositoryMission(List<Mission> missions , User owner)
        {
            this.missions = missions ;
            this.owner = owner ;
        }

        public int Add(Mission mission)
        {
            if (mission == null || missions.Any(m => m.Id == mission.Id))
                return 400;
            mission.Owner = owner;
            missions.Add(mission);
            return 200;
        }

        public int Delete(Mission mission)
        {
            if (mission == null || !missions.Contains(mission))
                return 400;

            missions.Remove(mission);
            return 200;
        }

        public Mission GetById(int id)
        {
            return missions.FirstOrDefault(m => m.Id == id);
        }

        public List<Mission> GetAll()
        {
            return missions.ToList(); // return copy
        }
    }
}
