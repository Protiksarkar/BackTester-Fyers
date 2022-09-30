using AutoMapper;
using Microsoft.VisualBasic;
using Skender.Stock.Indicators;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using TestingConsole.DBServices;
using TestingConsole.DTOs;
using TestingConsole.Services;

namespace TestingConsole.Strategy
{
    internal class Strategy1
    {
        internal StrategyLifecycle strategyState;
        private TradingRepo repo;
        private DataService dataService;
        private List<Quote> quotes;
        private int tmFrame;
        private double maxriskAmt;
        private InstrumentDTO tradingInst;
        private float capital;
        private List<Quote> candles;
        private List<EmaResult> ema20;
        private List<EmaResult> ema50;
        private List<EmaResult> ema200;
        private List<RsiResult> rsi13;
        private Dictionary<int, bool> orderDict;
        private decimal sl;
        private bool isBuyOrder;
        private OrderDTO placedOrder;

        public Strategy1(TradingRepo repository, DataService dtaService)
        {
            repo = repository;
            dataService = dtaService;
        }

        public void OnRun(List<Quote> qts, int timeFrame, InstrumentDTO tdingInst, float cap = 100000)
        {
            quotes = qts;
            tmFrame = timeFrame;
            tradingInst = tdingInst;
            capital = cap;

            candles = qts.Aggregate<Quote>(TimeSpan.FromSeconds(timeFrame)).ToList();
            ema20 = candles.Use(CandlePart.HLC3).GetEma(8).ToList();
            ema50 = candles.Use(CandlePart.HLC3).GetEma(21).ToList();
            ema200 = candles.Use(CandlePart.HLC3).GetEma(55).ToList();
            rsi13 = candles.Use(CandlePart.HLC3).GetRsi(8).ToList();
            orderDict = new Dictionary<int, bool>();

            Quote quote = null;
            strategyState = StrategyLifecycle.OPEN;
            sl = 0;
            placedOrder = new OrderDTO();
            foreach (var qt in quotes)
            {
                quote = qt;
                if (quote != null)
                {
                    switch (strategyState)
                    {
                        case StrategyLifecycle.OPEN:
                            strategyState = OnStrategyOpen(quote, tradingInst);
                            break;
                        case StrategyLifecycle.OPEN_ORDER_PLACED:
                            strategyState = OnOpenOrderPlaced(quote, tradingInst);
                            break;
                        case StrategyLifecycle.POSITION_OPENED:
                            strategyState = OnPositionOpened(quote);
                            break;
                        case StrategyLifecycle.CLOSE_ORDER_PLACED:
                            strategyState = OnCloseOrderPlaced(quote);
                            break;
                        case StrategyLifecycle.POSITION_CLOSED:
                            strategyState = OnPositionClosed(quote);
                            break;
                        case StrategyLifecycle.CLOSED:
                            strategyState = OnStrategyClose(quote);
                            break;
                    }
                }
                else
                {
                    throw new Exception("Custom exception");
                }
            }
        }


        public StrategyLifecycle OnStrategyOpen(Quote quote, InstrumentDTO inst)
        {
            var lastQuote1 = Utility.GetPrevQuote(ref candles, quote.Date, -1, tmFrame);
            if(lastQuote1==null) 
                return strategyState;
            var temp = lastQuote1.Date.TimeOfDay;

            var lastQuote2 = Utility.GetPrevQuote(ref candles, quote.Date, -2, tmFrame);
            var lastQuote3 = Utility.GetPrevQuote(ref candles, quote.Date, -3, tmFrame);

            var ema20Value = ema20.FirstOrDefault(x => x.Date == lastQuote1.Date);
            var ema50Value = ema50.FirstOrDefault(x => x.Date == lastQuote1.Date);
            var ema200Value = ema200.FirstOrDefault(x => x.Date == lastQuote1.Date);
            var rsi13Value = rsi13.FirstOrDefault(x => x.Date == lastQuote1.Date);

            if (lastQuote1 == null || lastQuote2 == null || lastQuote3 == null || ema20Value.Ema == null || ema50Value.Ema == null || ema200Value.Ema == null)
                return strategyState;

            var startTime = new DateTime(quote.Date.Year, quote.Date.Month, quote.Date.Day, 9, 25, 0);
            var endTime = new DateTime(quote.Date.Year, quote.Date.Month, quote.Date.Day, 15, 0, 0);
            if (quote.Date >= startTime && quote.Date <= endTime)
            {
                if (ema20Value.Ema > ema50Value.Ema && ema50Value.Ema > ema200Value.Ema && (Convert.ToDouble(lastQuote1.Close) > ema50Value.Ema))
                {
                    if ((lastQuote1.Low < lastQuote2.Low && lastQuote2.Low < lastQuote3.Low) && Utility.IsHighBreak(quote.High, lastQuote1.High) /*&& rsi13Value.Rsi < 40*/)
                    {
                        //buy
                        sl = lastQuote1.Low;
                        isBuyOrder = true;
                        var quantity = Utility.GetQuantity(inst.LotSize, Convert.ToSingle(quote.Close - lastQuote1.Low), capital * 0.02f);
                        placedOrder = PlaceOrder(inst.Symbol, (MarketQuoteDTO)quote, quantity);
                        orderDict.Add(placedOrder.Id, true);
                        return StrategyLifecycle.OPEN_ORDER_PLACED;
                    }
                }
                else if (ema20Value.Ema < ema50Value.Ema && ema50Value.Ema < ema200Value.Ema && (Convert.ToDouble(lastQuote1.Close) < ema50Value.Ema))
                {
                    if ((lastQuote1.High > lastQuote2.High && lastQuote2.High > lastQuote3.High) && Utility.IsLowBreak(quote.Low, lastQuote1.Low) /*&& rsi13Value.Rsi > 60*/)
                    {
                        //sell
                        sl = lastQuote1.High;
                        isBuyOrder = false;
                        var quantity = Utility.GetQuantity(inst.LotSize, Convert.ToSingle(lastQuote1.High - quote.Close), capital * 0.02f);
                        placedOrder = PlaceOrder(inst.Symbol, (MarketQuoteDTO)quote, quantity * -1);
                        orderDict.Add(placedOrder.Id, true);
                        return StrategyLifecycle.OPEN_ORDER_PLACED;
                    }
                }
            }
            return strategyState;
        }

        public StrategyLifecycle OnCloseOrderPlaced(Quote marketQuote)
        {
            return StrategyLifecycle.CLOSED;
        }

        public StrategyLifecycle OnOpenOrderPlaced(Quote quote, InstrumentDTO inst)
        {
            var lastQuote1 = Utility.GetPrevQuote(ref candles, quote.Date, -1, tmFrame);

            var endime = new DateTime(quote.Date.Year, quote.Date.Month, quote.Date.Day, 15, 10, 0);
            if (quote.Date > endime)
            {
                ExitPosition(quote.Date,quote.Close);
                orderDict[placedOrder.Id] = false;
                return StrategyLifecycle.CLOSED;
            }
            else if (isBuyOrder)
            {
                //if ((quote.Low <= sl) || (quote.Low < lastQuote1.Low))
                if (
                    
                    (quote.Low < sl)
                    || ((quote.Close < (decimal)placedOrder.Price) && (quote.Date >= placedOrder.TimeStamp.AddSeconds(tmFrame*2)))
                    || ((quote.Close - Convert.ToDecimal(placedOrder.Price)) >= ((decimal)placedOrder.Price - sl) * 2m) /*|| (quote.Low < lastQuote1.Low)*/
                    
                    )
                {
                    //sell
                    ExitPosition(quote.Date,quote.Close);
                    orderDict[placedOrder.Id] = false;
                    return StrategyLifecycle.CLOSED;
                }
            }
            else
            {
                //if ((quote.High >= sl) || (quote.High > lastQuote1.High))
                if (
                    (quote.High > sl)
                    || ((quote.Close > (decimal)placedOrder.Price) && (quote.Date >= placedOrder.TimeStamp.AddSeconds(tmFrame*2)))
                    || ((Convert.ToDecimal(placedOrder.Price) - quote.Close) >= (sl - (decimal)placedOrder.Price) * 2m) /*|| (quote.High > lastQuote1.High)*/
                    )
                {
                    //buy
                    ExitPosition(quote.Date, quote.Close);
                    orderDict[placedOrder.Id] = false;
                    return StrategyLifecycle.CLOSED;
                }
            }
            return strategyState;
        }

        public StrategyLifecycle OnPositionClosed(Quote marketQuote)
        {
            return StrategyLifecycle.CLOSED;
        }

        public StrategyLifecycle OnPositionOpened(Quote marketQuote)
        {
            return StrategyLifecycle.CLOSED;
        }

        public StrategyLifecycle OnStrategyClose(Quote marketQuote)
        {
            return StrategyLifecycle.OPEN;
        }

        private OrderDTO PlaceOrder(string symbol, MarketQuoteDTO quote, int quantity)
        {
            OrderDTO order = new OrderDTO
            {
                Symbol = symbol,
                TimeStamp = quote.Date,
                Quantity = quantity,
                Price = Convert.ToSingle(quote.Close),
            };
            return dataService.SaveOrder(order);
        }

        private bool ExitPosition(DateTime date,decimal price)
        {
            bool result = false;
            if (placedOrder != null)
            {
                OrderDTO order = new OrderDTO();
                order.Symbol = placedOrder.Symbol;
                order.Id = 0;
                order.TimeStamp = date.Date;
                order.Quantity = placedOrder.Quantity * -1;
                order.Price = Convert.ToSingle(price);
                order.TradeId = placedOrder.Id.ToString();
                repo.SaveOrder(order);
                result = true;
            }
            return result;
        }
    }
}
