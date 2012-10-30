using System;
using System.Collections.Generic;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateAccountCommand : DropboxCommand
    {
        private String _eMail = "";
        private String _firstName = "";
        private String _lastName = "";
        private String _password = "";
        private Boolean _statusInResponse;

        /// <summary>
        /// 
        /// </summary>
        public String EMail
        {
            get { return _eMail; }
            set { _eMail = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String Password
        {
            get { return _password; }
            set { _password = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Boolean StatusInResponse
        {
            get { return _statusInResponse; }
            set { _statusInResponse = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, string> CreateParameters()
        {
            var d = new Dictionary<String, String>();
            d["email"] = this.EMail;
            d["first_name"] = this.FirstName;
            d["last_name"] = this.LastName;
            d["password"] = this.Password;
            d["status_in_response"] = this.StatusInResponse.ToString().ToLower();
            return d;
        }
    }
}
