using System;
using System.Collections.Generic;
using System.Collections;

namespace Common
{
    public static class TextFilter
    {
        private static HashSet<string> hashSet = new HashSet<string>();
        private static byte[] fastCheck = new byte[char.MaxValue];
        private static byte[] fastLength = new byte[char.MaxValue];
        private static BitArray charCheck = new BitArray(char.MaxValue);
        private static BitArray endCheck = new BitArray(char.MaxValue);
        private static int maxWordLength = 0;
        private static int minWordLength = int.MaxValue;
        private static string mask = "*";


        private static int minLength = int.MaxValue;
        private static int maxLength = 0;

        /// <summary>
        /// 添加关键字
        /// </summary>
        /// <param name="word"></param>
        public static void AddKey(string word)
        {
            maxWordLength = System.Math.Max(maxWordLength, word.Length);
            minWordLength = System.Math.Min(minWordLength, word.Length);

            for (int i = 0; i < 7 && i < word.Length; i++)
            {
                fastCheck[word[i]] |= (byte)(1 << i);
            }

            for (int i = 7; i < word.Length; i++)
            {
                fastCheck[word[i]] |= 0x80;
            }

            if (word.Length == 1)
            {
                charCheck[word[0]] = true;
            }
            else
            {
                fastLength[word[0]] |= (byte)(1 << (System.Math.Min(7, word.Length - 2)));
                endCheck[word[word.Length - 1]] = true;
                hashSet.Add(word);
            }
        }

        /// <summary>
        /// 过滤某段文字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Filter(string text)
        {
            if (text == null) return null;
            if (text.Length == 0) return text;

           // var rets = text.ToCharArray();
            int index = 0;

            while (index < text.Length)
            {
                int count = 1;
                while (index < text.Length - 1 && (fastCheck[text[index]] & 1) == 0) index++;

                char begin = text[index];

                if (minWordLength == 1 && charCheck[begin])
                {
                    text = text.Replace(text[index], '*');
                    //rets[index] = '*';
                }

                for (int j = 1; j <= System.Math.Min(maxWordLength, text.Length - index - 1); j++)
                {
                    char current = text[index + j];

                    if ((fastCheck[current] & 1) == 0)
                    {
                        ++count;
                    }

                    if ((fastCheck[current] & (1 << System.Math.Min(j, 7))) == 0)
                    {
                        break;
                    }

                    if (j + 1 >= minWordLength)
                    {
                        if ((fastLength[begin] & (1 << System.Math.Min(j - 1, 7))) > 0 && endCheck[current])
                        {
                            string sub = text.Substring(index, j + 1);

                            if (hashSet.Contains(sub))
                            {
                                for (int ii = 0; ii < sub.Length; ii++)
                                {
                                   // rets[index + ii] = '*';
                                    text = text.Replace(sub, "*".PadRight(sub.Length, '*'));
                                }
                            }
                        }
                    }
                }

                index += count;
            }

            return text;
        }
        public static string FilterAll(string text)
        {
            string result = text;
            for (int i = 0, iMax = text.Length; i < iMax; i++)
            {
                result = Filter(result);
            }
            return result;
        }
        /// <summary>
        /// 判断某段文字里面有没有非法字符
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool HasIllegalWord(string text)
        {
            int index = 0;

            while (index < text.Length)
            {
                int count = 1;

                if (index > 0 || (fastCheck[text[index]] & 1) == 0)
                {
                    while (index < text.Length - 1 && (fastCheck[text[++index]] & 1) == 0) ;
                }

                char begin = text[index];

                if (minWordLength == 1 && charCheck[begin])
                {
                    return true;
                }

                int j = 1;
                int jMax = System.Math.Min(maxWordLength, text.Length - index - 1);
                for (;j <= jMax; j++)
                {
                    char current = text[index + j];

                    if ((fastCheck[current] & 1) == 0)
                    {
                        ++count;
                    }

                    if ((fastCheck[current] & (1 << System.Math.Min(j, 7))) == 0)
                    {
                        break;
                    }

                    if (j + 1 >= minWordLength)
                    {
                        if ((fastLength[begin] & (1 << System.Math.Min(j - 1, 7))) > 0 && endCheck[current])
                        {
                            string sub = text.Substring(index, j + 1);

                            if (hashSet.Contains(sub))
                            {
                                return true;
                            }
                        }
                    }
                }

                index += count;
            }

            return false;
        }
        
        private static string GetMaskString(int length)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for(int i=0;i<length;i++)
            {
                sb.Append(mask);
            }
            return sb.ToString();
        }
    }
}
