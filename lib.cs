using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;
using XCommas.Net;
using XCommas.Net.Objects;
using Skender.Stock.Indicators;

namespace _3cc
{
    public static class lib
    {
        public static XCommasApi api;
        public static string cbs = "https://api.cryptobasescanner.com/v1/bases?api_key=xxx&algorithm=day_trade"; //original
        public static string cbs2 = "https://api.cryptobasescanner.com/v1/bases?api_key=xxx&algorithm=";
        public static string[] odcp = { "original", "day_trade", "conservative", "position" };
        public static int odcp_index = 3;
        public static int accountId = 0;
        public static string email_token = "";
        public static List<Bot> Bots = new List<Bot>();
        //public static Bot Bot = null;
        public static string[] usdt_all = { "USDT_1INCH", "USDT_2CRZ", "USDT_AAVE", "USDT_ABBC", "USDT_ACE", "USDT_ACOIN", "USDT_ADA", "USDT_AERGO", "USDT_AGIX", "USDT_AGLD", "USDT_AI", "USDT_AIOZ", "USDT_AKRO", "USDT_ALBT", "USDT_ALEPH", "USDT_ALGO", "USDT_ALICE", "USDT_ALPACA", "USDT_ALPHA", "USDT_AMPL", "USDT_ANC", "USDT_ANKR", "USDT_AOA", "USDT_API3", "USDT_APL", "USDT_AR", "USDT_ARPA", "USDT_ARRR", "USDT_ARX", "USDT_ASD", "USDT_ATOM", "USDT_AURY", "USDT_AVA", "USDT_AVAX", "USDT_AXC", "USDT_AXS", "USDT_BADGER", "USDT_BAL", "USDT_BAND", "USDT_BASIC", "USDT_BAT", "USDT_BAX", "USDT_BCH", "USDT_BCHA", "USDT_BCHSV", "USDT_BEPRO", "USDT_BLOK", "USDT_BMON", "USDT_BNB", "USDT_BNS", "USDT_BNT", "USDT_BOA", "USDT_BOLT", "USDT_BOND", "USDT_BOSON", "USDT_BTC", "USDT_BTT", "USDT_BURP", "USDT_BUX", "USDT_BUY", "USDT_BZRX", "USDT_C98", "USDT_CAKE", "USDT_CARD", "USDT_CARR", "USDT_CAS", "USDT_CBC", "USDT_CELO", "USDT_CEUR", "USDT_CFG", "USDT_CGG", "USDT_CHR", "USDT_CHZ", "USDT_CIRUS", "USDT_CIX100", "USDT_CKB", "USDT_CLV", "USDT_COMB", "USDT_COMP", "USDT_COTI", "USDT_CPOOL", "USDT_CQT", "USDT_CRO", "USDT_CRPT", "USDT_CRV", "USDT_CTI", "USDT_CTSI", "USDT_CUDOS", "USDT_CUSD", "USDT_CWS", "USDT_DAG", "USDT_DAO", "USDT_DAPPT", "USDT_DAPPX", "USDT_DASH", "USDT_DEGO", "USDT_DERO", "USDT_DEXE", "USDT_DFI", "USDT_DFYN", "USDT_DGB", "USDT_DIA", "USDT_DINO", "USDT_DIVI", "USDT_DMG", "USDT_DMTR", "USDT_DODO", "USDT_DOGE", "USDT_DORA", "USDT_DOT", "USDT_DPET", "USDT_DPI", "USDT_DSLA", "USDT_DVPN", "USDT_DYDX", "USDT_DYP", "USDT_EDG", "USDT_EFX", "USDT_EGLD", "USDT_ELA", "USDT_ELON", "USDT_ENJ", "USDT_ENQ", "USDT_EOS", "USDT_EOSC", "USDT_EQX", "USDT_EQZ", "USDT_ERG", "USDT_ERSDL", "USDT_ETC", "USDT_ETH", "USDT_ETHO", "USDT_ETN", "USDT_EWT", "USDT_EXRD", "USDT_FCL", "USDT_FEAR", "USDT_FIL", "USDT_FKX", "USDT_FLAME", "USDT_FLOW", "USDT_FLUX", "USDT_FLY", "USDT_FORESTPLUS", "USDT_FORM", "USDT_FORTH", "USDT_FRM", "USDT_FRONT", "USDT_FTM", "USDT_FTT", "USDT_GALAX", "USDT_GAS", "USDT_GENS", "USDT_GHST", "USDT_GHX", "USDT_GLCH", "USDT_GLQ", "USDT_GMB", "USDT_GMEE", "USDT_GO", "USDT_GOM2", "USDT_GOVI", "USDT_GRIN", "USDT_GRT", "USDT_GSPI", "USDT_GTC", "USDT_HAI", "USDT_HAKA", "USDT_HAPI", "USDT_HBAR", "USDT_HERO", "USDT_HORD", "USDT_HOTCROSS", "USDT_HT", "USDT_HTR", "USDT_HYDRA", "USDT_HYVE", "USDT_ICP", "USDT_IDEA", "USDT_ILV", "USDT_INJ", "USDT_IOI", "USDT_IOST", "USDT_IOTX", "USDT_IXS", "USDT_JAR", "USDT_JASMY", "USDT_JST", "USDT_JUP", "USDT_KAI", "USDT_KAR", "USDT_KAT", "USDT_KCS", "USDT_KDA", "USDT_KEEP", "USDT_KLV", "USDT_KMD", "USDT_KOK", "USDT_KONO", "USDT_KRL", "USDT_KSM", "USDT_LABS", "USDT_LAYER", "USDT_LINK", "USDT_LITH", "USDT_LNCHX", "USDT_LOC", "USDT_LOCG", "USDT_LON", "USDT_LPOOL", "USDT_LPT", "USDT_LRC", "USDT_LSS", "USDT_LTC", "USDT_LTO", "USDT_LTX", "USDT_LUNA", "USDT_LYM", "USDT_LYXE", "USDT_MAHA", "USDT_MAKI", "USDT_MAN", "USDT_MANA", "USDT_MAP", "USDT_MARSH", "USDT_MASK", "USDT_MATIC", "USDT_MATTER", "USDT_MEM", "USDT_MHC", "USDT_MIR", "USDT_MITX", "USDT_MKR", "USDT_MLK", "USDT_MLN", "USDT_MNST", "USDT_MODEFI", "USDT_MOVR", "USDT_MSWAP", "USDT_MTL", "USDT_MTV", "USDT_MXC", "USDT_MXW", "USDT_NAKA", "USDT_NANO", "USDT_NDAU", "USDT_NEAR", "USDT_NEO", "USDT_NFT", "USDT_NFTB", "USDT_NGM", "USDT_NIF", "USDT_NIM", "USDT_NKN", "USDT_NMR", "USDT_NOIA", "USDT_NORD", "USDT_NTVRK", "USDT_NWC", "USDT_ODDZ", "USDT_OGN", "USDT_OMG", "USDT_ONE", "USDT_ONT", "USDT_OOE", "USDT_OPCT", "USDT_OPUL", "USDT_ORAI", "USDT_ORBS", "USDT_ORN", "USDT_OUSD", "USDT_OXEN", "USDT_OXT", "USDT_PBX", "USDT_PCX", "USDT_PDEX", "USDT_PERP", "USDT_PHA", "USDT_PHNX", "USDT_PIVX", "USDT_PLU", "USDT_PMON", "USDT_PNT", "USDT_POL", "USDT_POLK", "USDT_POLS", "USDT_POLX", "USDT_PRE", "USDT_PROM", "USDT_PRQ", "USDT_PUNDIX", "USDT_PUSH", "USDT_PYR", "USDT_QI", "USDT_QNT", "USDT_QRDO", "USDT_REAP", "USDT_REEF", "USDT_REN", "USDT_REP", "USDT_REQ", "USDT_REV", "USDT_REVV", "USDT_RFOX", "USDT_RFUEL", "USDT_RLC", "USDT_RLY", "USDT_RMRK", "USDT_RNDR", "USDT_ROSE", "USDT_ROSN", "USDT_ROUTE", "USDT_RUNE", "USDT_SAND", "USDT_SCLP", "USDT_SDAO", "USDT_SDN", "USDT_SENSO", "USDT_SFUND", "USDT_SHA", "USDT_SHFT", "USDT_SHIB", "USDT_SHR", "USDT_SKEY", "USDT_SKL", "USDT_SKU", "USDT_SLIM", "USDT_SLP", "USDT_SNX", "USDT_SOL", "USDT_SOLR", "USDT_SOLVE", "USDT_SOUL", "USDT_SOV", "USDT_SPI", "USDT_SRK", "USDT_STC", "USDT_STMX", "USDT_STND", "USDT_STORJ", "USDT_STRONG", "USDT_STX", "USDT_SUKU", "USDT_SUN", "USDT_SUPER", "USDT_SUSD", "USDT_SUSHI", "USDT_SUTER", "USDT_SWASH", "USDT_SWINGBY", "USDT_SXP", "USDT_SYLO", "USDT_TARA", "USDT_TCP", "USDT_TEL", "USDT_THETA", "USDT_TIDAL", "USDT_TKY", "USDT_TLM", "USDT_TLOS", "USDT_TOKO", "USDT_TOMO", "USDT_TONE", "USDT_TOWER", "USDT_TRB", "USDT_TRIAS", "USDT_TRIBE", "USDT_TRX", "USDT_TVK", "USDT_TXA", "USDT_UBX", "USDT_UBXT", "USDT_UMA", "USDT_UMB", "USDT_UNFI", "USDT_UNI", "USDT_UNO", "USDT_UOS", "USDT_USDC", "USDT_USDJ", "USDT_USDN", "USDT_VAI", "USDT_VEED", "USDT_VEGA", "USDT_VELO", "USDT_VET", "USDT_VID", "USDT_VIDT", "USDT_VRA", "USDT_VSYS", "USDT_WAVES", "USDT_WAXP", "USDT_WEST", "USDT_WILD", "USDT_WIN", "USDT_WNCG", "USDT_WOM", "USDT_WOO", "USDT_WSIENNA", "USDT_WXT", "USDT_XAVA", "USDT_XCAD", "USDT_XCH", "USDT_XCUR", "USDT_XDB", "USDT_XDC", "USDT_XED", "USDT_XEM", "USDT_XHV", "USDT_XLM", "USDT_XMR", "USDT_XNL", "USDT_XPR", "USDT_XPRT", "USDT_XRP", "USDT_XSR", "USDT_XTZ", "USDT_XVS", "USDT_XYM", "USDT_XYO", "USDT_YFDAI", "USDT_YFI", "USDT_YGG", "USDT_YLD", "USDT_YOP", "USDT_ZCX", "USDT_ZEC", "USDT_ZEE", "USDT_ZEN", "USDT_ZIL", "USDT_ZKT", "USDT_ZORT" };
        public static string bin_pairs = "USDT_1INCH, USDT_AAVE, USDT_ACM, USDT_ADA, USDT_AGLD, USDT_AION, USDT_AKRO, USDT_ALGO, USDT_ALICE, USDT_ALPACA, USDT_ALPHA, USDT_ANKR, USDT_ANT, USDT_AR, USDT_ARDR, USDT_ARPA, USDT_ASR, USDT_ATA, USDT_ATM, USDT_ATOM, USDT_AUDIO, USDT_AUTO, USDT_AVA, USDT_AVAX, USDT_AXS, USDT_BADGER, USDT_BAKE, USDT_BAL, USDT_BAND, USDT_BAR, USDT_BAT, USDT_BCH, USDT_BEAM, USDT_BEL, USDT_BETA, USDT_BLZ, USDT_BNB, USDT_BNT, USDT_BOND, USDT_BTC, USDT_BTCST, USDT_BTG, USDT_BTS, USDT_BTT, USDT_BURGER, USDT_BZRX, USDT_C98, USDT_CAKE, USDT_CELO, USDT_CELR, USDT_CFX, USDT_CHR, USDT_CHZ, USDT_CKB, USDT_CLV, USDT_COCOS, USDT_COMP, USDT_COS, USDT_COTI, USDT_CRV, USDT_CTK, USDT_CTSI, USDT_CTXC, USDT_CVC, USDT_CVP, USDT_DASH, USDT_DATA, USDT_DCR, USDT_DEGO, USDT_DENT, USDT_DEXE, USDT_DF, USDT_DGB, USDT_DIA, USDT_DNT, USDT_DOCK, USDT_DODO, USDT_DOGE, USDT_DOT, USDT_DREP, USDT_DUSK, USDT_DYDX, USDT_EGLD, USDT_ELF, USDT_ENJ, USDT_EOS, USDT_EPS, USDT_ERN, USDT_ETC, USDT_ETH, USDT_FARM, USDT_FET, USDT_FIDA, USDT_FIL, USDT_FIO, USDT_FIRO, USDT_FIS, USDT_FLM, USDT_FLOW, USDT_FOR, USDT_FORTH, USDT_FRONT, USDT_FTM, USDT_FTT, USDT_FUN, USDT_GALA, USDT_GBP, USDT_GHST, USDT_GNO, USDT_GRT, USDT_GTC, USDT_GTO, USDT_GXS, USDT_HARD, USDT_HBAR, USDT_HIVE, USDT_HNT, USDT_HOT, USDT_ICP, USDT_ICX, USDT_IDEX, USDT_ILV, USDT_INJ, USDT_IOST, USDT_IOTA, USDT_IOTX, USDT_IRIS, USDT_JST, USDT_JUV, USDT_KAVA, USDT_KEEP, USDT_KEY, USDT_KLAY, USDT_KMD, USDT_KNC, USDT_KSM, USDT_LINA, USDT_LINK, USDT_LIT, USDT_LPT, USDT_LRC, USDT_LSK, USDT_LTC, USDT_LTO, USDT_LUNA, USDT_MANA, USDT_MASK, USDT_MATIC, USDT_MBL, USDT_MBOX, USDT_MDT, USDT_MDX, USDT_MFT, USDT_MINA, USDT_MIR, USDT_MITH, USDT_MKR, USDT_MLN, USDT_MTL, USDT_NANO, USDT_NBS, USDT_NEAR, USDT_NEO, USDT_NKN, USDT_NMR, USDT_NU, USDT_NULS, USDT_OCEAN, USDT_OG, USDT_OGN, USDT_OM, USDT_OMG, USDT_ONE, USDT_ONG, USDT_ONT, USDT_ORN, USDT_OXT, USDT_PAXG, USDT_PERL, USDT_PERP, USDT_PHA, USDT_PNT, USDT_POLS, USDT_POLY, USDT_POND, USDT_PSG, USDT_PUNDIX, USDT_QNT, USDT_QTUM, USDT_QUICK, USDT_RAD, USDT_RAMP, USDT_RARE, USDT_RAY, USDT_REEF, USDT_REN, USDT_REP, USDT_REQ, USDT_RIF, USDT_RLC, USDT_ROSE, USDT_RSR, USDT_RUNE, USDT_RVN, USDT_SAND, USDT_SC, USDT_SFP, USDT_SHIB, USDT_SKL, USDT_SLP, USDT_SNX, USDT_SOL, USDT_SRM, USDT_STMX, USDT_STORJ, USDT_STPT, USDT_STRAX, USDT_STX, USDT_SUN, USDT_SUPER, USDT_SUSD, USDT_SUSHI, USDT_SXP, USDT_SYS, USDT_TCT, USDT_TFUEL, USDT_THETA, USDT_TKO, USDT_TLM, USDT_TOMO, USDT_TORN, USDT_TRB, USDT_TRIBE, USDT_TROY, USDT_TRU, USDT_TRX, USDT_TVK, USDT_TWT, USDT_UMA, USDT_UNFI, USDT_UNI, USDT_USDC, USDT_UTK, USDT_VET, USDT_VIDT, USDT_VITE, USDT_VTHO, USDT_WAN, USDT_WAVES, USDT_WAXP, USDT_WIN, USDT_WING, USDT_WNXM, USDT_WRX, USDT_WTC, USDT_XEC, USDT_XEM, USDT_XLM, USDT_XMR, USDT_XRP, USDT_XTZ, USDT_XVG, USDT_XVS, USDT_YFI, USDT_YFII, USDT_YGG, USDT_ZEC, USDT_ZEN, USDT_ZIL, USDT_ZRX";
        
        public static Dictionary<string, string[]> opts = null;

        public static int bot_id
        {
            get
            {
                if (lib.opts.ContainsKey("id")) return Convert.ToInt32(lib.opts["id"][0]);
                else if (Bots.Count > 0) return Bots[0].Id;
                else return 0;
            }
        }

        //some shorthands
        public static decimal D(this string s) => Decimal.Round(Convert.ToDecimal(s), 8);
        public static int I(this string s) => Convert.ToInt32(s);
        public static void Delay(int seconds, int multiplier = 1000) => Task.Delay(seconds * multiplier).GetAwaiter().GetResult();
        public static void Print(string s, string eol = "\n") => Console.Write($"{s}{eol}");
        public static XCommasResponse<Bot[]> GetBots() => api.GetBotsAsync(limit: 100, accountId: accountId).GetAwaiter().GetResult();
        public static HttpResponseMessage POSTJson(string uri, string json) => (new HttpClient()).PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
        public static HttpResponseMessage GET(string uri) => (new HttpClient()).GetAsync(uri).GetAwaiter().GetResult();
        public static string DATA(this HttpResponseMessage res) => res.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        //create the bot with the passed in options using 3c api
        //we need the bot_id(s) and pair(s) in each bot to iterate over start conditions
        //if we are triggering them using signals ourselves binance can handle every 50 milliseconds supposedly
        public static bool CreateUpdateBot(this Dictionary<string, string[]> opts)
        {
            if (opts.Count == 0) opts.Add("help", null);


            //retrieve bot to update if it's an update
            Bot b = null;
            bool hasId = opts.ContainsKey("id");
            if (hasId)
            {
                var bots = GetBots().Data;
                foreach (Bot _bot in bots)
                {
                    if (_bot.Id == lib.bot_id)
                    {
                        b = _bot;
                        break;
                    }
                }
            }

            //we default to urma settings and then use opts to tailor them
            BotData bd = new BotData
            {
                Pairs = new[] { "USDT_BTC" },
                ActiveSafetyOrdersCount = 1,
                //DisableAfterDealsCount=1,
                //MaxPrice = 69,
                BaseOrderVolume = 10,
                SafetyOrderVolume = 10,
                MartingaleStepCoefficient = 1.5m,
                MartingaleVolumeCoefficient = 1.4m,
                //MinVolumeBtc24h = 10m,
                MaxActiveDeals = 1,
                ProfitCurrency = ProfitCurrency.QuoteCurrency,
                SafetyOrderStepPercentage = 1.87m,
                MaxSafetyOrders = 7,
                TakeProfitType = TakeProfitType.Total,
                TakeProfit = 3m,
                Name = $"3cc bot {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}",
                TrailingEnabled = false,
                TrailingDeviation = .2m,
                StartOrderType = StartOrderType.Limit,
                Strategies = new BotStrategy[]
                {
                    new NonStopBotStrategy{ }
                }.ToList(),
            };
            List<BotStrategy> bs = new List<BotStrategy>(); //if non-empty after loop then we replace the Strategies with this
            foreach (var opt in opts)
            {
                var param = opts[opt.Key];
                switch (opt.Key.ToLower())
                {
                    case "help":
                        Console.WriteLine($@"Welcome to 3Commas Commander!

usage:
 3cc -<option> <param> -<option2> <param1> <param2> ...
  **Available settings
  -help     This help menu
  -name     Bot name
  -disable  Do not enable the bot after creation
  -pairs    Pairs to include e.g. USDT_BTC,USDT_ETH,USDT_SHIB
  -bo       Base order size
  -so       Safety order size
  -ss       Safety order step scale
  -os       Safety order volume scale
  -mad      Max active deals
  -sos      Price deviation to open safety orders (% from initial order)
  -mstc     Max safety trades count
  -tp       Take profit (%)
  -ttp      Trailing take profit (%)
  -mp       Max Price (price must be below Max Price to open deal)
  -dadc     Disable After Deals Count (allow to only run once for example)

  **Available start conditions
  -asap     Open new trade asap signal
  -tv       Trading View signal e.g. -tv 1w sell 1d buy
              -tv <1m=0, 5m=1, 15m=2, 1h=3, 4h=4, 1d=5, 1w=6, m=7, c=8> <buy=0, sbuy=1, sell=2, ssell=3>
  -qfl      QFL signal e.g. -qfl 3 o
              -qfl <%> <o,d,c,p>
  -custom   Signal with TradingView (json start info is provided after creation)

  **Advanced settings
  -id       bot_id (skips creation of bot and does action(s) on existing bot)
  -cbs      uses custom tv to signal start condition from crypto base scanner (loops)
              -cbs <original,day_trade,conservative,position>   
  -cbs2     experimental version that uses MaxPrice
");
                        Console.ReadKey();
                        Environment.Exit(0); //don't create a bot yet in this case
                        break;

                    //ie. -pairs USDT_BTC,USDT_ETH,USDT_SHIB  (NO SPACES)
                    case "pairs":
                        if (b != null) b.Pairs = param.Join("").Split(',').Select(p => p.Trim()).ToArray();
                        else bd.Pairs = param.Join("").Split(',').Select(p => p.Trim()).ToArray();
                        break;
                    case "bo":
                        if (b != null) b.BaseOrderVolume = param[0].D();
                        else bd.BaseOrderVolume = param[0].D();
                        break;
                    case "so":
                        if (b != null) b.SafetyOrderVolume = param[0].D();
                        else bd.SafetyOrderVolume = param[0].D();
                        break;
                    case "ss":
                        if (b != null) b.MartingaleStepCoefficient = param[0].D();
                        else bd.MartingaleStepCoefficient = param[0].D();
                        break;
                    case "os":
                        if (b != null) b.MartingaleVolumeCoefficient = param[0].D();
                        else bd.MartingaleVolumeCoefficient = param[0].D();
                        break;
                    case "mad":
                        if (b != null) b.MaxActiveDeals = param[0].I();
                        else bd.MaxActiveDeals = param[0].I();
                        break;
                    case "sos":
                        if (b != null) b.SafetyOrderStepPercentage = param[0].D();
                        else bd.SafetyOrderStepPercentage = param[0].D();
                        break;
                    case "mstc":
                        if (b != null) b.MaxSafetyOrders = param[0].I();
                        else bd.MaxSafetyOrders = param[0].I();
                        break;
                    case "tp":
                        if (b != null) b.TakeProfit = param[0].D();
                        else bd.TakeProfit = param[0].D();
                        break;
                    case "name":
                        if (b != null)  b.Name = param.Join(" "); //we will allow spaces here
                        else bd.Name = param.Join(" ");
                        break;
                    case "ttp":
                        if (b != null)
                        {
                            b.TrailingEnabled = true;
                            b.TrailingDeviation = param[0].D();
                        }
                        else
                        {
                            bd.TrailingEnabled = true;
                            bd.TrailingDeviation = param[0].D();
                        }
                        break;
                    case "asap":
                        bs.Add(new NonStopBotStrategy { });
                        break;
                    //i.e. -qfl 4 P or -qfl 3 4
                    case "qfl":
                        QflBotStrategy qbs = new QflBotStrategy { Options = new QflOptions { Percent = 3m, Type = QflType.Original } };
                        if (param.Length > 0) qbs.Options.Percent = param[0].D(); //first parameter is percent
                        if (param.Length > 1)
                        {
                            //1=C, 2=D, 3=O, 4=P .. number is useful if doing random input
                            int res;
                            if (int.TryParse(param[1], out res)) qbs.Options.Type = (QflType)res;
                            else
                            {
                                switch (param[1].ToLower())
                                {
                                    case "c": qbs.Options.Type = QflType.ConservativeTrader; break;
                                    case "d": qbs.Options.Type = QflType.DayTrading; break;
                                    case "o": qbs.Options.Type = QflType.Original; break;
                                    case "pcd":
                                    case "p": qbs.Options.Type = QflType.PositionTrader; break;
                                }
                            }
                        }
                        bs.Add(qbs);
                        break;
                    case "tv":
                        //<1m=0, 5m=1, 15m=2, 1h=3, 4h=4, 1d=5, 1w=6, m=7, c=8> <buy=0, sbuy=1, sell=2, ssell=3>
                        TradingViewBotStrategy tvs = new TradingViewBotStrategy { Options = new TradingViewOptions { Time = TradingViewTime.OneHour, Type = TradingViewIndicatorType.StrongBuy } };
                        if (param.Length > 0)
                        {
                            int res;
                            if (int.TryParse(param[0], out res)) tvs.Options.Time = (TradingViewTime)res;
                            else
                            {
                                switch (param[0].ToLower())
                                {
                                    case "1m": tvs.Options.Time = TradingViewTime.OneMinute; break;
                                    case "5m": tvs.Options.Time = TradingViewTime.FiveMinutes; break;
                                    case "15m": tvs.Options.Time = TradingViewTime.FifteenMinutes; break;
                                    case "1h": tvs.Options.Time = TradingViewTime.OneHour; break;
                                    case "4h": tvs.Options.Time = TradingViewTime.FourHours; break;
                                    case "1d": tvs.Options.Time = TradingViewTime.OneDay; break;
                                    case "1w": tvs.Options.Time = TradingViewTime.OneWeek; break;
                                    case "m": tvs.Options.Time = TradingViewTime.OneMonth; break;
                                    case "c": tvs.Options.Time = TradingViewTime.Cumulative; break;
                                }
                            }
                        }
                        if (param.Length > 1)
                        {
                            int res;
                            if (int.TryParse(param[1], out res)) tvs.Options.Type = (TradingViewIndicatorType)res;
                            else
                            {
                                switch (param[1].ToLower())
                                {
                                    case "buy": tvs.Options.Type = TradingViewIndicatorType.Buy; break;
                                    case "sbuy": tvs.Options.Type = TradingViewIndicatorType.StrongBuy; break;
                                    case "sell": tvs.Options.Type = TradingViewIndicatorType.Sell; break;
                                    case "ssell": tvs.Options.Type = TradingViewIndicatorType.StrongSell; break;
                                }
                            }
                        }
                        bs.Add(tvs);
                        break;
                    case "custom":
                        UnknownStrategy us = new UnknownStrategy("tv_custom_signal");
                        bs.Add(us);
                        //TaPresetsBotStrategy tpbs = new TaPresetsBotStrategy {  Options = new TaPresetsOptions { Type = TaPresetsType. } }
                        break;
                    case "mp":
                        if (b != null)  b.MaxPrice = param[0].D();
                        else bd.MaxPrice = param[0].D();
                        break;
                    case "dadc":
                        if (b != null) b.DisableAfterDealsCount = param[0].I();
                        else bd.DisableAfterDealsCount = param[0].I();
                        break;
                }
            }
            if (bs.Count > 0)
            {
                if (b != null)  b.Strategies = bs;
                else bd.Strategies = bs;
            }



            XCommasResponse<Bot> bot = null;
            if (!hasId)
            {
                bot = api.CreateBotAsync(accountId, Strategy.Long, bd).GetAwaiter().GetResult();
                if (!bot.IsSuccess)
                {
                    Console.WriteLine($"{bd.Pairs[0]} doesn't exist on exchange");
                    return false;
                }
                if (!opts.ContainsKey("disable") && !hasId) api.EnableBotAsync(bot.Data.Id).GetAwaiter().GetResult();
                Bots.Add(bot.Data);
                if (bot.IsSuccess) Console.WriteLine($"Created bot named {bot.Data.Name}");
            }
            else if (b != null)
            {
                bot = api.UpdateBotAsync(bot_id, new BotUpdateData(b)).GetAwaiter().GetResult();
                if (!bot.IsSuccess) Print($"Failed to update bot {b.Name}");
                if (Bots.Count == 0) Bots.Add(b); //only need a single item when updating since we are operating on the same bot in case of loops that update bot
                else Bots[0] = b;
                Console.WriteLine($"Updated bot named {b.Name}");
            }

            

            if (opts.ContainsKey("custom") && opts["custom"].Length == 0) //only output if not DIY custom
            {
                string jsonPair = "";
                if (bot.Data.Pairs.Length > 1) jsonPair = $",\n  \"pair\": \"{bot.Data.Pairs[0]}\"";
                Console.WriteLine($@"
Use this JSON in your TradingView webhook:
{{
  ""message_type"": ""bot"",
  ""bot_id"": {bot.Data.Id},
  ""email_token"": ""<enter email_token from 3c here>"",
  ""delay_seconds"": 0{jsonPair}
}}");
            }

            return true;

            //delete the test bot
            //api.DeleteBotAsync(bot.Data.Id).GetAwaiter().GetResult();

            //next iterate through Bots if -custom is passed to signal the bots with a custom signal
        }

        //easier string join syntax
        public static string Join(this string[] arr, string s) { return String.Join(s, arr); }


        public static Dictionary<string, string[]> GetOptions() { return GetOptions(null); }

        //get hyphenated option syntax ie "-opt1 1 2 -opt2 3 4 -opt3 param5 param6" 
        public static Dictionary<string, string[]> GetOptions(this string args)
        {
            var ret = new Dictionary<string, string[]>();
            if (args == null) args = Environment.GetCommandLineArgs().Join(" ");
            string[] opts = args.Trim().Split('-').Skip(1).ToArray(); //.ToLower()
            foreach (string opt in opts)
            {
                string[] arr = Regex.Replace(opt.Trim(), @"\s+", " ").Split(' ');
                ret.Add(arr[0], arr.Skip(1).ToArray());
            }
            lib.opts = ret; //save a copy for use in other methods
            return ret;
        }


        //read lines from file into array ie var lines = "test.txt".GetLines();
        public static string[] GetLines(this string file, string CustomMessage = "")
        {
            string[] arr = null;
            try
            {
                arr = File.ReadAllLines(file, Encoding.UTF8);
            }
            catch
            {
                Console.WriteLine(CustomMessage);
                Environment.Exit(1);
            }
            return arr;
        }

        public static bool StartTVCustom(int bot_id, string email_token, string pair = null)
        {
            string jsonPair = "";
            if (pair != null) jsonPair = $",\n  \"pair\": \"{pair}\"";
            string json = $@"{{
  ""message_type"": ""bot"",
  ""bot_id"": {bot_id},
  ""email_token"": ""{email_token}"",
  ""delay_seconds"": 0{jsonPair}
}}";
            var res = POSTJson("https://" + "3commas.io/trade_signal/trading_view", json);
            return res.StatusCode == HttpStatusCode.OK;
        }

        public static bool SellTVCustom(int bot_id, string email_token, string pair = null)
        {
            string jsonPair = "";
            if (pair != null) jsonPair = $",\n  \"pair\": \"{pair}\"";
            string json = $@"{{
  ""action"": ""close_at_market_price_all"",
  ""message_type"": ""bot"",
  ""bot_id"": {bot_id},
  ""email_token"": ""{email_token}"",
  ""delay_seconds"": 0{jsonPair}
}}";
            var res = POSTJson("https://" + "3commas.io/trade_signal/trading_view", json);
            return res.StatusCode == HttpStatusCode.OK;
        }

        //don't necessarily need dates from binance .. but if we do we can get them as datetime
        public static DateTime BinanceTimeStampToUtcDateTime(string binanceTimeStamp)
        {
            // Binance timestamp is milliseconds past epoch
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return epoch.AddMilliseconds(Convert.ToDouble(binanceTimeStamp));
        }

        public static dynamic GetKCPrice(string pair)
        {
            string[] arr = pair.Split('_');
            return GetJSON<KCL1Root>("https://" + $"api.kucoin.com/api/v1/market/orderbook/level1?symbol={arr[1]}-{arr[0]}");
        }
        public static dynamic GetJSON<T>(string endpoint)
        {
            var res = GET(endpoint);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                string json = res.DATA();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                //return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
            }
            return null;
        }

        public static IEnumerable<Quote> GetCandles(string pair = "SHIBUSDT", string interval = "1h", int period = 21, string exchange = "binance")
        {
            List<Quote> quotes = new List<Quote>();
            switch (exchange)
            {
                case "binance":
                    var res = GET("https://" + $"api.binance.com/api/v3/klines?symbol={pair}&interval={interval}&limit={period}");
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        string output = res.DATA();
                        output = output.Substring(2, output.Length - 4);
                        string[] candles = output.Split(new string[] { "],[" }, StringSplitOptions.None);
                        
                        foreach (string candle in candles)
                        {
                            string[] s = candle.Split(',');
                            Quote q = new Quote();
                            q.Date = lib.BinanceTimeStampToUtcDateTime(s[0]);
                            q.Open = Convert.ToDecimal(s[1].Replace("\"", ""));
                            q.High = Convert.ToDecimal(s[2].Replace("\"", ""));
                            q.Low = Convert.ToDecimal(s[3].Replace("\"", ""));
                            q.Close = Convert.ToDecimal(s[4].Replace("\"", ""));
                            q.Volume = Convert.ToDecimal(s[5].Replace("\"", ""));
                            quotes.Add(q);
                        }
                    }
                    break;
            }
            return quotes;
        }

        public static void TriggerCBS()
        {
            HashSet<string> botPairs = null; //= new HashSet<string>(lib.usdt_all);
            //var bots = lib.api.GetBotsAsync(1, null, null, 64861__).GetAwaiter().GetResult().Data;
            var bots = GetBots().Data;
            Bot b = null;

            //broke it out since I may want to do more
            foreach (Bot bot in bots) {
                if (bot.Id == lib.bot_id)
                {
                    b = bot;
                    break;
                }
            }

            if (b != null) botPairs = new HashSet<string>(b.Pairs);
            else
            {
                Print($"Exiting - could not find bot with bot_ID {lib.bot_id}");
                Environment.Exit(0);
            }
            //var opts = lib.GetOptions($"-pairs {lib.usdt_all.Join(",")} -bo 20 -so 30 -mstc 2 -name CBS_multi -tp 3 -sos 5 -custom -mad 2 -ss 2.5 -os 1.44"); // -disable
            //opts.CreateUpdateBot();
            //var bot = lib.api.GetBotsAsync(botId: 6858387).GetAwaiter().GetResult();
            //var hasRun = new Dictionary<string, bool>();
            while (true)
            {
                dynamic data = null;
                Console.WriteLine($"CBS base type: {odcp[odcp_index]}");
                try { data = lib.GetJSON<CBSRoot>(lib.cbs2 + odcp[odcp_index]); } catch { }
                odcp_index--;
                if (odcp_index < 1) odcp_index = 3;

                int cnt = 0;
                foreach (var pair in data.bases)
                {
                    if (pair.quoteCurrency == "usdt")
                    {
                        string pairName = pair.longName.ToString().Replace("-", "_");
                        var latestBase = pair.latestBase;
                        var stats = pair.marketStats[odcp_index];
                        decimal latestBasePrice = lib.D(latestBase.price);
                        decimal latestBaseLessHalfMedian = 0;
                        //decimal latestBaseCurrentDrop = -Convert.ToDecimal(latestBase.currentDrop);
                        //decimal latestBaseBounce = Convert.ToDecimal(latestBase.bounce) * 0.8m;
                        if (botPairs.Contains(pairName))
                        {
                            //NEED TO COMPARE CURRENT PRICE TO latestBasePrice AND ALSO ROUND ROBIN WITH bot_ID 64861__

                            latestBaseLessHalfMedian = Decimal.Round(latestBasePrice * 0.01m * (100m + lib.D(stats.medianDrop) * 0.5m), 8);
                            dynamic price = null;
                            try
                            {
                                price = lib.GetKCPrice(pairName);
                                lib.Print($"{pairName} - latestBasePrice (less half median drop): {latestBaseLessHalfMedian} latestPrice: {price.data.price}");
                            }
                            catch { }
                            if (price == null) continue;
                            if (lib.D(price.data.price) <= latestBaseLessHalfMedian)
                            {
                                if (!b.IsEnabled) lib.Print($"Cannot start deal on {pairName} because {b.Name} is not enabled");
                                else if (b.IsEnabled && b.ActiveDealsCount == b.MaxActiveDeals)
                                    lib.Print($"Cannot start deal on {pairName} because {b.Name} has no available active deals");
                                else
                                {
                                    lib.Print($"Attempting to start deal on {pairName}");
                                    lib.StartTVCustom(lib.bot_id, lib.email_token, pairName);
                                }
                            }
                            lib.Delay(2);
                        }
                        else
                        {
                            lib.Print($"Cannot start deal on {pairName} because it was not included or doesn't exist on exchange");
                        }
                        cnt++;
                    }
                }
                if (cnt == 0) lib.Print($"CBS scanner did not find any pairs. Trying again in 30 seconds.");
                lib.Delay(30);
            }
        }







    }
}

