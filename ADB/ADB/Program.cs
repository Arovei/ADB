using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace ADB
{
    class Program
    {
        static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();

        //Create a DiscordClient with WebSocket support
        private DiscordSocketClient client;

        public async Task Run()
        {
            client = new DiscordSocketClient();

            //Bot Token
            string token = "MjY5OTUzNjgzMjE3ODQyMTc4.C1xT0Q.JdoyNNbsPRnn-Ama--ktCbI-IHI";

            //
            // Hook into the MessageReceived event on DiscordSocketClient
            // Ping call
            //
            client.MessageReceived += async (message) =>
            {   // Check to see if the Message Content is "!ping"
                if (message.Content == "!ping")
                    // Send 'pong' back to the channel the message was sent in
                    await message.Channel.SendMessageAsync("pong");
            };

            //
            // Help call
            //
            client.MessageReceived += async (message) =>
            {
                if (message.Content == "!help")
                {
                    await message.Channel.SendMessageAsync(
                        "```Commands:```" +
                        "  `!roll` - Usage: <Roll times>D<Dice sides>\n" +
                        "   Rolls dice. Defaults to 1D6. Examples: !roll 2D6 | !roll 1D10"
                        );
                }
            };

            //
            // Dice roll call
            //
            client.MessageReceived += async (message) =>
            {
                if (message.Content.Contains("!roll"))
                {
                    //Check if !roll was input with no parameters
                    bool invalidCall = false;
                    if (message.Content == "!roll") //ToLower().Contains("d")
                    {
                        Console.WriteLine(message.Author + " called " + message.Content + " which was an invalid call. Defaulting to 1d6.");
                        //await message.Channel.SendMessageAsync((message.Author).Mention + " `Error: Invalid paramters. Must be in format of: <Roll amount>d<Dice sides>. Ex: 3d10`");
                        invalidCall = true;
                    }
                    //Valid amount of rolls allowed
                    int validAmount = 10;
                    //Take the input, remove the non-numerics
                    string input = message.Content;
                    List<int> diceVolume = input.Split(new string[] { "D", "d" , "!roll", " " }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                    //If call was blank, input default roll values
                    if (invalidCall == true)
                    {
                        diceVolume.Add(1);
                        diceVolume.Add(6);
                        input = "!roll 1D6";
                    }
                    //Check if dice sides and roll amounts are valid
                    bool validSides = DiceRoll.ValidDice(diceVolume, validAmount);
                    if (validSides == true)
                    {
                        //Calculate the dice roll(s)
                        List<int> output = DiceRoll.RollDice(diceVolume);

                        //Output result
                        int totalRoll = 0;
                        bool critSucc = false;
                        bool critFail = false;
                        string allRolls = "";
                        string repeatRoll = input.Substring(5, input.Length - 5);
                        List<string> rollList = new List<string>();
                        for (int i = 0; i < output.Count; i++)
                        {
                            rollList.Add(output[i].ToString());
                            if (output[i] == 20)
                            {
                                critSucc = true;
                            }
                            else if (output[i] == 1)
                            {
                                critFail = true;
                            }
                            totalRoll += output[i];
                        }
                        for (int i = 0; i < rollList.Count; i++)
                        {
                            if (i + 1 == rollList.Count)
                            {
                                allRolls += rollList[i];
                            }
                            else if (i < rollList.Count)
                            {
                                allRolls += rollList[i] + ", ";
                            }
                        }
                        await message.Channel.SendMessageAsync("`Roll results of" + repeatRoll + ": " + allRolls + "`\n```Total roll: " + totalRoll + "```");
                        if (critSucc == true && critFail == false)
                        {
                            await message.Channel.SendMessageAsync("`Critical success!`:tada:");
                        }
                        else if (critFail == true && critSucc == false)
                        {
                            await message.Channel.SendMessageAsync("`Critical failure...`:sob:");
                        }
                        else if (critSucc && critFail == true)
                        {
                            await message.Channel.SendMessageAsync("`Critical... uhh... both?`:thinking:");
                        }
                        Console.WriteLine(message.Author + " called " + message.Content + " resulting in a total roll of: " + totalRoll);
                    }
                    else if (validSides == false)
                    {
                        Console.WriteLine(message.Author + " called " + message.Content + " which was an invalid call.");
                        await message.Channel.SendMessageAsync((message.Author).Mention + " `Error: Invalid paramters. Dice size must be 4, 6, 8, 10, 12, or 20. Roll amount must be > 0 and <= " + validAmount + ".`");
                    }
                }
            };

            ///
            /// Music Stream call
            ///

            // Configure the client to use a Bot token, and use our token
            await client.LoginAsync(TokenType.Bot, token);
            // Connect the client to Discord's gateway
            await client.ConnectAsync();

            // Block this task until the program is exited.
            await Task.Delay(-1);
        }
    }
}
