// Copyright (c) 2022 Kaplas
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace TF3.Core.Helpers
{
    using System.Text;

    /// <summary>
    /// Converter between FullWidth and HalfWidth characters.
    /// https://source.winehq.org/source/dlls/kernel32/locale.c.
    /// </summary>
    public static class MapStringLib
    {
        private static readonly byte[] MiscSymbolsTable =
        {
            0xe0, 0xe1, 0x00, 0xe5, 0xe4, 0x00, 0x00, /* U+00A2- */
            0x00, 0x00, 0x00, 0xe2, 0x00, 0x00, 0xe3, /* U+00A9- */
        };

        private static readonly byte[] HangulMappingTable =
        {
            0x64, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,  /* U+FFA0- */
            0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f,  /* U+FFA8- */
            0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47,  /* U+FFB0- */
            0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x00,  /* U+FFB8- */
            0x00, 0x00, 0x4f, 0x50, 0x51, 0x52, 0x53, 0x54,  /* U+FFC0- */
            0x00, 0x00, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a,  /* U+FFC8- */
            0x00, 0x00, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f, 0x60,  /* U+FFD0- */
            0x00, 0x00, 0x61, 0x62, 0x63,                    /* U+FFD8- */
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1120:Comments should contain text", Justification = "Code alignment.")]
        private static readonly byte[] ComposeKatakanaMap =
        {
            /* */ 0x02, 0x0c, 0x0d, 0x01, 0xfb, 0xf2, 0xa1, /* U+FF61- */
            0xa3, 0xa5, 0xa7, 0xa9, 0xe3, 0xe5, 0xe7, 0xc3, /* U+FF68- */
            0xfc, 0xa2, 0xa4, 0xa6, 0xa8, 0xaa, 0xab, 0xad, /* U+FF70- */
            0xaf, 0xb1, 0xb3, 0xb5, 0xb7, 0xb9, 0xbb, 0xbd, /* U+FF78- */
            0xbf, 0xc1, 0xc4, 0xc6, 0xc8, 0xca, 0xcb, 0xcc, /* U+FF80- */
            0xcd, 0xce, 0xcf, 0xd2, 0xd5, 0xd8, 0xdb, 0xde, /* U+FF88- */
            0xdf, 0xe0, 0xe1, 0xe2, 0xe4, 0xe6, 0xe8, 0xe9, /* U+FF90- */
            0xea, 0xeb, 0xec, 0xed, 0xef, 0xf3, 0x99, 0x9a, /* U+FF98- */
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1120:Comments should contain text", Justification = "Code alignment.")]
        private static readonly byte[] DecomposeKatakanaMap =
        {
            /* */ 0x9e, 0x9f, 0x9e, 0x9f, 0x00, 0x00, 0x00, /* U+3099- */
            0x00, 0x67, 0x71, 0x68, 0x72, 0x69, 0x73, 0x6a, /* U+30a1- */
            0x74, 0x6b, 0x75, 0x76, 0x01, 0x77, 0x01, 0x78, /* U+30a8- */
            0x01, 0x79, 0x01, 0x7a, 0x01, 0x7b, 0x01, 0x7c, /* U+30b0- */
            0x01, 0x7d, 0x01, 0x7e, 0x01, 0x7f, 0x01, 0x80, /* U+30b8- */
            0x01, 0x81, 0x01, 0x6f, 0x82, 0x01, 0x83, 0x01, /* U+30c0- */
            0x84, 0x01, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, /* U+30c8- */
            0x01, 0x02, 0x8b, 0x01, 0x02, 0x8c, 0x01, 0x02, /* U+30d0- */
            0x8d, 0x01, 0x02, 0x8e, 0x01, 0x02, 0x8f, 0x90, /* U+30d8- */
            0x91, 0x92, 0x93, 0x6c, 0x94, 0x6d, 0x95, 0x6e, /* U+30e0- */
            0x96, 0x97, 0x98, 0x99, 0x9a, 0x9b, 0x00, 0x9c, /* U+30e8- */
            0x00, 0x00, 0x66, 0x9d, 0x4e, 0x00, 0x00, 0x08, /* U+30f0- */
            0x58, 0x58, 0x08, 0x65, 0x70, 0x00, 0x51,       /* U+30f8- */
        };

        private static readonly char[] MiscSymbolMap =
        {
            '\u00a2', '\u00a3', '\u00ac', '\u00af', '\u00a6', '\u00a5', '\u20a9',
        };

        /// <summary>
        /// Converts a string to full width characters.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The full width string.</returns>
        public static string ToFullWidth(string input)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                sb.Append(MapToFullWidth(input, i));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a string to half width characters.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The half width string.</returns>
        public static string ToHalfWidth(string input)
        {
            var sb = new StringBuilder();
            foreach (char chr in input)
            {
                sb.Append(MapToHalfWidth(chr));
            }

            return sb.ToString();
        }

        private static char MapToFullWidth(string input, int index)
        {
            char currentChar = input[index];

            if (currentChar > ' ' && currentChar <= '~' && currentChar != '\\')
            {
                return (char)(currentChar - 0x20 + 0xff00);
            }

            if (currentChar == ' ')
            {
                return (char)0x3000;
            }

            if (currentChar >= 0x00a2 && currentChar <= 0x00af)
            {
                int tableIndex = currentChar - 0x00a2;
                if (MiscSymbolsTable[tableIndex] != 0x00)
                {
                    return (char)(MiscSymbolsTable[tableIndex] + 0xff00);
                }

                return currentChar;
            }

            if (currentChar == 0x20a9)
            {
                return '\uffe6';
            }

            int n = ComposeKatakana(input, index, out char composedChar);
            if (n > 0)
            {
                return composedChar;
            }

            if (currentChar >= 0xffa0 && currentChar <= 0xffdc)
            {
                int tableIndex = currentChar - 0xffa0;
                if (HangulMappingTable[tableIndex] != 0x00)
                {
                    return (char)(HangulMappingTable[tableIndex] + 0x3100);
                }

                return currentChar;
            }

            return currentChar;
        }

        private static int ComposeKatakana(string input, int index, out char composedChar)
        {
            composedChar = '\0';

            char currentChar = input[index];

            switch (currentChar)
            {
                case '\u309b':
                case '\u309c':
                {
                    composedChar = (char)(currentChar - 0x02);
                    return 1;
                }

                case '\u30f0':
                case '\u30f1':
                case '\u30fd':
                {
                    composedChar = currentChar;
                    return 1;
                }

                default:
                {
                    int shift = currentChar - 0xff61;
                    if (shift < 0 || shift >= ComposeKatakanaMap.Length)
                    {
                        return 0;
                    }

                    composedChar = (char)(ComposeKatakanaMap[shift] + 0x3000);
                    break;
                }
            }

            if (input.Length - index <= 1)
            {
                return 1;
            }

            char before = composedChar;

            /* datakuten (voiced sound) */
            if (input[index + 1] == 0xff9e)
            {
                if ((currentChar >= 0xff76 && currentChar <= 0xff84) ||
                    (currentChar >= 0xff8a && currentChar <= 0xff8e) ||
                    currentChar == 0x30fd)
                {
                    composedChar++;
                }
                else if (currentChar == 0xff73)
                {
                    composedChar = '\u30f4'; /* KATAKANA LETTER VU */
                }
                else if (currentChar == 0xff9c)
                {
                    composedChar = '\u30f7'; /* KATAKANA LETTER VA */
                }
                else if (currentChar == 0x30f0)
                {
                    composedChar = '\u30f8'; /* KATAKANA LETTER VI */
                }
                else if (currentChar == 0x30f1)
                {
                    composedChar = '\u30f9'; /* KATAKANA LETTER VE */
                }
                else if (currentChar == 0xff66)
                {
                    composedChar = '\u30fa'; /* KATAKANA LETTER VO */
                }
            }

            /* handakuten (semi-voiced sound) */
            if (input[index + 1] == 0xff9f && currentChar >= 0xff8a && currentChar <= 0xff8e)
            {
                composedChar++;
                composedChar++;
            }

            return composedChar != before ? 2 : 1;
        }

        private static string MapToHalfWidth(char input)
        {
            int n = DecomposeKatakana(input, out string output);

            if (n > 0)
            {
                return output;
            }

            if (input == 0x3000)
            {
                return " ";
            }

            if (input == 0x3001)
            {
                return "\uff64";
            }

            if (input == 0x3002)
            {
                return "\uff61";
            }

            if (input == 0x300c || input == 0x300d)
            {
                return ((char)(input - 0x300c + 0xff62)).ToString();
            }

            if (input >= 0x3131 && input <= 0x3163)
            {
                int tmp = input - 0x3131 + 0xffa1;
                if (tmp >= 0xffbf)
                {
                    tmp += 0x03;
                }

                if (tmp >= 0xffc8)
                {
                    tmp += 0x02;
                }

                if (tmp >= 0xffd0)
                {
                    tmp += 0x02;
                }

                if (tmp >= 0xffd8)
                {
                    tmp += 0x02;
                }

                return ((char)tmp).ToString();
            }

            if (input == 0x3164)
            {
                return "\uffa0";
            }

            if (input == 0x2018 || input == 0x2019)
            {
                return "'";
            }

            if (input == 0x201c || input == 0x201d)
            {
                return "\"";
            }

            if (input > 0xff00 && input < 0xff5f && input != 0xff3c)
            {
                return ((char)(input - 0xff00 + 0x20)).ToString();
            }

            if (input >= 0xffe0 && input <= 0xffe6)
            {
                return MiscSymbolMap[input - 0xffe0].ToString();
            }

            return input.ToString();
        }

        private static int DecomposeKatakana(char input, out string output)
        {
            int shift = input - 0x3099;
            if (shift < 0 || shift >= DecomposeKatakanaMap.Length)
            {
                output = string.Empty;
                return 0;
            }

            byte k = DecomposeKatakanaMap[shift];
            if (k == 0)
            {
                output = input.ToString();
                return 1;
            }

            if (k > 0x60)
            {
                output = ((char)(k | 0xff00)).ToString();
                return 1;
            }

            var sb = new StringBuilder();
            int chr1 = k > 0x50 ? input - (k & 0xf) : DecomposeKatakanaMap[shift - k] | 0xff00;
            int chr2 = k == 2 ? 0xff9f : 0xff9e;
            sb.Append((char)chr1);
            sb.Append((char)chr2);
            output = sb.ToString();

            return 2;
        }
    }
}
