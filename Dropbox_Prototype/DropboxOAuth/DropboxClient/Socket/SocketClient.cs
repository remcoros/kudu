using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using HigLabo.Net.Internal;

namespace HigLabo.Net
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SocketClient
    {
        /// 改行文字列の値です。
        /// <summary>
        /// 改行文字列の値です。
        /// </summary>
        public static readonly String NewLine = "\r\n";
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AsyncSocketCallErrorEventArgs> Error;
        private String _UserName = "";
        private String _Password = "";
        private String _ServerName = "";
        private Int32 _Port = -1;
        private Boolean _Ssl = false;
        private Encoding _RequestEncoding = Encoding.UTF8;
        private Encoding _ResponseEncoding = Encoding.UTF8;
        private AutoResetEvent _SendDone = new AutoResetEvent(false);
        private AutoResetEvent _GetResponseDone = new AutoResetEvent(false);
        private Boolean _Commnicating = false;
        /// <summary>
        /// 接続が有効かどうかを示す値を取得します。
        /// </summary>
        public Boolean Connected
        {
            get
            {
#if NETFX_CORE
                if (this.Socket == null)
#else
                if (this.Socket == null || this.Socket.Connected == false)
#endif
                {
                    return false;
                }
                return true;
            }
        }
        /// 認証に使用するユーザー名を取得または設定します。
        /// <summary>
        /// Get or set UserName.
        /// 認証に使用するユーザー名を取得または設定します。
        /// </summary>
        public String UserName
        {
            get { return this._UserName; }
            set { this._UserName = value; }
        }
        /// 認証に使用するパスワードを取得または設定します。
        /// <summary>
        /// Get or set password.
        /// 認証に使用するパスワードを取得または設定します。
        /// </summary>
        public String Password
        {
            get { return this._Password; }
            set { this._Password = value; }
        }
        /// POP3メールサーバーのサーバー名を取得または設定します。
        /// <summary>
        /// Get or set server.
        /// POP3メールサーバーのサーバー名を取得または設定します。
        /// </summary>
        public String ServerName
        {
            get { return this._ServerName; }
            set { this._ServerName = value; }
        }
        /// 通信に使用するPort番号を取得または設定します。
        /// <summary>
        /// Get or set port.
        /// 通信に使用するPort番号を取得または設定します。
        /// </summary>
        public Int32 Port
        {
            get { return this._Port; }
            set { this._Port = value; }
        }
        /// 通信をSSLで暗号化するかどうかを示す値を取得または設定します。
        /// <summary>
        /// Get or set use ssl protocol.
        /// 通信をSSLで暗号化するかどうかを示す値を取得または設定します。
        /// </summary>
        public Boolean Ssl
        {
            get { return this._Ssl; }
            set { this._Ssl = value; }
        }
        /// 送信データのエンコーディングを取得または設定します。
        /// <summary>
        /// 送信データのエンコーディングを取得または設定します。
        /// </summary>
        public Encoding RequestEncoding
        {
            get { return _RequestEncoding; }
            set { _RequestEncoding = value; }
        }
        /// 受信データのエンコーディングを取得または設定します。
        /// <summary>
        /// 受信データのエンコーディングを取得または設定します。
        /// </summary>
        public Encoding ResponseEncoding
        {
            get { return _ResponseEncoding; }
            set { _ResponseEncoding = value; }
        }
        /// <summary>
        /// Get specify value whether communicating to server or not.
        /// Between send command and finish get all response data,this property get true.
        /// </summary>
        public Boolean Commnicating
        {
            get { return this._Commnicating; }
            protected set { _Commnicating = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        protected AutoResetEvent SendDone
        {
            get { return _SendDone; }
        }
        /// <summary>
        /// 
        /// </summary>
        protected AutoResetEvent GetResponseDone
        {
            get { return _GetResponseDone; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="port"></param>
        public SocketClient(String serverName, Int32 port)
        {
            this.ServerName = serverName;
            this.Port = port;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public SocketClient(String serverName, Int32 port, String userName, String password)
        {
            this.ServerName = serverName;
            this.Port = port;
            this.UserName = userName;
            this.Password = password;
        }
        /// <summary>
        /// Get string about mail account information.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(256);

            sb.AppendFormat("ServerName:{0}", this.ServerName);
            sb.AppendLine();
            sb.AppendFormat("Port:{0}", this.Port);
            sb.AppendLine();
            sb.AppendFormat("UserName:{0}", this.UserName);
            sb.AppendLine();
            sb.AppendFormat("SSL:{0}", this.Ssl);

            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        protected void OnError(Exception exception)
        {
            var eh = this.Error;
            if (eh != null)
            {
                eh(this, new AsyncSocketCallErrorEventArgs(exception));
            }
        }
        /// 終了処理を実行し、システムリソースを解放します。
        /// <summary>
        /// dipose and release system resoures.
        /// 終了処理を実行し、システムリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                this.DisposeSocket();
                ((IDisposable)this.SendDone).Dispose();
                ((IDisposable)this.GetResponseDone).Dispose();
            }
        }
    }
}
