using System;
using System.Linq;
using LiteDB;

namespace LiteDBClient
{
    class Program
    {
        static void Main(string[] args)
        {
	    ReadLine.HistoryEnabled = false;
            Console.Clear();
            Console.WriteLine("LiteDBClient\nFor help type \\h");
            string connString = "";
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
