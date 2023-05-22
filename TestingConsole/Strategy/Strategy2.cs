using AutoMapper;
using FyersAPI;
using Skender.Stock.Indicators;
using System.Diagnostics.Metrics;
using TestingConsole.DBServices;
using TestingConsole.DTOs;
using TestingConsole.Services;

namespace TestingConsole.Strategy
{
    internal class Strategy2
    {
        internal StrategyLifecycle strategyState;
        private BackTestService btService;
        private List<MarketQuoteDTO> quotes;
        private int interval;
        private InstrumentDTO tradingInst;
        private float capital;
        private List<MarketQuoteDTO> candles;
        private List<EmaResult> ema1;
        private List<EmaResult> ema2;
        private List<EmaResult> ema3;
        private List<EmaResult> ema4;
        private List<EmaResult> ema5;
        private List<RsiResult> rsi;
        private Dictionary<int, bool> orderDict;
        private decimal sl;
        private IMapper mapper;
        private bool isBuyOrder;
        private OrderDTO placedOrder;
        private bool isProfitEntered;
        private int counter;

        public Strategy2(TradingRepo repository, BackTestService dtaService, IMapper mpr)
        {
            btService = dtaService;
            placedOrder = new OrderDTO();
            orderDict = new Dictionary<int, bool>();
            sl = 0;
            mapper = mpr;
        }

        public void OnRun(List<MarketQuoteDTO> qts, int intvl, InstrumentDTO tdingInst, float cap = 100000)
        {
            //variable setup
            quotes = qts;
            interval = intvl;
            tradingInst = tdingInst;
            capital = cap;

            //indicators preparation
            candles = (mapper.Map<IEnumerable<MarketQuoteDTO>>(qts.Aggregate<MarketQuoteDTO>(TimeSpan.FromSeconds(interval)))).ToList();
            ema1 = candles.Use(CandlePart.Close).GetEma(3).ToList();
            ema2 = candles.Use(CandlePart.Close).GetEma(20).ToList();
            ema3 = candles.Use(CandlePart.Close).GetEma(3).ToList();
            ema4 = candles.Use(CandlePart.Close).GetEma(20).ToList();
            rsi = candles.Use(CandlePart.Close).GetRsi(8).ToList();

            //iterate through each candle
            RunIteration();
        }

        public void RunIteration()
        {
            strategyState = StrategyLifecycle.OPEN;
            for (int i = 0; i < candles.Count; i++)
            {
                if (strategyState == StrategyLifecycle.CLOSED)
                    strategyState = StrategyLifecycle.OPEN;
                
                if (candles[i] != null)
                {
                    if (strategyState == StrategyLifecycle.OPEN)
                        strategyState = EnterTrade();
                    if (strategyState == StrategyLifecycle.OPEN_ORDER_PLACED)
                        strategyState = ExitTrade();
                }
                else
                {
                    throw new Exception("Custom exception");
                }
            }
        }

        public StrategyLifecycle EnterTrade()
        {
            if (counter > 30 && ValidEntryTime(candles[counter]))
            {
                return strategyState;
            }
            else
                return strategyState;
            
        }

        public StrategyLifecycle ExitTrade()
        {
            if (ValidExitTime(candles[counter]))
            {
                ExitPosition(candles[counter+1].Date, candles[counter+1].Open);
                return StrategyLifecycle.CLOSED; 
            }
            else if (isBuyOrder)
            {
                if ((ema3[counter].Ema < ema4[counter].Ema && ema3[counter - 1].Ema > ema4[counter - 1].Ema))
                {
                    //sell
                    ExitPosition(candles[counter+1].Date, candles[counter + 1].Open);
                    return StrategyLifecycle.CLOSED;
                }
            }
            else
            {
                if ((ema3[counter].Ema > ema4[counter].Ema && ema3[counter - 1].Ema < ema4[counter - 1].Ema))
                {
                    //buy
                    ExitPosition(candles[counter+1].Date, candles[counter + 1].Open);
                    return StrategyLifecycle.CLOSED;
                }
            }
            return strategyState;
        }

        private bool ValidEntryTime(MarketQuoteDTO quote)
        {
            var startTime = new DateTime(quote.Date.Year, quote.Date.Month, quote.Date.Day, 9, 25, 0);
            var endTime = new DateTime(quote.Date.Year, quote.Date.Month, quote.Date.Day, 15, 0, 0);
            return (quote.Date >= startTime && quote.Date <= endTime);
        }

        private bool ValidExitTime(MarketQuoteDTO quote)
        {
            return quote.Date >= new DateTime(quote.Date.Year, quote.Date.Month, quote.Date.Day, 15, 15, 0);
        }

        private void PlaceBuyOrder()
        {
            sl = candles.Where((x, i) => i <= counter && i > counter - 5).Min(x=>x.Low);
            isBuyOrder = true;
            var quantity = Utility.GetQuantity(tradingInst.LotSize, Convert.ToSingle((candles[counter+1].Open + .1m) - sl), capital * 0.02f);
            placedOrder = btService.PlaceEntryOrder(tradingInst.SymbolTicker, candles[counter+1].Date, (candles[counter+1].Open + .1m), quantity);
        }

        private void PlaceSellOrder()
        {
            sl = candles.Where((x, i) => i <= counter && i > counter - 5).Max(x => x.High);
            isBuyOrder = false;
            var quantity = Utility.GetQuantity(tradingInst.LotSize, Convert.ToSingle(sl - (candles[counter+1].Open - 0.1m)), capital * 0.02f);
            placedOrder = btService.PlaceEntryOrder(tradingInst.SymbolTicker, candles[counter+1].Date, (candles[counter+1].Open - 0.1m), quantity * -1, (float)sl);
        }

        private void ExitPosition(DateTime time, decimal price)
        {
            btService.PlaceExitOrder(placedOrder, time, price);
        }
    }
}
