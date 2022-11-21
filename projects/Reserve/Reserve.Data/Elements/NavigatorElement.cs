using System.Collections.Generic;
using JetBrains.Annotations;
using Reserve.Data.Types;

namespace Reserve.Data.Elements
{
    [UsedImplicitly]
    public class NavigatorElement : Element
    {
        public int Order { get; set; }
        public string Title { get; set; }
        public string MdiIcon { get; set; }
        public string RedirectTo { get; set; }
        public bool MobileView { get; set; }
        public bool GuestOnly { get; set; }
        public bool AuthenticatedOnly { get; set; }
        public RoleType[] PermittedRoles { get; set; }
        public NavigatorElement[] Children { get; set; }
        public Dictionary<string, dynamic> Properties { get; set; }
    }
}