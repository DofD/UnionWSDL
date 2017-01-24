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

            var sourceFilename = args[0];
            string destinationFilename = null;
            if (args.Length > 1)
            {
                destinationFilename = args[1];
            }

            if (Uri.IsWellFormedUriString(sourceFilename, UriKind.Absolute))
            {
            }
            else
            {
                if (File.Exists(sourceFilename) == false)
                {
                    Console.WriteLine("Ошибка : .wsdl файл не существует!");

                    Console.ReadKey();
                    return;
                }

                var f = new FileInfo(sourceFilename);
                sourceFilename = f.FullName;

                if (destinationFilename == null)
                {
                    destinationFilename = Path.Combine(
                        Path.GetDirectoryName(sourceFilename),
                        Path.GetFileNameWithoutExtension(sourceFilename) + "_merged" + Path.GetExtension(sourceFilename));
                }
            }

            Console.WriteLine("Processing: {0}", sourceFilename);
            Console.WriteLine("Will create: {0}", destinationFilename);

            WSDLMerger.Merge(sourceFilename, destinationFilename);

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