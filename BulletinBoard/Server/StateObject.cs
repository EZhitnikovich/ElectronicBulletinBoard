using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

public class StateObject
{
    public const int BufferSize = 1024;

    public byte[] Buffer = new byte[BufferSize];

    public StringBuilder Sb = new StringBuilder();

    public Socket WorkSocket = null;
}