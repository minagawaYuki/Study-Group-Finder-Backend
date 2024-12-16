namespace StudyGroupFinder.Models
{
    public class PendingRequest
    {
        public int Id { get; set; }
        public int StudyGroupId { get; set; }
        public string UserId { get; set; }

        public virtual StudyGroup StudyGroup { get; set; }

    }

}
