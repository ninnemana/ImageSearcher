using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSearch {
    class Program {
        private const string path = "partnum.csv";
        private const string lib2 = "C:\\Users\\aninneman\\Desktop\\chris_image_search\\";
        private const string lib = "W:\\Resource Library\\Photography\\Part Number\\Full Resolution\\";
        private static bool _s_stop = false;
        public static string[] postfixes = new string[]{"a","b","c","d","e","f","g","h","i","j","k","l","m","p","q"};
        public static List<string> subs = new List<string>();

        static void Main(string[] args) {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            subs = postfixes.ToList<string>();

            Console.WriteLine("Enter the location of the Part Number CSV (" +  path + "): ");
            string part_file = Console.ReadLine();
            if (part_file.Length == 0) {
                part_file = path;
            }

            Console.WriteLine("Enter the location of the images ( " + lib + "): ");
            string img_dir = Console.ReadLine();
            if (img_dir.Length == 0) {
                img_dir = lib;
            }

            Console.WriteLine("Enter the postfixes that you want; comma separated (ALL): ");
            string posts = Console.ReadLine();
            if(posts.Length > 0){
                subs = new List<string>();
                foreach (string post in posts.Split(',')) {
                    subs.Add(post);
                }
            }

            string existsPath = @"exists.csv";  
            string missingPath = @"missing.csv";

            using (StreamReader readFile = new StreamReader(part_file)) {
                string line;
                int count = 0;
                StringBuilder existsBuilder = new StringBuilder();
                StringBuilder missingBuilder = new StringBuilder();

                try {
                    //Create a new subfolder under the current active folder 
                    string newPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Images");

                    // Create the subfolder
                    System.IO.Directory.CreateDirectory(newPath);
                    while ((line = readFile.ReadLine()) != null) {
                        Parallel.ForEach(subs, sub => {

                            string file_path = img_dir + line + "_" + sub + ".jpg";
                            FileInfo inf = new FileInfo(file_path);
                            if (inf.Exists) {
                                existsBuilder.Append(file_path + "\n");
                                System.IO.Directory.GetCurrentDirectory();
                                File.Copy(file_path, Path.Combine(newPath,line + "_" + sub + ".jpg"));
                            } else {
                                missingBuilder.Append(file_path + "\n");
                            }
                            count++;
                        });
                    }
                    File.WriteAllText(existsPath, existsBuilder.ToString());
                    File.WriteAllText(missingPath, missingBuilder.ToString());
                    Console.WriteLine("And done.");
                } catch (Exception e) {
                    Console.WriteLine("Oh shit, I fucked up. " + e.InnerException);
                }
                
            }
            Console.ReadLine();
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            //you have 2 options here, leave e.Cancel set to false and just handle any
            //graceful shutdown that you can while in here, or set a flag to notify the other
            //thread at the next check that it's to shut down.  I'll do the 2nd option
            e.Cancel = true;
            _s_stop = true;
            Console.WriteLine("CancelKeyPress fired...");
        }
    }
}
