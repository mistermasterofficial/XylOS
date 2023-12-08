using System;
using System.IO;
using Sys = Cosmos.System;
using Console = System.Console;

namespace XylOS
{
    public class MainProgramm
    {
        string working_dir;
        string cur_dir
        {
            get
            {
                return working_dir;
            }
            set
            {
                Directory.SetCurrentDirectory(value);
                working_dir = value;
            }
        }

        public MainProgramm()
        {
            cur_dir = @"0:\";
            Console.WriteLine("Welcome to XylOS v0.0.1 !");
        }

        public int run()
        {
            Console.Write($"{cur_dir}> ");
            string command = Console.ReadLine().ToLower();

            string file1;
            string file2;

            switch (command)
            {
                default: Console.WriteLine($"{command}: command not found"); break;
                case "cd":
                    Console.Write("Enter the directory > ");
                    string new_dir = Console.ReadLine();
                    if (Directory.Exists(new_dir))
                        cur_dir = new_dir;
                    else
                        Console.WriteLine($"{command}: directory not found");
                    break;
                case "ls":
                    Console.Write("Directories : ");
                    foreach (var content in Directory.GetDirectories(cur_dir))
                        Console.Write(content + " ");
                    Console.WriteLine();
                    Console.Write("Files : ");
                    foreach (var content in Directory.GetFiles(cur_dir))
                        Console.Write(content + " ");
                    Console.WriteLine();
                    break;
                case "cat":
                    Console.Write("Enter the file name > ");
                    string file = Console.ReadLine();
                    var stream = File.OpenRead(file);
                    for (int i = 0; i < stream.Length; i++)
                    {
                        Console.Write(Convert.ToChar(stream.ReadByte()));
                    }
                    stream.Close();
                    Console.WriteLine();
                    break;
                case "cp":
                    Console.Write("Enter the original file > "); file1 = Console.ReadLine();
                    Console.Write("Enter the copied file > "); file2 = Console.ReadLine();
                    if (File.Exists(file1) && Directory.Exists(Path.GetDirectoryName(file2)))
                        File.Copy(file1, file2);
                    else
                        Console.WriteLine($"{command}: original file is not exist");
                    break;
                case "rm":
                    Console.Write("File or directory? [f/d] > ");
                    string fd = Console.ReadLine().ToLower();
                    if (fd == "f")
                    {
                        Console.Write("Enter the file > "); string path = Console.ReadLine();
                        if (File.Exists(path))
                            File.Delete(path);
                        else
                            Console.WriteLine($"{command}: file (or directory) is not exist");
                    }
                    else
                    {
                        Console.Write("Enter the directory > "); string path = Console.ReadLine();
                        if (Directory.Exists(path))
                            Directory.Delete(path);
                        else
                            Console.WriteLine($"{command}: file (or directory) is not exist");
                    }
                    break;
                case "mkdir":
                    Console.Write("Enter the directory name > ");
                    string dir = Console.ReadLine();
                    Directory.CreateDirectory(dir);
                    break;
                case "mkfile":
                    Console.Write("Enter the file name > ");
                    file = Console.ReadLine();
                    File.Create(file);
                    break;
                case "notebook":
                    Console.Write("Enter the file > "); string f = Console.ReadLine();
                    if (File.Exists(f))
                    {
                        Notebook app = new(f);
                        app.mainloop();
                        app = null;
                    }
                    else
                    {
                        Console.WriteLine($"{command}: file is not exist");
                    }
                    break;
                case "restart":
                    Sys.Power.Reboot();
                    break;
                case "shutdown":
                    Sys.Power.Shutdown();
                    break;
                case "run":
                    Console.Write("Enter the file > ");
                    file = Console.ReadLine();

                    if (File.Exists(file))
                    {
                        byte[] code = File.ReadAllBytes(@"0:\main.run");

                        Task main = new Task(code);

                        try
                        {
                            while (Task.tasks.Count > 0)
                            {
                                Task.Run();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"RUNTIME_ERROR: {ex.ToString()}");
                        }
                    }
                    else 
                    {
                        Console.WriteLine($"{command}: file is not exist");
                    }
                    break;
                case "help":
                    Console.WriteLine(@"Commands:
cd - change directory
ls - view all files and directories
cat - view file content
cp - copy file
rm - delete file or directory
mkdir - make directory
mkfile - make file
notebook - edit file (hotkeys: F1 - exit, F10 - save & exit)
restart - restart your computer
shutdown - turn off your computer
run - run executable file
");
                    break;
            }
            return 0;
        }
    }

    class Notebook
    {
        string fp;
        string content;

        public Notebook(string fp)
        {
            this.fp = fp;
            content = File.ReadAllText(fp);
            Console.Clear();
        }

        public void mainloop()
        {
            while (true)
            {
                Console.Clear();
                Console.Write(content);
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace)
                {
                    string new_content = "";
                    for (int i = 0; i < content.Length - 1; i++)
                    {
                        new_content += content[i];
                    }
                    content = new_content;
                }
                else if (key.Key == ConsoleKey.F10)
                {
                    File.WriteAllText(fp, content);
                    Console.Clear();
                    return;
                }
                else if (key.Key == ConsoleKey.F1)
                {
                    Console.Clear();
                    return;
                }
                else
                {
                    content += key.KeyChar;
                }
            }
        }
    }
}
