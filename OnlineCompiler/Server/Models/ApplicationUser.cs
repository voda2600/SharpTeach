using Microsoft.AspNetCore.Identity;

namespace OnlineCompiler.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<StructureInfo> StructureInfos { get; set; }
    }
}
