
namespace ScriptEngine.HostedScript.Library
{
    public class ProcessContext : AutoContext<ProcessContext>, IDisposable
    {

        /// <summary>
        /// Перемещает файл из одного расположения в другое.
        /// </summary>
        /// <param name="source">Имя файла-источника</param>
        /// <param name="destination">Имя файла приемника</param>
        [ContextMethod("ПереместитьФайл", "MoveFile")]
        public void MoveFile(string source, string destination)
        {
            System.IO.File.Move(source, destination);
        }
    }
}
