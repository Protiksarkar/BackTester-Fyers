/*
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
    FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FyersAPI
{
    public abstract class BinaryQuotes
    {
        
        public static short Reverse(short value)
        {
            return (short)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        public static uint Reverse(uint value)
        {
            return (value & 0x000000FFU) << 24 |
                (value & 0x0000FF00U) << 8 |
                (value & 0x00FF0000U) >> 8 |
                (value & 0xFF000000U) >> 24;
        }
        
        public static long Reverse(long value)
        {
            ulong ulng = (ulong)~value;

            uint left = (uint)((ulng & 0xFFFFFFFF00000000) >> 32);
            uint right = (uint)(ulng & 0xFFFFFFFF);

            left = BinaryQuotes.Reverse(left);
            right = BinaryQuotes.Reverse(right);

            ulng = (ulong)right << 32 | left;

            return (long)~ulng;
        }
        

        public static bool TryParse(byte[] data, out BinaryQuotes[] quotes)
        {

            List<BinaryQuotes> tmp = new List<BinaryQuotes>();


            //short noOfPackets = BitConverter.ToInt16(data, 0);
            int position = 0;
            int len = data.Length;

            while (len > position)
            {

                if (len < position + 24)   //header == 24 bytes
                    break;

                Header header = new Header();

                                
                header.FyersToken = Reverse(BitConverter.ToInt64(data, position));
                position += 8;

                uint timeStamp = Reverse(BitConverter.ToUInt32(data, position));
                header.TimeStamp = Globals.FromUnixTime(timeStamp);
                position += 4;

                header.TransactionCode = Reverse(BitConverter.ToInt16(data, position));
                position += 2;

                header.TradeStatus = Reverse(BitConverter.ToInt16(data, position));
                position += 2;

                header.PacketLength = Reverse(BitConverter.ToInt16(data, position));
                if (header.PacketLength <= 0) //something bad happened
                    break;

                position += 2;

                header.HasLevelII = BitConverter.ToBoolean(data, position);
                position += 6; //5 bytes is reserved

                
                BinaryQuotes quote = null;

                if (header.PacketLength == 200)
                {

                    if (position + header.PacketLength > len)
                        break;

                    quote = new Level2Snapshot(header);
                    quote.Process(data, ref position);
                }
                else if (header.PacketLength == 104)
                {
                    //header.PacketLength includes header length
                     
                    if (position + header.PacketLength - 24 + 8 > len) //8 == bid/ask price bytes not included in header.PacketLength
                        break;

                    quote = new Level1Snapshot(header, true);
                    quote.Process(data, ref position);
                }
                else if (header.PacketLength == 48)
                {
                    if (position + header.PacketLength > len)
                        break;

                    quote = new IndexSnapshot(header);
                    quote.Process(data, ref position);
                }
                else
                {
                    break; //unknown format
                }

                if (quote != null)
                {
                    tmp.Add(quote);
                }

            }

            quotes = tmp.ToArray();

            return true;
        }


        public BinaryQuotes(Header header)
        {
            Header = header;
        }

        protected abstract void Process(byte[] data, ref int position);

        public abstract SnapshotType SnapshotType { get; }
        public Header Header { get; }

    }



    public class Header
    {
        /// <summary>
        /// Unique <a href="https://api-docs.fyers.in/v2/#fytoken">fyers token</a> for every symbol.
        /// </summary>
        public long FyersToken { get; set; }
        /// <summary>
        /// Timestamp of header
        /// </summary>
        public DateTime TimeStamp { get; set; }
        /// <summary>
        /// Unique codes assigned to the type of data received
        /// 7208 and 7207 - Market data for NSE and BSE (7207 is specifically for               indices)
        /// 31038 -  Market data for MCX
        /// 7202 - OI data (This will only be there for NSE FO, NSE CD and MCX)
        /// </summary>
        public short TransactionCode { get; set; }
        /// <summary>
        /// Current status of market (Whether market is open or closed)
        /// </summary>
        public short TradeStatus { get; set; }
        /// <summary>
        /// Length of current packet received
        /// </summary>
        public short PacketLength { get; set; }
        public bool HasLevelII { get; set; }
        
        public override string ToString()
        {
            return $"Token={FyersToken}, Time={TimeStamp:yyMMdd HH:mm:ss}, Code={TransactionCode}, Status={TradeStatus}, PacketLen={PacketLength}, HasL2={HasLevelII}";
        }

    }
    
    public enum SnapshotType
    {
        Index,
        Level1,
        Level2
    }
    
    [DataContract]
    public class IndexSnapshot : BinaryQuotes
    {   
        public IndexSnapshot(Header header) : base(header)
        {
            
        }

        protected override void Process(byte[] data, ref int position)
        {
            //start of index 7207/7208 - Index and stock data
            this.Multiplier = (int)Reverse(BitConverter.ToUInt32(data, position));
            position += 4;

            this.LastTradedPrice = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
            position += 4;

            this.Open = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
            position += 4;

            this.High = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
            position += 4;

            this.Low = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
            position += 4;

            this.Close = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
            position += 4;

            this.MinuteOpen = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
            position += 4;

            this.MinuteHigh = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
            position += 4;

            this.MinuteLow = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
            position += 4;

            this.MinuteClose = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
            position += 4;

            this.MinuteVolume = Reverse(BitConverter.ToInt64(data, position));
            position += 8;
        }

        public override SnapshotType SnapshotType => SnapshotType.Index;

        public int Multiplier { get; set; }
        public double LastTradedPrice { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double MinuteOpen { get; set; }
        public double MinuteHigh { get; set; }
        public double MinuteLow { get; set; }
        public double MinuteClose { get; set; }
        public long MinuteVolume { get; set; }

        public override string ToString()
        {
            return $"{this.Header}, Multiplier={this.Multiplier}, LTP={this.LastTradedPrice}, O={this.Open}, H={this.High}, L={this.Low}, C={this.Close}";
        }

    }

    public class Level1Snapshot : IndexSnapshot
    {
        private readonly bool isL1Data;

        public Level1Snapshot(Header header, bool isL1Data) : base(header)
        {
            this.isL1Data = isL1Data;
        }

        protected override void Process(byte[] data, ref int position)
        {
            base.Process(data, ref position);

            this.LastTradedQuantity = (int)Reverse(BitConverter.ToUInt32(data, position));
            position += 4;

            int lastTradedTime = (int)Reverse(BitConverter.ToUInt32(data, position));
            this.LastTradedTime = Globals.FromUnixTime(lastTradedTime);
            position += 4;

            this.AverageTradedPrice = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
            position += 4;

            this.TotalVolume = (int)Reverse(BitConverter.ToUInt32(data, position));
            position += 4;

            this.TotalBuyQuantity = Reverse(BitConverter.ToInt64(data, position));
            position += 8;
            
            this.TotalSellQuantity = Reverse(BitConverter.ToInt64(data, position));
            position += 8;

            if (this.isL1Data)
            {
                this.BidPrice = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
                position += 4;

                this.AskPrice = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
                position += 4;
            }
        }

        public override SnapshotType SnapshotType => SnapshotType.Level1;

        public int LastTradedQuantity { get; set; }
        public DateTime LastTradedTime { get; set; }
        public double AverageTradedPrice { get; set; }
        public int TotalVolume { get; set; }
        public long TotalBuyQuantity { get; set; }
        public long TotalSellQuantity { get; set; }

        public virtual double BidPrice { get; set; }
        public virtual double AskPrice { get; set; }


        public override string ToString()
        {
            return $"{base.ToString()}, TLQ={LastTradedQuantity}, LTT={LastTradedTime:yyMMdd HH:mm:ss}, ATP={AverageTradedPrice}, Vol={TotalVolume}, TBQ={TotalBuyQuantity}, TSQ={TotalSellQuantity}, Bid={this.BidPrice}, Ask={this.AskPrice}";
        }

    }
    
    public class Level2Snapshot : Level1Snapshot
    {
        public Level2Snapshot(Header header) : base(header, false)
        {
            this.Bids = new MarketDepthItem[5];
            this.Asks = new MarketDepthItem[5];
        }

        protected override void Process(byte[] data, ref int position)
        {
            base.Process(data, ref position);


            for (int i = 0; i < 5; i++)
            {
                this.Bids[i] = new MarketDepthItem();
                this.Asks[i] = new MarketDepthItem();

                this.Bids[i].price = Reverse(BitConverter.ToUInt32(data, position)) / (double)this.Multiplier;
                this.Asks[i].price = Reverse(BitConverter.ToUInt32(data, position + 60)) / (double)this.Multiplier;
                position += 4;

                this.Bids[i].volume = (int)Reverse(BitConverter.ToUInt32(data, position));
                this.Asks[i].volume = (int)Reverse(BitConverter.ToUInt32(data, position + 60));
                position += 4;

                this.Bids[i].ord = (int)Reverse(BitConverter.ToUInt32(data, position));
                this.Asks[i].ord = (int)Reverse(BitConverter.ToUInt32(data, position + 60));
                position += 4;
            }

            position += 60;
        }

        public override SnapshotType SnapshotType => SnapshotType.Level2;

        //The API does not provides the Bid price or Ask price for L1 data. Given we are inheriting L2 from L1 we will return the value from the market depth
        public override double BidPrice
        {
            get
            {
                if (this.Bids == null || this.Bids.Length == 0 || this.Bids[0] == null)
                    return 0.0d;

                return this.Bids[0].price;
            }
            set { }
        }

        public override double AskPrice
        {
            get
            {
                if (this.Asks == null || this.Asks.Length == 0 || this.Asks[0] == null)
                    return 0.0d;

                return this.Asks[0].price;
            }
            set { }
        }

        public MarketDepthItem[] Bids { get; private set; }
        public MarketDepthItem[] Asks { get; private set; }

        public override string ToString()
        {
            return $"{base.ToString()}, Bids={string.Join<MarketDepthItem>(",", Bids)}, Asks={string.Join<MarketDepthItem>(",", Asks)}";
        }
    }


}
