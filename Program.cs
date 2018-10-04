﻿using System;
using System.Linq;
using LiteDB;
using System.Text.RegularExpressions;

namespace LiteDBClient
{
    class AutoCompletionHandler : IAutoCompleteHandler
{
    // characters to start completion from
    public char[] Separators { get; set; } = new char[] { ' ', '.', '/', '\\' };

    // text - The current text entered in the console
    // index - The index of the terminal cursor within {text}
    public string[] GetSuggestions(string text, int index)
    {
        string cmdpattern = @"^db.*.";
        if (text.StartsWith("\\"))
            return new string[] { "a", "c", "h", "q", "s" }; 
        else if (text.StartsWith("db.") && text.Length == 3) 
        {
            string[] collections;
            using(var db = new LiteDatabase(Program.connString))
            {
                collections = db.GetCollectionNames().ToArray();
            }
            return collections;
        }
        else if (Regex.Match(text, cmdpattern).Success)
        {
            string[] commands = new string[] {"insert", "bulk", "update", "delete", "ensureIndex", "indexes", "dropIndex", "drop", "rename", "count", "min", "max", "find", "select"};
            var result = commands.Where (c => c.StartsWith(text.Split('.')[2])).ToArray();

            return result;
        }
        else
            return null;
    }
}

    class Program
    {
        public static string connString = "";
        static void Main(string[] args)
        {
	        ReadLine.HistoryEnabled = true;
            ReadLine.AutoCompletionHandler = new AutoCompletionHandler();
            Console.Clear();
            Console.WriteLine("LiteDBClient\nFor help type \\h");
            
            string prompt = "";
            Console.BackgroundColor = ConsoleColor.Black;
            while (prompt != "\\q") 
            {
		        Console.Write(">");
                prompt = ReadLine.Read();
		        ReadLine.AddHistory(prompt);
                if (prompt == "\\c") 
                {
                    Console.Write("Connection String: ");
                    connString = Console.ReadLine();
                }
                else if (prompt == "\\q") 
                {
                    
                }
                else if (prompt == "\\a") 
                {
                    Console.WriteLine("LiteDBClient (the simplest LiteDB shell ever)\nBy Jan Antos (http://www.janantos.com)\nLicenced under \"The Unlicence\" For more information, please refer to <http://unlicense.org>\n ");
                }
                else if (prompt == "\\h") 
                {
                    Console.WriteLine("LiteDBClient (the simplest LiteDB shell ever)\nUsage:\n\\a About\n\\c Set connection string\n\\q Quit console\n\\s Shrink database\nOr enter LiteDB command");
                }
                else if (prompt == "\\s") 
                {
                    Console.WriteLine("Shrink Database? [y/N]");
                    string answ = Console.ReadLine();
                    if(answ=="Y" || answ=="y") 
                    {
                        using(var db = new LiteDatabase(connString))
                        {
                            db.Shrink();
                        }
                    }
                }
                else if (prompt.StartsWith("db."))
                {
                    try 
                    {
                        int line = 1;
                        using(var db = new LiteDatabase(connString))
                        {
                            foreach (var x in db.Engine.Run(prompt).ToList())
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("[" + line.ToString() + "]:");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine(x.ToString());
                                line++;
                            }
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Items returned: " + (line-1).ToString());
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error.");
                        Console.WriteLine("No database opened.");
                        Console.ForegroundColor = ConsoleColor.White;
                    } 
                    catch (Exception ex){
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error.");
                        Console.WriteLine(ex.Message);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error.");
                    Console.WriteLine("Command not recognized");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
