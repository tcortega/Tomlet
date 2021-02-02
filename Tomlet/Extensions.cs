﻿using System;
using System.IO;
using System.Text;

namespace Tomlet
{
    internal static class Extensions
    {
        internal static bool IsWhitespace(this int val)
        {
            if (val == '\n' || val == '\r')
                return false; //TOML defines these as non-whitespace
            
            return char.IsWhiteSpace((char) val);
        }

        internal static bool IsEquals(this int val)
        {
            return val == '=';
        }

        internal static bool IsSingleQuote(this int val)
        {
            return val == '\'';
        }

        internal static bool IsDoubleQuote(this int val)
        {
            return val == '"';
        }

        internal static bool IsHashSign(this int val)
        {
            return val == '#';
        }

        internal static bool IsNewline(this int val)
        {
            return val == '\r' || val == '\n';
        }

        internal static bool IsDigit(this int val)
        {
            return char.IsDigit((char) val);
        }

        internal static bool IsHexDigit(this int val)
        {
            if (val.IsDigit())
                return true;

            var upper = char.ToUpperInvariant((char) val);
            return upper >= 'A' && upper <= 'F';
        }
        
        internal static bool IsHexDigit(this char c)
        {
            var val = (int) c;
            
            if (val.IsDigit())
                return true;

            var upper = char.ToUpperInvariant((char) val);
            return upper >= 'A' && upper <= 'F';
        }

        internal static bool TryPeek(this TextReader reader, out int nextChar)
        {
            nextChar = reader.Peek();
            return nextChar != -1;
        }
        
        internal static void SkipWhitespace(this TextReader reader)
        {
            while (reader.TryPeek(out var nextChar) && nextChar.IsWhitespace())
                reader.Read(); //Consume this char.
        }

        internal static void SkipPotentialCR(this TextReader reader)
        {
            if (reader.TryPeek(out var nextChar) && nextChar == '\r')
                reader.Read();
        }

        internal static void SkipAnyComment(this TextReader reader)
        {
            //Skip anything up until the \r or \n if we start with a hash.
            if (reader.TryPeek(out var maybeHash) && maybeHash.IsHashSign())
                reader.ReadWhile(commentChar => !commentChar.IsNewline());
        }

        internal static void SkipAnyNewlineOrWhitespace(this TextReader reader)
        {
            reader.ReadWhile(c => c.IsNewline() || c.IsWhitespace());
        }

        internal static char[] ReadChars(this TextReader reader, int count)
        {
            char[] result = new char[count];
            reader.ReadBlock(result, 0, 4);

            return result;
        }

        internal static string ReadWhile(this TextReader reader, Predicate<int> predicate)
        {
            var ret = new StringBuilder();
            //Read up until whitespace or an equals
            while (reader.TryPeek(out var nextChar) && predicate(nextChar))
            {
                ret.Append((char) reader.Read());
            }

            return ret.ToString();
        }

        internal static bool ExpectAndConsume(this TextReader reader, char expectWhat)
        {
            if (!reader.TryPeek(out var nextChar))
                return false;

            if (nextChar == expectWhat)
            {
                reader.Read();
                return true;
            }

            return false;
        }
    }
}