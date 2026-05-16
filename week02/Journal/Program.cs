using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Journal myJournal = new Journal();
        List<string> prompts = new List<string>
        {
            "What was the best part of your day?",
            "What are you grateful for today?",
            "Describe a challenge you faced and how you overcame it.",
            "What is something new you learned today?",
            "Write about a moment that made you smile today."
        };

        int choice = 0;
        while (choice != 5)
        {
            Console.WriteLine("\nWelcome to your journal. Please select one of the following choices:");
            Console.WriteLine("1. Write");
            Console.WriteLine("2. Display");
            Console.WriteLine("3. Load");
            Console.WriteLine("4. Save");
            Console.WriteLine("5. Exit");
            Console.Write("What would you like to do? ");
            
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                if (choice == 1)
                {
                    Random random = new Random();
                    string prompt = prompts[random.Next(prompts.Count)];
                    Console.WriteLine($"\n{prompt}");
                    Console.Write("> ");
                    string response = Console.ReadLine();
                    string date = DateTime.Now.ToShortDateString();

                    Entry newEntry = new Entry(date, prompt, response);
                    myJournal.AddEntry(newEntry);
                }
                else if (choice == 2)
                {
                    myJournal.DisplayAll();
                }
                else if (choice == 3)
                {
                    Console.Write("What is the name of the file? ");
                    string filename = Console.ReadLine();
                    myJournal.LoadFromFile(filename);
                }
                else if (choice == 4)
                {
                    Console.Write("What would you like to name the file ? ");
                    string filename = Console.ReadLine();
                    myJournal.SaveToFile(filename);
                }
            }
        }
    }
}