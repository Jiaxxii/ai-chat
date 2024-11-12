namespace Xiyu.VirtualLiveRoom.Component.DanmuItem.Data
{
    
    [System.Serializable]
    public readonly struct DanmuData
    {
        public DanmuData(Danmu danmu, DanmuContent danmuContent, DanmuHead danmuHead, DanmuUserName danmuUserName)
        {
            Danmu = danmu;
            DanmuContent = danmuContent;
            DanmuHead = danmuHead;
            DanmuUserName = danmuUserName;
        }

        public Danmu Danmu { get; }
        public DanmuContent DanmuContent { get; }
        public DanmuHead DanmuHead { get; }
        public DanmuUserName DanmuUserName { get; }
    }
}