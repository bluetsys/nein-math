﻿namespace NeinMath
{
    /// <remarks>
    /// Integer arrays may be null or bigger than necessary, appropriate length
    /// parameters must be used. The left parameter, if any, is always assumed
    /// as the more lengthy one. No checks, beware!
    /// </remarks>
    internal static class Calc
    {
        public static uint[] And(uint[] left, int leftLength,
                                 uint right, uint rightPad)
        {
            if (leftLength == 0)
                return new uint[] { };

            var bits = new uint[leftLength];
            bits[0] = left[0] & right;
            for (var i = 1; i < leftLength; i++)
                bits[i] = left[i] & rightPad;

            return bits;
        }

        public static uint[] And(uint[] left, int leftLength,
                                 uint[] right, int rightLength,
                                 uint rightPad)
        {
            var bits = new uint[leftLength];
            for (var i = 0; i < rightLength; i++)
                bits[i] = left[i] & right[i];
            for (var i = rightLength; i < leftLength; i++)
                bits[i] = left[i] & rightPad;

            return bits;
        }

        public static uint[] Or(uint[] left, int leftLength,
                                uint right, uint rightPad)
        {
            if (leftLength == 0)
                return new uint[] { right };

            var bits = new uint[leftLength];
            bits[0] = left[0] | right;
            for (var i = 1; i < leftLength; i++)
                bits[i] = left[i] | rightPad;

            return bits;
        }

        public static uint[] Or(uint[] left, int leftLength,
                                uint[] right, int rightLength,
                                uint rightPad)
        {
            var bits = new uint[leftLength];
            for (var i = 0; i < rightLength; i++)
                bits[i] = left[i] | right[i];
            for (var i = rightLength; i < leftLength; i++)
                bits[i] = left[i] | rightPad;

            return bits;
        }

        public static uint[] Xor(uint[] left, int leftLength,
                                 uint right, uint rightPad)
        {
            if (leftLength == 0)
                return new uint[] { right };

            var bits = new uint[leftLength];
            bits[0] = left[0] ^ right;
            for (var i = 1; i < leftLength; i++)
                bits[i] = left[i] ^ rightPad;

            return bits;
        }

        public static uint[] Xor(uint[] left, int leftLength,
                                 uint[] right, int rightLength,
                                 uint rightPad)
        {
            var bits = new uint[leftLength];
            for (var i = 0; i < rightLength; i++)
                bits[i] = left[i] ^ right[i];
            for (var i = rightLength; i < leftLength; i++)
                bits[i] = left[i] ^ rightPad;

            return bits;
        }

        public static uint[] Shift(uint[] value, int length,
                                   int shift, uint pad)
        {
            if (length == 0)
                return new uint[] { pad };

            if (shift < 0)
            {
                // big shifts move entire blocks
                var leapShift = -shift / 32;
                if (length <= leapShift)
                    return new uint[] { pad };
                var tinyShift = -shift % 32;

                // shifts the bits to the right
                var bits = new uint[length - leapShift];
                if (tinyShift == 0)
                {
                    for (var i = 0; i < bits.Length; i++)
                        bits[i] = value[i + leapShift];
                }
                else
                {
                    for (var i = 0; i < bits.Length - 1; i++)
                        bits[i] = (value[i + leapShift] >> tinyShift)
                            | (value[i + leapShift + 1] << (32 - tinyShift));
                    bits[bits.Length - 1] = (pad << (32 - tinyShift))
                        | (value[length - 1] >> tinyShift);
                }

                return bits;
            }
            else if (shift > 0)
            {
                // big shifts move entire blocks
                var leapShift = shift / 32;
                var tinyShift = shift % 32;

                // shifts the bits to the left
                var bits = new uint[length + leapShift + 1];
                if (tinyShift == 0)
                {
                    for (var i = leapShift; i < bits.Length - 1; i++)
                        bits[i] = value[i - leapShift];
                    bits[bits.Length - 1] = pad;
                }
                else
                {
                    for (var i = leapShift + 1; i < bits.Length - 1; i++)
                    {
                        bits[i] = (value[i - leapShift] << tinyShift)
                            | (value[i - leapShift - 1] >> (32 - tinyShift));
                    }
                    bits[leapShift] = value[0] << tinyShift;
                    bits[bits.Length - 1] = (pad << tinyShift)
                        | (value[length - 1] >> (32 - tinyShift));
                }

                return bits;
            }
            else
            {
                // no shift at all...
                var bits = new uint[length];
                for (var i = 0; i < length; i++)
                    bits[i] = value[i];

                return bits;
            }
        }

        public static uint[] Add(uint[] left, int leftLength,
                                 uint right)
        {
            if (leftLength == 0)
                return new uint[] { right };

            var bits = new uint[leftLength + 1];

            // first operation
            var digit = (long)left[0] + right;
            bits[0] = (uint)digit;
            var carry = digit >> 32;

            // adds the bits
            for (var i = 1; i < leftLength; i++)
            {
                digit = left[i] + carry;
                bits[i] = (uint)digit;
                carry = digit >> 32;
            }
            bits[bits.Length - 1] = (uint)carry;

            return bits;
        }

        public static uint[] Add(uint[] left, int leftLength,
                                 uint[] right, int rightLength)
        {
            var bits = new uint[leftLength + 1];
            var carry = 0L;

            // adds the bits
            for (var i = 0; i < rightLength; i++)
            {
                var digit = (left[i] + carry) + right[i];
                bits[i] = (uint)digit;
                carry = digit >> 32;
            }
            for (var i = rightLength; i < leftLength; i++)
            {
                var digit = left[i] + carry;
                bits[i] = (uint)digit;
                carry = digit >> 32;
            }
            bits[bits.Length - 1] = (uint)carry;

            return bits;
        }

        public static uint[] Add(uint[] value,
                                 int leftLength, int leftOffset,
                                 int rightLength, int rightOffset)
        {
            var bits = new uint[leftLength + 1];
            var carry = 0L;

            // adds the bits
            for (var i = 0; i < rightLength; i++)
            {
                var digit = (value[i + leftOffset] + carry)
                    + value[i + rightOffset];
                bits[i] = (uint)digit;
                carry = digit >> 32;
            }
            for (var i = rightLength; i < leftLength; i++)
            {
                var digit = value[i + leftOffset] + carry;
                bits[i] = (uint)digit;
                carry = digit >> 32;
            }
            bits[bits.Length - 1] = (uint)carry;

            return bits;
        }

        public static void AddSelf(uint[] left, int leftLength,
                                   uint[] right, int rightLength,
                                   int rightOffset)
        {
            var fullRightLength = rightOffset + rightLength;
            if (fullRightLength > leftLength)
                fullRightLength = leftLength;
            var carry = 0L;

            // adds the bits
            for (var i = rightOffset; i < fullRightLength; i++)
            {
                var digit = (left[i] + carry) + right[i - rightOffset];
                left[i] = (uint)digit;
                carry = digit >> 32;
            }
            if (fullRightLength < leftLength)
            {
                var digit = left[fullRightLength] + carry;
                left[fullRightLength] = (uint)digit;
            }
        }

        public static uint[] Subtract(uint[] left, int leftLength,
                                      uint right)
        {
            if (leftLength == 0)
                return new uint[] { };

            var bits = new uint[leftLength];

            // first operation
            var digit = (long)left[0] - right;
            bits[0] = (uint)digit;
            var carry = digit >> 63;

            // substracts the bits
            for (var i = 1; i < leftLength; i++)
            {
                digit = left[i] + carry;
                bits[i] = (uint)digit;
                carry = digit >> 63;
            }

            return bits;
        }

        public static uint[] Subtract(uint[] left, int leftLength,
                                      uint[] right, int rightLength)
        {
            var bits = new uint[leftLength];
            var carry = 0L;

            // substracts the bits
            for (var i = 0; i < rightLength; i++)
            {
                var digit = (left[i] + carry) - right[i];
                bits[i] = (uint)digit;
                carry = digit >> 63;
            }
            for (var i = rightLength; i < leftLength; i++)
            {
                var digit = left[i] + carry;
                bits[i] = (uint)digit;
                carry = digit >> 63;
            }

            return bits;
        }

        public static void SubtractSelf(uint[] left, int leftLength,
                                        uint[] right, int rightLength)
        {
            var carry = 0L;

            // substract the bits
            for (var i = 0; i < rightLength; i++)
            {
                var digit = (left[i] + carry) - right[i];
                left[i] = (uint)digit;
                carry = digit >> 63;
            }
            for (var i = rightLength; i < leftLength; i++)
            {
                var digit = left[i] + carry;
                left[i] = (uint)digit;
                carry = digit >> 63;
            }
        }

        public static void SubtractSelf(uint[] left, int leftLength,
                                        uint[] right, int rightLength,
                                        int rightOffset)
        {
            var carry = 0L;

            // substract the bits
            for (var i = rightOffset; i < rightLength + rightOffset; i++)
            {
                var digit = (left[i] + carry) - right[i - rightOffset];
                left[i] = (uint)digit;
                carry = digit >> 63;
            }
            for (var i = rightLength + rightOffset; i < leftLength; i++)
            {
                var digit = left[i] + carry;
                left[i] = (uint)digit;
                carry = digit >> 63;
            }
        }

        public static uint[] Square(uint[] value, int valueLength)
        {
            return Square(value, valueLength, 0);
        }

#if DEBUG

        public const int SquareThreshold = 128 / 32;

#else

        public const int SquareThreshold = 4096 / 32;

#endif

        public static uint[] Square(uint[] value, int valueLength,
                                    int valueOffset)
        {
            var bits = new uint[valueLength * 2];

            if (valueLength < SquareThreshold)
            {
                // squares the bits
                for (var i = 0; i < valueLength; i++)
                {
                    var carry = 0UL;
                    for (var j = 0; j < i; j++)
                    {
                        var digit1 = bits[i + j] + carry;
                        var digit2 = (ulong)value[j + valueOffset]
                            * (ulong)value[i + valueOffset];
                        bits[i + j] = (uint)(digit1 + (digit2 << 1));
                        carry = (digit2 + (digit1 >> 1)) >> 31;
                    }
                    var digit = (ulong)value[i + valueOffset]
                        * (ulong)value[i + valueOffset] + carry;
                    bits[i * 2] = (uint)digit;
                    bits[i * 2 + 1] = (uint)(digit >> 32);
                }
            }
            else
            {
                // divide & conquer
                var n = (valueLength + 1) / 2;

                var lowOffset = valueOffset;
                var highOffset = valueOffset + n;

                var lowLength = n; // n < valueLength (!)
                var highLength = valueLength - lowLength;

                var p1 = Square(value, highLength, highOffset);
                var p2 = Square(value, lowLength, lowOffset);
                var p3 = Square(Add(value, lowLength, lowOffset,
                    highLength, highOffset), lowLength + 1, 0);

                SubtractSelf(p3, p3.Length, p1, p1.Length);
                SubtractSelf(p3, p3.Length, p2, p2.Length);

                // merge the result
                AddSelf(bits, bits.Length, p2, p2.Length, 0);
                AddSelf(bits, bits.Length, p3, p3.Length, n);
                AddSelf(bits, bits.Length, p1, p1.Length, n * 2);
            }

            return bits;
        }

        public static uint[] Multiply(uint[] left, int leftLength,
                                      uint right)
        {
            var bits = new uint[leftLength + 1];

            // multiplies the bits
            var carry = 0UL;
            for (var j = 0; j < leftLength; j++)
            {
                var digits = (ulong)left[j] * right + carry;
                bits[j] = (uint)digits;
                carry = digits >> 32;
            }
            bits[leftLength] = (uint)carry;

            return bits;
        }

        public static uint[] Multiply(uint[] left, int leftLength,
                                      uint[] right, int rightLength)
        {
            return Multiply(left, leftLength, 0, right, rightLength, 0);
        }

#if DEBUG

        public const int MultiplyThreshold = 128 / 32;

#else

        public const int MultiplyThreshold = 2048 / 32;

#endif

        public static uint[] Multiply(uint[] left, int leftLength,
                                      int leftOffset,
                                      uint[] right, int rightLength,
                                      int rightOffset)
        {
            var bits = new uint[leftLength + rightLength];

            if (leftLength < MultiplyThreshold
                || rightLength < MultiplyThreshold)
            {
                // multiplies the bits
                for (var i = 0; i < rightLength; i++)
                {
                    var carry = 0UL;
                    for (var j = 0; j < leftLength; j++)
                    {
                        var digits = bits[i + j] + carry
                            + (ulong)left[j + leftOffset]
                            * (ulong)right[i + rightOffset];
                        bits[i + j] = (uint)digits;
                        carry = digits >> 32;
                    }
                    bits[i + leftLength] = (uint)carry;
                }
            }
            else
            {
                // divide & conquer
                var n = (((leftLength > rightLength)
                    ? leftLength : rightLength) + 1) / 2;

                var leftLowOffset = leftOffset;
                var leftHighOffset = leftOffset + n;
                var rightLowOffset = rightOffset;
                var rightHighOffset = rightOffset + n;

                var leftLowLength = (n < leftLength) ? n : leftLength;
                var leftHighLength = leftLength - leftLowLength;
                var rightLowLength = (n < rightLength) ? n : rightLength;
                var rightHighLength = rightLength - rightLowLength;

                var p1 = Multiply(left, leftHighLength, leftHighOffset,
                                  right, rightHighLength, rightHighOffset);
                var p2 = Multiply(left, leftLowLength, leftLowOffset,
                                  right, rightLowLength, rightLowOffset);
                var p3 = Multiply(Add(left, leftLowLength, leftLowOffset,
                    leftHighLength, leftHighOffset), leftLowLength + 1, 0,
                    Add(right, rightLowLength, rightLowOffset,
                    rightHighLength, rightHighOffset), rightLowLength + 1, 0);

                SubtractSelf(p3, p3.Length, p1, p1.Length);
                SubtractSelf(p3, p3.Length, p2, p2.Length);

                // merge the result
                AddSelf(bits, bits.Length, p2, p2.Length, 0);
                AddSelf(bits, bits.Length, p3, p3.Length, n);
                AddSelf(bits, bits.Length, p1, p1.Length, n * 2);
            }

            return bits;
        }

        public static uint[] Divide(uint[] left, int leftLength,
                                    uint right, out uint remainder)
        {
            var bits = new uint[leftLength];

            // divides the bits
            var carry = 0UL;
            for (var i = leftLength - 1; i >= 0; i--)
            {
                var value = (carry << 32) | left[i];
                bits[i] = (uint)(value / right);
                carry = value % right;
            }
            remainder = (uint)carry;

            return bits;
        }

        public static uint Remainder(uint[] left, int leftLength,
                                     uint right)
        {
            // divides the bits
            var carry = 0UL;
            for (var i = leftLength - 1; i >= 0; i--)
            {
                var value = (carry << 32) | left[i];
                carry = value % right;
            }

            return (uint)carry;
        }

        public static uint[] Divide(uint[] left, int leftLength,
                                    uint[] right, int rightLength,
                                    out uint[] remainder)
        {
            var bits = new uint[leftLength - rightLength + 1];

            // get more bits into the highest bit block
            var shifted = Bits.LeadingZeros(right[rightLength - 1]);
            left = Shift(left, leftLength, shifted, 0);
            right = Shift(right, rightLength, shifted, 0);

            // measure again (after shift...)
            leftLength = left[left.Length - 1] == 0
                ? left.Length - 1 : left.Length;

            // these values are useful
            var guess = new uint[rightLength + 1];
            var guessLength = 0;
            var delta = 0;

            // sub the divisor
            do
            {
                delta = Bits.Compare(left, leftLength,
                    right, rightLength, leftLength - rightLength);
                if (delta >= 0)
                {
                    ++bits[leftLength - rightLength];
                    SubtractSelf(left, leftLength,
                        right, rightLength, leftLength - rightLength);
                    leftLength = Bits.Length(left, leftLength);
                }
            }
            while (leftLength >= rightLength && delta > 0);

            // divides the rest of the bits
            var i = leftLength - 1;
            while (i >= rightLength)
            {
                // first guess for the current bit of the quotient
                var digits = (left[i - 1] | ((ulong)left[i] << 32))
                    / right[rightLength - 1];
                if ((digits & 0x100000000) != 0)
                    digits = 0x100000000;

                // the guess may be a little bit to big
                do
                {
                    MultiplyDivisor(right, rightLength, digits, guess);
                    guessLength = guess[guess.Length - 1] == 0
                        ? guess.Length - 1 : guess.Length;
                    delta = Bits.Compare(left, leftLength,
                        guess, guessLength, i - rightLength);
                    if (delta < 0)
                        --digits;
                }
                while (delta < 0);

                // we have the bit!
                SubtractSelf(left, leftLength,
                    guess, guessLength, i - rightLength);
                leftLength = Bits.Length(left, leftLength);
                bits[i - rightLength] = (uint)digits;
                if (digits == 0x100000000)
                    ++bits[i - rightLength + 1];
                i = leftLength - 1;
            }

            // repair the cheated shift
            remainder = Shift(left, leftLength, -1 * shifted, 0);

            return bits;
        }

        private static void MultiplyDivisor(uint[] left, int leftLength,
                                            ulong right, uint[] bits)
        {
            // multiplies the bits
            var carry = 0UL;
            for (var j = 0; j < leftLength; j++)
            {
                var digits = left[j] * right + carry;
                bits[j] = (uint)digits;
                carry = digits >> 32;
            }
            bits[leftLength] = (uint)carry;
        }
    }
}