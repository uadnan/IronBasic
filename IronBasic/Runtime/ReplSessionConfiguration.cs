using System;
using System.IO;

namespace IronBasic.Runtime
{
    /// <summary>
    /// Configuration for <see cref="ReplSession"/>
    /// </summary>
    public class ReplSessionConfiguration
    {
        #region Seal

        private bool _sealed;

        internal void Seal()
        {
            _sealed = true;
        }

        protected void VerifyCanSet()
        {
            if (_sealed)
                throw new InvalidOperationException("Settings can't be modified once owned by BasicEngine");
        }

        #endregion

        #region Grammar

        private Grammar _grammar;

        public Grammar Grammar
        {
            get { return _grammar; }
            set
            {
                VerifyCanSet();
                _grammar = value;
            }
        }

        #endregion

        #region PcjrTerm

        private string _pcjrTerm;

        /// <summary>
        /// Program for Term Command in case <see cref="Grammar"/> is <see cref="Grammar.Pcjr"/>
        /// </summary>
        public string PcjrTerm
        {
            get { return _pcjrTerm; }
            set
            {
                VerifyCanSet();
                _pcjrTerm = value;
            }
        }

        #endregion

        #region AllCodePoke

        private bool _allCodePoke;

        public bool AllCodePoke
        {
            get { return _allCodePoke; }
            set
            {
                VerifyCanSet();
                _allCodePoke = value;
            }
        }

        #endregion

        #region MaxMemory

        private int _maxMemory = 65534;

        public int MaxMemory
        {
            get { return _maxMemory; }
            set
            {
                VerifyCanSet();
                _maxMemory = value;
            }
        }

        #endregion

        #region Double Precision

        private bool _doublePrecision;

        /// <summary>
        /// Gets or sets use of double-precision in power operator
        /// </summary>
        public bool DoublePrecision
        {
            get { return _doublePrecision; }
            set
            {
                VerifyCanSet();
                _doublePrecision = value;
            }
        }

        #endregion

        #region SerialBufferSize

        private int _serialBufferSize = 128;

        public int SerialBufferSize
        {
            get { return _serialBufferSize; }
            set
            {
                VerifyCanSet();
                _serialBufferSize = value;
            }
        }

        #endregion

        #region MaxAllowedLineNumber

        private int _maxAllowedLineNumber = 65535;

        public int MaxAllowedLineNumber
        {
            get { return _maxAllowedLineNumber; }
            set
            {
                VerifyCanSet();
                _maxAllowedLineNumber = value;
            }
        }

        #endregion

        #region ReservedMemory

        private int _reservedMemory = 3429;

        public int ReservedMemory
        {
            get { return _reservedMemory; }
            set
            {
                VerifyCanSet();
                _reservedMemory = value;
            }
        }

        #endregion

        #region BreakOnControlC

        private bool _breakOnControlC = true;

        public bool BreakOnControlC
        {
            get { return _breakOnControlC; }
            set
            {
                VerifyCanSet();
                _breakOnControlC = value;
            }
        }

        #endregion

        #region IgnoreCaps

        private bool _ignoreCaps = true;

        public bool IgnoreCaps
        {
            get { return _ignoreCaps; }
            set
            {
                VerifyCanSet();
                _ignoreCaps = value;
            }
        }

        #endregion

        #region Input

        private TextReader _input;

        public TextReader Input
        {
            get { return _input; }
            set
            {
                VerifyCanSet();
                _input = value;
            }
        }

        #endregion

        #region Output

        private TextWriter _output;

        public TextWriter Output
        {
            get { return _output; }
            set
            {
                VerifyCanSet();
                _output = value;
            }
        }

        #endregion

        #region VideoMemory

        private int _videoMemory = 262144;

        public int VideoMemory
        {
            get { return _videoMemory; }
            set
            {
                VerifyCanSet();
                _videoMemory = value;
            }
        }

        #endregion
    }
}