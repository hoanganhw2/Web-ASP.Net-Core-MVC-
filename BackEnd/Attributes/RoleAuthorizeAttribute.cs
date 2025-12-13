using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BackEnd.Attributes
{
    /// <summary>
    /// Custom authorization attribute để kiểm tra role của user dựa trên session.
    /// Sử dụng: [RoleAuthorize("ADMIN")] hoặc [RoleAuthorize("ADMIN", "USER")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        /// <summary>
        /// Constructor nhận danh sách các role được phép truy cập.
        /// </summary>
        /// <param name="roles">Danh sách role names (ví dụ: "ADMIN", "USER")</param>
        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Lấy role_name từ session
            var roleName = context.HttpContext.Session.GetString("role_name");
            var userId = context.HttpContext.Session.GetInt32("id");

            // Kiểm tra user đã đăng nhập chưa
            if (userId == null || string.IsNullOrEmpty(roleName))
            {
                // Chưa đăng nhập -> redirect đến trang Login
                context.Result = new RedirectToActionResult("Login", "Users", null);
                return;
            }

            // Nếu không có role nào được chỉ định, chỉ cần đăng nhập là đủ
            if (_roles == null || _roles.Length == 0)
            {
                return;
            }

            // Kiểm tra role của user có nằm trong danh sách cho phép không
            if (!_roles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
            {
                // Không có quyền -> redirect đến trang AccessDenied
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }
        }
    }
}
