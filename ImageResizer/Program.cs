﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //創建 取消物件
            CancellationTokenSource cts = new CancellationTokenSource();

            cts.CancelAfter(1000);

            //ThreadPool.QueueUserWorkItem(x =>
            //{
            //    ConsoleKeyInfo key = Console.ReadKey();
            //    if (key.Key == ConsoleKey.C)
            //    {
            //        cts.Cancel();
            //    }
            //});

            string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            string destinationPath = Path.Combine(Environment.CurrentDirectory, "output"); ;

            ImageProcess imageProcess = new ImageProcess();

            imageProcess.Clean(destinationPath);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            //await imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0);
            try
            {
                await imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0, cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"{Environment.NewLine}縮放圖片任務已經取消");
            }

            sw.Stop();

            Console.WriteLine($"花費時間: {sw.ElapsedMilliseconds} ms");

            Console.ReadKey();
        }
    }
}