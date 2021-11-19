using System;
using XCommas.Net;
using System.Linq;
using XCommas.Net.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Skender.Stock.Indicators;

namespace _3cc
{
    public static class Program
    {
        public static void Main()
        {
            var lines = "secrets.txt".GetLines(@"3cc expects a file in the current directory named secrets.txt with the following:
API key (first line)
API secret (second line)
0 _or_ 1 (0 for Paper account and 1 for Real account - third line)
Exchange market code (e.g. paper_trading, binance_us, ftx_us, gdx, kucoin - fourth line)
email_token (retrieve from your 3c account in custom TV start condition JSON - fifth line)
");
            lib.api = new XCommasApi(lines[0], lines[1], default, (UserMode)Convert.ToInt32(lines[2]));
            if (lines.Length > 3) lib.email_token = lines[4];
            //extract account ID needed for bot creation from 4th line
            var accts = lib.api.GetAccountsAsync().GetAwaiter().GetResult().Data;
            foreach (var acct in accts) if (acct.MarketCode == lines[3]) lib.accountId = acct.Id;

            bool experimenting = false;
            if (!experimenting)
            {
                var opts = lib.GetOptions();

                //the MaxPrice version that creates multiple bots
                if (opts.ContainsKey("cbs2"))
                {
                    //Method that creates a new deal for each CBS scanner usdt match
                    var hasRun = new Dictionary<string, bool>();
                    int maxBots = 10;
                    while (true)
                    {
                        dynamic data = null;
                        try
                        {
                            //data = lib.GetJSON<CBSRoot>(lib.cbs);
                            data = lib.GetJSON<CBSRoot>(lib.cbs2 + lib.odcp[lib.odcp_index]);
                        }
                        catch { }
                        lib.odcp_index--;
                        if (lib.odcp_index < 1) lib.odcp_index = 3;

                        int cnt = 0;
                        foreach (var pair in data.bases)
                        {
                            if (pair.quoteCurrency == "usdt")
                            {
                                string longName = pair.longName.ToString().Replace("-", "_");
                                if (hasRun.ContainsKey(longName)) continue;
                                hasRun.Add(longName, true);
                                var latestBase = pair.latestBase;
                                var stats = pair.marketStats[lib.odcp_index];
                                decimal latestBasePrice = Convert.ToDecimal(latestBase.price);
                                decimal latestBaseCurrentDrop = -Convert.ToDecimal(latestBase.currentDrop);
                                decimal latestBaseBounce = Convert.ToDecimal(latestBase.bounce) * 0.8m;
                                decimal latestBaseLessHalfMedian = Decimal.Round(latestBasePrice * 0.01m * (100m + lib.D(stats.medianDrop) * 0.5m), 8);

                                //var opts = lib.GetOptions($"-pairs {longName} -bo 20 -so 30 -mstc 2 -name CBS_{longName} -tp {latestBaseBounce} -mp {latestBasePrice} -sos {latestBaseCurrentDrop} -dadc 1");
                                var opts2 = lib.GetOptions($"-pairs {longName} -bo 20 -so 30 -mstc 2 -name CBS_{longName} -tp {latestBaseBounce} -mp {latestBaseLessHalfMedian} -sos {latestBaseCurrentDrop} -dadc 1");
                                if (opts2.CreateUpdateBot())
                                {
                                    maxBots--;

                                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(pair);
                                    System.IO.File.AppendAllText(System.IO.Directory.GetCurrentDirectory() + "/data.txt", json);
                                }
                                if (maxBots == 0) Environment.Exit(0);
                                cnt++;
                            }
                        }
                        lib.Print($"Found {cnt} usdt pairs from CBS scanner..");
                        lib.Delay(30);
                    }
                    Environment.Exit(0);
                }



                opts.CreateUpdateBot();

                //some special features
                if (opts.ContainsKey("id") && opts.ContainsKey("qfl") && opts["qfl"].Length > 1 && opts["qfl"][1] == "pcd")
                {
                    //testing -qfl 3 pcd -id 64861__
                    //round robin the mode to hopefully snatch more deals
                    while (true)
                    {
                        lib.Print($"Switching {lib.Bots[0].Name} to Position");
                        lib.Delay(100);
                        opts["qfl"][1] = "c"; //switch to conservative
                        lib.Print($"Switching {lib.Bots[0].Name} to Conservative");
                        opts.CreateUpdateBot();
                        lib.Delay(100);
                        opts["qfl"][1] = "d"; //switch to day trade
                        lib.Print($"Switching {lib.Bots[0].Name} to Day Trade");
                        opts.CreateUpdateBot();
                        lib.Delay(100);
                        opts["qfl"][1] = "p";
                        opts.CreateUpdateBot();
                    }
                }
                //the version that uses multipair bot and triggers using custom TV
                if (opts.ContainsKey("cbs"))
                {
                    lib.TriggerCBS();
                }


            }
            else
            {
                List<Quote> quotes = lib.GetCandles("SHIBUSDT", "1m", 250, "binance").ToList();
                List<Quote> quotes2 = null;
                string currTime;
                while (true)
                {
                    //here I am updating existing quote record instead of getting all new records each time from binance api
                    //I am sure that maybe the same could be done for results call to reduce computation perhaps?
                    List<RsiResult> results = quotes.GetRsi(14).ToList();
                    int last = results.Count - 1;
                    currTime = results[last].Date.ToShortTimeString();
                    Console.WriteLine($"SHIBUSDT 1min RSI @ {currTime} UTC is {Decimal.Round((decimal)results[last].Rsi,8)}");
                    lib.Delay(10);
                    quotes2 = lib.GetCandles("SHIBUSDT", "1m", 1, "binance").ToList();
                    if (quotes2[0].Date.ToShortTimeString() == currTime)
                        quotes[quotes.Count - 1] = quotes2[0]; //replace the last quote with the updated quote
                    else
                    {
                        quotes.Add(quotes2[0]); //otherwise assume it's a new quote;
                        quotes.RemoveAt(0); //remove first record so the number of quotes stays constant
                    }
                }
            }




        } 
    } 
}

