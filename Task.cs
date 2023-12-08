using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cosmos.Debug.Kernel;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Sys = Cosmos.System;

namespace XylOS
{
    public class Task
    {
        public static List<Task> tasks = new List<Task>();

        byte[] code;
        uint pos = 0;
        List<uint> while_branch_poses = new List<uint>();

        uint[] main_memory;
        uint main_memory_cursor = 0;

        uint[] input_func;
        uint input_func_cursor = 0;
        uint[] output_func;
        uint output_func_cursor = 0;

        uint copied_value = 0;

        Access2Driver drivers = new Access2Driver();

        public Task(byte[] _code)
        {
            if (BitConverter.ToInt16(_code, 0) == Access2Driver.version)
            {
                main_memory = new uint[BitConverter.ToUInt16(_code, 2)];
                input_func = new uint[BitConverter.ToUInt16(_code, 2)];
                output_func = new uint[BitConverter.ToUInt16(_code, 2)];

                code = new byte[_code.Length - 4];
                for (int i = 0; i < code.Length; i++)
                {
                    code[i] = new byte();
                    code[i] = _code[i + 4];
                }

                for (int i = 0; i < main_memory.Length; i++)
                {
                    main_memory[i] = new uint();
                    main_memory[i] = 0;
                }

                for (int i = 0; i < input_func.Length; i++)
                {
                    input_func[i] = new uint();
                    input_func[i] = 0;
                }
                for (int i = 0; i < output_func.Length; i++)
                {
                    output_func[i] = new uint();
                    output_func[i] = 0;
                }

                tasks.Add(this);
            }
        }

        public void Execute()
        {
            byte command = code[pos];
            switch (command)
            {
                case 0x00:
                    main_memory_cursor++;
                    main_memory_cursor %= (uint)main_memory.Length;
                    break;
                case 0x01:
                    main_memory_cursor--;
                    main_memory_cursor %= (uint)main_memory.Length;
                    break;
                case 0x02:
                    main_memory[main_memory_cursor]++;
                    break;
                case 0x03:
                    main_memory[main_memory_cursor]--;
                    break;
                case 0x04:
                    if (Convert.ToBoolean(main_memory[main_memory_cursor]))
                    {
                        while_branch_poses.Add(pos);
                    }
                    else
                    {
                        uint entry_num = 0;
                        pos++;
                        while (code[pos] != 0x05 || entry_num > 0)
                        {
                            if (code[pos] == 0x04)
                            {
                                entry_num++;
                            }

                            if (code[pos] == 0x05)
                            {
                                entry_num--;
                            }

                            pos++;
                        }
                    }
                    break;
                case 0x05:
                    pos = while_branch_poses[while_branch_poses.Count - 1] - 1;
                    while_branch_poses.RemoveAt(while_branch_poses.Count - 1);
                    break;
                case 0x06:
                    main_memory[main_memory_cursor] = drivers.ExecuteInputFunc(ref main_memory, ref main_memory_cursor, input_func[input_func_cursor]);
                    break;
                case 0x07:
                    drivers.ExecuteOutputFunc(ref main_memory, ref main_memory_cursor, output_func[output_func_cursor]);
                    break;
                case 0x08:
                    input_func_cursor++;
                    input_func_cursor %= (uint)input_func.Length;
                    break;
                case 0x09:
                    input_func_cursor--;
                    input_func_cursor %= (uint)input_func.Length;
                    break;
                case 0x0A:
                    output_func_cursor++;
                    output_func_cursor %= (uint)output_func.Length;
                    break;
                case 0x0B:
                    output_func_cursor--;
                    output_func_cursor %= (uint)output_func.Length;
                    break;
                case 0x0C:
                    main_memory[main_memory_cursor] = ~main_memory[main_memory_cursor];
                    break;
                case 0x0D:
                    main_memory[main_memory_cursor] |= main_memory[main_memory_cursor + 1];
                    break;
                case 0x0E:
                    main_memory[main_memory_cursor] &= main_memory[main_memory_cursor + 1];
                    break;
                case 0x0F:
                    main_memory[main_memory_cursor] ^= main_memory[main_memory_cursor + 1];
                    break;
                case 0x10:
                    main_memory[main_memory_cursor] = main_memory[main_memory_cursor] == main_memory[main_memory_cursor + 1] ? (uint)1 : 0;
                    break;
                case 0x11:
                    main_memory[main_memory_cursor] = main_memory[main_memory_cursor] > main_memory[main_memory_cursor + 1] ? (uint)1 : 0;
                    break;
                case 0x12:
                    input_func[input_func_cursor] = main_memory[main_memory_cursor];
                    break;
                case 0x13:
                    output_func[output_func_cursor] = main_memory[main_memory_cursor];
                    break;
                case 0x14:
                    tasks.Remove(this);
                    break;
                case 0x15:
                    while_branch_poses.RemoveAt(while_branch_poses.Count - 1);
                    pos++;
                    while (code[pos] != 0x05)
                    {
                        pos++;
                    }
                    break;
                case 0x16:
                    main_memory[main_memory_cursor] <<= 1;
                    break;
                case 0x17:
                    main_memory[main_memory_cursor] >>= 1;
                    break;
                case 0x18:
                    copied_value = main_memory[main_memory_cursor];
                    break;
                case 0x19:
                    main_memory[main_memory_cursor] = copied_value;
                    break;
                case 0x1A:
                    main_memory[main_memory_cursor] = 0;
                    break;
            }
            pos++;
        }

        public static void Run()
        {
            tasks[tasks.Count - 1].Execute();
        }
    }
}



//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading;
//using Cosmos.Debug.Kernel;
//using Cosmos.System.FileSystem;
//using Cosmos.System.FileSystem.VFS;
//using Sys = Cosmos.System;

//namespace XylOS
//{
//    public class Task
//    {
//        public static List<Task> tasks = new List<Task>();

//        uint pos = 0;
//        List<uint> while_branch_poses = new List<uint>();

//        uint[] main_memory;
//        uint main_memory_cursor = 0;

//        uint[] input_func;
//        uint input_func_cursor = 0;
//        uint[] output_func;
//        uint output_func_cursor = 0;

//        uint copied_value = 0;

//        Access2Driver drivers = new Access2Driver();

//        FileStream stream;

//        public Task(string path)
//        {
//            stream = File.OpenRead(path);

//            var buffer = new byte[2];

//            stream.Read(buffer, 0, buffer.Length);
//            var version = Convert.ToUInt16(buffer);

//            stream.Read(buffer, 2, buffer.Length);
//            var memory_lenght = Convert.ToUInt16(buffer);

//            if (version == Access2Driver.version)
//            {
//                main_memory = new uint[memory_lenght];
//                input_func = new uint[memory_lenght];
//                output_func = new uint[memory_lenght];

//                for (int i = 0; i < main_memory.Length; i++)
//                {
//                    main_memory[i] = new uint();
//                    main_memory[i] = 0;
//                }

//                for (int i = 0; i < input_func.Length; i++)
//                {
//                    input_func[i] = new uint();
//                    input_func[i] = 0;
//                }
//                for (int i = 0; i < output_func.Length; i++)
//                {
//                    output_func[i] = new uint();
//                    output_func[i] = 0;
//                }

//                tasks.Add(this);
//            }
//        }

//        byte getCommand(uint pos_)
//        {
//            stream.Position = pos_;
//            byte cmd = (byte)stream.ReadByte();
//            stream.Position = pos_;
//            return cmd;
//        }

//        public void Execute()
//        {
//            byte command = getCommand(pos);
//            switch (command)
//            {
//                case 0x00:
//                    main_memory_cursor++;
//                    main_memory_cursor %= (uint)main_memory.Length;
//                    break;
//                case 0x01:
//                    main_memory_cursor--;
//                    main_memory_cursor %= (uint)main_memory.Length;
//                    break;
//                case 0x02:
//                    main_memory[main_memory_cursor]++;
//                    break;
//                case 0x03:
//                    main_memory[main_memory_cursor]--;
//                    break;
//                case 0x04:
//                    if (Convert.ToBoolean(main_memory[main_memory_cursor]))
//                    {
//                        while_branch_poses.Add(pos);
//                    }
//                    else
//                    {
//                        uint entry_num = 0;
//                        pos++;
//                        while (getCommand(pos) != 0x05 || entry_num > 0)
//                        {
//                            if (getCommand(pos) == 0x04)
//                            {
//                                entry_num++;
//                            }

//                            if (getCommand(pos) == 0x05)
//                            {
//                                entry_num--;
//                            }

//                            pos++;
//                        }
//                    }
//                    break;
//                case 0x05:
//                    pos = while_branch_poses[while_branch_poses.Count - 1] - 1;
//                    while_branch_poses.RemoveAt(while_branch_poses.Count - 1);
//                    break;
//                case 0x06:
//                    main_memory[main_memory_cursor] = drivers.ExecuteInputFunc(ref main_memory, ref main_memory_cursor, input_func[input_func_cursor]);
//                    break;
//                case 0x07:
//                    drivers.ExecuteOutputFunc(ref main_memory, ref main_memory_cursor, output_func[output_func_cursor]);
//                    break;
//                case 0x08:
//                    input_func_cursor++;
//                    input_func_cursor %= (uint)input_func.Length;
//                    break;
//                case 0x09:
//                    input_func_cursor--;
//                    input_func_cursor %= (uint)input_func.Length;
//                    break;
//                case 0x0A:
//                    output_func_cursor++;
//                    output_func_cursor %= (uint)output_func.Length;
//                    break;
//                case 0x0B:
//                    output_func_cursor--;
//                    output_func_cursor %= (uint)output_func.Length;
//                    break;
//                case 0x0C:
//                    main_memory[main_memory_cursor] = ~main_memory[main_memory_cursor];
//                    break;
//                case 0x0D:
//                    main_memory[main_memory_cursor] |= main_memory[main_memory_cursor + 1];
//                    break;
//                case 0x0E:
//                    main_memory[main_memory_cursor] &= main_memory[main_memory_cursor + 1];
//                    break;
//                case 0x0F:
//                    main_memory[main_memory_cursor] ^= main_memory[main_memory_cursor + 1];
//                    break;
//                case 0x10:
//                    main_memory[main_memory_cursor] = main_memory[main_memory_cursor] == main_memory[main_memory_cursor + 1] ? (uint)1 : 0;
//                    break;
//                case 0x11:
//                    main_memory[main_memory_cursor] = main_memory[main_memory_cursor] > main_memory[main_memory_cursor + 1] ? (uint)1 : 0;
//                    break;
//                case 0x12:
//                    input_func[input_func_cursor] = main_memory[main_memory_cursor];
//                    break;
//                case 0x13:
//                    output_func[output_func_cursor] = main_memory[main_memory_cursor];
//                    break;
//                case 0x14:
//                    tasks.Remove(this);
//                    break;
//                case 0x15:
//                    while_branch_poses.RemoveAt(while_branch_poses.Count - 1);
//                    pos++;
//                    while (getCommand(pos) != 0x05)
//                    {
//                        pos++;
//                    }
//                    break;
//                case 0x16:
//                    main_memory[main_memory_cursor] <<= 1;
//                    break;
//                case 0x17:
//                    main_memory[main_memory_cursor] >>= 1;
//                    break;
//                case 0x18:
//                    copied_value = main_memory[main_memory_cursor];
//                    break;
//                case 0x19:
//                    main_memory[main_memory_cursor] = copied_value;
//                    break;
//                case 0x1A:
//                    main_memory[main_memory_cursor] = 0;
//                    break;
//            }
//            pos++;
//        }

//        public static void Run()
//        {
//            tasks[tasks.Count - 1].Execute();
//        }
//    }
//}
