using System;
using System.IO;

namespace UnionWSDL
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("UnionWSDL");
            Console.WriteLine("");

            if (args.Length < 1)
            {
                Usage();

                Console.ReadKey();
                return;
            }

            var resultFilename = ConfigurationHelper.AppSetting("ResultFileName", "out.wsdl");

            foreach (var path in args)
            {
                var wsdlPath = path;
                if (Uri.IsWellFormedUriString(wsdlPath, UriKind.Absolute))
                {
                    wsdlPath = wsdlPath + "?singleWsdl";
                }
                else
                {
                    if (File.Exists(wsdlPath) == false)
                    {
                        Console.WriteLine("Ошибка : .wsdl файл не существует!");

                        Console.ReadKey();
                        return;
                    }

                    var f = new FileInfo(wsdlPath);
                    var sourceFilename = f.FullName;

                    if (resultFilename == null)
                    {
                        resultFilename = Path.Combine(
                            Path.GetDirectoryName(sourceFilename),
                            Path.GetFileNameWithoutExtension(sourceFilename) + "_merged" + Path.GetExtension(sourceFilename));
                    }
                }

                Console.WriteLine("Процесс: {0}", wsdlPath);

                CombinerWSDL.Union(wsdlPath, resultFilename);
            }

            Console.WriteLine("Сохранили результат в: {0}", resultFilename);
            Console.ReadKey();
        }

        private static void Usage()
        {
            Console.WriteLine("Использование: UnionWSDL wsdl1 [wsdl2] [[wsdl3] ...");
            Console.WriteLine("");
            Console.WriteLine("Аргументы:");
            Console.WriteLine("  wsdl1      путь к wsdl, может быть URL");
            Console.WriteLine("  [wsdl2]    путь к wsdl2, может быть URL. не обязательный аргумент");
            Console.WriteLine("");

            Console.WriteLine("");
        }
    }
}