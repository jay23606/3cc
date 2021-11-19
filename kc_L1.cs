using System;
using System.Collections.Generic;
using System.Text;

namespace _3cc
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Data
    {
        public long time { get; set; }
        public string sequence { get; set; }
        public string price { get; set; }
        public string size { get; set; }
        public string bestBid { get; set; }
        public string bestBidSize { get; set; }
        public string bestAsk { get; set; }
        public string bestAskSize { get; set; }
    }

    public class KCL1Root
    {
        public string code { get; set; }
        public Data data { get; set; }
    }


}
