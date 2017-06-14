using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADB
{
    class StatsRoll
    {
        //Decide stat roll option
        public static List<int> RollStats(int style)
        {
            //Create the final list to catch the total tallies
            List<int> finalTotal = new List<int>();

            //3D6 reroll ones
            if (style == 0)
            {
                //Create amount of dice to roll to send to DiceRoll
                List<int> diceVolume = new List<int>() { 1, 6 };

                //Loop to roll 6 dice sets for 6 stats
                for (int j = 0; j < 6; j++)
                {
                    //Create the output list to catch the results, needs to reset every loop
                    List<int> output = new List<int>() { 1, 1, 1, 1, 1, 1 };
                    //Loop to roll 3 dice per set
                    for (int i = 0; i < 3; i++)
                    {
                        //Reroll if a one
                        while (output[i] == 1)
                        {
                            output[i] = DiceRoll.RollDice(diceVolume)[0];
                        }
                    }                    
                    finalTotal.Add(output[0] + output[1] + output[2]);
                }
            }
            //4D6 - lowest 1D reroll ones
            if (style == 1)
            {
                //Create amount of dice to roll to send to DiceRoll
                //We will be rolling 6 stats, but four times for each
                //stat and dropping the lowest, so it will be easier to roll one dice at a time
                //and load the final result into finalTotal instead of trying to sift and add and remove
                List<int> diceVolume = new List<int>() { 1, 6 };                

                //Loop to roll 6 dice sets for 6 stats
                for (int j = 0; j < 6; j++)
                {
                    //Create the output list to catch the results, needs to reset every loop
                    List<int> output = new List<int>() { 1, 1, 1, 1, 1, 1 };
                    //Loop to roll 4 dice per set
                    for (int i = 0; i < 4; i++)
                    {
                        //Reroll if a one
                        while (output[i] == 1)
                        {
                            output[i] = DiceRoll.RollDice(diceVolume)[0];
                        }
                    }
                    //Sort rolls in order of size and reverse the order
                    output.Sort();
                    output.Reverse();

                    finalTotal.Add(output[0] + output[1] + output[2]);
                }
            }
            //1D20
            if (style == 2)
            {
                List<int> diceVolume = new List<int>() { 6, 20 };
                finalTotal = DiceRoll.RollDice(diceVolume);
            }
            finalTotal.Add(finalTotal[0] + finalTotal[1] + finalTotal[2] + finalTotal[3] + finalTotal[4] + finalTotal[5]);
            Console.WriteLine("Stat roll tally: " + finalTotal[0] + " " + finalTotal[1] + " " + finalTotal[2] + " " + finalTotal[3] 
                + " " + finalTotal[4] + " " + finalTotal[5] + "\nFor a total of: " + finalTotal[6]);
            return finalTotal;
        }        
    }
}
