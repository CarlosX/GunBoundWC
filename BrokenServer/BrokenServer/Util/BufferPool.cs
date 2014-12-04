using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BrokenServer
{
    public partial class Systems
    {
        public class BufferPool
        {
            private Queue m_Buffers;
            private int m_BufferSize;

            public BufferPool(int capacity, int bufferSize)
            {
                this.m_BufferSize = bufferSize;
                this.m_Buffers = new Queue(capacity);
                for (int i = 0; i < capacity; i++)
                {
                    this.m_Buffers.Enqueue(new byte[bufferSize]);
                }
            }

            public byte[] AquireBuffer()
            {
                if (this.m_Buffers.Count > 0)
                {
                    lock (this.m_Buffers)
                    {
                        if (this.m_Buffers.Count > 0)
                        {
                            return (byte[])this.m_Buffers.Dequeue();
                        }
                    }
                }
                return new byte[this.m_BufferSize];
            }

            public void ReleaseBuffer(byte[] m_Buffer)
            {
                lock (this.m_Buffers)
                {
                    this.m_Buffers.Enqueue(m_Buffer);
                }
            }
        }
    }
}
