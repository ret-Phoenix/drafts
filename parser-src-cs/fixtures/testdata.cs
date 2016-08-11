/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System.Diagnostics;

namespace ScriptEngine.HostedScript.Library
{
    /// <summary>
    /// Позволяет управлять процессом операционной системы. Получать текст из стандартных потоков,
    /// проверять активность, pid, завершать процесс и т.п.
    /// </summary>
    [ContextClass("Процесс", "Process")]
    public class ProcessContext : AutoContext<ProcessContext>, IDisposable
    {
        private System.Diagnostics.Process _p;
        private StdTextReadStream _stdOutContext;
        private StdTextReadStream _stdErrContext;
        private StdTextWriteStream _stdInContext;

        private IValue _outputEncoding;



        /// <summary>
        /// Устанавливает кодировку в которой будут считываться стандартные потоки вывода и ошибок.
        /// </summary>
        [ContextProperty("КодировкаВывода", "OutputEncoding")]
        public IValue OutputEncoding
        {
            get
            {
                return _outputEncoding;
            }
        }

        /// <summary>
        /// Не используется. Реализован для совместимости API с 1С:Предприятие
        /// </summary>
        /// <returns>Неопределено</returns>
        [ContextProperty("Параметры", "Parameters")]
        public IValue Parameters
        {
            get
            {
                return ValueFactory.Create();
            }
            set { }
        }

        [ContextProperty("АвтоматическийРазмер", "AutoSize")]
        public bool AutoSize
        {
            get { return _item.AutoSize; }
            set { _item.AutoSize = value; }
        }

        [ContextProperty("Имя", "Name")]
        public string Name
        {
            get { return this._name; }
            set {

                this._parent.Items.renameElement(this._name, value);
                this._name = value;
            }
        }

        private ProcessContext(System.Diagnostics.Process p, IValue encoding)
        {
            _p = p;
            _outputEncoding = encoding;
        }

        public ProcessContext(System.Diagnostics.Process p):this(p, ValueFactory.Create())
        {
        }

        /// <summary>
        /// Перемещает файл из одного расположения в другое.
        /// </summary>
        /// <param name="source">Имя файла-источника</param>
        /// <param name="destination">Имя файла приемника</param>
        [ContextMethod("ПереместитьФайл", "MoveFile")]
        public void MoveFile(string source, string destination = "новыйпуть")
        {
            System.IO.File.Move(source, destination);
        }

        /// <summary>
        /// Сравнивает строки без учета регистра.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>-1 первая строка больше, 1 - вторая строка больше. 0 - строки равны</returns>
        [ContextMethod("СтрСравнить", "StrCompare")]
        public int StrCompare(string first="один", string second)
        {
            return String.Compare(first, second, true);
        }

        /// <summary>
        /// Подключает внешнюю сборку среды .NET (*.dll) и регистрирует классы 1Script, объявленные в этой сборке.
        /// Публичные классы, отмеченные в dll атрибутом ContextClass, будут импортированы аналогично встроенным классам 1Script.
        /// Загружаемая сборка должна ссылаться на сборку ScriptEngine.dll
        /// <example>
        /// ПодключитьВнешнююКомпоненту("C:\MyAssembly.dll");
        /// КлассИзКомпоненты = Новый КлассИзКомпоненты(); // тип объявлен внутри компоненты
        /// </example>
        /// </summary>
        /// <param name="dllPath">Путь к внешней компоненте</param>
        [ContextMethod("ПодключитьВнешнююКомпоненту", "AttachAddIn")]
        public void AttachAddIn(string dllPath)
        {
            var assembly = System.Reflection.Assembly.LoadFrom(dllPath);
            EngineInstance.AttachAssembly(assembly);
        }

        /// <summary>
        /// Возвращает каталог временных файлов ОС
        /// </summary>
        /// <returns>Строка. Путь к каталогу временных файлов</returns>
        [ContextMethod("КаталогВременныхФайлов", "TempFilesDir")]
        public string TempFilesDir()
        {
            return Path.GetTempPath();
        }

        /// <summary>
        /// Запустить процесс на выполнение.
        /// </summary>
        [ContextMethod("Запустить", "Start")]
        public void Start()
        {
            _p.Start();
        }

        public void Dispose()
        {

        }

        public static ProcessContext Create(string cmdLine, string currentDir = null, bool redirectOutput = false, bool redirectInput = false, IValue encoding = null)
        {

        }

        public static System.Diagnostics.ProcessStartInfo PrepareProcessStartupInfo(string cmdLine, string currentDir)
        {
        }

    }
}
