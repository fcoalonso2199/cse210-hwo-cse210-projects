using System;
using System.Collections.Generic;
using System.Linq;

public class Scripture
{
    private Reference _reference;
    private List<Word> _words;

    public Scripture(Reference reference, string text)
    {
        _reference = reference;
        _words = new List<Word>();
        string[] splitWords = text.Split(' ');
        foreach (var word in splitWords)
        {
            _words.Add(new Word(word));
        }
    }

    public void HideRandomWords(int numberToHide)
    {
        Random random = new Random();
        
        // Advanced Requirement: Filter to select ONLY words that are not already hidden
        List<Word> visibleWords = _words.Where(w => !w.IsHidden()).ToList();

        int wordsToHide = Math.Min(numberToHide, visibleWords.Count);

        for (int i = 0; i < wordsToHide; i++)
        {
            int index = random.Next(visibleWords.Count);
            visibleWords[index].Hide();
            visibleWords.RemoveAt(index); 
        }
    }

    public string GetDisplayText()
    {
        List<string> textWords = new List<string>();
        foreach (var word in _words)
        {
            textWords.Add(word.GetDisplayText());
        }
        return $"{_reference.GetDisplayText()} - {string.Join(" ", textWords)}";
    }

    public bool IsCompletelyHidden()
    {
        return _words.All(w => w.IsHidden());
    }
}