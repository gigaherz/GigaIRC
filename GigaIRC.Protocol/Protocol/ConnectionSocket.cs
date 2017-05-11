using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GigaIRC.Protocol
{
    class ConnectionSocket
    {
        private TcpClient _ircConnection;
        private StreamReader _ircReader;
        private StreamWriter _ircWriter;

        private readonly Connection _connection;
        private readonly Logger _logger;

        public bool IsConnected => _ircConnection != null && _ircConnection.Connected;

        public ConnectionSocket(Connection connection)
        {
            _connection = connection;
            _logger = connection.Logger;
        }

        private bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            _logger.LogLine(2, "Certificate error: {0}", sslPolicyErrors);

            return true;
        }

        public async void Connect()
        {
            try
            {
                bool secure = false;
                int port;
                var address = _connection.Server.Address;

                if (_connection.Server.SecurePortRangeCollection.Count > 0)
                {
                    secure = true;
                    port = _connection.Server.SecurePortRangeCollection[0].Item1;
                    _logger.LogLine(1, "Opening Secure Connection...");
                }
                else
                {
                    port = _connection.Server.PortRangeCollection[0].Item1;
                    _logger.LogLine(1, "Opening Connection...");
                }

                _connection.ChangeState(ConnectionState.Connecting);

                _ircConnection = new TcpClient();
                await _ircConnection.ConnectAsync(address, port);

                _connection.UpdateRemote((IPEndPoint)_ircConnection.Client.RemoteEndPoint);

                lock (this)
                {
                    Stream stream = _ircConnection.GetStream();

                    //if (_connection.Server.SecurePortRanges.Length > 0)
                    if (secure)
                    {
                        // Create an SSL stream that will close the client's stream.
                        var sslStream = new SslStream(
                            stream,
                            false,
                            ValidateServerCertificate,
                            null
                        );

                        sslStream.AuthenticateAsClient(address);

                        stream = sslStream;
                    }

                    _ircReader = new StreamReader(stream, Encoding.UTF8);
                    _ircWriter = new StreamWriter(stream, Encoding.UTF8);
                }

                _logger.LogLine(1, "Sending user info...");
                _connection.ChangeState(ConnectionState.Identifying);

                await SendLine("");
                await SendLine("CAP LS");
                await SendLine("USER " + _connection.Identity.Username + " "
                    + _connection.Identity.Username + " * :"
                    + _connection.Identity.FullName);
                await Register(_connection.Identity.NicknameList[0]);

                _connection.ChangeState(ConnectionState.WaitingMOTD);

                while (_ircConnection.Connected)
                {
                    string line;
                    try
                    {
                        var task = _ircReader.ReadLineAsync();
                        line = await task;
                        line = line?.Trim().Replace('\t', ' ');
                    }
                    catch (Exception e)
                    {
                        _logger.LogLine(2, "ERROR: Exception caught: {0}", e);
                        break;
                    }

                    if (string.IsNullOrEmpty(line))
                        continue;

                    _logger.LogLine(0, "[{0} -->] {1}", _connection.Server, line);
                    var cmd = new Command(line);

                    await _connection.CommandReceived(cmd);
                }

                _connection.ChangeState(ConnectionState.Disconnected);

            }
            catch (Exception e)
            {
                _logger.LogLine(2, "Connection exception: " + e);
                Close();
            }
        }

        internal async Task Register(string nickname)
        {
            await SendLine("NICK " + _connection.Identity.NicknameList[0]);
            _logger.LogLine(1, "Awaiting reply...");
        }

        public async void Close()
        {
            if (_ircConnection != null && _ircConnection.Connected)
            {
                _ircWriter.WriteLine("QUIT :Program Closing.");
                await _ircWriter.FlushAsync();
                _ircConnection.Close();
            }
        }

        public async Task SendLine(string line)
        {
            if (_ircConnection != null && _ircConnection.Connected)
            {
                _logger.LogLine(0, "[--> " + _connection.Server + "] " + line);
                await _ircWriter.WriteLineAsync(line);
                await _ircWriter.FlushAsync();
            }
        }
    }
}
