using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
namespace Carrom.Headless.Networking
{
    public sealed class WebSocketServer
    {
        const int MaxHeaderBytes=16384,MaxFrameBytes=4*1024*1024;
        readonly int _port;readonly Action<WebSocketConnection,string> _message;TcpListener _listener;Thread _acceptThread;volatile bool _running;
        public WebSocketServer(int port,Action<WebSocketConnection,string> message){_port=port;_message=message;}
        public void Start(){if(_running)return;_listener=new TcpListener(IPAddress.Any,_port);_listener.Start();_running=true;_acceptThread=new Thread(AcceptLoop){IsBackground=true,Name="WebSocketAccept"};_acceptThread.Start();}
        void AcceptLoop(){while(_running){try{var tcp=_listener.AcceptTcpClient();tcp.NoDelay=true;ThreadPool.QueueUserWorkItem(_=>Handle(tcp));}catch{if(_running)Thread.Sleep(10);}}}
        void Handle(TcpClient tcp){try{using(tcp){var c=new WebSocketConnection(tcp,MaxFrameBytes);c.Handshake();c.Run(msg=>_message(c,msg));}}catch{}}
        public void Stop(){_running=false;try{_listener?.Stop();}catch{}if(_acceptThread!=null&&_acceptThread.IsAlive&&Thread.CurrentThread!=_acceptThread)try{_acceptThread.Join(1000);}catch{}}
    }
    public sealed class WebSocketConnection
    {
        readonly TcpClient _tcp;readonly NetworkStream _stream;readonly object _sendGate=new object();readonly int _maxFrameBytes;volatile bool _closed;
        public WebSocketConnection(TcpClient tcp,int maxFrameBytes){_tcp=tcp;_stream=tcp.GetStream();_tcp.NoDelay=true;_maxFrameBytes=maxFrameBytes;}
        public void Handshake(){var request=ReadHttpHeader();if(!request.StartsWith("GET ",StringComparison.Ordinal))throw new InvalidDataException("WebSocket upgrade requires GET.");string key=null;foreach(var line in request.Split(new[]{"\r\n"},StringSplitOptions.None)){if(line.StartsWith("Sec-WebSocket-Key:",StringComparison.OrdinalIgnoreCase))key=line.Substring(18).Trim();}if(string.IsNullOrEmpty(key))throw new InvalidDataException("Missing Sec-WebSocket-Key");using(var sha=SHA1.Create()){var accept=Convert.ToBase64String(sha.ComputeHash(Encoding.ASCII.GetBytes(key+"258EAFA5-E914-47DA-95CA-C5AB0DC85B11")));var response="HTTP/1.1 101 Switching Protocols\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Accept: "+accept+"\r\n\r\n";var bytes=Encoding.ASCII.GetBytes(response);_stream.Write(bytes,0,bytes.Length);}}
        string ReadHttpHeader(){var sb=new StringBuilder();int state=0;while(true){int b=_stream.ReadByte();if(b<0)throw new EndOfStreamException();sb.Append((char)b);if((state==0&&b=='\r')||(state==2&&b=='\r'))state++;else if((state==1&&b=='\n')||(state==3&&b=='\n')){if(state==3)break;state++;}else state=0;if(sb.Length>MaxHeaderBytes)throw new InvalidDataException("HTTP header too large");}return sb.ToString();}
        public void Run(Action<string> onText){while(!_closed){var frame=ReadFrame();if(frame.opcode==8){Close();break;}if(frame.opcode==9){SendControl(10,frame.payload);continue;}if(frame.opcode==1)onText(Encoding.UTF8.GetString(frame.payload));else if(frame.opcode!=10)throw new InvalidDataException("Only text, ping, pong and close frames are supported.");}}
        public void Send(string text){if(_closed)return;var payload=Encoding.UTF8.GetBytes(text);lock(_sendGate){WriteFrame(1,payload);}}
        void SendControl(byte opcode,byte[] payload){lock(_sendGate)WriteFrame(opcode,payload);}
        void WriteFrame(byte opcode,byte[] payload){if(payload.LongLength>_maxFrameBytes)throw new InvalidDataException("Frame too large.");_stream.WriteByte((byte)(0x80|opcode));if(payload.Length<126)_stream.WriteByte((byte)payload.Length);else if(payload.Length<=65535){_stream.WriteByte(126);_stream.WriteByte((byte)(payload.Length>>8));_stream.WriteByte((byte)payload.Length);}else{_stream.WriteByte(127);for(int i=7;i>=0;i--)_stream.WriteByte((byte)(payload.LongLength>>(8*i)));}_stream.Write(payload,0,payload.Length);_stream.Flush();}
        Frame ReadFrame(){int b0=_stream.ReadByte(),b1=_stream.ReadByte();if(b0<0||b1<0)throw new EndOfStreamException();bool masked=(b1&0x80)!=0;long len=b1&0x7f;if((b0&0x40)!=0)throw new InvalidDataException("Fragmented WebSocket messages are not supported.");if(len==126)len=(_stream.ReadByte()<<8)|_stream.ReadByte();else if(len==127){len=0;for(int i=0;i<8;i++){var b=_stream.ReadByte();if(b<0)throw new EndOfStreamException();len=(len<<8)|b;}}if(len<0||len>_maxFrameBytes)throw new InvalidDataException("Frame too large");if(!masked)throw new InvalidDataException("Client WebSocket frames must be masked.");if((b0&0x80)==0&&((b0&0x0f)==9||(b0&0x0f)==10)&&len>125)throw new InvalidDataException("Control frame too large");var mask=ReadExact(4);var payload=ReadExact((int)len);for(int i=0;i<payload.Length;i++)payload[i]=(byte)(payload[i]^mask[i%4]);return new Frame{opcode=(byte)(b0&15),payload=payload};}
        byte[] ReadExact(int n){var b=new byte[n];int o=0;while(o<n){int r=_stream.Read(b,o,n-o);if(r<=0)throw new EndOfStreamException();o+=r;}return b;}
        public void Close(){if(_closed)return;try{lock(_sendGate){if(!_closed)WriteFrame(8,Array.Empty<byte>());}}catch{} _closed=true;try{_tcp.Close();}catch{}}
        struct Frame{public byte opcode;public byte[] payload;}
    }
}
