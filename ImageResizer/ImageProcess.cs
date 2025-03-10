﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer
{
    public class ImageProcess
    {
        /// <summary>
        /// 清空目的目錄下的所有檔案與目錄
        /// </summary>
        /// <param name="destPath">目錄路徑</param>
        public void Clean(string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            else
            {
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);

                //int _delCnt = 0;

                foreach (var item in allImageFiles)
                {
                    //_delCnt++;
                    //var tid = String.Format("{0:D2}", Thread.CurrentThread.ManagedThreadId);

                    //向左補零
                    //var _delId = String.Format("{0:D2}", _delCnt);

                    //Console.WriteLine($"刪除第{_delId}張圖片，檔名({Path.GetFileName(item)}) 執行緒 (TID: {tid}) >>>> {DateTime.Now}");
                    File.Delete(item);


                }
            }
        }
        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public async Task ResizeImagesAsync(string sourcePath, string destPath, double scale)
        {

            var allFiles = FindImages(sourcePath);

            List<Task> tasks = new List<Task>();

            //int _processCnt = 0;
            foreach (var filePath in allFiles)
            {
                //_processCnt++;
                //向左補零
                //var _processId = String.Format("{0:D2}", _processCnt);
                Image imgPhoto = Image.FromFile(filePath);
                string imgName = Path.GetFileNameWithoutExtension(filePath);

                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;

                int destionatonWidth = (int)(sourceWidth * scale);
                int destionatonHeight = (int)(sourceHeight * scale);

                var task = Task.Run(() =>
                {
                    //var tid = String.Format("{0:D2}", Thread.CurrentThread.ManagedThreadId);
                    //Console.WriteLine($"縮放第{_processId}張圖片，檔名({Path.GetFileName(filePath)}) 執行緒 (TID: {tid}) >>>> {DateTime.Now}");
                    Bitmap processedImage = processBitmap((Bitmap)imgPhoto,
                        sourceWidth, sourceHeight,
                        destionatonWidth, destionatonHeight);

                    string destFile = Path.Combine(destPath, imgName + ".jpg");
                    processedImage.Save(destFile, ImageFormat.Jpeg);
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        /// <param name="token">取消物件</param>
        public async Task ResizeImagesAsync(string sourcePath, string destPath, double scale, CancellationToken token)
        {

            var allFiles = FindImages(sourcePath);

            List<Task> tasks = new List<Task>();

            int _processCnt = 0;
            foreach (var filePath in allFiles)
            {
                //if (token.IsCancellationRequested == true)
                //    break;
                _processCnt++;
                //向左補零
                var _processId = String.Format("{0:D2}", _processCnt);
                Image imgPhoto = Image.FromFile(filePath);
                string imgName = Path.GetFileNameWithoutExtension(filePath);

                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;

                int destionatonWidth = (int)(sourceWidth * scale);
                int destionatonHeight = (int)(sourceHeight * scale);

                var task = Task.Run(() =>
                {
                    var tid = String.Format("{0:D2}", Thread.CurrentThread.ManagedThreadId);

                    

                    Console.WriteLine($"縮放第{_processId}張圖片，檔名({Path.GetFileName(filePath)}) 執行緒 (TID: {tid}) >>>> {DateTime.Now}");
                    Bitmap processedImage = processBitmap((Bitmap)imgPhoto,
                        sourceWidth, sourceHeight,
                        destionatonWidth, destionatonHeight);

                    string destFile = Path.Combine(destPath, imgName + ".jpg");
                    processedImage.Save(destFile, ImageFormat.Jpeg);

                    if (token.IsCancellationRequested == true)
                    {
                        Console.WriteLine($"執行緒 (TID: {tid})被取消 >>>> {DateTime.Now}");
                    }
                }, token);

                tasks.Add(task);
            }

            //token.ThrowIfCancellationRequested();
            await Task.WhenAll(tasks);

       
        }

        /// <summary>
        /// 找出指定目錄下的圖片
        /// </summary>
        /// <param name="srcPath">圖片來源目錄路徑</param>
        /// <returns></returns>
        public List<string> FindImages(string srcPath)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(srcPath, "*.png", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpg", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpeg", SearchOption.AllDirectories));
            return files;
        }

        /// <summary>
        /// 針對指定圖片進行縮放作業
        /// </summary>
        /// <param name="img">圖片來源</param>
        /// <param name="srcWidth">原始寬度</param>
        /// <param name="srcHeight">原始高度</param>
        /// <param name="newWidth">新圖片的寬度</param>
        /// <param name="newHeight">新圖片的高度</param>
        /// <returns></returns>
        Bitmap processBitmap(Bitmap img, int srcWidth, int srcHeight, int newWidth, int newHeight)
        {
            Bitmap resizedbitmap = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(resizedbitmap);
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(img,
                new Rectangle(0, 0, newWidth, newHeight),
                new Rectangle(0, 0, srcWidth, srcHeight),
                GraphicsUnit.Pixel);
            return resizedbitmap;
        }
    }
}