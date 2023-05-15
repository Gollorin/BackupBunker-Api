namespace backup_bunker_api.Models
{
    public class InputSchedule
    {
        public string Id { get; set; }
        public List<string> Backups { get; set; }
    }
}
