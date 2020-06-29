using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Threading;
using System.Speech.Recognition;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("[Server] Starting to listen @port=1234");
            StartListener();
            Console.ReadLine();
        }

        private static async void StartListener()
        {
            var tcpListener = TcpListener.Create(1234);
            tcpListener.Start();
            Console.WriteLine("[Server] Started to listen @port=1234");
            while (true)
            {
                Console.WriteLine("[Server] waiting for the client");
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                Console.WriteLine("[Server] Client has connected");
                using (var networkStream = tcpClient.GetStream())
                {
                    var buffer = new byte[1024];
                    Console.WriteLine($"[Server] Reading from client : ThreadId={Thread.CurrentThread.ManagedThreadId.ToString()}");
                    var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine($"[Server] Client wrote {request} : ThreadId={Thread.CurrentThread.ManagedThreadId.ToString()}");
                    string result = await ReadAudioFromFileAsync(request);// await ReadAudioFromFileAsync(request);
                    string ServerResponseString = result;
                    byte[] ServerResponseBytes = Encoding.UTF8.GetBytes(ServerResponseString);

                    await networkStream.WriteAsync(ServerResponseBytes, 0, ServerResponseBytes.Length);
                    Console.WriteLine($"[Server] Response : ThreadId={Thread.CurrentThread.ManagedThreadId.ToString()}\n{result}");
                }
            }
        }

        public static Task<string> ReadAudioFromFileAsync(string request)
        {    // async here is required to be used by Edge.JS that is a node.js module enable communicating with C# files
            var tcs = new TaskCompletionSource<string>();
            Task<string> t1 = tcs.Task;

            string result = "";
            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine())
            {
                Grammar dictation = new DictationGrammar();
                dictation.Name = "Dictation Grammar";
                recognizer.LoadGrammar(dictation);
                recognizer.SetInputToWaveFile(request);
                recognizer.SpeechRecognized += (sender, e) =>
                {
                    result = e.Result.Text;
                    tcs.SetResult(result);
                };
                recognizer.RecognizeCompleted += (sender, e) =>
                {
                    if (result.Length == 0)
                        tcs.SetResult(result);
                };
                recognizer.Recognize();
                return tcs.Task;
            }
       }
    }
}