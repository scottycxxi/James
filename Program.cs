/*For next time
Verse.GetQuizVerse based on game mode
implement hard mode
count guesses and answers better based on game mode (shouldn't score for typing a given word) 

make a readme, cuz it's good to

long term
store scores long term
keep score on rounds with no help

game modes
1 - in order, prior verse context, 100% help
2 - in order, prior verse context, 60% help
3 - in order, prior verse context, 30% help
4 - in order, prior verse context, 0% help, (no word lengths)
5 - type the whole thing, hit enter after each verse, ball out
6 - random, prior and following verse context, 60% help
7 - random, prior and following verse context, 30% help
8 - random, prior and following verse context, 0% help (no word lengths)
9 - random, no context, ball out
*/
namespace James
{
    internal class Program
    {
        const string quitCode = "qq";
        const int allChapters = 0;
        const string bookName = "James";
        const string pathFolder = "Scripture";
        const string pathFileName = "James.txt";
        const string chapterLineStarter = "Chapter";
        const int verseScoreMax = 3;

        static void Main(string[] args)
        {
            List<Verse> book = ReadFile(GetRootDirectory(), out string bookName);
            if (book == null)
                throw new Exception("Book failed to load.");

            GetGameSetup(bookName,
                         out GameMode gameMode,
                         out bool includeContext,
                         out int whichChapter,
                         out bool wantsToQuit);
            if (wantsToQuit)
                return;

            List<Verse> versesToTest = GetVersesToTest(book, whichChapter);
            TestAllVerses(versesToTest, gameMode, includeContext, false, whichChapter);
        }

        #region Private Methods
        private static string GetRootDirectory()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            while (!currentDirectory.Substring(currentDirectory.Length - 5).Equals(bookName))
            {
                currentDirectory = Directory.GetParent(currentDirectory).FullName;
            }
            return currentDirectory;
        }

        private static List<Verse> ReadFile(string root, out string bookName)
        {
            List<Verse> book = new List<Verse>();
            //try
            //{
            using (StreamReader sr = new StreamReader(Path.Combine(root, pathFolder, pathFileName)))
            {
                bookName = sr.ReadLine(); //name of book
                int chapterNum = 0;
                string line = "";
                Verse prev = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.IndexOf(chapterLineStarter) == 0)
                    {
                        chapterNum = int.Parse(line.Substring(line.IndexOf(' ')).Trim());
                    }
                    else
                    {
                        int spaceIndex = line.IndexOf(' ');
                        int verseNum = int.Parse(line.Substring(0, spaceIndex));
                        Verse verse = new(chapterNum, verseNum, line.Substring(spaceIndex + 1).Trim(), bookName);
                        verse.Prev = prev;
                        if (prev != null)
                            prev.Next = verse;
                        book.Add(verse);
                        prev = verse;
                    }
                }
            }
            return book;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception($"An error occurred: {ex.Message}");

            //}
        }

        private static void GetGameSetup(string bookName,
                                        out GameMode gameMode,
                                        out bool includeContext,
                                        out int whichChapter,
                                        out bool wantsToQuit)
        {
            Console.WriteLine("Welcome to memorizing " + bookName);
            Console.WriteLine("Game Modes: 1 - 100% | 2 - 75% | 3 - 50% | 4 - 25% | 5 - 0%");
            Console.Write("What would you like to play? ");
            string gameModeInput = GetValidInput("1|2|3|4|5");
            if (gameModeInput.Equals(quitCode, StringComparison.OrdinalIgnoreCase))
            {
                gameMode = GameMode.gm100;
                includeContext = false;
                whichChapter = 0;
                wantsToQuit = true;
                return;
            }

            //todo hardmode

            Console.WriteLine("Context: 1 - Yes | 2 - No");
            Console.Write("Would you like to include context? ");
            string includeContextInput = GetValidInput("1|2");
            if (gameModeInput.Equals(quitCode, StringComparison.OrdinalIgnoreCase))
            {
                gameMode = GameMode.gm100;
                includeContext = false;
                whichChapter = 0;
                wantsToQuit = true;
                return;
            }

            Console.WriteLine("Chapters: 0 - All | 1 | 2 | 3 | 4 | 5");
            //todo make this more flexible
            Console.Write("Which chapter would you like to work on (or all)? ");
            string whichChapterInput = GetValidInput("1|2|3|4|5");
            if (whichChapterInput.Equals(quitCode, StringComparison.OrdinalIgnoreCase))
            {
                gameMode = GameMode.gm100;
                includeContext = false;
                whichChapter = 0;
                wantsToQuit = true;
                return;
            }

            gameMode = gameModeInput switch
            {
                "1" => GameMode.gm100,
                "2" => GameMode.gm75,
                "3" => GameMode.gm50,
                "4" => GameMode.gm25,
                "5" => GameMode.gm0,
                _ => throw new Exception("Game mode input " + gameModeInput + " not defined")
            };
            includeContext = includeContextInput.Equals("1");
            whichChapter = int.Parse(whichChapterInput);
            wantsToQuit = false;
        }

        private static string GetValidInput(string options)
        {
            List<string> eachOption = new List<string>();
            foreach (string option in options.Split('|'))
            {
                eachOption.Add(option);
            }
            eachOption.Add(quitCode);

            string s = "";
            while (s.Length == 0)
            {
                s = Console.ReadLine();
                if (!eachOption.Contains(s))
                {
                    Console.Write($"Please enter a valid input or '{quitCode}' to quit: ");
                    s = "";
                }
            }
            return s;
        }

        private static List<Verse> GetVersesToTest(List<Verse> book, int whichChapter)
        {
            List<Verse> versesToTest = new List<Verse>();
            foreach (Verse verse in book)
            {
                if (whichChapter == allChapters || whichChapter == verse.ChapterNum)
                    versesToTest.Add(verse);
            }
            Shuffle(versesToTest);
            return versesToTest;
        }

        private static void Shuffle(List<Verse> versesToTest)
        {
            Random randy = new();
            for (int s = 0; s < versesToTest.Count - 1; s++)
            {
                //generate a random index
                int idxToSwap = randy.Next(s, versesToTest.Count); // Generate a random index
                //swap values
                Verse temp = versesToTest[s];
                versesToTest[s] = versesToTest[idxToSwap];
                versesToTest[idxToSwap] = temp;
            }
        }

        private static void TestAllVerses(List<Verse> versesToTest,
                                          GameMode gameMode,
                                          bool includeContext,
                                          bool hardMode,
                                          int whichChapter)
        {
            int currentVerseCount = 0;
            int totalVerseCount = versesToTest.Count;
            Random randy = new();

            int verseScoreTotal = 0;
            int emergencyFixNumber = 5;
            int correctGuessesTotal = 0;
            int correctAnswersTotal = 0;
            foreach (Verse verse in versesToTest)
            {
                Console.WriteLine();
                Console.WriteLine($"Round {currentVerseCount + 1} of {totalVerseCount} - {bookName} {verse.ChapterNum}:{verse.VerseNum}");
                if (includeContext)
                    Console.WriteLine(verse.GetPrevVerse(bookName));
                verse.GetQuizVerse(randy, gameMode, hardMode,
                                   out string[] quizWords, out bool[] quizzedWords, out int quizWordCount);
                Console.WriteLine($"{bookName} {verse.ChapterNum}:{verse.VerseNum} {FormatArrayWords(quizWords)}");
                if (includeContext)
                    Console.WriteLine(verse.GetNextVerse(bookName));
                Console.WriteLine();
                Console.Write("--> ");
                string guess = Console.ReadLine();
                if (guess.Equals(quitCode, StringComparison.OrdinalIgnoreCase))
                    break;
                CheckLine(guess,
                          verse,
                          quizzedWords,
                          quizWordCount,
                          out int verseScore,
                          out int correctGuesses);
                if (verseScore == -1)
                {
                    break;
                }
                else
                {
                    currentVerseCount++;
                    verseScoreTotal += verseScore;
                    correctGuessesTotal += correctGuesses;
                    correctAnswersTotal += quizWordCount;
                }
            }
            WrapUpRound(currentVerseCount,
                        totalVerseCount,
                        verseScoreTotal,
                        correctGuessesTotal,
                        correctAnswersTotal);
        }

        private static void CheckLine(string guess,
                                      Verse verse,
                                      bool[] quizzedWords,
                                      int quizWordCount,
                                      out int verseScore,
                                      out int correctGuesses)
        {
            string lightVerse = verse.LightVerse;
            if (lightVerse.Equals(guess, StringComparison.OrdinalIgnoreCase))
            {
                string[] guessWords = guess.Split(' ');
                Console.WriteLine($"Score: {verseScoreMax}/{verseScoreMax}, Words: {quizWordCount}/{quizWordCount}");
                verseScore = verseScoreMax;
                correctGuesses = quizWordCount;
            }
            else
            {
                CountAndPointOutMistakes(guess,
                                         lightVerse,
                                         quizzedWords,
                                         out correctGuesses,
                                         out string formattedGuess,
                                         out string formattedVerse,
                                         out string formattedMistakes);
                Console.WriteLine("+".PadRight(20, '-') + "+");
                Console.WriteLine("Correct: " + formattedVerse);
                Console.WriteLine("  Guess: " + formattedGuess);
                Console.WriteLine("         " + formattedMistakes);

                //shouldn't happen
                if (formattedGuess.Length != formattedVerse.Length
                    || formattedVerse.Length != formattedMistakes.Length)
                {
                    Console.WriteLine("Whoops, those aren't all the same length...");
                    Console.WriteLine("Guess: " + guess);
                    Console.WriteLine("Lightverse: " + lightVerse);
                }

                Console.WriteLine("Score: 3 - Perfect | 2 - Really good | 1 - Getting there | 0 - Needs work");
                Console.Write("How would you score that? ");
                string verseScoreInput = GetValidInput("1|2|3|0");
                if (verseScoreInput.Equals(quitCode, StringComparison.OrdinalIgnoreCase))
                {
                    verseScore = -1;
                }
                else
                {
                    Console.WriteLine("+".PadRight(20, '-') + "+");
                    Console.WriteLine($"Score: {verseScoreInput}/{verseScoreMax}, Words: {correctGuesses}/{quizWordCount}");
                    verseScore = int.Parse(verseScoreInput);
                }
            }
        }

        private static void CountAndPointOutMistakes(string guess,
                                                     string lightVerse,
                                                     bool[] quizzedWords,
                                                     out int correctGuesses,
                                                     out string formattedGuess,
                                                     out string formattedVerse,
                                                     out string formattedMistakes)
        {
            string[] guessWords = guess.Split(' ');
            string[] verseWords = lightVerse.Split(' ');
            string[] mistakeWords = new string[Math.Max(guessWords.Length, verseWords.Length)];
            correctGuesses = 0;
            for (int i = 0; i < guessWords.Length; i++)
            {
                string guessWord = guessWords[i];
                string verseWord = "";
                if (verseWords.Length >= i)
                {
                    verseWord = verseWords[i];
                }
                int wordLength = Math.Max(guessWord.Length, verseWord.Length);

                if (guessWord.Equals(verseWord, StringComparison.OrdinalIgnoreCase))
                {
                    mistakeWords[i] = "".PadRight(wordLength);
                    correctGuesses += (bool)quizzedWords?[i] ? 1 : 0;
                    //todo maybe bad vibes
                }
                else
                {
                    mistakeWords[i] = "".PadRight(wordLength, 'x');
                    guessWords[i] = guessWord.PadRight(wordLength);
                    verseWords[i] = verseWord.PadRight(wordLength);
                }
            }
            if (verseWords.Length > guessWords.Length)
            {
                for (int i = guessWords.Length; i < verseWords.Length; i++)
                {
                    mistakeWords[i] = "".PadRight(verseWords[i].Length, 'x');
                }
            }
            formattedGuess = FormatArrayWords(guessWords);
            formattedVerse = FormatArrayWords(verseWords);
            formattedMistakes = FormatArrayWords(mistakeWords);
        }

        private static string FormatArrayWords(string[] words)
        {
            if (words.Length == 0)
                return "";
            string formattedWords = words[0];
            for (int i = 1; i < words.Length; i++)
            {
                formattedWords += " " + words[i];
            }
            return formattedWords;
        }

        private static void WrapUpRound(int currentVerseCount,
                                        int totalVerseCount,
                                        int verseScoreTotal,
                                        int correctGuessesTotal,
                                        int correctAnswersTotal)
        {
            Console.WriteLine($"Verses tested: {currentVerseCount}/{totalVerseCount}");
            Console.WriteLine($"Overall score: {verseScoreTotal}/{currentVerseCount * verseScoreMax}");
            Console.WriteLine($"Words guessed: {correctGuessesTotal}/{correctAnswersTotal}");
            Console.WriteLine("Nice work!");
            //write to file somehow??
        }
        #endregion
    }
}
