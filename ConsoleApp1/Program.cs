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
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                Console.WriteLine("[Server] Client has connected");
                using (var networkStream = tcpClient.GetStream())
                {
                    var buffer = new byte[1024];
                    Console.WriteLine("[Server] Reading from client");
                    var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine("[Server] Client wrote {0}", request);
                    string result = await ReadAudioFromFileAsync(request);
                    //string result = request;
                    string ServerResponseString = result;
                    byte[] ServerResponseBytes = Encoding.UTF8.GetBytes(ServerResponseString);

                    await networkStream.WriteAsync(ServerResponseBytes, 0, ServerResponseBytes.Length);
                    Console.WriteLine("[Server] Response has been written");
                }
            }
        }

        private static Task<string> ReadAudioFromFileAsync(string request)
        {
            var result = "";
            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine())
            {
                Grammar dictation = new DictationGrammar();
                dictation.Name = "Dictation Grammar";
                recognizer.LoadGrammar(dictation);
                recognizer.SetInputToWaveFile(request);
                recognizer.SpeechRecognized += (sender, e) =>
                {
                    result = e.Result.Text;
                };
                recognizer.RecognizeAsync();
            }
            return result;
        }
    }
}