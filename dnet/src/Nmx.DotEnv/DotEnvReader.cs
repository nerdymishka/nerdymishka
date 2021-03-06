using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NerdyMishka.Text.DotEnv;

namespace NerdyMishka
{
    public static class DotEnvReader
    {
        public static void ConfigureEnvironment(string filename = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                var path = typeof(DotEnvReader).Assembly.Location;
                var parent = System.IO.Path.GetDirectoryName(path);
                filename = System.IO.Path.Combine(parent, ".env");
            }

            if (!System.IO.File.Exists(filename))
                throw new FileNotFoundException($".env not found at {filename}");

            var variables = ReadFile(filename);
            foreach (var key in variables.Keys)
            {
                System.Environment.SetEnvironmentVariable(key,
                    variables[key],
                    EnvironmentVariableTarget.Process);
            }
        }

        public static IDictionary<string, string> ReadFile(string filename, Encoding encoding = null)
        {
            if (filename is null)
                throw new ArgumentNullException(nameof(filename));

            encoding ??= new UTF8Encoding(false);
            using (var sr = new StreamReader(filename, encoding))
            using (var parser = new Parser())
            {
                var tokens = Lexer.Tokenize(sr);
                sr.Close();
                return parser.Parse(tokens);
            }
        }

        public static IDictionary<string, string> ReadString(string content)
        {
            if (content is null)
                throw new ArgumentNullException(nameof(content));

            using (var sr = new StringReader(content))
            using (var parser = new Parser())
            {
                var tokens = Lexer.Tokenize(sr);
                sr.Close();
                return parser.Parse(tokens);
            }
        }
    }
}