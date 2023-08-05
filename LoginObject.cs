namespace SeleniumTikTok
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// LoginObject
    /// </summary>
    public class LoginObject
    {
        /// <summary>
        /// Khởi tạo object login
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="username">Tài khoản</param>
        /// <param name="password">Mật khẩu</param>
        /// <param name="videoPath">Thư mục video</param>
        /// <param name="caption">Tiêu đề</param>
        /// <param name="isHeadless"> Chạy ẩn danh</param>
        /// <param name="dateUp">Ngày up</param>
        public LoginObject(int id,string username, string password, string videoPath, string caption, bool isHeadless, string dateUp)
        {
            Id = id;
            Username = username;
            Password = password;
            VideoPath = videoPath;
            Caption = caption;
            this.isHeadless = isHeadless;
            DateUp = dateUp;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Tài khoản
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; }    
        /// <summary>
        /// Đường dẫn thư mục video
        /// </summary>
        public string VideoPath { get; set; }
        /// <summary>
        /// Tiêu đề của video
        /// </summary>
        public string Caption { get; set; }   
        /// <summary>
        /// Cờ chạy ẩn danh
        /// </summary>
        public bool isHeadless { get; set; }
        /// <summary>
        /// Ngày lên lịch
        /// </summary>
        public string DateUp { get; set; }

    }
}
