using System;
using System.Text;
using System.Collections.Generic;

class Hangman
{
    static string hangman;
    static char guess;
    static List<char> guessList;
    static List<int> indices;
    static bool win;
    static bool playing;

    static void Main()
    {
        playing = true;

        do
        {
            //
            // Game Setup
            //

            Console.Write("Let's play Hangman. Enter a word - ");
            hangman = GetHiddenConsoleInput();

            //check whether all characters are alphabetic; if not then try again
            while (!IsAlphabetic(hangman))
            {
                Console.WriteLine("\nYou entered '{0}'.", hangman);
                Console.WriteLine("Try typing a word with only letters - ");
                hangman = GetHiddenConsoleInput();
            }

            //change case since 'InPuT' can be mixed but should be 'input'
            hangman = hangman.ToLower();

            //length is ***
            int l = hangman.Length;
            StringBuilder display = new StringBuilder(Asteriskize(hangman));
            Console.WriteLine("\n\nWord length is: {0}, {1}", l, display);

            //
            // Guess Code
            //

            Console.WriteLine("\nTime to start guessing!");
            guessList = new List<char>();
            int guessCount = 0;
            DrawHangman(guessCount);

            while (display.ToString().Contains(@"*"))
            {
                do { guess = GetGuess(); }  //prompt for single alphabetic input
                while (InGuessList(guess)); //check whether it's already been guessed

                //case-insensitive
                StringComparison comp = StringComparison.OrdinalIgnoreCase;

                indices = new List<int>();

                if (hangman.Contains(guess, comp))
                {
                    int spot = hangman.IndexOf(guess);
                    indices.Add(hangman.IndexOf(guess));
                    maybeMore(spot + 1);

                    if (indices.Count > 1)
                    {
                        Console.Write("\nThere are {0} '{1}'s! - ", indices.Count, guess);
                    }
                    else
                    {
                        Console.Write("\nThere is 1 '{0}'! - ", guess);
                    }
                    foreach (int i in indices)
                    {
                        display[i] = guess;
                    }
                    Console.WriteLine(display + "\n");
                }
                else
                {
                    Console.Write("\nSorry, there are no {0}'s. ", guess);
                    guessCount++;
                    DrawHangman(guessCount);

                    if (guessCount != 6)
                    {
                        Console.WriteLine("Try again - {0} \n", display);
                    }
                    else
                    {
                        EndGame();
                    }
                }
            }
            win = true;
            EndGame();
        } while (playing);
    } //main

    static string GetHiddenConsoleInput()
    {
        StringBuilder input = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter) break;
            if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
            else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
        }
        return input.ToString();
    } //hidden input

    static char GetGuess()
    {
        string guessInput;
        bool isSingleChar;
        bool isAlphabetic = false;
        char c;

        do
        {
            Console.Write("Guess ONE (1) LETTER - ");
            guessInput = Console.ReadLine();
            isSingleChar = char.TryParse(guessInput, out c);
            if (isSingleChar)
            {
                isAlphabetic = IsAlphabetic(guessInput);
            }
        }
        while (!isSingleChar || !isAlphabetic);

        return c;
    } //get a valid [alphabetic char] guess

    static bool InGuessList(char g)
    {
        if (!guessList.Contains(g))
        {
            guessList.Add(g);
            return false;
        }
        else
        {
            Console.WriteLine("\nYou already guessed '{0}'!", g);
            return true;
        }
    } //

    static void EndGame()
    {
        if (win)
        {
            Console.WriteLine("Great job! You guessed '{0}' correctly.", hangman.ToUpper());
        } else
        {
            Console.WriteLine("Game over! The word was '{0}', by the way.", hangman.ToUpper());
        }

        ResetOrQuit();
    } //end game message, choose to reset or quit

    static void ResetOrQuit()
    {
        Console.Write("Press any key except (ESC) to play again - ");
        if (Console.ReadKey().Key != ConsoleKey.Escape)
        {
            hangman = "";
            win = false;
        }
        else
        {
            playing = false;
        }
        Console.WriteLine("\n");
    } //check to play again

    static void DrawHangman(int c)
    {
        string legLine, armLine, headLine;

        if (c == 6)
        {
            legLine = " / \\    |"; //both legs
        }
        else if (c == 5)
        {
            legLine = " /      |"; //one leg
        }
        else
        {
            legLine = "        |"; //default
        }

        if (c >= 4)
        {
            armLine = " /|\\    |"; //both arms + body
        }
        else if (c == 3)
        {
            armLine = " /|     |"; //left arm + body
        }
        else if (c == 2)
        {
            armLine = "  |     |"; //body only
        }
        else
        {
            armLine = "        |"; //default
        }

        if (c >= 1)
        {
            headLine = "  0    +|"; //head
        }
        else
        {
            headLine = "       +|"; //default
        }

        string top = "\n ________"; //top of structure
        string bottom = "        |\n=========="; //bottom of structure

        Console.WriteLine(top);
        Console.WriteLine(headLine);
        Console.WriteLine(armLine);
        Console.WriteLine(legLine);
        Console.WriteLine(bottom);
    } //draw hangman ascii art

    static bool IsAlphabetic(string s)
    {
        foreach (char c in s)
        {
            if (!(c >= 'A' && c <= 'Z') && !(c >= 'a' && c <= 'z'))
            {
                return false;
            }
        }
        return true;
    } //validate string as alphabetic

    static bool IsAlphabetic(char c)
    {
        if (!(c >= 'A' && c <= 'Z') && !(c >= 'a' && c <= 'z'))
        {
            return false;
        }

        return true;
    } //validate char as alphabetic


    static string Asteriskize(string s)
    {
        int l = s.Length;
        string asterisked = "";

        for (int i = 0; i < l; i++)
        {
            asterisked += @"*";
        }

        return asterisked;
    } //hide input string

    static List<int> maybeMore(int start)
    {
        string r = hangman.Substring(start);
        if (r.Contains(guess))
        {
            int newSpot = start + r.IndexOf(guess);
            indices.Add(newSpot);
            return maybeMore(newSpot + 1);
        }
        else
        {
            return indices;
        }
    } //check for further instances of the guess
} //class Hangman