using System;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    public class DataRow : IDataRow, IEquatable<DataRow>
    {
        public bool Equals(DataRow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key.Equals(other.Key) && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataRow) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Key.GetHashCode()*397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DataRow left, DataRow right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataRow left, DataRow right)
        {
            return !Equals(left, right);
        }

        public DateTime Key { get; set; }
        public object Value { get; set; }
    }
}