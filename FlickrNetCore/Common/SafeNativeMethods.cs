namespace FlickrNetCore.Common
{
    /// <summary>
    /// Summary description for SafeNativeMethods.
    /// </summary>
    [System.Security.SuppressUnmanagedCodeSecurity]
    internal class SafeNativeMethods
    {
        private SafeNativeMethods()
        {
        }

        internal static int GetErrorCode(System.IO.IOException ioe)
        {
            var permission = new System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode);
            permission.Demand();

            return System.Runtime.InteropServices.Marshal.GetHRForException(ioe) & 0xFFFF;
        }
    }
}