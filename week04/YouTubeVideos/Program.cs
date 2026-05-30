using System;
using System;
using System.Collections.Generic;

class Comment
{
    public string Author { get; set; }
    public string Text { get; set; }

    public Comment(string author, string text)
    {
        Author = author;
        Text = text;
    }
}

class Video
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int DurationSeconds { get; set; }
    private List<Comment> _comments = new List<Comment>();

    public Video(string title, string author, int duration)
    {
        Title = title;
        Author = author;
        DurationSeconds = duration;
    }

    public void AddComment(string author, string text)
    {
        _comments.Add(new Comment(author, text));
    }

    public int GetNumberOfComments()
    {
        return _comments.Count;
    }

    public List<Comment> GetComments()
    {
        return _comments;
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Video> videos = new List<Video>();

        // Creating videos
        Video v1 = new Video("Learning C#", "CodeMaster", 300);
        v1.AddComment("Mary", "Excellent explanation.");
        v1.AddComment("Alex", "Very clear, thanks.");
        v1.AddComment("Sofia", "Helped me a lot with my homework.");
        videos.Add(v1);

        Video v2 = new Video("OOP Programming Guide", "DevTech", 450);
        v2.AddComment("Francisco", "Great content.");
        v2.AddComment("Annie", "Audio could be better.");
        v2.AddComment("Jhon", "Thanks for uploading this.");
        videos.Add(v2);

        Video v3 = new Video("Understanding Abstraction", "EduTech", 200);
        v3.AddComment("Erik", "Concept explained very well.");
        v3.AddComment("William", "Loved the examples.");
        v3.AddComment("Diego", "Keep it up!");
        videos.Add(v3);

        foreach (Video video in videos)
        {
            Console.WriteLine($"Title: {video.Title}");
            Console.WriteLine($"Author: {video.Author}");
            Console.WriteLine($"Duration: {video.DurationSeconds} seconds");
            Console.WriteLine($"Number of comments: {video.GetNumberOfComments()}");
            Console.WriteLine("Comments:");
            
            foreach (Comment c in video.GetComments())
            {
                Console.WriteLine($"- {c.Author}: {c.Text}");
            }
            Console.WriteLine(new string('-', 30));
        }
    }
}