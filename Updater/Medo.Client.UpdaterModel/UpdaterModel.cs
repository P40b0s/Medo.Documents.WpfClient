using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Medo.Client.Updater.Model
{
    [Serializable]
    public class UpdaterModel
    {
        [DataMember]
        public string UpdatePath = "\\\\182.5.202.220\\Софт для работы\\МЭДО 2.0\\";
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public DateTime UpdateTime { get; set; }
        [DataMember]
        public List<ModuleVersion> Modules { get; set; }
        [DataMember]
        public string AboutChanges { get; set; }
        [DataMember]
        public bool IsCriticalUpdate { get; set; }
        [DataMember]
        public string VersionHistory { get; set; }
    }
    [Serializable]
    public class ModuleVersion
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string Subdir { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime EditTime { get; set; }      
    }
}
