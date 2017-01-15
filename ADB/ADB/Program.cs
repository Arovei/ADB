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

            // Hook into the MessageReceived event on DiscordSocketClient
            client.MessageReceived += async (message) =>
            {   // Check to see if the Message Content is "!ping"
                if (message.Content == "!ping")
                    // Send 'pong' back to the channel the message was sent in
                    await message.Channel.SendMessageAsync("pong");
            };

            // Dice roll request
            client.MessageReceived += async (message) =>
            {
                if (message.Content.Contains("!roll"))
                {
                    //Valid amount of rolls allowed
                    int validAmount = 10;
                    //Take the input, remove the non-numerics
                    string input = message.Content;
                    List<int> diceVolume = input.Split(new string[] { "D", "d" , "!roll", " " }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                    //Check if dice sides and roll amounts are valid
                    bool validSides = DiceRoll.ValidDice(diceVolume, validAmount);
                    if (validSides == true)
                    {
                        //Calculate the dice roll(s)
                        List<int> output = DiceRoll.RollDice(diceVolume);

                        //Output result
                        int totalRoll = 0;
                        bool natural = false;
                        string allRolls = "";
                        string repeatRoll = input.Substring(5, input.Length - 5);
                        List<string> rollList = new List<string>();
                        for (int i = 0; i < output.Count; i++)
                        {
                            rollList.Add(output[i].ToString());
                            if (output[i] == 20)
                            {
                                natural = true;
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
                        if (natural == true)
                        {
                            await message.Channel.SendMessageAsync("`You're a natural!`:tada:");
                        }
                    }
                    else if (validSides == false)
                    {
                        await message.Channel.SendMessageAsync("`Error: Invalid paramters. Dice size must be 4, 6, 8, 10, 12, or 20. Roll amount must be > 0 and <= " + validAmount + ".`");
                    }
                }
            };

            // Configure the client to use a Bot token, and use our token
            await client.LoginAsync(TokenType.Bot, token);
            // Connect the client to Discord's gateway
            await client.ConnectAsync();

            // Block this task until the program is exited.
            await Task.Delay(-1);
        }
    }
}
