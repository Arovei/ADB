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
        long[] report = new long[22];

        public async Task Run()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig { });
            bool firstBoot = true;
            report = ReportFile.ReportStorage(report, firstBoot);
            firstBoot = false;

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
            {
                if (message.Content == "!boop")
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
                        "  `!rollstats - Usage: <3d6, 4d6, d20>`\n" +
                        "   Rolls base stats. Examples: !rollstats d20 | !rollstats 3d6\n" +
                        "  `!report`\n" + 
                        "   Reports the statistics of D20 rolls over the course of the bots current run.\n" +
                        "  `!reportwipe`\n" + 
                        "   Resets the report back to 0.\n" +
                        "  `!flip`\n" + 
                        "   Flips a coin. Heads or tails.\n" +
                        "  `!ud`\n" +
                        "   Gets the top three definitions from Urban Dictionary." + 
                        "  `!boop`\n" + 
                        "   Who doesn't like a boop?\n"
                        );
                }
                if (message.Content == "!halp")
                {
                    await message.Channel.SendMessageAsync(
                        "```Commands:```" +
                        "  `!roll`\n" +
                        "   Rolls the dice, duhhhhh.\n" +
                        "  `!rollstats`\n" +
                        "   Get cool numbers that could be better if you keep rolling.\n" +
                        "  `!report`\n" +
                        "   Just a bunch of numbers n crap.\n" +
                        "  `!reportwipe`\n" +
                        "   Resets the bunch of numbers n crap.\n" +
                        "  `!flip`\n" +
                        "   You mean you don't have ANY coins?\n" +
                        "  `!ud`\n" +
                        "   If you really care what everyone else things a word means.\n" +
                        "  `!boop`\n" +
                        "   This can stay. I like this.\n"
                        );
                }
                if (message.Content == "!fullhelp")
                {
                    if (message.Author.Id == Credentials.BotOwner)
                    {
                        await message.Channel.SendMessageAsync(
                        "```Full Command List:```" +
                        "`!roll\n" +
                        "!rollstats\n" +
                        "!report, !reportwipe\n" +
                        "!flip\n" +
                        "!ud\n" +
                        "!boop\n" +
                        "!help, !halp, !fullhelp\n" +
                        "!ping\n" + 
                        "!play, !join\n" +
                        "!debug`"
                        );
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("`Must be bot owner.`");
                    }
                }
            };

            //
            // Wave!
            //
            client.MessageReceived += async (message) =>
            {
                if (message.Content.Contains("o/ ") || message.Content.Contains("o7 "))
                {
                    await message.Channel.SendMessageAsync("o/");
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
            // Urban Dictionary call
            //   All Urban Dictionary cs files are found at https://github.com/huming2207/UrbanDict.NET
            //
            client.MessageReceived += async (message) =>
            {
                if (message.Content.Contains("!ud") && (!message.Author.IsBot))
                {

                    if (message.Content == "!ud " || message.Content == "!ud")
                    {
                        await message.Channel.SendMessageAsync((message.Author).Mention + ": What good is searching for nothing going to do you?");
                    }

                    List<string> definitions = new List<string>();
                    string input = (message.Content).Substring(4);
                    UrbanDefine scan = new UrbanDefine();
                    var result = scan.QueryByTerm(input).Result;

                    if (result.ResultType == "no_results")
                    {
                        await message.Channel.SendMessageAsync((message.Author).Mention + ": Not defined in Urban Dictionary.");
                    }

                    for (int i = 0; i < result.ItemList.Count; i++)
                    {
                        definitions.Add(result.ItemList[i].Definition);
                    }

                    await message.Channel.SendMessageAsync((message.Author).Mention + ": Top three definitions for: `" + input + "`\n```" + definitions[0] + "```\n```" + definitions[1] + "```\n```" + definitions[2]+"```");
                }
            };

            //
            // Stat roll call
            //            
            client.MessageReceived += async (message) =>
            {
                if (message.Content.Contains("!rollstats") && (!message.Author.IsBot))
                {
                    int styleType = 0;
                    string styleWord = "";
                    if (message.Content == "!rollstats")
                    {
                        Console.WriteLine(message.Author + " called " + message.Content + ". Invalid call. Defaulting to 3d6.");
                        await message.Channel.SendMessageAsync("`Defaulting to 3d6.`");
                        styleType = 0;
                        styleWord = "3d6";
                    }
                    if (message.Content.Contains("3d6"))
                    {
                        styleType = 0;
                        styleWord = "3d6";
                    }
                    if (message.Content.Contains("4d6"))
                    {
                        styleType = 1;
                        styleWord = "4d6 - 1d6";
                    }
                    if (message.Content.Contains("d20"))
                    {
                        styleType = 2;
                        styleWord = "d20";
                    }
                    if (message.Content.Contains("help"))
                    {                        
                        await message.Channel.SendMessageAsync("`Roll types: 3d6, 4d6, d20.\nEx: !rollstats 4d6`");
                    }
                    else
                    {
                        Console.WriteLine(message.Author + " called " + message.Content);
                        List<int> stats = StatsRoll.RollStats(styleType);
                        await message.Channel.SendMessageAsync("`Roll results for " + styleWord + ":\nSTR: " + stats[0] + "\nDEX: "
                            + stats[1] + "\nCON: " + stats[2] + "\nINT: " + stats[3] + "\nWIS: " + stats[4]
                            + "\nCHA: " + stats[5] + "\nTotal points: " + stats[6] + "`");
                    }
                }
            };

            //
            // Dice roll call
            //
            client.MessageReceived += async (message) =>
            {
                if (message.Content.Contains("!roll"))
                {
                    bool invalidCall = false;
                    bool ovrde = false;
                    bool validSides = false;
                    bool subtract = false;
                    int reroll = 1;
                    //Check if !roll was input with no parameters
                    if (message.Content == "!roll")
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
                    //Check if flat amount is subtracted
                    if (message.Content.Contains("-"))
                    {
                        subtract = true;
                    }
                    //Take the input, remove the non-numerics
                    string input = message.Content;
                    List<int> diceVolume = input.Split(new string[] { "D", "d" , "!roll", " ", "override", "+", "-" }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    //For reference above ^ diceVolume[0] = amount of times to roll; diceVolume[1] = dice sides
                    //If call was blank, input default roll values
                    if (invalidCall == true)
                    {
                        diceVolume.Add(1);
                        diceVolume.Add(6);
                        input = "!roll 1D6";
                    }
                    //Roll request text output
                    List<string> repeatRoll = new List<string>();
                    for (int i = 0; i < diceVolume.Count; i++)
                    {
                        //Check if the next step in the list exists
                        if (i+1 < diceVolume.Count)
                        {
                            repeatRoll.Add(diceVolume[i].ToString() + "d" + diceVolume[i+1].ToString());
                            i++;
                        }
                        //If it doesnt exist, use as a flat add
                        else
                        {
                            repeatRoll.Add(diceVolume[i].ToString());
                        }
                    }

                    if (diceVolume.Count > 3)
                    {
                        if (diceVolume.Count % 2 == 0)
                        {
                            reroll = diceVolume.Count/2;
                        }
                        else
                        {
                            reroll = (diceVolume.Count - 1) / 2;
                        }
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
                        //Can't be in the for loop or it gets reset; total amount from multi-face dice roll.
                        int multiTotal = 0;
                        //Steps through diceVolume for the rolldice call
                        int diceStep = 0;
                        for (int j = 0; j < reroll; j++)
                        {
                            //Calculate the dice roll(s)
                            List<int> output = DiceRoll.RollDice(diceVolume);

                            //Output result
                            int totalRoll = 0;
                            bool critSucc = false; //Usage for both success and fail commented out for now
                            bool critFail = false;
                            string allRolls = "";
                                                        
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
                            //Save report to file if report was altered
                            if (diceVolume[1] == 20)
                            {
                                report = ReportFile.ReportStorage(report, firstBoot);
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
                            int pointlessAddition = totalRoll; //Can't do math in a message, apparently.
                            multiTotal += totalRoll;
                            //If a flat bonus is applied, add it to the total, but only if not a multiroll. Multiroll flat bonus applied later
                            //The < 4 is if you have a -# or +# to the rolls which would make count = 3 at least
                            if (diceVolume.Count < 4 && diceVolume.Count % 2 != 0)
                            {
                                if (subtract == false)
                                {
                                    pointlessAddition = totalRoll + diceVolume[diceVolume.Count - 1];
                                }
                                else
                                {
                                    pointlessAddition = totalRoll - diceVolume[diceVolume.Count - 1];
                                }
                            }
                            
                            //Gotta split the results if you ask for a lot of dice rolls. Discord only allows certain message lengths.
                            if (rollList.Count > 500)
                            {
                                await message.Channel.SendMessageAsync("`Roll results of: " + repeatRoll[j] + "`\n```Total roll: " + pointlessAddition + "```");
                            }
                            else
                            {
                                if (j + 1 != reroll)
                                {
                                    await message.Channel.SendMessageAsync("`Roll results of " + repeatRoll[j] + ": " + allRolls + "`\n```Total roll: " + pointlessAddition + "```");
                                }
                                else
                                {
                                    if (diceVolume.Count % 2 != 0)
                                    {
                                        await message.Channel.SendMessageAsync("`Roll results of " + repeatRoll[j] + (subtract ? "-" : "+") + repeatRoll[j + 1] + ": " + allRolls + "`\n```Total roll: " + pointlessAddition + "```");
                                    }
                                    else
                                    {
                                        await message.Channel.SendMessageAsync("`Roll results of " + repeatRoll[j] + ": " + allRolls + "`\n```Total roll: " + pointlessAddition + "```");
                                    }
                                }
                            }
                            if (multiTotal > totalRoll)
                            {
                                if (diceVolume.Count % 2 != 0)
                                {
                                    if (subtract == false)
                                    {
                                        multiTotal += diceVolume[diceVolume.Count - 1];
                                    }
                                    else
                                    {
                                        multiTotal -= diceVolume[diceVolume.Count - 1];
                                    }                                    
                                    await message.Channel.SendMessageAsync("```Total results of " + repeatRoll[0] + "+" + repeatRoll[1] + (subtract?"-":"+") + repeatRoll[2] + ": " + multiTotal + "```");
                                }
                                else
                                {
                                    await message.Channel.SendMessageAsync("```Total results of " + repeatRoll[0] + "+" + repeatRoll[1] + ": " + multiTotal + "```");
                                }                                
                            }
                            //Crit success, fail or override flavor output
                            /*if (critSucc == true && critFail == false)
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
                            }*/
                            if (diceVolume[0] <= 0)
                            {
                                await message.Channel.SendMessageAsync("`What exactly were you trying to accomplish with a roll amount of " + diceVolume[0] + "?` :thinking:");
                            }
                            Console.WriteLine(message.Author + " called " + message.Content + " resulting in a total roll of: {0}", pointlessAddition);
                            if (j+1 != reroll)
                            {
                                diceVolume[0] = diceVolume[diceStep+2];
                                diceVolume[1] = diceVolume[diceStep+3];
                                diceStep += 2;
                            }
                        }
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
                    string user = message.Author.ToString();
                    if (message.Content == "!report")
                    {
                        if (report[21] != 0)
                        {
                            string reportList = "";
                            Console.WriteLine(message.Author + " asked for a D20 report!");
                            //Add the list to one string so you don't spam discord.
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
                            /* TODO:
                             * Scan through report list and find highest and lowest int (not including report[21])
                             * Set those numbers to minDice and maxDice and get a count of how many tied for each
                            */
                            long minRoll = long.MaxValue;
                            long maxRoll = 0;
                            //int minDice = 0;
                            //int maxDice = 0;
                            int j = 0;
                            int[] minDice = new int[20];
                            minDice[0] = 0;
                            int[] maxDice = new int[20];
                            maxDice[0] = 0;
                            for (int i = 1; i < 21; i++)
                            {
                                if (report[i] < minRoll)
                                {
                                    minRoll = report[i];
                                    minDice[0] = i;
                                }
                                if (report[i] > maxRoll)
                                {
                                    maxRoll = report[i];
                                    maxDice[0] = i;
                                }
                            }
                            await message.Channel.SendMessageAsync("`D20 roll breakdown:\n" + reportList + " rolls total.\nMost frequent side: "
                                + maxDice[0] + " with " + maxRoll + " rolls.\nLeast frequent side: " + minDice[0] + " with " + minRoll + " rolls.`");
                            Console.WriteLine(message.Author + " called " + message.Content + ". Report posted.");
                        }
                        else if (report[21] == 0)
                        {
                            Console.WriteLine(message.Author + " called " + message.Content + ". Nothing to report, however.");
                            await message.Channel.SendMessageAsync("`Nothing to report yet.`");
                        }
                    }
                    if ((message.Content == "!reportwipe") && (user == Credentials.AuthUser))
                    {
                        for (int i = 0; i < report.Length; i++)
                        {
                            report[i] = 0;
                        }
                        Console.WriteLine(message.Author + " called " + message.Content + ". Report stats set to 0.");
                        await message.Channel.SendMessageAsync((message.Author).Mention + " `Report stats reset.`");
                    }
                    else if (message.Content == "!reportwipe")
                    {
                        Console.WriteLine(message.Author + " called " + message.Content + ". Not authorized.");
                        await message.Channel.SendMessageAsync("Must be authorized user.");
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
                    string user = message.Author.ToString();
                    if (message.Content.Contains("!play"))
                    {
                        Console.WriteLine(message.Author + " called " + message.Content + ".");
                        await message.Channel.SendMessageAsync("NYI, sorry.");
                    }
                    else if (message.Content.Contains("!join") && (user == Credentials.AuthUser))
                    {
                        Console.WriteLine(message.Author + " called " + message.Content + ". Joined voice channel.");
                        IVoiceChannel channel = null;
                        channel = channel ?? (message.Author as IGuildUser)?.VoiceChannel;
                        if (channel == null) { await message.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }
                        //await channel.ConnectAsync();
                        // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
                        var audioClient = await channel.ConnectAsync();
                    }
                    else if (message.Content.Contains("!join"))
                    {
                        Console.WriteLine(message.Author + " called " + message.Content + ". Not authorized.");
                        await message.Channel.SendMessageAsync("Must be authorized user.");
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
                    if (message.Content == "!debug")
                    {
                        Console.WriteLine(message.Author);
                        string user = message.Author.ToString();
                        if (user == Credentials.AuthUser)
                        {
                            Console.WriteLine(user + " matches authorized user.");
                        }
                        else
                        {
                            Console.WriteLine(user + " is not an authorized user.");
                        }
                        await message.Channel.SendMessageAsync((message.Author).Mention + "`Debug launched. Check console.`");
                    }
                }
            };

            //Set Playing message
            await client.SetGameAsync("!help");
            // Configure the client to use a Bot token, and use our token
            await client.LoginAsync(TokenType.Bot, Credentials.Token);
            // Connect the client to Discord's gateway
            await client.StartAsync();

            // Block this task until the program is exited.
            await Task.Delay(-1);
        }        
    }
}