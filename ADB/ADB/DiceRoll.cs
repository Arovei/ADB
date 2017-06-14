using System;
using System.Collections.Generic;
using System.Security.Cryptography;

// MSDN crypto page has a nice RNG, shamelessly copied

namespace ADB
{
    public class DiceRoll
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public static List<int> RollDice(List<int> input)
        {
            // input[0] = number of dice to roll
            // input[1] = dice sides

            List<int> output = new List<int>();

            // Roll dice loop
            for (int i = 0; i < input[0]; i++)
            {
                byte roll = Roller((byte)input[1]);
                output.Add((int)roll);
                //output[roll - 1]++;
            }
            return output;
        }

        public static byte Roller(byte numberSides)
        {
            // Create a byte array to hold the random value.
            byte[] randomNumber = new byte[1];
            do
            {
                // Fill the array with a random value.
                rng.GetBytes(randomNumber);
            }
            while (!RollRange(randomNumber[0], numberSides));
            // Return the random number mod the number
            // of sides.  The possible values are zero-
            // based, so we add one.
            return (byte)((randomNumber[0] % numberSides) + 1);
        }

        private static bool RollRange(byte roll, byte numSides)
        {
            // There are MaxValue / numSides full sets of numbers that can come up
            // in a single byte.  For instance, if we have a 6 sided die, there are
            // 42 full sets of 1-6 that come up.  The 43rd set is incomplete.
            int fullSetsOfValues = Byte.MaxValue / numSides;

            // If the roll is within this range of fair values, then we let it continue.
            // In the 6 sided die case, a roll between 0 and 251 is allowed.  (We use
            // < rather than <= since the = portion allows through an extra 0 value).
            // 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair
            // to use.
            return roll < numSides * fullSetsOfValues;
        }

        public static bool ValidDice(List<int> input, int validAmt)
        {
            // input[0] = number of dice to roll
            // input[1] = dice sides

            //Valid dice sides allowed
            int[] validSides = new int[6] { 4, 6, 8, 10, 12, 20 };
            bool validDice = false;
            //Check if roll request is greater than 0 but less than allowed amount
            if ((input[0] < validAmt+1) && (input[0] > 0))
            {
                validDice = true;
                for (int i = 0; i < validSides.Length; i++)
                {
                    if (validSides[i] == input[1])
                    {
                        validDice = true;
                        break;
                    }
                    else if (validSides[i] != input[1])
                    {
                        validDice = false;
                    }
                }
            }
            return validDice;
        }
    }
}
