using System;
using System.Collections.Generic;
using System.Threading;

abstract class Activity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    protected int Duration { get; private set; }

    public Activity(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void StartMessage()
    {
        Console.WriteLine($"\n--- Starting: {Name} ---");
        Console.WriteLine(Description);

        Console.Write("How many seconds for this session? ");
        if (!int.TryParse(Console.ReadLine(), out int duration) || duration <= 0)
        {
            Console.WriteLine("Invalid input. Defaulting to 10 seconds.");
            duration = 10;
        }
        Duration = duration;

        Console.WriteLine("Prepare to begin...");
        ShowAnimation(3);
    }

    public void EndMessage()
    {
        Console.WriteLine("\nGood job!");
        ShowAnimation(2);
        Console.WriteLine($"You have completed {Name} for {Duration} seconds.");
        ShowAnimation(3);
    }

    protected void ShowAnimation(int seconds)
    {
        if (seconds <= 0) return;

        char[] icons = { '|', '/', '-', '\\' };
        for (int i = seconds; i > 0; i--)
        {
            char icon = icons[(seconds - i) % 4];
            Console.Write($"\rPlease wait... {icon} {i}s");
            Thread.Sleep(1000);
        }
        Console.WriteLine("\rLet's continue!            ");
    }

    public abstract void Run();
}

class Breathing : Activity
{
    public Breathing() : base(
        "Breathing",
        "This activity will help you relax by guiding you through breathing in and out slowly.\n" +
        "Clear your mind and focus on your breathing.")
    { }

    public override void Run()
    {
        StartMessage();

        int elapsed = 0;
        while (elapsed < Duration)
        {
            Console.WriteLine();
            AnimateBreathe("Breathe in", 4);
            elapsed += 4;

            if (elapsed < Duration)
            {
                AnimateBreathe("Breathe out", 4);
                elapsed += 4;
            }
        }

        EndMessage();
    }

    private void AnimateBreathe(string message, int seconds)
    {
        for (int i = 1; i <= seconds; i++)
        {
            string dots = new string('.', i);
            Console.Write($"\r{message} {dots}   ");
            Thread.Sleep(1000);
        }
        Console.WriteLine();
    }
}

class Reflection : Activity
{
    public Reflection() : base(
        "Reflection",
        "This activity will help you reflect on times in your life when you have shown strength\n" +
        "and resilience. This will help you recognize the power you have and how you can use it\n" +
        "in other aspects of your life.")
    { }

    public override void Run()
    {
        StartMessage();

        List<string> prompts = new List<string>
        {
            "Think of a time when you stood up for someone else.",
            "Think of a time when you did something really difficult.",
            "Think of a time when you helped someone in need.",
            "Think of a time when you did something truly selfless."
        };

        List<string> questions = new List<string>
        {
            "Why was this experience meaningful to you?",
            "Have you done anything like this before?",
            "How did you get started?",
            "How did you feel when it was over?",
            "What made this time different than other times when you were not as successful?",
            "What is your favorite thing about this experience?",
            "What could you learn from this experience that applies to other situations?",
            "What did you learn about yourself through this experience?",
            "How can you keep this experience in mind in the future?"
        };

        Random rng = new Random();
        Console.WriteLine($"\n{prompts[rng.Next(prompts.Count)]}");

        List<string> remaining = new List<string>(questions);

        DateTime endTime = DateTime.Now.AddSeconds(Duration);
        while (DateTime.Now < endTime)
        {
            if (remaining.Count == 0)
                remaining = new List<string>(questions); // reinicia si se agotaron

            int index = rng.Next(remaining.Count);
            Console.WriteLine($"\n> {remaining[index]}");
            remaining.RemoveAt(index);

            ShowAnimation(5);
        }

        EndMessage();
    }
}

class Listing : Activity
{
    public Listing() : base(
        "Listing",
        "This activity will help you reflect on the good things in your life\n" +
        "by having you list as many things as you can in a certain area.")
    { }

    public override void Run()
    {
        StartMessage();

        List<string> prompts = new List<string>
        {
            "Who are people that you appreciate?",
            "What are your personal strengths?",
            "Who have you helped this week?",
            "Who are some of your personal heroes?"
        };

        Random rng = new Random();
        Console.WriteLine($"\n{prompts[rng.Next(prompts.Count)]}");
        Console.WriteLine("Take a moment to think about it...");
        ShowAnimation(5);

        Console.WriteLine("Now start listing items (press Enter after each one):");
        List<string> items = new List<string>();
        DateTime endTime = DateTime.Now.AddSeconds(Duration);

        while (DateTime.Now < endTime)
        {
            Console.Write("> ");
            string item = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(item))
                items.Add(item);
        }

        Console.WriteLine($"\nYou listed {items.Count} items!");
        EndMessage();
    }
}

class Program
{
    static Dictionary<string, int> activityCount = new Dictionary<string, int>
    {
        { "Breathing", 0 },
        { "Reflection", 0 },
        { "Listing", 0 }
    };

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\n--- Mindfulness App ---");
            Console.WriteLine("1. Breathing");
            Console.WriteLine("2. Reflection");
            Console.WriteLine("3. Listing");
            Console.WriteLine("4. View activity log"); 
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");

            string choice = Console.ReadLine()?.Trim();

            Activity activity = null;

            switch (choice)
            {
                case "1": activity = new Breathing(); break;
                case "2": activity = new Reflection(); break;
                case "3": activity = new Listing(); break;
                case "4":
                    ShowLog();
                    continue;
                case "5":
                    Console.WriteLine("Goodbye! Keep practicing mindfulness.");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please choose 1-5.");
                    continue;
            }

            activity.Run();
            activityCount[activity.Name]++;
        }
    }

    static void ShowLog()
    {
        Console.WriteLine("\n--- Activity Log ---");
        foreach (var entry in activityCount)
            Console.WriteLine($"{entry.Key}: {entry.Value} time(s)");
    }
}