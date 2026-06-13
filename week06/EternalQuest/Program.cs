using System;
using System.Collections.Generic;
using System.IO;

// --- Base Class ---
public abstract class Goal {
    protected string _name;
    protected string _description;
    protected int _points;

    public Goal(string name, string description, int points) {
        _name = name;
        _description = description;
        _points = points;
    }

    public abstract int RecordEvent();
    public abstract bool IsComplete();
    public abstract string GetDetails();
    public abstract int GetEarnedPoints();
    public abstract void ResetProgress();

    public virtual string Serialize() {
        return $"{GetType().Name}|{_name}|{_description}|{_points}";
    }

    public static Goal Deserialize(string line) {
        string[] parts = line.Split('|');
        if (parts.Length < 4) return null;

        string type = parts[0];
        string name = parts[1];
        string desc = parts[2];
        if (!int.TryParse(parts[3], out int points)) points = 0;

        switch (type) {
            case "SimpleGoal":
                bool complete = parts.Length > 4 && bool.TryParse(parts[4], out bool c) ? c : false;
                return new SimpleGoal(name, desc, points, complete);
            case "EternalGoal":
                int times = parts.Length > 4 && int.TryParse(parts[4], out int t) ? t : 0;
                return new EternalGoal(name, desc, points, times);
            case "ChecklistGoal":
                int target = parts.Length > 4 && int.TryParse(parts[4], out int tg) ? tg : 1;
                int current = parts.Length > 5 && int.TryParse(parts[5], out int cur) ? cur : 0;
                int bonus = parts.Length > 6 && int.TryParse(parts[6], out int b) ? b : 0;
                return new ChecklistGoal(name, desc, points, target, bonus, current);
            default:
                return null;
        }
    }
}

// --- Simple Goal ---
public class SimpleGoal : Goal {
    private bool _complete;

    public SimpleGoal(string name, string description, int points, bool complete = false)
        : base(name, description, points) {
        _complete = complete;
    }

    public override int RecordEvent() {
        if (!_complete) {
            _complete = true;
            return _points;
        }
        return 0;
    }

    public override bool IsComplete() => _complete;

    public override string GetDetails() {
        return $"{(_complete ? "[X]" : "[ ]")} {_name} ({_description})";
    }

    public override int GetEarnedPoints() {
        return _complete ? _points : 0;
    }

    public override void ResetProgress() {
        _complete = false;
    }

    public override string Serialize() {
        return base.Serialize() + $"|{_complete}";
    }
}

// --- Eternal Goal ---
public class EternalGoal : Goal {
    private int _timesRecorded;

    public EternalGoal(string name, string description, int points, int timesRecorded = 0)
        : base(name, description, points) {
        _timesRecorded = timesRecorded;
    }

    public override int RecordEvent() {
        _timesRecorded++;
        return _points;
    }

    public override bool IsComplete() => false;

    public override string GetDetails() {
        return $"[∞] {_name} ({_description}) - Recorded {_timesRecorded} times";
    }

    public override int GetEarnedPoints() {
        return _timesRecorded * _points;
    }

    public override void ResetProgress() {
        _timesRecorded = 0;
    }

    public override string Serialize() {
        return base.Serialize() + $"|{_timesRecorded}";
    }
}

// --- Checklist Goal ---
public class ChecklistGoal : Goal {
    private int _targetCount;
    private int _currentCount;
    private int _bonus;

    public ChecklistGoal(string name, string description, int points, int targetCount, int bonus, int currentCount = 0)
        : base(name, description, points) {
        _targetCount = Math.Max(1, targetCount);
        _bonus = Math.Max(0, bonus);
        _currentCount = Math.Max(0, currentCount);
    }

    public override int RecordEvent() {
        _currentCount++;
        if (_currentCount == _targetCount) {
            return _points + _bonus;
        }
        return _points;
    }

    public override bool IsComplete() => _currentCount >= _targetCount;

    public override string GetDetails() {
        return $"{(IsComplete() ? "[X]" : "[ ]")} {_name} ({_description}) - Completed {_currentCount}/{_targetCount}";
    }

    public override int GetEarnedPoints() {
        int earned = _currentCount * _points;
        if (_currentCount >= _targetCount) earned += _bonus;
        return earned;
    }

    public override void ResetProgress() {
        _currentCount = 0;
    }

    public override string Serialize() {
        return base.Serialize() + $"|{_targetCount}|{_currentCount}|{_bonus}";
    }
}

// --- Main Program ---
class Program {
    private static List<Goal> _goals = new List<Goal>();
    private static int _totalScore = 0;
    private const string saveFile = "goals.txt";

    static void Main(string[] args) {
        LoadGoals();

        bool running = true;
        while (running) {
            Console.WriteLine($"\n--- ETERNAL QUEST | Score: {_totalScore} ---");
            Console.WriteLine("1. Create new goal");
            Console.WriteLine("2. List goals");
            Console.WriteLine("3. Record event");
            Console.WriteLine("4. Delete a goal");
            Console.WriteLine("5. Reset goal progress");
            Console.WriteLine("6. Save and exit");
            Console.WriteLine("7. Reset all goals and score");


            string choice;
            do {
                Console.Write("Select an option (1-7): ");
                choice = Console.ReadLine()?.Trim();
                if (choice != "1" && choice != "2" && choice != "3" && choice != "4" && choice != "5" && choice != "6" && choice != "7") {
                    Console.WriteLine(" Invalid option. Please select 1, 2, 3, 4, 5, 6 or 7.");
                }
            } while (choice != "1" && choice != "2" && choice != "3" && choice != "4" && choice != "5" && choice != "6" && choice != "7");

            switch (choice) {
                case "1": CreateGoal(); break;
                case "2": DisplayGoals(); break;
                case "3": RecordGoalEvent(); break;
                case "4": DeleteGoal(); break;
                case "5": ResetGoalProgress(); break;
                case "6": SaveGoals(); running = false; break;
                case "7": ResetAll(); break;
            }
        }
    }

    private static void CreateGoal() {
        string type;
        do {
            Console.WriteLine("\nWhat type of goal do you want to create? (1. Simple, 2. Eternal, 3. Checklist)");
            type = Console.ReadLine()?.Trim();
            if (type != "1" && type != "2" && type != "3") {
                Console.WriteLine("⚠️ Invalid option. Please select 1, 2 or 3.");
            }
        } while (type != "1" && type != "2" && type != "3");

        Console.Write("Name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Description: ");
        string desc = Console.ReadLine() ?? "";

        int points = ReadNonNegativeInt("Points (integer): ");

        switch (type) {
            case "1":
                _goals.Add(new SimpleGoal(name, desc, points));
                break;
            case "2":
                _goals.Add(new EternalGoal(name, desc, points));
                break;
            case "3":
                int target = ReadPositiveInt("How many times must it be completed? ");
                int bonus = ReadNonNegativeInt("Bonus upon completion (integer): ");
                _goals.Add(new ChecklistGoal(name, desc, points, target, bonus));
                break;
        }

        Console.WriteLine("Goal created successfully.");
    }

    private static void DisplayGoals() {
        if (_goals.Count == 0) {
            Console.WriteLine("No goals yet.");
            return;
        }
        for (int i = 0; i < _goals.Count; i++) {
            Console.WriteLine($"{i + 1}. {_goals[i].GetDetails()}  (Earned: {_goals[i].GetEarnedPoints()} pts)");
        }
    }

    private static void RecordGoalEvent() {
        if (_goals.Count == 0) {
            Console.WriteLine("No goals to record. Create one first.");
            return;
        }

        DisplayGoals();

        int choice = ReadIndex("Select the goal number to record: ", _goals.Count);
        int earned = _goals[choice - 1].RecordEvent();
        _totalScore += earned;
        Console.WriteLine($"Event recorded! You earned {earned} points.");
    }

    private static void DeleteGoal() {
        if (_goals.Count == 0) {
            Console.WriteLine("No goals to delete.");
            return;
        }

        DisplayGoals();
        int choice = ReadIndex("Select the goal number to DELETE: ", _goals.Count);
        Goal g = _goals[choice - 1];
        int earned = g.GetEarnedPoints();

        Console.WriteLine($"This goal has contributed {earned} points to your total.");
        Console.Write("Subtract those points from total score when deleting? (y/n): ");
        string resp = Console.ReadLine()?.Trim().ToLower() ?? "n";
        if (resp == "y" || resp == "yes") {
            _totalScore -= earned;
            if (_totalScore < 0) _totalScore = 0;
            Console.WriteLine($"{earned} points subtracted from total score.");
        }

        _goals.RemoveAt(choice - 1);
        Console.WriteLine("Goal deleted successfully.");
    }
    private static void ResetAll() {
         _goals.Clear();        // elimina todas las metas
         _totalScore = 0;       // reinicia el puntaje
             Console.WriteLine("All goals and score have been reset to 0.");
        }

    private static void ResetGoalProgress() {
        if (_goals.Count == 0) {
            Console.WriteLine("No goals to reset.");
            return;
        }

        DisplayGoals();
        int choice = ReadIndex("Select the goal number to reset progress: ", _goals.Count);
        Goal g = _goals[choice - 1];
        int earned = g.GetEarnedPoints();

        Console.WriteLine($"Resetting will remove {earned} points previously earned from this goal.");
        Console.Write("Subtract those points from total score when resetting? (y/n): ");
        string resp = Console.ReadLine()?.Trim().ToLower() ?? "n";
        if (resp == "y" || resp == "yes") {
            _totalScore -= earned;
            if (_totalScore < 0) _totalScore = 0;
            Console.WriteLine($"{earned} points subtracted from total score.");
        }

        g.ResetProgress();
        Console.WriteLine("Goal progress has been reset.");
    }

    private static void SaveGoals() {
        try {
            using (StreamWriter sw = new StreamWriter(saveFile)) {
                sw.WriteLine(_totalScore);
                foreach (var g in _goals) {
                    sw.WriteLine(g.Serialize());
                }
            }
            Console.WriteLine("Goals saved.");
        } catch (Exception ex) {
            Console.WriteLine($"Error saving goals: {ex.Message}");
        }
    }

    private static void LoadGoals() {
        if (!File.Exists(saveFile)) return;
        try {
            string[] lines = File.ReadAllLines(saveFile);
            if (lines.Length == 0) return;

            if (!int.TryParse(lines[0], out _totalScore)) _totalScore = 0;

            for (int i = 1; i < lines.Length; i++) {
                Goal g = Goal.Deserialize(lines[i]);
                if (g != null) _goals.Add(g);
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error loading goals: {ex.Message}");
            _goals.Clear();
            _totalScore = 0;
        }
    }

    // --- Helper input methods ---
    private static int ReadNonNegativeInt(string prompt) {
        int value;
        while (true) {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out value) && value >= 0) return value;
            Console.WriteLine("⚠️ Invalid number. Enter a non-negative integer.");
        }
    }

    private static int ReadPositiveInt(string prompt) {
        int value;
        while (true) {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out value) && value > 0) return value;
            Console.WriteLine("⚠️ Invalid number. Enter a positive integer.");
        }
    }

    private static int ReadIndex(string prompt, int max) {
        int idx;
        while (true) {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out idx) && idx >= 1 && idx <= max) return idx;
            Console.WriteLine($"⚠️ Invalid number. Enter a number between 1 and {max}.");
        }
    }
}

/*
Notes:
- Menu options are validated and consistent.
- Delete and Reset options allow optional subtraction of previously earned points.
- All numeric inputs are validated.
- Save/load persists total score and each goal's internal state.
*/
