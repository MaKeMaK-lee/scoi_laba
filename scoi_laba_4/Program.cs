using System;
using System.Diagnostics;
using System.IO;



/* для подключения System.Drawing в своем проекте правой в проекте нажать правой кнопкой по Ссылкам -> Добавить ссылку
    отметить галочкой сборку System.Drawing    */
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace IMGapp
{

    class Program
    {
        static void Main(string[] args)
        {
            bool cycle = false;
            string img1name = "in1.jpg", img2name = "in2.jpg", imgOutname = "out.jpg";
            int mode = -1;
            int[] gistogrammaDataA = new int[256], gistogrammaDataOld = new int[256];
            double[] gistogrammaDataNormirovana = new double[256];
            //Входные имена

            Console.WriteLine("Выберите имя входного файла.\nЧтобы использовать имя in1.jpg нажмите ПРОБЕЛ.\nВ ином случае нажмите любую другую клавишу.");
            if (Console.ReadKey(true).Key != ConsoleKey.Spacebar)
            {
                Console.WriteLine("Введите имя входного файла:");
                img1name = Console.ReadLine();
            }

            //Выббор режима модификации изображения

            Console.WriteLine("Выберите режим:\n1 Метод Гаврилова\n2 Критерий Отсу\n3 Сгладить линейно Гауссово\n4 Сгладить медианно\n0 Сделать ФСЁ");
            mode = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();
            if (mode < 0 || mode > 4) { Console.WriteLine("Не, всё, я всё. Читать надо было."); return; }
            if (mode == 0)
            {
                mode++;
                cycle = true;
            }
            do
            {
                //Открываем входные картинки

                using (var img1 = new Bitmap("..\\..\\" + img1name))
                {
                    Console.WriteLine("Открываю изображение " + Directory.GetParent("..\\..\\") + "\\" + img1name);



                    var w = img1.Width;
                    var h = img1.Height;


                    using (var img_out = new Bitmap(w, h))   //создаем пустое изображение размером с исходное для сохранения результата
                    {
                        var time1 = DateTime.Now;
                        Stopwatch timer = new Stopwatch();
                        timer.Start();

                        //Это просто константы чтоб в цикле не считать. Я конечно старался не стараться и беречь драгоценную энергию, но вот, в общем.
                        //int xs = w / 4, xe = xs * 3, ys = h / 4, ye = ys * 3, xc = w / 2, yc = h / 2, Minxsys = Math.Min(xs, ys);//Читать их невозможно, но вы держитесь...

                        int[,] pxiMass = new int[h, w], pxiMassR = new int[h, w], pxiMassG = new int[h, w], pxiMassB = new int[h, w];
                        int pxi, PixelCount = h * w;
                        double avg = 0;
                        //попиксельно обрабатываем картинки
                        for (int i = 0; i < h; ++i)
                        {
                            for (int j = 0; j < w; ++j)
                            {
                                //считывыем пиксель картинки и получаем его цвет
                                var pix1 = img1.GetPixel(j, i);

                                //получаем цветовые компоненты цвета
                                int r1 = pix1.R;
                                int g1 = pix1.G;
                                int b1 = pix1.B;

                                //Считаем цвет пикселя в градациях серого и кидаем в массив
                                pxi = (int)(0.2125 * r1 + 0.7154 * g1 + 0.0721 * b1);
                                pxiMass[i, j] = pxi;

                                //Кинем в массивы цветов цвета
                                if (mode == 3 || mode == 4)
                                {
                                    pxiMassR[i, j] = r1;
                                    pxiMassG[i, j] = g1;
                                    pxiMassB[i, j] = b1;
                                }

                                //Данные для гистограммы начального изображения
                                gistogrammaDataOld[(r1 + g1 + b1) / 3]++;

                                //Для вычисления среднего арифметического
                                if (mode == 1)
                                {
                                    avg += pxi;
                                }

                                //записываем пиксель в изображение
                                pix1 = Color.FromArgb(pxi, pxi, pxi);
                                img1.SetPixel(j, i, pix1);

                            }
                        }
                        //Normiruem gistogrammu
                        double torrentT = 0;
                        if (mode == 2)
                        {
                            for (int kek = 0; kek < gistogrammaDataNormirovana.Length; kek++)
                            {
                                gistogrammaDataNormirovana[kek] = (double)gistogrammaDataOld[kek] / PixelCount;
                                torrentT += kek * gistogrammaDataNormirovana[kek];
                            }
                        }
                        //Edem v sosednee selo
                        if (mode == 1)
                        {
                            avg /= (w * h);
                            for (int i = 0; i < h; ++i)
                            {
                                for (int j = 0; j < w; ++j)
                                {
                                    pxi = (pxiMass[i, j] <= avg) ? 0 : 255;
                                    img_out.SetPixel(j, i, Color.FromArgb(pxi, pxi, pxi));
                                }
                            }
                        }
                        if (mode == 2)
                        {
                            double summPrevN = 0, summPrevNMulNumber = 0, torrent1 = 0, torrent2 = 0, sIgMaQb = 0, sigmaMax = 0, porebrik = 0;
                            for (int kek = 0; kek < gistogrammaDataNormirovana.Length; kek++)
                            {
                                double oMeGa2 = 1 - summPrevN;
                                torrent1 = summPrevNMulNumber / summPrevN;
                                torrent2 = (torrentT - torrent1 * summPrevN) / oMeGa2;
                                sIgMaQb = summPrevN * oMeGa2 * Math.Pow(Math.Abs(torrent1 - torrent2), 2);
                                if (sIgMaQb > sigmaMax)
                                {
                                    sigmaMax = sIgMaQb;
                                    porebrik = kek;
                                }


                                summPrevNMulNumber += kek * gistogrammaDataNormirovana[kek];//to nu1
                                summPrevN += gistogrammaDataNormirovana[kek];//omega1

                            }

                            for (int i = 0; i < h; ++i)
                            {
                                for (int j = 0; j < w; ++j)
                                {
                                    pxi = (pxiMass[i, j] <= porebrik) ? 0 : 255;
                                    img_out.SetPixel(j, i, Color.FromArgb(pxi, pxi, pxi));
                                }
                            }


                        }
                        if (mode == 3)
                        {
                            //Матрица Гаусса при Сигма = 1 Радиусе = 2 (Сумма = 0.98181)
                            double[] gaussianMatrix = {
                            0.00292, 0.01306, 0.02154, 0.01306, 0.00292,
                            0.01306, 0.05855 , 0.09653, 0.05855, 0.01306,
                            0.02154, 0.09653 , 0.15915, 0.09653, 0.02154,
                            0.01306, 0.05855 , 0.09653, 0.05855, 0.01306,
                            0.00292, 0.01306 , 0.02154, 0.01306, 0.00292,
                        };
                            double kRepair = 1 / 0.98181;//Я решил починить недостачу
                            double k;
                            int gNum;
                            double r, g, b;
                            for (int i = 0; i < h; ++i)
                            {
                                for (int j = 0; j < w; ++j)
                                {
                                    gNum = 0;
                                    r = g = b = 0;
                                    for (int ii = i - 2; ii <= i + 2; ++ii)
                                    {
                                        for (int jj = j - 2; jj < j + 2; ++jj)
                                        {
                                            if ((jj >= 0 && jj < w) && (ii >= 0 && ii < h))
                                            {
                                                k = gaussianMatrix[gNum++];
                                                r += pxiMassR[ii, jj] * k;
                                                g += pxiMassG[ii, jj] * k;
                                                b += pxiMassB[ii, jj] * k;
                                            }
                                        }
                                    }
                                    img_out.SetPixel(j, i, Color.FromArgb((int)Math.Round(r * kRepair), (int)Math.Round(g * kRepair), (int)Math.Round(b * kRepair)));
                                }
                            }
                        }
                        if (mode == 4)
                        {
                            int bNum, r, g, b;
                            int[] bufR = new int[121], bufG = new int[121], bufB = new int[121];
                            bool bubenec = false;
                            for (int i = 0; i < h; ++i)
                            {
                                for (int j = 0; j < w; ++j)
                                {
                                    bNum = r = g = b = 0;
                                    for (int ii = i - 5; ii <= i + 5; ++ii)
                                    {
                                        for (int jj = j - 5; jj < j + 5; ++jj)
                                        {
                                            if ((jj >= 0 && jj < w) && (ii >= 0 && ii < h))
                                            {
                                                bufR[bNum] = pxiMassR[ii, jj];
                                                bufG[bNum] = pxiMassG[ii, jj];
                                                bufB[bNum] = pxiMassB[ii, jj];
                                            }
                                            else//Заполним с разных сторон чтоб не сдвигать медиану (динамический размер лень)
                                            {
                                                bufR[bNum] = bubenec ? 0 : 255;
                                                bufG[bNum] = bubenec ? 0 : 255;
                                                bufB[bNum] = bubenec ? 0 : 255;
                                                bubenec = !bubenec;
                                            }
                                            bNum++;
                                        }
                                    }
                                    var keksR = from kekas in bufR orderby kekas select kekas;
                                    var keksG = from kekas in bufG orderby kekas select kekas;
                                    var keksB = from kekas in bufB orderby kekas select kekas;

                                    img_out.SetPixel(j, i, Color.FromArgb(keksR.ElementAt(61), keksG.ElementAt(61), keksB.ElementAt(61)));

                                }
                                if (i % (h / 10) == 0)
                                    Console.WriteLine($"Медианное сглаживание: обработано {i}/{h} строк.");
                            }

                        }


                        timer.Stop();

                        Console.WriteLine("Обработал изображение за " + timer.ElapsedMilliseconds + " мс.");

                        //Имя выхода (он есть).
                        //Console.WriteLine("\nВыберите в каво сохранить.\nЧтобы не писать что-то, нажмите ПРОБЕЛ.\nЧтобы написать имя, жмякните что-то иное.");
                        //if (Console.ReadKey(true).Key != ConsoleKey.Spacebar)
                        //{
                        //    Console.WriteLine("Введите имя выходного файла:");
                        //    imgOutname = Console.ReadLine();
                        //}
                        //А вот и выходит.
                        img_out.Save("..\\..\\" + img1name + (mode < 3 ? "_zbinary_" : "_smooth_") + GetStringModeName(mode) + ".jpg");
                        //img1.Save("..\\..\\" + img1name + "_zmono.jpg");


                        Console.WriteLine("Дело было сохренено по пути (нипутю) " + Directory.GetParent("..\\..\\") + "\\" + img1name + (mode < 3 ? "_zbinary_" : "_smooth_") + GetStringModeName(mode) + ".jpg");


                    }


                }
                Console.WriteLine();
                mode++;
                if (mode > 4)
                    cycle = false;
            } while (cycle == true);
            //Рисуем гистограммы.
            DropGistogramma(gistogrammaDataOld, img1name);



        }

        /// <summary>
        /// Функция постройки гистограммы и её печати в файл
        /// </summary>
        /// <param name="data">Массив данных</param>
        /// <param name="imgname">Имя файла (будет сохранено с припиской, так что лучше имя файла, для которого строится гистограмма)</param>
        public static void DropGistogramma(double[] data, string imgname = "DropGistogramma")
        {
            using (var img_out_gistogramma = new Bitmap(276, 120))
            {
                double max = data.Max<double>();


                using (var g = Graphics.FromImage(img_out_gistogramma))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;



                    g.DrawRectangle(Pens.Gray, 10, 10, 258, 102);


                    for (int i = 0; i < 256; i++)
                    {
                        g.DrawLine(Pens.Aqua, 11 + i, 111, 11 + i, 111 - (int)(100 * ((double)data[i] / max)));
                    }


                    img_out_gistogramma.Save("..\\..\\" + imgname + "_gistogramma.jpg");
                }



            }
        }
        /// <summary>
        /// Функция постройки гистограммы и её печати в файл
        /// </summary>
        /// <param name="data">Массив данных</param>
        /// <param name="imgname">Имя файла (будет сохранено с припиской, так что лучше имя файла, для которого строится гистограмма)</param>
        public static void DropGistogramma(int[] data, string imgname = "DropGistogramma")
        {
            using (var img_out_gistogramma = new Bitmap(276, 120))
            {
                int max = data.Max<int>();


                using (var g = Graphics.FromImage(img_out_gistogramma))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;



                    g.DrawRectangle(Pens.Gray, 10, 10, 258, 102);


                    for (int i = 0; i < 256; i++)
                    {
                        g.DrawLine(Pens.Aqua, 11 + i, 111, 11 + i, 111 - (int)(100 * ((double)data[i] / max)));
                    }


                    img_out_gistogramma.Save("..\\..\\" + imgname + "_gistogramma.jpg");
                }



            }
        }
        public static string GetStringModeName(int mode)
        {
            switch (mode)
            {
                case 1:
                    return "1_Гаврилова";
                case 2:
                    return "2_Отсу";
                case 3:
                    return "3_Линейно_Гаусс";
                case 4:
                    return "4_Медианно";
                default:
                    throw new Exception("pepkek");
            }
        }
        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }


}
