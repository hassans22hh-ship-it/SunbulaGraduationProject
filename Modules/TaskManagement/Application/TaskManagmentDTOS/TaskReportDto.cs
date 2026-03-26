namespace Application.TaskManagmentDTOS
{
    public class TaskReportDto
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int ActiveTasks { get; set; }
        public int ArchivedTasks { get; set; }
        
        public Dictionary<string, int> TasksByBehavior { get; set; } = new();
        public Dictionary<string, int> TasksByCategory { get; set; } = new();
        
        public int TotalTimeSpentMinutes { get; set; }
        public decimal TotalCoinsEarned { get; set; }
        
        public List<DailyReportDetailDto> Last7DaysProgress { get; set; } = new();
    }

    public class DailyReportDetailDto
    {
        public DateOnly Date { get; set; }
        public int TasksCompleted { get; set; }
        public int MinutesSpent { get; set; }
        public decimal CoinsEarned { get; set; }
    }
}
