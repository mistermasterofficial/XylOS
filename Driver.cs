using Cosmos.System;
using System;
using System.IO;
using Console = System.Console;
using Sys = Cosmos.System;

namespace XylOS
{
    public class Access2Driver
    {
        public static uint version = 0;

        TerminalDriver td = new TerminalDriver();
        PCSpeakerDriver pcsd = new PCSpeakerDriver();
        MemoryManagerDriver mmd = new MemoryManagerDriver();
        PowerDriver pd = new PowerDriver();

        public Access2Driver(){}

        public uint ExecuteInputFunc(ref uint[] main_memory, ref uint main_memory_cursor, uint func_num)
        {
            switch (func_num)
            {
                case 0:
                    return td.GetKeyChar();
                case 1:
                    return td.GetMods();
                case 2:
                    return td.GetCapsLock();
                case 3:
                    return td.GetNumLock();
                case 4:
                    return td.GetKey();
                case 5:
                    return mmd.GetFixedPos();
                case 6:
                    return td.GetKeyAvailable();
                case 7:
                    return td.GetX();
                case 8:
                    return td.GetY();
                case 9:
                    return td.GetWidth();
                case 10:
                    return td.GetHeight();
                case 11:
                    return td.GetCursorVisible();
                case 12:
                    return td.GetFGColor();
                case 13:
                    return td.GetBGColor();
            }
            return main_memory[main_memory_cursor];
        }

        public void ExecuteOutputFunc(ref uint[] main_memory, ref uint main_memory_cursor, uint func_num)
        {
            switch (func_num)
            {
                case 0:
                    td.Write(ref main_memory, ref main_memory_cursor);
                    break;
                case 1:
                    td.ChangeFGColor(ref main_memory, ref main_memory_cursor);
                    break;
                case 2:
                    td.ChangeBGColor(ref main_memory, ref main_memory_cursor);
                    break;
                case 3:
                    td.ChangeX(ref main_memory, ref main_memory_cursor);
                    break;
                case 4:
                    td.ChangeY(ref main_memory, ref main_memory_cursor);
                    break;
                case 5:
                    td.Clear();
                    break;
                case 6:
                    td.NewLine();
                    break;
                case 7:
                    pcsd.Beep();
                    break;
                case 8:
                    pcsd.FDBeep(ref main_memory, ref main_memory_cursor);
                    break;
                case 9:
                    pcsd.NDBeep(ref main_memory, ref main_memory_cursor);
                    break;
                case 10:
                    mmd.UpdateFixedPosFromMemory(ref main_memory, ref main_memory_cursor);
                    break; 
                case 11:
                    mmd.SetMMC(ref main_memory_cursor);
                    break;
                case 12:
                    td.UpdateKeyInfo();
                    break;
                case 13:
                    td.SetCursorVisible(ref main_memory, ref main_memory_cursor);
                    break;
                case 14:
                    mmd.UpdateFixedPosFromMMC(ref main_memory, ref main_memory_cursor);
                    break;
                case 15:
                    pd.Off();
                    break;
                case 16:
                    pd.Reboot();
                    break;
                case 17:
                    mmd.SetPrevMMC(ref main_memory_cursor);
                    break;
                case 18:
                    mmd.ClearAll(ref main_memory);
                    break;
            }
        }
    }

    public class MemoryManagerDriver
    {
        uint pointer = 0;
        uint prev_pointer = 0;
        public MemoryManagerDriver() { }

        public void UpdateFixedPosFromMemory(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            prev_pointer = pointer;
            pointer = main_memory[main_memory_cursor] % (uint)main_memory.Length;
        }

        public void UpdateFixedPosFromMMC(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            prev_pointer = pointer;
            pointer = main_memory_cursor % (uint)main_memory.Length;
        }

        public uint GetFixedPos()
        {
            return pointer;
        }

        public void SetMMC(ref uint main_memory_cursor)
        { 
            main_memory_cursor = pointer;
        }

        public void SetPrevMMC(ref uint main_memory_cursor)
        {
            main_memory_cursor = prev_pointer;
        }

        public void ClearAll(ref uint[] main_memory)
        {
            for(int i = 0; i<main_memory.Length; i++)
            {
                main_memory[i] = 0;
            }
        }
    }

    public class TerminalDriver
    {
        ConsoleKeyInfo key_info;

        public TerminalDriver(){}

        public uint GetKeyChar()
        {
            return key_info.KeyChar;
        }

        public uint GetMods()
        {
            return (uint)key_info.Modifiers;
        }

        public uint GetCapsLock()
        {
            return Convert.ToUInt32(Console.CapsLock);
        }

        public uint GetNumLock()
        {
            return Convert.ToUInt32(Console.NumberLock);
        }

        public uint GetKey()
        {
            return (uint)key_info.Key;
        }

        public uint GetKeyAvailable()
        {
            return Convert.ToUInt32(Console.KeyAvailable);
        }

        public uint GetX()
        {
            return (uint)Console.CursorLeft;
        }

        public uint GetY()
        {
            return (uint)Console.CursorTop;
        }

        public uint GetCursorVisible()
        {
            return Convert.ToUInt32(Console.CursorVisible);
        }

        public uint GetWidth()
        {
            return (uint)Terminal.GetWidth();
        }

        public uint GetHeight()
        {
            return (uint)Terminal.GetHeight();
        }

        public uint GetFGColor()
        {
            return (uint)Console.ForegroundColor;
        }

        public uint GetBGColor()
        {
            return (uint)Console.BackgroundColor;
        }

        public void UpdateKeyInfo()
        {
            key_info = Console.ReadKey(true);
        }

        public void Write(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            Console.Write(Terminal.chars[main_memory[main_memory_cursor] % 256]);
        }

        public void ChangeFGColor(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            Console.ForegroundColor = (ConsoleColor)main_memory[main_memory_cursor];
        }

        public void ChangeBGColor(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            Console.BackgroundColor = (ConsoleColor)main_memory[main_memory_cursor];
        }

        public void ChangeX(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            Console.CursorLeft = (int)(main_memory[main_memory_cursor]%(uint)Terminal.GetWidth());
        }

        public void ChangeY(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            Console.CursorTop = (int)(main_memory[main_memory_cursor]%(uint)Terminal.GetHeight());
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void NewLine()
        {
            Console.WriteLine();
        }

        public void SetCursorVisible(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            Console.CursorVisible = main_memory[main_memory_cursor]>=1 ? true : false;
        }
    }

    public class PCSpeakerDriver
    {
        public PCSpeakerDriver(){}

        public void Beep()
        {
            PCSpeaker.Beep();
        }

        public void FDBeep(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            PCSpeaker.Beep(main_memory[main_memory_cursor], main_memory[main_memory_cursor + 1]);
        }

        public void NDBeep(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            PCSpeaker.Beep((Notes)main_memory[main_memory_cursor], (Durations)main_memory[main_memory_cursor + 1]);
        }
    }

    public class PowerDriver
    {
        public PowerDriver() { }

        public void Off()
        {
            Power.Shutdown();
        }

        public void Reboot()
        {
            Power.Reboot();
        }
    }

    public class DirectoryDriver
    {
        int fp_read_index = 0;
        string fp = "";
        string dest_fp = "";

        int founded_fp_str_index = 0;
        string founded_fp = "";
        int founded_fp_index = 0;

        public DirectoryDriver() { }

        public void WriteFP(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            fp += Terminal.chars[main_memory[main_memory_cursor] % 256];
        }

        public void ClearFP()
        {
            fp = "";
        }

        public void FPtoDestFP()
        {
            dest_fp = fp;
        }

        public uint GetFP()
        {
            if (fp_read_index == fp.Length)
            {
                fp_read_index = 0;
            }
            else
            {
                for (int i = 0; i < 256; i++)
                {
                    if (Terminal.chars[i] == fp[fp_read_index])
                    {
                        fp_read_index++;
                        return (uint)i;
                    }
                }
            }
            return 0;
        }

        public void CreateDir()
        {
            Directory.CreateDirectory(fp);
        }

        public void MoveDir()
        {
            Directory.Move(fp, dest_fp);
        }

        public void DeleteDir()
        {
            Directory.Delete(fp);
        }

        public uint IsExists()
        {
            return Convert.ToUInt32(Directory.Exists(fp));
        }

        public uint GetFilesLenght()
        {
            return (uint)Directory.GetFiles(fp).Length;
        }

        public uint GetFilesFileChar()
        {
            founded_fp = Directory.GetFiles(founded_fp)[founded_fp_index];
            if (founded_fp_str_index == founded_fp.Length)
            {
                founded_fp_str_index = 0;
                founded_fp_index++;
                founded_fp_index %= Directory.GetFiles(founded_fp).Length;
                return 0;
            }
            for (int i = 0; i < 256; i++)
            {
                if (Terminal.chars[i] == founded_fp[founded_fp_str_index])
                {
                    founded_fp_str_index++;
                    return (uint)i;
                }
            }
            return 0;
        }

        public uint GetDirectoriesDirectoryChar()
        {
            founded_fp = Directory.GetDirectories(founded_fp)[founded_fp_index];
            if (founded_fp_str_index == founded_fp.Length)
            {
                founded_fp_str_index = 0;
                founded_fp_index++;
                founded_fp_index %= Directory.GetDirectories(founded_fp).Length;
                return 0;
            }
            for (int i = 0; i < 256; i++)
            {
                if (Terminal.chars[i] == founded_fp[founded_fp_str_index])
                {
                    founded_fp_str_index++;
                    return (uint)i;
                }
            }
            return 0;
        }

        public uint GetDirectoriesLenght()
        {
            return (uint)Directory.GetDirectories(fp).Length;
        }
    }

    public class FileDriver
    {
        int fp_read_index = 0;
        string fp = "";
        string dest_fp = "";
        FileStream stream;

        public FileDriver() { }

        public void WriteFP(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            fp += Terminal.chars[main_memory[main_memory_cursor] % 256];
        }

        public void ClearFP()
        {
            fp = "";
        }

        public void FPtoDestFP()
        {
            dest_fp = fp;
        }

        public uint GetFP()
        {
            if (fp_read_index == fp.Length)
            {
                fp_read_index = 0;
            }
            else
            {
                for (int i = 0; i < 256; i++)
                {
                    if (Terminal.chars[i] == fp[fp_read_index])
                    {
                        fp_read_index++;
                        return (uint)i;
                    }
                }
            }
            return 0;
        }

        public void OpenRead() 
        {
            stream = File.OpenRead(fp);
        }

        public void OpenWrite()
        {
            stream = File.OpenWrite(fp);
        }

        public void Write(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            stream.WriteByte((byte)main_memory[main_memory_cursor]);
        }

        public uint Read()
        {
            uint value = (uint)stream.ReadByte();
            stream.Position--;
            return value;
        }

        public uint GetLenght()
        {
            return (uint)stream.Length;
        }

        public uint GetPosition()
        {
            return (uint)stream.Position;
        }

        public void SetPosition(ref uint[] main_memory, ref uint main_memory_cursor)
        {
            stream.Position = main_memory[main_memory_cursor];
        }

        public void Close()
        {
            stream.Close();
        }

        public void CreateFile()
        { 
            File.Create(fp); 
        }

        public void CopyFile()
        {
            File.Copy(fp, dest_fp);
        }

        public void MoveFile()
        {
            File.Move(fp, dest_fp);
        }

        public void DeleteFile()
        { 
            File.Delete(fp); 
        }

        public uint IsExists()
        {
            return Convert.ToUInt32(File.Exists(fp));
        }
    }
}
