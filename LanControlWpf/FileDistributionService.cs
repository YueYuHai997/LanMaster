using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace LanControlWpf
{
    // File-distribution domain objects. The transfer engine is intentionally private-LAN only.
    [DataContract]
    public sealed class DistributionTask : INotifyPropertyChanged
    {
        [DataMember] public string Id { get; set; } = Guid.NewGuid().ToString("N");
        [DataMember] public string Name { get; set; }
        [DataMember] public string SourcePath { get; set; }
        [DataMember] public string TorrentPath { get; set; }
        [DataMember] public string SourceIp { get; set; }
        [DataMember] public string TorrentUrl { get; set; }
        [DataMember] public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DataMember] public List<string> TargetDeviceIds { get; set; } = new List<string>();
        [DataMember] public string Status { get; set; }
        public string PeerSummary { get; set; }
        public bool IsReceiving { get; set; }
        bool hasTransferManager;
        bool isPaused;
        bool isTransferComplete;
        public bool HasTransferManager { get { return hasTransferManager; } set { hasTransferManager = value; Raise("HasTransferManager"); Raise("ShowTransferControl"); } }
        public bool IsPaused { get { return isPaused; } set { isPaused = value; Raise("IsPaused"); Raise("TransferActionText"); } }
        public bool IsTransferComplete { get { return isTransferComplete; } set { isTransferComplete = value; Raise("IsTransferComplete"); Raise("ShowTransferControl"); } }
        public bool ShowTransferControl { get { return HasTransferManager && !IsTransferComplete; } }
        public string TransferActionText { get { return IsPaused ? (IsReceiving ? "继续下载" : "继续发送") : (IsReceiving ? "停止下载" : "停止发送"); } }
        public string DisplayName { get { return Name + " · " + Status + " · " + TargetDeviceIds.Count + " 台目标设备"; } }
        public event PropertyChangedEventHandler PropertyChanged;
        public void Raise(string name) { if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name)); }
    }

    public sealed class DistributionPeerStatus
    {
        public string DeviceId { get; set; }
        public string Username { get; set; }
        public double Progress { get; set; }
        public string State { get; set; }
        public long DownloadRate { get; set; }
        public long UploadRate { get; set; }
    }
}
