namespace TodoAPI.Entities
{
    public enum TaskCategory
    {
        None = 0,
        Work = 1,
        Personal = 2,
        HealthAndSport = 3,
        Chores = 4,
    }
    public enum TaskPriority
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4,
    }

    public enum TaskExparation
    {
        NotExpired = 0,
        Expired = 1
    }
}
