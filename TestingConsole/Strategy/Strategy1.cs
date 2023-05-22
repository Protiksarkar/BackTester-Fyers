//using AutoMapper;
//using FyersAPI;
//using Microsoft.VisualBasic;
//using Skender.Stock.Indicators;
//using System.Runtime.InteropServices;
//using System.Runtime.Intrinsics.X86;
//using TestingConsole.DBServices;
//using TestingConsole.DTOs;
//using TestingConsole.Services;

//namespace TestingConsole.Strategy
//{
//    internal class Strategy1
//    {
//        internal StrategyLifecycle strategyState;
//        private BackTestService btService;
//        private List<MarketQuoteDTO> quotes;
//        private int interval;
//        private InstrumentDTO tradingInst;
//        private float capital;
//        private List<MarketQuoteDTO> candles;
//        private List<EmaResult> ema1;
//        private List<EmaResult> ema2;
//        private List<EmaResult> ema3;
//        private List<EmaResult> ema4;
//        private List<EmaResult> ema5;
//        private List<RsiResult> rsi;
//        private Dictionary<int, bool> orderDict;
//        private decimal sl;
//        private IMapper mapper;
//        private bool isBuyOrder;
//        private OrderDTO placedOrder;
//        private bool isProfitEntered;

//        public Strategy1(TradingRepo repository, BackTestService dtaService, IMapper mpr)
//        {
//            btService = dtaService;
//            placedOrder = new OrderDTO();
//            orderDict = new Dictionary<int, bool>();
//            sl = 0;
//            mapper = mpr;
//        }

//        public void OnRun(List<MarketQuoteDTO> qts, int intvl, InstrumentDTO tdingInst, float cap = 100000)
//        {
//            //variable setup
//            quotes = qts;
//            interval = intvl;
//            tradingInst = tdingInst;
//            capital = cap;

//            //indicators preparation
//            candles = (mapper.Map<IEnumerable<MarketQuoteDTO>>(qts.Aggregate<MarketQuoteDTO>(TimeSpan.FromSeconds(interval)))).ToList();
//            ema1 = candles.Use(CandlePart.HLC3).GetEma(8).ToList();
//            ema2 = candles.Use(CandlePart.HLC3).GetEma(20).ToList();
//            ema3 = candles.Use(CandlePart.HLC3).GetEma(50).ToList();
//            ema4 = candles.Use(CandlePart.HLC3).GetEma(5).ToList();
//            ema5 = candles.Use(CandlePart.HLC3).GetEma(3).ToList();
//            rsi = candles.Use(CandlePart.HLC3).GetRsi(8).ToList();

//            //iterate through each quote
//            RunIteration();
//        }

//        public void RunIteration()
//        {
//            strategyState = StrategyLifecycle.OPEN;
//            foreach (var quote in candles)
//            {
//                if (quote != null)
//                {
//                    switch (strategyState)
//                    {
//                        case StrategyLifecycle.OPEN:
//                            strategyState = EnterTrade(quote, tradingInst);
//                            break;
//                        case StrategyLifecycle.OPEN_ORDER_PLACED:
//                            strategyState = ExitTrade(quote, tradingInst);
//                            break;
//                    }
//                }
//                else
//                {
//                    throw new Exception("Custom exception");
//                }
//            }
//        }

//        public StrategyLifecycle EnterTrade(MarketQuoteDTO quote, InstrumentDTO inst)
//        {
//            var lastQuote1 = Utility.GetPrevQuote(ref candles, quote.Date, -1, interval);
//            if (lastQuote1 == null)
//                return strategyState;

//            //variable preparation
//            var lastQuote2 = Utility.GetPrevQuote(ref candles, quote.Date, -2, interval);
//            var lastQuote3 = Utility.GetPrevQuote(ref candles, quote.Date, -3, interval);

//            var ema1Value = ema1.FirstOrDefault(x => x.Date == lastQuote1.Date);
//            var ema2Value = ema2.FirstOrDefault(x => x.Date == lastQuote1.Date);
//            var ema3Value = ema3.FirstOrDefault(x => x.Date == lastQuote1.Date);
//            var ema4Value = ema4.FirstOrDefault(x => x.Date == lastQuote1.Date);
//            var ema5Value = ema5.FirstOrDefault(x => x.Date == lastQuote1.Date);
//            var rsi13Value = rsi.FirstOrDefault(x => x.Date == lastQuote1.Date);

//            //Data validation
//            if (lastQuote1 == null || lastQuote2 == null || lastQuote3 == null || ema1Value.Ema == null || ema2Value.Ema == null || ema3Value.Ema == null)
//                return strategyState;

//            if (ValidEntryTime(quote))
//            {
//                if (

//                    (ema1Value.Ema > ema2Value.Ema && ema2Value.Ema > ema3Value.Ema && (Convert.ToDouble(lastQuote1.Close) > ema2Value.Ema))
//                    && ((lastQuote1.Low < lastQuote2.Low && lastQuote2.Low < lastQuote3.Low) && Utility.IsHighBreak(quote.High, lastQuote1.High) /*&& rsi13Value.Rsi < 40*/)
//                    //&& (ema5Value.Ema<ema4Value.Ema)
//                    )
//                {
//                    //buy
//                    PlaceBuyOrder(quote, lastQuote1, inst, (float)ema1Value.Ema, (float)ema2Value.Ema, (float)ema3Value.Ema);
//                    return StrategyLifecycle.OPEN_ORDER_PLACED;
//                }
//                else if (

//                    (ema1Value.Ema < ema2Value.Ema && ema2Value.Ema < ema3Value.Ema && (Convert.ToDouble(lastQuote1.Close) < ema2Value.Ema))
//                    && ((lastQuote1.High > lastQuote2.High && lastQuote2.High > lastQuote3.High) && Utility.IsLowBreak(quote.Low, lastQuote1.Low) /*&& rsi13Value.Rsi > 60*/)
//                    //&& (ema5Value.Ema>ema4Value.Ema)
//                    )
//                {
//                    //sell
//                    PlaceSellOrder(quote, lastQuote1, inst, (float)ema1Value.Ema, (float)ema2Value.Ema, (float)ema3Value.Ema);
//                    return StrategyLifecycle.OPEN_ORDER_PLACED;
//                }
//            }
//            return strategyState;
//        }

//        public StrategyLifecycle ExitTrade(MarketQuoteDTO quote, InstrumentDTO inst)
//        {
//            var lastQuote1 = Utility.GetPrevQuote(ref candles, quote.Date, -1, interval);

//            if (ValidExitTime(quote))
//            {
//                return ExitPosition(quote.Date, quote.Close);
//            }
//            else if (isBuyOrder)
//            {

//                if (quote.Low < sl)
//                {
//                    //sell
//                    return ExitPosition(quote.Date, sl - .1m);
//                }
//                else if ((quote.Low < (decimal)placedOrder.Price) && (quote.Date > placedOrder.TimeStamp.AddSeconds(interval * 2)))
//                {
//                    return ExitPosition(quote.Date, (decimal)placedOrder.Price - 0.1m);
//                }
//                else if ((quote.High - Convert.ToDecimal(placedOrder.Price)) >= ((decimal)placedOrder.Price - sl) * 2m) /*|| (quote.Low < lastQuote1.Low)*/
//                {
//                    //sell
//                    return ExitPosition(quote.Date, (decimal)placedOrder.Price + (((decimal)placedOrder.Price - sl) * 2m) );
//                }
//                //|| ((quote.Low < (decimal)placedOrder.Price) && (quote.Date > placedOrder.TimeStamp.AddSeconds(interval*2)))
//                //|| ((quote.High - Convert.ToDecimal(placedOrder.Price)) >= ((decimal)placedOrder.Price - sl) * 1.5m) /*|| (quote.Low < lastQuote1.Low)*/

//            }
//            else
//            {

//                if (quote.High > sl)
//                {
//                    //buy
//                    return ExitPosition(quote.Date, sl + .1m);
//                }
//                else if ((quote.High > (decimal)placedOrder.Price) && (quote.Date > placedOrder.TimeStamp.AddSeconds(interval * 2)))
//                {
//                    return ExitPosition(quote.Date, (decimal)placedOrder.Price+ 0.1m);
//                }
//                else if ((Convert.ToDecimal(placedOrder.Price) - quote.Low) >= (sl - (decimal)placedOrder.Price) * 2m)
//                {
//                    //buy
//                    return ExitPosition(quote.Date, (decimal)placedOrder.Price - (sl - (decimal)placedOrder.Price) * 2m);
//                }
//                //|| ((quote.High > (decimal)placedOrder.Price) && (quote.Date > placedOrder.TimeStamp.AddSeconds(interval*2)))
//                //|| ((Convert.ToDecimal(placedOrder.Price) - quote.Low) >= (sl - (decimal)placedOrder.Price) * 1.5m) /*|| (quote.High > lastQuote1.High)*/
//            }
//            return strategyState;
//        }

//        private bool ValidEntryTime(MarketQuoteDTO quote)
//        {
//            var startTime = new DateTime(quote.Date.Year, quote.Date.Month, quote.Date.Day, 9, 25, 0);
//            var endTime = new DateTime(quote.Date.Year, quote.Date.Month, quote.Date.Day, 15, 0, 0);
//            return (quote.Date >= startTime && quote.Date <= endTime);
//        }

//        private bool ValidExitTime(MarketQuoteDTO quote)
//        {
//            return quote.Date >= new DateTime(quote.Date.Year, quote.Date.Month, quote.Date.Day, 15, 10, 0);
//        }

//        private void PlaceBuyOrder(MarketQuoteDTO quote, MarketQuoteDTO lastQuote1, InstrumentDTO inst, float ema1, float ema2, float ema3)
//        {
//            sl = lastQuote1.Low;
//            isBuyOrder = true;
//            //var quantity = Utility.GetQuantity(inst.LotSize, Convert.ToSingle(lastQuote1.High - quote.Close), capital * 0.02f);
//            //placedOrder = btService.PlaceEntryOrder(inst.SymbolTicker, (MarketQuoteDTO)quote, quantity * -1);
//            var quantity = Utility.GetQuantity(inst.LotSize, Convert.ToSingle((lastQuote1.High + .1m) - sl), capital * 0.02f);
//            placedOrder = btService.PlaceEntryOrder(inst.SymbolTicker, quote.Date, (lastQuote1.High + .1m), quantity, sl, ema1, ema2, ema3);
//            isProfitEntered = false;
//            //orderDict.Add(placedOrder.Id, true);
//        }

//        private void PlaceSellOrder(MarketQuoteDTO quote, MarketQuoteDTO lastQuote1, InstrumentDTO inst, float ema1, float ema2, float ema3)
//        {
//            sl = lastQuote1.High;
//            isBuyOrder = false;
//            //var quantity = Utility.GetQuantity(inst.LotSize, Convert.ToSingle(lastQuote1.High - quote.Close), capital * 0.02f);
//            //placedOrder = btService.PlaceEntryOrder(inst.SymbolTicker, (MarketQuoteDTO)quote, quantity * -1);
//            var quantity = Utility.GetQuantity(inst.LotSize, Convert.ToSingle(sl - (lastQuote1.Low - 0.1m)), capital * 0.02f);
//            placedOrder = btService.PlaceEntryOrder(inst.SymbolTicker, quote.Date, (lastQuote1.Low - 0.1m), quantity * -1, sl, ema1, ema2, ema3);
//            isProfitEntered = false;
//            //orderDict.Add(placedOrder.Id, true);
//        }

//        private StrategyLifecycle ExitPosition(DateTime time, decimal price)
//        {
//            btService.PlaceExitOrder(placedOrder, time, price);
//            isProfitEntered = false;

//            return StrategyLifecycle.OPEN;
//        }
//    }
//}
