


using Architecture.Calender.Entities;

namespace Architecture.Calender.Repository
{
    public interface IRepositoryMission
    {
        public int Add(Mission mission);
        public int Delete(Mission mission);
        List<Mission> GetAll();

    }
}
