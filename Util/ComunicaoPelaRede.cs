using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Windows;
using VMIClientePix.ViewModel;
using static VMIClientePix.ViewModel.MainWindowViewModel;

namespace VMIClientePix.Util
{
    public class ComunicaoPelaRede
    {
        private static Socket socket;
        private static byte[] mensagem = new byte[128];
        private static IList<Socket> usuariosEmSessao = new List<Socket>();
        private static event AposSalvarCobrancaEventHandler AposSalvarCobranca;
        private static Timer timerIniciaListener;

        public static void IniciaServidor(AposSalvarCobrancaEventHandler aposSalvarCobranca)
        {
            try
            {
                AposSalvarCobranca = aposSalvarCobranca;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 3500);
                socket.Bind(endPoint);
                socket.Listen();
                socket.BeginAccept(AcceptCallback, null);
                Console.WriteLine("INICIADO SOCKET DE SERVIDOR\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir socket de servidor. A aplicação ainda funcionará normalmente, mas sem alguns recursos.\n\n" + ex.Message, "Conexão ao Servidor de Comunicação", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void IniciaListener(AposSalvarCobrancaEventHandler aposSalvarCobranca)
        {
            AposSalvarCobranca = aposSalvarCobranca;
            ConectaComServidor();
            timerIniciaListener = new Timer(1000);
            timerIniciaListener.AutoReset = true;
            timerIniciaListener.Elapsed += TimerIniciaListener_Elapsed;
            timerIniciaListener.Start();
        }

        private static void ConectaComServidor()
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(Credentials.GetLocalHost()), 3500);
                socket.Connect(endPoint);
                socket.BeginReceive(mensagem, 0, mensagem.Length, SocketFlags.None, ReceiveCallback, socket);
                Console.WriteLine("INICIADO SOCKET DE CLIENTE\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar com servidor de comunicação local. A aplicação ainda funcionará normalmente, mas sem alguns recursos.\n\n" + ex.Message, "Conexão ao Servidor de Comunicação", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void TimerIniciaListener_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsSocketConnected(socket))
            {
                timerIniciaListener.Stop();
                ConectaComServidor();
                timerIniciaListener.Start();
            }
        }

        /// <summary>
        /// Usado quando a instância dessa aplicação estiver rodando como servidor.
        /// </summary>
        /// <param name="ar"></param>
        private static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                //Socket que solicitou a conexão
                Socket socket_cliente = socket.EndAccept(ar);

                //Começa a ouvir este cliente de forma assíncrona
                socket_cliente.BeginReceive(mensagem, 0, mensagem.Length, SocketFlags.None, ReceiveCallback, socket_cliente);
                usuariosEmSessao.Add(socket_cliente);

                Console.WriteLine("CLIENTE CONECTOU AO SERVIDOR\n");

                //Retoma a operação assíncrona de esperar uma conexão de cliente
                socket.BeginAccept(AcceptCallback, null);
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Ouvinte foi Disposed");
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            Socket socketReceive = (Socket)ar.AsyncState;

            if (IsSocketConnected(socketReceive))
            {
                int recebidoLength = socketReceive.EndReceive(ar);

                byte[] recebidoEmByte = new byte[recebidoLength];

                Array.Copy(mensagem, recebidoEmByte, recebidoLength);

                string mensagemRecebida = Encoding.UTF8.GetString(recebidoEmByte);

                Console.WriteLine($"RECEBIDA MENSAGEM: {mensagemRecebida}\n");

                if (mensagemRecebida.StartsWith("listarcobrancas"))
                {
                    Console.WriteLine("COMANDO LISTAR COBRANÇAS\n");

                    var txid = mensagemRecebida.Split(' ')[1];

                    AposSalvarCobrancaEventArgs e = new AposSalvarCobrancaEventArgs
                    {
                        TxIdCobranca = txid
                    };

                    AposSalvarCobranca.Invoke(e);

                    //Somente executa se for servidor
                    foreach (var s in usuariosEmSessao)
                    {
                        if (IsSocketConnected(s) && s != socketReceive)
                        {
                            Console.WriteLine($"REPLICANDO MENSAGEM RECEBIDA PARA CLIENTE CONECTADO: {mensagemRecebida}\n");
                            socketReceive.BeginSend(mensagem, 0, mensagem.Length, SocketFlags.None, SendCallback, socketReceive);
                        }
                    }
                }

                socketReceive.BeginReceive(mensagem, 0, mensagem.Length, SocketFlags.None, ReceiveCallback, socketReceive);
            }
        }

        public static void NotificaListarCobrancas(string txid)
        {
            string mensagem = $"listarcobrancas {txid}";
            byte[] mensagemEmByte = Encoding.UTF8.GetBytes(mensagem);

            if (socket != null)
            {
                //Somente se for servidor
                if (usuariosEmSessao.Count > 0)
                {
                    foreach (var s in usuariosEmSessao)
                    {
                        if (IsSocketConnected(s))
                        {
                            Console.WriteLine($"NOTIFICANDO CLIENTES CONECTADOS\n");
                            s.BeginSend(mensagemEmByte, 0, mensagemEmByte.Length, SocketFlags.None, SendCallback, s);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"NOTIFICANDO SERVIDOR\n");
                    if (IsSocketConnected(socket))
                        socket.BeginSend(mensagemEmByte, 0, mensagemEmByte.Length, SocketFlags.None, SendCallback, socket);
                }
            }
        }

        /// <summary>
        /// Responsável por processar os envios do servidor para clientes
        /// </summary>
        /// <param name="asyncResult">Representa o estado da operação assíncrona</param>
        private static void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket_cliente = (Socket)asyncResult.AsyncState;
            socket_cliente.EndSend(asyncResult);
        }

        static bool IsSocketConnected(Socket s)
        {
            try
            {
                bool part1 = s.Poll(1000, SelectMode.SelectRead);
                bool part2 = s.Available == 0;
                if ((part1 && part2) || !s.Connected)
                    return false;
                else
                    return true;
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("Socket não está conectado porque foi disposed. " + ex.Message);
                return false;
            }
        }

        public static void FecharSocket()
        {
            foreach (var s in usuariosEmSessao)
            {
                if (s != null && IsSocketConnected(s))
                {
                    s.Shutdown(SocketShutdown.Both);
                    s.Close();
                    s.Dispose();
                }
            }

            if (socket != null && IsSocketConnected(socket))
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
            }

            if (timerIniciaListener != null)
            {
                timerIniciaListener.Stop();
                timerIniciaListener.Close();
                timerIniciaListener.Dispose();
            }
        }
    }
}
