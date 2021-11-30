using System;
using XCommas.Net;
using System.Linq;
using XCommas.Net.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Skender.Stock.Indicators;
using System.Collections.Concurrent;

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

            bool experimenting = true;
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
                //List<Quote> quotes = lib.GetCandles("SHIBUSDT", "1m", 250, "binance").ToList();
                //List<Quote> quotes2 = null;
                //string currTime;
                //while (true)
                //{
                //    //here I am updating existing quote record instead of getting all new records each time from binance api
                //    //I am sure that maybe the same could be done for results call to reduce computation perhaps?
                //    List<RsiResult> results = quotes.GetRsi(14).ToList();
                //    int last = results.Count - 1;
                //    currTime = results[last].Date.ToShortTimeString();
                //    Console.WriteLine($"SHIBUSDT 1min RSI @ {currTime} UTC is {Decimal.Round((decimal)results[last].Rsi,8)}");
                //    lib.Delay(10);
                //    quotes2 = lib.GetCandles("SHIBUSDT", "1m", 1, "binance").ToList();
                //    if (quotes2[0].Date.ToShortTimeString() == currTime)
                //        quotes[quotes.Count - 1] = quotes2[0]; //replace the last quote with the updated quote
                //    else
                //    {
                //        quotes.Add(quotes2[0]); //otherwise assume it's a new quote;
                //        quotes.RemoveAt(0); //remove first record so the number of quotes stays constant
                //    }
                //}

                //lib.bin_pairs are the pairs available in paper account
                string[] pairs = lib.bin_pairs.Split(",");
                Dictionary<string, decimal> top3 = new Dictionary<string, decimal>(), topX = new Dictionary<string, decimal>(), top3prev = null, topXprev = null;
                Dictionary<string, decimal> top3new = null;
                int idx = 0;
                while (true)
                {
                    ConcurrentDictionary<string, decimal> diff = new ConcurrentDictionary<string, decimal>();
                    Parallel.ForEach(pairs, pair_ =>
                    {
                        string[] qb = pair_.Split("_");
                        string pair = qb[1].Trim() + qb[0].Trim(); //get to binance candle format
                                                                   //get 4 candles (current candle Date, Open,High,Low,Close,Volume and 3 candles back e.g. 11:58:00, 11:59:00, 12:00:00, 12:00:37)
                        List<Quote> quotes = lib.GetCandles(pair, "1m", 4, "binance").ToList();

                        //get close differences into a dictionary for 3 minutes and change
                        decimal c0 = quotes[0].Close;
                        decimal c = quotes[quotes.Count - 2].Close;
                        diff.TryAdd(pair, 100 * (c - c0) / c);
                        //Task.Delay(50).GetAwaiter().GetResult(); //can't call api too much
                    });

                    if (top3prev != null) top3prev.Clear();
                    if (topXprev != null) topXprev.Clear();
                    top3prev = new Dictionary<string, decimal>(top3);
                    topXprev = new Dictionary<string, decimal>(topX);
                    top3.Clear();
                    topX.Clear();

                    int topNum = diff.Count / 10; //lets say they must remain in top 10% (this would be 27 rather than top 10--less churn)

                    //int cnt = 1;
                    foreach (KeyValuePair<string, decimal> pair in diff.OrderByDescending(key => key.Value)) //Descending
                    {
                        //Console.WriteLine("Key: {0}, Value: {1}", author.Key, author.Value);
                        if (top3.Count < 3) top3.Add(pair.Key, pair.Value);
                        if (topX.Count < topNum) topX.Add(pair.Key, pair.Value);
                        //cnt++;
                        if (topX.Count >= topNum) break;
                    }

                    //the top 3 was in the top 10 previously but is it still?
                    if (top3new != null) top3new.Clear();
                    else top3new = new Dictionary<string, decimal>();

                    //do
                    //{
                        foreach (KeyValuePair<string, decimal> pair in top3prev)
                        {
                            if (!topX.ContainsKey(pair.Key)) //is the prev top3 pair not in the current topX?
                            {
                                foreach (KeyValuePair<string, decimal> newPair in top3) //find a new top3 replacement
                                {
                                    if (!top3new.ContainsKey(newPair.Key))
                                    {
                                        top3new.Add(newPair.Key, newPair.Value);
                                        break;
                                    }
                                }
                            }
                            else //otherwise keep the previous top3 pair
                            {
                                if (!top3new.ContainsKey(pair.Key)) top3new.Add(pair.Key, topX[pair.Key]); //add the same pair with it's new value
                            }
                        }
                    //} while (top3new.Count < 3); //why the hell is it not 3?

                    

                    if (idx > 0)
                    {
                        foreach (KeyValuePair<string, decimal> pair in top3new)
                        {
                            Console.WriteLine($" pair: {pair.Key}, %: {Decimal.Round(pair.Value, 8)} iteration: {idx + 1}, time: {DateTime.Now.ToShortTimeString()}");
                        }
                        foreach (KeyValuePair<string, decimal> pair in top3prev)
                            if (!top3new.ContainsKey(pair.Key))
                            {
                                Console.WriteLine($"  *Remove {pair.Key}");
                                lib.SellTVCustom(123456, "email token", pair.Key);
                            }
                        foreach (KeyValuePair<string, decimal> pair in top3new)
                            if (!top3prev.ContainsKey(pair.Key))
                            {
                                Console.WriteLine($"  *Add {pair.Key}");
                                lib.StartTVCustom(123456, "email token", pair.Key);
                            }

                        //update top3 with top3new
                        top3.Clear();
                        foreach (KeyValuePair<string, decimal> pair in top3new) top3.Add(pair.Key, pair.Value);
                    }
                    else
                    {
                        foreach (KeyValuePair<string, decimal> pair in top3)
                        {
                            Console.WriteLine($" pair: {pair.Key} %: {Decimal.Round(pair.Value,8)} (first iteration), time: {DateTime.Now.ToShortTimeString()}");
                            lib.StartTVCustom(123456, "email token", pair.Key);
                        }
                    }
                    Console.WriteLine("");
                    idx++;
                    lib.Delay(60); //wait a minute and check again
                }


            }




        } 
    } 
}

