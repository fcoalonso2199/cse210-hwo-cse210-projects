using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "passages.txt";
        EnsureDefaultLibraryExists(filePath);

        List<Scripture> library = LoadLibraryFromFile(filePath);
        
        if (library.Count == 0)
        {
            Console.WriteLine("Error: Could not load scripture passages from the file.");
            return;
        }

        Scripture scripture = null;
        bool validSelection = false;

        while (!validSelection)
        {
            Console.Clear();
            Console.WriteLine("=================================================================");
            Console.WriteLine("                   SCRIPTURE MEMORIZER PROGRAM                   ");
            Console.WriteLine("=================================================================\n");
            Console.WriteLine("Select the passage you want to memorize:\n");

            // Print choices loaded from the file
            for (int i = 0; i < library.Count; i++)
            {
                string textSample = library[i].GetDisplayText().Split('-')[0].Trim();
                Console.WriteLine($" [{i + 1}] {textSample}");
            }
            Console.WriteLine($" [{library.Count + 1}] Select a random verse");
            Console.WriteLine(" [0] Exit program");
            
            Console.Write("\nEnter your choice: ");
            string choiceInput = Console.ReadLine();

            if (int.TryParse(choiceInput, out int choice))
            {
                if (choice == 0)
                {
                    Console.WriteLine("\nGoodbye!");
                    return;
                }
                else if (choice > 0 && choice <= library.Count)
                {
                    scripture = library[choice - 1];
                    validSelection = true;
                }
                else if (choice == library.Count + 1)
                {
                    Random rand = new Random();
                    scripture = library[rand.Next(library.Count)];
                    validSelection = true;
                }
                else
                {
                    Console.WriteLine("\nInvalid option. Press any key to try again...");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("\nPlease enter a valid number. Press any key to continue...");
                Console.ReadKey();
            }
        }

        // --- MAIN MEMORIZATION LOOP ---
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=================================================================");
            Console.WriteLine("                    SCRIPTURE MEMORIZER                          ");
            Console.WriteLine("=================================================================\n");
            
            Console.WriteLine(scripture.GetDisplayText());
            Console.WriteLine("\n=================================================================");

            if (scripture.IsCompletelyHidden())
            {
                Console.WriteLine("\nCongratulations! You have completed the passage from memory.");
                break;
            }

            Console.WriteLine("\nPress ENTER to hide words or type 'quit' to finish:");
            string input = Console.ReadLine();

            if (input.Trim().ToLower() == "quit")
            {
                break;
            }
            scripture.HideRandomWords(3);
        }

        Console.WriteLine("\nThank you for using the Scripture Memorizer. Goodbye!");
    }

    private static void EnsureDefaultLibraryExists(string filePath)
    {
        if (!File.Exists(filePath))
        {
            string[] defaultPassages = {
                "Proverbs|3|5|6|Trust in the LORD with all thine heart; and lean not unto thine own understanding. In all thy ways acknowledge him, and he shall direct thy paths.",
                "John|3|16||For God so loved the world, that he gave his only begotten Son, that whosoever believeth in him should not perish, but have everlasting life.",
                "Psalms|23|1|3|The LORD is my shepherd; I shall not want. He maketh me to lie down in green pastures: he leadeth me beside the still waters. He restoreth my soul.",
                "1 Nephi|3|7||And it came to pass that I, Nephi, said unto my father: I will go and do the things which the Lord hath commanded, for I know that the Lord giveth no commandments unto the children of men, save he shall prepare a way for them that they may accomplish the thing which he commandeth them.",
                "Alma|32|21||And now as I said concerning faith—faith is not to have a perfect knowledge of things; therefore if ye have faith ye hope for things which are not seen, which are true.",
                "Moroni|10|4|5|And when ye shall receive these things, I would exhort you that ye would ask God, the Eternal Father, in the name of Christ, if these things are not true; and if ye shall ask with a sincere heart, with real intent, having faith in Christ, he will manifest the truth of it unto you, by the power of the Holy Ghost. And by the power of the Holy Ghost ye may know the truth of all things."
            };
            File.WriteAllLines(filePath, defaultPassages);
        }
    }
    private static List<Scripture> LoadLibraryFromFile(string filePath)
    {
        List<Scripture> library = new List<Scripture>();

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split('|');
                if (parts.Length < 5) continue;

                string book = parts[0];
                int chapter = int.Parse(parts[1]);
                int startVerse = int.Parse(parts[2]);
                string endVerseStr = parts[3];
                string text = parts[4];

                Reference reference;
                if (string.IsNullOrEmpty(endVerseStr))
                {
                    reference = new Reference(book, chapter, startVerse);
                }
                else
                {
                    int endVerse = int.Parse(endVerseStr);
                    reference = new Reference(book, chapter, startVerse, endVerse);
                }

                library.Add(new Scripture(reference, text));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading the library file: {ex.Message}");
        }

        return library;
    }
}