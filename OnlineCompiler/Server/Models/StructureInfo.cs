using static OnlineCompiler.Server.Controllers.ExecutionController;

namespace OnlineCompiler.Server.Models
{
    public class StructureInfo
    {
        public int Id { get; set; }
        public string UserLogin { get; set; }
        public StructureType StructureType { get; set; }
        public DateTime LastSaved { get; set; }
        public string Code { get; set; }
    }
}
