using Xiyu.ArtificialIntelligence;

namespace Xiyu.AI.Client
{
    public class RequestOptions
    {
        public Multimap<string, string> HeaderParameters { get; set; } = new();
        public Multimap<string,string> QueryParameters { get;  set; }= new();

        
    }
}