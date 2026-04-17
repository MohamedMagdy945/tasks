namespace Architecture.Calender.Entities
{
    public abstract class Reminder
    {
        protected Mission mission;
        public Reminder(Mission mission)
        {
            this.mission = mission ;
        }
        public DateTime Time { get; set; }
        public abstract void Notify();
    }
}
