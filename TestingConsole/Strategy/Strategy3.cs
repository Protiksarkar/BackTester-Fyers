//using AutoMapper;
//using FyersAPI;
//using Skender.Stock.Indicators;
//using TestingConsole.DBServices;
//using TestingConsole.DTOs;
//using TestingConsole.Services;

//namespace TestingConsole.Strategy
//{
//    internal class Strategy2
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

//        public Strategy2(TradingRepo repository, BackTestService dtaService, IMapper mpr)
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
//            ema1 = candles.Use(CandlePart.HLC3).GetEma(3).ToList();
//            ema2 = candles.Use(CandlePart.HLC3).GetEma(5).ToList();
//            ema3 = candles.Use(CandlePart.HLC3).GetEma(8).ToList();
//            ema4 = candles.Use(CandlePart.HLC3).GetEma(20).ToList();
//            ema5 = candles.Use(CandlePart.HLC3).GetEma(50).ToList();
//            rsi = candles.Use(CandlePart.HLC3).GetRsi(8).ToList();

//            //iterate through each quote
//            RunIteration();
//        }

//        public void RunIteration()
//        {
//            strategyState = StrategyLifecycle.OPEN;
//            for (int i = 0; i < candles.Count; i++)
//            {
//                if (candles[i] != null)
//                {
//                    switch (strategyState)
//                    {
//                        case StrategyLifecycle.OPEN:
//                            strategyState = EnterTrade(i, tradingInst);
//                            break;
//                        case StrategyLifecycle.OPEN_ORDER_PLACED:
//                            strategyState = ExitTrade(i, tradingInst);
//                            break;
//                    }
//                }
//                else
//                {
//                    throw new Exception("Custom exception");
//                }
//            }
//        }

//        public StrategyLifecycle EnterTrade(int counter, InstrumentDTO inst)
//        {
//            if (counter < 50)
//                return strategyState;
//            else
//            {
//                if (ValidEntryTime(candles[counter]))
//                {
//                    if (
//                        (ema3[counter].Ema > ema4[counter].Ema) /*&& (ema4[counter].Ema > ema5[counter].Ema)*/
//                        && (ema1[counter].Ema > ema2[counter].Ema && ema1[counter - 1].Ema < ema2[counter - 1].Ema)
//                        )
//                    {
//                        //buy
//                        PlaceBuyOrder(candles[counter], candles[counter - 1], inst);
//                        return StrategyLifecycle.OPEN_ORDER_PLACED;
//                    }
//                    else if (
//                        (ema3[counter].Ema < ema4[counter].Ema) /*&& (ema4[counter].Ema < ema5[counter].Ema)*/
//                        && (ema1[counter].Ema < ema2[counter].Ema && ema1[counter - 1].Ema > ema2[counter - 1].Ema)
//                        )
//                    {
//                        //sell
//                        PlaceSellOrder(candles[counter], candles[counter - 1], inst);
//                        return StrategyLifecycle.OPEN_ORDER_PLACED;
//                    }
//                }
//                return strategyState;
//            }
//        }

//        public StrategyLifecycle ExitTrade(int counter, InstrumentDTO inst)
//        {

//            if (ValidExitTime(candles[counter]))
//            {
//                return ExitPosition(candles[counter].Date, candles[counter].Close);
//            }
//            else if (isBuyOrder)
//            {

//                if (candles[counter].Low < sl)
//                {
//                    //sell
//                    return ExitPosition(candles[counter].Date, sl - .1m);
//                }
//                else if (/*((candles[counter].High - Convert.ToDecimal(placedOrder.Price)) >= ((decimal)placedOrder.Price - sl) * 2m) &&*/ (ema1[counter].Ema < ema2[counter].Ema && ema1[counter - 1].Ema > ema2[counter - 1].Ema))
//                {
//                    //sell
//                    return ExitPosition(candles[counter].Date, (decimal)placedOrder.Price + (((decimal)placedOrder.Price - sl) * 2m));
//                }
//                //else if ((candles[counter].High - Convert.ToDecimal(placedOrder.Price)) >= ((decimal)placedOrder.Price - sl) * 2m) /*|| (quote.Low < lastQuote1.Low)*/
//                //{
//                //    //sell
//                //    return ExitPosition(candles[counter].Date, (decimal)placedOrder.Price + (((decimal)placedOrder.Price - sl) * 2m));
//                //}
//            }
//            else
//            {

//                if (candles[counter].High > sl)
//                {
//                    //buy
//                    return ExitPosition(candles[counter].Date, sl + .1m);
//                }
//                else if (/*((Convert.ToDecimal(placedOrder.Price) - candles[counter].Low) >= (sl - (decimal)placedOrder.Price) * 2m) &&*/ (ema1[counter].Ema > ema2[counter].Ema && ema1[counter - 1].Ema < ema2[counter - 1].Ema))
//                {
//                    //buy
//                    return ExitPosition(candles[counter].Date, (decimal)placedOrder.Price - (sl - (decimal)placedOrder.Price) * 2m);
//                }
//                //else if ((Convert.ToDecimal(placedOrder.Price) - candles[counter].Low) >= (sl - (decimal)placedOrder.Price) * 2m)
//                //{
//                //    //buy
//                //    return ExitPosition(candles[counter].Date, (decimal)placedOrder.Price - (sl - (decimal)placedOrder.Price) * 2m);
//                //}
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

//        private void PlaceBuyOrder(MarketQuoteDTO quote, MarketQuoteDTO lastQuote1, InstrumentDTO inst)
//        {
//            sl = lastQuote1.Low;
//            isBuyOrder = true;
//            var quantity = Utility.GetQuantity(inst.LotSize, Convert.ToSingle((lastQuote1.High + .1m) - sl), capital * 0.02f);
//            placedOrder = btService.PlaceEntryOrder(inst.SymbolTicker, quote.Date, (lastQuote1.High + .1m), quantity, sl);
//            isProfitEntered = false;
//            //orderDict.Add(placedOrder.Id, true);
//        }

//        private void PlaceSellOrder(MarketQuoteDTO quote, MarketQuoteDTO lastQuote1, InstrumentDTO inst)
//        {
//            sl = lastQuote1.High;
//            isBuyOrder = false;
//            var quantity = Utility.GetQuantity(inst.LotSize, Convert.ToSingle(sl - (lastQuote1.Low - 0.1m)), capital * 0.02f);
//            placedOrder = btService.PlaceEntryOrder(inst.SymbolTicker, quote.Date, (lastQuote1.Low - 0.1m), quantity * -1, sl);
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
