using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    /* 
     * INFORME DE CREATIVIDAD Y REQUISITOS SUPERADOS:
     * 1. Selección Inteligente de Palabras: El método Scripture.HideRandomWords() filtra dinámicamente las 
     *    palabras para elegir únicamente aquellas que NO han sido ocultas previamente. Esto garantiza una 
     *    progresión limpia sin re-seleccionar espacios vacíos.
     * 2. Biblioteca de Pasajes Externa (Persistencia de Archivos): El programa busca un archivo llamado 'passages.txt'.
     *    Si existe, lee los versículos desde allí. Si no existe, genera un archivo automático con pasajes por defecto.
     *    Esto permite al usuario expandir su biblioteca de memorización sin tener que modificar el código fuente.
     */

    static void Main(string[] args)
    {
        string filePath = "passages.txt";
        EnsureDefaultLibraryExists(filePath);

        List<Scripture> library = LoadLibraryFromFile(filePath);
        
        if (library.Count == 0)
        {
            Console.WriteLine("Error: Bible passages could not be loaded. Please ensure 'passages.txt' exists and is properly formatted.");
            return;
        }

        // Selecciona un pasaje al azar de la biblioteca cargada
        Random rand = new Random();
        Scripture scripture = library[rand.Next(library.Count)];

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=================================================================");
            Console.WriteLine("                    Scripture Memorizer                          ");                 
            Console.WriteLine("=================================================================\n");
            
            Console.WriteLine(scripture.GetDisplayText());
            Console.WriteLine("\n=================================================================");

            if (scripture.IsCompletelyHidden())
            {
                Console.WriteLine("\nCongratulations! You have completed the scripture memorization.");
                break;
            }

            Console.WriteLine("\nPress ENTER to hide words or type 'exit' to finish:");
            string input = Console.ReadLine();

            if (input.Trim().ToLower() == "exit")
            {
                break;
            }

            // Oculta 3 palabras en cada turno
            scripture.HideRandomWords(3);
        }

        Console.WriteLine("\nThanks for using the Scripture Memorizer. See you later!");
    }

    private static void EnsureDefaultLibraryExists(string filePath)
    {
        if (!File.Exists(filePath))
        {
            string[] defaultPassages = {
                "John|3|16||For God so loved the world that he gave his one and only Son, that whoever believes in him shall not perish but have eternal life.",
                "Proverbs|3|5|6|Trust in the Lord with all your heart and lean not on your own understanding; in all your ways submit to him, and he will make your paths straight.",
                "Philippians|4|13||I can do all this through him who gives me strength.",
                "Psalm|23|1|3|The Lord is my shepherd, I lack nothing. I make you lie down in green pastures, you restore my soul."
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
            Console.WriteLine($"Error reading library file: {ex.Message}");
        }

        return library;
    }
}
