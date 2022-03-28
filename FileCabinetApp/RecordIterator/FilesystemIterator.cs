using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileCabinetApp.RecordIterator
{
    /// <summary>
    /// FilesystemIterator.
    /// </summary>
    public class FilesystemIterator : IEnumerator<FileCabinetRecord>
    {
        private readonly int recordSize;
        private readonly int[] offset;
        private readonly FileStream fileStream;
        private readonly Func<BinaryReader, int[], FileCabinetRecord> readBinaryRecord;
        private int position;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="collection">collection.</param>
        /// <param name="recordSize">recordSize.</param>
        /// <param name="offset">offset.</param>
        /// <param name="readBinaryRecord">Func Delegate.</param>
        public FilesystemIterator(
            FileCabinetFilesystemService collection,
            int recordSize,
            int[] offset,
            Func<BinaryReader, int[], FileCabinetRecord> readBinaryRecord)
        {
            this.recordSize = recordSize;
            this.offset = offset;
            this.fileStream = collection.GetFileStream();
            this.readBinaryRecord = readBinaryRecord;
            this.position = -recordSize;
        }

        /// <summary>
        /// Gets current element.
        /// </summary>
        /// <value>
        /// Current element.
        /// </value>
        public FileCabinetRecord Current
        {
            get
            {
                byte[] readBytes = new byte[this.fileStream.Length];
                this.fileStream.Seek(0, SeekOrigin.Begin);
                this.fileStream.Read(readBytes, 0, readBytes.Length);
                using MemoryStream memoryStream = new (readBytes);
                using BinaryReader binaryReader = new (memoryStream);
                memoryStream.Seek(this.position, SeekOrigin.Begin);

                FileCabinetRecord record;
                short reserved = binaryReader.ReadInt16();

                if ((reserved >> 2 & 1) == 1)
                {
                    record = null;
                }
                else
                {
                    record = this.readBinaryRecord(binaryReader, this.offset);
                }

                return record;
            }
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>
        /// Record.
        /// </value>
        object IEnumerator.Current => this.Current;

        /// <summary>
        /// Checks the end of the file.
        /// </summary>
        /// <returns>boolean.</returns>
        public bool MoveNext()
        {
            this.position += this.recordSize;

            if (this.position < this.fileStream.Length)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            this.position = -this.recordSize;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing"> Indicates whether the method call comes from a Dispose method or from a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    return;
                }

                if (this.fileStream != null)
                {
                    this.fileStream.Close();
                    this.fileStream.Dispose();
                }
            }

            this.disposedValue = true;
        }
    }
}
