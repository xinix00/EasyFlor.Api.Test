using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Test
{

    public interface ILogger
        : IDisposable
    {
        void WriteLine(string line);
    }
    public class ConsoleLogger
        : ILogger
    {
        public void Dispose()
        {
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
    public class EmptyLogger
        : ILogger
    {
        public void Dispose()
        {
        }

        public void WriteLine(string line)
        {
        }
    }

    public class Logger
        : ILogger
    {
        private StreamWriter _logfile;

        public Logger(string name)
        {
            _logfile = new StreamWriter($"{name} - {DateTime.Now.ToString("yyyy-MM-dd HHmmss")}.txt");
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
            _logfile.WriteLine(line);
            _logfile.Flush();
        }

        public void Dispose()
        {
            _logfile.Flush();
            _logfile.Close();
            _logfile.Dispose();
        }
    }
}
