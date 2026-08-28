using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace LanControlWpf
{
    [DataContract] public class Settings { [DataMember] public string DeviceId { get; set; } [DataMember] public string Username { get; set; } [DataMember] public string Password { get; set; } [DataMember] public string AppPath { get; set; } [DataMember] public string BatPath { get; set; } [DataMember] public string LogFolder { get; set; } [DataMember] public string DistributionFolder { get; set; } [DataMember] public bool AllowRemote { get; set; } [DataMember] public bool AutoHide { get; set; } [DataMember] public List<SavedDevice> Devices { get; set; } }
    [DataContract] public class SavedDevice { [DataMember] public string DeviceId { get; set; } [DataMember] public string Username { get; set; } [DataMember] public string IP { get; set; } [DataMember] public string Mac { get; set; } [DataMember] public bool AllowRemote { get; set; } [DataMember] public bool HasApp { get; set; } [DataMember] public bool HasBat { get; set; } [DataMember] public DateTime LastSeen { get; set; } }
    public class DeviceInfo : INotifyPropertyChanged
    {
        public string DeviceId { get; set; } public string Username { get; set; } public string IP { get; set; } public string Mac { get; set; } public bool AllowRemote { get; set; } public bool HasApp { get; set; } public bool HasBat { get; set; } public DateTime LastSeen { get; set; } public bool IsOnline { get; set; }
        bool selected; public bool IsSelected { get { return selected; } set { selected = value; Raise("IsSelected"); } }
        public string RemoteStatus { get { return AllowRemote ? "已开启控制" : "未开启控制"; } } public string AppStatus { get { return HasApp ? "已配置应用" : "未配置应用"; } } public string SeenText { get { return LastSeen == default(DateTime) ? "--" : LastSeen.ToString("yyyy-MM-dd HH:mm"); } } public string OnlineStatus { get { return IsOnline ? "在线" : "离线"; } }
        public event PropertyChangedEventHandler PropertyChanged; void Raise(string n) { if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(n)); }
    }
}
