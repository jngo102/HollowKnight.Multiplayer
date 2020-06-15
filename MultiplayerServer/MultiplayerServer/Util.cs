using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerServer
{
    // Shamelessly pasted from https://stackoverflow.com/questions/1440392/use-byte-as-key-in-dictionary/30353296
    public class ByteArrayComparer : EqualityComparer<byte[]>
    {
        public override bool Equals(byte[] first, byte[] second)
        {
            if (first == null || second == null)
            {
                // null == null returns true.
                // non-null == null returns false.
                return first == second;
            }
            if (ReferenceEquals(first, second))
            {
                return true;
            }
            if (first.Length != second.Length)
            {
                return false;
            }
            // Linq extension method is based on IEnumerable, must evaluate every item.
            return first.SequenceEqual(second);
        }

        // This implementation works great if you assume the byte[] arrays
        // are themselves cryptographic hashes. It probably works alright too,
        // for general-purpose byte arrays.
        public override int GetHashCode(byte[] obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (obj.Length >= 4)
            {
                return BitConverter.ToInt32(obj, 0);
            }
            // Length occupies at most 2 bits. Might as well store them in the high order byte
            int value = obj.Length;
            foreach (var b in obj)
            {
                value <<= 8;
                value += b;
            }
            return value;
        }
    }
}
