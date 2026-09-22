namespace TodoAPI.Entities
{
    public enum TaskCategory
    {
        Work = 1,
        Personal = 2,
        HealthAndSport = 3,
        Chores = 4,
    }
    public enum TaskPriority
    {
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
