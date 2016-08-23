using System;
using IronBasic.Compilor;

namespace IronBasic.Runtime
{
    public sealed class BasicEngineSettings
    {
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
                VerifyAccess();
                _pcjrTerm = value;
            }
        }

        #endregion

        #region Grammar

        private Grammar _grammar;

        /// <summary>
        /// Gets of sets grammar/syntax
        /// </summary>
        public Grammar Grammar
        {
            get { return _grammar; }
            set
            {
                VerifyAccess();
                _grammar = value;
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
                VerifyAccess();
                _doublePrecision = value;
            }
        }

        #endregion

        #region Mutation

        private bool _sealed;

        internal void Seal()
        {
            _sealed = true;
        }

        private void VerifyAccess()
        {
            if (_sealed)
                throw new InvalidOperationException("Settings can't be modified once owned by BasicEngine");
        }

        #endregion
    }
}