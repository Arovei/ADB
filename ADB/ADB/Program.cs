using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
//using libsodium;
//using opus;

namespace ADB
{
    class Program
    {
        static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();

        //Create a DiscordClient with WebSocket support
        private DiscordSocketClient client;
        long[] report = new long[22];

        public async Task Run()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig { AudioMode = Discord.Audio.AudioMode.Outgoing });

            //Bot Token
            

            //
            // Ping call
            //
            client.MessageReceived += async (message) =>
            {   // Check to see if the Message Content is "!ping"
                if (message.Content == "!ping")
                    // Send 'pong' back to the channel the message was sent in
                    await message.Channel.SendMessageAsync("pong");
            };

            //
            // Ping call
            //
            client.MessageReceived += async (message) =>
            {   // Check to see if the Message Content is "!ping"
                if (message.Content == "!boop")
                    // Send 'pong' back to the channel the message was sent in
                    await message.Channel.SendMessageAsync("*Boops " + (message.Author).Mention + "'s nose!*");
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
                        "  `!roll - Usage: <Roll times>D<Dice sides>`\n" +
                        "   Rolls dice. Defaults to 1D6. Examples: !roll 2D6 | !roll 1D10\n" +
                        "  `!report - Usage: !report`\n" + 
                        "   Reports the statistics of D20 rolls over the course of the bots current run.\n" +
                        "  `!flip - Usage: !flip`\n" + 
                        "   Flips a coin. Heads or tails.\n" + 
                        "  `!boop - Usage: !boop`\n" + 
                        "   Who doesn't like a boop?\n"
                        );
                }
            };

            //
            // Flip a coin
            //
            client.MessageReceived += async (message) =>
            {
                if (message.Content == "!flip")
                {
                    //Use diceroller to flip a 2 sided "dice", create said dice first.
                    List<int> flipSides = new List<int> { 1, 2 };
                    List<int> output = DiceRoll.RollDice(flipSides);
                    string headsTails = "";

                    if (output[0] == 1)
                    {
                        headsTails = "Heads";
                    }
                    else if (output[0] == 2)
                    {
                        headsTails = "Tails";
                    }
                    await message.Channel.SendMessageAsync("`Flip: " + headsTails + "`");
                    Console.WriteLine(message.Author + " called " + message.Content + " resulting in a flip of: " + headsTails);
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
                    bool ovrde = false;
                    bool validSides = false;
                    if (message.Content == "!roll") //ToLower().Contains("d")
                    {
                        Console.WriteLine(message.Author + " called " + message.Content + " which was an invalid call. Defaulting to 1d6.");
                        //await message.Channel.SendMessageAsync((message.Author).Mention + " `Error: Invalid paramters. Must be in format of: <Roll amount>d<Dice sides>. Ex: 3d10`");
                        invalidCall = true;
                    }
                    //Valid amount of rolls allowed
                    int validAmount = 10;
                    //Override to change max dice rolls
                    if (message.Content.Contains("override"))
                    {
                        ovrde = true;
                    }
                    //Take the input, remove the non-numerics
                    string input = message.Content;
                    List<int> diceVolume = input.Split(new string[] { "D", "d" , "!roll", " ", "override", "+" }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    //For reference above ^ input[0] = amount of times to roll; input[1] = dice sides

                    //If call was blank, input default roll values
                    if (invalidCall == true)
                    {
                        diceVolume.Add(1);
                        diceVolume.Add(6);
                        input = "!roll 1D6";                        
                    }
                    //Fixes a null error later on         
                    if (!message.Content.Contains("+"))
                    {
                        diceVolume.Add(0);
                    }
                    //Check if dice sides and roll amounts are valid
                    if (!ovrde)
                    {
                        validSides = DiceRoll.ValidDice(diceVolume, validAmount);
                    }
                    else if (ovrde)
                    {
                        //Alright, you can override it, but the program can only handle so much, c'mon now.
                        if (diceVolume[0] > 20000)
                        {
                            validSides = false;
                        }
                        else if (diceVolume[0] <= 20000)
                        {
                            validSides = true;
                        }                        
                    }
                    if (validSides == true)
                    {
                        //Calculate the dice roll(s)
                        List<int> output = DiceRoll.RollDice(diceVolume);

                        //Output result
                        int totalRoll = 0;
                        bool critSucc = false;
                        bool critFail = false;
                        string allRolls = "";
                        //Roll request text output, remove the !roll portion.
                        string repeatRoll = input.Substring(5, input.Length - 5);
                        //Turning the <int> rolls into <string>'s
                        List<string> rollList = new List<string>();
                        //Set up the list of individual rolls and add the total.
                        for (int i = 0; i < output.Count; i++)
                        {
                            rollList.Add(output[i].ToString());
                            //Statistics report logging
                            if (diceVolume[1] == 20)
                            {
                                report[output[i]]++; //Roll count of current dice roll
                                report[21]++; //Total dice rolls overall for D20
                            }
                            //Some silly fun for rolling 0 or 20 for crit fail or success. 
                            //Skip if override was used.
                            if (!ovrde)
                            {
                                if (output[i] == 20)
                                {
                                    critSucc = true;
                                }
                                else if (output[i] == 1)
                                {
                                    critFail = true;
                                }
                            }
                            totalRoll += output[i];
                        }
                        //Get the total amount of all rolls combined
                        for (int i = 0; i < rollList.Count; i++)
                        {
                            //Final roll, don't add a comma at the end.
                            if (i + 1 == rollList.Count)
                            {
                                allRolls += rollList[i];
                            }
                            //Not final rolls, add a comma before next number.
                            else if (i < rollList.Count)
                            {
                                allRolls += rollList[i] + ", ";
                            }
                        }
                        int pointlessAddition = totalRoll + diceVolume[2]; //Can't do math in a message, apparently.

                        //Gotta split the results if you ask for a lot of dice rolls. Discord only allows certain message lengths.
                        if(rollList.Count > 500)
                        {
                            await message.Channel.SendMessageAsync("`Roll results of:" + repeatRoll + "`\n```Total roll: " + pointlessAddition + "```");
                        }
                        else
                        {
                            await message.Channel.SendMessageAsync("`Roll results of" + repeatRoll + ": " + allRolls + "`\n```Total roll: " + pointlessAddition + "```");
                        }                        
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
                        if(diceVolume[0] <= 0)
                        {
                            await message.Channel.SendMessageAsync("`What exactly were you trying to accomplish with a roll amount of " + diceVolume[0] + "?` :thinking:");
                        }
                        Console.WriteLine(message.Author + " called " + message.Content + " resulting in a total roll of: {0}", totalRoll + diceVolume[2]);
                    }
                    else if (validSides == false)
                    {
                        if (!ovrde)
                        {
                            Console.WriteLine(message.Author + " called " + message.Content + " which was an invalid call.");
                            await message.Channel.SendMessageAsync((message.Author).Mention + " `Error: Invalid parameters. Dice size must be 4, 6, 8, 10, 12, or 20. Roll amount must be > 0 and <= " + validAmount + ".`");
                        }
                        else if (ovrde)
                        {
                            Console.WriteLine(message.Author + " called " + message.Content + " which was too large an override call.");
                            await message.Channel.SendMessageAsync((message.Author).Mention + " `Error: Invalid override parameters. Roll amount must be less than or equal to 20000.`");
                        }
                    }
                }
            };

            ///
            /// Dice roll report for D20
            ///
            client.MessageReceived += async (message) =>
            {
                if (!message.Author.IsBot)
                {
                    if (message.Content.Contains("!report"))
                    {
                        string reportList = "";
                        Console.WriteLine(message.Author + " asked for a D20 report!");
                        //
                        for (int i = 1; i < report.Length; i++)
                        {
                            //Final roll, don't add a comma at the end.
                            if (i + 1 == report.Length)
                            {
                                reportList += report[i].ToString();
                            }
                            //Not final rolls, add a comma before next number.
                            else if (i < report.Length)
                            {
                                reportList += i + ": " + report[i].ToString() + "\n";
                            }                            
                        }
                        await message.Channel.SendMessageAsync("`D20 roll breakdown:\n" + reportList + " rolls total.`");
                    }
                }
            };

            ///
            /// Music Stream call
            ///
            client.MessageReceived += async (message) =>
            {
                if (!message.Author.IsBot)
                {
                    if (message.Content.Contains("!play"))
                    {

                    }
                    else if (message.Content.Contains("!join"))
                    {
                        IVoiceChannel channel = null;
                        channel = channel ?? (message.Author as IGuildUser)?.VoiceChannel;
                        if (channel == null) { await message.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }
                        //await channel.ConnectAsync();
                        // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
                        var audioClient = await channel.ConnectAsync();
                    }
                }
            };

            ///
            /// Debuggy stuff!
            ///
            client.MessageReceived += async (message) =>
            {
                if (!message.Author.IsBot)
                {
                    if (message.Content.Contains("!debug"))
                    {
                        Console.WriteLine(message.Author);
                        await message.Channel.SendMessageAsync((message.Author).Mention + "`Secret code unlocked!`");
                    }
                }
            };


            // Configure the client to use a Bot token, and use our token
            await client.LoginAsync(TokenType.Bot, Credentials.Token);
            // Connect the client to Discord's gateway
            await client.ConnectAsync();

            // Block this task until the program is exited.
            await Task.Delay(-1);
        }
    }
}
