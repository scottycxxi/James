using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
//some comments cuz idk what else to add honestly

namespace James
{

    internal class Verse
    {
        private readonly string fullVerse;
        private readonly string lightVerse;
        private readonly string bookName;

        private int chapterNum;
        private int verseNum;

        private Verse prev = null;
        private Verse next = null;

        public Verse(int chapterNum, int verseNum, string fullVerse, string bookName)
        {
            this.chapterNum = chapterNum;
            this.verseNum = verseNum;
            this.fullVerse = fullVerse;
            this.bookName = bookName;
            lightVerse = fullVerse.Replace(".", "")
                                  .Replace(",", "")
                                  .Replace(":", "")
                                  .Replace(";", "")
                                  .Replace("'", "")
                                  .Replace("\"", "")
                                  .Replace("--", " ")
                                  .Replace("-", "")
                                  .Replace("?", "")
                                  .Replace("!", "")
                                  .Replace("(", "")
                                  .Replace(")", "")
                                  .ToLower()
                                  .Trim();
            //todo have some other check for other punctuation
        }

        #region Properties
        public string FullVerse { get { return fullVerse; } }

        public string LightVerse { get { return lightVerse; } }

        public int ChapterNum { get { return chapterNum; } }

        public int VerseNum { get { return verseNum; } }

        public Verse Prev { get { return prev; } set { prev = value; } }

        public Verse Next { get { return next; } set { next = value; } }
        #endregion

        #region Public Methods
        public string GetPrevVerse(string bookName)
        {
            if (prev == null)
            {
                return "Start of " + bookName;
            }
            else if (prev.chapterNum != chapterNum)
            {
                return "Start of chapter " + chapterNum;
            }
            else
            {
                //todo why is this possible??? return prev.fullVerse;
                return $"{bookName} {prev.ChapterNum}:{prev.VerseNum} {prev.FullVerse}";
            }
        }

        public string GetNextVerse(string bookName)
        {
            if (next == null)
            {
                return "End of " + bookName;
            }
            else if (next.chapterNum != chapterNum)
            {
                return "End of chapter " + chapterNum;
            }
            else
            {
                return $"{bookName} {next.ChapterNum}:{next.VerseNum} {next.FullVerse}";
            }
        }

        public void GetQuizVerse(Random randy,
                                 GameMode gameMode,
                                 bool hardMode,
                                 out string[] quizWords,
                                 out bool[] quizzedWords,
                                 out int quizWordCount)
        {
            quizWords = lightVerse.Split(' ');
            quizzedWords = new bool[quizWords.Length];
            quizWordCount = 0;
            for (int i = 0; i < quizWords.Length; i++)
            {
                if (randy.Next(0, 99) >= (int) gameMode)
                {
                    //if the random number is higher than the game mode assistance, replace it with blank(s)
                    int quizWordLength = hardMode ? 1 : quizWords[i].Length;
                    quizWords[i] = "_".PadRight(quizWordLength, '_');
                    quizzedWords[i] = true;
                    quizWordCount++;
                }
            }
        }
        #endregion

        #region Overrides
        public int compareTo(Verse other)
        {
            if (this.chapterNum == other.chapterNum)
            {
                return this.verseNum - other.verseNum;
            }
            else
            {
                return this.chapterNum - other.chapterNum;
            }
        }

        public override string ToString()
        {
            return string.Concat(chapterNum, ":", verseNum, " ", FullVerse);
        }
        #endregion
    }
}
