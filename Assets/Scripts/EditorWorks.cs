using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using NUnit.Framework.Interfaces;

public class EditorWorks : MonoBehaviour
{
    public WordsLibrary russianWords;

    [ContextMenu ("From txt to asset")]
    public void MakeWorks()
    {
        List<string> words = new List<string>();
        Dictionary<string, string> wordsDict = new Dictionary<string, string>();
        string path = @"D:\words2.txt";
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            StreamReader sr = new StreamReader(fs);

            while (!sr.EndOfStream)
            {
                string word = sr.ReadLine().ToUpper();
                //words.Add(sr.ReadLine());
                if (!wordsDict.ContainsKey(word))
                {
                    wordsDict.Add(word, word);
                }

            }
            sr.Close();
        }
        words = wordsDict.Values.ToList();
        words.Sort();
        russianWords.words = words.ToArray();
    }

    [ContextMenu("Library Analisys")]
    public void LibraryAnalisys()
    {
        string longestWord = "";
        for (int i = 0; i < russianWords.words.Length; i++)
        {
            if (russianWords.words[i].Length > longestWord.Length)
            {
                longestWord = russianWords.words[i];
            }
        }
        Debug.Log("Longest word: "  + longestWord + " - " + longestWord.Length + " characters");
    }
}
