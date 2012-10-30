using System;
using HigLabo.Net.Extensions;

namespace HigLabo.Net.Dropbox
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountInfo : ResponseObject
    {
        private String _referralLink = "";
        private String _displayName = "";
        private String _country = "";
        private String _eMail = "";

        /// <summary>
        /// 
        /// </summary>
        public String ReferralLink
        {
            get { return _referralLink; }
            set { _referralLink = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public long Uid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String Country
        {
            get { return _country; }
            set { _country = value; }
        }
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
        public QuotaInfo QuotaInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonText"></param>
        public AccountInfo(String jsonText)
        {
            this.SetProperty(jsonText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonText"></param>
        public override void SetProperty(String jsonText)
        {
            var d = this.SetData(jsonText);
            this.ReferralLink = d.ToString("referral_link");
            this.DisplayName = d.ToString("display_name");
            this.Uid = d.ToInt64("uid") ?? this.Uid;
            this.Country = d.ToString("country");
            this.EMail = d.ToString("email");
            this.QuotaInfo = new QuotaInfo(d.ToString("quota_info"));
        }
    }
}
