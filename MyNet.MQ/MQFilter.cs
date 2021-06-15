
namespace MyNet.MQ
{
    public class MQFilter
    {
        public MQFilter(string filter,byte level)
        {
            TopicFilter = filter;
            QosLevel = level;
        }
        public string TopicFilter { get; set; }
        public byte QosLevel { get; set; }
    }
}
