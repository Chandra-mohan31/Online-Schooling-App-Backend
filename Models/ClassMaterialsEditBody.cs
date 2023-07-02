namespace ONLINE_SCHOOL_BACKEND.Models
{
    public class ClassMaterialsEditBody
    {
        public string Subject { get; set; }

        public string ForClass { get; set; }

        public string MaterialTitle { get; set; }

        public string Description { get; set; }

        public string MaterialContentUrl { get; set; }

        public DateTime PostedOn { get; set; }

        public string MaterialContentType { get; set; }
    }
}
