﻿/* MAIN PROGRAM FOR DOG-E Node VII
 * AgentAileron 2018
*/

using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;


namespace DogeNode7{
    // Class to hold bot stats
    public static class BotStats{
        public const string strPrefix = "$";
        static string[] authFile = Reg.Util.GetFileContents(@"./auth.txt");
        
        public static DateTime starttime = DateTime.Now;

        public static string selfId = authFile[2];
        public static string gAPIkey = authFile[4];
    
    }


    // Main Program Class
    class ProgramCode{

        static string auth_token;
        public static DiscordClient bot;
        static CommandsNextModule cmd_module;

        // == INITIALISATION == //
        static void Main(string[] args){

            // -- Attempt to load in the auth token from file, halt execution otherwise --
            auth_token = Reg.Util.GetFileContents(@"./auth.txt")[0];

            // Print a message on successful initialisation
            Console.WriteLine("-//- DOG-E (unsharded) Initialised successfully! -//-");
            Console.WriteLine("Auth token: " + auth_token);
            Console.WriteLine("Init time:  " + BotStats.starttime);

            // Begin asynchronous instance (Blocks this thread indefinitely)
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();

            Console.WriteLine("THIS TEXT SHOULD NEVER PRINT");  // Unreachable, unless MainAsync returns

        }


        // == ASYNC EXECUTION == //
        static async Task MainAsync(string[] args){

            // Initialise bot
            bot = new DiscordClient(new DiscordConfiguration{
                Token = auth_token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Error
            });

            // Initialise Command Interpreter
            cmd_module = bot.UseCommandsNext(new CommandsNextConfiguration{
                StringPrefix = BotStats.strPrefix
            });

            cmd_module.RegisterCommands<CommandModules.CommandListTopLevel>();  // Register all top level commands
            cmd_module.RegisterCommands<CommandModules.CommandListPseudoRNG>(); // Register PRNG commands
            cmd_module.RegisterCommands<CommandModules.CommandListSudo>();      // Register Sudo commands

            // Event trigger for capturing user statistics
            bot.MessageCreated += async newMsg =>{
                if (newMsg.Author.Id.ToString() != BotStats.selfId){    // Don't act on self-sent messages
                    await Reg.StatMethod.messageLogAsync(newMsg.Message);
                }

            };

            bot.PresenceUpdated += async usrChange =>{
                await Reg.StatMethod.userStateLogAsync(usrChange.Member, usrChange.PresenceBefore, usrChange.Status);
            };

            await bot.ConnectAsync();   // Connect the bot
            await Task.Delay(-1);       // Wait forever
        }

    } // Class boundary
} // NameSpace boundary