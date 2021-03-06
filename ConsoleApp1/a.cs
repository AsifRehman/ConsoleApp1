﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class a
    {
        public async Task<object> Invoke(dynamic i)
        {    // async here is required to be used by Edge.JS that is a node.js module enable communicating with C# files
            var tcs = new TaskCompletionSource<object>();
            // Initialize a new instance of the SpeechSynthesizer.
            SpeechSynthesizer synth = new SpeechSynthesizer();

            // Configure the audio output. 
            synth.SetOutputToDefaultAudioDevice();

            // Speak a string.
            synth.Speak("This example demonstrates a basic use of Speech Synthesizer");

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            // Create a new SpeechRecognitionEngine instance.

            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();

            recognizer.SetInputToDefaultAudioDevice();

            // Create a simple grammar that recognizes "red", "green", or "blue".
            Choices colors = new Choices();
            colors.Add(new string[] { "red", "green", "blue" });

            // Create a GrammarBuilder object and append the Choices object.
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(colors);

            // Create the Grammar instance and load it into the speech recognition engine.
            Grammar g = new Grammar(gb);
            recognizer.LoadGrammar(g);

            // Register a handler for the SpeechRecognized event.
            recognizer.SpeechRecognized += (sender, e) => {

                string speech = e.Result.Text;
                
                //handle custom commands
                switch (speech)
                {
                    case "red":
                        tcs.SetResult("Hello Red");
                        break;
                    case "green":
                        tcs.SetResult("Hello Green");
                        break;
                    case "blue":
                        tcs.SetResult("Hello Blue");
                        break;
                    case "Close":
                        tcs.SetResult("Hello Close");
                        break;
                    default:
                        tcs.SetResult("Hello Not Sure");
                        break;
                }

            };

            // For Edge JS we cannot await an Async Call (else it leads to error)
            recognizer.Recognize();
            return tcs.Task.Result;

            //// For pure C#
            // await recognizer.RecognizeAsync();              
            // return tcs.Task;
        }
    }
}
