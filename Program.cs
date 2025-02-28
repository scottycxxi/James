/*For next time
count guesses and answers better based on game mode (shouldn't score for typing a given word) 
make input arrow as long as verse name length so James 1:20 and ------> end at the same spot

take out 3/3 scoring altogether

long term
store scores long term
keep score on rounds with no help
save progress mid run
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

        static void Main(string[] args)
        {
            List<Verse> book = ReadFile(GetRootDirectory(), out string bookName);
            if (book == null)
                throw new Exception("Book failed to load.");

            GetGameSetup(bookName,
                         out GameMode gameMode,
                         out int whichChapter,
                         out bool wantsToQuit);
            if (wantsToQuit)
                return;

            List<Verse> versesToTest = GetVersesToTest(book, whichChapter, gameMode);
            List<Verse> completedVerses = new();
            bool retestMissedVerses = true;
            while (versesToTest.Count > 0 && retestMissedVerses)
            {
                TestAllVerses(ref versesToTest, ref completedVerses, gameMode, whichChapter, out retestMissedVerses);
                gameMode.ShowContext = versesToTest.Count > 0;
                //always show context on retesting rounds
                //todo, not on hardcore random mode
            }
        }

        #region Private Methods - Game Setup
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
                                        out int whichChapter,
                                        out bool wantsToQuit)
        {
            Console.WriteLine("Welcome to memorizing " + bookName);
            Console.WriteLine("Type \"" + quitCode + "\" to quit at any time");
            Console.WriteLine();
            Console.WriteLine("Chapters: 0 - All | 1 | 2 | 3 | 4 | 5 | 6");
            //todo make this more flexible for other books and zero
            Console.Write("Which chapter would you like to work on (or all)? ");
            string whichChapterInput = GetValidInput("1|2|3|4|5|6");
            if (whichChapterInput.Equals(quitCode, StringComparison.OrdinalIgnoreCase))
            {
                gameMode = new GameMode(GameModeType.ordered100Help); //as a default todo nullable??
                whichChapter = 0;
                wantsToQuit = true;
                return;
            }

            Console.WriteLine();
            Console.WriteLine("---Game Modes---");
            Console.WriteLine("In order");
            Console.WriteLine("    1 - 100% help");
            Console.WriteLine("    2 - 60% help");
            Console.WriteLine("    3 - 30% help");
            Console.WriteLine("    4 - 0% help");
            Console.WriteLine("Random order with context:");
            Console.WriteLine("    5 - 60% help");
            Console.WriteLine("    6 - 30% help");
            Console.WriteLine("    7 - 0% help");
            Console.WriteLine("Full run, no context, real deal:");
            Console.WriteLine("    8 - In order");
            Console.WriteLine("    9 - Random");
            Console.Write("What would you like to play? ");
            string gameModeInput = GetValidInput("1|2|3|4|5|6|7|8|9");
            if (gameModeInput.Equals(quitCode, StringComparison.OrdinalIgnoreCase))
            {
                gameMode = new GameMode(GameModeType.ordered100Help); //as a default todo nullable??
                whichChapter = 0;
                wantsToQuit = true;
                return;
            }

            GameModeType gameModeType = gameModeInput switch
            {
                "1" => GameModeType.ordered100Help,
                "2" => GameModeType.ordered60Help,
                "3" => GameModeType.ordered30Help,
                "4" => GameModeType.ordered0Help,
                "5" => GameModeType.random60Help,
                "6" => GameModeType.random30Help,
                "7" => GameModeType.random0Help,
                "8" => GameModeType.orderedNoContext,
                "9" => GameModeType.randomNoContext,
                _ => throw new Exception("Game mode input " + gameModeInput + " not defined")
            };
            gameMode = new GameMode(gameModeType);
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

        private static List<Verse> GetVersesToTest(List<Verse> book, int whichChapter, GameMode gameMode)
        {
            List<Verse> versesToTest = new List<Verse>();
            foreach (Verse verse in book)
            {
                if (whichChapter == allChapters || whichChapter == verse.ChapterNum)
                    versesToTest.Add(verse);
            }
            if (!gameMode.InOrder)
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
        #endregion

        #region Private Methods - Gameplay
        private static void TestAllVerses(ref List<Verse> versesToTest,
                                          ref List<Verse> completedVerses,
                                          GameMode gameMode,
                                          int whichChapter,
                                          out bool retestMissedVerses)
        {
            int testedVerseCount = 0;
            int correctVerseCount = 0;
            Random randy = new();
            completedVerses = new();

            int correctGuessesTotal = 0;
            int correctAnswersTotal = 0;
            foreach (Verse verse in versesToTest)
            {
                //before the verse
                Console.WriteLine();
                Console.WriteLine($"Round {testedVerseCount + 1} of {versesToTest.Count} - {bookName} {verse.ChapterNum}:{verse.VerseNum}");
                if (gameMode.ShowContext)
                    Console.WriteLine(verse.GetPrevVerse(bookName));
                //the verse
                verse.GetQuizVerse(randy, gameMode,
                                   out string[] quizWords, out bool[] quizzedWords, out int quizWordCount);
                string reference = verse.GetReference();
                Console.WriteLine($"{reference} {FormatArrayWords(quizWords)}");
                //after the verse
                //if (gameMode.ShowContext)
                //{
                //    Console.WriteLine(verse.GetNextVerse(bookName));
                //    Console.WriteLine();
                //}

                //getting the verse
                Console.Write($"-".PadRight(reference.Length - 1, '-') + "> ");
                string guess = Console.ReadLine();
                if (guess.Equals(quitCode, StringComparison.OrdinalIgnoreCase))
                    break;
                //scoring the verse
                guess = Verse.CleanPunctuation(guess);
                int correctGuesses = 0;
                if (gameMode.PercentHelp < 100)
                {
                    CheckLine(guess,
                              verse,
                              quizzedWords,
                              quizWordCount,
                              out correctGuesses);
                }

                testedVerseCount++;
                correctGuessesTotal += correctGuesses;
                correctAnswersTotal += quizWordCount;
                if (correctGuesses < quizWordCount)
                {
                    verse.Attempts++;
                }
                else
                {
                    verse.Completed = true;
                    correctVerseCount++;
                }
            }

            WrapUpRound(correctVerseCount,
                        testedVerseCount,
                        correctGuessesTotal,
                        correctAnswersTotal,
                        out retestMissedVerses);
            for (int i = versesToTest.Count - 1; i >= 0; i--)
            {
                Verse temp = versesToTest[i];
                if (temp.Completed)
                {
                    versesToTest.RemoveAt(i);
                    completedVerses.Add(temp);
                }
            }
        }

        private static void CheckLine(string guess,
                                      Verse verse,
                                      bool[] quizzedWords,
                                      int quizWordCount,
                                      out int correctGuesses)
        {
            string lightVerse = verse.LightVerse;
            if (lightVerse.Equals(guess, StringComparison.OrdinalIgnoreCase))
            {
                string[] guessWords = guess.Split(' ');
                Console.WriteLine($"Words: {quizWordCount}/{quizWordCount}");
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
                Console.WriteLine("+".PadRight(20, '-') + ">");
                Console.WriteLine("|Correct: " + formattedVerse);
                Console.WriteLine("|  Guess: " + formattedGuess);
                Console.WriteLine("|         " + formattedMistakes);
                Console.WriteLine($"|Words: {correctGuesses}/{quizWordCount}");
                Console.WriteLine("+".PadRight(20, '-') + ">");
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
            guess = Verse.CleanMultipleSpaces(guess);
            string[] guessWords = guess.ToLower().Split(' ');
            string[] verseWords = lightVerse.ToLower().Split(' ');
            string[] mistakeWords = new string[Math.Max(guessWords.Length, verseWords.Length)];
            correctGuesses = 0;
            for (int i = 0; i < guessWords.Length; i++)
            {
                string guessWord = guessWords[i];
                string verseWord = "";
                if (verseWords.Length > i)
                {
                    verseWord = verseWords[i];
                }
                int wordLength = Math.Max(guessWord.Length, verseWord.Length);

                //only gives points for correct words if quizzed and same, only show mistakes for wrong words if quizzed
                if (guessWord.Equals(verseWord, StringComparison.OrdinalIgnoreCase))
                {
                    mistakeWords[i] = "".PadRight(wordLength);
                    correctGuesses += (bool)quizzedWords[i] ? 1 : 0;
                }
                else
                {
                    char padChar = (bool)quizzedWords[i] ? 'x' : ' ';
                    mistakeWords[i] = "".PadRight(wordLength, padChar);
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

        private static void WrapUpRound(int correctVerseCount,
                                        int testedVerseCount,
                                        int correctGuessesTotal,
                                        int correctAnswersTotal,
                                        out bool retestMissedVerses)
        {
            //todo chapters tested?
            Console.WriteLine();
            Console.WriteLine($"Verses perfect: {correctVerseCount}/{testedVerseCount}");
            Console.WriteLine($"Words correct: {correctGuessesTotal}/{correctAnswersTotal}");
            Console.WriteLine("Nice work!");
            retestMissedVerses = false;
            if (correctVerseCount < testedVerseCount)
            {
                Console.WriteLine("Would you like to retest the verses you missed? (y/n): ");
                string retest = GetValidInput("y|n");
                retestMissedVerses = retest.Equals("y", StringComparison.OrdinalIgnoreCase);
                if (retestMissedVerses)
                    Console.Clear();
            }
            //todo write to file somehow??
        }
        #endregion
    }
}
