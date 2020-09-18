using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LR1
{
    enum TypeOfChannel
    {
        P1,
        P2,
        P3,
        P4,
        P5
    }
    enum TypeOfCode
    {
        DKOI,
        CRC
    }
    public class GodsClass
    {
        /// <summary>
        /// Variable for DKOI message
        /// </summary>
        string DKOI { get; set; }

        /// <summary>
        /// Variable for CRC message
        /// </summary>
        string CRC4 { get; set; }

        Stopwatch swTimmer;

        int ch1;
        int ch2;

        TypeOfCode type;
        TypeOfChannel channel;

        /// <summary>
        /// Constructor
        /// </summary>
        public GodsClass()
        {
            DKOI = "11101011" + "10000101" + "11010000" + "11001001" + "11001110" + "11001111" + "11010011" + "11100100" + "10000101" + "11001110" + "11010000" + "11010000" + "11001100" + "11110010" + "11001111" + "11001101" + "10000101" + "11001110" + "11001111" + "11010111" + "11010000" + "11011110";
            CRC4 = "10101001" + "00110101" + "00000000" + "00111111" + "00110000" + "00111010" + "00111100" + "01010011" + "10101010" + "10101001" + "00001001" + "00000000" + "00111010" + "00110000" + "00110000" + "00110110" + "10101010" + "10101001" + "01010000" + "00111100" + "00111001" + "00000000" + "00111010" + "00111100" + "00000101" + "00110000" + "01011111";

            swTimmer = new Stopwatch();

            type = 0;
            channel = 0;
        }
        /// <summary>
        /// Main function
        /// </summary>
        public void run()
        {
            for (; ; )
            {
                for (int i = 0; i < 5; i++)
                {
                    swTimmer.Reset();

                    string hamDecode = "";
                    string crcDecoded = "";

                    switch (channel)
                    {
                        case TypeOfChannel.P1:
                            ch1 = 5;
                            ch2 = 100;
                            break;
                        case TypeOfChannel.P2:
                            ch1 = 3;
                            ch2 = 100;
                            break;
                        case TypeOfChannel.P3:
                            ch1 = 2;
                            ch2 = 100;
                            break;
                        case TypeOfChannel.P4:
                            ch1 = 1;
                            ch2 = 100;
                            break;
                        case TypeOfChannel.P5:
                            ch1 = 9;
                            ch2 = 1000;
                            break;
                        default:
                            break;
                    }

                    type = (TypeOfCode)0;
                    string hammingEncoded = HamEncode(DKOI);
                    hammingEncoded = GenerateErrors(hammingEncoded, ch1, ch2, 7, type);

                    Console.WriteLine("DKOI length = " + DKOI.Length);
                    Console.WriteLine("Hamming encoded length = " + hammingEncoded.Length);

                    type = (TypeOfCode)1;
                    string crc4Encoded = CRCEncode(CRC4);

                    Console.WriteLine("CRC length = " + CRC4.Length);
                    Console.WriteLine("CRC encoded length = " + crc4Encoded.Length);

                    crc4Encoded = GenerateErrors(crc4Encoded, ch1, ch2, 24, type);

                    swTimmer.Start();
                    hamDecode = HammingDecode(hammingEncoded);
                    swTimmer.Stop();

                    Console.WriteLine("Hamming decode: " + swTimmer.Elapsed.TotalMilliseconds + "ms");

                    swTimmer.Reset();

                    swTimmer.Start();
                    crcDecoded = CRC4Decode(crc4Encoded);
                    swTimmer.Stop();

                    Console.WriteLine("CRC decode: " + swTimmer.Elapsed.TotalMilliseconds + "ms");

                    Console.WriteLine("Is message correct in Hamming?: " + (hamDecode == DKOI));
                    Console.WriteLine("Is message correct in CRC?: " + (crcDecoded == CRC4));

                    if (channel == TypeOfChannel.P5)
                    {
                        channel = 0;
                    }
                    else
                    {
                        channel++;
                    }

                    Console.WriteLine();
                    Console.WriteLine("==============================================================");
                    Console.WriteLine();
                }
                
                Console.ReadKey();
                Console.Clear();
            }
        }

        /// <summary>
        /// Hamming encode
        /// </summary>
        /// <param name="str">bit string for encode</param>
        /// <returns></returns>
        string HamEncode(string str)
        {
            string result = "";
            int x1, x2, x3;

            for (int i = 0; i < str.Length; i += 4)
            {
                x1 = ((int)(char.GetNumericValue(str[i]) +
                            char.GetNumericValue(str[i + 1]) +
                            char.GetNumericValue(str[i + 3]))) % 2;

                x2 = ((int)(char.GetNumericValue(str[i]) +
                            char.GetNumericValue(str[i + 2]) +
                            char.GetNumericValue(str[i + 3]))) % 2;

                x3 = ((int)(char.GetNumericValue(str[i + 1]) +
                            char.GetNumericValue(str[i + 2]) +
                            char.GetNumericValue(str[i + 3]))) % 2;

                result += x1.ToString() + x2.ToString();
                result += str[i];
                result += x3.ToString();
                result += str.Substring(i + 1, 3);
            }

            return result;
        }

        /// <summary>
        /// Hamming decode
        /// </summary>
        /// <param name="str">String for decode</param>
        /// <returns></returns>
        string HammingDecode(string str)
        {
            string result = "";
            string temp;

            for (int i = 0; i < str.Length; i += 7)
            {
                temp = str.Substring(i, 7);

                StringBuilder sb = new StringBuilder(temp);

                int x1 = (int)(char.GetNumericValue(temp[0]) +
                               char.GetNumericValue(temp[2]) +
                               char.GetNumericValue(temp[4]) +
                               char.GetNumericValue(temp[6]));

                string s1 = (x1 % 2).ToString();

                int x2 = (int)(char.GetNumericValue(temp[1]) +
                               char.GetNumericValue(temp[2]) +
                               char.GetNumericValue(temp[5]) +
                               char.GetNumericValue(temp[6]));

                string s2 = (x2 % 2).ToString();

                int x3 = (int)(char.GetNumericValue(temp[3]) + 
                               char.GetNumericValue(temp[4]) + 
                               char.GetNumericValue(temp[5]) + 
                               char.GetNumericValue(temp[6]));
                string s3 = (x3 % 2).ToString();

                if (s1 == "1" || s2 == "1" || s3 == "1")
                {
                    int pos = Convert.ToInt32(s3 + s2 + s1, 2);

                    sb[pos - 1] = (temp[pos - 1] == '1' ? '0' : '1');
                }

                sb.Remove(3, 1);
                sb.Remove(1, 1);
                sb.Remove(0, 1);

                result += sb.ToString();
            }

            return result;
        }

        /// <summary>
        /// CRC Encode
        /// </summary>
        /// <param name="str">String for encode</param>
        /// <returns></returns>
        string CRCEncode(string str)
        {
            string result = "";
            for (int i = 0; i < str.Length; i += 8)
            {
                result += str.Substring(i, 8);
                result += CRC4Calculate(str.Substring(i, 8));
            }
            return result;
        }
        string CRC4Decode(string str)
        {
            string data, newData, crc, newCrc, result = "";

            for (int i = 0; i < str.Length; i += 24)
            {
                data = str.Substring(0, 8);
                crc = str.Substring(i + 8, 16);
                newCrc = CRC4Calculate(data);

                if (crc != newCrc)
                {

                    for (int j = 0; j < 256; j++)
                    {
                        newData = Convert.ToString(j, 2).PadLeft(8, '0');
                        newCrc = CRC4Calculate(newData);

                        if (crc == newCrc)
                        {
                            data = newData;
                            break;
                        }
                    }
                }
                result += data;
            }

            return result;
        }
        /// <summary>
        /// Function for check the information bits
        /// </summary>
        /// <param name="str">Current string</param>
        /// <returns></returns>
        string CRC4Calculate(string str)
        {
            int messageInt = Convert.ToInt32(str, 2);
            string[] crc = new string[4];

            crc[0] = Convert.ToString((int)(messageInt % 11), 2);
            crc[1] = Convert.ToString((int)(messageInt % 13), 2);
            crc[2] = Convert.ToString((int)(messageInt % 14), 2);
            crc[3] = Convert.ToString((int)(messageInt % 15), 2);

            for (int i = 0; i < crc.Length; i++)
            {
                if (crc[i].Length < 4)
                {
                    for (int j = crc[i].Length; j < 4; j++)
                    {
                        crc[i] = "0" + crc[i];
                    }
                }
            }

            return crc[0] + crc[1] + crc[2] + crc[3];
        }

        /// <summary>
        /// Method for generate errors
        /// </summary>
        /// <param name="str">String for generate errors</param>
        /// <param name="chance1">Numerator</param>
        /// <param name="chance2">Denominator</param>
        /// <param name="blockSize">Number of block size</param>
        /// <param name="t">Type of message</param>
        /// <returns></returns>
        string GenerateErrors(string str, int chance1, int chance2, int blockSize, TypeOfCode t)
        {
            int[] errors = new int[(str.Length / blockSize) + 1];
            Array.Clear(errors, 0, errors.Length);

            Random rand = new Random(DateTime.Now.Millisecond);
            StringBuilder sb = new StringBuilder(str);

            for (int i = 0; i < sb.Length; i++)
            {
                if (rand.Next(0, chance2) < chance1)
                {
                    sb[i] = (sb[i] == '1' ? '0' : '1');
                    errors[i / blockSize]++;
                }
            }

            int counter = 0;
            for (int i = 0; i < errors.Length; i++)
            {

                if (errors[i] > 1)
                    counter++;
            }

            switch (type)
            {
                case TypeOfCode.DKOI:
                    Console.Write("All errors in DKOI: " + errors.Sum() + "\t");
                    Console.WriteLine("Double errors in DKOI: " + counter);
                    break;
                case TypeOfCode.CRC:
                    Console.Write("All errors in CRC: " + errors.Sum() + "\t");
                    Console.WriteLine("Double errors in CRC: " + counter);
                    break;
                default:
                    Console.WriteLine("*** ERROR ***");
                    break;
            }

            return sb.ToString();
        }
    }
}
