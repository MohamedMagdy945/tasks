using Architecture.Calender.Repository;

namespace Architecture.Calender.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        private List<Mission> missions;

        public RepositoryMission repositoryMission;

        public User() { 
            missions = new List<Mission>();
            repositoryMission = new RepositoryMission(missions , this);
        }
    }
}
