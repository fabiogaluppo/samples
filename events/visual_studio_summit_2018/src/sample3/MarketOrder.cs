//Sample provided by Fabio Galuppo  
//May 2018

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;

//Inspired by https://www.pubnub.com/developers/realtime-data-streams/financial-securities-market-orders/
sealed class MarketOrder
{
    public enum TypeOfTrade : byte
    {
        Day = 0x1,
        Limit = 0x2,
        Stop = 0x3,
        Market = 0x4,
        FillOrKill = 0x5
    }

    public readonly String Symbol;
    public readonly double BidPrice;
    public readonly int Quantity;
    public readonly DateTime Timestamp;
    public readonly TypeOfTrade TradeType;

    public const int SYMBOL_MAX_SIZE = 10;
    private static readonly String TIMESTAMP_MASK = "yyyyMMddHHmmssffff";

    public MarketOrder(String symbol, double bidPrice, int quantity, TypeOfTrade tradeType, DateTime timestamp)
    {
        Symbol = symbol.Length <= SYMBOL_MAX_SIZE ? symbol : symbol.Substring(0, SYMBOL_MAX_SIZE);
        BidPrice = bidPrice;
        Quantity = quantity;
        Timestamp = timestamp;
        TradeType = tradeType;
    }

    public override bool Equals(object obj)
    {
        MarketOrder that = obj as MarketOrder;
        if (null != obj)
        {
            if (Object.ReferenceEquals(this, that))
                return true;
            return this.Symbol.Equals(that.Symbol) &&
                   this.BidPrice == that.BidPrice &&
                   this.Quantity == that.Quantity &&
                   this.Timestamp.Equals(that.Timestamp) &&
                   this.TradeType == that.TradeType;
        }
        return false;
    }

    public override int GetHashCode()
    {
        const int PRIME = 31;
        int a = Symbol.GetHashCode(), b = BidPrice.GetHashCode(),
            c = Quantity.GetHashCode(), d = Timestamp.GetHashCode(),
            e = TradeType.GetHashCode();
        return (((a * PRIME + b) * PRIME + c) * PRIME + d) * PRIME + e;
    }

    public override string ToString()
    {
        return String.Format("Symbol:{0} BidPrice:{1:F4} Quantity:{2} Timestamp:{3} TradeType:{4}", 
            Symbol, BidPrice, Quantity, Timestamp.ToString(TIMESTAMP_MASK), TradeType);
    }

    [ThreadStatic]
    private static MemoryStream Writer_;

    public static byte[] ToBytes(MarketOrder order)
    {
        if (null == Writer_) Writer_ = new MemoryStream(42);
        var ms = Writer_;
        byte[] buffer =  ms.Append(Encoding.ASCII.GetBytes(order.Symbol.PadRight(SYMBOL_MAX_SIZE, ' '))) //10 bytes
                            .Append(Encoding.ASCII.GetBytes(" ")) //1 byte
                            .Append(BitConverter.GetBytes(order.BidPrice)) //8 bytes
                            .Append(BitConverter.GetBytes(order.Quantity)) //4 bytes
                            .Append((byte)order.TradeType) //1 byte
                            .Append(Encoding.ASCII.GetBytes(order.Timestamp.ToString(TIMESTAMP_MASK, CultureInfo.CurrentCulture))) //18 bytes
                            .ToArray();
        ms.Position = 0;
        return buffer;
    }

    public static MarketOrder FromBytes(byte[] bytes)
    {
        var symbol = Encoding.ASCII.GetString(bytes, 0, 11).TrimEnd();
        var bidPrice = BitConverter.ToDouble(bytes, 11);
        var quantity = BitConverter.ToInt32(bytes, 19);
        var tradeType = (TypeOfTrade)bytes[23];
        var timestamp = DateTime.ParseExact(Encoding.ASCII.GetString(bytes, 24, 18), TIMESTAMP_MASK, CultureInfo.CurrentCulture);
        return new MarketOrder(symbol, bidPrice, quantity, tradeType, timestamp);
    }
}