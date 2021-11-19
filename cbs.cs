using System;
using System.Collections.Generic;
using System.Text;

namespace _3cc
{
    //these classes are from https://json2csharp.com/
    //with data from cbs endpoint
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class LatestBase
    {
        public int id { get; set; }
        public int time { get; set; }
        public DateTime date { get; set; }
        public string price { get; set; }
        public string lowestPrice { get; set; }
        public string bounce { get; set; }
        public string currentDrop { get; set; }
        public DateTime crackedAt { get; set; }
        public object respectedAt { get; set; }
        public bool isLowest { get; set; }
    }

    public class MarketStat
    {
        public string algorithm { get; set; }
        public string ratio { get; set; }
        public string medianDrop { get; set; }
        public string medianBounce { get; set; }
        public int? hoursToRespected { get; set; }
        public int? crackedCount { get; set; }
        public int? respectedCount { get; set; }
    }

    public class Basis
    {
        public int id { get; set; }
        public string baseCurrency { get; set; }
        public string quoteCurrency { get; set; }
        public string exchangeName { get; set; }
        public string exchangeCode { get; set; }
        public string longName { get; set; }
        public string marketName { get; set; }
        public string symbol { get; set; }
        public string volume { get; set; }
        public string quoteVolume { get; set; }
        public string btcVolume { get; set; }
        public string usdVolume { get; set; }
        public double currentPrice { get; set; }
        public LatestBase latestBase { get; set; }
        public List<MarketStat> marketStats { get; set; }
    }

    public class CBSRoot
    {
        public List<Basis> bases { get; set; }
    }
}
