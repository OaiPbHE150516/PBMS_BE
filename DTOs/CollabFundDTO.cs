namespace pbms_be.DTOs
{
    public class CollabFundDTO
    {
    }

    // CreateCollabFundDTO
    public class CreateCollabFundDTO
    {
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
        public long TotalAmount { get; set; }
    }

    // UpdateCollabFundDTO
    public class UpdateCollabFundDTO
    {
        public int CollabFundID { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
        public long TotalAmount { get; set; }
        public int ActiveStateID { get; set; }

    }

    // ChangeActiveStateDTO
    public class ChangeCollabFundActiveStateDTO
    {
        public int CollabFundID { get; set; }
        public int ActiveStateID { get; set; }
    }
}
